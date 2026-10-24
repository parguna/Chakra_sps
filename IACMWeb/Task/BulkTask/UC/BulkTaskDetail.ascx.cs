using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.CustomObjects;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.AccessRequest.Common;
using DI.Chakra.Web.UsrControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Task.BulkTask.UC
{
    public partial class BulkTaskDetail : UIBaseUC
    {

        #region Members
        SessionStatusImpl _sessionStatus = new SessionStatusImpl();
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_ISFROMACCESSREQUEST = "V_ISFROMACCESSREQUEST";
        private const string V_REQUEST = "V_REQUEST";
        #endregion
        private long TaskId
        {
            get
            {
                if (ViewState["TaskId"] != null)
                {
                    return Convert.ToInt64(ViewState["TaskId"]);
                }
                return 0;
            }
            set
            {
                ViewState["TaskId"] = value;
            }
        }

        private int RequstorUserID
        {
            get
            {
                if (ViewState["RequestorUserID"] != null)
                {
                    return Convert.ToInt32(ViewState["RequestorUserID"]);
                }
                return 0;
            }
            set
            {
                ViewState["RequestorUserID"] = value;
            }
        }

        private long AdhocAccessRequestId
        {
            get
            {
                if (ViewState["AdhocAccessRequestId"] != null)
                {
                    return Convert.ToInt64(ViewState["AdhocAccessRequestId"]);
                }
                return 0;
            }
            set
            {
                ViewState["AdhocAccessRequestId"] = value;
            }
        }

        private Dictionary<int, string> SelectedRoles
        {
            get
            {
                if (ViewState["SelectedRoles"] != null)
                {
                    return (Dictionary<int, string>)ViewState["SelectedRoles"];
                }
                return new Dictionary<int, string>();
            }
            set
            {
                ViewState["SelectedRoles"] = value;
            }
        }

        private Dictionary<string, string> PortalGroups
        {
            get
            {
                if (ViewState["PortalGroups"] != null)
                {
                    return (Dictionary<string, string>)ViewState["PortalGroups"];
                }
                return new Dictionary<string, string>();
            }
            set
            {
                ViewState["PortalGroups"] = value;
            }
        }

        private Dictionary<string, string> PortalRoles
        {
            get
            {
                if (ViewState["PortalRoles"] != null)
                {
                    return (Dictionary<string, string>)ViewState["PortalRoles"];
                }
                return new Dictionary<string, string>();
            }
            set
            {
                ViewState["PortalRoles"] = value;
            }
        }

        public IsAdminMode IsAdminMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return IsAdminMode.No;
                return (IsAdminMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }

        public bool isFromAccessRequest
        {
            get
            {
                if (ViewState[V_ISFROMACCESSREQUEST] == null)
                    return false;
                return (bool)ViewState[V_ISFROMACCESSREQUEST];
            }
            set
            {
                ViewState[V_ISFROMACCESSREQUEST] = value;
            }
        }

        public Dto.Task Task
        {
            get
            {
                if (ViewState["Dto.Task"] != null)
                {
                    return (Dto.Task)ViewState["Dto.Task"];
                }
                return new Dto.Task();
            }
            set
            {
                ViewState["Dto.Task"] = value;
            }
        }

        public WorkflowConfiguration workFlowConfig
        {
            get
            {
                if (ViewState["WorkflowConfiguration"] != null)
                {
                    return (WorkflowConfiguration)ViewState["WorkflowConfiguration"];
                }
                return new WorkflowConfiguration();
            }
            set
            {
                ViewState["WorkflowConfiguration"] = value;
            }
        }

        public List<KeyValuePair<string, string>> EntitlementRequest
        {
            get
            {
                if (ViewState[V_REQUEST] != null)
                {
                    return (List<KeyValuePair<string, string>>)ViewState[V_REQUEST];
                }
                return new List<KeyValuePair<string, string>>();
            }
            set
            {
                ViewState[V_REQUEST] = value;
            }
        }

        public EndPoint SelectedEndpoint
        {
            get { return Task.BulkRequest.EndPoint; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ConfirmMessage1.ConfirmYesClick += ConfirmMessage1_ConfirmYesClick;
            if (IsPostBack)
            {
                foreach (RepeaterItem item in rptInhUserInfo.Items)
                {
                    TextBox password = (TextBox)item.FindControl("txtPassword");
                    password.Attributes["value"] = password.Text;
                }
            }

            if (!IsPostBack && CanLoad)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                if (this.ContextObjectId != null)
                    parameter.Add("Id", this.ContextObjectId.Value.ToString());
                else
                    parameter.Add("Id", Request.QueryString["Id"].ToString());

                Dto.Task task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.GetBulkRequestTask, parameter);
                if (task.BulkRequest == null)
                    return;
                this.Task = task;

                MasterPage mstr = this.Parent.Page.Master as MasterPage;
                (mstr.FindControl("lblPageTitle") as Label).Text = "Task Details";
                if (task.IsGroupTask)
                {
                    (mstr.FindControl("lblPageTitle") as Label).Text = "Group Task Details";
                }

                this.TaskId = task.Id;
                BulkGenericRequestDetails.BulkRequestID = Convert.ToInt32(task.BulkRequestId);
                BulkGenericRequestDetails.EntitlementDetailType = task.BulkRequest.ApplicationType;
                parameter.Clear();
                parameter.Add("id", this.Task.WorkflowConfigurationId.ToString());
                var workFlowCOnfigurationDetails = DomainApiClient<WorkflowConfiguration>.GetByAction<WorkflowConfiguration>
                    (DomainApiAction.Workflow.WorkflowConfiguration.GetWorkflowConfigurationDetailById, parameter);
                workFlowConfig = workFlowCOnfigurationDetails;


                long sessionUser = new SessionStatusImpl().UserId;
                Dictionary<string, string> ssnUser = new Dictionary<string, string>();
                ssnUser.Add("actionUserId", sessionUser.ToString());
                List<long> userGroups = DomainApiClient<User>.GetByAction<List<long>>(DomainApiAction.User.GetGroupIds, ssnUser);

                InitializeForm(Task);
                if (Session["RedirectFrom"] != null && Session["RedirectFrom"].ToString() == "TaskDetails")
                {
                    InitTaskDetails();
                }
                hdnActionType.Value = ((int)task.BulkRequest.AdhocActionType).ToString();
                Session["endpointSystemId"] = SelectedEndpoint.Id;
                hdnEndpointSystemId.Value = SelectedEndpoint.Id.ToString();
                if (Task.BulkRequest != null && task.BulkRequest.AdhocActionType != AdHocActionType.DeleteAccount && task.IsActionOnlyRequiredTask && task.BulkRequest.ApplicationType == ApplicationType.InhouseSystems)
                {
                    divEntitlements.Visible = true;
                    RebindPermissionGrid();
                    //BindFunctions();
                    if (Task.IsCompleted)
                    {
                        //AddPermissionButton.Enabled = false;
                        RequestGrid.Enabled = false;
                        RolesAutoCompleteBox.Enabled = false;
                        //ddlFunctions.Enabled = false;
                    }
                }

            }
        }

        public bool CanLoad
        {
            get
            {
                if (ViewState["BulkTaskDetail_CanLoad"] == null)
                    return true;
                return (bool)ViewState["BulkTaskDetail_CanLoad"];

            }
            set
            { ViewState["BulkTaskDetail_CanLoad"] = value; }
        }

        User GetUser(long Id)
        {
            return DomainApiClient<User>.GetById(Id);
        }

        private void InitializeForm(Dto.Task task)
        {
            if (task != null && task.BulkRequest != null)
            {
                hdnActionType.Value = ((int)task.BulkRequest.AdhocActionType).ToString();
                hdnEndpointSystemId.Value = task.BulkRequest.EndpointSystemId.ToString();
                long sessionUser = new SessionStatusImpl().UserId;
                Dictionary<string, string> ssnUser = new Dictionary<string, string>();
                ssnUser.Add("actionUserId", sessionUser.ToString());
                List<long> userGroups = DomainApiClient<Dto.User>.GetByAction<List<long>>(DomainApiAction.User.GetGroupIds, ssnUser);

                if (this.Task.Action != StatusType.Pending)
                    this.MasterPage.DisableImageButton = true;
                else
                {
                    if (task.AssignedTo == sessionUser)
                        this.MasterPage.DisableImageButton = false;
                    else if ((userGroups.Contains((int)(this.Task.ActionGroupId.HasValue ? this.Task.ActionGroupId.Value : 0)) && this.Task.IsGroupTask && task.AssignedTo == null))
                        this.MasterPage.DisableImageButton = false;
                    else if ((task.ActionUserId == sessionUser && !task.IsGroupTask) || ((task.IsGroupTask && task.AssignedTo == sessionUser)))
                        this.MasterPage.DisableImageButton = false;

                    //if ((userGroups.Contains((int)(this.Task.ActionGroupId.HasValue ? this.Task.ActionGroupId.Value : 0)) && (this.Task.IsGroupTask || (task.AssignedTo == null || (task.AssignedTo != null
                    //    && task.AssignedTo != sessionUser)))))
                    //    this.MasterPage.DisableImageButton = true;
                    //else if ((task.ActionUserId == sessionUser && !task.IsGroupTask) || ((task.IsGroupTask && task.AssignedTo == sessionUser)))
                    //    this.MasterPage.DisableImageButton = false;
                }

                if (!this.MasterPage.DisableImageButton)
                {
                    btnApprove.Visible = btnReject.Visible = IsAdminMode == IsAdminMode.Yes ? false : (task.BulkRequest.ProgressStatus == AccessRequestProgress.WaitingforApproval
                     ) ? true : false;
                    //btnSendNotification.Enabled = UCSendNotification.IsSendNotificationRequired();
                    //btnSendNotification.Visible = false;
                    if (btnApprove.Visible)
                    {
                        if (Task.IsGroupTask && (Task.AssignedTo != null && Task.AssignedTo != _sessionStatus.User.Id))
                        {
                            btnApprove.Visible = btnSubmit.Visible = btnReject.Visible = false;
                            // btnSendNotification.Visible = false;
                            this.MasterPage.FindControl("AccessPanel").Visible = false;
                        }
                        else
                        {
                            this.txtComments.Enabled = true;
                        }
                    }
                    else
                    {
                        //this.txtComments.Enabled = false;
                        this.MasterPage.FindControl("AccessPanel").Visible = false;
                    }
                }
                else
                {
                    this.txtComments.Enabled = this.btnApprove.Visible = btnReject.Visible = btnAttach.Visible = false;
                    // btnSendNotification.Visible = false;
                }

                string hstName = string.Empty;
                if (task.BulkRequest != null && task.BulkRequest.EndPoint != null)
                    hstName = task.BulkRequest.EndPoint.DisplayName;
                string requestName = hstName + " - " + UtilityMethods.GetEnumAttribute<Description>(task.BulkRequest.AdhocActionType);
                lblRequestName.Text = requestName;
                lblRequestedDate.Text = task.BulkRequest.CreatedDate.ToString();
                lblRequestFor.Text = task.BulkRequest.IsEmployee ? "Employee" : "Non-Employee";
                if (task.BulkRequest.RequestSource == AccessRequestSource.AdhocRequest)
                {
                    lblRequesteeName.Text = GetUser(task.BulkRequest.RequestedBy).DisplayName;
                }
                else
                {
                    lblRequesteeName.Text = "System";
                }

                //  lblSubmitBefore.Text = SubmitBefore;
                //lblStatus.Text = UtilityMethods.GetEnumAttribute<Description>(currentAccessRequest.RequestStatus);
                //if (task.IsActive && !task.IsCompleted)
                //{
                //    lblStatus.Text = "Pending - " + task.Approver;
                //}
                //else if (task.IsCompleted)
                //{
                //    lblStatus.Text = "Completed";
                //}
                //else
                //{
                //    lblStatus.Text = "Auto Completed";
                //}

                lblDelegationComments.Text = !string.IsNullOrEmpty(task.DelegationComments) ? task.DelegationComments.Replace("\n", "<br>") : string.Empty;
                DelegationCommentstr.Visible = !string.IsNullOrEmpty(task.DelegationComments);
                DelegatedBytr.Visible = task.IsDelegated;
                DelegatedFromtr.Visible = task.IsDelegated;
                if (task.IsDelegated && task.DelegatedFrom != null)
                {
                    User _user = GetUser(task.DelegatedFrom.Value);

                    if (_user != null)
                        lblDelegatedFrom.Text = _user.DisplayName;

                    _user = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetByUserName, new Dictionary<string, string> { { "name", task.DelegatedBy } });
                    if (_user != null)
                        lblDelegatedBy.Text = _user.DisplayName;
                    else
                        lblDelegatedBy.Text = task.DelegatedBy;
                }



                this.ExceptionRemarksValueLabel.Text = task.Remarks;
                this.ExceptionRemarkstr.Visible = string.IsNullOrEmpty(task.Remarks) ? false : true;
                if (task.BulkRequest != null && (task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd || task.BulkRequest.AdhocActionType == AdHocActionType.NewAccount)
                    && !string.IsNullOrWhiteSpace(task.Remarks) && task.Remarks.Contains("<b>System Remarks</b>"))
                { 
                    var delimeter = new string[] { "<b>System Remarks</b>" };
                    this.ExceptionRemarksValueLabel.Text = task.Remarks.Split(delimeter,StringSplitOptions.None)[0];
                    this.ExceptionRemarkstr.Visible = this.ExceptionRemarkstr.Visible = string.IsNullOrWhiteSpace(this.ExceptionRemarksValueLabel.Text) ? false : true;
                }
                SetApproverForm();
                ViewAttachmentUC.RequestId = this.Task.BulkRequestId;
                ViewAttachmentUC.AccessRequestSource = DI.Chakra.Infrastructure.Constants.BulkRequest;
                lnkViewAttachment.Text = "View Attachments (" + ViewAttachmentUC.getCount() + ")";
                btnSubmit.Visible = false;
                MSSLAdditionalInfoValueLabel.Text = task.BulkRequest.Remarks;
                AdditionalInformationtr.Visible = string.IsNullOrEmpty(MSSLAdditionalInfoValueLabel.Text) ? false : true;
                //ConflictsRemarkLabel.Text = currentAccessRequest.AdditionalInfo;
                //ConflictsRemarkImage.Visible = ConflictsRemarkPanel.Visible = ConflictsRemarkLabelText.Visible = ConflictsRemarkLabel.Visible = string.IsNullOrEmpty(ConflictsRemarkLabel.Text) ? false : true;
                ConflictsRemarkImage.Visible = false;
                ConflictsRemarktr.Visible = false;
                if (task.SubTask != null)
                {
                    RoleNameValueLabel.Text = task.SubTask.RoleName;
                    TCodeValueLabel.Text = task.SubTask.TCode;
                    RemarksValueLabel.Text = task.SubTask.Remarks;
                    RoleNameValuetr.Visible = RoleNameValueLabel.Visible = string.IsNullOrEmpty(RoleNameValueLabel.Text) ? false : true;
                    TCodeValueLabeltr.Visible = TCodeValueLabel.Visible = string.IsNullOrEmpty(TCodeValueLabel.Text) ? false : true;
                    RemarksValuetr.Visible = RemarksValueLabel.Visible = string.IsNullOrEmpty(RemarksValueLabel.Text) ? false : true;
                }
                if (!task.IsCompleted)
                {
                    if (task.IsActionOnlyRequiredTask && (task.BulkRequest.AdhocActionType == AdHocActionType.NewAccount 
                        || task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd) && 
                        (task.BulkRequest.ApplicationType == ApplicationType.MSSL_ISU || task.BulkRequest.ApplicationType == ApplicationType.MSSL_CRM || task.BulkRequest.ApplicationType == ApplicationType.EBS_ISU || 
                        task.BulkRequest.ApplicationType == ApplicationType.EBS_CRM || task.BulkRequest.ApplicationType == ApplicationType.EBS || task.BulkRequest.ApplicationType == ApplicationType.MSSL))
                    {
                        {
                            var adhocAcessRequest = task.BulkRequest;
                            if (task.BulkRequest.ApplicationType == ApplicationType.MSSL_ISU || task.BulkRequest.ApplicationType == ApplicationType.MSSL_CRM)
                            {
                                var data = (AdHocMSSLRequest)adhocAcessRequest.GetEntitlementDetail();

                                if (data != null && task.BulkRequest.AdhocActionType == AdHocActionType.NewAccount && string.IsNullOrWhiteSpace(data.PositionPrefix))
                                {
                                    MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                    PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                    MainPanel.Visible = task.IsActionOnlyRequiredTask;
                                }
                                else if (data != null && task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd && data.IsReallocate && string.IsNullOrWhiteSpace(data.PositionPrefix))
                                {
                                    MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                    PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                    MainPanel.Visible = task.IsActionOnlyRequiredTask;
                                }
                            }
                            else if (task.BulkRequest.ApplicationType == ApplicationType.EBS_ISU || task.BulkRequest.ApplicationType == ApplicationType.EBS_CRM)
                            {
                                var data = (AdHocEBSRequest)adhocAcessRequest.GetEntitlementDetail();

                                if (data != null && task.BulkRequest.AdhocActionType == AdHocActionType.NewAccount && string.IsNullOrEmpty(data.PositionPrefix))
                                {
                                    MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                    PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                    MainPanel.Visible = task.IsActionOnlyRequiredTask;
                                }
                                else if (data != null && task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd && data.IsReallocate && string.IsNullOrWhiteSpace(data.PositionPrefix))
                                {
                                    MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                    PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                    MainPanel.Visible = task.IsActionOnlyRequiredTask;
                                }
                            }
                            else if (task.BulkRequest.ApplicationType == ApplicationType.EBS || task.BulkRequest.ApplicationType == ApplicationType.MSSL)
                            {
                                var data = (AdHocSAPHANARequest)adhocAcessRequest.GetEntitlementDetail();

                                if (data != null && task.BulkRequest.AdhocActionType == AdHocActionType.NewAccount && string.IsNullOrEmpty(data.PositionPrefix))
                                {
                                    MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                    PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                    MainPanel.Visible = task.IsActionOnlyRequiredTask;
                                }
                                else if (data != null && task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd && data.IsReallocate && string.IsNullOrWhiteSpace(data.PositionPrefix))
                                {
                                    MainPanelHeaderSpan.InnerText = "Prefix Submission";
                                    PostionPrefixPanel.Visible = task.IsActionOnlyRequiredTask;
                                    MainPanel.Visible = task.IsActionOnlyRequiredTask;
                                }
                            }

                        }
                    }

                    else if (task.BulkRequest.ApplicationType == ApplicationType.HRMS_FLINS)
                    {
                        if (task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                        {
                            AdHocSAPHRFLINSRequest flinsRequest = (AdHocSAPHRFLINSRequest)task.BulkRequest.GetEntitlementDetail();
                            if (flinsRequest != null)
                            {
                                Dictionary<string, string> ListofEntilementWithCurrent = new Dictionary<string, string>();
                                foreach (AdHocSAPHRFLINSRequestDetail detail in flinsRequest.AdHocSAPHRFLINSRequestDetails)
                                {
                                    detail.Entitlement.Split(',').ToList().ForEach(lq =>
                                    {
                                        if (!ListofEntilementWithCurrent.ContainsKey(lq))
                                        {
                                            ListofEntilementWithCurrent.Add(lq, "Current Queue");
                                        }
                                    });

                                }
                                string Remark = "";
                                //SAPFacade sapFacade = new SAPFacade();
                                //task.BulkRequest.BulkRequestDetails.ForEach(lq =>
                                //{
                                //    var EndpointUser = sapFacade.GetSAPUserInfoByDIUserId(lq.UserId).FirstOrDefault(endpointuser => endpointuser.EndPointSystemId == task.BulkRequest.EndpointSystemId);
                                //    Remark = BulkGenericRequestDetails.ConflictBusinessRoles(ListofEntilementWithCurrent, EndpointUser.UserId, lq.UserId, lq.Name);
                                //    Remark = string.IsNullOrEmpty(Remark) ? Remark : Remark + "<br>" + Remark;
                                //});
                                //BulkGenericRequestDetails.ConflictRemark = Remark;
                            }
                        }
                    }
                    if (task.BulkRequest.ApplicationType == ApplicationType.VPN)
                    {
                        if (task.IsGroupTask && task.ActionGroupId.HasValue)
                        {
                            var groupId = task.ActionGroupId.Value;
                            BulkGenericRequestDetails.GroupID = groupId;
                            BulkGenericRequestDetails.EntitlementDetailType = task.BulkRequest.ApplicationType;
                            BulkGenericRequestDetails.IsActionOnlyRequiredTask = task.IsActionOnlyRequiredTask;
                        }

                    }
                    else if (task.BulkRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                    {
                        BulkGenericRequestDetails.BindADGroupInfo(task);
                    }


                    #region SAPEP

                    GroupEntilementtr.Visible = false;
                    RoleEntilementtr.Visible = false;
                    SAPEPEntitlementPanel.Visible = false;
                    btnGrpAdd.Visible = false;
                    btnroleAdd.Visible = false;


                    if (task.IsActionOnlyRequiredTask && (task.BulkRequest.ApplicationType == ApplicationType.SAP_EP || task.BulkRequest.ApplicationType == ApplicationType.CE))
                    {
                        MainPanel.Visible = true;
                        int WintelAdminGroupID = MasterConfigHelper.WBSAdmin;
                        bool IsWintelAdmin = DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.CheckUserGroup,
                                                        new Dictionary<string, string> { { "userId",(task.ActionUserId.HasValue) ? task.ActionUserId.ToString() : ( (task.AssignedTo.HasValue)? task.AssignedTo.ToString() : new SessionStatusImpl().User.Id.ToString() ) },
                                                                                                { "GroupID", WintelAdminGroupID.ToString() }
                                                        });



                        if (IsWintelAdmin)
                        {
                            MainPanelHeaderSpan.InnerText = "Entitlement Submission";
                            SAPEPEntitlementPanel.Visible = task.IsActionOnlyRequiredTask;
                            GroupEntilementtr.Visible = task.IsActionOnlyRequiredTask;
                            RoleEntilementtr.Visible = task.IsActionOnlyRequiredTask;
                            var adhocAcessRequest = task.BulkRequest;
                            var sapepDetails = (AdhocSAPEPRequest)adhocAcessRequest.GetEntitlementDetail();

                            btnGrpAdd.Visible = true;

                            btnroleAdd.Visible = true;

                            //if (task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                            //{

                            //    Session["PortalEndpointID"] = task.BulkRequest.EndpointSystemId;
                            //    GroupsAutoCompleteBox.Visible = false;
                            //    RolesAutoCompleteBox.Visible = false;
                            //    ExistingGroupAutoCompleteBox.Visible = true;
                            //    ExistingRoleAutoCompleteBox.Visible = true;
                            //    btnGrpAdd.Visible = false;
                            //    btnroleAdd.Visible = false;
                            //    trGroupGrid.Visible = false;
                            //    trRoleGrid.Visible = false;

                            //    Dictionary<string, string> param = new Dictionary<string, string> { { "EndpointUserID", sapepDetails.EndpointUserId.ToString() } };
                            //    Dictionary<string, string> dataSourceGroup = DomainApiClient<Dto.SAPEPGroupInfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAPEP.SAPEPGroupInfo.GetGroupSAPEPForRemove, param);

                            //    ExistingGroupAutoCompleteBox.DataSource = dataSourceGroup;
                            //    ExistingGroupAutoCompleteBox.DataTextField = "Value";
                            //    ExistingGroupAutoCompleteBox.DataValueField = "Key";
                            //    ExistingGroupAutoCompleteBox.DataBind();

                            //    Dictionary<string, string> dataSourceRole = DomainApiClient<Dto.SAPEPRoleInfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAPEP.SAPEPRoleInfo.GetRolesSAPEPForRemove, param);

                            //    ExistingRoleAutoCompleteBox.DataSource = dataSourceRole;
                            //    ExistingRoleAutoCompleteBox.DataTextField = "Value";
                            //    ExistingRoleAutoCompleteBox.DataValueField = "Key";
                            //    ExistingRoleAutoCompleteBox.DataBind();
                            //}
                            //else 

                            if (task.BulkRequest.AdhocActionType == AdHocActionType.NewAccount || task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd || task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                            {
                                Session["PortalEndpointID"] = task.BulkRequest.EndpointSystemId;
                                hdnEndpointSystemId.Value = task.BulkRequest.EndpointSystemId.ToString();
                                hdnActionType.Value = ((int)task.BulkRequest.AdhocActionType).ToString();
                                if (task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd || task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                                {
                                    var bulkRequestDetails = DomainApiClient<Dto.BulkRequest>.GetById(task.BulkRequest.Id, true);
                                    if (bulkRequestDetails != null)
                                    {
                                        hdnEndpointUserId.Value = UtilityMethods.GetStringFromList(bulkRequestDetails.BulkRequestDetails.Select(i => i.EndpointUserId.ToString()).ToList());
                                    }
                                }
                                GroupsAutoCompleteBox.Visible = true;
                                RolesAutoCompleteBox.Visible = true;
                                ExistingGroupAutoCompleteBox.Visible = false;
                                ExistingRoleAutoCompleteBox.Visible = false;
                                btnGrpAdd.Visible = true;
                                btnroleAdd.Visible = true;
                                trGroupGrid.Visible = true;
                                trRoleGrid.Visible = true;

                                if (!string.IsNullOrWhiteSpace(sapepDetails.Roles))
                                {
                                    Dictionary<string, string> param = new Dictionary<string, string> { { "EndpointSystemId", task.BulkRequest.EndpointSystemId.ToString() } };

                                    Dictionary<string, string> dataSourceRole = DomainApiClient<Dto.SAPEPRoleInfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAPEP.SAPEPRoleInfo.GetRoleSAPEP, param);
                                    string[] roleID = sapepDetails.Roles.Split(',');
                                    //string entiltment = "";
                                    foreach (string rolekey in roleID)
                                    {
                                        if (dataSourceRole.ContainsKey(rolekey))
                                            boundEPRoles(rolekey, dataSourceRole[rolekey]);
                                    }
                                    //RoleEntitlementLabel.Text = entiltment;
                                }
                                else { trRoleGrid.Visible = false; }

                                if (!string.IsNullOrWhiteSpace(sapepDetails.Groups))
                                {
                                    Dictionary<string, string> param = new Dictionary<string, string> { { "EndpointSystemId", task.BulkRequest.EndpointSystemId.ToString() } };

                                    Dictionary<string, string> dataSourceGroup = DomainApiClient<Dto.SAPEPGroupInfo>.GetByAction<Dictionary<string, string>>(DomainApiAction.Endpoint.SAPEP.SAPEPGroupInfo.GetGroupSAPEP, param);
                                    string[] groupIds = sapepDetails.Groups.Split(',');
                                    //string entiltment = "";
                                    foreach (string groupkey in groupIds)
                                    {
                                        if (dataSourceGroup.ContainsKey(groupkey))
                                            boundEPGroups(groupkey, dataSourceGroup[groupkey]);
                                        //string role = dataSourceRole.Where(item => item.Key == rolekey).Select(item => item.Value).FirstOrDefault();
                                        //entiltment = entiltment == "" ? role : entiltment + "," + role;
                                    }
                                }
                                else { trGroupGrid.Visible = false; }
                            }
                        }
                    }
                    #endregion

                    #region Inhouse systems
                    else if (task.IsActionOnlyRequiredTask && task.BulkRequest.ApplicationType == ApplicationType.InhouseSystems && task.BulkRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        rptInhUserInfo.DataSource = task.BulkRequest.BulkRequestDetails;
                        rptInhUserInfo.DataBind();
                        divInhouseUserDetails.Visible = true;
                    }

                    else if (task.BulkRequest.AdhocActionType != AdHocActionType.DeleteAccount && task.IsActionOnlyRequiredTask && task.BulkRequest.ApplicationType == ApplicationType.InhouseSystems)
                    {
                        divEntitlements.Visible = true;
                    }
                    #endregion

                    int GroupID = MasterConfigHelper.WintelGroupId;
                    bool IsWintelGroup = false;
                    if (task.IsGroupTask)
                    {
                        if (task.AssignedTo.HasValue && task.AssignedTo.Value > 0)
                        {
                            IsWintelGroup = DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.CheckUserGroup,
                                                                      new Dictionary<string, string> { { "userId", task.AssignedTo.ToString() },
                                                                                                { "GroupID", GroupID.ToString() }
                                                                      });
                        }

                    }
                    else
                    {
                        if (task.ActionUserId.HasValue && task.ActionUserId.Value > 0)
                        {
                            IsWintelGroup = DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.CheckUserGroup,
                                                                      new Dictionary<string, string> { { "userId", task.ActionUserId.ToString() },
                                                                                                { "GroupID", GroupID.ToString() }
                                                                      });
                        }
                    }
                    if (task.IsActionOnlyRequiredTask)
                    {
                        btnApprove.Visible = !task.IsActionOnlyRequiredTask;
                        //btnReject.Visible = !task.IsActionOnlyRequiredTask;
                        if (!task.IsCompleted)
                        {
                            btnSubmit.Visible = task.IsActionOnlyRequiredTask;
                        }
                        if ((!task.IsCompleted && task.IsActionOnlyRequiredTask) || PostionPrefixPanel.Visible || (task.IsActionOnlyRequiredTask && task.BulkRequest.ApplicationType == ApplicationType.VPN) || SAPEPEntitlementPanel.Visible)
                        {
                            btnApprove.Visible = false;
                            if(task.BulkRequest != null && task.BulkRequest.ProgressStatus == AccessRequestProgress.RequestRejected)
                            {
                                btnReject.Visible = false;
                            }
                            else
                            {
                                btnReject.Visible = true;
                            }
                            btnSubmit.Visible = true;
                        }
                        else if (task.BulkRequest.ApplicationType != ApplicationType.InhouseSystems)
                        {
                            btnApprove.Visible = true;
                            btnReject.Visible = true;
                            btnSubmit.Visible = false;
                        }
                    }
                    if (IsWintelGroup)
                    {
                        btnApprove.Text = "Submit";
                    }
                    else
                    {
                        btnApprove.Text = "Approve";
                    }
                }
            }
        }

        public List<PresentationApprovalChain> TaskApprovers
        {
            get
            {
                if (ViewState["TaskApprovers"] != null)
                {
                    return (List<PresentationApprovalChain>)ViewState["TaskApprovers"];
                }
                else
                {
                    return new List<PresentationApprovalChain>();
                }
            }
            set
            {
                ViewState["TaskApprovers"] = value;
            }
        }

        private void SetApproverForm()
        {

            var approvers = this.TaskApprovers;

            if (approvers.Count == 0)
            {
                approvers = this.GetApprovers();
                this.TaskApprovers = approvers;
            }

            grdApproverList.DataSource = approvers.ToList();
            grdApproverList.DataBind();

            //if (approvers.Any(a => a.IsActionOnlyRequired))
            //{
            //    divExecutionerGrid.Visible = true;
            //    divExecutionerHeader.Visible = true;
            //    grdExecutioner.DataSource = approvers.Where(app => app.IsActionOnlyRequired).Select(app => app).ToList();
            //    grdExecutioner.DataBind();
            //}
            //else
            //{
            //    divExecutionerGrid.Visible = false;
            //    divExecutionerHeader.Visible = false;
            //}
        }
        protected void grdApprover_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            byte state = (byte)dataItem.GetDataKeyValue("ApprovalState");

            var approver = this.TaskApprovers.FirstOrDefault(item => item.ApprovalState == state);

            e.DetailTableView.DataSource = approver.ApproverDetails;
        }


        List<Dto.Presentation.PresentationApprovalChain> GetApprovers()
        {

            if (Task != null && Task.BulkRequestId.HasValue && Task.BulkRequestId.Value != 0)
            {

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("bulkRequestId", Task.BulkRequestId.ToString());
                return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByBulkRequestId, parameter);

            }
            else
            {
                int requestId = 0;
                if (Request.QueryString["Id"] != null)
                {
                    requestId = Convert.ToInt32(Request.QueryString["Id"]);
                }
                if (requestId > 0)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("bulkRequestId", requestId.ToString());
                    return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByBulkRequestId, parameter);
                }
            }

            return new List<Dto.Presentation.PresentationApprovalChain>();
        }



        private void InitTaskDetails()
        {
            BackButton.Visible = true;
            //trbtn.Visible = false;
            ExistingGroupAutoCompleteBox.Visible = false;
            ExistingRoleAutoCompleteBox.Visible = false;
            GroupsAutoCompleteBox.Visible = false;
            //tdbtnGrpAdd.Visible = false;
            RolesAutoCompleteBox.Visible = false;
            //tdbtnroleAdd.Visible = false;
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            if (Session["RedirectFrom"] == null)
                Response.Redirect("~//Task//List.aspx");
            else if (Session["RedirectFrom"].ToString() == "MyTask")
                Response.Redirect("~//Task//List.aspx");
            else if (Session["RedirectFrom"].ToString() == "CreateDelegation")
                Response.Redirect("~//Task//List.aspx");
            else
                Response.Redirect("~//Task//GroupTasks.aspx");

        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            if (this.TaskId > 0)
            {
                var task = DomainApiClient<Dto.Task>.GetById(this.TaskId, true);
                var adhocRequestDetails = task.BulkRequest;
                if (task.BulkRequest.ApplicationType == ApplicationType.EBS || task.BulkRequest.ApplicationType == ApplicationType.MSSL ||
                    task.BulkRequest.ApplicationType == ApplicationType.MSSL_ISU || task.BulkRequest.ApplicationType == ApplicationType.MSSL_CRM || task.BulkRequest.ApplicationType == ApplicationType.EBS_ISU || 
                    task.BulkRequest.ApplicationType == ApplicationType.EBS_CRM)
                {
                    string Message = "Please enter";
                    string ControlMessage = "";
                    if (PostionPrefixPanel.Visible)
                    {
                        if (string.IsNullOrEmpty(txtPrefixValue.Text))
                        {
                            ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " prefix" : ControlMessage + ", prefix";
                        }
                        else if (txtPrefixValue.Text.Length != 4)
                            ControlMessage = string.IsNullOrWhiteSpace(ControlMessage) ? " a valid prefix" : ControlMessage + ", a valid prefix";
                    }
                    if (ControlMessage != "")
                    {
                        Message += ControlMessage.LastIndexOf(",") > 0 ? ControlMessage.Substring(0, ControlMessage.LastIndexOf(",")) + " and" + ControlMessage.Substring(ControlMessage.LastIndexOf(",") + 1) : ControlMessage;
                        MasterPage.ShowAlert(AlertType.Error, Message);
                        return;
                    }
                    else
                    {

                        var adhocAcessRequest = task.BulkRequest;
                        if (task.BulkRequest.ApplicationType == ApplicationType.MSSL_ISU || task.BulkRequest.ApplicationType == ApplicationType.MSSL_CRM)
                        {
                            var data = (AdHocMSSLRequest)adhocAcessRequest.GetEntitlementDetail();
                            if (data != null)
                            {
                                var mssldetail = (AdHocMSSLRequest)data;
                                if (PostionPrefixPanel.Visible)
                                {
                                    mssldetail.PositionPrefix = txtPrefixValue.Text;
                                }
                                adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(mssldetail);
                            }

                        }
                        else if (task.BulkRequest.ApplicationType == ApplicationType.EBS_ISU || task.BulkRequest.ApplicationType == ApplicationType.EBS_CRM)
                        {
                            var data = (AdHocEBSRequest)adhocAcessRequest.GetEntitlementDetail();
                            if (data != null)
                            {
                                var ebsdetail = (AdHocEBSRequest)data;
                                if (PostionPrefixPanel.Visible)
                                {
                                    ebsdetail.PositionPrefix = txtPrefixValue.Text;
                                }
                                adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(ebsdetail);
                            }
                        }
                        else if (task.BulkRequest.ApplicationType == ApplicationType.EBS || task.BulkRequest.ApplicationType == ApplicationType.MSSL)
                        {
                            var data = (AdHocSAPHANARequest)adhocAcessRequest.GetEntitlementDetail();
                            if (data != null)
                            {
                                var ebsdetail = (AdHocSAPHANARequest)data;
                                if (PostionPrefixPanel.Visible)
                                {
                                    ebsdetail.PositionPrefix = txtPrefixValue.Text;
                                }
                                adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(ebsdetail);
                            }
                        }
                        DomainApiClient<Dto.BulkRequest>.Save(adhocAcessRequest);
                        this.UpdateAccessRequest(AccessType.Approve);
                    }
                }
                if (task.BulkRequest.ApplicationType == ApplicationType.VPN)
                {
                    this.UpdateAccessRequest(AccessType.Approve);
                }
                if (task.BulkRequest.ApplicationType == ApplicationType.SAP_EP || task.BulkRequest.ApplicationType == ApplicationType.CE)
                {

                    var adhocAcessRequest = task.BulkRequest;
                    var sapepdetail = (AdhocSAPEPRequest)adhocAcessRequest.GetEntitlementDetail();
                    if (string.IsNullOrEmpty(sapepdetail.Groups) && string.IsNullOrEmpty(sapepdetail.Roles))
                    {
                        //if (task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                        //{
                        //    if (ExistingGroupAutoCompleteBox.CheckedItems.Count <= 0 && ExistingRoleAutoCompleteBox.CheckedItems.Count <= 0)
                        //    {
                        //        lblportalErrmsg.Text = "Please select at least one Group or Role";
                        //        lblportalErrmsg.Visible = true;
                        //    }
                        //    else
                        //    {
                        //        lblportalErrmsg.Text = "";
                        //        if (ExistingGroupAutoCompleteBox.CheckedItems.Count > 0)
                        //        {
                        //            sapepdetail.Groups = EntiltementString(ExistingGroupAutoCompleteBox, "Groups");
                        //        }
                        //        if (ExistingRoleAutoCompleteBox.CheckedItems.Count > 0)
                        //        {
                        //            sapepdetail.Roles = EntiltementString(ExistingRoleAutoCompleteBox, "Roles");
                        //        }
                        //        adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(sapepdetail);
                        //        new BulkRequestFacade().Save(adhocAcessRequest);
                        //        this.UpdateAccessRequest(AccessType.Approve);
                        //    }
                        //}
                        //else
                        {
                            if (PortalGroups.Count <= 0 && PortalRoles.Count <= 0)
                            {
                                MasterPage.ShowAlert(AlertType.Error, "Please select at least one Group or Role");
                                return;
                            }
                            else
                            {

                                if (PortalGroups.Count > 0)
                                {
                                    sapepdetail.Groups = GetPortalEntitlements("Groups");
                                }
                                if (PortalRoles.Count > 0)
                                {
                                    sapepdetail.Roles = GetPortalEntitlements("Roles");
                                }
                                adhocAcessRequest.EntitlementDetail = adhocAcessRequest.GetSerializedString(sapepdetail);
                                DomainApiClient<Dto.BulkRequest>.Save(adhocAcessRequest);
                                this.UpdateAccessRequest(AccessType.Approve);
                            }
                        }

                    }
                    else
                    {
                        this.UpdateAccessRequest(AccessType.Approve);
                    }
                }
                if (adhocRequestDetails.ApplicationType == ApplicationType.InhouseSystems)
                {
                    bool saveResult = false;
                    if (task.BulkRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        List<string> userNames = new List<string>();

                        if (adhocRequestDetails.AdhocActionType != AdHocActionType.DeleteAccount && this.EntitlementRequest.Count == 0)
                        {
                            ShowAlertMessage(AlertType.Error, "Please select entitlement.");
                            return;
                        }

                        if (rptInhUserInfo.Items.Count > 0)
                        {
                            foreach (RepeaterItem item in rptInhUserInfo.Items)
                            {
                                TextBox userName = (TextBox)item.FindControl("txtUsername");
                                TextBox password = (TextBox)item.FindControl("txtPassword");

                                if (string.IsNullOrWhiteSpace(userName.Text) || string.IsNullOrWhiteSpace(password.Text))
                                {
                                    ShowAlertMessage(DI.Chakra.Infrastructure.AlertType.Error, "Username/password cannot be empty");
                                    return;
                                }

                                string specialCharacter = "!@#$%^&*()+=[]\\\';,/{}|\":<>?";

                                for (int i = 0; i < specialCharacter.Length; i++)
                                {
                                    if (userName.Text.Contains(specialCharacter[i].ToString()))
                                    {
                                        ShowAlertMessage(DI.Chakra.Infrastructure.AlertType.Error, "Special characters are not allowed in username");
                                        return;
                                    }
                                }

                                userNames.Add(userName.Text);
                            }

                            if (userNames.Count != userNames.Distinct(StringComparer.CurrentCultureIgnoreCase).Count())
                            {
                                ShowAlertMessage(AlertType.Error, "duplicate usernames are not allowed");
                                return;
                            }

                            string strUserNames = String.Join(",", userNames);

                            var param = new Dictionary<string, string>();
                            param.Add("endPointSystemId", SelectedEndpoint.Id.ToString());
                            param.Add("userNames", strUserNames);
                            string response = DomainApiClient<Dto.InhouseUserInfo>.GetByAction<string>("CheckUserNamesExists", param);

                            if (!string.IsNullOrWhiteSpace(response))
                            {
                                ShowAlertMessage(AlertType.Error, string.Format("Inhouse user names {0} already exist, please select a different username", response));
                                return;
                            }

                            List<InhouseUserCustomInfo> users = new List<InhouseUserCustomInfo>();

                            foreach (RepeaterItem item in rptInhUserInfo.Items)
                            {
                                var inhCustomInfo = new InhouseUserCustomInfo()
                                {
                                    UserId = long.Parse(((HiddenField)item.FindControl("hdnUserId")).Value),
                                    Password = ((TextBox)item.FindControl("txtPassword")).Text,
                                    EndpointSystemId = this.SelectedEndpoint.Id,
                                    Username = ((TextBox)item.FindControl("txtUsername")).Text,
                                    Entitlements = this.EntitlementRequest,
                                    CreatedBy = new SessionStatusImpl().User.DisplayName
                                };
                                users.Add(inhCustomInfo);
                            }

                            param.Clear();
                            saveResult = DomainApiClient<Dto.InhouseUserInfo>.GetByAction<bool>("PopulateUserInfos", param, users);
                        }
                    }
                    if (saveResult || task.BulkRequest.AdhocActionType != AdHocActionType.NewAccount)
                    {
                        AdHocINHRequest INHRequest = GetEntitlementDetail();
                        adhocRequestDetails.EntitlementDetail = adhocRequestDetails.GetSerializedString(INHRequest);
                        DomainApiClient<Dto.BulkRequest>.Save(adhocRequestDetails);
                        this.UpdateAccessRequest(AccessType.Approve);
                    }
                    else if (!saveResult)
                    {
                        ShowAlertMessage(AlertType.Error, "Unable to submit the request, please try again later");
                    }
                }
                if (adhocRequestDetails.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                {
                    string validation = "";
                    BulkGenericRequestDetails.UpdateADEntitlement(this.TaskId, out validation);
                    if (string.IsNullOrWhiteSpace(validation))
                    {
                        UpdateAccessRequest(AccessType.Approve);
                        Session["EndpointSystemIdValue"] = null;
                    }
                }
            }
        }

        public AdHocINHRequest GetEntitlementDetail()
        {
            string strEntitlements = string.Empty;

            if (Task.BulkRequest.AdhocActionType != AdHocActionType.DeleteAccount)
            {
                if (SelectedEndpoint.IsFunctionLevel)
                    strEntitlements = string.Join("|", EntitlementRequest.Select(p => p.Key + Constants.SPLITTER_FUCNTIONS + p.Value));
                else
                    strEntitlements = string.Join(",", EntitlementRequest.Select(p => p.Key));
            }

            AdHocINHRequest entitlementDetail = new AdHocINHRequest
            {
                EntitlementType = EntitlementType.Permission,
                Entitlement = strEntitlements
            };
            return entitlementDetail;
        }

        private string EntiltementString(object Control, string TypeofEntiltement)
        {
            string entiltment = "";
            RadComboBox control = (RadComboBox)Control;
            foreach (RadComboBoxItem checkedItems in control.CheckedItems)
            {
                if (TypeofEntiltement == "Roles")
                {
                    entiltment = entiltment == "" ? checkedItems.Value : entiltment + "," + checkedItems.Value;
                }
                else
                {
                    entiltment = entiltment == "" ? checkedItems.Value : entiltment + "," + checkedItems.Value;
                }
            }
            return entiltment;
        }

        private string GetPortalEntitlements(string TypeofEntiltement)
        {
            string entiltment = "";

            if (TypeofEntiltement == "Roles")
            {
                foreach (KeyValuePair<string, string> role in PortalRoles)
                {
                    entiltment = entiltment == "" ? role.Key : entiltment + "," + role.Key;
                }
            }
            if (TypeofEntiltement == "Groups")
            {
                foreach (KeyValuePair<string, string> group in PortalGroups)
                {
                    entiltment = entiltment == "" ? group.Key : entiltment + "," + group.Key;
                }
            }
            return entiltment;
        }
        private void ConfirmMessage1_ConfirmYesClick(object sender, EventArgs e)
        {
            this.UpdateAccessRequest(AccessType.Approve);
            Session["EndpointSystemIdValue"] = null;
        }
        bool CanShowGRCMessage()
        {
            var applications = new ApplicationType[] { ApplicationType.MSSL_ISU, ApplicationType.MSSL_CRM, ApplicationType.EBS_CRM, ApplicationType.EBS_ISU,ApplicationType.EBS, ApplicationType.MSSL };
            var adhocActions = new AdHocActionType[] { AdHocActionType.NewAccount, AdHocActionType.AmendmentAdd };
            var accountTypes = new AccountType[] { AccountType.User };
            return applications.Contains(Task.BulkRequest.ApplicationType) && adhocActions.Contains(Task.BulkRequest.AdhocActionType);
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            string validation = "";
            BulkGenericRequestDetails.UpdateADEntitlement(this.TaskId, out validation);
            if (validation == "")
            {
                var parameter = new Dictionary<string, string>();
                parameter.Add("Id", this.TaskId.ToString());
                Dto.Task task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.GetBulkRequestTask, parameter);

                string comments = txtComments.Text.Trim();
                var applications = new ApplicationType[] { ApplicationType.MSSL_ISU, ApplicationType.MSSL_CRM, ApplicationType.EBS_CRM, ApplicationType.EBS_ISU, ApplicationType.EBS, ApplicationType.MSSL };

                if (Task != null &&
                (Task.BulkRequest != null && (applications.Contains(Task.BulkRequest.ApplicationType))
                ) && (Task.BulkRequest != null && (Task.BulkRequest.AdhocActionType == AdHocActionType.NewAccount || Task.BulkRequest.AdhocActionType == AdHocActionType.AmendmentAdd))
                && (TCodeValueLabeltr.Visible && !string.IsNullOrWhiteSpace(TCodeValueLabel.Text) && string.IsNullOrWhiteSpace(txtComments.Text))
                )
                {
                    ShowAlertMessage(AlertType.Error, "Approver comments is required for Critical Tcode Access", true);
                    return;
                }

                if ((Task != null && Task.BulkRequest != null && applications.Contains(Task.BulkRequest.ApplicationType) && this.BulkGenericRequestDetails != null && this.BulkGenericRequestDetails.MSSLEBSConflictRemark != null && this.BulkGenericRequestDetails.MSSLEBSConflictRemark.Count > 0) && string.IsNullOrWhiteSpace(txtComments.Text))
                {
                    ShowAlertMessage(AlertType.Error, "Approver comments is required for Conflicting Access", true);
                    return;
                }

                this.UpdateAccessRequest(AccessType.Approve);
                Session["EndpointSystemIdValue"] = null;
            }

            //GenericRequestDetailsUC.UpdateADEntitlement(this.TaskId);
            //this.UpdateAccessRequest(AccessType.Approve);
            //Session["EndpointSystemIdValue"] = null;
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtComments.Text))
            {
                this.UpdateAccessRequest(AccessType.Revoke);
            }
            else
            {
                ShowAlertMessage(AlertType.Warning, "Please enter comments for Task Reject");
                // "*Please enter comments.".Replace("\n", "<br>");
            }
        }
        public void ShowConfirm(string Message)
        {
            ConfirmMessage1.Message = Message;
            ConfirmMessageUpdatePanel.Update();
            ConfirmMessage1.Show();
        }
        private void UpdateAccessRequest(AccessType accessType)
        {
            try
            {
                bool isRequiredMail = false;
                var parameter1 = new Dictionary<string, string> { { "Id", Task.Id.ToString() } };

                Dto.Task task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.GetBulkRequestTask, parameter1);
                this.Task = task;

                string comments = txtComments.Text.Trim();

                if (task.SubTask != null)
                {
                    if (accessType == AccessType.Approve)
                    {
                        comments = string.Format("(Approved for {0}) {1}", !string.IsNullOrEmpty(task.SubTask.RoleName) ? task.SubTask.RoleName : task.SubTask.TCode, comments).Trim();
                    }
                    else if (accessType == AccessType.Revoke)
                    {
                        comments = string.Format("(Rejected for {0}) {1}", !string.IsNullOrEmpty(task.SubTask.RoleName) ? task.SubTask.RoleName : task.SubTask.TCode, comments).Trim();
                    }
                }
                string comment = string.Empty;
                string conflictRemarks = string.Empty;
                if (accessType == AccessType.Approve)
                {
                    if(BulkGenericRequestDetails.IsEBSMSSLConflictRemark)
                    {
                        conflictRemarks = BulkGenericRequestDetails.MSSLEBSConflictRemark != null && BulkGenericRequestDetails.MSSLEBSConflictRemark.Any() ? UtilityMethods.Serialize<List<PresentationConflictRemarks>>(this.BulkGenericRequestDetails.MSSLEBSConflictRemark) : null;
                    }
                    else
                    {
                        conflictRemarks = BulkGenericRequestDetails.ConflictRemark != null && BulkGenericRequestDetails.ConflictRemark.Any() ? string.Join("<br/>", BulkGenericRequestDetails.ConflictRemark) : "";
                    }
                    comment = txtComments.Text;
                }
                else
                {
                    if (BulkGenericRequestDetails.IsEBSMSSLConflictRemark && BulkGenericRequestDetails.MSSLEBSConflictRemark != null)
                    {
                        conflictRemarks = BulkGenericRequestDetails.MSSLEBSConflictRemark != null && BulkGenericRequestDetails.MSSLEBSConflictRemark.Any() ? UtilityMethods.Serialize<List<PresentationConflictRemarks>>(this.BulkGenericRequestDetails.MSSLEBSConflictRemark) : null;
                    }
                    comment = txtComments.Text;
                }

                if (accessType == AccessType.Revoke)
                { isRequiredMail = true; }


                Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("accessType", accessType.ToString());
                //parameter.Add("UserName", _sessionStatus.User.UserName);
                //parameter.Add("TaskId", Task.Id.ToString());
                //parameter.Add("Comments", comment);
                //parameter.Add("AccountCreatedNotificationTO", MasterConfigHelper.AccountCreatedNotificationTO.ToString());
                //parameter.Add("conflictRemarks", conflictRemarks);

                PresentationWorkFlowCriteria wfCriteriaParam = new PresentationWorkFlowCriteria();
                wfCriteriaParam.AccessType = accessType;
                wfCriteriaParam.TaskId = Task.Id;
                wfCriteriaParam.UserName = _sessionStatus.User.UserName;
                wfCriteriaParam.Comments = comment;
                wfCriteriaParam.AccountCreatedNotificationTO = MasterConfigHelper.AccountCreatedNotificationTO.ToString();
                wfCriteriaParam.ConflictRemarks = conflictRemarks;

                var criteria = DomainApiClient<PresentationWorkflowCriteriaBase>.GetByAction<PresentationWorkflowCriteriaBase>
                                                (DomainApiAction.Workflow.Presentation.PresentationWorkflowCriteriaBase.GetWorkFlowCriteria, null, wfCriteriaParam);

                bool status = DomainApiClient<Object>.GetByAction<bool>
                                                        ("WorkflowController", DomainApiAction.WorkflowProcessor.SubmitWorkflowCriteria, null, criteria);

                if (status)
                {
                    //if (task.IsGroupTask)
                    //    Response.Redirect("~//Task//GroupTasks.aspx");
                    //else
                    //Response.Redirect("~//Task//List.aspx");
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "GoBack", "goBack();", true);
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "This task is already in approved state. Please contact administrator more information.");
                }
            }
            catch
            {
                throw;
            }
        }



        protected void btnback_Click(object sender, EventArgs e)
        {
            if (Session["TaskDetailReferrer"] != null)
            {
                string Referrer = Session["TaskDetailReferrer"].ToString();
                Session["TaskDetailReferrer"] = null;
                Response.Redirect(Referrer);
            }
            else
                Response.Redirect("~//Task//List.aspx");
        }

        public static string ApplicationName(ApplicationType applicationType)
        {
            string returnValue = "";
            switch (applicationType)
            {

                case ApplicationType.ADSingaporePowerLocal:
                    returnValue = "Active Directory";
                    break;

                case ApplicationType.NPAC:
                    returnValue = "NPAC";
                    break;
                case ApplicationType.GEMS:
                    returnValue = "GEMS";
                    break;


                case ApplicationType.SAP_EP:
                case ApplicationType.CE:
                    returnValue = "SAPEP";
                    break;

                case ApplicationType.EBS_CRM:
                    returnValue = "SAP EBS CRM";
                    break;
                case ApplicationType.EBS_ISU:
                    returnValue = "SAP EBS ISU";
                    break;


                case ApplicationType.FLINS_BI:
                    returnValue = "SAP FLINS BI";
                    break;
                case ApplicationType.HRMS_FLINS:
                    returnValue = "SAP FLINS";
                    break;

                case ApplicationType.MSSL_CRM:
                    returnValue = "SAP MSSL CRM";
                    break;
                case ApplicationType.MSSL_ISU:
                    returnValue = "SAP MSSL ISU";
                    break;
                case ApplicationType.SAP_GRC:
                    returnValue = "SAP GRC";
                    break;
                case ApplicationType.SAP_PI:
                    returnValue = "SAP PI";
                    break;
                default:
                    returnValue = "";
                    break;
            }
            return returnValue;
        }

        protected void GroupAutoCompleteBox_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            //GroupEntitlementLabel.Text = "";
            //IList<string> groups = new List<string>();
            //for (int i = 0; i < GroupAutoCompleteBox.Items.Count; i++)
            //{
            //    if (GroupAutoCompleteBox.Items[i].Checked)
            //        groups.Add(GroupAutoCompleteBox.Items[i].Text);
            //    else
            //        if (groups.Contains(GroupAutoCompleteBox.Items[i].Text))
            //            groups.Remove(GroupAutoCompleteBox.Items[i].Text);
            //}
            //foreach (string group in groups)
            //    GroupEntitlementLabel.Text = GroupEntitlementLabel.Text + group + "</br>";
            //GroupEntitlementLabel.Visible = !string.IsNullOrEmpty(GroupEntitlementLabel.Text);
        }

        protected void RoleAutoCompleteBox_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            //RoleEntitlementLabel.Text = "";
            //IList<string> roles = new List<string>();
            //for (int i = 0; i < RoleAutoCompleteBox.Items.Count; i++)
            //{
            //    if (RoleAutoCompleteBox.Items[i].Checked)
            //    {
            //        roles.Add(RoleAutoCompleteBox.Items[i].Text);
            //    }
            //    else
            //        if(roles.Contains(RoleAutoCompleteBox.Items[i].Text))
            //            roles.Remove(RoleAutoCompleteBox.Items[i].Text);
            //}

            //foreach(string role in roles)
            //    RoleEntitlementLabel.Text = RoleEntitlementLabel.Text + role + "</br>";
            //RoleEntitlementLabel.Visible = !string.IsNullOrEmpty(RoleEntitlementLabel.Text);
        }

        protected void btnGrpAdd_Click(object sender, EventArgs e)
        {
            if (this.GroupsAutoCompleteBox.Entries.Count != 0)
            {

                string groupUniqueName = this.GroupsAutoCompleteBox.Entries[0].Value;
                string group = this.GroupsAutoCompleteBox.Entries[0].Text;
                var groups = this.PortalGroups;

                if (!groups.ContainsKey(groupUniqueName))
                {
                    groups.Add(groupUniqueName, group);
                }

                this.PortalGroups = groups;

                GroupGrid.DataSource = PortalGroups;
                GroupGrid.Rebind();
                trGroupGrid.Visible = true;

                this.GroupsAutoCompleteBox.Entries.Clear();
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select at least one group");

            }
        }

        protected void btnroleAdd_Click(object sender, EventArgs e)
        {
            if (this.RolesAutoCompleteBox.Entries.Count != 0)
            {

                string roleId = this.RolesAutoCompleteBox.Entries[0].Value;
                string role = this.RolesAutoCompleteBox.Entries[0].Text;
                var roles = this.PortalRoles;

                if (!roles.ContainsKey(roleId))
                {
                    roles.Add(roleId, role);
                }

                this.PortalRoles = roles;

                RoleGrid.DataSource = PortalRoles;
                RoleGrid.Rebind();
                trRoleGrid.Visible = true;

                this.RolesAutoCompleteBox.Entries.Clear();
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select at least one role");
            }
        }

        protected void DeleteRoleButton_Click(object sender, EventArgs e)
        {
            string roleId = ((LinkButton)sender).CommandArgument.ToString();
            var roles = this.PortalRoles;
            roles.Remove(roleId);
            this.PortalRoles = roles;

            RoleGrid.DataSource = PortalRoles;
            RoleGrid.Rebind();
        }

        protected void DeleteGroupButton_Click(object sender, EventArgs e)
        {
            string groupUniqueName = ((LinkButton)sender).CommandArgument;
            var groups = this.PortalGroups;
            groups.Remove(groupUniqueName);
            this.PortalGroups = groups;

            GroupGrid.DataSource = PortalGroups;
            GroupGrid.Rebind();
        }

        protected void GroupsAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {

        }

        protected void RolesAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {

        }

        #region Notification
        protected void lnkViewNotification_Click(object sender, EventArgs e)
        {
            ViewNotification.RequestId = this.Task.AccessRequestId;
            ViewNotification.BindNotifications();
            RadWinViewNotification.VisibleOnPageLoad = true;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            RadWinViewNotification.VisibleOnPageLoad = false;
        }

        protected void btnSendNotification_Click(object sender, EventArgs e)
        {
            UCSendNotification.BindNotifiableUsers(this.Task.BulkRequestId, DI.Chakra.Infrastructure.Constants.BulkRequest);
            SendNotificationDisplay(true);
        }

        private void SendNotificationDisplay(bool IsDisplay)
        {
            UCSendNotification.ClearControls();
            RadWinSendNotification.VisibleOnPageLoad = IsDisplay;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string validationMessage = UCSendNotification.ValidateNotification();
            if (!string.IsNullOrEmpty(validationMessage))
            {
                RadWinSendNotification.VisibleOnPageLoad = true;
                MasterPage.ShowAlert(AlertType.Warning, validationMessage);
                return;
            }
            Dto.Notification notification = UCSendNotification.GetNotificationDetails();
            string appendSubText = "Bulk Request Id - " + (long)Task.BulkRequestId + " ";
            if (!notification.Subject.Contains(appendSubText))
                notification.Subject = appendSubText + notification.Subject;
            notification.RequestId = (long)Task.BulkRequestId;
            DomainApiClient<Dto.Notification>.Save(notification);
            SendNotificationDisplay(false);
            MasterPage.ShowAlert(AlertType.Success, "Notification sent successfully");
        }

        protected void btnCancelSend_Click(object sender, EventArgs e)
        {
            UCSendNotification.RequestId = this.Task.AccessRequestId;
            SendNotificationDisplay(false);
        }
        #endregion

        #region Attach

        protected void lnkViewAttachment_Click(object sender, EventArgs e)
        {
            ViewAttachmentUC.RequestId = this.Task.BulkRequestId;
            ViewAttachmentUC.AccessRequestSource = Infrastructure.Constants.BulkRequest;
            ViewAttachmentUC.BindGrid();
            RadWinViewAttachment.VisibleOnPageLoad = true;
        }

        protected void btnViewOk_Click(object sender, EventArgs e)
        {
            RadWinViewAttachment.VisibleOnPageLoad = false;
        }

        protected void btnAttach_Click(object sender, EventArgs e)
        {
            RadWinAttachFile.VisibleOnPageLoad = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (AsyncUploadAttach.UploadedFiles.Count > 0)
            {
                List<Dto.Attachment> attachments = new List<Attachment>();
                foreach (Telerik.Web.UI.UploadedFile file in AsyncUploadAttach.UploadedFiles)
                {
                    Dto.Attachment attachment = new Dto.Attachment();
                    attachment.AccessRequestSource = Infrastructure.Constants.BulkRequest;
                    attachment.RequestTypeId = (long)RequestType.Provisioning;
                    attachment.TaskId = this.Task.Id;
                    attachment.FileName = file.FileName;
                    attachment.AccessRequestId = Convert.ToInt64(this.Task.BulkRequestId);
                    attachment.ContentType = file.ContentType;
                    byte[] fileData = new byte[file.ContentLength];
                    file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
                    attachment.AttachmentFile = fileData;
                    attachments.Add(attachment);
                }
                DomainApiClient<Dto.Attachment>.SaveList(attachments);
                ViewAttachmentUC.RequestId = this.Task.BulkRequestId;
                ViewAttachmentUC.AccessRequestSource = Infrastructure.Constants.BulkRequest;
                ViewAttachmentUC.attachmentCount = ViewAttachmentUC.getCount();
                lnkViewAttachment.Text = "View Attachments (" + ViewAttachmentUC.attachmentCount + ")";
                lnkViewAttachment.DataBind();
                AsyncUploadAttach.UploadedFiles.Clear();
                RadWinAttachFile.VisibleOnPageLoad = false;
                MasterPage.ShowAlert(AlertType.Success, "Attachment/s saved successfully.");
            }
            else
                MasterPage.ShowAlert(AlertType.Warning, "Pls attach at least one file.");
        }

        protected void btnCancelAdd_Click(object sender, EventArgs e)
        {
            AsyncUploadAttach.UploadedFiles.Clear();
            RadWinAttachFile.VisibleOnPageLoad = false;
        }

        #endregion

        void boundEPGroups(string groupUniqueName, string group)
        {
            var groups = this.PortalGroups;
            if (!groups.ContainsKey(groupUniqueName))
            {
                groups.Add(groupUniqueName, group);
            }

            this.PortalGroups = groups;

            GroupGrid.DataSource = PortalGroups;
            GroupGrid.Rebind();
            trGroupGrid.Visible = true;
        }

        void boundEPRoles(string roleId, string roleName)
        {
            var roles = this.PortalRoles;

            if (!roles.ContainsKey(roleId))
            {
                roles.Add(roleId, roleName);
            }

            this.PortalRoles = roles;

            RoleGrid.DataSource = PortalRoles;
            RoleGrid.Rebind();
            trRoleGrid.Visible = true;
        }

        protected void rptInhUserInfo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var userInfo = e.Item.DataItem as BulkRequestDetail;
                var userInfoUC = e.Item.FindControl("UserInfoUC") as UserInfoUC;
                userInfoUC.User = DomainApiClient<Dto.User>.GetById(userInfo.UserId);
                //var lnkButton = e.Item.FindControl("lnkViewCurrentAccess") as LinkButton;
                //if (lnkButton != null)
                //{
                //    this.MasterPage.Radscriptmanager.RegisterAsyncPostBackControl(lnkButton);
                //}
                //var currentAccessUC = e.Item.FindControl("ViewCurrentAccessUC") as ViewCurrentAccessUC;
                //currentAccessUC.UserId = userInfo.UserId;
            }
        }

        private void RebindPermissionGrid()
        {
            EntitlementRequest.Clear();
            var entitlement = Task.BulkRequest.Entitlement;
            var requests = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrWhiteSpace(entitlement))
            {
                if (SelectedEndpoint.IsFunctionLevel)
                {
                    foreach (var item in entitlement.Split('|'))
                    {
                        var ent = item.Split(new string[] { Constants.SPLITTER_FUCNTIONS }, StringSplitOptions.RemoveEmptyEntries);
                        if (ent.Count() > 1)
                            requests.Add(new KeyValuePair<string, string>(ent[0], ent[1]));
                        else
                            requests.Add(new KeyValuePair<string, string>(ent[0], string.Empty));
                    }
                }
                else
                {
                    foreach (var item in entitlement.Split(','))
                    {
                        requests.Add(new KeyValuePair<string, string>(item, string.Empty));
                    }
                }

                EntitlementRequest = requests;
            }
            FillRequestGrid();
        }

        void AddRequestForFunctions()
        {
            //if (RolesAutoCompleteBox.Entries.Count > 0 && ddlFunctions.CheckedItems.Count > 0)
            //{
            //    var requests = new List<KeyValuePair<string, string>>();
            //    string newPermission = RolesAutoCompleteBox.Entries[0].Text.Replace(";", "").Trim();
            //    List<string> lstFunctions = new List<string>();
            //    bool exists = false;
            //    foreach (var function in ddlFunctions.CheckedItems)
            //    {
            //        exists = EntitlementExists(newPermission, function.Text);
            //        if (exists)
            //        {
            //            break;
            //        }
            //        lstFunctions.Add(function.Text);
            //    }

            //    if (!exists)
            //    {
            //        string newFunctions = string.Join(",", lstFunctions);
            //        requests = this.EntitlementRequest;
            //        requests.Add(new KeyValuePair<string, string>(newPermission, newFunctions));
            //        this.EntitlementRequest = requests;
            //        FillRequestGrid();
            //        RolesAutoCompleteBox.Entries.Clear();
            //        ddlFunctions.Items.Clear();
            //    }
            //    else
            //    {
            //        MasterPage.ShowAlert(AlertType.Error, "The permission and function are already in the request list.");
            //    }
            //}
            //else
            //{
            //    MasterPage.ShowAlert(AlertType.Error, "Please select the permission and function.");
            //}
        }

        protected void RolesAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            //if (RolesAutoCompleteBox.Entries.Count == 0)
            //{
            //    ddlFunctions.Items.Clear();
            //}
        }

        private bool EntitlementExists(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                foreach (KeyValuePair<string, string> req in EntitlementRequest)
                {
                    if (req.Key == key && req.Value == value)
                        return true;
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> req in EntitlementRequest)
                {
                    if (req.Key == key && req.Value.Split(',').Contains(value))
                        return true;
                }
            }

            return false;
        }

        void AddRequestForPermissionOnly()
        {
            //if (INHRoleAutoCompleteBox.Entries.Count > 0)
            //{
            //    var requests = new List<KeyValuePair<string, string>>();
            //    string newPermission = INHRoleAutoCompleteBox.Entries[0].Text.Replace(";", "").Trim();
            //    string newFunctions = string.Empty;
            //    requests = this.EntitlementRequest;
            //    if (!EntitlementExists(newPermission, newFunctions))
            //    {
            //        requests.Add(new KeyValuePair<string, string>(newPermission, newFunctions));
            //        this.EntitlementRequest = requests;
            //        FillRequestGrid();
            //        INHRoleAutoCompleteBox.Entries.Clear();
            //    }
            //    else
            //    {
            //        MasterPage.ShowAlert(AlertType.Error, "The permission is already in the request list.");
            //    }
            //}
            //else
            //{
            //    MasterPage.ShowAlert(AlertType.Error, "Please select the permission.");
            //}
        }

        protected void AddPermissionButton_Click(object sender, EventArgs e)
        {
            if (Task.BulkRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                return;

            if (SelectedEndpoint.IsFunctionLevel && !SelectedEndpoint.CanAutomate)
            {
                AddRequestForFunctions();
            }
            else
            {
                AddRequestForPermissionOnly();
            }
        }

        private void FillRequestGrid()
        {
            if (this.EntitlementRequest == null)
            {
                EntitlementRequest = new List<KeyValuePair<string, string>>();
            }
            RequestGrid.DataSource = this.EntitlementRequest;
            divGrid.Visible = this.EntitlementRequest.Count == 0 ? false : true;
            RequestGrid.DataBind();
        }

        protected void RequestGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = this.EntitlementRequest;
                requests.RemoveAt(e.Item.ItemIndex);
                this.EntitlementRequest = requests;
                FillRequestGrid();
            }
        }

        bool IsAdhocRequestNull()
        {
            return Task.AccessRequest != null && Task.AccessRequest.AdHocAccessRequest != null;
        }

        protected void INHRoleAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            //if (RolesAutoCompleteBox.Entries.Count > 1)
            //{
            //    RolesAutoCompleteBox.Entries.RemoveAt(1);
            //    return;
            //}

            //if (!SelectedEndpoint.IsFunctionLevel)
            //    return;

            //if (RolesAutoCompleteBox.Entries.Count == 1 && IsAdhocRequestNull())
            //{
            //    Dictionary<string, string> parameter = new Dictionary<string, string>();
            //    parameter.Add("endpointId", Convert.ToString(Task.AccessRequest.AdHocAccessRequest.EndpointSystemId.Value));
            //    parameter.Add("permission", RolesAutoCompleteBox.Entries[0].Text);
            //    parameter.Add("endpointUserId", Convert.ToString(0));
            //    parameter.Add("type", Convert.ToInt32(Task.AccessRequest.AdHocAccessRequest.AdhocActionType).ToString());
            //    var functions = DomainApiClient<InhouseFunction>.
            //        GetByAction<List<InhouseFunction>>(DomainApiAction.Endpoint.InhouseSystems.GetInhouseFunctionActive, parameter);
            //    ddlFunctions.DataSource = functions;
            //    ddlFunctions.DataTextField = "FunctionName";
            //    ddlFunctions.DataValueField = "Id";
            //    ddlFunctions.DataBind();
            //    foreach (RadComboBoxItem itm in ddlFunctions.Items)
            //    {
            //        itm.Checked = true;
            //    }
            //}
            //else
            //{
            //    ddlFunctions.Items.Clear();
            //}
        }

        protected void INHRoleAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            //if (RolesAutoCompleteBox.Entries.Count == 0)
            //{
            //    ddlFunctions.Items.Clear();
            //}
        }

        protected void btnViewAccess_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptInhUserInfo.Items)
            {
                if (item != null)
                {
                    var lnkButton = item.FindControl("lnkViewCurrentAccess") as LinkButton;
                    if (lnkButton != null)
                    {
                        this.MasterPage.Radscriptmanager.RegisterAsyncPostBackControl(lnkButton);
                    }
                }

            }
            CloseWindow();
        }
        void ShowWindow()
        {
            var key = "showWindoww";
            string script = "Sys.Application.remove_load(showWindow);Sys.Application.add_load(showWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindow()
        {
            var key = "closeWindoww";
            string script = "Sys.Application.remove_load(closeWindow);Sys.Application.add_load(closeWindow);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        protected void lnkViewCurrentAccess_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "show")
            {

                var userid = Convert.ToInt64(e.CommandArgument);
                ucCurrentAccess.EntityId = userid;
                ucCurrentAccess.HideAccountDetail = true;
                ucCurrentAccess.Rebind();
                ShowWindow();
            }
        }



        protected void rptInhUserInfo_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            ScriptManager scriptMan = this.MasterPage.Radscriptmanager;
            LinkButton btn = e.Item.FindControl("lnkViewCurrentAccess") as LinkButton;
            if (btn != null)
            {
                btn.Command += lnkViewCurrentAccess_Command;
                scriptMan.RegisterAsyncPostBackControl(btn);
            }
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                if (this.ContextObjectId != null)
                    parameter.Add("Id", this.ContextObjectId.Value.ToString());
                else
                    parameter.Add("Id", Request.QueryString["Id"].ToString());
                Dto.Task task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>(DomainApiAction.Tasks.Task.GetBulkRequestTask, parameter);
                if (task != null && task.BulkRequest != null)
                {
                    this.TaskApprovers.Clear();
                    InitializeForm(task);

                }
            }
            catch (Exception)
            {
            }
        }
    }
}
