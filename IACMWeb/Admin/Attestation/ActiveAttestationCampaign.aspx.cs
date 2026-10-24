using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Web.Admin.Exception;

namespace DI.Chakra.Web.Admin.Attestation
{
    public partial class ActiveAttestationCampaign : ReportBasePage, IContextObjectEnabled
    {
        private static IList<PresentationAttetstationCampaign> CampaignList = new List<PresentationAttetstationCampaign>();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }
        
        protected void CampaignStatusGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            CampaignList = DomainApiClient<Dto.Presentation.PresentationAttetstationCampaign>
            .GetByAction<List<Dto.Presentation.PresentationAttetstationCampaign>>(DomainApiAction.Attestation.PresentationAttetstationCampaign.GetCampaignStatus, null).ToList();
            CampaignStatusGrid.DataSource = CampaignList;
        }

        protected void btnPageCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                report = RptSchd.GetReportingSchedule();
                scheduler = RptSchd.Scheduler;
                attributeParameter.Add("EndPointReport", "Users");
                if (Save())
                {
                    RptSchd.ClearpopupData();
                    usrModal.VisibleOnPageLoad = false;
                }
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Schduler is not filled with schedule values");
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            usrModal.VisibleOnPageLoad = false;
            RptSchd.InitializeSched();
        }

        protected void btnDetailCancel_Click(object sender, EventArgs e)
        {
            CampaignDetail.VisibleOnPageLoad = false;
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                CampaignStatusGrid.ExportSettings.FileName = "AttestationCampaignStatus";
                Export(CampaignStatusGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                CampaignStatusGrid.Export<IList<PresentationAttetstationCampaign>>(CampaignList, e.ActionType, "AttestationCampaignStatus");
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
                    TableCell cell = item["CloseDate"];
                    var closeDate = Convert.ToDateTime(cell.Text);
                    var daysToComplete = (closeDate - DateTime.Now).TotalDays;

                    var statusColumn = item["TemplateStatusColumn"];
                    if (daysToComplete < 2)
                        statusColumn.CssClass = "status-amber-info";
                    else if (daysToComplete < 10)
                        statusColumn.CssClass = "status-orange-info";
                }
                catch //(/*Exception ex*/)
                {

                }
            }
        }
    }
}