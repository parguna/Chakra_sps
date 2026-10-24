using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Linq;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class EndpointConfigurationDetail : DomainModelBase<EndpointConfigurationDetail, EntityModel.EndpointConfigurationDetail>
    {
        public Nullable<long> EndpointConfigurationId { get; set; }

        public long KeyId { get; set; }

        public string Value { get; set; }
        
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public EndpointConfiguration EndpointConfiguration { get; set; }

        public EndpointConfigurationKey EndpointConfigurationKey { get; set; }

        public static List<EndpointConfigurationDetail> GetByEndpointConfigurationId(long endpointConfigurationId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EndpointConfigurationDetail>(transaction).Get(ep => ep.EndpointConfigurationId == endpointConfigurationId).ToList());
            }
        }

        public static EndpointConfigurationDetail GetByKeyId(long endpointConfigurationId,long keyId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EndpointConfigurationDetail>(transaction).Get(ep => ep.EndpointConfigurationId == endpointConfigurationId
                && ep.KeyId == keyId).FirstOrDefault());
            }
        }
        public static string GetKeyValue(long endpointId, long keyId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                EndpointConfiguration getConfig = EndpointConfiguration.GetByEndpointId(endpointId);
                EndpointConfigurationDetail configDetail = ORMap(new RepositoryBase<EntityModel.EndpointConfigurationDetail>(transaction).Get(ep => ep.EndpointConfigurationId == getConfig.Id
                && ep.KeyId == keyId).FirstOrDefault());
                if (configDetail != null)
                {
                    return configDetail.Value;
                }
                return string.Empty;
            }
        }
        public static long GetWogEndpointSystemId()
        {
            return (long)EndpointConfigurationDetail.GetAll(true).FirstOrDefault(x => x.KeyId == (long)EndpointKey.IsWOG && x.Value == "True").EndpointConfiguration.EndpointSystemId;
        }
    }

}
