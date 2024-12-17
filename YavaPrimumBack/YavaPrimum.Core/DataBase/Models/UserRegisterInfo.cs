namespace YavaPrimum.Core.DataBase.Models
{
    public class UserRegisterInfo
    {
        public Guid UserRegisterInfoId { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; }
    }
}
