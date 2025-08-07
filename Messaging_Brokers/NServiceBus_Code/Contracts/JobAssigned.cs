namespace Contracts
{
    public class JobAssigned : ICommand
    {
        public Guid JobId { get; set; }
        public string AssignedTo { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
