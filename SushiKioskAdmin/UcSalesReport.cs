using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
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

            // 테이블 구조 생성
            salesTable = new DataTable();
            salesTable.Columns.Add("결제일시", typeof(DateTime));
            salesTable.Columns.Add("메뉴명", typeof(string));
            salesTable.Columns.Add("수량", typeof(int));
            salesTable.Columns.Add("결제금액", typeof(int));

            // susi_menu.csv 데이터를 기반으로 매출 데이터 생성
            GenerateDummyDataFromCsv();

            dgvSalesReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 열 헤더 높이를 원하는 크기(예: 40픽셀)로 지정
            dgvSalesReport.ColumnHeadersHeight = 30;

            // 높이를 자동으로 늘어나지 않게 고정
            dgvSalesReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // ★ 수정 포인트: UpdateReportAndChart()보다 차트를 초기화하는 InitChartStyle()을 반드시 먼저 호출해야 합니다!
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
        // 2. 폼 이벤트 handler (디자이너 연결용)
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
            // ★ 안전한 Series 이름 검사 방식 (FindByName 사용)
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

            // 1. 날짜 범위 데이터 필터링
            DataRow[] filteredRows = salesTable.Select($"결제일시 >= '{start}' AND 결제일시 <= '{end}'");

            // 2. 메뉴명 기준 그룹화 테이블 생성 (결제일시 제거, 총 수량 및 총 결제금액 집계)
            DataTable summaryTable = new DataTable();
            summaryTable.Columns.Add("메뉴명", typeof(string));
            summaryTable.Columns.Add("수량", typeof(int));
            summaryTable.Columns.Add("결제금액", typeof(int));

            if (filteredRows.Length > 0)
            {
                var groupedData = filteredRows
                    .GroupBy(r => r["메뉴명"].ToString())
                    .Select(g => new
                    {
                        MenuName = g.Key,
                        TotalQty = g.Sum(r => Convert.ToInt32(r["수량"])),
                        TotalPrice = g.Sum(r => Convert.ToInt32(r["결제금액"]))
                    });

                foreach (var item in groupedData)
                {
                    summaryTable.Rows.Add(item.MenuName, item.TotalQty, item.TotalPrice);
                }

                dgvSalesReport.DataSource = summaryTable;
                dgvSalesReport.Columns["수량"].DefaultCellStyle.Format = "N0";
                dgvSalesReport.Columns["결제금액"].DefaultCellStyle.Format = "N0";
            }
            else
            {
                dgvSalesReport.DataSource = null;
            }

            // 3. 차트 데이터 집계 및 생성
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

            // 4. 하단 요약 문구 업데이트
            int totalSum = filteredRows.Sum(r => Convert.ToInt32(r["결제금액"]));
            lblTotalSales.Text = $"{totalSum:N0} 원";
            lblTotalOrders.Text = $"{filteredRows.Length:N0} 건";
        }

        private void AddChartPoint(int xPos, int yValue, string labelName)
        {
            int index = chartSales.Series["매출액"].Points.AddXY(xPos, yValue);
            chartSales.Series["매출액"].Points[index].AxisLabel = labelName;
        }

        // ==========================================
        // 4. CSV 연동 및 데이터 생성 로직 (수정된 부분)
        // ==========================================

        private void GenerateDummyDataFromCsv()
        {
            // 메뉴 정보를 저장할 리스트 (메뉴명, 가격)
            List<(string MenuName, int Price)> menuList = new List<(string, int)>();

            string csvPath = Path.Combine(Application.StartupPath, "susi_menu.csv");

            // CSV 파일이 존재하면 읽어오기
            if (File.Exists(csvPath))
            {
                string[] lines = File.ReadAllLines(csvPath, Encoding.UTF8);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // CSV 컬럼: 0:ID, 1:한글메뉴명, 2:일어, 3:영어, 4:가격
                    string[] parts = line.Split(',');
                    if (parts.Length >= 5)
                    {
                        string menuName = parts[1].Trim();
                        if (int.TryParse(parts[4].Trim(), out int price))
                        {
                            menuList.Add((menuName, price));
                        }
                    }
                }
            }

            // CSV를 읽지 못했거나 비어있을 경우 예비용 기본 메뉴 설정
            if (menuList.Count == 0)
            {
                menuList.Add(("광어초밥", 3000));
                menuList.Add(("초새우초밥", 1500));
                menuList.Add(("유부초밥", 1000));
            }

            // 읽어온 메뉴들로 300개의 샘플 매출 데이터 파싱
            Random rand = new Random();
            for (int i = 0; i < 300; i++)
            {
                var selectedMenu = menuList[rand.Next(menuList.Count)];
                int qty = rand.Next(1, 4); // 수량 1~3개
                DateTime randDate = DateTime.Now.AddDays(-rand.Next(0, 60)).AddHours(-rand.Next(0, 12));

                salesTable.Rows.Add(randDate, selectedMenu.MenuName, qty, selectedMenu.Price * qty);
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
                        pdfDoc.Add(new Paragraph($"총 매출액: {lblTotalSales.Text} / 총 주문건수: {lblTotalOrders.Text}\n\n", cellFont));

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