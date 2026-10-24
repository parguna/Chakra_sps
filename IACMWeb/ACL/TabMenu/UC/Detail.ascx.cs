using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.ACL.TabMenu.UC
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
            this.MasterPage.ActionClicked += ActionClicked;
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

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/TabMenu - Save");
                Save();
            }
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            var list = DomainApiClient<Dto.TabMenu>.GetById((long)this.EntityId);
            if (null == list)
                return;

            txtUniqueName.Text = list.UniqueName;
            txtMenuName.Text = list.MenuName;
            if (list.ParentTabMenuId.HasValue)
            {
                ParentMenuAutoCompleteBox.SelectedParentTabMenu = DomainApiClient<Dto.TabMenu>.GetById((long)list.ParentTabMenuId.Value);
            }
            if (list.PageId.HasValue)
            {
                PageAutoCompleteBox.SelectedPage = DomainApiClient<Dto.WebPage>.GetById((long)list.PageId.Value);
            }
        }

        private void Save()
        {

            Dto.TabMenu menuInfo = new Dto.TabMenu();

            if (EntityId > 0)
                menuInfo = DomainApiClient<Dto.TabMenu>.GetById((long)this.EntityId);

            menuInfo.Id = EntityId;
            menuInfo.UniqueName = txtUniqueName.Text;
            menuInfo.MenuName = txtMenuName.Text;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("menuName", menuInfo.MenuName);
            parameter.Add("uniqueName", menuInfo.UniqueName);
            if (UIMode == UIMode.Add && DomainApiClient<Dto.TabMenu>.GetByAction<bool>
                (DomainApiAction.ACL.TabMenu.DoesAnyExists, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Unique Name Already Exists.");
                return;
            }
            if (PageAutoCompleteBox.SelectedPage != null)
                menuInfo.PageId = PageAutoCompleteBox.SelectedPage.Id;
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Page Name is required.");
                return;
            }
            try
            {
                if (ParentMenuAutoCompleteBox.SelectedParentTabMenu != null)
                    menuInfo.ParentTabMenuId = (long)ParentMenuAutoCompleteBox.SelectedParentTabMenu.Id;

                DomainApiClient<Dto.TabMenu>.Save(menuInfo);
                this.ContextObjectId = null;
                Response.Redirect("~/ACL/TabMenu/List.aspx");
                //MasterPage.ShowAlert(AlertType.Success, "Menu Saved Successfully");
            }
            catch
            { }
        }

        private void ViewOnlyMode()
        {
            ContentWrapper.Disabled = true;
        }

        protected void btnCancel_Click1(object sender, EventArgs e)
        {
            Response.Redirect("~/ACL/TabMenu/List.aspx");
        }
        #endregion
    }
}