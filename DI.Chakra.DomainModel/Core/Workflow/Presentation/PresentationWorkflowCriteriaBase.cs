using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public abstract class PresentationWorkflowCriteriaBase
    {
        public RequestType RequestType { get; set; }

        public static PresentationWorkflowCriteriaBase ORMap(EntityModel.Presentation.PresentationWorkflowCriteriaBase presentationWorkflowCriteriaBaseEntityModel)
        {
            if (presentationWorkflowCriteriaBaseEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationWorkflowCriteriaBase, PresentationWorkflowCriteriaBase>(presentationWorkflowCriteriaBaseEntityModel);
        }

        public static List<PresentationWorkflowCriteriaBase> ORMapList(IList<EntityModel.Presentation.PresentationWorkflowCriteriaBase> presentationWorkflowCriteriaBaseEntityModelEntityModels)
        {
            return presentationWorkflowCriteriaBaseEntityModelEntityModels.Select(ORMap).ToList();
        }


        //private static void SendEmailForAccessRequestRejected(int templateId,object objectId, Task task,string AccountCreatedNotificationTO, string currentUser)
        //{

        //    EmailInfo emailInfo = new EmailInfo();
        //    MailTemplate mailTemplate = MailTemplate.GetByTemplateId(templateId);
        //    if (mailTemplate == null) return;

        //    StringBuilder subjectContent = new StringBuilder(2);
        //    //subjectContent = TokenReplacer.Execute(template, objectId);

        //    string toMailid = string.Empty;
           
        //    User user = User.GetById(task.AccessRequest.RequesterId.Value);
        //    toMailid = (user == null && string.IsNullOrWhiteSpace(user.EmailId)) ? string.Empty : Convert.ToString(user.EmailId);

        //    emailInfo.Body = subjectContent[1].ToString();
        //    emailInfo.Subject = subjectContent[0].ToString();
        //    emailInfo.To = string.Format("{0}", toMailid);
        //    if (string.IsNullOrEmpty(toMailid))
        //    {
        //        emailInfo.Cc = string.Format("{0}", AccountCreatedNotificationTO);
        //    }
        //    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
        //if (template != null && template.IsActive.HasValue && !template.IsActive.Value)
        //       emailInfo.Status = "Excluded";
        //    emailInfo.Save(currentUser);

        //}

        //private static void SendEmailForAccessRequestRejected(Task task, string AccountCreatedNotificationTO)
        //{
        //    EmailInfo emailInfo = new EmailInfo();
        //    MailTemplate mailTemplate = MailTemplate.GetByTemplateName(Constants.AccessRequestRejected);
        //    if (mailTemplate == null) return;

        //    StringBuilder body = new StringBuilder(mailTemplate.MailContent);
        //    StringBuilder mailSubject = new StringBuilder(mailTemplate.MailSubject);

        //    string toMailid = string.Empty;
        //    if (task.AccessRequest != null)
        //    {
        //        mailSubject.Replace("@@REQUESTTYPE_REQ@@", task.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString());
        //        mailSubject.Replace("@@REQUESTNUMBER_REQ@@", task.AccessRequestId.ToString());
        //        mailSubject.Replace("@@USERDISPLAYNAME_REQ@@", task.AccessRequest.User.DisplayName);
        //        mailSubject.Replace("@@PERNRNO_REQ@@", task.AccessRequest.User.EmployeeId);
        //        mailSubject.Replace("@@APPLICATION_REQ@@", Convert.ToString(task.AccessRequest.AdHocAccessRequest.EndpointSystemType));

        //        User user = User.GetById(task.AccessRequest.RequesterId.Value);
        //        body.Replace("@@USERDISPLAYNAME_REQ@@", user.DisplayName);
        //        body.Replace("##APPROVER_REQ##", task.LastModifiedBy);
        //        body.Replace("##DATE##", DateTime.Now.ToString());
        //        body.Replace("@@REQUESTNUMBER_REQ@@", task.AccessRequestId.ToString());
        //        body.Replace("@@APPLICATION_REQ@@", Convert.ToString(task.AccessRequest.AdHocAccessRequest.EndpointSystemType));
        //        body.Replace("@@REQUESTTYPE_REQ@@", task.AccessRequest.AdHocAccessRequest.AdhocActionType.ToString());
        //        body.Replace("@@APPROVERCOMMENTS_REQ@@", task.AccessRequest.Comments);
        //        body.Replace("@@PERNRNO_REQ@@", task.AccessRequest.User.EmployeeId);
        //        body.Replace("@@REQUEST_ERRORMESSAGE_DISC@@", task.AccessRequest.Remarks);
        //        toMailid = (user == null && string.IsNullOrWhiteSpace(user.EmailId)) ? string.Empty : Convert.ToString(user.EmailId);

        //    }

        //    emailInfo.Body = body.ToString();
        //    emailInfo.Subject = mailSubject.ToString();
        //    emailInfo.To = string.Format("{0}", toMailid);
        //    if (string.IsNullOrEmpty(toMailid))
        //    {
        //        emailInfo.Cc = string.Format("{0}", AccountCreatedNotificationTO);
        //    }
        //    emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
        //if (template != null && template.IsActive.HasValue && !template.IsActive.Value)
        //        emailInfo.Status = "Excluded";
        //    emailInfo.Save();

        //}

        public static PresentationWorkflowCriteriaBase GetWorkFlowCriteria(AccessType accessType, string UserName, long TaskId, string Comments, string currentUser, string ConflictRemarks=null)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                Task task = Task.GetById(TaskId,true);

                if (task.IsGroupTask && task.AssignedBy == null && task.AssignedTo == null)
                {
                    task.AssignedTo = task.ActionUserId;
                    task.AssignedBy = task.ActionUserId;
                    task.Save(dataAccessTransaction);
                }
                // The below code was written to capture the actual approver of a group task
                // The captured action user will be reflected in the approver chain and send notification screen
                // - By Shriveni
                if (task.IsGroupTask && task.ActionUserId==null)
                {
                    task.ActionUserId = User.GetIdByUserName(UserName);
                }
                // End 

                if (task.AccessRequest != null)
                {
                    if (accessType == AccessType.Revoke)
                    {
                        task.AccessRequest.RequestStatus = AccessRequestStatus.Rejected;
                        task.AccessRequest.ProgressStatus = AccessRequestProgress.RequestRejected;
                    }
                    if (accessType == AccessType.Approve)
                        task.AccessRequest.RequestStatus = AccessRequestStatus.Approved;
                    task.AccessRequest.LastModifiedBy = UserName;
                    task.AccessRequest.LastModifiedDate = DateTime.Now;
                    task.AccessRequest.Comments = Comments;
                    
                    if (task.AccessRequest.ApplicationType == ApplicationType.EBS_ISU || task.AccessRequest.ApplicationType == ApplicationType.EBS_CRM ||
                        task.AccessRequest.ApplicationType == ApplicationType.MSSL_ISU || task.AccessRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                        task.AccessRequest.ApplicationType == ApplicationType.MSSL || task.AccessRequest.ApplicationType == ApplicationType.EBS)
                    {
                        
                        try
                        {
                            List<PresentationConflictRemarks> data = null;
                            if (!string.IsNullOrWhiteSpace(ConflictRemarks))
                            {
                                data = UtilityMethods.Deserialize<List<PresentationConflictRemarks>>(ConflictRemarks);
                                if (data != null && data.Count > 0)
                                {
                                    var ep = EndPoint.GetById(task.AccessRequest.EndpointSystemId.Value);
                                    var remarks = string.Join("<br/><br/>", data.Select(i => i));
                                    if (!string.IsNullOrWhiteSpace(task.Remarks))
                                    {
                                        task.Remarks = $"{task.Remarks} </br></br/> <b>System Remarks</b>: {remarks}";
                                    }
                                    else
                                    {
                                        task.Remarks = $"<b>System Remarks</b>:{remarks}";
                                    }
                                    task.AccessRequest.ConflictsRemarks = ConflictRemarks;
                                    DI.Chakra.Logger.ProvisioningLogger log = new Logger.ProvisioningLogger(task.AccessRequest.Id, task.AccessRequest.EndpointSystemId, ep?.EndpointSystemTypeId, string.Empty);
                                    log.WriteLog(Logger.LogLevelType.Info, $"TaskId:{task.Id} | {remarks}");
                                }
                                else
                                {
                                    task.AccessRequest.ConflictsRemarks = ConflictRemarks;
                                }
                            }else if (string.IsNullOrWhiteSpace(ConflictRemarks))
                            {
                                task.AccessRequest.ConflictsRemarks = ConflictRemarks;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(ConflictRemarks))
                        {
                            task.AccessRequest.ConflictsRemarks = ConflictRemarks;
                        }
                    }
                }
                else if (task.BulkRequestId != null && task.BulkRequest != null)
                {
                    if (accessType == AccessType.Revoke)
                    {
                        task.BulkRequest.RequestStatus = AccessRequestStatus.Rejected;
                        task.BulkRequest.ProgressStatus = AccessRequestProgress.RequestRejected;
                    }
                    if (accessType == AccessType.Approve)
                        task.BulkRequest.RequestStatus = AccessRequestStatus.Approved;

                    task.BulkRequest.LastModifiedBy  = UserName;
                    task.BulkRequest.LastModifiedDate = DateTime.Now;
                    //task.BulkRequest.ConflictsRemarks = ConflictRemarks;
                    if (task.BulkRequest.ApplicationType == ApplicationType.EBS_ISU || task.BulkRequest.ApplicationType == ApplicationType.EBS_CRM ||
                        task.BulkRequest.ApplicationType == ApplicationType.MSSL_ISU || task.BulkRequest.ApplicationType == ApplicationType.MSSL_CRM ||
                        task.BulkRequest.ApplicationType == ApplicationType.MSSL || task.BulkRequest.ApplicationType == ApplicationType.EBS
                        )
                    {
                        
                        try
                        {
                            List<PresentationConflictRemarks> data = null;
                            if (!string.IsNullOrWhiteSpace(ConflictRemarks))
                            {
                                data = UtilityMethods.Deserialize<List<PresentationConflictRemarks>>(ConflictRemarks);
                                if (data != null && data.Count > 0)
                                {
                                    EndPoint ep = EndPoint.GetById(task.BulkRequest.EndpointSystemId);
                                    var remarks = string.Join("<br/><br/>", data.Select(i => i));
                                    if (!string.IsNullOrWhiteSpace(task.Remarks))
                                    {
                                        task.Remarks = $"{task.Remarks} </br></br/> <b>System Remarks</b>:{remarks}";
                                    }
                                    else
                                    {
                                        task.Remarks = $"<b>System Remarks</b>:{remarks}";
                                    }
                                    task.BulkRequest.ConflictsRemarks = ConflictRemarks;
                                    DI.Chakra.Logger.ProvisioningLogger log = new Logger.ProvisioningLogger(task.BulkRequest.Id, task.BulkRequest.EndpointSystemId, ep?.EndpointSystemTypeId, string.Empty);
                                    log.WriteLog(Logger.LogLevelType.Info, $"TaskId:{task.Id} | {remarks}");
                                }
                                else
                                {
                                    task.BulkRequest.ConflictsRemarks = ConflictRemarks;
                                }
                            }
                            else if (string.IsNullOrWhiteSpace(ConflictRemarks))
                            {
                                task.BulkRequest.ConflictsRemarks = ConflictRemarks;
                            }
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        if (task.BulkRequest != null)
                        {
                            if (!string.IsNullOrWhiteSpace(ConflictRemarks))
                                task.BulkRequest.ConflictsRemarks = ConflictRemarks;
                        }
                    }
                }
                else if (task.EmployeeEventTriggerId.HasValue)
                {
                    if (accessType == AccessType.Revoke)
                    {
                        task.EmployeeEventTrigger.RequestStatus = AccessRequestStatus.Rejected;
                        var user = User.GetById(task.EmployeeEventTrigger.UserId);
                        if(user != null)
                        {
                            user.RequestStatus = AccessRequestStatus.Rejected;
                            user.Save(currentUser);
                        }
                    }
                }
                  
                task.LastModifiedBy = UserName;
                task.LastModifiedDate = DateTime.Now;
                  
                task.Comments = Comments;
                InterimStore.AddToInterimStore(task, currentUser);

                ApprovalActionCriteria _criteria = new ApprovalActionCriteria()
                {
                    RequestType = (RequestType)task.RequestTypeId,
                    TaskId = task.Id,
                    WorkflowInstanceId = task.WorkflowInstanceId,
                    accessType = accessType
                };

                return _criteria;
            }
        }
    }

    public class SubmitActionCriteria : PresentationWorkflowCriteriaBase
    {
        public List<Task> TaskList { get; set; }

        public long CampaignId { get; set; }
    }

    public class ApprovalActionCriteria : PresentationWorkflowCriteriaBase
    {
        public long TaskId { get; set; }

        public Guid WorkflowInstanceId { get; set; }
        public long AccesRequestId { get; set; }

        public AccessType accessType { get; set; }

    }

    public class WithdrawActionCriteria : PresentationWorkflowCriteriaBase
    {
        public Guid WorkflowInstanceId { get; set; }
    }


}

