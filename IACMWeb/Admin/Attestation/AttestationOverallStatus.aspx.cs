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

namespace DI.Chakra.Web.Admin.Attestation
{
    public partial class AttestationOverallStatus : ReportBasePage, IContextObjectEnabled
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

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            SessionUser.RedirectFrom = "AttestationOverallStatus";
        }
        
        protected void CampaignStatusGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            CampaignList = DomainApiClient<Dto.Presentation.PresentationAttetstationCampaign>
            .GetByAction<List<Dto.Presentation.PresentationAttetstationCampaign>>(DomainApiAction.Attestation.PresentationAttetstationCampaign.GetAttestationOverallStatus, null).ToList();
            CampaignStatusGrid.DataSource = CampaignList;
            CampaignStatusGrid.VirtualItemCount = CampaignList.Count;
        }

        protected void btnPageCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
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
           // RptSchd.InitializeSched();
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
                    GenericExport(data, e.ActionType, "AttestationCampaignOverallStatus");
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error,"No Records found for export");
                }
            }
            else if (e.ActionType == ACLAction.Withdraw)
            {
                if(this.ContextObjectId.HasValue && this.ContextObjectId.Value > 0)
                {
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
                    var statusColumn = item["TemplateStatusColumn"];
                    var lblCompletionMessage = item.FindControl("lblCompletionMessage") as Label;
                    lblCompletionMessage.Text = "Days To Complete: " + daysToComplete.ToString();
                    var pnlDescription = item.FindControl("pnlDescription") as Panel;
                    var hoverCompletionMessage = item.FindControl("hoverCompletionMessage") as HoverMenuExtender;
                    
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
                catch //(Exception ex)
                {

                }
            }
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

            if (this.ContextObjectId != null && ContextObjectId.HasValue)
            {
                attCampaignId = this.ContextObjectId.Value;
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
            List<AttestationCampaign> campaigns = new List<AttestationCampaign>();
            var dic = new Dictionary<string, string>();
            dic.Add("campaignId", attCampaignId.ToString());
            campaigns = DomainApiClient<AttestationCampaign>.GetByAction<List<AttestationCampaign>>("GetAllChildCampaignByCampaignId", dic);
            List<AttestationCampaign> allSuccessful = new List<AttestationCampaign>();
            foreach (var campaign in campaigns)
            {
                try
                {
                    var msg = $"Campaign :{campaign.CampaignName} withdrawn by {SessionUser.User.DisplayName} with reason: {txtComment.Text}";
                    if (campaign.Status != AttestationCampaignStatus.Completed)
                    {
                        if (campaign.WorkflowInstanceID != Guid.Empty)
                        {
                            DILog.UILogger.WriteLog(SessionUser.User.Id, LogLevelType.Info, $"{msg}");
                            SaveAttestationComment(campaign.Id, msg);
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
                            else
                            {
                                allSuccessful.Add(campaign);
                            }
                        }
                        else
                        {
                            SaveAttestationComment(campaign.Id, msg);
                            campaign.Comments += "Attestation Campaign has been cancelled.";
                            campaign.EndDate = DateTime.Now;
                            campaign.Status = AttestationCampaignStatus.Completed;
                            DomainApiClient<Dto.AttestationCampaign>.Save(campaign);
                        }
                        
                    }
                }
                catch (System.Exception ex)
                {
                    DILog.UILogger.WriteLog(SessionUser.User.Id, LogLevelType.Info, $"{ex.Message}",ex);
                    allSuccessful.Add(campaign);
                }
            }
            
            status = allSuccessful.Count == 0;

            if (!status)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Attestation campaign withdrawn failed.{allSuccessful.Select(i=>i.CampaignName)}");
                MasterPage.ShowAlert(AlertType.Error, $"Action not completed. Attestation withdrawn failed.");
                ConfirmTermination.VisibleOnPageLoad = false;
            }
            else
            {
                
                CampaignStatusGrid.DataSource = null;
                CampaignStatusGrid.Rebind();
                MasterPage.ShowAlert(AlertType.Success, "Campaign withdrawn successfully.");
                ConfirmTermination.VisibleOnPageLoad = false;
            }
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
        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Attestation Name", "AttestationName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Campaign Name", "CampaignName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Description", "Description", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Start Date", "CreatedDate", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Close Date", "CloseDate", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Overall Status", "OverallStatus", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Campaign Split", "CampaignSplit", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remediation", "RequiresAttestationRemediation", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Status", "DaysToComplete", null));
            return param;
        }

        protected void RefreshButton_Click(object sender, EventArgs e)
        {
            CampaignStatusGrid.DataSource = null;
            CampaignStatusGrid.Rebind();
        }
    }
}