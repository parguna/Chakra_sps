using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class QuickLinkRepository : RepositoryBase<QuickLink>
    {
        public QuickLinkRepository(DataAccessTransaction transaction)
            : base(transaction)
        {

        }
        public List<QuickLink> GetByRoleId(long RoleId)
        {
            List<QuickLink> menult = new List<QuickLink>();
             var query = from QR in CurrentContext.QuickLinkAccessRights
                         join r in CurrentContext.Roles
                                    on QR.RoleId equals r.Id
                         join Q in CurrentContext.QuickLinks
                                    on QR.QuickLinkId equals Q.Id
                                    where r.Id == RoleId
                                    select Q;
                        menult = query.ToList();

                        return menult;
        }





        public IList<string> GetQLCatagoriesByRoleID(long roleId)
        {
            try
            {
                var db = from item in CurrentContext.QuickLinks
                         join ar in CurrentContext.QuickLinkAccessRights on item.Id equals ar.QuickLinkId
                         where ar.RoleId == roleId
                         select item.Id;

                return (from d in CurrentContext.QuickLinks where db.Contains(d.Id) select d.Category).Distinct().OrderBy(a => a).ToList();


            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public bool DeleteByRoleId(long roleId, DataAccessTransaction dataAccessTransaction)
        {
            try
            {
                CurrentContext.Database.ExecuteSqlCommand("DELETE FROM DI_M_DefaultRoleQuickLink where RoleId = @RoleId", new System.Data.SqlClient.SqlParameter("@RoleId", roleId));
                CurrentContext.SaveChanges();

                CurrentContext.Database.ExecuteSqlCommand("DELETE FROM DI_M_QuickLinkAccessRights where RoleId = @RoleId", new System.Data.SqlClient.SqlParameter("@RoleId", roleId));
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
