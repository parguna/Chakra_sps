using System;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.Report.Endpoint.ADS
{
    public partial class OU : ReportBasePage
    {
        #region Members
        private static List<PresentationADSOUs> list = new List<PresentationADSOUs>();
        private string V_ENDPOINTSYSTEMID = "V_ENDPOINTSYSTEMID";
        #endregion

        #region Property
        private long EndPointSystemID
        {
            get
            {
                if (ViewState[V_ENDPOINTSYSTEMID] != null)
                {
                    return (long)ViewState[V_ENDPOINTSYSTEMID];
                }
                return 0;
            }
            set
            {
                ViewState[V_ENDPOINTSYSTEMID] = value;
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
                        parameter.Add("endpointsystemid", endpointsystemtypeid);
                        List<PresentationReportProcess> reportList = new List<PresentationReportProcess>();
                        list = DomainApiClient<PresentationADSOUs>.GetByAction<List<PresentationADSOUs>>
                    (DomainApiAction.Endpoint.Presentation.AD.GetByEndPointSystemId, parameter);
                        grdUser.DataSource = list;
                        ServiceExport(grdUser, fileformat);
                    }
                    catch (Exception ex)
                    { }
                }
                EndpointUC.Initialize();
                EndpointUC.SelectedApplication = new EndPointSystemType
                { EndpointSystemTypeName = DI.Chakra.Infrastructure.Enums.EndpointType.ActiveDirectory };
              
            }
        }
        protected void grdUser_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointsystemid", EndPointSystemID.ToString());
                list = DomainApiClient<PresentationADSOUs>.GetByAction<List<PresentationADSOUs>>
                    (DomainApiAction.Endpoint.Presentation.AD.GetByEndPointSystemId, parameter);
                grdUser.DataSource = list;
            }
            else
            {
                list = new List<PresentationADSOUs>();
                grdUser.DataSource = list;
            }
        }

        protected void EndpointUC_EndpointChanged(EndPoint EndPoint)
        {
            EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
            grdUser.Rebind();
        }


        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdUser.ExportSettings.FileName = "Report-ADS-OU";
                Export(grdUser, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdUser.Export<List<PresentationADSOUs>>(list, e.ActionType, "Report-ADS-Users");
            }
        }
        #endregion
       
    }
}