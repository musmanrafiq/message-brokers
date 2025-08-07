using Contracts;

namespace ReveiverApp
{
    public class JobSaga : Saga<JobSagaData>,
        IAmStartedByMessages<JobAssigned>,
        IHandleMessages<JobCompleted>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<JobSagaData> mapper)
        {
            mapper.ConfigureMapping<JobAssigned>(msg => msg.JobId).ToSaga(saga => saga.JobId);
            mapper.ConfigureMapping<JobCompleted>(msg => msg.JobId).ToSaga(saga => saga.JobId);
        }

        public Task Handle(JobAssigned message, IMessageHandlerContext context)
        {
            Console.WriteLine($"📌 Saga started for Job: {message.JobId}");
            return Task.CompletedTask;
        }

        public Task Handle(JobCompleted message, IMessageHandlerContext context)
        {
            Console.WriteLine($"✅ Job completed: {message.JobId} at {message.CompletedAt}");
            MarkAsComplete();
            return Task.CompletedTask;
        }
    }
}
