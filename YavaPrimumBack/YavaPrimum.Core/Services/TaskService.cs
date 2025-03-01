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
        private readonly ICandidateService _candidateService;

        public TaskService(YavaPrimumDBContext dBContext,
            ICandidateService candidateService)
        {
            _dBContext = dBContext;
            _candidateService = candidateService;
        }

        public async Task<Guid> Create(Tasks task)
        {
            await _dBContext.Tasks.AddAsync(task);
            await _dBContext.SaveChangesAsync();

            return task.TasksId;
        }

        public async Task<List<Tasks>> GetAll()
        {
            return await _dBContext.Tasks
                .Include(t => t.Status)
                .Include(t => t.User)
                .Include(t => t.Candidate)
                .ToListAsync();
        }

        public async Task<Tasks> GetById(Guid taskId)
        {
            return await _dBContext.Tasks
                .Include(t => t.Candidate)
                .Include(t => t.Candidate.Country)
                .Include(t => t.Status)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TasksId == taskId);
        }

        public async Task<List<Tasks>> GetAllByUserId(Guid userId)
        {
            return await _dBContext.Tasks
                .Include(t => t.Candidate)
                .Include(t => t.Candidate.Country)
                .Include(t => t.Status)
                .Include(t => t.User)
                .Where(t => t.User.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<TasksResponse>> ConvertToFront(List<Tasks> tasks)
        {
            List<Task<TasksResponse>> taskResponseTasks = tasks.Select(task => ConvertToFront(task)).ToList();
            TasksResponse[] taskResponses = await Task.WhenAll(taskResponseTasks);
            return taskResponses.ToList();
        }

        public async Task<TasksResponse> ConvertToFront(Tasks task)
        {
            TasksResponse tasksResponse = new TasksResponse
            {
                TaskId = task.TasksId,
                Status = task.Status.Name,
                DateTime = task.DateTime,
                User = task.User,
                Candidate = new CandidateRequestResponse
                {
                    Surname = task.Candidate.Surname,
                    FirstName = task.Candidate.FirstName,
                    Patronymic = task.Candidate.Patronymic,
                    Email = task.Candidate.Email,
                    Phone = task.Candidate.Phone,
                    Country = task.Candidate.Country.Name
                }
            };

            return tasksResponse;
        }


        public async Task Delete(Guid taskId)
        {
            await _dBContext.Tasks
                .Where(t => t.TasksId == taskId)
                .ExecuteDeleteAsync();

            await _dBContext.SaveChangesAsync();
        }

        public async Task PassedInterview(Guid taskId)
        {
           /* (await _dBContext.Tasks
                .FindAsync(taskId))
                .Status = true;

            await _dBContext.SaveChangesAsync();*/
        }

        public async Task FaidInterview(Guid taskId) //TODO поменять
        {
            /*(await _dBContext.Tasks
                .FindAsync(taskId))
                .Status = true;

            await _dBContext.SaveChangesAsync();*/
        }

        public async Task Update(Guid taskId, InterviewCreateRequest newTask)
        {
            /*Tasks task = await GetById(taskId);
            await _candidateService.Update(task.Candidate, newTask.Candidate);

            task.DateTime = DateTime.Parse(newTask.InterviewDate);
            task.TaskType = await _taskType.GetByName("Интервью");

            // Начинаем транзакцию
            using (var transaction = _dBContext.Database.BeginTransaction())
            {
                try
                {
                    _dBContext.Tasks.Update(task);
                    await _dBContext.SaveChangesAsync();

                    // Подтверждаем транзакцию
                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();
                    }
                    // Откатываем транзакцию в случае ошибки
                    transaction.Rollback();
                }
            }*/

        }


        public async Task RepeatInterview(Guid taskId, string dateTime)
        {
           /*Tasks task = await GetById(taskId);
            task.Status = true;

            Tasks repeatTask = new Tasks()
            {
                TasksId = Guid.NewGuid(),
                Candidate = task.Candidate,
                TaskType = task.TaskType,
                DateTime = DateTime.Parse(dateTime),
                Status = false,
                User = task.User
            };

            await _dBContext.Tasks.AddAsync(repeatTask);
            await _dBContext.SaveChangesAsync();*/
        }

        public async Task<TasksStatus> GetStatusByName(string name)
        {
            return _dBContext.TasksStatus.Where(t => t.Name == name).FirstOrDefault();
        }

        public async Task SetNewStatus(Tasks task, string status)
        {
            task.Status = await GetStatusByName(status);
            _dBContext.Tasks.Update(task);
            _dBContext.SaveChanges();
        }
    }
}

