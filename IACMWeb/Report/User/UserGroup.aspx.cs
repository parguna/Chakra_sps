using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Report.User
{
    public partial class UserGroup : ReportBasePage
    {
        private static IList<PresentationUser> ADUserList = new List<PresentationUser>();
        private const string V_UserGroup = "GroupUser";
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        public string Usergroupmap
        {
            get
            {
                if (ViewState[V_UserGroup] == null)
                    return "GroupUser";
                return (string)ViewState[V_UserGroup];
            }
            set
            {
                ViewState[V_UserGroup] = value;
            }
        }

        protected void Usergroup_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (Usergroupmap.Equals("GroupUser"))
                ADUserList = DomainApiClient<Dto.Presentation.PresentationUser>
              .GetByAction<List<Dto.Presentation.PresentationUser>>(DomainApiAction.Endpoint.Presentation.PresentationUser.GetUserGroup, null);
            UsergroupGrid.DataSource = ADUserList;

            

        }

        protected void ExportGrid(ACLAction action)
        {
            var RadGrid1 = new RadGrid();

            //remove the below code to dump all the columns, for now added the needed columns.
            RadGrid1.AutoGenerateColumns = false;

            GridBoundColumn boundColumn = new GridBoundColumn();
            boundColumn.DataField = "UserName";
            boundColumn.HeaderText = "User Name";
            RadGrid1.MasterTableView.Columns.Add(boundColumn);
            boundColumn = new GridBoundColumn();
            boundColumn.DataField = "DisplayName";
            boundColumn.HeaderText = "Display Name";
            RadGrid1.MasterTableView.Columns.Add(boundColumn);
            boundColumn = new GridBoundColumn();
            boundColumn.DataField = "GroupName";
            boundColumn.HeaderText = "Group Name";
            RadGrid1.MasterTableView.Columns.Add(boundColumn);
            boundColumn = new GridBoundColumn();
            boundColumn.DataField = "Description";
            boundColumn.HeaderText = "Group Description";
            RadGrid1.MasterTableView.Columns.Add(boundColumn);
            //remove the above code to dump all the columns, for now added the needed columns.

            RadGrid1.ID = string.Format("{0}_{1}", "Report-IDAS-UserGroups", DateTime.Now.Date.ToString());
            RadGrid1.DataSource = ADUserList;
            RadGrid1.Rebind();
            Page.Controls.Add(RadGrid1); // It will not add control in your page

            RadGrid1.ExportSettings.FileName = "Report-IDAS-UserGroups";
            RadGrid1.ExportSettings.IgnorePaging = true;
            RadGrid1.ExportSettings.ExportOnlyData = true;
            RadGrid1.ExportSettings.OpenInNewWindow = true;
            RadGrid1.ExportSettings.HideStructureColumns = true;
            RadGrid1.ExportSettings.SuppressColumnDataFormatStrings = true;

            RadGrid1.GridLines = GridLines.Both;

            if (action == ACLAction.Excel)
                RadGrid1.MasterTableView.ExportToExcel();
            else if (action == ACLAction.CSV)
                RadGrid1.MasterTableView.ExportToCSV();
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.ExcelAll)
            {
                //UsergroupGrid.ExportSettings.FileName = "GroupUser";
                //Export(UsergroupGrid, e.ActionType);
                UsergroupGrid.Export<IList<PresentationUser>>(ADUserList, e.ActionType, "GroupUser");
            }
            else if (e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.CSVAll)
            {
                UsergroupGrid.Export<IList<PresentationUser>>(ADUserList, e.ActionType, "GroupUser");
            }
        }
    }
}