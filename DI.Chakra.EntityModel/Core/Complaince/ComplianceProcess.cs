using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
   [Table("DI_M_ComplianceProcess")]
   public class ComplianceProcess:EntityBase, ICreatable, IUpdatable
    {
        
        public string ProcessName { get; set; }

        public string Description { get; set; }

        public long PolicyExecutionDefinitionId { get; set; }

        public bool Default { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


        [ForeignKey("PolicyExecutionDefinitionId")]
        public  SODPolicyExecutionDefinition PolicyExecutionDefinition { get; set; }
    }
}
