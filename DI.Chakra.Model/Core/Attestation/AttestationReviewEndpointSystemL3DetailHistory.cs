using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_M_AttestationReview_EndpointSystem_L3DetailHistory")]
    [Serializable]
    public class AttestationReviewEndpointSystemL3DetailHistory : IAggregateRoot
    {
        public AttestationReviewEndpointSystemL3DetailHistory()
        {
        }

        [Key]
        public long Id { get; set; }
        public long L3DetailId { get; set; }
        public long L2DetailId { get; set; }
        public string AccessType { get; set; }
        public string AccessName { get; set; }
        public string ResourceType { get; set; }
        public string ResourceCompletePath { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public bool Hidden { get; set; }
        public string BusinessOwner { get; set; }
        public string Comments { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public bool IsChanged { get; set; }
        public string PreviousAccess { get; set; }
    }
}
