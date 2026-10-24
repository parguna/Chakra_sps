using System;
using System.Collections.Generic;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class DashboardAcessRightsRepository : RepositoryBase<DashboardAccessRights>
    {
        public DashboardAcessRightsRepository(DataAccessTransaction transaction)
            : base(transaction)
        {

        }

        public bool DeleteByRoleId(long roleId, DataAccessTransaction dataAccessTransaction)
        {
            try
            {
                CurrentContext.Database.ExecuteSqlCommand("DELETE FROM DI_M_DefaultRoleDashboard where RoleId = @RoleId", new System.Data.SqlClient.SqlParameter("@RoleId", roleId));
                CurrentContext.SaveChanges();

                CurrentContext.Database.ExecuteSqlCommand("DELETE FROM DI_M_DashboardAccessRights where RoleId = @RoleId", new System.Data.SqlClient.SqlParameter("@RoleId", roleId));
                CurrentContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
