using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationWorkflowConfiguration : WorkflowConfiguration
    {
        public long WorkflowConfigurationId { get; set; }

        public string RequestType { get; set; }

        public string ApplicationType { get; set; }
    }
}
