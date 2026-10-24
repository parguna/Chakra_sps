using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Admin.Group.UC
{
    public partial class CloneUC : UIBaseUC, IAccessControl
    {
        #region Members

        private const string V_EntityID = "V_EntityID";
        private const string V_USERLIST = "V_USERLIST";
        private long UserId = 0;

        #endregion

        #region Properties

        public int EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return 0;
                return (int)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
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

        #region "Events"

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
                if (this.ContextObjectId.HasValue)
                {
                    this.EntityId = (int)this.ContextObjectId.Value;
                    SetPage();
                }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Group/List.aspx");
        }

        protected void GroupMemberGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = (GridDataItem)e.Item;
                this.UserId = Convert.ToInt64(item.GetDataKeyValue("Id"));
                delete();
             }
           

        }

        protected void GroupMemberGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            GroupMemberGrid.DataSource = UserList;
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                CloneGroup();
              
            }
        }

        protected void AddUser_Click(object sender, EventArgs e)
        {
            if (GroupMember.SelectedUser != null)
            {
                UserList = UserList;
                if (!UserList.Contains(GroupMember.SelectedUser))
                {
                    UserList.Add(GroupMember.SelectedUser);
                    GroupMemberGrid.Rebind();
                    GroupMember.Clear();
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "Group Member Already Exists.");
                }
            }
        }

        #endregion

        #region "Methods"

        protected void SetPage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", EntityId.ToString());
            var Group = DomainApiClient<Dto.Group>.GetByAction<Dto.Group>(DomainApiAction.Group.GetByIdIncludingUser, parameter);
           
            if (null == Group)
                return;
            GroupName.Text = Group.GroupName;
            GroupDescription.Text = Group.GroupDescription;
            CheckIsDefault.Checked = Group.IsDefaultGroup;
            Email.Text = Group.EmailId;

            GroupOwner.SelectedUser = Group.Owner;
            if (Group.Users.Count > 0)
            {
                UserList.Clear();
                //UserList = Group.Users;
                GroupMemberGrid.DataBind();
            }
        }

        private void CloneGroup()
        {
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "IDMS Group - Clone");
            Dto.Group group = new Dto.Group();

            group.GroupName = GroupName.Text.Trim();
            group.IsDefaultGroup = CheckIsDefault.Checked;
            group.GroupDescription = GroupDescription.Text;
            group.EmailId = Email.Text;

            if (string.IsNullOrEmpty(group.GroupName))
            {
                MasterPage.ShowAlert(AlertType.Error, "Role Name is required.");
                return;
            }

            if (GroupOwner.SelectedUser != null)
                group.GroupOwner = Convert.ToInt32(GroupOwner.SelectedUser.Id);
            group.Users = UserList;
           if ((DoesExist(GroupName.Text.Trim(), EntityId)))
            {
                MasterPage.ShowAlert(AlertType.Error, "Group Name Already Exists.");
                return;
            }

            DomainApiClient<Dto.Group>.Save(group);
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info,$"IDMS Group - Clone successfully. GroupName: {group.GroupName}");
            Response.Redirect("~/Admin/Group/List.aspx");
        }


        private bool DoesExist(string groupName, long groupID)
        {
            Dto.Group group = new Dto.Group();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("groupID", groupID.ToString());
            parameter.Add("groupName", groupName);
            return DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.DoesExist, parameter);
        }
        private void delete()
         {
           if (this.UserId > 0)
            {
            UserList.RemoveAt(UserList.FindIndex(user => user.Id == this.UserId));
            MasterPage.ShowAlert(AlertType.Success, "User deleted successfully");
            }
            GroupMemberGrid.Rebind();
         }
    #endregion

}
}