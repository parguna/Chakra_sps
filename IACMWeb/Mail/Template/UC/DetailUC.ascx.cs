using Application;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using DI.Chakra.Infrastructure;
using System.Linq;
using System.Collections.Generic;
using Telerik.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.Mail.Template.UC
{
    public partial class DetailUC : UIBaseUC, IAccessControl
    {
        #region Members

        private const string V_EntityID = "V_EntityID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_MAILTO = "V_MAILTO";
        private const string V_MAIL_KEYWORD = "V_MAIL_KEYWORD";
        private const string V_SELECTEDTOMAIL = "V_SELECTEDTOMAIL";
        private const string V_SELECTEDCCMAIL = "V_SELECTEDCCMAIL";
        private const string V_MAILCC = "V_MAILCC";

        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        #endregion

        public Dictionary<string, string> SelectedToEmails
        {
            get
            {
                if (ViewState[V_SELECTEDTOMAIL] != null)
                {
                    return (Dictionary<string, string>)ViewState[V_SELECTEDTOMAIL];
                }
                return new Dictionary<string, string>();
            }
            set
            {
                ViewState[V_SELECTEDTOMAIL] = value;
            }
        }

        public Dictionary<string, string> SelectedCCEmails
        {
            get
            {
                if (ViewState[V_SELECTEDCCMAIL] != null)
                {
                    return (Dictionary<string, string>)ViewState[V_SELECTEDCCMAIL];
                }
                return new Dictionary<string, string>();
            }
            set
            {
                ViewState[V_SELECTEDCCMAIL] = value;
            }
        }

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
        public string EmailTo
        {
            get
            {
                if (ViewState[V_MAILTO] == null)
                    return string.Empty;
                return (string)ViewState[V_MAILTO];
            }
            set
            {
                ViewState[V_MAILTO] = value;
            }
        }

        public string EmailCC
        {
            get
            {
                if (ViewState[V_MAILCC] == null)
                    return string.Empty;
                return (string)ViewState[V_MAILCC];
            }
            set
            {
                ViewState[V_MAILCC] = value;
            }
        }

        public List<string> EmailKeyWord
        {
            get
            {
                if (ViewState[V_MAIL_KEYWORD] == null)
                    return new List<string>();
                return (List<string>)ViewState[V_MAIL_KEYWORD];
            }
            set
            {
                ViewState[V_MAIL_KEYWORD] = value;
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

        #region Event
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                LoadMailKeyword();

                //MailTemplateEditor.Languages.Add(new SpellCheckerLanguage("en-US", "English"));
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

            //this.PopulateTemplateVariables();

        }

        public void RemoveButton(string name)
        {
            try
            {
                foreach (Telerik.Web.UI.EditorToolGroup group in MailTemplateEditor.Tools)
                {
                    EditorTool tool = group.FindTool(name);
                    if (tool != null)
                    {
                        group.Tools.Remove(tool);
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void LoadMailKeyword()
        {
            var keyWords = GetConfigurationMaster("MailKeywords", Product.IACM);
            EmailKeyWord.Clear();
            if (keyWords != null && !string.IsNullOrEmpty(keyWords.ConfigValue))
            {
                var listOfKeyWords = keyWords.ConfigValue.Split(',').ToList().Select(a => a.Trim()).ToList();
                EmailKeyWord = listOfKeyWords;
            }
        }

        private Dto.IConfigurationMaster GetConfigurationMaster(string ConfigKey, Product product)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigKey);
            parameter.Add("product", product.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);
            return config;
        }


        private void SetPage()
        {
            var mail = DomainApiClient<Dto.MailTemplate>.GetById(this.EntityId);
            if (null == mail)
                return;
            txtTempName.Text = mail.TemplateName;
            txtTempDesc.Text = mail.TemplateDescription;
            txtMailFrm.Text = mail.MailFrom;
            txtMailSubj.Text = mail.MailSubject;
            MailTemplateEditor.Content = mail.MailContent;
            EmailTo = mail.MailTo;
            EmailCC = mail.MailCC;

            if (!string.IsNullOrEmpty(EmailTo))
            {
                foreach (var to in EmailTo.Split(','))
                {
                    AddToEmailUnSaved(to.Trim(), "Existing");
                }
            }

            if (!string.IsNullOrEmpty(EmailCC))
            {
                foreach (var to in EmailCC.Split(','))
                {
                    AddCCEmailUnSaved(to.Trim(), "Existing");
                }
            }
        }
        private void Save()
        {
            MailTemplate mailTemplate = new MailTemplate();
            mailTemplate.Id = this.EntityId;
            mailTemplate.TemplateName = txtTempName.Text;

            if (UIMode == UIMode.Add)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("templateName", txtTempName.Text);
                bool isvalid = DomainApiClient<Dto.MailTemplate>.GetByAction<bool>(DomainApiAction.Email.MailTemplate.DoesNameAlreadyExists, parameter);
                if (isvalid)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Template Name already exists");
                    return;
                }
            }

            //if (SelectedToEmails == null || SelectedToEmails.Count <= 0)
            //{
            //    MasterPage.ShowAlert(AlertType.Error, "Mail to cannot be empty.");
            //    return;
            //}

            if (SelectedToEmails != null && SelectedToEmails.Count > 0 && !SelectedToEmails.Any(a => a.Value == "Added" || a.Value == "Existing"))
            {
                MasterPage.ShowAlert(AlertType.Error, "Mail to cannot be empty.");
                return;
            }

            EmailTo = string.Join(",", SelectedToEmails.Where(a => a.Value == "Added" || a.Value == "Existing").Select(a => a.Key).Distinct().ToList());
            EmailCC = string.Join(",", SelectedCCEmails.Where(a => a.Value == "Added" || a.Value == "Existing").Select(a => a.Key).Distinct().ToList());

            mailTemplate.TemplateDescription = txtTempDesc.Text;
            mailTemplate.MailContent = MailTemplateEditor.Content;
            mailTemplate.MailTo = EmailTo;
            mailTemplate.MailCC = EmailCC;
            mailTemplate.MailFrom = txtMailFrm.Text;
            mailTemplate.MailSubject = txtMailSubj.Text;

            DomainApiClient<Dto.MailTemplate>.Save(mailTemplate);
            Response.Redirect("~/Mail/Template/List.aspx");
        }
        private void ViewOnlyMode()
        {
            ContentWrapper.Enabled = false;
            //MailTemplateEditor.Enabled = false;
            MailTemplateEditor.EditModes = EditModes.Preview;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Mail/Template/List.aspx");
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Mail Template - Save");
            }
        }
        #endregion


        protected void ddlTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var toOption = ddlTo.SelectedItem.Text.Trim();
            UserSearchUC.Visible = false;
            txtTo.Visible = false;
            AddToEmail.Visible = false;

            if (toOption == "User")
            {
                UserSearchUC.Visible = true;
                AddToEmail.Visible = true;
            }
            else if (toOption == "Free Text")
            {
                txtTo.Visible = true;
                AddToEmail.Visible = true;
            }
        }

        protected void AddToEmailButton_Click(object sender, EventArgs e)
        {
            bool loadEpHierrchy = false;
            var funId = string.Empty;

            var toOption = ddlTo.SelectedItem.Text.Trim();
            if (toOption == "User")
            {
                if (UserSearchUC.SelectedUser != null)
                    funId = UserSearchUC.SelectedUser.EmailId;
            }
            else if (toOption == "Free Text")
            {
                if (txtTo.Text.Trim() != "")
                    funId = txtTo.Text.Trim();
            }

            if (!string.IsNullOrEmpty(funId))
            {
                var fns = this.SelectedToEmails;
                if (!fns.Any(a => a.Key.ToLower().Trim() == funId))
                {
                    loadEpHierrchy = true;
                }

                AddToEmailUnSaved(funId, "Added");

                this.RebindToEmailGrid();
            }
        }

        private void RebindToEmailGrid()
        {
            ToEmail_FormGrid.DataSource = this.SelectedToEmails;
            this.ToEmail_FormGrid.Rebind();
            ToEmail_FormGrid.Enabled = true;
            if (ToEmail_FormGrid.Items.Count > 0)
            {
                ToEmail_FormGrid.Visible = true;
            }
            else
            {
                ToEmail_FormGrid.Visible = false;
            }
        }

        private void AddToEmailUnSaved(string funId, string action)
        {
            var fns = this.SelectedToEmails;
            if (!fns.Any(a => a.Key.Trim() == funId))
            {
                fns.Add(funId, action);
            }
            else
                fns[funId] = action;

            this.SelectedToEmails = fns;
        }

        protected void ToEmail_FormGrid_PreRender(object sender, EventArgs e)
        {
            foreach (GridColumn col in ToEmail_FormGrid.Columns)
            {
                if (col.UniqueName == "Key")
                    col.HeaderText = T("App.Common.ToEmail");
            }
            this.ToEmail_FormGrid.DataSource = this.SelectedToEmails;
            ToEmail_FormGrid.Rebind();
        }

        protected void ToEmail_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                string epId = (string)Convert.ChangeType(item.GetDataKeyValue("Key"), typeof(string));
                var fns = this.SelectedToEmails;
                if (fns.Any(a => a.Key == epId))
                    fns[epId] = "Deleted";
                this.SelectedToEmails = fns;
                this.RebindToEmailGrid();
            }
        }

        protected void ToEmail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                if ((UIMode == UIMode.View) | (UIMode == UIMode.Edit))
                {
                    //(item["DeleteColumn"].Controls[0] as ImageButton).Enabled = false;
                }
                else
                {
                    //(item["DeleteColumn"].Controls[0] as ImageButton).Enabled = true;
                }
            }
        }


        protected void ddlCC_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ccOption = ddlCC.SelectedItem.Text.Trim();
            CCUserSearchUC.Visible = false;
            txtCC.Visible = false;
            AddCCEmailButton.Visible = false;

            if (ccOption == "User")
            {
                CCUserSearchUC.Visible = true;
                AddCCEmailButton.Visible = true;
            }
            else if (ccOption == "Free Text")
            {
                txtCC.Visible = true;
                AddCCEmailButton.Visible = true;
            }
        }

        protected void AddCCEmailButton_Click(object sender, EventArgs e)
        {
            bool loadEpHierrchy = false;
            var funId = string.Empty;

            var ccOption = ddlCC.SelectedItem.Text.Trim();
            if (ccOption == "User")
            {
                if (CCUserSearchUC.SelectedUser != null)
                    funId = CCUserSearchUC.SelectedUser.EmailId;
            }
            else if (ccOption == "Free Text")
            {
                if (txtCC.Text.Trim() != "")
                    funId = txtCC.Text.Trim();
            }

            if (!string.IsNullOrEmpty(funId))
            {
                var fns = this.SelectedCCEmails;
                if (!fns.Any(a => a.Key.ToLower().Trim() == funId))
                {
                    loadEpHierrchy = true;
                }

                AddCCEmailUnSaved(funId, "Added");

                this.RebindCCEmailGrid();
            }
        }

        private void RebindCCEmailGrid()
        {
            CCEmail_FormGrid.DataSource = this.SelectedCCEmails;
            this.CCEmail_FormGrid.Rebind();
            CCEmail_FormGrid.Enabled = true;
            if (CCEmail_FormGrid.Items.Count > 0)
            {
                CCEmail_FormGrid.Visible = true;
            }
            else
            {
                CCEmail_FormGrid.Visible = false;
            }
        }

        private void AddCCEmailUnSaved(string funId, string action)
        {
            var fns = this.SelectedCCEmails;
            if (!fns.Any(a => a.Key.Trim() == funId))
            {
                fns.Add(funId, action);
            }
            else
                fns[funId] = action;

            this.SelectedCCEmails = fns;
        }

        protected void CCEmail_FormGrid_PreRender(object sender, EventArgs e)
        {
            foreach (GridColumn col in ToEmail_FormGrid.Columns)
            {
                if (col.UniqueName == "Key")
                    col.HeaderText = T("App.Common.CCEmail");
            }
            this.CCEmail_FormGrid.DataSource = this.SelectedCCEmails;
            CCEmail_FormGrid.Rebind();
        }

        protected void CCEmail_FormGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                string epId = (string)Convert.ChangeType(item.GetDataKeyValue("Key"), typeof(string));
                var fns = this.SelectedCCEmails;
                if (fns.Any(a => a.Key == epId))
                    fns[epId] = "Deleted";
                this.SelectedCCEmails = fns;
                this.RebindCCEmailGrid();
            }
        }

        protected void CCEmail_FormGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                if ((UIMode == UIMode.View) | (UIMode == UIMode.Edit))
                {
                    //(item["DeleteColumn"].Controls[0] as ImageButton).Enabled = false;
                }
                else
                {
                    //(item["DeleteColumn"].Controls[0] as ImageButton).Enabled = true;
                }
            }
        }
    }
}