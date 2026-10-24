using System;
using System.Linq;
using System.Collections.Generic;
using DI.Chakra.Transaction;
using DI.Chakra.Repository;

namespace DI.Chakra.DomainModel
{
    public class AuditConfigurationDetail : DomainModelBase<AuditConfigurationDetail, EntityModel.AuditConfigurationDetail>
    {
        public long? TableId { get; set; } 

        public string PropertyName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public AuditConfiguration AuditLog { get; set; }

        public static List<string> GetPropertiesByTableName(string tableName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.AuditConfigurationDetail>(transaction).Get(query => query.AuditLog.TableName == tableName).Select(x=>x.PropertyName).ToList();
            }
        }
        public static long GetByTableProperties(string tableName, string property)
        {
            using (var transaction = new DataAccessTransaction())
            {
                AuditConfigurationDetail data = ORMap(new RepositoryBase<EntityModel.AuditConfigurationDetail>(transaction).Get(query => query.AuditLog.TableName == tableName && query.PropertyName == property).FirstOrDefault());
                if(data != null)
                    return data.Id;
                return 0;
            }
        }
        
    }
}
