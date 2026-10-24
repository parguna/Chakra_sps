using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto;
using Telerik.Web.UI;
using System.Data;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.Web.Report.MySchedule;

namespace DI.Chakra.Web.Report.Endpoint.SAP
{
    public partial class Groups : ReportBasePage
    {
        #region Members

        private static IList<SAPGroupInfo> dispList = new List<SAPGroupInfo>();

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
                        parameter.Add("endPointSystemId", endpointsystemtypeid);
                        dispList = DomainApiClient<SAPGroupInfo>.GetByAction<List<SAPGroupInfo>>(DomainApiAction.Endpoint.SAP.SAPGroupInfo.GetByEndPointSystemId, parameter);                        
                        grdGroups.DataSource = dispList;
                        ServiceExport(grdGroups, fileformat);
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
        protected void grdGroups_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {

            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endPointSystemId", EndPointSystemID.ToString());
                dispList = DomainApiClient<SAPGroupInfo>.GetByAction<List<SAPGroupInfo>>(DomainApiAction.Endpoint.SAP.SAPGroupInfo.GetByEndPointSystemId, parameter);
                if (dispList != null)
                {
                    grdGroups.DataSource = dispList;
                }
            }
            else
            {
                dispList = new List<SAPGroupInfo>();
                grdGroups.DataSource = dispList;
            }
        }

        protected void DropDownSelectectionChanged(EndPoint EndPoint)
        {
            try
            {
                EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
                grdGroups.Rebind();
            }
            catch (Exception ex)
            {
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdGroups.ExportSettings.FileName = "Report-SAP-Groups";
                grdGroups.Export(grdGroups, e.ActionType);
            }

            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdGroups.Export<IList<SAPGroupInfo>>(dispList, e.ActionType, "Report-SAP-Groups");
            }
        }
        #endregion

    }
}