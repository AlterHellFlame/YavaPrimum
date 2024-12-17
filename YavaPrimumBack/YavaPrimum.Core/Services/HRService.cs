using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class HRService : IHRService
    {
        private YavaPrimumDBContext _dBContext;

        public HRService(YavaPrimumDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        //public async Task<List<HR>> GetAll()
        //{
        //    return await _dBContext.HRs.ToListAsync();
        //}
    }
}
