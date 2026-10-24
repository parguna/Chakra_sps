using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto.Presentation
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

    public class CanProceedSecondStepCriteria
    {
        public long RequestTypeId { get; set; }
        public long EndpointSystemId { get; set; }
        public EmployeeType UserType { get; set; }
        public AccountType AccountType { get; set; }
        public AdHocActionType ActionType { get; set; }
        public DateTime RequestDate { get; set; }
        public long UserId { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public EndpointType EndPointSystemType { get; set; }
    }
}
