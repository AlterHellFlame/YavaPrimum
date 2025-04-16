using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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
                .Include(t => t.User.Post)
                .Include(t => t.User.Company)
                .Include(t => t.User.Company.Country)
                .Include(t => t.Candidate)
                .Include(t => t.Candidate.Country)
                .Include(t => t.CandidatePost)
                .ToListAsync();
        }

        public async Task<List<ArchiveTasks>> GetAllAcrhive()
        {
            return await _dBContext.ArchiveTasks
                .Include(t => t.Status)
                .Include(t => t.Task)
                .Include(t => t.Task.User)
                .Include(t => t.Task.User.Post)
                .Include(t => t.Task.User.Company)
                .Include(t => t.Task.User.Company.Country)
                .Include(t => t.Task.Candidate)
                .Include(t => t.Task.Candidate.Country)
                .Include(t => t.Task.CandidatePost)
                .Where(t => t.Status.TypeStatus != -1)
                .ToListAsync();
        }


        public async Task<List<ArchiveTasks>> GetAllAcrhiveByUserId(Guid userId)
        {
            return await _dBContext.ArchiveTasks
                .Include(t => t.Status)
                .Include(t => t.Task)
                .Include(t => t.Task.User)
                .Include(t => t.Task.User.Post)
                .Include(t => t.Task.User.Company)
                .Include(t => t.Task.User.Company.Country)
                .Include(t => t.Task.Candidate)
                .Include(t => t.Task.Candidate.Country)
                .Include(t => t.Task.CandidatePost)
                .Where(t => t.Task.User.UserId == userId && t.Status.TypeStatus != -1)
                .ToListAsync();
        }

        public async Task<Tasks> GetById(Guid taskId)
        {
            return await _dBContext.Tasks
                .Include(t => t.Status)
                .Include(t => t.User)
                .Include(t => t.User.Post)
                .Include(t => t.User.Company)
                .Include(t => t.User.Company.Country)
                .Include(t => t.Candidate)
                .Include(t => t.Candidate.Country)
                .Include(t => t.CandidatePost)
                .FirstOrDefaultAsync(t => t.TasksId == taskId);
        }

        public async Task<List<Tasks>> GetAllByUserId(Guid userId)
        {
            return await _dBContext.Tasks
                .Include(t => t.Status)
                .Include(t => t.User)
                .Include(t => t.User.Post)
                .Include(t => t.User.Company)
                .Include(t => t.User.Company.Country)
                .Include(t => t.Candidate)
                .Include(t => t.Candidate.Country)
                .Include(t => t.CandidatePost)
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
                TypeStatus = task.Status.TypeStatus,
                DateTime = task.DateTime,
                User = new UserRequestResponse()
                {
                    UserId = task.User.UserId,
                    Surname = task.User.Surname,
                    FirstName = task.User.FirstName,
                    Patronymic = task.User.Patronymic,
                    Post = task.User.Post.Name,
                    Company = task.User.Company.Name,
                    Email = task.User.Email,
                    Phone = task.User.Phone,
                    ImgUrl = task.User.ImgUrl
                },
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

        public async Task<List<TasksResponse>> ConvertArchiveToFront(List<ArchiveTasks> tasks)
        {
            List<Task<TasksResponse>> taskResponseTasks = tasks.Select(task => ConvertArchiveToFront(task)).ToList();
            TasksResponse[] taskResponses = await Task.WhenAll(taskResponseTasks);
            return taskResponses.ToList();
        }

        public async Task<TasksResponse> ConvertArchiveToFront(ArchiveTasks task)
        {
            TasksResponse tasksResponse = new TasksResponse
            {
                TaskId = task.ArchiveTasksId,
                Status = task.Status.Name,
                TypeStatus = task.Status.TypeStatus,
                DateTime = task.DateTimeOfCreated,
                User = new UserRequestResponse()
                {
                    UserId = task.Task.User.UserId,
                    Surname = task.Task.User.Surname,
                    FirstName = task.Task.User.FirstName,
                    Patronymic = task.Task.User.Patronymic,
                    Post = task.Task.User.Post.Name,
                    Company = task.Task.User.Company.Name,
                    Email = task.Task.User.Email,
                    Phone = task.Task.User.Phone,
                    ImgUrl = task.Task.User.ImgUrl
                },
                Candidate = new CandidateRequestResponse
                {
                    Surname = task.Task.Candidate.Surname,
                    FirstName = task.Task.Candidate.FirstName,
                    Patronymic = task.Task.Candidate.Patronymic,
                    Email = task.Task.Candidate.Email,
                    Phone = task.Task.Candidate.Phone,
                    Country = task.Task.Candidate.Country.Name
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
            await _dBContext.SaveChangesAsync();


            await HandleTaskInsertionAsync(task);
        }

        public async Task HandleTaskInsertionAsync(Tasks insertedTask)
        {
            // Проверка входного параметра
            if (insertedTask == null)
                throw new ArgumentNullException(nameof(insertedTask));

            // Добавление записи в таблицу ArchiveTasks
            var archiveTask = new ArchiveTasks
            {
                ArchiveTasksId = Guid.NewGuid(),
                Task = insertedTask,
                Status = insertedTask.Status,
                DateTimeOfCreated = DateTime.Now //Абоба
            };
            await _dBContext.ArchiveTasks.AddAsync(archiveTask);
            await _dBContext.SaveChangesAsync();

            // Получение имени нового статуса задачи
            var newStatusName = insertedTask.Status?.Name;
            if (string.IsNullOrWhiteSpace(newStatusName))
                throw new InvalidOperationException("Имя статуса задачи отсутствует или некорректно.");

            Console.WriteLine($"{newStatusName} Новый статус");
            Console.WriteLine($"{insertedTask.TasksId} Кому назначаем");

            // Логика обработки статуса "Взят кандидат"
            if (newStatusName == "Взят кандидат")
            {
                // Пометка соответствующих уведомлений как прочитанных
                await MarkNotificationsAsReadAsync(insertedTask.Candidate.CandidateId);

                // Получение идентификатора статуса "Назначен приём"
                var meetingStatus = await _dBContext.TasksStatus
                    .Where(ts => ts.Name == "Назначен приём")
                    .FirstOrDefaultAsync();

                if (meetingStatus == null)
                    throw new InvalidOperationException("Статус 'Назначен приём' не найден.");

                Console.WriteLine($"{meetingStatus.TasksStatusId} Статус ID");

                // Установка даты встречи через неделю
                var meetingDate = DateTime.Now.AddDays(7);
                Console.WriteLine($"{meetingDate} Дата встречи");

                // Обновление задачи: новый статус и дата
                insertedTask.Status = meetingStatus;
                insertedTask.DateTime = meetingDate;

                _dBContext.Tasks.Update(insertedTask);
                await _dBContext.SaveChangesAsync();
            }
        }


        private async Task MarkNotificationsAsReadAsync(Guid candidateId)
        {
            var notifications = await _dBContext.Notifications
                .Where(x => x.ArchiveTasks.Task.Candidate.CandidateId == candidateId &&
                            x.ArchiveTasks.Task.Status.Name == "Собеседование пройдено")
                .ToListAsync(); // Directly retrieve the notification objects

            foreach (var notification in notifications)
            {
                notification.IsReaded = true;
            }

            _dBContext.Notifications.UpdateRange(notifications);
            await _dBContext.SaveChangesAsync();


            foreach (var notification in notifications)
            {
                notification.IsReaded = true;
            }

            _dBContext.Notifications.UpdateRange(notifications);
            await _dBContext.SaveChangesAsync();
        }


    }
}

