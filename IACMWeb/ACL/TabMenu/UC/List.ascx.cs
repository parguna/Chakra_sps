using System;
using System.Collections.Generic;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using Application;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.ACL.TabMenu.UC
{
    public partial class List : UIBaseUC, IAccessControl
    {
        #region "Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvTabMenu_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindTabMenus();
        }

        protected void gvTabMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvTabMenu.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/TabMenu - Delete");
                delete();
            }
        }
        #endregion

        #region "Methods"
        private void BindTabMenus()
        {
            IList<Dto.Presentation.PresentationTabMenu> lstMenu = DomainApiClient<Dto.Presentation.PresentationTabMenu>.
                GetByAction<List<Dto.Presentation.PresentationTabMenu>>(DomainApiAction.ACL.PresentationTabMenu.GetAll, null);
            gvTabMenu.DataSource = lstMenu;
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", this.ContextObjectId.Value.ToString());
            if (DomainApiClient<Dto.TabMenu>.GetByAction<bool>(DomainApiAction.ACL.TabMenu.DoesChildExists, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Selected Menu cann't be deleted as it has submenus.");
                return;
            }
            DomainApiClient<Dto.TabMenu>.DeleteById((long)(this.ContextObjectId.Value));
            MasterPage.ShowAlert(AlertType.Success, "Menu Deleted Successfully");
            gvTabMenu.Rebind();
            this.ContextObjectId = null;
        }
        #endregion
    }
}