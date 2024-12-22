using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DataBase;
using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class CompanyService : ICompanyService
    {
        private YavaPrimumDBContext _dbContext;

        public CompanyService(YavaPrimumDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Company>> GetAll()
        {
            return await _dbContext.Company
                .Include(c => c.Country)
                .ToListAsync();
        }

        public async Task<Company> GetById(Guid companyId)
        {
            return await _dbContext.Company.FindAsync(companyId);
        }

        public async Task<Company> GetByName(string companyName)
        {
            return await _dbContext.Company
                .Include(c => c.Country)
                .FirstOrDefaultAsync(t => t.Name == companyName);
        }

        public async Task<Guid> Create(Company company)
        {
            await _dbContext.Company.AddAsync(company);
            await _dbContext.SaveChangesAsync();
            return company.CompanyId;
        }
    }
}
