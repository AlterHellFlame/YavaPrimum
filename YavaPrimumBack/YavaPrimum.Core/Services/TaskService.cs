using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Xml.Linq;
using System.Globalization;

namespace YavaPrimum.Core.Services
{
    public class TaskService : ITasksService
    {
        private readonly YavaPrimumDBContext _dBContext;
        private readonly INotificationsService _notificationsService;
        private readonly ICandidateService _candidateService;
        private readonly IUserService _userService;

        public TaskService(YavaPrimumDBContext dBContext, 
            INotificationsService notificationsService, 
            ICandidateService candidateService, IUserService userService)
        {
            _dBContext = dBContext;
            _notificationsService = notificationsService;
            _candidateService = candidateService;
            _userService = userService;
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

        public async Task<TasksStatus> GetStatusByName(string name)
        {
            return _dBContext.TasksStatus.Where(t => t.Name == name).First();
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

        public async Task SetNewStatus(Tasks task, StatusUpdateRequest updateRequest = null, string status = null)
        {
            if(updateRequest != null) status = updateRequest.Status;

            Console.WriteLine($"{task.Candidate.Surname} - {status}");

            var taskStatus = await GetStatusByName(status);
            if (task.Status == taskStatus) return;

            
            switch (task.Status.TypeStatus)
            {

                case -1: // Подтверждения
                    await HandleStatusMinusOne(task, taskStatus);
                    break;
                case 0: // Текущие задачи
                    await HandleStatusZero(task, taskStatus, updateRequest);//Собес назначен, Срок тестового назначен, Приём назначен
                    break;

                case 2: // Подтверждения
                    await HandleStatusTwo(task, taskStatus);
                    break;

                case 3: // Подтверждения
                    await HandleStatusThree(task, taskStatus);
                    break;

                default:
                    await HandleOtherStatuses(task, taskStatus);
                    break;
            }

            if (taskStatus.MessageTemplate != null && taskStatus.Name != "Собеседование пройдено" && taskStatus.Name != "Выполнено тестовое задание")
            {
                if (updateRequest.AdditionalData != null) task.AdditionalData = updateRequest.AdditionalData;
                await _notificationsService.SendMessage(task);
            }

            await _dBContext.SaveChangesAsync();
        }

        private async Task HandleStatusZero(Tasks task, TasksStatus status, StatusUpdateRequest updateRequest)
        {
            task.Status = status;
            if (status.Name == "Собеседование пройдено"  || status.Name ==  "Выполнено тестовое задание")
            {

                if (updateRequest.IsTestTask)//Если нужно тестовое задание
                {
                    var testTask = new Tasks
                    {
                        TasksId = Guid.NewGuid(),
                        Candidate = task.Candidate,
                        DateTime = Convert.ToDateTime(updateRequest.NewDateTime),
                        Status = await GetStatusByName("Срок тестового задания"),
                        User = task.User,
                        AdditionalData = updateRequest.AdditionalData
                    };

                    task.IsArchive = true;

                    string res = await _notificationsService.SendMessageToEmail(task.Candidate.Email, _notificationsService.GetTextMessageForTestTask(task), "Тестовое задание Primum");

                    await Create(testTask);

                }
                else
                {
                    if (updateRequest.AdditionalData != null) task.AdditionalData = updateRequest.AdditionalData;
                    await _notificationsService.SendCountryRecruiterNotifications(task);
                }
            }
        }

        private async Task HandleStatusTwo(Tasks task, TasksStatus status)
        {
            task.IsArchive = true;

            Tasks newTask = new Tasks()
            {
                TasksId = new Guid(),
                Candidate = task.Candidate,
                DateTime = DateTime.Now,
                Status = status,
                User = task.User,
                AdditionalData = task.AdditionalData
            };
            await Create(task);
        }

        private async Task HandleStatusMinusOne(Tasks task, TasksStatus status)
        {
            task.Status = status;

            if (status.Name == "Время подтверждено")
            {
                var kadrTask = await _dBContext.Tasks
                    .Include(t => t.User)
                    .Include(t => t.User.Post)
                    .Include(t => t.Status)
                    .FirstOrDefaultAsync(t => t.Candidate == task.Candidate
                                           && t.User.Post.Name == "Кадровик"
                                           && !t.IsArchive);

                if (kadrTask != null)
                {
                    StatusUpdateRequest statusUpdateRequest = new StatusUpdateRequest
                    {
                        Status = "Назначен приём"
                    };
                    await SetNewStatus(kadrTask, statusUpdateRequest);
                }
            }
            else if (status.Name == "Дата подтверждена")
            {
                var kadrTask = await _dBContext.Tasks
                    .Include(t => t.User)
                    .Include(t => t.User.Post)
                    .Include(t => t.Status)
                    .FirstOrDefaultAsync(t => t.Candidate == task.Candidate
                                           && t.User.Post.Name == "Кадровик"
                                           && !t.IsArchive);
            }
        }
        private async Task HandleStatusThree(Tasks task, TasksStatus status)
        {
            task.Status = status;

            if (status.Name == "Время подтверждено")
            {
                var kadrTask = await _dBContext.Tasks
                    .Include(t => t.User)
                    .Include(t => t.User.Post)
                    .Include(t => t.Status)
                    .FirstOrDefaultAsync(t => t.Candidate == task.Candidate
                                           && t.User.Post.Name == "Кадровик"
                                           && !t.IsArchive);

                if (kadrTask != null)
                {
                    StatusUpdateRequest statusUpdateRequest = new StatusUpdateRequest
                    {
                        Status = "Назначен приём"
                    };
                    await SetNewStatus(kadrTask, statusUpdateRequest);
                }
            }
        }

        private async Task HandleOtherStatuses(Tasks task, TasksStatus status)
        {
            task.IsArchive = true;

            var newTask = new Tasks
            {
                TasksId = Guid.NewGuid(),
                Candidate = task.Candidate,
                DateTime = DateTime.Now,
                Status = status,
                User = task.User,
                AdditionalData = task.AdditionalData
            };

            await Create(newTask);
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

        public async Task ChangeTime(Guid taskId, ChangeDateTimeRequest changeRequest)
        {
            
            Tasks task = await GetById(taskId);
            //task.IsArchive = true;

            Tasks newtask = new Tasks()
            {
                TasksId = Guid.NewGuid(),
                Candidate = task.Candidate,
                Status = await GetStatusByName(changeRequest.IsChangeDate ? "Запрос на смену даты" : "Запрос на смену времени"),
                DateTime = DateTime.Now,
                User = task.User,
                AdditionalData = changeRequest.AdditionalData
            };

            Tasks oldTask = await GetLastActiveTask(task.Candidate.CandidateId, task.User.UserId);

            Tasks kadrTask = await _dBContext.Tasks
                .Include(t => t.Status)
                .Include(t => t.User)
                .ThenInclude(u => u.Post)
                .Include(t => t.User)
                .ThenInclude(u => u.Company)
                .ThenInclude(c => c.Country)
                .Include(t => t.Candidate)
                .ThenInclude(c => c.Country)
                .Include(t => t.Candidate)
                .ThenInclude(c => c.Post)
                .Where(t => !t.IsArchive && t.User.Post.Name == "Кадровик" && t.Candidate.CandidateId == task.Candidate.CandidateId)
                .FirstAsync();


            kadrTask.Status = await GetStatusByName(changeRequest.IsChangeDate ? "Ожидается подтверждение даты" : "Ожидается подтверждение времени");

            await _notificationsService.ReadAllNotificationOfCandidate(task.Candidate.CandidateId);

            await Create(newtask);
                        oldTask.IsArchive = true;
            await _notificationsService.SendMessageToChangeDataOrTime(newtask, changeRequest.IsChangeDate);

            _dBContext.SaveChanges();

        }

        public async Task ChangeTimeWithoutCheck(Guid taskId, DateTime dateTime)
        {

            Tasks task = await GetById(taskId);

            task.DateTime = dateTime;
            await _dBContext.SaveChangesAsync();

        }


    }
}