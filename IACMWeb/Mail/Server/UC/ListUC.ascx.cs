using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using DI.Chakra.Logger;
using Application;

namespace DI.Chakra.Web.Mail.Server.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region member
        private const string V_EntityID = "V_EntityID";
        private static List<Dto.MailServerConfiguration> listmail;
        #endregion

        #region property
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
        #endregion
        #region events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                this.MasterPage.ActionClicked += ActionClicked;
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Mail Server - Delete");
                delete();
            }

            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdMailServer.ExportSettings.FileName = "Mail-Server";
                grdMailServer.Export(grdMailServer,e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdMailServer.Export< List<Dto.MailServerConfiguration>>(listmail, e.ActionType, "Mail-Server");
            }

        }
        protected void grdMailServer_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            listmail = DomainApiClient<Dto.MailServerConfiguration>.GetAll();
            grdMailServer.DataSource = listmail;
        }
        protected void grdMailServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = grdMailServer.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }
        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var mailServerConfiguration = DomainApiClient<Dto.MailServerConfiguration>.GetById((long)this.EntityId);
            if (mailServerConfiguration != null)
            {
                if (this.SendTestEmail(mailServerConfiguration, MailTo.Text.Trim()))
                {
                    MasterPage.ShowAlert(AlertType.Success, "Test email sent successfully");
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "Error Occured while sending test email");
                }
            }
            modalPopup.VisibleOnPageLoad = false;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            modalPopup.VisibleOnPageLoad = false;
        }
        protected void SendTestMail_Click(object sender, EventArgs e)
        {
            if (this.ContextObjectId == null)
                return;
            modalPopup.VisibleOnPageLoad = true;
            this.EntityId = (int)this.ContextObjectId.Value;
        }
        #endregion

        #region methods
        private void delete()
        {
            if (!ContextObjectId.HasValue)
                return;
            DomainApiClient<Dto.MailServerConfiguration>.DeleteById((long)(this.ContextObjectId));
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Mail server deleted successfully");
            grdMailServer.Rebind();            
        }                
        private bool SendTestEmail(Dto.MailServerConfiguration serverConfig, string toAddress)
        {
            bool isError = false;
            try
            {
                SmtpClient testSMPT = new SmtpClient(serverConfig.HostName);
                string sub = "Test email from  IDAS V5 Mail server Configuration.";
                string body = "This is a test email sent when configure Mail server from IDAS V5 .";
                string fromAddress = serverConfig.EmailID;
                MailMessage msg = new MailMessage(fromAddress, toAddress, sub, body);
                testSMPT.EnableSsl = serverConfig.EnableSSL;
                testSMPT.Port = int.Parse(serverConfig.PortNumber);

                if (!string.IsNullOrEmpty(serverConfig.UserName) && !string.IsNullOrEmpty(serverConfig.Password))
                {
                    serverConfig.Password = RSAHelper.DecryptData(serverConfig.Password);
                    testSMPT.Credentials = new System.Net.NetworkCredential(serverConfig.UserName, serverConfig.Password);
                }
                else
                    testSMPT.UseDefaultCredentials = false;

                ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                testSMPT.Send(msg);
                isError = true;
            }
            catch (Exception ex)
            {
                isError = false;
            }
            return isError;
        }
        #endregion         
    }
}