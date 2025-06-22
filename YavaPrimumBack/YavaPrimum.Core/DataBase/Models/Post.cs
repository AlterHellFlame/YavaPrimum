using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YavaPrimum.Core.DataBase.Models
{
    public class Post
    {
        [Key]
        public Guid PostId { get; set; }

        [MaxLength(30)]
        [Column(TypeName = "nvarchar(30)")]
        public string Name { get; set; }
    }
}
