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
    public partial class DormantUsers : ReportBasePage
    {
        #region Members

        private static IList<PresentationSAPDormantUser> dispList;

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
                //txtDormantDays.Text = value;
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
                        parameter.Add("DormantDays", DormantDays.ToString());
                        dispList = DomainApiClient<PresentationSAPDormantUser>.GetByAction<List<PresentationSAPDormantUser>>
                   (DomainApiAction.Endpoint.Presentation.SAP.GetSAPDormantUsersReportData, parameter);
                        grdDormantAccount.DataSource = dispList;
                        ServiceExport(grdDormantAccount, fileformat);
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
            //else
            //{
            //    DormantDays = DormantDays;
            //}

           
        }
        protected void grdDormantAccount_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                parameterList.Add("EndpointSystemId", EndPointSystemID.ToString());
                parameterList.Add("dormantDays", DormantDays.ToString());
                dispList = DomainApiClient<PresentationSAPDormantUser>.GetByAction<List<PresentationSAPDormantUser>>
                    (DomainApiAction.Endpoint.Presentation.SAP.GetSAPDormantUsersReportData, parameterList);
                if (dispList != null)
                {
                    grdDormantAccount.DataSource = dispList;
                }
            }
            else
            {
                dispList = new List<PresentationSAPDormantUser>();
                grdDormantAccount.DataSource = dispList;
            }
        }

        protected void DropDownSelectectionChanged(EndPoint EndPoint)
        {
            try
            {
                EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
                dispList = new List<PresentationSAPDormantUser>();

                grdDormantAccount.DataSource = dispList;
                grdDormantAccount.DataBind();
                
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
                grdDormantAccount.ExportSettings.FileName = "Report-SAP-Dormant-Users";
                grdDormantAccount.Export(grdDormantAccount, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdDormantAccount.Export<IList<PresentationSAPDormantUser>>(dispList, e.ActionType, "Report-SAP-Dormant-Users");
            }
        }


        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            IList<PresentationSAPDormantUser> DormantAccountList;
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
                parameterList.Add("dormantDays", txtDormantDays.Text);
                DormantDays = txtDormantDays.Text;

                DormantAccountList = DomainApiClient<PresentationSAPDormantUser>.GetByAction<List<PresentationSAPDormantUser>>
                    (DomainApiAction.Endpoint.Presentation.SAP.GetSAPDormantUsersReportData, parameterList);
                grdDormantAccount.DataSource = DormantAccountList;
                grdDormantAccount.CurrentPageIndex = 0;
                grdDormantAccount.DataBind();

            }
            else
            {
                DormantAccountList = new List<PresentationSAPDormantUser>();
                grdDormantAccount.DataSource = DormantAccountList;
                grdDormantAccount.DataBind();
            }

        }


        #endregion

        public void ShowAlertMessage(DI.Chakra.Infrastructure.AlertType AlertType, string ErrorMessage)
        {
            MasterPage.ShowAlert(AlertType, ErrorMessage);
        }

     
    }
}