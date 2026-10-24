using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Workflow.ServiceHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation; 
using DI.Chakra.Services.EmailingProcessor;
using DI.Chakra.Infrastructure;
using System.Text;

namespace DI.Chakra.Services.AttestationProcessor
{
    public class AttestationLevel1Service
    {
        public Attestation Attestation { get; set; }

        public long AttestationCampaignID { get; set; }

        public int HistoryId { get; set; }


        public AttestationLevel1Service(Attestation _attestation, int _historyId, long campaignId)

        {
            this.Attestation = _attestation;
            this.HistoryId = _historyId;
        }
        public bool RetrieveReviews(IList<User> _users)
        {
            try
            {
                //get the first approver for the attestation request.
                var firstApprover = this.Attestation.WorkflowConfiguration.WorkflowConfigurationDetails.FirstOrDefault(item => item.SequenceNumber == 1);

                if (firstApprover == null)
                {
                    DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Error, "Approver(s) not configured for this attestation", null);
                    return false;
                }
                this.RetrieveAttestationReviews(_users, firstApprover);
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Info, string.Format("Retreiving reviews for the Attestation - {0} is completed.", this.Attestation.AttestationName));

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("RetrieveReviewsError", ex);
            }
        }

        public bool RetrieveReviews(IList<PresentationUserEntitlement> roles)
        {
            try
            {
                //get the first approver for the attestation request.
                var firstApprover = this.Attestation.WorkflowConfiguration.WorkflowConfigurationDetails.FirstOrDefault(item => item.SequenceNumber == 1);

                if (firstApprover == null)
                {
                    DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Error, "Approver(s) not configured for this attestation", null);
                    return false;
                }
                this.RetrieveRoleAttestationReviews(roles, firstApprover);
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Info, string.Format("Retreiving reviews for the Attestation - {0} is completed.", this.Attestation.AttestationName));

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void RetrieveAttestationReviews(IList<User> _users, WorkflowConfigurationDetail firstApprover)
        {
            try
            {
                List<User> noApproverUserList = new List<User>();
                if (firstApprover.ApprovalUserType == ApprovalUserType.User)
                {
                    UserApproval(firstApprover.Approver, _users, noApproverUserList);
                }
                else if (firstApprover.ApprovalUserType == ApprovalUserType.Role)
                {
                    var approver = RoleTypes.GetById(firstApprover.Approver);

                    bool isApproverFromOrgScope = NOrganisationScope.GetAll().Where(t => t.Scope == approver.RoleTypeDesc).Any();

                    if (isApproverFromOrgScope)
                    {
                        OrgScopeApproval(approver.RoleTypeDesc, _users, noApproverUserList);
                    }
                    else
                    {

                        if (firstApprover.Approver == (byte)RoleType.User)
                        {
                            SelfApproval(_users, noApproverUserList);
                        }
                        //else if (firstApprover.Approver == (byte)RoleType.Manager)
                        //{
                        //    ManagerApproval(_users, noApproverUserList);
                        //}
                        else if (firstApprover.Approver == (byte)RoleType.ApplicationOwner)
                        {
                            ApplicationOwnerApproval(_users, noApproverUserList);
                        }
                        else if (firstApprover.Approver == (byte)RoleType.RO || firstApprover.Approver == (byte)RoleType.Manager)
                        {
                            ROApproval(_users, noApproverUserList);
                        }
                        else if (firstApprover.Approver == (byte)RoleType.CO)
                        {
                            COApproval(_users, noApproverUserList);
                        }
                        else if (firstApprover.Approver == (byte)RoleType.HeadOfDepartment || firstApprover.Approver == (byte)RoleType.HeadOfBranch ||
                            firstApprover.Approver == (byte)RoleType.HeadOfSection)
                        {
                            GetOrgHead(_users, noApproverUserList, (RoleType)firstApprover.Approver);
                        }
                    }
                }
                else if (firstApprover.ApprovalUserType == ApprovalUserType.Group)
                {
                    var group = Group.GetByIdIncludingUser(firstApprover.Approver);
                    GroupApproval(firstApprover, group);
                }
                //This is for only unmapped account scenario
                var actions = AttestationSnapshotAction.IsUnMappedAccountWithoutReviewer(this.AttestationCampaignID);
                if (actions)
                {
                    if (this.Attestation.SendEmailOnNoReviewer)
                    {
                        if(!noApproverUserList.Any(i=> i.Id == 0 && i.DisplayName == "Unmapped Account"))
                            noApproverUserList.Add(new User() { Id = 0, DisplayName = "Unmapped Account" });
                    }
                }
                if (noApproverUserList != null && noApproverUserList.Count > 0 && this.Attestation.SendEmailOnNoReviewer)
                {
                    SendEmailNotification(noApproverUserList, firstApprover);
                }

                if (noApproverUserList != null && noApproverUserList.Count > 0)
                {
                    var taskActionUserId = this.Attestation.CreateTaskOnNoReviewer ? this.Attestation.TaskActionUserId : MasterConfigHelper.ITSecurityGroupId;
                    var isGroupTask = (this.Attestation.CreateTaskOnNoReviewer == false || (this.Attestation.CreateTaskOnNoReviewer && this.Attestation.IsGroupTask)) ? true : false;
                    if(isGroupTask == false)
                    {
                        var usr = User.GetById(taskActionUserId.Value);
                        if (usr != null && usr.IsActive == false)
                        {
                            isGroupTask = true;
                            taskActionUserId = MasterConfigHelper.ITSecurityGroupId;
                        }
                    }
                    noApproverUserList.ForEach(a => { PopulateReviewExceptionList((long)taskActionUserId, a, this.Attestation.IsGroupTask); });
                }
            }

            catch (Exception ex)
            {

                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Error, "Exception occured while retrieving reviews", ex);
                throw;
            }
        }
        private void SendEmailNotification(List<User> reviewerNotFound, WorkflowConfigurationDetail firstApprover)
        {
            try
            {
                var campaignName = AttestationCampaign.GetById(this.AttestationCampaignID)?.CampaignName;
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"SendEmailNotification -{campaignName}");
                DomainModel.MailTemplate objTemplate = null;
                objTemplate = MailTemplate.GetByTemplateName(Infrastructure.Constants.AttestationException);
                string mailContent = string.Empty;
                string mailSubject = string.Empty;
                var emailInfo = new EmailInfo();
                mailSubject = objTemplate.MailSubject;
                System.Text.StringBuilder subject = new System.Text.StringBuilder(mailSubject);
                subject.Replace("@@CAMPAIGN_NAME@@", campaignName);

                mailContent = objTemplate.MailContent;
                System.Text.StringBuilder emailBody = new System.Text.StringBuilder(mailContent);
                emailBody.Replace("@@USERREVIEWERNOTFOUND@@", string.Join("<br />", reviewerNotFound.Distinct().Select(a => a.DisplayName)));
                emailBody.Replace("@@ATTESTATION_NAME@@", this.Attestation.AttestationName);
                emailBody.Replace("@@ATTESTATION_DESC@@", this.Attestation.AttestationDescription);
                emailBody.Replace("@@CAMPAIGN_NAME@@", campaignName);
                emailBody.Replace("@@REVIEWLASTDATE@@", DateTime.Now.AddDays(firstApprover.ResponseTimeInDays).ToString("dd/MM/yyyy"));
                emailBody.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailBody.Replace("@@IACMURL@@", MasterConfigHelper.IACMLoginUrl);
                emailInfo.Cc = objTemplate.MailCC;
                emailInfo.Bcc = objTemplate.MailBcc;
                emailInfo.To = $"{MasterConfigHelper.AccountCreatedNotificationTO},{objTemplate.MailTo}";
                emailInfo.From = objTemplate.MailFrom;
                emailInfo.Body = emailBody.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)  
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Exception, $"Error SendEmailNotification:{ex.Message} StackTrace:{ex.StackTrace}", ex);
            }
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
        private void ROApproval(IList<User> users, List<User> noApproverUserList)
        {
            try
            {
                long reviewer = 0;
                foreach (User user in users)
                {
                    reviewer = 0;
                    var roUser = User.GetByEmployeeId(user.ROEmployeeId);
                    if (roUser != null && roUser.Id > 0 && roUser.IsActive == true)
                    {
                        reviewer = roUser.Id;
                        PopulateReviewList(reviewer, user);
                    }
                    else
                    {
                        if (!noApproverUserList.Where(a => a.Id == user.Id).Any())
                        {
                            noApproverUserList.Add(user);
                        }

                        DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                            string.Format("{0}-{1} has been removed from Attestation {2} since no RO is assigned",
                                user.DisplayName, user.UserName, this.Attestation.AttestationName));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void COApproval(IList<User> users, List<User> noApproverUserList)
        {
            try
            {
                long reviewer = 0;
                foreach (User user in users)
                {
                    reviewer = 0;
                    var coUser = User.GetByEmployeeId(user.COEmployeeId);
                    if (coUser != null && coUser.Id > 0 && coUser.IsActive == true)
                    {
                        reviewer = coUser.Id;
                        PopulateReviewList(reviewer, user);
                    }
                    else
                    {
                        if (!noApproverUserList.Where(a => a.Id == user.Id).Any())
                        {
                            noApproverUserList.Add(user);
                        }

                        DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                            string.Format("{0}-{1} has been removed from Attestation {2} since no CO is assigned",
                                user.DisplayName, user.UserName, this.Attestation.AttestationName));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Used to get the organisation head details
        /// </summary>
        /// <param name="users"></param>
        /// <param name="noApproverUserList"></param>
        /// <param name="orgCode">Depending on Section/Department/Branch</param>
        private void GetOrgHead(IList<User> users, List<User> noApproverUserList, RoleType roleType)
        {
            long reviewer = 0;
            string orgCode = string.Empty;
            foreach (User user in users)
            {
                if(roleType == RoleType.HeadOfSection)
                {
                    orgCode = user.SectionCode;
                }
                else if(roleType == RoleType.HeadOfDepartment)
                {
                    orgCode = user.DepartmentCode;
                }
                else if (roleType == RoleType.HeadOfBranch)
                {
                    orgCode = user.BranchCode;
                }
                reviewer = 0;
                reviewer = GetAccessRequestReviewInfo(user.Id, orgCode);
                if (reviewer > 0)
                {
                    PopulateReviewList(reviewer, user);
                }
                else
                {
                    if (!noApproverUserList.Where(a => a.Id == user.Id).Any())
                    {
                        noApproverUserList.Add(user);
                    }

                    DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                        string.Format("{0}-{1} has been removed from Attestation {2} since no Head of Department is assigned",
                            user.DisplayName, user.UserName, this.Attestation.AttestationName));
                }
            }
        }
        private void RetrieveRoleAttestationReviews(IList<PresentationUserEntitlement> roles, WorkflowConfigurationDetail firstApprover)
        {
            try
            {
                List<PresentationUserEntitlement> noApproverRoleList = new List<PresentationUserEntitlement>();
                if (firstApprover.ApprovalUserType == ApprovalUserType.User)
                {
                    roleUserApproval(firstApprover.Approver, roles, noApproverRoleList);
                }
                else if (firstApprover.ApprovalUserType == ApprovalUserType.Role)
                {
                    if (firstApprover.Approver != (byte)RoleType.ApplicationOwner)
                    {
                        roles.ToList().ForEach(a =>
                        {
                            if (!noApproverRoleList.Where(b => b.Id == a.Id).Any())
                            {
                                noApproverRoleList.Add(a);
                            }

                            DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                            string.Format("{0}-({1}) has been removed from Attestation {2} since entitlement doesnt support following approver type {3}.",
                                a.Name, a.Description, this.Attestation.AttestationName, (RoleType)firstApprover.Approver));
                        });
                    }
                    else
                    {
                        roleApplicationOwnerApproval(roles, noApproverRoleList);
                    }
                }

                else if (firstApprover.ApprovalUserType == ApprovalUserType.Group)
                {
                    var group = Group.GetByIdIncludingUser(firstApprover.Approver);
                    GroupApproval(firstApprover, group);
                }

                if (noApproverRoleList != null && noApproverRoleList.Count > 0 && this.Attestation.SendEmailOnNoReviewer)
                {
                    ////new EmailComposer().Compose(EmailTemplate.NotifyAttestationApproval, 1);
                    //DomainModel.MailTemplate objTemplate = null;
                    //objTemplate = DomainModel.MailTemplate.GetByTemplateId((int)EmailTemplate.AttestationException);
                    //DomainModel.EmailInfo emailInfo = TokenReplacer.Execute(objTemplate, this.AttestationCampaignID);
                    //if (null == emailInfo)
                    //    return;
                    //emailInfo.From = objTemplate.MailFrom;
                    //emailInfo.Body = emailInfo.Body.Replace("@@USERREVIEWERNOTFOUND@@", string.Join("<br />", noApproverRoleList.Select(a => a.Name)));

                    //emailInfo.ActivityType = EmailActivityType.Attestation.ToString();

                    //if (string.IsNullOrEmpty(emailInfo.To))
                    //    emailInfo.To = DomainModel.ConfigurationMaster.GetByConfigKey("AccountCreatedNotificationTo", Product.IACM).ConfigValue.ToString();

                    //if (objTemplate != null && objTemplate.IsActive.HasValue && !objTemplate.IsActive.Value)
                    //    emailInfo.Status = "Excluded";

                    //EmailHandler.Send(emailInfo);
                }

                if (noApproverRoleList != null && noApproverRoleList.Count > 0)
                {
                    if (this.Attestation.CreateTaskOnNoReviewer)
                    {
                        var taskActionUserId = this.Attestation.TaskActionUserId;
                        if (this.Attestation.IsGroupTask)
                            taskActionUserId = Group.GetById((long)this.Attestation.TaskActionUserId).Id;
                        noApproverRoleList.ForEach(a => { PopulateReviewExceptionList((long)taskActionUserId, a, this.Attestation.IsGroupTask); });
                    }
                    else
                    {
                        noApproverRoleList.ForEach(a => { PopulateReviewExceptionList(null, a, this.Attestation.IsGroupTask); });
                    }
                }
            }
            catch (Exception ex)
            {

                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Error, "Exception occured while retrieving reviews for role attestation", ex);
                throw;
            }
        }

        public void AutoSubmitLevel1Task(long CampaignId)
        {
            try
            {
                var tasks = Task.GetByParentCampaignIdAndWorkFlowState(CampaignId, 1).ToList();
                foreach (var task in tasks)
                {
                    try
                    {
                        if (task != null && task.ActionUserId.HasValue && task.ActionUserId.Value == 1)
                        {
                            PresentationWorkflowCriteriaBase _criteria = new ApprovalActionCriteria()
                            {
                                RequestType = RequestType.Attestation,
                                TaskId = task.Id,
                                WorkflowInstanceId = task.WorkflowInstanceId
                            };

                            ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(_criteria);
                            _serviceAccessStrategy.AccessWorkflowService();
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Exception, string.Format("Error while auto submit for the Attestation - {0}.", this.Attestation.AttestationName), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Exception, string.Format("Error while auto submit for the Attestation - {0}.", this.Attestation.AttestationName), ex);
            }
        }

        public bool Save()
        {
            bool bReturn = false;

            try
            {
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Info, "Started Populating the tasks based on the reviews created.");

                List<Task> tasks = this.PopulateTasks();

                if (tasks == null || tasks.Count == 0)
                {
                    DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Error, string.Format("No task created for the Attestation - {0}, CampaignId ({1}).", this.Attestation.AttestationName,
                        this.AttestationCampaignID));
                    return false;
                }

                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Info, string.Format("Populated {0} tasks for the Attestation - {1}.", tasks.Count.ToString(), this.Attestation.AttestationName));

                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Info, string.Format("Started creating tasks for the Attestation - {0}.", this.Attestation.AttestationName));

                PresentationWorkflowCriteriaBase _criteria = new SubmitActionCriteria()
                {
                    RequestType = RequestType.Attestation,
                    TaskList = tasks,
                    CampaignId = this.AttestationCampaignID
                };

                ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(_criteria);
                _serviceAccessStrategy.AccessWorkflowService();
                
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Info, string.Format("Completed creating tasks for the Attestation - {0}.", this.Attestation.AttestationName));

                bReturn = true;

            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Exception, string.Format("Error while calling workflow for the Attestation - {0}.", this.Attestation.AttestationName), ex);
            }
            return bReturn;
        }
        private List<Task> PopulateTasks()
        {
            List<Task> tasks = new List<Task>();
            Task task = null;

            var hierarchyDetails = this.Attestation.AttestationHierarchy.FirstOrDefault();
            var reviewers = AttestationSnapshotAction.GetReviewerByCampaign(this.AttestationCampaignID, hierarchyDetails != null ? hierarchyDetails.EndpointHierarchy.Level : 1)
                .Where(a => !a.IsException && a.ReviewerId.HasValue && a.ReviewerId.Value != 0).ToList();
            var unmappedReviewsToSkip = AttestationSnapshotAction.GetByCampaignId(this.AttestationCampaignID, hierarchyDetails != null ? hierarchyDetails.EndpointHierarchy.Level : 1)
                .Where(a => !a.IsException && !a.ReviewerId.HasValue && a.AttestationSnapshot.DIUserName.ToLower() == "unmapped").ToList();

            var reviewersId = reviewers.Select(a => new { ReviewerId = (long)a.ReviewerId, IsReviewerGroup = a.IsReviewerGroup }).Distinct().ToList();

            if (reviewers.Any())
            {
                var firstApprover = this.Attestation.WorkflowConfiguration.WorkflowConfigurationDetails.FirstOrDefault(item => item.SequenceNumber == 1);
                Group group = new Group();
                bool isGroup = false;
                if (firstApprover.ApprovalUserType == ApprovalUserType.Group)
                {
                    group = Group.GetById(firstApprover.Approver);
                    isGroup = true;
                }

                foreach (var reviewer in reviewersId)
                {
                    isGroup = reviewer.IsReviewerGroup;
                    if (isGroup)
                    {
                        group = Group.GetById(reviewer.ReviewerId);
                    }
                    task = new Task()
                    {
                        ActionUserId = reviewer.ReviewerId,
                        AttestationId = this.Attestation.Id,
                        IsActive = true,
                        RequestTypeId = (byte)RequestType.Attestation,
                        WorkflowStateId = (byte)WorkflowState.Level1,
                        WorkflowConfigurationId = this.Attestation.WorkflowConfigurationId,
                        Action = StatusType.Pending,
                        IsGroupTask = isGroup,
                        CreatedDate = DateTime.Now,
                        AttestationCampaignId = this.AttestationCampaignID,
                    };

                    if (task.IsGroupTask && !firstApprover.RequiresApprovalFromAll && !firstApprover.IsMockGroup)
                    {
                        task.ActionUserId = null;
                        var deafultGroup = Group.GetById(MasterConfigHelper.ITSecurityGroupId);
                        task.ActionGroupId = firstApprover.ApprovalUserType == ApprovalUserType.Group ? firstApprover.Approver : (group != null ? group.Id : (deafultGroup != null ? deafultGroup.Id : 1));
                        if (!string.IsNullOrEmpty(group.GroupName))
                            task.GroupName = group.GroupName;
                    }

                    if (!task.IsGroupTask)
                    {
                        //var delegation = TaskDelegation.GetFinalDelegatedToApprover(RequestType.Attestation, reviewer.ReviewerId, new List<TaskDelegation>());
                        var delegation = TaskDelegation.GetByRequestType(RequestType.Attestation, reviewer.ReviewerId);
                        if (delegation != null)
                        {
                            //task.ActionUserId = delegation.DelegatedTo;
                            //task.IsDelegated = true;
                            //task.DelegatedBy = "System";
                            //task.DelegatedDate = DateTime.Now;
                            //task.DelegatedFrom = reviewer.ReviewerId;
                            //task.DelegationComments = delegation.DelegationComments;

                            List<long> delgateUsers = new List<long>();
                            task.ActionUserId = GetReviewerbyDelegation(delegation.DelegatedTo, delgateUsers);
                            task.IsDelegated = true;
                            task.DelegatedBy = User.GetById(delegation.ActionUser)?.DisplayName;
                            task.DelegatedDate = DateTime.Now;
                            task.DelegatedFrom = delgateUsers.Count > 1 ? delgateUsers[delgateUsers.Count - 2] : reviewer.ReviewerId;
                            //task.DelegationComments = $"{delegation.DelegationComments}";
                            task.DelegationComments = $"{ task.DelegationComments}<br/> Date - {task.DelegatedDate.Value.ToString("dd-MM-yyyy hh:mm tt")} By - {task.DelegatedBy}, From - {User.GetById(task.DelegatedFrom.Value)?.DisplayName}, To - {User.GetById(task.ActionUserId.Value)?.DisplayName}, Comments - {delegation.DelegationComments}";

                            var actionNeedsToUpdateWithDelegatedUser = AttestationSnapshotAction.GetReviewerByCampaign(this.AttestationCampaignID).Where(a => (long)a.ReviewerId == reviewer.ReviewerId && a.IsReviewerGroup == reviewer.IsReviewerGroup && !a.IsException).ToList();
                            foreach (var actionNeedToUpdateWithDelegatedUser in actionNeedsToUpdateWithDelegatedUser)
                            {
                                actionNeedToUpdateWithDelegatedUser.ReviewerId = task.ActionUserId;
                                actionNeedToUpdateWithDelegatedUser.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                    }

                    tasks.Add(task);
                }
            }

            if (unmappedReviewsToSkip != null && unmappedReviewsToSkip.Count > 0)
            {

                long actionUserId = MasterConfigHelper.ITSecurityGroupId;
                bool isGroup = true;
                string comments = null;
                if (this.Attestation.CreateTaskOnNoReviewer)
                {
                    if (this.Attestation.IsGroupTask)
                    {
                        actionUserId = this.Attestation.TaskActionUserId.Value;
                    }
                    else
                    {
                        if (this.Attestation.TaskActionUserId.HasValue)
                        {
                            var usr = User.GetById(this.Attestation.TaskActionUserId.Value);
                            if(usr != null && usr.IsActive)
                            {
                                actionUserId = usr.Id;
                                isGroup = false;
                            }
                            else
                            {
                                comments = $"Attestation task action user is Inactive:{usr?.DisplayName}";
                            }
                        }
                        else
                        {
                            comments = $"Attestation task action user doesnot exists. Hence assiging the task to ITSecurity team";
                        }
                    }
                }
                task = new Task()
                {
                    //skipped with default user (system)
                    ActionUserId = actionUserId,
                    AttestationId = this.Attestation.Id,
                    IsActive = true,
                    RequestTypeId = (byte)RequestType.Attestation,
                    WorkflowStateId = (byte)WorkflowState.Level1,
                    WorkflowConfigurationId = this.Attestation.WorkflowConfigurationId,
                    Action = StatusType.Pending,
                    IsGroupTask = isGroup,
                    Comments = comments,
                    CreatedDate = DateTime.Now,
                    AttestationCampaignId = this.AttestationCampaignID,
                };
                tasks.Add(task);

                unmappedReviewsToSkip.ForEach(a => { a.IsReviewerGroup = isGroup; a.ReviewerId = actionUserId; a.Save(MasterConfigHelper.ServiceUser); });
            }

            var exceptionReviewers = AttestationSnapshotAction.GetReviewerByCampaign(this.AttestationCampaignID)
                .Where(a => a.IsException)
                .Select(a => (long)a.ReviewerId).Distinct().ToList();

            //craeting exception task to configured user/group, if any.
            if (exceptionReviewers.Any() && this.Attestation.CreateTaskOnNoReviewer && this.Attestation.TaskActionUserId.HasValue)
            {
                long exceptionReviewer = exceptionReviewers.First();
                bool isGroup = this.Attestation.IsGroupTask;
                Group group = new Group();
                if (isGroup)
                {
                    group = Group.GetById(exceptionReviewer);
                    exceptionReviewer = 0;
                }

                if (!tasks.Any(a => a.IsGroupTask == isGroup &&
                (a.ActionGroupId == group.Id || a.ActionUserId == exceptionReviewer)))
                {
                    task = new Task()
                    {
                        Remarks = "This task create, as part of exception configuration - Reviewer not found.",
                        ActionUserId = exceptionReviewer,
                        AttestationId = this.Attestation.Id,
                        IsActive = true,
                        RequestTypeId = (byte)RequestType.Attestation,
                        WorkflowStateId = (byte)WorkflowState.Level1,
                        WorkflowConfigurationId = this.Attestation.WorkflowConfigurationId,
                        Action = StatusType.Pending,
                        IsGroupTask = isGroup,
                        CreatedDate = DateTime.Now,
                        AttestationCampaignId = this.AttestationCampaignID,
                    };

                    if (task.IsGroupTask)
                    {
                        task.ActionUserId = null;
                        task.ActionGroupId = group.Id;
                        if (!string.IsNullOrEmpty(group.GroupName))
                        {
                            task.GroupName = group.GroupName;
                            task.Approver = "Group - " + group.GroupName;
                        }
                    }
                    else
                        task.Approver = "User";

                    if (!task.IsGroupTask)
                    {
                        var delegation = TaskDelegation.GetByRequestType(RequestType.Attestation, exceptionReviewer);
                        if (delegation != null)
                        {
                            List<long> delgateUsers = new List<long>();
                            task.ActionUserId = GetReviewerbyDelegation(delegation.DelegatedTo, delgateUsers);
                            task.IsDelegated = true;
                            task.DelegatedBy = "System";
                            task.DelegatedDate = DateTime.Now;
                            task.DelegatedFrom = delgateUsers.Count > 1 ? delgateUsers[delgateUsers.Count - 2] : exceptionReviewer;
                            task.DelegationComments = $"{delegation.DelegationComments}";

                            var actionNeedsToUpdateWithDelegatedUser = AttestationSnapshotAction.GetReviewerByCampaign(this.AttestationCampaignID).Where(a => (long)a.ReviewerId == exceptionReviewer && a.IsException).ToList();
                            foreach (var actionNeedToUpdateWithDelegatedUser in actionNeedsToUpdateWithDelegatedUser)
                            {
                                actionNeedToUpdateWithDelegatedUser.ReviewerId = task.ActionUserId;
                                actionNeedToUpdateWithDelegatedUser.Save(MasterConfigHelper.ServiceUser);
                            }
                        }

                        //var delegation = TaskDelegation.GetFinalDelegatedToApprover(RequestType.Attestation, exceptionReviewer, new List<TaskDelegation>());
                        //if (delegation != null)
                        //{
                        //    task.ActionUserId = delegation.DelegatedTo;
                        //    task.IsDelegated = true;
                        //    task.DelegatedBy = "System";
                        //    task.DelegatedDate = DateTime.Now;
                        //    task.DelegatedFrom = exceptionReviewer;
                        //    task.DelegationComments = delegation.DelegationComments;

                           
                        //}
                    }

                    tasks.Add(task);
                }
                else
                {
                    var existingTask = tasks.Where(a => a.IsGroupTask == isGroup &&
                (a.ActionGroupId == group.Id || a.ActionUserId == exceptionReviewer)).ToList();

                    if (existingTask != null && existingTask.Count > 0)
                    {
                        existingTask.ForEach(a => { a.Remarks += "This task contains - Reviewer not found records as well."; });
                    }
                }
            }

            return tasks;
        }
        private void UserApproval(long approver, IList<User> userList, List<User> noApproverUserList)
        {
            var approverUser = User.GetById(approver);

            if (approverUser != null && approverUser.IsActive)
            {
                var actions = AttestationSnapshotAction.GetByCampaign(this.AttestationCampaignID).ToList();

                PopulateReviewList(approver, actions);
            }
            else
            {
                userList.ToList().ForEach(a =>
                 {
                     if (!noApproverUserList.Where(b => b.Id == a.Id).Any())
                     {
                         noApproverUserList.Add(a);
                     }

                     DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                     string.Format("{0}-{1} has been removed from Attestation {2} since user {3} not active.",
                         a.DisplayName, a.UserName, this.Attestation.AttestationName, approverUser == null ? "" : approverUser.DisplayName));
                 });
            }
        }
        private void roleUserApproval(long approver, IList<PresentationUserEntitlement> roleList, List<PresentationUserEntitlement> noApproverRoleList)
        {
            var approverUser = User.GetById(approver);

            if (approverUser != null && approverUser.IsActive)
            {
                var actions = AttestationSnapshotAction.GetByCampaign(this.AttestationCampaignID).ToList();

                PopulateReviewList(approver, actions);
            }
            else
            {
                roleList.ToList().ForEach(a =>
                {
                    if (!noApproverRoleList.Where(b => b.Id == a.Id).Any())
                    {
                        noApproverRoleList.Add(a);
                    }

                    DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                    string.Format("{0}-({1}) has been removed from Attestation {2} since user {3} not active.",
                        a.Name, a.Description, this.Attestation.AttestationName, approverUser == null ? "" : approverUser.DisplayName));
                });
            }
        }
        private void SelfApproval(IList<User> userList, List<User> noApproverUserList)
        {
            long reviewer = 0;
            var actions = AttestationSnapshotAction.GetByCampaign(this.AttestationCampaignID).ToList();

            foreach (var action in actions)
            {
                reviewer = action.AttestationSnapshot.UserId;
                if(reviewer > 0)
                    PopulateReviewList(reviewer, action);
                else
                {
                    if (!noApproverUserList.Where(a => a.Id == reviewer).Any())
                    {
                        noApproverUserList.Add(new User() { Id = 0 , DisplayName= "Unmapped Account"});
                    }
                }
            }
        }
        private void ManagerApproval(IList<User> userList, List<User> noApproverUserList)
        {
            long reviewer = 0;
            foreach (User user in userList)
            {
                reviewer = 0;
                var manager = new User();// User.GetImmediateManager(user.Id);
                if (manager != null && manager.Id != 0 && manager.IsActive == true)
                {
                    reviewer = manager.Id;
                    PopulateReviewList(reviewer, user);
                }
                else
                {
                    if (!noApproverUserList.Where(a => a.Id == user.Id).Any())
                    {
                        noApproverUserList.Add(user);
                    }

                    DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                        string.Format("{0}-{1} has been removed from Attestation {2} since no Manager is assigned",
                            user.DisplayName, user.UserName, this.Attestation.AttestationName));
                }
            }
        }
        private void UnMappedAccountApproverLevelSkipped()
        {

        }
        private void roleRoleOwnerApproval(IList<PresentationUserEntitlement> roleList, WorkflowConfigurationDetail firstApprover, List<PresentationUserEntitlement> noApproverRoleList)
        {
            long reviewer = 0;
            bool isGroup = false;
            foreach (var role in roleList)
            {
                reviewer = 0;
                isGroup = false;
                if (firstApprover.Approver == (int)RoleType.Level1Approver)
                {
                    if (role.Level1Approver.HasValue)
                    {
                        if (role.IsLevel1Group.HasValue && role.IsLevel1Group.Value)
                        {
                            var group = Group.GetById(role.Level1Approver.Value);
                            if (group != null)
                            {
                                reviewer = group.Id;
                                isGroup = true;
                            }
                        }
                        else
                        {
                            var user = User.GetById(role.Level1Approver.Value);
                            if (user != null && user.IsActive)
                                reviewer = user.Id;
                        }
                    }
                }
                else if (firstApprover.Approver == (int)RoleType.Level2Approver)
                {
                    if (role.Level2Approver.HasValue)
                    {
                        if (role.IsLevel2Group.HasValue && role.IsLevel2Group.Value)
                        {
                            var group = Group.GetById(role.Level2Approver.Value);
                            if (group != null)
                            {
                                reviewer = group.Id;
                                isGroup = true;
                            }
                        }
                        else
                        {
                            var user = User.GetById(role.Level2Approver.Value);
                            if (user != null && user.IsActive)
                                reviewer = user.Id;
                        }
                    }
                }

                if (reviewer > 0)
                {
                    PopulateReviewList(reviewer, role, isGroup);
                }
                else
                {
                    if (!noApproverRoleList.Where(a => a.Name == role.Name).Any())
                    {
                        noApproverRoleList.Add(role);
                    }

                    DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                        string.Format("{0}-{1} has been removed from Attestation {2} since no role owner available.",
                            role.Name, role.Description, this.Attestation.AttestationName));
                }
            }
        }
        private void OrgScopeApproval(string orgScope, IList<User> userList, List<User> noApproverUserList)
        {
            long reviewerId = 0;
            foreach (User user in userList)
            {
                var reviewer = NOrganisationHead.GetOrganisationLevelHead(user.Id, orgScope);
                if (reviewer != null && reviewer.IsActive == true)
                {
                    reviewerId = reviewer.Id;
                    PopulateReviewList(reviewerId, user);
                }
                else
                {
                    if (!noApproverUserList.Where(a => a.Id == user.Id).Any())
                    {
                        noApproverUserList.Add(user);
                    }

                    DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                        string.Format("{0}-{1} has been removed from Attestation {2} since no Manager for organisation",
                            user.DisplayName, user.UserName, this.Attestation.AttestationName));
                }
            }
        }
        private void ApplicationOwnerApproval(IList<User> userList, List<User> noApproverUserList)
        {
            try
            {
                List<long> reviewers = new List<long>();
                long epOwnerUserId = 0;
                Dictionary<long, long> epOwners = new Dictionary<long, long>();

                // all application owners (Distinct) for the endpoints selected for this attestation
                reviewers = this.Attestation.AttestationEndpoint.Select(item => item.EndPoint.OwnerUserId.Value).Distinct().ToList();

                // Dictionary of Endpoint System ID - Owner User ID
                epOwners = (from attEP in this.Attestation.AttestationEndpoint
                            select new { key = attEP.EndPoint.Id, value = attEP.EndPoint.OwnerUserId.Value })
                        .Distinct().OrderBy(c => c.value)
                        .ToDictionary(c => c.key, c => c.value);

                // iterate endpoint owners for this attestation
                foreach (var epOwner in epOwners)
                {
                    epOwnerUserId = epOwner.Value;
                    var epOwnerUser = User.GetById(epOwnerUserId);

                    if (epOwnerUser != null && epOwnerUser.IsActive)
                    {
                        var actions = AttestationSnapshotAction.GetByEndpoint(epOwner.Key, this.AttestationCampaignID).ToList();

                        // set endpoint owner id as reviewer Id
                        foreach (var action in actions)
                        {
                            // if this user is the reviewer
                            //if (epOwnerUserId == action.AttestationSnapshot.UserId)
                            //{
                            //    // log it and continue to next iteration
                            //    DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning, string.Format("{0}-{1} has been removed from Attestation {2} since he himself is a endpoint owner.",
                            //        action.AttestationSnapshot.DIUserDisplayName, action.AttestationSnapshot.AccountName, this.Attestation.AttestationName));
                            //    continue;
                            //}
                            //else
                            {
                                PopulateReviewList(epOwnerUserId, action);
                            }
                        }
                    }
                    else
                    {
                        userList.ToList().ForEach(a =>
                        {
                            if (!noApproverUserList.Where(b => b.Id == a.Id).Any())
                            {
                                noApproverUserList.Add(a);
                            }

                            DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                            string.Format("{0}-{1} has been removed from Attestation {2} since application owner is not active.",
                                a.DisplayName, a.UserName, this.Attestation.AttestationName));
                        });
                    }
                    // TODO: What if multiple endpoint owners for same endpoint
                    // Group Task?
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void roleApplicationOwnerApproval(IList<PresentationUserEntitlement> roleList, List<PresentationUserEntitlement> noApproverRoleList)
        {
            try
            {
                List<long> reviewers = new List<long>();
                long epOwnerUserId = 0;
                Dictionary<long, long> epOwners = new Dictionary<long, long>();

                // all application owners (Distinct) for the endpoints selected for this attestation
                reviewers = this.Attestation.AttestationEndpoint.Select(item => item.EndPoint.OwnerUserId.Value).Distinct().ToList();

                // Dictionary of Endpoint System ID - Owner User ID
                epOwners = (from attEP in this.Attestation.AttestationEndpoint
                            select new { key = attEP.EndPoint.Id, value = attEP.EndPoint.OwnerUserId.Value })
                        .Distinct().OrderBy(c => c.value)
                        .ToDictionary(c => c.key, c => c.value);

                // iterate endpoint owners for this attestation
                foreach (var epOwner in epOwners)
                {
                    epOwnerUserId = epOwner.Value;
                    var epOwnerUser = User.GetById(epOwnerUserId);

                    if (epOwnerUser != null && epOwnerUser.IsActive)
                    {
                        var actions = AttestationSnapshotAction.GetByEndpoint(epOwner.Key, this.AttestationCampaignID).ToList();

                        // set endpoint owner id as reviewer Id
                        foreach (var action in actions)
                        {
                            // if this user is the reviewer
                            if (epOwnerUserId == action.AttestationSnapshot.UserId)
                            {
                                // log it and continue to next iteration
                                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning, string.Format("{0}-{1} has been removed from Attestation {2} since he himself is a endpoint owner.",
                                    action.AttestationSnapshot.DIUserDisplayName, action.AttestationSnapshot.AccountName, this.Attestation.AttestationName));
                                continue;
                            }
                            else
                            {
                                PopulateReviewList(epOwnerUserId, action);
                            }
                        }
                    }
                    else
                    {
                        roleList.ToList().ForEach(a =>
                        {
                            if (!noApproverRoleList.Where(b => b.Id == a.Id).Any())
                            {
                                noApproverRoleList.Add(a);
                            }

                            DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                            string.Format("{0}-{1} has been removed from Attestation {2} since application owner is not active.",
                                a.Name, a.Description, this.Attestation.AttestationName));
                        });
                    }
                    // TODO: What if multiple endpoint owners for same endpoint
                    // Group Task?
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void GroupApproval(WorkflowConfigurationDetail firstApprover, Group group)
        {
            long reviewer = 0;
            bool isGroup = false;

            //foreach (User user in userList)
            {
                if (firstApprover.IsMockGroup)
                {
                    isGroup = false;
                    var groupUsers = group.Users.Select(item => item.Id).ToList();

                    //if first approver is group then create the task for only one user. 
                    //For the other users in the group tasks will be created thru workflow.
                    reviewer = groupUsers.Min();
                }
                else
                {
                    if (firstApprover.RequiresApprovalFromAll)
                    {
                        isGroup = false;
                        var groupUsers = group.Users.Select(item => item.Id).ToList();

                        //if first approver is group then create the task for only one user. 
                        //For the other users in the group tasks will be created thru workflow.
                        reviewer = groupUsers.Min();
                    }
                    else
                    {
                        isGroup = true;
                        reviewer = firstApprover.Approver;
                    }
                }

                var actions = AttestationSnapshotAction.GetByCampaign(this.AttestationCampaignID).ToList();

                if (actions != null && actions.Count > 0)
                {
                    foreach (var action in actions)
                    {
                        action.IsException = false;
                        action.ReviewerId = reviewer;
                        action.IsReviewerGroup = true;
                        action.Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
        }

        /// <summary>
        /// Construct ReviewerId & ReviewEndPointSystem 
        /// </summary>
        /// <param name="reviewer"></param>
        /// <param name="user"></param>
        /// <param name="isGroup"></param>
        private void PopulateReviewList(long reviewer, User user, bool isGroup = false)
        {
            // get list of snapshot actions for selected user and set reviewer details
            var actions = AttestationSnapshotAction.GetByDIUserId(user.Id, this.AttestationCampaignID);

            // if any first level actions are found for this user
            if (actions != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    action.IsException = false;
                    action.ReviewerId = reviewer;
                    action.Save(MasterConfigHelper.ServiceUser);
                }
            }
            else
            {
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                    string.Format(
                        "{0}-{1} removed from Attestation {2} since he/she does not have account in any endpoints.",
                        user.DisplayName, user.UserName, this.Attestation.AttestationName)); // TODO: Use Campaign Name
            }
        }

        private void PopulateReviewList(long reviewer, PresentationUserEntitlement role, bool isGroup = false)
        {
            // get list of snapshot actions for selected user and set reviewer details
            var actions = AttestationSnapshotAction.GetByRoleId(role.EntitlementId, role.EntitlementType, this.AttestationCampaignID);

            // if any first level actions are found for this user
            if (actions != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    action.IsException = false;
                    action.ReviewerId = reviewer;
                    action.IsReviewerGroup = isGroup;
                    action.Save(MasterConfigHelper.ServiceUser);
                }
            }
            else
            {
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                    string.Format(
                        "{0}-{1} removed from Attestation {2} since role doesnt exists.",
                        role.Name, role.Description, this.Attestation.AttestationName)); // TODO: Use Campaign Name
            }
        }

        private void PopulateReviewList(long reviewer, AttestationSnapshotAction action, bool isGroup = false)
        {
            // if any first level actions are found for this user
            if (action != null)
            {
                action.IsException = false;
                action.ReviewerId = reviewer;
                action.IsReviewerGroup = isGroup;
                action.Save(MasterConfigHelper.ServiceUser);
            }
        }

        private void PopulateReviewList(long reviewer, List<AttestationSnapshotAction> actions, bool isGroup = false)
        {
            // if any first level actions are found for this user
            if (actions != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    action.IsException = false;
                    action.ReviewerId = reviewer;
                    action.IsReviewerGroup = isGroup;
                    action.Save(MasterConfigHelper.ServiceUser);
                }
            }
        }

        private void PopulateReviewList(long reviewer, User user, long endpointSystemId, bool isGroup = false)
        {
            // get list of snapshot actions for selected user and set reviewer details
            var actions = AttestationSnapshotAction.GetByDIUserId(user.Id, this.AttestationCampaignID).Where(a => a.AttestationSnapshot.EndpointId == endpointSystemId).ToList();

            // if any first level actions are found for this user
            if (actions != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    action.IsException = false;
                    action.ReviewerId = reviewer;
                    action.Save(MasterConfigHelper.ServiceUser);
                }
            }
            else
            {
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                    string.Format(
                        "{0}-{1} removed from Attestation {2} since he/she does not have account in any endpoints.",
                        user.DisplayName, user.UserName, this.Attestation.AttestationName)); // TODO: Use Campaign Name
            }
        }

        public void PopulateReviewExceptionList(long? reviewer, User user, bool isGroupTask)
        {
            // get list of snapshot actions for selected user and set reviewer details
            var actions = AttestationSnapshotAction.GetByDIUserId(user.Id, this.AttestationCampaignID);

            // if any first level actions are found for this user
            if (actions != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    action.ReviewerId = reviewer;
                    action.IsException = true;
                    action.IsReviewerGroup = isGroupTask;
                    action.Save(MasterConfigHelper.ServiceUser);
                }
            }
            else
            {
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                    string.Format(
                        "{0}-{1} removed from Attestation {2} since he/she does not have account in any endpoints.",
                        user.DisplayName, user.UserName, this.Attestation.AttestationName)); // TODO: Use Campaign Name
            }
        }

        public void PopulateReviewExceptionList(long? reviewer, PresentationUserEntitlement role, bool isGroupTask)
        {
            // get list of snapshot actions for selected user and set reviewer details
            var actions = AttestationSnapshotAction.GetByRoleId(role.EntitlementId, role.EntitlementType, this.AttestationCampaignID);

            // if any first level actions are found for this user
            if (actions != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    action.ReviewerId = reviewer;
                    action.IsException = true;
                    action.IsReviewerGroup = isGroupTask;
                    action.Save(MasterConfigHelper.ServiceUser);
                }
            }
            else
            {
                DILog.AttestationLogger.WriteLog(this.Attestation.Id, LogLevelType.Warning,
                    string.Format(
                        "{0}-{1} removed from Attestation {2} since he/she does not have account in any endpoints.",
                        role.Name, role.Description, this.Attestation.AttestationName)); // TODO: Use Campaign Name
            }
        }
        long GetReviewerbyDelegation(long ReviewerId, List<long> delgateUsers)
        {
            delgateUsers.Add(ReviewerId);

            var delegation = TaskDelegation.GetByRequestType(RequestType.Attestation, ReviewerId);
            if (delegation != null)
            {
                return GetReviewerbyDelegation(delegation.DelegatedTo, delgateUsers);
            }
            else
            {
                return delgateUsers[delgateUsers.Count - 1];
            }

        }
    }
}


