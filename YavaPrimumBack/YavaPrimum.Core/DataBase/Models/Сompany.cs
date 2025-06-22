using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YavaPrimum.Core.DataBase.Models
{
    public class Company
    {
        [Key]
        public Guid CompanyId { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [ForeignKey("CountryId")]
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
    }
}
