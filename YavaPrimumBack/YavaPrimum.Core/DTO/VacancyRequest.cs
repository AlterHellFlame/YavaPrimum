using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DTO
{
    public class VacancyRequest
    {
        public string Post { get; set; }
        public int Count { get; set; }
        public string AdditionalData { get; set; }

    }
}
