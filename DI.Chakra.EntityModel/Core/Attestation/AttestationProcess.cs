using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_AttestationProcess")]
    public class AttestationProcess : EntityBase, ICreatable, IUpdatable
    {
        public string ProcessName { get; set; }

        public string Description { get; set; }

        public Nullable<long> AttestationId { get; set; }

        public Nullable<System.DateTime> ProcessStartAt { get; set; }

        public Nullable<System.DateTime> ProcessEndAt { get; set; }

        public string WorkflowStatus { get; set; }

        public Nullable<int> HistoryId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public  Attestation Attestation { get; set; }

        public Process process { get; set; }
    }
}
