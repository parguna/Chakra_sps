using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPObjectValues : DomainModelBase<SAPObjectValues, EntityModel.SAPObjectValues>
    {
        public string SapObjectType { get; set; }

        public string SapObjectCode { get; set; }

        public String SapOkFlag { get; set; }

        public string ModificationID { get; set; }

        public string OrgName { get; set; }
        
        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }



        public int? Version { get; set; }

        public static SAPObjectValues GetByObjectCode(string objectCode, string objectType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPObjectValues>(dataAccessTransaction).Get(sapobj => sapobj.SapObjectCode == objectCode && sapobj.SapObjectType == objectType).FirstOrDefault());
            }
        }

        public static IList<SAPObjectValues> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPObjectValues>(dataAccessTransaction).Get(sapobj => sapobj.EndPointSystemId == endPointSystemId && sapobj.LastModifiedDate <= modifiedDate).ToList());
            }
        }

        public static SAPObjectValues GetByObjectCode(string objectCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPObjectValues>(dataAccessTransaction).Get(sapobj => sapobj.SapObjectCode == objectCode).FirstOrDefault());
            }
        }


        public IList<SAPObjectValues> GetDeletedObjectValue(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPObjectValues> listDeletedSapObjects = new List<SAPObjectValues>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                    
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                    if (process != null)
                    {
                        listDeletedSapObjects = SAPObjectValues.GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
                    }
                }
                return listDeletedSapObjects;
            }
        }
    }
}
