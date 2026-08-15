using MySqlConnector;
using StudentManager.Data;
using StudentManager.Forms;
using workbench.Models;

namespace StudentManager.Forms;

public sealed class MainForm : Form
{
    private readonly StudentRepository _repository;

    private readonly TextBox _txtStudentNumber = new();
    private readonly TextBox _txtName = new();
    private readonly TextBox _txtDepartment = new();
    private readonly TextBox _txtPhone = new();
    private readonly TextBox _txtSearch = new();
    private readonly DataGridView _grid = new();
    private readonly ToolStripStatusLabel _statusLabel = new("준비");

    private int _selectedId;

    public MainForm(StudentRepository repository)
    {
        _repository = repository;
        InitializeUi();
        Shown += async (_, _) => await InitializeDataAsync();
    }

    private void InitializeUi()
    {
        Text = "C# + MySQL 학생관리 프로그램";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(920, 620);
        Size = new Size(1080, 700);
        Font = new Font("맑은 고딕", 10F);

        var title = new Label
        {
            Text = "학생관리 프로그램",
            AutoSize = true,
            Font = new Font("맑은 고딕", 18F, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 15)
        };

        var inputLayout = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 4,
            RowCount = 2,
            Dock = DockStyle.Top,
            Padding = new Padding(0, 0, 0, 10)
        };
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        AddInput(inputLayout, "학번", _txtStudentNumber, 0, 0);
        AddInput(inputLayout, "이름", _txtName, 2, 0);
        AddInput(inputLayout, "학과", _txtDepartment, 0, 1);
        AddInput(inputLayout, "연락처", _txtPhone, 2, 1);

        var buttonPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            Padding = new Padding(0, 0, 0, 12)
        };
        buttonPanel.Controls.Add(CreateButton("등록", async (_, _) => await InsertAsync()));
        buttonPanel.Controls.Add(CreateButton("수정", async (_, _) => await UpdateAsync()));
        buttonPanel.Controls.Add(CreateButton("삭제", async (_, _) => await DeleteAsync()));
        buttonPanel.Controls.Add(CreateButton("입력 초기화", (_, _) => ClearInputs()));

        var searchPanel = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 3,
            Dock = DockStyle.Top,
            Padding = new Padding(0, 0, 0, 8)
        };
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        searchPanel.Controls.Add(new Label
        {
            Text = "검색",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Padding = new Padding(0, 5, 8, 0)
        }, 0, 0);
        _txtSearch.Dock = DockStyle.Fill;
        _txtSearch.PlaceholderText = "학번, 이름 또는 학과";
        _txtSearch.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await LoadStudentsAsync();
            }
        };
        searchPanel.Controls.Add(_txtSearch, 1, 0);
        searchPanel.Controls.Add(CreateButton("검색", async (_, _) => await LoadStudentsAsync()), 2, 0);

        ConfigureGrid();

        var statusStrip = new StatusStrip();
        statusStrip.Items.Add(_statusLabel);

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            RowCount = 5,
            ColumnCount = 1
        };
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        content.Controls.Add(title, 0, 0);
        content.Controls.Add(inputLayout, 0, 1);
        content.Controls.Add(buttonPanel, 0, 2);
        content.Controls.Add(searchPanel, 0, 3);
        content.Controls.Add(_grid, 0, 4);

        Controls.Add(content);
        Controls.Add(statusStrip);
    }

    private void ConfigureGrid()
    {
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AutoGenerateColumns = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.RowHeadersVisible = false;

        _grid.Columns.Add(CreateColumn(nameof(Student.Id), "번호", 45));
        _grid.Columns.Add(CreateColumn(nameof(Student.StudentNumber), "학번", 90));
        _grid.Columns.Add(CreateColumn(nameof(Student.Name), "이름", 80));
        _grid.Columns.Add(CreateColumn(nameof(Student.Department), "학과", 110));
        _grid.Columns.Add(CreateColumn(nameof(Student.Phone), "연락처", 110));
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Student.CreatedAt),
            HeaderText = "등록일",
            FillWeight = 100,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" }
        });

        _grid.CellClick += (_, e) =>
        {
            if (e.RowIndex < 0 || _grid.Rows[e.RowIndex].DataBoundItem is not Student student)
                return;

            _selectedId = student.Id;
            _txtStudentNumber.Text = student.StudentNumber;
            _txtName.Text = student.Name;
            _txtDepartment.Text = student.Department;
            _txtPhone.Text = student.Phone;
            SetStatus($"{student.Name} 학생을 선택했습니다.");
        };
    }

    private async Task InitializeDataAsync()
    {
        try
        {
            SetBusy(true, "MySQL 연결 확인 중...");
            await _repository.TestConnectionAsync();
            await LoadStudentsAsync();
        }
        catch (Exception ex)
        {
            ShowDatabaseError(ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task LoadStudentsAsync()
    {
        try
        {
            SetBusy(true, "학생 목록을 조회하는 중...");
            List<Student> students = await _repository.GetAllAsync(_txtSearch.Text);
            _grid.DataSource = students;
            SetStatus($"{students.Count}명의 학생을 조회했습니다.");
        }
        catch (Exception ex)
        {
            ShowDatabaseError(ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task InsertAsync()
    {
        Student? student = ReadAndValidateInput();
        if (student is null)
            return;

        try
        {
            SetBusy(true, "학생을 등록하는 중...");
            await _repository.InsertAsync(student);
            ClearInputs();
            await LoadStudentsAsync();
            SetStatus("학생을 등록했습니다.");
        }
        catch (Exception ex)
        {
            ShowDatabaseError(ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task UpdateAsync()
    {
        if (_selectedId == 0)
        {
            MessageBox.Show("수정할 학생을 목록에서 선택하세요.", "알림");
            return;
        }

        Student? student = ReadAndValidateInput();
        if (student is null)
            return;

        student.Id = _selectedId;

        try
        {
            SetBusy(true, "학생 정보를 수정하는 중...");
            await _repository.UpdateAsync(student);
            ClearInputs();
            await LoadStudentsAsync();
            SetStatus("학생 정보를 수정했습니다.");
        }
        catch (Exception ex)
        {
            ShowDatabaseError(ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task DeleteAsync()
    {
        if (_selectedId == 0)
        {
            MessageBox.Show("삭제할 학생을 목록에서 선택하세요.", "알림");
            return;
        }

        DialogResult answer = MessageBox.Show(
            "선택한 학생을 삭제하시겠습니까?",
            "삭제 확인",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (answer != DialogResult.Yes)
            return;

        try
        {
            SetBusy(true, "학생을 삭제하는 중...");
            await _repository.DeleteAsync(_selectedId);
            ClearInputs();
            await LoadStudentsAsync();
            SetStatus("학생을 삭제했습니다.");
        }
        catch (Exception ex)
        {
            ShowDatabaseError(ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private Student? ReadAndValidateInput()
    {
        if (string.IsNullOrWhiteSpace(_txtStudentNumber.Text) ||
            string.IsNullOrWhiteSpace(_txtName.Text) ||
            string.IsNullOrWhiteSpace(_txtDepartment.Text))
        {
            MessageBox.Show("학번, 이름, 학과는 반드시 입력하세요.", "입력 확인");
            return null;
        }

        return new Student
        {
            StudentNumber = _txtStudentNumber.Text,
            Name = _txtName.Text,
            Department = _txtDepartment.Text,
            Phone = _txtPhone.Text
        };
    }

    private void ClearInputs()
    {
        _selectedId = 0;
        _txtStudentNumber.Clear();
        _txtName.Clear();
        _txtDepartment.Clear();
        _txtPhone.Clear();
        _grid.ClearSelection();
        _txtStudentNumber.Focus();
        SetStatus("입력란을 초기화했습니다.");
    }

    private void SetBusy(bool busy, string? message = null)
    {
        UseWaitCursor = busy;
        if (message is not null)
            SetStatus(message);
    }

    private void SetStatus(string message) => _statusLabel.Text = message;

    private static void ShowDatabaseError(Exception ex)
    {
        string detail = ex is MySqlException
            ? "MySQL 서버, 데이터베이스, 사용자 계정 및 연결 문자열을 확인하세요."
            : "설정과 입력값을 확인하세요.";

        MessageBox.Show(
            $"{detail}\n\n{ex.Message}",
            "처리 오류",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private static void AddInput(
        TableLayoutPanel panel,
        string labelText,
        TextBox textBox,
        int column,
        int row)
    {
        panel.Controls.Add(new Label
        {
            Text = labelText,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Padding = new Padding(0, 5, 8, 0)
        }, column, row);

        textBox.Dock = DockStyle.Fill;
        textBox.Margin = new Padding(3, 3, 18, 6);
        panel.Controls.Add(textBox, column + 1, row);
    }

    private static Button CreateButton(string text, EventHandler click)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = true,
            Padding = new Padding(10, 3, 10, 3),
            Margin = new Padding(0, 0, 8, 0)
        };
        button.Click += click;
        return button;
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        // 
        // MainForm
        // 
        ClientSize = new Size(284, 261);
        Name = "MainForm";
        ResumeLayout(false);

    }

    private static DataGridViewTextBoxColumn CreateColumn(
        string property,
        string header,
        float weight) =>
        new()
        {
            DataPropertyName = property,
            HeaderText = header,
            FillWeight = weight
        };
}
