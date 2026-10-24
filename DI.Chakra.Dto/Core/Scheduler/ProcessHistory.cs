using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class ProcessHistory : DtoBase 
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

        public Process Process { get; set; } 
    }
}
