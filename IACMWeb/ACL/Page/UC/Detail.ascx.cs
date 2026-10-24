using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.ACL.Page.UC
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
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/Page - Save");
                Save();
            }
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", Convert.ToString(this.EntityId));
            var list = DomainApiClient<Dto.WebPage>.GetByAction<Dto.WebPage>
                (DomainApiAction.ACL.WebPage.GetByIdInludePageUrl, parameter);

            if (null == list)
                return;
            txtUniqueName.Text = list.UniqueName;
            txtName.Text = list.Name;
            if (list.PageUrlId.HasValue)
            {
                PageUrlAutoCompleteBox.SelectedUrl = DomainApiClient<Dto.PageUrl>.GetById((long)list.PageUrlId.Value);
            }
        }

        private void Save()
        {
            Dto.WebPage pageInfo = new Dto.WebPage();

            if (EntityId > 0)
                pageInfo = DomainApiClient<Dto.WebPage>.GetById((long)this.EntityId);

            pageInfo.Id = EntityId;
            pageInfo.UniqueName = txtUniqueName.Text;
            pageInfo.Name = txtName.Text;

            if (PageUrlAutoCompleteBox.SelectedUrl != null)
            {
                pageInfo.PageUrlId = PageUrlAutoCompleteBox.SelectedUrl.Id;
            }
            else
            {
                Dictionary<string, string> uParam = new Dictionary<string, string>();
                uParam.Add("url", "#");
                Dictionary<long, string> urlIds = DomainApiClient<Dto.PageUrl>.GetByAction<Dictionary<long, string>>(DomainApiAction.ACL.PageUrl.GetByUrl, uParam);
                foreach (long key in urlIds.Keys)
                {
                    pageInfo.PageUrlId = key;
                }
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("name", pageInfo.Name);
            parameter.Add("uniqueName", pageInfo.UniqueName);
            if ((UIMode == UIMode.Add) && DomainApiClient<Dto.WebPage>.GetByAction<bool>
                (DomainApiAction.ACL.WebPage.DoesAnyExists, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Page Name or Unique Name Already Exists.");
                return;
            }
            DomainApiClient<Dto.WebPage>.Save(pageInfo);
            this.ContextObjectId = null;
            Response.Redirect("~/ACL/Page/List.aspx");
        }

        private void ViewOnlyMode()
        {
            ContentWrapper.Disabled = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "ACL/Page - Cancel");
            Response.Redirect("~/ACL/Page/List.aspx");
        }
        #endregion
    }
}