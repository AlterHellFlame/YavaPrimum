using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;

namespace YavaPrimum.Core.Interfaces
{
    public interface ITasksService
    {
        Task<Guid> Create(Tasks task);
        Task<List<Tasks>> GetAll();
        Task<List<Tasks>> GetAllByUserId(Guid userId);
        Task Delete(Guid taskId);
        Task PassedInterview(Guid taskId);
        Task Update(Guid taskId, InterviewCreateRequest newTask);
        Task<Tasks> GetById(Guid taskId);
        Task RepeatInterview(Guid taskId, string dateTime);
    }
}
