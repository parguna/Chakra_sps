// have to give correct type for GetByAction<Object>
using Application;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Master.UC
{
    public partial class QuickNavigator : DI.Chakra.Web.UIBaseUC
    {
        #region Class MemberVariable
        private const string S_DELETEICONVISIBLE = "S_DELETEICONVISIBLE";
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        private Role role = new Role();
        #endregion
        public string StatusButton
        {
            get
            {
                if (Session["statusButton"]==null)
                {
                   Session["statusButton"] = "Customize";
                }
                return Session["statusButton"].ToString();
            }
            set
            {
                Session["statusButton"] = value;
            }
        }


        public bool DeleteIconVisible
        {
            get
            {
                if (Session[S_DELETEICONVISIBLE] == null)
                    return false;
                return (bool)Session[S_DELETEICONVISIBLE];
            }
            set
            {
                Session[S_DELETEICONVISIBLE] = value;
            }
        }

        #region Page Event
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (sessionStatus.User.Roles != null)
                {
                    if (sessionStatus.User.Roles.Count > 0)
                    {
                        role = sessionStatus.User.Roles[0];
                        BindUserPreferenceLinks();
                    }
                }
            }
          
            Session["statusButton"] = this.StatusButton;
            this.btnCustomize.Text= this.StatusButton;
            if(Page.Title!= "Preferences")
            {
                this.StatusButton = "Customize";
                this.btnCustomize.Text = this.StatusButton;
                foreach(RepeaterItem items in  this.rptQuickLinks.Items)
                {
                    ((ImageButton)items.FindControl("imgDelete")).Visible = false;
                }
   
            }
           

        }
        #endregion

        #region Helper
        /// <summary>
        /// Bind User Selected Menus in the master page.
        /// If its first time or not added/modified by user, then Bind default menus based on user Role.
        /// </summary>
        private void BindUserPreferenceLinks()
        {
            if (sessionStatus.User != null && sessionStatus.User.Roles.Count > 0)
            {
                    this.rptQuickLinks.DataSource = (IList<UserPreference>)Session["QuickNav"];
                    this.rptQuickLinks.DataBind();
            }
        }
        #endregion

        protected void imgDelete_Click(object sender, ImageClickEventArgs e)
        {
            role = sessionStatus.User.Roles[0];
            var img = (Image)sender;
            var item = (RepeaterItem)img.NamingContainer;
            var hidd = (HiddenField)item.FindControl("TabMemnuId");
            long tabmenuId = Convert.ToInt64(hidd.Value.ToString());

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("userId", sessionStatus.User.Id.ToString());
            IList<UserPreference> deleteuser =
                DomainApiClient<Dto.UserPreference>.GetByAction<IList<Dto.UserPreference>>
                (DomainApiAction.Preference.UserPreference.GetUserPreferenceMenus, para);

            if (deleteuser.Count == 0)
            {
                Dictionary<string, string> paramtr = new Dictionary<string, string>();
                paramtr.Add("roleId", role.Id.ToString());
                IList<UserPreference> menus =
                    DomainApiClient<Dto.UserPreference>.GetByAction<IList<Dto.UserPreference>>
                    (DomainApiAction.Preference.UserPreference.GetDefaultMenus, paramtr);

                foreach (UserPreference menu in menus)
                {
                    UserQuickLink selectedmenu = new UserQuickLink();
                    selectedmenu.QuickLinkId = (long)menu.Id;
                    selectedmenu.Name = menu.MenuName;
                    selectedmenu.UserId = sessionStatus.User.Id;
                    selectedmenu.Position = menu.Position;
                    DomainApiClient<Dto.UserQuickLink>.Save(selectedmenu);
                }
            }
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", this.sessionStatus.UserId.ToString());
            UserPreference menulink =
                DomainApiClient<Dto.UserPreference>.GetByAction<IList<Dto.UserPreference>>
                (DomainApiAction.Preference.UserPreference.GetUserPreferenceMenus, parameter).
                Where(lnk => lnk.TabMenuId == tabmenuId).FirstOrDefault();
           
            if (menulink != null)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("quickLinkId", menulink.Id.ToString());
                param.Add("userId", this.sessionStatus.UserId.ToString());
                DomainApiClient<Dto.UserQuickLink>.GetByAction<Object>(DomainApiAction.Preference.UserQuickLink.Delete, param);   
            }
            LoadQuickMenu();
            BindUserPreferenceLinks();
            this.rptQuickLinks.DataBind();
            Response.Redirect("~/Preferences/Pref.aspx");
        }

        private void LoadQuickMenu()
        {
            if (sessionStatus.User != null && sessionStatus.User.Roles.Count > 0)
            {
                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("userId", sessionStatus.User.Id.ToString());
                IList<UserPreference> menus =
                    DomainApiClient<Dto.UserPreference>.GetByAction<IList<Dto.UserPreference>>
                    (DomainApiAction.Preference.UserPreference.GetUserPreferenceMenus, para);
                if (menus.Count > 0)
                {
                    Session["QuickNav"] = menus;
                    Session["QuickLink"] = "UserQuickLink";
                }
                else
                {
                    Dictionary<string, string> paramtr = new Dictionary<string, string>();
                    paramtr.Add("roleId", sessionStatus.User.Roles[0].Id.ToString());
                    menus = DomainApiClient<Dto.UserPreference>.GetByAction<IList<Dto.UserPreference>>
                              (DomainApiAction.Preference.UserPreference.GetDefaultMenus, paramtr);

                    Session["QuickNav"] = menus;
                    Session["QuickLink"] = "DefaultRoleQuickLink";
                    if (menus == null || menus.Count < 5)
                    {
                        Session["QuickLink"] = "UserQuickLink";
                    }
                }
            }
        }
        protected void btnCustomize_Click(object sender, EventArgs e)
        {
            if (DeleteIconVisible == false)
            {
                this.StatusButton = "Done";
                this.DeleteIconVisible = true;
                Response.Redirect("~/Preferences/Pref.aspx");
            }
            else
            {
                this.StatusButton = "Customize";
                this.DeleteIconVisible = false;
                Response.Redirect("~/Home/Home.aspx");
            }
            BindUserPreferenceLinks();          
        }
    }
}