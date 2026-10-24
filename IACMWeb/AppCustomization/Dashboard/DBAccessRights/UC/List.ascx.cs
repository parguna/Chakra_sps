using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.AppCustomization.Dashboard.DBAccessRights.UC
{
    public partial class List : UIBaseUC
    {
        #region "Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvDBAccessRights_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gvDBAccessRights.DataSource = DomainApiClient<Dto.DashboardAccessRights>.GetByAction<List<Dto.DashboardAccessRights>>
                (DomainApiAction.Preference.DashboardAccessRights.GetAllWithNavigation, null);
        }

        protected void gvDBAccessRights_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvDBAccessRights.SelectedItems[0] as GridDataItem;
            ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AppCustomization/Dashboard/DB/List.aspx");
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
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", ContextObjectId.Value.ToString());
            DomainApiClient<Dto.DashboardAccessRights>.GetByAction<object>
                (DomainApiAction.Preference.DashboardAccessRights.Delete, parameter);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Dashboard Access Rights Deleted Successfully");
            gvDBAccessRights.Rebind();
        }
        #endregion
    }
}