using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SushiKioskAdmin.Views
{
    public partial class UcSalesReport : UserControl
    {
        private DataTable salesTable;
        private Chart chartSales;

        public UcSalesReport()
        {
            InitializeComponent();
            InitSalesData();
        }

        // ==========================================
        // 1. 초기화 메서드
        // ==========================================

        private void InitSalesData()
        {
            // 그리드뷰 헤더 스타일 설정
            dgvSalesReport.EnableHeadersVisualStyles = false;
            dgvSalesReport.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvSalesReport.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;

            // 조회 기간 기본값 설정 (최근 1개월)
            dtpStartDate.Value = DateTime.Now.AddMonths(-1);
            dtpEndDate.Value = DateTime.Now;

            // 단위 선택 콤보박스 설정
            cmbPeriodUnit.Items.Clear();
            cmbPeriodUnit.Items.AddRange(new string[] { "요일별 (월~일)", "주차별 (1~5주)", "월별 (1~12월)" });
            cmbPeriodUnit.SelectedIndex = 0;

            // salesTable 구조 생성 (결제일시, 영수증번호, 메뉴명, 수량, 단가, 할인수량, 결제금액)
            salesTable = new DataTable();
            salesTable.Columns.Add("결제일시", typeof(DateTime));
            salesTable.Columns.Add("영수증번호", typeof(string));
            salesTable.Columns.Add("메뉴명", typeof(string));
            salesTable.Columns.Add("수량", typeof(int));
            salesTable.Columns.Add("단가", typeof(int));
            salesTable.Columns.Add("할인수량", typeof(int));
            salesTable.Columns.Add("결제금액", typeof(int)); // SubTotal (실제 결제액)

            // 실제 CSV 파일에서 매출 데이터 로드
            LoadSalesDataFromCsv();

            dgvSalesReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSalesReport.ColumnHeadersHeight = 30;
            dgvSalesReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            InitChartStyle();
            UpdateReportAndChart();
        }

        private void InitChartStyle()
        {
            chartSales = new Chart { Dock = DockStyle.Fill };

            ChartArea chartArea = new ChartArea("SalesArea");
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.LabelStyle.Format = "{0:N0}원";
            chartArea.AxisX.Interval = 1;

            chartSales.ChartAreas.Add(chartArea);

            Series series = new Series("매출액")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.DodgerBlue,
                IsValueShownAsLabel = true,
                LabelFormat = "{0:N0}"
            };
            chartSales.Series.Add(series);

            pnlChartArea.Controls.Clear();
            pnlChartArea.Controls.Add(chartSales);
        }

        // ==========================================
        // 2. 폼 이벤트 handler
        // ==========================================

        private void btnSearch_Click(object sender, EventArgs e) => UpdateReportAndChart();

        private void cmbPeriodUnit_SelectedIndexChanged(object sender, EventArgs e) => UpdateReportAndChart();

        private void btnExportCsv_Click(object sender, EventArgs e) => ExportToCsv();

        private void btnExportPdf_Click(object sender, EventArgs e) => ExportToPdf();

        // ==========================================
        // 3. 핵심 비즈니스 로직
        // ==========================================

        private void UpdateReportAndChart()
        {
            if (chartSales == null || chartSales.Series == null || chartSales.Series.FindByName("매출액") == null)
            {
                return;
            }

            DateTime start = dtpStartDate.Value.Date;
            DateTime end = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1);

            if (start > end)
            {
                MessageBox.Show("시작일이 종료일보다 뒤일 수 없습니다.", "안내");
                return;
            }

            // 최신 데이터 반영을 위해 매번 조회 시 파일 다시 로드
            LoadSalesDataFromCsv();

            // 1. 날짜 범위 데이터 필터링
            DataRow[] filteredRows = salesTable.Select($"결제일시 >= '{start}' AND 결제일시 <= '{end}'");

            // 2. 메뉴명 기준 그룹화 테이블 생성 (할인 없는 순수 정가 기준 금액 집계 포함)
            DataTable summaryTable = new DataTable();
            summaryTable.Columns.Add("메뉴명", typeof(string));
            summaryTable.Columns.Add("총판매수량", typeof(int));
            summaryTable.Columns.Add("총결제금액(정가기준)", typeof(int));

            if (filteredRows.Length > 0)
            {
                var groupedData = filteredRows
                    .GroupBy(r => r["메뉴명"].ToString())
                    .Select(g => new
                    {
                        MenuName = g.Key,
                        TotalQty = g.Sum(r => Convert.ToInt32(r["수량"])),
                        // 할인 없는 순수 정가 기준 금액: (수량 * 단가)의 합계
                        TotalOriginalPrice = g.Sum(r => Convert.ToInt32(r["수량"]) * Convert.ToInt32(r["단가"]))
                    });

                foreach (var item in groupedData)
                {
                    summaryTable.Rows.Add(item.MenuName, item.TotalQty, item.TotalOriginalPrice);
                }

                dgvSalesReport.DataSource = summaryTable;
                dgvSalesReport.Columns["총판매수량"].DefaultCellStyle.Format = "N0";
                dgvSalesReport.Columns["총결제금액(정가기준)"].DefaultCellStyle.Format = "N0";
            }
            else
            {
                dgvSalesReport.DataSource = null;
            }

            // 3. 차트 데이터 집계 및 생성 (실제 결제 금액 기준)
            chartSales.Series["매출액"].Points.Clear();
            int unitType = cmbPeriodUnit.SelectedIndex;

            if (unitType == 0) // 요일별
            {
                string[] dayNames = { "월요일", "화요일", "수요일", "목요일", "금요일", "토요일", "일요일" };
                DayOfWeek[] days = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };

                for (int i = 0; i < days.Length; i++)
                {
                    int daySum = filteredRows
                        .Where(r => Convert.ToDateTime(r["결제일시"]).DayOfWeek == days[i])
                        .Sum(r => Convert.ToInt32(r["결제금액"]));

                    AddChartPoint(i + 1, daySum, dayNames[i]);
                }
            }
            else if (unitType == 1) // 주차별
            {
                for (int week = 1; week <= 5; week++)
                {
                    int targetWeek = week;
                    int weekSum = filteredRows
                        .Where(r => (Convert.ToDateTime(r["결제일시"]).Day - 1) / 7 + 1 == targetWeek)
                        .Sum(r => Convert.ToInt32(r["결제금액"]));

                    AddChartPoint(week, weekSum, $"{week}주차");
                }
            }
            else // 월별
            {
                for (int month = 1; month <= 12; month++)
                {
                    int targetMonth = month;
                    int monthSum = filteredRows
                        .Where(r => Convert.ToDateTime(r["결제일시"]).Month == targetMonth)
                        .Sum(r => Convert.ToInt32(r["결제금액"]));

                    AddChartPoint(month, monthSum, $"{month}월");
                }
            }

            // 4. 하단 요약 지표 업데이트 (총 매출액, 총 건수, 할인된 금액)
            int totalSales = filteredRows.Sum(r => Convert.ToInt32(r["결제금액"]));

            // 총 할인 금액 계산: (할인수량 * 단가)의 합계
            int totalDiscount = filteredRows.Sum(r => Convert.ToInt32(r["할인수량"]) * Convert.ToInt32(r["단가"]));

            // 고유 영수증 건수 계산
            int totalOrders = filteredRows.Select(r => r["영수증번호"].ToString()).Distinct().Count();

            lblTotalSales.Text = $"{totalSales:N0} 원";
            lblTotalOrders.Text = $"{totalOrders:N0} 건";
            lblTotalDiscount.Text = $"{totalDiscount:N0} 원";
        }

        private void AddChartPoint(int xPos, int yValue, string labelName)
        {
            int index = chartSales.Series["매출액"].Points.AddXY(xPos, yValue);
            chartSales.Series["매출액"].Points[index].AxisLabel = labelName;
        }

        // ==========================================
        // 4. 실제 CSV 파일 연동 로직
        // ==========================================

        private void LoadSalesDataFromCsv()
        {
            salesTable.Clear();

            string historyPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (!File.Exists(historyPath) || !File.Exists(itemsPath)) return;

            // 1. sales_history에서 영수증별 결제일시 맵 생성
            var historyDict = new Dictionary<string, DateTime>();
            foreach (string line in File.ReadAllLines(historyPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split(',');
                if (parts.Length >= 2)
                {
                    string receiptNo = parts[0].Trim();
                    if (DateTime.TryParse(parts[1].Trim(), out DateTime payDate))
                    {
                        historyDict[receiptNo] = payDate;
                    }
                }
            }

            // 2. order_items에서 해당 영수증들의 상세 품목 결합
            foreach (string line in File.ReadAllLines(itemsPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split(',');
                if (parts.Length >= 6)
                {
                    string receiptNo = parts[0].Trim();
                    if (historyDict.ContainsKey(receiptNo))
                    {
                        DateTime payDate = historyDict[receiptNo];
                        string menuName = parts[1].Trim();
                        int price = int.TryParse(parts[2].Trim(), out int p) ? p : 0;
                        int qty = int.TryParse(parts[3].Trim(), out int q) ? q : 1;
                        int discountQty = int.TryParse(parts[4].Trim(), out int dq) ? dq : 0;
                        int subTotal = int.TryParse(parts[5].Trim(), out int st) ? st : 0;

                        salesTable.Rows.Add(payDate, receiptNo, menuName, qty, price, discountQty, subTotal);
                    }
                }
            }
        }

        // ==========================================
        // 5. 파일 내보내기 (CSV, PDF)
        // ==========================================

        private void ExportToCsv()
        {
            if (dgvSalesReport.Rows.Count == 0)
            {
                MessageBox.Show("내보낼 데이터가 없습니다.", "안내");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV 파일 (*.csv)|*.csv", FileName = $"매출리포트_{DateTime.Now:yyyyMMdd}.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();

                    var headers = dgvSalesReport.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText);
                    sb.AppendLine(string.Join(",", headers));

                    foreach (DataGridViewRow row in dgvSalesReport.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            var cells = row.Cells.Cast<DataGridViewCell>().Select(c => $"\"{c.Value}\"");
                            sb.AppendLine(string.Join(",", cells));
                        }
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("CSV 파일로 성공적으로 저장되었습니다.", "완료");
                }
            }
        }

        private void ExportToPdf()
        {
            if (dgvSalesReport.Rows.Count == 0)
            {
                MessageBox.Show("내보낼 데이터가 없습니다.", "안내");
                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF 파일 (*.pdf)|*.pdf", FileName = $"매출리포트_{DateTime.Now:yyyyMMdd}.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "malgun.ttf");
                        BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        iTextSharp.text.Font titleFont = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD);
                        iTextSharp.text.Font cellFont = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                        pdfDoc.Add(new Paragraph($"매출 리포트 ({dtpStartDate.Value:yyyy-MM-dd} ~ {dtpEndDate.Value:yyyy-MM-dd})", titleFont));
                        pdfDoc.Add(new Paragraph($"총 매출액: {lblTotalSales.Text} | 총 할인금액: {lblTotalDiscount.Text} | 총 주문건수: {lblTotalOrders.Text}\n\n", cellFont));

                        PdfPTable pdfTable = new PdfPTable(dgvSalesReport.Columns.Count) { WidthPercentage = 100 };
                        foreach (DataGridViewColumn col in dgvSalesReport.Columns)
                        {
                            pdfTable.AddCell(new PdfPCell(new Phrase(col.HeaderText, cellFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        }

                        foreach (DataGridViewRow row in dgvSalesReport.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    pdfTable.AddCell(new PdfPCell(new Phrase(cell.Value?.ToString() ?? "", cellFont)));
                                }
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                    }

                    MessageBox.Show("PDF 파일로 성공적으로 저장되었습니다.", "완료");
                }
            }
        }
    }
}