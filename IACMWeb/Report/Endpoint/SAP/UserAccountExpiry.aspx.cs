using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.Endpoint.SAP
{
    public partial class UserAccountExpiry : ReportBasePage
    {
        #region Members

        private static IList<PresentationSAPUserExpiryAccount> dispList;

        #endregion

        #region Properties

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

        #endregion

        #region Events

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
                        parameter.Add("EndpointSystemId", endpointsystemtypeid);
                        parameter.Add("AccountExpiryDays", AccountExpiryDays.ToString());
                        dispList = DomainApiClient<PresentationSAPUserExpiryAccount>.GetByAction<List<PresentationSAPUserExpiryAccount>>
                    (DomainApiAction.Endpoint.Presentation.SAP.GetSAPUserExpiryAccountReportData, parameter);
                        grdExpiryAccount.DataSource = dispList;
                        ServiceExport(grdExpiryAccount, fileformat);
                    }
                    catch (Exception ex)
                    { }
                }

                EndpointUC.Initialize();
                EndpointUC.SelectedApplication =
                    new EndPointSystemType { EndpointSystemTypeName = Infrastructure.Enums.EndpointType.SAP };

                Dictionary<string, string> repparameter = new Dictionary<string, string>();
                repparameter.Add("id", EndpointUC.SelectedApplication.Id.ToString());
                var Endpoint = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>
                    (DomainApiAction.Endpoint.EndPointSystemType.GetByIdIncludingEndpoints, repparameter);

                if (Endpoint == null || Endpoint.EndPoints.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "First Create SAP Endpoint");
                }
            }
           
        }
        protected void grdExpiryAccount_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                parameterList.Add("EndpointSystemId", EndPointSystemID.ToString());
                parameterList.Add("AccountExpiryDays", AccountExpiryDays.ToString());
                dispList = DomainApiClient<PresentationSAPUserExpiryAccount>.GetByAction<List<PresentationSAPUserExpiryAccount>>
                    (DomainApiAction.Endpoint.Presentation.SAP.GetSAPUserExpiryAccountReportData, parameterList);
                if (dispList != null)
                {
                    grdExpiryAccount.DataSource = dispList;
                }
            }
            else
            {
                dispList = new List<PresentationSAPUserExpiryAccount>();
                grdExpiryAccount.DataSource = dispList;
            }
        }

        protected void DropDownSelectectionChanged(EndPoint EndPoint)
        {
            try
            {
                EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
                dispList = new List<PresentationSAPUserExpiryAccount>();

                grdExpiryAccount.DataSource = dispList;
                grdExpiryAccount.DataBind();
                
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdExpiryAccount.ExportSettings.FileName = "Report-SAP-User-Expiry-Accounts";
                Export(grdExpiryAccount, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdExpiryAccount.Export<IList<PresentationSAPUserExpiryAccount>>(dispList, e.ActionType, "Report-SAP-User-Expiry-Accounts");
            }
        }


        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            IList<PresentationSAPUserExpiryAccount> ExpiryAccountList;
            if (EndPointSystemID == 0)
            {
                ShowAlertMessage(AlertType.Error, "Please select the Endpoint");

            }

            if (txtAccountExpiryDays.Text == string.Empty)
            {
                ShowAlertMessage(AlertType.Error, "Please enter the No of days to fetch Expiry Accounts");
            }


            if (EndPointSystemID > 0 && txtAccountExpiryDays.Text != string.Empty)
            {
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                parameterList.Add("endpointSystemid", EndPointSystemID.ToString());
                parameterList.Add("accountexpirydays", txtAccountExpiryDays.Text);
                AccountExpiryDays = txtAccountExpiryDays.Text;

                ExpiryAccountList = DomainApiClient<PresentationSAPUserExpiryAccount>.GetByAction<List<PresentationSAPUserExpiryAccount>>
                    (DomainApiAction.Endpoint.Presentation.SAP.GetSAPUserExpiryAccountReportData, parameterList);
                grdExpiryAccount.DataSource = ExpiryAccountList;
                grdExpiryAccount.CurrentPageIndex = 0;
                grdExpiryAccount.DataBind();

            }
            else
            {
                ExpiryAccountList = new List<PresentationSAPUserExpiryAccount>();
                grdExpiryAccount.DataSource = ExpiryAccountList;
                grdExpiryAccount.DataBind();
            }

        }


        #endregion

        public void ShowAlertMessage(DI.Chakra.Infrastructure.AlertType AlertType, string ErrorMessage)
        {
            MasterPage.ShowAlert(AlertType, ErrorMessage);
        }
    }
}