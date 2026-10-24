using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class EndpointUserViewConfiguration : DomainModelBase<EndpointUserViewConfiguration, EntityModel.EndpointUserViewConfiguration>
    {
        public long EndpointSystemTypeId { get; set; }
        public Nullable<long> EndpointSystemId { get; set; }
        public string KeyName { get; set; }
        public string Caption { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
        public EndPoint EndpointSystem { get; set; }
        public EndPointSystemType EndpointSystemType { get; set; }

        public static List<EndpointUserViewConfiguration> GetByEndpointSystemId(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {

                var list = new RepositoryBase<EntityModel.EndpointUserViewConfiguration>(transaction)
                   .Get(query => query.EndpointSystemId == endpointSystemId, true).OrderBy(q => q.Order).ToList();

                

                if (list.Count <= 0)
                {
                    var epTypeId = new RepositoryBase<EntityModel.EndPoint>(transaction)
                     .Get(query => query.Id == endpointSystemId, true).FirstOrDefault();

                    list = new RepositoryBase<EntityModel.EndpointUserViewConfiguration>(transaction)
                   .Get(query => query.EndpointSystemTypeId == epTypeId.EndpointSystemTypeId, true).OrderBy(q => q.Order).ToList();

                }

                return ORMapList(list);
            }
        }
    }
}
