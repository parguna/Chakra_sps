using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;


namespace DI.Chakra.Repository
{
    public class PresentationDashBoardPreferenceRepository : RepositoryBase<PresentationDashBoardPreference>
    {
        public PresentationDashBoardPreferenceRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }
        public List<PresentationDashBoardPreference> GetUserDashboards(long userId, long roleId)
        {
            List<PresentationDashBoardPreference> dashboardList = new List<PresentationDashBoardPreference>();
            if (userId != -1)
            {
                var query = from DR in this.CurrentContext.UserDashboardPreferences.Include("Dashboard")
                            join D in this.CurrentContext.Dashboards
                            on DR.DashboardId equals D.Id
                            where DR.UserId == userId
                            select new PresentationDashBoardPreference
                            {
                                DashboardId = DR.DashboardId,
                                DashBoardType = D.DashBoardType,
                                //Title = DR.Dashboard.Title,
                                DashboardName = DR.Name,
                                DashboardCategory = D.DashboardCategory,
                                TargetUrl = D.TargetUrl,
                                NavigateUrl = D.NavigateUrl,
                                Position = DR.Position
                            };
                dashboardList = query.OrderBy(p => p.Position).ToList();
            }

            if (dashboardList == null || dashboardList.Count < 3)
            {
                var defaultValue = GetDefaultDashboards(roleId);
                if (dashboardList != null)
                {
                    List<int> usrPosition = dashboardList.Select(a => a.Position).ToList();
                    var otherThenUserDefault = defaultValue.Where(a => !usrPosition.Contains(a.Position)).ToList();
                    dashboardList.AddRange(otherThenUserDefault);
                }
                else
                {
                    dashboardList = defaultValue;
                }
            }
            return dashboardList;
        }

        public List<PresentationDashBoardPreference> GetDefaultDashboards(long roleId)
        {
            List<PresentationDashBoardPreference> dashboardList = new List<PresentationDashBoardPreference>();
            if (roleId != -1)
            {
                var query = from drp in this.CurrentContext.DefaultRoleDashboardPreferences
                            join r in this.CurrentContext.Roles
                            on drp.RoleId equals r.Id
                            join D in this.CurrentContext.Dashboards
                            on drp.DashBoardId equals D.Id
                            where drp.RoleId == roleId
                            orderby drp.Position
                            select new PresentationDashBoardPreference
                            {
                                DashboardName = D.DashboardName,
                                DashBoardType = D.DashBoardType,
                                //Title = D.Title,
                                DashboardId = D.Id,
                                TargetUrl = D.TargetUrl,
                                NavigateUrl = D.NavigateUrl,
                                DashboardCategory = D.DashboardCategory,
                                Position = drp.Position
                            };
                dashboardList = query.ToList();
            }
            return dashboardList;
        }
    }
}
