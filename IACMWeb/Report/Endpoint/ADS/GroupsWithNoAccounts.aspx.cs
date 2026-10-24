using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.Endpoint.ADS
{
    public partial class GroupsWithNoAccounts : ReportBasePage
    {
        private static IList<PresentationADSGroupWithNoAccounts> GroupWithNoAccountsList;
        private long EndPointSystemID
        {
            get
            {
                if (ViewState["EndPointSystemID"] != null)
                {
                    return (long)ViewState["EndPointSystemID"];
                }
                return 0;
            }
            set
            {
                ViewState["EndPointSystemID"] = value;
            }
        }


        protected new void Page_Load(object sender, EventArgs e)
        {

            this.MasterPage.ActionClicked += ActionClicked;
            reportHistoryId = Request["reportHistoryId"];
            if (!Page.IsPostBack)
            {
                if (reportHistoryId != null)
                {
                    try
                    {
                        base.Page_Load(sender, e);
                        string endpointsystemtypeid = string.Empty;
                        parameter.TryGetValue("EndPointSystemId", out endpointsystemtypeid);
                        parameter.Clear();
                        parameter.Add("endpointSystemid", endpointsystemtypeid);

                        GroupWithNoAccountsList = DomainApiClient<PresentationADSGroupWithNoAccounts>.
                   GetByAction<List<PresentationADSGroupWithNoAccounts>>(DomainApiAction.Endpoint.Presentation.AD.GetADSGroupWithNoAccountsReportData, parameter);
                        grdGroupsWithNoAccounts.DataSource = GroupWithNoAccountsList;
                        ServiceExport(grdGroupsWithNoAccounts, fileformat);
                    }
                    catch (Exception ex)
                    { }
                }
                EndpointUC.Initialize();
                EndpointUC.SelectedApplication =
                    new EndPointSystemType { EndpointSystemTypeName = Infrastructure.Enums.EndpointType.ActiveDirectory };

                Dictionary<string, string> repparameter = new Dictionary<string, string>();
                repparameter.Add("id", EndpointUC.SelectedApplication.Id.ToString());
                var Endpoint = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>
                    (DomainApiAction.Endpoint.EndPointSystemType.GetByIdIncludingEndpoints, repparameter);

                if (Endpoint == null || Endpoint.EndPoints.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "First Create AD Endpoint");
                }

            }
        }

        protected void DropDownSelectectionChanged(EndPoint EndPoint)
        {
            try
            {
                EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
                grdGroupsWithNoAccounts.Rebind();
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }

        protected void grdGroupsWithNoAccounts_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());

                GroupWithNoAccountsList = DomainApiClient<PresentationADSGroupWithNoAccounts>.
                    GetByAction<List<PresentationADSGroupWithNoAccounts>>(DomainApiAction.Endpoint.Presentation.AD.GetADSGroupWithNoAccountsReportData, parameterList);
                grdGroupsWithNoAccounts.DataSource = GroupWithNoAccountsList;
            }
            else
            {
                GroupWithNoAccountsList = new List<PresentationADSGroupWithNoAccounts>();
                grdGroupsWithNoAccounts.DataSource = GroupWithNoAccountsList;
            }


        }


        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdGroupsWithNoAccounts.ExportSettings.FileName = "Report-AD-Groups-With-No-Accounts";
                Export(grdGroupsWithNoAccounts, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdGroupsWithNoAccounts.Export<IList<PresentationADSGroupWithNoAccounts>>(GroupWithNoAccountsList, e.ActionType, "Report-AD-Groups-With-No-Accounts");
            }
        }

        public void ShowAlertMessage(DI.Chakra.Infrastructure.AlertType AlertType, string ErrorMessage)
        {
            MasterPage.ShowAlert(AlertType, ErrorMessage);
        }

    }
}