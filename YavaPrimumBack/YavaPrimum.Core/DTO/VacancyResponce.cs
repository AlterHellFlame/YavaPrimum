using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DTO
{
    public class VacancyResponce
    {
        public Guid VacancyId { get; set; }
        public UserRequestResponse User { get; set; }
        public string Post { get; set; }
        public int Count { get; set; }
        public bool isClose { get; set; }
        public string AdditionalData { get; set; }
    }
}
