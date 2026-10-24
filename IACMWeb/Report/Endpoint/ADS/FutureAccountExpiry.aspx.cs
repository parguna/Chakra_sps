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
    public partial class FutureAccountExpiry : ReportBasePage
    {
        private static IList<PresentationADSFutureAccountExpiryAccount> AccountExpiryAccountList;
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

        private string AccountExpiryDays
        {
            get
            {
                if (ViewState["AccountExpiryDays"] != null)
                    return (string)ViewState["AccountExpiryDays"];
                else
                    return string.Empty;
            }

            set
            {
                ViewState["AccountExpiryDays"] = value;
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
                        parameter.Add("AccountExpiryDays", AccountExpiryDays);

                        AccountExpiryAccountList = DomainApiClient<PresentationADSFutureAccountExpiryAccount>.
                            GetByAction<List<PresentationADSFutureAccountExpiryAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSFutureAccountExpiryAccountReportData, parameter);
                        grdAccountExpiryAccount.DataSource = AccountExpiryAccountList;
                        ServiceExport(grdAccountExpiryAccount, fileformat);
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
                IList<PresentationADSFutureAccountExpiryAccount> AccountExpiryAccountList;
                AccountExpiryAccountList = new List<PresentationADSFutureAccountExpiryAccount>();
                grdAccountExpiryAccount.DataSource = AccountExpiryAccountList;
                grdAccountExpiryAccount.DataBind();
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            //IList<PresentationADSFutureAccountExpiryAccount> AccountExpiryAccountList;
            if (EndPointSystemID == 0)
            {
                ShowAlertMessage(AlertType.Error, "Please select the Endpoint");

            }

            if (txtAccountExpiryDays.Text == string.Empty)
            {
                ShowAlertMessage(AlertType.Error, "Please enter the No of days to fetch Account Expiry Accounts");
            }


            if (EndPointSystemID > 0 && txtAccountExpiryDays.Text != string.Empty)
            {
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());
                parameterList.Add("AccountExpiryDays", txtAccountExpiryDays.Text.ToString());
                AccountExpiryDays = txtAccountExpiryDays.Text;

                AccountExpiryAccountList = DomainApiClient<PresentationADSFutureAccountExpiryAccount>.
                    GetByAction<List<PresentationADSFutureAccountExpiryAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSFutureAccountExpiryAccountReportData, parameterList);
                grdAccountExpiryAccount.DataSource = AccountExpiryAccountList;
                grdAccountExpiryAccount.DataBind();
                grdAccountExpiryAccount.CurrentPageIndex = 0;
            }
            else
            {
                AccountExpiryAccountList = new List<PresentationADSFutureAccountExpiryAccount>();
                grdAccountExpiryAccount.DataSource = AccountExpiryAccountList;
                grdAccountExpiryAccount.DataBind();
            }

        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdAccountExpiryAccount.ExportSettings.FileName = "Report-AD-Future-Expiry-Accounts";
                Export(grdAccountExpiryAccount, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdAccountExpiryAccount.Export<IList<PresentationADSFutureAccountExpiryAccount>>(AccountExpiryAccountList, e.ActionType, "Report-AD-Future-Expiry-Accounts");
            }
        }

        public void ShowAlertMessage(DI.Chakra.Infrastructure.AlertType AlertType, string ErrorMessage)
        {
            MasterPage.ShowAlert(AlertType, ErrorMessage);
        }

        protected void grdAccountExpiryAccount_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            IList<PresentationADSFutureAccountExpiryAccount> AccountExpiryAccountList;
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            if (EndPointSystemID > 0)
            {
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());
                parameterList.Add("AccountExpiryDays", AccountExpiryDays);

                AccountExpiryAccountList = DomainApiClient<PresentationADSFutureAccountExpiryAccount>.
                    GetByAction<List<PresentationADSFutureAccountExpiryAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSFutureAccountExpiryAccountReportData, parameterList);
                grdAccountExpiryAccount.DataSource = AccountExpiryAccountList;
            }
            else
            {
                AccountExpiryAccountList = new List<PresentationADSFutureAccountExpiryAccount>();
                grdAccountExpiryAccount.DataSource = AccountExpiryAccountList;
            }
        }

        protected void grdAccountExpiryAccount_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            if (EndPointSystemID > 0)
            {
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());
                parameterList.Add("AccountExpiryDays", AccountExpiryDays);

                AccountExpiryAccountList = DomainApiClient<PresentationADSFutureAccountExpiryAccount>.
                    GetByAction<List<PresentationADSFutureAccountExpiryAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSFutureAccountExpiryAccountReportData, parameterList);
                grdAccountExpiryAccount.DataSource = AccountExpiryAccountList;
            }
            else
            {
                AccountExpiryAccountList = new List<PresentationADSFutureAccountExpiryAccount>();
                grdAccountExpiryAccount.DataSource = AccountExpiryAccountList;
            }
        }

    }
}