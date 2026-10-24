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
    public partial class RolesWithNoUsers :ReportBasePage
    {
        #region Members

        private static IList<PresentationSAPRoleWithNoUsers> dispList = new List<PresentationSAPRoleWithNoUsers>();

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
                        parameter.Add("EndpointSystemId", endpointsystemtypeid);
                        dispList = DomainApiClient<PresentationSAPRoleWithNoUsers>.GetByAction<List<PresentationSAPRoleWithNoUsers>>(DomainApiAction.Endpoint.Presentation.SAP.GetSAPRolesWithNoUsersReportData, parameter);
                        grdRoles.DataSource = dispList;
                        ServiceExport(grdRoles, fileformat);
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
       
        protected void grdUser_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", EndPointSystemID.ToString());
                dispList = DomainApiClient<PresentationSAPRoleWithNoUsers>.GetByAction<List<PresentationSAPRoleWithNoUsers>>(DomainApiAction.Endpoint.Presentation.SAP.GetSAPRolesWithNoUsersReportData, parameter);
                grdRoles.DataSource = dispList;
            }
            else
            {
                dispList = new List<PresentationSAPRoleWithNoUsers>();
                grdRoles.DataSource = dispList;
            }
        }
        protected void DropDownSelectectionChanged(EndPoint EndPoint)
        {
            try
            {
                EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
                grdRoles.Rebind();
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
                grdRoles.ExportSettings.FileName = "Report-SAP-Roles-With-No-Users";
                Export(grdRoles, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdRoles.Export<IList<PresentationSAPRoleWithNoUsers>>(dispList, e.ActionType, "Report-SAP-Roles-With-No-Users");
            }
        }
        #endregion

    }
}