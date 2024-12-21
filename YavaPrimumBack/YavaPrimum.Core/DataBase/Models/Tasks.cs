namespace YavaPrimum.Core.DataBase.Models
{
    public class Tasks
    {
        public Guid TasksId { get; set; }
        public bool Status { get; set; } = false;
        public DateTime DateTime { get; set; }

        public TaskType TaskType { get; set; }
        public User User { get; set; }
        public Candidate Candidate { get; set; }
    }
}
