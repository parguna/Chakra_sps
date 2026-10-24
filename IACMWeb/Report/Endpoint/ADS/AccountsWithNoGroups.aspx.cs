using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.Endpoint.ADS
{
    public partial class AccountsWithNoGroups : ReportBasePage
    {
        private static IList<PresentationADSAccountWithNoGroups> LockedDisabledAccountList;
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


                        LockedDisabledAccountList = DomainApiClient<PresentationADSAccountWithNoGroups>.
                   GetByAction<List<PresentationADSAccountWithNoGroups>>(DomainApiAction.Endpoint.Presentation.AD.GetADSAccountWithNoGroupsReportData, parameter);
                        grdAccountsWithNoGroups.DataSource = LockedDisabledAccountList;
                        ServiceExport(grdAccountsWithNoGroups,fileformat);
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
                grdAccountsWithNoGroups.Rebind();
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }

        protected void grdAccountsWithNoGroups_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
           
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());

                LockedDisabledAccountList = DomainApiClient<PresentationADSAccountWithNoGroups>.
                    GetByAction<List<PresentationADSAccountWithNoGroups>>(DomainApiAction.Endpoint.Presentation.AD.GetADSAccountWithNoGroupsReportData, parameterList);
                grdAccountsWithNoGroups.DataSource = LockedDisabledAccountList;
            }
            else
            {
                LockedDisabledAccountList = new List<PresentationADSAccountWithNoGroups>();
                grdAccountsWithNoGroups.DataSource = LockedDisabledAccountList;
            }


        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdAccountsWithNoGroups.ExportSettings.FileName = "Report-AD-Accounts-With-No-Groups";
                Export(grdAccountsWithNoGroups, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdAccountsWithNoGroups.Export< IList < PresentationADSAccountWithNoGroups >> (LockedDisabledAccountList, e.ActionType, "Report-AD-Accounts-With-No-Groups");
            }
        }

        public void ShowAlertMessage(DI.Chakra.Infrastructure.AlertType AlertType, string ErrorMessage)
        {
            MasterPage.ShowAlert(AlertType, ErrorMessage);
        }

    }
}