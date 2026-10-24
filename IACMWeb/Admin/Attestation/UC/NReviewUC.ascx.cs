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

namespace DI.Chakra.Web.Admin.Attestation.UC
{
    public partial class NReviewUC : UIBaseUC, IContextObjectRequired, IAccessControl
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
            
            if (!IsPostBack)
            {
                SessionUserId = SessionUser.UserId;
                if (Request.QueryString["Id"] != null)
                {
                    this.TaskId = Convert.ToInt64(Request.QueryString["Id"]);

                    DI.Chakra.Dto.Task task;
                    task = getCurrentTask(this.TaskId);
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
                    AttestationUploadUC.AllowAddComments = true;
                    AttestationUploadUC.CurrentUser = SessionUser.User.DisplayName;
                    AttestationUploadUC.CurrentUserName = SessionUser.User.UserName; 
                    if (task.IsGroupTask && !task.IsDelegated)
                        TaskActionUserId = task.AssignedTo.HasValue ? task.AssignedTo.Value : 0;
                    else
                        TaskActionUserId = task.ActionUserId.HasValue ? task.ActionUserId.Value : 0;

                    IsTaskCompleted = task.IsCompleted ? 1 : 0;
                    this.BindHeaderInfo(task);
                }
                IsUserView = true;
                RoleViewUC.Visible = false;
                UserViewUC.Visible = true;
            }

            if (!string.IsNullOrEmpty(CampaignName))
                this.Page.Title = string.Format("{0} - Review", CampaignName);

            MasterPage.PreferredPageTitle = this.Page.Title;
        }

        

        protected void btnOK_Click(object sender, EventArgs e)
        {
            AttestationDetails.VisibleOnPageLoad = false;
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == Infrastructure.Enums.ACLAction.Remark)
            {
                AttestationUploadUC.Initialize();
                AttestationUploadUC.BindComments();
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

                    //var RadGrid1 = new RadGrid();
                    //RadGrid1.ID = string.Format("{0}_{1}_{2}", "AttestationReport", TaskId , DateTime.Now.Date.ToString());
                    //RadGrid1.DataSource = temp;
                    //RadGrid1.Rebind();
                    //Page.Controls.Add(RadGrid1); // It will not add control in your page

                    //RadGrid1.ExportSettings.FileName = string.Format("{0}_{1}", "Attestation", DateTime.Now.Date.ToString());
                    //RadGrid1.ExportSettings.IgnorePaging = true;
                    //RadGrid1.ExportSettings.ExportOnlyData = true;
                    //RadGrid1.ExportSettings.OpenInNewWindow = true;
                    //RadGrid1.ExportSettings.HideStructureColumns = true;
                    //RadGrid1.ExportSettings.SuppressColumnDataFormatStrings = true;

                    //RadGrid1.GridLines = GridLines.Both;
                    //RadGrid1.MasterTableView.ExportToExcel();

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

                    //var RadGrid1 = new RadGrid();
                    //RadGrid1.ID = string.Format("{0}_{1}_{2}", "AttestationReport", TaskId , DateTime.Now.Date.ToString());
                    //RadGrid1.DataSource = temp;
                    //RadGrid1.Rebind();
                    //Page.Controls.Add(RadGrid1); // It will not add control in your page

                    //RadGrid1.ExportSettings.FileName = string.Format("{0}_{1}", "Attestation", DateTime.Now.Date.ToString());
                    //RadGrid1.ExportSettings.IgnorePaging = true;
                    //RadGrid1.ExportSettings.ExportOnlyData = true;
                    //RadGrid1.ExportSettings.OpenInNewWindow = true;
                    //RadGrid1.ExportSettings.HideStructureColumns = true;
                    //RadGrid1.ExportSettings.SuppressColumnDataFormatStrings = true;

                    //RadGrid1.GridLines = GridLines.Both;
                    //RadGrid1.MasterTableView.ExportToExcel();

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
                    MasterPage.ShowAlert(AlertType.Warning, "Please note that all items have to be reviewed to allow the review to be submitted. Please do save before submit.");
                }
            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.Revert)
            {
                this.RevertTaskInInterimStore();
                rebindGrid();
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
            btnConfirmSubmit.Enabled = false;
            btnCancelSubmit.Enabled = false;
            SubmitTask();
            Confirm.VisibleOnPageLoad = false;
        }

        protected void Cancel_Submit_Click(object sender, EventArgs e)
        {
            Confirm.VisibleOnPageLoad = false;
        }

        void SaveTaskInInterimStore()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", TaskId.ToString());
            parameter.Add("currentUser", SessionUser.User.UserName);
            var saved = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.SaveAttestationReview, parameter);
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
                    Response.Redirect("~/Report/Attestation/AttestationCampaigntaskDetail.aspx");
                else
                    Response.Redirect("~/Task/List.aspx");
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Action not completed. Please contact administrator for more information.");
            }
        }
        
    }
}