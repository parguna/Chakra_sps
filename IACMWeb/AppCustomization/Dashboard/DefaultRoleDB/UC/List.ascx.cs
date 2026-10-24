using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.AppCustomization.Dashboard.DefaultRoleDB.UC
{
    public partial class List : UIBaseUC, IAccessControl
    {
        #region "Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvDefaultRoleDB_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gvDefaultRoleDB.DataSource = DomainApiClient<Dto.DefaultRoleDashboardPreference>.
                GetByAction<List<Dto.DefaultRoleDashboardPreference>>
                (DomainApiAction.Preference.DefaultRoleDashboardPreference.GetAllOrderedWithNavigaton, null);
        }

        protected void gvDefaultRoleDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvDefaultRoleDB.SelectedItems[0] as GridDataItem;
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
            DomainApiClient<Dto.DefaultRoleDashboardPreference>.DeleteById(ContextObjectId.Value);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Dashboard Deleted Successfully");
            gvDefaultRoleDB.Rebind();
        }
        #endregion
    }
}