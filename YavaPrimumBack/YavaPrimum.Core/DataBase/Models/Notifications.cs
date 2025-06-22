using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YavaPrimum.Core.DataBase.Models
{
    public class Notifications
    {
        [Key]
        public Guid NotificationsId { get; set; }

        [ForeignKey("TasksId")]
        public Guid TasksId { get; set; }
        public Tasks Task { get; set; }

        [ForeignKey("RecipientUserId")]
        public Guid RecipientUserId { get; set; }
        public User Recipient { get; set; }

        [MaxLength(250)]
        [Column(TypeName = "nvarchar(250)")]
        public string TextMessage { get; set; }

        public DateTime DateTimeOfNotify { get; set; }
        public bool IsReaded { get; set; } = false;
    }
}
