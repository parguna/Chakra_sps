using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class EndPointAutoExpiry : DomainModelBase<EndPointAutoExpiry, EntityModel.EndPointAutoExpiry>
    {
        public long EndpointSystemTypeId { get; set; }

        public Nullable<long> EndPointSystemId { get; set; }

        public bool IsAccountAutoExpirySupported { get; set; }

        public bool IsEntitlementAutoExpirySupported { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public bool IsAutoExpiryApprovalRequired { get; set; }




        public static EndPointAutoExpiry GetByEndpointSystemTypeId(long endpointSystemTypeId) 
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EndPointAutoExpiry>(dataAccessTransaction).
                       Get(epAutuExpiry => epAutuExpiry.EndpointSystemTypeId == endpointSystemTypeId).FirstOrDefault());
            }
        }

        public static EndPointAutoExpiry GetByEndPointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EndPointAutoExpiry>(dataAccessTransaction).
                       Get(epAutuExpiry => epAutuExpiry.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }
    }
}
