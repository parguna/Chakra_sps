using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.Endpoint.INH
{
    public partial class Permissions : ReportBasePage
    {
        public InhousePermission inhgroup = new InhousePermission();
        private static List<InhousePermission> dispList;
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
        protected new  void Page_Load(object sender, EventArgs e)
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
                        parameter.Add("endpointSystemId", endpointsystemtypeid.ToString());
                        dispList = DomainApiClient<InhousePermission>.GetByAction<List<InhousePermission>>
                            (DomainApiAction.Endpoint.InhouseSystems.InhousePermission.GetByEndPointSystemId, parameter);
                        grdUser.DataSource = dispList;
                        ServiceExport(grdUser, fileformat);
                    }
                    catch (Exception ex)
                    { }
                }
                EndpointUC.Initialize();
                EndpointUC.SelectedApplication =
                    new EndPointSystemType { EndpointSystemTypeName = Infrastructure.Enums.EndpointType.InhouseSystems };

                Dictionary<string, string> repparameter = new Dictionary<string, string>();
                repparameter.Add("id", EndpointUC.SelectedApplication.Id.ToString());
                var Endpoint = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>
                    (DomainApiAction.Endpoint.EndPointSystemType.GetByIdIncludingEndpoints, repparameter);

                if (Endpoint == null || Endpoint.EndPoints.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "First Create Inhouse Endpoint");
                }
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
            }
        }

        protected void grdUser_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", EndPointSystemID.ToString());
                dispList = DomainApiClient<InhousePermission>.GetByAction<List<InhousePermission>>
                    (DomainApiAction.Endpoint.InhouseSystems.InhousePermission.GetByEndPointSystemId, parameter);
                if (dispList != null)
                {
                    grdUser.DataSource = dispList;
                }
            }
            else
            {
                dispList = new List<InhousePermission>();
                grdUser.DataSource = dispList;
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdUser.ExportSettings.FileName = "Report-INH-Permissions";
                Export(grdUser, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdUser.Export<List<InhousePermission>>(dispList, e.ActionType, "Report-INH-Permissions");
            }
        }

    }
}