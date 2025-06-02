using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;

namespace YavaPrimum.Core.Interfaces
{
    public interface INotificationsService 
    {
        Task<List<NotificationsResponse>> GetAllByUserId(Guid id);
        Task<Notifications> GetById(Guid id);
        Task ReadNotification(Guid id);
        Task ReadNotificationOfCandidate(Guid candidateId);
        Task SendCountryRecruiterNotifications(Tasks task);
        Task SendMessage(Tasks task);
        string GetMessageText(Tasks task);
        Task<string> SendMessageToEmail(string email, string myMessage, string subject);
        string GetTextMessageForTestTask(Tasks task);
        Task ReadNotificationOfTask(Guid taskId);
        Task SendMessageToChangeDataOrTime(Tasks task, bool isDate, bool isHr = true);
        Task ReadAllNotificationOfCandidate(Guid candidateId);
    }
}
