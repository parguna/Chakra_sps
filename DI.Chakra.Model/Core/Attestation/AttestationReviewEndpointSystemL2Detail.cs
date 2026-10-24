using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    [Table("DI_M_AttestationReview_EndpointSystem_L2Detail")]
    [Serializable]
    public class AttestationReviewEndpointSystemL2Detail : IAggregateRoot
    {
        public AttestationReviewEndpointSystemL2Detail()
        {
            this.AttestationReviewEndpointSystemL3Details = new List<AttestationReviewEndpointSystemL3Detail>();
        }

        [Key]
        public long Id { get; set; }
        public long ReviewEndpointId { get; set; }
        public EntitlementType AccessType { get; set; }
        public string AccessName { get; set; }
        public string AccessDescription { get; set; }
        public string ResourceType { get; set; }
        public string ResourceCompletePath { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public string Comments { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public bool IsChanged { get; set; }
        public string PreviousAccess { get; set; }

        [XmlIgnore]
        [ForeignKey("ReviewEndpointId")]
        protected virtual AttestationReviewEndpointSystem Attestation { get; set; }

        public virtual List<AttestationReviewEndpointSystemL3Detail> AttestationReviewEndpointSystemL3Details { get; set; }
    }
}
