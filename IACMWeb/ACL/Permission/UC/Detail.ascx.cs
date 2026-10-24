using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.ACL.Permission.UC
{
    public partial class Detail : UIBaseUC
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
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    if (this.ContextObjectId == null)
                        return;
                    this.EntityId = (long)this.ContextObjectId.Value;
                    SetPage();
                }
                if (UIMode == UIMode.View)
                    ViewOnlyMode();
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/Permission - Save");
                Save();
            }
        }

        #endregion

        #region Methods

        private void SetPage()
        {
            var list = DomainApiClient<Dto.Permission>.GetById((long)this.EntityId);
            if (null == list)
                return;

            txtPermissionName.Text = list.Name;
            txtIconURL.Text = list.IconURL;
            txtPerClientscript.Text = list.ClientScript;
        }

        private void Save()
        {
            Dto.Permission perInfo = new Dto.Permission();

            if (EntityId > 0)
                perInfo = DomainApiClient<Dto.Permission>.GetById(EntityId);

            perInfo.Id = EntityId;
            perInfo.Name = txtPermissionName.Text;
            perInfo.IconURL = txtIconURL.Text;
            perInfo.ClientScript = txtPerClientscript.Text;


            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("name", perInfo.Name.Trim());
            if (UIMode == UIMode.Add && DomainApiClient<Dto.Permission>.GetByAction<bool>
                (DomainApiAction.ACL.Permission.DoesExists, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Permission Name Already Exists.");
                return;
            }
            DomainApiClient<Dto.Permission>.Save(perInfo);
            this.ContextObjectId = null;
            Response.Redirect("~/ACL/Permission/List.aspx");
        }

        private void ViewOnlyMode()
        {
            ContentWrapper.Disabled = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/Permission - Cancel");
            Response.Redirect("~/ACL/Permission/List.aspx");
        }
        #endregion
    }
}