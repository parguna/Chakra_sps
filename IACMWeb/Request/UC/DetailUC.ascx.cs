using Application;
using System;
using System.Collections.Generic;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Dto.Presentation;
using System.Linq;
using System.Web.UI;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.Request.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Members
        private SessionStatusImpl _sessionStatus = new SessionStatusImpl();
        private const string V_REDIRECTURL = "V_REDIRECTURL";
        private const string V_ADHOCREQUEST = "V_ADHOCREQUEST";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_ACCESSREQUEST = "V_ACCESSREQUEST";
        public bool _isFromDashboard;
        #endregion

        #region Properties
        public AdHocAccessRequest AdhocAccessRequest
        {
            get
            {
                if (ViewState[V_ADHOCREQUEST] == null)
                    return new AdHocAccessRequest();
                return (AdHocAccessRequest)ViewState[V_ADHOCREQUEST];
            }
            set
            {
                ViewState[V_ADHOCREQUEST] = value;
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

        public bool IsPageCalledFromDashboard
        {
            set
            {
                _isFromDashboard = value;
            }
            get
            {
                return _isFromDashboard;
            }
        }

        public string RedirectURL
        {
            get
            {
                if (ViewState[V_REDIRECTURL] == null)
                    return "~/Request/List.aspx";
                return (string)ViewState[V_REDIRECTURL];
            }
            set
            {
                ViewState[V_REDIRECTURL] = value;
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

        #endregion

        #region Methods
        private void SetPage()
        {
            if (ContextObjectId > 0)
            {
                switch (this.TypeofRequest)
                {

                    case Constants.AccessRequest:
                    case Constants.AttestationRemediation:
                        {
                            ApprverChain.Visible = false;
                            AdhocAccessRequest = DomainApiClient<Dto.AdHocAccessRequest>.GetById(ContextObjectId.Value, true);
                            InfoUC.AdhocAccessRequest = AdhocAccessRequest;
                            GenericRequestDetails.AdHocRequestId = AdhocAccessRequest.Id;
                            //CommonTaskInfoUC.AdhocAccessRequest = AdhocAccessRequest;
                            // ApprverChain.AdhocAccessRequest = AdhocAccessRequest;
                            Dictionary<string, string> adhocRequestId = new Dictionary<string, string>();
                            adhocRequestId.Add("adhocAccessRequestId", Convert.ToString(AdhocAccessRequest.Id));
                            AccessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>
                               (DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, adhocRequestId);
                            WithDrawnButton.Visible = (IsAdminMode == IsAdminMode.Yes || AccessRequest.ProgressStatus == AccessRequestProgress.Completed
                                                                                        || AccessRequest.ProgressStatus == AccessRequestProgress.ExecutionFailed
                                                                                        || AccessRequest.ProgressStatus == AccessRequestProgress.Executing
                                                                                        || AccessRequest.ProgressStatus == AccessRequestProgress.Withdrawn
                                                                                        || AccessRequest.ProgressStatus == AccessRequestProgress.RequestRejected
                                                                                        || AccessRequest.ProgressStatus == AccessRequestProgress.WaitingForSync
                                                                                        || AccessRequest.ProgressStatus == AccessRequestProgress.WaitingForMailBoxCreation
                                                                                        || AccessRequest.ProgressStatus == AccessRequestProgress.WaitingForLicenseAssignment 
                                                                                        || (AccessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute
                                                                                        && AccessRequest.ExecutionTime.HasValue && AccessRequest.ExecutionTime.Value.Date <= DateTime.Now.Date)
                                ) ? false : true;
                            if(AccessRequest.RequestSource == AccessRequestSource.NonEmployeeExtension && IsAdminMode == IsAdminMode.No)
                            {
                                WithDrawnButton.Visible = false;
                            }
                            if(AccessRequest != null && AccessRequest.ApplicationType == ApplicationType.InhouseSystems && AccessRequest.AdHocAccessRequest != null && AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount && WithDrawnButton.Visible && IsAdminMode == IsAdminMode.No)
                            {
                                var details = AccessRequest.AdHocAccessRequest.GetEntitlementDetail() as AdHocINHRequest;
                                if(details != null && details.IsOffBoardRequest)
                                {
                                    WithDrawnButton.Visible = false;
                                }
                            }else if (AccessRequest != null && AccessRequest.ApplicationType == ApplicationType.VPN && AccessRequest.AdHocAccessRequest != null && AccessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount && WithDrawnButton.Visible && IsAdminMode == IsAdminMode.No)
                            {
                                var details = AccessRequest.AdHocAccessRequest.GetEntitlementDetail() as AdHocVPNRequest;
                                if (details != null && details.IsOffboardOrTransferRequest)
                                {
                                    WithDrawnButton.Visible = false;
                                }
                            }
                            SetApproverForm();
                        }
                        break;
                    case Constants.BulkRequest:
                        {
                            if (Session["RedirectFrom"] == null)
                                Session["RedirectFrom"] = "RequestDetails";

                            int progressStatus = !string.IsNullOrWhiteSpace(Request.QueryString["progressStatus"]) ? int.Parse(Request.QueryString["progressStatus"]) : 0;
                            string requestFor = Request.QueryString["requestFor"];
                            string requestForName = Request.QueryString["requestForName"];
                            string endpointsystemId = Request.QueryString["endpointsystemId"];
                            var datefilter = string.Empty;

                            if (!string.IsNullOrWhiteSpace(Request.QueryString["fromDate"]) && !string.IsNullOrWhiteSpace(Request.QueryString["toDate"]))
                            {
                                datefilter = $"&fromDate={Request.QueryString["fromDate"]}&toDate={Request.QueryString["toDate"]}";
                            }

                            Response.Redirect($"~/Request/BulkRequest/MyBulkRequestDetails.aspx?Id={ContextObjectId}&progressStatus={progressStatus}&requestFor={requestFor}&{datefilter}&requestForName={requestForName}&endpointsystemId={endpointsystemId}");// + Convert.ToInt32(item.GetDataKeyValue("AccessRequestID")));

                        }

                        break;
                    case Constants.NonEmployee:
                        ApprverChain.Visible = false;
                        var trigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetById(ContextObjectId.Value);
                        if (trigger == null)
                            return;

                        var user = DomainApiClient<Dto.User>.GetById(trigger.UserId);

                        Dictionary<string, string> param = new Dictionary<string, string>();
                        param.Add("triggerId", trigger.Id.ToString());
                        var task = DomainApiClient<Dto.Task>.GetByAction<Dto.Task>("GetTaskInfoByTriggerId", param);

                        InfoUC.SetFormForNonEmployeeUser(trigger, task, user);

                        GenericRequestDetails.SetNonEmployeeInfo(user, trigger);
                        GenericRequestDetails.SetWithDrawVisibility(user, trigger, task);
                        SetApproverFormForEmployeeEvent(trigger.Id);
                        WithDrawnButton.Visible = user.RequestStatus == AccessRequestStatus.Requested;


                        break;
                    case Constants.OUChange:
                    case Constants.OnBoarding:
                    case Constants.Rehire:
                    case Constants.Propagation:
                    case Constants.OffBoarding:
                    case Constants.ReOrganisation:
                    case Constants.UndoEvent:
                        {
                            if (!string.IsNullOrWhiteSpace(Request.QueryString["HRMSRequest"]) && Request.QueryString["HRMSRequest"] == "1")
                            {
                                ApprverChain.Visible = false;
                                var accessRequest = DomainApiClient<Dto.AccessRequest>.GetById(ContextObjectId.Value, true);
                                InfoUC.HRMSAccessRequest = accessRequest;
                                GenericRequestDetails.PopulateHRMSRequestDetails(accessRequest);
                                idHeadingApprovals.Visible = false;
                                gridApprovals.Visible = false;
                                WithDrawnButton.Visible = false;
                            }
                            else
                            {
                                ApprverChain.Visible = false;
                                AdhocAccessRequest = DomainApiClient<Dto.AdHocAccessRequest>.GetById(ContextObjectId.Value, true);
                                InfoUC.AdhocAccessRequest = AdhocAccessRequest;
                                GenericRequestDetails.AdHocRequestId = AdhocAccessRequest.Id;
                                Dictionary<string, string> adhocRequestId = new Dictionary<string, string>();
                                adhocRequestId.Add("adhocAccessRequestId", Convert.ToString(AdhocAccessRequest.Id));
                                AccessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>
                                   (DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, adhocRequestId);
                                WithDrawnButton.Visible = false;
                                SetApproverForm();
                            }
                        }
                        break;
                }
            }
            else
            {
                ApprverChain.Visible = false;
                WithDrawnButton.Visible = false;
            }
        }

        private void Withdraw()
        {
            if (this.TypeofRequest == Constants.NonEmployee)
            {
                var trigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetById(ContextObjectId.Value);
                if (trigger == null)
                    return;

                Dictionary<string, string> parametertask = new Dictionary<string, string>();
                parametertask.Add("employeeEventId", Convert.ToString(trigger.Id));
                var taskList = DomainApiClient<Dto.Task>.GetByAction<List<Dto.Task>>
                    ("GetTaskByEmployeeEventId", parametertask);
                Dto.Presentation.PresentationWorkflowCriteriaBase _criteria = null;
                taskList.ForEach(l =>
                {
                    _criteria = new Dto.Presentation.WithdrawActionCriteria()
                    {
                        RequestType = RequestType.EmployeeEvent,
                        WorkflowInstanceId = l.WorkflowInstanceId
                    };
                });

                if (_criteria != null)
                {
                    bool status = DomainApiClient<Object>.GetByAction<bool>("WorkflowController", DomainApiAction.WorkflowProcessor.WithDrawnRequest, null, _criteria);
                    if (status)
                    {
                        trigger.Comments = ReasonField.Text;
                        trigger.RequestStatus = AccessRequestStatus.Withdrawn;
                        DomainApiClient<Dto.EmployeeEventTrigger>.Save(trigger);
                        var user = DomainApiClient<Dto.User>.GetById(trigger.UserId);
                        user.RequestStatus = AccessRequestStatus.Withdrawn;
                        DomainApiClient<Dto.User>.Save(user);
                        CloseWindow();
                        Response.Redirect(RedirectURL);
                    }
                }
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("adhocAccessRequestId", Convert.ToString(AdhocAccessRequest.Id));
                Dto.AccessRequest accessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>
                    (DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, parameter);

                if (accessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute)
                {
                    accessRequest.ProgressStatus = AccessRequestProgress.Withdrawn;
                    accessRequest.RequestStatus = AccessRequestStatus.Withdrawn;
                    accessRequest.Comments = ReasonField.Text;
                    accessRequest.Remarks = DateTime.Now.ToLongDateString();
                    ReasonField.Text = "";
                    DomainApiClient<Dto.AccessRequest>.Save(accessRequest);
                    CloseWindow();
                    Response.Redirect(RedirectURL);
                    return;
                }

                accessRequest.Remarks = DateTime.Now.ToString();
                accessRequest.Comments = ReasonField.Text;
                ReasonField.Text = "";
                DomainApiClient<Dto.AccessRequest>.Save(accessRequest);
                accessRequest.RequestStatus = AccessRequestStatus.Withdrawn;

                Dictionary<string, string> parametertask = new Dictionary<string, string>();
                parametertask.Add("accessRequestId", Convert.ToString(accessRequest.Id));
                var taskList = DomainApiClient<Dto.Task>.GetByAction<List<Dto.Task>>
                    (DomainApiAction.Tasks.Task.GekByAccessRequestId, parametertask);
                Dto.Presentation.PresentationWorkflowCriteriaBase _criteria = null;
                taskList.ForEach(l =>
                {
                    _criteria = new Dto.Presentation.WithdrawActionCriteria()
                    {
                        RequestType = l.AttestationId != null ? RequestType.Attestation : RequestType.Provisioning,
                        WorkflowInstanceId = l.WorkflowInstanceId,
                        AccessRequest = l.AccessRequest
                    };
                });

                if (_criteria != null)
                {
                    bool status = DomainApiClient<Object>.GetByAction<bool>("WorkflowController", DomainApiAction.WorkflowProcessor.WithDrawnRequest, null, _criteria);
                    
                }
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Request:{this.AccessRequest?.Id} withdrawn by {new SessionStatusImpl().User.DisplayName} with Reason :{accessRequest?.Comments}");
                accessRequest.ProgressStatus = AccessRequestProgress.Withdrawn;
                DomainApiClient<Dto.AccessRequest>.Save(accessRequest);
                CloseWindow();
                Response.Redirect(RedirectURL);
            }

        }


        private void Refresh()
        {
            //    if (ContextObjectId > 0)
            //    {
            //        AdhocAccessRequest = DomainApiClient<Dto.AdHocAccessRequest>.GetById(ContextObjectId.Value, true);
            //        InfoUC.AdhocAccessRequest = AdhocAccessRequest;
            //        WithDrawnButton.Visible = (IsAdminMode == IsAdminMode.Yes || AccessRequest.ProgressStatus == AccessRequestProgress.Completed || AccessRequest.ProgressStatus == AccessRequestProgress.Withdrawn || AccessRequest.ProgressStatus == AccessRequestProgress.RequestRejected || AccessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute
            //) ? false : true;
            //        SetApproverForm();
            //    }
            SetPage();
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack && ContextObjectId != null)
            {
                SetPage();
            }
            else if (IsPageCalledFromDashboard)
            {
                SetPage();
            }
            ViewAttachmentUC.RequestId = AccessRequest.Id;

        }
        protected override void OnPreRender(EventArgs e)
        {
            lnkViewAttachment.Text = "View Attachments (" + ViewAttachmentUC.attachmentCount + ")";
            var AccessPanel = this.MasterPage.FindControl("AccessPanel");
            if (AccessPanel != null && AccessPanel.FindControl("rptPermissions") != null)
            {
                var Repeater = (Repeater)AccessPanel.FindControl("rptPermissions");
                foreach (RepeaterItem item in Repeater.Items)
                {
                    Panel Panel = item.FindControl("PermissionPanel") as Panel;
                    if (Panel != null)
                    {
                        if ((IsAdminMode == IsAdminMode.Yes || AccessRequest.ProgressStatus == AccessRequestProgress.Completed ||
                            AccessRequest.ProgressStatus == AccessRequestProgress.Withdrawn || AccessRequest.ProgressStatus == AccessRequestProgress.RequestRejected
                            || AccessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute) && ((Panel.ToolTip == Convert.ToString(ACLAction.Withdraw))))
                        {
                            Panel.Enabled = false;
                        }
                        else
                        {
                            Panel.Enabled = true;
                        }
                    }
                }
            }
            base.OnPreRender(e);
        }



        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Withdraw)
            {
                Withdraw();
            }

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
                return;

            Withdraw();
        }
        protected void Back_Click(object sender, EventArgs e)
        {
            int progressStatus = !string.IsNullOrWhiteSpace(Request.QueryString["progressStatus"]) ? int.Parse(Request.QueryString["progressStatus"]) : 0;
            string requestFor = Request.QueryString["requestFor"];
            string requestForName = Request.QueryString["requestForName"];
            string endpointsystemId = Request.QueryString["endpointsystemId"];
            string fromDate = Request.QueryString["fromDate"];
            string toDate = Request.QueryString["toDate"];
            string IncludeAdhocAccessRequest = Request.QueryString["IncludeAdhocAccessRequest"];
            _sessionStatus = new SessionStatusImpl();
            if (_sessionStatus.RedirectFrom == "MyNotifications")
                Response.Redirect("~/Notification/Detail.aspx?Id=" + Request.QueryString["NotifyId"].ToString());
            if (_sessionStatus.RedirectFrom == "AlertCenter")
                Response.Redirect("~/AlertCenter/List.aspx");
            if (_sessionStatus.RedirectFrom == "Home")
                Response.Redirect("~/Home/Home.aspx");
            if (_sessionStatus.RedirectFrom == "AdminRequest")
                Response.Redirect($"~/Prov/Request/List.aspx?progressStatus={progressStatus}&requestFor={requestFor}&fromDate={fromDate}&toDate={toDate}&requestForName={requestForName}&endpointsystemId={endpointsystemId}");
            if (_sessionStatus.RedirectFrom == "MyRequest")
                Response.Redirect("~/Request/List.aspx");
            if (_sessionStatus.RedirectFrom == "SubmitFailedAdhocRequest" || !string.IsNullOrWhiteSpace(IncludeAdhocAccessRequest))
            {
                Response.Redirect($"~/Prov/SubmitFailedAdhocRequest.aspx?IncludeAdhocAccessRequest={Request.QueryString["IncludeAdhocAccessRequest"]}&failedRequestType={Request.QueryString["failedRequestType"]}");
            }
            else Response.Redirect(RedirectURL);
        }
        protected void Refresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }
        #endregion

        protected void btnOk_Click(object sender, EventArgs e)
        {
            RadWinViewNotification.VisibleOnPageLoad = false;
        }

        protected void grdApprover_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            byte state = (byte)dataItem.GetDataKeyValue("ApprovalState");

            try
            {
                var approver1 = dataItem.GetDataKeyValue("ApproverName").ToString();
                var approver = this.Approvers.FirstOrDefault(item => item.ApprovalState == state && item.ApproverName == approver1);
                e.DetailTableView.DataSource = approver.ApproverDetails;
            }
            catch
            {
                var approver = this.Approvers.FirstOrDefault(item => item.ApprovalState == state);
                e.DetailTableView.DataSource = approver.ApproverDetails;
            }
        }

        public List<PresentationApprovalChain> Approvers
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

        bool ShowApproverForm()
        {
            var show = true;
            if (AccessRequest != null)
            {
                if (AccessRequest.BulkRequestId > 0 || AccessRequest.IsDependentRequest || (!string.IsNullOrWhiteSpace(AccessRequest.Remarks) && AccessRequest.Remarks.Equals("ATTESTATION REVIEW - NOAPPROVALREQUIRED")) )
                    show = false;
            }

            return show;
        }

        private void SetApproverForm()
        {
            if (!ShowApproverForm())
            {
                idHeadingApprovals.Visible = false;
                gridApprovals.Visible = false;
                return;
            }

            idHeadingApprovals.Visible = true;
            gridApprovals.Visible = true;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("adhocAccessRequestId", AdhocAccessRequest != null ? AdhocAccessRequest.Id.ToString() : ContextObjectId.Value.ToString());
            var approvers = DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByAdhocRequestId, parameter);

            Approvers = approvers;
            grdApproverList.MasterTableView.DetailTables[0].HierarchyDefaultExpanded = true;
            grdApproverList.Visible = true;
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

        private void SetApproverFormForEmployeeEvent(long eventTriggerId)
        {
            idHeadingApprovals.Visible = true;
            gridApprovals.Visible = true;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("triggerId", eventTriggerId.ToString());
            var approvers = DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByEmployeeRequestId, parameter);
            Approvers = approvers;
            grdApproverList.Visible = true;
            grdApproverList.DataSource = approvers.ToList();
            grdApproverList.DataBind();
        }

        protected void lnkViewNotification_Click(object sender, EventArgs e)
        {
            ViewNotification.RequestId = AccessRequest.Id;
            ViewNotification.BindNotifications();
            RadWinViewNotification.VisibleOnPageLoad = true;
        }

        protected void lnkViewAttachment_Click(object sender, EventArgs e)
        {
            RadWinViewAttachment.VisibleOnPageLoad = true;
        }

        protected void btnViewOk_Click(object sender, EventArgs e)
        {
            RadWinViewAttachment.VisibleOnPageLoad = false;
        }

        public void HideControls()
        {
            btnRefresh.Visible = false;
        }

        protected void lnkViewApprover_Click(object sender, EventArgs e)
        {
            ApprverChain.AdhocAccessRequest = AdhocAccessRequest;
            ApproverChain.VisibleOnPageLoad = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ApproverChain.VisibleOnPageLoad = false;
        }

        void ShowWindow()
        {
            var key = "showWindoww";
            string script = "Sys.Application.remove_load(showWindowWithdraw);Sys.Application.add_load(showWindowWithdraw);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindow()
        {
            var key = "closeWindoww";
            string script = "Sys.Application.remove_load(closeWindowWithdraw);Sys.Application.add_load(closeWindowWithdraw);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }
        void ShowWindowUsers()
        {
            var key = "ShowWindowUsers";
            string script = "Sys.Application.remove_load(showWindowUsers);Sys.Application.add_load(showWindowUsers);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindowUsers()
        {
            var key = "CloseWindowUsers";
            string script = "Sys.Application.remove_load(closeWindowUsers);Sys.Application.add_load(closeWindowUsers);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            CloseWindow();
        }

        protected void WithDrawnButton_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        protected void lnkGroupInfo_Command(object sender, CommandEventArgs e)
        {
            try
            {
                if (e.CommandArgument == null)
                    return;
                var lnkbtn = sender as LinkButton;
                var row = lnkbtn.NamingContainer as GridDataItem;
                var isGroupApproval = Convert.ToBoolean(row.GetDataKeyValue("IsGroupApproval").ToString());
                if (isGroupApproval)
                {
                    //var res = e.CommandArgument.ToString().Split('|');
                    //var isGroup = Convert.ToBoolean(res[0]);
                    //if (!isGroup || res.Length <= 1)
                    //    return;
                    var groupName = row.GetDataKeyValue("ApproverName").ToString();
                    //var groupName = res[1].Split('-')[1].Trim();
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("searchString", string.Empty);
                    parameter.Add("groupName", groupName);

                    Dictionary<long, string> users = DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>
                        (DomainApiAction.User.GetUsersByGroup, parameter);
                    if (users != null && users.Count > 0)
                    {
                        grdUsers.DataSource = users;
                        grdUsers.DataBind();
                        ShowWindowUsers();
                    }
                    else
                    {
                        ShowAlertMessage(AlertType.Error, "Group:{groupName} No members found");
                    }

                }

            }
            catch (Exception)
            {
            }
        }

        protected void btnCloseUserInfo_Click(object sender, EventArgs e)
        {
            CloseWindowUsers();
        }

        protected void grdApproverList_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                foreach (GridDataItem dataitem in grdApproverList.MasterTableView.Items)
                {
                    if (dataitem != null)
                    {
                        var status = (StatusType)dataitem.GetDataKeyValue("Action");
                        if ((status == StatusType.Approved || status == StatusType.AutoCompleted || status == StatusType.Rejected || status == StatusType.Pending || status == StatusType.Withdrawn))
                        {
                            dataitem.Expanded = true;
                        }
                        else
                        {
                            dataitem.Expanded = false;
                        }
                    }
                }
            }
        }

        protected void grdApproverList_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == RadGrid.ExpandCollapseCommandName)
            {
                foreach (GridItem item in e.Item.OwnerTableView.Items)
                {
                    if (item.Expanded && item != e.Item)
                    {
                        item.Expanded = false;
                    }
                }
            }
        }
    }
}