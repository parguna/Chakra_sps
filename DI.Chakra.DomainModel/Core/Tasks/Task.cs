using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class Task : DomainModelBase<Task, EntityModel.Task>
    {
        public byte RequestTypeId { get; set; }

        public Nullable<long> AttestationId { get; set; }
        public Nullable<long> AttestationCampaignId { get; set; }

        public Nullable<long> AccessRequestId { get; set; }

        public Nullable<long> BulkRequestId { get; set; }
        public Nullable<long> EmployeeEventTriggerId { get; set; }

        public Nullable<long> SubTaskId { get; set; }

        public Nullable<long> ActionUserId { get; set; }

        public Nullable<long> ActionGroupId { get; set; }

        public byte WorkflowStateId { get; set; }

        public long WorkflowConfigurationId { get; set; }

        public Guid WorkflowInstanceId { get; set; }

        public bool IsEscalated { get; set; }

        public bool IsNotResponded { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsActive { get; set; }

        public Nullable<DateTime> ActionDate { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public StatusType Action { get; set; }

        public string Approver { get; set; }

        public string Comments { get; set; }

        public string Remarks { get; set; }

        public bool IsDelegated { get; set; }

        public Nullable<long> DelegatedFrom { get; set; }

        public string DelegatedBy { get; set; }

        public string DelegationComments { get; set; }

        public Nullable<DateTime> DelegatedDate { get; set; }

        public bool IsActionOnlyRequiredTask { get; set; }

        public bool IsGroupTask { get; set; }

        public bool IsLocked { get; set; }

        public Nullable<long> AssignedTo { get; set; }

        public Nullable<long> AssignedBy { get; set; }

        public string GroupName { get; set; }

        public AccessRequest AccessRequest { get; set; }

        public SubTask SubTask { get; set; }

        public User ActionUser { get; set; }

        public User Delegated { get; set; }
        public Attestation Attestation { get; set; }
        public AttestationCampaign AttestationCampaign { get; set; }

        public WorkflowConfiguration WorkFlowConfiguration { get; set; }

        public EmployeeEventTrigger EmployeeEventTrigger { get; set; }

        public BulkRequest BulkRequest { get; set; }
        public static Task GetTaskInfoByTriggerId(long triggerId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(tsk => tsk.EmployeeEventTriggerId == triggerId).OrderBy(p => p.Id).FirstOrDefault());
            }
        }

        public static bool DoesAttestationTasksAlreadyExists(long attestationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Any(tsk => tsk.AttestationId == attestationId);
            }
        }


        public static bool DoesGroupTaskExistsForUser(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<long> userGroups = User.GetUserGroups(userId);
                return new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Any(tsk => userGroups.Contains((Int32)tsk.ActionGroupId.Value) && (tsk.AssignedTo == null || (tsk.AssignedTo != null && tsk.AssignedTo != userId)) && tsk.IsActive && !tsk.IsCompleted && tsk.IsGroupTask);
            }
        }

        public static List<long> GetActionUsersByRequestState(long accessRequestId, byte workflowStateId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(tsk => tsk.IsGroupTask == false && tsk.AccessRequestId == accessRequestId && tsk.WorkflowStateId == workflowStateId).Select(task => task.ActionUserId.Value).ToList();
            }
        }

        public static List<long> GetActionUsersByBulkRequestState(long bulkRequestId, byte workflowStateId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(tsk => tsk.BulkRequestId == bulkRequestId && tsk.WorkflowStateId == workflowStateId).Select(task => task.ActionUserId.Value).ToList();
            }
        }

        public static List<long> GetCurrentTaskByCampaignIdAndStateId(long campaignId, byte workflowStateId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(tsk => tsk.AttestationCampaignId == campaignId && tsk.WorkflowStateId == workflowStateId).Select(task => task.IsGroupTask ? task.AssignedTo.Value : task.ActionUserId.Value).Distinct().ToList();
            }
        }

        public static List<long> GetCompletedTaskByCampaignIdAndStateId(long campaignId, byte workflowStateId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(tsk => tsk.AttestationCampaignId == campaignId && tsk.WorkflowStateId == workflowStateId
                && tsk.IsCompleted && tsk.IsActive).Select(task => (task.IsGroupTask && !task.IsDelegated) ? task.AssignedTo.Value : task.ActionUserId.Value).Distinct().ToList();
            }
        }

        public static List<Task> GetByCreatedDate(DateTime fromDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(tk => tk.AttestationId.HasValue && tk.CreatedDate >= fromDate).ToList());
            }
        }

        public static decimal GetAttestationHealth(int days)
        {
            try
            {
                DateTime fromDate = DateTime.Now.AddDays(-days);
                var tasks = GetByCreatedDate(fromDate);

                if (tasks.Count == 0)
                {
                    return 100;
                }
                else
                {
                    return CalculateAttestationHealth(tasks);
                }
            }
            catch
            {
                throw;
            }
        }

        public static decimal CalculateAttestationHealth(List<Task> tasks)
        {
            int completedTaskCount = tasks.Count(item => item.IsCompleted == true || item.IsActive == false);

            decimal health = Math.Round(((completedTaskCount * (decimal)100) / tasks.Count), 2);

            return health;
        }

        public static List<Task> GetByUserId(long actionUserId, DateTime fromDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (actionUserId == 0)
                {
                    return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(tk => tk.CreatedDate >= fromDate).ToList());
                }
                return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(item => item.ActionUserId == actionUserId && item.CreatedDate >= fromDate).ToList());
            }
        }

        public static List<Task> GetByActionUserId(long actionUserId, bool activeAlone)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (activeAlone)
                {
                    return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(tsk => tsk.ActionUserId == actionUserId && tsk.IsActive && !tsk.IsCompleted).ToList());
                }
                return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(tsk => tsk.ActionUserId == actionUserId).ToList());
            }
        }

        public override long Save(string currentUser)
        {
            bool isNew = this.IsNew;

            using (DataAccessTransaction tranaction = new DataAccessTransaction(currentUser))
            {
                base.Save(tranaction);

                if (!isNew)
                {
                    //AccessRequest dbrequest = ORMap(new RepositoryBase<EntityModel.Task>(tranaction).Get(g => g.Id == this.Id, g => g.AccessRequest).Single()).AccessRequest;
                    if (this.AccessRequest != null)
                    {
                        this.AccessRequest.Save(tranaction);
                    }
                    if (this.BulkRequest != null)
                    {
                        this.BulkRequest.Save(tranaction);
                    }
                }

                tranaction.Commit();
            }
            return 0;
        }

        /// <summary>
        /// Saves the Attestation reviews for the task.
        /// </summary>
        /// <param name="reviews">Attestation reviews (Attestation Snapshot actions)</param>
        public void SaveAttestationTask(string currentUser, List<AttestationSnapshotAction> reviews)
        {
            bool isNew = this.IsNew;

            using (DataAccessTransaction tranaction = new DataAccessTransaction(currentUser))
            {
                base.Save(tranaction);

                if (!isNew && reviews != null && reviews.Count > 0)
                {
                    reviews.ForEach(asa =>
                    {
                        asa.IsChangedInPreviousLevel = true;
                        asa.Save(string.IsNullOrEmpty(asa.LastModifiedBy) ? currentUser : asa.LastModifiedBy);
                    });
                }

                tranaction.Commit();
            }
        }

        /// <summary>
        /// Saves the Attestation reviews for the task.
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="taskId"></param>
        /// <param name="campaignId"></param>
        public void SaveAttestationTask(string currentUser, long taskId, long campaignId, bool setDefaultValue, bool DefaultApprove)
        {
            try
            {
                bool isNew = this.IsNew;

                using (DataAccessTransaction tranaction = new DataAccessTransaction(currentUser))
                {
                    base.Save(tranaction);
                    tranaction.Commit();

                    var task = Task.GetByIdAttestationTaskIncluded(taskId);
                    if (setDefaultValue)
                    {
                        string defaultValueUpdate = @"UPDATE interim 
                    set interim.SavedApprove = {0},
                        interim.SavedRevoke = {1},
                        interim.UnSavedApprove = {0},
                        interim.UnSavedRevoke = {1},
                        interim.SavedIsChanged = 1,
                        interim.UnSavedIsChanged = 1,
                        interim.UnsavedComment = ISNULL(interim.UnsavedComment, '') + ' Auto Approved',
                        interim.SavedComment = ISNULL(interim.SavedComment, '') + ' Auto Approved',
                        interim.SavedEffectiveDate = interim.UnSavedEffectiveDate,
                        interim.SavedReserved1 = interim.UnSavedReserved1,
                        interim.SavedReserved2 = interim.UnSavedReserved2,
                        interim.SavedReserved3 = interim.UnSavedReserved3
                    FROM DI_M_AttestationSnapshotActionInterim interim
                    INNER JOIN DI_M_AttestationSnapshotAction act ON interim.AttestationSnapshotActionId = act.Id
                    WHERE interim.TaskId = {2} ";

                        try
                        {
                            AttestationSnapshotAction.ExecuteQuery(string.Format(defaultValueUpdate, DefaultApprove ? 1 : 0, !DefaultApprove ? 1 : 0, taskId));
                        }
                        catch (Exception ex)
                        {
                            DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, "Error while Save Attestation Task for Auto Approve.", ex);
                        }
                    }

                    string updateQuery = @"UPDATE ssAction 
                                        SET ssAction.[Approve] = interim.SavedApprove,
                                            ssAction.[Revoke] = interim.SavedRevoke,
                                            ssAction.Comment =  ISNULL(ssAction.Comment, '') + (CASE WHEN (interim.SavedComment IS NOT NULL AND ISNULL(interim.SavedComment, '') <> '') THEN (' Level: ' + '{1}' + ' By: ' + ISNULL(interim.LastModifiedBy, {{0}}) + ' On: ' + CONVERT(VARCHAR, ISNULL(interim.LastModifiedDate, GETDATE()), 103) + ' Message: ' +  ISNULL(interim.SavedComment, '') + '\r\n') ELSE '' END),
                                            ssAction.EffectiveDate = interim.SavedEffectiveDate,
	                                        ssAction.IsChangedInPreviousLevel = interim.SavedIsChanged,
                                            ssAction.IsChanged = interim.SavedIsChanged,
                                            ssAction.Reserved1 = interim.SavedReserved1,
                                            ssAction.Reserved2 = interim.SavedReserved2,
                                            ssAction.Reserved3 = interim.SavedReserved3,
	                                        ssAction.LastModifiedBy = interim.LastModifiedBy,
	                                        ssAction.LastModifiedDate = GETDATE()
                                        FROM DI_M_AttestationSnapshotAction ssAction
                                        INNER JOIN DI_M_AttestationSnapshotActionInterim interim ON ssAction.Id = interim.AttestationSnapshotActionId
                                        WHERE interim.TaskId =  {0} ";

                    try
                    {
                        // {0} in the formatted text below is EF's own positional-parameter
                        // token (bound as a real SqlParameter by Database.ExecuteSqlCommand),
                        // not a string.Format substitution - currentUser must never be
                        // concatenated into the SQL text directly.
                        string formattedUpdateQuery = string.Format(updateQuery, taskId, task == null ? 0 : task.WorkflowStateId);
                        AttestationSnapshotAction.ExecuteQuery(formattedUpdateQuery, currentUser);
                    }
                    catch (Exception ex)
                    {
                        DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, "Error while Save Attestation Task for DI_M_AttestationSnapshotAction.", ex);
                    }

                    //update History Table with changed Data
                    string updateHistory = @"UPDATE ssActionHistory 
                                        SET ssActionHistory.TaskId = ssAction.TaskId,
	                                        ssActionHistory.Approve = ssAction.Approve,
                                            ssActionHistory.[Revoke] = ssAction.[Revoke],
                                            ssActionHistory.Comment =  ssActionInterim.SavedComment,
                                            ssActionHistory.EffectiveDate = ssAction.EffectiveDate,
	                                        ssActionHistory.ReviewerId = ssAction.ReviewerId,
	                                        ssActionHistory.LastModifiedBy = ssAction.LastModifiedBy,
	                                        ssActionHistory.LastModifiedDate = GETDATE()
                                        FROM DI_M_AttestationSnapshotAction_History ssActionHistory 
                                        INNER JOIN DI_M_AttestationSnapshotAction ssAction ON ssActionHistory.AttestationSnapshotActionId = ssAction.Id
                                        LEFT OUTER JOIN DI_M_AttestationSnapshotActionInterim ssActionInterim ON ssActionInterim.AttestationSnapshotActionId = ssAction.Id
                                        WHERE ssAction.TaskId = {0} AND ssActionHistory.TaskId = {0} ";

                    try
                    {
                        AttestationSnapshotAction.ExecuteQuery(string.Format(updateHistory, taskId));
                    }
                    catch (Exception ex)
                    {
                        DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception,
                            string.Format("Error while Save Attestation Task for DI_M_AttestationSnapshotAction_History. Query {0}, TaskId {1}", updateHistory, taskId),
                            ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, "Error while Save Attestation Task.", ex);
            }
        }

        public static List<Task> GetPendingTaskByAccessRequestId(long accessRequestId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.AccessRequestId == accessRequestId && tsk.IsActive && tsk.IsCompleted == false).ToList());
            }
        }

        public static Task GetPendingTask(long accessRequestId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.AccessRequestId == accessRequestId && tsk.IsCompleted == true, tsk => tsk.WorkFlowConfiguration.WorkflowConfigurationDetails).OrderByDescending(m => m.LastModifiedDate).FirstOrDefault());
            }
        }

        public static long GetLatestTaskId()
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                var task = new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.IsCompleted == false && tsk.IsActive).FirstOrDefault();
                if (task != null)
                {
                    return task.Id;
                }
                return 0;
            }
        }

        public static Task GetLatestActiveAttestationTask()
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                var task = new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.IsCompleted == false && tsk.IsActive && tsk.AttestationCampaignId != null).OrderByDescending(t => t.CreatedDate).FirstOrDefault();

                return ORMap(task);
            }
        }

        public static Task GetProvisioningTask(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(tranaction)
                        .Get(tsk => tsk.Id == id,
                             tsk => tsk.AccessRequest,
                             tsk => tsk.AccessRequest.User,
                             tsk => tsk.AccessRequest.AdHocAccessRequest,
                             tsk => tsk.AccessRequest.AdHocAccessRequest.EndPoint,
                             tsk => tsk.SubTask).FirstOrDefault());
            }
        }

        public static Task GetBulkRequestTask(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(tranaction)
                        .Get(tsk => tsk.Id == id,
                             tsk => tsk.BulkRequest,
                             tsk => tsk.BulkRequest.BulkRequestDetails,
                             tsk => tsk.BulkRequest.EndPoint,
                             tsk => tsk.SubTask).FirstOrDefault());
            }
        }

        public static Task GetEmployeeEventTask(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(tranaction)
                        .Get(tsk => tsk.Id == id,
                             tsk => tsk.EmployeeEventTrigger,
                              tsk => tsk.EmployeeEventTrigger.User,
                             tsk => tsk.SubTask).FirstOrDefault());
            }
        }

        public static Task GetAttestationTask(long id, bool deepLoad = false)
        {
            return Task.GetById(id, deepLoad);
            //using (DataAccessTransaction transaction = new DataAccessTransaction())
            //{
            //   // if (!deepLoad)
            //   // {
            //   //     return Task.GetById(id);
            //   // }

            //   // var deepLoadedTask = ORMap(new RepositoryBase<EntityModel.Task>(transaction).Get(
            //   //     task => task.Id == id,
            //   //     task => task.TaskReviews,
            //   //     task => task.TaskReviews.Select(x => x.AttestationReview),
            //   //     task => task.TaskReviews.Select(x => x.AttestationReview.User),
            //   //     task => task.TaskReviews.Select(x => x.AttestationReview.User.NOrganisation),
            //   //     task => task.TaskReviews.Select(x => x.AttestationReview.ReviewEndpointSystems),
            //   //     task => task.TaskReviews.Select(x => x.AttestationReview.ReviewEndpointSystems.Select
            //   //             (x1 => x1.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details)),
            //   //     task => task.TaskReviews.Select(x => x.AttestationReview.ReviewEndpointSystems.Select
            //   //             (x1 => x1.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details.Select
            //   //             (x2 => x2.AttestationReviewEndpointSystemL3Details)))
            //   //).FirstOrDefault());

            //   // //remove 
            //   // if (deepLoad)
            //   // {
            //   //     foreach (TaskReview taskReview in deepLoadedTask.TaskReviews)
            //   //     {
            //   //         taskReview.AttestationReview.ReviewEndpointSystems.RemoveAll(item => item.TaskId != deepLoadedTask.Id);
            //   //     }
            //   // }
            //   // return deepLoadedTask;
            //}
        }

        public static Task GetApproverByAccessRequestId(long accessRequestId, byte workflowStateId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.AccessRequestId == accessRequestId && tsk.WorkflowStateId == workflowStateId && tsk.IsActive && tsk.IsCompleted).FirstOrDefault());
            }
        }

        public static Task GetApproverByBulkRequestId(long bulkRequestId, byte workflowStateId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.BulkRequestId == bulkRequestId && tsk.WorkflowStateId == workflowStateId && tsk.IsActive && tsk.IsCompleted).FirstOrDefault());
            }
        }

        public static List<Task> GetApproverByRequestState(long accessRequestId, byte workflowStateId, long workflowConfigId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.AccessRequestId == accessRequestId && tsk.WorkflowStateId == workflowStateId
                 && tsk.WorkflowConfigurationId == workflowConfigId).ToList());
            }
        }
        public static List<Task> GetTaskByApprover(long accessRequestId, byte workflowStateId, long approverId, long workflowConfigId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.AccessRequestId == accessRequestId && tsk.WorkflowStateId == workflowStateId && tsk.ActionUserId == approverId
                && tsk.WorkflowConfigurationId == workflowConfigId).ToList());
            }
        }
        public static Task GetAttestationTaskCloneById(long taskId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.Id == taskId, tsk => tsk.AttestationCampaignId).FirstOrDefault());
            }
        }

        public static Task GetCompletedTaskByWorkflowInstanceIdAndState(Guid workflowInstanceId, byte state)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(tranaction).
                    Get(item => item.WorkflowInstanceId == workflowInstanceId && item.WorkflowStateId == state
                    && item.IsCompleted == true && item.IsActive == true).FirstOrDefault());
            }
        }

        public static List<Task> GetPendingTaskByworkflowInstanceId(Guid workflowInstanceId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.WorkflowInstanceId == workflowInstanceId && tsk.IsActive && tsk.IsCompleted == false).ToList());
            }
        }

        public static List<Task> GekByAccessRequestId(long accessrequestId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => (tsk.AccessRequestId == accessrequestId), true).ToList());
            }
        }

        public static List<Task> GetByWorkflowInstanceId(Guid workflowInstanceId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.WorkflowInstanceId == workflowInstanceId).ToList());
            }
        }

        public static List<Task> GetByWorkflowInstanceIdAndState(Guid workflowInstanceId, byte stateId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Task>(tranaction).Get(tsk => tsk.WorkflowInstanceId == workflowInstanceId && tsk.WorkflowStateId == stateId).ToList());
            }
        }

        public static Task GetByIdProvioningTaskIncluded(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMap(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.Id == id,
                            task => task.AccessRequest,
                            task => task.AccessRequest.AdHocAccessRequest,
                            task => task.AccessRequest.AdHocAccessRequest.EndPoint,
                            task => task.AccessRequest.Application,
                            task => task.AccessRequest.User,
                            task => task.AccessRequest.Requester,
                            task => task.ActionUser,
                            task => task.AccessRequest.User.NBusinessRole,
                            task => task.AccessRequest.Requester.NBusinessRole,
                            task => task.ActionUser.NBusinessRole,
                            task => task.AccessRequest.User.NOrganisation,
                            task => task.AccessRequest.Requester.NOrganisation,
                            task => task.ActionUser.NOrganisation
                            ).
                            FirstOrDefault());
            }
        }

        public static List<Task> GetByCampaignIdAndWorkFlowState(long campaignId, byte workflowstateId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.AttestationCampaignId == campaignId
                            && task.WorkflowStateId == workflowstateId
                            ).
                            ToList());
            }
        }

        public static List<Task> GetByParentCampaignIdAndWorkFlowState(long campaignId, byte workflowstateId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var childCampaign = AttestationCampaign.GetChildCampaigns(campaignId);
                List<long> campaignIds = new List<long>();
                campaignIds.Add(campaignId);
                if (childCampaign != null && childCampaign.Count > 0)
                    campaignIds.AddRange(childCampaign.Select(a => a.Id).ToList());
                return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.AttestationCampaignId.HasValue &&
                            campaignIds.Contains(task.AttestationCampaignId.Value) &&
                            task.WorkflowStateId == workflowstateId
                            ).
                            ToList());
            }
        }

        public static Task GetByIdAttestationTaskIncluded(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMap(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.Id == id,
                            task => task.Attestation,
                            task => task.Attestation.AttestationEndpoint,
                            task => task.AttestationCampaign,
                            task => task.ActionUser
                            ).
                            FirstOrDefault());
            }
        }

        public static Task GetByIdAttestationTaskIncludedForEmail(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMap(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.Id == id,
                            task => task.Attestation,
                            task => task.Attestation.AttestationEndpoint,
                            task => task.AttestationCampaign,
                            task => task.ActionUser,
                            task => task.ActionUser.NOrganisation,
                            task => task.ActionUser.NBusinessRole
                            ).
                            FirstOrDefault());
            }
        }

        public static Task GetTaskByBulkRequestId(long bulkRequestId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                    Get(task =>
                            task.BulkRequestId == bulkRequestId && !task.IsCompleted,
                            task => task.BulkRequest,
                            task => task.BulkRequest.BulkRequestDetails,
                            task => task.AccessRequest,
                            task => task.AccessRequest.AdHocAccessRequest,
                            task => task.BulkRequest.EndPoint,
                            task => task.ActionUser,
                            task => task.SubTask
                            ).OrderBy(p => p.Id)
                            .FirstOrDefault());
            }
        }

        public static List<Task> GetByRequestId(long accessRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.AccessRequestId == accessRequestId,
                            task => task.AccessRequest
                            ).
                            ToList());
            }
        }

        public static List<Task> GetByBulkRequestId(long bulkRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.BulkRequestId == bulkRequestId,
                            task => task.BulkRequest,
                            task => task.BulkRequest.BulkRequestDetails
                            ).
                            ToList());
            }
        }

        public static Task GetByEventTriggerId(long triggerId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMap(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.EmployeeEventTriggerId == triggerId
                            ).FirstOrDefault());
            }
        }
        public static List<Task> GetByEventTriggerIdForWithDraw(long triggerId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                             Get(task =>
                                 task.EmployeeEventTriggerId == triggerId,
                                 task => task.WorkFlowConfiguration
                                 ).
                                 ToList());
            }
        }
        public static List<Task> GetByRequestIdIncludeActionUser(long accessRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.AccessRequestId == accessRequestId,
                            task => task.AccessRequest,
                            task => task.ActionUser,
                            task => task.WorkFlowConfiguration
                            ).
                            ToList());
            }
        }


        //public void SaveAttestationTask()
        //{
        //    bool isNew = this.IsNew;
        //    using (DataAccessTransaction transaction = new DataAccessTransaction())
        //    {
        //        base.Save(transaction);
        //        if (!isNew && this.TaskReviews != null)
        //        {
        //            TaskReview.SaveList(transaction, this.TaskReviews);

        //            AttestationReview.SaveList(transaction, this.TaskReviews.Select(x => x.AttestationReview).ToList());

        //            ReviewEndpointSystem.SaveList(transaction, this.TaskReviews.SelectMany(x => x.AttestationReview.ReviewEndpointSystems).ToList());

        //            ReviewEndpointSystem.SaveList(transaction, this.TaskReviews.SelectMany(x => x.AttestationReview.ReviewEndpointSystems).ToList());

        //            AttestationReviewEndpointSystem.SaveList(transaction,
        //                this.TaskReviews.SelectMany(x => x.AttestationReview.ReviewEndpointSystems.Select
        //                    (x1 => x1.AttestationReviewEndpointSystem)).ToList());

        //            AttestationReviewEndpointSystemL2Detail.SaveList(transaction,
        //                this.TaskReviews.SelectMany(x => x.AttestationReview.ReviewEndpointSystems.SelectMany
        //                    (x1 => x1.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details)).ToList());

        //            AttestationReviewEndpointSystemL3Detail.SaveList(transaction,
        //            this.TaskReviews.SelectMany(x => x.AttestationReview.ReviewEndpointSystems.SelectMany
        //                  (x1 => x1.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details.SelectMany
        //                  (x2 => x2.AttestationReviewEndpointSystemL3Details))).ToList());

        //        }


        //        transaction.Commit();
        //    }
        //}

        //public static bool UpdateGroupTaskByTaskId(Task task, string currentUser)
        //{
        //    using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
        //    {
        //        Task _task = Task.GetById(task.Id);
        //        if (_task != null)
        //        {
        //            List<Task> taskList = new List<Task>();

        //            if (_task.AttestationId.HasValue)
        //                taskList = ORMapList(new RepositoryBase<EntityModel.Task>(transaction).Get(tsk => tsk.AttestationId == task.AttestationId).ToList());
        //            else if (_task.AccessRequestId.HasValue)
        //                taskList = ORMapList(new RepositoryBase<EntityModel.Task>(transaction).Get(tsk => tsk.AccessRequestId == task.AccessRequestId).ToList());

        //            foreach (Task _tsk in taskList)
        //            {
        //                _tsk.AssignedTo = task.AssignedTo;
        //                _tsk.AssignedBy = task.AssignedBy;
        //                _tsk.LastModifiedBy = task.LastModifiedBy;
        //                _tsk.LastModifiedDate = task.LastModifiedDate;
        //                _tsk.Save(transaction);
        //            }
        //        }
        //    }
        //    return true;
        //}

        public static int GetResponseTime(Task task)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                int responseTime = 0;
                if (task.RequestTypeId == (byte)RequestType.Attestation)
                {
                    responseTime = new TaskRepository(transaction).GetResponseTime(task.Id);
                }
                if (task.RequestTypeId == (byte)RequestType.Provisioning)
                {
                    responseTime = new TaskRepository(transaction).GetResponseTimeIncludingEndpoint(task.Id);
                }
                return responseTime;
            }
        }

        public static IList<Task> GetTaskInfo(string attestationName)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(transaction).GetTaskInfo(attestationName));
            }
        }

        public static List<Task> GetExpiredTask()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(transaction).GetExpiredTask());
            }
        }

        public static int PurgeAllTasksData(int numberOfDays)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                DateTime purgeRangeDate = DateTime.Now.AddDays(-numberOfDays);
                List<Task> tasksToBePurgedList = ORMapList(new RepositoryBase<EntityModel.Task>(transaction).Get(tsk => tsk.CreatedDate < purgeRangeDate).ToList());

                Task.DeleteList(tasksToBePurgedList);

                return 0;
            }
        }

        public static TaskCompleteness GetTaskCompleteness(string attestationName)
        {
            TaskCompleteness taskCompletness = new TaskCompleteness();
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                int totalTask = 0;
                int completedTask = 0;
                totalTask = new TaskRepository(transaction).GetByAttestation(attestationName);
                completedTask = new TaskRepository(transaction).GetCompletedTaskByAttestation(attestationName);
                taskCompletness.TotalTask = totalTask.ToString();
                taskCompletness.CompletedTask = completedTask.ToString();
                return taskCompletness;
            }
        }
        public static CampaignTaskDetail GetCampaignTaskDetail(string campaignName)
        {
            CampaignTaskDetail campaignDetails = new CampaignTaskDetail();
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                int totalTask = 0;
                int completedTask = 0;
                var campaignTask = new TaskRepository(transaction).GetByCampaign(campaignName);
                var campaignDetail = AttestationCampaign.GetByCampaignName(campaignName);
                var attestationDetail = Attestation.GetById(campaignDetail.AttestationID);
                var workflow = WorkflowConfigurationDetail.GetByWorkflowConfigurationId(attestationDetail.WorkflowConfigurationId);
                totalTask = campaignTask.Count;
                completedTask = campaignTask.Where(a => a.IsCompleted).Count();
                campaignDetails.TotalTask = totalTask.ToString();
                campaignDetails.CompletedTask = completedTask.ToString();
                var notCompletedTask = campaignTask.Where(a => !a.IsCompleted);
                var completedPreTask = campaignTask.Where(a => a.IsCompleted);
                if (notCompletedTask.Any())
                    campaignDetails.CurrentLevel = notCompletedTask.FirstOrDefault().WorkflowStateId.ToString();
                else if (completedPreTask.Any())
                    campaignDetails.CurrentLevel = completedPreTask.Select(a => a.WorkflowStateId).Max().ToString();

                campaignDetails.TotalLevel = workflow.Select(a => a.SequenceNumber).Max().ToString();
                campaignDetails.StartDate = campaignDetail.CreatedDate;
                if (campaignDetail.CloseDate.HasValue)
                    campaignDetails.EndDate = campaignDetail.CloseDate.Value;
                return campaignDetails;
            }
        }
        public static void CreateTriggers()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new TaskRepository(dataAccessTransaction).CreateTriggers();
            }
        }

        public static string Serialize(Task task)
        {
            return UtilityMethods.Serialize<Task>(task, true);
        }
        public static Task DeSerialize(string xml)
        {
            return UtilityMethods.Deserialize<Task>(xml, true);
        }

        public static bool DoesExistsByWorkflowConfigurationId(long workflowConfigurationId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                    Any(task => task.WorkflowConfigurationId == workflowConfigurationId);
            }
        }


        public static long GetTaskIdByAccessRequestId(long requestId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Get(x => x.AccessRequestId == requestId).FirstOrDefault().Id;
            }
        }

        public static List<Task> GetEmployeeTaskByRequestState(long triggerId, byte stateId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(transaction).GetEmployeeTaskByRequestState(triggerId, stateId));
            }
        }

        public static List<Task> GetAccessRequestTaskByRequestState(long requestId, byte stateId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(transaction).GetAccessRequestTaskByRequestState(requestId, stateId));
            }
        }

        public static List<Task> GetBulkRequestTaskByRequestState(long bulkRequestId, byte stateId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(transaction).GetBulkRequestTaskByRequestState(bulkRequestId, stateId));
            }
        }
        public static Task GetAttestationTask(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(transaction)
                    .Get(tsk => tsk.Id == id,
                            tsk => tsk.ActionUser,
                            tsk => tsk.AttestationCampaign,
                            tsk => tsk.Attestation,
                            tsk => tsk.Delegated
                            ).FirstOrDefault());
            }
        }

        public static bool IsTaskExistsForAccessRequestAttestationRemediation(long accessrequestId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return new TaskRepository(transaction).IsTaskExistsForAccessRequestAttestationRemediation(accessrequestId);
            }
        }
        public static List<Task> GetAllPendingAttestationTask()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetAllPendingAttestationTask());
            }
        }
        public static List<Task> GetAllPendingProvisioningTaskForReminder()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new TaskRepository(dataAccessTransaction).GetAllPendingProvisioningTaskForReminder());
            }
        }
        public static List<Task> GetByCampaignIdAndAllState(long campaignId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMapList(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.AttestationCampaignId == campaignId
                            ).OrderByDescending(i => i.WorkflowStateId).
                            ToList());
            }
        }
        public static Task GetByCampaignIdAndMaxState(long campaignId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMap(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.AttestationCampaignId == campaignId && task.AccessRequestId == null && (task.Action == StatusType.Approved || task.Action == StatusType.AutoCompleted)
                            ).OrderByDescending(i => i.WorkflowStateId).
                            FirstOrDefault());
            }
        }
        public static Task GetByCampaignIdLastLevelApproval(long campaignId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMap(new RepositoryBase<EntityModel.Task>(dataAccessTransaction).
                        Get(task =>
                            task.AttestationCampaignId == campaignId).OrderByDescending(i => i.WorkflowStateId).
                            FirstOrDefault());
            }
        }

        public static Task GetSubTask(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Task>(tranaction)
                        .Get(tsk => tsk.Id == id,
                             tsk => tsk.SubTask).FirstOrDefault());
            }
        }
        public static bool DoesEmploymentEventTasksAlreadyExist(long triggid)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.Task>(dataAccessTransaction).Any(tsk => tsk.EmployeeEventTriggerId == triggid && tsk.RequestTypeId == (byte)RequestType.EmployeeEvent && (tsk.Action == StatusType.Pending || tsk.IsActive));
            }
        }

    }
}