using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.ACL.Page.UC
{
    public partial class PagePermission : UIBaseUC
    {
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_ENTITYID] == null)
                    return 0;
                return (long)ViewState[V_ENTITYID];
            }
            set
            {
                ViewState[V_ENTITYID] = value;
            }
        }

        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.View;

                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    if (this.ContextObjectId == null)
                        return;
                    this.EntityId = (long)this.ContextObjectId.Value;
                    BindPermissions();
                    SetPage();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ACL/Page/List.aspx");
        }

        protected void gdPagePermission_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("pageId", this.EntityId.ToString());
            gdPagePermission.DataSource = DomainApiClient<Dto.Presentation.PresentationPagePermission>.
                GetByAction<List<Dto.Presentation.PresentationPagePermission>>
                (DomainApiAction.ACL.PresentationPagePermission.GetByPageId, parameter);
        }

        protected void gdPagePermission_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long perId = (long)item.GetDataKeyValue("Id");
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("pagePermissionId", Convert.ToString(perId));
                var AccessRights = DomainApiClient<Dto.AccessRights>.GetByAction<List<Dto.AccessRights>>
                    (DomainApiAction.ACL.AccessRights.GetByPagePermission, parameter);
                if (AccessRights.Count > 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Permission Cannot be Deleted. It is in use.");
                    return;
                }               
                DomainApiClient<Dto.PagePermission>.DeleteById((long)(perId));
                gdPagePermission.Rebind();
                this.ContextObjectId = null;
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/Page - Page Permission - Delete");
                MasterPage.ShowAlert(AlertType.Success, "Permission Deleted Successfully.");
            }
        }

        protected void btnAddPermission_Click(object sender, EventArgs e)
        {
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/Page - Page Permission - Add");
            if (ddlPermision.SelectedIndex <= 0)
            {
                ShowAlertMessage(AlertType.Error, "Please select the permission");
            }
            else
            {
                if (Convert.ToInt32(ddlPermision.SelectedValue) > 0)
            {
                bool blnIsPerExists = false;
                foreach (GridDataItem item in gdPagePermission.Items)
                {
                    if (ddlPermision.SelectedValue == item["PermissionId"].Text)
                    {
                        blnIsPerExists = true;
                        break;
                    }
                }
                if (blnIsPerExists)
                    MasterPage.ShowAlert(AlertType.Error, "Permission Already Exists.");
                else
                {
                    Dto.PagePermission plObj = new Dto.PagePermission();
                    plObj.PermissionId = Convert.ToInt64(ddlPermision.SelectedValue);
                    plObj.PageId = this.EntityId;
                    Dto.PageUrl url = SelectUrl();
                    if (url != null)
                    {
                            plObj.UrlId = (int)url.Id;
                    }
                    plObj.UrlId = plObj.UrlId;
                    DomainApiClient<Dto.PagePermission>.Save(plObj);
                    this.ContextObjectId = null;
                    SetPage();
                    MasterPage.ShowAlert(AlertType.Success, "Permission Added Successfully.");
                }

            }

            ClearSelection();
            }
        }

        #endregion

        #region Methods
        private void SetPage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("pageId", this.EntityId.ToString());
            List<Dto.Presentation.PresentationPagePermission> getData = DomainApiClient<Dto.Presentation.PresentationPagePermission>.
                GetByAction<List<Dto.Presentation.PresentationPagePermission>>
                (DomainApiAction.ACL.PresentationPagePermission.GetByPageId, parameter);

            if (getData.Count > 0)
            {
                ddlPermision.SelectedValue = getData[0].PermissionId.ToString();
                if (getData[0].UrlId != null)
                    ACLDataMethodSearchUC.SelectedUrl = DomainApiClient<Dto.PageUrl>.GetById((long)getData[0].UrlId.Value);
            }
            gdPagePermission.DataSource = getData;
            gdPagePermission.DataBind();
        }

        private void BindPermissions()
        {
            try
            {
                ddlPermision.Items.Clear();
                ddlPermision.DataSource = DomainApiClient<Dto.Permission>.GetAll();
                ddlPermision.DataBind();
                ddlPermision.Items.Insert(0, "--Select--");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Dto.PageUrl SelectUrl()
        {
            if (ACLDataMethodSearchUC.ChildAutoCompleteBox.Entries.Count <= 0)
                return null;
            return DomainApiClient<Dto.PageUrl>.GetById(Convert.ToInt64(ACLDataMethodSearchUC.ChildAutoCompleteBox.Entries[0].Value));
        }

        private void ClearSelection()
        {
            ACLDataMethodSearchUC.ChildAutoCompleteBox.Entries.Clear();
            ddlPermision.ClearSelection();
        }
        #endregion
    }
}