using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace DI.Chakra.Web.Report.Attestation
{
    public partial class AttestationCampaigntaskDetail : ReportBasePage, IContextObjectRequired, IContextObjectEnabled
    {
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        private static IList<PresentationTask> ADUserList = new List<PresentationTask>();
        #region Members
        private const string V_EntityID = "V_EntityID";
        private const string V_CampaignID = "V_CampaignID";
        private const string V_RedirectFrom = "V_RedirectFrom";
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
                SetPage();
            }
        }

        public string RedirectFrom
        {
            get
            {
                if (ViewState[V_RedirectFrom] == null)
                    return "";
                return (string)ViewState[V_RedirectFrom];
            }
            set
            {
                ViewState[V_RedirectFrom] = value;
                SessionUser.RedirectFrom = value;
            }
        }

        public long CampaignId
        {
            get
            {
                if (ViewState[V_CampaignID] == null)
                    return -1;
                return (long)ViewState[V_CampaignID];
            }
            set
            {
                ViewState[V_CampaignID] = value;
            }
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("AttestationCampaignId", EntityId.ToString());
            var task = DomainApiClient<Dto.Presentation.PresentationTask>.GetByAction<List<DI.Chakra.Dto.Presentation.PresentationTask>>
                (DomainApiAction.Attestation.PresentationAttetstationCampaign.GetCampaignTaskDetail, parameter);

            if (task == null)
                return;
            AttestationCampaignTaskGrid.DataSource = task;
            AttestationCampaignTaskGrid.Rebind();
            CampaignId = EntityId;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["ContextId"] != null)
                {
                    long ContextObjectId = Convert.ToInt64(Request.QueryString["ContextId"]);
                    EntityId = ContextObjectId;
                    CampaignId = EntityId;

                    if (string.IsNullOrEmpty(RedirectFrom))
                        RedirectFrom = SessionUser.RedirectFrom;
                }
            }
            this.MasterPage.ActionClicked += ActionClicked;
            SessionUser.RedirectFrom = "AttestationCampaigntaskDetail";
        }

        protected void btnPageCancel_Click(object sender, EventArgs e)
        {
            this.ContextObjectId = CampaignId;
            SessionStatusImpl _sessionStatus = new SessionStatusImpl();
            if (_sessionStatus.RedirectFrom == "AttestationCampaignStatus")
            {
                
                Response.Redirect("~/Report/Attestation/AttestationCampaignStatus.aspx?ContextId=" + CampaignId.ToString());
            }
            else if (_sessionStatus.RedirectFrom == "AttestationCampaignReport")
            {
                Response.Redirect("~/Report/Attestation/AttestationCampaignReport.aspx");
            }
            else
            {
                var campaign = DomainApiClient<Dto.AttestationCampaign>.GetById(EntityId);
                if (campaign != null && campaign.ParentCampaign.HasValue)
                {
                    CampaignId = campaign.ParentCampaign.Value;
                }
                Response.Redirect("~/Report/Attestation/AttestationCampaignStatus.aspx?ContextId=" + CampaignId.ToString());
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //report = RptSchd.GetReportingSchedule();
                //scheduler = RptSchd.Scheduler;
                //attributeParameter.Add("EndPointReport", "Attestation Campaign Task");
                //if (Save())
                //{
                //    RptSchd.ClearpopupData();
                //    usrModal.VisibleOnPageLoad = false;
                //}
            }
            catch (System.Exception ex)
            {
                //  MasterPage.ShowAlert(AlertType.Error, "Schduler is not filled with schedule values");
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            usrModal.VisibleOnPageLoad = false;
            //RptSchd.InitializeSched();
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                AttestationCampaignTaskGrid.ExportSettings.FileName = "AttestationCampaignTask";
                Export(AttestationCampaignTaskGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                AttestationCampaignTaskGrid.Export<IList<PresentationTask>>(ADUserList, e.ActionType, "AttestationCampaignTask");
            }
            if (e.ActionType == ACLAction.Schedule)
            {
                // if (EndpointUC.SelectedEndpoint != null)
                {
                    usrModal.VisibleOnPageLoad = true;
                    //RptSchd.InitializeSched();
                }
                // RptSchd.RptType = EndpointUC.SelectedApplication.DisplayName.ToString() + "-User";

            }
            else
            {
                //  MasterPage.ShowAlert(AlertType.Error, "Selected any Endpoint");
            }

        }
        #endregion

        protected void AttestationCampaignTaskGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = AttestationCampaignTaskGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("TaskId");
            this.TypeofRequest = RequestType.Attestation.ToString();
        }

        protected void AttestationCampaignTaskGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (EntityId > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("AttestationCampaignId", EntityId.ToString());
                var task = DomainApiClient<Dto.Presentation.PresentationTask>.GetByAction<List<DI.Chakra.Dto.Presentation.PresentationTask>>
                    (DomainApiAction.Attestation.PresentationAttetstationCampaign.GetCampaignTaskDetail, parameter);

                if (task == null)
                    return;
                AttestationCampaignTaskGrid.DataSource = task;
                CampaignId = EntityId;
            }
        }

        protected void lnkTaskId_Command(object sender, CommandEventArgs e)
        {
            try
            {
                GridDataItem item = AttestationCampaignTaskGrid.Items[Convert.ToInt32(e.CommandArgument)];
                if (item == null)
                    return;

                Session["TaskRequestedBy"] = null;
                this.ContextObjectId = (long)item.GetDataKeyValue("TaskId");
                this.TypeofRequest = Convert.ToString(item.GetDataKeyValue("RequestType"));
                Response.Redirect($"~/Task/DetailType.aspx?contextId={this.ContextObjectId}&TypeOfRequest={RequestType.Attestation}&CampaignId={CampaignId}");
            }
            catch (Exception ex)
            {
                DILog.UILogger.WriteLog(SessionUser.User.Id, LogLevelType.Info, $"{ex.Message}", ex);
            }
        }
    }
}