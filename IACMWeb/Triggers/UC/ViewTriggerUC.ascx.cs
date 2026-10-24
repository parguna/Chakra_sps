using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Text;
using System.Web.UI;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using DI.Chakra.Dto.Presentation;
using System.Web;

namespace DI.Chakra.Web.Triggers.UC
{
    public partial class ViewTriggerUC : UIBaseUC
    {
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
        }

        
        private void BindDetailsToView()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", this.ContextObjectId.Value.ToString());
            var trigger = DomainApiClient<PresentationEmployeeEventTrigger>.GetByAction<PresentationEmployeeEventTrigger>
                (DomainApiAction.Provisioning.Presentation.PresentationEmployeeEventTrigger.GetById, parameter);
            this.UserNameLabel.Text = trigger.UserName;
            this.EventTypeLabel.Text = trigger.EmployeeEvent;
            this.EffectiveDateLabel.Text = trigger.EffectiveDate.ToString();
            this.CreatedByLabel.Text = trigger.CreatedBy;
            this.CreatedDateLabel.Text = trigger.CreatedDate.ToString();
            this.ActiveLabel.Text = trigger.Active;
            //this.RejoiningUserLabel.Text = trigger.RejoiningUser;
            this.AdminCheckRequiredLabel.Text = trigger.AdminCheckRequired;
            this.RequestCreatedLabel.Text = trigger.RequestCreated;
            this.orgEffectiveDate.Text = trigger.OldEffectiveDate;

            if (trigger.EventType == EmployeeEvents.OUChange || trigger.EventType == EmployeeEvents.ReOrganisation ||
                trigger.EventType == EmployeeEvents.Rehire)
            {
                this.ShowUserDetailButton.Visible = true;
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

        protected void ShowUserDetailButton_Click(object sender, EventArgs e)
        {
            this.UserDetailControl.UserNewDetail = this.UserDetail;
            this.UserDetailControl.UserOldDetail = this.OldUserDetail;
            this.UserDetailWindow.VisibleOnPageLoad = true;
            string script = "function f(){$find(\"" + 
                this.UserDetailWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }

    }
}