using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class AccessRightsRepository : RepositoryBase<AccessRights>
    {
        public AccessRightsRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<AccessRights> GetByCloneRoleId(long cloneRoleId)
        {
            return (from ar in CurrentContext.AccessRights
                    where ar.RoleId == cloneRoleId
                    select ar).ToList();
        }

        public bool DeleteById(long rightsId, DataAccessTransaction dataAccessTransaction)
        {
            CurrentContext.Database.ExecuteSqlCommand("UPDATE DI_M_AccessRights SET PagePermissionId= NULL WHERE Id = @Id", new System.Data.SqlClient.SqlParameter("@Id", rightsId));
            CurrentContext.SaveChanges();
            CurrentContext.Database.ExecuteSqlCommand("DELETE FROM DI_M_AccessRights where Id=@Id", new System.Data.SqlClient.SqlParameter("@Id", rightsId));
            CurrentContext.SaveChanges();

            return true;
        }

        public bool DeleteByRoleId(long roleId, DataAccessTransaction dataAccessTransaction)
        {
            CurrentContext.Database.ExecuteSqlCommand("UPDATE DI_M_AccessRights SET PagePermissionId= NULL WHERE RoleId = @RoleId", new System.Data.SqlClient.SqlParameter("@RoleId", roleId));
            CurrentContext.SaveChanges();
            CurrentContext.Database.ExecuteSqlCommand("DELETE FROM DI_M_AccessRights where RoleId = @RoleId", new System.Data.SqlClient.SqlParameter("@RoleId", roleId));
            CurrentContext.SaveChanges();

            return true;
        }
    }
}
