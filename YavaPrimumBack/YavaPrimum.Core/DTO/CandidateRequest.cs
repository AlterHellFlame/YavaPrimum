using System.Runtime.InteropServices;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.DTO
{
    public class CandidateRequest
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string SurName { get; set; }
        public string Post { get; set; }
        public string Country { get; set; }
    }

}

