using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class NOVASoDExceptionsMatrixRepository : RepositoryBase<NOVASoDExceptionsMatrix>
    {
        public NOVASoDExceptionsMatrixRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }
        public List<NOVASoDExceptionsMatrix> GetByUserId(long endpointId,long userId , string sapSection)
        {

            var query = "SELECT SOD.* FROM DI_NOVA_SoDExceptionsMatrix SOD WHERE SOD.AffectedUsers = LTRIM(RTRIM(@SapSection))";
            // OR M.HRMSSectionCode IN (SELECT organisationcode FROM DI_M_User WHERE id = {userId})";
            var queryParameters = new[] { new System.Data.SqlClient.SqlParameter("@SapSection", (object)sapSection ?? DBNull.Value) };
            return CurrentContext.Database.SqlQuery<NOVASoDExceptionsMatrix>(query, queryParameters).ToList();
        }
       
    }
}
