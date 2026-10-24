using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_SODConflictFunctions")]
    public class SODConflictFunctions : EntityBase,ICreatable,IUpdatable
    {
        public string SodConflictCode { get; set; }

        public string SodConflictDescription { get; set; }

        public string CompliancePolicyType { get; set; }

        public long RiskCodeId { get; set; }

        public Nullable<DateTime> LastGeneratedDate { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string Status { get; set; }

        [ForeignKey("RiskCodeId")]
        public  SODRiskCode SodRiskCode { get; set; }

        public  List<SODFunction> Functions { get; set; }

        public  List<CompliancePolicy> CompliancePolicies { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

    }
}
