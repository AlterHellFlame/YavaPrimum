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
        private readonly INotificationsService _notificationsService;
        private readonly ICandidateService _candidateService;

        public TaskService(YavaPrimumDBContext dBContext,
            INotificationsService notificationsService,
            ICandidateService candidateService)
        {
            _dBContext = dBContext;
            _notificationsService = notificationsService;
            _candidateService = candidateService;
        }

        public async Task<Guid> Create(Tasks task)
        {
            await _dBContext.Tasks.AddAsync(task);
            await _dBContext.SaveChangesAsync();


            /*if (task.Status.Name == "Взят кандидат")
            {
                // Пометка соответствующих уведомлений как прочитанных
                //await _notificationsService.ReadNotificationOfCandidate(task.Candidate.CandidateId);

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
                task.Status = meetingStatus;
                task.DateTime = meetingDate;

                _dBContext.Tasks.Update(task);
                await _dBContext.SaveChangesAsync();
            }*/

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
                .Include(t => t.Candidate.Post)
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
                .Include(t => t.Candidate.Post)
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
                .Include(t => t.Candidate.Post)
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


        public async Task SetActive(Guid taskId)
        {
            // Получаем задачу с указанным `taskId` и её Candidate
            var task = await _dBContext.Tasks
                .Include(t => t.Candidate)
                .FirstOrDefaultAsync(t => t.TasksId == taskId);

            // Проверяем, существует ли задача и её Candidate
            if (task == null || task.Candidate == null)
            {
                throw new Exception("Задача или Candidate не найдены.");
            }

            // Находим все задачи с тем же Candidate, кроме текущей задачи
            var tasksToArchive = await _dBContext.Tasks
                .Where(t => t.Candidate.CandidateId == task.Candidate.CandidateId && t.TasksId != taskId && t.User.Post == task.User.Post)
                .ToListAsync();

            // Устанавливаем `IsArchive = true` для найденных задач
            foreach (var archiveTask in tasksToArchive)
            {
                archiveTask.IsArchive = true;
            }

            // Сохраняем изменения в базе данных
            await _dBContext.SaveChangesAsync();
        }

        public async Task SetNewDate(Tasks task, DateTime dateTime)
        {
            task.DateTime = dateTime;
            await _dBContext.SaveChangesAsync();
        }

        public async Task SetNewStatus(Tasks task, string status)
        {
            Console.WriteLine(task.Candidate.Surname + " " + status);
            TasksStatus taskStatus = await GetStatusByName(status);
            if (task.Status.TypeStatus == 0)
            {
                task.Status = taskStatus;
                if (taskStatus.Name == "Собеседование пройдено")
                {
                    await _notificationsService.SendCountryRecruiterNotifications(task);
                    await _dBContext.SaveChangesAsync();
                    return;
                }
            }
            else if (task.Status.TypeStatus == 3)
            {
                task.Status = taskStatus;
                task.DateTime = DateTime.Now;

                if (taskStatus.Name == "Время подтверждено")
                {
                    Tasks? KadrTask = _dBContext.Tasks
                         .Include(t => t.User)
                         .Include(t => t.User.Post)
                         .Include(t => t.Status)
                         .Where(t => t.Candidate == task.Candidate
                         && t.User.Post.Name == "Кадровик"
                         && t.IsArchive == false)
                         .FirstOrDefault();

                    await SetNewStatus(KadrTask, "Назначен приём");
                }
            }
            else if(task.Status.TypeStatus == 4)
            {
                task.Status = taskStatus;
            }
            else
            {
                task.IsArchive = true;

                Tasks newTask = new Tasks()
                {
                    TasksId = new Guid(),
                    Candidate = task.Candidate,
                    DateTime = DateTime.Now,
                    Status = taskStatus,
                    User = task.User,
                };
                await Create(task);

            }

            if (taskStatus.MessageTemplate != null)
            {

                await _notificationsService.SendMessage(task);
            }

            await _dBContext.SaveChangesAsync();

         }

        public async Task<Tasks> GetLastActiveTask(Guid candidateId, Guid userId)
        {

            // Находим последнюю
            var findTask = await _dBContext.Tasks
                .Include(t => t.Status)
                .Include(t => t.User)
                .Include(t => t.User.Post)
                .Include(t => t.User.Company)
                .Include(t => t.User.Company.Country)
                .Include(t => t.Candidate)
                .Include(t => t.Candidate.Country)
                .Include(t => t.Candidate.Post)
                .Where(t => t.Candidate.CandidateId == candidateId
                            && t.User.UserId == userId
                            && t.IsArchive == false)
                .FirstOrDefaultAsync();

            if (findTask == null)
            {
                throw new InvalidOperationException("Задача с подходящими критериями не найдена.");
            }

            return findTask;

        }


    }
}