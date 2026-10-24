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
    [Table("DI_M_ProcessHistroy")]
    public class ProcessHistory
    {
        [Key]
        public long Id { get; set; }
        public ProcessType ProcessType { get; set; }
        public long ProcessId { get; set; }
        public Nullable<DateTime> ProcessStartAt { get; set; }
        public Nullable<DateTime> ProcessEndAt { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("ProcessId")]
        public virtual Process Process { get; set; } 
    }
   
}
