using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using DI.Chakra.Model.GridData;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Model.CustomEntities;

namespace DI.Chakra.DataAccess
{
    public class TaskDA
    {
        public UserDA _userDA;

        public TaskDA()
        {
            _userDA = new UserDA();
        }
        public void CreateAttestationTask(Task task)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Tasks.Add(task);
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var taskEntry = _context.Entry(task);
                    if (taskEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<Task>();
                        Task attachedTask = set.Find(task.Id);

                        if (attachedTask != null)
                        {
                            var attachedEntry = _context.Entry(attachedTask);
                            attachedEntry.CurrentValues.SetValues(task);
                        }
                    }

                    foreach (TaskReview taskReview in task.TaskReviews)
                    {
                        var taskReviewEntry = _context.Entry(taskReview);
                        if (taskReviewEntry.State == EntityState.Detached)
                        {
                            var set = _context.Set<TaskReview>();
                            TaskReview attachedTaskReview = set.Find(taskReview.Id);

                            if (attachedTaskReview != null)
                            {
                                var attachedEntry = _context.Entry(attachedTaskReview);
                                attachedEntry.CurrentValues.SetValues(taskReview);
                            }
                        }

                        var reviewEntry = _context.Entry(taskReview.AttestationReview);
                        if (reviewEntry.State == EntityState.Detached)
                        {
                            var set = _context.Set<AttestationReview>();
                            AttestationReview attachedReview = set.Find(taskReview.AttestationReview.Id);

                            if (attachedReview != null)
                            {
                                var attachedEntry = _context.Entry(attachedReview);
                                attachedEntry.CurrentValues.SetValues(taskReview.AttestationReview);
                            }
                        }

                        foreach (ReviewEndpointSystem reviewEndpointSystem in taskReview.AttestationReview.ReviewEndpointSystems)
                        {
                            var reviewEPEntry = _context.Entry(reviewEndpointSystem);
                            if (reviewEPEntry.State == EntityState.Detached)
                            {
                                var set = _context.Set<ReviewEndpointSystem>();
                                ReviewEndpointSystem attachedreviewEP = set.Find(reviewEndpointSystem.Id);

                                if (attachedreviewEP != null)
                                {
                                    var attachedEntry = _context.Entry(attachedreviewEP);
                                    attachedEntry.CurrentValues.SetValues(reviewEndpointSystem);
                                }
                            }

                            var reviewEPSEntry = _context.Entry(reviewEndpointSystem.AttestationReviewEndpointSystem);
                            if (reviewEPSEntry.State == EntityState.Detached)
                            {
                                var set = _context.Set<AttestationReviewEndpointSystem>();
                                AttestationReviewEndpointSystem attachedreviewEPS = set.Find(reviewEndpointSystem.AttestationReviewEndpointSystem.Id);

                                if (attachedreviewEPS != null)
                                {
                                    var attachedEntry = _context.Entry(attachedreviewEPS);
                                    attachedEntry.CurrentValues.SetValues(reviewEndpointSystem.AttestationReviewEndpointSystem);
                                }
                            }

                            foreach (AttestationReviewEndpointSystemL2Detail l2Detail in reviewEndpointSystem.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details)
                            {
                                var entry = _context.Entry(l2Detail);
                                if (entry.State == EntityState.Detached)
                                {
                                    var set = _context.Set<AttestationReviewEndpointSystemL2Detail>();
                                    var attachedEntity = set.Find(l2Detail.Id);

                                    if (attachedEntity != null)
                                    {
                                        var attachedEntry = _context.Entry(attachedEntity);
                                        attachedEntry.CurrentValues.SetValues(l2Detail);
                                    }
                                }

                                foreach (AttestationReviewEndpointSystemL3Detail l3Detail in l2Detail.AttestationReviewEndpointSystemL3Details)
                                {
                                    var l3entry = _context.Entry(l3Detail);
                                    if (l3entry.State == EntityState.Detached)
                                    {
                                        var set = _context.Set<AttestationReviewEndpointSystemL3Detail>();
                                        var attachedEntity = set.Find(l3Detail.Id);

                                        if (attachedEntity != null)
                                        {
                                            var attachedEntry = _context.Entry(attachedEntity);
                                            attachedEntry.CurrentValues.SetValues(l3Detail);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Configuration.ProxyCreationEnabled = false;

                    if (!deepLoad)
                    {
                        return _context.Tasks.FirstOrDefault(item => item.Id == taskId);
                    }
                    return (from item in _context.Tasks
                        .Include("TaskReviews")
                        .Include("TaskReviews.AttestationReview")
                        .Include("TaskReviews.AttestationReview.User")
                        .Include("TaskReviews.AttestationReview.User.Organisation")
                        .Include("TaskReviews.AttestationReview.ReviewEndpointSystems")
                        .Include("TaskReviews.AttestationReview.ReviewEndpointSystems.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details")
                        .Include("TaskReviews.AttestationReview.ReviewEndpointSystems.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details.AttestationReviewEndpointSystemL3Details")
                            where item.Id == taskId
                            select item).FirstOrDefault();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Configuration.ProxyCreationEnabled = false;

                    if (!deepLoad)
                    {
                        return _context.AttestationReviews.FirstOrDefault(item => item.Id == reviewId);
                    }
                    return (from item in _context.AttestationReviews
                        .Include("AttestationReview.User")
                        .Include("AttestationReview.ReviewEndpointSystems")
                        .Include("AttestationReview.ReviewEndpointSystems.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details")
                            where item.Id == reviewId
                            select item).FirstOrDefault();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.AttestationReviews.Where(leAtnReview => leAtnReview.RemediationStatus == AttestationRemediationStatus.Pending)
                           .Include("User")
                           .Include("ReviewEndpointSystems")
                           .Include("ReviewEndpointSystems.AttestationReviewEndpointSystem")
                           .Include("ReviewEndpointSystems.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details")
                            select item).ToList();
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Configuration.ProxyCreationEnabled = false;

                    return (from item in _context.Tasks
                           .Include("TaskReviews")
                            where item.Id == taskId
                            select item).FirstOrDefault();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Configuration.ProxyCreationEnabled = false;

                    return _context.Tasks.Where(item => item.WorkflowInstanceId == workflowInstanceId && !item.IsCompleted && item.IsActive).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Tasks.Where(item => item.WorkflowInstanceId == workflowInstanceId).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public Task GetCompletedTaskByWorkflowInstanceIdAndState(Guid workflowInstanceId, byte state)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Tasks.FirstOrDefault(item => item.WorkflowInstanceId == workflowInstanceId && item.WorkflowStateId == state
                                        && item.IsCompleted == true && item.IsActive == true);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<Task> GetTasksByActionUserId(long actionUserId, bool activeAlone)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (activeAlone)
                    {
                        return _context.Tasks.Where(item => item.ActionUserId == actionUserId && item.IsActive && item.IsCompleted == false).ToList();
                    }
                    return _context.Tasks.Where(item => item.ActionUserId == actionUserId).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<TaskGridData> GetTaskDetailsByActionUserId(long actionUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var attestationTasks = (from task in _context.Tasks
                                            join attestattion in _context.Attestations
                                                on task.AttestationId equals attestattion.Id
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where (task.ActionUserId == actionUserId && (!task.IsGroupTask || (task.IsGroupTask && task.AssignedTo == actionUserId)))
                                            select new TaskGridData
                                            {
                                                TaskId = task.Id,
                                                RequestTypeId = task.RequestTypeId,
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                EventName = string.Empty,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                UserName = string.Empty,
                                                AdHocAccessRequestId = 0,
                                                AccessRequestID = 0, //requests.AccessRequestId,
                                                RequestedBy = string.Empty,
                                                AssignedTo = task.AssignedTo,
                                                AssignedBy = task.AssignedBy,
                                                AccountType = AccountType.None,
                                                IsDelegated = task.IsDelegated,
                                                IsGroup = task.IsGroupTask,
                                                RequestedFor = string.Empty,
                                                AssignedToName = string.Empty,
                                                AssignedByName = string.Empty,
                                                StartDate = null,
                                                EndDate = null,
                                                RequestedDate = null,
                                                Type = string.Empty,
                                                StatusType = task.Action,
                                                RequestReason = string.Empty,
                                                DelegatedBy = task.DelegatedBy,
                                                DelegatedComment = task.DelegationComments
                                            });

                    var provisioningTasks = (from tsk in _context.Tasks
                                             join acsrqst in _context.AccessRequests
                                             on tsk.AccessRequestId equals acsrqst.Id
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.Id
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                             join assignedby in _context.Users
                                                    on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                             join assignedUser in _context.Users
                                               on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                             from assignedUser in detail.DefaultIfEmpty()
                                             from assignedby in Adetail.DefaultIfEmpty()
                                             join wfConfig in _context.WorkflowConfigurations
                                             on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && (!tsk.IsGroupTask || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.Id,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = string.Empty,
                                                 AdHocAccessRequestId = adhocrqst.Id,
                                                 AccessRequestID = acsrqst.Id,
                                                 RequestedBy = acsrqst.RequestSource == AccessRequestSource.AdhocRequest ? rqsteduser.DisplayName : "System",
                                                 AssignedTo = tsk.AssignedTo,
                                                 AssignedBy = tsk.AssignedBy,
                                                 AccountType = adhocrqst.AccountType,
                                                 IsDelegated = tsk.IsDelegated,
                                                 IsGroup = tsk.IsGroupTask,
                                                 RequestedFor = rqsteruser.DisplayName,
                                                 AssignedToName = assignedUser.DisplayName,
                                                 AssignedByName = assignedby.DisplayName,
                                                 StartDate = adhocrqst.ValidFrom,
                                                 EndDate = adhocrqst.ValidTo,
                                                 RequestedDate = adhocrqst.CreatedDate,
                                                 Type = ((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                                 StatusType = tsk.Action,
                                                 RequestReason = acsrqst.Comments,
                                                 DelegatedBy = tsk.DelegatedBy,
                                                 DelegatedComment = tsk.DelegationComments
                                             });

                    var tasks = attestationTasks.Union(provisioningTasks);

                    tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

                    return tasks.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<TaskGridData> GetTaskDetailsByActionUserIdAndTaskStatus(long actionUserId, StatusType taskStatus)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var attestationTasks = (from task in _context.Tasks
                                            join attestattion in _context.Attestations
                                                on task.AttestationId equals attestattion.Id
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where (task.ActionUserId == actionUserId && task.Action == taskStatus && (!task.IsGroupTask || (task.IsGroupTask && task.AssignedTo == actionUserId)))
                                            select new TaskGridData
                                            {
                                                TaskId = task.Id,
                                                RequestTypeId = task.RequestTypeId,
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                EventName = string.Empty,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                UserName = string.Empty,
                                                AdHocAccessRequestId = 0,
                                                AccessRequestID = 0, //requests.AccessRequestId,
                                                RequestedBy = string.Empty,
                                                AssignedTo = task.AssignedTo,
                                                AssignedBy = task.AssignedBy,
                                                AccountType = AccountType.None,
                                                IsDelegated = task.IsDelegated,
                                                IsGroup = task.IsGroupTask,
                                                RequestedFor = string.Empty,
                                                AssignedToName = string.Empty,
                                                AssignedByName = string.Empty,
                                                StartDate = null,
                                                EndDate = null,
                                                RequestedDate = null,
                                                Type = string.Empty,
                                                StatusType = task.Action,
                                                RequestReason = string.Empty,
                                                DelegatedBy = task.DelegatedBy,
                                                DelegatedComment = task.DelegationComments
                                            });

                    var provisioningTasks = (from tsk in _context.Tasks
                                             join acsrqst in _context.AccessRequests
                                             on tsk.AccessRequestId equals acsrqst.Id
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.Id
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                             join assignedby in _context.Users
                                                    on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                             join assignedUser in _context.Users
                                               on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                             from assignedUser in detail.DefaultIfEmpty()
                                             from assignedby in Adetail.DefaultIfEmpty()
                                             join wfConfig in _context.WorkflowConfigurations
                                             on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             //where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus && (!tsk.IsGroupTask || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))
                                             where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus)
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.Id,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = string.Empty,
                                                 AdHocAccessRequestId = adhocrqst.Id,
                                                 AccessRequestID = acsrqst.Id,
                                                 RequestedBy = acsrqst.RequestSource == AccessRequestSource.AdhocRequest ? rqsteduser.DisplayName : "System",
                                                 AssignedTo = tsk.AssignedTo,
                                                 AssignedBy = tsk.AssignedBy,
                                                 AccountType = adhocrqst.AccountType,
                                                 IsDelegated = tsk.IsDelegated,
                                                 IsGroup = tsk.IsGroupTask,
                                                 RequestedFor = rqsteruser.DisplayName,
                                                 AssignedToName = assignedUser.DisplayName,
                                                 AssignedByName = assignedby.DisplayName,
                                                 StartDate = adhocrqst.ValidFrom,
                                                 EndDate = adhocrqst.ValidTo,
                                                 RequestedDate = adhocrqst.CreatedDate,
                                                 Type = ((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                                 StatusType = tsk.Action,
                                                 RequestReason = acsrqst.Comments,
                                                 DelegatedBy = tsk.DelegatedBy,
                                                 DelegatedComment = tsk.DelegationComments
                                             });

                    var tasks = attestationTasks.Union(provisioningTasks);

                    tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

                    return tasks.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<TaskGridData> GetGroupTaskDetailsByActionUserId(long actionUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<long> userGroups = _context.Users.FirstOrDefault(item => item.Id == actionUserId).Groups.Select(item => item.Id).ToList();

                    var attestationTasks = (from task in _context.Tasks
                                            join attestattion in _context.Attestations
                                                on task.AttestationId equals attestattion.Id
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where (userGroups.Contains((int)task.ActionGroupId.Value) && task.IsGroupTask)
                                            select new TaskGridData
                                            {
                                                TaskId = task.Id,
                                                RequestTypeId = task.RequestTypeId,
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                EventName = string.Empty,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                UserName = string.Empty,
                                                AdHocAccessRequestId = 0,
                                                AccessRequestID = 0,
                                                RequestedBy = string.Empty,
                                                AssignedTo = task.AssignedTo,
                                                AssignedBy = task.AssignedBy,
                                                AccountType = AccountType.None,
                                                IsDelegated = task.IsDelegated,
                                                IsGroup = task.IsGroupTask,
                                                RequestedFor = string.Empty,
                                                AssignedToName = string.Empty,
                                                AssignedByName = string.Empty,
                                                StartDate = null,
                                                EndDate = null,
                                                RequestedDate = null,
                                                Type = string.Empty
                                            });

                    var provisioningTasks = (from tsk in _context.Tasks
                                             join acsrqst in _context.AccessRequests
                                             on tsk.AccessRequestId equals acsrqst.Id
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.Id
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                             join assignedby in _context.Users
                                                    on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                             join assignedUser in _context.Users
                                               on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                             from assignedUser in detail.DefaultIfEmpty()
                                             from assignedby in Adetail.DefaultIfEmpty()
                                             join wfConfig in _context.WorkflowConfigurations
                                             on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             where (userGroups.Contains((Int32)tsk.ActionGroupId.Value) && acsrqst.AdHocAccessRequestId.HasValue && tsk.IsGroupTask)
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.Id,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = string.Empty,
                                                 AdHocAccessRequestId = adhocrqst.Id,
                                                 AccessRequestID = acsrqst.Id,
                                                 RequestedBy = acsrqst.RequestSource == AccessRequestSource.AdhocRequest ? rqsteduser.DisplayName : "System",
                                                 AssignedTo = tsk.AssignedTo,
                                                 AssignedBy = tsk.AssignedBy,
                                                 AccountType = adhocrqst.AccountType,
                                                 IsDelegated = tsk.IsDelegated,
                                                 IsGroup = tsk.IsGroupTask,
                                                 RequestedFor = rqsteruser.DisplayName,
                                                 AssignedToName = assignedUser.DisplayName,
                                                 AssignedByName = assignedby.DisplayName,
                                                 StartDate = adhocrqst.ValidFrom,
                                                 EndDate = adhocrqst.ValidTo,
                                                 RequestedDate = adhocrqst.CreatedDate,
                                                 Type = ((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")
                                             });

                    var tasks = attestationTasks.Union(provisioningTasks);

                    tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

                    return tasks.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IList<TaskGridData> GetGroupTaskDetailsByActionUserIdAndTaskStatus(long actionUserId, StatusType taskStatus)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<long> userGroups = _context.Users.FirstOrDefault(item => item.Id == actionUserId).Groups.Select(item => item.Id).ToList();

                    var attestationTasks = (from task in _context.Tasks
                                            join attestattion in _context.Attestations
                                                on task.AttestationId equals attestattion.Id
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where (userGroups.Contains((Int32)task.ActionGroupId.Value) && task.Action == taskStatus && task.IsGroupTask)
                                            select new TaskGridData
                                            {
                                                TaskId = task.Id,
                                                RequestTypeId = task.RequestTypeId,
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                EventName = string.Empty,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                UserName = string.Empty,
                                                AdHocAccessRequestId = 0,
                                                AccessRequestID = 0,
                                                RequestedBy = string.Empty,
                                                AssignedTo = task.AssignedTo,
                                                AssignedBy = task.AssignedBy,
                                                AccountType = AccountType.None,
                                                IsDelegated = task.IsDelegated,
                                                IsGroup = task.IsGroupTask,
                                                RequestedFor = string.Empty,
                                                AssignedToName = string.Empty,
                                                AssignedByName = string.Empty,
                                                StartDate = null,
                                                EndDate = null,
                                                RequestedDate = null,
                                                Type = string.Empty
                                            });

                    var provisioningTasks = (from tsk in _context.Tasks
                                             join acsrqst in _context.AccessRequests
                                             on tsk.AccessRequestId equals acsrqst.Id
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.Id
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                             join assignedby in _context.Users
                                                    on (long)tsk.AssignedBy equals assignedby.Id into Adetail
                                             join assignedUser in _context.Users
                                               on (long)tsk.AssignedTo equals assignedUser.Id into detail
                                             from assignedUser in detail.DefaultIfEmpty()
                                             from assignedby in Adetail.DefaultIfEmpty()
                                             join wfConfig in _context.WorkflowConfigurations
                                             on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             where (userGroups.Contains((Int32)tsk.ActionGroupId.Value) && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus && tsk.IsGroupTask)
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.Id,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = string.Empty,
                                                 AdHocAccessRequestId = adhocrqst.Id,
                                                 AccessRequestID = acsrqst.Id,
                                                 RequestedBy = acsrqst.RequestSource == AccessRequestSource.AdhocRequest ? rqsteduser.DisplayName : "System",
                                                 AssignedTo = tsk.AssignedTo,
                                                 AssignedBy = tsk.AssignedBy,
                                                 AccountType = adhocrqst.AccountType,
                                                 IsDelegated = tsk.IsDelegated,
                                                 IsGroup = tsk.IsGroupTask,
                                                 RequestedFor = rqsteruser.DisplayName,
                                                 AssignedToName = assignedUser.DisplayName,
                                                 AssignedByName = assignedby.DisplayName,
                                                 StartDate = adhocrqst.ValidFrom,
                                                 EndDate = adhocrqst.ValidTo,
                                                 RequestedDate = adhocrqst.CreatedDate,
                                                 Type = ((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")
                                             });

                    var tasks = attestationTasks.Union(provisioningTasks);

                    tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

                    return tasks.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<TaskGridData> getSubmittedAttestationTaskDetailsByActionUserId(int actionUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from task in _context.Tasks
                            where task.IsCompleted
                            join attestattion in _context.Attestations
                                on task.AttestationId equals attestattion.Id
                            join wfConfig in _context.WorkflowConfigurations
                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                            where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                            where task.ActionUserId == actionUserId
                            select new TaskGridData
                            {
                                TaskId = task.Id,
                                RequestTypeId = task.RequestTypeId,
                                AttestationId = task.AttestationId,
                                AttestationName = attestattion.AttestationName,
                                EventName = string.Empty,
                                CreatedDate = task.CreatedDate,
                                IsActive = task.IsActive,
                                IsCompleted = task.IsCompleted,
                                ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                UserName = string.Empty,
                                SubmittedDate = task.LastModifiedDate.Value
                            }).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from task in _context.Tasks
                            join attestattion in _context.Attestations
                                on task.AttestationId equals attestattion.Id
                            where (attestattion.Id == paramAttestationId || paramAttestationId == -1)
                            join attestattionProcess in _context.AttestationProcesses
                                on task.AttestationId equals attestattionProcess.AttestationId
                            join wfConfig in _context.WorkflowConfigurations
                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                            join user in _context.Users
                                on task.ActionUserId equals user.Id
                            join org in _context.Organisations
                                on user.OrganisationId.Value equals org.Id
                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                            where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId &&
                            wfConfigDetail.SequenceNumber == task.WorkflowStateId &&
                            task.CreatedDate >= attestattionProcess.ProcessStartAt
                            select new TaskReportData
                            {
                                TaskId = task.Id,
                                Level = task.WorkflowStateId,
                                Organisation = org.OrganisationName,
                                ActionUser = user.DisplayName,
                                WorkflowInstanceId = task.WorkflowInstanceId,
                                SubmittedOn = task.LastModifiedDate.Value,
                                AttestationName = attestattion.AttestationName,
                                CreatedDate = task.CreatedDate,
                                IsActive = task.IsActive,
                                IsCompleted = task.IsCompleted,
                                ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays
                            }).OrderByDescending(rvw => rvw.IsActive).ThenBy(rvw1 => rvw1.IsCompleted).ThenBy(rvw2 => rvw2.Level).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateTask(Task task)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var taskEntry = _context.Entry(task);
                    if (taskEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<Task>();
                        Task attachedTask = set.Find(task.Id);

                        if (attachedTask != null)
                        {
                            var attachedEntry = _context.Entry(attachedTask);
                            attachedEntry.CurrentValues.SetValues(task);
                        }
                    }
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var pendingTaskCount = (from task in _context.Tasks
                                            where task.WorkflowInstanceId == workflowInstanceId
                                            from taskReview in task.TaskReviews
                                            from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                                            where reviewEP.AttestationReviewEndpointSystem.IsCurrentLevelApproved == false
                                            select taskReview).Count();
                    return pendingTaskCount > 0 ? true : false;
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.ReviewEndpointSystems.Add(reviewEndpointSystem);
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var reviewEPEntry = _context.Entry(reviewEP);
                    if (reviewEPEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<ReviewEndpointSystem>();
                        ReviewEndpointSystem attachedreviewEP = set.Find(reviewEP.Id);

                        if (attachedreviewEP != null)
                        {
                            var attachedEntry = _context.Entry(attachedreviewEP);
                            attachedEntry.CurrentValues.SetValues(reviewEP);
                        }
                    }
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var reviewEPEntry = _context.Entry(reviewEP);
                    if (reviewEPEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<AttestationReviewEndpointSystem>();
                        AttestationReviewEndpointSystem attachedreviewEP = set.Find(reviewEP.Id);

                        if (attachedreviewEP != null)
                        {
                            var attachedEntry = _context.Entry(attachedreviewEP);
                            attachedEntry.CurrentValues.SetValues(reviewEP);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public TaskDelegation GetTaskDelegationByUserId(long userId, RequestType requestType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    DateTime today = DateTime.Now.Date;

                    if (requestType == RequestType.Attestation)
                    {
                        return _context.TaskDelegations.FirstOrDefault(item => item.ActionUser == userId && item.IsActive && item.StartDate <= today &&
                        item.EndDate >= today && item.DelegateAttestationTask);
                    }
                    if (requestType == RequestType.Provisioning)
                    {
                        return _context.TaskDelegations.FirstOrDefault(item => item.ActionUser == userId && item.IsActive && item.StartDate <= today &&
                                                                               item.EndDate >= today && item.DelegateProvisioningTask);
                    }
                    if (requestType == RequestType.Compliance)
                    {
                        return _context.TaskDelegations.FirstOrDefault(item => item.ActionUser == userId && item.IsActive && item.StartDate <= today &&
                                                                               item.EndDate >= today && item.DelegateSODTask);
                    }
                    return null;
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (isAdmin)
                    {
                        return (from item in _context.TaskDelegations
                                join user in _context.Users
                                    on item.DelegatedTo equals user.Id
                                join user1 in _context.Users
                                    on item.ActionUser equals user1.Id
                                where item.EndDate >= DateTime.Today
                                orderby item.IsActive descending
                                select new TaskDelegationGridData
                                {
                                    ActionUser = item.ActionUser,
                                    ActionUserName = user1.DisplayName,
                                    CreatedDate = item.CreatedDate,
                                    DelegateAttestationTask = item.DelegateAttestationTask,
                                    DelegatedToUserId = item.DelegatedTo,
                                    DelegatedToUserName = user.DisplayName,
                                    DelegateProvisioningTask = item.DelegateProvisioningTask,
                                    DelegateSODTask = item.DelegateSODTask,
                                    DelegationId = item.Id,
                                    EndDate = item.EndDate,
                                    IsActive = item.IsActive,
                                    StartDate = item.StartDate,
                                    DelegationComments = item.DelegationComments,
                                    CreatedBy = item.CreatedBy,
                                    LastModifiedBy = item.LastModifiedBy,
                                    LastModifiedDate = item.LastModifiedDate
                                }).ToList();
                    }
                    return (from item in _context.TaskDelegations
                            join user in _context.Users
                                on item.DelegatedTo equals user.Id
                            join user1 in _context.Users
                                on item.ActionUser equals user1.Id
                            where item.ActionUser == userId && item.EndDate >= DateTime.Today
                            orderby item.IsActive descending
                            select new TaskDelegationGridData
                            {
                                ActionUser = item.ActionUser,
                                ActionUserName = user1.DisplayName,
                                CreatedDate = item.CreatedDate,
                                DelegateAttestationTask = item.DelegateAttestationTask,
                                DelegatedToUserId = item.DelegatedTo,
                                DelegatedToUserName = user.DisplayName,
                                DelegateProvisioningTask = item.DelegateProvisioningTask,
                                DelegateSODTask = item.DelegateSODTask,
                                DelegationId = item.Id,
                                EndDate = item.EndDate,
                                IsActive = item.IsActive,
                                StartDate = item.StartDate,
                                DelegationComments = item.DelegationComments,
                                CreatedBy = item.CreatedBy,
                                LastModifiedBy = item.LastModifiedBy,
                                LastModifiedDate = item.LastModifiedDate
                            }).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<TaskDelegation> GetAllDelegationsByUserId(long userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.TaskDelegations.Where(item => item.ActionUser == userId && item.IsActive).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.TaskDelegations.FirstOrDefault(item => item.Id == delegationId);
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.TaskDelegations.Add(taskDelegation);
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(taskDelegation);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<TaskDelegation>();
                        TaskDelegation attachedDelegation = set.Find(taskDelegation.Id);

                        if (attachedDelegation != null)
                        {
                            var attachedEntry = _context.Entry(attachedDelegation);
                            attachedEntry.CurrentValues.SetValues(taskDelegation);
                        }
                    }
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AttestationReviewEndpointSystemHistories.Where(item => item.ReviewEndpointId == reviewEndpointId).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AttestationReviewEndpointSystemL2DetailHistories.Where(item => item.L2DetailId == l2DetailId).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AttestationReviewEndpointSystemL3DetailHistories.Where(item => item.L2DetailId == l3DetailId).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AttestationReviewEndpointSystemHistories.Where(item => item.ReviewEndpointId == reviewEndpointId && !string.IsNullOrEmpty(item.Comments)).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AttestationReviewEndpointSystemL2DetailHistories.Where(item => item.L2DetailId == l2DetailId && !string.IsNullOrEmpty(item.Comments)).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AttestationReviewEndpointSystemL3DetailHistories.Where(item => item.L2DetailId == l3DetailId && !string.IsNullOrEmpty(item.Comments)).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AccessRequestHistories.Where(item => item.AccessRequestId == accessRequestId && !string.IsNullOrEmpty(item.Comments)).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from task in _context.Tasks
                            join user in _context.Users
                                on task.ActionUserId equals user.Id
                            where task.WorkflowInstanceId == workflowInstanceId && task.IsCompleted
                            select new PreviousApproversGridData
                            {
                                ApprovedDate = task.LastModifiedDate.Value,
                                UserName = user.DisplayName
                            }).OrderByDescending(rvw => rvw.ApprovedDate).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Tasks.Add(task);
                    _context.SaveChanges();

                    return task.Id;
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var taskEntry = _context.Entry(task);
                    if (taskEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<Task>();
                        Task attachedTask = set.Find(task.Id);

                        if (attachedTask != null)
                        {
                            var attachedEntry = _context.Entry(attachedTask);
                            attachedEntry.CurrentValues.SetValues(task);
                        }
                    }

                    if (task.AccessRequest != null)
                    {
                        var reviewEntry = _context.Entry(task.AccessRequest);
                        if (reviewEntry.State == EntityState.Detached)
                        {
                            var set = _context.Set<AccessRequest>();
                            AccessRequest attachedAccReq = set.Find(task.AccessRequest.Id);

                            if (attachedAccReq != null)
                            {
                                var attachedEntry = _context.Entry(attachedAccReq);
                                attachedEntry.CurrentValues.SetValues(task.AccessRequest);
                            }
                        }
                        _context.SaveChanges();
                    }
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Configuration.ProxyCreationEnabled = false;

                    if (!deepLoad)
                    {
                        return _context.Tasks.FirstOrDefault(item => item.Id == taskId);
                    }
                    return (from item in _context.Tasks
                        .Include("AccessRequest")
                        .Include("AccessRequest.User")
                        .Include("AccessRequest.AdHocAccessRequest")
                        .Include("AccessRequest.AdHocAccessRequest.EndPoint")
                        .Include("SubTask")
                            where item.Id == taskId
                            select item).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public Task GetPendingTaskByAccessRequestId(long accessRequestId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.Tasks.FirstOrDefault(item => item.AccessRequestId == accessRequestId && item.IsActive && item.IsCompleted == false);
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var accReqEntry = _context.Entry(accessRequest);
                    if (accReqEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<AccessRequest>();
                        AccessRequest attachedAccReq = set.Find(accessRequest.Id);

                        if (attachedAccReq != null)
                        {
                            var attachedEntry = _context.Entry(attachedAccReq);
                            attachedEntry.CurrentValues.SetValues(accessRequest);
                        }
                    }
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Tasks.Add(task);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<TaskGridData> GetPendingTaskDetails(int actionUserId, DateTime fromDate)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (actionUserId != 0)
                    {
                        var attestationTasks = (from task in _context.Tasks
                                                join user in _context.Users
                                                  on task.ActionUserId equals user.Id
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.Id
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                      task.IsActive &&
                                                      task.IsCompleted == false &&
                                                      task.ActionUserId == actionUserId
                                                select new TaskGridData
                                                {
                                                    TaskId = task.Id,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.Id,
                                                    UserName = user.DisplayName,
                                                    AttestationId = task.AttestationId,
                                                    AttestationName = attestattion.AttestationName,
                                                    CreatedDate = task.CreatedDate,
                                                    IsActive = task.IsActive,
                                                    IsCompleted = task.IsCompleted,
                                                    ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                    StartDate = null,
                                                    EndDate = null
                                                });

                        var provisioningTasks = (from tsk in _context.Tasks
                                                 join acsrqst in _context.AccessRequests
                                                 on tsk.AccessRequestId equals acsrqst.Id
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.Id
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                                 where tsk.CreatedDate >= fromDate &&
                                                     tsk.IsActive &&
                                                     tsk.IsCompleted == false &&
                                                     tsk.ActionUserId == actionUserId
                                                 select new TaskGridData
                                                 {
                                                     TaskId = tsk.Id,
                                                     RequestTypeId = tsk.RequestTypeId,
                                                     AttestationId = tsk.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = tsk.CreatedDate,
                                                     IsActive = tsk.IsActive,
                                                     IsCompleted = tsk.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.Id,
                                                     RequestedBy = rqsteduser.DisplayName,
                                                     StartDate = adhocrqst.ValidFrom,
                                                     EndDate = adhocrqst.ValidTo
                                                 });

                        return attestationTasks.Union(provisioningTasks).OrderByDescending(rvw => rvw.CreatedDate).ToList();
                    }
                    else
                    {
                        var attestationTasks = (from task in _context.Tasks
                                                join user in _context.Users
                                                  on task.ActionUserId equals user.Id
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.Id
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                     task.IsActive &&
                                                     task.IsCompleted == false
                                                select new TaskGridData
                                                {
                                                    TaskId = task.Id,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.Id,
                                                    UserName = user.DisplayName,
                                                    AttestationId = task.AttestationId,
                                                    AttestationName = attestattion.AttestationName,
                                                    CreatedDate = task.CreatedDate,
                                                    IsActive = task.IsActive,
                                                    IsCompleted = task.IsCompleted,
                                                    ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                    StartDate = null,
                                                    EndDate = null
                                                });

                        var provisioningTasks = (from tsk in _context.Tasks
                                                 join acsrqst in _context.AccessRequests
                                                 on tsk.AccessRequestId equals acsrqst.Id
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.Id
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                                 where tsk.CreatedDate >= fromDate &&
                                                     tsk.IsActive &&
                                                     tsk.IsCompleted == false
                                                 select new TaskGridData
                                                 {
                                                     TaskId = tsk.Id,
                                                     RequestTypeId = tsk.RequestTypeId,
                                                     AttestationId = tsk.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = tsk.CreatedDate,
                                                     IsActive = tsk.IsActive,
                                                     IsCompleted = tsk.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.Id,
                                                     RequestedBy = rqsteduser.DisplayName,
                                                     StartDate = adhocrqst.ValidFrom,
                                                     EndDate = adhocrqst.ValidTo
                                                 });

                        return attestationTasks.Union(provisioningTasks).OrderByDescending(rvw => rvw.CreatedDate).ToList();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public List<TaskGridData> GetCompletedTaskDetails(int actionUserId, DateTime fromDate)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (actionUserId != 0)
                    {
                        var attestationTasks = (from task in _context.Tasks
                                                join user in _context.Users
                                                  on task.ActionUserId equals user.Id
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.Id
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                      task.IsActive &&
                                                      task.IsCompleted &&
                                                      task.ActionUserId == actionUserId
                                                select new TaskGridData
                                                {
                                                    TaskId = task.Id,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.Id,
                                                    UserName = user.DisplayName,
                                                    AttestationId = task.AttestationId,
                                                    AttestationName = attestattion.AttestationName,
                                                    CreatedDate = task.CreatedDate,
                                                    IsActive = task.IsActive,
                                                    IsCompleted = task.IsCompleted,
                                                    ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                    StartDate = null,
                                                    EndDate = null
                                                });

                        var provisioningTasks = (from task in _context.Tasks
                                                 join acsrqst in _context.AccessRequests
                                                 on task.AccessRequestId equals acsrqst.Id
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.Id
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                 where task.CreatedDate >= fromDate &&
                                                    task.IsActive &&
                                                    task.IsCompleted &&
                                                    task.ActionUserId == actionUserId
                                                 select new TaskGridData
                                                 {
                                                     TaskId = task.Id,
                                                     RequestTypeId = task.RequestTypeId,
                                                     AttestationId = task.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = task.CreatedDate,
                                                     IsActive = task.IsActive,
                                                     IsCompleted = task.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.Id,
                                                     RequestedBy = rqsteduser.DisplayName,
                                                     StartDate = adhocrqst.ValidFrom,
                                                     EndDate = adhocrqst.ValidTo
                                                 });

                        return attestationTasks.Union(provisioningTasks).OrderByDescending(rvw => rvw.CreatedDate).ToList();
                    }
                    else
                    {
                        var attestationTasks = (from task in _context.Tasks
                                                join user in _context.Users
                                                  on task.ActionUserId equals user.Id
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.Id
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                     task.IsActive &&
                                                     task.IsCompleted
                                                select new TaskGridData
                                                {
                                                    TaskId = task.Id,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.Id,
                                                    UserName = user.DisplayName,
                                                    AttestationId = task.AttestationId,
                                                    AttestationName = attestattion.AttestationName,
                                                    CreatedDate = task.CreatedDate,
                                                    IsActive = task.IsActive,
                                                    IsCompleted = task.IsCompleted,
                                                    ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                    StartDate = null,
                                                    EndDate = null
                                                });

                        var provisioningTasks = (from task in _context.Tasks
                                                 join acsrqst in _context.AccessRequests
                                                 on task.AccessRequestId equals acsrqst.Id
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.Id
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                 where task.CreatedDate >= fromDate &&
                                                    task.IsActive &&
                                                    task.IsCompleted
                                                 select new TaskGridData
                                                 {
                                                     TaskId = task.Id,
                                                     RequestTypeId = task.RequestTypeId,
                                                     AttestationId = task.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = task.CreatedDate,
                                                     IsActive = task.IsActive,
                                                     IsCompleted = task.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.Id,
                                                     RequestedBy = rqsteduser.DisplayName,
                                                     StartDate = adhocrqst.ValidFrom,
                                                     EndDate = adhocrqst.ValidTo
                                                 });

                        return attestationTasks.Union(provisioningTasks).OrderByDescending(rvw => rvw.CreatedDate).ToList();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public List<TaskGridData> GetAutoCompletedTaskDetails(int actionUserId, DateTime fromDate)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (actionUserId != 0)
                    {
                        var attestationTasks = (from task in _context.Tasks
                                                join user in _context.Users
                                                  on task.ActionUserId equals user.Id
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.Id
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                      task.IsActive == false &&
                                                      task.IsCompleted == false &&
                                                      task.ActionUserId == actionUserId
                                                select new TaskGridData
                                                {
                                                    TaskId = task.Id,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.Id,
                                                    UserName = user.DisplayName,
                                                    AttestationId = task.AttestationId,
                                                    AttestationName = attestattion.AttestationName,
                                                    CreatedDate = task.CreatedDate,
                                                    IsActive = task.IsActive,
                                                    IsCompleted = task.IsCompleted,
                                                    ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                    StartDate = null,
                                                    EndDate = null
                                                });

                        var provisioningTasks = (from task in _context.Tasks
                                                 join acsrqst in _context.AccessRequests
                                                 on task.AccessRequestId equals acsrqst.Id
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.Id
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                 where task.CreatedDate >= fromDate &&
                                                    task.IsActive == false &&
                                                    task.IsCompleted == false &&
                                                    task.ActionUserId == actionUserId
                                                 select new TaskGridData
                                                 {
                                                     TaskId = task.Id,
                                                     RequestTypeId = task.RequestTypeId,
                                                     AttestationId = task.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = task.CreatedDate,
                                                     IsActive = task.IsActive,
                                                     IsCompleted = task.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.Id,
                                                     RequestedBy = rqsteduser.DisplayName,
                                                     StartDate = adhocrqst.ValidFrom,
                                                     EndDate = adhocrqst.ValidTo
                                                 });

                        return attestationTasks.Union(provisioningTasks).OrderByDescending(rvw => rvw.CreatedDate).ToList();
                    }
                    else
                    {
                        var attestationTasks = (from task in _context.Tasks
                                                join user in _context.Users
                                                  on task.ActionUserId equals user.Id
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.Id
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                     task.IsActive == false &&
                                                     task.IsCompleted == false
                                                select new TaskGridData
                                                {
                                                    TaskId = task.Id,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.Id,
                                                    UserName = user.DisplayName,
                                                    AttestationId = task.AttestationId,
                                                    AttestationName = attestattion.AttestationName,
                                                    CreatedDate = task.CreatedDate,
                                                    IsActive = task.IsActive,
                                                    IsCompleted = task.IsCompleted,
                                                    ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                    StartDate = null,
                                                    EndDate = null
                                                });

                        var provisioningTasks = (from task in _context.Tasks
                                                 join acsrqst in _context.AccessRequests
                                                 on task.AccessRequestId equals acsrqst.Id
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.Id
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                 where task.CreatedDate >= fromDate &&
                                                    task.IsActive == false &&
                                                    task.IsCompleted == false
                                                 select new TaskGridData
                                                 {
                                                     TaskId = task.Id,
                                                     RequestTypeId = task.RequestTypeId,
                                                     AttestationId = task.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = task.CreatedDate,
                                                     IsActive = task.IsActive,
                                                     IsCompleted = task.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.Id,
                                                     RequestedBy = rqsteduser.DisplayName,
                                                     StartDate = adhocrqst.ValidFrom,
                                                     EndDate = adhocrqst.ValidTo
                                                 });

                        return attestationTasks.Union(provisioningTasks).OrderByDescending(rvw => rvw.CreatedDate).ToList();
                    }
                }
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
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    return (from task in _iacmdbcontext.Tasks
                            where task.AttestationId == paramAttestationId && task.Id == (_iacmdbcontext.Tasks.Where(maxTsk => maxTsk.AttestationId == paramAttestationId).Select(max => max.Id).Max())
                            join taskReview in _iacmdbcontext.TaskReviews
                                on task.Id equals taskReview.TaskId
                            join attesReview in _iacmdbcontext.AttestationReviews
                                on taskReview.ReviewId equals attesReview.Id
                            join ReP in _iacmdbcontext.ReviewEndpointSystems
                                on attesReview.Id equals ReP.ReviewId
                            where ReP.TaskId == task.Id
                            join AReP in _iacmdbcontext.AttestationReviewEndpointSystems
                                on ReP.ReviewEndpointId equals AReP.Id
                            join EPS in _iacmdbcontext.EndpointSystems
                                on AReP.EndpointSystemId equals EPS.Id
                            select new AttestationActionGridData
                            {
                                AttestationReviewId = attesReview.Id,
                                AttestationUserName = attesReview.User.UserName,
                                Institution = attesReview.User.TenantId,
                                EndPointType = EPS.EndPointSystemType.DisplayName,
                                HostName = EPS.DisplayName,

                                Comments = attesReview.ActionComments,
                                ActionId = attesReview.ActionId
                            }).ToList();
                }
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
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    AttestationReview aAttReview = _iacmdbcontext.AttestationReviews.FirstOrDefault(review => review.Id == ipAttestationReviewAction.AttestationReviewId);
                    if (aAttReview != null)
                    {
                        aAttReview.ActionComments = ipAttestationReviewAction.Comments;
                        aAttReview.ActionId = ipAttestationReviewAction.ActionId;
                        aAttReview.ActionTakenBy = ipAttestationReviewAction.ActionTakenBy;
                        aAttReview.LastModifiedBy = ipAttestationReviewAction.LastModifiedBy;
                        aAttReview.LastModifiedDate = ipAttestationReviewAction.LastModifiedDate;
                        _iacmdbcontext.SaveChanges();
                    }
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var taskEntry = _context.Entry(attestationReview);
                    if (taskEntry.State == EntityState.Detached)
                    {
                        var set = _context.Set<AttestationReview>();
                        AttestationReview attachedTask = set.Find(attestationReview.Id);

                        if (attachedTask != null)
                        {
                            var attachedEntry = _context.Entry(attachedTask);
                            attachedEntry.CurrentValues.SetValues(attestationReview);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task GetTaskByTaskId(long taskId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Tasks.FirstOrDefault(tsk => tsk.Id == taskId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Task> GetActiveTasksByRequestId(long requestId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Tasks.Where(item => item.AccessRequestId == requestId && item.IsActive && !item.IsCompleted).ToList();
                }
            }
            catch
            {
                throw;
            }
        }


        public bool UpdateGroupTaskByTaskId(Task task)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    Task _task = _context.Tasks.Where(tsk => tsk.Id == task.Id).FirstOrDefault();
                    if (_task != null)
                    {
                        List<Task> taskList = new List<Task>();

                        if (_task.AttestationId.HasValue)
                            taskList = _context.Tasks.Where(tsk => tsk.AttestationId == task.AttestationId).ToList();
                        else if (_task.AccessRequestId.HasValue)
                            taskList = _context.Tasks.Where(tsk => tsk.AccessRequestId == task.AccessRequestId).ToList();

                        foreach (Task _tsk in taskList)
                        {
                            _tsk.AssignedTo = task.AssignedTo;
                            _tsk.AssignedBy = task.AssignedBy;
                            _tsk.LastModifiedBy = task.LastModifiedBy;
                            _tsk.LastModifiedDate = task.LastModifiedDate;
                        }

                        _context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Updating Task.");
                throw;
            }
        }

        public bool IsGroupTaskExistsForUser(long userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<long> userGroups = _context.Users.FirstOrDefault(item => item.Id == userId).Groups.Select(item => item.Id).ToList();
                    return _context.Tasks.Any(item => userGroups.Contains((Int32)item.ActionGroupId.Value) && item.IsActive && !item.IsCompleted && item.IsGroupTask);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetResponseTime(Task task)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (task.RequestTypeId == (byte)RequestType.Attestation)
                    {
                        return (from tasks in _context.Tasks
                                join attestattion in _context.Attestations
                                    on tasks.AttestationId equals attestattion.Id
                                join wfConfig in _context.WorkflowConfigurations
                                    on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                where tasks.Id == task.Id
                                select wfConfigDetail.ResponseTimeInDays).FirstOrDefault();
                    }
                    if (task.RequestTypeId == (byte)RequestType.Provisioning)
                    {
                        return (from tasks in _context.Tasks
                                join acsrqst in _context.AccessRequests
                                on tasks.AccessRequestId equals acsrqst.Id
                                join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                join rqsteduser in _context.Users
                                on adhocrqst.RequesterUserId equals rqsteduser.Id
                                join wfConfig in _context.WorkflowConfigurations
                                on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                where tasks.Id == task.Id
                                select wfConfigDetail.ResponseTimeInDays).FirstOrDefault();
                    }
                    return 0;

                }
            }

            catch (Exception ex)
            {
                return 0;
            }
        }

        public IList<TaskGridData> GetTasksByStatus(StatusType taskStatus)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var attestationTasks = (from task in _context.Tasks
                                            join attestattion in _context.Attestations
                                                on task.AttestationId equals attestattion.Id
                                            join actionuser in _context.Users
                                        on task.ActionUserId equals actionuser.Id
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where task.Action == taskStatus
                                            select new TaskGridData
                                            {
                                                TaskId = task.Id,
                                                RequestTypeId = task.RequestTypeId,
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                EventName = string.Empty,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                UserName = actionuser.UserName,
                                                AdHocAccessRequestId = 0,
                                                RequestedBy = string.Empty,
                                                AssignedTo = task.AssignedTo,
                                                AssignedBy = task.AssignedBy,
                                                AccountType = AccountType.None,
                                                IsDelegated = task.IsDelegated,
                                                IsGroup = task.IsGroupTask,
                                                RequestedFor = string.Empty,
                                                AssignedToName = string.Empty,
                                                StartDate = null,
                                                EndDate = null,
                                                TaskAccessRequest = AccessRequestStatus.None,
                                                AccessRequestID = 0,
                                                Type = string.Empty,
                                                StatusType = task.Action,
                                                RequestReason = string.Empty,
                                                DelegatedBy = task.DelegatedBy,
                                                DelegatedComment = task.DelegationComments
                                            });

                    var provisioningTasks = (from tsk in _context.Tasks
                                             join acsrqst in _context.AccessRequests
                                             on tsk.AccessRequestId equals acsrqst.Id
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.Id
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                             join actionuser in _context.Users
                                               on tsk.ActionUserId equals actionuser.Id
                                             join wfConfig in _context.WorkflowConfigurations
                                           on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             where tsk.Action == taskStatus
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.Id,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = actionuser.UserName,
                                                 AdHocAccessRequestId = adhocrqst.Id,
                                                 RequestedBy = acsrqst.RequestSource == AccessRequestSource.AdhocRequest ? rqsteduser.DisplayName : "System",
                                                 AssignedTo = tsk.AssignedTo,
                                                 AssignedBy = tsk.AssignedBy,
                                                 AccountType = adhocrqst.AccountType,
                                                 IsDelegated = tsk.IsDelegated,
                                                 IsGroup = tsk.IsGroupTask,
                                                 RequestedFor = rqsteruser.DisplayName,
                                                 AssignedToName = string.Empty,
                                                 StartDate = adhocrqst.ValidFrom,
                                                 EndDate = adhocrqst.ValidTo,
                                                 TaskAccessRequest = acsrqst.RequestStatus,
                                                 AccessRequestID = acsrqst.Id,
                                                 Type = ((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                                 StatusType = tsk.Action,
                                                 RequestReason = acsrqst.Comments,
                                                 DelegatedBy = tsk.DelegatedBy,
                                                 DelegatedComment = tsk.DelegationComments
                                             });

                    var tasks = attestationTasks.Union(provisioningTasks);

                    tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

                    return tasks.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IList<TaskGridData> GetAllTasks()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var attestationTasks = (from task in _context.Tasks
                                            join attestattion in _context.Attestations
                                                on task.AttestationId equals attestattion.Id
                                            join actionuser in _context.Users
                                        on task.ActionUserId equals actionuser.Id
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.Id
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            select new TaskGridData
                                            {
                                                TaskId = task.Id,
                                                RequestTypeId = task.RequestTypeId,
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                EventName = string.Empty,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                UserName = actionuser.UserName,
                                                AdHocAccessRequestId = 0,
                                                RequestedBy = string.Empty,
                                                AssignedTo = task.AssignedTo,
                                                AssignedBy = task.AssignedBy,
                                                AccountType = AccountType.None,
                                                IsDelegated = task.IsDelegated,
                                                IsGroup = task.IsGroupTask,
                                                RequestedFor = string.Empty,
                                                AssignedToName = string.Empty,
                                                StartDate = null,
                                                EndDate = null,
                                                TaskAccessRequest = AccessRequestStatus.None,
                                                AccessRequestID = 0,
                                                Type = string.Empty,
                                                StatusType = task.Action,
                                                RequestReason = string.Empty,
                                                DelegatedBy = task.DelegatedBy,
                                                DelegatedComment = task.DelegationComments
                                            });

                    var provisioningTasks = (from tsk in _context.Tasks
                                             join acsrqst in _context.AccessRequests
                                             on tsk.AccessRequestId equals acsrqst.Id
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.Id
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.Id
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.Id
                                             join actionuser in _context.Users
                                               on tsk.ActionUserId equals actionuser.Id
                                             join wfConfig in _context.WorkflowConfigurations
                                             on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.Id == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.Id,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = actionuser.UserName,
                                                 AdHocAccessRequestId = adhocrqst.Id,
                                                 RequestedBy = acsrqst.RequestSource == AccessRequestSource.AdhocRequest ? rqsteduser.DisplayName : "System",
                                                 AssignedTo = tsk.AssignedTo,
                                                 AssignedBy = tsk.AssignedBy,
                                                 AccountType = adhocrqst.AccountType,
                                                 IsDelegated = tsk.IsDelegated,
                                                 IsGroup = tsk.IsGroupTask,
                                                 RequestedFor = rqsteruser.DisplayName,
                                                 AssignedToName = string.Empty,
                                                 StartDate = adhocrqst.ValidFrom,
                                                 EndDate = adhocrqst.ValidTo,
                                                 TaskAccessRequest = acsrqst.RequestStatus,
                                                 AccessRequestID = acsrqst.Id,
                                                 Type = ((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                                 StatusType = tsk.Action,
                                                 RequestReason = acsrqst.Comments,
                                                 DelegatedBy = tsk.DelegatedBy,
                                                 DelegatedComment = tsk.DelegationComments
                                             });

                    var tasks = attestationTasks.Union(provisioningTasks);

                    tasks = tasks.OrderByDescending(rvw => rvw.CreatedDate).ThenBy(rvw1 => rvw1.IsCompleted);

                    return tasks.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task GetApproverByRequestState(long accessRequestId, byte stateId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Tasks.FirstOrDefault(item => item.AccessRequestId == accessRequestId && item.WorkflowStateId == stateId
                        && item.IsActive && item.IsCompleted);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<long> GetActionUsersByRequestState(long accessRequestId, byte stateId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Tasks.Where(item => item.AccessRequestId == accessRequestId && item.WorkflowStateId == stateId)
                        .Select(task => task.ActionUserId.Value).ToList();
                }
            }
            catch
            {
                throw;
            }
        }


        public List<TaskDelegation> GetDelegationsByActionUserID(long ActionUserID)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.TaskDelegations.Include(lq => lq.DelegatedToUser).Where(item => item.ActionUser == ActionUserID)
                    .Select(task => task).ToList();
            }
        }

        public void DeleteDelegateTask(int DelegationId)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                var delegateDetail = _context.TaskDelegations.Where(item => item.Id == DelegationId).Select(task => task).FirstOrDefault();
                _context.TaskDelegations.Remove(delegateDetail);
                _context.SaveChanges();
            }
        }

        public List<Task> GetTasksByRequestIdAndState(long requestId, byte stateId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Tasks.Where(item => item.AccessRequestId == requestId && item.WorkflowStateId == stateId).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        #region Dashboard
        public void GetTaskCompleteness(string attestationName, out int totalTask, out int completedTask)
        {

            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {
                    var lqTotalTask = (from task in context.Tasks
                                       join attestation in context.Attestations on task.AttestationId equals attestation.Id
                                       where attestation.AttestationName == attestationName
                                       select task).Count();

                    var lqCompletedTask = (from task in context.Tasks
                                           join attestation in context.Attestations on task.AttestationId equals attestation.Id
                                           where attestation.AttestationName == attestationName && task.IsCompleted == true
                                           select task).Count();
                    totalTask = lqTotalTask;
                    completedTask = lqCompletedTask;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<Task> GetTaskInfo(string attestationName)
        {
            using (IACMDBContext context = new IACMDBContext())
            {
                var lqTaskInfo = (from task in context.Tasks
                                  join attestation in context.Attestations on task.AttestationId equals attestation.Id
                                  where attestation.AttestationName == attestationName
                                  select new
                                  {
                                      Id = task.Id,
                                      IsCompleted = task.IsCompleted,
                                      IsDelegated = task.IsDelegated,
                                      Comments = task.Comments,
                                      ActionUserId = task.ActionUserId,
                                      AccessRequestId = task.AccessRequestId

                                  }).ToList().Select(leTaskInfo => new Task
                                  {
                                      Id = leTaskInfo.Id,
                                      IsCompleted = leTaskInfo.IsCompleted,
                                      IsDelegated = leTaskInfo.IsDelegated,
                                      Comments = leTaskInfo.Comments,
                                      ActionUserId = leTaskInfo.ActionUserId,
                                      AccessRequestId = leTaskInfo.AccessRequestId

                                  }).ToList();
                return lqTaskInfo;
            }
        }
        public IList<AttestationStatusSummary> GetTaskSummary(string endPointTypeName)
        {
            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {

                    List<AttestationStatusSummary> lstTaskSummary = new List<AttestationStatusSummary>();
                    var lqTaskSummary = (from task in context.Tasks
                                         join attestation in context.Attestations on task.AttestationId equals attestation.Id
                                         join attestationEndPoint in context.AttestationEndpoints on attestation.Id equals attestationEndPoint.AttestationId
                                         join endPointType in context.EndpointSystemTypes on attestationEndPoint.EndpointSystemType equals endPointType.EndpointSystemTypeName
                                         where endPointType.DisplayName == endPointTypeName
                                         select new
                                         {
                                             IsCompleted = task.IsCompleted,
                                             IsDelegated = task.IsDelegated,
                                             IsEscalated = task.IsEscalated

                                         }).ToList().Select(leTaskSummary => new AttestationStatusSummary
                                         {
                                             StatusCount = 1,

                                             Status = ((!leTaskSummary.IsCompleted && !leTaskSummary.IsCompleted) ? "Pending" :
                                                             (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "Completed" :
                                                             (leTaskSummary.IsCompleted && leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "Delegated" :
                                                             (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && leTaskSummary.IsEscalated) ? "Escalated" : ""),


                                             IsExploded = ((!leTaskSummary.IsCompleted && !leTaskSummary.IsCompleted) ? true :
                                                             (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? false :
                                                             (leTaskSummary.IsCompleted && leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? false :
                                                             (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && leTaskSummary.IsEscalated) ? false : false),

                                             BGColor = ((!leTaskSummary.IsCompleted && !leTaskSummary.IsCompleted) ? "#ffdd00" :
                                                             (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "#279691" :
                                                             (leTaskSummary.IsCompleted && leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "#59F0BD" :
                                                             (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && leTaskSummary.IsEscalated) ? "#F51B31" : ""),


                                         }).ToList().GroupBy(leTaskSummary => leTaskSummary.Status);

                    foreach (var grpTaskSummary in lqTaskSummary)
                    {
                        int statusCount = grpTaskSummary.Count();
                        string status = grpTaskSummary.FirstOrDefault().Status;
                        bool isExploded = grpTaskSummary.FirstOrDefault().IsExploded;
                        string bgColor = grpTaskSummary.FirstOrDefault().BGColor;

                        lstTaskSummary.Add(new AttestationStatusSummary(statusCount, status, isExploded, bgColor));
                    }

                    return lstTaskSummary;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<AttestationStatusSummary> GetRevokeStatus(string endPointTypeName)
        {
            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {

                    List<AttestationStatusSummary> lstRevokeSummary = new List<AttestationStatusSummary>();
                    var lqRevokeSummary = (from endPointType in context.EndpointSystemTypes
                                           join endPointSystem in context.EndpointSystems on endPointType.Id equals endPointSystem.EndpointSystemTypeId
                                           join accessRequest in context.AccessRequests on endPointSystem.Id equals accessRequest.EndpointSystemId
                                           where (endPointType.DisplayName == endPointTypeName && accessRequest.RequestSource == AccessRequestSource.AttestationRemediation)
                                           select new
                                           {
                                               ProgressStatus = accessRequest.ProgressStatus

                                           }).ToList().Select(leTaskSummary => new AttestationStatusSummary
                                           {
                                               StatusCount = 1,

                                               Status = ((leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Completed)) ? "Completed" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Executing)) ? "Executing" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ExecutionFailed)) ? "ExecutionFailed" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ReadytoExecute)) ? "ReadytoExecute" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.RequestRejected)) ? "RequestRejected" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ValidationFailed)) ? "ValidationFailed" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.WaitingforApproval)) ? "WaitingforApproval" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Withdrawn)) ? "Withdrawn" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.None)) ? "None" : ""),

                                               IsExploded = false,

                                               BGColor = ((leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Completed)) ? "#90C3D4" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Executing)) ? "#C390D4" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ExecutionFailed)) ? "#D4A190" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ReadytoExecute)) ? "#A1D490" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.RequestRejected)) ? "#C2BF7A" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ValidationFailed)) ? "#7AC29B" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.WaitingforApproval)) ? "#7A7DC2" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Withdrawn)) ? "#C27AA1" :
                                                          (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.None)) ? "black" : ""),


                                           }).ToList().GroupBy(leTaskSummary => leTaskSummary.Status);

                    foreach (var grpRevokeSummary in lqRevokeSummary)
                    {
                        int statusCount = grpRevokeSummary.Count();
                        string status = grpRevokeSummary.FirstOrDefault().Status;
                        bool isExploded = grpRevokeSummary.FirstOrDefault().IsExploded;
                        string bgColor = grpRevokeSummary.FirstOrDefault().BGColor;

                        lstRevokeSummary.Add(new AttestationStatusSummary(statusCount, status, isExploded, bgColor));
                    }

                    return lstRevokeSummary;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
