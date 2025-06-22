using System.ComponentModel.DataAnnotations;

namespace YavaPrimum.Core.DTO
{
    public class VacancyRequest
    {
        [MaxLength(30)]
        public string Post { get; set; }

        public byte Count { get; set; } // Исправлено на `byte`, так как в БД `tinyint`

        public string AdditionalData { get; set; }
    }
}
