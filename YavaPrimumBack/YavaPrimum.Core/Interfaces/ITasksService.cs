using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.Interfaces
{
    public interface ITasksService
    {
        Task<Guid> Create(Tasks task);
        Task<List<Tasks>> GetAll();
        Task<List<Tasks>> GetAllByUserId(Guid userId);
    }
}
