namespace YavaPrimum.Core.DataBase.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Tasks
    {
        [Key]
        public Guid TasksId { get; set; }

        public TasksStatus Status { get; set; }
        public DateTime DateTime { get; set; }
        public User User { get; set; }
        public Candidate Candidate { get; set; }

        public bool IsArchive { get; set; } = false;

        [MaxLength(500)] // Ограничиваем до 500 символов
        [Column(TypeName = "varchar(500)")] // Указываем тип в БД
        public string? AdditionalData { get; set; }
    }

}
