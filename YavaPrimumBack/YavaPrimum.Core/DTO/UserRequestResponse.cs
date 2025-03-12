using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DTO
{
    public class UserRequestResponse
    {
        public Guid UserId { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; } // Отчество
        public string ImgUrl { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Post { get; set; }
    }

}


