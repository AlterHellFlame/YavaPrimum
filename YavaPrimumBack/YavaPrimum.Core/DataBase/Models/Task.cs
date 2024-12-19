namespace YavaPrimum.Core.DataBase.Models
{
    public class Task
    {
        public Guid TaskId { get; set; }
        public User User { get; set; }
        public Candidate Candidate { get; set; }
        public string Status { get; set; }
        public DateTime DateTime { get; set; }
    }
}
