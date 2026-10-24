
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Logger;
using Application;

namespace DI.Chakra.Web.AppCustomization.QuickLink.QLAccessRights.UC
{
    public partial class List : UIBaseUC, IAccessControl
    {

        #region "Events"

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvQLAccessRights_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gvQLAccessRights.DataSource = DomainApiClient<PresentationQuickLinkAccessRights>.GetByAction<IList<PresentationQuickLinkAccessRights>>(DomainApiAction.Preference.QuickLink.QLAcessRights.GetQuickLinkAccessRights, null);
        }

        protected void gvQLAccessRights_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvQLAccessRights.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "AppCustomization QLAccessRights - Delete");
                delete();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AppCustomization/QuickLink/QL/List.aspx");
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

            //  appCustFacade.DeleteQLAccessRightsById((int)this.ContextObjectId.Value);
            DomainApiClient<Dto.QuickLinkAccessRights>.DeleteById(this.ContextObjectId.Value);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Quick Link Access Rights Deleted Successfully");
            gvQLAccessRights.Rebind();
        }

        #endregion

    }
}