using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{

    public class SODRunPolicy : DtoBase
    {
        public Nullable<long> CompliancePolicyId { get; set; }

        public string UniqueRunCode { get; set; }

        public Nullable<int> RiskLevelId { get; set; }

        public Nullable<int> RiskScoreFrom { get; set; }

        public Nullable<int> RiskScoreTo { get; set; }

        public DateTime RunDate { get; set; }

        public Nullable<bool> RunAtObjectLevel { get; set; }

        public Nullable<long> WorkflowConfigurationId { get; set; }

        public string ExceptionCode { get; set; }

        public List<string> gblAttributes;

        public bool ObjectChecked;

        public  List<SODReport> lstReport { get; set; }

        public  WorkflowConfiguration WorkflowConfiguration { get; set; }

        public  SODRiskLevel RiskLevel { get; set; }

        public  CompliancePolicy compliancepolicy { get; set; }

        public  List<SODRunPolicySelection> EndpointSelection { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

    }
}
