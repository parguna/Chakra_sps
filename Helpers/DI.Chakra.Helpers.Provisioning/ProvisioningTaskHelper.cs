using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.Workflow.ServiceHelpers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DI.Chakra.Helpers.Provisioning
{
    public class ProvisioningTaskHelper
    {
        public void CreateProvisioningTasks()
        {
            try
            {
                var requests = AccessRequest.GetSubmittedAccessRequests();

                if (requests.Count > 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, string.Format("{0} adhoc requests found for workflow submission.", requests.Count.ToString()));

                    foreach (AccessRequest request in requests)
                    {
                        var adhocRequest = AdHocAccessRequest.GetByIdIncludeEndPoint(request.AdHocAccessRequestId.Value, false);
                        var user = User.GetById(request.UserId);

                        var wfConfig = WorkflowConfiguration.GetByEndpointSystemId(adhocRequest.EndpointSystemId.Value, Convert.ToString(Convert.ToByte(user.EmployeeType)),
                                                Convert.ToString(Convert.ToByte(adhocRequest.AccountType)), Convert.ToString(Convert.ToByte(adhocRequest.AdhocActionType)));

                        if (wfConfig == null)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, string.Format("Workflow is not configured for the request {0}", request.Id));
                            continue;
                        }
                        wfConfig.WorkflowConfigurationDetails = WorkflowConfigurationDetail.GetByWorkflowConfigurationId(wfConfig.Id);

                        var tasks = PopulateApprovalTasks(request, wfConfig);

                        DI.Chakra.DomainModel.Presentation.PresentationWorkflowCriteriaBase _criteria = new DI.Chakra.DomainModel.Presentation.SubmitActionCriteria()
                        {
                            RequestType = RequestType.Provisioning,
                            TaskList = tasks
                        };

                        ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(_criteria);
                        _serviceAccessStrategy.AccessWorkflowService();

                        request.RequestStatus = AccessRequestStatus.Requested;
                        request.Save(MasterConfigHelper.ServiceUser);
                    }
                }                
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while creating tasks for submitted adhoc requests.", ex);
            }
        }

        public bool IsApprovalRequired(AccessRequestSource requestSource)
        {
            return RequestApprovalConfiguration.DoesApprovalRequired(requestSource);
        }
        private List<Task> PopulateApprovalTasks(AccessRequest request, WorkflowConfiguration wfConfig)
        {
            bool isGroup = false;
            string exceptionRemarks = string.Empty;
            List<Task> tasks = new List<Task>();
            var firstApprover = wfConfig.WorkflowConfigurationDetails.FirstOrDefault(item => item.SequenceNumber == 1);

            var reviewers = GetReviewerByRoleRequest(request, firstApprover, ref isGroup, ref exceptionRemarks);

            foreach (int reviewer in reviewers)
            {
                Task task = new Task()
                {
                    AccessRequestId = request.Id,
                    IsActive = true,
                    RequestTypeId = (byte)RequestType.Provisioning,
                    WorkflowStateId = (byte)WorkflowState.Level1,
                    IsGroupTask = isGroup,
                    IsActionOnlyRequiredTask = firstApprover.IsActionRequired,
                    Remarks = exceptionRemarks,
                    WorkflowConfigurationId = wfConfig.Id,
                    Action = StatusType.Pending,
                };

                if (task.IsGroupTask)
                {
                    task.ActionGroupId = reviewer;
                    var group = Group.GetById(reviewer);
                    task.GroupName = group != null ? group.GroupName : "";
                }
                else
                {
                    task.ActionUserId = reviewer;
                }

                if (!task.IsGroupTask)
                {
                    // var delegation = TaskDelegation.GetByRequestType(RequestType.Provisioning, reviewer);
                    List<TaskDelegation> delegations = new List<TaskDelegation>();
                    var delegation = TaskDelegation.GetFinalDelegatedToApprover(RequestType.Provisioning, reviewer, delegations);
                    if (delegation != null && request.UserId != delegation.DelegatedTo)
                    {
                        task.ActionUserId = delegation.DelegatedTo;
                        task.IsDelegated = true;
                        task.DelegatedBy = "System";
                        task.DelegatedDate = DateTime.Now;
                        task.DelegatedFrom = reviewer;
                        task.DelegationComments = delegation.DelegationComments;
                    }
                }

                tasks.Add(task);
            }

            return tasks;
        }



        private List<long> GetReviewerByRoleRequest(AccessRequest accessRequest, WorkflowConfigurationDetail firstApprover, ref bool isGroup, ref string exceptionRemarks)
        {
            List<long> reviewers = new List<long>();
            //look for role based approval
            if (firstApprover.ApprovalUserType == ApprovalUserType.Role)
            {
                var user = User.GetById(accessRequest.UserId);

                var approver = RoleTypes.GetById(firstApprover.Approver);

                bool isApproverFromOrgScope = NOrganisationScope.GetAll().Where(t => t.Scope == approver.RoleTypeDesc).Any();

                if (isApproverFromOrgScope)
                {
                    var reviewer = NOrganisationHead.GetOrganisationLevelHead(user.Id, approver.RoleTypeDesc);

                    if (reviewer == null)
                    {
                        isGroup = true;
                        reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                        exceptionRemarks = string.Format("The request {0} is routed to IT security team since the user {1}-{2} is not assigned to any manager.", accessRequest.Id, user.DisplayName, user.UserName);
                        DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, exceptionRemarks);
                    }
                    else
                    {
                        reviewers.Add(reviewer.Id);
                    }
                }
                else
                {


                    //Manager approval
                    if (firstApprover.Approver == (byte)RoleType.Manager)
                    {
                        long rvwr = User.GetNManagerFromOrg(user);
                        if (rvwr != 0)
                        {
                            reviewers.Add(rvwr);
                        }
                        else
                        {
                            isGroup = true;
                            reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                            exceptionRemarks = string.Format("The request {0} is routed to IT security team since the user {1}-{2} is not assigned to any manager.", accessRequest.Id, user.DisplayName, user.UserName);
                            DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, exceptionRemarks);
                        }
                    }
                    //Application Owner approval
                    else if (firstApprover.Approver == (byte)RoleType.ApplicationOwner)
                    {
                        EndPoint application = EndPoint.GetById(Convert.ToInt64(accessRequest.EndpointSystemId));
                        long rvwr = Convert.ToInt64(application.OwnerUserId);

                        if (rvwr != 0)
                        {
                            reviewers.Add(rvwr);
                        }
                        else
                        {
                            isGroup = true;
                            reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                            exceptionRemarks = string.Format("The request {0} is routed to IT security team since the application {1} has no owner.", accessRequest.Id, application.DisplayName);
                            DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, exceptionRemarks);
                        }
                    }
                }
            }
            else if (firstApprover.ApprovalUserType == ApprovalUserType.Group)
            {
                if (firstApprover.IsMockGroup)
                {
                    isGroup = false;
                    var groupUsers = Group.GetByIdIncludingUser(firstApprover.Approver).Users.Select(item => item.Id).ToList();
                    reviewers = groupUsers;
                }
                else
                {
                    if (firstApprover.RequiresApprovalFromAll)
                    {
                        isGroup = false;
                        var groupUsers = Group.GetByIdIncludingUser(firstApprover.Approver).Users.Select(item => item.Id).ToList();
                        reviewers = groupUsers;
                    }
                    else
                    {
                        isGroup = true;
                        reviewers.Add(firstApprover.Approver);
                    }
                }
            }
            //look for user based approval
            else if (firstApprover.ApprovalUserType == ApprovalUserType.User)
            {
                reviewers.Add(firstApprover.Approver);
            }

            return reviewers;
        }
    }
}
