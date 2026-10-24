using DI.Chakra.Infrastructure;
using AutoMapper;
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
    public class EndpointObjects : DomainModelBase<EndpointObjects, EntityModel.EndpointObjects>
    {
        public int EndpointSystemTypeId { get; set; }

        public string ObjectName { get; set; }

        public bool IsTrustedSource { get; set; }
        public List<DiscoveryProcess> DiscoveryProcess { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static List<EndpointObjects> GetByEndpointSystemTypeId(long? endpointId, long endpointSystemTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<EndpointObjects> objects = new List<EndpointObjects>();
                objects = ORMapList(new RepositoryBase<EntityModel.EndpointObjects>(dataAccessTransaction).
                    Get(eo => eo.EndpointSystemTypeId == endpointSystemTypeId).ToList());

                //if (endpointId != null && endpointSystemTypeId == (long)EndpointType.ActiveDirectory)
                //{
                //    long wogKeyId = EndpointConfigurationKey.GetByKeyName("IsWOG").Id;
                //    long endpointConfigId = EndpointConfiguration.GetByEndpointSystemId(Convert.ToInt64(endpointId)).Id;
                //    EndpointConfigurationDetail detail = EndpointConfigurationDetail.GetByKeyId(Convert.ToInt64(endpointConfigId), wogKeyId);
                //    if (detail != null && detail.Value == "True")
                //        return objects.Where(x => x.ObjectName == "User").ToList();
                //    else
                //        return objects;
                //}
                //else
                    return objects;
            }
        }

        public static List<EndpointObjects> GetTrustedSourceObjectsByEndpointSystemType(long endpointSystemTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<EndpointObjects> objects = new List<EndpointObjects>();
                objects = ORMapList(new RepositoryBase<EntityModel.EndpointObjects>(dataAccessTransaction).
                    Get(eo => eo.EndpointSystemTypeId == endpointSystemTypeId && eo.IsTrustedSource == true).ToList());

                return objects;
            }
        }
        //Suresh - Commenting all the below methods as I find it not needed. Later delete the code completely 
        /*
        public static IList<EndpointObjects> GetADObjects()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EndPointSystemType endPointSystemType = EndPointSystemType.GetByName(EndpointType.ActiveDirectory);
                return EndpointObjects.GetByEndpointSystemTypeId(endPointSystemType.Id);
            }
        }

        public static IList<EndpointObjects> GetLinuxObjects()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EndPointSystemType endPointSystemType = EndPointSystemType.GetByName(EndpointType.Linux);
                return EndpointObjects.GetByEndpointSystemTypeId(endPointSystemType.Id);
            }
        }

        public static IList<EndpointObjects> GetLNSObjects()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EndPointSystemType endPointSystemType = EndPointSystemType.GetByName(EndpointType.LotusNotes);
                return EndpointObjects.GetByEndpointSystemTypeId(endPointSystemType.Id);
            }
        }

        public static IList<EndpointObjects> GetORACLEObjects()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EndPointSystemType endPointSystemType = EndPointSystemType.GetByName(EndpointType.OracleEBS);
                return EndpointObjects.GetByEndpointSystemTypeId(endPointSystemType.Id);
            }
        }

        public static IList<EndpointObjects> GetSAPObjects()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EndPointSystemType endPointSystemType = EndPointSystemType.GetByName(EndpointType.SAP);
                return EndpointObjects.GetByEndpointSystemTypeId(endPointSystemType.Id);
            }
        }

        public static IList<EndpointObjects> GetSAPEPObjects()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EndPointSystemType endPointSystemType = EndPointSystemType.GetByName(EndpointType.SAPEnterprisePortal);
                return EndpointObjects.GetByEndpointSystemTypeId(endPointSystemType.Id);
            }
        }

        public static IList<EndpointObjects> GetORACLEUMObjects()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EndPointSystemType endPointSystemType = EndPointSystemType.GetByName(Constants.OracleUM);
                return EndpointObjects.GetByEndpointSystemTypeId(endPointSystemType.Id);
            }
        }
        */
    }
}

