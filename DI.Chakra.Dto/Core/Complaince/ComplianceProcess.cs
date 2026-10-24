using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class ComplianceProcess:DtoBase
    {
        
        public string ProcessName { get; set; }

        public string Description { get; set; }

        public long PolicyExecutionDefinitionId { get; set; }

        public bool Default { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public Process process { get; set;}

        public  SODPolicyExecutionDefinition PolicyExecutionDefinition { get; set; }
    }
}
