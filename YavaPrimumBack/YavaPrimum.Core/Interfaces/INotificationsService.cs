using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
