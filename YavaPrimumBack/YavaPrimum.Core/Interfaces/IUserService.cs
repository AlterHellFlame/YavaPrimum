using System.Net.Mail;
using System.Net;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Services;

namespace YavaPrimum.Core.Interfaces
{
    public interface IUserService
    {
        Task<User> GetByEMail(string email);
        Task<bool> IsUserExistByEMail(string email);
        Task UpdateUser(User user);
        Task DeleteUserById(Guid id);
        Task<User> GetById(Guid id);
        Task<List<User>> GetAll();
        Task<UserRequestResponse> GetByIdToFront(Guid id);
        Task<User> GetAnotherUserOfCandidate(Tasks task);

    }
}