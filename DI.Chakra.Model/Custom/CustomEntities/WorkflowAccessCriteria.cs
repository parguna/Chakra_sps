using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Model.CustomEntities
{

    public abstract class WorkflowCriteriaBase
    {
        public RequestType RequestType { get; set; }
    }

    public class SubmitActionCriteria : WorkflowCriteriaBase
    {
        public List<Task> TaskList { get; set; }
    }

    public class ApprovalActionCriteria : WorkflowCriteriaBase
    {
        public long TaskId { get; set; }
        public Guid WorkflowInstanceId { get; set; }
    }

    public class WithdrawActionCriteria : WorkflowCriteriaBase
    {
        public AccessRequest AccessRequest { get; set; }
        public Guid WorkflowInstanceId { get; set; }
    }

}
