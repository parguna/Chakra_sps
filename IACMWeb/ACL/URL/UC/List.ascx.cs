using System;
using System.Collections.Generic;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using Application;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.ACL.URL.UC
{
    public partial class List : UIBaseUC, IAccessControl
    {
        #region "Events"       
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvurl_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindURLs();
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/URL - Delete");
                delete();
            }
        }

        protected void gvurl_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvurl.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }
        #endregion

        #region "Methods"
        private void BindURLs()
        {
            IList<Dto.PageUrl> lstURL = DomainApiClient<Dto.PageUrl>.GetAll();
            gvurl.DataSource = lstURL;
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("pageUrlId", this.ContextObjectId.Value.ToString());
            if (DomainApiClient<Dto.PageUrl>.GetByAction<bool>(DomainApiAction.ACL.PageUrl.HasAnyWebPage, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Selected URL cann't be deleted as it is linked with Pages and respective page permissions.");
                return;
            }
            DomainApiClient<Dto.PageUrl>.DeleteById(this.ContextObjectId.Value);
            MasterPage.ShowAlert(AlertType.Success, "URL Deleted Successfully");
            gvurl.Rebind();
            this.ContextObjectId = null;
        }
        #endregion
    }
}