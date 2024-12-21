using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DTO
{
    public class TasksRequest
    {
        public User User { get; set; }
        public Candidate Candidate { get; set; }
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }
    }
}
