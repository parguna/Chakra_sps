using DI.Chakra.Model.SOD;
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
    [Table("DI_M_ComplianceProcess")]
    public class ComplianceProcess
    {
        [Key]
        public long Id { get; set; }
        public string ProcessName { get; set; }
        public string Description { get; set; }
        public long PolicyExecutionDefinitionId { get; set; }
        public bool Default { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("PolicyExecutionDefinitionId")]
        public virtual SODPolicyExceutionDefinition PolicyExecutionDefinition { get; set; }
    }
}
