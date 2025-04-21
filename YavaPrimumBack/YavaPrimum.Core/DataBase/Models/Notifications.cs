namespace YavaPrimum.Core.DataBase.Models
{
    public class Notifications
    {
        public Guid NotificationsId { get; set; }
        public Tasks Task { get; set; }
        public User Recipient { get; set; }
        public string TextMessage { get; set; }
        public DateTime DateTimeOfNotify { get; set; }
        public bool IsReaded { get; set; } = false;
    }
}
