using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;

namespace YavaPrimum.Core.Interfaces
{
    public interface ITasksService
    {
        Task<Guid> Create(Tasks task);
        Task<List<Tasks>> GetAll();
        Task<List<Tasks>> GetAllByUserId(Guid userId);
        Task<List<TasksResponse>> ConvertToFront(List<Tasks> tasks);
        Task<TasksResponse> ConvertToFront(Tasks tasks);
        Task Delete(Guid taskId);
        Task PassedInterview(Guid taskId);
        Task FaidInterview(Guid taskId);
        Task Update(Guid taskId, InterviewCreateRequest newTask);
        Task<Tasks> GetById(Guid taskId);
        Task RepeatInterview(Guid taskId, string dateTime);
        Task<TasksStatus> GetStatusByName(string name);
        Task SetNewStatus(Tasks tasks, string status);
    }
}
