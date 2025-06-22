using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YavaPrimum.Core.DataBase.Models
{
    public class Country
    {
        [Key]
        public Guid CountryId { get; set; }

        [MaxLength(30)]
        [Column(TypeName = "nvarchar(30)")]
        public string Name { get; set; }

        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string PhoneMask { get; set; }
    }
}
