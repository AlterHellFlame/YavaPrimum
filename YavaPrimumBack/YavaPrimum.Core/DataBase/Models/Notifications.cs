namespace YavaPrimum.Core.DataBase.Models
{
    public class Notifications
    {
        public Guid NotificationsId { get; set; }
        public ArchiveTasks ArchiveTasks { get; set; }
        public User Recipient { get; set; }
        public string TextMessage { get; set; }
        public bool IsReaded { get; set; } = false;
    }
}
