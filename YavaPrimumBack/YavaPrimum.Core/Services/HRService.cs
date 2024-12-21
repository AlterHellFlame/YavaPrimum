using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class HRService : IHRService
    {
        private YavaPrimumDBContext _dbContext;

        public HRService(YavaPrimumDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public async Task<User> GetById(Guid HRId)
        //{
        //    return await _dbContext.User.FindAsync(HRId);
        //}
    }
}
