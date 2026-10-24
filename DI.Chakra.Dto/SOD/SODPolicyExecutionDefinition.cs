using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SODPolicyExecutionDefinition : DtoBase
    {
        public Nullable<long> CompliancePolicyId { get; set; }

        public string UniqueRunCode { get; set; }

        public SODOutputType OutputType { get; set; }

        public Nullable<System.DateTime> RunDate { get; set; }

        public Nullable<bool> RunAtObjectLevel { get; set; }

        public CompliancePolicy CompliancePolicy { get; set; }

        public List<SODEndPointPolicySelection> EndpointSelection { get; set; }

        public List<SODException> Exceptions { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
