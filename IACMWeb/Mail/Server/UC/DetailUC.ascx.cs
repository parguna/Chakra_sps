using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;

namespace DI.Chakra.Web.Mail.Server.UC
{
    public partial class DetailUC : UIBaseUC, IAccessControl
    {
        #region Variables
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        private const string V_EntityID = "V_EntityID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        #endregion

        #region Properties
        public int EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return 0;
                return (int)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
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

        #region events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    if (this.ContextObjectId == null)
                        return;
                    this.EntityId = (int)this.ContextObjectId.Value;
                    SetPage();
                }
                if (UIMode == UIMode.View)
                    ViewOnlyMode();
            }
        }
        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Mail/Server/List.aspx");
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Mail Server - Save");
            }
        }
        #endregion

        #region methods
        private void ViewOnlyMode()
        {
            ContentWrapper.Enabled = false;
        }
        private void SetPage()
        {
            var mail = DomainApiClient<Dto.MailServerConfiguration>.GetById(this.EntityId);
            if (null == mail)
                return;
            txtEmailId.Text = mail.EmailID;
            txtUserName.Text = mail.UserName;
            txtPassword.Attributes.Add("value", RSAHelper.DecryptData(mail.Password));
            txtPortNo.Text = mail.PortNumber;
            txtHostName.Text = mail.HostName;
            chkEnableSSL.Checked = (bool)mail.EnableSSL;
        }         
        private void Save()
        {
            if(string.IsNullOrEmpty(txtEmailId.Text.Trim()) || string.IsNullOrEmpty(txtHostName.Text.Trim()) || string.IsNullOrEmpty(txtPortNo.Text))
            {
                MasterPage.ShowAlert(AlertType.Error, "Please enter required details");
                return;
            }
            Dto.MailServerConfiguration mailServerConfiguration = new Dto.MailServerConfiguration();
            mailServerConfiguration.Id = this.EntityId;
            mailServerConfiguration.EmailID = txtEmailId.Text.Trim();
            mailServerConfiguration.HostName = txtHostName.Text.Trim();
            mailServerConfiguration.UserName = txtUserName.Text.Trim();
            mailServerConfiguration.Password = RSAHelper.EncryptData(txtPassword.Text);            
            mailServerConfiguration.PortNumber = txtPortNo.Text;
            mailServerConfiguration.EnableSSL = chkEnableSSL.Checked;
            DomainApiClient<Dto.MailServerConfiguration>.Save(mailServerConfiguration);
            Response.Redirect("~/Mail/Server/List.aspx");  
        }
        #endregion
    }
}