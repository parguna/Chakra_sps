using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.ACL.Page.UC
{
    public partial class List : UIBaseUC, IAccessControl
    {
        #region "Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvPage_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindPages();
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/Page - Delete");
                delete();
            }
        }

        protected void gvPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvPage.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }
        #endregion

        #region "Methods"
        private void BindPages()
        {
            IList<Dto.WebPage> lstPage = DomainApiClient<Dto.WebPage>.GetByAction<List<Dto.WebPage>>
                (DomainApiAction.ACL.WebPage.GetAllInludePageUrl, null);
            gvPage.DataSource = lstPage;
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id",this.ContextObjectId.Value.ToString());
            if (DomainApiClient<Dto.TabMenu>.GetByAction<bool>(DomainApiAction.ACL.TabMenu.DoesChildExists, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Selected Page cann't be deleted as it is linked to Menus.");
                return;
            }
            Dictionary<string, string> parameterDel = new Dictionary<string, string>();
            parameterDel.Add("pageId", ContextObjectId.Value.ToString());
            DomainApiClient<Dto.WebPage>.GetByAction<bool>(DomainApiAction.ACL.WebPage.DeletePageById, parameterDel);
            MasterPage.ShowAlert(AlertType.Success, "Page Deleted Successfully.");
            gvPage.Rebind();
            this.ContextObjectId = null;
        }
        #endregion
    }
}