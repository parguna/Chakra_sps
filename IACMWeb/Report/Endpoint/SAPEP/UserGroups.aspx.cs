using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto.Presentation;
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
    public partial class UserGroups : ReportBasePage
    {
        private static IList<PresentationSAPEPUserToGroups> dispList = new List<PresentationSAPEPUserToGroups>();
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
                        dispList = DomainApiClient<PresentationSAPEPUserToGroups>.GetByAction<List<PresentationSAPEPUserToGroups>>(DomainApiAction.Endpoint.Presentation.SAPEP.GetSAPEPUserToGroupsReportData, parameter);
                        grdUser.DataSource = dispList;
                        ServiceExport(grdUser, fileformat);
                    }
                    catch (Exception ex)
                    { }
                }
                grdUser.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
                grdUser.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
                grdUser.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
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
                parameter.Add("endpointsystemid", EndPointSystemID.ToString());
                dispList = DomainApiClient<PresentationSAPEPUserToGroups>.GetByAction< List<PresentationSAPEPUserToGroups>>(DomainApiAction.Endpoint.Presentation.SAPEP.GetSAPEPUserToGroupsReportData,parameter);
                if (dispList != null)
                {
                    grdUser.DataSource = dispList;
                }
            }
            else
            {
                dispList = new List<PresentationSAPEPUserToGroups>();
                grdUser.DataSource = dispList;
            }

        }
       
    }
}