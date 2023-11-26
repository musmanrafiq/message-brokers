using RabbitMQ.Client;
using System.Text;

namespace ConsoleApp.Producer
{
    internal class Program
    {
        private static string AMQP_URL = "";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(AMQP_URL)
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "ulogs", type: ExchangeType.Fanout);

            while (true)
            {
                Console.WriteLine("Type your message to send!, press ESC to exit");

                var message = Console.ReadLine();

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "ulogs",
                                     routingKey: string.Empty,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine($" [x] Sent {message}");


            }


        }


    }
}