using System;
using System.Collections.Generic;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class DashboardRepository : RepositoryBase<Dashboard>
    {
        public DashboardRepository(DataAccessTransaction transaction)
            : base(transaction)
        {

        }

        public List<Dashboard> GetByRoleIdAndCategory(long RoleId, string category)
        {
            List<Dashboard> dashboardList = new List<Dashboard>();
            var query = from DR in this.CurrentContext.DashboardAccessRights
                        join r in this.CurrentContext.Roles
                    on DR.RoleId equals r.Id
                        join d in this.CurrentContext.Dashboards
                    on DR.DashboardId equals d.Id
                        where r.Id == RoleId && d.DashboardCategory == category && d.TargetUrl != null
                        select d;

            dashboardList = query.ToList();

            return dashboardList;
        }
        public List<Dashboard> GetByRoleId(long RoleId)
        {
            List<Dashboard> dashboardList = new List<Dashboard>();
                var query = from DR in this.CurrentContext.DashboardAccessRights
                            join r in this.CurrentContext.Roles
                        on DR.RoleId equals r.Id
                            join d in this.CurrentContext.Dashboards
                        on DR.DashboardId equals d.Id
                            where r.Id == RoleId && d.TargetUrl != null
                            select d;

                dashboardList = query.ToList();
          
                return dashboardList;
        }

        public List<string> GetDashboardCategoryByRoleId(long roleId)
        {
            var db = from item in this.CurrentContext.Dashboards
                     join ar in this.CurrentContext.DashboardAccessRights on item.Id equals ar.DashboardId
                     where ar.RoleId == roleId
                     select item.Id;
            return (from d in this.CurrentContext.Dashboards where db.Contains(d.Id) select d.DashboardCategory).Distinct().ToList();
        }
    }
}



