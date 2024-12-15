using YavaHire.Core.DataBase.Models;

namespace YavaHire.Core.Interfaces
{
    public interface IHRService
    {
        Task<List<HR>> GetAll();
    }
}