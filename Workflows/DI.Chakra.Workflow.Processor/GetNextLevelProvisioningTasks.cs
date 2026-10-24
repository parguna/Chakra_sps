
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
    class GetNextLevelProvisioningTasks
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();
        private List<long> delgateUsers = new List<long>();

        public List<Task> ExecuteForAdhocRequest(Task existingTask, WorkflowConfigurationDetail currentApprover, ref bool isSkipNextLevel)
        {
            try
            {
                List<Task> nextLevelTasks = new List<Task>();

                List<AccessRequestReviewInfo> reviewInfos = GetProvisioningReviewersInformation(existingTask, currentApprover);
                var secGroup = Group.GetById(MasterConfigHelper.ITSecurityGroupId);
                isSkipNextLevel = reviewInfos.Count == 0;

                if (reviewInfos.Count > 0)
                {
                    foreach (AccessRequestReviewInfo reviewInfo in reviewInfos)
                    {
                        Task newTask = new Task()
                        {
                            AccessRequestId = existingTask.AccessRequestId,
                            IsActive = true,
                            RequestTypeId = (byte)RequestType.Provisioning,
                            WorkflowStateId = currentApprover.SequenceNumber,
                            WorkflowInstanceId = existingTask.WorkflowInstanceId,
                            WorkflowConfigurationId = existingTask.WorkflowConfigurationId,
                            Action = StatusType.Pending,
                            AttestationCampaignId = existingTask.AttestationCampaignId,
                            IsGroupTask = reviewInfo.IsGroup,
                            IsActionOnlyRequiredTask = currentApprover.IsActionRequired,
                            Remarks = reviewInfo.Remarks,
                        };

                        newTask.Approver = utility.GetApprover(currentApprover);

                        if (newTask.IsGroupTask)
                        {
                            if (!string.IsNullOrEmpty(reviewInfo.GroupName))
                            {
                                newTask.GroupName = reviewInfo.GroupName;
                            }
                            else
                            {
                                var group = Group.GetById(reviewInfo.GroupId);
                                newTask.GroupName = group != null ? group.GroupName : "";
                            }
                            newTask.ActionGroupId = reviewInfo.ReviewerId;
                            if (reviewInfo.ReviewerId > 0)
                            {
                                AssignGroupId(newTask.ActionGroupId.Value, newTask);
                            }

                        }
                        else
                        {
                            newTask.ActionUserId = reviewInfo.ReviewerId;
                        }

                        newTask.SubTask = reviewInfo.SubTaskInfo;

                        if (!newTask.IsGroupTask)
                        {
                            var delegation = TaskDelegation.GetByRequestType(RequestType.Provisioning, reviewInfo.ReviewerId);
                            if (delegation != null)
                            {
                                delgateUsers.Clear();
                                newTask.ActionUserId = GetReviewerbyDelegation(delegation.DelegatedTo);
                                newTask.IsDelegated = true;
                                newTask.DelegatedBy = "System";
                                newTask.DelegatedDate = DateTime.Now;
                                newTask.DelegatedFrom = delgateUsers.Count > 1 ? delgateUsers[delgateUsers.Count - 2] : reviewInfo.ReviewerId;
                                newTask.DelegationComments = delegation.DelegationComments;
                            }
                        }

                        newTask.Save(MasterConfigHelper.ServiceUser);

                        nextLevelTasks.Add(newTask);
                    }
                }
                else
                {
                    existingTask.WorkflowStateId = currentApprover.SequenceNumber;
                    //nextLevelTasks.Add(existingTask);
                }

                return nextLevelTasks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Task> ExecuteForBulkRequest(Task existingTask, WorkflowConfigurationDetail currentApprover, ref bool isSkipNextLevel)
        {
            try
            {
                List<Task> nextLevelTasks = new List<Task>();

                List<AccessRequestReviewInfo> reviewInfos = GetProvisioningReviewersInformation(existingTask, currentApprover);
                var secGroup = Group.GetById(MasterConfigHelper.ITSecurityGroupId);
                isSkipNextLevel = reviewInfos.Count == 0;

                if (reviewInfos.Count > 0)
                {
                    foreach (AccessRequestReviewInfo reviewInfo in reviewInfos)
                    {
                        Task newTask = new Task()
                        {
                            BulkRequestId = existingTask.BulkRequestId,
                            IsActive = true,
                            RequestTypeId = (byte)RequestType.BulkRequest,
                            WorkflowStateId = currentApprover.SequenceNumber,
                            WorkflowInstanceId = existingTask.WorkflowInstanceId,
                            WorkflowConfigurationId = existingTask.WorkflowConfigurationId,
                            Action = StatusType.Pending,

                            IsGroupTask = reviewInfo.IsGroup && !currentApprover.IsMockGroup,
                            IsActionOnlyRequiredTask = currentApprover.IsActionRequired,
                            Remarks = reviewInfo.Remarks,
                        };

                        newTask.Approver = utility.GetApprover(currentApprover);

                        if (newTask.IsGroupTask)
                        {
                            if (!string.IsNullOrEmpty(reviewInfo.GroupName))
                            {
                                newTask.GroupName = reviewInfo.GroupName;
                            }
                            else
                            {
                                var group = Group.GetById(reviewInfo.GroupId);

                                newTask.GroupName = group != null ? group.GroupName : "";
                            }
                            newTask.ActionGroupId = reviewInfo.ReviewerId;
                            if (reviewInfo.ReviewerId > 0)
                            {
                                AssignGroupId(newTask.ActionGroupId.Value, newTask);
                            }
                        }
                        else
                        {
                            newTask.ActionUserId = reviewInfo.ReviewerId;
                        }

                        newTask.SubTask = reviewInfo.SubTaskInfo;

                        if (!newTask.IsGroupTask)
                        {
                            var delegation = TaskDelegation.GetByRequestType(RequestType.Provisioning, reviewInfo.ReviewerId);
                            if (delegation != null)
                            {
                                newTask.ActionUserId = GetReviewerbyDelegation(delegation.DelegatedTo);
                                newTask.IsDelegated = true;
                                newTask.DelegatedBy = "System";
                                newTask.DelegatedDate = DateTime.Now;
                                newTask.DelegatedFrom = delgateUsers.Count > 1 ? delgateUsers[delgateUsers.Count - 2] : reviewInfo.ReviewerId;
                                newTask.DelegationComments = delegation.DelegationComments;
                            }
                        }

                        newTask.Save(MasterConfigHelper.ServiceUser);

                        nextLevelTasks.Add(newTask);
                    }
                }
                else
                {
                    existingTask.WorkflowStateId = currentApprover.SequenceNumber;
                    //nextLevelTasks.Add(existingTask);
                }

                return nextLevelTasks;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, "ExecuteForBulkRequest", ex);
                throw ex;
            }
        }
        /// <summary>
        /// Orgname 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        AccessRequestReviewInfo GetAccessRequestReviewInfo(long userId, string orgCode)
        {
            AccessRequestReviewInfo reviewInfo = null;
            if (string.IsNullOrWhiteSpace(orgCode))
                return reviewInfo;

            var nOrg = NOrganisation.GetNOrganisationByCode(orgCode);
            if (nOrg != null)
            {
                var nOrgHead = NOrganisationHead.GetByOrganisationId(nOrg.Id);
                if (nOrgHead == null || !nOrgHead.HeadId.HasValue)
                    return reviewInfo;

                var head = User.GetById(nOrgHead.HeadId.Value);
                if (head != null && head.IsActive)
                {
                    reviewInfo = new AccessRequestReviewInfo();
                    reviewInfo.ReviewerId = head.Id;
                    reviewInfo.IsGroup = false;
                }
            }
            return reviewInfo;
        }
        List<AccessRequestReviewInfo> GetGroupReviewers(RoleType roleType, long requestId, bool isbulkRequest)
        {
            var groupId = MasterConfigHelper.ITSecurityGroupId;
            List<AccessRequestReviewInfo> lstGroupReviewers = new List<AccessRequestReviewInfo>();

            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
            reviewInfo.ReviewerId = groupId;
            reviewInfo.GroupId = groupId;
            reviewInfo.IsGroup = true;
            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

            if (isbulkRequest)
                reviewInfo.Remarks = string.Format("The bulk request {0} is routed to IAM since the {1} is NOT available.", requestId, Convert.ToString(roleType));
            else
                reviewInfo.Remarks = string.Format("The request {0} is routed to IAM since the {1} is NOT available.", requestId, Convert.ToString(roleType));

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

        public List<AccessRequestReviewInfo> GetProvisioningReviewersInformation(Task existingTask, WorkflowConfigurationDetail currentApprover)
        {
            try
            {
                List<AccessRequestReviewInfo> reviewers = new List<AccessRequestReviewInfo>();

                AdHocAccessRequest adhocRequest = null;
                BulkRequest bulkRequest = null;

                var requestType = (RequestType)existingTask.RequestTypeId;
                AccessRequestStatus requestStatus = AccessRequestStatus.None;
                EndPoint endpoint = null;
                long requestId = 0;
                long userId = 0;
                long supervisorId = 0;
                var displayName = string.Empty;
                if (requestType == RequestType.Provisioning)
                {
                    adhocRequest = existingTask.AccessRequest.AdHocAccessRequest;
                    requestStatus = existingTask.AccessRequest.RequestStatus;
                    endpoint = adhocRequest.EndPoint;
                    displayName = endpoint?.DisplayName;
                    requestId = existingTask.AccessRequestId.Value;
                    userId = existingTask.AccessRequest.UserId;

                    if (adhocRequest.SupervisorChanged)
                    {
                        supervisorId = adhocRequest.SelectedSupervisor.HasValue ? adhocRequest.SelectedSupervisor.Value : 0;
                    }
                }
                else if (requestType == RequestType.BulkRequest)
                {
                    bulkRequest = existingTask.BulkRequest;
                    requestStatus = existingTask.BulkRequest.RequestStatus;
                    endpoint = bulkRequest.EndPoint;
                    displayName = endpoint?.DisplayName;
                    requestId = existingTask.BulkRequestId.Value;
                    userId = existingTask.BulkRequest.BulkRequestDetails.FirstOrDefault().UserId;

                    if (bulkRequest.SupervisorChanged)
                    {
                        supervisorId = bulkRequest.SelectedSupervisor.HasValue ? bulkRequest.SelectedSupervisor.Value : 0;
                    }
                }

                //look for role based approval
                if (currentApprover.ApprovalUserType == ApprovalUserType.Role)
                {
                    var user = User.GetById(userId);

                    if (currentApprover.Approver == (byte)RoleType.RO)
                    {
                        long reviewer = 0;

                        if (supervisorId != 0)
                        {
                            reviewer = supervisorId;
                        }
                        else
                        {
                            var ro = User.GetROByUser(user.Id);

                            if (ro != null)
                            {
                                reviewer = ro.Id;
                            }
                        }

                        if (reviewer != 0)
                        {
                            var delegation = TaskDelegation.GetByRequestType(RequestType.Provisioning, reviewer);
                            if (delegation != null)
                            {
                                bool isApproverInRequestFor = false;

                                if (requestType == RequestType.Provisioning)
                                {
                                    isApproverInRequestFor = delegation.DelegatedTo == adhocRequest.RequesteeUserId;
                                }
                                else if (requestType == RequestType.BulkRequest)
                                {
                                    isApproverInRequestFor = bulkRequest.BulkRequestDetails.Any(item => item.UserId == delegation.DelegatedTo);
                                }

                                if (isApproverInRequestFor)
                                {
                                    string exceptionRemarks = string.Empty;
                                    if (requestType == RequestType.Provisioning)
                                    {
                                        exceptionRemarks = string.Format("The request {0} is routed to IT security team since the approvers cannot approve their own request.", requestId);
                                    }
                                    else if (requestType == RequestType.BulkRequest)
                                    {
                                        exceptionRemarks = string.Format("The bulk request {0} is routed to IT security team since the approvers cannot approve their own request.", requestId);
                                    }

                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                                    reviewInfo.AccessRequestId = requestId;
                                    reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.Remarks = exceptionRemarks;
                                    reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                    reviewers.Add(reviewInfo);

                                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);
                                }
                                else
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.AccessRequestId = requestId;
                                    reviewInfo.ReviewerId = reviewer;
                                    reviewInfo.IsGroup = false;
                                    reviewers.Add(reviewInfo);
                                }
                            }
                            else
                            {
                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.AccessRequestId = requestId;
                                reviewInfo.ReviewerId = reviewer;
                                reviewInfo.IsGroup = false;
                                reviewers.Add(reviewInfo);
                            }
                        }
                        else
                        {
                            string exceptionRemarks = string.Format("The request {0} is routed to IT security team since the user {1}-{2} is not assigned to any RO.", requestId, user.DisplayName, user.UserName);

                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.Remarks = exceptionRemarks;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                            reviewers.Add(reviewInfo);

                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);
                        }
                    }

                    else if (currentApprover.Approver == (byte)RoleType.CO)
                    {
                        var co = User.GetCOByUser(user.Id);

                        if (co != null && co.IsActive)
                        {
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.ReviewerId = co.Id;
                            reviewInfo.IsGroup = false;
                            reviewers.Add(reviewInfo);
                        }
                        else
                        {
                            string exceptionRemarks = string.Format("The request {0} is routed to IAM team since the application {1} has no CO or CO is inactive.", requestId, displayName);
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.Remarks = exceptionRemarks;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                            reviewers.Add(reviewInfo);

                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);
                        }
                    }
                    else if (currentApprover.Approver == (byte)RoleType.ApplicationOwner)
                    {
                        long reviewer = 0;
                        if (requestType == RequestType.BulkRequest)
                        {
                            if (bulkRequest.EndpointSystemId > 0)
                            {
                                if (bulkRequest.EndPoint.OwnerUserId.HasValue)
                                {
                                    reviewer = bulkRequest.EndPoint.OwnerUserId.Value;
                                }
                            }
                        }
                        else
                        {
                            if (adhocRequest.EndpointSystemId.HasValue)
                            {
                                if (adhocRequest.EndPoint.OwnerUserId.HasValue)
                                {
                                    reviewer = adhocRequest.EndPoint.OwnerUserId.Value;
                                }
                            }
                        }


                        if (reviewer != 0)
                        {

                            User owner = User.GetById(reviewer);

                            if (owner != null && owner.IsActive)
                            {
                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.AccessRequestId = requestId;
                                reviewInfo.ReviewerId = reviewer;
                                reviewInfo.IsGroup = false;
                                reviewers.Add(reviewInfo);
                            }
                            else
                            {
                                string exceptionRemarks = string.Format("The request {0} is routed to IAM team since the application {1} has no owner or owner is inactive.", requestId, displayName);
                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                                reviewInfo.AccessRequestId = requestId;
                                reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                                reviewInfo.IsGroup = true;
                                reviewInfo.Remarks = exceptionRemarks;
                                reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                reviewers.Add(reviewInfo);

                                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);
                            }
                        }
                        else
                        {
                            string exceptionRemarks = string.Format("The request {0} is routed to IT security team since the application {1} has no owner.", requestId, displayName);

                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.Remarks = exceptionRemarks;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                            reviewers.Add(reviewInfo);

                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);
                        }
                    }
                    else if (currentApprover.Approver == (byte)RoleType.HeadOfSection)
                    {
                        var reviewInfo = GetAccessRequestReviewInfo(user.Id, user.SectionCode);
                        if (reviewInfo != null)
                            reviewers.Add(reviewInfo);
                        else
                            reviewers.AddRange(GetGroupReviewers(RoleType.HeadOfSection, requestId, requestType == RequestType.BulkRequest));
                    }
                    else if (currentApprover.Approver == (byte)RoleType.HeadOfBranch)
                    {
                        var reviewInfo = GetAccessRequestReviewInfo(user.Id, user.BranchCode);
                        if (reviewInfo != null)
                            reviewers.Add(reviewInfo);
                        else
                            reviewers.AddRange(GetGroupReviewers(RoleType.HeadOfBranch, requestId, requestType == RequestType.BulkRequest));
                    }
                    else if (currentApprover.Approver == (byte)RoleType.HeadOfDepartment)
                    {                       
                        var reviewInfo = GetAccessRequestReviewInfo(user.Id, user.DepartmentCode);
                        if (reviewInfo != null)
                            reviewers.Add(reviewInfo);
                        else
                            reviewers.AddRange(GetGroupReviewers(RoleType.HeadOfDepartment, requestId, requestType == RequestType.BulkRequest));
                    }
                }
                //look for group based approval
                else if (currentApprover.ApprovalUserType == ApprovalUserType.Group)
                {
                    var group = Group.GetById(currentApprover.Approver);
                    if (currentApprover.IsMockGroup)
                    {
                        var reviewerIds = utility.GetUsersByGroupId(currentApprover.Approver);

                        if (reviewerIds != null && reviewerIds.Count > 0)
                        {
                            foreach (int reviewerId in reviewerIds)
                            {
                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.ReviewerId = reviewerId;
                                reviewInfo.AccessRequestId = requestId;
                                reviewInfo.GroupId = currentApprover.Approver;
                                reviewInfo.IsGroup = true;
                                reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                reviewers.Add(reviewInfo);
                            }
                        }
                        else
                        {

                            string exceptionRemarks = string.Format("The request {0} is routed to IAM team since the group {1} doesn't have any members.", requestId, group?.GroupName);

                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.Remarks = exceptionRemarks;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                            reviewers.Add(reviewInfo);

                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);
                        }
                    }
                    else
                    {
                        if (currentApprover.RequiresApprovalFromAll)
                        {
                            var reviewerIds = utility.GetUsersByGroupId(currentApprover.Approver);

                            if (reviewerIds != null && reviewerIds.Count > 0)
                            {
                                foreach (int reviewerId in reviewerIds)
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewerId;
                                    reviewInfo.AccessRequestId = requestId;
                                    //reviewInfo.GroupId = currentApprover.Approver;
                                    reviewInfo.IsGroup = false;
                                    //reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
                                    reviewers.Add(reviewInfo);
                                }
                            }
                            else
                            {
                                string exceptionRemarks = string.Format("The request {0} is routed to IAM team since the group {1} doesn't have any members.", requestId, group?.GroupName);

                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                //reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                                reviewInfo.AccessRequestId = requestId;
                                reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                                reviewInfo.IsGroup = true;
                                reviewInfo.Remarks = exceptionRemarks;
                                reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                reviewers.Add(reviewInfo);

                                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);
                            }
                        }
                        else
                        {
                            var reviewerIds = utility.GetUsersByGroupId(currentApprover.Approver);
                            if (reviewerIds != null && reviewerIds.Count > 0)
                            {
                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.ReviewerId = currentApprover.Approver;
                                reviewInfo.AccessRequestId = requestId;
                                reviewInfo.GroupId = currentApprover.Approver;
                                reviewInfo.IsGroup = true;
                                reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                reviewers.Add(reviewInfo);
                            }
                            else
                            {
                                string exceptionRemarks = string.Format("The request {0} is routed to IAM team since the group {1} doesn't have any members.", requestId, group?.GroupName);

                                AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                                reviewInfo.AccessRequestId = requestId;
                                reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                                reviewInfo.IsGroup = true;
                                reviewInfo.Remarks = exceptionRemarks;
                                reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                                reviewers.Add(reviewInfo);

                                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);
                            }
                        }
                    }
                }
                //look for user based approval
                else if (currentApprover.ApprovalUserType == ApprovalUserType.User)
                {
                    User reviewer = User.GetById(currentApprover.Approver);
                    if (reviewer != null && reviewer.IsActive)
                    {
                        AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                        reviewInfo.AccessRequestId = requestId;
                        reviewInfo.ReviewerId = currentApprover.Approver;
                        reviewInfo.IsGroup = false;
                        reviewers.Add(reviewInfo);
                    }
                    else
                    {
                        string exceptionRemarks = string.Format("The request {0} is routed to IAM team since the user {1} is inactive.", requestId, reviewer != null ? reviewer.DisplayName : currentApprover.Approver.ToString());

                        AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                        reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                        reviewInfo.AccessRequestId = requestId;
                        reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                        reviewInfo.IsGroup = true;
                        reviewInfo.Remarks = exceptionRemarks;
                        reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);

                        reviewers.Add(reviewInfo);

                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);
                    }
                }
                //look for individual system config based approval
                else if (currentApprover.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
                {
                    if (currentApprover.Approver == (byte)RoleType.TCodeOwner)
                    {
                        reviewers = this.GetTCodeReviewerBySystemConfiguration(existingTask);
                    }
                    else
                    {
                        reviewers = this.GetRoleReviewerBySystemConfiguration(existingTask, currentApprover);
                    }
                }

                return reviewers;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"GetProvisioningReviewersInformation stackTrace:{ex.StackTrace}", ex);
                throw ex;
            }
        }

        private List<AccessRequestReviewInfo> GetTCodeReviewerBySystemConfiguration(Task existingTask)
        {
            


            utility.LogDebugStep($"GetTCodeReviewerBySystemConfiguration: {existingTask.Id}");
            var reviewers = new List<AccessRequestReviewInfo>();

            var isBulkRequest = existingTask.BulkRequest != null && existingTask.BulkRequest.Id > 0;

            if (existingTask == null || (isBulkRequest && existingTask.BulkRequest == null) || (isBulkRequest ==  false && existingTask.AccessRequest == null) || (isBulkRequest == false && existingTask.AccessRequest != null && existingTask.AccessRequest.AdHocAccessRequest == null))
                return reviewers;

            long requestId = 0;
            ApplicationType applicationType;
            IEntitlementDetail sapDetail;
            long endpointSystemId = 0;
            if (!isBulkRequest)
            {
                requestId = Convert.ToInt64(existingTask.AccessRequestId);
                applicationType = existingTask.AccessRequest.ApplicationType;
                sapDetail = existingTask.AccessRequest.AdHocAccessRequest.GetEntitlementDetail();
                endpointSystemId = existingTask.AccessRequest.EndpointSystemId.Value;
            }
            else
            {
                requestId = Convert.ToInt64(existingTask.BulkRequestId);
                applicationType = existingTask.BulkRequest.ApplicationType;
                sapDetail = existingTask.BulkRequest.GetEntitlementDetail();
                endpointSystemId = existingTask.BulkRequest.EndpointSystemId;
            }
            if (requestId == 0 || sapDetail == null)
                return reviewers;


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
                            IndirecttechnicalroleofPosition = SAPHRP1001.GetRolesByPosition(positionparam, endpointSystemId).ToList();
                            if (IndirecttechnicalroleofPosition == null) IndirecttechnicalroleofPosition = new List<string>();
                        }

                        var dpendentEndpoint = EndPoint.GetByApplicationType(applicationType == ApplicationType.MSSL_ISU ? ApplicationType.MSSL_CRM : ApplicationType.MSSL_ISU);
                        var dependentPositionparam = applicationType == ApplicationType.MSSL_ISU ? Postionmatrix.Position_MCP : Postionmatrix.Position_MSP;
                        if (!string.IsNullOrWhiteSpace(dependentPositionparam) && dpendentEndpoint != null)
                        {
                            dependentSystemId = dpendentEndpoint.Id;
                            dependentIndirecttechnicalroleofPosition = SAPHRP1001.GetRolesByPosition(dependentPositionparam, dependentSystemId);
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
                            IndirecttechnicalroleofPosition = SAPHRP1001.GetRolesByPosition(positionparam, endpointSystemId);
                            if (IndirecttechnicalroleofPosition == null) IndirecttechnicalroleofPosition = new List<string>();
                        }

                        var dpendentEndpoint = EndPoint.GetByApplicationType(applicationType == ApplicationType.EBS_ISU ? ApplicationType.EBS_CRM : ApplicationType.EBS_ISU);
                        var dependentPositionparam = applicationType == ApplicationType.EBS_ISU ? Postionmatrix.Position_CRP : Postionmatrix.Position_ISP;
                        if (!string.IsNullOrWhiteSpace(dependentPositionparam) && dpendentEndpoint != null)
                        {
                            dependentSystemId = dpendentEndpoint.Id;
                            dependentIndirecttechnicalroleofPosition = SAPHRP1001.GetRolesByPosition(dependentPositionparam, dependentSystemId);
                            if (dependentIndirecttechnicalroleofPosition == null) dependentIndirecttechnicalroleofPosition = new List<string>();
                        }
                    }
                }
            }
            else if (sapDetail is AdHocSAPHANARequest)
            {
                var ebsRequest = sapDetail as AdHocSAPHANARequest;
                criticalTCodes = SAPHANAXLCriticalTCodeMatrix.GetAllSAPHANAXLCriticalTCodesMatrixByEndpointId(endpointSystemId).Select(p => new TransactionAndOwner { Transaction = p.Transaction, Description = p.TransactionDescription, Owner = p.TransactionOwner }).ToList();
                businessRoles = UtilityMethods.GetListFromString(ebsRequest.Roles);
                var ep = EndPoint.GetById(endpointSystemId);
                applicationName = ep.ApplicationCode;
                technicalRoles = SAPHANAXLRolesMatrix.GetTechnicalRolesForSAPHANA(businessRoles, applicationName);
                if (!string.IsNullOrWhiteSpace(ebsRequest.Position))
                {
                    position = ebsRequest.Position;
                    //to pass indirect role logic 
                    var Postionmatrix = SAPHANAXLPositionsMatrix.GetSAPHANAPositionMatrixByPosition(endpointSystemId,ebsRequest.Position);
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
            utility.LogDebugStep($"technicalRoles.Count: {technicalRoles?.Count}");

            if ((technicalRoles != null && technicalRoles.Count > 0)
              ||
              (IndirecttechnicalroleofPosition != null || IndirecttechnicalroleofPosition.Count > 0)
              ||
              (dependentSystemId > 0 && (dependentIndirecttechnicalroleofPosition != null || dependentIndirecttechnicalroleofPosition.Count > 0))
              )
            {
                try
                {
                    Dictionary<string, Dictionary<string, string>> rolesTCodeOwners = new Dictionary<string, Dictionary<string, string>>();
                    Dictionary<string, string> tcodeOwners = new Dictionary<string, string>();
                    if (technicalRoles != null && technicalRoles.Count > 0)
                    {
                        foreach (string technicalRole in technicalRoles.Distinct())
                        {
                            var roleTCodes = new List<string>();
                            if(applicationType == ApplicationType.EBS || applicationType == ApplicationType.MSSL)
                            {
                                roleTCodes = SAPHANARoleToAuthMap.GetByRoleCode(technicalRole, Convert.ToInt64(endpointSystemId));
                            }
                            else
                            {
                                roleTCodes = SAPRoleToAuthMap.GetByRoleCode(technicalRole, Convert.ToInt64(endpointSystemId));
                            }
                            tcodeOwners = null;
                            foreach (string tcode in roleTCodes)
                            {
                                if (tcodeOwners == null)
                                {
                                    tcodeOwners = new Dictionary<string, string>();
                                }
                                var criticalTCode = criticalTCodes.FirstOrDefault(item => !string.IsNullOrWhiteSpace(item.Transaction) && item.Transaction.Equals(tcode, StringComparison.CurrentCultureIgnoreCase));
                                if (criticalTCode != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(criticalTCode.Owner))
                                    {
                                        var desc = $"{criticalTCode.Description}[{criticalTCode.Transaction}]";
                                        if (!tcodeOwners.Keys.Contains(criticalTCode.Owner))
                                        {
                                            tcodeOwners.Add(criticalTCode.Owner, desc);
                                        }
                                        else
                                        {
                                            var kvp = tcodeOwners.FirstOrDefault(item => item.Key == criticalTCode.Owner);
                                            if (!kvp.Value.Contains(desc))
                                            {
                                                tcodeOwners[criticalTCode.Owner] += ", " + desc;
                                            }
                                        }
                                    }
                                }
                            }
                            if (tcodeOwners != null && tcodeOwners.Count > 0)
                            {
                                if (!rolesTCodeOwners.Keys.Contains(technicalRole))
                                {
                                    rolesTCodeOwners.Add(technicalRole, tcodeOwners);
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
                                
                            tcodeOwners = null;
                            foreach (string tcode in indirectroleTCodes)
                            {
                                if (tcodeOwners == null)
                                {
                                    tcodeOwners = new Dictionary<string, string>();
                                }
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
                            if (tcodeOwners != null && tcodeOwners.Count > 0)
                            {
                                var key = $"{position}({indirecttechnicalRole})";
                                if (!rolesTCodeOwners.Keys.Contains(key))
                                {
                                    rolesTCodeOwners.Add(key, tcodeOwners);
                                }
                            }
                        }
                    }

                    if (dependentIndirecttechnicalroleofPosition != null && dependentIndirecttechnicalroleofPosition.Count > 0)
                    {
                        foreach (string indirecttechnicalRole in dependentIndirecttechnicalroleofPosition.Distinct())
                        {
                            var indirectroleTCodes = SAPRoleToAuthMap.GetByRoleCode(indirecttechnicalRole, dependentSystemId);
                            tcodeOwners = null;
                            foreach (string tcode in indirectroleTCodes)
                            {
                                if (tcodeOwners == null)
                                {
                                    tcodeOwners = new Dictionary<string, string>();
                                }
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
                            if (tcodeOwners != null && tcodeOwners.Count > 0)
                            {
                                var key = $"{position}({indirecttechnicalRole})";
                                if (!rolesTCodeOwners.Keys.Contains(key))
                                {
                                    rolesTCodeOwners.Add(key, tcodeOwners);
                                }
                            }
                        }
                    }
                    foreach (var mTcodeOwner in rolesTCodeOwners.Distinct())
                    {
                        foreach (KeyValuePair<string, string> tcodeOwner in mTcodeOwner.Value)
                        {
                            var owner = User.GetByEmployeeId(tcodeOwner.Key);

                            if (owner != null && owner.Id >= 0 && owner.IsActive)
                            {
                                if (reviewers != null && !reviewers.Exists(i => i.IsGroup == false && i.ReviewerId == owner.Id))
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = owner.Id;
                                    reviewInfo.IsGroup = false;
                                    var Remarks = $" {mTcodeOwner.Key}[{ tcodeOwner.Value}]";
                                    reviewInfo.SubTaskInfo = this.CreateSubTaskShell(string.Empty, Remarks, owner.Id, 0);
                                    reviewers.Add(reviewInfo);
                                }
                                else
                                {
                                    var existingRecod = reviewers?.FirstOrDefault(i => i.IsGroup == false && i.ReviewerId == owner.Id);
                                    if(existingRecod != null)
                                    {
                                        var Remarks = $"{mTcodeOwner.Key}[{ tcodeOwner.Value}]";
                                        existingRecod.SubTaskInfo.TCode +="<br/>"+Remarks;
                                    }
                                }
                            }
                            else
                            {
                                var itSecurityId = MasterConfigHelper.ITSecurityGroupId;
                                var reviewerIds = utility.GetUsersByGroupId(itSecurityId);
                                if (reviewers != null && !reviewers.Exists(i => i.IsGroup && i.GroupId == itSecurityId))
                                {
                                    AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                                    reviewInfo.ReviewerId = reviewerIds.FirstOrDefault();
                                    reviewInfo.GroupId = itSecurityId;
                                    reviewInfo.IsGroup = true;
                                    reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
                                    var remarks = $"{mTcodeOwner.Key}[{ tcodeOwner.Value}]";
                                    reviewInfo.Remarks = $"The request {requestId} is routed to IAM team since the critical t-code(s) has no owner.";
                                    reviewInfo.SubTaskInfo = this.CreateSubTaskShell(string.Empty, remarks, owner.Id, 0);
                                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, reviewInfo.Remarks);
                                    reviewers.Add(reviewInfo);
                                }
                                else
                                {
                                    var existingRecod = reviewers?.FirstOrDefault(i => i.IsGroup && i.GroupId == itSecurityId);
                                    if (existingRecod != null)
                                    {
                                        var Remarks = $"{mTcodeOwner.Key}[{ tcodeOwner.Value}]";
                                        existingRecod.SubTaskInfo.TCode += "<br/>" + Remarks;
                                    }
                                }
                            }
                        }
                    }
                    utility.LogDebugStep($"reviewers.Count: {reviewers?.Count}");
                }
                catch (Exception ex)
                {
                    DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, ex.Message, ex);
                }
            }
            return reviewers;
        }

        public List<AccessRequestReviewInfo> GetRoleReviewerBySystemConfiguration(Task existingTask, WorkflowConfigurationDetail currentApprover)
        {

            List<AccessRequestReviewInfo> reviewInfos = new List<AccessRequestReviewInfo>();

            AdHocAccessRequest adhocRequest = null;
            BulkRequest bulkRequest = null;

            var requestType = (RequestType)existingTask.RequestTypeId;
            ApplicationType appType = ApplicationType.None;

            long endpointSystemId = 0;
            long requestId = 0;

            if (requestType == RequestType.Provisioning)
            {
                adhocRequest = existingTask.AccessRequest.AdHocAccessRequest;
                appType = adhocRequest.ApplicationType;
                endpointSystemId = adhocRequest.EndpointSystemId.Value;
                requestId = existingTask.AccessRequestId.Value;
            }
            else if (requestType == RequestType.BulkRequest)
            {
                bulkRequest = existingTask.BulkRequest;
                appType = bulkRequest.ApplicationType;
                endpointSystemId = bulkRequest.EndpointSystemId;
                requestId = bulkRequest.Id;
            }

            var matrix = new ApprovalChainFinder().GetRoleReviewersIApprovers(adhocRequest, bulkRequest);
            if (matrix != null)
            {
                reviewInfos = this.GetRoleReviewersByConfig(currentApprover, matrix, requestId);
            }

            return reviewInfos;
        }

        private List<AccessRequestReviewInfo> GetRoleReviewersByConfig(WorkflowConfigurationDetail currentApprover, List<IApprover> approvers, long requestId)
        {
            List<AccessRequestReviewInfo> reviewInfos = new List<AccessRequestReviewInfo>();
            List<ApproverInfo> distinctApprovers = new List<ApproverInfo>();

            foreach (IApprover approver in approvers)
            {
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
                if (approverType == ApprovalUserType.None) continue;
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
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.IsGroup = false;

                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, reviewerId, 1);
                            reviewInfos.Add(reviewInfo);
                        }
                        else
                        {
                            string exceptionRemarks = string.Format("The request - {0} is routed to IT security team since the business role - {1} for the system - {2} Level 1 owner cannot be found or is inactive (user {3}).", requestId, approver.BusinessRole, approver.SystemName, approver.Owner);
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);

                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
                            reviewInfo.Remarks = exceptionRemarks;

                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, MasterConfigHelper.ITSecurityGroupId, 1);
                            reviewInfos.Add(reviewInfo);
                        }
                    }
                    else if (approver.ApproverType == ApprovalUserType.Group && !string.IsNullOrEmpty(approver.Owner))
                    {
                        var reviewerIds = utility.GetUsersByGroupName(approver.Owner);
                        if (reviewerIds.Count != 0)
                        {
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = approver.Owner;
                            reviewInfo.GroupId = utility.GetGroupIdByName(reviewInfo.GroupName);
                            reviewInfo.ReviewerId = reviewInfo.GroupId;
                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, reviewInfo.GroupId, 1);

                            reviewInfos.Add(reviewInfo);
                        }
                        else
                        {
                            string exceptionRemarks = string.Format("The request - {0} is routed to IT security team since the business role - {1} for the system - {2} Level 1 owner cannot be found or is inactive (group {3}).", requestId, approver.BusinessRole, approver.SystemName, approver.Owner);
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);

                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
                            reviewInfo.Remarks = exceptionRemarks;
                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, MasterConfigHelper.ITSecurityGroupId, 1);

                            reviewInfos.Add(reviewInfo);
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
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.IsGroup = false;

                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, reviewerId, 2);
                            reviewInfos.Add(reviewInfo);
                        }
                        else
                        {
                            string exceptionRemarks = string.Format("The request - {0} is routed to IT security team since the business role - {1} for the system - {2} has no level 2 owner (user {3}).", requestId, approver.BusinessRole, approver.SystemName, approver.Owner);
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);

                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
                            reviewInfo.Remarks = exceptionRemarks;

                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, MasterConfigHelper.ITSecurityGroupId, 2);
                            reviewInfos.Add(reviewInfo);
                        }
                    }
                    else if (approver.ApproverType == ApprovalUserType.Group && !string.IsNullOrEmpty(approver.Owner))
                    {
                        var reviewerIds = utility.GetUsersByGroupName(approver.Owner);
                        if (reviewerIds.Count != 0)
                        {
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = approver.Owner;
                            reviewInfo.GroupId = utility.GetGroupIdByName(reviewInfo.GroupName);
                            reviewInfo.ReviewerId = reviewInfo.GroupId;
                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, reviewInfo.GroupId, 2);

                            reviewInfos.Add(reviewInfo);
                        }
                        else
                        {
                            string exceptionRemarks = string.Format("The request - {0} is routed to IT security team since the business role - {1} for the system - {2} has no level 2 owner (group {3}).", requestId, approver.BusinessRole, approver.SystemName, approver.Owner);
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);

                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
                            reviewInfo.Remarks = exceptionRemarks;
                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, MasterConfigHelper.ITSecurityGroupId, 2);

                            reviewInfos.Add(reviewInfo);
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
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.IsGroup = false;

                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, reviewerId, 3);
                            reviewInfos.Add(reviewInfo);
                        }
                        else
                        {
                            string exceptionRemarks = string.Format("The request - {0} is routed to IT security team since the business role - {1} for the system - {2} has no level 3 owner (user {3}).", requestId, approver.BusinessRole, approver.SystemName, approver.Owner);
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);

                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
                            reviewInfo.Remarks = exceptionRemarks;

                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, MasterConfigHelper.ITSecurityGroupId, 3);
                            reviewInfos.Add(reviewInfo);
                        }
                    }
                    else if (approver.ApproverType == ApprovalUserType.Group && !string.IsNullOrEmpty(approver.Owner))
                    {
                        var reviewerIds = utility.GetUsersByGroupName(approver.Owner);
                        if (reviewerIds.Count != 0)
                        {
                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = approver.Owner;
                            reviewInfo.GroupId = utility.GetGroupIdByName(reviewInfo.GroupName);
                            reviewInfo.ReviewerId = reviewInfo.GroupId;
                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, reviewInfo.GroupId, 3);

                            reviewInfos.Add(reviewInfo);
                        }
                        else
                        {
                            string exceptionRemarks = string.Format("The request - {0} is routed to IT security team since the business role - {1} for the system - {2} has no level 3 owner (group {3}).", requestId, approver.BusinessRole, approver.SystemName, approver.Owner);
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks, null);

                            AccessRequestReviewInfo reviewInfo = new AccessRequestReviewInfo();
                            reviewInfo.ReviewerId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.AccessRequestId = requestId;
                            reviewInfo.GroupId = MasterConfigHelper.ITSecurityGroupId;
                            reviewInfo.IsGroup = true;
                            reviewInfo.GroupName = utility.GetGroupNameById(reviewInfo.GroupId);
                            reviewInfo.Remarks = exceptionRemarks;
                            reviewInfo.SubTaskInfo = this.CreateSubTaskShell(approver.BusinessRole, string.Empty, MasterConfigHelper.ITSecurityGroupId, 3);

                            reviewInfos.Add(reviewInfo);
                        }
                    }
                }

            }

            return reviewInfos;
        }

        private SubTask CreateSubTaskShell(string roleName, string tcode, long owner, int level)
        {

            SubTask subtask = new SubTask()
            {
                RoleName = roleName,
                TCode = tcode,
                OwnerId = owner,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now,
                IsActive = true,
            };

            if (level == 0 || !string.IsNullOrWhiteSpace(tcode))
            {
                subtask.Remarks = "Below T-codes require your approval as they are critical T-codes.";
            }
            else
            {
                subtask.Remarks = string.Format("This entitlement require your approval as Role Owner Level {0}.", level);
            }

            return subtask;
        }

        string GetApprover(WorkflowConfigurationDetail currentApprover)
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

        long GetReviewerbyDelegation(long ReviewerId)
        {
            delgateUsers.Add(ReviewerId);

            var delegation = TaskDelegation.GetByRequestType(RequestType.Provisioning, ReviewerId);
            if (delegation != null)
            {
                return GetReviewerbyDelegation(delegation.DelegatedTo);
            }
            else
            {
                return delgateUsers[delgateUsers.Count - 1];
            }

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
    }

    class TransactionAndOwner
    {
        public string Transaction { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
    }
}
