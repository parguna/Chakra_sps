using System.Web;
using System;
using System.Collections.Generic;
using System.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using Application;
using DI.Chakra.Logger;
using System.Linq;

namespace DI.Chakra.Web.Admin.Role.UC 
{
    public partial class DetailUC : UIBaseUC , IAccessControl
    {

        #region Members
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_USERLIST = "USERLIST";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private long UserId = 0;
        
        #endregion

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_ENTITYID] == null)
                    return 0;
                return (long)ViewState[V_ENTITYID];
            }
            set
            {
                ViewState[V_ENTITYID] = value;
            }
        }
        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.View;

                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }      
        public List<User> UserList
        {
            get
            {
                if (ViewState[V_USERLIST] == null)
                    return new List<User>();
                return (List<User>)ViewState[V_USERLIST];
            }
            set
            {
                ViewState[V_USERLIST] = value;
            }
        }
        #endregion


        #region Methods

        private void SetPage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", EntityId.ToString());
            var Role = DomainApiClient<Dto.Role>.GetByAction<Dto.Role>(DomainApiAction.ACL.Role.GetByIdIncludingUser, parameter);

            if (null == Role)
                return;
            RoleName.Text = Role.RoleName;
            RoleDescription.Text = Role.RoleDescription;
            CheckIsDefault.Checked = Role.IsDefaultRole;
            if (Role.IsDefaultRole)
            {
                RoleOwner.Disabled = false;
                CheckIsDefault.Enabled = false;
                CheckIsDefault.InputAttributes.Add("disabled", "true");
                RoleName.Enabled = false;
            }
            RoleOwner.SelectedUser = Role.Owner;
            if (Role.Users.Count > 0)
            {
                UserList.Clear();
                UserList = Role.Users;
                RoleMemberGrid.DataBind();
            }
        }

        private void Save()
        {
            System.Text.StringBuilder logMessage = new System.Text.StringBuilder();
            Dto.Role Role = new Dto.Role();
            Role.Id = EntityId;
            Role.RoleName = RoleName.Text.Trim();
            Role.IsDefaultRole = CheckIsDefault.Checked;
            Role.RoleDescription = RoleDescription.Text;
            if (Role.Id == 0)
            {
                logMessage.AppendLine($"New Role:{Role.RoleName}");
            }
           
            if (string.IsNullOrWhiteSpace(Role.RoleName))
            {
                MasterPage.ShowAlert(AlertType.Error, "Role Name is required.");
                return;
            }

            if (RoleOwner.SelectedUser != null)
                Role.RoleOwner = Convert.ToInt32(RoleOwner.SelectedUser.Id);
            Role.Users = UserList;
            if ((UIMode == UIMode.Add) && (DoesExist(RoleName.Text.Trim(), EntityId)))
            {
                MasterPage.ShowAlert(AlertType.Error, "Role Name Already Exists.");
                return;
            }
            else if (UIMode == UIMode.Edit)
            {
                logMessage.AppendLine($"Update Role:{Role.RoleName}");
                var dbRole = DomainApiClient<Dto.Role>.GetById(EntityId,true);
                if (dbRole.RoleName != RoleName.Text.Trim())
                {
                    if (DoesExist(RoleName.Text.Trim(), EntityId))
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Role Name Already Exists.");
                        return;
                    }
                    logMessage.AppendLine($"RoleName: NewValue: {Role.RoleName} OldValue: {dbRole.RoleName}");
                }
                if (Role.RoleDescription != dbRole.RoleDescription)
                {
                    logMessage.AppendLine($"RoleDescription: NewValue: {Role.RoleDescription} OldValue: {dbRole.RoleDescription}");
                }
                if (Role.IsDefaultRole != dbRole.IsDefaultRole)
                {
                    logMessage.AppendLine($"IsDefaultRole: NewValue: {Role.IsDefaultRole} OldValue: {dbRole.IsDefaultRole}");
                }
                if (Role.RoleOwner != dbRole.RoleOwner)
                {
                    logMessage.AppendLine($"RoleOwner: NewValue: {RoleOwner?.SelectedUser} OldValue: {dbRole.Owner?.DisplayName}");
                }
                if (Role != null && dbRole != null && (Role.Users.Count > 0 || dbRole.Users.Count > 0))
                {
                    var oldList = dbRole.Users.Select(i => i.DisplayName).OrderBy(i => i).ToList();
                    var newList = Role.Users.Select(i => i.DisplayName).OrderBy(i => i).ToList();
                    var any = newList.Except(oldList).Count() > 0 || oldList.Except(newList).Count() > 0;
                    if ((oldList.Count > 0 || newList.Count > 0) && any)
                        logMessage.AppendLine($"Group Member: NewValue: {string.Join(",", newList)} OldValue: {string.Join(",", oldList)}");
                }
            }

            DomainApiClient<Dto.Role>.Save(Role);
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, logMessage.ToString());
            Response.Redirect("~/Admin/Role/List.aspx");
        }
        private void ViewOnlyMode()
        {
            ContentPanel.Enabled = false;
            memberDetails.Visible = true;
            //RoleMemberGrid.Enabled = false;
        }
        private void delete()
        {
            UserList.RemoveAt(UserList.FindIndex(user => user.Id == this.UserId));
            MasterPage.ShowAlert(AlertType.Success, "User deleted successfully");
            RoleMemberGrid.Rebind();
        }

        private bool DoesExist(string roleName, long roleId)
        {
            Dto.Role role = new Dto.Role();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("rolename", roleName);
            parameter.Add("roleId", EntityId.ToString());
            return DomainApiClient<Dto.Role>.GetByAction<bool>(DomainApiAction.ACL.Role.DoesExists, parameter);
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                //memberDetails.Visible = false;
                if (UIMode==UIMode.Edit ||UIMode== UIMode.View)
                {
                    this.EntityId = this.ContextObjectId != null ? (int)this.ContextObjectId.Value : 0;
                    SetPage();
                }

                if (UIMode==UIMode.View)
                    ViewOnlyMode();                
            }
        }       
        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Role/List.aspx");
        }
        protected void AddUser_Click(object sender, EventArgs e)
        {
            var selectedUser = RoleMember.SelectedUser;
            if (selectedUser != null)
            {
                UserList = UserList;
                User usr = UserList.Find(m => m.Id == selectedUser.Id);
                if (usr == null)
                {
                    UserList.Add(RoleMember.SelectedUser);
                    RoleMemberGrid.Rebind();
                    RoleMember.Clear();
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "Role Member Already Exists.");
                }
            }
        }
        protected void RoleMemberGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            RoleMemberGrid.DataSource = UserList;
        }
        protected void RoleMemberGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = (GridDataItem)e.Item;
                this.UserId = Convert.ToInt64(item.GetDataKeyValue("Id"));
                delete();
            }
        }
        protected void RoleMemberGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RoleMemberGrid.SelectedItems[0] as GridDataItem;
            this.UserId = (int)g.GetDataKeyValue("Id");
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "IDMS Role - Save");
                Save();
            }
            else if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "IDMS Role - Delete");
                delete();
            }
        }
        #endregion
    }
}