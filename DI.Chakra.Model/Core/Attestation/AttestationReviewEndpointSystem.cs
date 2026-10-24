using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_M_AttestationReview_EndpointSystem")]
    [Serializable]
    public class AttestationReviewEndpointSystem : IAggregateRoot
    {

        public AttestationReviewEndpointSystem()
        {
            this.AttestationReviewEndpointSystemL2Details = new List<AttestationReviewEndpointSystemL2Detail>();
        }

        [Key]
        public long Id { get; set; }
        public long EndpointUserId { get; set; }
        public string EndpointUserName { get; set; }
        public AccountType AccountType { get; set; }
        public bool IsActive { get; set; }
        public long EndpointSystemId { get; set; }
        public string HostName { get; set; }
        public long EndpointSystemTypeId { get; set; }
        public string EndpointSystemTypeName { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public string Comments { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public bool IsCurrentLevelApproved { get; set; }
        public bool IsChanged { get; set; }
        public string PreviousAccess { get; set; }
        
        public virtual List<AttestationReviewEndpointSystemL2Detail> AttestationReviewEndpointSystemL2Details { get; set; }
    }
}
