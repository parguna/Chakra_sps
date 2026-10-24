
using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Logger;
using DeepIdentity.Model;
using DeepIdentity.Model.GridData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace DeepIdentity.Facade
{


    public class TaskFacade
    {
        TaskDA _taskDA;
        InterimStoreDA _interimStoreDA;

        public TaskFacade()
        {
            _taskDA = new TaskDA();
            _interimStoreDA = new InterimStoreDA();
        }

        public void CreateAttestationTask(Task task)
        {
            try
            {
                _taskDA.CreateAttestationTask(task);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateAttestationTask(Task task)
        {
            try
            {
                _taskDA.UpdateAttestationTask(task);
            }
            catch
            {
                throw;
            }
        }

        public Task GetAttestationTask(long taskId, bool deepLoad)
        {
            try
            {
                Task task = _taskDA.GetAttestationTask(taskId, deepLoad);

                //remove 
                if (deepLoad)
                {
                    foreach (TaskReview taskReview in task.TaskReviews)
                    {
                        taskReview.AttestationReview.ReviewEndpointSystems.RemoveAll(item => item.TaskId != task.TaskId);
                    }
                }
                return task;
            }
            catch
            {
                throw;
            }
        }

        public AttestationReview GetAttestationReviewById(long reviewId, bool deepLoad)
        {
            try
            {
                return _taskDA.GetAttestationReviewById(reviewId, deepLoad);
            }
            catch
            {
                throw;
            }
        }

        public List<AttestationReview> GetPendingRemediations()
        {
            try
            {
                return _taskDA.GetPendingRemediations();
            }
            catch
            {
                throw;
            }
        }

        public Task GetAttestationTaskForReview(long taskId, bool isInitialLoad)
        {
            try
            {
                InterimStore interimStore;
                if (isInitialLoad)
                {
                    interimStore = _interimStoreDA.GetFromInterimStoreByTaskId(taskId, true, InterimStoreItemType.Task);
                }
                else
                {
                    interimStore = _interimStoreDA.GetFromInterimStoreByTaskId(taskId, false, InterimStoreItemType.Task);
                }

                if (interimStore != null)
                {
                    return UtilityMethods.Deserialize<Task>(interimStore.XmlData);
                }
                else
                {
                    Task task = this.GetAttestationTask(taskId, true);

                    task.TaskReviews = task.TaskReviews.OrderByDescending(item => item.AttestationReview.IsChanged).ToList();

                    interimStore = new InterimStore()
                    {
                        InterimStoreItemType = InterimStoreItemType.Task,
                        IsActive = true,
                        IsSaved = true,
                        TaskId = taskId,
                        XmlData = UtilityMethods.Serialize<Task>(task)
                    };
                    _interimStoreDA.AddToInterimStore(interimStore);

                    interimStore = new InterimStore()
                    {
                        InterimStoreItemType = InterimStoreItemType.Task,
                        IsActive = true,
                        IsSaved = false,
                        TaskId = taskId,
                        XmlData = UtilityMethods.Serialize<Task>(task)
                    };
                    _interimStoreDA.AddToInterimStore(interimStore);

                    return task;
                }
            }
            catch
            {
                throw;
            }
        }

        public Task GetAttestationTaskCloneById(long taskId)
        {
            try
            {
                return _taskDA.GetAttestationTaskCloneById(taskId);
            }
            catch
            {
                throw;
            }
        }

        public List<Task> GetPendingTasksByWorkflowInstanceId(Guid workflowInstanceId)
        {
            try
            {
                return _taskDA.GetPendingTasksByWorkflowInstanceId(workflowInstanceId);
            }
            catch
            {
                throw;
            }
        }

        public List<Task> GetTasksByWorkflowInstanceId(Guid workflowInstanceId)
        {
            try
            {
                return _taskDA.GetTasksByWorkflowInstanceId(workflowInstanceId);
            }
            catch
            {
                throw;
            }
        }

        public bool IsAnyPendingAttestationTasks(Guid workflowInstanceId)
        {
            try
            {
                return _taskDA.IsAnyPendingAttestationTasks(workflowInstanceId);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateEscalatedFlag(Task task)
        {
            try
            {
                _taskDA.UpdateTask(task);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateAutoCompleteFlag(Task task)
        {
            try
            {
                _taskDA.UpdateTask(task);
            }
            catch
            {
                throw;
            }
        }

        public void CreateAttestationReviewEndpointSystem(ReviewEndpointSystem reviewEndpointSystem)
        {
            try
            {
                _taskDA.CreateAttestationReviewEndpointSystem(reviewEndpointSystem);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateReviewEndpointSystem(ReviewEndpointSystem reviewEP)
        {
            try
            {
                _taskDA.UpdateReviewEndpointSystem(reviewEP);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateAttestationReviewEndpointSystem(AttestationReviewEndpointSystem reviewEP)
        {
            try
            {
                _taskDA.UpdateAttestationReviewEndpointSystem(reviewEP);
            }
            catch
            {
                throw;
            }
        }

        public TaskDelegation GetTaskDelegationByUserId(int userId, RequestType requestType)
        {
            try
            {
                return _taskDA.GetTaskDelegationByUserId(userId, requestType);
            }
            catch
            {
                throw;
            }
        }

        public List<TaskDelegationGridData> GetDelegationsByUserId(int userId, bool isAdmin)
        {
            try
            {
                return _taskDA.GetDelegationsByUserId(userId, isAdmin);
            }
            catch
            {
                throw;
            }
        }

        public List<TaskDelegation> GetDelegationsByActionUserID(int ActionUserID)
        {
            try
            {
                return _taskDA.GetDelegationsByActionUserID(ActionUserID);
            }
            catch
            {
                throw;
            }
        }

        public List<Task> GetTasksByActionUserId(int actionUserId, bool activeAlone)
        {
            try
            {
                return _taskDA.GetTasksByActionUserId(actionUserId, activeAlone);
            }
            catch
            {
                throw;
            }
        }

        public List<TaskReportData> GetAllTasks(int paramAttestationId)
        {
            try
            {
                return _taskDA.GetAllTasks(paramAttestationId);
            }
            catch
            {
                throw;
            }
        }

        public TaskDelegation GetDelegationById(int delegationId)
        {
            try
            {
                return _taskDA.GetDelegationById(delegationId);
            }
            catch
            {
                throw;
            }
        }

        public void CreateTaskDelegation(TaskDelegation taskDelegation)
        {
            try
            {
                //if (taskDelegation.IsAdvance)
                //{
                this.DeActivateAllDelegationsForUser(taskDelegation.ActionUser);
                //}
                //else
                //{
                //    this.DelegateTasks(taskDelegation);
                //}
                _taskDA.CreateTaskDelegation(taskDelegation);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateTaskDelegation(TaskDelegation taskDelegation)
        {
            try
            {
                _taskDA.UpdateTaskDelegation(taskDelegation);
            }
            catch
            {
                throw;
            }
        }

        private void DeActivateAllDelegationsForUser(int userId)
        {
            try
            {
                List<TaskDelegation> delegations = _taskDA.GetAllDelegationsByUserId(userId);
                foreach (TaskDelegation taskDelegation in delegations)
                {
                    taskDelegation.IsActive = false;
                    taskDelegation.LastModifiedBy = "System";
                    taskDelegation.LastModifiedDate = DateTime.Now;
                    _taskDA.UpdateTaskDelegation(taskDelegation);
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<TaskGridData> GetTaskDetailsByActionUserId(int actionUserId)
        {
            try
            {
                return _taskDA.GetTaskDetailsByActionUserId(actionUserId);
            }
            catch
            {
                throw;
            }
        }

        public IList<TaskGridData> GetTaskDetailsByActionUserIdAndTaskStatus(int actionUserId, StatusType taskStatus)
        {
            try
            {
                return _taskDA.GetTaskDetailsByActionUserIdAndTaskStatus(actionUserId, taskStatus);
            }
            catch
            {
                throw;
            }
        }

        public IList<TaskGridData> GetGroupTaskDetailsByActionUserId(int actionUserId)
        {
            try
            {
                return _taskDA.GetGroupTaskDetailsByActionUserId(actionUserId);
            }
            catch
            {
                throw;
            }
        }

        public IList<TaskGridData> GetGroupTaskDetailsByActionUserIdAndTaskStatus(int actionUserId, StatusType taskStatus)
        {
            try
            {
                return _taskDA.GetGroupTaskDetailsByActionUserIdAndTaskStatus(actionUserId, taskStatus);
            }
            catch
            {
                throw;
            }
        }

        public List<AttestationReviewEndpointSystemHistory> GetReviewEndpointSystemHistories(long reviewEndpointId)
        {
            try
            {
                return _taskDA.GetReviewEndpointSystemHistories(reviewEndpointId);
            }
            catch
            {
                throw;
            }
        }

        public List<AttestationReviewEndpointSystemL2DetailHistory> GetReviewEndpointSystemL2DetailHistories(long l2DetailId)
        {
            try
            {
                return _taskDA.GetReviewEndpointSystemL2DetailHistories(l2DetailId);
            }
            catch
            {
                throw;
            }
        }

        public List<AttestationReviewEndpointSystemL3DetailHistory> GetReviewEndpointSystemL3DetailHistories(long l3DetailId)
        {
            try
            {
                return _taskDA.GetReviewEndpointSystemL3DetailHistories(l3DetailId);
            }
            catch
            {
                throw;
            }
        }

        public List<AttestationReviewEndpointSystemHistory> GetReviewEndpointSystemCommentHistories(long reviewEndpointId)
        {
            try
            {
                return _taskDA.GetReviewEndpointSystemCommentHistories(reviewEndpointId);
            }
            catch
            {
                throw;
            }
        }

        public List<AttestationReviewEndpointSystemL2DetailHistory> GetReviewEndpointSystemL2DetailCommentHistories(long l2DetailId)
        {
            try
            {
                return _taskDA.GetReviewEndpointSystemL2DetailCommentHistories(l2DetailId);
            }
            catch
            {
                throw;
            }
        }

        public List<AttestationReviewEndpointSystemL3DetailHistory> GetReviewEndpointSystemL3DetailCommentHistories(long l3DetailId)
        {
            try
            {
                return _taskDA.GetReviewEndpointSystemL3DetailCommentHistories(l3DetailId);
            }
            catch
            {
                throw;
            }
        }

        public List<AccessRequestHistory> GetAccessRequestCommentHistories(int accessRequestId)
        {
            try
            {
                return _taskDA.GetAccessRequestCommentHistories(accessRequestId);
            }
            catch
            {
                throw;
            }
        }

        public List<PreviousApproversGridData> GetPreviousApproversByWorkflowInstanceId(Guid workflowInstanceId)
        {
            try
            {
                return _taskDA.GetPreviousApproversByWorkflowInstanceId(workflowInstanceId);
            }
            catch
            {
                throw;
            }
        }

        public long CreateProvisioningTask(Task task)
        {
            try
            {
                return _taskDA.CreateProvisioningTask(task);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateProvisioningTask(Task task)
        {
            try
            {
                _taskDA.UpdateProvisioningTask(task);
            }
            catch
            {
                throw;
            }
        }


        public Task GetProvisioningTask(long taskId, bool deepLoad)
        {
            try
            {
                return _taskDA.GetProvisioningTask(taskId, deepLoad); ;
            }
            catch
            {
                throw;
            }
        }

        public void UpdateProvisioningAccessRequest(AccessRequest accessRequest)
        {
            try
            {
                _taskDA.UpdateProvisioningAccessRequest(accessRequest);
            }
            catch
            {
                throw;
            }
        }

        public void CreateComplianceTask(Task task)
        {
            try
            {
                _taskDA.CreateComplianceTask(task);
            }
            catch
            {
                throw;
            }
        }

        public IList<AttestationActionGridData> getAllAttestationReviewForAttestationAction(int paramAttestationId)
        {
            try
            {
                return _taskDA.getAllAttestationReviewForAttestationAction(paramAttestationId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void updateAttestationReview(AttestationActionGridData ipAttestationReviewAction)
        {
            try
            {
                _taskDA.updateAttestationReview(ipAttestationReviewAction);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateAttestationReview(AttestationReview attestationReview)
        {
            try
            {
                _taskDA.UpdateAttestationReview(attestationReview);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<TaskGridData> getSubmittedAttestationTaskDetailsByActionUserId(int actionUserId)
        {
            try
            {
                return _taskDA.getSubmittedAttestationTaskDetailsByActionUserId(actionUserId);
            }
            catch
            {
                throw;
            }
        }

        private void DelegateTasks(TaskDelegation delegation)
        {
            var tasks = _taskDA.GetTasksByActionUserId(delegation.ActionUser, true);

            foreach (Task task in tasks)
            {
                task.ActionUserId = delegation.DelegatedTo;
                task.LastModifiedBy = delegation.CreatedBy;
                task.LastModifiedDate = DateTime.Now;
                task.IsDelegated = true;
                task.DelegatedBy = delegation.CreatedBy;
                task.DelegatedDate = DateTime.Now;
                task.DelegatedFrom = delegation.ActionUser;

                _taskDA.UpdateTask(task);
            }
        }

        public void CreateSelectedTasksDelegation(TaskDelegation taskDelegation, IList<long> selectedtaskList)
        {
            try
            {
                this.DelegateSelectedTasks(taskDelegation, selectedtaskList);
                _taskDA.CreateTaskDelegation(taskDelegation);
            }
            catch
            {
                throw;
            }
        }

        private void DelegateSelectedTasks(TaskDelegation delegation, IList<long> selectedtaskList)
        {
            var tasks = _taskDA.GetTasksByActionUserId(delegation.ActionUser, true);

            foreach (Task task in tasks)
            {
                if (selectedtaskList.Contains(task.TaskId))
                {
                    task.ActionUserId = delegation.DelegatedTo;
                    if (task.IsGroupTask && task.AssignedTo != null)
                    {
                        task.AssignedTo = task.ActionUserId;
                    }
                    task.LastModifiedBy = delegation.CreatedBy;
                    task.LastModifiedDate = DateTime.Now;
                    task.IsDelegated = true;
                    task.DelegatedBy = delegation.CreatedBy;
                    task.DelegatedDate = DateTime.Now;
                    task.DelegatedFrom = delegation.ActionUser;
                    task.DelegationComments = delegation.DelegationComments;

                    _taskDA.UpdateTask(task);
                }
            }
        }

        public Task GetPendingTaskByAccessRequestId(long accessRequestId)
        {
            try
            {
                return _taskDA.GetPendingTaskByAccessRequestId(accessRequestId);
            }
            catch
            {
                throw;
            }
        }

        public Task GetTaskByTaskId(long taskId)
        {
            try
            {
                return _taskDA.GetTaskByTaskId(taskId);
            }
            catch
            {
                throw;
            }
        }

        public List<Task> GetActiveTasksByRequestId(long requestId)
        {
            try
            {
                return _taskDA.GetActiveTasksByRequestId(requestId);
            }
            catch
            {
                throw;
            }
        }

        public bool UpdateGroupTaskByTaskId(Task task)
        {
            return _taskDA.UpdateGroupTaskByTaskId(task);
        }

        public bool IsGroupTaskExistsForUser(int userId)
        {
            try
            {
                return _taskDA.IsGroupTaskExistsForUser(userId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public int GetResponseTime(Task task)
        {
            return _taskDA.GetResponseTime(task);
        }

        public IList<TaskGridData> GetTasksByStatus(StatusType taskStatus)
        {
            try
            {
                return _taskDA.GetTasksByStatus(taskStatus);
            }
            catch
            {
                throw;
            }
        }

        public IList<TaskGridData> GetAllTasks()
        {
            try
            {
                return _taskDA.GetAllTasks();
            }
            catch
            {
                throw;
            }
        }

        public int GetApproverByRequestState(long accessRequestId, byte stateId)
        {
            try
            {
                var task = _taskDA.GetApproverByRequestState(accessRequestId, stateId);
                return task != null ? task.ActionUserId.Value : 0;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Error while getting action users by adhoc request id - {0} and workflow state - {1}.", accessRequestId, stateId), ex, ProvisioningEventType.Others, "Provisioning Engine");
                return 0;
            }
        }

        public List<int> GetActionUsersByRequestState(long accessRequestId, byte stateId)
        {
            try
            {
                return _taskDA.GetActionUsersByRequestState(accessRequestId, stateId);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, string.Format("Error while getting action users by adhoc request id - {0} and workflow state - {1}.", accessRequestId, stateId), ex, ProvisioningEventType.Others, "Provisioning Engine");
                return new List<int>();
            }
        }

        public void UpdateTask(Task task)
        {
            try
            {
                _taskDA.UpdateTask(task);
            }
            catch
            {
                throw;
            }
        }

        public void DeleteDelegateTask(int DelegationId)
        {
            try
            {
                _taskDA.DeleteDelegateTask(DelegationId);
            }
            catch
            {
                throw;
            }
        }

        public List<Task> GetTasksByRequestIdAndState(long requestId, byte stateId)
        {
            try
            {
                return _taskDA.GetTasksByRequestIdAndState(requestId, stateId);
            }
            catch
            {
                throw;
            }
        }

    }
}