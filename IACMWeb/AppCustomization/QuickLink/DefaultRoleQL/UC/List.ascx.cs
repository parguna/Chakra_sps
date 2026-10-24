
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

namespace DI.Chakra.Web.AppCustomization.QuickLink.DefaultRoleQL.UC
{
    public partial class List : UIBaseUC, IAccessControl
    {
      //  private AppCustomizationFacade appCustFacade = new AppCustomizationFacade();

        #region "Events"

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvDefaultRoleQL_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gvDefaultRoleQL.DataSource = DomainApiClient<PresentationDefaultRoleQuickLink>.GetByAction<IList<PresentationDefaultRoleQuickLink>>(DomainApiAction.Preference.PresentationDefaultRoleQuickLink.GetDefaultRoleQLRights, null);
        }

        protected void gvDefaultRoleQL_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvDefaultRoleQL.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "AppCustomization Role - Delete");
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
            DomainApiClient<Dto.DefaultRoleQuickLink>.DeleteById(this.ContextObjectId.Value);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Quick Link Deleted Successfully");
            gvDefaultRoleQL.Rebind();
        }

        #endregion

    }
}