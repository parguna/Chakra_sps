using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Configuration.Role
{
    public partial class ChangeAccessRights : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        #region Members
        private const string V_ID = "ENTITY_ID";
        private const string V_ACCESSRIGHTS = "ACCESSRIGHTS";
        private const string V_PAGEPERMISSION_IDS = "V_PAGEPERMISSION_IDS";

        private List<AccessRights> Rights = new List<AccessRights>();

        #endregion

        #region Properties
        public int EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return 0;
                return (int)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }
        public List<long> PageIdsToBeSaved
        {
            get
            {
                if (this.ViewState[V_PAGEPERMISSION_IDS] == null)
                {
                    this.ViewState[V_PAGEPERMISSION_IDS] = new List<long>();
                }
                return (List<long>)(this.ViewState[V_PAGEPERMISSION_IDS]);
            }
            set
            {
                ViewState[V_PAGEPERMISSION_IDS] = value;
            }
        }

        public IList<AccessRights> AccessRights
        {
            get
            {
                if (ViewState[V_ACCESSRIGHTS] == null)
                    return new List<AccessRights>();
                return (IList<AccessRights>)ViewState[V_ACCESSRIGHTS];
            }
            set
            {
                ViewState[V_ACCESSRIGHTS] = value;
            }
        }
        #endregion

        #region Methods
        private void BindMenu()
        {
            Menu.DataSource = DomainApiClient<Dto.TabMenu>.GetAll();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            EntityId = Convert.ToInt32(ContextObjectId);
            parameter.Add("roleId", Convert.ToString(EntityId));
            AccessRights = DomainApiClient<Dto.AccessRights>.GetByAction<List<Dto.AccessRights>>(DomainApiAction.ACL.AccessRights.GetAccessRightsByRoleId, parameter);
            Menu.DataBind();
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                EntityId = Convert.ToInt32(ContextObjectId);
                Dto.Role role = DomainApiClient<Dto.Role>.GetById(EntityId);
                if (role != null)
                {
                    lblRoleName.Text = role.RoleName;
                }
                BindMenu();
            }
            this.MasterPage.ActionClicked += ActionClicked;
        }
        protected void Menu_NodeDataBound(object sender, RadTreeNodeEventArgs child)
        {
            var TabMenu = (TabMenu)child.Node.DataItem;
            var Count = AccessRights.Where(lq => lq.PagePermission != null && lq.PagePermission.PageId == TabMenu.PageId).Count();
            if (Count > 0)
                child.Node.Checked = true;
        }
        protected void Menu_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("pageId", e.Node.Value);
            ActionRightsUC.Visible = true;
            var Permission = DomainApiClient<Dto.PagePermission>.GetByAction<List<PagePermission>>(DomainApiAction.ACL.PagePermission.GetPagePermissions, parameter, null);
            PageIdsToBeSaved.Add(Convert.ToInt64(e.Node.Value));
            Permission.ForEach(lq =>
                {
                    var RightsFortheRole = AccessRights.Where(l => (l.PagePermissionId == lq.Id || l.PagePermissionId == -lq.Id) && l.RoleId == EntityId).FirstOrDefault();
                    if (RightsFortheRole == null)
                    {
                        AccessRights.Add(new AccessRights() { PagePermission = lq, PagePermissionId = (int)-lq.Id, RoleId = EntityId });
                        Rights.Add(new AccessRights() { PagePermission = lq, PagePermissionId = (int)-lq.Id, RoleId = EntityId });
                    }
                    else
                        Rights.Add(RightsFortheRole);
                });
            ActionRightsUC.CurrentAccessRights = Rights;
            ActionRightsUC.Initialize(Convert.ToInt32(e.Node.Value));
        }
        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();

            if (e.ActionType == ACLAction.Save)
            {
                try
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Save IDMS role access- Role Name:{lblRoleName.Text}");
                    foreach (RadTreeNode node in Menu.GetAllNodes())
                    {
                        if (node.Checked && !PageIdsToBeSaved.Contains(Convert.ToInt64(node.Value)))
                            continue;

                        parameter.Clear();
                        parameter.Add("pageId", node.Value);
                        var Permission = DomainApiClient<Dto.PagePermission>.GetByAction<List<PagePermission>>(DomainApiAction.ACL.PagePermission.GetPagePermissions, parameter, null);
                        foreach (var item in Permission)
                        {

                            //}
                            //Permission.ForEach(lq =>
                            //    {
                            var AccessRight = this.AccessRights.Where(ar => ar.RoleId == EntityId && (ar.PagePermissionId == item.Id || ar.PagePermissionId == -item.Id)).FirstOrDefault();
                            if (AccessRight != null)
                            {
                                List<PagePermission> dbPagePermission = null;
                                if (AccessRight.PagePermission.UrlId != null)
                                {
                                    Dictionary<string, string> parameter1 = new Dictionary<string, string>();
                                    parameter1.Add("urlId", AccessRight.PagePermission.UrlId.ToString());
                                    dbPagePermission = DomainApiClient<PagePermission>.GetByAction<List<PagePermission>>(DomainApiAction.ACL.PagePermission.GetByUrlId, parameter1);
                                }
                                if (!node.Checked)
                                {
                                    var orgValue = AccessRight.PagePermissionId;
                                    AccessRight.PagePermissionId = AccessRight.PagePermissionId > 0 ? AccessRight.PagePermissionId : -AccessRight.PagePermissionId;
                                    DomainApiClient<Dto.AccessRights>.GetByAction<bool>(DomainApiAction.ACL.AccessRights.DeleteAccessRights, null, AccessRight);
                                    AccessRight.PagePermissionId = orgValue;
                                }
                                else
                                {
                                    if (AccessRight.Id == 0)
                                    {
                                        if (AccessRight.PagePermission != null && AccessRight.PagePermissionId > 0)
                                        {
                                            DomainApiClient<Dto.AccessRights>.Save(AccessRight);
                                        }
                                    }
                                    else
                                    {
                                        if (AccessRight.PagePermission != null && AccessRight.PagePermissionId < 0)
                                        {
                                            var orgValue = AccessRight.PagePermissionId;
                                            AccessRight.PagePermissionId = AccessRight.PagePermissionId > 0 ? AccessRight.PagePermissionId : -AccessRight.PagePermissionId;
                                            DomainApiClient<Dto.AccessRights>.GetByAction<bool>(DomainApiAction.ACL.AccessRights.DeleteAccessRights, null, AccessRight);
                                            AccessRight.PagePermissionId = orgValue;
                                        }
                                    }
                                }
                                if (dbPagePermission != null && dbPagePermission.Count > 0)
                                {
                                    if (AccessRight.PagePermission.PermissionId != 1 && AccessRight.PagePermissionId > 0 && AccessRight.PagePermission.PermissionId != 12)
                                    {
                                        bool isAddPermission = node.Checked ? true : false;
                                        Dictionary<string, string> parameter1 = new Dictionary<string, string>();
                                        parameter1.Add("isAddPermission", isAddPermission.ToString());
                                        parameter1.Add("urlId", AccessRight.PagePermission.UrlId.ToString());
                                        parameter1.Add("roleID", AccessRight.RoleId.ToString());
                                        DomainApiClient<PagePermission>.GetByAction<bool>(DomainApiAction.ACL.PagePermission.UpdateUrlPermission, parameter1);
                                    }
                                    //AccessRights accessRightsInfo = this.AccessRights.Where(x => (x.PagePermissionId == dbPagePermission.Id || x.PagePermissionId == -dbPagePermission.Id) && x.RoleId == EntityId).FirstOrDefault();
                                    //if (accessRightsInfo != null)
                                    //{
                                    //    var orgValue = accessRightsInfo.PagePermissionId;
                                    //    accessRightsInfo.PagePermissionId = accessRightsInfo.PagePermissionId > 0 ? accessRightsInfo.PagePermissionId : -accessRightsInfo.PagePermissionId;

                                    //    if (!node.Checked)
                                    //        DomainApiClient<Dto.AccessRights>.GetByAction<bool>(DomainApiAction.ACL.AccessRights.DeleteAccessRights, null, accessRightsInfo);
                                    //    else
                                    //        DomainApiClient<Dto.AccessRights>.Save(accessRightsInfo);

                                    //    accessRightsInfo.PagePermissionId = orgValue;
                                    //}
                                }
                            }
                        }
                    }
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.DebugStep, $"Save IDMS role access successfully.-RoleName:{lblRoleName.Text}");
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
                BindMenu();
                ActionRightsUC.Visible = false;
                Response.Redirect("List.aspx");
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("roleId", Convert.ToString(ContextObjectId));
                var Permission = DomainApiClient<Dto.PresentationAccessRights>.GetByAction<List<PresentationAccessRights>>(DomainApiAction.ACL.AccessRights.GetExportData, parameters, null);
                GenericExport(Permission, e.ActionType, "AcessRights");
            }
        }

        protected void UpdateAccessRight(AccessRights UpdateAccessRight)
        {
            var ExistRecord = this.AccessRights.Where(l => l.RoleId == UpdateAccessRight.RoleId && (l.PagePermissionId != null && (l.PagePermissionId == UpdateAccessRight.PagePermissionId || l.PagePermissionId == -UpdateAccessRight.PagePermissionId))).FirstOrDefault();
            if (ExistRecord != null)
                this.AccessRights.Remove(ExistRecord);
            this.AccessRights.Add(UpdateAccessRight);
        }

        #endregion

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Role/List.aspx");
        }

        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Role Name", "rolename", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Menu Path", "menupath", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Permissions", "permissions", null));
            return param;
        }

        protected void ExportExcelButton_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("roleId", Convert.ToString(ContextObjectId));
            var Permission = DomainApiClient<Dto.PresentationAccessRights>.GetByAction<List<PresentationAccessRights>>(DomainApiAction.ACL.AccessRights.GetExportData, parameters, null);
            GenericExport(Permission, ACLAction.Excel, "AcessRights");
        }

        protected void ExportCSVButton_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("roleId", Convert.ToString(ContextObjectId));
            var Permission = DomainApiClient<Dto.PresentationAccessRights>.GetByAction<List<PresentationAccessRights>>(DomainApiAction.ACL.AccessRights.GetExportData, parameters, null);
            GenericExport(Permission, ACLAction.CSV, "AcessRights");
        }
    }
}