using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YavaPrimum.Core.DataBase.Models
{
    public class Candidate
    {
        [Key]
        public Guid CandidateId { get; set; }

        [MaxLength(40)]
        [Column(TypeName = "nvarchar(40)")]
        public string Surname { get; set; }

        [MaxLength(40)]
        [Column(TypeName = "nvarchar(40)")]
        public string FirstName { get; set; }

        [MaxLength(40)]
        [Column(TypeName = "nvarchar(40)")]
        public string Patronymic { get; set; } // Отчество

        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string Phone { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; }

        [ForeignKey("PostId")]
        public Guid PostId { get; set; }
        public Post Post { get; set; }

        [ForeignKey("CountryId")]
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
    }
}
