
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Workflow.ServiceHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.ProvisioningUtility
{
    public class ProvisioningTaskHelper
    {
        private List<long> delgateUsers = new List<long>();
        
        readonly string serviceUser;

        public ProvisioningTaskHelper()
        {
            serviceUser = MasterConfigHelper.ServiceUser;
        }

        public void CreateProvisioningTasks()
        {
            List<AccessRequest> requests = new List<AccessRequest>();
            try
            {
                requests = AccessRequest.GetSubmittedAccessRequests();
               
                if (requests.Count > 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("{0} Access requests found for workflow submission.", requests.Count.ToString()));
                    System.Threading.Tasks.Task[] taskArray = new System.Threading.Tasks.Task[requests.Count];
                    var i = 0;
                    foreach (AccessRequest request in requests)
                    {
                        if (MasterConfigHelper.IsDeploymentInProgress)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreateProvisioningTasks - Deployment InProgress");
                            return;
                        }
                        taskArray[i] = System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            Provisioning(request);
                        });
                        i++;
                    }
                    System.Threading.Tasks.Task.WaitAll(taskArray);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating tasks for submitted adhoc requests.", ex);
            }
            finally
            {
                requests?.Clear();
                requests = null;
            }
        }


        void Provisioning(AccessRequest request)
        {
            List<Task> tasks = null;
            try
            {
                if (request.AdHocAccessRequestId == null)
                {
                    //DILog.ProvisioningLogger.WriteLog(request.Id, request.EndpointSystemId, LogLevelType.Warning, $"Skipping the request as Adhoc request is NOT present.");
                    return;
                }

                var adhocRequest = AdHocAccessRequest.GetByIdIncludeEndPoint(request.AdHocAccessRequestId.Value, false);
                var user = User.GetById(request.UserId);
                WorkflowConfiguration wfConfig = null;

                var endPoint = EndPoint.GetById(adhocRequest.EndpointSystemId.Value);
                if (endPoint.ApplicationType == ApplicationType.MicrosoftTeams)
                {
                    var adhocSkype = (AdhocSkypeForBusinessRequest)adhocRequest.GetEntitlementDetail();
                    wfConfig = WorkflowConfiguration.GetByEndpointSystemId(Convert.ToInt64(RequestType.Provisioning), adhocRequest.EndpointSystemId.Value, Convert.ToString(Convert.ToByte(user.EmployeeType)),
                                        Convert.ToString(Convert.ToByte(adhocRequest.AccountType)), Convert.ToString(Convert.ToByte(adhocRequest.AdhocActionType)), request.CreatedDate, ((int)adhocSkype.EntitlementType).ToString());
                }
                else
                {
                    wfConfig = WorkflowConfiguration.GetByEndpointSystemId(Convert.ToInt64(RequestType.Provisioning), adhocRequest.EndpointSystemId.Value, Convert.ToString(Convert.ToByte(user.EmployeeType)),
                                        Convert.ToString(Convert.ToByte(adhocRequest.AccountType)), Convert.ToString(Convert.ToByte(adhocRequest.AdhocActionType)), request.CreatedDate);
                }

                if (wfConfig == null)
                {
                    request.ProgressStatus = AccessRequestProgress.WorkflowNotFound;
                    request.Save(serviceUser);
                    var message = $"Workflow is not configured for the request {request.Id}";
                    DILog.ProvisioningLogger.WriteLog(request.Id, request.EndpointSystemId, LogLevelType.Info, message);
                    return;
                }
                wfConfig.WorkflowConfigurationDetails = WorkflowConfigurationDetail.GetByWorkflowConfigurationId(wfConfig.Id);

                tasks = PopulateApprovalTasks(request, wfConfig);

                DI.Chakra.DomainModel.Presentation.PresentationWorkflowCriteriaBase _criteria = new DI.Chakra.DomainModel.Presentation.SubmitActionCriteria()
                {
                    RequestType = RequestType.Provisioning,
                    TaskList = tasks
                };

                ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(_criteria);
                _serviceAccessStrategy.AccessWorkflowService();

                request.RequestStatus = AccessRequestStatus.Requested;
                request.ProgressStatus = AccessRequestProgress.WaitingforApproval;
                request.Save(serviceUser);
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, $"Error while creating tasks for submitted adhoc requests. The request Id is {request.Id}", ex);
            }
            finally
            {
                tasks?.Clear();
                tasks = null;
            }
        }
        public void CreateBulkProvisioningTasks()
        {
            List<Task> tasks = null;
            List<BulkRequest> requests = new List<BulkRequest>();
            try
            {
                requests = BulkRequest.GetSubmittedBulkRequests();
                var serviceUser = MasterConfigHelper.ServiceUser;
                foreach (BulkRequest request in requests)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreateBulkProvisioningTasks - Deployment InProgress");
                        return;
                    }
                    var user = User.GetById(request.BulkRequestDetails.FirstOrDefault().UserId);

                    WorkflowConfiguration wfConfig = null;

                    var endPoint = EndPoint.GetById(request.EndpointSystemId);
                    if (endPoint.ApplicationType == ApplicationType.MicrosoftTeams)
                    {
                        var adhocSkype = (AdhocSkypeForBusinessRequest)request.GetEntitlementDetail();
                        wfConfig = WorkflowConfiguration.GetByEndpointSystemId(Convert.ToInt64(RequestType.Provisioning), request.EndpointSystemId, Convert.ToString(Convert.ToByte(user.EmployeeType)),
                                            Convert.ToString(Convert.ToByte(AccountType.User)), Convert.ToString(Convert.ToByte(request.AdhocActionType)), request.CreatedDate, ((int)adhocSkype.EntitlementType).ToString());
                    }
                    else
                    {
                        wfConfig = WorkflowConfiguration.GetByEndpointSystemId(Convert.ToInt64(RequestType.Provisioning), request.EndpointSystemId, Convert.ToString(Convert.ToByte(user.EmployeeType)),
                                            Convert.ToString(Convert.ToByte(AccountType.User)), Convert.ToString(Convert.ToByte(request.AdhocActionType)), request.CreatedDate);
                    }


                    if (wfConfig == null)
                    {
                        request.ProgressStatus = AccessRequestProgress.WorkflowNotFound;
                        request.Save(serviceUser);
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Workflow is not configured for the Bulk request {0}", request.Id));
                        continue;
                    }
                    wfConfig.WorkflowConfigurationDetails = WorkflowConfigurationDetail.GetByWorkflowConfigurationId(wfConfig.Id);

                    tasks = PopulateApprovalTasks(request, wfConfig);

                    DI.Chakra.DomainModel.Presentation.PresentationWorkflowCriteriaBase _criteria = new DI.Chakra.DomainModel.Presentation.SubmitActionCriteria()
                    {
                        RequestType = RequestType.BulkRequest,
                        TaskList = tasks
                    };

                    ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(_criteria);
                    _serviceAccessStrategy.AccessWorkflowService();

                    request.RequestStatus = AccessRequestStatus.Requested;
                    request.ProgressStatus = AccessRequestProgress.WaitingforApproval;
                    request.Save(serviceUser);

                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating tasks for submitted bulk requests.", ex);
            }
            finally
            {
                requests?.Clear();
                requests = null;
            }
        }

        public void CreateEmployeeEventTasks()
        {
            List<EmployeeEventTrigger> triggers = new List<EmployeeEventTrigger>();
            List<Task> tasks = null;
            try
            {
                triggers = EmployeeEventTrigger.GetSubmittedTriggers();
                var serviceUser = MasterConfigHelper.ServiceUser;
                foreach (EmployeeEventTrigger trigger in triggers)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"CreateEmployeeEventTasks - Deployment InProgress");
                        return;
                    }

                    var wfConfig = WorkflowConfiguration.GetByRequestType((byte)RequestType.EmployeeEvent);

                    if (wfConfig == null)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "Workflow is not configured for Employee creation/extension requests");
                        continue;
                    }

                    // check in the task table if  the task is already pouplated for the trigger
                    if (!Task.DoesEmploymentEventTasksAlreadyExist(trigger.Id))
                    {
                        wfConfig.WorkflowConfigurationDetails = WorkflowConfigurationDetail.GetByWorkflowConfigurationId(wfConfig.Id);
                        tasks = PopulateApprovalTasks(trigger, wfConfig);

                        DI.Chakra.DomainModel.Presentation.PresentationWorkflowCriteriaBase _criteria = new DI.Chakra.DomainModel.Presentation.SubmitActionCriteria()
                        {
                            RequestType = RequestType.EmployeeEvent,
                            TaskList = tasks
                        };
                        ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(_criteria);
                        _serviceAccessStrategy.AccessWorkflowService();
                    }
                    else
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, $"Task is already created for the trigger:{trigger.Id}");

                    trigger.RequestStatus = AccessRequestStatus.Requested;
                    trigger.Save(serviceUser);
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while creating tasks for non-employee events.", ex);
            }
            finally
            {
                tasks?.Clear();
                tasks = null;

                triggers?.Clear();
                triggers = null;
            }
        }

        List<Task> PopulateApprovalTasks(AccessRequest request, WorkflowConfiguration wfConfig)
        {
            bool isGroup = false;
            string exceptionRemarks = string.Empty;
            List<Task> tasks = new List<Task>();
            WorkflowConfigurationDetail firstApprover = wfConfig.WorkflowConfigurationDetails.FirstOrDefault(item => item.SequenceNumber == 1);
            //if (request.AttestationCampaignId.HasValue)
            //{
            //    var apprv = wfConfig.WorkflowConfigurationDetails.OrderBy(i=>i.SequenceNumber).FirstOrDefault(item => item.IsActionRequired);
            //    if(apprv != null)
            //    {
            //        firstApprover = apprv;
            //    }
            //}
            
            var secGroup = Group.GetById(MasterConfigHelper.ITSecurityGroupId);

            var reviewers = GetReviewers(request.Id, request.UserId, request.AdHocAccessRequest.SelectedSupervisor,
                request.EndpointSystemId.Value, firstApprover, ref isGroup, ref exceptionRemarks);

            foreach (int reviewer in reviewers)
            {
                Task task = new Task()
                {
                    AccessRequestId = request.Id,
                    IsActive = true,
                    RequestTypeId = (byte)RequestType.Provisioning,
                    WorkflowStateId = (byte)WorkflowState.Level1,
                    IsGroupTask = isGroup && !firstApprover.IsMockGroup,
                    IsActionOnlyRequiredTask = firstApprover.IsActionRequired,
                    Remarks = exceptionRemarks,
                    WorkflowConfigurationId = wfConfig.Id,
                    Action = StatusType.Pending,
                    AttestationCampaignId = request.AttestationCampaignId
                };

                if (task.IsGroupTask)
                {
                    AssignGroupId(reviewer, task);
                }
                else
                {
                    task.ActionUserId = reviewer;
                }

                if (!task.IsGroupTask)
                {
                    var delegation = TaskDelegation.GetByRequestType(RequestType.Provisioning, reviewer);
                    //List<TaskDelegation> delegations = new List<TaskDelegation>();
                    //var delegation = TaskDelegation.GetFinalDelegatedToApprover(RequestType.Provisioning, reviewer, delegations);
                    if (delegation != null)
                    {
                        List<TaskDelegation> delgations = new List<TaskDelegation>();
                        if (request.UserId != delegation.DelegatedTo)
                        {
                            delgations.Add(delegation);
                            task.ActionUserId = GetReviewerbyDelegation(delegation.DelegatedTo, delgations);
                            task.IsDelegated = true;
                            task.DelegatedBy = "System";
                            task.DelegatedDate = DateTime.Now;
                            task.DelegatedFrom = delgateUsers.Count > 1 ? delgateUsers[delgateUsers.Count - 2] : reviewer;
                            StringBuilder delegationComments = new StringBuilder();
                            task.DelegationComments = string.Join("|", delgations.Select(item => GetDelegationSystemComment(item.ActionUser, item.DelegatedTo, item.DelegationComments)));
                            delgations.Clear();

                        }
                        else
                        {
                            exceptionRemarks += string.Format(" The request {0} is routed to IAM team since the user and approver should not be the same.", request.Id);
                            task.ActionUserId = null;
                            task.IsGroupTask = true;
                            if(secGroup != null)
                            {
                                AssignGroupId(secGroup.Id, task);
                            }else
                            {
                                task.ActionGroupId = 0;
                            }
                            //task.GroupName = secGroup != null ? secGroup.GroupName : "";
                            //task.ActionGroupId = secGroup != null ? secGroup.Id : 0;
                            task.Remarks = exceptionRemarks;
                        }
                    }
                }

                tasks.Add(task);
            }
            reviewers?.Clear();
            reviewers = null;
            return tasks;
        }

        List<Task> PopulateApprovalTasks(BulkRequest request, WorkflowConfiguration wfConfig)
        {
            bool isGroup = false;
            string exceptionRemarks = string.Empty;
            List<Task> tasks = new List<Task>();

            var secGroup = Group.GetById(MasterConfigHelper.ITSecurityGroupId);
            var firstApprover = wfConfig.WorkflowConfigurationDetails.FirstOrDefault(item => item.SequenceNumber == 1);

            var reviewers = GetReviewers(request.Id, request.BulkRequestDetails.FirstOrDefault().UserId, request.SelectedSupervisor,
                request.EndpointSystemId, firstApprover, ref isGroup, ref exceptionRemarks);

            foreach (int reviewer in reviewers)
            {
                Task task = new Task()
                {
                    BulkRequestId = request.Id,
                    IsActive = true,
                    RequestTypeId = (byte)RequestType.BulkRequest,
                    WorkflowStateId = (byte)WorkflowState.Level1,
                    IsGroupTask = isGroup && !firstApprover.IsMockGroup,
                    IsActionOnlyRequiredTask = firstApprover.IsActionRequired,
                    Remarks = exceptionRemarks,
                    WorkflowConfigurationId = wfConfig.Id,
                    Action = StatusType.Pending,
                };

                if (task.IsGroupTask)
                {
                    //task.ActionGroupId = reviewer;
                    //var group = Group.GetById(reviewer);
                    //task.GroupName = group != null ? group.GroupName : "";
                    AssignGroupId(reviewer, task);
                }
                else
                {
                    task.ActionUserId = reviewer;
                }

                if (!task.IsGroupTask)
                {
                    var delegation = TaskDelegation.GetByRequestType(RequestType.Provisioning, reviewer);
                    //List<TaskDelegation> delegations = new List<TaskDelegation>();
                    //var delegation = TaskDelegation.GetFinalDelegatedToApprover(RequestType.Provisioning, reviewer, delegations);
                    if (delegation != null)
                    {
                        bool isApproverInRequestFor = request.BulkRequestDetails.Any(item => item.UserId == delegation.DelegatedTo);
                        if (!isApproverInRequestFor)
                        {
                            var delgations = new List<TaskDelegation>();
                            delgations.Add(delegation);
                            task.ActionUserId = GetReviewerbyDelegation(delegation.DelegatedTo, delgations);
                            task.IsDelegated = true;
                            task.DelegatedBy = "System";
                            task.DelegatedDate = DateTime.Now;
                            task.DelegatedFrom = delgateUsers.Count > 1 ? delgateUsers[delgateUsers.Count - 2] : reviewer;
                            StringBuilder delegationComments = new StringBuilder();
                            task.DelegationComments = string.Join("|", delgations.Select(item => GetDelegationSystemComment(item.ActionUser, item.DelegatedTo, item.DelegationComments)));
                            delgations.Clear();
                        }
                        else
                        {
                            exceptionRemarks += string.Format(" The request {0} is routed to IAM team since the user and approver should not be the same.", request.Id);
                            task.ActionUserId = null;
                            task.IsGroupTask = true;
                            if(secGroup != null)
                            {
                                AssignGroupId(secGroup.Id, task);
                            }
                            else
                            {
                                task.ActionGroupId = 0;
                            }
                            //task.GroupName = secGroup != null ? secGroup.GroupName : "";
                            //task.ActionGroupId = secGroup != null ? secGroup.Id : 0;
                            task.Remarks = exceptionRemarks;
                        }
                    }
                }

                tasks.Add(task);
            }
            reviewers?.Clear();
            reviewers = null;
            return tasks;
        }

        List<Task> PopulateApprovalTasks(EmployeeEventTrigger trigger, WorkflowConfiguration wfConfig)
        {
            bool isGroup = false;
            string exceptionRemarks = string.Empty;
            List<Task> tasks = new List<Task>();

            var secGroup = Group.GetById(MasterConfigHelper.ITSecurityGroupId);
            var firstApprover = wfConfig.WorkflowConfigurationDetails.FirstOrDefault(item => item.SequenceNumber == 1);

            var reviewers = GetReviewers(0, trigger.UserId, null, 0, firstApprover, ref isGroup, ref exceptionRemarks);

            foreach (long reviewer in reviewers)
            {
                var existingTask = Task.GetByEventTriggerId(trigger.Id);

                Task task = new Task()
                {
                    EmployeeEventTriggerId = trigger.Id,
                    IsActive = true,
                    RequestTypeId = (byte)RequestType.EmployeeEvent,
                    WorkflowStateId = (byte)WorkflowState.Level1,
                    IsGroupTask = isGroup && !firstApprover.IsMockGroup,
                    IsActionOnlyRequiredTask = firstApprover.IsActionRequired,
                    Remarks = exceptionRemarks,
                    WorkflowConfigurationId = wfConfig.Id,
                    Action = StatusType.Pending,
                };


                bool IsAprrovedTask = false;
                // check whether task is Approved Task
                if (existingTask != null)
                {
                    IsAprrovedTask = existingTask.Action == StatusType.Approved;
                }
                

                if (task.IsGroupTask)
                {
                    if (existingTask != null && existingTask.IsActive && existingTask.ActionGroupId == reviewer )
                        continue;

                    //task.ActionGroupId = reviewer;
                    //var group = Group.GetById(reviewer);
                    //task.GroupName = group != null ? group.GroupName : "";
                    AssignGroupId(reviewer, task);
                }
                else
                {
                    if (existingTask != null && existingTask.IsActive && existingTask.ActionUserId == reviewer && existingTask.IsCompleted && IsAprrovedTask)
                        continue;

                    task.ActionUserId = reviewer;
                }

                if (!task.IsGroupTask)
                {
                    var delegation = TaskDelegation.GetByRequestType(RequestType.Provisioning, reviewer);
                    //List<TaskDelegation> delegations = new List<TaskDelegation>();
                    //var delegation = TaskDelegation.GetFinalDelegatedToApprover(RequestType.Provisioning, reviewer, delegations);
                    if (delegation != null)
                    {
                        var delgations = new List<TaskDelegation>();
                        if (trigger.UserId != delegation.DelegatedTo)
                        {
                            delgations.Add(delegation);
                            task.ActionUserId = GetReviewerbyDelegation(delegation.DelegatedTo, delgations);
                            task.IsDelegated = true;
                            task.DelegatedBy = "System";
                            task.DelegatedDate = DateTime.Now;
                            task.DelegatedFrom = delgateUsers.Count > 1 ? delgateUsers[delgateUsers.Count - 2] : reviewer;

                            //task.DelegationComments = delegation.DelegationComments;
                            //if (task.DelegatedFrom.HasValue && task.ActionUserId.HasValue)
                            //{
                            //    task.DelegationComments = GetDelegationSystemComment(task.DelegatedFrom.Value, task.ActionUserId.Value, delegation.DelegationComments);
                            //}
                            //else
                            //{
                            //    task.DelegationComments = GetDelegationSystemComment(delegation.ActionUser, delegation.DelegatedTo, delegation.DelegationComments);
                            //}
                            task.DelegationComments = string.Join("|", delgations.Select(item => GetDelegationSystemComment(item.ActionUser, item.DelegatedTo, item.DelegationComments)));
                            delgations.Clear();
                        }
                        else
                        {
                            exceptionRemarks += string.Format(" The request {0} is routed to IAM team since the user and approver should not be the same.", trigger.Id);
                            task.ActionUserId = null;
                            task.IsGroupTask = true;
                            if(secGroup != null)
                            {
                                AssignGroupId(secGroup.Id, task);
                            }else
                            {
                                task.ActionGroupId = 0;
                            }
                            //task.GroupName = secGroup != null ? secGroup.GroupName : "";
                            //task.ActionGroupId = secGroup != null ? secGroup.Id : 0;
                            task.Remarks = exceptionRemarks;
                        }
                    }
                }

                tasks.Add(task);
            }
            reviewers?.Clear();
            reviewers = null;
            return tasks;
        }
        private string GetDelegationSystemComment(long DelegatedFrom,long delegatedTo,string DelegationComments)
        {
            var comments = string.Empty;
            try
            {
                comments = $"Delegation From System: Date - {DateTime.Now.ToString("dd-MM-yyyy")}, From - {User.GetById(DelegatedFrom)?.DisplayName}, By - {User.GetById(DelegatedFrom)?.DisplayName}, To - {User.GetById(delegatedTo)?.DisplayName}, Comments - {DelegationComments}";
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"GetDelegationSystemComment:{ex.Message}");
                comments = DelegationComments;
            }
            return comments;
        }
        List<long> GetReviewers(long requestId, long userId, long? selectedSupervisor, long endpointSystemId, WorkflowConfigurationDetail firstApprover, ref bool isGroup, ref string exceptionRemarks)
        {
            List<long> reviewers = new List<long>();

            if (firstApprover == null) return reviewers;

            //look for role based approval
            if (firstApprover.ApprovalUserType == ApprovalUserType.Role)
            {
                var user = User.GetById(userId);

                if (firstApprover.Approver == (byte)RoleType.RO)
                {         

                    if (selectedSupervisor.HasValue) 
                    {
                         
                        User selSupervisorUser = User.GetById(selectedSupervisor.Value);

                        if (selSupervisorUser != null && selSupervisorUser.IsActive)
                            reviewers.Add(selectedSupervisor.Value);
                        else
                        {
                            isGroup = true;
                            reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                            if (requestId > 0)
                            {
                                exceptionRemarks = $"The request {requestId} is routed to IAM since the selected supervisor is in-active.";
                                DILog.ProvisioningLogger.WriteLog(requestId, null, LogLevelType.Warning, exceptionRemarks);
                            }
                        }
                    }
                    else
                    {
                        var ro = User.GetROByUser(user.Id);

                        if (ro != null && ro.IsActive)
                        {
                            reviewers.Add(ro.Id);
                        }
                        else
                        {
                            isGroup = true;
                            reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                            if (requestId > 0)
                            {
                                exceptionRemarks = string.Format("The request {0} is routed to IAM since the user {1}-{2} is not assigned to any RO.", requestId, user.DisplayName, user.UserName);
                                DILog.ProvisioningLogger.WriteLog(requestId, null, LogLevelType.Warning, exceptionRemarks);
                            }
                        }
                    }
                }
                else if (firstApprover.Approver == (byte)RoleType.CO)
                {
                    bool reviewerRequired = true;
                    if (!user.FullTimeEmployee)
                    {
                        var accessRequest = AccessRequest.GetById(requestId, true);
                        if(accessRequest.ApplicationType == ApplicationType.Office365 && accessRequest.AccountType == AccountType.User 
                            
                            && (accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount || accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
                        {
                            if (accessRequest.BulkRequestId.HasValue)
                            {
                                var o365RequestDetails =BulkRequest.GetById(accessRequest.BulkRequestId.Value,true);
                                if(o365RequestDetails != null)
                                {
                                    var bulkRequestDetails = o365RequestDetails.GetEntitlementDetail() as AdHocO365Request; 
                                    if(bulkRequestDetails != null && bulkRequestDetails.ExternalEmailAction != ExternalEmailActionType.Enable)
                                    {
                                        //If user select the ExternalEmailAction enable then it should route to CO.Else No CO approval required
                                        reviewerRequired = false;
                                    }
                                }
                            }
                            else
                            {
                                var o365RequestDetails = accessRequest.AdHocAccessRequest.GetEntitlementDetail() as AdHocO365Request;
                                if (o365RequestDetails != null)
                                {
                                    if(o365RequestDetails.ExternalEmailAction != ExternalEmailActionType.Enable)
                                    {
                                        //If user select the ExternalEmailAction enable then it should route to CO.Else No CO approval required
                                        reviewerRequired = false;
                                    }
                                }
                            }
                            
                        }
                    }
                    if (reviewerRequired)
                    {
                        var co = User.GetCOByUser(user.Id);

                        if (co != null && co.IsActive)
                        {
                            reviewers.Add(co.Id);
                        }
                        else
                        {
                            isGroup = true;
                            reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                            if (requestId > 0)
                            {
                                if (co != null)
                                {
                                    exceptionRemarks = string.Format("The request {0} is routed to IAM since the user {1}-{2} is not assigned to active CO.", requestId, user.DisplayName, user.UserName);
                                }
                                else
                                {
                                    exceptionRemarks = string.Format("The request {0} is routed to IAM since the user {1}-{2} is not assigned to any CO.", requestId, user.DisplayName, user.UserName);
                                }
                                DILog.ProvisioningLogger.WriteLog(requestId, null, LogLevelType.Warning, exceptionRemarks);
                            }
                        }
                    }
                }
                //Application Owner approval
                else if (firstApprover.Approver == (byte)RoleType.ApplicationOwner)
                {
                    EndPoint application = EndPoint.GetById(Convert.ToInt64(endpointSystemId));
                    long rvwr = Convert.ToInt64(application.OwnerUserId);

                    if (rvwr != 0)
                    {
                        User owner = User.GetById(rvwr);

                        if (owner != null && owner.IsActive)
                        {
                            reviewers.Add(rvwr);
                        }
                        else
                        {
                            isGroup = true;
                            reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                            exceptionRemarks = string.Format("The request {0} is routed to IAM since the application {1} has owner is not active.", requestId, application.DisplayName);
                            DILog.ProvisioningLogger.WriteLog(requestId, null, LogLevelType.Warning, exceptionRemarks);
                        }
                    }
                    else
                    {
                        isGroup = true;
                        reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                        exceptionRemarks = string.Format("The request {0} is routed to IAM since the application {1} has no owner.", requestId, application.DisplayName);
                        DILog.ProvisioningLogger.WriteLog(requestId, null, LogLevelType.Warning, exceptionRemarks);
                    }
                }
                else if (firstApprover.Approver == (byte)RoleType.HeadOfSection)
                {
                    var reviewInfo = GetAccessRequestReviewInfo(user.Id, user.SectionCode);
                    if (reviewInfo != 0)
                        reviewers.Add(reviewInfo);
                    else
                    {
                        isGroup = true;
                        reviewers.Add(MasterConfigHelper.ITSecurityGroupId);
                        exceptionRemarks = string.Format("The request {0} is routed to IAM since the {1} is NOT available for the section - {2}.", requestId, Convert.ToString(RoleType.HeadOfSection),user.OrgSection);
                        DILog.ProvisioningLogger.WriteLog(requestId, null, LogLevelType.Warning, exceptionRemarks);
                    }
                }
                else if (firstApprover.Approver == (byte)RoleType.HeadOfBranch)
                {
                    var reviewInfo = GetAccessRequestReviewInfo(user.Id, user.BranchCode);
                    if (reviewInfo != 0)
                        reviewers.Add(reviewInfo);
                    else
                    {
                        isGroup = true;
                        reviewers.Add(MasterConfigHelper.ITSecurityGroupId);
                        exceptionRemarks = string.Format("The request {0} is routed to IAM since the {1} is NOT available for the branch - {2}.", requestId, Convert.ToString(RoleType.HeadOfBranch),user.OrgBranch);
                        DILog.ProvisioningLogger.WriteLog(requestId, null, LogLevelType.Warning, exceptionRemarks);
                    }
                }
                else if (firstApprover.Approver == (byte)RoleType.HeadOfDepartment)
                {
                    var reviewInfo = GetAccessRequestReviewInfo(user.Id, user.DepartmentCode);
                    if (reviewInfo != 0)
                        reviewers.Add(reviewInfo);
                    else
                    {
                        isGroup = true;
                        reviewers.Add(MasterConfigHelper.ITSecurityGroupId);
                        exceptionRemarks = string.Format("The request {0} is routed to IAM since the {1} is NOT available for the department - {2}.", requestId, Convert.ToString(RoleType.HeadOfDepartment),user.OrgDepartment);
                        DILog.ProvisioningLogger.WriteLog(requestId, null, LogLevelType.Warning, exceptionRemarks);
                    }
                }
            }
            else if (firstApprover.ApprovalUserType == ApprovalUserType.Group)
            {
                if (firstApprover.IsMockGroup)
                {
                    isGroup = false;
                    var group = Group.GetByIdIncludingOnlyUser(firstApprover.Approver);
                    var groupUsers = group.Users?.Select(item => item.Id).ToList();
                    if (groupUsers != null && groupUsers.Count > 0)
                    {
                        reviewers = groupUsers;
                    }
                    else
                    {
                        isGroup = true;
                        reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                        exceptionRemarks = string.Format("The request {0} is routed to IAM since the group {1} doesn't have any members.", requestId, group != null ? group.GroupName : firstApprover.Approver.ToString());
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks);
                    }
                }
                else
                {
                    if (firstApprover.RequiresApprovalFromAll)
                    {
                        isGroup = false;
                        var group = Group.GetByIdIncludingOnlyUser(firstApprover.Approver);
                        var groupUsers = group.Users?.Select(item => item.Id).ToList();
                        if (groupUsers != null && groupUsers.Count > 0)
                        {
                            reviewers = groupUsers;
                        }
                        else
                        {
                            isGroup = true;
                            reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                            exceptionRemarks = string.Format("The request {0} is routed to IAM since the group {1} doesn't have any members.", requestId, group != null ? group.GroupName : firstApprover.Approver.ToString());
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks);
                        }
                    }
                    else
                    {
                        isGroup = true;
                        var group = Group.GetByIdIncludingOnlyUser(firstApprover.Approver);
                        var groupUsers = group.Users?.Select(item => item.Id).ToList();
                        if (groupUsers != null && groupUsers.Count > 0)
                        {
                            reviewers.Add(firstApprover.Approver);
                        }
                        else
                        {
                            isGroup = true;
                            reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                            exceptionRemarks = string.Format("The request {0} is routed to IAM since the group {1} doesn't have any members.", requestId, group != null ? group.GroupName : firstApprover.Approver.ToString());
                            DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks);
                        }
                    }
                }
            }
            //look for user based approval
            else if (firstApprover.ApprovalUserType == ApprovalUserType.User)
            {
                User reviewer = User.GetById(firstApprover.Approver);
                if (reviewer == null || !reviewer.IsActive)
                {
                    isGroup = true;
                    reviewers.Add(MasterConfigHelper.ITSecurityGroupId);

                    exceptionRemarks = string.Format("The request {0} is routed to IAM since the user {1} is not available / active in IDMS.", requestId, reviewer != null ? reviewer.DisplayName : firstApprover.Approver.ToString());
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Warning, exceptionRemarks);
                }
                else
                    reviewers.Add(reviewer.Id);
            }
            else if (firstApprover.ApprovalUserType == ApprovalUserType.UseApplicationConfiguration)
            {
                //TODO:Siva
            }
            return reviewers;
        }

        private long GetAccessRequestReviewInfo(long id, string orgCode)
        {
            if (string.IsNullOrWhiteSpace(orgCode))
                return 0;

            var nOrg = NOrganisation.GetByOrgCode(orgCode);
            if (nOrg != null && nOrg.IsActive)
            {
                var nOrgHead = NOrganisationHead.GetByOrganisationId(nOrg.Id);
                if (nOrgHead == null || !nOrgHead.HeadId.HasValue)
                    return 0;

                var user = User.GetById(nOrgHead.HeadId.Value);
                if (user != null && user.IsActive)
                {
                    return user.Id;
                }
            }
            return 0;
        }

        long GetReviewerbyDelegation(long ReviewerId, List<TaskDelegation> delgations)
        {
            delgateUsers.Add(ReviewerId);
            
            var delegation = TaskDelegation.GetByUserId(ReviewerId, RequestType.Provisioning);
            if (delegation != null)
            {
                delgations.Add(delegation);
                return GetReviewerbyDelegation(delegation.DelegatedTo, delgations);
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
}