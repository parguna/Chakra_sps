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
using DI.Chakra.Web.Report.MySchedule;

namespace DI.Chakra.Web.Report.Endpoint.SAP
{
    public partial class GroupUsers : ReportBasePage
    {
        private static List<PresentationSAPUsersToGroups> UsersToGroups;
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
                        UsersToGroups = (List<PresentationSAPUsersToGroups>)DomainApiClient<PresentationSAPUsersToGroups>.
                            GetByAction<List<PresentationSAPUsersToGroups>>(DomainApiAction.Endpoint.Presentation.SAP.
                            GetSAPUserToGroupsReportData, null);
                        grdUser.DataSource = UsersToGroups;
                        ServiceExport(grdUser, fileformat);
                    }
                    catch (Exception ex)
                    { }
                }
            }
        }

        protected void grdUser_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            UsersToGroups = (List<PresentationSAPUsersToGroups>)DomainApiClient< PresentationSAPUsersToGroups>.GetByAction<List< PresentationSAPUsersToGroups>>(DomainApiAction.Endpoint.Presentation.SAP.GetSAPUserToGroupsReportData,null);
            if (UsersToGroups != null) grdUser.DataSource = UsersToGroups;
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            grdUser.Rebind();
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdUser.ExportSettings.FileName = "Report-SAP-GroupUsers";
                grdUser.Export(grdUser, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdUser.Export<List<PresentationSAPUsersToGroups>>(UsersToGroups, e.ActionType, "Report-SAP-GroupUsers");
            }
        }
    }
}