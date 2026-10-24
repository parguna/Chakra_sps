using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Prov.SnapShot.UC
{
    public partial class ListUC : UIBaseUC
    {
        #region Members
        public long listUserID = 0;
        private const string V_RoleAccessDetail = "V_RoleAccessDetail";

        public IList<PresentationOUChangeSnapshots> RoleAccessDetails
        {
            get
            {
                if (ViewState[V_RoleAccessDetail] == null)
                    return new List<PresentationOUChangeSnapshots>();
                return (List<PresentationOUChangeSnapshots>)ViewState[V_RoleAccessDetail];
            }
            set
            {
                ViewState[V_RoleAccessDetail] = value;
            }
        }

        private XLDataConverterHelper xlHelper;

        public XLDataConverterHelper XlHelper
        {
            get
            {
                if (xlHelper == null)
                {
                    xlHelper = new XLDataConverterHelper();
                }
                return xlHelper;
            }
            set { xlHelper = value; }
        }
        #endregion

        #region Events
        protected void btnReprt_Click(object sender, EventArgs e)
        {
            if (UserSearchUC.SelectedUser != null)
            {
                GrdPanel.Visible = true;
                grdsnapshot.Rebind();
            }
        }
        protected void btnUndo_Click(object sender, EventArgs e)
        {
            if (UserSearchUC.SelectedUser == null)
                return;

            if ((!UserSearchUC.SelectedUser.FullTimeEmployee && !UserSearchUC.SelectedUser.IsActive) || (UserSearchUC.SelectedUser.TerminationDate.HasValue && UserSearchUC.SelectedUser.TerminationDate < DateTime.Now))
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Error, $"User {UserSearchUC.SelectedUser.KnownAs} is Inactive or Invalid Termination Date, hence undo cannot be performed.");
                return;
            }

            btnUndo.Enabled = false;
            if (chkOlder.Checked)
            {
                btnUndo.Visible = false;
                return;
            }
            var userid = UserSearchUC.SelectedUser.Id;
            var m_session = new SessionStatusImpl();
            var requestorid = m_session.UserId;

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("userID", userid.ToString());
            parameters.Add("requestorID", requestorid.ToString());

            var result = DomainApiClient<PresentationAccessSnapshots>.GetByAction<bool>(DomainApiAction.Provisioning.Presentation.PresentationAccessSnapshots.ExecuteUndoProvisioningEntitlements, parameters);
            if (result)
            {
                DILog.UILogger.WriteLog(requestorid, LogLevelType.Info, string.Format("Undo the OuTransfer/Offboard for {0} by {1}", UserSearchUC.SelectedUser.UserName, m_session.User.DisplayName));
                MasterPage.ShowAlert(Infrastructure.AlertType.Success, "Latest OrgTransfer/Offboard Undo event is Triggered");
            }
            else
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Error, "Latest OrgTransfer/Offboard Undo event is Failed");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            UserSearchUC.AllUsers = true;
            if (!Page.IsPostBack)
            {
                btnUndo.Visible = false;
            }
            XlHelper.InvisibleBetween(grdsnapshot.FilterMenu);
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                GetUserData();
                if (RoleAccessDetails != null && RoleAccessDetails.Count > 0)
                    GenericExport(RoleAccessDetails, e.ActionType, "AccessSnapshotDetails");
                else
                {
                    ShowAlertMessage(Infrastructure.AlertType.Error, "No data found for export");
                }
            }
        }

        protected void grdsnapshot_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GetUserData();
            if (RoleAccessDetails != null && RoleAccessDetails.Count > 0)
            {
                btnUndo.Visible = !chkOlder.Checked;
                grdsnapshot.DataSource = RoleAccessDetails.OrderByDescending(x => x.CreatedDate).ToList();
            }
            else
            {
                btnUndo.Visible = false;
                grdsnapshot.DataSource = new List<PresentationOUChangeSnapshots>();
            }
        }

        private void GetUserData()
        {
            long userId = 0;
            if (UserSearchUC.SelectedUser != null)
            {
                userId = Convert.ToInt64(UserSearchUC.SelectedUser.Id);
            }
            else
            {
                return;
            }


            List<long> lstUserIds = new List<long>();
            if (userId > 0)
                lstUserIds.Add(userId);
            if (chkOlder.Checked)
            {
                var param = new Dictionary<string, string>();
                param.Add("includeOldRecord", "true");
                RoleAccessDetails = DomainApiClient<Dto.Presentation.PresentationOUChangeSnapshots>.GetByAction<List<Dto.Presentation.PresentationOUChangeSnapshots>>(DomainApiAction.Provisioning.Presentation.PresentationOUChangeSnapshots.GetByUserId, param, lstUserIds);
            }
            else
            {
                RoleAccessDetails = DomainApiClient<Dto.Presentation.PresentationOUChangeSnapshots>.GetByAction<List<Dto.Presentation.PresentationOUChangeSnapshots>>(DomainApiAction.Provisioning.Presentation.PresentationOUChangeSnapshots.GetByUserId, null, lstUserIds);
            }
        }
        #endregion
        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Created Date", "CreatedDate", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Employeee Event", "EmployeeEventType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("User Name", "UserName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Employee Id", "EmployeeNo", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("EmailID", "EmailID", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("EndpointSystem Type", "EndpointSystemType", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Endpoint System Name", "EndpointsystemName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Account Name", "AccountName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Entitlement", "Entitlements", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Entitlement Type", "EntitlementType", null));
            return param;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void grdsnapshot_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // setting header text based on application level values
                foreach (GridColumn col in grdsnapshot.Columns)
                {
                    if (col.UniqueName == "EndpointSystemType")
                        col.HeaderText = T("App.Common.EndpointType");
                }
                grdsnapshot.Rebind();
            }
        }

        protected void chkOlder_CheckedChanged(object sender, EventArgs e)
        {
            if(UserSearchUC.SelectedUser != null && chkOlder.Checked)
            {
                btnUndo.Visible = false;
            }
            else
            {
                if(chkOlder.Checked)
                    btnUndo.Visible = false;
                else
                {
                    grdsnapshot.Rebind();
                }
            }
            
        }
    }
}
