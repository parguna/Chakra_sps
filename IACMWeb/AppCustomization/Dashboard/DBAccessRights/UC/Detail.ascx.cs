using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.AppCustomization.Dashboard.DBAccessRights.UC
{
    public partial class Detail : UIBaseUC
    {
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";

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
            MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    if (ContextObjectId == null)
                        return;
                    EntityId = ContextObjectId.Value;
                    SetPage();
                }
                else if (UIMode == UIMode.Add)
                {
                    BindCategory();
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
            Response.Redirect("~/AppCustomization/Dashboard/DBAccessRights/List.aspx");
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindNames();
        }

        protected void ddlName_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDashBoardValues();
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            var list = DomainApiClient<DashboardAccessRights>.GetById(EntityId, true);

            BindRole();
            ddlSelectItem(ddlRole, list.RoleId.ToString());

            BindCategory();
            ddlSelectItem(ddlCategory, list.Dashboard.DashboardCategory.ToString());

            BindNames();
            ddlSelectItem(ddlName, list.Dashboard.Id.ToString());

            BindDashBoardType();
            ddlSelectItem(ddlDBType, ((int)list.Dashboard.DashBoardType).ToString());

            txtTargetURL.Text = list.Dashboard.TargetUrl;
            txtNavigateURL.Text = list.Dashboard.NavigateUrl;
        }

        private void Save()
        {
            DashboardAccessRights itemInfo = new DashboardAccessRights();

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
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("roleId", itemInfo.RoleId.ToString());
                parameter.Add("dashboardId", (Convert.ToInt64(ddlName.SelectedValue)).ToString());

                if (DomainApiClient<DashboardAccessRights>.GetByAction<bool>
                    (DomainApiAction.Preference.DashboardAccessRights.DoesExists, parameter))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Dashboard access rights already given for this Role.");
                    return;
                }
                
            }
            
            itemInfo.DashboardId = Convert.ToInt64(ddlName.SelectedValue);
            DomainApiClient<DashboardAccessRights>.Save(itemInfo);
            Response.Redirect("~/AppCustomization/Dashboard/DBAccessRights/List.aspx");
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

        private void BindCategory()
        {
            try
            {
                var list = DomainApiClient<Dto.Dashboard>.GetAll().Select(db => db.DashboardCategory).Distinct().ToList();
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
            catch (Exception ex)
            {
                throw ex;
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
                    ddlSelectItem(ddlDBType, ((int)list.DashBoardType).ToString());
                    txtTargetURL.Text = list.TargetUrl;
                    txtNavigateURL.Text = list.NavigateUrl;
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