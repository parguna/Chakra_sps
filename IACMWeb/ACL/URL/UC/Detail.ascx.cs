using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DI.Chakra.Web.ACL.URL.UC
{
    public partial class Detail : UIBaseUC
    {
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        public static readonly string[] VALID_EXTENSIONS = new string[3] { ".aspx", ".htm", ".html" };

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
                    this.EntityId = this.ContextObjectId.Value;
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
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/URL - Save");
                Save();
            }
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            var list = DomainApiClient<Dto.PageUrl>.GetById(EntityId);
            if (null == list)
                return;
            txtURL.Text = list.Url;
        }

        private void Save()
        {
            Dto.PageUrl url = new Dto.PageUrl();

            if (EntityId > 0)
                url = DomainApiClient<Dto.PageUrl>.GetById(EntityId);

            url.Url = txtURL.Text.Trim();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("url", url.Url);
            if (((UIMode == UIMode.Add) | (UIMode == UIMode.Edit)
                && DomainApiClient<Dto.PageUrl>.GetByAction<bool>(DomainApiAction.ACL.PageUrl.DoesExists, parameter)))
            {
                MasterPage.ShowAlert(AlertType.Error, "URL Already Exists.");
                return;
            }
            if (!VALID_EXTENSIONS.Contains(Path.GetExtension(url.Url.ToLower())))
            {
                MasterPage.ShowAlert(AlertType.Error, "ASPX and HTML Forms only allowed");
                return;
            }
            DomainApiClient<Dto.PageUrl>.Save(url);
            this.ContextObjectId = null;
            Response.Redirect("~/ACL/URL/List.aspx");
        }

        private void ViewOnlyMode()
        {
            ContentWrapper.Disabled = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/URL - Cancel");
            Response.Redirect("~/ACL/URL/List.aspx");
        }
        #endregion
    }
}