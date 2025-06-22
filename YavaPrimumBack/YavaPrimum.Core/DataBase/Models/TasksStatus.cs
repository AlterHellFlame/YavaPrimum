using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YavaPrimum.Core.DataBase.Models
{
    public class TasksStatus
    {
        [Key]
        public Guid TasksStatusId { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        public byte TypeStatus { get; set; }

        [MaxLength(250)]
        [Column(TypeName = "nvarchar(250)")]
        public string? MessageTemplate { get; set; }
    }
}
