using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DTO
{
    public class TasksResponse
    {
        public Guid TaskId { get; set; }
        public string Status { get; set; }
        public int TypeStatus { get; set; }
        public UserRequestResponse User { get; set; }
        public DateTime? DateTime { get; set; }
        public CandidateRequestResponse Candidate { get; set; }
    }

}
