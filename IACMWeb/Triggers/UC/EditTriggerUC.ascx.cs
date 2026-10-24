using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace DI.Chakra.Web.Triggers.UC
{
    public partial class EditTriggerUC : UIBaseUC
    {
        SessionStatusImpl sessionStatus = new SessionStatusImpl();
        public User UserDetail
        {
            get
            {
                if (ViewState["UserDetailObj"] != null)
                {
                    return (User)ViewState["UserDetailObj"];
                }
                return null;
            }
            set
            {
                ViewState["UserDetailObj"] = value;
            }
        }

        public User OldUserDetail
        {
            get
            {
                if (ViewState["OldUserDetailObj"] != null)
                {
                    return (User)ViewState["OldUserDetailObj"];
                }
                return null;
            }
            set
            {
                ViewState["OldUserDetailObj"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindDetailsToView();
            }
            this.MasterPage.ActionClicked += ActionClicked;
        }

        private void BindDetailsToView()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", this.ContextObjectId.Value.ToString());
            var trigger = DomainApiClient<PresentationEmployeeEventTrigger>.GetByAction<PresentationEmployeeEventTrigger>
                (DomainApiAction.Provisioning.Presentation.PresentationEmployeeEventTrigger.GetById, parameter);

            if (trigger.IsRequestCreated)
            {
                this.EffectiveDatePicker.Enabled = false;
                this.ActiveCheckBox.Enabled = false;
                EffectiveDatePicker.MinDate = DateTime.MinValue;
            }
            else
            {
                EffectiveDatePicker.MinDate = trigger.EffectiveDate < DateTime.Now ? trigger.EffectiveDate : DateTime.Now;
            }


            this.UserNameLabel.Text = trigger.UserName;
            this.EventTypeLabel.Text = trigger.EmployeeEvent;
            this.CreatedByLabel.Text = trigger.CreatedBy;
            this.CreatedDateLabel.Text = trigger.CreatedDate.ToString();
            this.RejoiningUserLabel.Text = trigger.RejoiningUser;
            this.AdminCheckRequiredLabel.Text = trigger.AdminCheckRequired;
            this.orgEffectiveDate.Text = trigger.OldEffectiveDate;
            this.RequestCreatedLabel.Text = trigger.RequestCreated;
            this.EffectiveDatePicker.SelectedDate = trigger.EffectiveDate;
            this.ActiveCheckBox.Checked = trigger.IsActive;



            if (trigger.EventType == EmployeeEvents.OUChange || trigger.EventType == EmployeeEvents.ReOrganisation ||
                (trigger.EventType == EmployeeEvents.NewJoinee && trigger.EventType == EmployeeEvents.Rehire))
            {
                this.ShowUserDetailButton.Visible = true;
                string userDetail = string.Empty;
                string oldUserDetail = string.Empty;
                if (!string.IsNullOrEmpty(trigger.UserDetails))
                {
                    var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                    this.UserDetail = user;
                }
                if (!string.IsNullOrEmpty(trigger.OldUserDetails))
                {
                    var user = UtilityMethods.Deserialize<User>(trigger.OldUserDetails);
                    this.OldUserDetail = user;
                }
            }
            else
            {
                this.ShowUserDetailButton.Visible = false;
            }
        }

        private void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                this.SaveTrigger();
            }
        }

        private void SaveTrigger()
        {
            //if (AsyncUploadAttach.UploadedFiles.Count > 0)
            //{
            var trigger = DomainApiClient<Dto.EmployeeEventTrigger>.GetById(this.ContextObjectId.Value);
            if (trigger != null)
            {
                var changes = trigger.EffectiveDate.ToString();
                bool isActiveChanged = trigger.IsActive;
                trigger.IsActive = this.ActiveCheckBox.Checked;
                if (!string.IsNullOrEmpty(this.EffectiveDatePicker.SelectedDate.ToString()))
                {
                    DateTime dt = Convert.ToDateTime(this.EffectiveDatePicker.SelectedDate);
                    trigger.EffectiveDate = dt;
                }
                if(!changes.Equals(trigger.EffectiveDate.ToString(),StringComparison.InvariantCultureIgnoreCase))
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Edit Trigger - Save Trigger Id:{trigger.Id} Before Save: User:{UserNameLabel.Text}  EffectiveDate:{changes} IsActive:{isActiveChanged.GetBoolToString()} After Save:{trigger.EffectiveDate} IsActive:{trigger.IsActive.GetBoolToString()} - Save");
                //DomainApiClient<Dto.EmployeeEventTrigger>.Save(trigger);
                this.ContextObjectId = null;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("currentUser", sessionStatus.User.DisplayName);
                
                DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<bool>("UdpateEventTrigger", parameters, trigger);
            }
            //}
            //else
            //{
            //    MasterPage.ShowAlert(AlertType.Error, "Please attach file to amend Trigger");
            //    return;
            //}
            Response.Redirect("~/Triggers/List.aspx");
        }

        protected void ShowUserDetailButton_Click(object sender, EventArgs e)
        {
            this.UserDetailControl.UserNewDetail = this.UserDetail;
            this.UserDetailControl.UserOldDetail = this.OldUserDetail;
            this.UserDetailWindow.VisibleOnPageLoad = true;
            string script = "function f(){$find(\"" + this.UserDetailWindow.ClientID +
                "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
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

    }
}