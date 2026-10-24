using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_TaskReminder")]
    public class TaskReminder : EntityBase, ICreatable, IUpdatable
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
        [ForeignKey("TaskId")]
        public Task Task { get; set; }
    }
}
