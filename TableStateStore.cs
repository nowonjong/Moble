using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Kiosk
{
    // JSON 파일 전체 구조
    public class TableStateData
    {
        // 매장의 총 테이블 수
        public int TotalTables { get; set; } = 10;

        // 테이블이 실제로 비워진 횟수
        // Payment에서 OCCUPIED → AVAILABLE이 될 때마다 1 증가
        public long ReleaseSequence { get; set; } = 0;

        // 각 테이블 상태
        public Dictionary<string, string> Tables { get; set; }
            = new Dictionary<string, string>();
    }


    public static class TableStateStore
    {
        private const int TOTAL_TABLES = 10;

        private const string OCCUPIED = "OCCUPIED";
        private const string AVAILABLE = "AVAILABLE";


        // 두 프로그램이 같은 Windows 계정에서 접근할 공용 위치
        private static readonly string folderPath =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "SushiMoble"
            );

        private static readonly string filePath =
            Path.Combine(folderPath, "table_state.json");


        // -------------------------------------------------
        // 테이블 사용 시작
        // Here_In에서 테이블 선택이 확정되었을 때 사용할 예정
        // -------------------------------------------------
        public static void Occupy(string tableCode)
        {
            TableStateData data = Load();

            if (!data.Tables.ContainsKey(tableCode))
                return;

            data.Tables[tableCode] = OCCUPIED;

            Save(data);
        }


        // -------------------------------------------------
        // 테이블 사용 종료
        // Payment에서 결제가 정상 완료되었을 때 사용할 예정
        // -------------------------------------------------
        public static bool Release(string tableCode)
        {
            TableStateData data = Load();

            if (!data.Tables.ContainsKey(tableCode))
                return false;

            // 이미 빈자리라면 다시 처리하지 않음
            if (data.Tables[tableCode] == AVAILABLE)
                return false;

            // 사용 중 → 빈자리
            data.Tables[tableCode] = AVAILABLE;

            // 실제 퇴장 발생 횟수 증가
            data.ReleaseSequence++;

            Save(data);

            return true;
        }


        // -------------------------------------------------
        // 현재 테이블 상태 읽기
        // -------------------------------------------------
        public static TableStateData Load()
        {
            try
            {
                EnsureFolderExists();

                // 최초 실행이라 파일이 없으면 새 상태 생성
                if (!File.Exists(filePath))
                {
                    TableStateData initialData = CreateInitialData();

                    Save(initialData);

                    return initialData;
                }

                string json = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    TableStateData initialData = CreateInitialData();

                    Save(initialData);

                    return initialData;
                }

                TableStateData data =
                    JsonConvert.DeserializeObject<TableStateData>(json);

                if (data == null)
                {
                    return CreateInitialData();
                }

                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[TableStateStore Load 오류] " + ex.Message
                );

                return CreateInitialData();
            }
        }


        // -------------------------------------------------
        // 최초 테이블 상태 생성
        //
        // T01 ~ T10 모두 빈자리로 시작
        // -------------------------------------------------
        private static TableStateData CreateInitialData()
        {
            TableStateData data = new TableStateData();

            data.TotalTables = TOTAL_TABLES;
            data.ReleaseSequence = 0;

            for (int i = 1; i <= TOTAL_TABLES; i++)
            {
                string tableCode = $"T{i:D2}";

                data.Tables[tableCode] = AVAILABLE;
            }

            return data;
        }


        // -------------------------------------------------
        // JSON 저장
        // -------------------------------------------------
        private static void Save(TableStateData data)
        {
            try
            {
                EnsureFolderExists();

                string json =
                    JsonConvert.SerializeObject(
                        data,
                        Newtonsoft.Json.Formatting.Indented
                    );

                // 바로 원본에 쓰지 않고 임시 파일에 먼저 저장
                string tempPath = filePath + ".tmp";

                File.WriteAllText(tempPath, json);

                // 저장 완료 후 원본 파일로 교체
                File.Move(
                    tempPath,
                    filePath,
                    true
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[TableStateStore Save 오류] " + ex.Message
                );
            }
        }


        // -------------------------------------------------
        // 저장 폴더가 없으면 자동 생성
        // -------------------------------------------------
        private static void EnsureFolderExists()
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }


        // -------------------------------------------------
        // 디버깅용
        // 실제 JSON 저장 위치 확인
        // -------------------------------------------------
        public static string GetFilePath()
        {
            return filePath;
        }
    }
}