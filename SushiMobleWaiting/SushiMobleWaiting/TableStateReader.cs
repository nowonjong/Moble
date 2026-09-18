using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SushiMobleWaiting
{
    public class TableStateData
    {
        public int TotalTables { get; set; }

        public long ReleaseSequence { get; set; }

        public Dictionary<string, string> Tables { get; set; }
            = new Dictionary<string, string>();
    }


    public static class TableStateReader
    {
        private static readonly string filePath =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "SushiMoble",
                "table_state.json"
            );


        // 전체 테이블 상태 읽기
        public static TableStateData Load()
        {
            try
            {
                if (!File.Exists(filePath))
                    return CreateDefaultData();

                string json = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(json))
                    return CreateDefaultData();

                TableStateData data =
                    JsonConvert.DeserializeObject<TableStateData>(json);

                return data ?? new TableStateData
                {
                    TotalTables = 10,
                    ReleaseSequence = 0
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[TableStateReader 오류] " + ex.Message
                );

                return CreateDefaultData();
            }
        }

        private static TableStateData CreateDefaultData()
        {
            TableStateData data = new TableStateData
            {
                TotalTables = 10,
                ReleaseSequence = 0
            };

            for (int i = 1; i <= 10; i++)
            {
                string tableCode = $"T{i:00}";
                data.Tables[tableCode] = "AVAILABLE";
            }

            return data;
        }


        // 현재 사용 중 테이블 개수
        public static int GetOccupiedCount()
        {
            TableStateData data = Load();

            return data.Tables.Count(
                x => x.Value == "OCCUPIED"
            );
        }


        // 현재 빈 테이블 개수
        public static int GetAvailableCount()
        {
            TableStateData data = Load();

            return data.Tables.Count(
                x => x.Value == "AVAILABLE"
            );
        }


        // 현재 만석인지 확인
        public static bool IsFull()
        {
            return GetAvailableCount() == 0;
        }


        // 현재 ReleaseSequence
        public static long GetReleaseSequence()
        {
            return Load().ReleaseSequence;
        }


        // 디버깅용
        public static string GetFilePath()
        {
            return filePath;
        }
    }
}