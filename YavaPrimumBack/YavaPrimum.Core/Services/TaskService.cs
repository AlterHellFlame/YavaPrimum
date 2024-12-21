using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class TaskService : ITasksService
    {
        private readonly YavaPrimumDBContext _dBContext;

        public TaskService(YavaPrimumDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<Guid> Create(Tasks task)
        {
            await _dBContext.Tasks.AddAsync(task);
            await _dBContext.SaveChangesAsync();

            return task.TasksId;
        }

        public async Task<Guid> Create(TasksRequest taskRequest)
        {
            Tasks task = new Tasks()
            {
                TasksId = Guid.NewGuid(),
                Candidate = taskRequest.Candidate,
                DateTime = taskRequest.DateTime,
                User = taskRequest.User,
                Status = taskRequest.Status,
            };
            Console.WriteLine("Попытка добавить таску " + taskRequest.Candidate.SecondName);
            await _dBContext.Candidate.AddAsync(taskRequest.Candidate);
            await _dBContext.SaveChangesAsync();
            return taskRequest.Candidate.CandidateId;
        }

        public async Task<List<Tasks>> GetAll()
        {
            return await _dBContext.Tasks.ToListAsync();
        }

        public async Task<List<Tasks>> GetAllByUserId(Guid userId)
        {
            return await _dBContext.Tasks.Where(t => t.User.UserId == userId).ToListAsync();
        }
    }
}
