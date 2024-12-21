using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DataBase;
using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class TaskTypeService : ITaskTypeService
    {
        private YavaPrimumDBContext _dbContext;

        public TaskTypeService(YavaPrimumDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TaskType>> GetAll()
        {
            return await _dbContext.TaskType
                .ToListAsync();
        }

        public async Task<TaskType> GetById(Guid taskTypeId)
        {
            return await _dbContext.TaskType.FindAsync(taskTypeId);
        }

        public async Task<TaskType> GetByName(string taskTypeName)
        {
            return await _dbContext.TaskType
                .FirstOrDefaultAsync(t => t.Name == taskTypeName);
        }

        public async Task<Guid> Create(TaskType taskType)
        {
            await _dbContext.TaskType.AddAsync(taskType);
            await _dbContext.SaveChangesAsync();
            return taskType.TaskTypeId;
        }
    }
}
