using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DTO
{
    public class PostCountryResponse
    {
        public List<string> Posts { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Companies { get; set; }
        public List<string> PhoneMask { get; set; }
    }
}
