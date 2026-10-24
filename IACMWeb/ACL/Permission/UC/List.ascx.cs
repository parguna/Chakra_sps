using System;
using System.Collections.Generic;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using Application;

namespace DI.Chakra.Web.ACL.Permission.UC
{
    public partial class List : UIBaseUC, IAccessControl
    {
        #region "Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        protected void gvPermission_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindPermissions();
        }

        protected void gvPermission_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvPermission.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/Permission - Delete");
                delete();
            }
        }
        #endregion

        #region "Methods"
        private void BindPermissions()
        {
            IList<Dto.Permission> lstURL = DomainApiClient<Dto.Permission>.GetAll();
            gvPermission.DataSource = lstURL;
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("permissionId", this.ContextObjectId.Value.ToString());

            if (DomainApiClient<Dto.PagePermission>.GetByAction<bool>
                (DomainApiAction.ACL.PagePermission.DoesExistsByPermissionId, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Selected Permission cann't be deleted as it is linked to Pages.");
                return;
            }
            DomainApiClient<Dto.Permission>.DeleteById((long)(this.ContextObjectId));
            MasterPage.ShowAlert(AlertType.Success, "Permission Deleted Successfully");
            gvPermission.Rebind();
            this.ContextObjectId = null;
        }
        #endregion
    }
}