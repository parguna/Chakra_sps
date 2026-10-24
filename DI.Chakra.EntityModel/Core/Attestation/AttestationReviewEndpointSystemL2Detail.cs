using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AttestationReview_EndpointSystem_L2Detail")]
    [Serializable]
    public class AttestationReviewEndpointSystemL2Detail : EntityBase, ICreatable, IUpdatable
    {
        public long ReviewEndpointId { get; set; }
        public long ReviewEntitlementId { get; set; }

        public EntitlementType AccessType { get; set; }

        public string AccessName { get; set; }

        public string AccessDescription { get; set; }

        public string ResourceType { get; set; }

        public string ResourceCompletePath { get; set; }

        public bool Approve { get; set; }

        public bool Revoke { get; set; }

        public string Comments { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public bool IsChanged { get; set; }

        public string PreviousAccess { get; set; }

        [ForeignKey("ReviewEndpointId")]
        protected  AttestationReviewEndpointSystem Attestation { get; set; }

        public  List<AttestationReviewEndpointSystemL3Detail> AttestationReviewEndpointSystemL3Details { get; set; }
    }
}
