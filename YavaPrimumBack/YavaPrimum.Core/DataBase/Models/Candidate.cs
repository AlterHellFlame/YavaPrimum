using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YavaPrimum.Core.DataBase.Models
{
    public class Candidate
    {
        [Key]
        public Guid CandidateId { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Surname { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Patronymic { get; set; } // Отчество

        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Phone { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        public Post Post { get; set; }
        public Country Country { get; set; }
    }
}
