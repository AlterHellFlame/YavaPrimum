using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YavaPrimum.Core.DataBase.Models
{
    public class Vacancy
    {
        [Key]
        public Guid VacancyId { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("PostId")]
        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public byte Count { get; set; } // Исправлено на `byte`, так как в БД `tinyint`

        public bool isClose { get; set; }

        [MaxLength(250)]
        [Column(TypeName = "varchar(250)")] // В базе `varchar`
        public string AdditionalData { get; set; }
    }
}
