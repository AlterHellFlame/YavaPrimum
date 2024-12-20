using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YavaPrimum.Core.DataBase.Models
{
    public class Tasks
    {
        public Guid TasksId { get; set; }
        public User User { get; set; }
        public Candidate Candidate { get; set; }
        public string Status { get; set; }
        public DateTime DateTime { get; set; }
    }
}
