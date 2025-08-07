using System.Data.SqlClient;

namespace ReceiverApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Receiver";

            var endpointConfiguration = new EndpointConfiguration("Receiver");
            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString("Server=localhost\\SQLEXPRESS;Database=nserbdb;Trusted_Connection=True;TrustServerCertificate=True;");

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(() =>
            new SqlConnection("Server=localhost\\SQLEXPRESS;Database=nserbdb;Trusted_Connection=True;TrustServerCertificate=True;"));

            // Optional but recommended for development
            endpointConfiguration.EnableInstallers();

            // No persistence needed for simple command handling
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            // Keep running until Escape key is pressed
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Escape)
                        break;
                }

                // Sleep briefly to avoid CPU spin
                await Task.Delay(100);
            }

            Console.WriteLine("⏹️ Stopping receiver...");

            await endpointInstance.Stop()
                .ConfigureAwait(false);

            Console.WriteLine("✅ Receiver stopped.");
        }

    }


}
