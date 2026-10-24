using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Web.AppCustomization.QuickLink.QL.UC
{
    public partial class Detail : UIBaseUC
    {

        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_LINKNAME = "V_LINKNAME";

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

        public string QuickLinkName
        {
            get
            {
                if (ViewState[V_ENTITYID] == null)
                    return string.Empty;
                return (string)ViewState[V_LINKNAME];
            }
            set
            {
                ViewState[V_LINKNAME] = value;
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
            this.NameAutoCompleteBox.AutocompleteSelection += NameAutoCompleteBox_AutocompleteSelection;
            this.NameAutoCompleteBox.OnEntryRemoved += NameAutoCompleteBox_OnEntryRemoved;

            if (!Page.IsPostBack)
            {
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    if (this.ContextObjectId == null)
                        return;
                    this.EntityId = (long)this.ContextObjectId.Value;
                    SetPage();
                }
                if (UIMode == UIMode.View)
                    ViewOnlyMode();
            }
        }

        private void NameAutoCompleteBox_OnEntryRemoved()
        {
            txtCategory.Text = string.Empty;
        }

        private void NameAutoCompleteBox_AutocompleteSelection(Dto.TabMenu SelectedTabMenu)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("menuId", SelectedTabMenu.Id.ToString());
            Dto.TabMenu menu = (DomainApiClient<Dto.TabMenu>.GetByAction<Dto.TabMenu>(DomainApiAction.ACL.TabMenu.GetCategoryById, parameter));
            if (menu != null)
            {
                string menuName = menu.MenuName;

                if (!string.IsNullOrEmpty(menuName))
                {
                    txtCategory.Text = menuName;
                }
            }
            else
            {
                txtCategory.Text = String.Empty;
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
            Response.Redirect("~/AppCustomization/QuickLink/QL/List.aspx");
        }

        #endregion

        #region Methods

        private void SetPage()
        {
            Dto.QuickLink link = DomainApiClient<Dto.QuickLink>.GetById(this.ContextObjectId.Value);  
            if (null == link)
                return;
            QuickLinkName = link.Name;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("menuName", link.Name);
            long tabmenuId = (DomainApiClient<Dto.TabMenu>.GetByAction<Dto.TabMenu>(DomainApiAction.ACL.TabMenu.GetByMenuname, parameter)).Id;

            NameAutoCompleteBox.SelectedParentTabMenu = DomainApiClient<Dto.TabMenu>.GetById(tabmenuId); 
            txtCategory.Text = link.Category;
        }

        private void Save()
        {
            Dto.QuickLink qlInfo = new Dto.QuickLink();

            qlInfo.Id = this.EntityId;
            if (NameAutoCompleteBox.SelectedParentTabMenu != null)
            {
                qlInfo.Name = NameAutoCompleteBox.SelectedParentTabMenu.MenuName;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("qlName", qlInfo.Name);
                bool IsExists= DomainApiClient<Dto.QuickLink>.GetByAction<bool>(DomainApiAction.Preference.QuickLink.QL.DoesExists, parameter);

                if (IsExists && qlInfo.Name != QuickLinkName)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Quick Link Already Exists.");
                    return;
                }
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Quick Link Name is required.");
                return;
            }

            if (NameAutoCompleteBox.SelectedParentTabMenu.ParentTabMenuId != null)
                qlInfo.QuickLinkId = (long)NameAutoCompleteBox.SelectedParentTabMenu.Id;
            else
                qlInfo.QuickLinkId = 0;
            qlInfo.Category = txtCategory.Text;
           
            DomainApiClient<Dto.QuickLink>.Save(qlInfo);

            Response.Redirect("~/AppCustomization/QuickLink/QL/List.aspx");
        }
        private void ViewOnlyMode()
        {
            ContentWrapper.Disabled = true;
        }
        protected void AutoCompleteOnEntryAdded(long menuId)
        { 
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("menuId", menuId.ToString());
            string menuName = (DomainApiClient<Dto.TabMenu>.GetByAction<Dto.TabMenu>(DomainApiAction.ACL.TabMenu.GetCategoryById, parameter)).MenuName;

            if (!string.IsNullOrEmpty(menuName))
            {
                txtCategory.Text = menuName;
            }
            else
            {
                txtCategory.Text = String.Empty;
            }
        }      

        #endregion
    }
}