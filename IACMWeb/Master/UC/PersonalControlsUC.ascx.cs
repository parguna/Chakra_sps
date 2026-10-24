using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Master.UC
{
    public partial class PersonalControlsUC : UIBaseUC
    {
        #region Class MemberVariable
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        static string prevPage = String.Empty;
        #endregion

        #region Properties
        public string DashboardText
        {
            set
            {
                DashboardTitle.InnerText = value;
            }
        }
        public string DashboardValue
        {
            set
            {
                DashboardId.InnerText = value;
            }
        }
        //public string DboardTitle
        //{
        //    set
        //    {
        //        lnkConfig.Attributes.Add("Title", value);
        //    }
        //}

        public string PositionValue
        {
            set { Position.InnerText = value; }
        }
        public string DashBoardType
        {
            set
            {
                DashboardType.InnerText = value;
            }
        }
        #endregion
        private void LoadDashBoard()
        {
            if (!Page.IsPostBack)
            {
                if (sessionStatus.User != null && sessionStatus.User.Roles.Count > 0)
                {
                    //All Dashboard List
                    Role role = sessionStatus.User.Roles[0];
                    List<Role> roles = sessionStatus.User.Roles;
                    IList<Dto.Dashboard> getDashBoards = null;
                    List<Dto.Dashboard> dashBoards= new List<Dto.Dashboard>();
                    foreach (Role r in roles)
                    {
                    Dictionary<string, string> para = new Dictionary<string, string>();
                        para.Add("roleId", r.Id.ToString());
                        getDashBoards =
                         DomainApiClient<Dto.Dashboard>.
                         GetByAction<IList<Dto.Dashboard>>
                         (DomainApiAction.Preference.Dashboard.GetByRoleId, para);
                        foreach (Dto.Dashboard dashboard in getDashBoards)
                        {
                            if(!dashBoards.Any(item=> item.Id==dashboard.Id))
                                dashBoards.Add(dashboard);
                        }
                    }
                    if((Infrastructure.Enums.DashboardType)Enum.Parse(typeof(Infrastructure.Enums.DashboardType), DashboardType.InnerText) == Infrastructure.Enums.DashboardType.Chart)
                        dashBoards = dashBoards.Where(dboard => dboard.DashBoardType == Infrastructure.Enums.DashboardType.Chart).ToList();
                    else
                        dashBoards = dashBoards.Where(dboard => dboard.DashBoardType == Infrastructure.Enums.DashboardType.List).ToList();

                    //Dashboard Preference List

                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("userId", sessionStatus.User.Id.ToString());
                    param.Add("roleId", role.Id.ToString());
                    IList<Dto.Presentation.PresentationDashBoardPreference> dashboardPreference =
                         DomainApiClient<Dto.Presentation.PresentationDashBoardPreference>.
                         GetByAction<IList<Dto.Presentation.PresentationDashBoardPreference>>
                         (DomainApiAction.Preference.PresentationDashBoardPreference.GetUserDashboards, param);
                    
                    foreach (Dto.Dashboard menu in dashBoards)
                    {
                        var dashboardPrefIds = dashboardPreference.Where(upd1 => upd1.DashboardId == menu.Id).FirstOrDefault();
                        ListItem obj = new ListItem();
                        obj.Text = menu.DashboardName;
                        obj.Value = menu.Id.ToString();                        
                        //obj.Attributes.Add("Title", UtilityMethods.GetEnumAttribute<DashboardType>(menu.DashBoardType));
                        if (dashboardPrefIds != null)
                        {
                            obj.Value = (-1 * menu.Id).ToString();
                            obj.Attributes.Add("disabled", "disabled");
                        }
                        this.lbDashboard.Items.Add(obj);
                    }
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            // if (!IsPostBack)
            //   prevPage = Request.UrlReferrer.ToString();
            LoadDashBoard();
            DashboardList.OpenerElementID = lnkConfig.ClientID;
        }
        private bool IsDashboardLayoutApplicable(int selectedDashboardType)
        {
            bool valid = false;
            string[] dashBoardtypes = DashboardType.InnerText.Split(',');
            if (dashBoardtypes.Length > 0)
            {
                valid = dashBoardtypes.Contains(selectedDashboardType.ToString());
            }
            return valid;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (sessionStatus.User != null && sessionStatus.User.Roles.Count > 0)
            {
                var item = lbDashboard.SelectedItem;
                if (item != null && Convert.ToInt64(item.Value) >= 0)
                {
                    Dictionary<string, string> para = new Dictionary<string, string>();
                    para.Add("id", Convert.ToInt64(item.Value).ToString());
                    para.Add("roleId", sessionStatus.User.Roles[0].Id.ToString());
                    DashboardType selectedDashbaordType =
                         DomainApiClient<Dto.Dashboard>.GetByAction<DashboardType>
                         (DomainApiAction.Preference.Dashboard.GetDashboardType, para);

                    // int clickedDashboardType = Convert.ToInt32(DashboardType.InnerText);
                    //if (!IsDashboardLayoutApplicable((int)selectedDashbaordType))
                    //{
                    //    Response.Redirect("~/Preferences/UserPreference.aspx?Err=True");
                    //}
                    //else
                    {
                        string tabMenuName = DashboardTitle.InnerText;
                        long dashBoardId = Convert.ToInt64(DashboardId.InnerText);
                        int position = Convert.ToInt32(Position.InnerText);
                        Role role = sessionStatus.User.Roles[0];
                        Dictionary<string, string> param = new Dictionary<string, string>();
                        param.Add("userId", sessionStatus.User.Id.ToString());
                        param.Add("roleId", role.Id.ToString());
                        IList<Dto.Presentation.PresentationDashBoardPreference> dashBoards =
                             DomainApiClient<Dto.Presentation.PresentationDashBoardPreference>.
                             GetByAction<IList<Dto.Presentation.PresentationDashBoardPreference>>
                             (DomainApiAction.Preference.PresentationDashBoardPreference.GetUserDashboards, param);

                        if (dashBoards.Count > 0)
                        {
                            Dictionary<string, string> parameter = new Dictionary<string, string>();
                            parameter.Add("dashBoardId", dashBoardId.ToString());
                            parameter.Add("userId", sessionStatus.User.Id.ToString());
                            DomainApiClient<Dto.UserDashboardPreference>.GetByAction<Object>
                            (DomainApiAction.Preference.UserDashboardPreference.Delete, parameter);

                            UserDashboardPreference selectedDashboard = new UserDashboardPreference();
                            selectedDashboard.DashboardId = Convert.ToInt64(item.Value);
                            selectedDashboard.UserId = sessionStatus.User.Id;
                            selectedDashboard.Name = item.Text;
                            selectedDashboard.Position = position;
                            DomainApiClient<Dto.UserDashboardPreference>.Save(selectedDashboard);
                        }
                        
                        Response.Redirect("~/Preferences/UserPreference.aspx");
                    }                    
                }
                Response.Redirect("~/Preferences/UserPreference.aspx");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Response.Redirect(prevPage);
            Response.Redirect("~/Preferences/UserPreference.aspx");
        }
    }
}