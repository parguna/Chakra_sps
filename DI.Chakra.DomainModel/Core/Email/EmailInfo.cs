using DI.Chakra.Dto;
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

namespace DI.Chakra.DomainModel
{
    public class EmailInfo : DomainModelBase<EmailInfo, EntityModel.EmailInfo>
    {
        public string ActivityType { get; set; }

        public string Priority { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; set; }

        public string Attachment { get; set; }

        public string Status { get; set; }


        public DateTime? CreatedDateTime { get; set; }

        public DateTime? DispatchDateTime { get; set; }

        public DateTime? SentDateTime { get; set; }

        public int NoOfAttempts { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }



        public EmailInfo()
        {
            this.CreatedDateTime = DateTime.Now;
            this.Priority = "Normal";
            this.Status = "Pending";
            this.IsHtml = true;
            this.DispatchDateTime = DateTime.Now;
        }

        public override long Save(string currentUser)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(currentUser))
            {
                EmailInfo emailInfo = new EmailInfo();
                emailInfo.ActivityType = this.ActivityType ?? string.Empty;
                emailInfo.Priority = this.Priority ?? string.Empty;
                emailInfo.From = this.From ?? string.Empty;
                emailInfo.To = this.To ?? string.Empty;
                emailInfo.Cc = this.Cc ?? string.Empty;
                emailInfo.Bcc = this.Bcc ?? string.Empty;
                emailInfo.Subject = this.Subject ?? string.Empty;
                emailInfo.Body = this.Body ?? string.Empty;
                emailInfo.Attachment = this.Attachment ?? string.Empty;
                emailInfo.Status = this.Status ?? string.Empty;
                emailInfo.CreatedBy = this.CreatedBy ?? string.Empty;
                emailInfo.CreatedDateTime = (this.CreatedDateTime == DateTime.MinValue) ? (DateTime?)null : this.CreatedDateTime;
                emailInfo.SentDateTime = (this.SentDateTime == DateTime.MinValue) ? (DateTime?)null : this.SentDateTime;
                base.Save(tranaction);
                tranaction.Commit();
            }
            return 0;
        }
        public static EmailInfo GetEmailInfoForSendingEmailForBulkRequest(Task currentTask)
        {
            EmailInfo emailInfo = null;
            string appName = string.Empty;
            try
            {
                emailInfo = new EmailInfo();
                MailTemplate template = MailTemplate.GetByTemplateName("NotifyBulkRequestApproval");
                if (template != null)
                {
                    var user = User.GetById(currentTask.ActionUserId.Value);
                    var requester = User.GetById(currentTask.BulkRequest.RequestedBy);

                    

                    string accounttype = string.Format("{0}", UtilityMethods.GetEnumAttribute<Description>(AccountType.User));

                    if (string.IsNullOrEmpty(user.EmailId))
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Notification email is not sent to {0} since he has no valid email id", user.UserName), null);
                        return null;
                    }
                    // TODO:
                    // Need to change this code
                    if (currentTask.BulkRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                           currentTask.BulkRequest.ApplicationType == ApplicationType.MSSL_ISU)
                    {
                        appName = Constants.MSSL_ISU_CREM;
                    }
                    else if (currentTask.BulkRequest.ApplicationType == ApplicationType.EBS_CRM ||
                        currentTask.BulkRequest.ApplicationType == ApplicationType.EBS_ISU)
                    {
                        appName = Constants.EBS_ISU_CREM;
                    }
                    else
                    {
                        appName = UtilityMethods.GetEnumAttribute<Description>(currentTask.BulkRequest.ApplicationType);
                    }

                    StringBuilder subject = new StringBuilder(template.MailSubject);
                    subject.Replace("@@applicationName@@", appName);
                    subject.Replace("@@requestType@@", currentTask.BulkRequest.AdhocActionType.ToString());
                    subject.Replace("@@requestNum@@", currentTask.BulkRequest.Id.ToString());

                    subject.Replace("@@RECEIVERID@@", requester.EmployeeId.ToString());

                    string displayName = string.Join(",", currentTask.BulkRequest.BulkRequestDetails.Select(item => item.Name).ToList());
                    string employeeId = string.Join(",", currentTask.BulkRequest.BulkRequestDetails.Select(item => item.EmployeeId).ToList());

                    StringBuilder body = new StringBuilder(template.MailContent);
                    body.Replace("@@approverName@@", user.DisplayName);
                    body.Replace("@@requestNum@@", currentTask.BulkRequest.Id.ToString());
                    body.Replace("@@applicationName@@", appName);
                    body.Replace("@@requestType@@", currentTask.BulkRequest.AdhocActionType.ToString());

                    body.Replace("@@userDisplayName@@", displayName);
                    body.Replace("@@pernrNum@@", employeeId);
                    body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);

                    body.Replace("@@AccountType@@", accounttype);

                    if (string.IsNullOrEmpty(currentTask.BulkRequest.Description))
                        body.Replace("@@COMMENTS@@", string.Empty);
                    else
                        body.Replace("@@COMMENTS@@", currentTask.BulkRequest.Description?.Replace("\n", "<br>"));

                    emailInfo.Body = body.ToString();
                    emailInfo.Subject = subject.ToString(); //string.Format(template.MailSubject, appName);
                    emailInfo.To = user.EmailId;
                    emailInfo.From = template.MailFrom;
                    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                }
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, $"GetEmailInfoForSendingEmailForBulkRequest: TaskId - {currentTask.Id}, BulkRequest : {currentTask.BulkRequestId}", ex,null,string.Empty);
                emailInfo = null;
            }
            return emailInfo;
        }

        public static EmailInfo GetEmailInfoForSendingEmailForProvisioningRequest(Task currentTask)
        {
            EmailInfo emailInfo = null;
            try
            {
                List<User> approvers = new List<User>();
                emailInfo = new EmailInfo();
                MailTemplate template = MailTemplate.GetByTemplateName("NotifyAdhocProvisioningApproval");
                if (template != null)
                {
                    if (currentTask.IsGroupTask)
                    {
                        if (currentTask.ActionUserId.HasValue)
                        {
                            approvers.Add(User.GetById(currentTask.ActionUserId.Value));
                        }
                        else
                        {
                            if (currentTask.ActionGroupId.HasValue)
                            {
                                var groups = Group.GetByIdIncludingUsers(currentTask.ActionGroupId.Value);
                                groups.Users.ForEach(i =>
                                {
                                    if (i.IsActive && !string.IsNullOrWhiteSpace(i.EmailId))
                                    {
                                        approvers.Add(i);
                                    }
                                });

                            }
                        }
                    }
                    else
                    {
                        approvers.Add(User.GetById(currentTask.ActionUserId.Value));
                    }
                    var user = string.Join(",",approvers.Select(i=> i.DisplayName));
                    var requester = User.GetById(currentTask.AccessRequest.AdHocAccessRequest.RequesterUserId);
                    var RequesteeUser = User.GetById(currentTask.AccessRequest.AdHocAccessRequest.RequesteeUserId);

                    string appName = currentTask.AccessRequest.AdHocAccessRequest.EndPoint.DisplayName;

                    string accounttype = string.Format("{0}", UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.AdHocAccessRequest.AccountType));

                    if (approvers.Count == 0)
                    {
                        DILog.EmailLogger.WriteLog(LogLevelType.Info, $"No approvers found : Task Id - {currentTask.Id}",null,string.Empty);
                        return null;
                    }

                    StringBuilder subject = new StringBuilder(template.MailSubject);
                    subject.Replace("@@applicationName@@", appName);
                    subject.Replace("@@requestType@@", currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                    subject.Replace("@@requestNum@@", currentTask.AccessRequestId.ToString());

                    subject.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                    subject.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                    subject.Replace("@@RECEIVERID@@", requester.EmployeeId.ToString());
                    var requestType = currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString();
                    if (currentTask.AttestationCampaignId.HasValue)
                    {
                        requestType = $"Attestation Remediation - {requestType}";
                    }
                    StringBuilder body = new StringBuilder(template.MailContent);
                    body.Replace("@@approverName@@", user);
                    body.Replace("@@requestNum@@", currentTask.AccessRequestId.ToString());
                    body.Replace("@@applicationName@@", appName);
                    body.Replace("@@requestType@@", requestType);
                    body.Replace("@@EMPLOYEETYPE@@", RequesteeUser.EmployeeType.ToString());
                    body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                    body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);
                    body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                    body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                    body.Replace("@@AccountType@@", accounttype);
                    if(currentTask.AccessRequest != null && (currentTask.AccessRequest.RequestSource == AccessRequestSource.OffBoarding || currentTask.AccessRequest.RequestSource == AccessRequestSource.OnBoarding))
                    {
                        body.Replace("@@REQUESTBY@@", "System");
                    }
                    else
                    {
                        body.Replace("@@REQUESTBY@@", requester.DisplayName);
                    }
                    if (string.IsNullOrEmpty(currentTask.AccessRequest.AdHocAccessRequest.Comments))
                        body.Replace("@@COMMENTS@@", string.Empty);
                    else
                        body.Replace("@@COMMENTS@@", currentTask.AccessRequest.AdHocAccessRequest.Comments?.Replace("\n", "<br>"));

                    emailInfo.Body = body.ToString();
                    emailInfo.Subject = subject.ToString(); //string.Format(template.MailSubject, appName);
                    emailInfo.To = string.Join(",", approvers.Select(i => i.EmailId));
                    emailInfo.From = template.MailFrom;
                    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                    
                }
                else
                {
                    DILog.EmailLogger.WriteLog(LogLevelType.Error, $"GetEmailInfoForSendingEmailForProvisioningRequest: Mail Template Not Found for the task : TaskId - {currentTask.Id}",null,string.Empty);
                    emailInfo = null;
                }
            }
            catch (Exception ex)
            {
                emailInfo = null;
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, ex.Message, ex,null,string.Empty);
            }
            return emailInfo;
        }
        public static EmailInfo GetEmailInfoForSendEmailForEmployeeEventApproved(Task currentTask, EmployeeEventTrigger trigger)
        {
            EmailInfo emailInfo = null;
            try
            {

                emailInfo = new EmailInfo();
                MailTemplate template = MailTemplate.GetByTemplateName("NotifyEmployeeEventTask");
                var user = User.GetById(currentTask.ActionUserId.Value);
                var requester = User.GetByUserName(trigger.CreatedBy);
                var RequesteeUser = User.GetById(trigger.UserId);

                if (string.IsNullOrEmpty(user.EmailId))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Notification email is not sent to {0} since he has no valid email id", user.UserName), null);
                    return null;
                }

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
                body.Replace("@@approverName@@", user.DisplayName);
                body.Replace("@@requestType@@", requestType);

                body.Replace("@@requesterDisplayName@@", requester.DisplayName);
                body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = user.EmailId;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

            }
            catch (Exception ex)
            {
                emailInfo = null;
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception,$"GetEmailInfoForSendEmailForEmployeeEventApproved : TaskID {currentTask.Id}", ex,null,string.Empty);
            }
            return emailInfo;
        }
        public static EmailInfo GetEmailInfoForAdvanceDelegation(Dto.TaskDelegation task)
        {
            EmailInfo emailInfo = null;
            try
            {

                emailInfo = new EmailInfo();
                MailTemplate template = MailTemplate.GetByTemplateName(Constants.AdvanceDelegationNotification);
                var delegatedTo = User.GetById(task.DelegatedTo);
                var delegatedFrom = User.GetById(task.ActionUser);
                var delegatedBy = User.GetByUserName(task.CreatedBy);
                User delegationModifiedBy = null;
                if (!string.IsNullOrWhiteSpace(task.LastModifiedBy))
                {
                    delegationModifiedBy = User.GetByUserName(task.LastModifiedBy);
                }
                var delegatedFromRO = User.GetByEmployeeId(delegatedFrom.ROEmployeeId);
                var toAddress = string.Empty;
                if (delegatedTo != null && string.IsNullOrEmpty(delegatedTo.EmailId))
                {
                    DILog.EmailLogger.WriteLog(LogLevelType.Info, $"For the delegation task (TaskId : {task.Id}) AdvanceDelegation Notification email is not sent to {delegatedTo.UserName} since he has no valid email id", null,string.Empty);
                    return null;
                }
                toAddress = delegatedTo.EmailId;
                var bccAddress = string.Empty;
                if (delegatedFromRO != null && !string.IsNullOrWhiteSpace(delegatedFromRO.EmailId))
                {
                    bccAddress = delegatedFromRO.EmailId;
                }
                else
                {
                    DILog.EmailLogger.WriteLog(LogLevelType.Info,  $"For the delegation task(TaskId: { task.Id}) AdvanceDelegation Notification email is not sent to RO since his:{delegatedFrom.UserName} RO:{delegatedFromRO?.UserName} email id is empty" , null,string.Empty);
                }

                if (delegatedFrom != null && !string.IsNullOrWhiteSpace(delegatedFrom.EmailId))
                {
                    if (string.IsNullOrWhiteSpace(bccAddress)) bccAddress = delegatedFrom.EmailId;
                    else
                    {
                        bccAddress = bccAddress + "," + delegatedFrom.EmailId;
                    }
                }
                
                StringBuilder subject = new StringBuilder(template.MailSubject);

                string requestType = string.Empty;

               
                subject.Replace("@@FROM_DELETAK@@", delegatedFrom.DisplayName);

                subject.Replace("@@TO_DELETAK@@", delegatedTo.DisplayName);
                subject.Replace(" @@ADDELEGATED_USER@@", delegatedBy?.DisplayName);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@APPROVER_DELTSK@@", delegatedTo.DisplayName);
                body.Replace("@@ADDELEGATED_USER@@", delegatedBy.DisplayName);
                body.Replace("@@FROM_DELETAK@@", delegatedFrom.DisplayName);
                body.Replace("@@FROMDATE@@", task.StartDate?.MailDateTimeFormat());
                body.Replace("@@TODATE@@", task.EndDate?.MailDateTimeFormat());
                body.Replace("@@TO_DELETAK@@", delegatedTo.DisplayName);
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@DELEGATIONCOMMENT@@", task.DelegationComments);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@PROVISION@@", task.DelegateProvisioningTask ? "Yes":"No");
                body.Replace("@@ATTESTATION@@", task.DelegateAttestationTask ? "Yes" : "No");
                body.Replace("@@CREATEDDATE@@",task.CreatedDate.MailDateTimeFormat() );
                body.Replace("@@MODIFIEDDATE@@", task.LastModifiedDate?.MailDateTimeFormat());
                
                if (delegationModifiedBy != null)
                {
                    body.Replace("@@ModifiedBy@@", delegationModifiedBy?.DisplayName);
                    body.Replace("@@ModifiedByText@@", $"Modified by {delegationModifiedBy?.DisplayName}");
                }
                else
                {
                    body.Replace("@@ModifiedBy@@", "");
                    body.Replace("@@ModifiedByText@@","");
                }
                
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = toAddress;
                emailInfo.From = template.MailFrom;
                emailInfo.Cc = bccAddress;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

            }
            catch (Exception ex)
            {
                emailInfo = null;
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, $"Task Id :{task.Id} GetEmailInfoForSendEmailForEmployeeEventApproved {ex.Message}", ex,null,string.Empty);
            }
            return emailInfo;
        }
        public static EmailInfo GetEmailInfoForDelegationTask(Task currentTask, string delegationMessage)
        {
            EmailInfo emailInfo = null;
            try
            {
                var wf = WorkflowConfiguration.GetWorkflowConfigurationDetailById(currentTask.WorkflowConfigurationId)?.WorkflowConfigurationDetails?.FirstOrDefault(i=>i.SequenceNumber == currentTask.WorkflowStateId);
                List<User> approvers = new List<User>();
                emailInfo = new EmailInfo();
                MailTemplate template = MailTemplate.GetByTemplateName("DelegateAttestationTask");
                if (template != null)
                {
                    if (currentTask.IsGroupTask)
                    {
                        if (currentTask.ActionUserId.HasValue)
                        {
                            approvers.Add(User.GetById(currentTask.ActionUserId.Value));
                        }
                        else
                        {
                            if (currentTask.ActionGroupId.HasValue)
                            {
                                var groups = Group.GetByIdIncludingUsers(currentTask.ActionGroupId.Value);
                                groups.Users.ForEach(i =>
                                {
                                    if (i.IsActive && !string.IsNullOrWhiteSpace(i.EmailId))
                                    {
                                        approvers.Add(i);
                                    }
                                });

                            }
                        }
                    }
                    else
                    {
                        approvers.Add(currentTask.ActionUser);
                    }
                    if (approvers.Count == 0)
                    {
                        DILog.EmailLogger.WriteLog(LogLevelType.Info, $"No approvers found : Task Id - {currentTask.Id}", null, string.Empty);
                        return null;
                    }
                    var user = string.Join(",", approvers.Select(i => i.DisplayName));
                    var requester = currentTask.Delegated;
                    var attestionEp = AttestationEndpoint.GetByAttestationId(currentTask.AttestationId.Value);
                    string appName = string.Join(",", attestionEp.Select(i => i.EndPoint.DisplayName));

                    var campaignName = currentTask.AttestationCampaign?.CampaignName;
                    StringBuilder subject = new StringBuilder(template.MailSubject);
                    subject.Replace("@@DELEGATED_USER@@", requester.DisplayName);
                    subject.Replace("@@ATTESTATIONCAMPAIGN_NAME@@", campaignName);
                    subject.Replace("@@CAMPAIGN_NAME@@", campaignName);
                    subject.Replace("@@TASKID@@", currentTask.Id.ToString());

                    StringBuilder body = new StringBuilder(template.MailContent);
                    body.Replace("@@APPROVER@@", user);
                    body.Replace("@@REVIEWER_NAME@@", user);
                    body.Replace("@@DELEGATED_USER@@", requester.DisplayName);
                    body.Replace("@@ATTESTATION_ENDPOINT@@", appName);
                    body.Replace("@@DELEGATER_COMMENTS@@", delegationMessage);
                    body.Replace("@@ATTESTATION_NAME@@", currentTask.Attestation?.AttestationName);
                    body.Replace("@@ATTESTATION_DESC@@", currentTask.Attestation?.AttestationDescription);
                    body.Replace("@@ATTTSK_CAMPAIGN_NAME@@", campaignName);
                    body.Replace("@@CAMPAIGN_NAME@@", campaignName);
                    subject.Replace("@@CAMPAIGN_NAME@@", campaignName);
                    var enddate = currentTask.CreatedDate;
                    if(wf != null && wf.ResponseTimeInDays > 0 )
                    {
                        enddate = currentTask.CreatedDate.AddDays(wf.ResponseTimeInDays);
                    }
                    body.Replace("@@REVIEWLASTDATE@@", enddate.ToString("dd/MM/yyyy"));
                    body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                    body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);

                    emailInfo.Body = body.ToString();
                    emailInfo.Subject = subject.ToString(); //string.Format(template.MailSubject, appName);
                    emailInfo.To = $"{template.MailTo},{string.Join(",", approvers.Select(i => i.EmailId))}";
                    emailInfo.Bcc = template.MailBcc;
                    emailInfo.Cc = template.MailCC;
                    emailInfo.From = $"{template.MailFrom}";
                    emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                }
                else
                {
                    DILog.EmailLogger.WriteLog(LogLevelType.Error, $"GetEmailInfoForDelegationTask: Mail Template Not Found for the task : TaskId - {currentTask.Id}", null, string.Empty);
                    emailInfo = null;
                }
            }
            catch (Exception ex)
            {
                emailInfo = null;
                DILog.EmailLogger.WriteLog(null, LogLevelType.Exception, ex.Message, ex, null, string.Empty);
            }
            return emailInfo;
        }
    }
}
