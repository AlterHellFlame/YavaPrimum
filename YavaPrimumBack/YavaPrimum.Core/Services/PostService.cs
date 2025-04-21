using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class PostService : IPostService
    {
        private YavaPrimumDBContext _dbContext;

        public PostService(YavaPrimumDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Post>> GetAll()
        {
            return await _dbContext.Post
                .Where(p => p.Name != "!Админ")
                .ToListAsync();
        }

        public async Task<Post> GetById(Guid postId)
        {
            return await _dbContext.Post.FindAsync(postId);
        }

        public async Task<Post> GetByName(string postName)
        {
            return await _dbContext.Post
                .FirstOrDefaultAsync(t => t.Name == postName);
        }

        public async Task<Guid> Create(Post post)
        {
            await _dbContext.Post.AddAsync(post);
            await _dbContext.SaveChangesAsync();
            return post.PostId;
        }
    }
}
