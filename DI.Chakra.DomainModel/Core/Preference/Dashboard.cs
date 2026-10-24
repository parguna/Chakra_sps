using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Transaction;
using DI.Chakra.Repository;

namespace DI.Chakra.DomainModel
{
    public class Dashboard : DomainModelBase<Dashboard, EntityModel.Dashboard>
    {
        public string DashboardUniqueName { get; set; }

        public string DashboardName { get; set; }

        public string DashboardCategory { get; set; }

        public DashboardType DashBoardType { get; set; }

        public string TargetUrl { get; set; }

        public string NavigateUrl { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static List<Dashboard> GetByDashBoardCategory(string dashboardCategory)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.Dashboard>(dataAccessTransaction).Get(db => db.DashboardCategory == dashboardCategory).ToList());
            }
        }

        public static List<Dashboard> GetByRoleId(long roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new DashboardRepository(dataAccessTransaction).GetByRoleId(roleId));
            }
        }

        public static List<string> GetDashboardCategoryByRoleId(long roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new DashboardRepository(dataAccessTransaction).GetDashboardCategoryByRoleId(roleId);
            }
            // return null;
        }
        public static DashboardType GetDashboardType(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                DashboardType type = 0;
                Dashboard dashboard = Dashboard.GetById(id);
                if (dashboard != null)
                    type = dashboard.DashBoardType;
                return type;
            }
        }
        public static bool DoesExists(string name, long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (id == 0)
                    return new RepositoryBase<EntityModel.Dashboard>(dataAccessTransaction).Any(db => db.DashboardName == name);
                else
                    return new RepositoryBase<EntityModel.Dashboard>(dataAccessTransaction).Any(db => db.DashboardName == name && db.Id != id);

            }
        }

        public static List<Dashboard> GetByRoleIdAndCategory(long roleId, string category)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new DashboardRepository(dataAccessTransaction).GetByRoleIdAndCategory(roleId, category));
            }
        }
    }
}
