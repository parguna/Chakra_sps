using DI.Chakra.Services.EmailingProcessor;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Workflow.Processor.Common;
using System;
using System.Text;
using DI.Chakra.DomainModel; 
using System.Linq;
using System.Collections.Generic;

namespace DI.Chakra.Workflow.Processor
{
    public class WithdrawRequest
    {
        UtilityWFFunctions utility = new UtilityWFFunctions();
        const string DATETIMEFORMAT = "dd/MM/yyyy hh:mm tt";

        public void Execute(Guid workflowInstanceId)
        {
            try
            {
                var tasks = Task.GetPendingTaskByworkflowInstanceId(workflowInstanceId);
                var reqType = RequestType.None;
                var MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
                if (tasks != null && tasks.Count > 0)
                {
                    reqType = (RequestType)tasks[0].RequestTypeId;
                }

                if (reqType == RequestType.Provisioning)
                {
                  
                    foreach (Task task in tasks)
                    {
                        task.ActionDate = DateTime.Now;
                        task.IsCompleted = true;
                        task.Comments = "Request is withdrawn by the user.";
                        task.IsActive = false;
                        task.Action = StatusType.Withdrawn;
                         
                        task.Save(MasterConfigHelperServiceUser);

                        //clear if any data available in interim store
                        InterimStore.DeleteInterimStoreByTaskId(task.Id);

                        var accessRequest = AccessRequest.GetById(task.AccessRequestId.Value, true);
                        accessRequest.RequestStatus = AccessRequestStatus.Withdrawn;
                        accessRequest.ProgressStatus = AccessRequestProgress.Withdrawn;
                        
                        if (task.ActionUserId.HasValue)
                        {
                            this.NotifyApproverAboutRequestWithdrawn(task.ActionUserId.Value, accessRequest);
                        }
                        else
                        {
                            var approvers = utility.GetUsersByGroupId(task.ActionGroupId.Value);

                            foreach (long approver in approvers)
                            {
                                this.NotifyApproverAboutRequestWithdrawn(approver, accessRequest);
                            }
                        }
                        accessRequest.Save(MasterConfigHelperServiceUser);
                    }
                }
                else if (reqType == RequestType.BulkRequest)
                {
                    foreach (Task task in tasks)
                    {
                        task.ActionDate = DateTime.Now;
                        task.IsCompleted = true;
                        task.Comments = "Request is withdrawn by the user.";
                        task.IsActive = false;
                        task.Action = StatusType.Withdrawn;
                        task.Save(MasterConfigHelperServiceUser);

                        //clear if any data available in interim store
                        InterimStore.DeleteInterimStoreByTaskId(task.Id);

                        var bulkRequest = BulkRequest.GetById(task.BulkRequestId.Value, true);
                        bulkRequest.RequestStatus = AccessRequestStatus.Withdrawn;
                        bulkRequest.ProgressStatus = AccessRequestProgress.Withdrawn;
                        if (task.ActionUserId.HasValue)
                        {
                            this.NotifyApproverAboutRequestWithdrawn(task.ActionUserId.Value, bulkRequest);
                        }
                        else
                        {
                            var approvers = utility.GetUsersByGroupId(task.ActionGroupId.Value);

                            foreach (long approver in approvers)
                            {
                                this.NotifyApproverAboutRequestWithdrawn(approver, bulkRequest);
                            }
                        }
                        bulkRequest.Save(MasterConfigHelperServiceUser);
                    }
                }
                else if (reqType == RequestType.EmployeeEvent)
                {
                    foreach (Task task in tasks)
                    {
                        task.ActionDate = DateTime.Now;
                        task.IsCompleted = true;
                        task.Comments = "Request is withdrawn by the user.";
                        task.IsActive = false;
                        task.Action = StatusType.Withdrawn;
                        task.LastModifiedDate = DateTime.Now;
                        task.Save(MasterConfigHelper.ServiceUser);

                        //clear if any data available in interim store
                        InterimStore.DeleteInterimStoreByTaskId(task.Id);

                        var trigger = EmployeeEventTrigger.GetById(task.EmployeeEventTriggerId.Value, true);

                        if (task.ActionUserId.HasValue)
                        {
                            this.NotifyApproverAboutRequestWithdrawn(task.ActionUserId.Value, trigger);
                        }
                        else
                        {
                            var approvers = utility.GetUsersByGroupId(task.ActionGroupId.Value);

                            foreach (long approver in approvers)
                            {
                                this.NotifyApproverAboutRequestWithdrawn(approver, trigger);
                            }
                        }
                        trigger.RequestStatus = AccessRequestStatus.Withdrawn;
                        trigger.LastModifiedDate = DateTime.Now;
                        trigger.Save(MasterConfigHelperServiceUser);
                    }
                }
                else if (reqType == RequestType.Attestation)
                {
                    var campaignId = tasks[0].AttestationCampaignId.Value;

                    var actions = AttestationSnapshotAction.GetByCampaignId(campaignId);

                    // Update attestation campaign comments
                    var campaign = AttestationCampaign.GetById(campaignId);
                    var withDrawnBy = campaign.LastModifiedBy;
                    var campaignWithDrawReason = campaign.Comments;
                    campaign.Comments += "Attestation Campaign has been cancelled.";
                    campaign.Status = AttestationCampaignStatus.Completed;
                    campaign.EndDate = DateTime.Now;
                    campaign.Save(MasterConfigHelper.ServiceUser);

                    foreach (Task task in tasks)
                    {
                        task.ActionDate = DateTime.Now;
                        task.Comments += $"Attestation Campaign has been cancelled. WithDrawReason:{campaign.Comments}";
                        task.IsActive = false;
                        task.IsCompleted = true;
                        task.Action = StatusType.Withdrawn;
                        task.Save(MasterConfigHelper.ServiceUser);

                        //clear if any data available in interim store
                        InterimStore.DeleteInterimStoreByTaskId(task.Id);

                        campaign.LastModifiedBy = withDrawnBy;
                        campaign.Comments = campaignWithDrawReason;
                        if (task.ActionUserId.HasValue)
                        {
                            this.NotifyApproverAboutAttestationCampaignWithdrawn(task.ActionUserId.Value, campaign);
                        }
                        else
                        {
                            if (task.ActionGroupId.HasValue)
                            {
                                this.NotifyApproverAboutAttestationCampaignWithdrawn(task.ActionGroupId.Value, campaign, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the WithdrawRequest activity.";
                DILog.ProvisioningLogger.WriteLog(null, null,  LogLevelType.Exception, message, ex);
            }
        }

        public void NotifyApproverAboutRequestWithdrawn(long approverUserId, AccessRequest accessRequest)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate mailTemplate = utility.GetMailTemplate(Constants.AccessRequestWithdrawn);

                if (mailTemplate == null) return;

                var requester = utility.GetUserByUserId(accessRequest.AdHocAccessRequest.RequesterUserId);
                var requestee = utility.GetUserByUserId(accessRequest.UserId);
                var approver = utility.GetUserByUserId(approverUserId);

                string appName = string.Empty;
                if (accessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                      accessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.MSSL_ISU)
                {
                    appName = Constants.MSSL_ISU_CREM;
                }
                else if (accessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.EBS_CRM ||
                    accessRequest.AdHocAccessRequest.ApplicationType == ApplicationType.EBS_ISU)
                {
                    appName = Constants.EBS_ISU_CREM;
                }
                else
                {
                    appName = UtilityMethods.GetEnumAttribute<Description>(accessRequest.AdHocAccessRequest.ApplicationType);
                }

                StringBuilder body = new StringBuilder(mailTemplate.MailContent);
                StringBuilder mailSubject = new StringBuilder(mailTemplate.MailSubject);

                string toMailid = string.Empty;

                mailSubject.Replace("@@ApplicationName@@", appName);
                mailSubject.Replace("@@REQUESTTYPE@@", accessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                mailSubject.Replace("@@REQUESTID@@", accessRequest.Id.ToString());
                mailSubject.Replace("@@USER@@", requestee.DisplayName);
                mailSubject.Replace("@@pernrNum@@", requestee.EmployeeId);

                body.Replace("@@APPROVER@@", approver.DisplayName);
                body.Replace("@@USER@@", requestee.DisplayName);
                body.Replace("@@REQUESTER@@", requester.DisplayName);
                body.Replace("@@DATE@@", DateTime.Now.MailDateTimeFormat());
                body.Replace("@@REQUESTID@@", accessRequest.Id.ToString());
                body.Replace("@@ApplicationName@@", appName);
                body.Replace("@@REQUESTTYPE@@", accessRequest.AdHocAccessRequest.AdhocActionType.ToString());
                body.Replace("@@pernrNum@@", requestee.EmployeeId);
                body.Replace("@@REQUESTBY@@", requester.DisplayName);
                body.Replace("@@EMPTYPE@@", requestee.EmployeeType.ToString());
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@REASON@@", accessRequest.Comments);
                toMailid = (approver == null && string.IsNullOrWhiteSpace(approver.EmailId)) ? string.Empty : Convert.ToString(approver.EmailId);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = mailSubject.ToString();
                emailInfo.To = string.Format("{0}", toMailid);

                emailInfo.From = mailTemplate.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyApproverAboutRequestWithdrawn:{ex.Message}", ex);
            }
        }

        public void NotifyApproverAboutRequestWithdrawn(long approverUserId, BulkRequest bulkRequest)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate mailTemplate = utility.GetMailTemplate(Constants.BulkRequestWithdrawn);

                if (mailTemplate == null) return;

                var requester = utility.GetUserByUserId(bulkRequest.RequestedBy);
                var approver = utility.GetUserByUserId(approverUserId);

                string appName = string.Empty;
                if (bulkRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                      bulkRequest.ApplicationType == ApplicationType.MSSL_ISU)
                {
                    appName = Constants.MSSL_ISU_CREM;
                }
                else if (bulkRequest.ApplicationType == ApplicationType.EBS_CRM ||
                    bulkRequest.ApplicationType == ApplicationType.EBS_ISU)
                {
                    appName = Constants.EBS_ISU_CREM;
                }
                else
                {
                    appName = UtilityMethods.GetEnumAttribute<Description>(bulkRequest.ApplicationType);
                }

                StringBuilder body = new StringBuilder(mailTemplate.MailContent);
                StringBuilder mailSubject = new StringBuilder(mailTemplate.MailSubject);

                string toMailid = string.Empty;

                mailSubject.Replace("@@ApplicationName@@", appName);
                mailSubject.Replace("@@REQUESTTYPE@@", bulkRequest.AdhocActionType.ToString());
                mailSubject.Replace("@@REQUESTID@@", bulkRequest.Id.ToString());

                string displayName = string.Join(",", bulkRequest.BulkRequestDetails.Select(item => $"{item.Name}({item.EmployeeId})").ToList());
                string employeeId = string.Join(",", bulkRequest.BulkRequestDetails.Select(item => item.EmployeeId).ToList());

                body.Replace("@@APPROVER@@", approver.DisplayName);
                body.Replace("@@USER@@", displayName);
                body.Replace("@@REQUESTER@@", requester.DisplayName);
                body.Replace("@@DATE@@", DateTime.Now.MailDateTimeFormat());
                body.Replace("@@REQUESTID@@", bulkRequest.Id.ToString());
                body.Replace("@@ApplicationName@@", appName);
                body.Replace("@@REQUESTTYPE@@", bulkRequest.AdhocActionType.ToString());
                body.Replace("@@pernrNum@@", employeeId);
                body.Replace("@@WITHDRAWREASON@@", bulkRequest.Comments);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@EMPTYPE@@", bulkRequest.IsEmployee ? EmployeeType.Permanent.ToString():"Non-Employee");
                toMailid = (approver == null && string.IsNullOrWhiteSpace(approver.EmailId)) ? string.Empty : Convert.ToString(approver.EmailId);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = mailSubject.ToString();
                emailInfo.To = string.Format("{0}", toMailid);

                emailInfo.From = mailTemplate.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyApproverAboutRequestWithdrawn:{ex.Message}", ex);
            }
        }

        public void NotifyApproverAboutRequestWithdrawn(long approverUserId, EmployeeEventTrigger trigger)
        {
            try
            {
                EmailInfo emailInfo = new EmailInfo();

                MailTemplate mailTemplate = utility.GetMailTemplate(Constants.EmployeeEventWithdrawn);

                if (mailTemplate == null) return;

                var requester = User.GetByUserName(trigger.CreatedBy);
                var requestee = utility.GetUserByUserId(trigger.UserId);
                var approver = utility.GetUserByUserId(approverUserId);

                StringBuilder body = new StringBuilder(mailTemplate.MailContent);
                StringBuilder mailSubject = new StringBuilder(mailTemplate.MailSubject);

                string requestType = string.Empty;
                if (trigger.EventType == EmployeeEvents.NonEmployeeCreation)
                {
                    mailSubject.Replace("@@REQUESTNUM@@", trigger.Id.ToString());
                    body.Replace("@@REQUESTNUM@@", trigger.Id.ToString());
                    requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeCreation);
                }
                else if (trigger.EventType == EmployeeEvents.NonEmployeeExtension)
                {
                    mailSubject.Replace("@@REQUESTNUM@@", trigger.Id.ToString());
                    body.Replace("@@REQUESTNUM@@", trigger.Id.ToString());
                    requestType = UtilityMethods.GetEnumAttribute<Description>(AccessRequestSource.EmployeeExtension);
                }

                string toMailid = string.Empty;

                body.Replace("@@approverName@@", approver.DisplayName);
                body.Replace("@@requestType@@", requestType);

                body.Replace("@@DATE@@", DateTime.Now.MailDateTimeFormat());
                body.Replace("@@requesterDisplayName@@", requester.DisplayName);
                body.Replace("@@userDisplayName@@", requestee.DisplayName);
                body.Replace("@@pernrNum@@", requestee.EmployeeId);
                body.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                body.Replace("@@EMPTYPE@@", requestee.EmployeeType.ToString()); 
                toMailid = (approver == null && string.IsNullOrWhiteSpace(approver.EmailId)) ? string.Empty : Convert.ToString(approver.EmailId);

                emailInfo.Body = body.ToString();
                emailInfo.Subject = mailSubject.ToString();
                emailInfo.To = string.Format("{0}", toMailid);

                emailInfo.From = mailTemplate.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyApproverAboutRequestWithdrawn:{ex.Message}", ex);
            }
        }
        public void NotifyApproverAboutAttestationCampaignWithdrawn(long approverId, AttestationCampaign campaign,bool isGroup=false)
        {
            try
            {
                string approverstring = string.Empty;
                List<User> users = new List<User>();
                EmailInfo emailInfo = new EmailInfo();
                if (isGroup)
                {
                    var groups = Group.GetByIdIncludingUsers(approverId);
                    approverstring = groups?.GroupName;
                    if(groups != null && groups.Users != null)
                    {
                        groups.Users.ForEach(i =>
                        {
                            if (i != null && i.IsActive && !string.IsNullOrWhiteSpace(i.EmailId))
                            {
                                users.Add(i);
                            }
                        });
                    }
                }
                else
                {
                    var usr = utility.GetUserByUserId(approverId);
                    if (usr != null)
                        users.Add(usr);
                    approverstring = usr?.DisplayName;
                }
                MailTemplate mailTemplate = utility.GetMailTemplate(Constants.NotifyAttestationWithDrawn);

                if (mailTemplate == null) return;

                string campaignName = campaign.CampaignName;
                StringBuilder body = new StringBuilder(mailTemplate.MailContent);
                StringBuilder mailSubject = new StringBuilder(mailTemplate.MailSubject);

                string toMailid = string.Empty;

                mailSubject.Replace("@@CAMPAIGNNAME@@", campaignName);
                mailSubject.Replace("@@WITHDRAWBY@@", campaign.LastModifiedBy);

                body.Replace("@@APPROVER@@", approverstring);
                body.Replace("@@CAMPAIGNNAME@@", campaignName);
                body.Replace("@@DATE@@", DateTime.Now.MailDateTimeFormat());
                body.Replace("@@WITHDRAWBY@@", campaign.LastModifiedBy);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                var cmment = campaign.Comments.Trim().Replace(@"\r\n","").Replace("!~","").Replace("!~Attestation Campaign has been cancelled.","").Split(new string[] { "Update By:"},StringSplitOptions.RemoveEmptyEntries).ToList().Distinct().ToList();
                var cmt = cmment.LastOrDefault()?.Split(new[] { " with reason: " }, StringSplitOptions.RemoveEmptyEntries);
                body.Replace("@@WITHDRAWREASON@@", (cmt != null && cmt.Length == 2 ?  cmt[1]: cmt[0]));

                toMailid = string.Join(",", users.Select(i=>i.EmailId));

                emailInfo.Body = body.ToString();
                emailInfo.Subject = mailSubject.ToString();
                emailInfo.To = $"{toMailid},{mailTemplate.MailTo}";
                emailInfo.Cc = $"{mailTemplate.MailCC}";
                emailInfo.Bcc = $"{mailTemplate.MailBcc}";
                emailInfo.From = mailTemplate.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"NotifyApproverAboutAttestationCampaignWithdrawn:{ex.Message}", ex);
            }
        }
    }
}
