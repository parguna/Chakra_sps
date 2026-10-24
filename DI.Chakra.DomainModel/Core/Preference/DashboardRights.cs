using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class DashboardAccessRights : DomainModelBase<DashboardAccessRights, EntityModel.DashboardAccessRights>
    {

        public long RoleId { get; set; }

        public long DashboardId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Dashboard Dashboard { get; set; }

        public Role Role { get; set; }

        public static bool DoesExists(long roleId, long dashboardId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.DashboardAccessRights>(dataAccessTransaction).Any(d => (d.DashboardId == dashboardId) && (d.RoleId == roleId));
            }
        }

        public new static void Delete(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DashboardAccessRights dashboardAccessRights = DashboardAccessRights.GetById(id);

                DefaultRoleDashboardPreference defaultRoleDashboardPreference = Mapper.Map<EntityModel.DefaultRoleDashboardPreference, DefaultRoleDashboardPreference>(new RepositoryBase<EntityModel.DefaultRoleDashboardPreference>(dataAccessTransaction).Get(m => m.RoleId == dashboardAccessRights.RoleId && m.DashBoardId == dashboardAccessRights.DashboardId).FirstOrDefault());

                if (defaultRoleDashboardPreference != null)
                    defaultRoleDashboardPreference.Delete(dataAccessTransaction);

                dashboardAccessRights.Delete(dataAccessTransaction);

                dataAccessTransaction.Commit();
            }
        }

        public static List<DashboardAccessRights> GetAllWithNavigation()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.DashboardAccessRights>(dataAccessTransaction).
                    Get(null, true).OrderBy(d => d.RoleId).ToList());
            }
        }

        public static bool DeleteByRoleId(long roleId, DataAccessTransaction dataAccessTransaction)
        {
            return new DashboardAcessRightsRepository(dataAccessTransaction).DeleteByRoleId(roleId, dataAccessTransaction);
        }

    }
}
