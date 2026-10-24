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
    public class SqlConfiguration : DomainModelBase<SqlConfiguration, EntityModel.SqlConfiguration>, IDatabaseConfiguration, IEndpointConfiguration 
    {
        public string ServerName { get; set; }

        public string Database { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool SqlAuthentication { get; set; }

        public Nullable<bool> MapAll { get; set; }

        public Nullable<long> EndPointSystemId { get; set; }

        public EndPoint EndpointSystem { get; set; }

        public DatabaseTypes DatabaseType { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static List<SqlConfiguration> GetByEndPointSystemId(long endPointSystemId) 
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SqlConfiguration>(dataAccessTransaction)
                       .Get(sc => sc.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static void DeleteByEndPointSystemId(DataAccessTransaction dataAccessTransaction, long endPointSystemId)
        {
            List<SqlConfiguration> sqlConfigurationList = GetByEndPointSystemId(endPointSystemId);    
            if (sqlConfigurationList != null)
                SqlConfiguration.DeleteList(dataAccessTransaction, sqlConfigurationList);
            
        }

        public static SqlConfiguration GetEndpointConfiguration(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SqlConfiguration>(dataAccessTransaction)
                       .Get(ac => ac.EndpointSystem.Id == endpointSystemId, ac => ac.EndpointSystem, ac => ac.EndpointSystem.EndPointSystemType, ac => ac.EndpointSystem.ListAttributeMappingColumns).FirstOrDefault());
            }
        }

        public static SqlConfiguration GetByIdIncludingEndpointSystem(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SqlConfiguration>(dataAccessTransaction)
                       .GetById(id, ac => ac.EndpointSystem, ac => ac.EndpointSystem.ListAttributeMappingColumns));
            }
        }

        public override long Save(string currentUser)
        {
            bool isNew = this.IsNew;

            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                base.Save(transaction);

                if (!isNew)
                {
                    SqlConfiguration sqlConfiguration = GetByIdIncludingEndpointSystem(this.Id);
                    List<AttributeMappingColumns> dbAttributeMappingColumnsList = sqlConfiguration.EndpointSystem.ListAttributeMappingColumns.ToList();

                    AttributeMappingColumns.DeleteList(transaction, dbAttributeMappingColumnsList);

                    if (this.EndpointSystem.ListAttributeMappingColumns != null)
                    {
                        foreach (var listAttributeMappings in this.EndpointSystem.ListAttributeMappingColumns)
                        {
                            listAttributeMappings.EndPoint = null;
                        }
                        AttributeMappingColumns.SaveList(transaction, this.EndpointSystem.ListAttributeMappingColumns.ToList());
                    }
                    this.EndPointSystemId = this.EndpointSystem.Id;

                    this.Save(transaction);
                }

                transaction.Commit();
            }
            return 0;
        }
    }
}
