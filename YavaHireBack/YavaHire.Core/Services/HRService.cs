using Microsoft.EntityFrameworkCore;
using YavaHire.Core.DataBase;
using YavaHire.Core.DataBase.Models;
using YavaHire.Core.Interfaces;

namespace YavaHire.Core.Services
{
    public class HRService : IHRService
    {
        private YavaHireDBContext _dBContext;

        public HRService(YavaHireDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<List<HR>> GetAll()
        {
            return await _dBContext.HRs.ToListAsync();
        }
    }
}
