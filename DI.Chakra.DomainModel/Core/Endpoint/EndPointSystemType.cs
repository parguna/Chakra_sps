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
    public class EndPointSystemType : DomainModelBase<EndPointSystemType, EntityModel.EndPointSystemType>
    {
        public EndpointType EndpointSystemTypeName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string ObjectName { get; set; }

        public bool CanBeTrustedSource { get; set; }

        public List<EndPoint> EndPoints { get; set; }

        public List<EndpointObjects> EndpointObjects { get; set; }

        public static EndpointType GetEndPointTypeName(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).GetById(id).EndpointSystemTypeName;
            }
        }

        public static EndPointSystemType GetByIdIncludingEndpoints(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).GetById(id, epSysType => epSysType.EndPoints));
            }
        }

        public static EndPointSystemType GetByName(EndpointType name)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).Get(ept => ept.EndpointSystemTypeName == name).FirstOrDefault());
            }
        }

        public static EndPointSystemType GetByName(string name)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).Get(ept => Convert.ToString(ept.EndpointSystemTypeName) == name).FirstOrDefault());
            }
        }

        public static IList<EndPointSystemType> GetAllByEPSTypeID()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                int EPSTypeID = Convert.ToInt32(EndpointType.InhouseSystems);
                return ORMapList(new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).Get(eps => eps.IsActive == true && eps.Id != EPSTypeID).ToList());
            }
        }

        public static IList<EndPointSystemType> GetAllByINHVPN()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<long> EPSTypeIDList = new List<long>();
                EPSTypeIDList.Add(Convert.ToInt32(EndpointType.InhouseSystems));
                EPSTypeIDList.Add(Convert.ToInt32(EndpointType.VPN));
                return ORMapList(new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).Get(eps => eps.IsActive == true && !EPSTypeIDList.Contains(eps.Id)).ToList());
            }
        }

        public static IList<EndPointSystemType> GetAllforAttestation()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).Get(eps => eps.IsActive == true, eps => eps.EndPoints).ToList());
            }
        }

        public static EndPointSystemType GetByType(EndpointType EndpointType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                return ORMap(new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).Get(lq => lq.EndpointSystemTypeName == EndpointType, eps => eps.EndPoints).FirstOrDefault());
            }
        }
        public static EndPointSystemType GetByEndpointSystemTypeName(EndpointType endpointSystemTypeName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).
                    Get(eps => eps.IsActive == true && eps.EndpointSystemTypeName == endpointSystemTypeName, eps => eps.EndPoints).
                    FirstOrDefault());
            }
        }

        public static EndPointSystemType GetByEndpointSystemTypeName(long endpointSystemTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).
                    Get(eps => eps.IsActive == true && eps.EndpointSystemTypeName == (EndpointType)endpointSystemTypeId, eps => eps.EndPoints).
                    FirstOrDefault());
            }
        }

        public static IList<EndPointSystemType> GetByIsActiveForProvisioning()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<long> EPSTypeIDList = new List<long>();
                EPSTypeIDList.Add(Convert.ToInt32(EndpointType.InhouseSystems));
                EPSTypeIDList.Add(Convert.ToInt32(EndpointType.VPN));

                return ORMapList(new RepositoryBase<EntityModel.EndPointSystemType>(dataAccessTransaction).Get(eps => eps.IsActive == true && !EPSTypeIDList.Contains(eps.Id), eps => eps.EndPoints).ToList());
            }
        }

        public static Dictionary<long, string> GetEndpointSystemType()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new EndPointRepository(dataAccessTransaction).GetEndPointSystemTypes();
            }
        }
    }
    
}
