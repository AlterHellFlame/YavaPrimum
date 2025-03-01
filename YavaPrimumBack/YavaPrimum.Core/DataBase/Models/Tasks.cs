namespace YavaPrimum.Core.DataBase.Models
{
    public class Tasks
    {
        public Guid TasksId { get; set; }
        public TasksStatus Status { get; set; }

        public DateTime? DateTime { get; set; }

        public Post CandidatePost { get; set; }
        public User User { get; set; }
        public Candidate Candidate { get; set; }

    }
}
