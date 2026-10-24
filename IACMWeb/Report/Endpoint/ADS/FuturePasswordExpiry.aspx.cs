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
    public partial class FuturePasswordExpiry : ReportBasePage
    {
        private static IList<PresentationADSFuturePasswordExpiryAccount> PasswordExpiryAccountList;
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

        private string PasswordExpiryDays
        {
            get
            {
                if (ViewState["PasswordExpiryDays"] != null)
                    return Convert.ToString(ViewState["PasswordExpiryDays"]);
                else
                    return String.Empty;
            }

            set
            {
                ViewState["PasswordExpiryDays"] = value;
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
                        parameter.Add("passwordExpiryDays", PasswordExpiryDays);
                        PasswordExpiryAccountList = DomainApiClient<PresentationADSFuturePasswordExpiryAccount>.
                            GetByAction<List<PresentationADSFuturePasswordExpiryAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSFuturePasswordExpiryAccountReportData, parameter);
                        grdPasswordExpiryAccount.DataSource = PasswordExpiryAccountList;
                        ServiceExport(grdPasswordExpiryAccount, fileformat);
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
                //grdPasswordExpiryAccount.Rebind();
                IList<PresentationADSFuturePasswordExpiryAccount> PasswordExpiryAccountList;
                PasswordExpiryAccountList = new List<PresentationADSFuturePasswordExpiryAccount>();
                grdPasswordExpiryAccount.DataSource = PasswordExpiryAccountList;
                grdPasswordExpiryAccount.DataBind();
               
                
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }
        

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            //IList<PresentationADSFuturePasswordExpiryAccount> PasswordExpiryAccountList;
            if (EndPointSystemID == 0)
            {
                ShowAlertMessage(AlertType.Error, "Please select the Endpoint");

            }

            if (txtPasswordExpiryDays.Text == string.Empty)
            {
                ShowAlertMessage(AlertType.Error, "Please enter the No of days to fetch Password Expiry Accounts");
            }


            if (EndPointSystemID > 0 && txtPasswordExpiryDays.Text != string.Empty)
            {
                PasswordExpiryDays = txtPasswordExpiryDays.Text;
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());
                parameterList.Add("passwordExpiryDays", PasswordExpiryDays);

                PasswordExpiryAccountList = DomainApiClient<PresentationADSFuturePasswordExpiryAccount>.
                    GetByAction<List<PresentationADSFuturePasswordExpiryAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSFuturePasswordExpiryAccountReportData, parameterList);
                grdPasswordExpiryAccount.DataSource = PasswordExpiryAccountList;
                grdPasswordExpiryAccount.DataBind();
                grdPasswordExpiryAccount.CurrentPageIndex = 0;
            }
            else
            {
                PasswordExpiryAccountList = new List<PresentationADSFuturePasswordExpiryAccount>();
                grdPasswordExpiryAccount.DataSource = PasswordExpiryAccountList;
                grdPasswordExpiryAccount.DataBind();
            }

        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdPasswordExpiryAccount.ExportSettings.FileName = "Report-AD-Future-Password-Expiry-Accounts";
                Export(grdPasswordExpiryAccount, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdPasswordExpiryAccount.Export<IList<PresentationADSFuturePasswordExpiryAccount>>(PasswordExpiryAccountList, e.ActionType, "Report-AD-Future-Password-Expiry-Accounts");
            }
        }

        public void ShowAlertMessage(DI.Chakra.Infrastructure.AlertType AlertType, string ErrorMessage)
        {
            MasterPage.ShowAlert(AlertType, ErrorMessage);
        }

        protected void grdPasswordExpiryAccount_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            IList<PresentationADSFuturePasswordExpiryAccount> PasswordExpiryAccountList;
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            if (EndPointSystemID > 0)
            {
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());
                parameterList.Add("passwordExpiryDays", PasswordExpiryDays);

                PasswordExpiryAccountList = DomainApiClient<PresentationADSFuturePasswordExpiryAccount>.
                    GetByAction<List<PresentationADSFuturePasswordExpiryAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSFuturePasswordExpiryAccountReportData, parameterList);
                grdPasswordExpiryAccount.DataSource = PasswordExpiryAccountList;
            }
            else
            {
                PasswordExpiryAccountList = new List<PresentationADSFuturePasswordExpiryAccount>();
                grdPasswordExpiryAccount.DataSource = PasswordExpiryAccountList;
            }
        }

        protected void grdPasswordExpiryAccount_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            if (EndPointSystemID > 0)
            {
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());
                parameterList.Add("passwordExpiryDays", PasswordExpiryDays);

                PasswordExpiryAccountList = DomainApiClient<PresentationADSFuturePasswordExpiryAccount>.
                    GetByAction<List<PresentationADSFuturePasswordExpiryAccount>>(DomainApiAction.Endpoint.Presentation.AD.GetADSFuturePasswordExpiryAccountReportData, parameterList);
                grdPasswordExpiryAccount.DataSource = PasswordExpiryAccountList;
            }
            else
            {
                PasswordExpiryAccountList = new List<PresentationADSFuturePasswordExpiryAccount>();
                grdPasswordExpiryAccount.DataSource = PasswordExpiryAccountList;
            }
        }
    }
}