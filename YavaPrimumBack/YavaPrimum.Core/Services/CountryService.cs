using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DataBase;
using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class CountryService : ICountryService
    {
        private YavaPrimumDBContext _dbContext;

        public CountryService(YavaPrimumDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Country>> GetAll()
        {
            return await _dbContext.Country
                .ToListAsync();
        }

        public async Task<Country> GetById(Guid countryId)
        {
            return await _dbContext.Country.FindAsync(countryId);
        }

        public async Task<Country> GetByName(string countryName)
        {
            return await _dbContext.Country
                .FirstOrDefaultAsync(t => t.Name == countryName);
        }

        public async Task<Guid> Create(Country country)
        {
            await _dbContext.Country.AddAsync(country);
            await _dbContext.SaveChangesAsync();
            return country.CountryId;
        }
    }
}
