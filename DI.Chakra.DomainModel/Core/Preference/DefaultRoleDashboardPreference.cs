using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class DefaultRoleDashboardPreference : DomainModelBase<DefaultRoleDashboardPreference, EntityModel.DefaultRoleDashboardPreference>
    {       
        public long RoleId { get; set; }

        public long DashBoardId { get; set; }

        public int Position { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public  Dashboard Dashboard { get; set; }

        public  Role Role { get; set; }

        public static bool DoesExists(long roleId, long dashBoardId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.DefaultRoleDashboardPreference>(dataAccessTransaction).Any(d => (d.DashBoardId == dashBoardId) && (d.RoleId == roleId)) ;
            }
        }

        public static DefaultRoleDashboardPreference GetByIdWithNavigaton(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.DefaultRoleDashboardPreference>(dataAccessTransaction).GetById(id, d => d.Dashboard, d => d.Role));
            }
        }

        public static List<DefaultRoleDashboardPreference> GetAllOrderedWithNavigaton()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.DefaultRoleDashboardPreference>(dataAccessTransaction).GetAll(d => d.Dashboard, d => d.Role).OrderBy(d => d.RoleId).OrderBy(d => d.Position).ToList());
            }
        }

        public static bool DoesExistsByPosition(long dbPosition, long roleID)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var dbItem = new RepositoryBase<EntityModel.DefaultRoleDashboardPreference>(dataAccessTransaction).Get(c => (c.Position == dbPosition) && (c.RoleId == roleID)).FirstOrDefault();
                return dbItem != null ? true : false;

            }
        }
    }
}
