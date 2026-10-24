using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using Telerik.Web.UI;

namespace DI.Chakra.Web.AppCustomization.Dashboard.DB.UC
{
    public partial class List : UIBaseUC
    {
        #region "Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvDashboard_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gvDashboard.DataSource = DomainApiClient<Dto.Dashboard>.GetAll(); 
        }

        protected void gvDashboard_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvDashboard.SelectedItems[0] as GridDataItem;
            ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }
            else if(e.ActionType == ACLAction.DefaultRoleSettings)
            {
                Response.Redirect("~/AppCustomization/Dashboard/DefaultRoleDB/List.aspx");
            }
            else if(e.ActionType == ACLAction.AccessRights)
            {
                Response.Redirect("~/AppCustomization/Dashboard/DBAccessRights/List.aspx");
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
            DomainApiClient<Dto.Dashboard>.DeleteById(ContextObjectId.Value);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Dashboard Deleted Successfully");
            gvDashboard.Rebind();
        }
        #endregion
    }
}