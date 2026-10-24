using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DomainModel.Presentation;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.DomainModel;
using System;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;

namespace DI.Chakra.Workflow.Processor.Common
{
    public class ApprovalChainFinder
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();
        private const string ROLEOWNER1 = "Role Owner - Level 1";
        private const string ROLEOWNER2 = "Role Owner - Level 2";
        private const string ROLEOWNER3 = "Role Owner - Level 3";
        private const string Supervisor = "Supervisor";

        public List<PresentationApprovalChain> Execute(long triggerId)
        {
            List<PresentationApprovalChain> approvers = new List<PresentationApprovalChain>();

            try
            {
                User tempUser = null;

                Task eventTask = Task.GetByEventTriggerId(triggerId);
                if (eventTask != null)
                {

                    WorkflowConfiguration wfConfig = WorkflowConfiguration.GetById(eventTask.WorkflowConfigurationId, true);
                    //continue from here.

                    foreach (WorkflowConfigurationDetail currentApprover in wfConfig.WorkflowConfigurationDetails)
                    {

                        byte workflowStateId = currentApprover.SequenceNumber;

                        PresentationApprovalChain approver = new PresentationApprovalChain();
                        List<Task> tasks = new List<Task>();

                        var trigger = EmployeeEventTrigger.GetById(triggerId);

                        tasks = Task.GetEmployeeTaskByRequestState(triggerId, workflowStateId);

                        if (tasks.Count == 0)
                        {

                            var reviewInfos = this.GetReviewerByTrigger(trigger, currentApprover);

                            if (reviewInfos.Count > 0)
                            {
                                approver.ApprovalState = currentApprover.SequenceNumber;
                                approver.Action = StatusType.NotInitiated;

                                var reviewInfo = reviewInfos.FirstOrDefault();

                                approver.ApproverType = utility.GetApprover(currentApprover);

                                if (reviewInfo.IsGroup)
                                {
                                    approver.ApproverName = reviewInfo.GroupName;
                                    approver.ApproverType = $"Group - {reviewInfo.GroupName}";
                                }
                                else
                                {
                                    List<string> approverNames = new List<string>();
                                    foreach (AccessRequestReviewInfo item in reviewInfos)
                                    {
                                        approverNames.Add(utility.GetUserByUserId(item.ReviewerId).DisplayName);
                                    }
                                    approver.ApproverName = string.Join(",", approverNames);
                                }

                                approvers.Add(approver);
                            }
                        }
                        else
                        {
                            StatusType action = StatusType.NotInitiated;
                            DateTime? actionDate = null;
                            string approverType = utility.GetApprover(currentApprover);
                            string approverName = string.Empty;
                            string comments = string.Empty;
                            string delegatedBy = string.Empty;
                            string delegatedFrom = string.Empty;
                            string delegationComments = string.Empty;
                            byte state = currentApprover.SequenceNumber;

                            var task = tasks.FirstOrDefault();

                            if (task.IsGroupTask)
                            {
                                var pendingGroupTask = tasks.OrderByDescending(item => item.IsDelegated).ThenByDescending(item => item.LastModifiedDate).FirstOrDefault(item => item.IsActive && !item.IsCompleted && item.IsGroupTask);

                                if (pendingGroupTask != null)
                                {
                                    action = StatusType.Pending;
                                    actionDate = null;
                                    comments = pendingGroupTask.Comments;

                                    if (!string.IsNullOrEmpty(pendingGroupTask.DelegatedBy))
                                    {
                                        tempUser = User.GetByUserName(pendingGroupTask.DelegatedBy);
                                        if (tempUser != null)
                                        {
                                            delegatedBy = tempUser.DisplayName;
                                        }
                                        else
                                        {
                                            delegatedBy = pendingGroupTask.DelegatedBy;
                                        }
                                    }
                                    else
                                    {
                                        delegatedBy = string.Empty;
                                    }

                                    delegatedFrom = pendingGroupTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(pendingGroupTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                    delegationComments = pendingGroupTask.DelegationComments;

                                    if (currentApprover.RequiresApprovalFromAll)
                                    {
                                        List<string> approverNames = new List<string>();

                                        var pendingGroupTasks = tasks.Where(item => item.IsActive && !item.IsCompleted && item.IsGroupTask);
                                        foreach (Task item in pendingGroupTasks)
                                        {
                                            approverNames.Add(utility.GetUserByUserId(item.ActionUserId.Value).DisplayName);
                                        }
                                        approverName = string.Join(",", approverNames);
                                    }
                                    else
                                    {
                                        if (pendingGroupTask.IsDelegated)
                                        {
                                            approverName = utility.GetUserByUserId(pendingGroupTask.ActionUserId.Value).DisplayName;
                                        }
                                        else
                                        {
                                            approverName = pendingGroupTask.GroupName;
                                        }
                                    }
                                }
                                else
                                {
                                    var completedGroupTask = tasks.FirstOrDefault(item => item.IsActive && item.IsCompleted && item.IsGroupTask);
                                    if (completedGroupTask != null)
                                    {
                                        action = completedGroupTask.Action == StatusType.None ? StatusType.Approved : completedGroupTask.Action;
                                        actionDate = completedGroupTask.LastModifiedDate;
                                        comments = completedGroupTask.Comments;
                                        if (!string.IsNullOrEmpty(completedGroupTask.DelegatedBy))
                                        {
                                            tempUser = User.GetByUserName(completedGroupTask.DelegatedBy);
                                            if (tempUser != null)
                                            {
                                                delegatedBy = tempUser.DisplayName;
                                            }
                                            else
                                            {
                                                delegatedBy = completedGroupTask.DelegatedBy;
                                            }
                                        }
                                        else
                                        {
                                            delegatedBy = string.Empty;
                                        }
                                        delegatedFrom = completedGroupTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(completedGroupTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                        delegationComments = completedGroupTask.DelegationComments;

                                        if (currentApprover.RequiresApprovalFromAll)
                                        {
                                            List<string> approverNames = new List<string>();

                                            var completedGroupTasks = tasks.Where(item => item.IsActive && item.IsCompleted && item.IsGroupTask);
                                            foreach (Task item in completedGroupTasks)
                                            {
                                                approverNames.Add(utility.GetUserByUserId(item.ActionUserId.Value).DisplayName);
                                            }
                                            approverName = string.Join(",", approverNames);
                                        }
                                        else
                                        {
                                            approverName = utility.GetUserByUserId(completedGroupTask.ActionUserId.Value).DisplayName;
                                        }
                                    }
                                    else
                                    {
                                        var autoCompletedGroupTask = tasks.FirstOrDefault(item => !item.IsActive && !item.IsCompleted && item.IsGroupTask);

                                        if (autoCompletedGroupTask != null)
                                        {
                                            if (completedGroupTask == null && pendingGroupTask == null)
                                            {
                                                action = autoCompletedGroupTask.Action == StatusType.None ? StatusType.AutoCompleted : autoCompletedGroupTask.Action;
                                                actionDate = autoCompletedGroupTask.LastModifiedDate;
                                                comments = autoCompletedGroupTask.Comments;
                                                if (!string.IsNullOrEmpty(autoCompletedGroupTask.DelegatedBy))
                                                {
                                                    tempUser = User.GetByUserName(autoCompletedGroupTask.DelegatedBy);
                                                    if (tempUser != null)
                                                    {
                                                        delegatedBy = tempUser.DisplayName;
                                                    }
                                                    else
                                                    {
                                                        delegatedBy = autoCompletedGroupTask.DelegatedBy;
                                                    }
                                                }
                                                else
                                                {
                                                    delegatedBy = string.Empty;
                                                }
                                                delegatedFrom = autoCompletedGroupTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(autoCompletedGroupTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                                delegationComments = autoCompletedGroupTask.DelegationComments;

                                                if (currentApprover.RequiresApprovalFromAll)
                                                {
                                                    List<string> approverNames = new List<string>();

                                                    var autoCompletedGroupTasks = tasks.Where(item => !item.IsActive && !item.IsCompleted && item.IsGroupTask);
                                                    foreach (Task item in autoCompletedGroupTasks)
                                                    {
                                                        approverNames.Add(utility.GetUserByUserId(item.ActionUserId.Value).DisplayName);
                                                    }
                                                    approverName = string.Join(",", approverNames);
                                                }
                                                else
                                                {
                                                    approverName = utility.GetUserByUserId(autoCompletedGroupTask.ActionUserId.Value).DisplayName;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var pendingTask = tasks.FirstOrDefault(item => item.IsActive && !item.IsCompleted && !item.IsGroupTask);

                                if (pendingTask != null)
                                {
                                    action = StatusType.Pending;
                                    actionDate = null;
                                    comments = pendingTask.Comments;
                                    if (!string.IsNullOrEmpty(pendingTask.DelegatedBy))
                                    {
                                        tempUser = User.GetByUserName(pendingTask.DelegatedBy);
                                        if (tempUser != null)
                                        {
                                            delegatedBy = tempUser.DisplayName;
                                        }
                                        else
                                        {
                                            delegatedBy = pendingTask.DelegatedBy;
                                        }
                                    }
                                    else
                                    {
                                        delegatedBy = string.Empty;
                                    }
                                    delegatedFrom = pendingTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(pendingTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                    delegationComments = pendingTask.DelegationComments;

                                    if (currentApprover.RequiresApprovalFromAll)
                                    {
                                        List<string> approverNames = new List<string>();

                                        var allTasks = tasks.Where(item => item.IsActive);
                                        foreach (Task item in allTasks)
                                        {
                                            approverNames.Add(utility.GetUserByUserId(item.ActionUserId.Value).DisplayName);
                                        }
                                        approverName = string.Join(",", approverNames);
                                    }
                                    else
                                    {
                                        var tmp = utility.GetUserByUserId(pendingTask.ActionUserId.Value);
                                        approverName = $"{tmp.DisplayName}({tmp.UserName})";
                                    }
                                }
                                else
                                {
                                    var completedTask = tasks.FirstOrDefault(item => item.IsActive && item.IsCompleted && !item.IsGroupTask);

                                    if (completedTask != null)
                                    {
                                        action = completedTask.Action == StatusType.None ? StatusType.Approved : completedTask.Action;
                                        actionDate = completedTask.LastModifiedDate;
                                        comments = completedTask.Comments;
                                        if (!string.IsNullOrEmpty(completedTask.DelegatedBy))
                                        {
                                            tempUser = User.GetByUserName(completedTask.DelegatedBy);
                                            if (tempUser != null)
                                            {
                                                delegatedBy = tempUser.DisplayName;
                                            }
                                            else
                                            {
                                                delegatedBy = completedTask.DelegatedBy;
                                            }
                                        }
                                        else
                                        {
                                            delegatedBy = string.Empty;
                                        }
                                        delegatedFrom = completedTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(completedTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                        delegationComments = completedTask.DelegationComments;

                                        if (currentApprover.RequiresApprovalFromAll)
                                        {
                                            List<string> approverNames = new List<string>();

                                            var allTasks = tasks.Where(item => item.IsActive);
                                            foreach (Task item in allTasks)
                                            {
                                                approverNames.Add(utility.GetUserByUserId(item.ActionUserId.Value).DisplayName);
                                            }
                                            approverName = string.Join(",", approverNames);
                                        }
                                        else
                                        {
                                            approverName = utility.GetUserByUserId(completedTask.ActionUserId.Value).DisplayName;
                                        }
                                    }
                                    else
                                    {
                                        var autoCompletedTask = tasks.FirstOrDefault(item => !item.IsActive && !item.IsCompleted && !item.IsGroupTask);

                                        if (autoCompletedTask != null)
                                        {
                                            if (completedTask == null && pendingTask == null)
                                            {
                                                action = autoCompletedTask.Action == StatusType.None ? StatusType.AutoCompleted : autoCompletedTask.Action;
                                                actionDate = autoCompletedTask.LastModifiedDate;
                                                comments = autoCompletedTask.Comments;
                                                if (!string.IsNullOrEmpty(autoCompletedTask.DelegatedBy))
                                                {
                                                    tempUser = User.GetByUserName(autoCompletedTask.DelegatedBy);
                                                    if (tempUser != null)
                                                    {
                                                        delegatedBy = tempUser.DisplayName;
                                                    }
                                                    else
                                                    {
                                                        delegatedBy = autoCompletedTask.DelegatedBy;
                                                    }
                                                }
                                                else
                                                {
                                                    delegatedBy = string.Empty;
                                                }
                                                delegatedFrom = autoCompletedTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(autoCompletedTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                                delegationComments = autoCompletedTask.DelegationComments;

                                                if (currentApprover.RequiresApprovalFromAll)
                                                {
                                                    List<string> approverNames = new List<string>();

                                                    foreach (Task item in tasks)
                                                    {
                                                        approverNames.Add(utility.GetUserByUserId(item.ActionUserId.Value).DisplayName);
                                                    }
                                                    approverName = string.Join(",", approverNames);
                                                }
                                                else
                                                {
                                                    approverName = utility.GetUserByUserId(autoCompletedTask.ActionUserId.Value).DisplayName;
                                                }
                                            }
                                        }
                                    }
                                }

                                var withDrawTask = tasks.FirstOrDefault(item => item.Action == StatusType.Withdrawn);
                                if (withDrawTask != null)
                                {
                                    action = withDrawTask.Action == StatusType.None ? StatusType.AutoCompleted : withDrawTask.Action;
                                    actionDate = withDrawTask.LastModifiedDate;
                                    comments = withDrawTask.Comments;
                                    if (!string.IsNullOrEmpty(withDrawTask.DelegatedBy))
                                    {
                                        tempUser = User.GetByUserName(withDrawTask.DelegatedBy);
                                        if (tempUser != null)
                                        {
                                            delegatedBy = tempUser.DisplayName;
                                        }
                                        else
                                        {
                                            delegatedBy = withDrawTask.DelegatedBy;
                                        }
                                    }
                                    else
                                    {
                                        delegatedBy = string.Empty;
                                    }
                                    delegatedFrom = withDrawTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(withDrawTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                    delegationComments = withDrawTask.DelegationComments;

                                    if (currentApprover.RequiresApprovalFromAll)
                                    {
                                        List<string> approverNames = new List<string>();

                                        foreach (Task item in tasks)
                                        {
                                            approverNames.Add(utility.GetUserByUserId(item.ActionUserId.Value).DisplayName);
                                        }
                                        approverName = string.Join(",", approverNames);
                                    }
                                    else
                                    {
                                        approverName = utility.GetUserByUserId(withDrawTask.ActionUserId.Value).DisplayName;
                                    }
                                }
                            }

                            approver.ApprovalState = state;
                            approver.Action = action;
                            approver.ApproverType = approverType;
                            approver.ApproverName = approverName;

                            ApproverDetail detail = new ApproverDetail();
                            detail.TaskOwner = approverName;
                            detail.Action = action;
                            detail.ActionDate = actionDate;
                            detail.Comments = comments;
                            detail.DelegatedBy = delegatedBy;
                            detail.DelegatedFrom = delegatedFrom;
                            detail.DelegationComments = delegationComments;
                            detail.ApprovalState = state;

                            approver.ApproverDetails.Add(detail);

                            approvers.Add(approver);

                            if (action == StatusType.Rejected)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error while getting approval chain for the trigger - " + triggerId.ToString();
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, message, ex);
            }

            return approvers;
        }

        void LogDebugStep(string message)
        {
            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.DebugStep, message);
        }

        WorkflowConfiguration getWorkflowConfiguration(User user, EndPoint endPoint, AdHocAccessRequest adhocRequest, AccessRequest request)
        {
            WorkflowConfiguration wfConfig = null;
            EmployeeType employeeType = user.EmployeeType;
            if (endPoint.ApplicationType == ApplicationType.MicrosoftTeams)
            {
                var adhocSkype = (AdhocSkypeForBusinessRequest)adhocRequest.GetEntitlementDetail();
                wfConfig = WorkflowConfiguration.GetByEndpointSystemId(Convert.ToInt64(RequestType.Provisioning), adhocRequest.EndpointSystemId.Value, Convert.ToString(Convert.ToByte(employeeType)),
                                    Convert.ToString(Convert.ToByte(adhocRequest.AccountType)), Convert.ToString(Convert.ToByte(adhocRequest.AdhocActionType)), request.CreatedDate, ((int)adhocSkype.EntitlementType).ToString());
            }
            else
            {
                wfConfig = WorkflowConfiguration.GetByEndpointSystemId(Convert.ToInt64(RequestType.Provisioning), adhocRequest.EndpointSystemId.Value, Convert.ToString(Convert.ToByte(user.EmployeeType)),
                                    Convert.ToString(Convert.ToByte(adhocRequest.AccountType)), Convert.ToString(Convert.ToByte(adhocRequest.AdhocActionType)), request.CreatedDate);
            }
            wfConfig = WorkflowConfiguration.GetById(wfConfig.Id, true);
            return wfConfig;
        }


        WorkflowConfiguration getWorkflowConfiguration(User user, EndPoint endPoint, BulkRequest request)
        {
            WorkflowConfiguration wfConfig = null;
            EmployeeType employeeType = user.EmployeeType;
            if (endPoint.ApplicationType == ApplicationType.MicrosoftTeams)
            {
                var adhocSkype = (AdhocSkypeForBusinessRequest)request.GetEntitlementDetail();
                wfConfig = WorkflowConfiguration.GetByEndpointSystemId(Convert.ToInt64(RequestType.Provisioning), request.EndpointSystemId, Convert.ToString(Convert.ToByte(employeeType)),
                                    Convert.ToString(Convert.ToByte(AccountType.User)), Convert.ToString(Convert.ToByte(request.AdhocActionType)), request.CreatedDate, ((int)adhocSkype.EntitlementType).ToString());
            }
            else
            {
                wfConfig = WorkflowConfiguration.GetByEndpointSystemId(Convert.ToInt64(RequestType.Provisioning), request.EndpointSystemId, Convert.ToString(Convert.ToByte(user.EmployeeType)),
                                    Convert.ToString(Convert.ToByte(AccountType.User)), Convert.ToString(Convert.ToByte(request.AdhocActionType)), request.CreatedDate);
            }
            wfConfig = WorkflowConfiguration.GetById(wfConfig.Id, true);
            return wfConfig;
        }

        public List<PresentationApprovalChain> Execute(long requestId, bool isBulkRequest)
        {
            List<PresentationApprovalChain> approvers = new List<PresentationApprovalChain>();

            try
            {
                AdHocAccessRequest adhocRequest = null;
                AccessRequest accessRequest = null;
                BulkRequest bulkRequest = null;
                WorkflowConfiguration wfConfig = null;

                User tempUser = null;
                utility.LogDebugStep($"ApprovalChainFinder Execute, isBulkRequest: {isBulkRequest}");
                if (!isBulkRequest)
                {
                    accessRequest = AccessRequest.GetByIdIncludeAdHocAccessRequest(requestId);
                    if (accessRequest.AttestationCampaignId.HasValue && Task.IsTaskExistsForAccessRequestAttestationRemediation(requestId))
                    {
                        return approvers;
                    }
                    utility.LogDebugStep($"ApprovalChainFinder Execute, accessRequest: {accessRequest == null}");
                    adhocRequest = accessRequest.AdHocAccessRequest;
                    var task = Task.GetByRequestId(requestId).FirstOrDefault();
                    utility.LogDebugStep($"ApprovalChainFinder Execute, task: {task == null}");
                    if (task != null)
                    {
                        if (accessRequest.RequestStatus == AccessRequestStatus.Rejected ||
                            accessRequest.RequestStatus == AccessRequestStatus.Withdrawn || accessRequest.RequestStatus == AccessRequestStatus.Granted
                            )
                        {
                            List<Task> tasks = new List<Task>();
                            tasks = Task.GetByRequestId(requestId);
                            foreach (var actionUser in tasks)
                            {
                                if ((actionUser.IsActive == false && actionUser.IsCompleted == false)) continue;

                                PresentationApprovalChain approver = new PresentationApprovalChain();
                                approver.ApprovalState = actionUser.WorkflowStateId;
                                approver.Action = actionUser.Action;
                                approver.ApproverType = "";
                                approver.ApproverName = "";
                                approver.IsActionOnlyRequired = actionUser.IsActionOnlyRequiredTask;

                                if ((actionUser.ActionUserId != null && actionUser.ActionGroupId != null) || (actionUser.ActionUserId == null && actionUser.ActionGroupId != null))
                                {
                                    Group grpName = Group.GetById(Convert.ToInt64(actionUser.ActionGroupId));
                                    approver.ApproverType = $"Group - {grpName.GroupName}";

                                }
                                else if (actionUser.ActionUserId != null && actionUser.ActionGroupId == null)
                                {
                                    approver.ApproverType = "User";
                                    if (actionUser.Approver != null)
                                    {
                                        var roleType = EnumExt.GetValueFromDescription<RoleType>(actionUser.Approver);
                                        if(roleType != RoleType.None)
                                            approver.ApproverType = roleType.GetDescription();
                                    }
                                }
                                else
                                {
                                    if (actionUser.Approver != null)
                                    {
                                        var roleType = EnumExt.GetValueFromDescription<RoleType>(actionUser.Approver);
                                        if (roleType != RoleType.None)
                                            approver.ApproverType = roleType.GetDescription();
                                    }
                                }
                                if (actionUser.ActionUserId != null && actionUser.ActionGroupId == null)
                                {
                                    User usrname = User.GetById(Convert.ToInt64(actionUser.ActionUserId));
                                    approver.ApproverName = usrname?.UserName;
                                }
                                else if ((actionUser.ActionUserId != null && actionUser.ActionGroupId != null) || (actionUser.ActionUserId == null && actionUser.ActionGroupId != null))
                                {
                                    Group grpName = Group.GetById(Convert.ToInt64(actionUser.ActionGroupId));
                                    approver.ApproverName = grpName.GroupName;
                                }
                                else
                                {
                                    if (actionUser.ActionGroupId.HasValue)
                                    {
                                        Group grpName = Group.GetById(Convert.ToInt64(actionUser.ActionGroupId));
                                        approver.ApproverName = grpName?.GroupName;
                                    }
                                }
                                string approverType = string.Empty;
                                string approverName = string.Empty;
                                DateTime? actionDate = null;
                                string comments = string.Empty;
                                string remarks = string.Empty;
                                string delegatedBy = string.Empty;
                                string delegatedFrom = string.Empty;
                                string delegationComments = string.Empty;
                                var subTaskRemarks = string.Empty;

                                List<string> approverNames = new List<string>();
                                byte workflowStateId = actionUser.WorkflowStateId;

                                if (actionUser.IsGroupTask)
                                {
                                    var completedGroupTask = actionUser;
                                    if (completedGroupTask != null)
                                    {
                                        remarks = GetTaskSubTaskRemarks(completedGroupTask.Id);
                                        actionDate = completedGroupTask.LastModifiedDate;
                                        comments = completedGroupTask.Comments;
                                        if (!string.IsNullOrEmpty(completedGroupTask.DelegatedBy))
                                        {
                                            tempUser = User.GetByUserName(completedGroupTask.DelegatedBy);
                                            if (tempUser != null)
                                            {
                                                delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                            }
                                            else
                                            {
                                                delegatedBy = completedGroupTask.DelegatedBy;
                                            }
                                        }
                                        else
                                        {
                                            delegatedBy = string.Empty;
                                        }
                                        delegatedFrom = completedGroupTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(completedGroupTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                        delegationComments = completedGroupTask.DelegationComments;

                                        string taskOwner = string.Empty;
                                        if (completedGroupTask.ActionUserId.HasValue)
                                        {
                                            var owner = utility.GetUserByUserId(completedGroupTask.ActionUserId.Value);
                                            string grouptaskowner = $"{owner.DisplayName}({owner.UserName})";
                                            if (completedGroupTask.IsGroupTask) approver.IsGroupApproval = true;
                                            taskOwner = string.Format("{0} ({1})", grouptaskowner, completedGroupTask.GroupName);
                                        }

                                        if ((completedGroupTask.Approver == ROLEOWNER1 && completedGroupTask.ActionGroupId.HasValue) || (completedGroupTask.Approver == ROLEOWNER2 && completedGroupTask.ActionGroupId.HasValue) ||
                                            (completedGroupTask.Approver == ROLEOWNER3 && completedGroupTask.ActionGroupId.HasValue))
                                        {
                                            var owner = utility.GetGroupNameById(completedGroupTask.ActionGroupId.Value);
                                            if (completedGroupTask.IsGroupTask) approver.IsGroupApproval = true;
                                            taskOwner = string.Format("{0}", completedGroupTask.GroupName);
                                        }


                                        approverNames.Add(completedGroupTask.GroupName);

                                        ApproverDetail detail = new ApproverDetail();
                                        detail.TaskOwner = taskOwner;
                                        detail.Action = completedGroupTask.Action;
                                        detail.ActionDate = actionDate;
                                        detail.Comments = comments;
                                        detail.DelegatedBy = delegatedBy;
                                        detail.DelegatedFrom = delegatedFrom;
                                        detail.DelegationComments = delegationComments;
                                        detail.ApprovalState = workflowStateId;
                                        detail.Remarks = remarks;
                                        approver.ApproverDetails.Add(detail);
                                    }

                                }

                                if (!actionUser.IsGroupTask)
                                {

                                    var completedGroupTask = actionUser;
                                    if (completedGroupTask != null)
                                    {
                                        remarks = GetTaskSubTaskRemarks(completedGroupTask.Id);
                                        actionDate = completedGroupTask.LastModifiedDate;
                                        comments = completedGroupTask.Comments;

                                        if (!string.IsNullOrEmpty(completedGroupTask.DelegatedBy))
                                        {
                                            tempUser = User.GetByUserName(completedGroupTask.DelegatedBy);
                                            if (tempUser != null)
                                            {
                                                delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                            }
                                            else
                                            {
                                                delegatedBy = completedGroupTask.DelegatedBy;
                                            }
                                        }
                                        else
                                        {
                                            delegatedBy = string.Empty;
                                        }
                                        if (completedGroupTask.DelegatedFrom.HasValue)
                                        {
                                            var downer = utility.GetUserByUserId(completedGroupTask.DelegatedFrom.Value);
                                            string deletefromowner = $"{downer.DisplayName}({downer.UserName})";
                                            delegatedFrom = deletefromowner;
                                        }
                                        else
                                        {
                                            delegatedFrom = string.Empty;
                                        }

                                        delegationComments = completedGroupTask.DelegationComments;

                                        approverName = completedGroupTask.GroupName;
                                        approverType = $"Group - {completedGroupTask.GroupName}";
                                        ApproverDetail detail = new ApproverDetail();
                                        if (completedGroupTask.ActionUserId.HasValue)
                                        {
                                            var owner = utility.GetUserByUserId(completedGroupTask.ActionUserId.Value);
                                            string taskOwner = $"{owner.DisplayName}({owner.UserName})";

                                            detail.TaskOwner = taskOwner;
                                        }
                                        else
                                        {
                                            detail.TaskOwner = completedGroupTask.GroupName;
                                            approver.IsGroupApproval = true;
                                        }


                                        detail.Action = completedGroupTask.Action;
                                        detail.ActionDate = actionDate;
                                        detail.Comments = comments;
                                        detail.DelegatedBy = delegatedBy;
                                        detail.DelegatedFrom = delegatedFrom;
                                        detail.DelegationComments = delegationComments;
                                        detail.ApprovalState = workflowStateId;
                                        detail.Remarks = remarks;
                                        approver.ApproverDetails.Add(detail);
                                    }

                                }
                                approvers.Add(approver);
                            }
                        }
                        else
                            wfConfig = WorkflowConfiguration.GetById(task.WorkflowConfigurationId, true);
                    }

                    else
                    {
                        User user = User.GetById(accessRequest.UserId);
                        EndPoint ep = EndPoint.GetById(accessRequest.EndpointSystemId.Value);
                        wfConfig = getWorkflowConfiguration(user, ep, adhocRequest, accessRequest);
                    }
                }
                else
                {
                    bulkRequest = BulkRequest.GetById(requestId, true);
                    var task = Task.GetByBulkRequestId(requestId).FirstOrDefault();
                    if (task != null)
                    {
                        if (bulkRequest != null && (bulkRequest.RequestStatus == AccessRequestStatus.Rejected ||
                            bulkRequest.RequestStatus == AccessRequestStatus.Withdrawn || bulkRequest.RequestStatus == AccessRequestStatus.Granted
                            ))
                        {
                            List<Task> tasks = new List<Task>();
                            tasks = Task.GetByBulkRequestId(requestId);
                            foreach (var actionUser in tasks)
                            {
                                if ((actionUser.IsActive == false && actionUser.IsCompleted == false)) continue;

                                PresentationApprovalChain approver = new PresentationApprovalChain();
                                approver.ApprovalState = actionUser.WorkflowStateId;
                                approver.Action = StatusType.Approved;
                                approver.ApproverType = "";
                                approver.ApproverName = "";
                                if ((actionUser.ActionUserId != null && actionUser.ActionGroupId != null) || (actionUser.ActionUserId == null && actionUser.ActionGroupId != null))
                                {
                                    Group grpName = Group.GetById(Convert.ToInt64(actionUser.ActionGroupId));
                                    approver.ApproverType = $"Group - {grpName.GroupName}";

                                }
                                else if (actionUser.ActionUserId != null && actionUser.ActionGroupId == null)
                                {
                                    approver.ApproverType = "User";
                                    if (actionUser.Approver != null)
                                    {
                                        var roleType = EnumExt.GetValueFromDescription<RoleType>(actionUser.Approver);
                                        if (roleType != RoleType.None)
                                            approver.ApproverType = roleType.GetDescription();
                                    }
                                }
                                else
                                {
                                    if (actionUser.Approver != null)
                                    {
                                        var roleType = EnumExt.GetValueFromDescription<RoleType>(actionUser.Approver);
                                        if (roleType != RoleType.None)
                                            approver.ApproverType = roleType.GetDescription();
                                    }
                                }
                                if (actionUser.ActionUserId != null && actionUser.ActionGroupId == null)
                                {
                                    User usrname = User.GetById(Convert.ToInt64(actionUser.ActionUserId));
                                    approver.ApproverName = usrname?.UserName;
                                }

                                else if ((actionUser.ActionUserId != null && actionUser.ActionGroupId != null) || (actionUser.ActionUserId == null && actionUser.ActionGroupId != null))
                                {
                                    Group grpName = Group.GetById(Convert.ToInt64(actionUser.ActionGroupId));
                                    approver.ApproverName = grpName.GroupName;
                                }
                                else
                                {
                                    if (actionUser.ActionGroupId.HasValue)
                                    {
                                        Group grpName = Group.GetById(Convert.ToInt64(actionUser.ActionGroupId));
                                        approver.ApproverName = grpName?.GroupName;
                                    }
                                }
                                string approverType = string.Empty;
                                string approverName = string.Empty;
                                DateTime? actionDate = null;
                                string comments = string.Empty;
                                string remarks = string.Empty;
                                string delegatedBy = string.Empty;
                                string delegatedFrom = string.Empty;
                                string delegationComments = string.Empty;
                                var subTaskRemarks = string.Empty;

                                List<string> approverNames = new List<string>();
                                byte workflowStateId = actionUser.WorkflowStateId;

                                if (actionUser.IsGroupTask)
                                {
                                    var completedGroupTask = actionUser;
                                    if (completedGroupTask != null)
                                    {
                                        remarks = GetTaskSubTaskRemarks(completedGroupTask.Id);
                                        actionDate = completedGroupTask.LastModifiedDate;
                                        comments = completedGroupTask.Comments;
                                        if (!string.IsNullOrEmpty(completedGroupTask.DelegatedBy))
                                        {
                                            tempUser = User.GetByUserName(completedGroupTask.DelegatedBy);
                                            if (tempUser != null)
                                            {
                                                delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                            }
                                            else
                                            {
                                                delegatedBy = completedGroupTask.DelegatedBy;
                                            }
                                        }
                                        else
                                        {
                                            delegatedBy = string.Empty;
                                        }
                                        delegatedFrom = completedGroupTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(completedGroupTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                        delegationComments = completedGroupTask.DelegationComments;

                                        string taskOwner = string.Empty;
                                        if (completedGroupTask.ActionUserId.HasValue)
                                        {
                                            var owner = utility.GetUserByUserId(completedGroupTask.ActionUserId.Value);
                                            string grouptaskowner = $"{owner.DisplayName}({owner.UserName})";
                                            if (completedGroupTask.IsGroupTask) approver.IsGroupApproval = true;
                                            taskOwner = string.Format("{0} ({1})", grouptaskowner, completedGroupTask.GroupName);
                                        }

                                        if ((completedGroupTask.Approver == ROLEOWNER1 && completedGroupTask.ActionGroupId.HasValue) || (completedGroupTask.Approver == ROLEOWNER2 && completedGroupTask.ActionGroupId.HasValue) ||
                                            (completedGroupTask.Approver == ROLEOWNER3 && completedGroupTask.ActionGroupId.HasValue))
                                        {
                                            var owner = utility.GetGroupNameById(completedGroupTask.ActionGroupId.Value);
                                            if (completedGroupTask.IsGroupTask) approver.IsGroupApproval = true;
                                            taskOwner = string.Format("{0}", completedGroupTask.GroupName);
                                        }


                                        approverNames.Add(completedGroupTask.GroupName);

                                        ApproverDetail detail = new ApproverDetail();
                                        detail.TaskOwner = taskOwner;
                                        detail.Action = completedGroupTask.Action;
                                        detail.ActionDate = actionDate;
                                        detail.Comments = comments;
                                        detail.DelegatedBy = delegatedBy;
                                        detail.DelegatedFrom = delegatedFrom;
                                        detail.DelegationComments = delegationComments;
                                        detail.ApprovalState = workflowStateId;
                                        detail.Remarks = remarks;
                                        approver.ApproverDetails.Add(detail);
                                    }

                                }

                                if (!actionUser.IsGroupTask)
                                {

                                    var completedGroupTask = actionUser;
                                    if (completedGroupTask != null)
                                    {
                                        remarks = GetTaskSubTaskRemarks(completedGroupTask.Id);
                                        actionDate = completedGroupTask.LastModifiedDate;
                                        comments = completedGroupTask.Comments;

                                        if (!string.IsNullOrEmpty(completedGroupTask.DelegatedBy))
                                        {
                                            tempUser = User.GetByUserName(completedGroupTask.DelegatedBy);
                                            if (tempUser != null)
                                            {
                                                delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                            }
                                            else
                                            {
                                                delegatedBy = completedGroupTask.DelegatedBy;
                                            }
                                        }
                                        else
                                        {
                                            delegatedBy = string.Empty;
                                        }
                                        if (completedGroupTask.DelegatedFrom.HasValue)
                                        {
                                            var downer = utility.GetUserByUserId(completedGroupTask.DelegatedFrom.Value);
                                            string deletefromowner = $"{downer.DisplayName}({downer.UserName})";
                                            delegatedFrom = deletefromowner;
                                        }
                                        else
                                        {
                                            delegatedFrom = string.Empty;
                                        }

                                        delegationComments = completedGroupTask.DelegationComments;

                                        approverName = completedGroupTask.GroupName;
                                        approverType = $"Group - {completedGroupTask.GroupName}";
                                        ApproverDetail detail = new ApproverDetail();
                                        if (completedGroupTask.ActionUserId.HasValue)
                                        {
                                            var owner = utility.GetUserByUserId(completedGroupTask.ActionUserId.Value);
                                            string taskOwner = $"{owner.DisplayName}({owner.UserName})";

                                            detail.TaskOwner = taskOwner;
                                        }
                                        else
                                        {
                                            detail.TaskOwner = completedGroupTask.GroupName;
                                            approver.IsGroupApproval = true;
                                        }


                                        detail.Action = completedGroupTask.Action;
                                        detail.ActionDate = actionDate;
                                        detail.Comments = comments;
                                        detail.DelegatedBy = delegatedBy;
                                        detail.DelegatedFrom = delegatedFrom;
                                        detail.DelegationComments = delegationComments;
                                        detail.ApprovalState = workflowStateId;
                                        detail.Remarks = remarks;
                                        approver.ApproverDetails.Add(detail);
                                    }

                                }
                                approvers.Add(approver);
                            }
                        }
                        else
                        {
                            wfConfig = WorkflowConfiguration.GetById(task.WorkflowConfigurationId, true);
                        }
                    }
                    else
                    {
                        User user = User.GetById(bulkRequest.BulkRequestDetails.First().UserId);
                        EndPoint ep = EndPoint.GetById(bulkRequest.EndpointSystemId);
                        wfConfig = getWorkflowConfiguration(user, ep, bulkRequest);
                    }
                }



                //continue from here.
                if (wfConfig != null)
                {
                    foreach (WorkflowConfigurationDetail currentApprover in wfConfig.WorkflowConfigurationDetails.OrderBy(i => i.SequenceNumber))
                    {

                        byte workflowStateId = currentApprover.SequenceNumber;

                        PresentationApprovalChain approver = new PresentationApprovalChain();
                        approver.IsActionOnlyRequired = currentApprover.IsActionRequired;
                        List<Task> tasks = new List<Task>();

                        if (isBulkRequest)
                        {
                            tasks = Task.GetBulkRequestTaskByRequestState(requestId, workflowStateId);
                        }
                        else
                        {
                            tasks = Task.GetAccessRequestTaskByRequestState(requestId, workflowStateId);
                        }

                        if (tasks.Count == 0)
                        {
                            var reviewInfos = this.GetProvisioningReviewersInformation(accessRequest, adhocRequest, bulkRequest, currentApprover);

                            if (reviewInfos.Count > 0)
                            {
                                approver.ApprovalState = workflowStateId;
                                approver.Action = StatusType.NotInitiated;

                                var reviewInfo = reviewInfos.FirstOrDefault();

                                approver.ApproverType = utility.GetApprover(currentApprover);

                                if (currentApprover.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                                {
                                    var indReviews = reviewInfos.Where(item => !item.IsGroup);
                                    var groupReviews = reviewInfos.Where(item => item.IsGroup);
                                    List<string> approverNames = new List<string>();

                                    foreach (AccessRequestReviewInfo item in indReviews)
                                    {
                                        var owner = utility.GetUserByUserId(item.ReviewerId);

                                        approverNames.Add($"{owner.DisplayName}({owner.UserName})");
                                    }

                                    foreach (AccessRequestReviewInfo item in groupReviews)
                                    {
                                        if (!approverNames.Contains(item.GroupName))
                                        {
                                            approverNames.Add(item.GroupName);
                                        }
                                    }
                                    approver.IsGroupApproval = groupReviews.Count() > 0;
                                    approver.ApproverName = string.Join(",", approverNames);
                                }
                                else
                                {
                                    if (reviewInfo.IsGroup)
                                    {
                                        approver.ApproverName = reviewInfo.GroupName;
                                        approver.ApproverType = $"Group - {reviewInfo.GroupName}";
                                        approver.IsGroupApproval = true;
                                    }
                                    else
                                    {
                                        List<string> approverNames = new List<string>();
                                        foreach (AccessRequestReviewInfo item in reviewInfos)
                                        {
                                            var owner = utility.GetUserByUserId(item.ReviewerId);
                                            string taskOwner = $"{owner.DisplayName}({owner.UserName})";
                                            // approverNames.Add(utility.GetUserByUserId(item.ReviewerId).DisplayName);
                                            approverNames.Add(taskOwner);
                                        }
                                        approver.ApproverName = string.Join(",", approverNames);
                                    }
                                }

                                approvers.Add(approver);
                            }
                        }
                        else
                        {
                            string approverType = utility.GetApprover(currentApprover);
                            string approverName = string.Empty;
                            DateTime? actionDate = null;
                            string comments = string.Empty;
                            string remarks = string.Empty;
                            string delegatedBy = string.Empty;
                            string delegatedFrom = string.Empty;
                            string delegationComments = string.Empty;
                            var subTaskRemarks = string.Empty;

                            if (currentApprover.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                            {
                                var indtasks = tasks.Where(item => !item.IsGroupTask);
                                var grptasks = tasks.Where(item => item.IsGroupTask);

                                List<string> approverNames = new List<string>();

                                if (grptasks.Count() > 0)
                                {
                                    var pendingGroupTask = grptasks.OrderByDescending(item => item.IsDelegated).ThenByDescending(item => item.LastModifiedDate).FirstOrDefault(item => item.IsActive && !item.IsCompleted);

                                    if (pendingGroupTask != null)
                                    {

                                        remarks = GetTaskSubTaskRemarks(pendingGroupTask.Id);
                                        actionDate = null;
                                        comments = pendingGroupTask.Comments;

                                        if (!string.IsNullOrEmpty(pendingGroupTask.DelegatedBy))
                                        {
                                            tempUser = User.GetByUserName(pendingGroupTask.DelegatedBy);
                                            if (tempUser != null)
                                            {
                                                delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                            }
                                            else
                                            {
                                                delegatedBy = pendingGroupTask.DelegatedBy;
                                            }
                                        }
                                        else
                                        {
                                            delegatedBy = string.Empty;
                                        }
                                        delegatedFrom = string.Empty;
                                        if (pendingGroupTask.DelegatedFrom.HasValue)
                                        {
                                            var dUsr = utility.GetUserByUserId(pendingGroupTask.DelegatedFrom.Value);
                                            if (dUsr != null)
                                                delegatedFrom = $"{dUsr.DisplayName}({dUsr?.UserName})";
                                        }
                                        
                                        delegationComments = pendingGroupTask.DelegationComments;

                                        string taskOwner = string.Empty;
                                        if (pendingGroupTask.IsDelegated)
                                        {
                                            var owner = utility.GetUserByUserId(pendingGroupTask.ActionUserId.Value);
                                            taskOwner = $"{owner.DisplayName}({owner.UserName})";

                                            //taskOwner = utility.GetUserByUserId(pendingGroupTask.ActionUserId.Value).DisplayName;
                                            approverNames.Add(taskOwner);
                                        }
                                        else
                                        {
                                            taskOwner = pendingGroupTask.GroupName;
                                            approverNames.Add(taskOwner);
                                        }

                                        ApproverDetail detail = new ApproverDetail();
                                        detail.TaskOwner = taskOwner;
                                        detail.Action = pendingGroupTask.Action;
                                        detail.ActionDate = actionDate;
                                        detail.Comments = comments;
                                        detail.DelegatedBy = delegatedBy;
                                        detail.DelegatedFrom = delegatedFrom;
                                        detail.DelegationComments = delegationComments;
                                        detail.ApprovalState = workflowStateId;
                                        detail.Remarks = remarks;
                                        approver.ApproverDetails.Add(detail);
                                    }
                                    else
                                    {
                                        var completedGroupTask = grptasks.FirstOrDefault(item => item.IsActive && item.IsCompleted);
                                        if (completedGroupTask != null)
                                        {
                                            remarks = GetTaskSubTaskRemarks(completedGroupTask.Id);
                                            actionDate = completedGroupTask.LastModifiedDate;
                                            comments = completedGroupTask.Comments;
                                            if (!string.IsNullOrEmpty(completedGroupTask.DelegatedBy))
                                            {
                                                tempUser = User.GetByUserName(completedGroupTask.DelegatedBy);
                                                if (tempUser != null)
                                                {
                                                    delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                                }
                                                else
                                                {
                                                    delegatedBy = completedGroupTask.DelegatedBy;
                                                }
                                            }
                                            else
                                            {
                                                delegatedBy = string.Empty;
                                            }
                                            delegatedFrom = completedGroupTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(completedGroupTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                            delegationComments = completedGroupTask.DelegationComments;

                                            string taskOwner = string.Empty;
                                            if (completedGroupTask.ActionUserId.HasValue)
                                            {

                                                // utility.GetUserByUserId(completedGroupTask.ActionUserId.Value).DisplayName
                                                var owner = utility.GetUserByUserId(completedGroupTask.ActionUserId.Value);
                                                string grouptaskowner = $"{owner.DisplayName}({owner.UserName})";
                                                if (completedGroupTask.IsGroupTask) approver.IsGroupApproval = true;
                                                taskOwner = string.Format("{0} ({1})", grouptaskowner, completedGroupTask.GroupName);
                                            }
                                            approverNames.Add(completedGroupTask.GroupName);

                                            ApproverDetail detail = new ApproverDetail();
                                            detail.TaskOwner = taskOwner;
                                            detail.Action = completedGroupTask.Action;
                                            detail.ActionDate = actionDate;
                                            detail.Comments = comments;
                                            detail.DelegatedBy = delegatedBy;
                                            detail.DelegatedFrom = delegatedFrom;
                                            detail.DelegationComments = delegationComments;
                                            detail.ApprovalState = workflowStateId;
                                            detail.Remarks = remarks;
                                            approver.ApproverDetails.Add(detail);
                                        }
                                        else
                                        {
                                            var autoCompletedGroupTask = grptasks.FirstOrDefault(item => !item.IsActive && !item.IsCompleted && item.IsGroupTask);

                                            if (autoCompletedGroupTask != null)
                                            {
                                                if (completedGroupTask == null && pendingGroupTask == null)
                                                {
                                                    remarks = GetTaskSubTaskRemarks(autoCompletedGroupTask.Id);
                                                    actionDate = autoCompletedGroupTask.LastModifiedDate;
                                                    comments = autoCompletedGroupTask.Comments;

                                                    if (!string.IsNullOrEmpty(autoCompletedGroupTask.DelegatedBy))
                                                    {
                                                        tempUser = User.GetByUserName(autoCompletedGroupTask.DelegatedBy);
                                                        if (tempUser != null)
                                                        {
                                                            delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                                        }
                                                        else
                                                        {
                                                            delegatedBy = autoCompletedGroupTask.DelegatedBy;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        delegatedBy = string.Empty;
                                                    }
                                                    delegatedFrom = autoCompletedGroupTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(autoCompletedGroupTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                                    delegationComments = autoCompletedGroupTask.DelegationComments;

                                                    string taskOwner = string.Empty;
                                                    if (autoCompletedGroupTask.IsDelegated)
                                                    {
                                                        var owner = utility.GetUserByUserId(autoCompletedGroupTask.ActionUserId.Value);
                                                        taskOwner = $"{owner.DisplayName}({owner.UserName})";

                                                        //  taskOwner = utility.GetUserByUserId(autoCompletedGroupTask.ActionUserId.Value).DisplayName;
                                                        approverNames.Add(taskOwner);
                                                    }
                                                    else
                                                    {
                                                        approver.IsGroupApproval = true;
                                                        taskOwner = autoCompletedGroupTask.GroupName;
                                                        approverNames.Add(taskOwner);
                                                    }

                                                    ApproverDetail detail = new ApproverDetail();
                                                    detail.TaskOwner = taskOwner;
                                                    detail.Action = autoCompletedGroupTask.Action;
                                                    detail.ActionDate = actionDate;
                                                    detail.Comments = comments;
                                                    detail.DelegatedBy = delegatedBy;
                                                    detail.DelegatedFrom = delegatedFrom;
                                                    detail.DelegationComments = delegationComments;
                                                    detail.ApprovalState = workflowStateId;
                                                    detail.Remarks = remarks;
                                                    approver.ApproverDetails.Add(detail);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (indtasks.Count() > 0)
                                {
                                    foreach (Task item in indtasks)
                                    {
                                        var owner = utility.GetUserByUserId(item.ActionUserId.Value);
                                        string taskOwner = $"{owner.DisplayName}({owner.UserName})";
                                        approverNames.Add(taskOwner);

                                        actionDate = item.LastModifiedDate;
                                        comments = item.Comments;
                                        remarks = GetTaskSubTaskRemarks(item.Id);
                                        if (!string.IsNullOrEmpty(item.DelegatedBy))
                                        {
                                            tempUser = User.GetByUserName(item.DelegatedBy);
                                            if (tempUser != null)
                                            {
                                                delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                            }
                                            else
                                            {
                                                delegatedBy = item.DelegatedBy;
                                            }
                                        }
                                        else
                                        {
                                            delegatedBy = string.Empty;
                                        }
                                        delegatedFrom = string.Empty;
                                        if (item.DelegatedFrom.HasValue)
                                        {
                                            var dUsr = utility.GetUserByUserId(item.DelegatedFrom.Value);
                                            if(dUsr != null)
                                                delegatedFrom = $"{dUsr.DisplayName}({dUsr?.UserName})";
                                        }
                                        delegationComments = item.DelegationComments;

                                        ApproverDetail detail = new ApproverDetail();
                                        detail.TaskOwner = taskOwner;
                                        detail.Action = item.Action;
                                        detail.ActionDate = actionDate;
                                        detail.Comments = comments;
                                        detail.DelegatedBy = delegatedBy;
                                        detail.DelegatedFrom = delegatedFrom;
                                        detail.DelegationComments = delegationComments;
                                        detail.ApprovalState = workflowStateId;
                                        detail.Remarks = remarks;
                                        approver.ApproverDetails.Add(detail);
                                    }
                                }
                                approverName = string.Join(",", approverNames);
                            }
                            else
                            {
                                var task = tasks.FirstOrDefault();

                                if (task.IsGroupTask)
                                {
                                    ApproverDetail detail = new ApproverDetail();
                                    approver.IsGroupApproval = true;
                                    var pendingGroupTask = tasks.OrderByDescending(item => item.IsDelegated).ThenByDescending(item => item.LastModifiedDate).FirstOrDefault(item => item.IsActive && !item.IsCompleted && item.IsGroupTask);

                                    if (pendingGroupTask != null)
                                    {
                                        remarks = GetTaskSubTaskRemarks(pendingGroupTask.Id);
                                        actionDate = null;
                                        comments = pendingGroupTask.Comments;

                                        if (!string.IsNullOrEmpty(pendingGroupTask.DelegatedBy))
                                        {
                                            tempUser = User.GetByUserName(pendingGroupTask.DelegatedBy);
                                            if (tempUser != null)
                                            {
                                                delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                            }
                                            else
                                            {
                                                delegatedBy = pendingGroupTask.DelegatedBy;
                                            }
                                        }
                                        else
                                        {
                                            delegatedBy = string.Empty;
                                        }

                                        if (pendingGroupTask.DelegatedFrom.HasValue)
                                        {
                                            var owner = utility.GetUserByUserId(pendingGroupTask.DelegatedFrom.Value);
                                            string deletefromowner = $"{owner.DisplayName}({owner.UserName})";

                                            //delegatedFrom = pendingGroupTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(pendingGroupTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                            delegatedFrom = deletefromowner;
                                        }
                                        else
                                        {
                                            delegatedFrom = string.Empty;
                                        }
                                        delegationComments = pendingGroupTask.DelegationComments;

                                        if (pendingGroupTask.IsDelegated)
                                        {
                                            var owner = utility.GetUserByUserId(pendingGroupTask.ActionUserId.Value);
                                            string taskOwner = $"{owner.DisplayName}({owner.UserName})";
                                            approverName = taskOwner;
                                        }
                                        else
                                        {
                                            approverName = pendingGroupTask.GroupName;
                                            approverType = $"Group - {pendingGroupTask.GroupName}";
                                            approver.IsGroupApproval = true;
                                        }


                                        detail.TaskOwner = approverName;
                                        detail.Action = pendingGroupTask.Action;
                                        detail.ActionDate = actionDate;
                                        detail.Comments = comments;
                                        detail.DelegatedBy = delegatedBy;
                                        detail.DelegatedFrom = delegatedFrom;
                                        detail.DelegationComments = delegationComments;
                                        detail.ApprovalState = workflowStateId;
                                        detail.Remarks = remarks;
                                        approver.ApproverDetails.Add(detail);
                                    }
                                    else
                                    {
                                        var completedGroupTask = tasks.FirstOrDefault(item => item.IsActive && item.IsCompleted && item.IsGroupTask);
                                        if (completedGroupTask != null)
                                        {
                                            remarks = GetTaskSubTaskRemarks(completedGroupTask.Id);
                                            actionDate = completedGroupTask.LastModifiedDate;
                                            comments = completedGroupTask.Comments;

                                            if (!string.IsNullOrEmpty(completedGroupTask.DelegatedBy))
                                            {
                                                tempUser = User.GetByUserName(completedGroupTask.DelegatedBy);
                                                if (tempUser != null)
                                                {
                                                    delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                                }
                                                else
                                                {
                                                    delegatedBy = completedGroupTask.DelegatedBy;
                                                }
                                            }
                                            else
                                            {
                                                delegatedBy = string.Empty;
                                            }
                                            // delegatedFrom = completedGroupTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(completedGroupTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                            if (completedGroupTask.DelegatedFrom.HasValue)
                                            {
                                                var downer = utility.GetUserByUserId(completedGroupTask.DelegatedFrom.Value);
                                                string deletefromowner = $"{downer.DisplayName}({downer.UserName})";


                                                delegatedFrom = deletefromowner;
                                            }
                                            else
                                            {
                                                delegatedFrom = string.Empty;
                                            }

                                            delegationComments = completedGroupTask.DelegationComments;

                                            approverName = completedGroupTask.GroupName;
                                            approverType = $"Group - {completedGroupTask.GroupName}";
                                            detail = new ApproverDetail();
                                            if (completedGroupTask.ActionUserId.HasValue)
                                            {
                                                var owner = utility.GetUserByUserId(completedGroupTask.ActionUserId.Value);
                                                string taskOwner = $"{owner.DisplayName}({owner.UserName})";

                                                detail.TaskOwner = taskOwner;//utility.GetUserByUserId(completedGroupTask.ActionUserId.Value).DisplayName;
                                            }
                                            else
                                            {
                                                detail.TaskOwner = completedGroupTask.GroupName;
                                                approver.IsGroupApproval = true;
                                            }

                                            //  detail.TaskOwner = taskOwner;
                                            detail.Action = completedGroupTask.Action;
                                            detail.ActionDate = actionDate;
                                            detail.Comments = comments;
                                            detail.DelegatedBy = delegatedBy;
                                            detail.DelegatedFrom = delegatedFrom;
                                            detail.DelegationComments = delegationComments;
                                            detail.ApprovalState = workflowStateId;
                                            detail.Remarks = remarks;
                                            approver.ApproverDetails.Add(detail);
                                        }
                                        else
                                        {
                                            var NoncompletedInActiveGroupTask = tasks.FirstOrDefault(item => !item.IsActive && item.IsCompleted && item.IsGroupTask);

                                            if (NoncompletedInActiveGroupTask != null)
                                            {

                                                if (currentApprover.RequiresApprovalFromAll)
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    var owner = utility.GetGroupById(NoncompletedInActiveGroupTask.ActionGroupId.Value);
                                                    remarks = GetTaskSubTaskRemarks(NoncompletedInActiveGroupTask.Id);

                                                    string taskOwner = $"{owner.GroupName}";
                                                    approverName = taskOwner;

                                                    actionDate = NoncompletedInActiveGroupTask.LastModifiedDate;
                                                    comments = NoncompletedInActiveGroupTask.Comments;
                                                    if (!string.IsNullOrEmpty(NoncompletedInActiveGroupTask.DelegatedBy))
                                                    {
                                                        tempUser = User.GetByUserName(NoncompletedInActiveGroupTask.DelegatedBy);
                                                        if (tempUser != null)
                                                        {
                                                            delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                                        }
                                                        else
                                                        {
                                                            delegatedBy = NoncompletedInActiveGroupTask.DelegatedBy;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        delegatedBy = string.Empty;
                                                    }

                                                    if (NoncompletedInActiveGroupTask.DelegatedFrom.HasValue)
                                                    {
                                                        var downer = utility.GetUserByUserId(NoncompletedInActiveGroupTask.DelegatedFrom.Value);
                                                        string deletefromowner = $"{downer.DisplayName}({downer.UserName})";
                                                        delegatedFrom = deletefromowner;
                                                    }
                                                    else
                                                    {
                                                        delegatedFrom = string.Empty;
                                                    }

                                                    delegationComments = NoncompletedInActiveGroupTask.DelegationComments;

                                                    var approverDetail = new ApproverDetail();
                                                    approverDetail.TaskOwner = taskOwner;
                                                    approverDetail.Action = NoncompletedInActiveGroupTask.Action;
                                                    approverDetail.ActionDate = actionDate;
                                                    approverDetail.Comments = comments;
                                                    approverDetail.DelegatedBy = delegatedBy;
                                                    approverDetail.DelegatedFrom = delegatedFrom;
                                                    approverDetail.DelegationComments = delegationComments;
                                                    approverDetail.ApprovalState = workflowStateId;
                                                    approverDetail.Remarks = remarks;
                                                    approver.ApproverDetails.Add(approverDetail);
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var pendingTask = tasks.Where(item => item.IsActive && !item.IsCompleted && !item.IsGroupTask).OrderByDescending(item => item.LastModifiedDate).FirstOrDefault();

                                    if (pendingTask != null)
                                    {
                                        if (currentApprover.RequiresApprovalFromAll)
                                        {
                                            List<string> approverNames = new List<string>();

                                            var allTasks = tasks.Where(item => item.IsActive);
                                            foreach (Task item in allTasks)
                                            {
                                                var owner = utility.GetUserByUserId(item.ActionUserId.Value);
                                                string taskOwner = $"{owner.DisplayName}({owner.UserName})";

                                                approverNames.Add(taskOwner);

                                                actionDate = null;
                                                comments = item.Comments;
                                                remarks = GetTaskSubTaskRemarks(item.Id);
                                                if (!string.IsNullOrEmpty(item.DelegatedBy))
                                                {
                                                    tempUser = User.GetByUserName(item.DelegatedBy);
                                                    if (tempUser != null)
                                                    {
                                                        delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                                    }
                                                    else
                                                    {
                                                        delegatedBy = item.DelegatedBy;
                                                    }
                                                }
                                                else
                                                {
                                                    delegatedBy = string.Empty;
                                                }

                                                // delegatedFrom = item.DelegatedFrom.HasValue ? utility.GetUserByUserId(item.DelegatedFrom.Value).DisplayName : string.Empty;
                                                if (item.DelegatedFrom.HasValue)
                                                {
                                                    var downer = utility.GetUserByUserId(item.DelegatedFrom.Value);
                                                    string deletefromowner = $"{downer.DisplayName}({downer.UserName})";


                                                    delegatedFrom = deletefromowner;
                                                }
                                                else
                                                {
                                                    delegatedFrom = string.Empty;
                                                }

                                                delegationComments = item.DelegationComments;

                                                ApproverDetail detail = new ApproverDetail();
                                                detail.TaskOwner = taskOwner;
                                                detail.Action = item.Action;
                                                detail.ActionDate = actionDate;
                                                detail.Comments = comments;
                                                detail.DelegatedBy = delegatedBy;
                                                detail.DelegatedFrom = delegatedFrom;
                                                detail.DelegationComments = delegationComments;
                                                detail.ApprovalState = workflowStateId;
                                                detail.Remarks = remarks;
                                                approver.ApproverDetails.Add(detail);
                                            }

                                            approverName = string.Join(",", approverNames);
                                        }
                                        else
                                        {
                                            /*   string taskOwner = utility.GetUserByUserId(pendingTask.ActionUserId.Value).DisplayName*/
                                            ;
                                            var owner = utility.GetUserByUserId(pendingTask.ActionUserId.Value);
                                            string taskOwner = $"{owner.DisplayName}({owner.UserName})";
                                            approverName = taskOwner;

                                            actionDate = null;
                                            comments = pendingTask.Comments;
                                            remarks = GetTaskSubTaskRemarks(pendingTask.Id);
                                            if (!string.IsNullOrEmpty(pendingTask.DelegatedBy))
                                            {
                                                tempUser = User.GetByUserName(pendingTask.DelegatedBy);
                                                if (tempUser != null)
                                                {
                                                    delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                                }
                                                else
                                                {
                                                    delegatedBy = pendingTask.DelegatedBy;
                                                }
                                            }
                                            else
                                            {
                                                delegatedBy = string.Empty;
                                            }
                                            // delegatedFrom = pendingTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(pendingTask.DelegatedFrom.Value).DisplayName : string.Empty;

                                            if (pendingTask.DelegatedFrom.HasValue)
                                            {
                                                var downer = utility.GetUserByUserId(pendingTask.DelegatedFrom.Value);
                                                string deletefromowner = $"{downer.DisplayName}({downer.UserName})";


                                                delegatedFrom = deletefromowner;
                                            }
                                            else
                                            {
                                                delegatedFrom = string.Empty;
                                            }
                                            delegationComments = pendingTask.DelegationComments;

                                            ApproverDetail detail = new ApproverDetail();
                                            detail.TaskOwner = taskOwner;
                                            detail.Action = pendingTask.Action;
                                            detail.ActionDate = actionDate;
                                            detail.Comments = comments;
                                            detail.DelegatedBy = delegatedBy;
                                            detail.DelegatedFrom = delegatedFrom;
                                            detail.DelegationComments = delegationComments;
                                            detail.ApprovalState = workflowStateId;
                                            detail.Remarks = remarks;
                                            approver.ApproverDetails.Add(detail);
                                        }
                                    }
                                    else
                                    {
                                        var completedTask = tasks.Where(item => item.IsActive && item.IsCompleted && !item.IsGroupTask).OrderByDescending(item => item.LastModifiedDate).FirstOrDefault();

                                        if (completedTask != null)
                                        {
                                            if (currentApprover.RequiresApprovalFromAll)
                                            {
                                                List<string> approverNames = new List<string>();

                                                var allTasks = tasks.Where(item => item.IsActive);
                                                foreach (Task item in allTasks)
                                                {
                                                    var owner = utility.GetUserByUserId(item.ActionUserId.Value);
                                                    string taskOwner = $"{owner.DisplayName}({owner.UserName})";

                                                    // string taskOwner = utility.GetUserByUserId(item.ActionUserId.Value).DisplayName;
                                                    approverNames.Add(taskOwner);

                                                    actionDate = item.LastModifiedDate;
                                                    comments = item.Comments;
                                                    remarks = GetTaskSubTaskRemarks(item.Id);
                                                    if (!string.IsNullOrEmpty(item.DelegatedBy))
                                                    {
                                                        tempUser = User.GetByUserName(item.DelegatedBy);
                                                        if (tempUser != null)
                                                        {
                                                            delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                                        }
                                                        else
                                                        {
                                                            delegatedBy = item.DelegatedBy;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        delegatedBy = string.Empty;
                                                    }
                                                    // delegatedFrom = item.DelegatedFrom.HasValue ? utility.GetUserByUserId(item.DelegatedFrom.Value).DisplayName : string.Empty;

                                                    if (item.DelegatedFrom.HasValue)
                                                    {
                                                        var downer = utility.GetUserByUserId(item.DelegatedFrom.Value);
                                                        string deletefromowner = $"{downer.DisplayName}({downer.UserName})";
                                                        delegatedFrom = deletefromowner;
                                                    }
                                                    else
                                                    {
                                                        delegatedFrom = string.Empty;
                                                    }
                                                    delegationComments = item.DelegationComments;

                                                    ApproverDetail detail = new ApproverDetail();
                                                    detail.TaskOwner = taskOwner;
                                                    detail.Action = item.Action;
                                                    detail.ActionDate = actionDate;
                                                    detail.Comments = comments;
                                                    detail.DelegatedBy = delegatedBy;
                                                    detail.DelegatedFrom = delegatedFrom;
                                                    detail.DelegationComments = delegationComments;
                                                    detail.ApprovalState = workflowStateId;
                                                    detail.Remarks = remarks;
                                                    approver.ApproverDetails.Add(detail);
                                                }

                                                approverName = string.Join(",", approverNames);
                                            }
                                            else
                                            {
                                                var owner = utility.GetUserByUserId(completedTask.ActionUserId.Value);
                                                string taskOwner = $"{owner.DisplayName}({owner.UserName})";

                                                //string taskOwner = utility.GetUserByUserId(completedTask.ActionUserId.Value).DisplayName;
                                                approverName = taskOwner;

                                                actionDate = completedTask.LastModifiedDate;
                                                comments = completedTask.Comments;
                                                remarks = GetTaskSubTaskRemarks(completedTask.Id);
                                                if (!string.IsNullOrEmpty(completedTask.DelegatedBy))
                                                {
                                                    tempUser = User.GetByUserName(completedTask.DelegatedBy);
                                                    if (tempUser != null)
                                                    {
                                                        delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                                    }
                                                    else
                                                    {
                                                        delegatedBy = completedTask.DelegatedBy;
                                                    }
                                                }
                                                else
                                                {
                                                    delegatedBy = string.Empty;
                                                }
                                                //  delegatedFrom = completedTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(completedTask.DelegatedFrom.Value).DisplayName : string.Empty;

                                                if (completedTask.DelegatedFrom.HasValue)
                                                {
                                                    var downer = utility.GetUserByUserId(completedTask.DelegatedFrom.Value);
                                                    string deletefromowner = $"{downer.DisplayName}({downer.UserName})";
                                                    delegatedFrom = deletefromowner;
                                                }
                                                else
                                                {
                                                    delegatedFrom = string.Empty;
                                                }

                                                delegationComments = completedTask.DelegationComments;

                                                ApproverDetail detail = new ApproverDetail();
                                                detail.TaskOwner = taskOwner;
                                                detail.Action = completedTask.Action;
                                                detail.ActionDate = actionDate;
                                                detail.Comments = comments;
                                                detail.DelegatedBy = delegatedBy;
                                                detail.DelegatedFrom = delegatedFrom;
                                                detail.DelegationComments = delegationComments;
                                                detail.ApprovalState = workflowStateId;
                                                detail.Remarks = remarks;
                                                approver.ApproverDetails.Add(detail);
                                            }
                                        }
                                        else
                                        {
                                            var autoCompletedTask = tasks.FirstOrDefault(item => !item.IsActive && !item.IsCompleted && !item.IsGroupTask);

                                            if (autoCompletedTask != null)
                                            {
                                                if (completedTask == null && pendingTask == null)
                                                {
                                                    if (currentApprover.RequiresApprovalFromAll)
                                                    {
                                                        List<string> approverNames = new List<string>();

                                                        var allTasks = tasks.Where(item => item.IsActive);
                                                        foreach (Task item in allTasks)
                                                        {
                                                            remarks = GetTaskSubTaskRemarks(item.Id);
                                                            //string taskOwner = utility.GetUserByUserId(item.ActionUserId.Value).DisplayName;
                                                            var owner = utility.GetUserByUserId(item.ActionUserId.Value);
                                                            string taskOwner = $"{owner.DisplayName}({owner.UserName})";

                                                            approverNames.Add(taskOwner);

                                                            actionDate = item.LastModifiedDate;
                                                            comments = item.Comments;
                                                            if (!string.IsNullOrEmpty(item.DelegatedBy))
                                                            {
                                                                tempUser = User.GetByUserName(item.DelegatedBy);
                                                                if (tempUser != null)
                                                                {
                                                                    delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                                                }
                                                                else
                                                                {
                                                                    delegatedBy = item.DelegatedBy;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                delegatedBy = string.Empty;
                                                            }
                                                            // delegatedFrom = item.DelegatedFrom.HasValue ? utility.GetUserByUserId(item.DelegatedFrom.Value).DisplayName : string.Empty;


                                                            if (item.DelegatedFrom.HasValue)
                                                            {
                                                                var downer = utility.GetUserByUserId(item.DelegatedFrom.Value);
                                                                string deletefromowner = $"{downer.DisplayName}({downer.UserName})";
                                                                delegatedFrom = deletefromowner;
                                                            }
                                                            else
                                                            {
                                                                delegatedFrom = string.Empty;
                                                            }

                                                            delegationComments = item.DelegationComments;

                                                            ApproverDetail detail = new ApproverDetail();
                                                            detail.TaskOwner = taskOwner;
                                                            detail.Action = item.Action;
                                                            detail.ActionDate = actionDate;
                                                            detail.Comments = comments;
                                                            detail.DelegatedBy = delegatedBy;
                                                            detail.DelegatedFrom = delegatedFrom;
                                                            detail.DelegationComments = delegationComments;
                                                            detail.ApprovalState = workflowStateId;
                                                            detail.Remarks = remarks;
                                                            approver.ApproverDetails.Add(detail);
                                                        }

                                                        approverName = string.Join(",", approverNames);
                                                    }
                                                    else
                                                    {
                                                        var owner = utility.GetUserByUserId(autoCompletedTask.ActionUserId.Value);
                                                        string taskOwner = $"{owner.DisplayName}({owner.UserName})";

                                                        // string taskOwner = utility.GetUserByUserId(autoCompletedTask.ActionUserId.Value).DisplayName;
                                                        approverName = taskOwner;

                                                        actionDate = autoCompletedTask.LastModifiedDate;
                                                        comments = autoCompletedTask.Comments;
                                                        remarks = GetTaskSubTaskRemarks(autoCompletedTask.Id);
                                                        if (!string.IsNullOrEmpty(autoCompletedTask.DelegatedBy))
                                                        {
                                                            tempUser = User.GetByUserName(autoCompletedTask.DelegatedBy);
                                                            if (tempUser != null)
                                                            {
                                                                delegatedBy = $"{tempUser.DisplayName}({tempUser.UserName})";
                                                            }
                                                            else
                                                            {
                                                                delegatedBy = autoCompletedTask.DelegatedBy;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            delegatedBy = string.Empty;
                                                        }
                                                        // delegatedFrom = autoCompletedTask.DelegatedFrom.HasValue ? utility.GetUserByUserId(autoCompletedTask.DelegatedFrom.Value).DisplayName : string.Empty;
                                                        if (autoCompletedTask.DelegatedFrom.HasValue)
                                                        {
                                                            var downer = utility.GetUserByUserId(autoCompletedTask.DelegatedFrom.Value);
                                                            string deletefromowner = $"{downer.DisplayName}({downer.UserName})";
                                                            delegatedFrom = deletefromowner;
                                                        }
                                                        else
                                                        {
                                                            delegatedFrom = string.Empty;
                                                        }

                                                        delegationComments = autoCompletedTask.DelegationComments;

                                                        ApproverDetail detail = new ApproverDetail();
                                                        detail.TaskOwner = taskOwner;
                                                        detail.Action = autoCompletedTask.Action;
                                                        detail.ActionDate = actionDate;
                                                        detail.Comments = comments;
                                                        detail.DelegatedBy = delegatedBy;
                                                        detail.DelegatedFrom = delegatedFrom;
                                                        detail.DelegationComments = delegationComments;
                                                        detail.ApprovalState = workflowStateId;
                                                        detail.Remarks = remarks;
                                                        approver.ApproverDetails.Add(detail);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                approverName = User.GetById(task.ActionUserId.Value)?.DisplayName;
                                            }
                                        }
                                    }
                                }
                            }

                            bool isPending = tasks.Any(item => item.IsActive && !item.IsCompleted);
                            bool isRejected = tasks.Any(item => item.Action == StatusType.Rejected);
                            bool isWithdrawn = tasks.Any(item => item.Action == StatusType.Withdrawn);

                            if (isPending)
                            {
                                approver.Action = StatusType.Pending;
                                if (tasks.Any(item => item.IsActive && item.Action == StatusType.Approved))
                                {
                                    approver.Action = StatusType.Approved;
                                }
                            }
                            else if (isRejected)
                            {
                                approver.Action = StatusType.Rejected;
                            }
                            else if (isWithdrawn)
                            {
                                approver.Action = StatusType.Withdrawn;
                            }
                            else
                            {
                                approver.Action = StatusType.Approved;
                            }
                            bool showApprover = true;
                            if (!isBulkRequest)
                            {
                                if (adhocRequest != null && accessRequest.AccountType == AccountType.User &&
                                    accessRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal && adhocRequest.AdhocActionType == AdHocActionType.NewAccount
                                    && string.IsNullOrWhiteSpace(GetRequesterCommentForAD(adhocRequest.GetEntitlementDetail(), adhocRequest.ApplicationType))
                                    )
                                {
                                    var group = Group.GetById(MasterConfigHelper.WintelGroupId);
                                    if (group != null && approverType == $"Group - {group.GroupName}" && approverName.Equals(group.GroupName, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        showApprover = false;
                                    }
                                }
                            }
                            else
                            {
                                if (bulkRequest != null && bulkRequest.AdhocActionType == AdHocActionType.NewAccount && bulkRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                                {
                                    var adHocADRequest = (AdHocADRequest)bulkRequest.GetEntitlementDetail();
                                    if (adHocADRequest != null && string.IsNullOrWhiteSpace(adHocADRequest.RequesterComment))
                                    {
                                        var group = Group.GetById(MasterConfigHelper.WintelGroupId);
                                        if (group != null && workflowStateId == 2 && approverType == $"Group - {group.GroupName}" && approverName.Equals(group.GroupName, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            showApprover = false;
                                        }
                                    }

                                }
                            }
                            approver.ApprovalState = workflowStateId;
                            approver.ApproverType = approverType;
                            approver.ApproverName = approverName;

                            if (showApprover)
                                approvers.Add(approver);

                            if (approver.Action == StatusType.Rejected || approver.Action == StatusType.Withdrawn)
                            {
                                break;
                            }
                        }
                    }
                }

                if (!isBulkRequest && accessRequest.ApplicationType == ApplicationType.InhouseSystems && (accessRequest.RequestSource == AccessRequestSource.OffBoarding || accessRequest.RequestSource == AccessRequestSource.OUChange || accessRequest.RequestSource == AccessRequestSource.UndoEvent))
                {
                    approvers = approvers.Where(x => x.IsActionOnlyRequired).ToList();
                }

                if (accessRequest != null && accessRequest.ProgressStatus == AccessRequestProgress.Withdrawn && accessRequest.RequestStatus == AccessRequestStatus.Withdrawn)
                {
                    approvers = approvers.Where(x => x.Action != StatusType.NotInitiated).OrderBy(x => x.ApprovalState).ToList();
                }
            }
            catch (Exception ex)
            {
                string message = "Error while getting approval chain for the request - " + requestId.ToString();
                DILog.ProvisioningLogger.WriteLog(requestId, null, LogLevelType.Exception, message, ex);
            }

            return approvers;
        }
        private string GetRequesterCommentForAD(IEntitlementDetail entitlementDetail, ApplicationType applicationType)
        {
            if (applicationType == ApplicationType.ADSingaporePowerLocal)
            {
                var ent = entitlementDetail as AdHocADRequest;
                if (ent != null) { return ent.RequesterComment; }
            }
            else if (applicationType == ApplicationType.SAPLocal)
            {
                var ent = entitlementDetail as AdHocSAPLocalRequest;
                if (ent != null) { return ent.RequesterComment; }
            }
            return string.Empty;
        }
        List<AccessRequestReviewInfo> GetReviewerByTrigger(EmployeeEventTrigger trigger, WorkflowConfigurationDetail currentApprover)
        {
            List<AccessRequestReviewInfo> reviewers = new List<AccessRequestReviewInfo>();

            //look for role based approval
            if (currentApprover.ApprovalUserType == ApprovalUserType.Role)
            {
                var user = User.GetById(trigger.UserId);

                //section head approval
                if (currentApprover.Approver == (byte)RoleType.RO)
                {
                    long reviewer = 0;

                    var ro = User.GetROByUser(user.Id);

                    if (ro != null)
                    {
                        reviewer = ro.Id;
                    }

                    if (reviewer != 0)
                    {
                        AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                        reviewInfo.ReviewerId = reviewer;
                        reviewInfo.IsGroup = false;
                        reviewers.Add(reviewInfo);
                    }
                    else
                    {
                        var reviewerIds = utility.GetUsersByGroupId(MasterConfigHelper.ITSecurityGroupId);

                        foreach (int reviewerId in reviewerIds)
                        {
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = reviewerId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                            reviewers.Add(reviewInfo);
                        }
                    }
                }
            }

            return reviewers;
        }

        bool isSingaporePowerLocalADRequestRequiredWintelApproval(AdHocAccessRequest adHocAccessRequest, BulkRequest bulkRequest, Group group)
        {
            var isAD = false;
            if ((bulkRequest != null && bulkRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal) ||
                (adHocAccessRequest != null && adHocAccessRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                )
            {
                isAD = true;
            }

            if (group != null && group.Id == MasterConfigHelper.WintelGroupId && isAD)
            {
                AdHocADRequest adHocADRequest = null;
                if (bulkRequest == null && adHocAccessRequest != null && adHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount && adHocAccessRequest.AccountType == AccountType.User)
                {
                    adHocADRequest = (AdHocADRequest)adHocAccessRequest.GetEntitlementDetail();
                }

                if (bulkRequest != null && bulkRequest.AdhocActionType == AdHocActionType.NewAccount && bulkRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                {
                    adHocADRequest = (AdHocADRequest)bulkRequest.GetEntitlementDetail();
                }

                if (adHocADRequest != null && string.IsNullOrWhiteSpace(adHocADRequest.RequesterComment))
                {
                    return true;
                }
            }
            return false;
        }

        List<AccessRequestReviewInfo> GetGroupReviewers()
        {
            var groupId = MasterConfigHelper.ITSecurityGroupId;
            List<AccessRequestReviewInfo> lstGroupReviewers = new List<AccessRequestReviewInfo>();

            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
            reviewInfo.ReviewerId = groupId;
            reviewInfo.GroupId = groupId;
            reviewInfo.IsGroup = true;
            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
            lstGroupReviewers.Add(reviewInfo);

            //var reviewerIds = utility.GetUsersByGroupId(groupId);

            //foreach (int reviewerId in reviewerIds)
            //{
            //    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
            //    reviewInfo.ReviewerId = reviewerId;
            //    reviewInfo.GroupId = groupId;
            //    reviewInfo.IsGroup = true;
            //    reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
            //    lstGroupReviewers.Add(reviewInfo);
            //}
            return lstGroupReviewers;
        }

        public List<AccessRequestReviewInfo> GetProvisioningReviewersInformation(AccessRequest accessRequest, AdHocAccessRequest adhocRequest, BulkRequest bulkRequest, WorkflowConfigurationDetail currentApprover)
        {
            try
            {
                LogDebugStep("GetProvisioningReviewersInformation");
                List<AccessRequestReviewInfo> reviewers = new List<AccessRequestReviewInfo>();

                EndPoint endpoint = null;
                long userId = 0;
                long supervisorId = 0;

                if (bulkRequest == null)
                {
                    endpoint = adhocRequest.EndPoint;
                    userId = accessRequest.UserId;

                    if (adhocRequest.SupervisorChanged)
                    {
                        supervisorId = adhocRequest.SelectedSupervisor.HasValue ? adhocRequest.SelectedSupervisor.Value : 0;
                    }
                }
                else
                {
                    endpoint = bulkRequest.EndPoint;
                    userId = bulkRequest.BulkRequestDetails.FirstOrDefault().UserId;

                    if (bulkRequest.SupervisorChanged)
                    {
                        supervisorId = bulkRequest.SelectedSupervisor.HasValue ? bulkRequest.SelectedSupervisor.Value : 0;
                    }
                }

                LogDebugStep($"currentApprover.ApprovalUserType: {currentApprover.ApprovalUserType}, currentApprover.Approver:{currentApprover.Approver}");
                //look for role based approval
                if (currentApprover.ApprovalUserType == ApprovalUserType.Role)
                {
                    var user = User.GetById(userId);

                    //section head approval
                    if (currentApprover.Approver == (byte)RoleType.RO)

                    {
                        long reviewer = 0;

                        if (supervisorId != 0)
                        {
                            reviewer = supervisorId;
                        }
                        else
                        {
                            var ro = User.GetROByUser(userId);

                            if (ro != null && ro.IsActive)
                            {
                                reviewer = ro.Id;
                            }
                        }

                        if (reviewer != 0)
                        {
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = reviewer;
                            reviewInfo.IsGroup = false;
                            reviewers.Add(reviewInfo);
                        }
                        else
                        {
                            reviewers.AddRange(GetGroupReviewers());
                        }
                    }
                    //department head approval
                    else if (currentApprover.Approver == (byte)RoleType.CO)
                    {
                        long reviewer = 0;
                        bool reviewerRequired = true;
                        if (!user.FullTimeEmployee)
                        {
                            if (accessRequest != null && accessRequest.ApplicationType == ApplicationType.Office365 && accessRequest.AccountType == AccountType.User
                                && (accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount || accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
                            {
                                if (accessRequest.BulkRequestId.HasValue)
                                {
                                    var o365RequestDetails = BulkRequest.GetById(accessRequest.BulkRequestId.Value, true);
                                    if (o365RequestDetails != null)
                                    {
                                        var bulkRequestDetails = o365RequestDetails.GetEntitlementDetail() as AdHocO365Request;
                                        if (bulkRequestDetails != null && bulkRequestDetails.ExternalEmailAction != ExternalEmailActionType.Enable)
                                        {
                                            //If user select the ExternalEmailAction enable then it should route to CO.Else No CO approval required
                                            reviewerRequired = false;
                                        }
                                    }
                                }
                                else
                                {
                                    var o365RequestDetails = accessRequest.AdHocAccessRequest.GetEntitlementDetail() as AdHocO365Request;
                                    if (o365RequestDetails != null)
                                    {
                                        if (o365RequestDetails.ExternalEmailAction != ExternalEmailActionType.Enable)
                                        {
                                            //If user select the ExternalEmailAction enable then it should route to CO.Else No CO approval required
                                            reviewerRequired = false;
                                        }
                                    }
                                }

                            }
                            else if (bulkRequest != null && bulkRequest.ApplicationType == ApplicationType.Office365
                                && (bulkRequest.AdhocActionType == AdHocActionType.NewAccount || bulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
                            {
                                var o365RequestDetails = BulkRequest.GetById(bulkRequest.Id, true);
                                if (o365RequestDetails != null)
                                {
                                    var bulkRequestDetails = o365RequestDetails.GetEntitlementDetail() as AdHocO365Request;
                                    if (bulkRequestDetails != null && bulkRequestDetails.ExternalEmailAction != ExternalEmailActionType.Enable)
                                    {
                                        //If user select the ExternalEmailAction enable then it should route to CO.Else No CO approval required
                                        reviewerRequired = false;
                                    }
                                }
                            }
                        }
                        if (reviewerRequired)
                        {
                            var co = User.GetCOByUser(userId);
                            if (co != null && co.IsActive)
                            {
                                reviewer = co.Id;
                            }

                            if (reviewer != 0)
                            {
                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.ReviewerId = reviewer;
                                reviewInfo.IsGroup = false;
                                reviewers.Add(reviewInfo);
                            }
                            else
                            {
                                reviewers.AddRange(GetGroupReviewers());
                            }
                        }

                    }
                    //ApplicationOwner approval
                    else if (currentApprover.Approver == (byte)RoleType.ApplicationOwner)
                    {
                        long reviewer = 0;

                        if (endpoint.OwnerUserId.HasValue)
                        {
                            User owner = User.GetById(endpoint.OwnerUserId.Value);
                            if (owner != null && owner.IsActive)
                            {
                                reviewer = endpoint.OwnerUserId.Value;
                            }
                        }

                        if (reviewer != 0)
                        {
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = reviewer;
                            reviewInfo.IsGroup = false;
                            reviewers.Add(reviewInfo);
                        }
                        else
                        {
                            reviewers.AddRange(GetGroupReviewers());
                        }
                    }
                    else if (currentApprover.Approver == (byte)RoleType.HeadOfSection)
                    {
                        var reviewInfo = GetAccessRequestReviewInfo(user.Id, user.SectionCode);
                        if (reviewInfo != null)
                            reviewers.Add(reviewInfo);
                        else
                            reviewers.AddRange(GetGroupReviewers());
                    }
                    else if (currentApprover.Approver == (byte)RoleType.HeadOfBranch)
                    {
                        var reviewInfo = GetAccessRequestReviewInfo(user.Id, user.BranchCode);
                        if (reviewInfo != null)
                            reviewers.Add(reviewInfo);
                        else
                            reviewers.AddRange(GetGroupReviewers());
                    }
                    else if (currentApprover.Approver == (byte)RoleType.HeadOfDepartment)
                    {
                        LogDebugStep("currentApprover.Approver == (byte)RoleType.HeadOfDepartment");
                        var reviewInfo = GetAccessRequestReviewInfo(user.Id, user.DepartmentCode);
                        if (reviewInfo != null)
                            reviewers.Add(reviewInfo);
                        else
                            reviewers.AddRange(GetGroupReviewers());
                    }
                }
                //look for group based approval
                else if (currentApprover.ApprovalUserType == ApprovalUserType.Group)
                {

                    var group = Group.GetById(currentApprover.Approver);

                    if (currentApprover.IsMockGroup)
                    {

                    }

                    if (isSingaporePowerLocalADRequestRequiredWintelApproval(adhocRequest, bulkRequest, group))
                        return new List<AccessRequestReviewInfo>();

                    var reviewerIds = utility.GetUsersByGroupId(currentApprover.Approver);

                    if (reviewerIds.Count > 0)
                    {

                        foreach (int reviewerId in reviewerIds)
                        {
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = reviewerId;
                            reviewInfo.GroupId = currentApprover.Approver;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
                            reviewers.Add(reviewInfo);
                        }
                    }
                    else
                    {
                        var itReviewerIds = utility.GetUsersByGroupId(MasterConfigHelper.ITSecurityGroupId);

                        foreach (int reviewerId in itReviewerIds)
                        {
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = reviewerId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                            reviewers.Add(reviewInfo);
                        }
                    }
                }
                //look for user based approval
                else if (currentApprover.ApprovalUserType == ApprovalUserType.User)
                {
                    User user = User.GetById(currentApprover.Approver);

                    if (user != null && user.IsActive)
                    {
                        AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                        reviewInfo.ReviewerId = currentApprover.Approver;
                        reviewInfo.IsGroup = false;
                        reviewers.Add(reviewInfo);
                    }
                    else
                    {
                        var reviewerIds = utility.GetUsersByGroupId(MasterConfigHelper.ITSecurityGroupId);

                        foreach (int reviewerId in reviewerIds)
                        {
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = reviewerId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                            reviewers.Add(reviewInfo);
                        }
                    }

                }
                //look for individual system config based approval
                else if (currentApprover.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                {
                    if (currentApprover.Approver == (byte)RoleType.TCodeOwner)
                    {
                        reviewers = this.GetTCodeReviewerBySystemConfiguration(accessRequest, adhocRequest, bulkRequest);
                    }
                    else
                    {
                        reviewers = this.GetRoleReviewerBySystemConfiguration(accessRequest, adhocRequest, bulkRequest, currentApprover);
                    }
                }

                return reviewers;
            }
            catch (Exception ex)
            {
                LogDebugStep($"GetProvisioningReviewersInformation: Exception: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw ex;
            }
        }

        AccessRequestReviewInfo GetAccessRequestReviewInfo(long userId, string orgCode)
        {
            LogDebugStep($"GetAccessRequestReviewInfo, orgCode:{orgCode}, userId: {userId}");
            AccessRequestReviewInfo reviewInfo = null;
            if (string.IsNullOrWhiteSpace(orgCode))
                return reviewInfo;

            var nOrg = NOrganisation.GetNOrganisationByCode(orgCode);
            LogDebugStep($"nOrg == null: {nOrg == null}");
            if (nOrg != null)
            {
                var nOrgHead = NOrganisationHead.GetByOrganisationId(nOrg.Id);
                LogDebugStep($"nOrgHead == null: {nOrgHead == null}");
                if (nOrgHead == null || !nOrgHead.HeadId.HasValue)
                    return reviewInfo;

                var head = User.GetById(nOrgHead.HeadId.Value);
                LogDebugStep($"head == null: {head == null}");
                if (head != null && head.IsActive)
                {
                    reviewInfo = new AccessRequestReviewInfo();
                    reviewInfo.ReviewerId = head.Id;
                    reviewInfo.IsGroup = false;
                }
            }
            return reviewInfo;
        }

        List<AccessRequestReviewInfo> GetTCodeReviewerBySystemConfiguration(AccessRequest accessRequest, AdHocAccessRequest adhocRequest, BulkRequest bulkRequest)
        {
            utility.LogDebugStep($"GetTCodeReviewerBySystemConfiguration");
            var reviewers = new List<AccessRequestReviewInfo>();

            var isBulkRequest = bulkRequest != null && bulkRequest.Id > 0;
            long requestId = 0;
            ApplicationType applicationType;
            IEntitlementDetail sapDetail;
            long endpointSystemId = 0;

            if (!isBulkRequest)
            {
                requestId = accessRequest.Id;
                applicationType = accessRequest.ApplicationType;
                sapDetail = adhocRequest.GetEntitlementDetail();
                endpointSystemId = accessRequest.EndpointSystemId.Value;
            }
            else
            {
                requestId = bulkRequest.Id;
                applicationType = bulkRequest.ApplicationType;
                sapDetail = bulkRequest.GetEntitlementDetail();
                endpointSystemId = bulkRequest.EndpointSystemId;
            }
            if (requestId == 0 || sapDetail == null || endpointSystemId == 0)
                return reviewers;

            utility.LogDebugStep($"GetTCodeReviewerBySystemConfiguration: {requestId}");
            var businessRoles = new List<string>();
            var technicalRoles = new List<string>();
            var IndirecttechnicalroleofPosition = new List<string>();
            var dependentIndirecttechnicalroleofPosition = new List<string>();
            long dependentSystemId = 0;
            var criticalTCodes = new List<TransactionAndOwner>();
            var applicationName = string.Empty;
            var position = string.Empty;
            if (sapDetail is AdHocMSSLRequest)
            {
                var msslRequest = sapDetail as AdHocMSSLRequest;
                criticalTCodes = SAPXLMSSLCriticalTCodeMatrix.GetAll().Select(p => new TransactionAndOwner { Transaction = p.Transaction, Description = p.TransactionDescription, Owner = p.TransactionOwner }).ToList();
                businessRoles = UtilityMethods.GetListFromString(msslRequest.Roles);
                applicationName = applicationType == ApplicationType.MSSL_ISU ? Constants.MSSLMSP : Constants.MSSLMCP;
                technicalRoles = SAPXLMSSLRolesMatrix.GetTechnicalRolesForMSSL(businessRoles, applicationName);
                if (!string.IsNullOrWhiteSpace(msslRequest.Position))
                {
                    position = msslRequest.Position;
                    //to pass indirect role logic 
                    var Postionmatrix = SAPXLMSSLPositionsMatrix.GetMSSLPositionMatrixByPosition(msslRequest.Position);
                    if (Postionmatrix != null)
                    {
                        var positionparam = applicationType == ApplicationType.MSSL_ISU ? Postionmatrix.Position_MSP : Postionmatrix.Position_MCP;
                        if (!string.IsNullOrWhiteSpace(positionparam))
                        {
                            IndirecttechnicalroleofPosition = SAPHRP1001.GetRolesByPosition(positionparam, endpointSystemId);
                            if (IndirecttechnicalroleofPosition == null) IndirecttechnicalroleofPosition = new List<string>();
                        }

                        var dpendentEndpoint = EndPoint.GetByApplicationType(applicationType == ApplicationType.MSSL_ISU ? ApplicationType.MSSL_CRM : ApplicationType.MSSL_ISU);
                        var dependentPositionparam = applicationType == ApplicationType.MSSL_ISU ? Postionmatrix.Position_MCP : Postionmatrix.Position_MSP;
                        if (!string.IsNullOrWhiteSpace(dependentPositionparam) && dpendentEndpoint != null)
                        {
                            dependentSystemId = dpendentEndpoint.Id;
                            dependentIndirecttechnicalroleofPosition = SAPHRP1001.GetRolesByPosition(dependentPositionparam, dependentSystemId).ToList();

                            if (dependentIndirecttechnicalroleofPosition == null) dependentIndirecttechnicalroleofPosition = new List<string>();
                        }
                    }
                }
            }
            else if (sapDetail is AdHocEBSRequest)
            {
                var ebsRequest = sapDetail as AdHocEBSRequest;
                criticalTCodes = SAPEBSXLCriticalTCodesMatrix.GetAll().Select(p => new TransactionAndOwner { Transaction = p.Transaction, Description = p.TransactionDescription, Owner = p.TransactionOwner }).ToList();
                businessRoles = UtilityMethods.GetListFromString(ebsRequest.Roles);
                applicationName = applicationType == ApplicationType.EBS_ISU ? Constants.EBSISP : Constants.EBSCRP;
                technicalRoles = SAPEBSXLRoleMatrix.GetTechnicalRolesForEBS(businessRoles, applicationName);
                if (!string.IsNullOrWhiteSpace(ebsRequest.Position))
                {
                    position = ebsRequest.Position;
                    //to pass indirect role logic 
                    var Postionmatrix = SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPosition(ebsRequest.Position);
                    if (Postionmatrix != null)
                    {
                        var positionparam = applicationType == ApplicationType.EBS_ISU ? Postionmatrix.Position_ISP : Postionmatrix.Position_CRP;
                        if (!string.IsNullOrWhiteSpace(positionparam))
                        {
                            IndirecttechnicalroleofPosition = SAPHRP1001.GetRolesByPosition(positionparam, endpointSystemId).ToList();
                            if (IndirecttechnicalroleofPosition == null) IndirecttechnicalroleofPosition = new List<string>();
                        }

                        var dpendentEndpoint = EndPoint.GetByApplicationType(applicationType == ApplicationType.EBS_ISU ? ApplicationType.EBS_CRM : ApplicationType.EBS_ISU);
                        var dependentPositionparam = applicationType == ApplicationType.EBS_ISU ? Postionmatrix.Position_CRP : Postionmatrix.Position_ISP;
                        if (!string.IsNullOrWhiteSpace(dependentPositionparam) && dpendentEndpoint != null)
                        {
                            dependentSystemId = dpendentEndpoint.Id;
                            dependentIndirecttechnicalroleofPosition = SAPHRP1001.GetRolesByPosition(dependentPositionparam, dependentSystemId).ToList();
                            if (dependentIndirecttechnicalroleofPosition == null) dependentIndirecttechnicalroleofPosition = new List<string>();
                        }
                    }
                }
            }
            if (sapDetail is AdHocSAPHANARequest)
            {
                var ep = EndPoint.GetById(endpointSystemId);
                var sapHanaRequest = sapDetail as AdHocSAPHANARequest;
                criticalTCodes = SAPHANAXLCriticalTCodeMatrix.GetAllSAPHANAXLCriticalTCodesMatrixByEndpointId(endpointSystemId).Select(p => new TransactionAndOwner { Transaction = p.Transaction, Description = p.TransactionDescription, Owner = p.TransactionOwner }).ToList();
                businessRoles = UtilityMethods.GetListFromString(sapHanaRequest.Roles);
                applicationName = ep.ApplicationCode;
                technicalRoles = SAPHANAXLRolesMatrix.GetTechnicalRolesForSAPHANA(businessRoles, applicationName);
                if (!string.IsNullOrWhiteSpace(sapHanaRequest.Position))
                {
                    position = sapHanaRequest.Position;
                    //to pass indirect role logic 
                    var Postionmatrix = SAPHANAXLPositionsMatrix.GetSAPHANAPositionMatrixByPosition(endpointSystemId,sapHanaRequest.Position);
                    if (Postionmatrix != null)
                    {
                        var positionparam = Postionmatrix.Position;
                        if (!string.IsNullOrWhiteSpace(positionparam))
                        {
                            IndirecttechnicalroleofPosition = SAPHANAHRP1001.GetRolesByPosition(positionparam, endpointSystemId);
                            if (IndirecttechnicalroleofPosition == null) IndirecttechnicalroleofPosition = new List<string>();
                        }
                    }
                }
            }
            utility.LogDebugStep($"technicalRoles.Count: {technicalRoles.Count}");

            if ((technicalRoles != null && technicalRoles.Count > 0)
                ||
                (IndirecttechnicalroleofPosition != null || IndirecttechnicalroleofPosition.Count > 0)
                ||
              (dependentSystemId > 0 && (dependentIndirecttechnicalroleofPosition != null || dependentIndirecttechnicalroleofPosition.Count > 0))
                )
            {
                try
                {
                    Dictionary<string, string> tcodeOwners = new Dictionary<string, string>();
                    Dictionary<string, string> tcodeOwnersofindirectrole = new Dictionary<string, string>();
                    if (technicalRoles != null && technicalRoles.Count > 0)
                    {
                        foreach (string technicalRole in technicalRoles.Distinct())
                        {
                            var roleTCodes = new List<string>();
                            if(applicationType == ApplicationType.EBS || applicationType == ApplicationType.MSSL)
                            {
                                roleTCodes = SAPHANARoleToAuthMap.GetByRoleCode(technicalRole, endpointSystemId);
                            }
                            else
                            {
                                roleTCodes = SAPRoleToAuthMap.GetByRoleCode(technicalRole, endpointSystemId);
                            }
                            

                            foreach (string tcode in roleTCodes)
                            {
                                var criticalTCode = criticalTCodes.FirstOrDefault(item => !string.IsNullOrWhiteSpace(item.Transaction) && item.Transaction.Equals(tcode, StringComparison.CurrentCultureIgnoreCase));
                                if (criticalTCode != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(criticalTCode.Owner))
                                    {
                                        if (!tcodeOwners.Keys.Contains(criticalTCode.Owner))
                                        {
                                            tcodeOwners.Add(criticalTCode.Owner, criticalTCode.Transaction);
                                        }
                                        else
                                        {
                                            var kvp = tcodeOwners.FirstOrDefault(item => item.Key == criticalTCode.Owner);
                                            if (!kvp.Value.Contains(criticalTCode.Transaction))
                                            {
                                                tcodeOwners[criticalTCode.Owner] += ", " + criticalTCode.Transaction;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (IndirecttechnicalroleofPosition != null && IndirecttechnicalroleofPosition.Count > 0)
                    {
                        foreach (string indirecttechnicalRole in IndirecttechnicalroleofPosition.Distinct())
                        {
                            var indirectroleTCodes = new List<string>();
                            if (applicationType == ApplicationType.EBS || applicationType == ApplicationType.MSSL)
                            {
                                indirectroleTCodes = SAPHANARoleToAuthMap.GetByRoleCode(indirecttechnicalRole, endpointSystemId);
                            }
                            else
                            {
                                indirectroleTCodes = SAPRoleToAuthMap.GetByRoleCode(indirecttechnicalRole, endpointSystemId);
                            }
                            

                            foreach (string tcode in indirectroleTCodes)
                            {
                                var criticalindirectroleTCode = criticalTCodes.FirstOrDefault(item => !string.IsNullOrWhiteSpace(item.Transaction) && item.Transaction.Equals(tcode, StringComparison.CurrentCultureIgnoreCase));
                                if (criticalindirectroleTCode != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(criticalindirectroleTCode.Owner))
                                    {
                                        var desc = $"{criticalindirectroleTCode.Description}[{criticalindirectroleTCode.Transaction}]";
                                        if (!tcodeOwners.Keys.Contains(criticalindirectroleTCode.Owner))
                                        {
                                            tcodeOwners.Add(criticalindirectroleTCode.Owner, desc);
                                        }
                                        else
                                        {
                                            var kvp = tcodeOwners.FirstOrDefault(item => item.Key == criticalindirectroleTCode.Owner);
                                            if (!kvp.Value.Contains(desc))
                                            {
                                                tcodeOwners[criticalindirectroleTCode.Owner] += ", " + desc;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        utility.LogDebugStep($"tcodeOwners.Count: {tcodeOwners.Count}");
                    }
                    if (dependentIndirecttechnicalroleofPosition != null && dependentIndirecttechnicalroleofPosition.Count > 0)
                    {
                        foreach (string indirecttechnicalRole in dependentIndirecttechnicalroleofPosition.Distinct())
                        {
                            var indirectroleTCodes = SAPRoleToAuthMap.GetByRoleCode(indirecttechnicalRole, dependentSystemId);

                            foreach (string tcode in indirectroleTCodes)
                            {
                                var criticalindirectroleTCode = criticalTCodes.FirstOrDefault(item => !string.IsNullOrWhiteSpace(item.Transaction) && item.Transaction.Equals(tcode, StringComparison.CurrentCultureIgnoreCase));
                                if (criticalindirectroleTCode != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(criticalindirectroleTCode.Owner))
                                    {
                                        var desc = $"{criticalindirectroleTCode.Description}[{criticalindirectroleTCode.Transaction}]";
                                        if (!tcodeOwners.Keys.Contains(criticalindirectroleTCode.Owner))
                                        {
                                            tcodeOwners.Add(criticalindirectroleTCode.Owner, desc);
                                        }
                                        else
                                        {
                                            var kvp = tcodeOwners.FirstOrDefault(item => item.Key == criticalindirectroleTCode.Owner);
                                            if (!kvp.Value.Contains(desc))
                                            {
                                                tcodeOwners[criticalindirectroleTCode.Owner] += ", " + desc;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (KeyValuePair<string, string> tcodeOwner in tcodeOwners)
                    {
                        var owner = User.GetByEmployeeId(tcodeOwner.Key);

                        if (owner != null && owner.Id >= 0 && owner.IsActive)
                        {
                            if (reviewers != null && !reviewers.Exists(i => i.ReviewerId == owner.Id))
                            {
                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.ReviewerId = owner.Id;
                                reviewInfo.IsGroup = false;
                                reviewers.Add(reviewInfo);
                            }
                        }
                        else
                        {
                            var itSecurityId = MasterConfigHelper.ITSecurityGroupId;
                            var reviewerIds = utility.GetUsersByGroupId(itSecurityId);

                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = reviewerIds.FirstOrDefault();
                            reviewInfo.GroupId = itSecurityId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
                            reviewInfo.Remarks = string.Format("The request {0} is routed to IAM team since the critical t-code(s) {1} has no owner.", requestId, tcodeOwner.Value);
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, reviewInfo.Remarks);
                            reviewers.Add(reviewInfo);
                        }
                    }
                    utility.LogDebugStep($"reviewers.Count: {reviewers.Count}");
                }
                catch (Exception ex)
                {
                    DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                }
            }
            return reviewers;
        }

        public List<IApprover> GetRoleReviewersIApprovers(AdHocAccessRequest adhocRequest, BulkRequest bulkRequest)
        {
            List<IApprover> matrix = new List<IApprover>();
            ApplicationType appType = ApplicationType.None;
            long endpointSystemId = 0;

            if (bulkRequest == null)
            {
                appType = adhocRequest.ApplicationType;
                endpointSystemId = adhocRequest.EndpointSystemId.Value;
            }
            else
            {
                appType = bulkRequest.ApplicationType;
                endpointSystemId = bulkRequest.EndpointSystemId;
            }

            switch (appType)
            {
                case ApplicationType.HRMS_FLINS:
                    {
                        AdHocSAPHRFLINSRequest request = null;

                        if (bulkRequest == null)
                        {
                            request = (AdHocSAPHRFLINSRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocSAPHRFLINSRequest)bulkRequest.GetEntitlementDetail();
                        }

                        if (request != null)
                        {
                            foreach (AdHocSAPHRFLINSRequestDetail requestDetail in request.AdHocSAPHRFLINSRequestDetails)
                            {
                                matrix.Add(SAPHRFLINSRoleMatrix.GetFLINSMatrixByRequest(requestDetail));
                            }
                        }

                        break;
                    }

                case ApplicationType.SRM:
                    {
                        AdHocSAPSRMRequest request = null;

                        if (bulkRequest == null)
                        {
                            request = (AdHocSAPSRMRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocSAPSRMRequest)bulkRequest.GetEntitlementDetail();
                        }

                        if (request != null)
                        {
                            foreach (AdHocSAPSRMRequestDetail requestDetail in request.AdHocSAPSRMRequestDetails)
                            {
                                matrix.Add(SAPSRMMainRoleMatrix.GetSRMMatrixByRequest(requestDetail.Entitlement));
                            }
                        }

                        break;
                    }

                case ApplicationType.FLINS_BI:
                    {
                        AdHocSAPFLINSBIRequest request = null;

                        if (bulkRequest == null)
                        {
                            request = (AdHocSAPFLINSBIRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocSAPFLINSBIRequest)bulkRequest.GetEntitlementDetail();
                        }

                        if (request != null)
                        {
                            List<string> bussinessRoles = UtilityMethods.GetListFromString(request.Entitlement);

                            foreach (string bussinessRole in bussinessRoles)
                            {
                                matrix.Add(SAPBIRoleCategory.GetFLINSBIMatrixByRequest(bussinessRole));
                            }
                        }

                        break;
                    }

                case ApplicationType.SAP_PI:
                    {
                        AdHocSAPPIRequest request = null;

                        if (bulkRequest == null)
                        {
                            request = (AdHocSAPPIRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocSAPPIRequest)bulkRequest.GetEntitlementDetail();
                        }

                        if (request != null)
                        {
                            List<string> bussinessRoles = UtilityMethods.GetListFromString(request.Entitlement);

                            foreach (string bussinessRole in bussinessRoles)
                            {
                                matrix.Add(SAPPIRole.GetPIMatrixByRequest(bussinessRole));
                            }
                        }

                        break;
                    }

                case ApplicationType.SAP_GRC:
                    {
                        AdHocSAPGRCRequest request = null;

                        if (bulkRequest == null)
                        {
                            request = (AdHocSAPGRCRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocSAPGRCRequest)bulkRequest.GetEntitlementDetail();
                        }
                        if (request != null)
                        {
                            List<string> bussinessRoles = UtilityMethods.GetListFromString(request.Entitlement);

                            foreach (string bussinessRole in bussinessRoles)
                            {
                                var role = SAPGRCRole.GetGRCMatrixByRequest(bussinessRole);
                                if (role != null)
                                    matrix.Add(role);
                            }
                        }

                        break;
                    }

                case ApplicationType.GEMS:
                    {
                        AdHocGEMSRequest request = null;

                        if (bulkRequest == null)
                        {
                            request = (AdHocGEMSRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocGEMSRequest)bulkRequest.GetEntitlementDetail();
                        }

                        if (request != null)
                        {
                            List<string> bussinessRoles = UtilityMethods.GetListFromString(request.Entitlement);

                            foreach (string bussinessRole in bussinessRoles)
                            {
                                matrix.Add(GEMSRoleMatrix.GetGEMSMatrixByRequest(bussinessRole));
                            }
                        }

                        break;
                    }

                case ApplicationType.MSSL_ISU:
                case ApplicationType.MSSL_CRM:
                    {
                        string position = string.Empty;
                        string roles = string.Empty;
                        string applicationName = string.Empty;

                        if (bulkRequest == null)
                        {
                            var sapDetail = (AdHocMSSLRequest)adhocRequest.GetEntitlementDetail();
                            if (sapDetail != null)
                            {
                                roles = sapDetail.Roles;
                                position = sapDetail.Position;
                                applicationName = sapDetail.ApplicationName;
                            }
                        }
                        else
                        {
                            var sapDetail = (AdHocMSSLRequest)bulkRequest.GetEntitlementDetail();
                            if (sapDetail != null)
                            {
                                roles = sapDetail.Roles;
                                position = sapDetail.Position;
                                applicationName = sapDetail.ApplicationName;
                            }
                        }

                        if (!string.IsNullOrEmpty(position))
                        {
                            matrix.Add(SAPXLMSSLPositionsMatrix.GetMSSLPositionMatrixByPosition(position));
                        }

                        if (!string.IsNullOrEmpty(roles))
                        {
                            List<string> bussinessRoles = UtilityMethods.GetListFromString(roles);

                            foreach (string bussinessRole in bussinessRoles)
                            {
                                matrix.Add(SAPXLMSSLRolesMatrix.GetMSSLRoleMatrix(bussinessRole, applicationName));
                            }
                        }

                        break;
                    }

                case ApplicationType.EBS_ISU:
                case ApplicationType.EBS_CRM:
                    {
                        string position = string.Empty;
                        string roles = string.Empty;
                        string applicationName = string.Empty;

                        if (bulkRequest == null)
                        {
                            var sapDetail = (AdHocEBSRequest)adhocRequest.GetEntitlementDetail();
                            if (sapDetail != null)
                            {
                                roles = sapDetail.Roles;
                                position = sapDetail.Position;
                                applicationName = sapDetail.ApplicationName;
                            }
                        }
                        else
                        {
                            var sapDetail = (AdHocEBSRequest)bulkRequest.GetEntitlementDetail();
                            if (sapDetail != null)
                            {
                                roles = sapDetail.Roles;
                                position = sapDetail.Position;
                                applicationName = sapDetail.ApplicationName;
                            }
                        }

                        if (!string.IsNullOrEmpty(position))
                        {
                            matrix.Add(SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPosition(position));
                        }

                        if (!string.IsNullOrEmpty(roles))
                        {
                            List<string> bussinessRoles = UtilityMethods.GetListFromString(roles);

                            foreach (string bussinessRole in bussinessRoles)
                            {
                                matrix.Add(SAPEBSXLRoleMatrix.GetEBSRoleMatrix(bussinessRole, applicationName));
                            }
                        }

                        break;
                    }

                case ApplicationType.InhouseSystems:
                    {
                        AdHocINHRequest request = null;

                        if (bulkRequest == null)
                        {
                            request = (AdHocINHRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocINHRequest)bulkRequest.GetEntitlementDetail();
                        }
                        if (request != null)
                        {
                            List<string> bussinessRoles = UtilityMethods.GetListFromString(request.Entitlement);

                            foreach (string bussinessRole in bussinessRoles)
                            {
                                matrix.Add(InhousePermission.GetByNameAndEndpointSystemId(bussinessRole, endpointSystemId));
                            }
                        }

                        break;
                    }
                case ApplicationType.IDMS:
                    {
                        AdHocIACMRequest request = null;

                        if (bulkRequest == null)
                        {
                            request = (AdHocIACMRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocIACMRequest)bulkRequest.GetEntitlementDetail();
                        }

                        if (request != null)
                        {
                            foreach (AdHocIACMRequestDetail requestDetail in request.AdHocIACMRequestDetails)
                            {
                                IApprover tmpApprover = new GEMSRoleMatrix();
                                if (requestDetail.EntitlementType == EntitlementType.Role)
                                {
                                    var roleInfo = IACMRoleInfo.GetByRoleName(requestDetail.Entitlement);
                                    if (roleInfo != null)
                                    {
                                        if (roleInfo.Level1Approver != null && !Convert.ToBoolean(roleInfo.IsLevel1Group))
                                        {
                                            var user = User.GetById(Convert.ToInt64(roleInfo.Level1Approver));
                                            if (user != null)
                                            {
                                                tmpApprover.Level1Approver = user.EmployeeId;
                                                tmpApprover.Level1ApproverType = ApprovalUserType.User;
                                            }
                                        }
                                        if (roleInfo.Level2Approver != null && !Convert.ToBoolean(roleInfo.IsLevel2Group))
                                        {
                                            var user = User.GetById(Convert.ToInt64(roleInfo.Level2Approver));
                                            if (user != null)
                                            {
                                                tmpApprover.Level2Approver = user.EmployeeId;
                                                tmpApprover.Level2ApproverType = ApprovalUserType.User;
                                            }
                                        }
                                    }
                                }
                                else if (requestDetail.EntitlementType == EntitlementType.Group)
                                {
                                    var groupInfo = IACMGroupInfo.GetByGroupName(requestDetail.Entitlement);
                                    if (groupInfo != null)
                                    {
                                        if (groupInfo.Level1Approver != null && !Convert.ToBoolean(groupInfo.IsLevel1Group))
                                        {
                                            var user = User.GetById(Convert.ToInt64(groupInfo.Level1Approver));
                                            if (user != null)
                                            {
                                                tmpApprover.Level1Approver = user.EmployeeId;
                                                tmpApprover.Level1ApproverType = ApprovalUserType.User;
                                            }
                                        }
                                        if (groupInfo.Level2Approver != null && !Convert.ToBoolean(groupInfo.IsLevel2Group))
                                        {
                                            var user = User.GetById(Convert.ToInt64(groupInfo.Level2Approver));
                                            if (user != null)
                                            {
                                                tmpApprover.Level2Approver = user.EmployeeId;
                                                tmpApprover.Level2ApproverType = ApprovalUserType.User;
                                            }
                                        }
                                    }
                                }
                                if (tmpApprover != null)
                                    matrix.Add(tmpApprover);
                            }
                        }
                        break;
                    }
                case ApplicationType.VPN:
                    {
                        AdHocVPNRequest request = null;

                        if (bulkRequest == null)
                        {
                            request = (AdHocVPNRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocVPNRequest)bulkRequest.GetEntitlementDetail();
                        }

                        if (request != null)
                        {
                            foreach (AdHocVPNRequestDetail requestDetail in request.AdHocVPNRequestDetails)
                            {
                                var matrixType = (VPNMatrixType)(Enum.Parse(typeof(VPNMatrixType), requestDetail.MatrixType));
                                var tmp = VPNMatrix.GetApplication(requestDetail.ApplicationName, matrixType);
                                matrix.Add(tmp);
                            }
                        }
                        break;
                    }
                case ApplicationType.MicrosoftTeams:
                    {
                        AdhocSkypeForBusinessRequest request = null;
                        if (bulkRequest == null)
                        {
                            request = (AdhocSkypeForBusinessRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdhocSkypeForBusinessRequest)bulkRequest.GetEntitlementDetail();
                        }

                        if (request != null)
                        {

                            if (request.EntitlementType == EntitlementType.ResponseGroup || adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                            {
                                foreach (var responseGroup in request.SkypeForBusinessResponseGroups)
                                {
                                    var tmp = SkypeForBusinessProvisioningMatrix.GetByName(responseGroup.Name, EntitlementType.ResponseGroup);
                                    matrix.Add(tmp);
                                }
                            }
                            //else if(request.EntitlementType == EntitlementType.Telephone || request.EntitlementType == EntitlementType.DCC
                            //    || request.EntitlementType == EntitlementType.DCCS || request.EntitlementType == EntitlementType.IDD
                            //    || request.EntitlementType == EntitlementType.Move
                            //    )
                            //{
                            //    var tmp = SkypeForBusinessProvisioningMatrix.GetSkypeforbusinessMatrixDataByEntitlementType(adhocRequest.EndpointSystemId.Value, request.EntitlementType);
                            //    matrix.Add(tmp);
                            //}

                        }
                    }

                    break;
                case ApplicationType.OneCard:
                    {
                        AdHocSMSRequest request = null;

                        if (bulkRequest == null)
                        {
                            request = (AdHocSMSRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocSMSRequest)bulkRequest.GetEntitlementDetail();
                        }
                        if (request != null)
                        {
                            List<string> bussinessRoles = UtilityMethods.GetListFromString(request.Entitlement);

                            foreach (string bussinessRole in bussinessRoles)
                            {
                                matrix.Add(SMSXLRoleCatalogue.GetBuildingandSectionByTechRoles(bussinessRole));
                            }
                        }
                    }
                    break;
                case ApplicationType.EAM:
                    {
                        AdHocNovaRequest request = null;
                        if (bulkRequest == null)
                        {
                            request = (AdHocNovaRequest)adhocRequest.GetEntitlementDetail();
                        }
                        else
                        {
                            request = (AdHocNovaRequest)bulkRequest.GetEntitlementDetail();
                        }
                        if (request != null && request.RequestDetails != null)
                        {
                            foreach (var detail in request.RequestDetails)
                            {
                                if (detail.NovaMatrixType == NovaMatrixType.User)
                                {
                                    matrix.Add(NOVAUserRoleMatrix.GetNovaMatrix(detail));
                                }
                                else if (detail.NovaMatrixType == NovaMatrixType.Management)
                                {
                                    matrix.Add(NOVAManagementRoleMatrix.GetNovaMatrix(detail));
                                }
                                else if (detail.NovaMatrixType == NovaMatrixType.Exceptional)
                                {
                                    matrix.Add(NOVAExceptionalRoleMatrix.GetNovaMatrix(detail));
                                }
                            }

                        }
                    }
                    break;
                case ApplicationType.EBS:
                case ApplicationType.MSSL:
                    {
                        long epId = 0;
                        if (adhocRequest != null)
                            epId = adhocRequest.EndpointSystemId.Value;
                        else
                            epId = bulkRequest.EndpointSystemId;
                        string position = string.Empty;
                        string roles = string.Empty;
                        string applicationName = string.Empty;

                        if (bulkRequest == null)
                        {
                            var sapDetail = (AdHocSAPHANARequest)adhocRequest.GetEntitlementDetail();
                            if (sapDetail != null)
                            {
                                roles = sapDetail.Roles;
                                position = sapDetail.Position;
                                applicationName = sapDetail.ApplicationName;
                            }
                        }
                        else
                        {
                            var sapDetail = (AdHocSAPHANARequest)bulkRequest.GetEntitlementDetail();
                            if (sapDetail != null)
                            {
                                roles = sapDetail.Roles;
                                position = sapDetail.Position;
                                applicationName = sapDetail.ApplicationName;
                            }
                        }

                        if (!string.IsNullOrEmpty(position))
                        {
                            matrix.Add(SAPHANAXLPositionsMatrix.GetSAPHANAPositionMatrixByPosition(epId, position));
                        }

                        if (!string.IsNullOrEmpty(roles))
                        {
                            List<string> bussinessRoles = UtilityMethods.GetListFromString(roles);

                            foreach (string bussinessRole in bussinessRoles)
                            {
                                matrix.Add(SAPHANAXLRolesMatrix.GetSAPHANARoleMatrix(bussinessRole, epId));
                            }
                        }

                        break;
                    }
            }

            matrix.RemoveAll(p => p == null);
            return matrix;
        }

        List<AccessRequestReviewInfo> GetRoleReviewerBySystemConfiguration(AccessRequest accessRequest, AdHocAccessRequest adhocRequest, BulkRequest bulkRequest, WorkflowConfigurationDetail currentApprover)
        {
            List<AccessRequestReviewInfo> reviewInfos = new List<AccessRequestReviewInfo>();

            //else if (appType == ApplicationType.OneCard)
            //{
            //    AdHocSMSRequest request = null;

            //    if (bulkRequest == null)
            //    {
            //        request = (AdHocSMSRequest)adhocRequest.GetEntitlementDetail();
            //    }
            //    else
            //    {
            //        request = (AdHocSMSRequest)bulkRequest.GetEntitlementDetail();
            //    }
            //    if (request != null)
            //    {
            //        List<string> bussinessRoles = UtilityMethods.GetListFromString(request.Entitlement);

            //        foreach (string bussinessRole in bussinessRoles)
            //        {
            //            matrix.Add(provConfigFacade.GetSMSMatrixByRequest(bussinessRole));
            //        }
            //    }
            //}
            var matrix = GetRoleReviewersIApprovers(adhocRequest, bulkRequest);
            if (matrix != null)
            {
                reviewInfos = this.GetRoleReviewersByConfig(currentApprover, matrix);
            }

            return reviewInfos;
        }

        List<AccessRequestReviewInfo> GetRoleReviewersByConfig(WorkflowConfigurationDetail currentApprover, List<IApprover> approvers)
        {
            List<AccessRequestReviewInfo> reviewInfos = new List<AccessRequestReviewInfo>();
            List<ApproverInfo> distinctApprovers = new List<ApproverInfo>();

            try
            {
                foreach (IApprover approver in approvers)
                {
                    if (approver == null)
                    {
                        LogDebugStep($"approver is null");
                        continue;
                    }
                    string approverName = string.Empty;
                    ApprovalUserType approverType = ApprovalUserType.None;

                    if (currentApprover.Approver == (byte)RoleType.Level1Approver)
                    {
                        approverName = approver.Level1Approver;
                        approverType = approver.Level1ApproverType;
                    }
                    else if (currentApprover.Approver == (byte)RoleType.Level2Approver)
                    {
                        approverName = approver.Level2Approver;
                        approverType = approver.Level2ApproverType;
                    }
                    else if (currentApprover.Approver == (byte)RoleType.Level3Approver)
                    {
                        approverName = approver.Level3Approver;
                        approverType = approver.Level3ApproverType;
                    }

                    var distinctApprover = distinctApprovers.FirstOrDefault(item => item.Owner == approverName);
                    if (distinctApprover != null)
                    {
                        distinctApprover.BusinessRoles.Add(approver.BusinessRole);
                    }
                    else
                    {
                        List<string> roles = new List<string>();
                        roles.Add(approver.BusinessRole);

                        ApproverInfo newApprover = new ApproverInfo()
                        {
                            BusinessRoles = roles,
                            Owner = approverName,
                            ApproverType = approverType,
                            SystemName = approver.SystemName
                        };

                        distinctApprovers.Add(newApprover);
                    }
                }

                foreach (ApproverInfo approver in distinctApprovers)
                {
                    if (currentApprover.Approver == (byte)RoleType.Level1Approver)
                    {
                        if (approver.ApproverType == ApprovalUserType.User && !string.IsNullOrEmpty(approver.Owner))
                        {
                            long reviewerId = utility.GetUserIdByEmployeeId(approver.Owner);

                            if (reviewerId != 0)
                            {
                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.ReviewerId = reviewerId;
                                reviewInfo.IsGroup = false;

                                reviewInfos.Add(reviewInfo);
                            }
                            else
                            {
                                var reviewerIds = utility.GetUsersByGroupId(MasterConfigHelper.ITSecurityGroupId);
                                foreach (int reviewer in reviewerIds)
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewer;
                                    reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                    reviewInfos.Add(reviewInfo);
                                }
                            }
                        }
                        else if (approver.ApproverType == ApprovalUserType.Group && !string.IsNullOrEmpty(approver.Owner))
                        {
                            var reviewerIds = utility.GetUserIdByGroupName(approver.Owner);
                            if (reviewerIds.Count != 0)
                            {
                                foreach (int reviewerId in reviewerIds)
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewerId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.GroupName = approver.Owner;
                                    reviewInfo.GroupId = utility.GetGroupIdByName(reviewInfo.GroupName);

                                    reviewInfos.Add(reviewInfo);
                                }
                            }
                            else
                            {
                                reviewerIds = utility.GetUsersByGroupId(MasterConfigHelper.ITSecurityGroupId);
                                foreach (int reviewerId in reviewerIds)
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewerId;
                                    reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                    reviewInfos.Add(reviewInfo);
                                }
                            }
                        }
                    }
                    else if (currentApprover.Approver == (byte)RoleType.Level2Approver)
                    {
                        if (approver.ApproverType == ApprovalUserType.User && !string.IsNullOrEmpty(approver.Owner))
                        {
                            long reviewerId = utility.GetUserIdByEmployeeId(approver.Owner);

                            if (reviewerId != 0)
                            {
                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.ReviewerId = reviewerId;
                                reviewInfo.IsGroup = false;

                                reviewInfos.Add(reviewInfo);
                            }
                            else
                            {
                                var reviewerIds = utility.GetUsersByGroupId(MasterConfigHelper.ITSecurityGroupId);
                                foreach (int reviewer in reviewerIds)
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewer;
                                    reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                    reviewInfos.Add(reviewInfo);
                                }
                            }
                        }
                        else if (approver.ApproverType == ApprovalUserType.Group && !string.IsNullOrEmpty(approver.Owner))
                        {
                            var reviewerIds = utility.GetUserIdByGroupName(approver.Owner);
                            if (reviewerIds.Count != 0)
                            {
                                foreach (int reviewerId in reviewerIds)
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewerId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.GroupName = approver.Owner;
                                    reviewInfo.GroupId = utility.GetGroupIdByName(reviewInfo.GroupName);

                                    reviewInfos.Add(reviewInfo);
                                }
                            }
                            else
                            {
                                reviewerIds = utility.GetUsersByGroupId(MasterConfigHelper.ITSecurityGroupId);
                                foreach (int reviewerId in reviewerIds)
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewerId;
                                    reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                    reviewInfos.Add(reviewInfo);
                                }
                            }
                        }
                    }
                    else if (currentApprover.Approver == (byte)RoleType.Level3Approver)
                    {
                        if (approver.ApproverType == ApprovalUserType.User && !string.IsNullOrEmpty(approver.Owner))
                        {
                            long reviewerId = utility.GetUserIdByEmployeeId(approver.Owner);

                            if (reviewerId != 0)
                            {
                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.ReviewerId = reviewerId;
                                reviewInfo.IsGroup = false;

                                reviewInfos.Add(reviewInfo);
                            }
                            else
                            {
                                var reviewerIds = utility.GetUsersByGroupId(MasterConfigHelper.ITSecurityGroupId);
                                foreach (int reviewer in reviewerIds)
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewer;
                                    reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                    reviewInfos.Add(reviewInfo);
                                }
                            }
                        }
                        else if (approver.ApproverType == ApprovalUserType.Group && !string.IsNullOrEmpty(approver.Owner))
                        {
                            var reviewerIds = utility.GetUserIdByGroupName(approver.Owner);
                            if (reviewerIds.Count != 0)
                            {
                                foreach (int reviewerId in reviewerIds)
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewerId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.GroupName = approver.Owner;
                                    reviewInfo.GroupId = utility.GetGroupIdByName(reviewInfo.GroupName);

                                    reviewInfos.Add(reviewInfo);
                                }
                            }
                            else
                            {
                                reviewerIds = utility.GetUsersByGroupId(MasterConfigHelper.ITSecurityGroupId);
                                foreach (int reviewerId in reviewerIds)
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewerId;
                                    reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                    reviewInfos.Add(reviewInfo);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogDebugStep($"GetRoleReviewersByConfig: {ex.StackTrace}");
            }

            return reviewInfos;
        }
        private string GetTaskSubTaskRemarks(long taskId)
        {
            System.Text.StringBuilder subTaskRemarks = new System.Text.StringBuilder();
            var tsk = Task.GetSubTask(taskId);
            if (tsk != null)
            {
                if (!string.IsNullOrWhiteSpace(tsk.Remarks))
                {
                    subTaskRemarks.Append(tsk.Remarks);
                    subTaskRemarks.Append("<br/><br/>");
                }
                if (tsk.SubTask != null)
                {
                    if (!string.IsNullOrWhiteSpace(tsk.SubTask.Remarks))
                    {
                        subTaskRemarks.Append($"<b>Remarks:</b> {tsk.SubTask.Remarks}");
                        subTaskRemarks.Append("<br/>");
                    }
                    if (!string.IsNullOrWhiteSpace(tsk.SubTask.RoleName))
                    {
                        subTaskRemarks.Append($"<b>RoleName:</b> {tsk.SubTask.RoleName}");
                    }
                    else if (!string.IsNullOrWhiteSpace(tsk.SubTask.TCode))
                    {
                        subTaskRemarks.Append($"<b>TCode:</b> {tsk.SubTask.TCode}");
                    }
                }

            }
            if (subTaskRemarks != null && subTaskRemarks.Length > 0 && !string.IsNullOrWhiteSpace(subTaskRemarks.ToString()))
                return subTaskRemarks.ToString();

            return null;
        }
        //List<AccessRequestReviewInfo> GetReviewersForHeads(long userId, RoleType roleType)
        //{
        //    var reviewers = new List<AccessRequestReviewInfo>();
        //    long reviewer = 0;
        //    var user = User.GetById(userId);
        //    NOrganisation nOrg = null;
        //    switch (roleType)
        //    {
        //        case RoleType.HeadOfSection:
        //            nOrg = NOrganisation.GetNOrganisationByName(user.OrgSection);
        //            break;
        //        case RoleType.HeadOfBranch:
        //            nOrg = NOrganisation.GetNOrganisationByName(user.OrgBranch);
        //            break;
        //        case RoleType.HeadOfDepartment:
        //            nOrg = NOrganisation.GetNOrganisationByName(user.OrgDepartment);
        //            break;
        //        default:
        //            nOrg = null;
        //            break;
        //    }
        //    var nOrgHead = NOrganisationHead.GetByOrganisationId(nOrg.Id);
        //    reviewer = nOrgHead.Id;

        //    if (reviewer != 0)
        //    {
        //        AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
        //        reviewInfo.ReviewerId = reviewer;
        //        reviewInfo.IsGroup = false;
        //        reviewers.Add(reviewInfo);
        //    }
        //    return reviewers;
        //}

    }
}
