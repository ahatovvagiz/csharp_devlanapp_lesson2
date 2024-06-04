using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Server
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public string NicknameFrom { get; set; }
        public string NicknameTo { get; set; }
        public string SerializeMessageToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Message? DeserializeFromJson(string message) => JsonSerializer.Deserialize<Message>(message);

        public void Print()
        {
            Console.WriteLine(ToString());
        }
        public override string ToString()
        {
            return $"{this.DateTime} получено сообщение {this.Text}  от  {this.NicknameFrom}";
        }
    }
    class Program
    {
        static bool exit = false;
        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(obj =>
            {
                Server("Hello");
            });

            Console.WriteLine("Для выхода нажмите любую клавишу...");
            Console.ReadKey();

        }

        static void tasj1()
        {
            Message msg = new Message() { Text = "Hello", DateTime = DateTime.Now, NicknameFrom = "Ivan", NicknameTo = "All" };
            string json = msg.SerializeMessageToJson();
            Console.WriteLine(json);
            Message? msgDeserialized = Message.DeserializeFromJson(json);
        }

        public static void Server(string message)
        {
            UdpClient udpClient = new UdpClient(12345);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("Сервер ждет сообщение от клиента.Нажмите любую клавишу для выхода.");
            //bool exit = false;
            while (!exit)
            {
                byte[] buffer = udpClient.Receive(ref iPEndPoint);
                var messageText = Encoding.UTF8.GetString(buffer);

                ThreadPool.QueueUserWorkItem(obj =>
                {
                    Message message1 = Message.DeserializeFromJson(messageText);
                    message1.Print();

                    // Отправляем подтверждение клиенту
                    string confirmationMessage = "Сообщение успешно обработано на сервере";
                    byte[] confirmationBuffer = Encoding.UTF8.GetBytes(confirmationMessage);
                    udpClient.Send(confirmationBuffer, confirmationBuffer.Length, iPEndPoint);

                    if (message1.Text.ToLower() == "exit")
                    {
                        exit = true;
                        Console.WriteLine("Завершение работы...");
                    }
                });
            }
            udpClient.Close();
        }
    }
}