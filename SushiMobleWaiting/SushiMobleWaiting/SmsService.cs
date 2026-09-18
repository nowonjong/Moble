using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SushiMobleWaiting
{
    public class SmsService
    {
        private readonly string apiKey =
            Environment.GetEnvironmentVariable("SOLAPI_API_KEY");

        private readonly string apiSecret =
            Environment.GetEnvironmentVariable("SOLAPI_API_SECRET");

        private readonly string senderNumber =
            Environment.GetEnvironmentVariable("SOLAPI_SENDER_NUMBER");




        // SOLAPI 인증 헤더 생성
        private string GetAuthorizationHeader()
        {
            if (string.IsNullOrEmpty(apiKey) ||
                string.IsNullOrEmpty(apiSecret))
            {
                throw new Exception("SOLAPI API Key 또는 Secret이 설정되지 않았습니다.");
            }

            // 요청할 때마다 새로운 Salt 생성
            string salt = Guid.NewGuid()
                .ToString("N")
                .Substring(0, 16);

            // 현재 UTC 시간
            string date = DateTime.UtcNow.ToString(
                "yyyy-MM-ddTHH:mm:ss'Z'"
            );

            string data = date + salt;

            // API Secret을 이용해 HMAC-SHA256 서명 생성
            byte[] secretBytes = Encoding.UTF8.GetBytes(apiSecret);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            string signature;

            using (HMACSHA256 hmac = new HMACSHA256(secretBytes))
            {
                byte[] hash = hmac.ComputeHash(dataBytes);

                StringBuilder builder = new StringBuilder();

                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }

                signature = builder.ToString();
            }

            return
                $"HMAC-SHA256 apiKey={apiKey}, " +
                $"date={date}, " +
                $"salt={salt}, " +
                $"signature={signature}";
        }


        // 실제 문자 발송
        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            if (string.IsNullOrEmpty(senderNumber))
            {
                throw new Exception("SOLAPI 발신번호가 설정되지 않았습니다.");
            }

            using (HttpClient client = new HttpClient())
            {
                // SOLAPI 인증
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization",
                    GetAuthorizationHeader()
                );

                // SOLAPI에 보낼 JSON 데이터
                var payload = new
                {
                    messages = new[]
                    {
                        new
                        {
                            to = phoneNumber,
                            from = senderNumber,
                            text = message
                        }
                    }
                };

                string json = JsonConvert.SerializeObject(payload);

                StringContent content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                HttpResponseMessage response =
                    await client.PostAsync(
                        "https://api.solapi.com/messages/v4/send-many/detail",
                        content
                    );

                string responseBody =
                    await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine(responseBody);

                // HTTP 요청 자체가 실패한 경우
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                // HTTP 200이어도 개별 메시지 등록이 실패할 수 있으므로 확인
                JObject result = JObject.Parse(responseBody);

                JArray failedMessages =
                    result["failedMessageList"] as JArray;

                if (failedMessages != null &&
                    failedMessages.Count > 0)
                {
                    return false;
                }

                return true;
            }
        }
    }
}