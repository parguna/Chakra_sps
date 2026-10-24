using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.AppCustomization.QuickLink.QL.UC
{
    public partial class List : UIBaseUC, IAccessControl
    {

        #region "Events"

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvQuicklink_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gvQuicklink.DataSource = DomainApiClient<Dto.QuickLink>.GetAll();
        }

        protected void gvQuicklink_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvQuicklink.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "AppCustomization - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.DefaultRoleSettings)
            {
                Response.Redirect("~/AppCustomization/QuickLink/DefaultRoleQL/List.aspx");
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

            DomainApiClient<Dto.QuickLink>.DeleteById(this.ContextObjectId.Value);

            // checking whether item was deleted before chowing message
            var qLink = DomainApiClient<Dto.QuickLink>.GetById(this.ContextObjectId.Value);
            if (qLink == null || qLink.Id == 0)
            {
                MasterPage.ShowAlert(AlertType.Success, "Quick Link Deleted Successfully");
            }

            gvQuicklink.Rebind();
            ContextObjectId = null;
        }

        #endregion
    }
}