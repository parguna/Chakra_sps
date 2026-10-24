
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.AppCustomization.QuickLink.DefaultRoleQL.UC
{
    public partial class Detail : UIBaseUC
    {
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
    //    private AppCustomizationFacade appCusFacade = new AppCustomizationFacade();

        #region Properties

        public int EntityId
        {
            get
            {
                if (ViewState[V_ENTITYID] == null)
                    return 0;
                return (int)ViewState[V_ENTITYID];
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
                    if (this.ContextObjectId == null)
                        return;
                    this.EntityId = (int)this.ContextObjectId.Value;
                    SetPage();
                }
                else if (UIMode == UIMode.Add)
                {
                    BindRole();
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
            Response.Redirect("~/AppCustomization/QuickLink/DefaultRoleQL/List.aspx");
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindNames();
        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCategoryByRoleID(Convert.ToInt32(ddlRole.SelectedValue));
        }

        #endregion

        #region Methods

        private void SetPage()
        {
        //  var list = appCusFacade.GetDefaultRoleQLById(this.EntityId);
            var list = DomainApiClient<Dto.DefaultRoleQuickLink>.GetById(this.EntityId,true);
            BindRole();
            ddlSelectItem(ddlRole, list.Role.Id.ToString());

            BindCategoryByRoleID(Convert.ToInt32(ddlRole.SelectedValue));
            ddlSelectItem(ddlCategory, list.QuickLink.Category.ToString());

            BindNames();
            ddlSelectItem(ddlName, list.QuickLink.Id.ToString());

            txtPosition.Text = list.Position.ToString();

        }

        private void Save()
        {
            DI.Chakra.Dto.DefaultRoleQuickLink itemInfo = new DI.Chakra.Dto.DefaultRoleQuickLink();

            if (ddlRole.SelectedValue == "--Select--" || ddlCategory.SelectedValue == "--Select--" || ddlName.SelectedValue == "--Select--")
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select all the values.");
                return;
            }

            if (string.IsNullOrEmpty(txtPosition.Text.Trim()))
            {
                MasterPage.ShowAlert(AlertType.Error, "Please enter position.");
                return;
            }

            itemInfo.Id = this.EntityId;
            itemInfo.RoleId = Convert.ToInt32(ddlRole.SelectedValue);

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("roleId", ddlRole.SelectedValue);
            parameter.Add("quickLinkId", ddlName.SelectedValue);

            if (DomainApiClient<Dto.DefaultRoleQuickLink>.GetByAction<bool>(DomainApiAction.Preference.QuickLink.DefaultRoleQuicklink.DoesExists, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Quick Link already exists for this Role.");
                return;
            }
            parameter.Clear();
            parameter.Add("dbPosition", txtPosition.Text);
            parameter.Add("roleID", ddlRole.SelectedValue);

            if (DomainApiClient<Dto.DefaultRoleQuickLink>.GetByAction<bool>(DomainApiAction.Preference.QuickLink.DefaultRoleQuicklink.DoesExistsByPosition, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Quick Link Position already exists for this Role.");
                return;
            }

            itemInfo.QuickLinkId = Convert.ToInt32(ddlName.SelectedValue);
            itemInfo.Position = Convert.ToInt16(txtPosition.Text);
        //    appCusFacade.SaveItems(itemInfo);
            DomainApiClient<Dto.DefaultRoleQuickLink>.Save(itemInfo);

            Response.Redirect("~/AppCustomization/QuickLink/DefaultRoleQL/List.aspx");
        }

        private void ViewOnlyMode()
        {
            ContentWrapper.Disabled = true;
        }

        private void ddlSelectItem(DropDownList ddl, string strValue)
        {
            if (strValue != null)
                ddl.SelectedValue = strValue;
        }

        private void BindCategoryByRoleID(int roleId)
        {
            try
            {
            
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("roleId", roleId.ToString());

                var list =DomainApiClient<Dto.DefaultRoleQuickLink>.GetByAction<IList<string>>(DomainApiAction.Preference.QuickLink.DefaultRoleQuicklink.GetQLCatagoriesByRoleID, parameter);
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
                //  var list = appCusFacade.GetAllRoles();
                var list = DomainApiClient<Dto.Role>.GetAll();
                if (null != list)
                {
                    ddlRole.Items.Clear();
                    ddlRole.DataTextField = "RoleName";
                    ddlRole.DataValueField = "Id";
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
                    parameter.Add("category", ddlCategory.SelectedValue);          
                    var list = DomainApiClient<Dto.QuickLink>.GetByAction<List<Dto.QuickLink>>(DomainApiAction.Preference.QuickLink.QLAcessRights.GetByCategory, parameter); 
                    if (null != list)
                    {
                        ddlName.ClearSelection();
                        ddlName.DataSource = list;
                        ddlName.DataBind();
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