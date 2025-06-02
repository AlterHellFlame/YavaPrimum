using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;

namespace YavaPrimum.Core.Interfaces
{
    public interface IVacancyService
    {
        Task<List<Vacancy>> GetAll();
        Task Close(Guid id);
        Task Update(Guid id, VacancyRequest vacancy);
        Task Create(VacancyRequest vacancy, User user);
    }
}
