using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationWorkflowApproverGridData
    {
        public long WorkflowConfigurationDetailId { get; set; }

        public byte SequenceNumber { get; set; }

        public string ApprovalUserType { get; set; }

        public string Approver { get; set; }

        public int ResponseTimeInDays { get; set; }

        public string IsReminderRequired { get; set; }

        public string IsEscalationRequired { get; set; }

        public string AdditionalInfo { get; set; }
    }
}
