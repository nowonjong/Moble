namespace Kiosk;

public sealed class PointUseDialog : Form
{
    private readonly NumericUpDown pointInput = new();

    public int UsedPoint => Decimal.ToInt32(pointInput.Value);

    public PointUseDialog(int availablePoint, int orderAmount)
    {
        int maximum = Math.Min(availablePoint, orderAmount);
        Text = "포인트 사용";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(360, 160);

        Label guide = new()
        {
            AutoSize = true,
            Location = new Point(25, 20),
            Text = $"사용할 포인트를 입력하세요. (보유 {availablePoint:N0}P)"
        };

        pointInput.Location = new Point(25, 55);
        pointInput.Size = new Size(305, 30);
        pointInput.Minimum = 0;
        pointInput.Maximum = maximum;
        // 뉴머릭업다운 단위 10으로 조정
        pointInput.Increment = 10;
        pointInput.ThousandsSeparator = true;

        Button ok = new()
        {
            Text = "확인",
            DialogResult = DialogResult.OK,
            Location = new Point(170, 105),
            Size = new Size(75, 32)
        };
        Button cancel = new()
        {
            Text = "취소",
            DialogResult = DialogResult.Cancel,
            Location = new Point(255, 105),
            Size = new Size(75, 32)
        };

        Controls.AddRange(new Control[] { guide, pointInput, ok, cancel });
        AcceptButton = ok;
        CancelButton = cancel;
    }
}
