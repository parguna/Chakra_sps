using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_M_AttestationReview_EndpointSystemHistory")]
    [Serializable]
    public class AttestationReviewEndpointSystemHistory : IAggregateRoot
    {
        public AttestationReviewEndpointSystemHistory()
        {
        }

        [Key]
        public long Id { get; set; }
        public long ReviewEndpointId { get; set; }
        public int EndpointUserId { get; set; }
        public string EndpointUserName { get; set; }
        public long EndpointSystemId { get; set; }
        public string HostName { get; set; }
        public int EndpointSystemTypeId { get; set; }
        public string EndpointSystemTypeName { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public string Comments { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public bool IsCurrentLevelApproved { get; set; }
        public bool IsChanged { get; set; }
        public string PreviousAccess { get; set; }
    }
}
