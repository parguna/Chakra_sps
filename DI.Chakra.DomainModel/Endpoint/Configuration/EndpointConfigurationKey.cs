using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class EndpointConfigurationKey : DomainModelBase<EndpointConfigurationKey, EntityModel.EndpointConfigurationKey>
    {
        public string KeyName { get; set; }

        public string Caption { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


        public static EndpointConfigurationKey GetByKeyName(string keyName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EndpointConfigurationKey>(transaction)
                    .Get(query => query.KeyName == keyName).FirstOrDefault());
            }
        }
    }

}
