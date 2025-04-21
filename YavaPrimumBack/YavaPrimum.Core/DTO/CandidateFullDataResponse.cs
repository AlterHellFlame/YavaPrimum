using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YavaPrimum.Core.DTO
{
    public class CandidateFullDataResponse
    {
        public CandidateRequestResponse Candidate {  get; set; }
        public List<TasksResponse> Tasks { get; set; }
    }
}
