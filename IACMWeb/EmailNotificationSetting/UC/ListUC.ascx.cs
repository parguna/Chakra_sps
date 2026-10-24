using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.EmailNotificationSetting.UC
{
    public partial class ListUC : UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                EmailNotificationSettingsGrid.Rebind();
            }
        }
        protected void EmailNotificationSettingsGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ShowInfo")
            {

            }
        }
        protected void EmailNotificationSettingsGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindNotifications();
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
            }
        }
        private void Save()
        {
            List<MailNotificationSettings> settings = DomainApiClient<Dto.MailNotificationSettings>.GetAll();
            if (settings.Count == 0)
                settings = new List<MailNotificationSettings>();
            foreach (GridDataItem item in EmailNotificationSettingsGrid.Items)
            {
                MailNotificationSettings setting = new MailNotificationSettings();
                setting.TriggerName = (string)item["EventType"].Text;
                CheckBox chkJob = (CheckBox)item.FindControl("chkJob");
                setting.MailTriggerFromJob = chkJob.Checked;
                CheckBox chkOnEffectiveDateExecution = (CheckBox)item.FindControl("chkOnEffectiveDateExecution");
                setting.MailTriggerOnEffectiveDate = chkOnEffectiveDateExecution.Checked;

                TextBox txtFrom = (TextBox)item.FindControl("txtFrom");
                setting.From = txtFrom.Text;
                TextBox txtTo = (TextBox)item.FindControl("txtTo");
                setting.To = txtTo.Text;
                TextBox txtCC = (TextBox)item.FindControl("txtCC");
                setting.Cc = txtCC.Text;
                TextBox txtPreconfiguredDays = (TextBox)item.FindControl("txtPreconfigureNotificationDays");
                setting.PreConfigureNotificationDays = txtPreconfiguredDays.Text != string.Empty ? Convert.ToInt32(txtPreconfiguredDays.Text) : 0;

                if (settings.Any(m => m.TriggerName == setting.TriggerName))
                    setting.Id = settings.Where(m => m.TriggerName == setting.TriggerName).FirstOrDefault().Id;
                settings.Add(setting);
            }
            if(settings.Count > 0)
            {
                DomainApiClient<Dto.MailNotificationSettings>.SaveList(settings);
            }
        }
        private void BindNotifications()
        {
            var array = Enum.GetValues(typeof(EmployeeEvents));
            EmailNotificationSettingsGrid.DataSource = array;
        }

        protected void chkOnEffectiveDateExecution_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            GridDataItem item = (GridDataItem)chk.Parent.Parent;
            if (chk.Checked)
            {
                TextBox txtPreconfiguredDays = (TextBox)item.FindControl("txtPreconfigureNotificationDays");
                txtPreconfiguredDays.Enabled = false;
            }
            CheckBox chkJob = (CheckBox)item.FindControl("chkJob");
            if (chkJob.Checked) chkJob.Checked = false;
        }

        protected void chkJob_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            GridDataItem item = (GridDataItem)chk.Parent.Parent;
            TextBox txtPreconfiguredDays = (TextBox)item.FindControl("txtPreconfigureNotificationDays");
            txtPreconfiguredDays.Enabled = chk.Checked;
            txtPreconfiguredDays.Text = "";
            CheckBox chkEffectiveDate = (CheckBox)item.FindControl("chkOnEffectiveDateExecution");
            if (chkEffectiveDate.Checked) chkEffectiveDate.Checked = false;
        }

        protected void EmailNotificationSettingsGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            //if(e.Item is GridDataItem)
            //{
            //    GridDataItem item = (GridDataItem)e.Item;
            //    CheckBox chkJobBox = (CheckBox)item["TemplateColumn"].FindControl("chkJob");
            //    CheckBox chkOnEffectiveDateBox = (CheckBox)item["TemplateColumn"].FindControl("chkOnEffectiveDateExecution");
            //    Dictionary<string, string> parameters = new Dictionary<string, string>();
            //    parameters.Add("TriggerName", item["EventType"].Text.ToString());
            //    MailNotificationSettings mailSettings = DomainApiClient<Dto.MailNotificationSettings>.GetByAction<Dto.MailNotificationSettings>("GetMailNotificationSettingsByTrigger", parameters);
            //    if(mailSettings != null)
            //    {
            //        chkJobBox.Checked = mailSettings.MailTriggerFromJob;
            //        chkOnEffectiveDateBox.Checked = mailSettings.MailTriggerOnEffectiveDate;
            //    }
                
            //}
        }

        protected void EmailNotificationSettingsGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem && e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                CheckBox chkJobBox = (CheckBox)item["TemplateColumn"].FindControl("chkJob");
                CheckBox chkOnEffectiveDateBox = (CheckBox)item["TemplateColumn"].FindControl("chkOnEffectiveDateExecution");
                TextBox txtFrom = (TextBox)item["TemplateColumn"].FindControl("txtFrom");
                TextBox txtTo = (TextBox)item["TemplateColumn"].FindControl("txtTo");
                TextBox txtCC = (TextBox)item["TemplateColumn"].FindControl("txtCC");
                TextBox txtPreconfigureNotificationDays = (TextBox)item["TemplateColumn"].FindControl("txtPreconfigureNotificationDays");
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("TriggerName", item["EventType"].Text.ToString());
                MailNotificationSettings mailSettings = DomainApiClient<Dto.MailNotificationSettings>.GetByAction<Dto.MailNotificationSettings>("GetMailNotificationSettingsByTrigger", parameters);
                if (mailSettings != null)
                {
                    chkJobBox.Checked = mailSettings.MailTriggerFromJob;
                    chkOnEffectiveDateBox.Checked = mailSettings.MailTriggerOnEffectiveDate;
                    txtFrom.Text = mailSettings.From;
                    txtTo.Text = mailSettings.To;
                    txtCC.Text = mailSettings.Cc;
                    txtPreconfigureNotificationDays.Text = mailSettings.PreConfigureNotificationDays > 0 ? mailSettings.PreConfigureNotificationDays.ToString() : "";
                }

            }
        }
    }
}