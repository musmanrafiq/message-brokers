using Contracts;

namespace Sender
{
    internal class Program
    {
        static async Task Main()
        {
            Console.Title = "Sender";

            var endpointConfiguration = new EndpointConfiguration("Sender");
            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

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
                    JobId = "jobid",
                    AssignedTo = "usman",
                    AssignedAt = DateTime.UtcNow
                };

                await endpointInstance.Send(jobAssigned);
                Console.WriteLine($"Sent JobAssigned: {jobAssigned.JobId}");

                // Optionally simulate job completed
                await Task.Delay(2000); // 2 sec delay

                var jobCompleted = new JobCompleted
                {
                    JobId = jobAssigned.JobId,
                    CompletedAt = DateTime.UtcNow
                };

                var sendOptions = new SendOptions();
                sendOptions.SetDestination("Receiver"); // <-- Set the target endpoint name
                await endpointInstance.Send(jobCompleted, sendOptions);
            }

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
