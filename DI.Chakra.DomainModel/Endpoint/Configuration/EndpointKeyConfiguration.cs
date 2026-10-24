using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class EndpointKeyConfiguration : DomainModelBase<EndpointKeyConfiguration, EntityModel.EndpointKeyConfiguration>
    {
        public long EndpointSystemTypeId { get; set; }

        public long KeyId { get; set; }

        public int Order { get; set; }

        public string DataType { get; set; }

        public string RegExValidationPattern { get; set; }

        public string ClientScript { get; set; }

        public bool IsRequired { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public EndpointConfigurationKey EndpointConfigurationKey { get; set; }

        public static List<EndpointKeyConfiguration> GetByEndpointSystemTypeId(long endpointSystemTypeId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EndpointKeyConfiguration>(transaction)
                    .Get(query => query.EndpointSystemTypeId == endpointSystemTypeId, eagerload => eagerload.EndpointConfigurationKey).ToList());
            }
        }
    }

}
