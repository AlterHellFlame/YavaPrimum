using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DTO
{
    public class TasksRequest
    {
        public CandidateRequestResponse Candidate { get; set; }
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }
        public string TaskType { get; set; }
    }
}
