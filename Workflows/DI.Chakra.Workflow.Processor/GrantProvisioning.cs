
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Services.EmailingProcessor;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Text;

namespace DI.Chakra.Workflow.Processor
{
    public class GrantProvisioning
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();
        string MasterConfigHelper_ServiceUser;
        public void Execute(Task task)
        {
            try
            {
                MasterConfigHelper_ServiceUser = MasterConfigHelper.ServiceUser;
                if (task.RequestTypeId == (short)RequestType.Provisioning)
                {
                    var currentTask = Task.GetProvisioningTask(task.Id);
                    if (currentTask != null)
                    {
                        if (currentTask.AccessRequest.ApplicationType == ApplicationType.InhouseSystems)
                        {
                            if (currentTask.AccessRequest.RequestStatus == AccessRequestStatus.Approved)
                            {
                                currentTask.AccessRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                                currentTask.AccessRequest.LastModifiedBy = "System";

                                if (currentTask.AccessRequest.EndpointSystemId.HasValue)
                                {
                                    var endpoint = EndPoint.GetById(currentTask.AccessRequest.EndpointSystemId.Value);
                                    if (endpoint != null && endpoint.CanAutomate)
                                    {
                                        currentTask.AccessRequest.ExecutionTime = currentTask.AccessRequest.ActivationDate.Value <= DateTime.Now ? DateTime.Now : currentTask.AccessRequest.ActivationDate.Value;
                                    }
                                    else
                                    {
                                        currentTask.AccessRequest.ExecutionTime = DateTime.Now;
                                    }
                                }

                                currentTask.AccessRequest.LastModifiedDate = DateTime.Now;
                                currentTask.AccessRequest.RequestStatus = AccessRequestStatus.Granted;
                            }
                            else if (currentTask.AccessRequest.RequestStatus == AccessRequestStatus.Rejected)
                            {
                                currentTask.AccessRequest.LastModifiedBy = "System";
                                currentTask.AccessRequest.LastModifiedDate = DateTime.Now;
                                currentTask.AccessRequest.ProgressStatus = AccessRequestProgress.RequestRejected;
                            }
                        }
                        else if (currentTask.AccessRequest.ApplicationType == ApplicationType.VPN)
                        {
                            if (currentTask.AccessRequest.RequestStatus == AccessRequestStatus.Approved)
                            {
                                if (currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount ||
                                    currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                                {
                                    var vpnDetail = (AdHocVPNRequest)currentTask.AccessRequest.AdHocAccessRequest.GetEntitlementDetail();

                                    if (vpnDetail.EndpointUserId == 0)
                                    {
                                        currentTask.AccessRequest.AdHocAccessRequest.EntitlementDetail = currentTask.AccessRequest.AdHocAccessRequest.GetSerializedString(vpnDetail);
                                        currentTask.AccessRequest.AdHocAccessRequest.Save(MasterConfigHelper_ServiceUser);
                                    }
                                }

                                currentTask.AccessRequest.LastModifiedBy = "System";
                                currentTask.AccessRequest.LastModifiedDate = DateTime.Now;

                                currentTask.AccessRequest.ExecutionTime = currentTask.AccessRequest.ActivationDate.Value <= DateTime.Now ? DateTime.Now : currentTask.AccessRequest.ActivationDate.Value;
                                currentTask.AccessRequest.RequestStatus = AccessRequestStatus.Granted;


                                if (currentTask.AccessRequest.RequestSource == AccessRequestSource.AdhocRequest)
                                {
                                    currentTask.AccessRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                                }
                                else
                                {
                                    if (currentTask.AccessRequest.RequestSource == AccessRequestSource.OUChange ||
                                        currentTask.AccessRequest.RequestSource == AccessRequestSource.OffBoarding
                                        )
                                    {
                                        var vpnDetail = (AdHocVPNRequest)currentTask.AccessRequest.AdHocAccessRequest.GetEntitlementDetail();
                                        if (vpnDetail != null && vpnDetail.IsOffboardOrTransferRequest && (currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount))
                                        {
                                            currentTask.AccessRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                                        }
                                        else
                                        {
                                            currentTask.AccessRequest.ProgressStatus = AccessRequestProgress.Completed;
                                        }
                                    }
                                    else
                                    {
                                        currentTask.AccessRequest.ProgressStatus = AccessRequestProgress.Completed;
                                    }
                                }

                            }
                            else if (currentTask.AccessRequest.RequestStatus == AccessRequestStatus.Withdrawn)
                            {
                                utility.DeleteAutoCreatedADForVPN(currentTask.AccessRequest, currentTask.AccessRequest.AdHocAccessRequest);
                            }
                            else if (currentTask.AccessRequest.RequestStatus == AccessRequestStatus.Rejected)
                            {
                                currentTask.AccessRequest.LastModifiedBy = "System";
                                currentTask.AccessRequest.LastModifiedDate = DateTime.Now;
                                currentTask.AccessRequest.ProgressStatus = AccessRequestProgress.RequestRejected;

                                utility.DeleteAutoCreatedADForVPN(currentTask.AccessRequest, currentTask.AccessRequest.AdHocAccessRequest);
                            }
                        }
                        else
                        {
                            if (currentTask.AccessRequest.RequestStatus == AccessRequestStatus.Approved)
                            {
                                currentTask.AccessRequest.LastModifiedBy = "System";
                                currentTask.AccessRequest.ExecutionTime = currentTask.AccessRequest.ActivationDate.Value <= DateTime.Now ? DateTime.Now : currentTask.AccessRequest.ActivationDate.Value;
                                currentTask.AccessRequest.LastModifiedDate = DateTime.Now;
                                currentTask.AccessRequest.RequestStatus = AccessRequestStatus.Granted;
                                currentTask.AccessRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;

                                bool isReallocate = false;
                                if (currentTask.AccessRequest.ApplicationType == ApplicationType.EBS_CRM || currentTask.AccessRequest.ApplicationType == ApplicationType.EBS_ISU)
                                {
                                    var sapDetail = (AdHocEBSRequest)currentTask.AccessRequest.AdHocAccessRequest.GetEntitlementDetail();
                                    isReallocate = sapDetail.IsReallocate;
                                }
                                else if (currentTask.AccessRequest.ApplicationType == ApplicationType.MSSL_CRM || currentTask.AccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                                {
                                    var sapDetail = (AdHocMSSLRequest)currentTask.AccessRequest.AdHocAccessRequest.GetEntitlementDetail();
                                    isReallocate = sapDetail.IsReallocate;
                                }
                                else if (currentTask.AccessRequest.ApplicationType == ApplicationType.EBS || currentTask.AccessRequest.ApplicationType == ApplicationType.MSSL)
                                {
                                    var sapDetail = (AdHocSAPHANARequest)currentTask.AccessRequest.AdHocAccessRequest.GetEntitlementDetail();
                                    isReallocate = sapDetail.IsReallocate;
                                }

                                if (isReallocate)
                                {
                                    //Setting 24 hours because when we do Reallocate during working hours will affect the user. Thats why we are setting the execution time at mid night
                                    currentTask.AccessRequest.ExecutionTime = currentTask.AccessRequest.ExecutionTime.Value.Date.AddHours(24);
                                }
                            }
                            else if (currentTask.AccessRequest.RequestStatus == AccessRequestStatus.Rejected)
                            {
                                currentTask.AccessRequest.LastModifiedBy = "System";
                                currentTask.AccessRequest.LastModifiedDate = DateTime.Now;
                                currentTask.AccessRequest.ProgressStatus = AccessRequestProgress.RequestRejected;

                                if (currentTask.AccessRequest.ApplicationType == ApplicationType.Office365 && currentTask.AccessRequest.AccountType == AccountType.User)
                                {
                                    utility.DeleteAutoCreatedADForVPN(currentTask.AccessRequest, currentTask.AccessRequest.AdHocAccessRequest);
                                }
                            }
                            else if (currentTask.AccessRequest.RequestStatus == AccessRequestStatus.Withdrawn)
                            {
                                if (currentTask.AccessRequest.ApplicationType == ApplicationType.Office365 && currentTask.AccessRequest.AccountType == AccountType.User)
                                {
                                    utility.DeleteAutoCreatedADForVPN(currentTask.AccessRequest, currentTask.AccessRequest.AdHocAccessRequest);
                                }
                            }
                        }

                        currentTask.AccessRequest.Save(MasterConfigHelper_ServiceUser);
                    }
                }
                else if (task.RequestTypeId == (short)RequestType.BulkRequest)
                {
                    var currentTask = Task.GetBulkRequestTask(task.Id);
                    if (currentTask != null)
                    {
                        if (currentTask.BulkRequest.ApplicationType == ApplicationType.InhouseSystems)
                        {
                            if (currentTask.BulkRequest.RequestStatus == AccessRequestStatus.Approved)
                            {
                                currentTask.BulkRequest.LastModifiedBy = "System";

                                if (currentTask.BulkRequest.EndpointSystemId > 0)
                                {
                                    var endpoint = EndPoint.GetById(currentTask.BulkRequest.EndpointSystemId);
                                    if (endpoint != null && endpoint.CanAutomate)
                                    {
                                        currentTask.BulkRequest.ExecutionTime = currentTask.BulkRequest.ActivationDate.Value <= DateTime.Now ? DateTime.Now : currentTask.BulkRequest.ActivationDate.Value;
                                    }
                                    else
                                    {
                                        currentTask.BulkRequest.ExecutionTime = DateTime.Now;
                                    }
                                }

                                currentTask.BulkRequest.LastModifiedDate = DateTime.Now;
                                currentTask.BulkRequest.RequestStatus = AccessRequestStatus.Granted;
                                currentTask.BulkRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                            }
                            else if (currentTask.BulkRequest.RequestStatus == AccessRequestStatus.Rejected)
                            {
                                currentTask.BulkRequest.LastModifiedBy = "System";
                                currentTask.BulkRequest.LastModifiedDate = DateTime.Now;
                                currentTask.BulkRequest.ProgressStatus = AccessRequestProgress.RequestRejected;
                            }
                        }
                        else if (currentTask.BulkRequest.ApplicationType == ApplicationType.VPN)
                        {
                            if (currentTask.BulkRequest.RequestStatus == AccessRequestStatus.Approved)
                            {
                                currentTask.BulkRequest.LastModifiedBy = "System";
                                currentTask.BulkRequest.LastModifiedDate = DateTime.Now;

                                currentTask.BulkRequest.ExecutionTime = currentTask.BulkRequest.ActivationDate.Value <= DateTime.Now ? DateTime.Now : currentTask.BulkRequest.ActivationDate.Value;
                                currentTask.BulkRequest.RequestStatus = AccessRequestStatus.Granted;

                                if (currentTask.BulkRequest.RequestSource == AccessRequestSource.AdhocRequest)
                                {
                                    currentTask.BulkRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                                }
                                else
                                {
                                    currentTask.BulkRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                                }
                            }
                            else if (currentTask.BulkRequest.RequestStatus == AccessRequestStatus.Withdrawn)
                            {
                                utility.DeleteAutoCreatedADForVPN(currentTask.BulkRequest);
                            }
                            else if (currentTask.BulkRequest.RequestStatus == AccessRequestStatus.Rejected)
                            {
                                currentTask.BulkRequest.LastModifiedBy = "System";
                                currentTask.BulkRequest.LastModifiedDate = DateTime.Now;
                                currentTask.BulkRequest.ProgressStatus = AccessRequestProgress.RequestRejected;

                                utility.DeleteAutoCreatedADForVPN(currentTask.BulkRequest);
                            }
                        }
                        else
                        {
                            if (currentTask.BulkRequest.RequestStatus == AccessRequestStatus.Approved)
                            {
                                currentTask.BulkRequest.LastModifiedBy = "System";
                                currentTask.BulkRequest.ExecutionTime = currentTask.BulkRequest.ActivationDate.Value <= DateTime.Now ? DateTime.Now : currentTask.BulkRequest.ActivationDate.Value;
                                currentTask.BulkRequest.LastModifiedDate = DateTime.Now;
                                currentTask.BulkRequest.RequestStatus = AccessRequestStatus.Granted;
                                currentTask.BulkRequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;

                                bool isReallocate = false;
                                if (currentTask.BulkRequest.ApplicationType == ApplicationType.EBS_CRM || currentTask.BulkRequest.ApplicationType == ApplicationType.EBS_ISU)
                                {
                                    var sapDetail = (AdHocEBSRequest)currentTask.BulkRequest.GetEntitlementDetail();
                                    isReallocate = sapDetail.IsReallocate;
                                }
                                else if (currentTask.BulkRequest.ApplicationType == ApplicationType.MSSL_CRM || currentTask.BulkRequest.ApplicationType == ApplicationType.MSSL_ISU)
                                {
                                    var sapDetail = (AdHocMSSLRequest)currentTask.BulkRequest.GetEntitlementDetail();
                                    isReallocate = sapDetail.IsReallocate;
                                }
                                if (currentTask.BulkRequest.ApplicationType == ApplicationType.EBS || currentTask.BulkRequest.ApplicationType == ApplicationType.MSSL)
                                {
                                    var sapDetail = (AdHocSAPHANARequest)currentTask.BulkRequest.GetEntitlementDetail();
                                    isReallocate = sapDetail.IsReallocate;
                                }
                                if (isReallocate)
                                {
                                    currentTask.BulkRequest.ExecutionTime = currentTask.BulkRequest.ExecutionTime.Value.Date.AddHours(24);
                                }
                            }
                            else if (currentTask.BulkRequest.RequestStatus == AccessRequestStatus.Rejected)
                            {
                                currentTask.BulkRequest.LastModifiedBy = "System";
                                currentTask.BulkRequest.LastModifiedDate = DateTime.Now;
                                currentTask.BulkRequest.ProgressStatus = AccessRequestProgress.RequestRejected;
                                if (currentTask.BulkRequest.ApplicationType == ApplicationType.Office365)
                                {
                                    utility.DeleteAutoCreatedADForVPN(currentTask.BulkRequest);
                                }
                            }
                        }

                        currentTask.BulkRequest.Save(MasterConfigHelper_ServiceUser);
                    }
                }
                else if (task.RequestTypeId == (short)RequestType.EmployeeEvent)
                {
                    var currentTask = Task.GetEmployeeEventTask(task.Id);
                    if (currentTask != null)
                    {
                        var trigger = currentTask.EmployeeEventTrigger;
                        var user = User.GetById(currentTask.EmployeeEventTrigger.UserId);
                        if (currentTask.EmployeeEventTrigger.EventType == EmployeeEvents.NonEmployeeCreation)
                        {

                            if (user == null)
                            {
                                user = User.GetById(currentTask.EmployeeEventTrigger.UserId);
                            }
                            if (currentTask.EmployeeEventTrigger.RequestStatus == AccessRequestStatus.Approved)
                            {
                                user.RequestStatus = AccessRequestStatus.Approved;
                                user.IsActive = true;
                                user.LastModifiedBy = "System";
                                user.LastModifiedDate = DateTime.Now;
                                user.Reserved4 = Constants.EmployeeStatus.OnBoarded;
                                user.Save(MasterConfigHelper_ServiceUser);

                                User.InsertUserRole(user.Id, MasterConfigHelper.RoleUser);
                                User approver = null;
                                var requester = currentTask.EmployeeEventTrigger.CreatedBy;
                                if (!string.IsNullOrWhiteSpace(requester))
                                {
                                    approver = User.GetByUserName(requester);
                                }
                                this.TrackEventTriggers(user);
                                this.SendEmailForUser(user, approver);

                                trigger.RequestStatus = AccessRequestStatus.Granted;
                                trigger.IsActive = true;
                                trigger.IsRequestCreated = true;
                                trigger.LastModifiedBy = "System";
                                trigger.LastModifiedDate = DateTime.Now;

                                trigger.Save(MasterConfigHelper_ServiceUser);
                                this.SendEmailForEmployeeEventApproved(trigger, currentTask.Comments, currentTask.ActionUserId.Value);
                            }
                            else if (currentTask.EmployeeEventTrigger.RequestStatus == AccessRequestStatus.Rejected)
                            {
                                if (user.IsActive == false && currentTask.EmployeeEventTrigger.OldUserDetails != null)
                                {
                                    user = UtilityMethods.Deserialize<User>(currentTask.EmployeeEventTrigger.OldUserDetails);
                                }
                                user.RequestStatus = AccessRequestStatus.Rejected;
                                user.IsActive = false;
                                user.LastModifiedBy = "System";
                                user.LastModifiedDate = DateTime.Now;

                                user.Save(MasterConfigHelper_ServiceUser);
                                this.SendEmailForEmployeeEventRejected(trigger);
                            }
                            else if (currentTask.Action == StatusType.Withdrawn || currentTask.EmployeeEventTrigger.RequestStatus == AccessRequestStatus.Withdrawn)
                            {
                                if (currentTask.EmployeeEventTrigger.OldUserDetails != null)
                                {
                                    var olduser = UtilityMethods.Deserialize<User>(currentTask.EmployeeEventTrigger.OldUserDetails);
                                    if (olduser != null)
                                    {
                                        user.DisplayName = olduser.DisplayName;
                                        user.KnownAs = olduser.KnownAs;
                                        user.LastName = olduser.LastName;
                                        user.TerminationDate = olduser.TerminationDate;
                                    }
                                }
                                user.RequestStatus = AccessRequestStatus.Withdrawn;
                                user.LastModifiedBy = "System";
                                user.LastModifiedDate = DateTime.Now;
                                user.Save(MasterConfigHelper_ServiceUser);
                            }
                        }
                        else if (currentTask.EmployeeEventTrigger.EventType == EmployeeEvents.NonEmployeeExtension)
                        {
                            if (currentTask.EmployeeEventTrigger.RequestStatus == AccessRequestStatus.Approved)
                            {
                                var serializedUser = UtilityMethods.Deserialize<User>(currentTask.EmployeeEventTrigger.UserDetails);
                                try
                                {
                                    user.DisplayName = serializedUser.DisplayName;
                                    user.KnownAs = serializedUser.KnownAs;
                                    user.LastName = serializedUser.LastName;
                                    user.JoinDate = serializedUser.JoinDate;
                                    user.TerminationDate = serializedUser.TerminationDate;
                                }
                                catch (Exception ex)
                                {
                                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Non-Employee Extension Approval data update failed", ex);
                                }
                                user.IsActive = true;
                                user.RequestStatus = AccessRequestStatus.Approved;
                                user.LastModifiedBy = "System";
                                user.LastModifiedDate = DateTime.Now;
                                user.Reserved4 = Constants.EmployeeStatus.OnBoarded;
                                user.Save(MasterConfigHelper_ServiceUser);
                                this.TrackEventTriggers(user);

                                trigger.RequestStatus = AccessRequestStatus.Granted;
                                trigger.IsActive = true;
                                trigger.LastModifiedBy = "System";
                                trigger.LastModifiedDate = DateTime.Now;

                                trigger.Save(MasterConfigHelper_ServiceUser);
                                EmployeeEventTrigger.CreatePropagationTrigger(user.Id, MasterConfigHelper.ServiceUser);
                                this.SendEmailForEmployeeEventApproved(trigger, currentTask.Comments, currentTask.ActionUserId);
                            }
                            else if (currentTask.EmployeeEventTrigger.RequestStatus == AccessRequestStatus.Rejected)
                            {
                                user.IsActive = true;
                                user.RequestStatus = AccessRequestStatus.Rejected;
                                user.LastModifiedBy = "System";
                                user.LastModifiedDate = DateTime.Now;

                                user.Save(MasterConfigHelper_ServiceUser);
                                this.SendEmailForEmployeeEventRejected(trigger);
                            }
                            else if(currentTask.Action == StatusType.Withdrawn || currentTask.EmployeeEventTrigger.RequestStatus == AccessRequestStatus.Withdrawn)
                            {
                                if (currentTask.EmployeeEventTrigger.OldUserDetails != null)
                                {
                                    var olduser = UtilityMethods.Deserialize<User>(currentTask.EmployeeEventTrigger.OldUserDetails);
                                    if (olduser != null)
                                    {
                                        user.DisplayName = olduser.DisplayName;
                                        user.KnownAs = olduser.KnownAs;
                                        user.LastName = olduser.LastName;
                                        user.TerminationDate = olduser.TerminationDate;
                                    }
                                    user.RequestStatus = AccessRequestStatus.Withdrawn;
                                    user.LastModifiedBy = "System";
                                    user.LastModifiedDate = DateTime.Now;
                                    user.Save(MasterConfigHelper_ServiceUser);
                                }
                            }
                        }

                        User.DoAfterSaveUser(user, MasterConfigHelper_ServiceUser);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, "Error while executing the GrantProvisioning activity", ex);
            }
        }

        private void TrackEventTriggers(User user)
        {
            //trigger off board event if the termination date is greater than equal to today during first time discovery
            if (user.TerminationDate.HasValue)
            {
                //if (user.TerminationDate.Value.Date >= DateTime.Today)
                //{
                this.PopulateOffBoardTrigger(user);
                //}
            }
        }

        private void PopulateOffBoardTrigger(User user)
        {
            var evnt = EmployeeEvents.Resignation;
            var trigger = EmployeeEventTrigger.GetByUserIdAndEvent(user.Id, evnt);

            if (trigger != null)
            {
                trigger.EffectiveDate = user.TerminationDate.Value.Date.Equals(DateTime.MaxValue.Date) ? user.TerminationDate.Value : user.TerminationDate.Value.Date.AddHours(Constants.MaxHour);
                trigger.LastModifiedBy = trigger.CreatedBy;
                trigger.LastModifiedDate = DateTime.Now;

                trigger.Save(MasterConfigHelper_ServiceUser);
            }
            else
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    CreatedBy = user.CreatedBy,
                    CreatedDate = user.CreatedDate.Value,
                    EffectiveDate = user.TerminationDate.Value.Date.Equals(DateTime.MaxValue.Date) ? user.TerminationDate.Value : user.TerminationDate.Value.Date.AddHours(Constants.MaxHour),
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true,
                    RequestStatus = AccessRequestStatus.Submitted
                };

                newTrigger.Save(MasterConfigHelper_ServiceUser);
            }
        }

        public bool SendEmailForUser(User user,User toMail = null)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();
                MailTemplate template = utility.GetMailTemplate(Constants.NotificationForNewUser);
                User ro = null;
                if (toMail != null)
                {
                    ro = toMail;
                }

                if (ro == null)
                {
                    ro = User.GetROByUser(user.Id);
                    if(ro == null)
                        return false;
                }
                StringBuilder mailsubject = new StringBuilder(template.MailSubject);
                mailsubject.Replace("@@userDisplayName@@", user.DisplayName);
                mailsubject.Replace("@@pernrNum@@", user.EmployeeId);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@supervisorName@@", ro.DisplayName);
                body.Replace("@@userDisplayName@@", user.DisplayName);
                body.Replace("@@pernrNum@@", user.EmployeeId);

                body.Replace("@@coyCode@@", user.CompanyCode);
                body.Replace("@@coyText@@", user.CompanyName);
                body.Replace("@@deptCode@@", user.DepartmentCode);
                body.Replace("@@deptText@@", user.DepartmentName);
                body.Replace("@@divCode@@", user.DivisionCode);
                body.Replace("@@divText@@", user.DivisionName);
                body.Replace("@@orgCode@@", user.SectionCode);
                body.Replace("@@orgText@@", user.SectionName);
                body.Replace("@@cctrCode@@", user.CostCenter);
                body.Replace("@@cctrText@@", user.CostCenterText);
                body.Replace("@@ValidFrom@@", user.JoinDate?.ToShortDateString());
                body.Replace("@@ValidTo@@", user.TerminationDate?.ToShortDateString());
                body.Replace("@@ReferenceUser@@", utility.GetUserByUserId(user.ReferenceUserId)?.DisplayName);


                body.Replace("@@coyCode@@", user.CompanyCode);
                body.Replace("@@coyText@@", user.CompanyName);
                body.Replace("@@deptCode@@", user.DepartmentCode);
                body.Replace("@@deptText@@", user.DepartmentName);
                body.Replace("@@branchCode@@", user.BranchCode);
                body.Replace("@@branchText@@", user.OrgBranch);

                body.Replace("@@sectionCode@@", user.SectionCode);
                body.Replace("@@sectionText@@", user.OrgBranch);

                body.Replace("@@unitCode@@", user.UnitCode);
                body.Replace("@@unitText@@", user.OrgUnit);

                //body.Replace("@@divCode@@", user.DivisionCode);
                //body.Replace("@@divText@@", user.DivisionName);
                body.Replace("@@orgCode@@", user.OrganisationCode);
                body.Replace("@@orgText@@", getOrganisationText(user.OrganisationCode));


                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = mailsubject.ToString();
                emailInfo.To = ro.EmailId;
                emailInfo.From = template.MailFrom;
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string getOrganisationText(string orgCode)
        {
            if (string.IsNullOrWhiteSpace(orgCode)) return string.Empty;

            var org = NOrganisation.GetByOrgCode(orgCode);

            if (org == null) return string.Empty;
            else
                return org.Description;
        }

        public bool SendEmailForEmployeeEventApproved(EmployeeEventTrigger trigger, string reason, long? approverId)
        {
            try
            {

                EmailInfo emailInfo = new EmailInfo();
                MailTemplate template = utility.GetMailTemplate("NotifyEmployeeEventTaskApproved");
                var requester = User.GetByUserName(trigger.CreatedBy);
                var RequesteeUser = utility.GetUserByUserId(trigger.UserId);
                User approver = null;
                if (approverId.HasValue)
                    approver = User.GetById(approverId.Value);

                StringBuilder subject = new StringBuilder(template.MailSubject);

                string requestType = string.Empty;

                if (trigger.EventType == EmployeeEvents.NonEmployeeCreation)
                {
                    requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeCreation);
                }
                else if (trigger.EventType == EmployeeEvents.NonEmployeeExtension)
                {
                    requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeExtension);
                }

                subject.Replace("@@requestType@@", requestType);

                subject.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                subject.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@requestType@@", requestType);
                body.Replace("@@APPROVER@@", approver?.DisplayName);
                body.Replace("@@requesterDisplayName@@", requester.DisplayName);
                body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);
                body.Replace("@@EMPTYPE@@", RequesteeUser.EmployeeType.GetDescription());
                body.Replace("@@COMMENTS@@", reason);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = requester.EmailId;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

                throw ex;
            }
        }

        private bool SendEmailForEmployeeEventRejected(EmployeeEventTrigger trigger)
        {
            try
            {

                EmailInfo emailInfo = new EmailInfo();
                MailTemplate template = utility.GetMailTemplate("NotifyEmployeeEventTaskRejected");
                var requester = User.GetByUserName(trigger.CreatedBy);
                var RequesteeUser = utility.GetUserByUserId(trigger.UserId);

                StringBuilder subject = new StringBuilder(template.MailSubject);

                string requestType = string.Empty;

                if (trigger.EventType == EmployeeEvents.NonEmployeeCreation)
                {
                    requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeCreation);
                }
                else if (trigger.EventType == EmployeeEvents.NonEmployeeExtension)
                {
                    requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeExtension);
                }

                subject.Replace("@@requestType@@", requestType);

                subject.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                subject.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@requestType@@", requestType);

                body.Replace("@@requesterDisplayName@@", requester.DisplayName);
                body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = requester.EmailId;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

                throw ex;
            }
        }

    }
}
