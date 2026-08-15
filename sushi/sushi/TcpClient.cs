using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetTcpClient = System.Net.Sockets.TcpClient;

namespace sushi
{
    public static class TcpClient
    {
        public static string ServerIp = "192.168.0.62";
        public const int ServerPort = 9000;

        public static async Task<string> SendJsonAsync(string json)
        {
            using CancellationTokenSource timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using NetTcpClient client = new NetTcpClient();

            await client.ConnectAsync(ServerIp, ServerPort, timeout.Token);

            using NetworkStream stream = client.GetStream();

            byte[] sendData = Encoding.UTF8.GetBytes(json);
            await stream.WriteAsync(sendData.AsMemory(0, sendData.Length), timeout.Token);
            await stream.FlushAsync(timeout.Token);

            using MemoryStream responseBuffer = new MemoryStream();
            byte[] buffer = new byte[4096];

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), timeout.Token);

                if (bytesRead == 0)
                    break;

                responseBuffer.Write(buffer, 0, bytesRead);
            }

            if (responseBuffer.Length == 0)
                throw new Exception("관리자 서버에서 응답이 오지 않았습니다.");

            return Encoding.UTF8.GetString(responseBuffer.ToArray());
        }
    }
}