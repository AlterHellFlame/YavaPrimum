using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;

namespace YavaPrimum.Core.Interfaces
{
    public interface IUserService
    {
        string GeneratePasswordHas(string password);
        Task<User> GetByEMail(string email);
        Task<bool> IsUserExistByEMail(string email);
        Task<User> GetById(Guid id);
        Task<string> Login(string email, string password);
        Task<Guid> Register(RegisterUserRequest registerUserRequest);
        bool Verify(string password, string passwordHash);
        Task<List<User>> GetAll();
        Task<UserResponse> GetByIdToFront(Guid id);
        Task<string> SendMessageToEmail(string email, string myMessage, string subject);
        Task<User> SetNewPassByEMail(string email, string newPass);
    }
}