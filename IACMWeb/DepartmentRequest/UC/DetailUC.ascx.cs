using Application;
using System;
using System.Collections.Generic;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.DepartmentRequest.UC
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
                    return "~/DepartmentRequest/List.aspx";
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
                AdhocAccessRequest = DomainApiClient<Dto.AdHocAccessRequest>.GetById(ContextObjectId.Value, true);
                InfoUC.AdhocAccessRequest = AdhocAccessRequest;
                CommonTaskInfoUC.AdhocAccessRequest = AdhocAccessRequest;
                ApprverChain.AdhocAccessRequest = AdhocAccessRequest;
                Dictionary<string, string> adhocRequestId = new Dictionary<string, string>();
                adhocRequestId.Add("adhocAccessRequestId", Convert.ToString(AdhocAccessRequest.Id));
                AccessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>
                   (DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, adhocRequestId);
                WithDrawnButton.Visible = (IsAdminMode == IsAdminMode.Yes || AccessRequest.ProgressStatus == AccessRequestProgress.Completed || AccessRequest.ProgressStatus == AccessRequestProgress.Withdrawn || AccessRequest.ProgressStatus == AccessRequestProgress.RequestRejected || AccessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute) ? false : true;
                WithDrawnButton.Visible = false;
                SetApproverForm();
            }
            else
            {
                ApprverChain.Visible = false;
                WithDrawnButton.Visible = false;
            }
        }

        private void Withdraw()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("adhocAccessRequestId", Convert.ToString(AdhocAccessRequest.Id));
            Dto.AccessRequest accessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>
                (DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, parameter);
            accessRequest.Comments = ReasonField.Text;
            ReasonField.Text = "";
            accessRequest.RequestStatus = AccessRequestStatus.Withdrawn;
            accessRequest.ProgressStatus = AccessRequestProgress.Withdrawn;
            DomainApiClient<Dto.AccessRequest>.Save(accessRequest);
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

         
            Response.Redirect(RedirectURL);
        }


        private void Refresh()
        {
            if (ContextObjectId > 0)
            {
                AdhocAccessRequest = DomainApiClient<Dto.AdHocAccessRequest>.GetById(ContextObjectId.Value, true);
                InfoUC.AdhocAccessRequest = AdhocAccessRequest;
                WithDrawnButton.Visible = (IsAdminMode == IsAdminMode.Yes || AccessRequest.ProgressStatus == AccessRequestProgress.Completed || AccessRequest.ProgressStatus == AccessRequestProgress.Withdrawn || AccessRequest.ProgressStatus == AccessRequestProgress.RequestRejected || AccessRequest.ProgressStatus == AccessRequestProgress.ReadytoExecute) ? false : true;
                WithDrawnButton.Visible = false;
                SetApproverForm();
            }
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
            Withdraw();
        }
        protected void Back_Click(object sender, EventArgs e)
        {
            _sessionStatus = new SessionStatusImpl();
            if (_sessionStatus.RedirectFrom == "MyNotifications")
                Response.Redirect("~/Notification/Detail.aspx?Id=" + Request.QueryString["NotifyId"].ToString());
            if (_sessionStatus.RedirectFrom == "AlertCenter")
                Response.Redirect("~/AlertCenter/List.aspx");
            if (_sessionStatus.RedirectFrom == "Home")
                Response.Redirect("~/Home/Home.aspx");
            if (_sessionStatus.RedirectFrom == "AdminRequest")
                Response.Redirect("~/Prov/Request/List.aspx");
            if (_sessionStatus.RedirectFrom == "MyRequest")
                Response.Redirect("~/Request/List.aspx");
            if (_sessionStatus.RedirectFrom == "MyDepartmentRequest")
                Response.Redirect("~/DepartmentRequest/List.aspx");
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

        private void SetApproverForm()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("accessRequestId", AdhocAccessRequest != null ? AdhocAccessRequest.Id.ToString() : ContextObjectId.Value.ToString());
            var approvers = DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByRequestId, parameter);
            foreach (Dto.Presentation.PresentationApprovalChain Approver in approvers)
            {
                //if (Approver.Remarks != null)
                //    Approver.Remarks = Approver.Remarks.Trim().Replace("\n", "<br/>");
            }
            grdApprover.Visible = false;
            grdApproverList.Visible = true;
            grdApproverList.DataSource = approvers;
            grdApproverList.DataBind();
        }
       
    }
}