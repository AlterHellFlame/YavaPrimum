using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase;

namespace YavaPrimum.Core.Services
{
    public class PersonalOfficerService
    {
        private YavaPrimumDBContext _dBContext;

        public PersonalOfficerService(YavaPrimumDBContext dBContext)
        {
            _dBContext = dBContext;
        }
    }
}
