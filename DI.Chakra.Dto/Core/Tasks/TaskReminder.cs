using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class TaskReminder : DtoBase
    {
        public long TaskId { get; set; }
        public DI.Chakra.Infrastructure.Enums.RequestType RequestType { get; set; }
        public int ReminderCount { get; set; }
        public bool IsEsclation { get; set; } = false;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Task Task { get; set; }
    }
}
