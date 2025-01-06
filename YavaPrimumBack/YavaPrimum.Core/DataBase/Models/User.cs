namespace YavaPrimum.Core.DataBase.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string SurName { get; set; }

        public string ImgUrl { get; set; } = "profile_photo/default.jpg";
        public string Phone { get; set; }

        public Company Company { get; set; }
        public Post Post { get; set; }
        public UserRegisterInfo UserRegisterInfo { get; set; }
    }
}
