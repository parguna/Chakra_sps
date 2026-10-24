using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class Process : DtoBase
    {

        public ProcessType ProcessType { get; set; }

        public long ProcessTypeKeyId { get; set; }

        public Nullable<DateTime> ProcessStartAt { get; set; }

        public Nullable<DateTime> ProcessEndAt { get; set; }

        public Nullable<DateTime> LastRunOn { get; set; }

        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public long Scheduler_SchedulerId { get; set; }

        public Scheduler Scheduler { get; set; }

        public int DiscoveryInstance { get; set; }
    }
}
