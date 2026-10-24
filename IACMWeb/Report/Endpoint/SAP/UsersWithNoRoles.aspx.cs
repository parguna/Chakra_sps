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
    public partial class UsersWithNoRoles : ReportBasePage
    {
        #region Members

        private static IList<PresentationSAPUserWithNoRoles> dispList = new List<PresentationSAPUserWithNoRoles>();

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
                        dispList = DomainApiClient<PresentationSAPUserWithNoRoles>.GetByAction<List<PresentationSAPUserWithNoRoles>>
                            (DomainApiAction.Endpoint.Presentation.SAP.GetSAPUsersWithNoRolesReportData, parameter);
                        grdUser.DataSource = dispList;
                        ServiceExport(grdUser, fileformat);
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
                parameter.Add("EndpointSystemId", EndPointSystemID.ToString());
                dispList = DomainApiClient<PresentationSAPUserWithNoRoles>.GetByAction<List<PresentationSAPUserWithNoRoles>>(DomainApiAction.Endpoint.Presentation.SAP.GetSAPUsersWithNoRolesReportData, parameter);
                if (dispList != null)
                {
                    grdUser.DataSource = dispList;
                }
            }
            else
            {
                dispList = new List<PresentationSAPUserWithNoRoles>();
                grdUser.DataSource = dispList;
            }
        }

        protected void DropDownSelectectionChanged(EndPoint EndPoint)
        {
            try
            {
                EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
                grdUser.Rebind();
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
                grdUser.ExportSettings.FileName = "Report-SAP-Users-With-No-Roles";
                Export(grdUser,e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdUser.Export<IList<PresentationSAPUserWithNoRoles>>(dispList, e.ActionType, "Report-ADS-Users");
            }
        }
        #endregion

    }
}