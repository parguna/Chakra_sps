using System.Collections.Generic;
using EntityFramework.BulkInsert.Extensions;
using DI.Chakra.Transaction;

namespace DI.Chakra.Repository
{
    public class EFBulkInserter<TEntity>
    {
        public void BulkInsert( List<TEntity> entity )
        {

            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                transaction.Context.BulkInsert(entity);
                transaction.Context.SaveChanges();
            }

        }
    }
}
