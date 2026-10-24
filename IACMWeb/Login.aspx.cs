using Application;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Web.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web
{
    public partial class Login : FrontUIPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //Calling this method to clear the existing session if any
                LoginHelper.ClearUserSessionAndAuthendicationCookie();

                try
                {
                    //txtUserName.Focus();
                    lblHeadingNotes.Text = MasterConfigHelper.LoginHeadingNotes;
                    lblSubHeadingNotes.Text = MasterConfigHelper.LoginSubHeadingNotes;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            //if(string.IsNullOrWhiteSpace(txtUserName.Text))
            //{
            //    MasterPage.ShowAlert(AlertType.Error, "Please enter the username");
            //    return;
            //}
            //else if (string.IsNullOrWhiteSpace(txtPassword.Text))
            //{
            //    MasterPage.ShowAlert(AlertType.Error, "Please enter the password");
            //    return;
            //}
            LoginHelper loginHelper = new LoginHelper(txtUserName.Text, txtPassword.Text);

            string validationMessage = string.Empty;

            bool isValid = false;

            try
            {
                isValid = loginHelper.IsValid(true, out validationMessage);
                var userName = txtUserName.Text;
                if (isValid)
                {
                    long userId = new SessionStatusImpl().User.Id;
                    Session["Lastlogininfo"] = new SessionStatusImpl().User.LastLogonDate;
                    Session["LoginUserName"] = new SessionStatusImpl().User.UserName;
                    Session["LoginUserId"] = userId;
                    DomainApiClient<DI.Chakra.Dto.User>.GetByAction<object>(DomainApiAction.User.UpdateLastLogonInformation, null, new SessionStatusImpl().User);
                    DILog.UILogger.WriteLog(userId, userName, LogLevelType.Info, string.Format("IACM Authentication Successful for the User: {0}", userName),null);
                    FillUserAccess(userId);
                }
                else
                {
                    
                    DILog.UILogger.WriteLog(0, userName, LogLevelType.Info, string.Format("IACM Authentication Failed for the User: {0}", userName),null);
                    MasterPage.ShowAlert(AlertType.Error, validationMessage);
                }
            }
            catch (Exception)
            {

                throw;
            }

            if (isValid)
            {
                string landingPageName;
                Session["Culture"] = loginHelper.setCulture(out landingPageName);
                if (!string.IsNullOrEmpty(landingPageName))
                {
                    Session["LandingPageName"] = landingPageName;
                    List<Dto.TabMenu> tabMenus = (List<Dto.TabMenu>)Session["TabMenus"];
                    var tabMenu = tabMenus.Where(x => x.UniqueName == landingPageName).FirstOrDefault();
                    if (tabMenu != null)
                    {
                        Response.Redirect("~" + tabMenu.PageUrl);
                    }
                }

                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("absolutePath", "/Task/List.aspx");
                //WebPage page = DomainApiClient<Dto.WebPage>.GetByAction<Dto.WebPage>
                //    (DomainApiAction.ACL.WebPage.GetByAbsolutePath, parameter);


                //List<Dto.PagePermission> pagePermissions = (List<Dto.PagePermission>)Session["PagePermissions"];
                //bool hasAccess = pagePermissions.Any(x => x.PageId == page.Id);

                //if (!hasAccess)
                //{
                    Response.Redirect("~/Home/Home.aspx");
                //}
                //else
                //{
                //    Response.Redirect("~/Task/List.aspx");
                //}

            }
        }

        private void FillUserAccess(long userId)
        {
            SessionStatusImpl sessionStatus = new SessionStatusImpl();
            Dictionary<string, string> parameter = new Dictionary<string, string>();

            parameter.Add("userId", userId.ToString());
            IList<TabMenu> menuItems = DomainApiClient<Dto.TabMenu>.GetByAction<List<TabMenu>>
                (DomainApiAction.ACL.TabMenu.GetTabMenu, parameter);

            Session["TabMenus"] = menuItems;

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


            Dictionary<string, string> perpara = new Dictionary<string, string>();
            perpara.Add("userId", new SessionStatusImpl().UserId.ToString());
            var pagePermissions = DomainApiClient<Dto.PagePermission>.GetByAction<IList<Dto.PagePermission>>
                (DomainApiAction.ACL.PagePermission.GetPagePermissionsByUser, perpara).ToList();
            Session["PagePermissions"] = pagePermissions;


            Dictionary<String, string> repParameter = new Dictionary<String, string>();
            repParameter.Add("userId", sessionStatus.UserId.ToString());
            repParameter.Add("menuUniqueName", "REPORT");

            var reportMenu = DomainApiClient<Dto.TabMenu>.GetByAction<List<Dto.TabMenu>>
                 (DomainApiAction.ACL.TabMenu.GetMenu, repParameter);
            Session["ReportMenus"] = reportMenu;
        }
    }
}