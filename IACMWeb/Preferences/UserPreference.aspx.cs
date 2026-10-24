using Application;
using DI.Chakra.DBConfigHelper;

using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

using DI.Chakra.Web.Master.UC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Logger;
using System.Linq;

namespace DI.Chakra.Web.Preferences
{
    public partial class UserPreference : UIBasePage, IAccessControl
    {
        #region Class MemberVariable
        private const string V_EntityID = "V_EntityID";
        private const string V_ID = "V_ID";
      
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        List<Dto.ConfigurationMaster> listConfig = new List<Dto.ConfigurationMaster>();
        private List<Dto.UserCulturePreference> usrCulPrefenece = new List<Dto.UserCulturePreference>();
        private Dto.UserCulturePreference usrpreference = new Dto.UserCulturePreference();
        #endregion


        #region "Properties"

        public long EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return 0;
                return (long)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
            }
        }

        public long id
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return 0;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        #endregion

        #region "Events"

        protected void Page_Load(object sender, EventArgs e)
        {

            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {

                // Culture Preference
                this.EntityId = sessionStatus.UserId;
                listConfig = DomainApiClient<Dto.ConfigurationMaster>.GetAll();
                usrCulPrefenece = GetUserPreference();

                if (usrCulPrefenece.Count != 0)
                {
                    if (usrCulPrefenece[0].Id != 0)
                    {
                        BindCultures();
                        this.id = usrCulPrefenece[0].Id;
                        Cultures.SelectedValue = usrCulPrefenece[0].CultureName;
                        
                        BindCultureDateTimeFormat(Cultures.SelectedValue);
                        ShortDatePatterns.ClearSelection();
                        ShortTimePatterns.ClearSelection();
                        LongDatePatterns.ClearSelection();
                        LongTimePatterns.ClearSelection();

                        ShortDatePatterns.SelectedValue = usrCulPrefenece[0].DateFormat;
                        ShortTimePatterns.SelectedValue = usrCulPrefenece[0].TimeFormat;
                        LongDatePatterns.SelectedValue = usrCulPrefenece[0].LongDateFormat;
                        LongTimePatterns.SelectedValue = usrCulPrefenece[0].LongTimeFormat;


                    }
                    else
                    {
                        setCultures();
                        this.id = 0;
                    }
                }
                else
                {
                    setCultures();
                    this.id = 0;
                }

                // DashBoard 
                if (sessionStatus.User != null && sessionStatus.User.Roles.Count > 0)
                {
                    BindLandingPages();
                    if (usrCulPrefenece.Count != 0 && usrCulPrefenece[0].Id != 0 && !string.IsNullOrEmpty(usrCulPrefenece[0].LandingPageMenuName))
                    {
                        ddlPageList.Items.FindByValue(usrCulPrefenece[0].LandingPageMenuName).Selected = true;
                    }
                    Dto.Role role = sessionStatus.User.Roles[0];
                    Dictionary<String, string> parameter = new Dictionary<String, string>();
                    parameter.Add("userId", sessionStatus.User.Id.ToString());
                    parameter.Add("roleId", role.Id.ToString());

                    IList<Dto.Presentation.PresentationDashBoardPreference> dashboards = DomainApiClient<Dto.Presentation.PresentationDashBoardPreference>.GetByAction<List<Dto.Presentation.PresentationDashBoardPreference >>(DomainApiAction.Preference.PresentationDashBoardPreference.GetUserDashboards, parameter);
                    
                    int i = 0;
                    ArrayList ucControls = new ArrayList();
                    ucControls.Add(dashboardLayout1);
                    ucControls.Add(dashboardLayout3);
                    foreach (PresentationDashBoardPreference preference in dashboards)
                    {
                        if (preference.DashBoardType == DashboardType.Chart)
                        {
                            PersonalControlsUC chartObject = (PersonalControlsUC)ucControls[i];
                            if (chartObject != null)
                            {
                                chartObject.DashboardText = preference.DashboardName;
                                chartObject.DashboardValue = preference.DashboardId.ToString();
                                chartObject.PositionValue = preference.Position.ToString();
                                i++;
                            }
                        }
                        else
                        {
                                dashboardLayout2.DashboardText = preference.DashboardName;
                                dashboardLayout2.DashboardValue = preference.DashboardId.ToString();
                                dashboardLayout2.PositionValue = preference.Position.ToString();
                        }
                    }
                }

                if (Request.QueryString["Err"] != null)
                    MasterPage.ShowAlert(AlertType.Warning, "Please select appropriate dashboard type.");
            }
        }

        protected void btnExecute_Click(object sender, EventArgs e)
        {
            
            string Error = string.Empty;
            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("file", txtWeb.Text);
            parameter.Add("Error", Error);
            DomainApiClient<Object>.GetByAction<bool>("PSScriptRunner", DomainApiAction.PSScriptRunner.Run, parameter,null);
            ScriptError.Text = Error;
        }

        protected void ddlCultures_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShortDatePatterns.Items.Clear();
            ShortTimePatterns.Items.Clear();
            LongDatePatterns.Items.Clear();
            LongTimePatterns.Items.Clear();
            BindCultureDateTimeFormat(Cultures.SelectedValue);
        }

        #endregion

        #region "Methods"

        private void BindCultures()
        {
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                Cultures.Items.Add(new ListItem(ci.EnglishName, ci.Name));
            }

        }

        private void BindLandingPages()
        {
            List <Dto.TabMenu> tabMenus = (List<Dto.TabMenu>)Session["TabMenus"];
            tabMenus = tabMenus.Where(x => x.PageUrl != "#").ToList();
            if (tabMenus!= null)
            {
                foreach (var item in tabMenus)
                {
                    ddlPageList.Items.Add(new ListItem(item.MenuName, item.UniqueName));
                }
            }
        }
        /// <summary>
        /// Sets Culture
        /// </summary>
        private void setCultures()
        {
            BindCultures();

            foreach (Dto.ConfigurationMaster item in listConfig)
            {
                if (item.ConfigKey == ConfigConstants.APPLICATIONCULTURE)
                {
                    Cultures.Items.FindByValue(item.ConfigValue).Selected = true;
                }
            }

            BindCultureDateTimeFormat(Cultures.SelectedValue);

        }





        /// <summary>
        /// Sets default date time format of the culture
        /// </summary>
        /// <param name="cultureName">Name of the Culture</param>
        private void BindCultureDateTimeFormat(string cultureName)
        {
            CultureInfo culInfo = CultureInfo.GetCultureInfo(cultureName);

            foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (!ShortDatePatterns.Items.Contains(new ListItem(cultureInfo.DateTimeFormat.ShortDatePattern)))

                    ShortDatePatterns.Items.Add(new ListItem(cultureInfo.DateTimeFormat.ShortDatePattern));
                else if(!ShortTimePatterns.Items.Contains(new ListItem(cultureInfo.DateTimeFormat.ShortTimePattern)))

                ShortTimePatterns.Items.Add(new ListItem(cultureInfo.DateTimeFormat.ShortTimePattern));
                else if (!LongDatePatterns.Items.Contains(new ListItem(cultureInfo.DateTimeFormat.LongDatePattern)))

                    LongDatePatterns.Items.Add(new ListItem(cultureInfo.DateTimeFormat.LongDatePattern));
                else if (!LongTimePatterns.Items.Contains(new ListItem(cultureInfo.DateTimeFormat.LongTimePattern)))
                    LongTimePatterns.Items.Add(new ListItem(cultureInfo.DateTimeFormat.LongTimePattern));

            }
            ShortDatePatterns.Items.FindByValue(culInfo.DateTimeFormat.ShortDatePattern).Selected = true;
            ShortTimePatterns.Items.FindByValue(culInfo.DateTimeFormat.ShortTimePattern).Selected = true;
            LongDatePatterns.Items.FindByValue(culInfo.DateTimeFormat.LongDatePattern).Selected = true;
            LongTimePatterns.Items.FindByValue(culInfo.DateTimeFormat.LongTimePattern).Selected = true;
        }

        public string[] GetDatePattern(string pattern)
        {

            string[] DateTimePattern = null;
            if (!string.IsNullOrEmpty(pattern))
            {
                var start = pattern.IndexOf("h", StringComparison.InvariantCultureIgnoreCase);

                DateTimePattern = new string[2];

                if (start < 0)
                    DateTimePattern[0] = pattern;
                else
                {
                    DateTimePattern[0] = pattern.Substring(0, start - 1);
                    DateTimePattern[1] = pattern.Substring(start);
                }
            }
            return DateTimePattern;
        }

        /// <summary>
        /// Gets an instance of ConfigurationMaster ("Application Culture")
        /// </summary>
        /// <returns> Retrives read only instance of ConfigurationMaster("Application Culture")</returns>
        private Dto.ConfigurationMaster getCultures()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigConstants.APPLICATIONCULTURE);
            parameter.Add("product", Product.IACM.ToString());

            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
(DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);

           if (config == null)
                return null;
            config.ConfigKey = ConfigConstants.APPLICATIONCULTURE;
            config.ConfigValue = Cultures.SelectedValue.ToString();
            return (Dto.ConfigurationMaster)config;
        }

        /// <summary>
        /// Gets list of user's  Preferenced culture
        /// </summary>
        /// <returns>retrives a list UserCulturePreference object</returns>
        private List<Dto.UserCulturePreference> GetUserPreference()
        {

            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("userId", EntityId.ToString());

            return DomainApiClient<Dto.UserCulturePreference>.GetByAction<List<Dto.UserCulturePreference>>(DomainApiAction.Preference.UserCulturePreference.GetByUserId, parameter);

           

        }

        private void Save()
        {
            try
            {
                usrpreference.Id = this.id;
                usrpreference.UserId = Convert.ToInt32(EntityId);
                usrpreference.CultureName = Cultures.SelectedValue.ToString();
                //     var pattern = GetDatePattern(ShortDatePatterns.SelectedValue.ToString());
                usrpreference.DateFormat = "dd-MM-yyyy"; //ShortDatePatterns.SelectedValue.ToString(); //HOpattern[0]; 
                usrpreference.TimeFormat = ShortTimePatterns.SelectedValue.ToString();// pattern[1];
                usrpreference.LongDateFormat = LongDatePatterns.SelectedValue.ToString();
                usrpreference.LongTimeFormat = LongTimePatterns.SelectedValue.ToString();
                usrpreference.LandingPageMenuName = ddlPageList.SelectedValue.ToString();
                DomainApiClient<Dto.UserCulturePreference>.Save(usrpreference);
                MasterPage.ShowAlert(AlertType.Success, "Culture Updated Successfully.");
                LoginHelper loginHelper = new LoginHelper();
                string landingPageName;
                Session["Culture"] = loginHelper.setCulture(out landingPageName);
            }
            catch(Exception ex)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Exception, "User Preference", ex);
                MasterPage.ShowAlert(AlertType.Error, "Culture Updation Failed.");
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Uuser Preference - Save");
            }
        }

        #endregion

    }
}