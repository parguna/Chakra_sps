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
    public partial class DormantAccounts : ReportBasePage
    {
        private static IList<PresentationADSDormantAccount> DormantAccountList;
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

        private string DormantDays
        {
            get
            {
                if (ViewState["DormantDays"] != null)
                    return (string)ViewState["DormantDays"];
                else
                    return string.Empty;
            }
            set
            {
                ViewState["DormantDays"] = value;
            }
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
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
                        parameter.Add("dormantDays", DormantDays.ToString());
                        DormantAccountList = DomainApiClient<PresentationADSDormantAccount>.
                        GetByAction<List<PresentationADSDormantAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSUserByDormantDaysReportData, parameter);

                        grdDormantAccount.DataSource = DormantAccountList;
                        ServiceExport(grdDormantAccount,fileformat);
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
                IList<PresentationADSDormantAccount> DormantAccountList;
                DormantAccountList = new List<PresentationADSDormantAccount>();
                grdDormantAccount.DataSource = DormantAccountList;
                grdDormantAccount.DataBind();
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }

        protected void grdDormantAccount_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {

            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());
                parameterList.Add("dormantDays", DormantDays.ToString());

                DormantAccountList = DomainApiClient<PresentationADSDormantAccount>.
                    GetByAction<List<PresentationADSDormantAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSUserByDormantDaysReportData, parameterList);
                grdDormantAccount.DataSource = DormantAccountList;
            }
            else
            {
                DormantAccountList = new List<PresentationADSDormantAccount>();
                grdDormantAccount.DataSource = DormantAccountList;
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (EndPointSystemID == 0)
            {
                ShowAlertMessage(AlertType.Error, "Please select the Endpoint");

            }

            if (txtDormantDays.Text == string.Empty)
            {
                ShowAlertMessage(AlertType.Error, "Please enter the No of days to fetch Dormant Accounts");
            }

            if (EndPointSystemID > 0 && txtDormantDays.Text != string.Empty)
            {
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());
                parameterList.Add("dormantDays", txtDormantDays.Text.ToString());
                DormantDays = txtDormantDays.Text;

                DormantAccountList = DomainApiClient<PresentationADSDormantAccount>.
                    GetByAction<List<PresentationADSDormantAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSUserByDormantDaysReportData, parameterList);
                grdDormantAccount.DataSource = DormantAccountList;
                grdDormantAccount.CurrentPageIndex = 0;
                grdDormantAccount.DataBind();
            }
            else
            {
                DormantAccountList = new List<PresentationADSDormantAccount>();
                grdDormantAccount.DataSource = DormantAccountList;
                grdDormantAccount.DataBind();
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdDormantAccount.ExportSettings.FileName = "Report-AD-Dormant-Accounts";
                Export(grdDormantAccount, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdDormantAccount.Export<IList<PresentationADSDormantAccount>>(DormantAccountList, e.ActionType, "Report-AD-Dormant-Accounts");
            }
        }

        public void ShowAlertMessage(DI.Chakra.Infrastructure.AlertType AlertType, string ErrorMessage)
        {
            MasterPage.ShowAlert(AlertType, ErrorMessage);
        }
    }
}
