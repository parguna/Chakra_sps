using System;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using System.Collections.Generic;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Report.Endpoint.Audit
{
    public partial class RoleAccess : ReportBasePage
    {
        #region Members
        private static IList<PresentationRoleAccess> roleAccessList = new List<PresentationRoleAccess>();
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
                
                    try
                    {
                        base.Page_Load(sender, e);
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("endpointsystemid", EndPointSystemID.ToString());
                        roleAccessList = DomainApiClient<Dto.Presentation.PresentationRoleAccess>
                        .GetByAction<List<Dto.Presentation.PresentationRoleAccess>>(DomainApiAction.Endpoint.Presentation.PresentationRoleAccess.GetAccessToRoles, null);
                        grdUser.DataSource = roleAccessList;
                        //ServiceExport(grdUser, fileformat);
                    }
                    catch (Exception ex)
                    { }
                }
        }

        protected void ExportGrid(ACLAction action)
        {
            var RadGrid1 = new RadGrid();
            
            //remove the below code to dump all the columns, for now added the needed columns.
            RadGrid1.AutoGenerateColumns = false;

            GridBoundColumn boundColumn = new GridBoundColumn();
            boundColumn.DataField = "RoleName";
            boundColumn.HeaderText = "Role Name";
            RadGrid1.MasterTableView.Columns.Add(boundColumn);
            //boundColumn = new GridBoundColumn();
            //boundColumn.DataField = "DisplayName";
            //boundColumn.HeaderText = "Display Name";
            //RadGrid1.MasterTableView.Columns.Add(boundColumn);
            boundColumn = new GridBoundColumn();
            boundColumn.DataField = "PageName";
            boundColumn.HeaderText = "Page Name";
            RadGrid1.MasterTableView.Columns.Add(boundColumn);
            boundColumn = new GridBoundColumn();
            boundColumn.DataField = "AccessRights";
            boundColumn.HeaderText = "Access Rights";
            RadGrid1.MasterTableView.Columns.Add(boundColumn);
            //remove the above code to dump all the columns, for now added the needed columns.

            RadGrid1.ID = string.Format("{0}_{1}", "Report-RoleAccessMatrix", DateTime.Now.Date.ToString());
            RadGrid1.DataSource = roleAccessList;
            RadGrid1.Rebind();
            Page.Controls.Add(RadGrid1); // It will not add control in your page

            RadGrid1.ExportSettings.FileName = "Report-RoleAccessMatrix";
            RadGrid1.ExportSettings.IgnorePaging = true;
            RadGrid1.ExportSettings.ExportOnlyData = true;
            RadGrid1.ExportSettings.OpenInNewWindow = true;
            RadGrid1.ExportSettings.HideStructureColumns = true;
            RadGrid1.ExportSettings.SuppressColumnDataFormatStrings = true;

            RadGrid1.GridLines = GridLines.Both;

            if(action == ACLAction.Excel)
                RadGrid1.MasterTableView.ExportToExcel();
            else if(action == ACLAction.CSV)
                RadGrid1.MasterTableView.ExportToCSV();
        }

        protected void grdUser_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endpointsystemid", EndPointSystemID.ToString());
            roleAccessList = DomainApiClient<Dto.Presentation.PresentationRoleAccess>
                .GetByAction<List<Dto.Presentation.PresentationRoleAccess>>(DomainApiAction.Endpoint.Presentation.PresentationRoleAccess.GetAccessToRoles, null);
            grdUser.DataSource = roleAccessList;
        }

        protected void grdUser_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            e.DetailTableView.DataSource = grdUser.DataSource;
        }


        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.ExcelAll)
            {
                ExportGrid(ACLAction.Excel);
            }
            else if (e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.CSVAll)
            {
                ExportGrid(ACLAction.CSV);
            }
        }
        #endregion

    }
}   