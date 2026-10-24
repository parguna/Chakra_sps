using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.AppCustomization.Dashboard.DefaultRoleDB.UC
{
    public partial class Detail : UIBaseUC
    {
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private SessionStatusImpl SessionUser = new SessionStatusImpl();

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
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    if (ContextObjectId == null)
                        return;
                    EntityId = this.ContextObjectId.Value;
                    SetPage();
                }
                else if (UIMode == UIMode.Add)
                {
                    BindRole();
                    BindDashBoardType();
                }
                if (UIMode == UIMode.View)
                    ViewOnlyMode();
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AppCustomization/Dashboard/DefaultRoleDB/List.aspx");
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BindNames();
            BindNamesByRoleId(Convert.ToInt64(ddlRole.SelectedValue), ddlCategory.SelectedValue);
        }

        protected void ddlName_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDashBoardValues();
        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ddlRole.SelectedValue !="--Select--")
                BindCategoryByRoleID(Convert.ToInt64(ddlRole.SelectedValue));
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", EntityId.ToString());
            var list = DomainApiClient<DefaultRoleDashboardPreference>.
                GetByAction<DefaultRoleDashboardPreference>
                (DomainApiAction.Preference.DefaultRoleDashboardPreference.GetByIdWithNavigaton, parameter);

            BindRole();
            ddlSelectItem(ddlRole, list.RoleId.ToString());

            BindCategoryByRoleID(Convert.ToInt64(ddlRole.SelectedValue));
            ddlSelectItem(ddlCategory, list.Dashboard.DashboardCategory.ToString());

            BindNames();
            ddlSelectItem(ddlName, list.Dashboard.Id.ToString());

            BindDashBoardType();
            ddlSelectItem(ddlDBType, ((long)list.Dashboard.DashBoardType).ToString());

            txtTargetURL.Text = list.Dashboard.TargetUrl;
            txtNavigateURL.Text = list.Dashboard.NavigateUrl;
           
                txtPosition.Text = list.Position.ToString();
        }

        private void Save()
        {
            DefaultRoleDashboardPreference itemInfo = new DefaultRoleDashboardPreference();

            if (ddlRole.SelectedValue == "--Select--" || ddlCategory.SelectedValue == "--Select--" ||
                ddlName.SelectedValue == "--Select--" || ddlDBType.SelectedValue == "--Select--")
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select all the values.");
                return;
            }
            itemInfo.Id = EntityId;
            itemInfo.RoleId = Convert.ToInt64(ddlRole.SelectedValue);
            
            if (UIMode == UIMode.Add)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("roleId", itemInfo.RoleId.ToString());
                param.Add("dashboardid", (Convert.ToInt64(ddlName.SelectedValue)).ToString());
                if (DomainApiClient<DefaultRoleDashboardPreference>.GetByAction<bool>
                    (DomainApiAction.Preference.DefaultRoleDashboardPreference.DoesExists, param))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Dashboard already exists for this Role.");
                    return;
                }
            }

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("roleId", itemInfo.RoleId.ToString());
            parameter.Add("dbPosition", (Convert.ToInt32(txtPosition.Text).ToString()));
            if (DomainApiClient<DefaultRoleDashboardPreference>.GetByAction<bool>
                (DomainApiAction.Preference.DefaultRoleDashboardPreference.DoesExistsByPosition, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Dashboard Position already exists for this Role.");
                return;
            }


            itemInfo.DashBoardId = Convert.ToInt64(ddlName.SelectedValue);
            if (txtPosition.Text != null)
            {
                itemInfo.Position = Convert.ToInt32(txtPosition.Text);
            }
            
            DomainApiClient<DefaultRoleDashboardPreference>.Save(itemInfo);
            Response.Redirect("~/AppCustomization/Dashboard/DefaultRoleDB/List.aspx");
        }

        private void ViewOnlyMode()
        {
            ContentWrapper.Disabled = true;
        }

        private void ddlSelectItem(DropDownList ddl, string strValue)
        {
            ddl.ClearSelection();
            if (ddl.Items.FindByValue(strValue) != null)
                ddl.Items.FindByValue(strValue).Selected = true;
        }

        private void BindCategoryByRoleID(long roleId)
        {
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("roleId", roleId.ToString());
                var list = DomainApiClient<Dto.Dashboard>.GetByAction<List<string>>
                    (DomainApiAction.Preference.Dashboard.GetDashboardCategoryByRoleId, parameter);
                if (null != list)
                {
                    ddlCategory.Items.Clear();
                    ddlCategory.DataSource = list;
                    ddlCategory.DataBind();
                    ddlCategory.Items.Insert(0, "--Select--");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void BindRole()
        {
            try
            {
                var list = DomainApiClient<Role>.GetAll();
                if (null != list)
                {
                    ddlRole.Items.Clear();
                    ddlRole.DataSource = list;
                    ddlRole.DataBind();
                    ddlRole.Items.Insert(0, "--Select--");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void BindNames()
        {
            try
            {
                if (ddlCategory.SelectedValue != "0")
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("dashboardCategory", ddlCategory.SelectedValue);
                    var list = DomainApiClient<Dto.Dashboard>.GetByAction<List<Dto.Dashboard>>
                        (DomainApiAction.Preference.Dashboard.GetByDashBoardCategory, parameter);
                    if (null != list)
                    {
                        ddlName.ClearSelection();
                        ddlName.DataSource = list;
                        ddlName.DataBind();
                        ddlName.Items.Insert(0, "--Select--");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void BindDashBoardType()
        {
            ddlDBType.Items.Insert(0, "--Select--");
            try
            {
                foreach (DashboardType item in Enum.GetValues(typeof(DashboardType)))
                {
                    var description = UtilityMethods.GetEnumAttribute<Description>(item);
                    ListItem listItem = new ListItem(description, ((byte)item).ToString());
                    ddlDBType.Items.Add(listItem);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void BindDashBoardValues()
        {
            try
            {
                var list = DomainApiClient<Dto.Dashboard>.GetById(Convert.ToInt64(ddlName.SelectedValue));
                if (null != list)
                {
                    ddlSelectItem(ddlDBType, ((long)list.DashBoardType).ToString());
                    txtTargetURL.Text = list.TargetUrl;
                    txtNavigateURL.Text = list.NavigateUrl;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void BindNamesByRoleId(long roleId, string category)
        {
            try
            {
                if (ddlCategory.SelectedValue != "0")
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("roleId", roleId.ToString());
                    parameter.Add("category", category.ToString());
                    var list = DomainApiClient<Dto.Dashboard>.GetByAction<List<Dto.Dashboard>>
                        (DomainApiAction.Preference.Dashboard.GetByRoleIdAndCategory, parameter);
                    if (null != list)
                    {
                        ddlName.ClearSelection();
                        ddlName.DataSource = list;
                        ddlName.DataBind();
                        ddlName.Items.Insert(0, "--Select--");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
