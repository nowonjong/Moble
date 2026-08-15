using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MySqlConnector;

namespace workbench.Data
{
    public static class MysqlDatabase
    {
        public static string LoadConnectionString()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            if(!File.Exists(path))
            {
                throw new FileNotFoundException("appsettings.json 파일을 찾을 수 없습니다.", path);
            }
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));

            if(!document.RootElement.TryGetProperty("ConnectionStrings", out JsonElement secting) ||
                !secting.TryGetProperty("StudentDb", out JsonElement value) ||
                string.IsNullOrWhiteSpace(value.GetString()))
            {
                throw new InvalidOperationException("appsettings.json에 ConnectionStrings:StudentDb 설정이 업습니다.");
            }
            return value.GetString()!;
        }
        public static MySqlConnection CreateConnection(string connectionString) =>
            new(connectionString);
    }
}
