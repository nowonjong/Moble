using MySqlConnector;
using StudentManager.Forms;
using workbench.Data;
using workbench.Models;

namespace StudentManager.Data;

/// <summary>
/// 학생 데이터의 CRUD를 담당합니다.
/// SQL 값은 문자열로 이어 붙이지 않고 항상 매개변수로 전달합니다.
/// </summary>
public sealed class StudentRepository
{
    private readonly string _connectionString;

    public StudentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task TestConnectionAsync()
    {
        await using MySqlConnection connection =
            MysqlDatabase.CreateConnection(_connectionString);
        await connection.OpenAsync();
    }

    public async Task<List<Student>> GetAllAsync(string keyword = "")
    {
        const string sql = @"
            SELECT id, student_number, name, department, phone, created_at
            FROM students
            WHERE @keyword = ''
               OR student_number LIKE CONCAT('%', @keyword, '%')
               OR name LIKE CONCAT('%', @keyword, '%')
               OR department LIKE CONCAT('%', @keyword, '%')
            ORDER BY id DESC;
            @";

        var students = new List<Student>();

        await using MySqlConnection connection =
            MysqlDatabase.CreateConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@keyword", keyword.Trim());

        await using MySqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(new Student
            {
                Id = reader.GetInt32("id"),
                StudentNumber = reader.GetString("student_number"),
                Name = reader.GetString("name"),
                Department = reader.GetString("department"),
                Phone = reader.GetString("phone"),
                CreatedAt = reader.GetDateTime("created_at")
            });
        }

        return students;
    }

    public async Task<int> InsertAsync(Student student)
    {
        const string sql = @"
            INSERT INTO students (student_number, name, department, phone)
            VALUES (@studentNumber, @name, @department, @phone);
            @";

        await using MySqlConnection connection =
            MysqlDatabase.CreateConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);
        AddStudentParameters(command, student);
        return await command.ExecuteNonQueryAsync();
    }

    public async Task<int> UpdateAsync(Student student)
    {
        const string sql = @"
            UPDATE students
            SET student_number = @studentNumber,
                name = @name,
                department = @department,
                phone = @phone
            WHERE id = @id;
            @";

        await using MySqlConnection connection =
            MysqlDatabase.CreateConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);
        AddStudentParameters(command, student);
        command.Parameters.AddWithValue("@id", student.Id);
        return await command.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM students WHERE id = @id;";

        await using MySqlConnection connection =
            MysqlDatabase.CreateConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        return await command.ExecuteNonQueryAsync();
    }

    private static void AddStudentParameters(MySqlCommand command, Student student)
    {
        command.Parameters.AddWithValue("@studentNumber", student.StudentNumber.Trim());
        command.Parameters.AddWithValue("@name", student.Name.Trim());
        command.Parameters.AddWithValue("@department", student.Department.Trim());
        command.Parameters.AddWithValue("@phone", student.Phone.Trim());
    }
}
