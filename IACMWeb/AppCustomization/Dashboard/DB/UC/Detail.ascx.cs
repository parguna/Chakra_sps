using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace DI.Chakra.Web.AppCustomization.Dashboard.DB.UC
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
            MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                if (UIMode == UIMode.Edit || UIMode == UIMode.View)
                {
                    if (ContextObjectId == null)
                        return;
                    EntityId = ContextObjectId.Value;
                    SetPage();
                }
                else if (UIMode == UIMode.Add)
                    BindDashBoardType();
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
            Response.Redirect("~/AppCustomization/Dashboard/DB/List.aspx");
        }
        #endregion

        #region Methods
        private void SetPage()
        {           
            var list = DomainApiClient<Dto.Dashboard>.GetById(EntityId);
            txtCategory.Text= list.DashboardCategory;
            txtName.Text=list.DashboardName;
            BindDashBoardType();
            ddlSelectItem(ddlDBType, ((int)list.DashBoardType).ToString());

            //if (list..HasValue)
            //{
            //    TargetUrlAutoCompleteBox.SelectedUrl = aclFacade.GetPageUrlById(list.PageUrlId.Value);
            //}

            //if (list.PageUrlId.HasValue)
            //{
            //    NavigateUrlAutoCompleteBox.SelectedUrl = aclFacade.GetPageUrlById(list.PageUrlId.Value);
            //}

            txtTargetURL.Text = list.TargetUrl;
            txtNavigateURL.Text = list.NavigateUrl;
        }

        private void Save()
        {
            Dto.Dashboard itemInfo = new Dto.Dashboard();
            if (ddlDBType.SelectedValue == "--Select--")
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select all the values.");
                return;
            }
            itemInfo.Id = EntityId;
            itemInfo.DashboardCategory = txtCategory.Text;
            itemInfo.DashboardName = txtName.Text;
            itemInfo.DashBoardType = (DashboardType)Convert.ToInt16( ddlDBType.SelectedValue);
            itemInfo.TargetUrl = txtTargetURL.Text;
            itemInfo.NavigateUrl = txtNavigateURL.Text;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("name", txtName.Text);
            parameter.Add("id", EntityId.ToString());

            if (DomainApiClient<Dto.Dashboard>.GetByAction<bool>
                (DomainApiAction.Preference.Dashboard.DoesExists, parameter))
            {
                MasterPage.ShowAlert(AlertType.Error, "Dashboard Already Exists.");
                return;
            }
            DomainApiClient<Dto.Dashboard>.Save(itemInfo);
            Response.Redirect("~/AppCustomization/Dashboard/DB/List.aspx");
        }

        private void ViewOnlyMode()
        {
            ContentWrapper.Disabled = true;
        }

        private void ddlSelectItem(DropDownList ddl, string strValue)
        {
            if (ddl.Items.FindByValue(strValue) != null)
                ddl.Items.FindByValue(strValue).Selected = true;
        }

        public void BindDashBoardType()
        {
            ddlDBType.Items.Insert(0, "--Select--");
            try
            {
                foreach (DashboardType item in Enum.GetValues(typeof(DashboardType)))
                {
                    var description = UtilityMethods.GetEnumAttribute<Description>(item);
                    ListItem listItem = new ListItem(description, ((byte)item).ToString());
                    ddlDBType.Items.Add(listItem);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}