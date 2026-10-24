using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_Process")]
    public class Process
    {
        [Key]
        public long Id { get; set; }
        public ProcessType ProcessType { get; set; }
        public long ProcessTypeKeyId { get; set; }
        public Nullable<DateTime> ProcessStartAt { get; set; }
        public Nullable<DateTime> ProcessEndAt { get; set; }
        public Nullable<DateTime> LastRunOn { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        public long Scheduler_SchedulerId { get; set; } 

        [ForeignKey("Scheduler_SchedulerId")]
        public virtual Scheduler Scheduler { get; set; }
    }
}
