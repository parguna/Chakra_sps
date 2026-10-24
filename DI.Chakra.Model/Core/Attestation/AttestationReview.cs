using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DI.Chakra.Model
{
    [Table("DI_M_AttestationReview")]
    [Serializable]
    public class AttestationReview : IAggregateRoot
    {
        public AttestationReview()
        {
            this.ReviewEndpointSystems = new List<ReviewEndpointSystem>();
        }

        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Comments { get; set; }
        public string SubmittedBy { get; set; }
        public Nullable<System.DateTime> SubmittedDate { get; set; }        
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string ActionId { get; set; }
        public string ActionTakenBy { get; set; }
        public string ActionComments { get; set; }
        public int HistoryId { get; set; }
        public bool IsChanged { get; set; }

        public AttestationRemediationStatus RemediationStatus { get; set; }

        public long ReviewerId;
        public bool IsGroupTask;

        public virtual List<ReviewEndpointSystem> ReviewEndpointSystems { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
