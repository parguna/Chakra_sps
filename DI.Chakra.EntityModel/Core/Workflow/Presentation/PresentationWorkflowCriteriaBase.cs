using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public abstract class PresentationWorkflowCriteriaBase
    {
        public RequestType RequestType { get; set; }
    }

    public class SubmitActionCriteria : PresentationWorkflowCriteriaBase
    {
        public List<Task> TaskList { get; set; }
    }

    public class ApprovalActionCriteria : PresentationWorkflowCriteriaBase
    {
        public long TaskId { get; set; }

        public Guid WorkflowInstanceId { get; set; }
        public long AccesRequestId { get; set; }

        public AccessType accessType { get; set; }
    }

    public class WithdrawActionCriteria : PresentationWorkflowCriteriaBase
    {
        public AccessRequest AccessRequest { get; set; }

        public Guid WorkflowInstanceId { get; set; }
    }
}
