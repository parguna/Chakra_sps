using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_SODConflictFunctions")]
    public class SODConflictFunction
    {
        [Key]
        public long Id { get; set; }

        public string SodConflictCode { get; set; }
        public string SodConflictDescription { get; set; }
        public string CompliancePolicyType { get; set; }
        public long RiskCodeId { get; set; }
        //public Nullable<int> SodConflictType { get; set; }
        public Nullable<DateTime> LastGeneratedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Status { get; set; }

        [ForeignKey("RiskCodeId")]
        public virtual SODRiskCode SodRiskCode { get; set; }
        [DataMember]
        public virtual List<SodFunction> Functions { get; set; }
        public virtual List<CompliancePolicy> CompliancePolicies { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

    }
}
