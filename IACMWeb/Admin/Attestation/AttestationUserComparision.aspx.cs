using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Admin.Attestation
{
    public partial class AttestationUserComparision : B5UIBasePage, IContextObjectRequired, IAccessControl
    {
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {

        }
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
        public bool isEditable
        {
            get
            {
                if (ViewState["isEditable"] != null)
                {
                    return Convert.ToBoolean(ViewState["isEditable"]);
                }
                return false;
            }
            set
            {
                ViewState["isEditable"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                int sessionTimeoutMinutes = HttpContext.Current.Session.Timeout;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SessionTimeout", $"var sessionTimeout = {sessionTimeoutMinutes};", true);
                //btnSave.Attributes.Add("onclick", "return false");
                hdnUsername.Value = SessionUser.User.UserName;
                //TODO: DHASA - Define
                entitlementNotes.Text = HttpUtility.HtmlDecode(MasterConfigHelper.UserAttestationComparisionNotes);

                DILog.UILogger.WriteLog(SessionUser.UserId, SessionUser.User.DisplayName, LogLevelType.Debug, $"AttestationUserComparision",null);
                this.hdnTaskId.Value = Request.QueryString["SelectedtaskId"].ToString();
                TaskId = Convert.ToInt64(this.hdnTaskId.Value);
                var task = getCurrentTask(this.TaskId);

                if (task.IsGroupTask && !task.IsDelegated)
                    TaskActionUserId = task.AssignedTo.HasValue ? task.AssignedTo.Value : 0;
                else
                    TaskActionUserId = task.ActionUserId.HasValue ? task.ActionUserId.Value : 0;

                this.hdnActionUserId.Value = TaskActionUserId.ToString();
                var msg = MasterConfigHelper.AttestationServiceAccountWarningMessage;
                if (!string.IsNullOrWhiteSpace(msg))
                    lblConfirmMessageSpecialAccount.Text = msg;
                else
                {
                    lblConfirmMessageSpecialAccount.Text = "Are you sure you want to revoke account?";
                }

                lblConfirmMessageSpecialAccountEntitlement.Text = lblConfirmMessageSpecialAccount.Text;
                isEditable = true;

                if (TaskActionUserId != SessionUser.UserId || task.IsCompleted || task.Action == StatusType.Approved || task.Action == StatusType.AutoCompleted)
                {
                    isEditable = false;
                    string script = @"<script type='text/javascript'>
                        document.getElementById('btnSave').style.display = 'none';
                        document.getElementById('fbtnSave').style.display = 'none';
                        document.getElementById('btnRevert').style.display = 'none';
                        document.getElementById('fbtnRevert').style.display = 'none';
                        document.getElementById('btnSaveAndSubmit').style.display = 'none';
                        document.getElementById('fbtnSaveAndSubmit').style.display = 'none';
                    </script>";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "HideSaveAndSubmitButton", script);
                }
                else
                {
                    string script = @"<script type='text/javascript'>
                        document.getElementById('btnSaveAndSubmit').style.display = '';
                        document.getElementById('fbtnSaveAndSubmit').style.display = '';
                    </script>";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "HideSaveAndSubmitButton", script);
                }
                hdnIsEditable.Value = (isEditable ? 1 : 0).ToString();

                var campaign = getAttestationCampaign(this.TaskId);
                AssignBackUrl(campaign.Id);
                CampaignName = campaign.CampaignName;
                hdnCampaignName.Value = CampaignName;
                
                if (!string.IsNullOrEmpty(CampaignName))
                    this.Title = string.Format("{0} - Comparision", CampaignName);
            }
        }
        private void AssignBackUrl(long campaignId)
        {
            SessionStatusImpl _sessionStatus = new SessionStatusImpl();
            var url = ResolveUrl("~/Home//Home.aspx");
            if (MasterConfigHelper.UseOldReviewPage)
            {

                url = $"~/Admin/Attestation/NReview.aspx?Id={this.TaskId}&IsAdminMode=No";
                if (_sessionStatus.RedirectFrom == "AttestationCampaigntaskDetail" || _sessionStatus.RedirectFrom == "AttestationCampaignStatus")
                    url =$"{url}&CampaignId={campaignId}";
                url = ResolveUrl($"{url}");
            }
            else
            {
                if (_sessionStatus.RedirectFrom == "Tasks")
                {
                    string source = this.Request.QueryString["source"] ?? "";
                    string taskStatus = this.Request.QueryString["ddltaskstatus"] ?? "";
                    string showAllTasks = this.Request.QueryString["showalltask"] ?? "";
                    string fromDate = this.Request.QueryString["fromDate"] ?? "";
                    string toDate = this.Request.QueryString["toDate"] ?? "";
                    string requestFor = this.Request.QueryString["requestFor"] ?? "";
                    string requestForName = this.Request.QueryString["requestForName"] ?? "";
                    string epId = this.Request.QueryString["endpointsystemId"] ?? "";

                    url = ResolveUrl($"~//Prov//Task//List.aspx?ddltaskstatus={taskStatus}&showalltask={showAllTasks}&toDate={toDate}&fromDate={fromDate}&requestFor={requestFor}&requestForName={requestForName}&endpointsystemId={epId}");
                    //Response.Redirect("~//Prov//Task//List.aspx");
                }
                else if (_sessionStatus.RedirectFrom == "Home")
                {
                    url = ResolveUrl("~//Home//Home.aspx");
                }
                else if (_sessionStatus.RedirectFrom == "GroupTask")
                    url = ResolveUrl("~//Task//GroupTasks.aspx");
                else if (_sessionStatus.RedirectFrom == "AttestationCampaigntaskDetail")
                    url = ResolveUrl("~//Report//Attestation//AttestationCampaigntaskDetail.aspx?ContextId=" + campaignId.ToString());
                else if (_sessionStatus.RedirectFrom == "AttestationCampaignStatus")
                    url = ResolveUrl("~/Report/Attestation/AttestationCampaignStatus.aspx?ContextId=" + campaignId.ToString());
                else if (_sessionStatus.RedirectFrom == "AttestationOverallStatus")
                    url = ResolveUrl("~/Report/Attestation/AttestationOverallStatus.aspx");
                else if (_sessionStatus.RedirectFrom == "AttestationCampaignReport")
                {
                    url = ResolveUrl("~/Report/Attestation/AttestationCampaignReport.aspx");
                }
                else
                {
                    string source = this.Request.QueryString["source"] ?? "";
                    string taskStatus = this.Request.QueryString["ddltaskstatus"] ?? "";
                    string showAllTasks = this.Request.QueryString["showalltask"] ?? "";
                    string fromDate = this.Request.QueryString["fromDate"] ?? "";
                    string toDate = this.Request.QueryString["toDate"] ?? "";
                    string requestFor = this.Request.QueryString["requestFor"] ?? "";
                    string requestForName = this.Request.QueryString["requestForName"] ?? "";
                    string epId = this.Request.QueryString["endpointsystemId"] ?? "";

                    url = ResolveUrl($"~//Task//List.aspx?ddltaskstatus={taskStatus}&showalltask={showAllTasks}&toDate={toDate}&fromDate={fromDate}&requestFor={requestFor}&requestForName={requestForName}&endpointsystemId={epId}");
                    //Response.Redirect("~//Task//List.aspx");
                }
            }
            hdnBackUrl.Value = url;
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
        Dto.Task getCurrentTask(long taskId)
        {
            return DomainApiClient<Dto.Task>.GetById(this.TaskId);
        }

        [WebMethod]
        public static string GetAttestationUserComparisionData(long taskId, int draw, int start, int length, DataTableSearch search = null)
        {
            if (length > 50) length = 50;

            var param = new Dictionary<string, string>();
            param.Add("taskId", taskId.ToString());
            param.Add("draw", draw.ToString());
            param.Add("start", start.ToString());
            param.Add("length", length.ToString());
            //param.Add("search", search != null && !string.IsNullOrWhiteSpace(search.Value) ? search.Value : "");
            var previousApprovers = DomainApiClient<Dto.AttestationSnapshotAction>.
                GetByAction<PresentationGridPagedData<ViewUserEntitlementData>>
                ("GetAttestationUserComparisionData", param, search);
            var response = new
            {
                draw = draw,
                recordsTotal = previousApprovers.TotalCount,
                recordsFiltered = previousApprovers.TotalCount, // In this example, no filtering is applied
                data = previousApprovers.Data
            };
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string jsonResponse = serializer.Serialize(response);

            return jsonResponse;

        }

        //protected void lnkAttestationComparision_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect(Request.QueryString["previousurl"].ToString());
        //}
    }
}