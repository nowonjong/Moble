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
        private DataTable paymentTable;
        private Chart chartSales;

        public UcSalesReport()
        {
            InitializeComponent();
            InitSalesData();
        }

        private void InitSalesData()
        {
            dgvSalesReport.EnableHeadersVisualStyles = false;
            dgvSalesReport.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvSalesReport.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;

            dtpStartDate.Value = DateTime.Now.AddMonths(-1);
            dtpEndDate.Value = DateTime.Now;

            cmbPeriodUnit.Items.Clear();
            cmbPeriodUnit.Items.AddRange(new string[] { "요일별 (월~일)", "주차별 (1~5주)", "월별 (1~12월)" });
            cmbPeriodUnit.SelectedIndex = 0;

            salesTable = new DataTable();
            salesTable.Columns.Add("결제일시", typeof(DateTime));
            salesTable.Columns.Add("영수증번호", typeof(string));
            salesTable.Columns.Add("메뉴명", typeof(string));
            salesTable.Columns.Add("수량", typeof(int));
            salesTable.Columns.Add("단가", typeof(int));
            salesTable.Columns.Add("할인수량", typeof(int));
            salesTable.Columns.Add("메뉴결제금액", typeof(int));

            paymentTable = new DataTable();
            paymentTable.Columns.Add("결제일시", typeof(DateTime));
            paymentTable.Columns.Add("영수증번호", typeof(string));
            paymentTable.Columns.Add("원주문금액", typeof(int));
            paymentTable.Columns.Add("사용포인트", typeof(int));
            paymentTable.Columns.Add("실제결제금액", typeof(int));
            paymentTable.Columns.Add("적립포인트", typeof(int));
            paymentTable.Columns.Add("회원번호", typeof(int));

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

        private void btnSearch_Click(object sender, EventArgs e) => UpdateReportAndChart();
        private void cmbPeriodUnit_SelectedIndexChanged(object sender, EventArgs e) => UpdateReportAndChart();
        private void btnExportCsv_Click(object sender, EventArgs e) => ExportToCsv();
        private void btnExportPdf_Click(object sender, EventArgs e) => ExportToPdf();

        private void UpdateReportAndChart()
        {
            if (chartSales == null || chartSales.Series == null || chartSales.Series.FindByName("매출액") == null)
                return;

            DateTime start = dtpStartDate.Value.Date;
            DateTime end = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1);

            if (start > end)
            {
                MessageBox.Show("시작일이 종료일보다 뒤일 수 없습니다.", "안내");
                return;
            }

            LoadSalesDataFromCsv();

            DataRow[] filteredItemRows = salesTable.Select($"결제일시 >= #{start:yyyy-MM-dd HH:mm:ss}# AND 결제일시 <= #{end:yyyy-MM-dd HH:mm:ss}#");
            DataRow[] filteredPaymentRows = paymentTable.Select($"결제일시 >= #{start:yyyy-MM-dd HH:mm:ss}# AND 결제일시 <= #{end:yyyy-MM-dd HH:mm:ss}#");

            DataTable summaryTable = new DataTable();
            summaryTable.Columns.Add("메뉴명", typeof(string));
            summaryTable.Columns.Add("수량", typeof(int));
            summaryTable.Columns.Add("결제금액", typeof(int));

            if (filteredItemRows.Length > 0)
            {
                var groupedData = filteredItemRows
                    .GroupBy(r => r["메뉴명"].ToString())
                    .Select(g => new
                    {
                        MenuName = g.Key,
                        TotalQty = g.Sum(r => Convert.ToInt32(r["수량"])),
                        TotalAmount = g.Sum(r => Convert.ToInt32(r["메뉴결제금액"]))
                    });

                foreach (var item in groupedData)
                    summaryTable.Rows.Add(item.MenuName, item.TotalQty, item.TotalAmount);

                dgvSalesReport.DataSource = summaryTable;
                dgvSalesReport.Columns["수량"].DefaultCellStyle.Format = "N0";
                dgvSalesReport.Columns["결제금액"].DefaultCellStyle.Format = "N0";
            }
            else
            {
                dgvSalesReport.DataSource = null;
            }

            chartSales.Series["매출액"].Points.Clear();
            int unitType = cmbPeriodUnit.SelectedIndex;

            if (unitType == 0)
            {
                string[] dayNames = { "월요일", "화요일", "수요일", "목요일", "금요일", "토요일", "일요일" };
                DayOfWeek[] days = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };

                for (int i = 0; i < days.Length; i++)
                {
                    int daySum = filteredPaymentRows
                        .Where(r => Convert.ToDateTime(r["결제일시"]).DayOfWeek == days[i])
                        .Sum(r => Convert.ToInt32(r["실제결제금액"]));

                    AddChartPoint(i + 1, daySum, dayNames[i]);
                }
            }
            else if (unitType == 1)
            {
                for (int week = 1; week <= 5; week++)
                {
                    int targetWeek = week;

                    int weekSum = filteredPaymentRows
                        .Where(r => (Convert.ToDateTime(r["결제일시"]).Day - 1) / 7 + 1 == targetWeek)
                        .Sum(r => Convert.ToInt32(r["실제결제금액"]));

                    AddChartPoint(week, weekSum, $"{week}주차");
                }
            }
            else
            {
                for (int month = 1; month <= 12; month++)
                {
                    int targetMonth = month;

                    int monthSum = filteredPaymentRows
                        .Where(r => Convert.ToDateTime(r["결제일시"]).Month == targetMonth)
                        .Sum(r => Convert.ToInt32(r["실제결제금액"]));

                    AddChartPoint(month, monthSum, $"{month}월");
                }
            }

            int totalSales = filteredPaymentRows.Sum(r => Convert.ToInt32(r["실제결제금액"]));
            int itemDiscount = filteredItemRows.Sum(r => Convert.ToInt32(r["할인수량"]) * Convert.ToInt32(r["단가"]));
            int pointDiscount = filteredPaymentRows.Sum(r => Convert.ToInt32(r["사용포인트"]));
            int totalDiscount = itemDiscount + pointDiscount;
            int totalOrders = filteredPaymentRows.Length;

            lblTotalSales.Text = $"{totalSales:N0} 원";
            lblTotalOrders.Text = $"{totalOrders:N0} 건";
            lblTotalDiscount.Text = $"{totalDiscount:N0} 원";
        }

        private void AddChartPoint(int xPos, int yValue, string labelName)
        {
            int index = chartSales.Series["매출액"].Points.AddXY(xPos, yValue);
            chartSales.Series["매출액"].Points[index].AxisLabel = labelName;
        }

        private void LoadSalesDataFromCsv()
        {
            salesTable.Clear();
            paymentTable.Clear();

            string historyPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (!File.Exists(historyPath))
                return;

            var historyDict = new Dictionary<string, DateTime>();

            foreach (string line in File.ReadAllLines(historyPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                // ReceiptNo,PaymentDate,Source,OrderType,OriginalAmount,UsedPoint,TotalAmount,EarnedPoint,MemberId,PaymentMethod
                if (parts.Length < 10)
                    continue;

                string receiptNo = parts[0].Trim();

                if (!DateTime.TryParse(parts[1].Trim(), out DateTime payDate))
                    continue;

                int originalAmount = int.TryParse(parts[4].Trim(), out int original) ? original : 0;
                int usedPoint = int.TryParse(parts[5].Trim(), out int used) ? used : 0;
                int totalAmount = int.TryParse(parts[6].Trim(), out int total) ? total : 0;
                int earnedPoint = int.TryParse(parts[7].Trim(), out int earned) ? earned : 0;
                int memberId = int.TryParse(parts[8].Trim(), out int member) ? member : 0;

                historyDict[receiptNo] = payDate;
                paymentTable.Rows.Add(payDate, receiptNo, originalAmount, usedPoint, totalAmount, earnedPoint, memberId);
            }

            if (!File.Exists(itemsPath))
                return;

            foreach (string line in File.ReadAllLines(itemsPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 6)
                    continue;

                string receiptNo = parts[0].Trim();

                if (!historyDict.ContainsKey(receiptNo))
                    continue;

                DateTime payDate = historyDict[receiptNo];
                string menuName = parts[1].Trim();
                int price = int.TryParse(parts[2].Trim(), out int p) ? p : 0;
                int qty = int.TryParse(parts[3].Trim(), out int q) ? q : 1;
                int discountQty = int.TryParse(parts[4].Trim(), out int dq) ? dq : 0;
                int subTotal = int.TryParse(parts[5].Trim(), out int st) ? st : 0;

                salesTable.Rows.Add(payDate, receiptNo, menuName, qty, price, discountQty, subTotal);
            }
        }

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
                            pdfTable.AddCell(new PdfPCell(new Phrase(col.HeaderText, cellFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                        foreach (DataGridViewRow row in dgvSalesReport.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                    pdfTable.AddCell(new PdfPCell(new Phrase(cell.Value?.ToString() ?? "", cellFont)));
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