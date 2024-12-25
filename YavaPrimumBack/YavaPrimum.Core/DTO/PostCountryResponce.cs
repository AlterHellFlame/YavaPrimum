using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DTO
{
    public class PostCountryResponce
    {
        public List<string> Posts { get; set; }
        public List<string> Countres { get; set; }
    }
}
