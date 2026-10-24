using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPEPConfiguration : DomainModelBase<SAPEPConfiguration, EntityModel.SAPEPConfiguration>, IEndpointConfiguration
    {
        public long EndPointSystemId { get; set; }

        public string HostName { get; set; }

        public string IPAddress { get; set; }

        public string DomainName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string WSDLUrl { get; set; }

        public Nullable<bool> MapAll { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public EndPoint EndpointSystem { get; set; }

        public static List<SAPEPConfiguration> GetByEndpointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPConfiguration>(dataAccessTransaction).Get(ac => ac.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static void DeleteByEndPointSystemId(DataAccessTransaction dataAccessTransaction, long endPointSystemId)
        {
            List<SAPEPConfiguration> sapepConfigurationList = GetByEndpointSystemId(endPointSystemId);
            if (sapepConfigurationList != null)
            {
                SAPEPConfiguration.DeleteList(dataAccessTransaction, sapepConfigurationList);
            }
        }

        public static SAPEPConfiguration GetEndpointConfiguration(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPConfiguration>(dataAccessTransaction)
                       .Get(ac => ac.EndpointSystem.Id == endpointSystemId, ac => ac.EndpointSystem, ac => ac.EndpointSystem.EndPointSystemType, ac => ac.EndpointSystem.ListAttributeMappingColumns).FirstOrDefault());
            }
        }

        public static SAPEPConfiguration GetByIdIncludingEndpointSystem(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPConfiguration>(dataAccessTransaction)
                       .Get(ac => ac.Id == id, ac => ac.EndpointSystem, ac => ac.EndpointSystem.ListAttributeMappingColumns).FirstOrDefault());
            }
        }

        public override void Save()
        {
            bool isNew = this.IsNew;

            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                base.Save(transaction);

                if (!isNew)
                {
                    SAPEPConfiguration sapepConfiguration = GetByIdIncludingEndpointSystem(this.Id);
                    List<AttributeMappingColumns> dbAttributeMappingColumnsList = sapepConfiguration.EndpointSystem.ListAttributeMappingColumns.ToList();

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

                    this.Save();
                }

                transaction.Commit();
            }
        }
    }
}
