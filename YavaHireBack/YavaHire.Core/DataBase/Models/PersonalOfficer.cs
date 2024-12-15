using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YavaHire.Core.DataBase.Models
{
    public class PersonalOfficer
    {
        public int PersonalOfficerId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string SurName { get; set; } //Отчество
    }
}
