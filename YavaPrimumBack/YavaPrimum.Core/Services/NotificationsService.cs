using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class NotificationsService: INotificationsService
    {
        private readonly YavaPrimumDBContext _dbContext;
        private readonly ITasksService _tasksService;
        private readonly IUserService _userService;

        public NotificationsService(YavaPrimumDBContext dbContext, ITasksService tasksService, IUserService userService)
        {
            _dbContext = dbContext;
            _tasksService = tasksService;
            _userService = userService;
        }

        public async Task<Notifications> GetById(Guid id)
        {

            Notifications n = await _dbContext.Notifications
                .Include(t => t.ArchiveTasks)
                 .Include(t => t.ArchiveTasks.Status)
                .Include(c => c.ArchiveTasks.Task.Candidate)
                .Include(c => c.ArchiveTasks.Task.Candidate.Country)
                .Include(c => c.ArchiveTasks.Task.Candidate)
                .Include(u => u.ArchiveTasks.Task.CandidatePost)
                .Include(u => u.ArchiveTasks.Task.User)
                .Include(u => u.ArchiveTasks.Task.User.Company)
                .Include(u => u.ArchiveTasks.Task.User.Company.Country)
                .Include(u => u.ArchiveTasks.Task.User.Post)
                .Include(u => u.ArchiveTasks.Task.Status)
                .Include(u => u.ArchiveTasks.Task)
                .Include(r => r.Recipient)
                .Include(r => r.Recipient.Post)
                .Include(r => r.Recipient.Company)
                .Include(r => r.Recipient.Company.Country)
                .Where(r => r.NotificationsId == id)
                .FirstOrDefaultAsync();

            return n;
        }

        public async Task<List<NotificationsResponse>> GetAllByUserId(Guid id)
        {
            List<Notifications> notifications = await _dbContext.Notifications
                .Include(t => t.ArchiveTasks)
                .Include(t => t.ArchiveTasks.Status)
                .Include(c => c.ArchiveTasks.Task.Candidate)
                .Include(c => c.ArchiveTasks.Task.Candidate.Country)
                .Include(c => c.ArchiveTasks.Task.Candidate)
                .Include(u => u.ArchiveTasks.Task.CandidatePost)
                .Include(u => u.ArchiveTasks.Task.User)
                .Include(u => u.ArchiveTasks.Task.User.Company)
                .Include(u => u.ArchiveTasks.Task.User.Company.Country)
                .Include(u => u.ArchiveTasks.Task.User.Post)
                .Include(u => u.ArchiveTasks.Task.Status)
                .Include(u => u.ArchiveTasks.Task)
                .Include(r => r.Recipient)
                .Include(r => r.Recipient.Post)
                .Include(r => r.Recipient.Company)
                .Include(r => r.Recipient.Company.Country)
                .Where(r => r.Recipient.UserId == id)
                .OrderByDescending(n => n.ArchiveTasks.DateTimeOfCreated) // Сортировка по дате создания (от новой к старой)
                .ToListAsync();

            List<NotificationsResponse> notificationsResponse = new List<NotificationsResponse>();

            foreach (var notification in notifications)
            {
                notificationsResponse.Add(new NotificationsResponse
                {
                    NotificationsId = notification.NotificationsId,
                    Task = await _tasksService.ConvertToFront(task: notification.ArchiveTasks.Task),
                    DateTime = notification.ArchiveTasks.DateTimeOfCreated,
                    IsReaded = notification.IsReaded,
                    TextMessage = notification.TextMessage,
                    Recipient = await _userService.ConvertToFront(notification.Recipient),
                    Status = notification.ArchiveTasks.Status.Name,
                });
            }

            if (notificationsResponse.Count > 0)
            {
                Console.WriteLine("Уведомлений у " + notificationsResponse[0].Recipient.Surname + " : " + notificationsResponse.Count);
            }
            else
            {
                Console.WriteLine("Уведомлений нет.");
            }

            return notificationsResponse;
        }


        public async Task ReadNotification(Guid id)
        {
            Notifications notification = await GetById(id);
            Console.WriteLine(notification.ToString);
            notification.IsReaded = true;

            _dbContext.Update(notification);
            _dbContext.SaveChanges();

        }
    }
}
