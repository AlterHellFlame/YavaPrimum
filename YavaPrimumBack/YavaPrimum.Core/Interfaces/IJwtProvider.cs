using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
        Task<Guid> GetUserIdFromToken(string token);
    }
}