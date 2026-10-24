
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.Services.EmailingProcessor;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace DI.Chakra.Workflow.Processor
{
    public class SendNotificationEmail
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();

        public SendEmailResponse Execute(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration)
        {
            RequestType requestType = RequestType.None;
            bool isMailSent = true;

            try
            {
                requestType = (RequestType)currentTask.RequestTypeId;

                if (requestType == RequestType.Attestation)
                {
                    AttestationCampaign attestationCampaign = null;
                    Attestation attestation = Attestation.GetByIdIncludingAttestationProcesses(currentTask.AttestationId.Value);
                    if (currentTask.AttestationCampaignId.HasValue)
                    {
                        attestationCampaign = AttestationCampaign.GetById(currentTask.AttestationCampaignId.Value);
                    }
                    isMailSent = SendEmailForAttestation(currentTask, currentWorkflowConfiguration, attestation, attestationCampaign?.CampaignName);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    var deepLoadedTask = Task.GetProvisioningTask(currentTask.Id);
                    isMailSent = SendEmailForAdhocProvisioning(deepLoadedTask);
                }
                else if (requestType == RequestType.BulkRequest)
                {
                    var deepLoadedTask = Task.GetBulkRequestTask(currentTask.Id);
                    isMailSent = SendEmailForBulkRequest(deepLoadedTask);
                }
                else if (requestType == RequestType.EmployeeEvent)
                {
                    var trigger = EmployeeEventTrigger.GetById(currentTask.EmployeeEventTriggerId.Value);
                    isMailSent = SendEmailForEmployeeEvent(currentTask, currentWorkflowConfiguration, trigger);
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the SendNotificationEmail activity.";
                if (requestType == RequestType.Attestation)
                {
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Exception, message, ex);
                }
                else
                {
                    DILog.WorkflowLogger.WriteLog(LogLevelType.Exception, message, ex);
                }
            }

            return new SendEmailResponse()
            {
                IsMailSent = isMailSent
            };
        }

        bool SendEmailForAttestation(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration, Attestation attestation,string campaignName)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();
                List<User> approvers = new List<User>();
                var receiver = string.Empty;
                MailTemplate template = utility.GetMailTemplate("NotifyAttestationApproval");
                if (template != null)
                {
                    if (currentTask.IsGroupTask)
                    {
                        if (currentTask.ActionUserId.HasValue)
                        {
                            var usr = User.GetById(currentTask.ActionUserId.Value);
                            receiver = usr?.DisplayName;
                            if(!string.IsNullOrWhiteSpace(usr.EmailId))
                                approvers.Add(usr);
                        }
                        else
                        {
                            if (currentTask.ActionGroupId.HasValue)
                            {
                                var groups = Group.GetByIdIncludingUsers(currentTask.ActionGroupId.Value);
                                receiver = groups?.GroupName;
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
                        var usr = User.GetById(currentTask.ActionUserId.Value);
                        receiver = usr?.DisplayName;
                        if(usr != null)
                            approvers.Add(usr);
                        
                    }
                    //if (approvers.Count == 0)
                    //{
                    //    DILog.EmailLogger.WriteLog(LogLevelType.Info, $"No approvers found");
                    //    return false;
                    //}
                    if (string.IsNullOrWhiteSpace(receiver))
                    {
                        receiver = "MAIL RECEIVER";
                    }
                    var user = string.Join(",", approvers.Select(i => i.DisplayName));
                    StringBuilder subject = new StringBuilder(template.MailSubject);
                    subject.Replace("@@APPROVAL_LEVEL@@", currentTask.WorkflowStateId.ToString());
                    subject.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
                    subject.Replace("@@CAMPAIGN_NAME@@", campaignName);

                    StringBuilder body = new StringBuilder(template.MailContent);
                    body.Replace("@@REVIEWER_NAME@@", receiver);
                    body.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
                    body.Replace("@@CAMPAIGN_NAME@@", campaignName);
                    body.Replace("@@ATTESTATION_DESC@@", attestation.AttestationDescription);
                    body.Replace("@@APPROVAL_LEVEL@@", currentTask.WorkflowStateId.ToString());
                    body.Replace("@@REVIEWLASTDATE@@", utility.GetTaskCompletionDate(currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays, currentTask.CreatedDate).MailDateFormat());
                    body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                    body.Replace("@@DELEGATIONCOMMENTS@@", !string.IsNullOrWhiteSpace(currentTask.DelegationComments) ? currentTask.DelegationComments :"-");
                    body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                    
                    emailInfo.Body = body.ToString();
                    emailInfo.Subject = subject.ToString();
                    emailInfo.To = $"{string.Join(",", approvers.Select(i => i.EmailId))},{template.MailTo}";
                    emailInfo.From = template.MailFrom;
                    emailInfo.Cc = template.MailCC;
                    emailInfo.ActivityType = EmailActivityType.Attestation.ToString();

                    EmailHandler.Send(emailInfo);
                    return true;
                }
                else
                {
                    DILog.EmailLogger.WriteLog(LogLevelType.Warning, "NotifyAttestationApproval Mail Template Not Found");
                    return false;
                }
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

                throw ex;
            }
        }

        public bool SendEmailForEmployeeEvent(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration, EmployeeEventTrigger trigger)
        {
            try
            {

                EmailInfo emailInfo = new EmailInfo();
                
                MailTemplate template = utility.GetMailTemplate("NotifyEmployeeEventTask");
                if (template != null)
                {
                    var user = utility.GetUserByUserId(currentTask.ActionUserId.Value);
                    var requester = User.GetByUserName(trigger.CreatedBy);
                    var RequesteeUser = utility.GetUserByUserId(trigger.UserId);
                    var iacmUserInfo = IACMUserInfo.GetByEmployeeId(RequesteeUser.EmployeeId);
                    User refUser = User.GetById(RequesteeUser.ReferenceUserId);
                    
                    if (string.IsNullOrEmpty(user.EmailId))
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Notification email is not sent to {0} since he has no valid email id", user.UserName), null);
                        return false;
                    }

                    StringBuilder subject = new StringBuilder(template.MailSubject);

                    string requestType = string.Empty;
                    var toDate = "";
                    if (trigger.EventType == EmployeeEvents.NonEmployeeCreation)
                    {
                        requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeCreation);
                        toDate = iacmUserInfo.DeletedDate.Value.ToString("dd-MM-yyyy");
                    }
                    else if (trigger.EventType == EmployeeEvents.NonEmployeeExtension)
                    {
                        requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeExtension);
                        toDate = trigger.EffectiveDate.ToString("dd-MM-yyyy");
                        if (!string.IsNullOrWhiteSpace(trigger.UserDetails))
                        {
                            var usr = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                            if(usr != null && usr.TerminationDate.HasValue){
                                toDate = usr.TerminationDate.Value.ToString("dd-MM-yyyy");
                            }
                        }
                    }

                    subject.Replace("@@requestType@@", requestType);

                    subject.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                    subject.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                    StringBuilder body = new StringBuilder(template.MailContent);
                    body.Replace("@@approverName@@", user.DisplayName);
                    body.Replace("@@requestType@@", requestType);
                    body.Replace("@@EMPTYPE@@", RequesteeUser.EmployeeType.ToString());
                    body.Replace("@@requesterDisplayName@@", requester.DisplayName);
                    body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                    body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);
                    body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                    body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                    body.Replace("@@VALIDFROM@@", RequesteeUser.JoinDate.Value.ToString("dd-MM-yyyy"));
                    body.Replace("@@VALIDTO@@", toDate);
                    body.Replace("@@REFUSER@@", refUser.DisplayName);

                    emailInfo.Body = body.ToString();
                    emailInfo.Subject = subject.ToString();
                    emailInfo.To = user.EmailId;
                    emailInfo.From = template.MailFrom;
                    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                    EmailHandler.Send(emailInfo);
                    return true;
                }else
                {
                         DILog.EmailLogger.WriteLog(LogLevelType.Warning, "NotifyEmployeeEventTask Mail Template Not Found");
                        return false;
                    }
               
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

                throw ex;
            }
        }

        bool SendEmailForAdhocProvisioning(Task currentTask)
        {
            try
            {
                var emailInfo = EmailInfo.GetEmailInfoForSendingEmailForProvisioningRequest(currentTask);
                emailInfo.Save(MasterConfigHelper.ServiceUser);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SendEmailForBulkRequest(Task currentTask)
        {
            try
            {
                List<User> approvers = new List<User>();
                EmailInfo emailInfo = new EmailInfo();
                MailTemplate template = utility.GetMailTemplate("NotifyBulkRequestApproval");
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
                    var user = string.Join(",", approvers.Select(i => i.DisplayName));
                    //var user = utility.GetUserByUserId(currentTask.ActionUserId.Value);
                    var requester = utility.GetUserByUserId(currentTask.BulkRequest.RequestedBy);

                    string appName = currentTask.BulkRequest.EndPoint.DisplayName;

                    string accounttype = string.Format("{0}", UtilityMethods.GetEnumAttribute<Description>(AccountType.User));

                    if (approvers.Count == 0)
                    {
                        DILog.EmailLogger.WriteLog(LogLevelType.Info, $"No approvers found");
                        return false;
                    }

                    StringBuilder subject = new StringBuilder(template.MailSubject);
                    subject.Replace("@@applicationName@@", appName);
                    subject.Replace("@@requestType@@", currentTask.BulkRequest.AdhocActionType.ToString());
                    subject.Replace("@@requestNum@@", currentTask.BulkRequest.Id.ToString());

                    subject.Replace("@@RECEIVERID@@", requester.EmployeeId.ToString());

                    string displayName = string.Join(",", currentTask.BulkRequest.BulkRequestDetails.Select(item => $"{item.Name}({item.EmployeeId})").ToList());
                    string employeeId = string.Join(",", currentTask.BulkRequest.BulkRequestDetails.Select(item => item.EmployeeId).ToList());

                    StringBuilder body = new StringBuilder(template.MailContent);
                    body.Replace("@@approverName@@", user);
                    body.Replace("@@requestNum@@", currentTask.BulkRequest.Id.ToString());
                    body.Replace("@@applicationName@@", appName);
                    body.Replace("@@requestType@@", currentTask.BulkRequest.AdhocActionType.ToString());
                    body.Replace("@@REQUESTBY@@", requester.DisplayName);
                    body.Replace("@@userDisplayName@@", displayName);
                    body.Replace("@@pernrNum@@", employeeId);
                    body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);

                    body.Replace("@@AccountType@@", accounttype);

                    if (string.IsNullOrEmpty(currentTask.BulkRequest.Description))
                        body.Replace("@@COMMENTS@@", string.Empty);
                    else
                        body.Replace("@@COMMENTS@@", currentTask.BulkRequest.Description?.Replace("\n", "<br>"));
                    body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                    body.Replace("@@EMPTYPE@@", currentTask.BulkRequest.IsEmployee ? EmployeeType.Permanent.ToString() :"Non-Employee");
                    emailInfo.Body = body.ToString();
                    emailInfo.Subject = subject.ToString(); //string.Format(template.MailSubject, appName);
                    emailInfo.To = string.Join(",", approvers.Select(i => i.EmailId));
                    emailInfo.From = template.MailFrom;
                    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                    EmailHandler.Send(emailInfo);
                    return true;
                }
                else
                {
                    DILog.EmailLogger.WriteLog(LogLevelType.Warning, "NotifyBulkRequestApproval Mail Template Not Found");
                    return false;
                }
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);

                throw ex;
            }
        }
        private void GetApproverInformation(Task currentTask, out string emailIds, out string approverName)
        {
            emailIds = string.Empty;
            approverName = string.Empty;
            try
            {
                if (currentTask.IsGroupTask == false)
                {
                    if (currentTask.ActionUserId.HasValue)
                    {
                        var usr = utility.GetUserByUserId(currentTask.ActionUserId.Value);
                        emailIds = usr?.EmailId;
                        approverName = usr?.DisplayName;
                    }

                }
                else
                {
                    if (currentTask.ActionGroupId.HasValue)
                    {
                        approverName = currentTask.GroupName;
                        emailIds = $"{string.Join(",", User.GetUsetEmailIdsByGroupId(currentTask.ActionGroupId.Value))}";
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"GetApproverInformation:{ex.Message}", ex);
            }
        }

        public bool SendEmailForAttestationCompletion(AttestationCampaign attestationCampaign)
        {
            bool isMailSent = false;
            try
            {
                var attestation = Attestation.GetById(attestationCampaign.AttestationID);
                DILog.EmailLogger.WriteLog(LogLevelType.Info, $"SendEmailForAttestationCompletion from Workflow");
                EmailInfo emailInfo = new EmailInfo();
                MailTemplate template = utility.GetMailTemplate("NotifyAttestationComplete");
                StringBuilder subject = new StringBuilder(template.MailSubject);
                var iamTeam = string.Join(",", MasterConfigHelper.AlertNotificationReceivers);
                subject.Replace("@@CAMPAIGN_NAME@@", attestationCampaign.CampaignName);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
                body.Replace("@@ATTESTATION_DESC@@", $"{attestation.AttestationDescription}");
                body.Replace("@@CAMPAIGN_NAME@@", attestationCampaign.CampaignName);
                body.Replace("@@CAMPIGN_START_DATE@@", attestationCampaign?.CreatedDate.ToString("dd/MM/yyyy"));
                body.Replace("@@CAMPIGN_END_DATE@@", attestationCampaign.EndDate?.ToString("dd/MM/yyyy"));
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();

                emailInfo.Subject = subject.ToString();
                if (!string.IsNullOrWhiteSpace(template.MailCC))
                    emailInfo.Cc = template.MailCC;

                if (!string.IsNullOrWhiteSpace(template.MailBcc))
                    emailInfo.Bcc = template.MailBcc;
                if (string.IsNullOrWhiteSpace(template.MailTo))
                {
                    template.MailTo = iamTeam;
                }
                emailInfo.To = $"{template.MailTo}";
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {

            }
            return isMailSent;
        }
    }
}
