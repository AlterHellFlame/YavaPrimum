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

        public async Task<List<NotificationsResponse>> GetNotificatonsByUserId(Guid id)
        {
            List<Notifications> notifications = await _dbContext.Notifications
                .Include(t => t.Task)
                .Include(t => t.Status)
                .Include(c => c.Task.Candidate)
                .Include(c => c.Task.Candidate.Country)
                .Include(c => c.Task.Candidate)
                .Include(u => u.Task.User)
                .Include(u => u.Task.User.Company)
                .Include(u => u.Task.User.Company.Country)
                .Include(u => u.Task.User.Post)
                .Include(r => r.Recipient)
                .Include(r => r.Recipient.Post)
                .Include(r => r.Recipient.Company)
                .Include(r => r.Recipient.Company.Country)
                .Where(r => r.Recipient.UserId == id)
                .ToListAsync();

            List<NotificationsResponse> notificationsResponse = new List<NotificationsResponse>();

            foreach (var notification in notifications)
            {
                notificationsResponse.Add(new NotificationsResponse
                {
                    Task = await _tasksService.ConvertToFront(notification.Task),
                    DateTime = notification.DateTime,
                    IsReaded = notification.IsReaded,
                    TextMessage = notification.TextMessage,
                    Recipient = await _userService.ConvertToFront(notification.Recipient)
                });
            }
            if (notificationsResponse != null)
            {
                Console.WriteLine("Уведомлений у" + notificationsResponse[0].Recipient.Surname + " : " + notificationsResponse.Count);
            }
            return notificationsResponse;
        }

        public async Task ReadNotification(Guid id)
        {
            Notifications notification = await _dbContext.Notifications
            /*.Include(t => t.Task)
            .Include(t => t.Status)
            .Include(c => c.Task.Candidate)
            .Include(c => c.Task.Candidate.Country)
            .Include(c => c.Task.Candidate)
            .Include(r => r.Recipient)
            .Include(r => r.Recipient.Post)
            .Include(r => r.Recipient.Company)
            .Include(r => r.Recipient.Company.Country)*/
            .Where(r => r.NotificationsId == id)
            .FirstOrDefaultAsync();

            notification.IsReaded = true;

            _dbContext.Update(notification);
            _dbContext.SaveChanges();

        }
    }
}
