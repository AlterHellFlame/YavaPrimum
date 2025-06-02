using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YavaPrimum.Core.DTO
{
    public class ChangeDateTimeRequest
    {
        public Guid TaskId { get; set; } // ✅ Идентификатор задачи
        public bool IsChangeDate { get; set; } // ✅ Флаг изменения даты
        public string? AdditionalData { get; set; } // ✅ Дополнительная информация (опционально)
    }

}
