using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure;
using DI.Chakra.Web.Report.MySchedule;

namespace DI.Chakra.Web.Report.Endpoint.SAPEP
{
    public partial class Roles : ReportBasePage
    {
        private static IList<SAPEPRoleInfo> dispList = new List<SAPEPRoleInfo>();
        private long EndPointSystemID
        {
            get
            {
                if (ViewState["EndPointSystemID"] != null)
                {
                    return (int)ViewState["EndPointSystemID"];
                }
                return 0;
            }
            set
            {
                ViewState["EndPointSystemID"] = value;
            }
        }
        protected new void Page_Load(object sender, EventArgs e)
        {
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
                        dispList = DomainApiClient<SAPEPRoleInfo>.GetByAction<List<SAPEPRoleInfo>>(DomainApiAction.Endpoint.SAPEP.SAPEPRoleInfo.GetByEndPointId, parameter);
                        grdRole.DataSource = dispList;
                        ServiceExport(grdRole, fileformat);
                    }
                    catch (Exception ex)
                    { }
                }
                grdRole.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
                grdRole.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
                grdRole.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
                EndpointUC.Initialize();
                EndpointUC.SelectedApplication =
                    new EndPointSystemType { EndpointSystemTypeName = Infrastructure.Enums.EndpointType.SAPEnterprisePortal };

                Dictionary<string, string> repparameter = new Dictionary<string, string>();
                repparameter.Add("id", EndpointUC.SelectedApplication.Id.ToString());
                var Endpoint = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>
                    (DomainApiAction.Endpoint.EndPointSystemType.GetByIdIncludingEndpoints, repparameter);

                if (Endpoint == null || Endpoint.EndPoints.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "First Create SAP EnterprisePortal Endpoint");
                }
            }
        }

        protected void DropDownSelectectionChanged(EndPoint EndPoint)
        {
            try
            {
                EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
                grdRole.Rebind();
            }
            catch (Exception ex)
            {
            }
        }



        protected void grdRole_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointsystemid", EndPointSystemID.ToString());
                dispList = DomainApiClient<SAPEPRoleInfo>.GetByAction<List<SAPEPRoleInfo>>(DomainApiAction.Endpoint.SAPEP.SAPEPRoleInfo.GetByEndPointId, parameter);
                if (dispList != null)
                {
                    grdRole.DataSource = dispList;
                }
            }
            else
            {
                dispList = new List<SAPEPRoleInfo>();
                grdRole.DataSource = dispList;
            }

        }
        
    }
}