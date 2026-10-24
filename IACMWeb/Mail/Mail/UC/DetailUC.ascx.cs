using System;
using System.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;

namespace DI.Chakra.Web.Mail.Mail.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Members      
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";       
        #endregion

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

        #region methods
        private void SetPage()
        {
            Dto.EmailInfo MailList;
            Dto.EmailInfoHistory sentMailList;
            string mailStatus;

            if (Session["MAIL_STATUS"] != null)
                mailStatus = Session["MAIL_STATUS"].ToString();
            else
                mailStatus = string.Empty;
            if (mailStatus.ToUpper() == "SENT")
            {
                sentMailList = DomainApiClient<Dto.EmailInfoHistory>.GetById(EntityId);
                if (null == sentMailList)
                    return;
                lblFrom.Text = sentMailList.From;
                txtTo.Text = sentMailList.To;
                txtCC.Text = sentMailList.Cc;
                txtBcc.Text = sentMailList.Bcc;
                txtSubject.Text = sentMailList.Subject;
                RadEditor1.Content = sentMailList.Body;
            }
            else
            {
                MailList = DomainApiClient<Dto.EmailInfo>.GetById(EntityId);
                if (null == MailList)
                    return;
                lblFrom.Text = MailList.From;
                txtTo.Text = MailList.To;
                txtCC.Text = MailList.Cc;
                txtBcc.Text = MailList.Bcc;
                txtSubject.Text = MailList.Subject;
                RadEditor1.Content = MailList.Body;
            }
            
        }
        private void Save()
        {
            Dto.EmailInfo mailInfo = new Dto.EmailInfo();
            mailInfo.Id = EntityId;
            mailInfo.To = txtTo.Text;
            mailInfo.Cc = txtCC.Text;
            mailInfo.Bcc = txtBcc.Text;
            mailInfo.Subject = txtSubject.Text;
            mailInfo.Body = RadEditor1.Content;
            DomainApiClient<Dto.EmailInfo>.Save(mailInfo);
            Response.Redirect("~/Mail/Mail/List.aspx");
        }
        private void ViewOnlyMode()
        {
            RadEditor1.EditModes = Telerik.Web.UI.EditModes.Preview;
            ContentWrapper.Enabled = false;
        }
        #endregion
        public string ActivityType
        {

            get
            {
                if (ViewState["ActivityType"] == null || string.IsNullOrWhiteSpace(ViewState["ActivityType"].ToString())) return null;
                return ViewState["ActivityType"].ToString();
            }
            set
            {
                ViewState["ActivityType"] = value;
            }
        }

        public string FromDate
        {
            get
            {
                if (ViewState["FromDate"] == null || string.IsNullOrWhiteSpace(ViewState["FromDate"].ToString())) return null;
                return ViewState["FromDate"].ToString();
            }
            set
            {
                ViewState["FromDate"] = value;
            }
        }

        public string ToDate
        {
            get
            {
                if (ViewState["ToDate"] == null || string.IsNullOrWhiteSpace(ViewState["ToDate"].ToString())) return null;
                return ViewState["ToDate"].ToString();
            }
            set
            {
                ViewState["ToDate"] = value;
            }
        }

        public string Status
        {

            get
            {
                if (ViewState["Status"] == null || string.IsNullOrWhiteSpace(ViewState["Status"].ToString())) return null;
                return ViewState["Status"].ToString();
            }
            set
            {
                ViewState["Status"] = value;
            }
        }
        #region events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                ActivityType = Request.QueryString["activityType"];
                FromDate = Request.QueryString["fromDate"];
                ToDate = Request.QueryString["toDate"];
                Status = Request.QueryString["Status"];
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            var url = "~/Mail/Mail/List.aspx";
            if (!string.IsNullOrWhiteSpace(this.ActivityType) && !string.IsNullOrWhiteSpace(this.Status) && !string.IsNullOrWhiteSpace(this.FromDate) && !string.IsNullOrWhiteSpace(this.ToDate))
            {
                url = url + $"?activityType={this.ActivityType}&Status={Status}&fromDate={FromDate}&toDate={ToDate}";
            }
            Response.Redirect(url);
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
            }
        }
        #endregion
    }
}