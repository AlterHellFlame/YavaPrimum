using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;

namespace YavaPrimum.Core.Interfaces
{
    public interface IAuthService
    {
        string GeneratePasswordHas(string password);
        Task<string> Login(string email, string password);
        Task<Guid> Register(UserRequestResponse userRequestResponse);
        bool Verify(string password, string passwordHash);
        Task<string> SendMessageToEmail(string email, string myMessage, string subject);
        Task<User> SetNewPassByEMail(string email, string newPass);
    }
}
