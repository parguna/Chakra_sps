using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_AttestationProcessHistory")]
    public class AttestationProcessHistory : IAggregateRoot
    {
        public AttestationProcessHistory()
        {
            //this.Schedulers = new List< Scheduler>();
        }

        [Key]
        public long Id { get; set; }
        public Nullable<long> AttestationId { get; set; }
        public Nullable<System.DateTime> ProcessStartAt { get; set; }
        public Nullable<System.DateTime> ProcessEndAt { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string WorkflowStatus { get; set; }
        public Nullable<int> HistoryId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        [ForeignKey("AttestationId")]
        public virtual Attestation Attestation { get; set; }
        //public virtual List< Scheduler> Schedulers { get; set; }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }
}
