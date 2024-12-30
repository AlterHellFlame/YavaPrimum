using Azure.Core;
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
        private readonly ITaskTypeService _taskType;
        private readonly ICandidateService _candidateService;

        public TaskService(YavaPrimumDBContext dBContext,
            ITaskTypeService taskType,
            ICandidateService candidateService)
        {
            _dBContext = dBContext;
            _taskType = taskType;
            _candidateService = candidateService;
        }

        public async Task<Guid> Create(Tasks task)
        {
            await _dBContext.Tasks.AddAsync(task);
            await _dBContext.SaveChangesAsync();

            return task.TasksId;
        }

        public async Task<Guid> Create(TasksRequest taskRequest, User user)
        {
            /*Tasks task = new Tasks()
            {
                TasksId = Guid.NewGuid(),
                Candidate = taskRequest.Candidate,
                DateTime = taskRequest.DateTime,
                User = user,
                Status = taskRequest.Status,
                TaskType = await _taskType.GetByName(taskRequest.TaskType),
            };
            Console.WriteLine("Попытка добавить таску " + taskRequest.Candidate.SecondName);
            await _dBContext.Candidate.AddAsync(taskRequest.Candidate);
            await _dBContext.SaveChangesAsync();*/
            Console.WriteLine("Тут ничего не работает");
            return new Guid();
        }

        public async Task<List<Tasks>> GetAll()
        {
            return await _dBContext.Tasks
                .Include(t => t.TaskType)
                .ToListAsync();
        }

        public async Task<Tasks> GetById(Guid taskId)
        {
            return await _dBContext.Tasks
                .Include(t => t.Candidate)
                .Include(t => t.User)
                .Include(t => t.TaskType)
                .FirstOrDefaultAsync(t => t.TasksId == taskId);
        }

        public async Task<List<Tasks>> GetAllByUserId(Guid userId)
        {
            return await _dBContext.Tasks
                .Include(t => t.Candidate)
                .Include(t => t.Candidate.Country)
                .Include(t => t.Candidate.HR)
                .Include(t => t.Candidate.OP)
                .Include(t => t.Candidate.Post)
                .Include(t => t.TaskType)
                .Where(t => t.User.UserId == userId)
                .ToListAsync();
        }

        public async Task Delete(Guid taskId)
        {
            await _dBContext.Tasks
                .Where(t => t.TasksId == taskId)
                .ExecuteDeleteAsync();

            await _dBContext.SaveChangesAsync();
        }

        public async Task CommitTask(Guid taskId)
        {
            (await _dBContext.Tasks
                .FindAsync(taskId))
                .Status = true;

            await _dBContext.SaveChangesAsync();
        }

        public async Task Update(Guid taskId, InterviewCreateRequest newTask)
        {
            Tasks task = await GetById(taskId);


            task.DateTime = DateTime.Parse(newTask.InterviewDate);

            //task.Candidate = await _candidateService.GetById(newTask.Candidate.CandidateId);
            task.TaskType = await _taskType.GetByName("Интервью");

            await _dBContext.SaveChangesAsync();
        }

        public async Task RepeatInterview(Guid taskId)
        {
            Tasks task = await GetById(taskId);
            task.Status = true;

            Tasks repeatTask = new Tasks()
            {
                TasksId = Guid.NewGuid(),
                Candidate = task.Candidate,
                TaskType = task.TaskType,
                DateTime = task.DateTime,
                Status = task.Status,
                User = task.User
            };

            await _dBContext.Tasks.AddAsync(repeatTask);
            await _dBContext.SaveChangesAsync();
        }
    }
}

