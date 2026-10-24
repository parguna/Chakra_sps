 
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Workflow.Processor
{
    public class CreateTask
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();

        public void Execute(List<Task> taskList, WorkflowConfiguration currentWorkflowConfiguration, Guid workflowInstanceId)
        {
            RequestType requestType = RequestType.None;
            try
            {
                var currentTask = taskList.FirstOrDefault();
                requestType = (RequestType)currentTask.RequestTypeId;

                WorkflowConfigurationDetail currentApprover = currentWorkflowConfiguration.WorkflowConfigurationDetails.Find(item => item.SequenceNumber == currentTask.WorkflowStateId);
                var currentUser = MasterConfigHelper.ServiceUser;
                foreach (Task task in taskList)
                {
                    task.Approver = utility.GetApprover(currentApprover);
                    if (task != null && currentApprover != null && task.IsGroupTask && task.ActionGroupId.HasValue && currentApprover.ApprovalUserType != ApprovalUserType.Group)
                    {
                        task.Approver = utility.GetApprover(task.GroupName, false);
                    }
                    //task.WorkflowConfigurationId = currentApprover.Id;
                    task.WorkflowInstanceId = workflowInstanceId;

                    if (task.WorkflowStateId == (byte)WorkflowState.None)
                    {
                        task.WorkflowStateId = (byte)WorkflowState.Level1;
                    }

                    task.Save(currentUser);

                    if (requestType == RequestType.Attestation)
                    {
                        long campaignId = 0;

                        if (task.AttestationCampaignId.HasValue)
                            campaignId = task.AttestationCampaignId.Value;
                        // Update Snapshot actions with Task ID
                        var actionItem = task.ActionUserId.HasValue ? task.ActionUserId.Value : 0;
                        if(task.IsGroupTask)
                            actionItem = task.ActionGroupId.HasValue ? task.ActionGroupId.Value : 0;
                        var actions = AttestationSnapshotAction.GetByReviewerId(actionItem, task.IsGroupTask, campaignId);

                        //update the actions with the task details
                        if (actions != null && actions.Count > 0)
                        {
                            //string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET TaskId = {0}, ReviewerId = {1} WHERE Id in ({2}) ",
                            //task.Id, actionItem, string.Join(",", actions.Select(a => a.Id).ToArray()));
                            //AttestationSnapshotAction.ExecuteQuery(updateInterim);
                            string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET TaskId = {0}, ReviewerId = {1} WHERE Id in ({2}) ".ToSecureString(),
                            task.Id, actionItem, string.Join(",".ToSecureString(), actions.Select(a => a.Id).ToArray()));
                            AttestationSnapshotAction.ExecuteQuery(updateInterim);
                        }

                        //find all the child associated with user with taskId
                        if (actions != null && actions.Count > 0)
                        {
                            //string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET TaskId = {0}, ReviewerId = {1} WHERE RootSnapshotId in ({2}) ",
                            //task.Id, actionItem, string.Join(",", actions.Select(a => a.Id).ToArray()));
                            //AttestationSnapshotAction.ExecuteQuery(updateInterim);

                            string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET TaskId = {0}, ReviewerId = {1} WHERE RootSnapshotId in (
                                SELECT AttestationSnapshotId from DI_M_AttestationSnapshotAction WHERE id in ({2})) ",
                                task.Id, actionItem, string.Join(",", actions.Select(a => a.Id).ToArray()));
                            AttestationSnapshotAction.ExecuteQuery(updateInterim);
                        }

                        //created history table reference
                        if (actions != null && actions.Count > 0)
                        {
                            string updateHistory = string.Format(@"INSERT INTO DI_M_AttestationSnapshotAction_History ([Version], AttestationSnapshotActionId, UserId, TaskId, Approve, [Revoke], Comment, EffectiveDate, ReviewerId, LastModifiedBy, LastModifiedDate)
                                                                SELECT {0}, snapAction.Id, snap.UserId, snapAction.TaskId, snapAction.Approve, snapAction.[Revoke], snapAction.Comment, snapAction.EffectiveDate, snapAction.ReviewerId, snapAction.LastModifiedBy, snapAction.LastModifiedDate
                                                                FROM DI_M_AttestationSnapshotAction snapAction
                                                                JOIN DI_M_AttestationSnapshot snap ON snapAction.AttestationSnapshotId = snap.Id
                                                                WHERE TaskId = {1} ", task.WorkflowStateId, task.Id);
                            AttestationSnapshotAction.ExecuteQuery(updateHistory);
                        }

                        //update the interim with the task details
                        if (actions != null && actions.Count > 0)
                        {
                            string updateInterim = string.Format(@"UPDATE interim 
                                                            SET interim.TaskId = act.TaskId,
                                                                interim.ReviewerId = act.ReviewerId
                                                            FROM DI_M_AttestationSnapshotActionInterim interim
                                                            INNER JOIN DI_M_AttestationSnapshotAction act ON interim.AttestationSnapshotActionId = act.Id
                                                            INNER JOIN DI_M_AttestationSnapshot snap on act.AttestationSnapshotId = snap.Id
                                                            WHERE snap.AttestationCampaignId = {0} ",
                            campaignId);
                            AttestationSnapshotAction.ExecuteQuery(updateInterim);
                        }

                        if (currentApprover.ApprovalUserType == ApprovalUserType.Group)
                        {
                            if (currentApprover.IsMockGroup || (!currentApprover.IsMockGroup && currentApprover.RequiresApprovalFromAll))
                            {
                                CreateAttestationTaskForGroupUsers(task, currentApprover, currentUser);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the CreateTask activity.";
                if (requestType == RequestType.Attestation)
                {
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Exception, message, ex);
                }
                else
                {
                    DILog.WorkflowLogger.WriteLog(LogLevelType.Exception, message, ex);
                }
            }
        }

        private void CreateAttestationTaskForGroupUsers(Task currentTask, WorkflowConfigurationDetail currentApprover, string currentUser)
        {
            try
            {
                List<ReviewInfo> reviewers = GetGroupReviewersInformation(currentTask, currentApprover);
                List<long> distinctReviewers = reviewers.Select(item => item.ReviewerId).Distinct().ToList();

                foreach (int reviewer in distinctReviewers)
                {
                    Task task = new Task()
                    {
                        ActionUserId = reviewer,
                        AttestationId = currentTask.AttestationId,
                        AttestationCampaignId = currentTask.AttestationCampaignId,
                        IsActive = true,
                        RequestTypeId = (byte)RequestType.Attestation,
                        WorkflowStateId = currentTask.WorkflowStateId,
                        WorkflowInstanceId = currentTask.WorkflowInstanceId,
                        IsGroupTask = currentTask.IsGroupTask,
                        IsActionOnlyRequiredTask = currentTask.IsActionOnlyRequiredTask,
                        GroupName = currentTask.GroupName,
                        Remarks = currentTask.Remarks,
                        Approver = currentTask.Approver,
                    };
                    
                    task.Save(currentUser);
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<ReviewInfo> GetGroupReviewersInformation(Task currentTask, WorkflowConfigurationDetail currentApprover)
        {
            try
            {
                var actions = AttestationSnapshotAction.GetByTask((long)currentTask.AttestationCampaignId, currentTask.Id).Where(x=> x.ReviewerId != null);
                List<ReviewInfo> reviewers = new List<ReviewInfo>();

                foreach(var action in actions)
                {
                    var reviewerIds = utility.GetUsersByGroupId(currentApprover.Approver);

                    foreach (int reviewerId in reviewerIds)
                    {
                        if (reviewerId != currentTask.ActionUserId)
                        {
                            ReviewInfo reviewInfo = new ReviewInfo();
                            reviewInfo.ReviewId = action.AttestationSnapshotId;
                            reviewInfo.ReviewerId = reviewerId;
                            reviewInfo.ReviewEndpointId = action.AttestationSnapshot.EndpointId;
                            reviewers.Add(reviewInfo);
                        }
                    }
                }

                return reviewers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
