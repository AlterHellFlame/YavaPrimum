using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DTO
{
    public class NotificationsResponse
    {
        public Guid NotificationsId { get; set; }
        public TasksResponse Task { get; set; }
        public UserRequestResponse Recipient { get; set; }
        public DateTime DateTime { get; set; }
        public string TextMessage { get; set; }
        public bool IsReaded { get; set; }
        public string Status { get; set; }
    }
}
