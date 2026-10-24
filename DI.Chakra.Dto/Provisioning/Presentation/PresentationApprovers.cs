using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationApprovers : AccessRequest
    {
        public int ApproverLevelId { get; set; }

        public string ApproverName { get; set; }

        public StatusType Action { get; set; }

        public System.DateTime ActionDate { get; set; }

        public byte WorkflowState { get; set; }
        
    }
}
