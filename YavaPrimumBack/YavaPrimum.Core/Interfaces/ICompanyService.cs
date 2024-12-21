using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.Interfaces
{
    public interface ICompanyService
    {
        Task<Guid> Create(Company company);
        Task<List<Company>> GetAll();
        Task<Company> GetById(Guid companyId);
        Task<Company> GetByName(string companyName);
    }
}