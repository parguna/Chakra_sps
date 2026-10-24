using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_ProcessHistroy")]
    public class ProcessHistory : EntityBase ,ICreatable,IUpdatable
    {
        public ProcessType ProcessType { get; set; }

        public long ProcessId { get; set; }

        public Nullable<DateTime> ProcessStartAt { get; set; }

        public Nullable<DateTime> ProcessEndAt { get; set; }

        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("ProcessId")]
        public Process Process { get; set; } 
    }
}
