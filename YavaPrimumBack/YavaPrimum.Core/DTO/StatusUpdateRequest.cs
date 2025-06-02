using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YavaPrimum.Core.DTO
{
    // Новая модель запроса
    public class StatusUpdateRequest
    {
        public string Status { get; set; }
        public string? AdditionalData { get; set; }
        public string NewDateTime { get; set; }
        public bool IsTestTask { get; set; } = false;
    }
}
