 
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.Services.EmailingProcessor;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Text;
using System.Linq;

namespace DI.Chakra.Workflow.Processor
{
    public class SendReminderEmail
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();

        public SendEmailResponse Execute(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration)
        {
            RequestType requestType = RequestType.None;
            bool isMailSent = true;

            try
            {
                requestType = (RequestType)currentTask.RequestTypeId;
                Task deepLoadedTask = null;
                if (requestType == RequestType.Attestation)
                {
                    isMailSent = true;
                    if (MasterConfigHelper.AllowAttestationTaskReminder == false)
                    {
                        DILog.EmailLogger.WriteLog(LogLevelType.Info, $"SendReminderEmailForAttestation from Workflow");
                        deepLoadedTask = Task.GetAttestationTask(currentTask.Id, true);
                        if (deepLoadedTask != null && deepLoadedTask.AttestationCampaign != null)
                        {
                            isMailSent = SendReminderEmailForAttestation(deepLoadedTask, currentWorkflowConfiguration);
                        }
                    }
                }
                else if (requestType == RequestType.Provisioning)
                {
                    isMailSent = true;
                    if (MasterConfigHelper.AllowProvisioningTaskReminder == false)
                    {
                        deepLoadedTask = Task.GetProvisioningTask(currentTask.Id);
                        isMailSent = SendEmailForAdhocProvisioning(deepLoadedTask, currentWorkflowConfiguration);
                    }
                }
                else if (requestType == RequestType.BulkRequest)
                {
                    isMailSent = true;
                    if (MasterConfigHelper.AllowProvisioningTaskReminder == false)
                    {
                        deepLoadedTask = Task.GetBulkRequestTask(currentTask.Id);
                        isMailSent = SendEmailForBulkRequest(currentTask, currentWorkflowConfiguration);
                    }
                }
                else if (requestType == RequestType.EmployeeEvent)
                {
                    isMailSent = true;
                    if (MasterConfigHelper.AllowProvisioningTaskReminder == false)
                    {
                        var trigger = EmployeeEventTrigger.GetById(currentTask.EmployeeEventTriggerId.Value);
                        isMailSent = SendEmailForEmployeeEvent(currentTask, currentWorkflowConfiguration, trigger);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the SendReminderEmail activity.";
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

//        private bool SendEmailForAttestation(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration, Attestation attestation)
//        {
//            try
//            {
//                EmailInfo emailInfo = new EmailInfo();
//
//                MailTemplate template = utility.GetMailTemplate("RemindAttestationApproval");
//                string emailIds, approverName;
//                GetApproverInformation(currentTask, out emailIds, out approverName);
//
//                if (string.IsNullOrEmpty(emailIds))
//                {
//                    DILog.AttestationLogger.WriteLog(attestation.Id, LogLevelType.Info, string.Format("Reminder email is not sent to {0} since he has no valid email id", currentTask.Approver));
//                    return false;
//                }
//
//                StringBuilder body = new StringBuilder(template.MailContent);
//                body.Replace("@@RECEIVER@@", approverName);
//                body.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
//                body.Replace("@@ATTESTATION_DESC@@", attestation.AttestationDescription);
//                body.Replace("@@APPROVAL_LEVEL@@", currentTask.WorkflowStateId.ToString());
//                body.Replace("@@REVIEWLASTDATE@@", utility.GetTaskCompletionDate(currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays, currentTask.CreatedDate).MailDateFormat());
//                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
//                emailInfo.Body = body.ToString();
//                emailInfo.Subject = string.Format(template.MailSubject, currentTask.WorkflowStateId.ToString());
//                emailInfo.To = emailIds;
//                emailInfo.From = template.MailFrom;
//                emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
//               
//                EmailHandler.Send(emailInfo);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAttestation:{ex.Message}", ex);
//                return false;
//            }
//        }

        public bool SendEmailForEmployeeEvent(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration, EmployeeEventTrigger trigger)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = utility.GetMailTemplate("RemindEmployeeEventApproval");
                var user = utility.GetUserByUserId(currentTask.ActionUserId.Value);
                var requester = User.GetByName(trigger.CreatedBy);
                var RequesteeUser = utility.GetUserByUserId(trigger.UserId);

                if (string.IsNullOrEmpty(user.EmailId))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Reminder email is not sent to {0} since he has no valid email id", user.UserName), null);
                    return false;
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
                body.Replace("@@RECEIVER@@", user.DisplayName);
                body.Replace("@@requestType@@", requestType);

                body.Replace("@@requesterDisplayName@@", requester.DisplayName);
                body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@EMPTYPE@@", RequesteeUser.EmployeeType.ToString());
                body.Replace("@@REVIEWLASTDATE@@", utility.GetTaskCompletionDate(currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays, currentTask.CreatedDate).MailDateFormat());
                body.Replace("@@REQUESTEDDATE@@", trigger.CreatedDate.MailDateTimeFormat());

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = user.EmailId;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForEmployeeEvent:{ex.Message}", ex);
                return false;
            }
        }
        private string ApplicationName(long endpointId)
        {
            var endpoint = EndPoint.GetById(endpointId);

            if (endpoint == null)
                return string.Empty;

            return endpoint.DisplayName;

        }
        public bool SendEmailForAdhocProvisioning(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = utility.GetMailTemplate("RemindAdhocProvisioningApproval");
                string emailIds, approverName;
                GetApproverInformation(currentTask, out emailIds, out approverName);
                var requester = utility.GetUserByUserId(currentTask.AccessRequest.AdHocAccessRequest.RequesterUserId);
                var RequesteeUser = utility.GetUserByUserId(currentTask.AccessRequest.AdHocAccessRequest.RequesteeUserId);

                string appName = string.Empty;
                try
                {
                    appName = ApplicationName(currentTask.AccessRequest.EndpointSystemId.Value);
                }
                catch (Exception)
                {
                }
                if (string.IsNullOrWhiteSpace(appName))
                {

                    if (currentTask.AccessRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                            currentTask.AccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                    {
                        appName = Constants.MSSL_ISU_CREM;
                    }
                    else if (currentTask.AccessRequest.ApplicationType == ApplicationType.EBS_CRM ||
                        currentTask.AccessRequest.ApplicationType == ApplicationType.EBS_ISU)
                    {
                        appName = Constants.EBS_ISU_CREM;
                    }
                    else
                    {
                        appName = UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.ApplicationType);
                    }
                }

                string requestDetail = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType), UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.AdHocAccessRequest.AccountType));

                if (string.IsNullOrEmpty(emailIds))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, "Reminder email is not sent to  since he has no valid email id", null);
                    return false;
                }

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@applicationName@@", appName);
                subject.Replace("@@requestType@@", currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                subject.Replace("@@requestNum@@", currentTask.AccessRequestId.ToString());

                subject.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                subject.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);

                subject.Replace("@@RECEIVERID@@", requester.EmployeeId.ToString());


                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@approverName@@", approverName);
                body.Replace("@@requestNum@@", currentTask.AccessRequestId.ToString());
                body.Replace("@@applicationName@@", appName);
                body.Replace("@@requestType@@", currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                body.Replace("@@REQUESTBY@@", requester.DisplayName);
                body.Replace("@@userDisplayName@@", RequesteeUser.DisplayName);
                body.Replace("@@pernrNum@@", RequesteeUser.EmployeeId);
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@EMPTYPE@@", RequesteeUser.EmployeeType.ToString());

                body.Replace("@@AccountType@@", UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.AdHocAccessRequest.AccountType));

                if (string.IsNullOrEmpty(currentTask.AccessRequest.AdHocAccessRequest.Comments))
                    body.Replace("@@COMMENTS@@", string.Empty);
                else
                    body.Replace("@@COMMENTS@@", currentTask.AccessRequest.AdHocAccessRequest.Comments?.Replace("\n", "<br>"));

                emailInfo.Body = body.ToString();
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Subject = subject.ToString();
                emailInfo.To = emailIds;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAdhocProvisioning:{ex.Message}", ex);
                return false;
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

        public bool SendEmailForBulkRequest(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration)
        {
            try
            {

                if (currentTask == null || currentTask.BulkRequest == null) return false;
                
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = utility.GetMailTemplate("RemindBulkRequestApproval");
                //var user = utility.GetUserByUserId(currentTask.ActionUserId.Value);
                string emailIds, approverName;
                GetApproverInformation(currentTask, out emailIds, out approverName);
                User requester = null;
                if(currentTask.BulkRequest != null)
                    requester = utility.GetUserByUserId(currentTask.BulkRequest.RequestedBy);

                string appName = string.Empty;

                string accounttype = string.Format("{0}", UtilityMethods.GetEnumAttribute<Description>(AccountType.User));

                if (string.IsNullOrEmpty(emailIds))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Notification email is not sent to {0} since he has no valid email id", currentTask.Approver), null);
                    return false;
                }
                try
                {
                    appName = ApplicationName(currentTask.BulkRequest.EndpointSystemId);
                }
                catch (Exception)
                {
                }
                if (string.IsNullOrWhiteSpace(appName))
                {

                    if (currentTask.BulkRequest != null && (currentTask.BulkRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                           currentTask.BulkRequest.ApplicationType == ApplicationType.MSSL_ISU))
                    {
                        appName = Constants.MSSL_ISU_CREM;
                    }
                    else if (currentTask.BulkRequest != null && (currentTask.BulkRequest.ApplicationType == ApplicationType.EBS_CRM ||
                        currentTask.BulkRequest.ApplicationType == ApplicationType.EBS_ISU))
                    {
                        appName = Constants.EBS_ISU_CREM;
                    }
                    else
                    {
                        appName = UtilityMethods.GetEnumAttribute<Description>(currentTask.BulkRequest.ApplicationType);
                    }
                }
                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@applicationName@@", appName);
                subject.Replace("@@requestType@@", currentTask.BulkRequest.AdhocActionType.ToString());
                subject.Replace("@@requestNum@@", currentTask.BulkRequestId.ToString());

                subject.Replace("@@RECEIVERID@@", requester?.EmployeeId?.ToString());

                string displayName = string.Join(",", currentTask.BulkRequest.BulkRequestDetails.Select(item => item.Name).ToList());
                string employeeId = string.Join(",", currentTask.BulkRequest.BulkRequestDetails.Select(item => item.EmployeeId).ToList());

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@approverName@@", approverName);
                body.Replace("@@requestNum@@", currentTask.BulkRequestId.ToString());
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
                body.Replace("@@REQUESTBY@@", requester?.DisplayName);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@EMPTYPE@@", currentTask.BulkRequest.IsEmployee ? EmployeeType.Permanent.ToString():"Non-Employee");

                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString(); //string.Format(template.MailSubject, appName);
                emailInfo.To = emailIds;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForBulkRequest:{ex.Message}", ex);
                return false;
            }
        }
        public bool SendReminderEmailForAttestation(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration)
        {
            try
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Info, $"SendReminderEmailForAttestation from Workflow");
                EmailInfo emailInfo = new EmailInfo();
                var wfDetail = WorkflowConfigurationDetail.GetByWorkflowConfigurationId(currentTask.WorkflowConfigurationId)?.FirstOrDefault(i => i.SequenceNumber == currentTask.WorkflowStateId);
                var enddate = utility.GetTaskCompletionDate(currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays, currentTask.CreatedDate).MailDateFormat();
                MailTemplate template = utility.GetMailTemplate("RemindAttestationApproval");
                User user = null;
                var receiverName = string.Empty;
                string emailIds = string.Empty;
                if (currentTask.ActionUserId.HasValue)
                {
                    user = utility.GetUserByUserId(currentTask.ActionUserId.Value);
                    receiverName = user?.DisplayName;
                    if (user != null)
                    {
                        emailIds = user?.EmailId;
                    }
                    else
                        DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendReminderEmailForAttestation:ActionUser does not exists for Attestation:{currentTask?.AttestationCampaignId} ");
                }
                else
                {
                    if (currentTask.ActionGroupId.HasValue)
                    {
                        var group = Group.GetById(currentTask.ActionGroupId.Value);
                        receiverName = group?.GroupName;
                        if (group != null)
                        {
                            emailIds = User.GetEmailIdsByGroupName(group.GroupName);
                        }
                        else
                        {
                            DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendReminderEmailForAttestation:ActionGroup does not exists for Attestation:{currentTask?.AttestationCampaignId} ");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(emailIds))
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Reminder email is not sent to {0} since he has no valid email id", user.UserName), null);
                    return false;
                }

                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@ATTESTATION_ENDPOINTNAME@@", currentTask.Attestation?.AttestationName);
                subject.Replace("@@ATTE_TYPE@@", $"{currentTask.Attestation?.AttestationType}");

                subject.Replace("@@CAMPAIGN_NAME@@", currentTask.AttestationCampaign?.CampaignName);

                StringBuilder body = new StringBuilder(template.MailContent);
                
                body.Replace("@@REVIEWER_NAME@@", receiverName);
                body.Replace("@@ATTESTATION_NAME@@", currentTask.Attestation?.AttestationName);
                body.Replace("@@ATTE_TYPE@@", $"{currentTask.Attestation?.AttestationType}");
                body.Replace("@@ATTESTATION_DESC@@", $"{currentTask.Attestation?.AttestationDescription}");
                body.Replace("@@CAMPAIGN_NAME@@", currentTask.AttestationCampaign?.CampaignName);
                body.Replace("@@APPROVAL_LEVEL@@", $"{currentTask.WorkflowStateId}"); 
                body.Replace("@@CAMPIGN_START_DATE@@", currentTask.AttestationCampaign?.CreatedDate.ToString("dd/MM/yyyy"));
                body.Replace("@@REVIEWLASTDATE@@", enddate);
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                
                emailInfo.Subject = subject.ToString();
                if(!string.IsNullOrWhiteSpace(template.MailCC))
                    emailInfo.Cc = template.MailCC;

                if (!string.IsNullOrWhiteSpace(template.MailBcc))
                    emailInfo.Bcc = template.MailBcc;

                emailInfo.To = $"{user.EmailId},{template.MailTo}";
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendReminderEmailForAttestation:{ex.Message}", ex);
                return false;
            }
        }
    }
}
