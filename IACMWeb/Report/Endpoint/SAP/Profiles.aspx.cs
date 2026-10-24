using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using Telerik.Web.UI;
using System.Data;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.Web.Report.MySchedule;

namespace DI.Chakra.Web.Report.Endpoint.SAP
{
    public partial class Profiles : ReportBasePage
    {
        #region Members

        private static IList<PresentationSAPProfileReport> dispList = new List<PresentationSAPProfileReport>();

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
                        parameter.Add("endpointSystemId", endpointsystemtypeid);
                        dispList = DomainApiClient<PresentationSAPProfileReport>.GetByAction<List<PresentationSAPProfileReport>>(DomainApiAction.Endpoint.Presentation.SAP.GetSAPUserToProfilesReportData, parameter);
                        grdProfiles.DataSource = dispList;
                        ServiceExport(grdProfiles, fileformat);
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
       
        protected void grdProfiles_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", EndPointSystemID.ToString());
                dispList = DomainApiClient<PresentationSAPProfileReport>.GetByAction<List<PresentationSAPProfileReport>>(DomainApiAction.Endpoint.Presentation.SAP.GetSAPUserToProfilesReportData,parameter);
                if (dispList != null)
                {
                    grdProfiles.DataSource = dispList;
                }
            }
            else
            {
                dispList = new List<PresentationSAPProfileReport>();
                grdProfiles.DataSource = dispList;
            }
        }
        protected void DropDownSelectectionChanged(EndPoint EndPoint)
        {
            try
            {
                EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
                grdProfiles.Rebind();
            }
            catch (Exception ex)
            {
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdProfiles.ExportSettings.FileName = "Report-SAP-Profiles";
                Export(grdProfiles, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdProfiles.Export<IList<PresentationSAPProfileReport>>(dispList, e.ActionType, "Report-SAP-Profiles");
            }
        }
        #endregion

    }
}