using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DI.Chakra.Dto
{
    public class AttestationReviewEndpointSystemL2Detail : DtoBase
    {
        public long ReviewEndpointId { get; set; }

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

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public bool IsChanged { get; set; }

        public string PreviousAccess { get; set; }

        protected  AttestationReviewEndpointSystem Attestation { get; set; }

        public  List<AttestationReviewEndpointSystemL3Detail> AttestationReviewEndpointSystemL3Details { get; set; }
    }
}
