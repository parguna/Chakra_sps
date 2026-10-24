using AjaxControlToolkit;
using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.Attestation
{
    public partial class AttestationCampaignStatus : ReportBasePage, IContextObjectRequired, IContextObjectEnabled
    {
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        public List<PresentationAttetstationCampaign> CampaignList
        {
            get
            {
                if (ViewState["CampaignList"] == null)
                    return new List<PresentationAttetstationCampaign>();
                return ViewState["CampaignList"] as List<PresentationAttetstationCampaign>;
            }
            set
            {
                ViewState["CampaignList"] = value;
            }
        }
        #region Members
        private const string V_EntityID = "V_EntityID";
        private const string V_RedirectFrom = "V_RedirectFrom";
        private const string V_RedirectTo = "V_RedirectTo";
        private const string V_ParentCampaignId = "V_ParentCampaignId";
        #endregion

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return -1;
                return (long)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
                CampaignStatusGrid.Rebind();
            }
        }
        public long ParentCampaignId
        {
            get
            {
                if (ViewState[V_ParentCampaignId] == null)
                    return -1;
                return (long)ViewState[V_ParentCampaignId];
            }
            set
            {
                ViewState[V_ParentCampaignId] = value;
            }
        }

        public string RedirectFrom
        {
            get
            {
                if (ViewState[V_RedirectFrom] == null)
                    return "AttestationOverallStatus";
                return (string)ViewState[V_RedirectFrom];
            }
            set
            {
                ViewState[V_RedirectFrom] = value;
                SessionUser.RedirectFrom = value;
            }
        }

        public string RedirectTo
        {
            get
            {
                if (ViewState[V_RedirectTo] == null)
                    return "AttestationCampaignTaskDetail";
                return (string)ViewState[V_RedirectTo];
            }
            set
            {
                ViewState[V_RedirectTo] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["ContextId"] != null)
                {
                    long ContextObjectId = Convert.ToInt64(Request.QueryString["ContextId"]);
                    EntityId = ContextObjectId;
                    var campaign = DomainApiClient<Dto.AttestationCampaign>.GetById(EntityId);
                    if (campaign != null && campaign.ParentCampaign.HasValue)
                        ParentCampaignId = campaign.ParentCampaign.Value;
                    if ( ParentCampaignId > -1)
                    {
                        EntityId = ParentCampaignId;
                        ContextObjectId = EntityId;
                    }
                    if (string.IsNullOrEmpty(RedirectFrom))
                        RedirectFrom = SessionUser.RedirectFrom;
                }
            }

            this.MasterPage.ActionClicked += ActionClicked;             
            SessionUser.RedirectFrom = "AttestationCampaignStatus";
        }
        
        protected void CampaignStatusGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("AttestationCampaignId", EntityId.ToString());
            CampaignList = DomainApiClient<Dto.Presentation.PresentationAttetstationCampaign>
            .GetByAction<List<Dto.Presentation.PresentationAttetstationCampaign>>(DomainApiAction.Attestation.PresentationAttetstationCampaign.GetCampaignStatus, parameter).ToList();
            CampaignStatusGrid.DataSource = CampaignList;
        }

        protected void btnPageCancel_Click(object sender, EventArgs e)
        {
            SessionStatusImpl _sessionStatus = new SessionStatusImpl();
            if (RedirectFrom == "AttestationOverallStatus" || _sessionStatus.RedirectFrom == "AttestationOverallStatus")
                Response.Redirect("~/Admin/Attestation/AttestationOverallStatus.aspx");
            else if (RedirectFrom == "AttestationCampaignReport" || _sessionStatus.RedirectFrom == "AttestationCampaignReport")
            {
                Response.Redirect("~/Report/Attestation/AttestationCampaignReport.aspx");
            }
            else
                Response.Redirect("~/Admin/Attestation/AttestationOverallStatus.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //report = RptSchd.GetReportingSchedule();
                //scheduler = RptSchd.Scheduler;
                //attributeParameter.Add("EndPointReport", "Users");
                //if (Save())
                //{
                //    RptSchd.ClearpopupData();
                //    usrModal.VisibleOnPageLoad = false;
                //}
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Schduler is not filled with schedule values");
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            usrModal.VisibleOnPageLoad = false;
            //RptSchd.InitializeSched();
        }

        protected void btnDetailCancel_Click(object sender, EventArgs e)
        {
            CampaignDetail.VisibleOnPageLoad = false;
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = CampaignList;
                if (data.Count > 0)
                {

                    if (!string.IsNullOrWhiteSpace(CampaignStatusGrid.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(CampaignStatusGrid.MasterTableView.FilterExpression).ToList();
                    if (CampaignStatusGrid.MasterTableView.SortExpressions != null && CampaignStatusGrid.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(CampaignStatusGrid.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "AttestationCampaignStatus");
                }
            }
            else if (e.ActionType == ACLAction.Withdraw)
            {
                GridDataItem g = CampaignStatusGrid.SelectedItems[0] as GridDataItem;
                var contextID = (long)g.GetDataKeyValue("AttestationCampaignId");
                if (contextID  > 0)
                {
                    var campaign = DomainApiClient<Dto.AttestationCampaign>.GetById(contextID);
                    if(campaign != null && campaign.Status == Infrastructure.Enums.AttestationCampaignStatus.Completed)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, $"Completed Campaign({campaign.CampaignName}) cannot be withdrawn.");
                        return;
                    }
                    lblMsg.Text = "";
                    txtComment.Text = "";
                    btnCancelAttestationRejection.Enabled = true;
                    btnAttestationConfirmRejectionSubmit.Enabled = true;
                    ConfirmTermination.VisibleOnPageLoad = true;
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, $"Please select a campaign for WithDraw.");
                }
            }
        }

        protected void CampaignStatusGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ShowInfo")
            {                
                GridDataItem item = e.Item as GridDataItem;
                this.ContextObjectId = (long)item.GetDataKeyValue("AttestationCampaignId");
                var campaign = CampaignList.Where(a => a.AttestationCampaignId == ContextObjectId).ToList();
                AttestationCampaignDetailUC.Campaign = campaign.First();
                CampaignDetail.VisibleOnPageLoad = true;
            }
        }

        protected void CampaignStatusGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = CampaignStatusGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("AttestationCampaignId");
        }

        protected void CampaignStatusGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                try
                {
                    GridDataItem item = e.Item as GridDataItem;
                    var daysToComplete = (item.DataItem as DI.Chakra.Dto.Presentation.PresentationAttetstationCampaign).DaysToComplete;
                    var objCampaign = (item.DataItem as DI.Chakra.Dto.Presentation.PresentationAttetstationCampaign);
                    var statusColumn = item["TemplateStatusColumn"];
                    //statusColumn.ToolTip = "Days To Complete: " + daysToComplete.ToString();
                    var pnlDescription = item.FindControl("pnlDescription") as Panel;
                    var hoverCompletionMessage = item.FindControl("hoverCompletionMessage") as HoverMenuExtender;
                    var lblCompletionMessage = item.FindControl("lblCompletionMessage") as Label;
                    lblCompletionMessage.Text = "Days To Complete: " + daysToComplete.ToString(); ;
                    if (objCampaign.OverallStatus != DI.Chakra.Infrastructure.Enums.AttestationCampaignStatus.Completed.ToString())
                    {

                        if (daysToComplete < 2)
                        {
                            pnlDescription.CssClass = "HoverPopupltTwoDays";
                            statusColumn.CssClass = "status-amber-info";
                            hoverCompletionMessage.Enabled = true;
                        }
                        else if (daysToComplete < 10)
                        {
                            pnlDescription.CssClass = "HoverPopupltTenDays";
                            statusColumn.CssClass = "status-orange-info";
                            hoverCompletionMessage.Enabled = true;
                        }
                    }
                    else
                    {
                        hoverCompletionMessage.Enabled = false;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Attestation Name", "AttestationName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Campaign Name", "CampaignName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Description", "Description", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Start Date", "CreatedDate", Constants.DateFormatForSP));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Close Date", "CloseDate", Constants.DateFormatForSP));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Overall Status", "OverallStatus", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Level", "Level", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Outstanding Task", "TotalItem", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remediation", "RequiresAttestationRemediation", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Submitted Date", "SubmittedDate", Constants.DateFormatForSP));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Status", "DaysToComplete", null));
            return param;
        }
        protected void btnConfirmSubmit_Click(object sender, EventArgs e)
        {

            btnCancelAttestationRejection.Enabled = false;
            btnAttestationConfirmRejectionSubmit.Enabled = false;
            TerminateAttestationCampaign();

        }

        protected void btnCancelSubmit_Click(object sender, EventArgs e)
        {
            ConfirmTermination.VisibleOnPageLoad = false;
        }
        private void TerminateAttestationCampaign()
        {
            bool status = false;
            long attCampaignId = 0;
            GridDataItem g = CampaignStatusGrid.SelectedItems[0] as GridDataItem;
            var contextID = (long)g.GetDataKeyValue("AttestationCampaignId");
            if (contextID > 0)
            {
                attCampaignId = contextID;
            }
            else
            {
                lblMsg.Text = "Action not completed. Please select an Attestation Campaign to withdrawn.";
                ConfirmTermination.VisibleOnPageLoad = false;
                return;
            }
            lblMsg.Text = "";
            if (String.IsNullOrWhiteSpace(txtComment.Text))
            {
                lblMsg.Text = "Please enter reason for withdrawn Campaign.";
                btnCancelAttestationRejection.Enabled = true;
                btnAttestationConfirmRejectionSubmit.Enabled = true;
                return;
            }
            var campaign = DomainApiClient<AttestationCampaign>.GetById(attCampaignId);
            if (campaign != null && campaign.Status == DI.Chakra.Infrastructure.Enums.AttestationCampaignStatus.WorkInProgress && campaign.WorkflowInstanceID != Guid.Empty)
            {
                var msg = $"Campaign :{campaign.CampaignName} withdrawn by {SessionUser.User.DisplayName} with reason: {txtComment.Text}";
                DILog.UILogger.WriteLog(SessionUser.User.Id, LogLevelType.Info, $"{msg}");
                SaveAttestationComment(attCampaignId, msg);
                Dto.Presentation.PresentationWorkflowCriteriaBase _criteria = new Dto.Presentation.WithdrawActionCriteria()
                {
                    RequestType = RequestType.Attestation,
                    WorkflowInstanceId = campaign.WorkflowInstanceID
                };
                status = DomainApiClient<object>.GetByAction<bool>("WorkflowController", DomainApiAction.WorkflowProcessor.WithDrawnRequest, null, _criteria);
                if (status)
                {
                    DILog.UILogger.WriteLog(SessionUser.User.Id, LogLevelType.Info, $"Campaign: {campaign.CampaignName} withdrawn successful");
                }
            }
            if (!status)
            {
                MasterPage.ShowAlert(AlertType.Error, $"Action not completed. Attestation withdrawn failed.");
                ConfirmTermination.VisibleOnPageLoad = false;
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Success, "Campaign withdrawn successfully.");
                ConfirmTermination.VisibleOnPageLoad = false;
            }
            CampaignStatusGrid.DataSource = null;
            CampaignStatusGrid.Rebind();
            // refresh grid
        }
        public void SaveAttestationComment(long campaignId, string comment)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("campaignId", campaignId.ToString());
            parameters.Add("currentUser", SessionUser.User.UserName);
            parameters.Add("comment", comment);
            DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                            (DomainApiAction.Tasks.Task.UpdateAttestationComment, parameters);
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            CampaignStatusGrid.DataSource = null;
            CampaignStatusGrid.Rebind();
        }
    }
}