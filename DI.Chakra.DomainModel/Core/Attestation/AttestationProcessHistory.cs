using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class AttestationProcessHistory : DomainModelBase<AttestationProcessHistory, EntityModel.AttestationProcessHistory>
    {
        public Nullable<long> AttestationId { get; set; }

        public Nullable<System.DateTime> ProcessStartAt { get; set; }

        public Nullable<System.DateTime> ProcessEndAt { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public string WorkflowStatus { get; set; }

        public Nullable<int> HistoryId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Attestation Attestation { get; set; }
    }
}
