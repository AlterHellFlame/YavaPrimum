namespace YavaPrimum.Core.DataBase.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; } //Отчество

        public string ImgUrl { get; set; }
        public string Phone { get; set; }

        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public Company Company { get; set; }
        public Post Post { get; set; }
    }
}
