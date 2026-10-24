using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_SODConflict")]
    public class SODConflict
    {
        [Key]
        public long Id { get; set; }
        public string SODConflictCode { get; set; }
        public string SODConflictDescription { get; set; }
        public Nullable<long> FunctionId1 { get; set; }
        public Nullable<long> FunctionId2 { get; set; }
        public long RiskCodeId { get; set; }
        public int CompliancePolicyId { get; set; }
        public Nullable<int> SodConflictType { get; set; }

        [ForeignKey("RiskCodeId")]
        public virtual SODRiskCode SodRiskId { get; set; }
        [ForeignKey("FunctionId1")]
        public virtual SodFunction function1 { get; set; }
        [ForeignKey("FunctionId2")]
        public virtual SodFunction function2 { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        //public List<CompliancePolicy> CompliancePolicies { get; set; }

    }
}
