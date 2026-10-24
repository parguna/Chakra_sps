using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class UserSODComplianceReport : DtoBase
    {
        public long UserId { get; set; }

        public long EndpointSystemId { get; set; }

        public string CompliancePolicyCode { get; set; }

        public string CompliancePolicyName { get; set; }

        public string CompliancePolicyType { get; set; }

        public string ConflictName { get; set; }

        public string ConflictDescription { get; set; }

        public string Function1Name { get; set; }

        public string Function1Description { get; set; }

        public string Function2Name { get; set; }

        public string Function2Description { get; set; }

        public string Type { get; set; }

        public string Endpoint { get; set; }

        public string ConflictEntitlement1 { get; set; }

        public string ConflictEntitlement2 { get; set; }

        public string ConflictEntitlement1Endpoint { get; set; }

        public string ConflictEntitlement2Endpoint { get; set; }

        public string Risk { get; set; }

        public string RiskDescription { get; set; }

        public string RiskLevel { get; set; }

        public int RiskScore { get; set; }

        public bool IsSensitiveRole { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public bool isFromAccessRequest { get; set; }
    }
}
