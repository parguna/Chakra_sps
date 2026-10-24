using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Workflow.Processor
{
    public class GetNextLevelApprovalTasks
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();

        public GetNextLevelApprovalTasksResponse Execute(Task task, WorkflowConfiguration workflowConfiguration, byte currentState)
        {
            utility.LogDebugStep("GetNextLevelApprovalTasks");
            RequestType requestType = RequestType.None;

            Task nextLevelApprovalTask = new Task();
            bool isPreviousLevelApproved = true;
            bool isSkipNextLevel = false;

            try
            {
                utility.LogDebugStep("GetNextLevelApprovalTasks");
                WorkflowConfigurationDetail currentApprover = new WorkflowConfigurationDetail();

                requestType = (RequestType)task.RequestTypeId;
                utility.LogDebugStep($"GetNextLevelApprovalTasks: requestType:{requestType}, currentState: {currentState}");

                currentApprover = workflowConfiguration.WorkflowConfigurationDetails.Find(item => item.SequenceNumber == currentState);
                utility.LogDebugStep("currentApprover");
                if (requestType == RequestType.Attestation)
                {
                    nextLevelApprovalTask = new GetNextLevelAttestationTasks().Execute(task, currentApprover).FirstOrDefault();

                    if (!isSkipNextLevel && isPreviousLevelApproved)
                    {
                        Task existingTask = Task.GetAttestationTask(task.Id, false);
                        //var previousApproverType = workflowConfiguration.WorkflowConfigurationDetails.Find(item => item.SequenceNumber == existingTask.WorkflowStateId);                        
                        if (nextLevelApprovalTask != null && !nextLevelApprovalTask.IsGroupTask) //&& !(previousApproverType.ApprovalUserType == ApprovalUserType.Role && previousApproverType.Approver == (byte)RoleType.Manager))
                        {
                            var currentApprovers = Task.GetCurrentTaskByCampaignIdAndStateId(existingTask.AttestationCampaignId.Value, currentState);
                            var previousApprover = Task.GetCompletedTaskByCampaignIdAndStateId(existingTask.AttestationCampaignId.Value, existingTask.WorkflowStateId);

                            var isConsecutiveTaskForSameUser = currentApprovers.Intersect(previousApprover).ToList();
                            var anyNewApprover = currentApprovers.Except(previousApprover).ToList();
                            var anyPreApproverNotInCurrApprover = previousApprover.Except(currentApprovers).ToList();

                            if (isConsecutiveTaskForSameUser.Any() && !anyNewApprover.Any() && !anyPreApproverNotInCurrApprover.Any())
                            {
                                var pendingTasks = Task.GetPendingTaskByworkflowInstanceId(nextLevelApprovalTask.WorkflowInstanceId);
                                isSkipNextLevel = true;
                                foreach (Task pendingTask in pendingTasks)
                                {
                                    if (isConsecutiveTaskForSameUser.Contains(pendingTask.ActionUserId.Value))
                                    {
                                        pendingTask.Action = StatusType.Approved;
                                        pendingTask.IsCompleted = true;
                                        pendingTask.Remarks = "Auto Approved by system, as the same user approved in previous level.";
                                        pendingTask.Save(MasterConfigHelper.ServiceUser);
                                    }
                                    else
                                    {
                                        isSkipNextLevel = false;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (requestType == RequestType.Provisioning)
                {
                    utility.LogDebugStep("RequestType.Provisioning");
                    Task existingTask = Task.GetProvisioningTask(task.Id);
                    utility.LogDebugStep($"existingTask: {existingTask == null}");
                    if (existingTask != null)
                    {
                        var accessRequest = existingTask.AccessRequest;
                        utility.LogDebugStep($"existingTask.AccessRequest.RequestStatus: {existingTask.AccessRequest?.RequestStatus}");
                        if (accessRequest != null && accessRequest.RequestStatus == AccessRequestStatus.Approved)
                        {
                            if (existingTask.AccessRequest.ApplicationType == ApplicationType.VPN && currentState == MasterConfigHelper.VPNADAccountCreationState)
                            {
                                utility.LogDebugStep("VPNADAccountCreation");
                                var createAdrequest = new CreateADRequestForVPN();
                                createAdrequest.Execute(existingTask);
                            }
                            //Discussed with Kanchana and AD Account creation can be done manually
                            //if (existingTask.AccessRequest.ApplicationType == ApplicationType.GEMS && currentState == MasterConfigHelper.GEMSADAccountCreationState)
                            //{
                            //    utility.LogDebugStep("GEMSADAccountCreation");
                            //    var createAdrequest = new CreateADRequestForGEMS();
                            //    createAdrequest.Execute(existingTask);
                            //}
                            
                            
                            if (existingTask.AccessRequest.ApplicationType == ApplicationType.Office365 && currentState == MasterConfigHelper.O365ADAccountCreationState)
                            {
                                if (existingTask.AccessRequest.AccountType == AccountType.User)
                                {
                                    utility.LogDebugStep("O365ADAccountCreation");
                                    var createAdrequest = new CreateADRequestForVPN();
                                    createAdrequest.ExecuteForO365(existingTask);
                                }
                                bool reviewerRequired = true;
                                var user = User.GetById(accessRequest.UserId);
                                        
                                {
                                    utility.LogDebugStep($"Office365 check for next level - 1");
                                    if (accessRequest.AdHocAccessRequestId.HasValue)
                                    {
                                        accessRequest.AdHocAccessRequest = AdHocAccessRequest.GetById(accessRequest.AdHocAccessRequestId.Value);
                                        if (accessRequest.AdHocAccessRequest != null && accessRequest.ApplicationType == ApplicationType.Office365 && accessRequest.AccountType == AccountType.User
                                            && (accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount || accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
                                        {
                                            utility.LogDebugStep($"Office365 check for next level - 2");
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
                                    else if (accessRequest.BulkRequestId.HasValue)
                                    {
                                        utility.LogDebugStep($"Office365 check for next level - 3");
                                        var o365RequestDetails = BulkRequest.GetById(accessRequest.BulkRequestId.Value, true);
                                        if (o365RequestDetails != null && o365RequestDetails.ApplicationType == ApplicationType.Office365
                                            && (o365RequestDetails.AdhocActionType == AdHocActionType.NewAccount || o365RequestDetails.AdhocActionType == AdHocActionType.AmendmentAdd))
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
                                utility.LogDebugStep($"Office365 check for next level required:{reviewerRequired}");
                                if (!reviewerRequired)
                                {
                                    return new GetNextLevelApprovalTasksResponse()
                                    {
                                        SkipNextLevel = true,
                                        IsApproved = isPreviousLevelApproved,
                                        NextLevelTask = task
                                    };
                                }
                            }

                            utility.LogDebugStep("AccessRequestStatus.Approved");

                            var nextLevelTasks = new GetNextLevelProvisioningTasks().ExecuteForAdhocRequest(existingTask, currentApprover, ref isSkipNextLevel);
                            if (nextLevelTasks == null || nextLevelTasks.Count() == 0)
                            {
                                return new GetNextLevelApprovalTasksResponse()
                                {
                                    SkipNextLevel = true,
                                    IsApproved = isPreviousLevelApproved,
                                    NextLevelTask = task
                                };
                            }
                            nextLevelApprovalTask = nextLevelTasks.FirstOrDefault();
                            utility.LogDebugStep($"nextLevelApprovalTask: {nextLevelApprovalTask == null}");



                            isSkipNextLevel = canSkipNextLevel(existingTask, nextLevelApprovalTask, requestType, currentState, nextLevelTasks);



                        }
                        else
                        {
                            existingTask.WorkflowStateId = currentState;
                            nextLevelApprovalTask = existingTask;
                            isPreviousLevelApproved = false;
                        }

                        if (!isSkipNextLevel && isPreviousLevelApproved)
                        {
                            
                            if ((currentApprover.Approver != (byte)RoleType.TCodeOwner) && !nextLevelApprovalTask.IsGroupTask)
                            {
                                var currentApprovers = Task.GetActionUsersByRequestState(existingTask.AccessRequestId.Value, currentState);

                                var previousObj = Task.GetApproverByAccessRequestId(existingTask.AccessRequestId.Value, existingTask.WorkflowStateId);
                                if (previousObj != null)
                                {
                                    long previousApprover = previousObj != null ? previousObj.ActionUserId.Value : 0;
                                    bool isLevelApproversAreSame = true;
                                    bool isConsecutiveTaskForSameUser = currentApprovers.Contains(previousApprover);
                                    if (isConsecutiveTaskForSameUser)
                                    {
                                        var pendingTasks = Task.GetPendingTaskByworkflowInstanceId(nextLevelApprovalTask.WorkflowInstanceId);
                                        foreach (Task pendingTask in pendingTasks)
                                        {
                                            if (pendingTask.IsGroupTask)
                                            {
                                                isLevelApproversAreSame = false;
                                            }
                                            else
                                            {
                                                if (pendingTask.ActionUserId == previousApprover)
                                                {
                                                    pendingTask.Action = StatusType.Approved;
                                                    pendingTask.IsCompleted = true;
                                                }
                                                else
                                                {

                                                    if (currentApprover.RequiresApprovalFromAll == false)
                                                        pendingTask.IsActive = false;
                                                    else
                                                    {
                                                        isLevelApproversAreSame = false;
                                                    }
                                                }
                                                pendingTask.Save(MasterConfigHelper.ServiceUser);
                                            }
                                        }
                                        if (isLevelApproversAreSame)
                                        {
                                            isSkipNextLevel = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (requestType == RequestType.BulkRequest)
                {
                    Task existingTask = Task.GetBulkRequestTask(task.Id);
                    if (existingTask != null)
                    {
                        if (existingTask.BulkRequest.RequestStatus == AccessRequestStatus.Approved)
                        {
                            var nextLevelTasks = new GetNextLevelProvisioningTasks().ExecuteForBulkRequest(existingTask, currentApprover, ref isSkipNextLevel);
                            if (nextLevelTasks == null || nextLevelTasks.Count() == 0)
                            {
                                return new GetNextLevelApprovalTasksResponse()
                                {
                                    SkipNextLevel = true,
                                    IsApproved = isPreviousLevelApproved,
                                    NextLevelTask = task
                                };
                            }
                            nextLevelApprovalTask = nextLevelTasks.FirstOrDefault();

                            isSkipNextLevel = canSkipNextLevel(existingTask, nextLevelApprovalTask, requestType, currentState, nextLevelTasks);
                        }
                        else
                        {
                            existingTask.WorkflowStateId = currentState;
                            nextLevelApprovalTask = existingTask;
                            isPreviousLevelApproved = false;
                        }


                        if (!isSkipNextLevel && isPreviousLevelApproved)
                        {
                            if (currentApprover.Approver != (byte)RoleType.TCodeOwner && !nextLevelApprovalTask.IsGroupTask)
                            {
                                bool isLevelApproversAreSame = true;
                                var currentApprovers = Task.GetActionUsersByBulkRequestState(existingTask.BulkRequestId.Value, currentState);
                                var previousObj = Task.GetApproverByAccessRequestId(existingTask.BulkRequestId.Value, existingTask.WorkflowStateId);
                                if (previousObj != null)
                                {
                                    long previousApprover = previousObj != null ? previousObj.ActionUserId.Value : 0;

                                    bool isConsecutiveTaskForSameUser = currentApprovers.Contains(previousApprover);

                                    if (isConsecutiveTaskForSameUser)
                                    {
                                        var pendingTasks = Task.GetPendingTaskByworkflowInstanceId(nextLevelApprovalTask.WorkflowInstanceId);
                                        foreach (Task pendingTask in pendingTasks)
                                        {
                                            if (pendingTask.IsGroupTask)
                                            {
                                                isLevelApproversAreSame = false;
                                            }
                                            else
                                            {
                                                if (pendingTask.ActionUserId == previousApprover)
                                                {
                                                    pendingTask.Action = StatusType.Approved;
                                                    pendingTask.IsCompleted = true;
                                                }
                                                else
                                                {
                                                    if (currentApprover.RequiresApprovalFromAll == false)
                                                        pendingTask.IsActive = false;
                                                    else
                                                    {
                                                        isLevelApproversAreSame = false;
                                                    }
                                                }
                                                pendingTask.Save(MasterConfigHelper.ServiceUser);
                                            }
                                        }
                                        if (isLevelApproversAreSame)
                                        {
                                            isSkipNextLevel = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the GetNextLevelApprovalAttestationTasks activity.";
                if (requestType == RequestType.Attestation)
                {
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, message, ex);
                }
                else
                {
                    DILog.WorkflowLogger.WriteLog(LogLevelType.Exception, message, ex);
                }
            }

            return new GetNextLevelApprovalTasksResponse()
            {
                SkipNextLevel = isSkipNextLevel,
                IsApproved = isPreviousLevelApproved,
                NextLevelTask = nextLevelApprovalTask
            };
        }
        private List<int> GetNPACAdhocActionTypeForApprovalCheck()
        {
            List<int> adhocActionType = new List<int>();
            try
            {
                char[] delimiter = { ',' };
                var adhocActionTypesConfig = MasterConfigHelper.NPACAdhocActionTypeApprovals;

                var actionTypes = adhocActionTypesConfig?.Split(delimiter);
                if(!string.IsNullOrWhiteSpace(adhocActionTypesConfig) && actionTypes != null && actionTypes.Length > 0)
                {
                    foreach (var item in actionTypes)
                    {
                        int temp;
                        if (int.TryParse(item, out temp))
                        {
                            if (temp == (int)AdHocActionType.NewAccount || temp == (int)AdHocActionType.AmendmentAdd || temp == (int)AdHocActionType.AmendmentRemove || temp == (int)AdHocActionType.DeleteAccount)
                            {
                                adhocActionType.Add(temp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                utility.LogDebugStep($"GetNPACAdhocActionTypeForApprovalCheck failed {ex.Message} {ex.StackTrace}");
            }
            return adhocActionType;
        }
        bool canSkipNextLevel(Task existingTask, Task nextLevelApprovalTask, RequestType requestType, byte currentState, List<Task> nextLevelTasks)
        {
            bool returnFlag = false;
            string comments = "Auto Approved.";
            if (existingTask == null || nextLevelApprovalTask == null) return false;
            if (requestType == RequestType.Provisioning)
            {
                var accessRequest = existingTask.AccessRequest;
                if (accessRequest == null) return true;
                var adhocRequest = accessRequest.AdHocAccessRequest;
                if (accessRequest.ApplicationType == ApplicationType.InhouseSystems
                     && adhocRequest != null && nextLevelApprovalTask != null && nextLevelApprovalTask.IsActionOnlyRequiredTask)
                {
                    var endpoint = EndPoint.GetById(accessRequest.EndpointSystemId.Value);
                    if (endpoint.CanAutomate)
                    {
                        comments = $"Inhouse Automated system does not require Processing Team Approval. Auto-created by System.";
                        returnFlag = true;
                    }
                }
                    if (accessRequest.ApplicationType == ApplicationType.Office365
                     && accessRequest.AccountType == AccountType.Shared && adhocRequest != null
                     && nextLevelApprovalTask != null && nextLevelApprovalTask.IsGroupTask)
                {
                    int groupId = MasterConfigHelper.O365Admin;
                    AdHocO365Request request = (AdHocO365Request)adhocRequest.GetEntitlementDetail();
                    if (adhocRequest.AdhocActionType != AdHocActionType.AmendmentRemove && nextLevelApprovalTask.ActionGroupId == groupId && request.MailBoxConversionType == MailBoxConversionType.SharedToUser)
                    {
                        comments = $"Auto Approval for Internal Shared Mail Box(No License)";
                        returnFlag = true;
                    }
                }
                if (accessRequest.AdHocAccessRequestId.HasValue && accessRequest.ApplicationType == ApplicationType.NPAC)
                {
                    if (adhocRequest != null && nextLevelApprovalTask != null)
                    {
                        var grpName = MasterConfigHelper.NPACApprovalGroup;
                        var npacDetail = (AdHocNPACRequest)adhocRequest.GetEntitlementDetail();
                        var validAdhocActions = GetNPACAdhocActionTypeForApprovalCheck();
                        if (npacDetail != null && npacDetail.Entitlement != "1" && npacDetail.Entitlement != MasterConfigHelper.NPACLevel99 && !string.IsNullOrWhiteSpace(grpName)
                            && validAdhocActions.Contains((int)adhocRequest.AdhocActionType)
                            )
                        {
                            var wfConf = WorkflowConfiguration.GetById(nextLevelApprovalTask.WorkflowConfigurationId, true);
                            if (wfConf != null && wfConf.WorkflowConfigurationDetails.Exists(i => i.SequenceNumber == MasterConfigHelper.NPACApprovalSkipLevel) && currentState == MasterConfigHelper.NPACApprovalSkipLevel)
                            {
                                var wfConfig = wfConf.WorkflowConfigurationDetails.FirstOrDefault(i => i.SequenceNumber == MasterConfigHelper.NPACApprovalSkipLevel);
                                if (adhocRequest != null && adhocRequest.ApplicationType == ApplicationType.NPAC && wfConfig.ApprovalUserType == ApprovalUserType.Group
                               && currentState == MasterConfigHelper.NPACApprovalSkipLevel)
                                {
                                    var aGroup = Group.GetByGroupName(grpName);
                                    if (aGroup != null && wfConfig.Approver == aGroup.Id)
                                    {
                                        comments = $"No Approval required for Level:{npacDetail.Entitlement}.";
                                        returnFlag = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (adhocRequest != null && adhocRequest.AccountType == AccountType.User
                         && adhocRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal
                         && nextLevelApprovalTask != null && nextLevelApprovalTask.IsGroupTask)
                {
                    int groupId = MasterConfigHelper.WintelGroupId;
                    var requesterComment = getRequesterCommentForAD(accessRequest.AdHocAccessRequest.GetEntitlementDetail(), accessRequest.AdHocAccessRequest.ApplicationType);
                    if (nextLevelApprovalTask.ActionGroupId == groupId && string.IsNullOrWhiteSpace(requesterComment))
                    {
                        comments = $"No comments from the requester. Thus auto approved.";
                        returnFlag = true;
                    }
                }
                else if (adhocRequest.ApplicationType == ApplicationType.VPN)
                {
                    if (accessRequest.RequestSource == AccessRequestSource.OffBoarding || accessRequest.RequestSource == AccessRequestSource.OUChange)
                    {
                        var wfConfig = WorkflowConfiguration.GetWorkflowConfigurationDetailById(existingTask.WorkflowConfigurationId);
                        if (wfConfig != null && wfConfig.WorkflowConfigurationDetails != null && wfConfig.WorkflowConfigurationDetails.Any())
                        {
                            var action = wfConfig.WorkflowConfigurationDetails.FirstOrDefault(i => i.IsActionRequired);
                            if (action != null && nextLevelApprovalTask.WorkflowStateId != action.SequenceNumber)
                            {
                                comments = $"Auto Approval for RequestSource:{accessRequest.RequestSource}";
                                returnFlag = true;
                            }
                        }
                    }
                    else if (accessRequest.RequestSource == AccessRequestSource.AdhocRequest)
                    {
                        if (accessRequest.EventUndoRequestId.HasValue && accessRequest.EventUndoRequestId > 0)
                        {
                            var wfConfig = WorkflowConfiguration.GetWorkflowConfigurationDetailById(existingTask.WorkflowConfigurationId);
                            if (wfConfig != null && wfConfig.WorkflowConfigurationDetails != null && wfConfig.WorkflowConfigurationDetails.Any())
                            {
                                var action = wfConfig.WorkflowConfigurationDetails.FirstOrDefault(i => i.IsActionRequired);
                                if (action != null && nextLevelApprovalTask.WorkflowStateId != action.SequenceNumber)
                                {
                                    comments = $"Auto Approval for UnDoEvent:EventUndoRequestId:{accessRequest.EventUndoRequestId}";
                                    returnFlag = true;
                                }
                            }
                        }
                        if (accessRequest.ApplicationType == ApplicationType.VPN && accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                        {
                            var request = (AdHocVPNRequest)accessRequest.AdHocAccessRequest.GetEntitlementDetail();
                            if (request != null && request.UpdateOneKeyToken == false)
                            {
                                var wfConfig = WorkflowConfiguration.GetWorkflowConfigurationDetailById(existingTask.WorkflowConfigurationId);
                                if (wfConfig != null && wfConfig.WorkflowConfigurationDetails != null && wfConfig.WorkflowConfigurationDetails.Any())
                                {
                                    var action = wfConfig.WorkflowConfigurationDetails.FirstOrDefault(i => i.IsActionRequired);
                                    if (action != null && nextLevelApprovalTask.WorkflowStateId == action.SequenceNumber)
                                    {
                                        comments = $"User did not requst for Update Token Number(No action requied). Hence auto approved";
                                        returnFlag = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (adhocRequest.ApplicationType == ApplicationType.InhouseSystems && (accessRequest.RequestSource == AccessRequestSource.OffBoarding || accessRequest.RequestSource == AccessRequestSource.OUChange || accessRequest.RequestSource == AccessRequestSource.UndoEvent || accessRequest.RequestSource == AccessRequestSource.NonEmployeeExtension || accessRequest.AttestationCampaignId.HasValue))
                {
                    var wfConfig = WorkflowConfiguration.GetWorkflowConfigurationDetailById(existingTask.WorkflowConfigurationId);
                    if (wfConfig != null && wfConfig.WorkflowConfigurationDetails != null && wfConfig.WorkflowConfigurationDetails.Any())
                    {
                        var action = wfConfig.WorkflowConfigurationDetails.FirstOrDefault(i => i.IsActionRequired);
                        if (action != null && nextLevelApprovalTask.IsActionOnlyRequiredTask == false && nextLevelApprovalTask.WorkflowStateId != action.SequenceNumber)
                        {
                            comments = $"Auto Approval for RequestSource:{accessRequest.RequestSource}";
                            returnFlag = true;
                        }
                    }
                }
            }
            else if (requestType == RequestType.BulkRequest)
            {
                var bulkRequest = existingTask.BulkRequest;
                if (bulkRequest != null
                    && bulkRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal
                    && nextLevelApprovalTask != null && nextLevelApprovalTask.IsGroupTask)
                {
                    int groupId = MasterConfigHelper.WintelGroupId;
                    var requesterComment = getRequesterCommentForAD(bulkRequest.GetEntitlementDetail(), bulkRequest.ApplicationType);
                    if (nextLevelApprovalTask.ActionGroupId == groupId && string.IsNullOrWhiteSpace(requesterComment))
                    {
                        comments = $"No comments from the requester. Thus auto approved.";
                        returnFlag = true;
                    }
                }
            }

            if (MasterConfigHelper.MockSkipApproval && nextLevelApprovalTask.IsActionOnlyRequiredTask == false)
            {
                utility.LogDebugStep("GetNextLevelApprovalTasksResponse MockSkipApproval:");
                comments = $"Auto approved because of MockSkipApproval.";
                returnFlag = true;
            }

            if (returnFlag)
            {
                foreach (var nextLevelTask in nextLevelTasks)
                {
                    //if (nextLevelTask.IsGroupTask)
                    //{
                    //    var grp = Group.GetByIdIncludingUser(nextLevelTask.ActionGroupId.Value);
                    //    if (grp != null && grp.Users.Count > 0)
                    //    {
                    //        nextLevelTask.ActionGroupId = grp.Users.FirstOrDefault().Id;
                    //    }
                    //    else
                    //    {
                    //        nextLevelTask.ActionUserId = 1;
                    //    }
                    //}

                    nextLevelTask.Comments = comments;
                    nextLevelTask.Action = StatusType.Approved;
                    nextLevelTask.IsCompleted = true;
                    nextLevelTask.Save(MasterConfigHelper.ServiceUser);
                }
            }

            return returnFlag;
        }

        string getRequesterCommentForAD(IEntitlementDetail entitlementDetail, ApplicationType applicationType)
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
    }
}
