using Contracts;

namespace ReceiverApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Receiver";

            var endpointConfiguration = new EndpointConfiguration("Receiver");

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString("Server=localhost\\SQLEXPRESS;Database=nserbdb;Trusted_Connection=True;TrustServerCertificate=True;");

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

    public class JobAssignedHandler : IHandleMessages<JobAssigned>
    {
        public Task Handle(JobAssigned message, IMessageHandlerContext context)
        {
            Console.WriteLine("📥 Received JobAssigned:");
            Console.WriteLine($"  🔹 JobId     : {message.JobId}");
            Console.WriteLine($"  🔹 AssignedTo: {message.AssignedTo}");
            Console.WriteLine($"  🔹 AssignedAt: {message.AssignedAt}");

            return Task.CompletedTask;
        }
    }
}
