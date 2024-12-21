using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.Interfaces
{
    public interface ICountryService
    {
        Task<Guid> Create(Country country);
        Task<List<Country>> GetAll();
        Task<Country> GetById(Guid countryId);
        Task<Country> GetByName(string countryName);
    }
}