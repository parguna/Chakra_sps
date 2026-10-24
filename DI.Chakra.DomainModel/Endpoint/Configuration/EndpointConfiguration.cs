using System;
using System.Linq;
using System.Collections.Generic;
using DI.Chakra.Transaction;
using DI.Chakra.Repository;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class EndpointConfiguration : DomainModelBase<EndpointConfiguration, EntityModel.EndpointConfiguration>
    {
        public Nullable<long> EndpointSystemId { get; set; }

        public bool MapAll { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public EndPoint EndpointSystem { get; set; }

        public List<EndpointConfigurationDetail> ConfigurationDetails { get; set; }

        public static EndpointConfiguration GetByEndpointSystemId(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndpointConfiguration>(transaction)
                    .Get(query => query.EndpointSystemId == endpointSystemId, eagerload => eagerload.EndpointSystem, eagerload => eagerload.ConfigurationDetails, eagerload => eagerload.ConfigurationDetails.Select(t => t.EndpointConfigurationKey), eagerload => eagerload.EndpointSystem.ListAttributeMappingColumns, eagerload => eagerload.EndpointSystem.EndPointSystemType).FirstOrDefault());
            }
        }

        public static EndpointConfiguration GetByEndpointSystemTypeId(long endpointSystemTypeId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndpointConfiguration>(transaction)
                    .Get(query => query.EndpointSystem.EndpointSystemTypeId == endpointSystemTypeId, eagerload => eagerload.EndpointSystem, eagerload => eagerload.ConfigurationDetails, eagerload => eagerload.ConfigurationDetails.Select(t => t.EndpointConfigurationKey), eagerload => eagerload.EndpointSystem.ListAttributeMappingColumns, eagerload => eagerload.EndpointSystem.EndPointSystemType).FirstOrDefault());
            }
        }

        public static Dictionary<long,bool> GetDictionaryByEndpointSystemTypeId(List<long> endpointIds)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.EndpointConfiguration>(transaction)
                    .Get(query => endpointIds.Contains(query.EndpointSystemId.Value)).ToDictionary(x => x.EndpointSystemId.Value, z => z.MapAll);
            }
        }

        public static EndpointConfiguration GetByEndpointId(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EndpointConfiguration>(transaction)
                    .Get(query => query.EndpointSystemId == endpointSystemId).FirstOrDefault());
            }
        }
        public override long Save(string currentUser)
        {
            using (DataAccessTransaction dataAccessTrasaction = new DataAccessTransaction(currentUser))
            {
                base.Save(dataAccessTrasaction);
                dataAccessTrasaction.Commit();
            }
            return 0;
        }

        public static EndpointConfiguration GetBySystemCode(string systemCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new EndpointConfigurationRepository(dataAccessTransaction).GetBySystemCode(systemCode));
            }
        }
        public static bool IsSSOEnableForEndpoint(long endpointSystemId)
        {
            bool isEnabled = false;
            using (var transaction = new DataAccessTransaction())
            {

                var endpointConfig = new RepositoryBase<EntityModel.EndpointConfiguration>(transaction)
                    .Get(query => query.EndpointSystemId == endpointSystemId, eagerload => eagerload.EndpointSystem, eagerload => eagerload.ConfigurationDetails, eagerload => eagerload.ConfigurationDetails.Select(t => t.EndpointConfigurationKey), eagerload => eagerload.EndpointSystem.ListAttributeMappingColumns, eagerload => eagerload.EndpointSystem.EndPointSystemType).FirstOrDefault();

                if(endpointConfig != null && endpointConfig.ConfigurationDetails.Count > 0)
                {
                    var config = endpointConfig.ConfigurationDetails.FirstOrDefault(i => i.KeyId == (long)Infrastructure.Enums.EndpointKey.EnableSSO);
                    if (config != null && !string.IsNullOrWhiteSpace(config.Value) && Convert.ToBoolean(config.Value))
                    {
                        isEnabled = true;
                    }
                }
                return isEnabled;
            }
        }
    }
}
