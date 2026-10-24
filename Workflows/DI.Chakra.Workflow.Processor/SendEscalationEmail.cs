 
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
    public class SendEscalationEmail
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
                    isMailSent = true;
                    if (MasterConfigHelper.AllowAttestationTaskReminder == false)
                    {
                        DILog.EmailLogger.WriteLog(LogLevelType.Info, $"SendEsclationEmailForAttestation from Workflow");

                        Attestation attestation = Attestation.GetByIdIncludingAttestationProcesses(currentTask.AttestationId.Value);
                        isMailSent = SendEsclationEmailForAttestation(currentTask, currentWorkflowConfiguration, attestation);
                    }
                    //TODO:Siva
                    //isMailSent = new EmailComposer().Compose(EmailTemplate.EscalateAttestationApproval, currentTask.Id);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    var deepLoadedTask = Task.GetProvisioningTask(currentTask.Id);
                    isMailSent = SendEmailForAdhocProvisioning(deepLoadedTask, currentWorkflowConfiguration);
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the SendEscalationEmail activity.";
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

        public static bool SendEsclationEmailForAttestation(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration, Attestation attestation)
        {
            bool sent = false;
            try
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Info, $"SendEsclationEmailForAttestation from Workflow");
                EmailInfo emailInfo = new EmailInfo();
                var emailIds = string.Empty;
                MailTemplate template = MailTemplate.GetByTemplateName("EscalateAttestationApproval");
                User manager = null;
                User user = null;
                var epName = string.Empty;
                var iamTeam = string.Join(",", MasterConfigHelper.AlertNotificationReceivers);
                epName = string.Join(",", attestation.AttestationEndpoint.Select(i => i.EndPoint?.DisplayName));
                var reviewerName = string.Empty;
                var receiver = string.Empty;
                var campaign = AttestationCampaign.GetById(currentTask.AttestationCampaignId.Value);
                if (currentTask.ActionUserId.HasValue)
                {
                    user = User.GetById(currentTask.ActionUserId.Value);
                    reviewerName = user.DisplayName;
                    if (user != null && !string.IsNullOrWhiteSpace(user.ROEmployeeId))
                    {
                        manager = User.GetByEmployeeId(user.ROEmployeeId);
                    }

                    if (manager != null && manager.IsActive == false)
                    {
                        manager = User.GetByEmployeeId(manager.ROEmployeeId);
                    }
                }
                else if (currentTask.IsGroupTask && currentTask.ActionGroupId.HasValue)
                {
                    manager = null;
                    emailIds = string.Join(",", User.GetEmailIdsByGroupId(currentTask.ActionGroupId.Value));
                    var grp = Group.GetById(currentTask.ActionGroupId.Value)?.GroupName;
                    reviewerName = $"Group({grp}) Reviewer";
                    receiver = $"Group({grp}) Member";
                }

                if (manager == null)
                {
                    if (string.IsNullOrWhiteSpace(emailIds))
                        emailIds = iamTeam;
                    receiver = "Receiver";
                }
                else
                {
                    emailIds = manager.EmailId;
                    receiver = manager.DisplayName;
                }
                StringBuilder subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@ATTTSK_ENDPOINTNAME_DISP@@", epName);
                subject.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
                subject.Replace("@@CAMPAIGN_NAME@@", campaign.CampaignName);
                subject.Replace("@@APPROVALLEVEL@@", currentTask.WorkflowStateId.ToString());
                subject.Replace("@@RECEIVER@@", receiver);
                subject.Replace("@@REVIEWER_NAME@@", reviewerName);


                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@RECEIVER@@", receiver);
                body.Replace("@@REVIEWER_NAME@@", reviewerName);
                body.Replace("@@ATTTSK_ENDPOINTNAME_DISP@@", epName);
                body.Replace("@@ATTESTATION_DESC@@", attestation.AttestationDescription);
                body.Replace("@@ATTESTATION_NAME@@", attestation.AttestationName);
                body.Replace("@@APPROVALLEVEL@@", currentTask.WorkflowStateId.ToString());
                body.Replace("@@CAMPAIGN_NAME@@", campaign.CampaignName);
                body.Replace("@@CAMPIGN_START_DATE@@", campaign.CreatedDate.ToString("dd/MM/yyyy"));
                body.Replace("@@REVIEWLASTDATE@@", currentTask.CreatedDate.AddDays(currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays).ToString("dd/MM/yyyy"));
                body.Replace("@@APPROVALLEVEL@@", currentTask.WorkflowStateId.ToString());
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                if (currentTask.IsGroupTask)
                {
                    emailInfo.Cc = $"{template?.MailCC},{iamTeam}";
                }
                else
                {
                    emailInfo.Cc = $"{template?.MailCC},{user?.EmailId},{iamTeam}";
                }
                
                if (currentTask.IsGroupTask)
                {
                    emailInfo.Cc = emailIds;
                }

                if (!string.IsNullOrWhiteSpace(template.MailBcc))
                    emailInfo.Bcc = template.MailBcc;

                emailInfo.To = $"{template.MailTo},{emailIds}";
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                EmailHandler.Send(emailInfo);
                sent = true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAttestation:{ex.Message}", ex);

            }
            return sent;
        }

        public bool SendEmailForAdhocProvisioning(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration)
        {
            bool sent = false;
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate template = utility.GetMailTemplate("EscalateAdhocProvisioningApproval");
                var user = utility.GetUserByUserId(currentTask.ActionUserId.Value);
                var manager = utility.GetUserByEmployeeId(user.ROEmployeeId);
                var requester = utility.GetUserByUserId(currentTask.AccessRequest.RequesterId.Value);

                string appName = string.Empty;

                if (currentTask.AccessRequest.EndpointSystemId.HasValue)
                {
                    appName = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.ApplicationType), currentTask.AccessRequest.AdHocAccessRequest.EndPoint.DisplayName);
                }

                string requestDetail = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.AdHocAccessRequest.AdhocActionType), UtilityMethods.GetEnumAttribute<Description>(currentTask.AccessRequest.AccountType));

                if (manager == null)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Escalation email is not sent to {0} since he has no valid email id. This task will be terminated here since there is no way to escalate the manager.", manager.UserName), null);
                    return false;
                }

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@RECEIVER@@", manager.DisplayName);
                body.Replace("@@REVIEWER_NAME@@", user.DisplayName);
                body.Replace("@@APPLICATION_NAME@@", appName);
                body.Replace("@@REQUESTER@@", requester.DisplayName);
                body.Replace("@@REQUESTEDDATE@@", currentTask.AccessRequest.CreatedDate.MailDateTimeFormat());
                body.Replace("@@REQUEST_DETAIL@@", requestDetail);
                body.Replace("@@REVIEWLASTDATE@@", utility.GetTaskCompletionDate(currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays, currentTask.CreatedDate).MailDateFormat());
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);

                body.Replace("@@RequestBy@@", utility.GetUserByUserId(Convert.ToInt64(currentTask.AccessRequest.RequesterId))?.DisplayName);
                body.Replace("@@AccountType@@", currentTask.AccessRequest.AccountType.GetDescription());
                body.Replace("@@EmployeeType@@", user.EmployeeType.GetDescription());
                body.Replace("@@Comments@@", currentTask.AccessRequest.Comments?.ToString());

                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = string.Format(template.MailSubject, appName);
                emailInfo.To = manager.EmailId;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();

                EmailHandler.Send(emailInfo);
                sent = true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailForAdhocProvisioning:{ex.Message}", ex);
            }
            return sent;
        }

        private bool SendEmailForBulkRequest(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration, BulkRequest bulkRequest)
        {

            return true;
            //try
            //{
            //    EmailInfo emailInfo = new EmailInfo();

            //    MailTemplate template = utility.GetMailTemplate("EscalateAdhocProvisioningApproval");
            //    var user = utility.GetUserByUserId(currentTask.ActionUserId);
            //    var manager = utility.GetUserByUserId(user.ManagerId.Value);
            //    var requester = utility.GetUserByUserId(adHocAccessRequest.RequesterUserId);

            //    string appName = string.Empty;

            //    if (adHocAccessRequest.EndpointSystemId.HasValue)
            //    {
            //        appName = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.EntitlementDetailType), adHocAccessRequest.EndPoint.HostName);
            //    }
            //    else
            //    {
            //        appName = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.EntitlementDetailType), adHocAccessRequest.MiscSystem.SystemName);
            //    }

            //    string requestDetail = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AdhocActionType), UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AccountType));

            //    if (manager == null)
            //    {
            //        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Escalation email is not sent to {0} since he has no valid email id. This task will be terminated here since there is no way to escalate the manager.", manager.UserName), null);
            //        return false;
            //    }

            //    StringBuilder body = new StringBuilder(template.MailContent);
            //    body.Replace("@@RECEIVER@@", manager.DisplayName);
            //    body.Replace("@@REVIEWER_NAME@@", user.DisplayName);
            //    body.Replace("@@APPLICATION_NAME@@", appName);
            //    body.Replace("@@REQUESTER@@", requester.DisplayName);
            //    body.Replace("@@REQUESTERDATE@@", DateTime.Now.ToString());
            //    body.Replace("@@REQUEST_DETAIL@@", requestDetail);
            //    body.Replace("@@REVIEWLASTDATE@@", utility.GetTaskCompletionDate(currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays, currentTask.CreatedDate).ToShortDateString());
            //    body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
            //    emailInfo.Body = body.ToString();
            //    emailInfo.Subject = string.Format(template.MailSubject, appName);
            //    emailInfo.To = manager.EmailId;
            //    emailInfo.From = template.MailFrom;
            //    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString(); ;

            //    EmailHandler.Send(emailInfo);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        private bool SendEmailForEmployeeEvent(Task currentTask, WorkflowConfiguration currentWorkflowConfiguration, EmployeeEventTrigger trigger)
        {
            try
            {
                //EmailInfo emailInfo = new EmailInfo();

                //MailTemplate template = utility.GetMailTemplate("EscalateAdhocProvisioningApproval");
                //var user = utility.GetUserByUserId(currentTask.ActionUserId);
                //var manager = utility.GetUserByUserId(user.ManagerId.Value);
                //var requester = utility.GetUserByUserId(adHocAccessRequest.RequesterUserId);

                //string appName = string.Empty;

                //if (adHocAccessRequest.EndpointSystemId.HasValue)
                //{
                //    appName = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.EntitlementDetailType), adHocAccessRequest.EndPoint.HostName);
                //}
                //else
                //{
                //    appName = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.EntitlementDetailType), adHocAccessRequest.MiscSystem.SystemName);
                //}

                //string requestDetail = string.Format("{0} - {1}", UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AdhocActionType), UtilityMethods.GetEnumAttribute<Description>(adHocAccessRequest.AccountType));

                //if (manager == null)
                //{
                //    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Info, string.Format("Escalation email is not sent to {0} since he has no valid email id. This task will be terminated here since there is no way to escalate the manager.", manager.UserName), null);
                //    return false;
                //}

                //StringBuilder body = new StringBuilder(template.MailContent);
                //body.Replace("@@RECEIVER@@", manager.DisplayName);
                //body.Replace("@@REVIEWER_NAME@@", user.DisplayName);
                //body.Replace("@@APPLICATION_NAME@@", appName);
                //body.Replace("@@REQUESTER@@", requester.DisplayName);
                //body.Replace("@@REQUESTERDATE@@", DateTime.Now.ToString());
                //body.Replace("@@REQUEST_DETAIL@@", requestDetail);
                //body.Replace("@@REVIEWLASTDATE@@", utility.GetTaskCompletionDate(currentWorkflowConfiguration.WorkflowConfigurationDetails[0].ResponseTimeInDays, currentTask.CreatedDate).ToShortDateString());
                //body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                //emailInfo.Body = body.ToString();
                //emailInfo.Subject = string.Format(template.MailSubject, appName);
                //emailInfo.To = manager.EmailId;
                //emailInfo.From = template.MailFrom;
                //emailInfo.ActivityType = EmailActivityType.Provisioning.ToString(); ;

                //EmailHandler.Send(emailInfo);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
