using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPPosition : DomainModelBase<SAPPosition, EntityModel.SAPPosition>
    {
        

        public long EndPointSystemId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public DateTime? BEGDA { get; set; }

        public DateTime? ENDDA { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public static SAPPosition GetByCode(string code, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPPosition>(dataAccessTransaction).Get(sapPosition => sapPosition.Code == code && sapPosition.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
        }
        public static List<SAPPosition> GetByEndpointId(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPPosition>(dataAccessTransaction).Get(sapPosition => sapPosition.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static void DeleteByEndpointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPPosition>(dataAccessTransaction).Get(sapposition => sapposition.EndPointSystemId == endPointSystemId).ToList());
                DeleteList(ItemstobeDeleted);
            }
        }
    }
}
