using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationWorkflowConfiguration : WorkflowConfiguration
    {
        public long WorkflowConfigurationId { get; set; }

        public string RequestType { get; set; }

        public string ApplicationType { get; set; }
    }
}
