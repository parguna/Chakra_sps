using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    [Serializable]
    public class PresentationAttestationEntitlementReviewDetail
    {
        public long Level { get; set; }

        public long BindingLevel { get; set; }
        public long? ParentBindingLevel { get; set; }
        public long KeyId { get; set; }

        public long EndpointSystemId { get; set; }

        public string EndpointSystemName { get; set; }

        public EndpointType EndpointType { get; set; }

        public long SnapshotActionId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Type { get; set; }

        public AccountType AccountType { get; set; }

        public string ReviewCategory { get; set; }
        public string DIUserName { get; set; }
        public string AccountName { get; set; }
        public bool Approved { get; set; }

        public bool Rejected { get; set; }

        public Nullable<DateTime> EffectiveDate { get; set; }

        public string Comments { get; set; }

        public string Entity { get; set; }
        public string ParentEntity { get; set; }

        public bool IsSODConflict { get; set; }

        public bool IsSensitiveAccess { get; set; }

        public bool IsChanged { get; set; }
        public bool IsChangedInPreviousLevel { get; set; }

        public bool IsDelta { get; set; }

        public bool IsNextLevelAvailable { get; set; }

        public bool IsActionable { get; set; }

        public bool IsActionExtendable { get; set; }

        public string LastLogon { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string Reserved6 { get; set; }
        public string Reserved7 { get; set; }
        public string Reserved8 { get; set; }
        public string Reserved9 { get; set; }
        public string Reserved10 { get; set; }
        public string ActionReserved1 { get; set; }
        public string ActionReserved2 { get; set; }
        public string ActionReserved3 { get; set; }
        public long RequesterUserId { get; internal set; }
        public long EndpointSystemType { get; internal set; }
        public AdHocActionType AdhocActionType { get; internal set; }
        public long RequesteeUserId { get; internal set; }
        public long EntitlementId { get; internal set; }
        public bool AccountStatus { get; set; }
        static PresentationAttestationSnapshotActionsForTask GetTask(long taskId, bool isInitialLoad, string currentUser)
        {
            try
            {
                PresentationAttestationSnapshotActionsForTask action = null;
                Task task = DomainModel.Task.GetById(taskId);

                if (task.IsCompleted)
                {
                    action = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdFromHistory(taskId, (long)task.AttestationCampaignId, false);
                    if (isInitialLoad && action != null && action.AttestationSnapshotActions != null && action.AttestationSnapshotActions.Count > 0 && action.AttestationSnapshotActions.Any(a => a.IsChanged))
                        action.AttestationSnapshotActions = action.AttestationSnapshotActions.OrderByDescending(item => item.IsChanged).ToList();
                    return action;
                }

                action = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskId(taskId, (long)task.AttestationCampaignId);

                if (action != null && action.AttestationSnapshotActions != null && action.AttestationSnapshotActions.Count > 0 && action.AttestationSnapshotActions.Any(a => a.IsChangedInPreviousLevel))
                {
                    action.AttestationSnapshotActions = action.AttestationSnapshotActions.OrderByDescending(a => a.IsChangedInPreviousLevel).ThenBy(a => a.AttestationSnapshot.Entity).ToList();
                }
                else
                {
                    action.AttestationSnapshotActions = action.AttestationSnapshotActions.OrderBy(a => a.AttestationSnapshot.Entity).ToList();
                }

                return action;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting Task data", ex);
                return null;
            }
        }

        static PresentationAttestationSnapshotActionsForTask GetTask(long taskId, long levelToLoad, string entity, string accountName, bool isInitialLoad, string currentUser)
        {
            try
            {
                PresentationAttestationSnapshotActionsForTask action = null;
                Task task = DomainModel.Task.GetById(taskId);

                if (task.IsCompleted)
                {
                    action = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdFromHistory(taskId, (long)task.AttestationCampaignId, false, entity);
                    if (isInitialLoad && action != null && action.AttestationSnapshotActions != null && action.AttestationSnapshotActions.Count > 0 && action.AttestationSnapshotActions.Any(a => a.IsChanged))
                        action.AttestationSnapshotActions = action.AttestationSnapshotActions.OrderByDescending(item => item.IsChanged).ToList();
                    return action;
                }

                action = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskId(taskId, (long)task.AttestationCampaignId, levelToLoad, entity, accountName);

                action.AttestationSnapshotActions = action.AttestationSnapshotActions.OrderBy(a => a.AttestationSnapshot.Entity).ToList();

                return action;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting Task data", ex);
                return null;
            }
        }

        /// <summary>
        /// Checks whether all the reviews in the Task have been marked asa either Approved or Revoked.
        /// </summary>
        /// <returns>If any of the account or entitlement review is neither approved nor revoked, returns false; else returns true.</returns>
        public static bool CanSubmitTask(long taskId)
        {
            try
            {
                // get unsaved task data
                var orgTask = Task.GetById(taskId);
                if (orgTask != null && orgTask.AttestationCampaignId.HasValue)
                {
                    var task = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskId(taskId, orgTask.AttestationCampaignId.Value);
                    var incompleteExists = (from taskReview in task.AttestationSnapshotActions
                                                // where action is actionable and is neither Approved nor Revoked
                                            where (taskReview.IsActionable && taskReview.Revoke == false && taskReview.Approve == false)
                                            select taskReview).Any();

                    if (!incompleteExists && !task.IsUnSavedChangesAvailable)
                        return true;
                }

                // default - no submit allowed
                return false;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while checking Task data", ex);
                return false;
            }
        }

        public static bool UpdateAttestationReviewAction(long snapshotActionId, bool value, string accessType, long taskId, string currentUser, long taskActionUserId)
        {
            try
            {
                EndPoint endPoint = null;
                var orgTask = Task.GetById(taskId);
                var attestationTask = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                if (orgTask != null && orgTask.AttestationCampaignId.HasValue)
                {
                    
                    var task = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdByActionId(taskId, orgTask.AttestationCampaignId.Value, snapshotActionId);

                    if (task != null && task.AttestationSnapshotActions.Count > 0)
                    {
                        bool approve; bool revoke;
                        if (accessType == "Revoke")
                        {
                            revoke = value;
                            approve = !value;
                        }
                        else
                        {
                            approve = value;
                            revoke = !value;
                        }

                        //set the action detail for the given snapshotid       
                        List<long> actionIdsToUpdate = new List<long>();
                        List<long> parentActionIdsToUpdate = new List<long>();
                        var snapshotAction = task.AttestationSnapshotActions.Where(a => a.Id == snapshotActionId).FirstOrDefault();

                        if (snapshotAction != null)
                        {
                            var endpointSystemId = snapshotAction.AttestationSnapshot.EndpointId;
                            endPoint = EndPoint.GetById(endpointSystemId);
                            actionIdsToUpdate.Add(snapshotAction.Id);
                            #region For EBS MSSL Only
                            if (snapshotAction.AttestationSnapshot != null && snapshotAction.AttestationSnapshot.EntitlementType == EntitlementType.Account || snapshotAction.AttestationSnapshot.EntitlementType == EntitlementType.Position)
                            {
                                if (endPoint != null && (endPoint.ApplicationType == ApplicationType.MSSL_CRM || endPoint.ApplicationType == ApplicationType.MSSL_ISU ||
                                    endPoint.ApplicationType == ApplicationType.EBS_CRM || endPoint.ApplicationType == ApplicationType.EBS_ISU))
                                {
                                    
                                    EndPoint dependentEndpoint = null;
                                    ApplicationType dependentApplicationType = ApplicationType.None;
                                    if (endPoint.ApplicationType == ApplicationType.MSSL_CRM || endPoint.ApplicationType == ApplicationType.MSSL_ISU)
                                    {
                                        dependentApplicationType = endPoint.ApplicationType == Infrastructure.Enums.ApplicationType.MSSL_CRM ? Infrastructure.Enums.ApplicationType.MSSL_ISU : Infrastructure.Enums.ApplicationType.MSSL_CRM;
                                    }
                                    else if (endPoint.ApplicationType == ApplicationType.EBS_CRM || endPoint.ApplicationType == ApplicationType.EBS_ISU)
                                    {
                                        dependentApplicationType = endPoint.ApplicationType == Infrastructure.Enums.ApplicationType.EBS_CRM ? Infrastructure.Enums.ApplicationType.EBS_ISU : Infrastructure.Enums.ApplicationType.EBS_CRM;
                                    }
                                    dependentEndpoint = EndPoint.GetByApplicationType(dependentApplicationType);
                                    if (dependentEndpoint != null)
                                    {

                                        if (snapshotAction.AttestationSnapshot.EntitlementType == EntitlementType.Account)
                                        {
                                            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
                                            {
                                                var attestSnapShotAction = new AttestationSnapshotActionRepository(transaction).GetDependentAttestationSnapshotAction(snapshotAction.AttestationSnapshot.AccountName, dependentEndpoint.Id, orgTask.AttestationCampaignId.Value, null, EntitlementType.Account);
                                                if (attestSnapShotAction != null)
                                                {
                                                    actionIdsToUpdate.Add(attestSnapShotAction.Id);
                                                    var dependentTask = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdByActionId(taskId, orgTask.AttestationCampaignId.Value, attestSnapShotAction.Id);
                                                    if (dependentTask != null && dependentTask.AttestationSnapshotActions.Any())
                                                    {
                                                        task.AttestationSnapshotActions.AddRange(dependentTask.AttestationSnapshotActions);
                                                    }
                                                }
                                            }
                                            
                                        }
                                        else if (snapshotAction.AttestationSnapshot.EntitlementType == EntitlementType.Position)
                                        {
                                            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
                                            {
                                                var attestSnapShotAction = new AttestationSnapshotActionRepository(transaction).GetDependentAttestationSnapshotAction(snapshotAction.AttestationSnapshot.AccountName, dependentEndpoint.Id, orgTask.AttestationCampaignId.Value, snapshotAction.AttestationSnapshot.Reserved8, EntitlementType.Position);
                                                if (attestSnapShotAction != null)
                                                {
                                                    actionIdsToUpdate.Add(attestSnapShotAction.Id);
                                                }
                                            }
                                                
                                        }
                                    }
                                }
                            }
                            #endregion
                        }

                        //set action value to all its child entry in snapshot
                        if (snapshotAction != null && snapshotAction.AttestationSnapshot != null)
                        {
                            var maxLevel = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.EndpointId == endPoint.Id).Select(a => a.AttestationSnapshot.Level).Max();
                            getChildSnapshotActionIds(snapshotAction, task, maxLevel, taskId, taskActionUserId, currentUser, actionIdsToUpdate);

                            //update ischanged value for all the parent
                            if (!string.IsNullOrEmpty(snapshotAction.AttestationSnapshot.ParentEntity))
                            {
                                getParentSnapshotActionIds(snapshotAction, task, parentActionIdsToUpdate);
                            }
                        }

                        if (attestationTask != null)
                        {
                            attestationTask.UnSavedChangesAvailable = true;
                            attestationTask.Save(currentUser);
                        }
                        else
                        {
                            attestationTask = new AttestationSnapshotTaskStatus();
                            attestationTask.AttestationId = orgTask.AttestationId.HasValue ? orgTask.AttestationId.Value : 0;
                            attestationTask.CampaignId = orgTask.AttestationCampaignId.Value;
                            attestationTask.TaskId = taskId;
                            attestationTask.UnSavedChangesAvailable = true;
                            attestationTask.Save(currentUser);
                        }

                        //store the changes into interim
                        // LastModifiedBy embeds currentUser (attacker-influenced) as a SQL string
                        // value, so each entry now carries the query template ("{0}", "{1}" ...
                        // are EF SqlParameter tokens, not string.Format substitutions) alongside
                        // the actual parameter values, and ExecuteQuery(List<Tuple<string,object[]>>)
                        // binds them as real parameters instead of concatenating text into the SQL.
                        List<Tuple<string, object[]>> updateDBScripts = new List<Tuple<string, object[]>>();
                        var additionalUpdate = string.Empty;
                        if (approve)
                        {
                            additionalUpdate = ",UnSavedEffectiveDate=NULL";
                        }
                        updateDBScripts.Add(Tuple.Create<string, object[]>(
                        @"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedApprove = {0}, UnSavedRevoke = {1}, UnSavedIsChanged = {2}, TaskId = {3}, ReviewerId = {4}, LastModifiedBy = {5} " + additionalUpdate + " WHERE AttestationSnapshotActionId in (" + string.Join(",", actionIdsToUpdate.Select(a => a)) + ")",
                        new object[] { approve ? 1 : 0, revoke ? 1 : 0, 1, taskId, taskActionUserId, currentUser }));

                        //EndPoint ToUpdateEp = null;

                        //if (endPoint != null && (endPoint.ApplicationType == ApplicationType.MSSL_CRM || endPoint.ApplicationType == ApplicationType.MSSL_ISU || endPoint.ApplicationType == ApplicationType.EBS_CRM || endPoint.ApplicationType == ApplicationType.EBS_ISU))
                        //{
                            
                        //    if(endPoint.ApplicationType == ApplicationType.MSSL_CRM || endPoint.ApplicationType == ApplicationType.MSSL_ISU)
                        //    {
                        //        ToUpdateEp = EndPoint.GetByApplicationType(endPoint.ApplicationType == ApplicationType.MSSL_CRM ? ApplicationType.MSSL_ISU
                        //            : ApplicationType.MSSL_CRM);
                        //    }
                        //    if (endPoint.ApplicationType == ApplicationType.EBS_CRM || endPoint.ApplicationType == ApplicationType.EBS_ISU)
                        //    {
                        //        ToUpdateEp = EndPoint.GetByApplicationType(endPoint.ApplicationType == ApplicationType.EBS_CRM ? ApplicationType.EBS_ISU
                        //            : ApplicationType.EBS_CRM);
                        //    }
                        //}

                        if (snapshotAction != null && snapshotAction.AttestationSnapshot.Level > 1 && approve)
                        {
                            if (parentActionIdsToUpdate.Count > 0)
                                updateDBScripts.Add(Tuple.Create<string, object[]>(
                            @"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedEffectiveDate=null, UnSavedApprove = 1, UnSavedRevoke = 0 ,UnSavedIsChanged = {0}, TaskId = {1}, ReviewerId = {2}, LastModifiedBy = {3} WHERE AttestationSnapshotActionId in (" + string.Join(",", parentActionIdsToUpdate.Select(a => a)) + ")",
                            new object[] { 1, taskId, taskActionUserId, currentUser }));
                        }
                        else
                        {
                            if(parentActionIdsToUpdate.Count > 0)
                            updateDBScripts.Add(Tuple.Create<string, object[]>(
                            @"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedIsChanged = {0}, TaskId = {1}, ReviewerId = {2}, LastModifiedBy = {3} WHERE AttestationSnapshotActionId in (" + string.Join(",", parentActionIdsToUpdate.Select(a => a)) + ")",
                            new object[] { 1, taskId, taskActionUserId, currentUser }));
                        }

                        AttestationSnapshotAction.ExecuteQuery(updateDBScripts);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while updating attestation review in snapshot", ex);
                return false;
            }
            return true;
        }

        public static bool UpdateAttestationReviewComments(long snapshotActionId, string comments, string category, long taskId, string currentUser, long taskActionUserId)
        {
            try
            {
                var orgTask = Task.GetById(taskId);
                var attestationTask = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                if (orgTask != null && orgTask.AttestationCampaignId.HasValue)
                {
                    var task = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskId(taskId, orgTask.AttestationCampaignId.Value);

                    if (task != null && task.AttestationSnapshotActions.Count > 0)
                    {
                        //set the action detail for the given snapshotid
                        List<long> actionIdsToUpdate = new List<long>();
                        var snapshotAction = task.AttestationSnapshotActions.FirstOrDefault(a => a.Id == snapshotActionId);
                        if (snapshotAction != null)
                        {
                            actionIdsToUpdate.Add(snapshotAction.Id);
                        }

                        if (attestationTask != null)
                        {
                            attestationTask.UnSavedChangesAvailable = true;
                            attestationTask.Save(currentUser);
                        }
                        else
                        {
                            attestationTask = new AttestationSnapshotTaskStatus();
                            attestationTask.AttestationId = orgTask.AttestationId.HasValue ? orgTask.AttestationId.Value : 0;
                            attestationTask.CampaignId = orgTask.AttestationCampaignId.Value;
                            attestationTask.TaskId = taskId;
                            attestationTask.UnSavedChangesAvailable = true;
                            attestationTask.Save(currentUser);
                        }

                        //store the changes into interim
                        // comments and currentUser are attacker-influenced string values, so they
                        // are bound as real SqlParameters ("{0}"/"{1}" are EF tokens, not
                        // string.Format substitutions) instead of being concatenated into the SQL.
                        string updateDBScripts = @"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedComment = {0}, UnSavedIsChanged = {1}, TaskId = {2}, ReviewerId = {3}, LastModifiedBy = {4} WHERE AttestationSnapshotActionId in (" + string.Join(",", actionIdsToUpdate.Select(a => a)) + ")";

                        AttestationSnapshotAction.ExecuteQuery(updateDBScripts, comments, 1, taskId, taskActionUserId, currentUser);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while updating attestation review comments", ex);
                return false;
            }
            return true;
        }

        public static bool UpdateAttestationReviewReservedColumn(long snapshotActionId, string value, string accessType, long taskId, string currentUser, long taskActionUserId)
        {
            try
            {
                var orgTask = Task.GetById(taskId);
                var attestationTask = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                if (orgTask != null && orgTask.AttestationCampaignId.HasValue)
                {
                    var task = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskId(taskId, orgTask.AttestationCampaignId.Value);

                    if (task != null && task.AttestationSnapshotActions.Count > 0)
                    {
                        //set the action detail for the given snapshotid    
                        List<long> actionIdsToUpdate = new List<long>();
                        var snapshotAction = task.AttestationSnapshotActions.Where(a => a.Id == snapshotActionId).FirstOrDefault();
                        var endpointSystemId = snapshotAction.AttestationSnapshot.EndpointId;
                        if (snapshotAction != null)
                        {
                            actionIdsToUpdate.Add(snapshotAction.Id);
                        }

                        if (attestationTask != null)
                        {
                            attestationTask.UnSavedChangesAvailable = true;
                            attestationTask.Save(currentUser);
                        }
                        else
                        {
                            attestationTask = new AttestationSnapshotTaskStatus();
                            attestationTask.AttestationId = orgTask.AttestationId.HasValue ? orgTask.AttestationId.Value : 0;
                            attestationTask.CampaignId = orgTask.AttestationCampaignId.Value;
                            attestationTask.TaskId = taskId;
                            attestationTask.UnSavedChangesAvailable = true;
                            attestationTask.Save(currentUser);
                        }

                        //store the changes into interim
                        // value and currentUser are attacker-influenced string values, so they
                        // are bound as real SqlParameters ("{0}"/"{1}" are EF tokens, not
                        // string.Format substitutions) instead of being concatenated into the SQL.
                        string updateDBScripts = @"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedReserved1 = {0}, UnSavedIsChanged = {1}, TaskId = {2}, ReviewerId = {3}, LastModifiedBy = {4} WHERE AttestationSnapshotActionId in (" + string.Join(",", actionIdsToUpdate.Select(a => a)) + ")";

                        AttestationSnapshotAction.ExecuteQuery(updateDBScripts, value, 1, taskId, taskActionUserId, currentUser);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while updating attestation review in snapshot", ex);
                return false;
            }
            return true;
        }

        private static void getChildSnapshotActionIds(AttestationSnapshotAction attestationSnapshotAction, PresentationAttestationSnapshotActionsForTask task, long maxLevel, long taskId, long taskActionUserId, string currentUser,
            List<long> idsToUpdate)
        {
            var currentSnapshot = attestationSnapshotAction.AttestationSnapshot;
            var currentLevel = currentSnapshot.Level;
            var endpointSystemId = currentSnapshot.EndpointId;
            var campaignId = currentSnapshot.AttestationCampaignId;
            string accountName = currentSnapshot.AccountName;

            var childActions = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level > currentLevel && a.IsActionable && a.IsVisible).ToList();
            if (childActions != null && childActions.Count > 0)
            {
                childActions.ForEach(a => { idsToUpdate.Add(a.Id); });
            }
        }
        private static List<AttestationSnapshotAction> GetParentSnapshotActionIds(AttestationSnapshotAction attestationSnapshotAction, PresentationAttestationSnapshotActionsForTask task,
            List<long> idsToUpdate)
        {
            var currentSnapshot = attestationSnapshotAction.AttestationSnapshot;
            var currentLevel = currentSnapshot.Level;
            var endpointSystemId = currentSnapshot.EndpointId;
            var parententity = new List<string>() { currentSnapshot.ParentEntity };
            string accountName = currentSnapshot.AccountName;

            var parentActions = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level < currentLevel && a.IsActionable && a.IsVisible).ToList();
            if (parentActions != null && parentActions.Count > 0)
            {
                parentActions.ForEach(a => { idsToUpdate.Add(a.Id); });
                return parentActions;
            }
            return null;
        }

        private static void getParentSnapshotActionIds(AttestationSnapshotAction attestationSnapshotAction, PresentationAttestationSnapshotActionsForTask task,
            List<long> idsToUpdate)
        {
            var currentSnapshot = attestationSnapshotAction.AttestationSnapshot;
            var currentLevel = currentSnapshot.Level;
            var endpointSystemId = currentSnapshot.EndpointId;
            var parententity = new List<string>() { currentSnapshot.ParentEntity };
            string accountName = currentSnapshot.AccountName;

            var parentActions = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level < currentLevel && a.IsVisible).ToList();
            if (parentActions != null && parentActions.Count > 0)
            {
                parentActions.ForEach(a => { idsToUpdate.Add(a.Id); });
            }
        }

        public static bool SaveAttestationReview(long taskId, string currentUser)
        {
            try
            {
                AttestationSnapshotAction.ExecuteQuery(string.Format(@"UPDATE interim 
                    set interim.SavedApprove = interim.UnSavedApprove,
                        interim.SavedRevoke = interim.UnSavedRevoke,
                        interim.SavedComment = interim.UnSavedComment,
                        interim.SavedEffectiveDate = interim.UnSavedEffectiveDate,
                        interim.SavedIsChanged = interim.UnSavedIsChanged,
                        interim.SavedReserved1 = interim.UnSavedReserved1,
                        interim.SavedReserved2 = interim.UnSavedReserved2,
                        interim.SavedReserved3 = interim.UnSavedReserved3
                    FROM DI_M_AttestationSnapshotActionInterim interim
                    INNER JOIN DI_M_AttestationSnapshotAction act ON interim.AttestationSnapshotActionId = act.Id
                    WHERE interim.TaskId = {0}", taskId));

                var attestationTask = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                var orgTask = Task.GetById(taskId);
                if (attestationTask != null)
                {
                    attestationTask.UnSavedChangesAvailable = false;
                    attestationTask.Save(currentUser);
                }
                else
                {
                    attestationTask = new AttestationSnapshotTaskStatus();
                    attestationTask.AttestationId = orgTask.AttestationId.HasValue ? orgTask.AttestationId.Value : 0;
                    attestationTask.CampaignId = orgTask.AttestationCampaignId.Value;
                    attestationTask.TaskId = taskId;
                    attestationTask.UnSavedChangesAvailable = false;
                    attestationTask.Save(currentUser);
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while saving attestation review changes", ex);
                return false;
            }

            return true;
        }

        public static bool SaveAttestationCampaignAttachment(long taskId, string currentUser, AttestationCampaignAttachment attachment)
        {
            try
            {
                Task task = Task.GetById(taskId);
                long campaignId = (long)task.AttestationCampaignId;
                AttestationCampaign campaign = AttestationCampaign.GetById(campaignId);
                attachment.AttestationCampaignId = campaignId;
                attachment.Save(currentUser);
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while saving attestation attachment", ex);
                return false;
            }

            return true;
        }

        public static bool UpdateAttestationComment(long campaignId, string currentUser, string comment)
        {
            try
            {
                var usr = User.GetByUserName(currentUser);
                AttestationCampaign campaign = AttestationCampaign.GetById(campaignId);
                string prevComments = Convert.ToString(campaign.Comments); // get any previous comments
                campaign.Comments = $"{campaign.Comments}{Environment.NewLine}Update By:[{usr.DisplayName}({usr.UserName})] On [{DateTime.Now.ToString("dd-MM-yyyy hh:mm tt")}] {comment} {"!~"}";
                //campaign.Comments = String.Format("{1}{0}{2}{0} Update By:[{3}({6})] On [{4}]{5}",
                //   Environment.NewLine, campaign.Comments, comment, usr.DisplayName, DateTime.Now.ToString("dd-MM-yyyy hh:mm tt"), "!~", usr.UserName); // concatenate to previous comments
                campaign.Save(currentUser);
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, "Error while saving attestation attachment", ex);
                return false;
            }

            return true;
        }

        public static bool RevertAttestationReview(long taskId, string currentUser)
        {
            try
            {
                AttestationSnapshotAction.ExecuteQuery(string.Format(@"UPDATE interim 
                    set interim.UnSavedApprove = interim.SavedApprove,
                        interim.UnSavedRevoke = interim.SavedRevoke,
                        interim.UnSavedComment = interim.SavedComment,
                        interim.UnSavedEffectiveDate = interim.SavedEffectiveDate,
                        interim.UnSavedIsChanged = interim.SavedIsChanged,
                        interim.UnSavedReserved1 = interim.SavedReserved1,
                        interim.UnSavedReserved2 = interim.SavedReserved2,
                        interim.UnSavedReserved3 = interim.SavedReserved3
                    FROM DI_M_AttestationSnapshotActionInterim interim
                    INNER JOIN DI_M_AttestationSnapshotAction act ON interim.AttestationSnapshotActionId = act.Id
                    WHERE interim.TaskId = {0}", taskId));

                var attestationTask = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                var orgTask = Task.GetById(taskId);
                if (attestationTask != null)
                {
                    attestationTask.UnSavedChangesAvailable = false;
                    attestationTask.Save(currentUser);
                }
                else
                {
                    attestationTask = new AttestationSnapshotTaskStatus();
                    attestationTask.AttestationId = orgTask.AttestationId.HasValue ? orgTask.AttestationId.Value : 0;
                    attestationTask.CampaignId = orgTask.AttestationCampaignId.Value;
                    attestationTask.TaskId = taskId;
                    attestationTask.UnSavedChangesAvailable = false;
                    attestationTask.Save(currentUser);
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while reverting attestation review changes", ex);
                return false;
            }

            return true;
        }

        public static bool GetTaskHasUnsavedChanges(long taskId, string currentUser)
        {
            try
            {
                var task = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                if (task == null) return false;
                return task.UnSavedChangesAvailable;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting unsaved change details for given task.", ex);
                return false;
            }
        }

        public static AttestationType GetAttestationTypeForTask(long taskId)
        {
            try
            {
                var task = Task.GetByIdAttestationTaskIncluded(taskId);
                if (task == null || task.Attestation == null)  return AttestationType.User;
                return task.Attestation.AttestationType;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting attestation type details for given task.", ex);
                return AttestationType.User;
            }
        }

        static List<AttestationSnapshotAction> needToOrder = new List<AttestationSnapshotAction>();

        public static IEnumerable<AttestationSnapshotAction> GetList(string parentEntity = null, string accountName = null)
        {

            foreach (var item in needToOrder
               .Where(x => x.AttestationSnapshot.ParentEntity == parentEntity &&
               (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(x.AttestationSnapshot.AccountName) || x.AttestationSnapshot.AccountName == accountName))
               .OrderBy(x => x.AttestationSnapshot.Level)
               .ToList())
            {

                yield return item;

                foreach (var child in GetList(item.AttestationSnapshot.Entity, item.AttestationSnapshot.AccountName))
                {
                    yield return child;
                }

            }
        }

        public static IEnumerable<AttestationSnapshotAction> GetList()
        {
            List<AttestationSnapshotAction> ReportingOrder = new List<AttestationSnapshotAction>();
            long levelToLoad = 1;
            if (needToOrder != null && needToOrder.Count > 0)
            {
                levelToLoad = needToOrder.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
            }
            var level1 = needToOrder.Where(a => a.AttestationSnapshot.Level == levelToLoad).ToList();
            if (level1 != null && level1.Count > 0)
            {
                foreach (var level in level1)
                {
                    ReportingOrder.Add(level);
                    //check for child
                    ReportingOrder.AddRange(GetChildList(level));
                }
            }
            return ReportingOrder;
        }

        static IEnumerable<AttestationSnapshotAction> GetChildList(AttestationSnapshotAction parent)
        {
            List<AttestationSnapshotAction> ReportingOrder = new List<AttestationSnapshotAction>();
            if (parent != null)
            {
                var parentEntity = parent.AttestationSnapshot.Entity;
                var accountName = parent.AttestationSnapshot.AccountName;

                var childs = needToOrder.Where(x => x.AttestationSnapshot.EndpointId == parent.AttestationSnapshot.EndpointId && x.AttestationSnapshot.ParentEntity == parentEntity &&
                x.AttestationSnapshot.Level == (parent.AttestationSnapshot.Level + 1) &&
               (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(x.AttestationSnapshot.AccountName) || x.AttestationSnapshot.AccountName == accountName))
               .OrderBy(x => x.AttestationSnapshot.Level).ToList();

                if (childs != null && childs.Count > 0)
                {
                    foreach (var child in childs)
                    {
                        ReportingOrder.Add(child);
                        var childRows = GetChildList(child);
                        if (childRows != null && childRows.Count() > 0)
                            ReportingOrder.AddRange(childRows);
                    }
                }
            }

            return ReportingOrder;
        }

        public static List<AttestationSnapshotAction> GetAttestationTaskReviewForReport(long taskId, string currentUser, bool isInitialLoad = false)
        {
            try
            {
                List<AttestationSnapshotAction> output = new List<AttestationSnapshotAction>();
                var task = GetTask(taskId, isInitialLoad, currentUser);

                //merge the 2 objects(interim + snapshot) and construct the view for export, might be a costly operation
                var orgTask = Task.GetById(taskId);
                var campaignDetails = AttestationCampaign.GetByCampaignIdIncludeAttestaion((long)orgTask.AttestationCampaignId);

                {
                    var availableLevels = task.AttestationSnapshotActions.Select(a => a.AttestationSnapshot.Level).Distinct().ToList();
                    var nonActionableSnapshot = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdForReport(taskId, (long)orgTask.AttestationCampaignId, true);
                    task.AttestationSnapshotActions.AddRange(
                        nonActionableSnapshot.AttestationSnapshotActions.Where(a => !availableLevels.Contains(a.AttestationSnapshot.Level))
                        );
                }

                needToOrder = task.AttestationSnapshotActions.OrderBy(a => a.AttestationSnapshot.Level).ThenByDescending(a => a.IsChanged).ToList();
                output = GetList().ToList();

                return output;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting attestation review report", ex);
                throw;
            }
        }

        public static List<PresentationAttestationEntitlementReviewDetail> GetUsersForAttestationTaskReview(long taskId, string currentUser, long level, bool isInitialLoad = true)
        {
            try
            {
                List<PresentationAttestationEntitlementReviewDetail> output = new List<PresentationAttestationEntitlementReviewDetail>();

                if (isInitialLoad || level == 0)
                {
                    var orgTask = Task.GetByIdAttestationTaskIncluded(taskId);

                    //if (attestationType == AttestationType.Role)
                    //    return GetEndpointsForAttestationTaskReview(taskId, currentUser, level, isInitialLoad);

                    long levelToLoad = 1;
                    var task = GetTask(taskId, isInitialLoad, currentUser);
                    if (task != null && task.AttestationSnapshotActions != null && task.AttestationSnapshotActions.Count > 0)
                    {
                        levelToLoad = task.AttestationSnapshotActions.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
                    }
                    var levelSpecificReviews = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level == levelToLoad && a.IsVisible).ToList();

                    if (!levelSpecificReviews.Any())
                    {
                        var campaignId = Task.GetById(taskId).AttestationCampaignId;
                        var tmpTaskSnapshotAction = AttestationSnapshotAction.GetByTask((long)campaignId, taskId);
                        if (tmpTaskSnapshotAction != null && tmpTaskSnapshotAction.Count > 0)
                        {
                            levelToLoad = tmpTaskSnapshotAction.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
                        }
                        levelSpecificReviews = tmpTaskSnapshotAction.Where(a => a.AttestationSnapshot.Level == levelToLoad && a.IsVisible).ToList();
                    }

                    var di_users = levelSpecificReviews.GroupBy(a => a.AttestationSnapshot.DIUserName).ToList();

                    foreach (var diUser in di_users)
                    {
                        PresentationAttestationEntitlementReviewDetail reviewDetail = new PresentationAttestationEntitlementReviewDetail();
                        reviewDetail.KeyId = diUser.Select(a => a.AttestationSnapshot.UserId).FirstOrDefault();
                        reviewDetail.Entity = diUser.Select(a => a.AttestationSnapshot.DIUserName).FirstOrDefault();
                        reviewDetail.SnapshotActionId = 0;
                        reviewDetail.Name = diUser.Select(a => a.AttestationSnapshot.DIUserName).FirstOrDefault();
                        reviewDetail.DisplayName = diUser.Select(a => a.AttestationSnapshot.DIUserDisplayName).FirstOrDefault();
                        reviewDetail.Type = "User";
                        reviewDetail.ReviewCategory = "User";
                        reviewDetail.DIUserName = "";
                        reviewDetail.AccountName = "";
                        reviewDetail.IsChanged = diUser.Select(a => a.IsChanged).Max();
                        reviewDetail.IsChangedInPreviousLevel = false;
                        reviewDetail.Comments = String.Empty;
                        reviewDetail.IsSensitiveAccess = diUser.Select(a => a.AttestationSnapshot.HasRisk).Max();
                        reviewDetail.IsSODConflict = reviewDetail.IsSensitiveAccess;
                        reviewDetail.Level = 0;
                        reviewDetail.BindingLevel = 0;
                        reviewDetail.EndpointSystemId = 0;
                        reviewDetail.EndpointType = EndpointType.None;
                        reviewDetail.EndpointSystemType = 0;
                        reviewDetail.IsNextLevelAvailable = true;
                        reviewDetail.IsActionable = false;
                        reviewDetail.IsActionExtendable = false;
                        reviewDetail.Reserved1 = diUser.Select(a => a.AttestationSnapshot.Reserved1).FirstOrDefault();
                        reviewDetail.Reserved2 = diUser.Select(a => a.AttestationSnapshot.Reserved2).FirstOrDefault();
                        reviewDetail.Reserved3 = diUser.Select(a => a.AttestationSnapshot.Reserved3).FirstOrDefault();
                        reviewDetail.Reserved4 = diUser.Select(a => a.AttestationSnapshot.Reserved4).FirstOrDefault();
                        reviewDetail.Reserved5 = diUser.Select(a => a.AttestationSnapshot.Reserved5).FirstOrDefault();
                        reviewDetail.Reserved6 = diUser.Select(a => a.AttestationSnapshot.Reserved6).FirstOrDefault();
                        reviewDetail.Reserved7 = diUser.Select(a => a.AttestationSnapshot.Reserved7).FirstOrDefault();
                        reviewDetail.Reserved8 = diUser.Select(a => a.AttestationSnapshot.Reserved8).FirstOrDefault();
                        reviewDetail.Reserved9 = diUser.Select(a => a.AttestationSnapshot.Reserved9).FirstOrDefault();
                        reviewDetail.Reserved10 = diUser.Select(a => a.AttestationSnapshot.Reserved10).FirstOrDefault();
                        reviewDetail.ActionReserved1 = diUser.Select(a => a.Reserved1).FirstOrDefault();
                        reviewDetail.ActionReserved2 = diUser.Select(a => a.Reserved2).FirstOrDefault();
                        reviewDetail.ActionReserved3 = diUser.Select(a => a.Reserved3).FirstOrDefault();
                        output.Add(reviewDetail);
                    }

                    // bubble up the changed users
                    output = output.OrderByDescending(a=>a.AccountName).ToList();
                }

                return output;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting attestation review for Level 0 in user based view", ex);
                throw;
            }
        }

        public static List<PresentationAttestationEntitlementReviewDetail> GetEndpointsForAttestationTaskReview(long taskId, string currentUser, long level, bool isInitialLoad = true)
        {
            try
            {
                List<PresentationAttestationEntitlementReviewDetail> output = new List<PresentationAttestationEntitlementReviewDetail>();

                var orgTask = Task.GetByIdAttestationTaskIncluded(taskId);
                if (!orgTask.AttestationCampaignId.HasValue) return output;
                var campaign = AttestationCampaign.GetById(orgTask.AttestationCampaignId.Value, true);
                if (campaign != null && (campaign.Attestation == null)) return output;
                if (isInitialLoad || level == 0)
                {                    
                    var attestationType = orgTask.Attestation.AttestationType;
                    
                    var task = GetTask(taskId, isInitialLoad, currentUser);                    
                    var endpoints = task.AttestationSnapshotActions.GroupBy(a => a.AttestationSnapshot.EndpointId).ToList();
                    if (campaign.Attestation.AttestationType == AttestationType.User || endpoints.Count > 1)
                    {
                        long levelToLoad = 1;
                        if (task.AttestationSnapshotActions != null && task.AttestationSnapshotActions.Count > 0)
                        {
                            levelToLoad = task.AttestationSnapshotActions.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
                        }
                        var levelSpecificReviews = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level == levelToLoad && a.IsVisible).ToList();

                        if (!levelSpecificReviews.Any())
                        {
                            var campaignId = Task.GetById(taskId).AttestationCampaignId;
                            var tmpTaskSnapshotAction = AttestationSnapshotAction.GetByTask((long)campaignId, taskId);
                            if (tmpTaskSnapshotAction != null && tmpTaskSnapshotAction.Count > 0)
                            {
                                levelToLoad = tmpTaskSnapshotAction.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
                            }
                            levelSpecificReviews = tmpTaskSnapshotAction.Where(a => a.AttestationSnapshot.Level == levelToLoad && a.IsVisible).ToList();
                        }

                        foreach (var endpoint in endpoints)
                        {
                            PresentationAttestationEntitlementReviewDetail reviewDetail = new PresentationAttestationEntitlementReviewDetail();
                            reviewDetail.KeyId = endpoint.Key;
                            reviewDetail.Entity = endpoint.Select(a => a.AttestationSnapshot.EndpointName).FirstOrDefault();
                            reviewDetail.SnapshotActionId = 0;
                            reviewDetail.Name = endpoint.Select(a => a.AttestationSnapshot.EndpointName).FirstOrDefault();
                            reviewDetail.DisplayName = endpoint.Select(a => a.AttestationSnapshot.EndpointType.ToString()).FirstOrDefault();
                            reviewDetail.Reserved1 = endpoint.Select(a => a.AttestationSnapshot.Reserved1).FirstOrDefault();
                            reviewDetail.Reserved2 = endpoint.Select(a => a.AttestationSnapshot.Reserved2).FirstOrDefault();
                            reviewDetail.Reserved3 = endpoint.Select(a => a.AttestationSnapshot.Reserved3).FirstOrDefault();
                            reviewDetail.Reserved4 = endpoint.Select(a => a.AttestationSnapshot.Reserved4).FirstOrDefault();
                            reviewDetail.Reserved5 = endpoint.Select(a => a.AttestationSnapshot.Reserved5).FirstOrDefault();
                            reviewDetail.Reserved6 = endpoint.Select(a => a.AttestationSnapshot.Reserved6).FirstOrDefault();
                            reviewDetail.Reserved7 = endpoint.Select(a => a.AttestationSnapshot.Reserved7).FirstOrDefault();
                            reviewDetail.Reserved8 = endpoint.Select(a => a.AttestationSnapshot.Reserved8).FirstOrDefault();
                            reviewDetail.Reserved9 = endpoint.Select(a => a.AttestationSnapshot.Reserved9).FirstOrDefault();
                            reviewDetail.Reserved10 = endpoint.Select(a => a.AttestationSnapshot.Reserved10).FirstOrDefault();
                            reviewDetail.Type = "Endpoint";
                            reviewDetail.ReviewCategory = "Endpoint";
                            reviewDetail.DIUserName = "";
                            reviewDetail.AccountName = "";
                            reviewDetail.IsChanged = false;
                            reviewDetail.IsChangedInPreviousLevel = false;
                            reviewDetail.Comments = String.Empty;
                            reviewDetail.IsSensitiveAccess = false;
                            reviewDetail.IsSODConflict = reviewDetail.IsSensitiveAccess;
                            reviewDetail.Level = 0;
                            reviewDetail.BindingLevel = 0;
                            reviewDetail.EndpointSystemId = endpoint.Key;
                            reviewDetail.EndpointType = endpoint.Select(a => a.AttestationSnapshot.EndpointType).FirstOrDefault();
                            reviewDetail.EndpointSystemType = Convert.ToInt64(endpoint.Select(a => a.AttestationSnapshot.EndpointType).FirstOrDefault());
                            reviewDetail.IsNextLevelAvailable = true;
                            reviewDetail.IsActionable = false;
                            reviewDetail.IsActionExtendable = false;
                            reviewDetail.ActionReserved1 = endpoint.Select(a => a.Reserved1).FirstOrDefault();
                            reviewDetail.ActionReserved2 = endpoint.Select(a => a.Reserved2).FirstOrDefault();
                            reviewDetail.ActionReserved3 = endpoint.Select(a => a.Reserved3).FirstOrDefault();
                            output.Add(reviewDetail);
                        }
                    }
                    else if (endpoints.Count == 1)
                    {
                        long levelToLoad = 1;
                        if (task.AttestationSnapshotActions != null && task.AttestationSnapshotActions.Count > 0)
                        {
                            levelToLoad = task.AttestationSnapshotActions.Select(a => a.AttestationSnapshot.Level).Distinct().Max();
                        }

                        var levelSpecificReviews = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level == levelToLoad && a.IsVisible).ToList();

                        if (!levelSpecificReviews.Any())
                        {
                            var campaignId = Task.GetById(taskId).AttestationCampaignId;
                            var tmpTaskSnapshotAction = AttestationSnapshotAction.GetByTask((long)campaignId, taskId);
                            if (tmpTaskSnapshotAction != null && tmpTaskSnapshotAction.Count > 0)
                            {
                                levelToLoad = tmpTaskSnapshotAction.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
                            }
                            levelSpecificReviews = tmpTaskSnapshotAction.Where(a => a.AttestationSnapshot.Level == levelToLoad && a.IsVisible).ToList();
                        }

                        if (levelSpecificReviews != null && levelSpecificReviews.Count > 0)
                        {
                            levelSpecificReviews = levelSpecificReviews.GroupBy(a => a.AttestationSnapshot.Entity).Select(a => a.First()).ToList();
                        }

                        long minLvlForEndpoint = 0;
                        if (endpoints.First() != null && orgTask.AttestationCampaignId.HasValue)
                            minLvlForEndpoint = endpoints.First().Key == 0 ? 0 : AttestationSnapshot.GetMinLevelInAttestationSnapshotForEndpoint(orgTask.AttestationCampaignId.Value, endpoints.First().Key);
                        foreach (var levelSpecificReview in levelSpecificReviews)
                        {
                            PresentationAttestationEntitlementReviewDetail reviewDetail = new PresentationAttestationEntitlementReviewDetail();
                            reviewDetail.KeyId = levelSpecificReview.Id;
                            reviewDetail.Entity = levelSpecificReview.AttestationSnapshot.Name;
                            reviewDetail.SnapshotActionId = levelSpecificReview.Id;
                            reviewDetail.Name = levelSpecificReview.AttestationSnapshot.Name;
                            reviewDetail.DisplayName = levelSpecificReview.AttestationSnapshot.Description;
                            reviewDetail.Reserved1 = levelSpecificReview.AttestationSnapshot.Reserved1;
                            reviewDetail.Reserved2 = levelSpecificReview.AttestationSnapshot.Reserved2;
                            reviewDetail.Reserved3 = levelSpecificReview.AttestationSnapshot.Reserved3;
                            reviewDetail.Reserved4 = levelSpecificReview.AttestationSnapshot.Reserved4;
                            reviewDetail.Reserved5 = levelSpecificReview.AttestationSnapshot.Reserved5;
                            reviewDetail.Reserved6 = levelSpecificReview.AttestationSnapshot.Reserved6;
                            reviewDetail.Reserved7 = levelSpecificReview.AttestationSnapshot.Reserved7;
                            reviewDetail.Reserved8 = levelSpecificReview.AttestationSnapshot.Reserved8;
                            reviewDetail.Reserved9 = levelSpecificReview.AttestationSnapshot.Reserved9;
                            reviewDetail.Reserved10 = levelSpecificReview.AttestationSnapshot.Reserved10;
                            reviewDetail.Type = levelSpecificReview.AttestationSnapshot.EntitlementType.ToString();
                            reviewDetail.ReviewCategory = levelSpecificReview.AttestationSnapshot.EntitlementType.ToString(); ;
                            reviewDetail.DIUserName = levelSpecificReview.AttestationSnapshot.Name;
                            reviewDetail.AccountName = levelSpecificReview.AttestationSnapshot.Name;
                            reviewDetail.IsChanged = levelSpecificReview.IsChanged;
                            reviewDetail.IsChangedInPreviousLevel = levelSpecificReview.IsChangedInPreviousLevel;
                            reviewDetail.Comments = levelSpecificReview.Comment;
                            reviewDetail.IsSensitiveAccess = levelSpecificReview.AttestationSnapshot.HasRisk;
                            reviewDetail.IsSODConflict = levelSpecificReview.AttestationSnapshot.HasRisk;
                            reviewDetail.Level = levelSpecificReview.AttestationSnapshot.Level;
                            reviewDetail.BindingLevel = 0;
                            reviewDetail.EndpointSystemId = levelSpecificReview.AttestationSnapshot.EndpointId;
                            reviewDetail.EndpointType = levelSpecificReview.AttestationSnapshot.EndpointType;
                            reviewDetail.EndpointSystemType = Convert.ToInt64(levelSpecificReview.AttestationSnapshot.EndpointType);
                            reviewDetail.IsNextLevelAvailable = minLvlForEndpoint == 0 ? false : levelSpecificReview.AttestationSnapshot.Level > minLvlForEndpoint;
                            reviewDetail.IsActionable = false;
                            reviewDetail.IsActionExtendable = false;
                            reviewDetail.ActionReserved1 = levelSpecificReview.Reserved1;
                            reviewDetail.ActionReserved2 = levelSpecificReview.Reserved2;
                            reviewDetail.ActionReserved3 = levelSpecificReview.Reserved3;
                            reviewDetail.Approved = levelSpecificReview.Approve;
                            reviewDetail.Rejected = levelSpecificReview.Revoke;
                            reviewDetail.Comments = levelSpecificReview.Comment;
                            reviewDetail.EffectiveDate = levelSpecificReview.EffectiveDate;
                            output.Add(reviewDetail);
                        }
                    }
                    // bubble up the changed users
                    //output = output.OrderByDescending(a => a.IsChanged).ToList();
                }

                return output;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting attestation review for Level 0 in user based role view", ex);
                throw;
            }
        }

        public static List<PresentationAttestationEntitlementReviewDetail> GetDetailsForAttestationTaskReviewByUser(long taskId, string currentReviewer, long level, long bindingLevel, string entity, long endpointSystemId, string accountName, bool isInitialLoad = false)
        {
            try
            {
                List<PresentationAttestationEntitlementReviewDetail> output = new List<PresentationAttestationEntitlementReviewDetail>();
                List<AttestationSnapshotAction> levelSpecificReviews = new List<AttestationSnapshotAction>();
                long maxLvlForEndpoint = 0;
                long campaignId = 0;
                bool tryToLoadNextLevelIfParentIsNotVisible = false;
                PresentationAttestationSnapshotActionsForTask task = null;

                if (task == null || task.AttestationSnapshotActions.Count == 0)
                    task = GetTask(taskId, level + 1, entity, accountName, false, currentReviewer);
                //need to merge the 2 objects(interim + snapshot) and construct the view, might be a costly operation
                // did the same thing for excel export GetAttestationTaskReviewForExport
                //if we are merging both snapshot and interim remove this flag tryToLoadFromSnapshotIfnotFoundInInterim

                //this is in case, no action in any level
                if (task != null && task.AttestationSnapshotActions.Count == 0)
                {
                    campaignId = Task.GetById(taskId).AttestationCampaignId.Value;
                    task.AttestationSnapshotActions = AttestationSnapshotAction.GetByTask(campaignId, taskId);
                }
                else
                    campaignId = task.AttestationSnapshotActions.First().AttestationSnapshot.AttestationCampaignId;

                if (task != null && task.AttestationSnapshotActions.Count > 0)
                {
                    maxLvlForEndpoint = endpointSystemId == 0 ? 0 : AttestationSnapshot.GetMaxLevelInAttestationSnapshotForEndpoint(campaignId, endpointSystemId);
                    if (level > 0)
                        tryToLoadNextLevelIfParentIsNotVisible = task.AttestationSnapshotActions.Any(a => a.AttestationSnapshot.Level == level + 1 && !a.IsVisible && a.AttestationSnapshot.ParentEntity == entity);
                    //we are looking for child records of level expanded from page
                    if (level == 0)
                    {
                        long levelToLoad = 1;
                        if (task.AttestationSnapshotActions != null && task.AttestationSnapshotActions.Count > 0)
                        {
                            levelToLoad = task.AttestationSnapshotActions.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
                        }
                        levelSpecificReviews = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level == levelToLoad && a.AttestationSnapshot.DIUserName == entity && a.IsVisible).ToList();

                        if (!levelSpecificReviews.Any())
                        {
                            levelSpecificReviews = AttestationSnapshotAction.GetByTask(campaignId, taskId, levelToLoad).Where(a => a.IsVisible && a.AttestationSnapshot.DIUserName == entity).ToList();
                        }
                    }
                    else
                    {
                        levelSpecificReviews = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level == level + 1 && a.AttestationSnapshot.ParentEntity == entity && a.AttestationSnapshot.EndpointId == endpointSystemId && a.IsVisible).ToList();
                    }

                    if (tryToLoadNextLevelIfParentIsNotVisible)
                    {
                        var inVisibleParent = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level == level + 1 && !a.IsVisible && a.AttestationSnapshot.ParentEntity == entity).ToList();

                        if (inVisibleParent != null && inVisibleParent.Count > 0)
                        {
                            foreach (var item in inVisibleParent)
                                levelSpecificReviews.AddRange(task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level == level + 2 && a.AttestationSnapshot.ParentEntity == item.AttestationSnapshot.Entity && a.AttestationSnapshot.EndpointId == endpointSystemId && a.IsVisible).ToList());
                        }
                    }

                    //filter based on the flag isassociatedwithuser from endpointhierarchy
                    var entitlementTypes = levelSpecificReviews.Select(a => a.AttestationSnapshot.EntitlementType).Distinct().ToList();
                    var epHierarchy = EndpointHierarchy.GetEndpointHierarchyByEntitlementType(endpointSystemId, entitlementTypes);
                    List<AttestationSnapshotAction> finalReviews = new List<AttestationSnapshotAction>();
                    if (epHierarchy.Any(a => a.IsAssociatedWithUser))
                    {
                        foreach (var hierarcy in epHierarchy)
                        {
                            if (hierarcy.IsAssociatedWithUser)
                                finalReviews.AddRange(levelSpecificReviews.Where(a => a.AttestationSnapshot.EndpointId == hierarcy.EndpointId && a.AttestationSnapshot.EntitlementType == hierarcy.EntitlementType && a.AttestationSnapshot.AccountName == accountName).ToList());
                        }
                    }
                    else
                        finalReviews = levelSpecificReviews.ToList();

                    foreach (var review in finalReviews)
                    {
                        PresentationAttestationEntitlementReviewDetail reviewDetail = new PresentationAttestationEntitlementReviewDetail();
                        if(review.AttestationSnapshot.EndpointType == EndpointType.SRM || review.AttestationSnapshot.EndpointType == EndpointType.EAM_NOVA)
                        {
                            if (output.Any(i => i.Name == review.AttestationSnapshot.Name && i.Type == review.AttestationSnapshot.EntitlementType.ToString()
                        && i.EndpointSystemId == review.AttestationSnapshot.EndpointId && review.AttestationSnapshot.Reserved1 == i.Reserved1 && i.AccountName == review.AttestationSnapshot.AccountName
                        ))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (output.Any(i => i.Name == review.AttestationSnapshot.Name && i.Type == review.AttestationSnapshot.EntitlementType.ToString()
                        && i.EndpointSystemId == review.AttestationSnapshot.EndpointId && i.AccountName == review.AttestationSnapshot.AccountName
                        ))
                            {
                                continue;
                            }
                        }
                        
                        reviewDetail.KeyId = review.Id;
                        reviewDetail.Entity = review.AttestationSnapshot.Name;
                        reviewDetail.ParentEntity = review.AttestationSnapshot.ParentEntity;
                        reviewDetail.SnapshotActionId = review.Id;
                        reviewDetail.Name = review.AttestationSnapshot.Name;
                        reviewDetail.DisplayName = level != 0 ? review.AttestationSnapshot.Description : review.AttestationSnapshot.EndpointName;
                        reviewDetail.Type = review.AttestationSnapshot.EntitlementType.ToString();
                        reviewDetail.AccountType = review.AttestationSnapshot.AccountType;
                        reviewDetail.ReviewCategory = review.AttestationSnapshot.EntitlementType.ToString();
                        reviewDetail.DIUserName = review.AttestationSnapshot.DIUserName;
                        reviewDetail.AccountName = review.AttestationSnapshot.AccountName;
                        reviewDetail.EndpointSystemId = review.AttestationSnapshot.EndpointId;
                        reviewDetail.EndpointSystemName = review.AttestationSnapshot.EndpointName;
                        reviewDetail.Reserved1 = review.AttestationSnapshot.Reserved1;
                        reviewDetail.Reserved2 = review.AttestationSnapshot.Reserved2;
                        reviewDetail.Reserved3 = review.AttestationSnapshot.Reserved3;
                        reviewDetail.Reserved4 = review.AttestationSnapshot.Reserved4;
                        reviewDetail.Reserved5 = review.AttestationSnapshot.Reserved5;
                        reviewDetail.Reserved6 = review.AttestationSnapshot.Reserved6;
                        reviewDetail.Reserved7 = review.AttestationSnapshot.Reserved7;
                        reviewDetail.Reserved8 = review.AttestationSnapshot.Reserved8;
                        reviewDetail.Reserved9 = review.AttestationSnapshot.Reserved9;
                        reviewDetail.Reserved10 = review.AttestationSnapshot.Reserved10;
                        reviewDetail.IsChanged = review.IsChanged;
                        reviewDetail.IsChangedInPreviousLevel = review.IsChangedInPreviousLevel;
                        reviewDetail.Comments = review.Comment;
                        reviewDetail.IsSensitiveAccess = review.AttestationSnapshot.HasRisk;
                        reviewDetail.IsSODConflict = review.AttestationSnapshot.HasRisk;
                        reviewDetail.Level = review.AttestationSnapshot.Level;
                        reviewDetail.BindingLevel = bindingLevel + 1;
                        reviewDetail.ParentBindingLevel = bindingLevel;
                        reviewDetail.Approved = review.Approve;
                        reviewDetail.Rejected = review.Revoke;
                        reviewDetail.EffectiveDate = review.EffectiveDate;
                        reviewDetail.IsChanged = review.IsChanged;
                        reviewDetail.IsDelta = review.IsDelta;
                        reviewDetail.IsNextLevelAvailable = maxLvlForEndpoint == 0 ? true : reviewDetail.Level < maxLvlForEndpoint;
                        reviewDetail.IsActionable = review.IsActionable;
                        reviewDetail.IsActionExtendable = review.IsActionExtendable;
                        reviewDetail.EndpointType = review.AttestationSnapshot.EndpointType;
                        reviewDetail.EndpointSystemType = (int)review.AttestationSnapshot.EndpointType;
                        if (review.AttestationSnapshot.LastLogon.HasValue)
                            reviewDetail.LastLogon = review.AttestationSnapshot.LastLogon.ToString();
                        reviewDetail.ActionReserved1 = review.Reserved1;
                        reviewDetail.ActionReserved2 = review.Reserved2;
                        reviewDetail.ActionReserved3 = review.Reserved3;
                        reviewDetail.AccountStatus = review.AttestationSnapshot.AccountStatus;
                        output.Add(reviewDetail);
                    }
                }
                if(output.FirstOrDefault(i=> i.EndpointType == EndpointType.SRM || i.EndpointType == EndpointType.EAM_NOVA) != null && level == 2)
                {
                    output = output.OrderBy(i=> i.Reserved1).ThenBy(a => a.AccountType).ThenBy(a => a.Type).ThenBy(a => a.DisplayName).ThenBy(a => a.Name).ToList();
                }
                else
                {
                    output = output.OrderBy(a => a.AccountType).ThenBy(a => a.Type).ThenBy(a => a.Name).ToList();
                }
                

                return output;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting attestation review for Level 1 in user based view", ex);
                throw;
            }
        }

        public static List<PresentationAttestationEntitlementReviewDetail> GetDetailsForAttestationTaskReviewByRole(long taskId, long keyId, string currentUser, long level, long bindingLevel, string entity, long endpointSystemId, long displayHierarchyStartLevel, bool isInitialLoad = false)
        {
            try
            {
                List<PresentationAttestationEntitlementReviewDetail> output = new List<PresentationAttestationEntitlementReviewDetail>();
                List<AttestationSnapshotAction> levelSpecificReviews = new List<AttestationSnapshotAction>();
                long minLvlForEndpoint = 1;
                var task = GetTask(taskId, isInitialLoad, currentUser);
                var orgTask = Task.GetByIdAttestationTaskIncluded(taskId);
                //need to merge the 2 objects(interim + snapshot) and construct the view, might be a costly operation
                // did the same thing for excel export GetAttestationTaskReviewForExport

                //this is in case, no action in any level
                if (task != null && task.AttestationSnapshotActions.Count == 0)
                {
                    var campaignId = Task.GetById(taskId).AttestationCampaignId;
                    task.AttestationSnapshotActions = AttestationSnapshotAction.GetByTask((long)campaignId, taskId);
                }

                if (task != null && task.AttestationSnapshotActions.Count > 0)
                {
                    var campaignId = task.AttestationSnapshotActions.First().AttestationSnapshot.AttestationCampaignId;

                    var attestationHierarchyAssoicatedWithUser = AttestationHierarchy.GetAttestationHierarchy(orgTask.AttestationId.Value, endpointSystemId).Where(a => a.EndpointHierarchy.IsAssociatedWithUser).Select(a => a.EndpointHierarchy.Level);
                    long attestationMaxHierarchyAssoicatedWithUser = 0;
                    if (attestationHierarchyAssoicatedWithUser.Any())
                        attestationMaxHierarchyAssoicatedWithUser = attestationHierarchyAssoicatedWithUser.Max();
                    displayHierarchyStartLevel = Math.Max(displayHierarchyStartLevel, attestationMaxHierarchyAssoicatedWithUser);

                    if (level == 0)
                    {
                        var grpLevelSpecificReviews = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.Level == displayHierarchyStartLevel && a.IsVisible && a.AttestationSnapshot.EndpointId == endpointSystemId)
                            .GroupBy(a => new { a.AttestationSnapshot.Entity, a.AttestationSnapshot.EntitlementType }).ToList();

                        if (!grpLevelSpecificReviews.Any())
                        {
                            grpLevelSpecificReviews = AttestationSnapshotAction.GetByTask(campaignId, taskId, displayHierarchyStartLevel).Where(a => a.IsVisible && a.AttestationSnapshot.EndpointId == endpointSystemId)
                            .GroupBy(a => new { a.AttestationSnapshot.Entity, a.AttestationSnapshot.EntitlementType }).ToList();
                        }

                        foreach (var grpLevelSpecificReview in grpLevelSpecificReviews)
                        {
                            levelSpecificReviews.Add(new AttestationSnapshotAction
                            {
                                Id = -1,
                                AttestationSnapshotId = -1,
                                TaskId = taskId,
                                Approve = false,
                                Revoke = false,
                                Comment = "",
                                ReviewerId = -1,
                                Expired = false,
                                IsChanged = false,
                                IsChangedInPreviousLevel = false,
                                IsDelta = false,
                                IsVisible = true,
                                IsActionable = false,
                                IsActionExtendable = false,
                                Reserved1 = "",
                                Reserved2 = "",
                                Reserved3 = "",
                                CreatedBy = "",
                                CreatedDate = DateTime.Now,
                                LastModifiedBy = "",
                                LastModifiedDate = DateTime.Now,
                                AttestationSnapshot = grpLevelSpecificReview.Select(a => a.AttestationSnapshot).FirstOrDefault(),
                            });
                        }
                    }
                    else
                    {
                        var epBasedSnapshotActions = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.EndpointId == endpointSystemId).ToList();
                        //This means, we have more entitlement types(levels) associated with user
                        //In this case we will show only the last entitlement/level and its users.
                        //assumption : level 1 - Account Recon table, level 2 - Entitlement Recon table
                        if (displayHierarchyStartLevel > 2)
                        {
                            levelSpecificReviews = (from parentActions in epBasedSnapshotActions
                                                    join childLevelActions in epBasedSnapshotActions
                                                    on parentActions.AttestationSnapshot.AccountName equals childLevelActions.AttestationSnapshot.AccountName
                                                    where childLevelActions.AttestationSnapshot.Entity == entity &&
                                                    childLevelActions.AttestationSnapshot.Level == displayHierarchyStartLevel && parentActions.AttestationSnapshot.Level == 1 &&
                                                    parentActions.IsVisible && childLevelActions.IsVisible
                                                    select new AttestationSnapshotAction
                                                    {
                                                        Id = childLevelActions.Id,
                                                        AttestationSnapshotId = childLevelActions.AttestationSnapshotId,
                                                        TaskId = (long)task.TaskID,
                                                        Approve = childLevelActions.Approve,
                                                        Revoke = childLevelActions.Revoke,
                                                        Comment = childLevelActions.Comment,
                                                        ReviewerId = childLevelActions.ReviewerId,
                                                        Expired = false,
                                                        IsChanged = childLevelActions.IsChanged,
                                                        IsChangedInPreviousLevel = childLevelActions.IsChangedInPreviousLevel,
                                                        IsDelta = childLevelActions.IsDelta,
                                                        IsVisible = childLevelActions.IsVisible,
                                                        IsActionable = childLevelActions.IsActionable,
                                                        IsActionExtendable = childLevelActions.IsActionExtendable,
                                                        Reserved1 = childLevelActions.Reserved1,
                                                        Reserved2 = childLevelActions.Reserved2,
                                                        Reserved3 = childLevelActions.Reserved3,
                                                        CreatedBy = childLevelActions.CreatedBy,
                                                        CreatedDate = childLevelActions.CreatedDate,
                                                        LastModifiedBy = childLevelActions.LastModifiedBy,
                                                        LastModifiedDate = childLevelActions.LastModifiedDate,
                                                        AttestationSnapshot = parentActions.AttestationSnapshot,
                                                    }).ToList();
                        }
                        else
                        {
                            levelSpecificReviews = (from parentActions in epBasedSnapshotActions
                                                    join childLevelActions in epBasedSnapshotActions
                                                    on parentActions.AttestationSnapshot.Entity equals childLevelActions.AttestationSnapshot.ParentEntity
                                                    where childLevelActions.AttestationSnapshot.Entity == entity && childLevelActions.IsVisible
                                                    && childLevelActions.AttestationSnapshot.AccountName == parentActions.AttestationSnapshot.AccountName
                                                    select new AttestationSnapshotAction
                                                    {
                                                        Id = childLevelActions.Id,
                                                        AttestationSnapshotId = childLevelActions.AttestationSnapshotId,
                                                        TaskId = (long)task.TaskID,
                                                        Approve = childLevelActions.Approve,
                                                        Revoke = childLevelActions.Revoke,
                                                        Comment = childLevelActions.Comment,
                                                        ReviewerId = childLevelActions.ReviewerId,
                                                        Expired = false,
                                                        IsChanged = childLevelActions.IsChanged,
                                                        IsChangedInPreviousLevel = childLevelActions.IsChangedInPreviousLevel,
                                                        IsDelta = childLevelActions.IsDelta,
                                                        IsVisible = childLevelActions.IsVisible,
                                                        IsActionable = childLevelActions.IsActionable,
                                                        IsActionExtendable = childLevelActions.IsActionExtendable,
                                                        Reserved1 = childLevelActions.Reserved1,
                                                        Reserved2 = childLevelActions.Reserved2,
                                                        Reserved3 = childLevelActions.Reserved3,
                                                        CreatedBy = childLevelActions.CreatedBy,
                                                        CreatedDate = childLevelActions.CreatedDate,
                                                        LastModifiedBy = childLevelActions.LastModifiedBy,
                                                        LastModifiedDate = childLevelActions.LastModifiedDate,
                                                        AttestationSnapshot = parentActions.AttestationSnapshot,
                                                    }).ToList();
                        }
                    }

                    foreach (var review in levelSpecificReviews)
                    {
                        PresentationAttestationEntitlementReviewDetail reviewDetail = new PresentationAttestationEntitlementReviewDetail();
                        reviewDetail.KeyId = review.Id;
                        reviewDetail.Entity = review.AttestationSnapshot.Name;
                        reviewDetail.ParentEntity = review.AttestationSnapshot.ParentEntity;
                        reviewDetail.SnapshotActionId = review.Id;
                        reviewDetail.Name = review.AttestationSnapshot.Name;
                        //reviewDetail.DisplayName = review.AttestationSnapshot.Description;
                        reviewDetail.DisplayName = review.AttestationSnapshot.EntitlementType == EntitlementType.Account ? review.AttestationSnapshot.DIUserDisplayName : review.AttestationSnapshot.Description;
                        reviewDetail.Type = review.AttestationSnapshot.EntitlementType.ToString();
                        reviewDetail.AccountType = review.AttestationSnapshot.AccountType;
                        reviewDetail.ReviewCategory = review.AttestationSnapshot.EntitlementType.ToString();
                        reviewDetail.DIUserName = review.AttestationSnapshot.DIUserName;
                        reviewDetail.AccountName = review.AttestationSnapshot.AccountName;
                        reviewDetail.EndpointSystemId = review.AttestationSnapshot.EndpointId;
                        reviewDetail.EndpointSystemName = review.AttestationSnapshot.EndpointName;
                        reviewDetail.Reserved1 = review.AttestationSnapshot.Reserved1;
                        reviewDetail.Reserved2 = review.AttestationSnapshot.Reserved2;
                        reviewDetail.Reserved3 = review.AttestationSnapshot.Reserved3;
                        reviewDetail.Reserved4 = review.AttestationSnapshot.Reserved4;
                        reviewDetail.Reserved5 = review.AttestationSnapshot.Reserved5;
                        reviewDetail.Reserved6 = review.AttestationSnapshot.Reserved6;
                        reviewDetail.Reserved7 = review.AttestationSnapshot.Reserved7;
                        reviewDetail.Reserved8 = review.AttestationSnapshot.Reserved8;
                        reviewDetail.Reserved9 = review.AttestationSnapshot.Reserved9;
                        reviewDetail.Reserved10 = review.AttestationSnapshot.Reserved10;
                        reviewDetail.IsChanged = review.IsChanged;
                        reviewDetail.IsChangedInPreviousLevel = review.IsChangedInPreviousLevel;
                        reviewDetail.Comments = review.Comment;
                        reviewDetail.IsSensitiveAccess = review.AttestationSnapshot.HasRisk;
                        reviewDetail.IsSODConflict = review.AttestationSnapshot.HasRisk;
                        reviewDetail.Level = review.AttestationSnapshot.Level;
                        reviewDetail.BindingLevel = bindingLevel + 1;
                        reviewDetail.ParentBindingLevel = bindingLevel;
                        reviewDetail.Approved = review.Approve;
                        reviewDetail.Rejected = review.Revoke;
                        reviewDetail.EffectiveDate = review.EffectiveDate;
                        reviewDetail.IsChanged = review.IsChanged;
                        reviewDetail.IsDelta = review.IsDelta;
                        reviewDetail.IsNextLevelAvailable = minLvlForEndpoint == 0 ? true : reviewDetail.Level > minLvlForEndpoint;
                        reviewDetail.IsActionable = review.IsActionable;
                        reviewDetail.IsActionExtendable = review.IsActionExtendable;
                        reviewDetail.ActionReserved1 = review.Reserved1;
                        reviewDetail.ActionReserved2 = review.Reserved2;
                        reviewDetail.ActionReserved3 = review.Reserved3;
                        if (review.AttestationSnapshot.LastLogon.HasValue)
                            reviewDetail.LastLogon = review.AttestationSnapshot.LastLogon.ToString();
                        reviewDetail.EndpointSystemType = (int)review.AttestationSnapshot.EndpointType;
                        reviewDetail.EndpointType = review.AttestationSnapshot.EndpointType;
                        output.Add(reviewDetail);
                    }
                }
                output = output.OrderBy(a => a.AccountType).ThenBy(a => a.Type).ThenBy(a => a.Name).ToList();

                return output;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting attestation review for Level 1 in user based role view", ex);
                throw;
            }
        }

        public static AttestationSnapshotAction ORMap(EntityModel.AttestationSnapshotAction attestationSnapshotAction)
        {
            if (attestationSnapshotAction == null)
                return null;

            return Mapper.Map<EntityModel.AttestationSnapshotAction, AttestationSnapshotAction>(attestationSnapshotAction);
        }

        public static List<AttestationSnapshotAction> ORMapList(IList<EntityModel.AttestationSnapshotAction> attestationSnapshotAction)
        {
            return attestationSnapshotAction.Select(ORMap).ToList();
        }

        public static bool UpdateAttestationReviewAll(bool value, string accessType, long taskId, string currentUser, long taskActionUserId)
        {
            try
            {
                var orgTask = Task.GetById(taskId);
                var attestationTask = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                if (orgTask != null && orgTask.AttestationCampaignId.HasValue)
                {
                    var task = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskId(taskId, orgTask.AttestationCampaignId.Value);

                    if (task != null && task.AttestationSnapshotActions.Count > 0)
                    {
                        bool approve; bool revoke;
                        if (accessType == "Revoke")
                        {
                            revoke = value;
                            approve = !value;
                        }
                        else
                        {
                            approve = value;
                            revoke = !value;
                        }

                        //set the action detail for the given snapshotid       
                        List<long> actionIdsToUpdate = new List<long>();

                        task.AttestationSnapshotActions.ForEach(a => { actionIdsToUpdate.Add(a.Id); });

                        //List<long> parentActionIdsToUpdate = new List<long>();
                        //var snapshotAction = task.AttestationSnapshotActions.Where(a => a.Id == snapshotActionId).FirstOrDefault();

                        //if (snapshotAction != null)
                        //{
                        //    actionIdsToUpdate.Add(snapshotAction.Id);
                        //}

                        ////set action value to all its child entry in snapshot
                        //if (snapshotAction != null && snapshotAction.AttestationSnapshot != null)
                        //{
                        //    var endpointSystemId = snapshotAction.AttestationSnapshot.EndpointId;
                        //    var maxLevel = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.EndpointId == endpointSystemId).Select(a => a.AttestationSnapshot.Level).Max();
                        //    getChildSnapshotActionIds(snapshotAction, task, maxLevel, taskId, taskActionUserId, currentUser, actionIdsToUpdate);

                        //    //update ischanged value for all the parent
                        //    if (!string.IsNullOrEmpty(snapshotAction.AttestationSnapshot.ParentEntity))
                        //    {
                        //        getParentSnapshotActionIds(snapshotAction, task, parentActionIdsToUpdate);
                        //    }
                        //}

                        if (attestationTask != null)
                        {
                            attestationTask.UnSavedChangesAvailable = true;
                            attestationTask.Save(currentUser);
                        }
                        else
                        {
                            attestationTask = new AttestationSnapshotTaskStatus();
                            attestationTask.AttestationId = orgTask.AttestationId.HasValue ? orgTask.AttestationId.Value : 0;
                            attestationTask.CampaignId = orgTask.AttestationCampaignId.Value;
                            attestationTask.TaskId = taskId;
                            attestationTask.UnSavedChangesAvailable = true;
                            attestationTask.Save(currentUser);
                        }

                        //store the changes into interim
                        List<Tuple<string, object[]>> updateDBScripts = new List<Tuple<string, object[]>>();
                        //actionIdsToUpdate.ForEach(a => {
                        //    updateDBScripts.Add(
                        //    string.Format(@"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedApprove = {0}, UnSavedRevoke = {1}, UnSavedIsChanged = {2}, TaskId = {3}, ReviewerId = {4}, LastModifiedBy = '{5}' WHERE AttestationSnapshotActionId = {6}",
                        //    approve ? 1 : 0, revoke ? 1 : 0, 1, taskId, taskActionUserId, currentUser, a)); });                        

                        //if(snapshotAction != null && snapshotAction.AttestationSnapshot.Level > 1 && approve)
                        //{
                        //    parentActionIdsToUpdate.ForEach(a => {
                        //        updateDBScripts.Add(
                        //        string.Format(@"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedApprove = 1, UnSavedRevoke = 0 ,UnSavedIsChanged = {0}, TaskId = {1}, ReviewerId = {2}, LastModifiedBy = '{3}' WHERE AttestationSnapshotActionId = {4}",
                        //        1, taskId, taskActionUserId, currentUser, a));
                        //    });
                        //}
                        //else
                        //{
                        //    parentActionIdsToUpdate.ForEach(a => {
                        //        updateDBScripts.Add(
                        //        string.Format(@"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedIsChanged = {0}, TaskId = {1}, ReviewerId = {2}, LastModifiedBy = '{3}' WHERE AttestationSnapshotActionId = {4}",
                        //        1, taskId, taskActionUserId, currentUser, a));
                        //    });
                        //}


                        updateDBScripts.Add(Tuple.Create<string, object[]>(
                        @"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedApprove = {0}, UnSavedRevoke = {1}, UnSavedIsChanged = {2}, TaskId = {3}, ReviewerId = {4}, LastModifiedBy = {5} WHERE AttestationSnapshotActionId in (" + string.Join(",", actionIdsToUpdate.Select(a => a)) + ")",
                        new object[] { approve ? 1 : 0, revoke ? 1 : 0, 1, taskId, taskActionUserId, currentUser }));


                        AttestationSnapshotAction.ExecuteQuery(updateDBScripts);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while updating attestation review in snapshot", ex);
                return false;
            }
            return true;
        }
        public static string UpdateAllAttestationReviewEffectiveDate(string entity, DateTime? effectiveDate, string category, long taskId, string currentUser, long taskActionUserId)
        {
            string errorRemarkes = "Error while updating attestation Effective Date";
            try
            {
                var orgTask = Task.GetById(taskId);
                var attestationTask = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                if (orgTask != null && orgTask.AttestationCampaignId.HasValue)
                {
                    var task = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdByActionId(taskId, orgTask.AttestationCampaignId.Value, entity);

                    if (task != null && task.AttestationSnapshotActions != null && task.AttestationSnapshotActions.Count > 0)
                    {
                        foreach (var action in task.AttestationSnapshotActions)
                        {
                            UpdateAttestationReviewEffectiveDate(action.Id, effectiveDate, category, taskId, currentUser, taskActionUserId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, errorRemarkes, ex);
                return errorRemarkes;
            }


            return "";
        }
        public static string UpdateAttestationReviewEffectiveDate(long snapshotActionId, DateTime? effectiveDate, string category, long taskId, string currentUser, long taskActionUserId)
        {
            string errorRemarkes = "Error while updating attestation Effective Date";
            try
            {
                var orgTask = Task.GetById(taskId);
                var attestationTask = AttestationSnapshotTaskStatus.GetByTaskId(taskId);
                if (orgTask != null && orgTask.AttestationCampaignId.HasValue)
                {
                    var task = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdByActionId(taskId, orgTask.AttestationCampaignId.Value, snapshotActionId);

                    if (task != null && task.AttestationSnapshotActions.Count > 0)
                    {
                        //set the action detail for the given snapshotid
                        var snapshotAction = task.AttestationSnapshotActions.Where(a => a.Id == snapshotActionId).FirstOrDefault();
                        if (snapshotAction != null)
                        {
                            //set the action detail for the given snapshotid       
                            List<long> actionIdsToUpdate = new List<long>();
                            List<long> parentActionIdsToUpdate = new List<long>();

                            if (snapshotAction != null)
                            {
                                actionIdsToUpdate.Add(snapshotAction.Id);
                            }
                            List<AttestationSnapshotAction> parentActions = null;
                            //set action value to all its child entry in snapshot
                            if (snapshotAction != null && snapshotAction.AttestationSnapshot != null)
                            {
                                var endpointSystemId = snapshotAction.AttestationSnapshot.EndpointId;
                                var maxLevel = task.AttestationSnapshotActions.Where(a => a.AttestationSnapshot.EndpointId == endpointSystemId).Select(a => a.AttestationSnapshot.Level).Max();
                                getChildSnapshotActionIds(snapshotAction, task, maxLevel, taskId, taskActionUserId, currentUser, actionIdsToUpdate);

                                //update ischanged value for all the parent
                                if (!string.IsNullOrEmpty(snapshotAction.AttestationSnapshot.ParentEntity))
                                {
                                    parentActions = GetParentSnapshotActionIds(snapshotAction, task, parentActionIdsToUpdate);
                                }
                            }

                            if (parentActions != null && parentActions.Any(a => a.EffectiveDate < effectiveDate))
                            {
                                errorRemarkes = "Selected Effective Date is Greated than the parent Effective Date.";
                                throw new Exception();
                            }

                            if (attestationTask != null)
                            {
                                attestationTask.UnSavedChangesAvailable = true;
                                attestationTask.Save(currentUser);
                            }
                            else
                            {
                                attestationTask = new AttestationSnapshotTaskStatus();
                                attestationTask.AttestationId = orgTask.AttestationId.HasValue ? orgTask.AttestationId.Value : 0;
                                attestationTask.CampaignId = orgTask.AttestationCampaignId.Value;
                                attestationTask.TaskId = taskId;
                                attestationTask.UnSavedChangesAvailable = true;
                                attestationTask.Save(currentUser);
                            }

                            //store the changes into interim
                            // effectiveDate/currentUser are bound as real SqlParameters ("{n}" are
                            // EF tokens, not string.Format substitutions) instead of being
                            // concatenated into the SQL text.
                            List<Tuple<string, object[]>> updateDBScripts = new List<Tuple<string, object[]>>();

                            updateDBScripts.Add(Tuple.Create<string, object[]>(
                            @"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedEffectiveDate = {0}, UnSavedIsChanged = {1}, TaskId = {2}, ReviewerId = {3}, LastModifiedBy = {4} WHERE AttestationSnapshotActionId in (" + string.Join(",", actionIdsToUpdate.Select(a => a)) + ")",
                            new object[] { effectiveDate.Value.ToString("dd MMM yyyy"), 1, taskId, taskActionUserId, currentUser }));
                            if(parentActionIdsToUpdate.Count > 0)
                            updateDBScripts.Add(Tuple.Create<string, object[]>(
                            @"UPDATE DI_M_AttestationSnapshotActionInterim SET UnSavedIsChanged = {0}, TaskId = {1}, ReviewerId = {2}, LastModifiedBy = {3} WHERE AttestationSnapshotActionId in (" + string.Join(",", parentActionIdsToUpdate.Select(a => a)) + ")",
                            new object[] { 1, taskId, taskActionUserId, currentUser }));

                            AttestationSnapshotAction.ExecuteQuery(updateDBScripts);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, errorRemarkes, ex);
                return errorRemarkes;
            }


            return "";
        }

    }
}
