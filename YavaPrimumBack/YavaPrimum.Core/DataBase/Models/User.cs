using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YavaPrimum.Core.DataBase.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [MaxLength(40)]
        [Column(TypeName = "nvarchar(40)")]
        public string Surname { get; set; }

        [MaxLength(40)]
        [Column(TypeName = "nvarchar(40)")]
        public string FirstName { get; set; }

        [MaxLength(40)]
        [Column(TypeName = "nvarchar(40)")]
        public string Patronymic { get; set; } // Отчество

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string ImgUrl { get; set; }

        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string Phone { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; }

        [MaxLength(60)]
        [Column(TypeName = "nvarchar(60)")]
        public string PasswordHash { get; set; }

        [ForeignKey("CompanyId")]
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        [ForeignKey("PostId")]
        public Guid PostId { get; set; }
        public Post Post { get; set; }
    }
}
