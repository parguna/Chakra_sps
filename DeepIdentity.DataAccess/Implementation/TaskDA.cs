using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using DeepIdentity.Model.GridData;
using DeepIdentity.Infrastructure;

namespace DeepIdentity.DataAccess
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
                        Task attachedTask = set.Find(task.TaskId);

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
                            TaskReview attachedTaskReview = set.Find(taskReview.TaskReviewId);

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
                            AttestationReview attachedReview = set.Find(taskReview.AttestationReview.ReviewId);

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
                                ReviewEndpointSystem attachedreviewEP = set.Find(reviewEndpointSystem.ReviewEndpointSystemId);

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
                                AttestationReviewEndpointSystem attachedreviewEPS = set.Find(reviewEndpointSystem.AttestationReviewEndpointSystem.ReviewEndpointId);

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
                                    var attachedEntity = set.Find(l2Detail.L2DetailId);

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
                                        var attachedEntity = set.Find(l3Detail.L3DetailId);

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
                        return _context.Tasks.FirstOrDefault(item => item.TaskId == taskId);
                    }
                    return (from item in _context.Tasks
                        .Include("TaskReviews")
                        .Include("TaskReviews.AttestationReview")
                        .Include("TaskReviews.AttestationReview.User")
                        .Include("TaskReviews.AttestationReview.User.Organisation")
                        .Include("TaskReviews.AttestationReview.ReviewEndpointSystems")
                        .Include("TaskReviews.AttestationReview.ReviewEndpointSystems.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details")
                        .Include("TaskReviews.AttestationReview.ReviewEndpointSystems.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details.AttestationReviewEndpointSystemL3Details")
                            where item.TaskId == taskId
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
                        return _context.AttestationReviews.FirstOrDefault(item => item.ReviewId == reviewId);
                    }
                    return (from item in _context.AttestationReviews
                        .Include("AttestationReview.User")
                        .Include("AttestationReview.ReviewEndpointSystems")
                        .Include("AttestationReview.ReviewEndpointSystems.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details")
                            where item.ReviewId == reviewId
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
                    return (from item in _context.AttestationReviews
                           .Include("AttestationReview.User")
                           .Include("AttestationReview.ReviewEndpointSystems")
                           .Include("AttestationReview.ReviewEndpointSystems.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details")
                            where item.RemediationStatus == AttestationRemediationStatus.Pending
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
                            where item.TaskId == taskId
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

        public List<Task> GetTasksByActionUserId(int actionUserId, bool activeAlone)
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

        public IList<TaskGridData> GetTaskDetailsByActionUserId(int actionUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var attestationTasks = (from task in _context.Tasks
                                            join attestattion in _context.Attestations
                                                on task.AttestationId equals attestattion.AttestationId
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where (task.ActionUserId == actionUserId && (!task.IsGroupTask || (task.IsGroupTask && task.AssignedTo == actionUserId)))
                                            select new TaskGridData
                                            {
                                                TaskId = task.TaskId,
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
                                                RequestReason = string.Empty
                                            });

                    var provisioningTasks = (from tsk in _context.Tasks
                                             join acsrqst in _context.AccessRequests
                                             on tsk.AccessRequestId equals acsrqst.AccessRequestId
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.UserId
                                             join assignedby in _context.Users
                                                    on tsk.AssignedBy equals assignedby.UserId into Adetail
                                             join assignedUser in _context.Users
                                               on tsk.AssignedTo equals assignedUser.UserId into detail
                                             from assignedUser in detail.DefaultIfEmpty()
                                             from assignedby in Adetail.DefaultIfEmpty()
                                             join wfConfig in _context.WorkflowConfigurations
                                             on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && (!tsk.IsGroupTask || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.TaskId,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = string.Empty,
                                                 AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
                                                 AccessRequestID = acsrqst.AccessRequestId,
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
                                                 RequestReason = acsrqst.Comments
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

        public IList<TaskGridData> GetTaskDetailsByActionUserIdAndTaskStatus(int actionUserId, StatusType taskStatus)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var attestationTasks = (from task in _context.Tasks
                                            join attestattion in _context.Attestations
                                                on task.AttestationId equals attestattion.AttestationId
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where (task.ActionUserId == actionUserId && task.Action == taskStatus && (!task.IsGroupTask || (task.IsGroupTask && task.AssignedTo == actionUserId)))
                                            select new TaskGridData
                                            {
                                                TaskId = task.TaskId,
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
                                                RequestReason = string.Empty
                                            });

                    var provisioningTasks = (from tsk in _context.Tasks
                                             join acsrqst in _context.AccessRequests
                                             on tsk.AccessRequestId equals acsrqst.AccessRequestId
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.UserId
                                             join assignedby in _context.Users
                                                    on tsk.AssignedBy equals assignedby.UserId into Adetail
                                             join assignedUser in _context.Users
                                               on tsk.AssignedTo equals assignedUser.UserId into detail
                                             from assignedUser in detail.DefaultIfEmpty()
                                             from assignedby in Adetail.DefaultIfEmpty()
                                             join wfConfig in _context.WorkflowConfigurations
                                             on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             where (tsk.ActionUserId == actionUserId && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus && (!tsk.IsGroupTask || (tsk.IsGroupTask && tsk.AssignedTo == actionUserId)))
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.TaskId,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = string.Empty,
                                                 AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
                                                 AccessRequestID = acsrqst.AccessRequestId,
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
                                                 RequestReason = acsrqst.Comments
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

        public IList<TaskGridData> GetGroupTaskDetailsByActionUserId(int actionUserId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<int> userGroups = _context.Users.FirstOrDefault(item => item.UserId == actionUserId).Groups.Select(item => item.GroupId).ToList();

                    var attestationTasks = (from task in _context.Tasks
                                            join attestattion in _context.Attestations
                                                on task.AttestationId equals attestattion.AttestationId
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where (userGroups.Contains(task.ActionGroupId.Value) && task.IsGroupTask)
                                            select new TaskGridData
                                            {
                                                TaskId = task.TaskId,
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
                                             on tsk.AccessRequestId equals acsrqst.AccessRequestId
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.UserId
                                             join assignedby in _context.Users
                                                    on tsk.AssignedBy equals assignedby.UserId into Adetail
                                             join assignedUser in _context.Users
                                               on tsk.AssignedTo equals assignedUser.UserId into detail
                                             from assignedUser in detail.DefaultIfEmpty()
                                             from assignedby in Adetail.DefaultIfEmpty()
                                             join wfConfig in _context.WorkflowConfigurations
                                             on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             where (userGroups.Contains(tsk.ActionGroupId.Value) && acsrqst.AdHocAccessRequestId.HasValue && tsk.IsGroupTask)
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.TaskId,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = string.Empty,
                                                 AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
                                                 AccessRequestID = acsrqst.AccessRequestId,
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


        public IList<TaskGridData> GetGroupTaskDetailsByActionUserIdAndTaskStatus(int actionUserId, StatusType taskStatus)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<int> userGroups = _context.Users.FirstOrDefault(item => item.UserId == actionUserId).Groups.Select(item => item.GroupId).ToList();

                    var attestationTasks = (from task in _context.Tasks
                                            join attestattion in _context.Attestations
                                                on task.AttestationId equals attestattion.AttestationId
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where (userGroups.Contains(task.ActionGroupId.Value) && task.Action == taskStatus && task.IsGroupTask)
                                            select new TaskGridData
                                            {
                                                TaskId = task.TaskId,
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
                                             on tsk.AccessRequestId equals acsrqst.AccessRequestId
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.UserId
                                             join assignedby in _context.Users
                                                    on tsk.AssignedBy equals assignedby.UserId into Adetail
                                             join assignedUser in _context.Users
                                               on tsk.AssignedTo equals assignedUser.UserId into detail
                                             from assignedUser in detail.DefaultIfEmpty()
                                             from assignedby in Adetail.DefaultIfEmpty()
                                             join wfConfig in _context.WorkflowConfigurations
                                             on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             where (userGroups.Contains(tsk.ActionGroupId.Value) && acsrqst.AdHocAccessRequestId.HasValue && tsk.Action == taskStatus && tsk.IsGroupTask)
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.TaskId,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = string.Empty,
                                                 AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
                                                 AccessRequestID = acsrqst.AccessRequestId,
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
                                on task.AttestationId equals attestattion.AttestationId
                            join wfConfig in _context.WorkflowConfigurations
                                on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                            where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                            where task.ActionUserId == actionUserId
                            select new TaskGridData
                            {
                                TaskId = task.TaskId,
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
                                on task.AttestationId equals attestattion.AttestationId
                            where (attestattion.AttestationId == paramAttestationId || paramAttestationId == -1)
                            join attestattionProcess in _context.AttestationProcesses
                                on task.AttestationId equals attestattionProcess.AttestationId
                            join wfConfig in _context.WorkflowConfigurations
                                on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                            join user in _context.Users
                                on task.ActionUserId equals user.UserId
                            join org in _context.Organisations
                                on user.OrganisationId.Value equals org.OrganisationId
                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                            where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId &&
                            wfConfigDetail.SequenceNumber == task.WorkflowStateId &&
                            task.CreatedDate >= attestattionProcess.ProcessStartAt
                            select new TaskReportData
                            {
                                TaskId = task.TaskId,
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
                        Task attachedTask = set.Find(task.TaskId);

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
                        ReviewEndpointSystem attachedreviewEP = set.Find(reviewEP.ReviewEndpointSystemId);

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
                        AttestationReviewEndpointSystem attachedreviewEP = set.Find(reviewEP.ReviewEndpointId);

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

        public TaskDelegation GetTaskDelegationByUserId(int userId, RequestType requestType)
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
                                    on item.DelegatedTo equals user.UserId
                                join user1 in _context.Users
                                    on item.ActionUser equals user1.UserId
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
                                    DelegationId = item.DelegationId,
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
                                on item.DelegatedTo equals user.UserId
                            join user1 in _context.Users
                                on item.ActionUser equals user1.UserId
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
                                DelegationId = item.DelegationId,
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

        public List<TaskDelegation> GetAllDelegationsByUserId(int userId)
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
                    return _context.TaskDelegations.FirstOrDefault(item => item.DelegationId == delegationId);
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
                        TaskDelegation attachedDelegation = set.Find(taskDelegation.DelegationId);

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
                                on task.ActionUserId equals user.UserId
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

                    return task.TaskId;
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
                        Task attachedTask = set.Find(task.TaskId);

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
                            AccessRequest attachedAccReq = set.Find(task.AccessRequest.AccessRequestId);

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
                        return _context.Tasks.FirstOrDefault(item => item.TaskId == taskId);
                    }
                    return (from item in _context.Tasks
                        .Include("AccessRequest")
                        .Include("AccessRequest.User")
                        .Include("AccessRequest.AdHocAccessRequest")
                        .Include("AccessRequest.AdHocAccessRequest.EndPoint")
                        .Include("SubTask")
                            where item.TaskId == taskId
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
                        AccessRequest attachedAccReq = set.Find(accessRequest.AccessRequestId);

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
                                                  on task.ActionUserId equals user.UserId
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.AttestationId
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                      task.IsActive &&
                                                      task.IsCompleted == false &&
                                                      task.ActionUserId == actionUserId
                                                select new TaskGridData
                                                {
                                                    TaskId = task.TaskId,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.UserId,
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
                                                 on tsk.AccessRequestId equals acsrqst.AccessRequestId
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                                 where tsk.CreatedDate >= fromDate &&
                                                     tsk.IsActive &&
                                                     tsk.IsCompleted == false &&
                                                     tsk.ActionUserId == actionUserId
                                                 select new TaskGridData
                                                 {
                                                     TaskId = tsk.TaskId,
                                                     RequestTypeId = tsk.RequestTypeId,
                                                     AttestationId = tsk.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = tsk.CreatedDate,
                                                     IsActive = tsk.IsActive,
                                                     IsCompleted = tsk.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
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
                                                  on task.ActionUserId equals user.UserId
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.AttestationId
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                     task.IsActive &&
                                                     task.IsCompleted == false
                                                select new TaskGridData
                                                {
                                                    TaskId = task.TaskId,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.UserId,
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
                                                 on tsk.AccessRequestId equals acsrqst.AccessRequestId
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                                 where tsk.CreatedDate >= fromDate &&
                                                     tsk.IsActive &&
                                                     tsk.IsCompleted == false
                                                 select new TaskGridData
                                                 {
                                                     TaskId = tsk.TaskId,
                                                     RequestTypeId = tsk.RequestTypeId,
                                                     AttestationId = tsk.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = tsk.CreatedDate,
                                                     IsActive = tsk.IsActive,
                                                     IsCompleted = tsk.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
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
                                                  on task.ActionUserId equals user.UserId
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.AttestationId
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                      task.IsActive &&
                                                      task.IsCompleted &&
                                                      task.ActionUserId == actionUserId
                                                select new TaskGridData
                                                {
                                                    TaskId = task.TaskId,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.UserId,
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
                                                 on task.AccessRequestId equals acsrqst.AccessRequestId
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                 where task.CreatedDate >= fromDate &&
                                                    task.IsActive &&
                                                    task.IsCompleted &&
                                                    task.ActionUserId == actionUserId
                                                 select new TaskGridData
                                                 {
                                                     TaskId = task.TaskId,
                                                     RequestTypeId = task.RequestTypeId,
                                                     AttestationId = task.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = task.CreatedDate,
                                                     IsActive = task.IsActive,
                                                     IsCompleted = task.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
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
                                                  on task.ActionUserId equals user.UserId
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.AttestationId
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                     task.IsActive &&
                                                     task.IsCompleted
                                                select new TaskGridData
                                                {
                                                    TaskId = task.TaskId,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.UserId,
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
                                                 on task.AccessRequestId equals acsrqst.AccessRequestId
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                 where task.CreatedDate >= fromDate &&
                                                    task.IsActive &&
                                                    task.IsCompleted
                                                 select new TaskGridData
                                                 {
                                                     TaskId = task.TaskId,
                                                     RequestTypeId = task.RequestTypeId,
                                                     AttestationId = task.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = task.CreatedDate,
                                                     IsActive = task.IsActive,
                                                     IsCompleted = task.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
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
                                                  on task.ActionUserId equals user.UserId
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.AttestationId
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                      task.IsActive == false &&
                                                      task.IsCompleted == false &&
                                                      task.ActionUserId == actionUserId
                                                select new TaskGridData
                                                {
                                                    TaskId = task.TaskId,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.UserId,
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
                                                 on task.AccessRequestId equals acsrqst.AccessRequestId
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                 where task.CreatedDate >= fromDate &&
                                                    task.IsActive == false &&
                                                    task.IsCompleted == false &&
                                                    task.ActionUserId == actionUserId
                                                 select new TaskGridData
                                                 {
                                                     TaskId = task.TaskId,
                                                     RequestTypeId = task.RequestTypeId,
                                                     AttestationId = task.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = task.CreatedDate,
                                                     IsActive = task.IsActive,
                                                     IsCompleted = task.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
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
                                                  on task.ActionUserId equals user.UserId
                                                join attestattion in _context.Attestations
                                                    on task.AttestationId equals attestattion.AttestationId
                                                join wfConfig in _context.WorkflowConfigurations
                                                    on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                where task.CreatedDate >= fromDate &&
                                                     task.IsActive == false &&
                                                     task.IsCompleted == false
                                                select new TaskGridData
                                                {
                                                    TaskId = task.TaskId,
                                                    RequestTypeId = task.RequestTypeId,
                                                    UserId = user.UserId,
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
                                                 on task.AccessRequestId equals acsrqst.AccessRequestId
                                                 join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                                 on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                                 join rqsteduser in _context.Users
                                                 on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                                 join wfConfig in _context.WorkflowConfigurations
                                                 on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                                 from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                                 where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                                 where task.CreatedDate >= fromDate &&
                                                    task.IsActive == false &&
                                                    task.IsCompleted == false
                                                 select new TaskGridData
                                                 {
                                                     TaskId = task.TaskId,
                                                     RequestTypeId = task.RequestTypeId,
                                                     AttestationId = task.AttestationId,
                                                     AttestationName = string.Empty,
                                                     EventName = adhocrqst.EndPoint.DisplayName + " - " + (((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? "Amendment" : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account")),
                                                     CreatedDate = task.CreatedDate,
                                                     IsActive = task.IsActive,
                                                     IsCompleted = task.IsCompleted,
                                                     ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                     UserName = string.Empty,
                                                     AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
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
                            where task.AttestationId == paramAttestationId && task.TaskId == (_iacmdbcontext.Tasks.Where(maxTsk => maxTsk.AttestationId == paramAttestationId).Select(max => max.TaskId).Max())
                            join taskReview in _iacmdbcontext.TaskReviews
                                on task.TaskId equals taskReview.TaskId
                            join attesReview in _iacmdbcontext.AttestationReviews
                                on taskReview.ReviewId equals attesReview.ReviewId
                            join ReP in _iacmdbcontext.ReviewEndpointSystems
                                on attesReview.ReviewId equals ReP.ReviewId
                            where ReP.TaskId == task.TaskId
                            join AReP in _iacmdbcontext.AttestationReviewEndpointSystems
                                on ReP.ReviewEndpointId equals AReP.ReviewEndpointId
                            join EPS in _iacmdbcontext.EndpointSystems
                                on AReP.EndpointSystemId equals EPS.EndpointSystemId
                            select new AttestationActionGridData
                            {
                                AttestationReviewId = attesReview.ReviewId,
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
                    AttestationReview aAttReview = _iacmdbcontext.AttestationReviews.FirstOrDefault(review => review.ReviewId == ipAttestationReviewAction.AttestationReviewId);
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
                        AttestationReview attachedTask = set.Find(attestationReview.ReviewId);

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
                    return _context.Tasks.FirstOrDefault(tsk => tsk.TaskId == taskId);
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
                    Task _task = _context.Tasks.Where(tsk => tsk.TaskId == task.TaskId).FirstOrDefault();
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
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Updating Task.");
                throw;
            }
        }

        public bool IsGroupTaskExistsForUser(int userId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<int> userGroups = _context.Users.FirstOrDefault(item => item.UserId == userId).Groups.Select(item => item.GroupId).ToList();
                    return _context.Tasks.Any(item => userGroups.Contains(item.ActionGroupId.Value) && item.IsActive && !item.IsCompleted && item.IsGroupTask);
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
                                    on tasks.AttestationId equals attestattion.AttestationId
                                join wfConfig in _context.WorkflowConfigurations
                                    on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                where tasks.TaskId == task.TaskId
                                select wfConfigDetail.ResponseTimeInDays).FirstOrDefault();
                    }
                    if (task.RequestTypeId == (byte)RequestType.Provisioning)
                    {
                        return (from tasks in _context.Tasks
                                join acsrqst in _context.AccessRequests
                                on tasks.AccessRequestId equals acsrqst.AccessRequestId
                                join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                join rqsteduser in _context.Users
                                on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                join wfConfig in _context.WorkflowConfigurations
                                on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                where tasks.TaskId == task.TaskId
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
                                                on task.AttestationId equals attestattion.AttestationId
                                            join actionuser in _context.Users
                                        on task.ActionUserId equals actionuser.UserId
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            where task.Action == taskStatus
                                            select new TaskGridData
                                            {
                                                TaskId = task.TaskId,
                                                RequestTypeId = task.RequestTypeId,
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                EventName = string.Empty,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                UserName = actionuser.DisplayName,
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
                                                RequestReason = string.Empty
                                            });

                    var provisioningTasks = (from tsk in _context.Tasks
                                             join acsrqst in _context.AccessRequests
                                             on tsk.AccessRequestId equals acsrqst.AccessRequestId
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.UserId
                                             join actionuser in _context.Users
                                               on tsk.ActionUserId equals actionuser.UserId
                                             join wfConfig in _context.WorkflowConfigurations
                                           on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             where tsk.Action == taskStatus
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.TaskId,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = actionuser.DisplayName,
                                                 AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
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
                                                 AccessRequestID = acsrqst.AccessRequestId,
                                                 Type = ((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                                 StatusType = tsk.Action,
                                                 RequestReason = acsrqst.Comments
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
                                                on task.AttestationId equals attestattion.AttestationId
                                            join actionuser in _context.Users
                                        on task.ActionUserId equals actionuser.UserId
                                            join wfConfig in _context.WorkflowConfigurations
                                                on attestattion.WorkflowConfigurationId equals wfConfig.WorkflowConfigurationId
                                            from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                            where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == task.WorkflowStateId
                                            select new TaskGridData
                                            {
                                                TaskId = task.TaskId,
                                                RequestTypeId = task.RequestTypeId,
                                                AttestationId = task.AttestationId,
                                                AttestationName = attestattion.AttestationName,
                                                EventName = string.Empty,
                                                CreatedDate = task.CreatedDate,
                                                IsActive = task.IsActive,
                                                IsCompleted = task.IsCompleted,
                                                ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                UserName = actionuser.DisplayName,
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
                                                RequestReason = string.Empty
                                            });

                    var provisioningTasks = (from tsk in _context.Tasks
                                             join acsrqst in _context.AccessRequests
                                             on tsk.AccessRequestId equals acsrqst.AccessRequestId
                                             join adhocrqst in _context.AdHocAccessRequests.Include("EndPoint")
                                             on acsrqst.AdHocAccessRequestId equals adhocrqst.AdHocAccessRequestId
                                             join rqsteduser in _context.Users
                                             on adhocrqst.RequesterUserId equals rqsteduser.UserId
                                             join rqsteruser in _context.Users
                                                on adhocrqst.RequesteeUserId equals rqsteruser.UserId
                                             join actionuser in _context.Users
                                               on tsk.ActionUserId equals actionuser.UserId
                                             join wfConfig in _context.WorkflowConfigurations
                                             on adhocrqst.EndpointSystemId equals wfConfig.EndpointSystemId
                                             from wfConfigDetail in wfConfig.WorkflowConfigurationDetails
                                             where wfConfig.WorkflowConfigurationId == wfConfigDetail.WorkflowConfigurationId && wfConfigDetail.SequenceNumber == tsk.WorkflowStateId
                                             select new TaskGridData
                                             {
                                                 TaskId = tsk.TaskId,
                                                 RequestTypeId = tsk.RequestTypeId,
                                                 AttestationId = tsk.AttestationId,
                                                 AttestationName = string.Empty,
                                                 EventName = adhocrqst.EndPoint.DisplayName,
                                                 CreatedDate = tsk.CreatedDate,
                                                 IsActive = tsk.IsActive,
                                                 IsCompleted = tsk.IsCompleted,
                                                 ReponseTimeInDays = wfConfigDetail.ResponseTimeInDays,
                                                 UserName = actionuser.DisplayName,
                                                 AdHocAccessRequestId = adhocrqst.AdHocAccessRequestId,
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
                                                 AccessRequestID = acsrqst.AccessRequestId,
                                                 Type = ((int)adhocrqst.AdhocActionType == 2 || (int)adhocrqst.AdhocActionType == 3) ? ((int)adhocrqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)adhocrqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                                 StatusType = tsk.Action,
                                                 RequestReason = acsrqst.Comments
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

        public List<int> GetActionUsersByRequestState(long accessRequestId, byte stateId)
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


        public List<TaskDelegation> GetDelegationsByActionUserID(int ActionUserID)
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
                var delegateDetail = _context.TaskDelegations.Where(item => item.DelegationId == DelegationId).Select(task => task).FirstOrDefault();
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

    }
}
