using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YavaPrimum.Core.DataBase.Models
{
    public class Vacancy
    {
        public Guid VacancyId { get; set; }
        public User User { get; set; }
        public Post Post { get; set; } 
        public int Count { get; set; }
        public bool isClose { get; set; }
        public string AdditionalData { get; set; }
    }
}
