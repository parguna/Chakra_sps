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
    public partial class TCodes : ReportBasePage
    {
        #region Members

        private static IList<PresentationSAPTCodeReport> dispList = new List<PresentationSAPTCodeReport>();

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

        #endregion

        #region Events

        protected  new void Page_Load(object sender, EventArgs e)
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
                        parameter.Add("endpointsystemid", endpointsystemtypeid);
                        dispList = DomainApiClient<PresentationSAPTCodeReport>.GetByAction<List<PresentationSAPTCodeReport>>(DomainApiAction.Endpoint.Presentation.SAP.GetAllSapTCodesReportData, parameter);
                        grdTCodes.DataSource = dispList;
                        ServiceExport(grdTCodes, fileformat);
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
       
        protected void grdTCodes_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointsystemid", EndPointSystemID.ToString());
                dispList = DomainApiClient<PresentationSAPTCodeReport>.GetByAction<List<PresentationSAPTCodeReport>>(DomainApiAction.Endpoint.Presentation.SAP.GetAllSapTCodesReportData,parameter);
                if (dispList != null)
                {
                    grdTCodes.DataSource = dispList;
                }
            }
            else
            {
                dispList = new List<PresentationSAPTCodeReport>();
                grdTCodes.DataSource = dispList;
            }
        }

        protected void DropDownSelectectionChanged(EndPoint EndPoint)
        {
            try
            {
                EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
                grdTCodes.Rebind();
            }
            catch (Exception ex)
            {
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdTCodes.ExportSettings.FileName = "Report-SAP-TCodes";
                Export(grdTCodes,e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdTCodes.Export<IList<PresentationSAPTCodeReport>>(dispList, e.ActionType, "Report-SAP-TCodes");
            }
        }
        #endregion

    }
}