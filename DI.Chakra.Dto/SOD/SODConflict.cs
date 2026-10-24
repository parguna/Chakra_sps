using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{

    public class SODConflict : DtoBase
    {
        public string SODConflictCode { get; set; }

        public string SODConflictDescription { get; set; }

        public Nullable<long> FunctionId1 { get; set; }

        public Nullable<long> FunctionId2 { get; set; }

        public long RiskCodeId { get; set; }

        public int CompliancePolicyId { get; set; }

        public Nullable<int> SodConflictType { get; set; }

        public SODRiskCode SodRiskId { get; set; }

        public SODFunction function1 { get; set; }

        public SODFunction function2 { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

    }
}
