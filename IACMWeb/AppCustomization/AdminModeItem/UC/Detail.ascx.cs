using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.AppCustomization.AdminModeItem.UC
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
                BindAdminType();
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
                Save();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AppCustomization/AdminModeItem/List.aspx");
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            
            var adminModeItem = DomainApiClient<Dto.AdminModeItem>.GetById(this.EntityId);
            if (null == adminModeItem)
                return;

            txtName.Text = adminModeItem.Name;
            txtDescription.Text = adminModeItem.Description;
            txtIcon.Text = adminModeItem.Icon;
            txtInformation.Text = adminModeItem.Information;
            if (ddlAdminType.Items.FindByValue(((int)adminModeItem.Type).ToString()) != null)
                ddlAdminType.Items.FindByValue(((int)adminModeItem.Type).ToString()).Selected = true;

            ACLDataMethodSearchUC.SelectedParentTabMenu = DomainApiClient<Dto.TabMenu>.GetById(adminModeItem.TabMenuId);
            txtTableName.Text = adminModeItem.TableName;
        }

        private void Save()
        {
            if (txtName.Text.Trim() == "")
            {
                MasterPage.ShowAlert(AlertType.Error, "Please enter a valid name.");
                return;
            }

            if (txtDescription.Text.Trim() == "")
            {
                MasterPage.ShowAlert(AlertType.Error, "Please enter a Description.");
                return;
            }

            if (txtIcon.Text.Trim() == "")
            {
                MasterPage.ShowAlert(AlertType.Error, "Please enter a Icon.");
                return;
            }

            if (txtInformation.Text.Trim() == "")
            {
                MasterPage.ShowAlert(AlertType.Error, "Please enter a valid information.");
                return;
            }

            if (ACLDataMethodSearchUC.ChildAutoCompleteBox.Entries == null || ACLDataMethodSearchUC.ChildAutoCompleteBox.Entries.Count == 0)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select menu.");
                return;
            }

            if (ddlAdminType.SelectedValue == "--Select--")
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select Type.");
                return;
            }

            Dto.AdminModeItem adminModeItem = new Dto.AdminModeItem();
            adminModeItem.Id = EntityId;
            adminModeItem.Name = txtName.Text;
            adminModeItem.Description = txtDescription.Text;
            adminModeItem.Icon = txtIcon.Text;
            adminModeItem.Information = txtInformation.Text;
            adminModeItem.Type = (AdminModeItemType)Convert.ToInt32(ddlAdminType.SelectedValue);
            adminModeItem.TableName = txtTableName.Text;

            if (ACLDataMethodSearchUC.ChildAutoCompleteBox.Entries.Count > 0)
                adminModeItem.TabMenuId = Convert.ToInt64(ACLDataMethodSearchUC.ChildAutoCompleteBox.Entries[0].Value);

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("name", adminModeItem.Name.Trim());
            
            if (UIMode == UIMode.Add && DomainApiClient<Dto.AdminModeItem>.GetByAction<bool>
                (DomainApiAction.Preference.AdminModeItem.DoesExists, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Admin Item Name already exists.");
                return;
            }
            DomainApiClient<Dto.AdminModeItem>.Save(adminModeItem);
            Response.Redirect("~/AppCustomization/AdminModeItem/List.aspx");
        }

        private void ViewOnlyMode()
        {
            ContentWrapper.Disabled = true;
        }
        #endregion  
             
        public void BindAdminType()
        {         
            ddlAdminType.Items.Insert(0, "--Select--");
            try
            {
                foreach (AdminModeItemType item in Enum.GetValues(typeof(AdminModeItemType)))
                {       
                    var description = UtilityMethods.GetEnumAttribute<Description>(item);
                    ListItem listItem = new ListItem(description, ((byte)item).ToString());
                    ddlAdminType.Items.Add(listItem);
                }             
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}