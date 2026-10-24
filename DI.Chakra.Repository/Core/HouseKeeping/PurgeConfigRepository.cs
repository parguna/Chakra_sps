using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class PurgeConfigRepository : RepositoryBase<PurgeConfig>
    {
        public PurgeConfigRepository(DataAccessTransaction transaction) : base(transaction) { }


        private const string PROVISIOINGTABLE = "DI_Log_Provisioning";
        public void PurgeTable(string tableName, int noOfDaysToKeep)
        {
            if (tableName != PROVISIOINGTABLE)
            {
                string cmd = "DELETE FROM " + tableName + " WHERE CREATEDDATE <  DATEADD(day, -" + noOfDaysToKeep + ", GETDATE())";
                this.CurrentContext.Database.CommandTimeout = 0;
                this.CurrentContext.Database.ExecuteSqlCommand(cmd);
                CurrentContext.SaveChanges();
            }
            else
            {
                string cmd = "DELETE FROM " + tableName + " WHERE LOGDATE <  DATEADD(day, -" + noOfDaysToKeep + ", GETDATE())";
                this.CurrentContext.Database.CommandTimeout = 0;
                this.CurrentContext.Database.ExecuteSqlCommand(cmd);
                CurrentContext.SaveChanges();
            }
        }
    }
}
