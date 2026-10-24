using System;
using System.Collections.Generic;
using System.Web.UI;
using DI.Chakra.Infrastructure;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using Application;
using DI.Chakra.Logger;
using System.Linq;

namespace DI.Chakra.Web.Admin.Group.UC
{
    public partial class DetailUC : UIBaseUC , IAccessControl
    {
        #region Members

        private const string V_EntityID = "V_EntityID";
        private const string V_USERLIST = "V_USERLIST";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
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
            var Group = DomainApiClient<Dto.Group>.GetByAction<Dto.Group>(DomainApiAction.Group.GetByIdIncludingUser, parameter);

            if (null == Group)
                return;
            GroupName.Text = Group.GroupName;
            GroupDescription.Text = Group.GroupDescription;
            Email.Text = Group.EmailId;
            CheckIsDefault.Checked = Group.IsDefaultGroup;
            if (Group.IsDefaultGroup)
            {
                GroupOwner.Disabled = false;
                CheckIsDefault.Enabled = false;
                CheckIsDefault.InputAttributes.Add("disabled", "true");
                GroupName.Enabled = false;
            }
            GroupOwner.SelectedUser = Group.Owner;
            if (Group.Users.Count > 0)
            {
                UserList.Clear();
                UserList = Group.Users;
                GroupMemberGrid.DataBind();
            }
        }
        private void ViewOnlyMode()
        {
            ContentPanel.Enabled = false;
            // GroupMemberGrid.Enabled = false;
            memberDetails.Visible = true;
        }
        private void Save()
        {
            var logMessage = new System.Text.StringBuilder();
            Dto.Group group = new Dto.Group();
            group.Id = EntityId;
            group.GroupName = GroupName.Text.Trim();
            group.IsDefaultGroup = CheckIsDefault.Checked;
            group.GroupDescription = GroupDescription.Text;
            group.EmailId = Email.Text;
            if (group.Id == 0)
            {
                logMessage.AppendLine($"New Group:{group.GroupName}");
            }
            
            if (GroupOwner.SelectedUser != null)
            {
                group.GroupOwner = Convert.ToInt32(GroupOwner.SelectedUser.Id);
            }

            group.Users = UserList;

            if (string.IsNullOrWhiteSpace(group.GroupName))
            {
                MasterPage.ShowAlert(AlertType.Error, "Group Name is required.");
                return;
            }

            if ((UIMode == UIMode.Add) && (DoesExist(GroupName.Text.Trim(), EntityId)))
            {
                MasterPage.ShowAlert(AlertType.Error, "Group Name Already Exists.");
                return;
            }
            else if (UIMode == UIMode.Edit)
            {
                logMessage.AppendLine($"Update Group:{group.GroupName}");

                var dbGroup = DomainApiClient<Dto.Group>.GetById(EntityId,true);
                logMessage.AppendLine($"Save group: Group Name:{group.GroupName}");
                if (dbGroup.GroupName != GroupName.Text.Trim())
                {
                    if (DoesExist(GroupName.Text.Trim(), EntityId))
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Group Name Already Exists.");
                        return;
                    }
                    logMessage.AppendLine($"GroupName: NewValue:{group.GroupName} OldValue:{dbGroup.GroupName}");
                }
                
                if (dbGroup.GroupDescription != group.GroupDescription)
                {
                    logMessage.AppendLine($"GroupDescription: NewValue:{group.GroupDescription} OldValue:{dbGroup.GroupDescription}");
                }

                if (dbGroup.EmailId != group.EmailId)
                {
                    logMessage.AppendLine($"EmailId: NewValue:{group.EmailId} OldValue:{dbGroup.EmailId}");
                }

                if (dbGroup.IsDefaultGroup != group.IsDefaultGroup)
                {
                    logMessage.AppendLine($"IsDefaultGroup: NewValue:{group.IsDefaultGroup} OldValue:{dbGroup.IsDefaultGroup}");
                }

                if ((group.GroupOwner != null || dbGroup.GroupOwner != null) && group.GroupOwner != dbGroup.GroupOwner)
                {
                    logMessage.AppendLine($"Group Owner: NewValue:{GroupOwner?.SelectedUser?.DisplayName} OldValue:{dbGroup?.Owner?.DisplayName}");
                }

                if (group != null && dbGroup != null && (group.Users.Count > 0 || dbGroup.Users.Count > 0))
                {
                    var oldList = dbGroup.Users.Select(i => i.DisplayName).OrderBy(i=> i).ToList();
                    var newList = group.Users.Select(i => i.DisplayName).OrderBy(i => i).ToList();
                    var any = newList.Except(oldList).Count() > 0 || oldList.Except(newList).Count() > 0;
                    if ((oldList.Count > 0 || newList.Count > 0) && any)
                        logMessage.AppendLine($"Group Member: NewValue: {string.Join(",", newList)} OldValue: {string.Join(",", oldList)}");
                }
            }
            
            DomainApiClient<Dto.Group>.Save(group);
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, logMessage.ToString());
            Response.Redirect("~/Admin/Group/List.aspx");
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

        private bool DoesExist(string groupName, long groupID)
        {
            Dto.Group group = new Dto.Group();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("groupID", groupID.ToString());
            parameter.Add("groupName", groupName);
            return DomainApiClient<Dto.Group>.GetByAction<bool>(DomainApiAction.Group.DoesExist, parameter);
        }

      #endregion

      #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked+=ActionClicked;

            if (!IsPostBack)
            {
                //memberDetails.Visible = false;
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    this.EntityId = (int)this.ContextObjectId.Value;
                    SetPage();
                }

                if (UIMode == UIMode.View)
                    ViewOnlyMode();
            }
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
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Group/List.aspx");
        }
        protected void AddUser_Click(object sender, EventArgs e)
        {
            if (GroupMember.SelectedUser != null)
            {
                UserList = UserList;
                User usr = UserList.Find(m => m.Id == GroupMember.SelectedUser.Id);
                if (usr == null)
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
        protected void GroupMemberGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            GroupMemberGrid.DataSource = UserList;
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "IDMS Group - Save");
                Save();
            }
            else if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "IDMS Group - Delete");
                delete();
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Group/List.aspx");
        }

        #endregion
    }
}