using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Collections.Generic;
using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Web;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.Admin.Attestation
{
    public partial class NReview : UIBasePage, IContextObjectRequired, IAccessControl
    {
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        private bool IsUserView
        {
            get
            {
                if (ViewState["IsUserView"] != null)
                {
                    return Convert.ToBoolean(ViewState["IsUserView"]);
                }
                return true;
            }
            set
            {
                ViewState["IsUserView"] = value;
            }
        }

        public long TaskId
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

        public long TaskActionUserId
        {
            get
            {
                if (ViewState["TaskActionUserId"] != null)
                {
                    return Convert.ToInt64(ViewState["TaskActionUserId"]);
                }
                return 0;
            }
            set
            {
                ViewState["TaskActionUserId"] = value;
            }
        }

        public int IsTaskCompleted
        {
            get
            {
                if (ViewState["IsTaskCompleted"] != null)
                {
                    return Convert.ToInt16(ViewState["IsTaskCompleted"]);
                }
                return 0;
            }
            set
            {
                ViewState["IsTaskCompleted"] = value;
            }
        }

        public long SessionUserId
        {
            get
            {
                if (ViewState["SessionUserId"] != null)
                {
                    return Convert.ToInt64(ViewState["SessionUserId"]);
                }
                return 0;
            }
            set
            {
                ViewState["SessionUserId"] = value;
            }
        }

        public string CampaignName
        {
            get
            {
                if (ViewState["CampaignName"] != null)
                {
                    return Convert.ToString(ViewState["CampaignName"]);
                }
                return "";
            }
            set
            {
                ViewState["CampaignName"] = value;
            }
        }

        public long HasAttachments
        {
            get
            {
                if (ViewState["HasAttachments"] != null)
                {
                    return Convert.ToInt32(ViewState["HasAttachments"]);
                }
                return 0;
            }
            set
            {
                ViewState["HasAttachments"] = value;
            }
        }
        public long CampaignId
        {
            get
            {
                if (ViewState["CampaignId"] != null)
                {
                    return Convert.ToInt64(ViewState["CampaignId"]);
                }
                return 0;
            }
            set
            {
                ViewState["CampaignId"] = value;
            }
        }

        Dto.Task getCurrentTask(long taskId)
        {
            return DomainApiClient<Dto.Task>.GetById(this.TaskId);
        }

        Dto.AttestationCampaign getAttestationCampaign(long taskId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Dto.Task task = getCurrentTask(this.TaskId);
            if (task != null)
            {
                parameters.Add("campaignId", task.AttestationCampaignId.ToString());
                return DomainApiClient<Dto.AttestationCampaign>.GetByAction<Dto.AttestationCampaign>
                 (DomainApiAction.Attestation.AttestationCampaign.GetByCampaignId, parameters);
            }
            return null;
        }
        long getAttestationCampaignAttachmentCount(long campaignId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Dto.Task task = getCurrentTask(this.TaskId);
            if (task != null)
            {
                parameters.Add("attestationCampaignId", campaignId.ToString());
                return DomainApiClient<Dto.AttestationCampaignAttachment>.GetByAction<long>
                 (DomainApiAction.Attestation.AttestationCampaign.GetAttachmentCount, parameters);
            }
            return 0;
        }

        private void BindHeaderInfo(DI.Chakra.Dto.Task task)
        {
            Dictionary<string, string> parameterId = new Dictionary<string, string>();
            parameterId.Add("id", Convert.ToString(task.AttestationId));
            Dto.Attestation attestation = DomainApiClient<Dto.Attestation>.GetByAction<Dto.Attestation>
                (DomainApiAction.Attestation.GetByIdIncludingAttestationProcesses, parameterId);
            var currentApprover = attestation.WorkflowConfiguration.WorkflowConfigurationDetails.
                FirstOrDefault(item => item.SequenceNumber == task.WorkflowStateId);
            //this.lnkAttName.Text = attestation.AttestationName;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("workflowInstanceId", task.WorkflowInstanceId.ToString());
            parameter.Add("workflowStateId", task.WorkflowStateId.ToString());
            var previousApprovers = DomainApiClient<Dto.Presentation.PresentationPreviousApprovers>.
                GetByAction<List<Dto.Presentation.PresentationPreviousApprovers>>
                (DomainApiAction.Attestation.Presentation.PresentationPreviousApprovers.GetByWorkflowInstanceId, parameter);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            UserViewUC.SaveButtonClick += UserViewUC_SaveButtonClick;
            UserViewUC.SaveAndSubmitButtonClick += UserViewUC_SaveAndSubmitButtonClick;
            if (!IsPostBack)
            {
                if (Request.QueryString["CampaignId"] != null)
                {
                    CampaignId = Convert.ToInt64(Request.QueryString["CampaignId"]);
                }
                SessionUserId = SessionUser.UserId;
                if (Request.QueryString["Id"] != null)
                {

                    this.TaskId = Convert.ToInt64(Request.QueryString["Id"]);

                    DI.Chakra.Dto.Task task;
                    task = getCurrentTask(this.TaskId);

                    var parame = new Dictionary<string, string>();
                    parame.Add("id", task.AttestationId.ToString());
                    var isFLins = DomainApiClient<Dto.Attestation>.GetByAction<string>("GetFlinsAttestationMessage", parame);
                    this.lblFlinsMessage.Visible = false;
                    divSapMessage.Visible = false;
                    this.lblFlinsMessage.Text = null;
                    if (!string.IsNullOrWhiteSpace(isFLins))
                    {
                        this.lblFlinsMessage.Visible = true;
                        this.lblFlinsMessage.Text = isFLins;
                        divSapMessage.Visible = true;
                    }
                    btnDownloadTCode.Visible = false;
                    divTCodeSection.Visible = false;
                    parame.Clear();
                    parame.Add("id", task.AttestationCampaignId.ToString());
                    var sapSystems = DomainApiClient<Dto.Attestation>.GetByAction<Dictionary<int, string>>("GetSAPApplicationTypeFromAttestation", parame);
                    if (sapSystems != null && sapSystems.Count > 0)
                    {
                        btnDownloadTCode.Visible = true;
                        divTCodeSection.Visible = true;
                        ddlApplicationType.Items.Clear();
                        foreach (var item in sapSystems)
                        {
                            ddlApplicationType.Items.Add(new ListItem(item.Value.ToString(), item.Key.ToString()));
                        }
                    }
                    var campaign = getAttestationCampaign(this.TaskId);
                    CampaignName = campaign.CampaignName;
                    HasAttachments = 0;
                    if (string.IsNullOrEmpty(campaign.Comments))
                    {
                        HasAttachments = getAttestationCampaignAttachmentCount(campaign.Id);
                    }
                    else
                        HasAttachments = 1;
                    AttestationUploadUC.TaskId = task.Id;
                    AttestationUploadUC.CampaignId = task.AttestationCampaignId.HasValue ? (long)task.AttestationCampaignId : 0;
                    AttestationUploadUC.IsTaskCompleted = task.IsCompleted;
                    Button2.Enabled = !task.IsCompleted;
                    AttestationUploadUC.AllowAddComments = true;
                    AttestationUploadUC.CurrentUser = SessionUser.User.DisplayName;
                    AttestationUploadUC.CurrentUserName = SessionUser.User.UserName;
                    if (task.IsGroupTask && !task.IsDelegated)
                        TaskActionUserId = task.AssignedTo.HasValue ? task.AssignedTo.Value : 0;
                    else
                        TaskActionUserId = task.ActionUserId.HasValue ? task.ActionUserId.Value : 0;

                    bool isEditable = true;
                    var actionUserId = task.ActionUserId.HasValue ? task.ActionUserId.Value : 0;
                    if (task.IsGroupTask && !task.IsDelegated && task.AssignedTo.HasValue)
                        actionUserId = (long)task.AssignedTo;

                    if (task.IsGroupTask && task.ActionGroupId.HasValue && TaskActionUserId == 0)
                    {
                        Dictionary<string, string> param = new Dictionary<string, string>();
                        param.Add("GroupID", task.ActionGroupId.Value.ToString());
                        param.Add("userId", SessionUser.UserId.ToString());
                        var isGroupUser = DomainApiClient<Dto.Group>.GetByAction<bool>("CheckUserGroup", param);
                        if (isGroupUser)
                        {
                            actionUserId = SessionUser.UserId;
                        }
                    }
                    if (actionUserId != SessionUser.UserId || task.IsCompleted || task.Action == StatusType.Approved || task.Action == StatusType.AutoCompleted)
                        isEditable = false;
                    AllRetain.Enabled = false;
                    AllRevoke.Enabled = false;

                    if (isEditable)
                    {
                        AllRetain.Enabled = true;
                        AllRevoke.Enabled = true;
                    }

                    //if (campaign != null && (campaign.Attestation != null && campaign.Attestation.AttestationType == AttestationType.Privileged))
                    //{
                    //    AllRetain.Enabled = false;
                    //    AllRevoke.Enabled = false;
                    //}
                    IsTaskCompleted = task.IsCompleted ? 1 : 0;
                    this.BindHeaderInfo(task);
                    bool isAllowedToView = MasterConfigHelper.ShowUserComparisionLinkForOnlyOtherSystem;
                    if (isAllowedToView)
                    {
                        var exs = IsOtherSystemEndpointExists(this.TaskId);
                        if (!exs)
                        {
                            divComparision.Visible = false;
                        }
                        else
                        {
                            divComparision.Visible = true;
                            if (MasterConfigHelper.ShowReviewAttestationNotes)
                            {
                                lblComparisionNotes.Text = HttpUtility.HtmlDecode(MasterConfigHelper.ReviewAttestationNotes ?? "");
                                lblComparisionNotes.Visible = true;
                            }
                            else
                            {
                                lblComparisionNotes.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        divComparision.Visible = true;
                        if (MasterConfigHelper.ShowReviewAttestationNotes)
                        {
                            lblComparisionNotes.Text = HttpUtility.HtmlDecode(MasterConfigHelper.ReviewAttestationNotes ?? "");
                            lblComparisionNotes.Visible = true;
                        }
                        else
                        {
                            lblComparisionNotes.Visible = false;
                        }
                    }
                }
                IsUserView = true;
                RoleViewUC.Visible = false;
                UserViewUC.Visible = true;
                UserViewUC.EnableAction = (IsTaskCompleted == 1 || SessionUserId != TaskActionUserId) ? false : true;
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ValidationAllowed", "validateActionAllowed();", true);

            if (!string.IsNullOrEmpty(CampaignName))
                this.Title = string.Format("{0} - Review", CampaignName);

            MasterPage.PreferredPageTitle = this.Title;
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            AttestationDetails.VisibleOnPageLoad = false;
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if ((e.ActionType != Infrastructure.Enums.ACLAction.ExportFlatReport) && (e.ActionType != Infrastructure.Enums.ACLAction.ExportHierarchyReport)
               && (e.ActionType != Infrastructure.Enums.ACLAction.AttestationDetail) && (e.ActionType != Infrastructure.Enums.ACLAction.Remark) && IsTaskCompleted > 0)
            {
                return;
            }
            if (e.ActionType == Infrastructure.Enums.ACLAction.Remark)
            {

                AttestationUploadUC.Initialize();
                AttestationUploadUC.BindComments();
                bool enableRemarks = true;
                if (IsTaskCompleted > 0 || (SessionUserId != TaskActionUserId))
                {
                    enableRemarks = false;
                }
                AttestationUploadUC.AllowAddComments = enableRemarks;
                Button2.Enabled = enableRemarks;
                AttestationUpload.VisibleOnPageLoad = true;
                //string script = "function f(){$find(\"" + AttestationUpload.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
                return;
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.AttestationDetail)
            {
                AttestationDetailUC.TaskId = this.TaskId;
                AttestationDetailUC.Initialize();
                AttestationDetails.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.ExportFlatReport)
            {
                try
                {
                    HttpContext.Current.Session["ExportStatus"] = "false";
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", this.TaskId.ToString());
                    parameter.Add("isInitialLoad", "true");
                    parameter.Add("currentUser", "admin");

                    var excelFileOutput = DomainApiClient<Dto.Presentation.PresentationExportAttestationReportForUI>.GetByAction<Dto.Presentation.PresentationExportAttestationReportForUI>
                                (DomainApiAction.Tasks.Task.GetAttestationTaskReviewForExportFlat, parameter);

                    if (excelFileOutput.excelFile != null)
                    {
                        HttpResponse response = HttpContext.Current.Response;
                        response.Clear();
                        response.ClearContent();
                        response.ClearHeaders();
                        response.Buffer = true;
                        response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("{0}_{1}_Flat.xlsx", "Attestation", DateTime.Now.Date.ToString()));
                        response.BinaryWrite(excelFileOutput.excelFile);
                        response.End();
                    }
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    HttpContext.Current.Session["ExportStatus"] = "true";
                }
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.ExportHierarchyReport)
            {
                try
                {
                    HttpContext.Current.Session["ExportStatus"] = "false";
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", this.TaskId.ToString());
                    parameter.Add("isInitialLoad", "true");
                    parameter.Add("currentUser", "admin");

                    var excelFileOutput = DomainApiClient<Dto.Presentation.PresentationExportAttestationReportForUI>.GetByAction<Dto.Presentation.PresentationExportAttestationReportForUI>
                                (DomainApiAction.Tasks.Task.GetAttestationTaskReviewForExportHierarchy, parameter);

                    if (excelFileOutput.excelFile != null)
                    {
                        HttpResponse response = HttpContext.Current.Response;
                        response.Clear();
                        response.ClearContent();
                        response.ClearHeaders();
                        response.Buffer = true;
                        response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("{0}_{1}_Hierarchy.xlsx", "Attestation", DateTime.Now.Date.ToString()));
                        response.BinaryWrite(excelFileOutput.excelFile);
                        response.End();
                    }
                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    HttpContext.Current.Session["ExportStatus"] = "true";
                }
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.UserView)
            {
                IsUserView = true;
                RoleViewUC.Visible = false;
                UserViewUC.Visible = true;
                rebindGrid();
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.AccessView)
            {
                IsUserView = false;
                UserViewUC.Visible = false;
                RoleViewUC.Visible = true;
                rebindGrid();
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.Save)
            {
                this.SaveTaskInInterimStore();
                rebindGrid();
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.Submit)
            {
                if (CanSubmitTask())
                {
                    btnConfirmSubmit.Enabled = true;
                    btnCancelSubmit.Enabled = true;
                    Confirm.VisibleOnPageLoad = true;
                    //SubmitTask();
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please perform save before submit and ensure all accesses are reviewed.");
                }
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.Revert)
            {
                RevertConfirm.VisibleOnPageLoad = true;

            }
        }

        protected void Save_Comment_Upload_Click(object sender, EventArgs e)
        {
            AttestationUploadUC.SaveData();
            AttestationUpload.VisibleOnPageLoad = false;
        }

        protected void Cancel_Comment_Upload_Click(object sender, EventArgs e)
        {
            AttestationUpload.VisibleOnPageLoad = false;
        }

        protected void Confirm_Submit_Click(object sender, EventArgs e)
        {
            bool isSaved = true;
            if (CanSubmitTask() == false)
            {
                isSaved = SaveTaskInInterimStore();
            }
            btnConfirmSubmit.Enabled = false;
            btnCancelSubmit.Enabled = false;
            SubmitTask();
            Confirm.VisibleOnPageLoad = false;
        }

        protected void Cancel_Submit_Click(object sender, EventArgs e)
        {
            Confirm.VisibleOnPageLoad = false;
        }

        bool SaveTaskInInterimStore()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", TaskId.ToString());
            parameter.Add("currentUser", SessionUser.User.UserName);
            var saved = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.SaveAttestationReview, parameter);
            return saved;
        }

        bool CanSubmitTask()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", TaskId.ToString());
            var canSubmit = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.CanSubmitTask, parameter);
            return canSubmit;
        }

        void rebindGrid()
        {
            if (UserViewUC.Visible)
                this.UserViewUC.Rebind();
            else if (RoleViewUC.Visible)
                this.RoleViewUC.Rebind();
        }

        void RevertTaskInInterimStore()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", TaskId.ToString());
            parameter.Add("currentUser", SessionUser.User.UserName);
            var saved = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.RevertAttestationReview, parameter);
        }

        void SubmitTask()
        {
            bool status;
            var task = getCurrentTask(this.TaskId);
            Dto.Presentation.PresentationWorkflowCriteriaBase _criteria = new Dto.Presentation.ApprovalActionCriteria()
            {
                RequestType = RequestType.Attestation,
                TaskId = this.TaskId,
                WorkflowInstanceId = task.WorkflowInstanceId
            };
            status = DomainApiClient<Object>.GetByAction<bool>("WorkflowController", DomainApiAction.WorkflowProcessor.SubmitWorkflowCriteria, null, _criteria);


            if (status)
            {
                SessionStatusImpl _sessionStatus = new SessionStatusImpl();
                if (_sessionStatus.RedirectFrom == "Tasks")
                    Response.Redirect("~/Prov/Task/List.aspx");
                else if (_sessionStatus.RedirectFrom == "Home")
                {
                    Response.Redirect("~/Home/Home.aspx");
                }
                else if (_sessionStatus.RedirectFrom == "GroupTask")
                    Response.Redirect("~/Task/GroupTasks.aspx");
                else if (_sessionStatus.RedirectFrom == "AttestationCampaigntaskDetail")
                    Response.Redirect($"~/Report/Attestation/AttestationCampaigntaskDetail.aspx?ContextId={CampaignId}");
                else
                    Response.Redirect("~/Task/List.aspx");
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Action not completed. Please contact administrator for more information.");
            }
        }

        protected void AllRetain_CheckedChanged(object sender, EventArgs e)
        {
            //if (UserViewUC.ShowOnlyRevokeItem)
            //{
            //    MasterPage.ShowAlert(AlertType.Warning, "Please clear Show All Revoke Item");
            //    return;
            //}
            //Dictionary<string, string> parameter = new Dictionary<string, string>();
            //parameter.Add("value", true.ToString());
            //parameter.Add("accessType", AccessType.Approve.ToString());
            //parameter.Add("taskId", this.TaskId.ToString());
            //parameter.Add("currentUser", SessionUser.User.UserName);
            //parameter.Add("taskActionUserId", TaskActionUserId.ToString());
            //var temp = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
            //            (DomainApiAction.Tasks.Task.UpdateAttestationReviewAll, parameter);

            //rebindGrid();
        }

        protected void AllRevoke_CheckedChanged(object sender, EventArgs e)
        {
            //if (UserViewUC.ShowOnlyRevokeItem)
            //{
            //    MasterPage.ShowAlert(AlertType.Warning, "Please clear Show All Revoke Item");
            //    return;
            //}
            //Dictionary<string, string> parameter = new Dictionary<string, string>();
            //parameter.Add("value", true.ToString());
            //parameter.Add("accessType", AccessType.Revoke.ToString());
            //parameter.Add("taskId", this.TaskId.ToString());
            //parameter.Add("currentUser", SessionUser.User.UserName);
            //parameter.Add("taskActionUserId", TaskActionUserId.ToString());
            //var temp = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
            //            (DomainApiAction.Tasks.Task.UpdateAttestationReviewAll, parameter);

            //rebindGrid();
        }

        protected void btnRevertConfirmSubmit_Click(object sender, EventArgs e)
        {
            this.RevertTaskInInterimStore();
            rebindGrid();
            RevertConfirm.VisibleOnPageLoad = false;
        }

        protected void btnRevertCancelSubmit_Click(object sender, EventArgs e)
        {
            RevertConfirm.VisibleOnPageLoad = false;
        }
        private void UserViewUC_SaveAndSubmitButtonClick(object sender, EventArgs e)
        {
            btnConfirmSubmit.Enabled = true;
            btnCancelSubmit.Enabled = true;
            Confirm.VisibleOnPageLoad = true;

        }

        private void UserViewUC_SaveButtonClick(object sender, EventArgs e)
        {
            this.SaveTaskInInterimStore();
            rebindGrid();
        }

        protected void btnDownloadTCode_Click(object sender, EventArgs e)
        {
            try
            {
                //if(ddlApplicationType.Items.Count > 0 && ddlApplicationType.SelectedIndex > 0)
                {
                    var parameter = new Dictionary<string, string>();
                    parameter.Add("applicationType", ddlApplicationType.SelectedValue);
                    var data = DomainApiClient<Dto.Attestation>
                        .GetByAction<IDMSHttpResponse<byte[]>>("DownloadTCodeReport", parameter);
                    if (data != null)
                    {
                        Session["ExportStatus"] = "true";
                        HttpResponse response = Response;
                        response.Clear();
                        response.ClearContent();
                        response.ClearHeaders();
                        response.Buffer = true;
                        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        response.AddHeader("Content-Length", data.Data.Length.ToString());
                        response.AddHeader("Content-Disposition", "attachment;filename=" + $"{ddlApplicationType.SelectedItem.Text.GetValidFileName()}_RoleToTCode.xls");
                        response.BinaryWrite(data.Data);
                        response.End();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
            }
            catch
            {
            }
        }

        protected void ddlApplicationType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        protected void lnkAttestationComparision_Click(object sender, EventArgs e)
        {
            Response.Redirect($"AttestationUserComparision.aspx?SelectedtaskId={this.TaskId}&previousurl={HttpUtility.UrlEncode(HttpContext.Current.Request.Url.AbsoluteUri)}");
        }

        bool IsOtherSystemEndpointExists(long taskId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            bool result = false;
            if (taskId > 0)
            {
                parameters.Add("taskId", taskId.ToString());
                result = DomainApiClient<Dto.Attestation>.GetByAction<bool>
                 (DomainApiAction.Attestation.AttestationCampaign.DoesExistsOtherSystemEndpoint, parameters);
            }
            return result;
        }
        
        protected void imgAttestationComparision_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect($"AttestationUserComparision.aspx?SelectedtaskId={this.TaskId}&previousurl={HttpUtility.UrlEncode(HttpContext.Current.Request.Url.AbsoluteUri)}");
        }

        //protected void btnTemp_Click(object sender, EventArgs e)
        //{
        //    var parameter = new Dictionary<string, string>();
        //    parameter.Add("applicationType", ddlApplicationType.SelectedValue);
        //    var data = DomainApiClient<Dto.Attestation>
        //        .GetByAction<IDMSHttpResponse<byte[]>>("DownloadTCodeReport", parameter);

        //    if (data != null)
        //    {
        //        Session["ExportStatus"] = "true";
        //        HttpResponse response = Response;
        //        response.Clear();
        //        response.ClearContent();
        //        response.ClearHeaders();
        //        response.Buffer = true;
        //        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        response.AddHeader("Content-Length", data.Data.Length.ToString());
        //        response.AddHeader("Content-Disposition", "attachment;filename=" + $"{ddlApplicationType.SelectedItem.Text}_RoleToTCode.xls");
        //        response.BinaryWrite(data.Data);
        //        response.End();
        //        HttpContext.Current.ApplicationInstance.CompleteRequest();
        //    }
        //}
    }
}