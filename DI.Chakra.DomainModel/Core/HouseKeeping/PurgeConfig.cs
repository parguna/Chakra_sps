using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class PurgeConfig : DomainModelBase<PurgeConfig, EntityModel.PurgeConfig>
    {
        public string TableName { get; set; }

        public string ObjectName { get; set; }

        public string Description { get; set; }

        public int DaysToKeep { get; set; }

        public string Status { get; set; }

        public PurgeMode PurgeMode { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Nullable<System.DateTime> LastRunOn { get; set; }

        //public static bool DoesExists(string tableName)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return new RepositoryBase<EntityModel.PurgeConfig>(dataAccessTransaction).Any(pc => pc.TableName == tableName);
        //    }
        //}

        public static bool DoesExists(string tableName, long purgeID)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                bool flag = true;
                if (purgeID > 0)
                {
                    var ExistTableName = PurgeConfig.GetById(purgeID).TableName;
                    flag = (ExistTableName != tableName) ? true : false;
                }
                if (flag)
                {
                    var dbPurge = ORMap(new RepositoryBase<EntityModel.PurgeConfig>(transaction).
                   Get(c => c.TableName == tableName).FirstOrDefault());

                    return dbPurge != null ? true : false;
                }
                return false;
            }
        }


        public static void PurgeTable(string tableName, int noOfDaysToKeep)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new PurgeConfigRepository(dataAccessTransaction).PurgeTable(tableName, noOfDaysToKeep);
            }
        }
    }
}
