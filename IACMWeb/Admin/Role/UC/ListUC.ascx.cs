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

namespace DI.Chakra.Web.Admin.Role.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Members
        private SessionStatusImpl _m_session = new SessionStatusImpl();
        private static IList<Dto.Role> listRole;
        #endregion

        #region Method
        private void BindRoles()
        {
            listRole = DomainApiClient<Dto.Role>.GetAll();
            RoleGrid.DataSource = listRole;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;

            if (!IsPostBack)
            {
                RoleGrid.DataBind();
            }
        }
        protected void RoleGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindRoles();
        }
        protected void RoleGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = RoleGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }
        #endregion

        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", this.ContextObjectId.ToString());
            var Role = DomainApiClient<Dto.Role>.GetByAction<Dto.Role>(DomainApiAction.ACL.Role.GetByIdIncludingUser, param);
            if (Role.IsDefaultRole)
                MasterPage.ShowAlert(AlertType.Error, "Default Role cannot be deleted");
            else
            {
                Dto.Role role = new Dto.Role();
                var roleId = (long)this.ContextObjectId;

                Dictionary<string, string> parameter = new Dictionary<string, string>();

                parameter.Add("roleId", roleId.ToString());
                _m_session = new SessionStatusImpl();
                parameter.Add("currentUser", _m_session.User.UserName.ToString());
                var isDeleted = DomainApiClient<Dto.Role>.GetByAction<bool>(DomainApiAction.ACL.Role.DeleteRole, parameter);

                if (isDeleted)
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Role:{Role?.RoleName} deleted successfully");
                    MasterPage.ShowAlert(AlertType.Success, "Role deleted successfully");
                }
                else
                    MasterPage.ShowAlert(AlertType.Error, "Could not delete Role. Please check whether Users are assigned to the Role.");
            }
            //}
            //bool isExists = DomainApiClient<Dto.Role>.GetById((long)this.ContextObjectId) != null ? true : false;
            //if (isExists)
            //{
            //    DomainApiClient<Dto.Role>.DeleteById((long)this.ContextObjectId.Value);
            //    this.ContextObjectId = null;
            //    MasterPage.ShowAlert(AlertType.Success, "Role deleted successfully");
            //}
            RoleGrid.Rebind();
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "IDAS Role - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                RoleGrid.ExportSettings.FileName = "Role";
                RoleGrid.Export(RoleGrid,e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                RoleGrid.Export<IList<Dto.Role>>(listRole, e.ActionType, "Role");
            }
        }

        protected void btncancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
    }
}