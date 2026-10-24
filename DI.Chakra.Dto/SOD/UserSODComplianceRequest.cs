using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class UserSODComplianceRequest : DtoBase
    {
        public long UserId { get; set; }

        public long RequesteeUserId { get; set; }

        public long EndpointSystemId { get; set; }

        public EndpointType EndpointTypeValue { get; set; }

        public long PolicyId { get; set; }

        public string EntitlementDetails { get; set; }

        public bool IsFromAccessRequest { get; set; }

        public long AccessRequestId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
