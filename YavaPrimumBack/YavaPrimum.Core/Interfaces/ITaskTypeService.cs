using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.Interfaces
{
    public interface ITaskTypeService
    {
        Task<Guid> Create(TaskType taskType);
        Task<List<TaskType>> GetAll();
        Task<TaskType> GetById(Guid taskTypeId);
        Task<TaskType> GetByName(string taskTypeName);
    }
}