
using DI.Chakra.DomainModel.Presentation;
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
    public class TaskDelegation : DomainModelBase<TaskDelegation, EntityModel.TaskDelegation>
    {
        public long ActionUser { get; set; }

        public long DelegatedTo { get; set; }

        public Nullable<DateTime> StartDate { get; set; }

        public Nullable<DateTime> EndDate { get; set; }

        public bool DelegateAttestationTask { get; set; }

        public bool DelegateProvisioningTask { get; set; }

        public bool DelegateSODTask { get; set; }

        public bool IsActive { get; set; }

        public string DelegationComments { get; set; }
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public User DelegatedToUser { get; set; }

        //public string DelegateToUserName
        //{
        //    get
        //    {
        //        return this.DelegatedToUser.DisplayName;
        //    }
        //}

        public static List<TaskDelegation> GetActiveActionUser(long actionUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == actionUser && td.IsActive).ToList());
            }
        }
        public static bool IsDelegationAllowed(RequestType requestType, long actionUser)
        {
            bool allowDelegate = true;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                DateTime today = DateTime.Now.Date;
                var taskDelegation = new List<TaskDelegation>();
                if (requestType == RequestType.Provisioning)
                {
                    taskDelegation = ORMapList(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == actionUser && td.IsActive && td.StartDate <= today &&
                    td.EndDate >= today && td.DelegateProvisioningTask).ToList());
                    if(taskDelegation != null && taskDelegation.Count > 0)
                    {
                        allowDelegate = false;
                    }
                }
                if (requestType == RequestType.Attestation)
                {
                    taskDelegation = ORMapList(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == actionUser && td.IsActive && td.StartDate <= today &&
                    td.EndDate >= today && td.DelegateAttestationTask).ToList());
                    if (taskDelegation != null && taskDelegation.Count > 0)
                    {
                        allowDelegate = false;
                    }
                }
            }
            return allowDelegate;
        }
        public static TaskDelegation GetAdvanceDelegationByActionUser(RequestType requestType, long actionUser,Nullable<DateTime> startDate=null, Nullable<DateTime> endDate = null)
        {
            TaskDelegation delegation = null;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                DateTime today = DateTime.Now.Date;
                var taskDelegation = new List<TaskDelegation>();

                if (requestType == RequestType.Provisioning)
                {
                    if(!startDate.HasValue && !endDate.HasValue)
                    {
                       taskDelegation = ORMapList(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == actionUser && td.IsActive && td.StartDate <= today &&
                       td.EndDate >= today && td.DelegateProvisioningTask).ToList());
                        if (taskDelegation != null && taskDelegation.Count > 0)
                        {

                            delegation = TaskDelegation.GetById(taskDelegation.FirstOrDefault().Id, true);
                        }
                    }
                    else
                    {
                        taskDelegation = ORMapList(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == actionUser && td.IsActive && 
                       td.EndDate >= today && td.DelegateProvisioningTask).ToList());
                        taskDelegation.Add(new TaskDelegation() { Id=0, StartDate = startDate, EndDate = endDate });
                        var exists =
                                    (
                                        from e1 in taskDelegation
                                        where taskDelegation
                                            .Where(e2 => e1.Id != e2.Id)
                                            .Where(e2 => e1.StartDate <= e2.EndDate)
                                            .Where(e2 => e1.EndDate >= e2.StartDate)
                                            .Any()
                                        select e1
                                    ).FirstOrDefault();

                        if (exists != null && exists.Id > 0)
                        {
                            delegation = TaskDelegation.GetById(exists.Id, true);
                        }
                    }
                }
                if (requestType == RequestType.Attestation)
                {
                    if (!startDate.HasValue && !endDate.HasValue)
                    {
                        taskDelegation = ORMapList(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == actionUser && td.IsActive && td.StartDate <= today &&
                        td.EndDate >= today && td.DelegateAttestationTask).ToList());
                        if (taskDelegation != null && taskDelegation.Count > 0)
                        {

                            delegation = TaskDelegation.GetById(taskDelegation.FirstOrDefault().Id, true);
                        }
                    }
                    else
                    {
                        taskDelegation = ORMapList(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == actionUser && td.IsActive &&
                       td.EndDate >= today && td.DelegateAttestationTask).ToList());
                        taskDelegation.Add(new TaskDelegation() { Id = 0, StartDate = startDate, EndDate = endDate });
                        var exists =
                                    (
                                        from e1 in taskDelegation
                                        where taskDelegation
                                            .Where(e2 => e1.Id != e2.Id)
                                            .Where(e2 => e1.StartDate <= e2.EndDate)
                                            .Where(e2 => e1.EndDate >= e2.StartDate)
                                            .Any()
                                        select e1
                                    ).FirstOrDefault();

                        if (exists != null && exists.Id > 0)
                        {
                            delegation = TaskDelegation.GetById(exists.Id, true);
                        }
                    }
                }
            }
            return delegation;
        }

        public static List<TaskDelegation> GetByActionUser(long actionUser,Nullable<bool> isAdmin)
        {
            List<TaskDelegation> delegations = new List<TaskDelegation>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                if(isAdmin == null || isAdmin == false)
                    delegations = ORMapList(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == actionUser, td => td.DelegatedToUser).Select(task => task).Where(t => t.StartDate != null).ToList());
                else
                {
                    delegations = ORMapList(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.Id > 0, td => td.DelegatedToUser).Select(task => task).Where(t => t.StartDate != null).ToList());
                }
            }

            return delegations;
        }
        
        public static TaskDelegation GetByAllProperty(long actionUserId, long delegatedTo, DateTime startDate, DateTime endDate, bool isAttestationTask, bool isProvisioningTask, string comments)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == actionUserId && td.DelegatedTo == delegatedTo &&
                td.IsActive && td.StartDate == startDate && td.EndDate == endDate && (td.DelegateProvisioningTask == isProvisioningTask || td.DelegateAttestationTask == isAttestationTask) &&
                td.DelegationComments == comments).FirstOrDefault());
            }
        }

        public static TaskDelegation GetByRequestType(RequestType requestType, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                DateTime today = DateTime.Now.Date;

                if (requestType == RequestType.Attestation)
                {
                    return ORMap(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == userId && td.IsActive && td.StartDate <= today &&
                    td.EndDate >= today && td.DelegateAttestationTask).FirstOrDefault());
                }
                if (requestType == RequestType.Provisioning)
                {
                    return ORMap(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == userId && td.IsActive && td.StartDate <= today &&
                    td.EndDate >= today && td.DelegateProvisioningTask).FirstOrDefault());
                }
                if (requestType == RequestType.Compliance)
                {
                    return ORMap(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == userId && td.IsActive && td.StartDate <= today &&
                    td.EndDate >= today && td.DelegateSODTask).FirstOrDefault());
                }
                return null;
            }
        }

        public static TaskDelegation GetByIdIncludeUser(long Id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.TaskDelegation>(transaction).GetById(Id, user => user.DelegatedToUser, user => user.DelegatedToUser.NOrganisation, user => user.DelegatedToUser.NBusinessRole));
            }
        }
        public static void CreateSelectedTasksDelegation(TaskDelegation taskDelegation, IList<long> selectedtaskList, string user)
        {
            string commentFormat = "{0}<br/> Date - {1} By - {3}, From - {2}, To - {4}, Comments - {5}";
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(user))
            {
                foreach (var lq in selectedtaskList)
                {
                    long? actionUserId = null;
                    var task = Task.GetById(lq);
                    if (task != null)
                    {
                        actionUserId = task.ActionUserId;

                        long tmpActionUserId = 0;

                        if (task.ActionUserId.HasValue)
                            tmpActionUserId = task.ActionUserId.Value;

                        task.ActionUserId = taskDelegation.DelegatedTo;

                        List<TaskDelegation> delegations = new List<TaskDelegation>();
                        TaskDelegation delegation = null;

                        task.LastModifiedBy = taskDelegation.CreatedBy;
                        task.IsDelegated = true;
                        task.DelegatedBy = taskDelegation.CreatedBy;
                        task.DelegatedDate = DateTime.Now;
                        //task.DelegatedFrom = taskDelegation.ActionUser;
                        //task.DelegationComments = taskDelegation.DelegationComments;
                        User userTo = null, userFrom = null;
                        var fromUserName = "-";
                        if (task.IsGroupTask)
                        {
                            if (tmpActionUserId == 0 && task.AssignedTo.HasValue)
                            {
                                tmpActionUserId = task.AssignedTo.Value;
                                userFrom = User.GetById(task.AssignedTo.Value);
                                task.DelegatedFrom = userFrom.Id;
                            }
                            else
                            {
                                userFrom = User.GetById(tmpActionUserId);
                                if(userFrom != null)
                                    task.DelegatedFrom = tmpActionUserId;
                            }

                            if(userFrom == null&& task.ActionGroupId.HasValue)
                            {
                                task.DelegatedFrom = null;
                                //fromUserName = Group.GetById(task.ActionGroupId.Value)?.GroupName;
                            }
                            task.AssignedTo = task.ActionUserId;
                        }
                        else
                        {
                            if (actionUserId.HasValue)
                            {
                                userFrom = User.GetById(actionUserId.Value);
                                task.DelegatedFrom = userFrom.Id;
                            }
                            else
                            {
                                if (task.DelegatedFrom.HasValue)
                                    userFrom = User.GetById(task.DelegatedFrom.Value);
                            }
                        }
                        if(userFrom != null)
                        {
                            fromUserName = userFrom.DisplayName;
                        }
                        //if (task.DelegatedFrom.HasValue)
                        //    userFrom = User.GetById(task.DelegatedFrom.Value);
                        var delegatedBy = task.DelegatedBy;
                        if (task.ActionUserId.HasValue)
                            userTo = User.GetById(task.ActionUserId.Value);
                        task.DelegationComments = string.Format(commentFormat, task.DelegationComments, task.DelegatedDate.Value.ToString("dd-MM-yyyy hh:mm tt"), fromUserName, delegatedBy, userTo?.DisplayName, taskDelegation?.DelegationComments);
                        task.Save(dataAccessTransaction);

                        User tmpUserObj = User.GetById(tmpActionUserId);
                        var workFlowConfiguration = WorkflowConfiguration.GetById(task.WorkflowConfigurationId, true);
                        var approver = workFlowConfiguration.WorkflowConfigurationDetails.Where(w => w.SequenceNumber == task.WorkflowStateId).FirstOrDefault();
                        ApproverDelegationHistory approverHistory = new ApproverDelegationHistory();
                        approverHistory.AccessRequestId = task.AccessRequestId.HasValue ? task.AccessRequestId.Value : 0;
                        approverHistory.ApproverName = tmpUserObj != null ? Convert.ToString(tmpUserObj.DisplayName) : string.Empty;
                        approverHistory.Level = task.WorkflowStateId.ToString();
                        approverHistory.Remarks = taskDelegation.DelegationComments;
                        approverHistory.ApproverRole = task.IsGroupTask ? string.Format("Group - {0}", task.GroupName) : GetApprover(approver);
                        approverHistory.DelegatedDateTime = DateTime.Now.Subtract(new TimeSpan(0, 1, 0));
                        approverHistory.TaskId = task.Id;
                        approverHistory.IsActionOnlyRequired = task.IsActionOnlyRequiredTask;
                        approverHistory.Save(dataAccessTransaction);

                        // check for further delegations
                        if (task.AttestationId != null)
                        {
                            delegation = GetFinalDelegatedToApprover(RequestType.Attestation, taskDelegation.DelegatedTo, delegations);
                        }
                        else
                        {
                            delegation = GetFinalDelegatedToApprover(RequestType.Provisioning, taskDelegation.DelegatedTo, delegations);
                        }
                        
                        if (delegation != null)
                        {
                            if (task.IsGroupTask && task.AssignedTo != null)
                            {
                                if (tmpActionUserId == 0) tmpActionUserId = task.AssignedTo.Value;
                                task.AssignedTo = task.ActionUserId;
                            }
                            


                            if (task.ActionUserId.HasValue)
                                tmpActionUserId = task.ActionUserId.Value;
                            task.ActionUserId = delegation.DelegatedTo;
                            task.IsDelegated = true;
                            task.DelegatedDate = DateTime.Now;
                            task.DelegatedFrom = delegation.ActionUser;

                            delegatedBy = string.Empty;
                            userTo = null; userFrom = null;
                            if (task.DelegatedFrom.HasValue)
                                userFrom = User.GetById(task.DelegatedFrom.Value);
                            delegatedBy = task.DelegatedBy;
                            if (task.ActionUserId.HasValue)
                                userTo = User.GetById(task.ActionUserId.Value);
                            task.DelegationComments = string.Format(commentFormat, task.DelegationComments, task.DelegatedDate.ToString(), userFrom?.DisplayName, delegatedBy, userTo?.DisplayName, taskDelegation?.DelegationComments);
                            task.DelegatedBy = delegation.CreatedBy;
                            
                            task.Save(dataAccessTransaction);

                            User tmpUserObj1 = User.GetById(tmpActionUserId);
                            ApproverDelegationHistory approverHistory1 = new ApproverDelegationHistory();
                            approverHistory1.AccessRequestId = task.AccessRequestId.HasValue ? task.AccessRequestId.Value : 0;
                            approverHistory1.ApproverName = tmpUserObj1 != null ? Convert.ToString(tmpUserObj1.DisplayName) : string.Empty;
                            approverHistory1.Level = task.WorkflowStateId.ToString();
                            approverHistory1.Remarks = delegation.DelegationComments;
                            approverHistory1.ApproverRole = task.IsGroupTask ? string.Format("Group - {0}", task.GroupName) : GetApprover(approver);
                            approverHistory1.DelegatedDateTime = DateTime.Now.Subtract(new TimeSpan(0, 1, 0));
                            approverHistory1.TaskId = task.Id;
                            approverHistory1.IsActionOnlyRequired = task.IsActionOnlyRequiredTask;
                            approverHistory1.Save(dataAccessTransaction);
                        }
                        try
                        {
                            //update the action user details in the snapshot
                            if (task.AttestationId.HasValue && task.AttestationCampaignId.HasValue && task.ActionUserId.HasValue)
                            {
                                AttestationSnapshotAction.UpdateReviewerDetail((long)task.AttestationId, (long)task.AttestationCampaignId, task.Id, (long)task.ActionUserId, user);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }
                taskDelegation.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();


            }
        }
        private static string GetApprover(WorkflowConfigurationDetail currentApprover)
        {
            string approverType = string.Empty;

            if (currentApprover.ApprovalUserType == ApprovalUserType.User)
            {
                approverType = "User";
            }
            else if (currentApprover.ApprovalUserType == ApprovalUserType.Group)
            {
                string groupName = Group.GetById(currentApprover.Approver).GroupName;
                approverType = "Group - " + groupName;
            }
            else if (currentApprover.ApprovalUserType == ApprovalUserType.Role)
            {
                var approver = RoleTypes.GetById(currentApprover.Approver);

                bool isApproverFromOrgScope = NOrganisationScope.GetAll().Where(t => t.Scope == approver.RoleTypeDesc).Any();

                if (isApproverFromOrgScope)
                {
                    approverType = RoleTypes.GetAll().Where(t => t.RoleTypeDesc == approver.RoleTypeDesc).Select(a => a.DisplayName).FirstOrDefault();

                }
                else
                {
                    approverType = UtilityMethods.GetEnumAttribute<Description>((RoleType)currentApprover.Approver);
                }
            }
            else if (currentApprover.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
            {
                approverType = UtilityMethods.GetEnumAttribute<Description>((RoleType)currentApprover.Approver);
            }
            
            return approverType;
        }

        public static TaskDelegation GetFinalDelegatedToApprover(RequestType requestType, long userId, List<TaskDelegation> delegations)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                DateTime today = DateTime.Today;
                TaskDelegation delegation = null;

                // check whether a delegation is configured for the specific Task Type
                if (requestType == RequestType.Attestation)
                {
                    delegation = ORMap(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == userId && td.IsActive && td.StartDate <= today &&
                    td.EndDate >= today && td.DelegateAttestationTask).FirstOrDefault());
                }
                else if (requestType == RequestType.Provisioning)
                {
                    delegation = ORMap(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == userId && td.IsActive && td.StartDate <= today &&
                    td.EndDate >= today && td.DelegateProvisioningTask).FirstOrDefault());
                }
                else if (requestType == RequestType.Compliance)
                {
                    delegation = ORMap(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.ActionUser == userId && td.IsActive && td.StartDate <= today &&
                    td.EndDate >= today && td.DelegateSODTask).FirstOrDefault());
                }

                // if there is a delegation to an actual user 
                // AND that user is not already in the previous delegations (to avoid circular reference)
                if (delegation != null && delegation.DelegatedTo > 0 && !delegations.Any(x => x.ActionUser == delegation.DelegatedTo))
                {
                    delegations.Add(delegation);
                    GetFinalDelegatedToApprover(requestType, delegation.DelegatedTo, delegations);
                }

                if (delegations.Count > 0)
                    return delegations.LastOrDefault();

                return null;
            }
        }

        public static TaskDelegation GetByUserId(long userId, RequestType requestType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMap(new TaskDelegationRepository(transaction).GetByUserId(userId, requestType));
            }
        }

        public static string DeactivateAdvancedDelegation(string modifiedby = null)
        {
            string message = Constants.DelegationNoRecords;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    if (string.IsNullOrWhiteSpace(modifiedby))
                        modifiedby = MasterConfigHelper.ServiceUser;

                    var delegations = ORMapList(new RepositoryBase<EntityModel.TaskDelegation>(dataAccessTransaction).Get(td => td.IsActive && (td.EndDate == null || td.EndDate < DateTime.Now)).ToList());
                    if (delegations != null && delegations.Count > 0)
                    {
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"Deactivate Advance Delegation. DelegationId:{string.Join(" | ", delegations.Select(i => i.Id))}");
                        foreach (var delegation in delegations)
                        {
                            delegation.IsActive = false;
                            delegation.Save(modifiedby);
                        }
                        message = Constants.DelegationExpiredMessage;
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, message);
                    }
                }
            }
            catch (Exception ex)
            {
                message = Constants.DelegationExpiredfailedMessage;
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Info, $"{message}.{ex.Message} {ex.StackTrace}", ex);
            }
            return message;
        }
    }
}
