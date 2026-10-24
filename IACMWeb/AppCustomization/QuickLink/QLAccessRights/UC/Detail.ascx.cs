
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.AppCustomization.QuickLink.QLAccessRights.UC
{
    public partial class Detail : UIBaseUC
    {
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
    //  private AppCustomizationFacade appCusFacade = new AppCustomizationFacade();

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
                    BindCategory();
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
            Response.Redirect("~/AppCustomization/QuickLink/QLAccessRights/List.aspx");
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindNames();
        }

        #endregion

        #region Methods

        private void SetPage()
        {
            var list = DomainApiClient<Dto.QuickLinkAccessRights>.GetById(EntityId,true);
      
            BindRole();
            ddlSelectItem(ddlRole, list.RoleId.ToString());

            BindCategory();
            ddlSelectItem(ddlCategory, list.QuickLink.Category.ToString());
           
            BindNames();
            ddlSelectItem(ddlName, list.QuickLink.Id.ToString());

        }

        private void Save()
        {
            DI.Chakra.Dto.QuickLinkAccessRights itemInfo = new DI.Chakra.Dto.QuickLinkAccessRights();

            if (ddlRole.SelectedValue == "--Select--" || ddlCategory.SelectedValue == "--Select--" || ddlName.SelectedValue == "--Select--")
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select all the values.");
                return;
            }

            itemInfo.Id = this.EntityId;
            itemInfo.RoleId = Convert.ToInt32(ddlRole.SelectedValue);

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("roleId", itemInfo.RoleId.ToString());
            parameter.Add("quickLinkId", ddlName.SelectedValue);

            if (UIMode == UIMode.Add)
            {
                if (DomainApiClient<Dto.QuickLinkAccessRights>.GetByAction<bool>(DomainApiAction.Preference.QuickLink.QLAcessRights.DoesExists, parameter))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Quick Link already exists for this Role.");
                    return;
                }
            }
            itemInfo.QuickLinkId = Convert.ToInt32(ddlName.SelectedValue);
            //appCusFacade.SaveQLAccessRights(itemInfo);
            DomainApiClient<Dto.QuickLinkAccessRights>.Save(itemInfo);

            Response.Redirect("~/AppCustomization/QuickLink/QLAccessRights/List.aspx");
        }

        private void ViewOnlyMode()
        {
            ContentWrapper.Disabled = true;
        }

        private void ddlSelectItem(DropDownList ddl, string strValue)
        {
            if (ddl.Items.FindByValue(strValue) != null)
                ddl.Items.FindByValue(strValue).Selected = true;
        }

        private void BindCategory()
        {
            try
            {
            
                var list = DomainApiClient<Dto.QuickLink>.GetAll();
                List<string> lstCategory = list.Where(m => m.Category != null).Select(m => m.Category).Distinct().OrderBy(a=> a).ToList();
                if (null != lstCategory)
                {
                    ddlCategory.Items.Clear();
                    ddlCategory.DataSource = lstCategory;
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
                //var list = appCusFacade.GetAllRoles();
                var list = DomainApiClient<Dto.Role>.GetAll();
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
                    parameter.Add("category", ddlCategory.SelectedValue);
                    var list = DomainApiClient<Dto.QuickLink>.GetByAction<List<Dto.QuickLink>>(DomainApiAction.Preference.QuickLink.QLAcessRights.GetByCategory, parameter); 
                    if (null != list)
                    {
                        list = list.OrderBy(l => l.Name).ToList();
                        ddlName.ClearSelection();
                        ddlName.DataTextField = "Name";
                        ddlName.DataValueField = "Id";
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