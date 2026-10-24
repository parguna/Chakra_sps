using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Logger;
using Application;

namespace DI.Chakra.Web.Admin.Role.UC
{
    public partial class CloneUC : UIBaseUC, IAccessControl
    {

        #region Members
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_USERLIST = "USERLIST";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        public long UserId = 0;

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

            RoleOwner.SelectedUser = Role.Owner;
            if (Role.Users.Count > 0)
            {
                UserList.Clear();
                //UserList = Role.Users;
                RoleMemberGrid.DataBind();
            }
        }

        private void CloneRole()
        {
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "IDMS Role - Clone");
            Dto.Role role = new Dto.Role();
            role.RoleName = RoleName.Text.Trim();
            role.IsDefaultRole = CheckIsDefault.Checked;

            role.RoleDescription = RoleDescription.Text;

            if (string.IsNullOrEmpty(role.RoleName))
            {
                MasterPage.ShowAlert(AlertType.Error, "Role Name is required.");
                return;
            }

            if (RoleOwner.SelectedUser != null)
                role.RoleOwner = Convert.ToInt32(RoleOwner.SelectedUser.Id);

            role.Users = UserList;

            if (DoesExist(RoleName.Text.Trim(), EntityId))
            {
                MasterPage.ShowAlert(AlertType.Error, "Role Name Already Exists.");
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("cloneMenu", Convert.ToString(chkMenu.Checked));
            parameters.Add("cloneQL", Convert.ToString(chkQL.Checked));
            parameters.Add("cloneDB", Convert.ToString(chkDashBoard.Checked));
            parameters.Add("cloneRoleID", Convert.ToString(EntityId));
            DomainApiClient<AccessRights>.GetByAction<long>(DomainApiAction.ACL.AccessRights.CloneRole, parameters, role);
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"IDMS Role - Clone successfully. RoleName:{role.RoleName}");
            Response.Redirect("~/Admin/Role/List.aspx");
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                if (this.ContextObjectId.HasValue)
                {
                    this.EntityId = (int)this.ContextObjectId.Value;
                    SetPage();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Role/List.aspx");
        }

        protected void AddUser_Click(object sender, EventArgs e)
        {
            if (RoleMember.SelectedUser != null)
            {
                UserList = UserList;
                if (!UserList.Contains(RoleMember.SelectedUser))
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

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                CloneRole();
              
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
            this.UserId = g.GetDataKeyValue("Id") != null ? Convert.ToInt64(g.GetDataKeyValue("Id")) : -1 ;
        }

        #endregion
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
    }
}