using ChatServer.DataAccess;
using ChatServer.Domain;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            //using (var context = new ChatContext())
            {
                var localIp = IPAddress.Parse("127.0.0.1");
                var port = 8080;
                var endPoint = new IPEndPoint(localIp, port);

                socket.Bind(endPoint);
                socket.Listen(5); // Максимальное число соединений в очереди.

                Console.WriteLine($"Приложение слушает порт {port}.");

                while (true)
                {
                    var incomingSocket = await socket.AcceptAsync(); //Блокирует поток, пока не получит сообщение
                    Console.WriteLine($"Получено входящее сообщение.");
                    await GetMessage(incomingSocket);
                    await SendMessages(socket);
                }
            }
        }

        static public async Task SendMessages(Socket socket)
        {
            using (var context = new ChatContext())
            {
                var messages = await context.Messages.ToListAsync();
                var json = JsonConvert.SerializeObject(messages);

                var sendData = Encoding.UTF8.GetBytes(json);
                ArraySegment<byte> data = new ArraySegment<byte>(sendData);
                var res = await socket.SendAsync(data, SocketFlags.None);
                Console.WriteLine("Сообщения обновились.");
            }
        }

        static public async Task GetMessage(Socket incomingSocket)
        {
            var stringBuilder = new StringBuilder();
            while (incomingSocket.Available > 0)
            {
                var buffer = new byte[1024];
                ArraySegment<byte> data = new ArraySegment<byte>(buffer);
                await incomingSocket.ReceiveAsync(data, SocketFlags.None);
                stringBuilder.Append(Encoding.UTF8.GetString(buffer));
            }
            using (var context = new ChatContext())
            {
                var message = JsonConvert.DeserializeObject<Message>(stringBuilder.ToString());
                context.Messages.Add(message);
                await context.SaveChangesAsync();
            }
            incomingSocket.Send(Encoding.UTF8.GetBytes("Вернись в палату, долбоеб"));
        }
    }
}
