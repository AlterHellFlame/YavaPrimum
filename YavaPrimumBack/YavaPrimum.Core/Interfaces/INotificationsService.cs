using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DTO;

namespace YavaPrimum.Core.Interfaces
{
    public interface INotificationsService 
    {
        Task<List<NotificationsResponse>> GetNotificatonsByUserId(Guid id);
        Task ReadNotification(Guid id);
    }
}
