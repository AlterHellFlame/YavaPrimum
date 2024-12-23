using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YavaPrimum.Core.DTO
{
    public class UserResponse
    {
        public string FirstName { get; set; }
        public string SecondName   { get; set; }
        public string SurName { get; set; }
        public string Company {  get; set; }
        public string Email { get; set; }
        public string Post {  get; set; }
        public string Country {  set; get; }
    }
}
