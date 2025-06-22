using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YavaPrimum.Core.DataBase.Models
{
    public class Tasks
    {
        [Key]
        public Guid TasksId { get; set; }

        [ForeignKey("TasksStatusId")]
        public Guid TasksStatusId { get; set; }
        public TasksStatus Status { get; set; }

        public DateTime DateTime { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("CandidateId")]
        public Guid CandidateId { get; set; }
        public Candidate Candidate { get; set; }

        public bool IsArchive { get; set; } = false;

        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")] // Исправлено на nvarchar в соответствии с базой данных
        public string? AdditionalData { get; set; }
    }
}
