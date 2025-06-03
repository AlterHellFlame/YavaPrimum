using Microsoft.AspNetCore.Http;
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
        Task<Tasks> GetById(Guid taskId);
        Task<TasksStatus> GetStatusByName(string name);
        Task SetActive(Guid taskId);
        Task SetNewDate(Tasks task, DateTime dateTime);
        Task SetNewStatus(Tasks task, StatusUpdateRequest updateRequest = null, string status = null);
        Task<Tasks> GetLastActiveTask(Guid candidateId, Guid userId);
        Task ChangeTime(Guid taskId, ChangeDateTimeRequest changeRequest);
        Task ChangeTimeWithoutCheck(Guid taskId, DateTime dateTime);
    }
}
