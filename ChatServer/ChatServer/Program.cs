using ChatServer.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
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
                    var incomingSocket = socket.Accept(); //Блокирует поток, пока не получит сообщение
                    Console.WriteLine($"Получено входящее сообщение.");
                    while (incomingSocket.Available > 0)
                    {
                        var buffer = new byte[incomingSocket.Available];
                        incomingSocket.Receive(buffer);
                        var mes = Encoding.UTF8.GetString(buffer);
                        var user = mes.Substring(0, mes.LastIndexOf("|userMessage|"));
                        var text = mes.Substring(mes.LastIndexOf("|userMessage|")+13);
                        var s = mes.Split( new[] { "qwe" }, StringSplitOptions.None);
                        Console.WriteLine($"{user} : {text}");
                        incomingSocket.Send(Encoding.UTF8.GetBytes("Вернись в палату, долбоеб"));
                    }

                    incomingSocket.Close();
                    Console.WriteLine("Ожидается еще.");
                }
            }
        }
    }
}
