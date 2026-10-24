using System;
using DI.Chakra.Infrastructure;
using System.Collections.Generic;
using System.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.UsrControl.Common.Task
{
    public partial class InfoUC : UIBaseUC
    {
        #region Members
        private Dto.AdHocAccessRequest mAdhocAccessRequest = null;
        private Dto.AccessRequest mHRMSAccessRequest = null;
        private const string V_ACCESSREQUEST = "V_ACCESSREQUEST";
        #endregion

        #region Properties
        public Dto.AdHocAccessRequest AdhocAccessRequest
        {
            set
            {
                mAdhocAccessRequest = value;
                SetForm(mAdhocAccessRequest);
            }
        }
        public Dto.AccessRequest HRMSAccessRequest
        {
            set
            {
                mHRMSAccessRequest = value;
                SetHRMSAccessRequestForm(mHRMSAccessRequest);
            }
        }
        private DI.Chakra.Dto.AccessRequest AccessRequest
        {
            get
            {
                if (ViewState[V_ACCESSREQUEST] == null)
                    return new DI.Chakra.Dto.AccessRequest();
                return (DI.Chakra.Dto.AccessRequest)ViewState[V_ACCESSREQUEST];
            }
            set
            {
                ViewState[V_ACCESSREQUEST] = value;
            }
        }

        private Dto.Task _employeeEventTask = null;
        public Dto.Task EmployeeEventTask
        {
            set
            {
                _employeeEventTask = value;
                SetFormForEmployeeEvent(_employeeEventTask);
            }
        }
        #endregion

        #region Methods
        void ShowWindow()
        {
            var key = "showWindoww";
            string script = "Sys.Application.remove_load(showWindow);Sys.Application.add_load(showWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }
        public bool ShowWithDraw
        {
            set
            {
                this.divWithDraw.Visible = value;
            }
            
        }
        void CloseWindow()
        {
            var key = "closeWindoww";
            string script = "Sys.Application.remove_load(closeWindow);Sys.Application.add_load(closeWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }
        private void SetFormForEmployeeEvent(Dto.Task task)
        {
            if (task.Id > 0)
            {
                ShowApprovedBy(task);
                lblTaskStatus.Text = task.Action.GetDescription();
                var employeeEventTrigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetById(Convert.ToInt64(task.EmployeeEventTriggerId));

                LabelTaskName.Text = employeeEventTrigger.EventType.GetDescription();
                LabelTaskID.Text = task.Id.ToString();

                LabelRequestedDate.Text = task.CreatedDate.ToString("dd-MM-yyyy");
                requesttimeLabel.Text = task.CreatedDate.ToShortTimeString();

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", Convert.ToString(employeeEventTrigger.UserId));

                var currentUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                 UserInfoUC.User = currentUser;

                Dto.User user = null;

                user = UtilityMethods.Deserialize<Dto.User>(employeeEventTrigger.OldUserDetails);

                if (user == null)
                    user = currentUser;

                reqDepartment.Text = user.OrgDepartment;
                reqDesignation.Text = user.PositionName;
                reqSupName.Text = user.Supervisor;// user.ManagerDisplayName;
                reqCompany.Text = user.OrgCompany;
                //reqDivision.Text = user.OrgDivision;
                reqSection.Text = user.OrgSection;
                reqUnit.Text = user.OrgUnit;
                reqBranch.Text = user.OrgBranch;
                if (string.IsNullOrWhiteSpace(user.NCompany))
                {
                    lblNonEmployeeCompany.Text = user.NCompany;
                    nonEmployeeCompanyDiv.Visible = true;
                }

                CurrentAccessUC.EntityId = user.Id;

                Dictionary<string, string> ParamRequestUserId = new Dictionary<string, string>();
                ParamRequestUserId.Add("name", employeeEventTrigger.CreatedBy);
                var requester = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                    (DomainApiAction.User.GetByUserName, ParamRequestUserId);

                if (requester != null)
                {
                    LabelRequestByName.Text = requester.DisplayName;
                }
                else
                {
                    LabelRequestByName.Text = String.Empty;
                }

                string status = employeeEventTrigger.RequestStatus == Infrastructure.Enums.AccessRequestStatus.Requested ? Infrastructure.Enums.AccessRequestProgress.WaitingforApproval.GetDescription() : employeeEventTrigger.RequestStatus.GetDescription();
                string approver = string.Empty;
                if (task.ActionUserId != null)
                {
                    Dictionary<string, string> pactionUser = new Dictionary<string, string>();
                    pactionUser.Add("id", Convert.ToString(task.ActionUserId.Value));
                    Dto.User actionUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                        (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, pactionUser);

                    approver = string.Format("{0}", (task.IsGroupTask) ?
                       actionUser.DisplayName : actionUser.DisplayName + " - " + task.Approver);
                }
                else if (task.IsGroupTask)
                {
                    approver = string.Format("{0}", Convert.ToString(task.GroupName));

                    if (task.AssignedTo.HasValue)
                    {
                        Dictionary<string, string> pactionUser = new Dictionary<string, string>();
                        pactionUser.Add("id", Convert.ToString(task.AssignedTo.Value));
                        Dto.User actionUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                            (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, pactionUser);

                        approver = string.Format("{0} - [{1}]", approver, actionUser.DisplayName);
                    }
                }
                LabelStatus.Text = !string.IsNullOrWhiteSpace(approver) ? status + " ("+approver+")" : status;

            }
        }
        public void SetFormForNonEmployeeUser(Dto.EmployeeEventTrigger employeeEventTrigger, Dto.Task task, Dto.User user)
        {
            //ShowWithDraw = user.RequestStatus == AccessRequestStatus.Withdrawn;
            if (user.RequestStatus == AccessRequestStatus.Withdrawn)
            {
                lblWithDrawReason.Text = employeeEventTrigger.Comments;
            }
            LabelTaskName.Text = employeeEventTrigger.EventType.GetDescription();
            if (task != null)
            {
                lblTaskStatus.Text = task?.Action.GetDescription();
                lblTaskStatus.Visible = true;
                divTaskStatus.Visible = true;
                ShowApprovedBy(task);
            }
            else
            {
                divTaskStatus.Visible = false;
                lblTaskStatus.Visible = false;
            }

            //Added by JH for https://dev.azure.com/DeepIdentityPTE/STS/_workitems/edit/8454/ or https://deepidentityhelp.freshdesk.com/a/tickets/1502
            //LabelRequestedDate.Text = user.CreatedDate.HasValue ? user.CreatedDate.Value.ToString("dd-MM-yyyy") : string.Empty;
            LabelRequestedDate.Text = string.Empty;
            if (employeeEventTrigger.CreatedDate.ToString().Length>1)
            {
                LabelRequestedDate.Text = employeeEventTrigger.CreatedDate.ToString("dd-MM-yyyy");
            }

            //requesttimeLabel.Text = user.CreatedDate.HasValue ? user.CreatedDate.Value.ToShortTimeString() : string.Empty;
            requesttimeLabel.Text = string.Empty;
            if (employeeEventTrigger.CreatedDate.ToString().Length > 1)
            {
                requesttimeLabel.Text = employeeEventTrigger.CreatedDate.ToShortTimeString();
            }

            UserInfoUC.User = user;

            Dto.User oldUser = null;

            if (!string.IsNullOrWhiteSpace(employeeEventTrigger.OldUserDetails))
            {
                oldUser = UtilityMethods.Deserialize<Dto.User>(employeeEventTrigger.OldUserDetails);
            }

            if (oldUser == null)
                oldUser = user;

            reqDepartment.Text = oldUser.OrgDepartment;
            reqDesignation.Text = oldUser.PositionName;
            reqSupName.Text = oldUser.Supervisor; 
            reqCompany.Text = oldUser.OrgCompany;
           // reqDivision.Text = oldUser.OrgDivision;
            reqSection.Text = oldUser.OrgSection;
            reqUnit.Text = oldUser.OrgUnit;
            reqBranch.Text = oldUser.OrgBranch;


            if (string.IsNullOrWhiteSpace(user.NCompany))
            {
                lblNonEmployeeCompany.Text = user.NCompany;
                nonEmployeeCompanyDiv.Visible = true;
            }

            CurrentAccessUC.EntityId = user.Id;

            Dictionary<string, string> ParamRequestUserId = new Dictionary<string, string>();
            ParamRequestUserId.Add("name", employeeEventTrigger.CreatedBy);
            var requester = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                (DomainApiAction.User.GetByUserName, ParamRequestUserId);

            if (requester != null)
            {
                LabelRequestByName.Text = requester.DisplayName;
            }
            else
            {
                LabelRequestByName.Text = String.Empty;
            }

            string status = employeeEventTrigger.RequestStatus == Infrastructure.Enums.AccessRequestStatus.Granted ? Infrastructure.Enums.AccessRequestProgress.Completed.ToString() : employeeEventTrigger.RequestStatus.GetDescription();
            LabelStatus.Text = status;
            if (task == null || employeeEventTrigger.RequestStatus == Infrastructure.Enums.AccessRequestStatus.Granted || employeeEventTrigger.RequestStatus == Infrastructure.Enums.AccessRequestStatus.Rejected)
                return;

            LabelTaskID.Text = task?.Id.ToString();
            string approver = string.Empty;
            if (task.ActionUserId != null)
            {
                Dictionary<string, string> pactionUser = new Dictionary<string, string>();
                pactionUser.Add("id", Convert.ToString(task.ActionUserId.Value));
                Dto.User actionUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, pactionUser);

                approver = string.Format("{0}", (task.IsGroupTask) ?
                   actionUser.DisplayName : actionUser.DisplayName + " - " + task.Approver);
            }
            else if (task.IsGroupTask)
            {
                approver = string.Format("{0}", Convert.ToString(task.GroupName));

                if (task.AssignedTo.HasValue)
                {
                    Dictionary<string, string> pactionUser = new Dictionary<string, string>();
                    pactionUser.Add("id", Convert.ToString(task.AssignedTo.Value));
                    Dto.User actionUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                        (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, pactionUser);

                    approver  = string.Format("{0} - [{1}]", approver, actionUser.DisplayName);
                }
            }
            if (!string.IsNullOrWhiteSpace(approver))
            {
                status = Infrastructure.Enums.AccessRequestProgress.WaitingforApproval.GetDescription();
                LabelStatus.Text = !string.IsNullOrWhiteSpace(approver)? status + " ("+ approver+")" : status;
            }
            if (employeeEventTrigger != null && employeeEventTrigger.RequestStatus == AccessRequestStatus.Withdrawn)
            {
                status = AccessRequestStatus.Withdrawn.GetDescription();
                LabelStatus.Text = status;
            }
        }

        private void ShowApprovedBy(Dto.Task task)
        {
            try
            {
                if (task != null && task.Action == StatusType.Approved && task.ActionUserId.HasValue)
                {
                    var usr = DomainApiClient<Dto.User>.GetById(task.ActionUserId.Value);
                    if (usr != null)
                    {
                        divTaskApprovedBy.Visible = true;
                        lblTaskApprovedBy.Text = usr.DisplayName;
                    }
                }
            }
            catch
            {
            }
        }

        private void SetForm(Dto.AdHocAccessRequest AdhocAccessRequest)
        {
            string parentRequestType = string.Empty;
            if (AdhocAccessRequest != null)
            {
                Dictionary<string, string> adhocRequestId = new Dictionary<string, string>();
                adhocRequestId.Add("adhocAccessRequestId", Convert.ToString(AdhocAccessRequest.Id));

                Dto.AccessRequest accessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>
                    (DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, adhocRequestId);

                if (accessRequest != null && accessRequest.ParentRequestId.HasValue)
                {
                    Dictionary<string, string> requestId = new Dictionary<string, string>();
                    requestId.Add("accessrequestId", Convert.ToString(accessRequest.ParentRequestId.Value));

                    Dto.AccessRequest parentAccessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>
                        (DomainApiAction.Provisioning.AccessRequest.GetAccessRequestById, requestId);

                    if (parentAccessRequest != null && parentAccessRequest.AdHocAccessRequest != null &&
                        parentAccessRequest.AdHocAccessRequest.AdhocActionType != AdhocAccessRequest.AdhocActionType &&
                        (parentAccessRequest.AdHocAccessRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.NewAccount &&
                        AdhocAccessRequest.AdhocActionType == Infrastructure.Enums.AdHocActionType.AmendmentAdd))
                    {
                        parentRequestType = " " + UtilityMethods.GetEnumAttribute<Description>(parentAccessRequest.AdHocAccessRequest.AdhocActionType);
                    }
                }

                string hstName = string.Empty;
                hstName = AdhocAccessRequest.EndPoint?.DisplayName;

                string taskName = string.Format("{0}{1} - {2}", hstName, parentRequestType, UtilityMethods.GetEnumAttribute<Description>(AdhocAccessRequest.AdhocActionType));
                LabelTaskName.Text = taskName;
                divTaskID.Visible = false;

                LabelRequestedDate.Text = AdhocAccessRequest.CreatedDate.ToString("dd-MM-yyyy");
                requesttimeLabel.Text = AdhocAccessRequest.CreatedDate.ToShortTimeString();

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", Convert.ToString(AdhocAccessRequest.RequesteeUserId));

                var user = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

                UserInfoUC.User = user;
                reqDepartment.Text =  accessRequest.OrgDepartment;
                reqDesignation.Text = user.PositionName;
                reqSupName.Text = accessRequest.RESPONSETXID;
                reqCompany.Text = accessRequest.OrgCompany;
                //reqDivision.Text = accessRequest.OrgDivision;
                reqSection.Text = accessRequest.OrgSection;
                reqUnit.Text = accessRequest.OrgUnit;
                reqBranch.Text = accessRequest.OrgBranch;

                CurrentAccessUC.EntityId = AdhocAccessRequest.RequesteeUserId;

                Dictionary<string, string> ParamRequestUserId = new Dictionary<string, string>();
                ParamRequestUserId.Add("id", Convert.ToString(AdhocAccessRequest.RequesterUserId));

                Dto.User userRequester = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, ParamRequestUserId);
                if (userRequester != null)
                {
                    LabelRequestByName.Text = accessRequest.RequestSource == AccessRequestSource.AdhocRequest ? userRequester.DisplayName : "System"; 
                }
                else
                {
                    LabelRequestByName.Text = String.Empty;
                }
                if (accessRequest != null && accessRequest.EmployeeEventTriggerId.HasValue)
                {
                    LabelRequestByName.Text = "System";
                }

                string status = UtilityMethods.GetEnumAttribute<Description>(accessRequest.ProgressStatus);
                string approver = string.Empty;

                Dictionary<string, string> parameterTask = new Dictionary<string, string>();
                parameterTask.Add("accessRequestId", Convert.ToString(accessRequest.Id));

                //Suresh - Since GetPendingTaskByAccessRequestId is returnig list, I have change the logic to list.. Need to verify whether 
                //GetPendingTaskByAccessRequestId will return more than one value and update logic accordingly
                List<Dto.Task> task = DomainApiClient<Dto.Task>.GetByAction<List<Dto.Task>>
                    (DomainApiAction.Tasks.Task.GetPendingTaskByAccessRequestId, parameterTask);
                List<string> approvers = new List<string>();
                if (task != null && task.Count > 0)
                {
                    
                    divTaskID.Visible = true;
                    foreach (var eachTask in task)
                    {
                        LabelTaskID.Text = task[0].Id.ToString();
                        if (eachTask.ActionUserId != null)
                        {
                            Dictionary<string, string> pactionUser = new Dictionary<string, string>();
                            pactionUser.Add("id", Convert.ToString(eachTask.ActionUserId.Value));
                            Dto.User actionUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                                (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, pactionUser);

                            approvers.Add(//string.Format("{0}", (eachTask.IsGroupTask) ?
                               actionUser.DisplayName //: actionUser.DisplayName + " - " + eachTask.Approver)
                               );
                        }
                        else if (eachTask.IsGroupTask)
                        {
                           var approverGroup = string.Format("{0}", Convert.ToString(eachTask.GroupName));

                            if (eachTask.AssignedTo.HasValue)
                            {
                                Dictionary<string, string> pactionUser = new Dictionary<string, string>();
                                pactionUser.Add("id", Convert.ToString(eachTask.AssignedTo.Value));
                                Dto.User actionUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, pactionUser);

                                approvers.Add(string.Format("{0} - [{1}]", approverGroup, actionUser.DisplayName));
                            }
                            else
                                approvers.Add(approverGroup);
                        }
                    }
                }                  
                 
                LabelStatus.Text = ( approvers!=null && approvers.Count>0 ) ? status +" ("+ string.Join( ",", approvers) +")" : status;
                var id =Convert.ToInt64(Request.QueryString["TaskId"]);
                var taskInfo = DomainApiClient<Dto.Task>.GetById(id);
                if(taskInfo != null)
                {
                    ShowApprovedBy(taskInfo);
                    divTaskID.Visible = true;
                    LabelTaskID.Text = $"{taskInfo.Id}";
                    divTaskStatus.Visible = true;
                    lblTaskStatus.Text = taskInfo.Action.GetDescription();
                }
            }
        }
        private void SetHRMSAccessRequestForm(Dto.AccessRequest hrmsAccessRequest)
        {
            string parentRequestType = string.Empty;
            if (hrmsAccessRequest != null)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("adhocAccessRequestId", Convert.ToString(hrmsAccessRequest.Id));

                Dto.AccessRequest accessRequest = DomainApiClient<Dto.AccessRequest>.GetById(hrmsAccessRequest.Id,true);

                string hstName = string.Empty;
                var ep = DomainApiClient<Dto.EndPoint>.GetById(accessRequest.EndpointSystemId.Value);
                hstName = ep?.DisplayName;
                string taskName = string.Format("{0} - {1}", hstName, UtilityMethods.GetEnumAttribute<Description>(accessRequest.RequestSource));
                LabelTaskName.Text = taskName;
                divTaskID.Visible = false;

                LabelRequestedDate.Text = hrmsAccessRequest.CreatedDate.ToString("dd-MM-yyyy");
                requesttimeLabel.Text = hrmsAccessRequest.CreatedDate.ToShortTimeString();

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", Convert.ToString(hrmsAccessRequest.UserId));

                var user = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

                UserInfoUC.User = user;
                reqDepartment.Text = accessRequest.OrgDepartment;
                reqDesignation.Text = user.PositionName;
                reqSupName.Text = accessRequest.RESPONSETXID;
                reqCompany.Text = accessRequest.OrgCompany;
                //reqDivision.Text = accessRequest.OrgDivision;
                reqSection.Text = accessRequest.OrgSection;
                reqUnit.Text = accessRequest.OrgUnit;
                reqBranch.Text = accessRequest.OrgBranch;

                CurrentAccessUC.EntityId = hrmsAccessRequest.UserId;
                if (hrmsAccessRequest.RequesterId.HasValue)
                {

                    Dictionary<string, string> ParamRequestUserId = new Dictionary<string, string>();
                    ParamRequestUserId.Add("id", Convert.ToString(hrmsAccessRequest.RequesterId));

                    Dto.User userRequester = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                        (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, ParamRequestUserId);
                    if (userRequester != null)
                    {
                        LabelRequestByName.Text = accessRequest.RequestSource == AccessRequestSource.AdhocRequest ? userRequester.DisplayName : "System";
                    }
                    else
                    {
                        LabelRequestByName.Text = String.Empty;
                    }
                }
                string status = UtilityMethods.GetEnumAttribute<Description>(accessRequest.ProgressStatus);
                string approver = string.Empty;

                Dictionary<string, string> parameterTask = new Dictionary<string, string>();
                parameterTask.Add("accessRequestId", Convert.ToString(accessRequest.Id));
                
                List<string> approvers = new List<string>();
                LabelStatus.Text = (approvers != null && approvers.Count > 0) ? status + " (" + string.Join(",", approvers) + ")" : status;
            }
        }
        #endregion

        #region Events
        protected void linkbuttonViewCurrentAccess_Click(object sender, EventArgs e)
        {
            CurrentAccess.Width = GetUnit(80);
            CurrentAccessUC.HideAccountDetail = true;
            CurrentAccessUC.Rebind();
            ShowWindow();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            CloseWindow();
        }
        #endregion
    }
}