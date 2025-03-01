using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;

namespace YavaPrimum.Core.Interfaces
{
    public interface IUserService
    {
        Task<User> GetByEMail(string email);
        Task<bool> IsUserExistByEMail(string email);
        Task<User> GetById(Guid id);
        Task<List<User>> GetAll();
        Task<UserRequestResponse> GetByIdToFront(Guid id);
        Task<UserRequestResponse> ConvertToFront(User user);
    }
}