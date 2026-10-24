using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;

namespace DI.Chakra.Workflow.Processor
{

    public class ApproverInfo
    {
        public ApprovalUserType ApproverType { get; set; }
        public string Owner { get; set; }
        public string SystemName { get; set; }
        public List<string> BusinessRoles { get; set; }

        public string BusinessRole
        {
            get
            {
                return string.Join(",", BusinessRoles);
            }
        }
    }

    public class AccessRequestReviewInfo
    {
        public long ReviewerId { get; set; }
        public long AccessRequestId { get; set; }
        public bool IsGroup { get; set; }
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public string Remarks { get; set; }

        public SubTask SubTaskInfo { get; set; }
    }

    public class ReviewInfo
    {
        public long ReviewerId { get; set; }
        public long ReviewId { get; set; }
        public long ReviewEndpointId { get; set; }
    }

    public class CheckForPendingParallelTasksResponse
    {
        public bool IsPendingTasksExists { get; set; }
        public bool CanContinueApproval { get; set; }
    }

    public class GetNextLevelApprovalTasksResponse
    {
        public bool IsApproved { get; set; }
        public bool SkipNextLevel { get; set; }
        public Task NextLevelTask { get; set; }
    }

    public class GetPendingTasksByWorkflowInstanceResponse
    {
        public List<Task> PendingTasks { get; set; }
    }

    public class GetTaskFromInterimStoreResponse
    {
        public Task Task { get; set; }
    }

    public class IsWorkflowStateConflictResponse
    {
        public bool StateConflict { get; set; }
    }
    public class IsPositionPrefixMissingResponse
    {
        public bool PositionPrefixMissing { get; set; }
    }
    public class IsNextLevelApprovalRequiredResponse
    {
        public bool IsRequired { get; set; }
    }

    public class GetWorkflowConfigurationByTaskResponse
    {
        public WorkflowConfiguration WorkflowConfiguration { get; set; }
    }

    public class MakeTaskLightWeightResponse
    {
        public Task LightWeightTask { get; set; }
    }

    public class SendEmailResponse
    {
        public bool IsMailSent { get; set; }
    }

}
