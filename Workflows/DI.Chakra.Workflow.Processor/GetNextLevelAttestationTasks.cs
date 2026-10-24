 
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Services.EmailingProcessor;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.Workflow.Processor
{
    class GetNextLevelAttestationTasks
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();
        private List<long> delgateUsers = new List<long>();
        public List<Task> Execute(Task existingTask, WorkflowConfigurationDetail currentApprover)
        {
            try
            {
                List<Task> nextLevelTasks = new List<Task>();
                string currentUser = MasterConfigHelper.ServiceUser;
                List<AttestationSnapshotAction> actions = GetNextLevelReviewersInformation(existingTask, currentApprover);
                if (actions != null && actions.Count > 0)
                {
                    var currentReviewers = actions.Where(a => a.ReviewerId.HasValue && !a.IsException)
                        .Select(a => new { ReviewerId = (long)a.ReviewerId, IsReviewerGroup = a.IsReviewerGroup }).Distinct().ToList();

                    Group group = new Group();
                    bool isGroup = false;
                    if (currentApprover.ApprovalUserType == ApprovalUserType.Group)
                    {
                        group = Group.GetById(currentApprover.Approver);
                        isGroup = true;
                    }

                    AttestationCampaign attestcampaign = AttestationCampaign.GetById((long)existingTask.AttestationCampaignId, true);
                    var attEndpoints = AttestationEndpoint.GetByAttestationIdIncludeEndpoint(attestcampaign.AttestationID);
                    string attEndpointsstring = attEndpoints != null ? string.Join(",", attEndpoints.Select(x => x.EndPoint.DisplayName)) : "";
                    string comments = string.Format("{0} for Target System {1}",
                                                     attestcampaign != null ? attestcampaign.CampaignName : "-",
                                                     attEndpointsstring);

                    foreach (var reviewer in currentReviewers)
                    {
                        isGroup = reviewer.IsReviewerGroup;
                        if (isGroup)
                        {
                            group = Group.GetById(reviewer.ReviewerId);
                        }

                        Task task = new Task()
                        {
                            AttestationId = existingTask.AttestationId,
                            AttestationCampaignId = existingTask.AttestationCampaignId,
                            IsActive = true,
                            RequestTypeId = (byte)RequestType.Attestation,
                            WorkflowStateId = currentApprover.SequenceNumber,
                            WorkflowInstanceId = existingTask.WorkflowInstanceId,
                            IsActionOnlyRequiredTask = currentApprover.IsActionRequired,
                            WorkflowConfigurationId = existingTask.WorkflowConfigurationId,
                            IsGroupTask = isGroup,
                            Action = StatusType.Pending,
                            Comments = comments
                        };

                        if (currentApprover.ApprovalUserType != ApprovalUserType.Group && !isGroup)
                        {
                            task.ActionUserId = reviewer.ReviewerId;
                        }
                        else
                        {
                            if (currentApprover.IsMockGroup)
                            {
                                task.ActionUserId = reviewer.ReviewerId;
                            }
                            else
                            {
                                if (currentApprover.RequiresApprovalFromAll)
                                {
                                    task.ActionUserId = reviewer.ReviewerId;
                                }
                                else
                                {
                                    //task.ActionUserId = reviewer;
                                    task.ActionGroupId = reviewer.ReviewerId;
                                    if (!string.IsNullOrEmpty(group.GroupName))
                                        task.GroupName = group.GroupName;
                                }
                            }
                        }

                        task.Approver = utility.GetApprover(currentApprover);

                        if (!task.IsGroupTask)
                        {
                            var delegation = TaskDelegation.GetByRequestType(RequestType.Attestation, reviewer.ReviewerId);
                            if (delegation != null)
                            {
                                task.ActionUserId = GetReviewerbyDelegation(delegation.DelegatedTo);
                                task.IsDelegated = true;
                                task.DelegatedBy = User.GetById(delegation.ActionUser)?.DisplayName;
                                task.DelegatedDate = DateTime.Now;
                                task.DelegatedFrom = delgateUsers.Count > 1 ? delgateUsers[delgateUsers.Count - 2] : reviewer.ReviewerId;
                                task.DelegationComments = $"{ task.DelegationComments}<br/> Date - {task.DelegatedDate.Value.ToString("dd-MM-yyyy hh:mm tt")} By - {task.DelegatedBy}, From - {User.GetById(task.DelegatedFrom.Value)?.DisplayName}, To - {User.GetById(task.ActionUserId.Value)?.DisplayName}, Comments - {delegation.DelegationComments}";
                            }
                        }

                        task.Save(currentUser);
                        if (task.IsDelegated)
                        {
                            var deepLoadedTask = DomainModel.Task.GetAttestationTask(task.Id);
                            var emailInfo = DomainModel.EmailInfo.GetEmailInfoForDelegationTask(deepLoadedTask, task.DelegationComments);
                            GetEmailInfoForSendingEmailForProvisioningRequest(emailInfo);
                        }
                        var reviewsToUpdateWithTaskId = actions.Where(a => a.ReviewerId == reviewer.ReviewerId && a.IsReviewerGroup == reviewer.IsReviewerGroup && !a.IsException).ToList();
                        //foreach (var reviewToUpdateWithTaskId in reviewsToUpdateWithTaskId)
                        //{
                        //    reviewToUpdateWithTaskId.TaskId = task.Id;
                        //    reviewToUpdateWithTaskId.ReviewerId = task.IsGroupTask ? -1 : task.ActionUserId;
                        //    reviewToUpdateWithTaskId.Save(MasterConfigHelper.ServiceUser);
                        //}

                        //update the action with the task details
                        if (reviewsToUpdateWithTaskId != null && reviewsToUpdateWithTaskId.Count > 0)
                        {
                            string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET TaskId = {0}, ReviewerId = {1},LastModifiedDate=GETDATE() WHERE Id in ({2}) ",
                        task.Id, task.IsGroupTask ? -1 : task.ActionUserId, string.Join(",", reviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                            try
                            {
                                AttestationSnapshotAction.ExecuteQuery(updateInterim);
                            }
                            catch (Exception ex)
                            {
                                DILog.AttestationLogger.WriteLog(task.Id, LogLevelType.Exception, string.Format("error while running the update attestation action - {0}", updateInterim), ex);
                            }
                        }

                        //find all the child associated with user with taskId
                        if (reviewsToUpdateWithTaskId != null && reviewsToUpdateWithTaskId.Count > 0)
                        {
                            string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET TaskId = {0}, ReviewerId = {1},LastModifiedDate=GETDATE() WHERE RootSnapshotId in (SELECT AttestationSnapshotId from DI_M_AttestationSnapshotAction WHERE id in ({2})) ",
                            task.Id, task.IsGroupTask ? -1 : task.ActionUserId, string.Join(",", reviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                            try
                            {
                                AttestationSnapshotAction.ExecuteQuery(updateInterim);
                            }
                            catch (Exception ex)
                            {
                                DILog.AttestationLogger.WriteLog(task.Id, LogLevelType.Exception, string.Format("error while running the update attestation action with rootid - {0}", updateInterim), ex);
                            }
                        }

                        //created history table reference
                        if (actions != null && actions.Count > 0)
                        {
                            string updateHistory = string.Format(@"INSERT INTO DI_M_AttestationSnapshotAction_History ([Version], AttestationSnapshotActionId, UserId, TaskId, Approve, [Revoke], Comment, EffectiveDate, ReviewerId, LastModifiedBy, LastModifiedDate)
                                                                SELECT {0}, snapAction.Id, snap.UserId, snapAction.TaskId, snapAction.Approve, snapAction.[Revoke], snapAction.Comment, snapAction.EffectiveDate, snapAction.ReviewerId, snapAction.LastModifiedBy, snapAction.LastModifiedDate
                                                                FROM DI_M_AttestationSnapshotAction snapAction
                                                                JOIN DI_M_AttestationSnapshot snap ON snapAction.AttestationSnapshotId = snap.Id
                                                                WHERE TaskId = {1} ", task.WorkflowStateId, task.Id);

                            try
                            {
                                AttestationSnapshotAction.ExecuteQuery(updateHistory);
                            }
                            catch (Exception ex)
                            {
                                DILog.AttestationLogger.WriteLog(task.Id, LogLevelType.Exception, string.Format("error while running the update attestation action history - {0}", updateHistory), ex);
                            }
                        }

                        //update the interim with the task details
                        if (reviewsToUpdateWithTaskId != null && reviewsToUpdateWithTaskId.Count > 0)
                        {
                            string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotActionInterim SET TaskId = {0}, ReviewerId = {1}, SavedComment = '', UnSavedComment = '' WHERE AttestationSnapshotActionId in ({2}) ",
                        task.Id, task.IsGroupTask ? -1 : task.ActionUserId, string.Join(",", reviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                            try
                            {
                                AttestationSnapshotAction.ExecuteQuery(updateInterim);
                            }
                            catch (Exception ex)
                            {
                                DILog.AttestationLogger.WriteLog(task.Id, LogLevelType.Exception, string.Format("error while running the update attestation action interim - {0}", updateInterim), ex);
                            }
                        }

                        nextLevelTasks.Add(task);
                    }

                    //craeting exception task to configured user/group, if any.
                    var exceptionReviewers = actions.Where(a => a.ReviewerId.HasValue && a.IsException).Select(a => (long)a.ReviewerId).Distinct().ToList();
                    var attestationCampaignId = existingTask.AttestationCampaignId;
                    var attestationCampaign = AttestationCampaign.GetById((long)attestationCampaignId, true);
                    var attestation = attestationCampaign.Attestation;
                    if (exceptionReviewers.Any() && attestation.CreateTaskOnNoReviewer && attestation.TaskActionUserId.HasValue)
                    {
                        long exceptionReviewer = exceptionReviewers.First();
                        bool isExpectionToGroup = attestation.IsGroupTask;
                        Group exceptionGroup = new Group();
                        if (isExpectionToGroup)
                            exceptionGroup = Group.GetById(exceptionReviewer);

                       
                        if (!nextLevelTasks.Any(a => a.IsGroupTask == isGroup &&
                            (a.ActionGroupId == group.Id || a.ActionUserId == exceptionReviewer)))
                        {

                            Task task = new Task()
                            {
                                Remarks = "This task create, as part of exception configuration - Reviewer not found.",
                                AttestationId = existingTask.AttestationId,
                                AttestationCampaignId = existingTask.AttestationCampaignId,
                                IsActive = true,
                                RequestTypeId = (byte)RequestType.Attestation,
                                WorkflowStateId = currentApprover.SequenceNumber,
                                WorkflowInstanceId = existingTask.WorkflowInstanceId,
                                IsActionOnlyRequiredTask = currentApprover.IsActionRequired,
                                WorkflowConfigurationId = existingTask.WorkflowConfigurationId,
                                IsGroupTask = isExpectionToGroup,
                                Action = StatusType.Pending,
                                Comments = comments
                            };

                            if (!isExpectionToGroup)
                            {
                                task.ActionUserId = exceptionReviewer;
                                task.Approver = "User";
                            }
                            else
                            {
                                //task.ActionUserId = reviewer;
                                task.ActionGroupId = exceptionGroup.Id;
                                if (!string.IsNullOrEmpty(exceptionGroup.GroupName))
                                {
                                    task.GroupName = exceptionGroup.GroupName;
                                    task.Approver = "Group - " + exceptionGroup.GroupName;
                                }
                            }

                            if (!task.IsGroupTask)
                            {
                                var delegation = TaskDelegation.GetFinalDelegatedToApprover(RequestType.Attestation, exceptionReviewer, new List<TaskDelegation>());
                                if (delegation != null)
                                {
                                    task.ActionUserId = delegation.DelegatedTo;
                                    task.IsDelegated = true;
                                    task.DelegatedBy = "System";
                                    task.DelegatedDate = DateTime.Now;
                                    task.DelegatedFrom = exceptionReviewer;
                                    task.DelegationComments = delegation.DelegationComments;
                                }
                            }

                            task.Save(currentUser);

                            var excpetionReviewsToUpdateWithTaskId = actions.Where(a => a.ReviewerId == exceptionReviewer && a.IsReviewerGroup == isExpectionToGroup && a.IsException).ToList();
                            //update the action with the task details
                            if (excpetionReviewsToUpdateWithTaskId != null && excpetionReviewsToUpdateWithTaskId.Count > 0)
                            {
                                string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET TaskId = {0}, ReviewerId = {1},LastModifiedDate=GETDATE() WHERE Id in ({2}) ",
                            task.Id, task.IsGroupTask ? -1 : task.ActionUserId, string.Join(",", excpetionReviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                                try
                                {
                                    AttestationSnapshotAction.ExecuteQuery(updateInterim);
                                }
                                catch (Exception ex)
                                {
                                    DILog.AttestationLogger.WriteLog(task.Id, LogLevelType.Exception, string.Format("error while running the update attestation action - {0}", updateInterim), ex);
                                }
                            }

                            //find all the child associated with user with taskId
                            if (excpetionReviewsToUpdateWithTaskId != null && excpetionReviewsToUpdateWithTaskId.Count > 0)
                            {
                                string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET TaskId = {0}, ReviewerId = {1},LastModifiedDate=GETDATE() WHERE RootSnapshotId in (SELECT AttestationSnapshotId from DI_M_AttestationSnapshotAction WHERE id in ({2})) ",
                                task.Id, task.IsGroupTask ? -1 : task.ActionUserId, string.Join(",", excpetionReviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                                try
                                {
                                    AttestationSnapshotAction.ExecuteQuery(updateInterim);
                                }
                                catch (Exception ex)
                                {
                                    DILog.AttestationLogger.WriteLog(task.Id, LogLevelType.Exception, string.Format("error while running the update attestation action with rootid - {0}", updateInterim), ex);
                                }
                            }

                            //created history table reference
                            if (actions != null && actions.Count > 0)
                            {
                                string updateHistory = string.Format(@"INSERT INTO DI_M_AttestationSnapshotAction_History ([Version], AttestationSnapshotActionId, UserId, TaskId, Approve, [Revoke], Comment, EffectiveDate, ReviewerId, LastModifiedBy, LastModifiedDate)
                                                                SELECT {0}, snapAction.Id, snap.UserId, snapAction.TaskId, snapAction.Approve, snapAction.[Revoke], snapAction.Comment, snapAction.EffectiveDate, snapAction.ReviewerId, snapAction.LastModifiedBy, snapAction.LastModifiedDate
                                                                FROM DI_M_AttestationSnapshotAction snapAction
                                                                JOIN DI_M_AttestationSnapshot snap ON snapAction.AttestationSnapshotId = snap.Id
                                                                WHERE TaskId = {1} ", task.WorkflowStateId, task.Id);

                                try
                                {
                                    AttestationSnapshotAction.ExecuteQuery(updateHistory);
                                }
                                catch (Exception ex)
                                {
                                    DILog.AttestationLogger.WriteLog(task.Id, LogLevelType.Exception, string.Format("error while running the update attestation action history - {0}", updateHistory), ex);
                                }
                            }

                            //update the interim with the task details
                            if (excpetionReviewsToUpdateWithTaskId != null && excpetionReviewsToUpdateWithTaskId.Count > 0)
                            {
                                string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotActionInterim SET TaskId = {0}, ReviewerId = {1} WHERE AttestationSnapshotActionId in ({2})",
                            task.Id, task.IsGroupTask ? -1 : task.ActionUserId, string.Join(",", excpetionReviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                                try
                                {
                                    AttestationSnapshotAction.ExecuteQuery(updateInterim);
                                }
                                catch (Exception ex)
                                {
                                    DILog.AttestationLogger.WriteLog(task.Id, LogLevelType.Exception, string.Format("error while running the update attestation action interim - {0}", updateInterim), ex);
                                }
                            }

                            nextLevelTasks.Add(task);
                        }
                        else
                        {
                            var existingNextLvlTask = nextLevelTasks.Where(a => a.IsGroupTask == isGroup &&
                                (a.ActionGroupId == group.Id || a.ActionUserId == exceptionReviewer)).ToList();

                            if (existingNextLvlTask != null && existingNextLvlTask.Count > 0)
                            {
                                existingNextLvlTask.ForEach(a =>
                                {
                                    a.Remarks += "This task contains - Reviewer not found records as well.";
                                    a.Save(currentUser);
                                });

                                foreach (var nextLvlTask in existingNextLvlTask)
                                {
                                    var excpetionReviewsToUpdateWithTaskId = actions.Where(a => a.ReviewerId == exceptionReviewer && a.IsReviewerGroup == isExpectionToGroup && a.IsException).ToList();
                                    //update the action with the task details
                                    if (excpetionReviewsToUpdateWithTaskId != null && excpetionReviewsToUpdateWithTaskId.Count > 0)
                                    {
                                        string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET TaskId = {0}, ReviewerId = {1},LastModifiedDate=GETDATE() WHERE Id in ({2}) ",
                                    nextLvlTask.Id, nextLvlTask.IsGroupTask ? -1 : nextLvlTask.ActionUserId, string.Join(",", excpetionReviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                                        try
                                        {
                                            AttestationSnapshotAction.ExecuteQuery(updateInterim);
                                        }
                                        catch (Exception ex)
                                        {
                                            DILog.AttestationLogger.WriteLog(nextLvlTask.Id, LogLevelType.Exception, string.Format("error while running the update attestation action - {0}", updateInterim), ex);
                                        }
                                    }

                                    //find all the child associated with user with taskId
                                    if (excpetionReviewsToUpdateWithTaskId != null && excpetionReviewsToUpdateWithTaskId.Count > 0)
                                    {
                                        string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotAction SET TaskId = {0}, ReviewerId = {1},LastModifiedDate=GETDATE() WHERE RootSnapshotId in (SELECT AttestationSnapshotId from DI_M_AttestationSnapshotAction WHERE id in ({2})) ",
                                        nextLvlTask.Id, nextLvlTask.IsGroupTask ? -1 : nextLvlTask.ActionUserId, string.Join(",", excpetionReviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                                        try
                                        {
                                            AttestationSnapshotAction.ExecuteQuery(updateInterim);
                                        }
                                        catch (Exception ex)
                                        {
                                            DILog.AttestationLogger.WriteLog(nextLvlTask.Id, LogLevelType.Exception, string.Format("error while running the update attestation action with root id - {0}", updateInterim), ex);
                                        }
                                    }

                                    //created history table reference
                                    if (actions != null && actions.Count > 0 && excpetionReviewsToUpdateWithTaskId != null && excpetionReviewsToUpdateWithTaskId.Count > 0)
                                    {
                                        //history for action
                                        string updateHistory = string.Format(@"INSERT INTO DI_M_AttestationSnapshotAction_History ([Version], AttestationSnapshotActionId, UserId, TaskId, Approve, [Revoke], Comment, EffectiveDate, ReviewerId, LastModifiedBy, LastModifiedDate)
                                                                SELECT {0}, snapAction.Id, snap.UserId, snapAction.TaskId, snapAction.Approve, snapAction.[Revoke], snapAction.Comment, snapAction.EffectiveDate, snapAction.ReviewerId, snapAction.LastModifiedBy, snapAction.LastModifiedDate
                                                                FROM DI_M_AttestationSnapshotAction snapAction
                                                                JOIN DI_M_AttestationSnapshot snap ON snapAction.AttestationSnapshotId = snap.Id
                                                                WHERE TaskId = {1} AND snapAction.Id in ({2}) ", nextLvlTask.WorkflowStateId, nextLvlTask.Id,
                                                                string.Join(",", excpetionReviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                                        try
                                        {
                                            AttestationSnapshotAction.ExecuteQuery(updateHistory);
                                        }
                                        catch (Exception ex)
                                        {
                                            DILog.AttestationLogger.WriteLog(nextLvlTask.Id, LogLevelType.Exception, string.Format("error while running the update attestation action history1 - {0}", updateHistory), ex);
                                        }

                                        //history for child action
                                        updateHistory = string.Format(@"INSERT INTO DI_M_AttestationSnapshotAction_History ([Version], AttestationSnapshotActionId, UserId, TaskId, Approve, [Revoke], Comment, EffectiveDate, ReviewerId, LastModifiedBy, LastModifiedDate)
                                                                SELECT {0}, snapAction.Id, snap.UserId, snapAction.TaskId, snapAction.Approve, snapAction.[Revoke], snapAction.Comment, snapAction.EffectiveDate, snapAction.ReviewerId, snapAction.LastModifiedBy, snapAction.LastModifiedDate
                                                                FROM DI_M_AttestationSnapshotAction snapAction
                                                                JOIN DI_M_AttestationSnapshot snap ON snapAction.AttestationSnapshotId = snap.Id
                                                                WHERE TaskId = {1}  AND snapAction.RootSnapshotId in ({2}) ", nextLvlTask.WorkflowStateId, nextLvlTask.Id,
                                                                string.Join(",", excpetionReviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                                        try
                                        {
                                            AttestationSnapshotAction.ExecuteQuery(updateHistory);
                                        }
                                        catch (Exception ex)
                                        {
                                            DILog.AttestationLogger.WriteLog(nextLvlTask.Id, LogLevelType.Exception, string.Format("error while running the update attestation action history2 - {0}", updateHistory), ex);
                                        }
                                    }

                                    //update the interim with the task details
                                    if (excpetionReviewsToUpdateWithTaskId != null && excpetionReviewsToUpdateWithTaskId.Count > 0)
                                    {
                                        string updateInterim = string.Format(@"UPDATE DI_M_AttestationSnapshotActionInterim SET TaskId = {0}, ReviewerId = {1} WHERE AttestationSnapshotActionId in ({2})",
                                    nextLvlTask.Id, nextLvlTask.IsGroupTask ? -1 : nextLvlTask.ActionUserId, string.Join(",", excpetionReviewsToUpdateWithTaskId.Select(a => a.Id).ToArray()));

                                        try
                                        {
                                            AttestationSnapshotAction.ExecuteQuery(updateInterim);
                                        }
                                        catch (Exception ex)
                                        {
                                            DILog.AttestationLogger.WriteLog(nextLvlTask.Id, LogLevelType.Exception, string.Format("error while running the update attestation action interim - {0}", updateInterim), ex);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return nextLevelTasks;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(existingTask.Id, LogLevelType.Error,
                        "Error while getting next level attestation task.", ex);
                throw ex;
            }
        }
        private void UserApproval(long AttestationID,string CampaignName, List<Task> taskList, WorkflowConfigurationDetail currentApprover, List<AttestationSnapshotAction> reviewers, List<AttestationSnapshotAction> reviewerNotFound)
        {
            var approverUser = User.GetById(currentApprover.Approver);
            bool isApproverValid = false;
            if (approverUser != null && approverUser.IsActive)
            {
                isApproverValid = true;
            }
            //var attestationCampaign = AttestationCampaign.GetByCampaignId(taskList.First().AttestationCampaignId.Value);
            foreach (Task currentTask in taskList)
            {
                var taskSnapshotActions = AttestationSnapshotAction.GetByTask((long)currentTask.AttestationCampaignId, currentTask.Id).ToList();

                if (isApproverValid)
                {
                    taskSnapshotActions.ForEach(a =>
                    {
                        a.IsException = false;
                        a.ReviewerId = approverUser.Id;
                        a.IsReviewerGroup = false;
                        if (!reviewers.Where(b => b.Id == a.Id).Any())
                        {
                            reviewers.Add(a);
                        }
                    });

                }
                else
                {
                    taskSnapshotActions.ForEach(a =>
                    {
                        if (!reviewerNotFound.Where(b => b.Id == a.Id).Any())
                        {
                            reviewerNotFound.Add(a);
                        }

                        DILog.AttestationLogger.WriteLog(AttestationID, LogLevelType.Warning,
                        string.Format("{0}-{1} has been removed from Attestation {2} since no Manager is assigned.",
                            a.AttestationSnapshot.DIUserDisplayName, a.AttestationSnapshot.DIUserName, CampaignName));
                    });
                }
            }
        }
        private void SelfApproval(List<Task> taskList, WorkflowConfigurationDetail currentApprover, List<AttestationSnapshotAction> reviewers, List<AttestationSnapshotAction> reviewerNotFound)
        {
            var attestationCampaign = AttestationCampaign.GetByCampaignId(taskList.First().AttestationCampaignId.Value);
            foreach (Task currentTask in taskList)
            {
                var taskSnapshotActions = AttestationSnapshotAction.GetByTask((long)currentTask.AttestationCampaignId, currentTask.Id).ToList();

                foreach (var action in taskSnapshotActions)
                {
                    if (action.AttestationSnapshot.UserId <= 0)
                    {
                        reviewerNotFound.Add(action);
                        continue;
                    }

                    action.IsException = false;
                    action.ReviewerId = action.AttestationSnapshot.UserId;
                    action.IsReviewerGroup = false;
                    if (!reviewers.Where(b => b.Id == action.Id).Any())
                    {
                        reviewers.Add(action);
                    }
                }
            }
        }
        
        private void ManagerApproval(long attestationId, string campaignName, List<Task> taskList, WorkflowConfigurationDetail currentApprover, List<AttestationSnapshotAction> reviewers, List<AttestationSnapshotAction> reviewerNotFound)
        {
            //var attestationCampaign = AttestationCampaign.GetByCampaignId(taskList.First().AttestationCampaignId.Value);
            foreach (Task currentTask in taskList)
            {
                var taskSnapshotActions = AttestationSnapshotAction.GetByTask((long)currentTask.AttestationCampaignId, currentTask.Id).ToList();
                taskSnapshotActions = taskSnapshotActions.Where(a => a.AttestationSnapshot.Level == 1).ToList();
                foreach (var action in taskSnapshotActions)
                {
                    if (action.AttestationSnapshot.UserId <= 0)
                    {
                        reviewerNotFound.Add(action);
                        continue;
                    }

                    var reviewer = User.GetReportingOfficerByUser(action.AttestationSnapshot.UserId);
                    if (reviewer != null && reviewer.IsActive == true)
                    {
                        action.IsException = false;
                        action.ReviewerId = reviewer.Id;
                        action.IsReviewerGroup = false;
                        if (!reviewers.Where(b => b.Id == action.Id).Any())
                        {
                            reviewers.Add(action);
                        }
                    }
                    else
                    {
                        if (!reviewerNotFound.Where(a => a.Id == action.Id).Any())
                        {
                            reviewerNotFound.Add(action);
                        }

                        DILog.AttestationLogger.WriteLog(attestationId, LogLevelType.Warning,
                            string.Format("{0}-{1} has been removed from Attestation {2} since no Manager for organisation",
                                action.AttestationSnapshot.DIUserDisplayName, action.AttestationSnapshot.DIUserName, campaignName));
                    }
                }
            }
        }
        private void COApproval(long attestationId,string campaignName, List<Task> taskList, WorkflowConfigurationDetail currentApprover, List<AttestationSnapshotAction> reviewers, List<AttestationSnapshotAction> reviewerNotFound)
        {
            //var attestationCampaign = AttestationCampaign.GetByCampaignId(taskList.First().AttestationCampaignId.Value);
            foreach (Task currentTask in taskList)
            {
                var taskSnapshotActions = AttestationSnapshotAction.GetByTask((long)currentTask.AttestationCampaignId, currentTask.Id).ToList();
                taskSnapshotActions = taskSnapshotActions.Where(a => a.AttestationSnapshot.Level == 1).ToList();
                foreach (var action in taskSnapshotActions)
                {
                    if (action.AttestationSnapshot.UserId <= 0)
                    {
                        reviewerNotFound.Add(action);
                        continue;
                    }

                    var reviewer = User.GetCOByUser(action.AttestationSnapshot.UserId);
                    if (reviewer != null && reviewer.IsActive == true)
                    {
                        action.IsException = false;
                        action.ReviewerId = reviewer.Id;
                        action.IsReviewerGroup = false;
                        if (!reviewers.Where(b => b.Id == action.Id).Any())
                        {
                            reviewers.Add(action);
                        }
                    }
                    else
                    {
                        if (!reviewerNotFound.Where(a => a.Id == action.Id).Any())
                        {
                            reviewerNotFound.Add(action);
                        }

                        DILog.AttestationLogger.WriteLog(attestationId, LogLevelType.Warning,
                            string.Format("{0}-{1} has been removed from Attestation {2} since no Manager for organisation",
                                action.AttestationSnapshot.DIUserDisplayName, action.AttestationSnapshot.DIUserName, campaignName));
                    }
                }
            }
        }
        private void OrgScopeApproval(string orgScope, List<Task> taskList, WorkflowConfigurationDetail currentApprover, List<AttestationSnapshotAction> reviewers, List<AttestationSnapshotAction> reviewerNotFound)
        {
            var attestationCampaign = AttestationCampaign.GetByCampaignId(taskList.First().AttestationCampaignId.Value);
            foreach (Task currentTask in taskList)
            {
                var taskSnapshotActions = AttestationSnapshotAction.GetByTask((long)currentTask.AttestationCampaignId, currentTask.Id).ToList();
                taskSnapshotActions = taskSnapshotActions.Where(a => a.AttestationSnapshot.Level == 1).ToList();
                foreach (var action in taskSnapshotActions)
                {
                    if (action.AttestationSnapshot.UserId <= 0)
                    {
                        reviewerNotFound.Add(action);
                        continue;
                    }

                    var reviewer = NOrganisationHead.GetOrganisationLevelHead(action.AttestationSnapshot.UserId, orgScope);
                    if (reviewer != null && reviewer.IsActive == true)
                    {
                        action.IsException = false;
                        action.ReviewerId = reviewer.Id;
                        action.IsReviewerGroup = false;
                        if (!reviewers.Where(b => b.Id == action.Id).Any())
                        {
                            reviewers.Add(action);
                        }
                    }
                    else
                    {
                        if (!reviewerNotFound.Where(a => a.Id == action.Id).Any())
                        {
                            reviewerNotFound.Add(action);
                        }

                        DILog.AttestationLogger.WriteLog(attestationCampaign.AttestationID, LogLevelType.Warning,
                            string.Format("{0}-{1} has been removed from Attestation {2} since no Manager for organisation",
                                action.AttestationSnapshot.DIUserDisplayName, action.AttestationSnapshot.DIUserName, attestationCampaign.CampaignName));
                    }
                }
            }
        }
        private void ApplicationOwnerApproval(AttestationCampaign attestationCampaign, List<Task> taskList, WorkflowConfigurationDetail currentApprover, List<AttestationSnapshotAction> reviewers, List<AttestationSnapshotAction> reviewerNotFound)
        {
            try
            {
                //var attestationCampaign = AttestationCampaign.GetByCampaignId(taskList.First().AttestationCampaignId.Value);
                IList<AttestationSnapshotAction> actions = null;
                //AttestationReview review = null;
                long epOwnerUserId = 0;
                Dictionary<long, long> epOwners = new Dictionary<long, long>();

                // Dictionary of Endpoint System ID - Owner User ID
                epOwners = (from attEP in attestationCampaign.Attestation.AttestationEndpoint
                            select new { key = attEP.EndPoint.Id, value = attEP.EndPoint.OwnerUserId.Value })
                        .Distinct().OrderBy(c => c.value)
                        .ToDictionary(c => c.key, c => c.value);

                // iterate endpoint owners for this attestation
                foreach (var epOwner in epOwners)
                {
                    foreach (var task in taskList)
                    {
                        // get all snapshot actions for the given endpoint and campaign
                        actions = AttestationSnapshotAction.GetByEndpointAndTask(epOwner.Key, attestationCampaign.Id, task.Id);
                        epOwnerUserId = epOwner.Value;
                        var epOwnerUser = User.GetById(epOwnerUserId);

                        if (actions == null || actions.Count == 0) continue;

                        if (epOwnerUser != null && epOwnerUser.IsActive)
                        {
                            // set endpoint owner id as reviewer Id
                            foreach (var action in actions)
                            {
                                // if this user is the reviewer
                                if (epOwnerUserId == action.AttestationSnapshot.UserId)
                                {
                                    // log it and continue to next iteration
                                    DILog.AttestationLogger.WriteLog(attestationCampaign.Id, LogLevelType.Warning, string.Format("{0}-{1} has been removed from Attestation {2} since he himself is a endpoint owner.",
                                        action.AttestationSnapshot.DIUserDisplayName, action.AttestationSnapshot.DIUserName, attestationCampaign.Attestation.AttestationName));
                                    continue;
                                }
                                else
                                {
                                    action.IsException = false;
                                    action.ReviewerId = epOwnerUserId;
                                    action.IsReviewerGroup = false;
                                    if (!reviewers.Where(b => b.Id == action.Id).Any())
                                    {
                                        reviewers.Add(action);
                                    }
                                }
                            }
                        }
                        else
                        {
                            actions.ToList().ForEach(a =>
                            {
                                if (!reviewerNotFound.Where(b => b.Id == a.Id).Any())
                                {
                                    reviewerNotFound.Add(a);
                                }

                                DILog.AttestationLogger.WriteLog(attestationCampaign.Id, LogLevelType.Warning,
                                string.Format("{0}-{1} has been removed from Attestation {2} since application owner is not active.",
                                    a.AttestationSnapshot.DIUserDisplayName, a.AttestationSnapshot.DIUserName, attestationCampaign.Attestation.AttestationName));
                            });
                        }
                        // TODO: What if multiple endpoint owners for same endpoint
                        // Group Task?
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void GroupApproval(List<Task> taskList, WorkflowConfigurationDetail currentApprover, List<AttestationSnapshotAction> reviewers, List<AttestationSnapshotAction> reviewerNotFound)
        {
            long reviewer = 0;
            bool isGroup = false;
            var attestationCampaign = AttestationCampaign.GetByCampaignId(taskList.First().AttestationCampaignId.Value);
            var group = Group.GetByIdIncludingOnlyUser(currentApprover.Approver);

            if (currentApprover.IsMockGroup)
            {
                isGroup = false;
                var groupUsers = group.Users.Select(item => item.Id).ToList();

                //if first approver is group then create the task for only one user. 
                //For the other users in the group tasks will be created thru workflow.
                reviewer = groupUsers.Min();
            }
            else
            {
                if (currentApprover.RequiresApprovalFromAll)
                {
                    isGroup = false;
                    if(group != null && group.Users != null && group.Users.Count > 0)
                    {
                        var groupUsers = group.Users.Select(item => item.Id).ToList();

                        //if first approver is group then create the task for only one user. 
                        //For the other users in the group tasks will be created thru workflow.
                        reviewer = groupUsers.Min();
                    }
                }
                else
                {
                    isGroup = true;
                    reviewer = currentApprover.Approver;
                }
            }

            if (attestationCampaign != null && group != null)
            {
                var isUserExists = group.Users.Count > 0;
                foreach (var task in taskList)
                {
                    IList<AttestationSnapshotAction> actions = null;
                    actions = AttestationSnapshotAction.GetByTask(attestationCampaign.Id, task.Id).ToList();
                    foreach (var action in actions)
                    {
                        if (action.AttestationSnapshot.UserId <= 0) continue;

                        if (isUserExists)
                        {
                            action.IsException = false;
                            action.ReviewerId = reviewer;
                            action.IsReviewerGroup = isGroup;
                            if (!reviewers.Where(b => b.Id == action.Id).Any())
                            {
                                reviewers.Add(action);
                            }
                        }
                        else
                        {
                            if (!reviewerNotFound.Where(b => b.Id == action.Id).Any())
                            {
                                reviewerNotFound.Add(action);
                            }
                        }
                    }
                }
            }
        }

        private List<AttestationSnapshotAction> GetNextLevelReviewersInformation(Task task, WorkflowConfigurationDetail currentApprover)
        {
            try
            {
                List<AttestationSnapshotAction> reviewers = new List<AttestationSnapshotAction>();
                List<AttestationSnapshotAction> reviewerNotFound = new List<AttestationSnapshotAction>();
                var tasks = Task.GetByWorkflowInstanceIdAndState(task.WorkflowInstanceId, task.WorkflowStateId);
                if (tasks == null || tasks.Count <= 0) return reviewers;
                var attestationCampaign = AttestationCampaign.GetByCampaignId(tasks.First().AttestationCampaignId.Value);
                if (attestationCampaign == null) return reviewers;
                var attestation = Attestation.GetById(attestationCampaign.AttestationID);
                if (attestation == null) return reviewers;
                
                if (AttestationSnapshotAction.IsSnapshotActionExistsForCampaign(attestationCampaign.Id) == false) return reviewers;

                if (currentApprover.ApprovalUserType == ApprovalUserType.User)
                {
                    UserApproval(attestationCampaign.AttestationID, attestationCampaign.CampaignName, tasks, currentApprover, reviewers, reviewerNotFound);
                }
                else if (currentApprover.ApprovalUserType == ApprovalUserType.Role)
                {
                    var approver = RoleTypes.GetById(currentApprover.Approver);

                    bool isApproverFromOrgScope = NOrganisationScope.GetAll().Where(t => t.Scope == approver.RoleTypeDesc).Any();

                    if (isApproverFromOrgScope)
                    {
                        OrgScopeApproval(approver.RoleTypeDesc, tasks, currentApprover, reviewers, reviewerNotFound);
                    }
                    else
                    {
                        if (currentApprover.Approver == (byte)RoleType.User)
                        {
                            SelfApproval(tasks, currentApprover, reviewers, reviewerNotFound);
                        }
                        //else if (currentApprover.Approver == (byte)RoleType.Manager)
                        //{
                        //    ManagerApproval(tasks, currentApprover, reviewers, reviewerNotFound);
                        //}
                        else if (currentApprover.Approver == (byte)RoleType.ApplicationOwner)
                        {
                            ApplicationOwnerApproval(attestationCampaign,tasks, currentApprover, reviewers, reviewerNotFound);
                        }
                        else if(currentApprover.Approver == (byte)RoleType.RO || currentApprover.Approver == (byte)RoleType.Manager)
                        {
                            ManagerApproval(attestationCampaign.Id, attestationCampaign.CampaignName, tasks, currentApprover, reviewers, reviewerNotFound);
                        }
                        else if (currentApprover.Approver == (byte)RoleType.CO)
                        {
                            COApproval(attestationCampaign.Id, attestationCampaign.CampaignName, tasks, currentApprover, reviewers, reviewerNotFound);
                        }
                        else if(currentApprover.Approver == (byte)RoleType.HeadOfBranch
                            || currentApprover.Approver == (byte)RoleType.HeadOfSection
                            || currentApprover.Approver == (byte)RoleType.HeadOfDepartment
                            )
                        {
                            RoleLevelApproval(tasks, currentApprover, reviewers, reviewerNotFound);
                        }
                    }
                }
                else if (currentApprover.ApprovalUserType == ApprovalUserType.Group)
                {
                    GroupApproval(tasks, currentApprover, reviewers, reviewerNotFound);
                }

                //check for unmapped accounts
                if (attestation.IncludeUnmappedAccount)
                {
                    var snapshotActions = AttestationSnapshotAction.GetByCampaign(attestationCampaign.Id).ToList();
                    if (currentApprover.ApprovalUserType == ApprovalUserType.Group ||
                        currentApprover.ApprovalUserType == ApprovalUserType.User ||
                        (currentApprover.ApprovalUserType == ApprovalUserType.Role && currentApprover.Approver == (byte)RoleType.ApplicationOwner))
                    {
                        List<long> reviewersActionItem = new List<long>();
                        if (reviewers != null && reviewers.Count > 0)
                            reviewersActionItem.AddRange(reviewers.Select(a => a.Id).Distinct().ToList());
                        if (reviewerNotFound != null && reviewerNotFound.Count > 0)
                            reviewersActionItem.AddRange(reviewerNotFound.Select(a => a.Id).Distinct().ToList());
                        reviewersActionItem = reviewersActionItem.Distinct().ToList();
                        
                        var unmappedItems = snapshotActions.Where(a => !reviewersActionItem.Contains(a.Id)).ToList();
                        if (unmappedItems.Any())
                        {
                            if ((currentApprover.ApprovalUserType == ApprovalUserType.Role && currentApprover.Approver == (byte)RoleType.ApplicationOwner))
                            {
                                foreach (var unmappedItem in unmappedItems)
                                {
                                    EndPoint endpoint = EndPoint.GetById(unmappedItem.AttestationSnapshot.EndpointId);
                                    var reviewerId = endpoint.OwnerUserId.Value;
                                    var epOwnerUser = User.GetById(reviewerId);

                                    if (epOwnerUser != null && epOwnerUser.IsActive)
                                    {
                                        if (reviewerId != 0)
                                        {
                                            unmappedItem.IsException = false;
                                            unmappedItem.ReviewerId = reviewerId;
                                            unmappedItem.IsReviewerGroup = false;
                                        }
                                    }
                                    else
                                    {
                                        if (!reviewerNotFound.Where(b => b.Id == unmappedItem.Id).Any())
                                        {
                                            reviewerNotFound.Add(unmappedItem);
                                        }

                                        DILog.AttestationLogger.WriteLog(attestation.Id, LogLevelType.Warning,
                                        string.Format("{0}-{1} has been removed from Attestation {2} since application owner is not active.",
                                            unmappedItem.AttestationSnapshot.DIUserDisplayName, unmappedItem.AttestationSnapshot.DIUserName,
                                            attestation.AttestationName));
                                    }
                                }
                            }
                            else
                            {
                                var approver = currentApprover.Approver;
                                var isGroupReview = currentApprover.ApprovalUserType == ApprovalUserType.Group;
                                if (currentApprover.ApprovalUserType == ApprovalUserType.User)
                                {
                                    var usr = User.GetById(approver);
                                    if(usr != null && usr.IsActive == false)
                                    {
                                        approver = MasterConfigHelper.ITSecurityGroupId;
                                        isGroupReview = true;
                                    }
                                }
                                unmappedItems.ForEach(a =>
                                {
                                    a.ReviewerId = approver;
                                    if (currentApprover.ApprovalUserType == ApprovalUserType.Group)
                                        a.IsReviewerGroup = true;
                                });
                            }

                            reviewers = reviewers.Union(unmappedItems).ToList();
                        }
                    }
                    else
                    {
                        //skip this level also
                        List<long> reviewersActionItem = new List<long>();
                        if (reviewers != null && reviewers.Count > 0)
                            reviewersActionItem.AddRange(reviewers.Select(a => a.Id).Distinct().ToList());
                        if (reviewerNotFound != null && reviewerNotFound.Count > 0)
                            reviewersActionItem.AddRange(reviewerNotFound.Select(a => a.Id).Distinct().ToList());
                        var unmappedItems = snapshotActions.Where(a => !reviewersActionItem.Contains(a.Id)).ToList();
                        if (unmappedItems.Any())
                        {
                            reviewerNotFound.AddRange(unmappedItems);
                        }
                    }
                }

                if (reviewerNotFound.Count > 0)
                {
                    var attestationCampaignId = reviewerNotFound.First().AttestationSnapshot.AttestationCampaignId;
                    if (attestationCampaignId == 0) return reviewers;

                    if (attestationCampaign != null && attestation != null && attestation.SendEmailOnNoReviewer)
                    {
                        //TODO:Siva SendEmail should be written in Provisioning Email Helper.
                        SendEmailNotification(attestationCampaign, attestation, reviewerNotFound, currentApprover.SequenceNumber,task, currentApprover);
                    }

                    if (attestationCampaign != null && attestation != null && attestation.CreateTaskOnNoReviewer && attestation.TaskActionUserId.HasValue)
                    {
                        var taskActionUserId = attestation.TaskActionUserId;
                        bool isGroupTask = false;
                        if (attestation.IsGroupTask)
                        {
                            taskActionUserId = Group.GetById((long)taskActionUserId).Id;
                            isGroupTask = true;
                        }
                        else
                        {
                            var usr = User.GetById(attestation.TaskActionUserId.Value);
                            if(usr != null && usr.IsActive == false)
                            {
                                isGroupTask = true;
                                taskActionUserId = MasterConfigHelper.ITSecurityGroupId;
                            }
                        }
                        reviewerNotFound.ForEach(a => { a.ReviewerId = taskActionUserId; a.IsException = true; a.IsReviewerGroup = isGroupTask; });
                        reviewers = reviewers.Union(reviewerNotFound).ToList();
                    }
                    else
                    {
                        reviewerNotFound.ForEach(a => {
                            //This is required to avoid auto approval.
                            a.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            a.IsReviewerGroup = true;
                            });
                    }
                }
                return reviewers;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(null, LogLevelType.Warning, $"GetNextLevelReviewersInformation: {ex.Message}", ex);
                throw ex;
            }
        }
        private void RoleLevelApproval(List<Task> taskList, WorkflowConfigurationDetail currentApprover, List<AttestationSnapshotAction> reviewers, List<AttestationSnapshotAction> reviewerNotFound)
        {
            var attestationCampaign = AttestationCampaign.GetByCampaignId(taskList.First().AttestationCampaignId.Value);
            foreach (Task currentTask in taskList)
            {
                var taskSnapshotActions = AttestationSnapshotAction.GetByTask((long)currentTask.AttestationCampaignId, currentTask.Id).ToList();
                taskSnapshotActions = taskSnapshotActions.Where(a => a.AttestationSnapshot.Level == 1).ToList();
                foreach (var action in taskSnapshotActions)
                {
                    if (action.AttestationSnapshot.UserId <= 0)
                    {
                        reviewerNotFound.Add(action);
                        continue;
                    }
                    User reviewer = null;

                    if (currentApprover.Approver == (byte)RoleType.HeadOfSection)
                    {
                        var usr = User.GetById(action.AttestationSnapshot.UserId);
                        if (usr != null && !string.IsNullOrWhiteSpace(usr.SectionCode))
                        {
                            reviewer = GetAccessRequestReviewInfo(usr.SectionCode);
                        }
                    }
                    else if (currentApprover.Approver == (byte)RoleType.HeadOfBranch)
                    {
                        var usr = User.GetById(action.AttestationSnapshot.UserId);
                        if (usr != null && !string.IsNullOrWhiteSpace(usr.BranchCode))
                        {
                            reviewer = GetAccessRequestReviewInfo(usr.BranchCode);
                        }
                    }
                    else if (currentApprover.Approver == (byte)RoleType.HeadOfDepartment)
                    {
                        var usr = User.GetById(action.AttestationSnapshot.UserId);
                        if (usr != null && !string.IsNullOrWhiteSpace(usr.DepartmentCode))
                        {
                            reviewer = GetAccessRequestReviewInfo(usr.DepartmentCode);
                        }
                    }
                    if (reviewer != null && reviewer.IsActive == true)
                    {
                        action.IsException = false;
                        action.ReviewerId = reviewer.Id;
                        action.IsReviewerGroup = false;
                        if (!reviewers.Where(b => b.Id == action.Id).Any())
                        {
                            reviewers.Add(action);
                        }
                    }
                    else
                    {
                        if (!reviewerNotFound.Where(a => a.Id == action.Id).Any())
                        {
                            reviewerNotFound.Add(action);
                        }

                        DILog.AttestationLogger.WriteLog(attestationCampaign.AttestationID, LogLevelType.Warning,
                            string.Format("{0}-{1} has been removed from Attestation {2} since no Manager for organisation",
                                action.AttestationSnapshot.DIUserDisplayName, action.AttestationSnapshot.DIUserName, attestationCampaign.CampaignName));
                    }
                }
            }
        }
        private User GetAccessRequestReviewInfo(string code)
        {
            User reviewInfo = null;
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return reviewInfo;

                var nOrg = NOrganisation.GetNOrganisationByCode(code);
                if (nOrg != null)
                {
                    var nOrgHead = NOrganisationHead.GetByOrganisationId(nOrg.Id);
                    if (nOrgHead == null || !nOrgHead.HeadId.HasValue)
                        return reviewInfo;

                    reviewInfo = User.GetById(nOrgHead.HeadId.Value);
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, $"GetNextLevelAttestationTask - GetAccessRequestReviewInfo failed", ex);
            }
            return reviewInfo;
        }
        private static void AssignGroupId(long reviewer, Task task)
        {
            task.ActionGroupId = reviewer;
            var group = Group.GetById(reviewer);
            if (group != null)
            {
                task.GroupName = group.GroupName;
                var userId = Group.GetByIdAndCheckOnlyOneUsers(group.Id);
                if (userId != null && userId.HasValue && userId > 0)
                {
                    task.AssignedTo = userId;
                    task.ActionUserId = userId;
                    task.AssignedBy = userId;
                }
            }
            else
            {
                task.GroupName = "";
            }
        }
        private void SendEmailNotification(AttestationCampaign attestationCampaign, Attestation attestation, List<AttestationSnapshotAction> reviewerNotFoundList, byte approverLevel, Task currentTask, WorkflowConfigurationDetail currentWorkflowConfiguration)
        {
            try
            {
                var campaignName = attestationCampaign.CampaignName;
                DomainModel.MailTemplate objTemplate = MailTemplate.GetByTemplateName(Infrastructure.Constants.AttestationException);
                string mailContent = string.Empty;
                string mailSubject = string.Empty;

                var emailInfo = new EmailInfo();
                mailSubject = objTemplate.MailSubject;
                System.Text.StringBuilder subject = new System.Text.StringBuilder(mailSubject);
                subject.Replace("@@CAMPAIGN_NAME@@", campaignName);

                mailContent = objTemplate.MailContent;
                System.Text.StringBuilder emailBody = new System.Text.StringBuilder(mailContent);
                emailBody.Replace("@@USERREVIEWERNOTFOUND@@", string.Join("<br />", reviewerNotFoundList.Distinct().Select(a => a.AttestationSnapshot.DIUserName)));
                emailBody.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
                emailBody.Replace("@@ATTESTATION_DESC@@", attestation.AttestationDescription);
                emailBody.Replace("@@CAMPAIGN_NAME@@", campaignName);
                emailBody.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailBody.Replace("@@REVIEWLASTDATE@@", DateTime.Now.AddDays(currentWorkflowConfiguration.ResponseTimeInDays).ToString("dd/MM/yyyy"));
                emailBody.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                emailInfo.Cc = objTemplate.MailCC;
                emailInfo.Bcc = objTemplate.MailBcc;
                emailInfo.Subject = subject.ToString();
                emailInfo.To = $"{MasterConfigHelper.AccountCreatedNotificationTO},{objTemplate.MailTo}";
                emailInfo.From = objTemplate.MailFrom;
                emailInfo.Body = emailBody.ToString();
                emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error SendEmailNotification:{ex.Message} StackTrace:{ex.StackTrace}", ex);
            }
        }
        long GetReviewerbyDelegation(long ReviewerId)
        {
            delgateUsers.Add(ReviewerId);

            var delegation = TaskDelegation.GetByRequestType(RequestType.Attestation, ReviewerId);
            if (delegation != null)
            {
                return GetReviewerbyDelegation(delegation.DelegatedTo);
            }
            else
            {
                return delgateUsers[delgateUsers.Count - 1];
            }

        }
        public static bool GetEmailInfoForSendingEmailForProvisioningRequest(EmailInfo emailInfo)
        {
            bool mailSent = false;
            try
            {
                if (emailInfo != null)
                {
                    EmailHandler.Send(emailInfo);
                    mailSent = true;
                }
            }
            catch (Exception ex)
            {
                mailSent = false;
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error:{ex.Message} StackTrace:{ex.StackTrace}", ex, emailInfo.Id, emailInfo.Subject);
            }
            return mailSent;
        }
        //public static void NotifyAttestationNoReviewerFound(AttestationCampaign attestationCampaign, Attestation attestation, List<AttestationSnapshotAction> reviewerNotFoundList, byte approverLevel,Task currentTask, WorkflowConfigurationDetail currentWorkflowConfiguration)
        //{
        //    var utility = new UtilityWFFunctions();
        //    IList<EmailInfo> listMails = new List<EmailInfo>();
        //    EmailInfo emailInfo = null;

        //    try
        //    {
        //        var campaignName = attestationCampaign.CampaignName;
        //        MailTemplate mailTemplate = MailTemplate.GetByTemplateName(Infrastructure.Constants.AttestationException);
        //        if (mailTemplate != null)
        //        {
        //            string mailContent = string.Empty;
        //            string mailSubject = string.Empty;
        //            emailInfo = new EmailInfo();
        //            mailContent = mailTemplate.MailContent;
        //            mailSubject = mailTemplate.MailSubject;
        //            System.Text.StringBuilder subject = new System.Text.StringBuilder(mailSubject);
        //            subject.Replace("@@APPROVAL_LEVEL@@", approverLevel.ToString());
        //            subject.Replace("@@ATTESTATIONCAMPAIGN_NAME@@", campaignName);
        //            emailInfo.To = $"{string.Join(",", MasterConfigHelper.AlertNotificationReceivers)},{mailTemplate.MailTo}";
        //            emailInfo.From = mailTemplate.MailFrom;
        //            emailInfo.Cc = mailTemplate.MailCC;
        //            emailInfo.Bcc = mailTemplate.MailBcc;
        //            System.Text.StringBuilder bldrAD = new System.Text.StringBuilder(mailContent);
        //            bldrAD.Replace("@@RECEIVER@@", "Receiver");
        //            bldrAD.Replace("@@USERREVIEWERNOTFOUND@@", string.Join("<br />", reviewerNotFoundList.Select(a => a.AttestationSnapshot.DIUserName)));
        //            bldrAD.Replace("@@APPROVAL_LEVEL@@", approverLevel.ToString());
        //            bldrAD.Replace("@@ATTESTATIONCAMPAIGN_NAME@@", campaignName);
        //            bldrAD.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
        //            bldrAD.Replace("@@ATTESTATION_DESC@@", attestation.AttestationDescription);
        //            bldrAD.Replace("@@REVIEWLASTDATE@@", utility.GetTaskCompletionDate(currentWorkflowConfiguration.ResponseTimeInDays, currentTask.CreatedDate).ToString("dd/MM/yyyy"));
        //            bldrAD.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
        //            bldrAD.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
        //            emailInfo.Subject = subject.ToString();
        //            emailInfo.Body = bldrAD.ToString();
        //            emailInfo.IsHtml = true;
        //            emailInfo.CreatedBy = "System";
        //            emailInfo.CreatedDate = DateTime.Now;
        //            emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
        //            emailInfo.CreatedDateTime = DateTime.Now;
        //            listMails.Add(emailInfo);
        //            EmailHandler.Send(listMails);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.AttestationLogger.WriteLog(LogLevelType.Exception, $"Error while sending email when there is no approver.Error Message - {ex.Message}", ex);
        //    }
        //}
    }
}
