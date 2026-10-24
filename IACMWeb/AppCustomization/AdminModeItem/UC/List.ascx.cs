using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using Telerik.Web.UI;

namespace DI.Chakra.Web.AppCustomization.AdminModeItem.UC
{
    public partial class List : UIBaseUC, IAccessControl
    {
        #region "Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvAdminItem_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gvAdminItem.DataSource = DomainApiClient<Dto.AdminModeItem>.GetAll(true, 50, 1); 
        }

        protected void gvAdminItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvAdminItem.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "AppCustomization Admin Mode - Delete");
                delete();
            }
        }
        #endregion

        #region "Methods"
        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            DomainApiClient<Dto.AdminModeItem>.DeleteById(this.ContextObjectId.Value);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "AdminMode Item Deleted Successfully");
            gvAdminItem.Rebind();
        }
        #endregion
    }
}