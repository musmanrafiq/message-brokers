using Contracts;

namespace ReveiverApp
{
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
