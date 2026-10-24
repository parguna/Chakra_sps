using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{

    public class AttestationReviewEndpointSystem : DtoBase
    {
        public long EndpointUserId { get; set; }
        public long ReviewEntitlementId { get; set; }
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

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public bool IsCurrentLevelApproved { get; set; }

        public bool IsChanged { get; set; }

        public string PreviousAccess { get; set; }

        public virtual List<AttestationReviewEndpointSystemL2Detail> AttestationReviewEndpointSystemL2Details { get; set; }
    }
}
