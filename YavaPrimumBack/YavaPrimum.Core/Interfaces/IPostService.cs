using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.Interfaces
{
    public interface IPostService
    {
        Task<Guid> Create(Post post);
        Task<List<Post>> GetAll();
        Task<Post> GetById(Guid postId);
        Task<Post> GetByName(string postName);
    }
}