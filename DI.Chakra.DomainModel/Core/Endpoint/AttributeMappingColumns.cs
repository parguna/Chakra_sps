using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;
namespace DI.Chakra.DomainModel
{
    public class AttributeMappingColumns : DomainModelBase<AttributeMappingColumns, EntityModel.AttributeMappingColumns>
    {
        public string DiColumn { get; set; }

        public string Tgcolumn { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public Nullable<long> EndpointSystemId { get; set; }

        public Nullable<long> EndpointSystemTypeId { get; set; }

        public EndPoint EndPoint { get; set; }

        public EndPointSystemType EndPointSystemType { get; set; }

        public static List<AttributeMappingColumns> GetByEndpointSystemId(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AttributeMappingColumns>(dataAccessTransaction).Get(amc => amc.EndpointSystemId == endpointSystemId).ToList());
            }
        }

        public static List<AttributeMappingColumns> GetByEndPoindId(long endPointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AttributeMappingColumns>(dataAccessTransaction).
                    Get(amc => amc.EndPoint.Id == endPointId).ToList());
            }
        }


        public static List<AttributeMappingColumns> GetWinAttributeMappingColumns()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EndPointSystemType EndPntSysType = null;
                return ORMapList(new RepositoryBase<EntityModel.AttributeMappingColumns>(dataAccessTransaction).
                    Get(est => EndPntSysType.DisplayName == Constants.WINDOWS).ToList());             
            }
        }



        public static void DeleteByEndEndpointSystemId(long endpointSystemId)
        {
            AttributeMappingColumns.DeleteList(AttributeMappingColumns.GetByEndpointSystemId(endpointSystemId));
        }

        //yet to develop
        public static List<AttributeMappingColumns> GetAttributeMappingColumns(long endpointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AttributeMappingColumns>(dataAccessTransaction).
                    Get(amc => amc.EndPoint.Id == endpointId).ToList());
            }
        }
    }
}
