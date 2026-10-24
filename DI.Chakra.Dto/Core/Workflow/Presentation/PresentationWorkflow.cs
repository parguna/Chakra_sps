using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationWorkflow
    {

        public long WorkflowConfigurationId { get; set; }
        public string WorkflowName { get; set; }
        public string Description { get; set; }
        public string RequestType { get; set; }
        public string ApplicationType { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
