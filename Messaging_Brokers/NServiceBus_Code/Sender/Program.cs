using Contracts;

namespace Sender
{
    internal class Program
    {
        static async Task Main()
        {
            Console.Title = "Sender";

            var endpointConfiguration = new EndpointConfiguration("Sender");

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString("Server=localhost\\SQLEXPRESS;Database=nserbdb;Trusted_Connection=True;TrustServerCertificate=True;");
            transport.Routing().RouteToEndpoint(typeof(JobAssigned), "Receiver");

            //endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            endpointConfiguration.EnableInstallers();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press any key to send a JobAssigned message. Press ESC to exit.");

            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Escape)
                    break;

                var jobAssigned = new JobAssigned
                {
                    JobId = Guid.NewGuid(),
                    AssignedTo = "usman",
                    AssignedAt = DateTime.UtcNow
                };

                await endpointInstance.Send(jobAssigned);
                Console.WriteLine($"Sent JobAssigned: {jobAssigned.JobId}");
            }

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
