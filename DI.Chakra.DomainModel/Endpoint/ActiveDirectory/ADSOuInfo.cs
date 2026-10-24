using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ADSOuInfo : DomainModelBase<ADSOuInfo, EntityModel.ADSOuInfo>
    {

        public long EndPointSystemId { get; set; }

        public string ou { get; set; }

        public string description { get; set; }

        public string distinguishedName { get; set; }

        public string instanceType { get; set; }

        public DateTime? whenCreated { get; set; }

        public DateTime? whenChanged { get; set; }

        public Int64 uSNCreated { get; set; }

        public Int64 uSNChanged { get; set; }

        public string showInAdvancedViewOnly { get; set; }

        public string name { get; set; }

        public string objectGUID { get; set; }

        public string systemFlags { get; set; }

        public string objectCategory { get; set; }

        public string isCriticalSystemObject { get; set; }

        public string gPLink { get; set; }

        public string nTSecurityDescriptor { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int? Version { get; set; }


        public static List<ADSOuInfo> GetByEndPointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSOuInfo>(dataAccessTransaction)
                       .Get(lq => lq.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static ADSOuInfo GetByOu(string ou)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSOuInfo>(dataAccessTransaction)
                       .Get(ouInfo => ouInfo.ou == ou).FirstOrDefault());
            }
        }

        public static Dictionary<long, string> GetBySearchString(string searchString, long endpointsystemid)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                EndPoint endpoint = EndPoint.GetById(endpointsystemid);
                if (endpoint.EndpointSystemTypeId != (long)Infrastructure.Enums.EndpointType.ActiveDirectory)
                {
                    endpoint = EndPoint.GetByEndpointTypeId((long)Infrastructure.Enums.EndpointType.ActiveDirectory);
                    endpointsystemid = endpoint.Id;
                }

                return new ADSOuInfoRepository(dataAccessTransaction).GetBySearchString(CheckNull.AsteriskToString(searchString), endpointsystemid);
            }
        }

        public static bool DoesExists(string distinguishedName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.ADSOuInfo>(dataAccessTransaction)
                     .Any(ouinfo => ouinfo.distinguishedName == distinguishedName);
            }
        }
        public static bool IsADOUValid(long endpointSystemId, string value)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.ADSOuInfo>(dataAccessTransaction)
                     .Any(ouinfo => ouinfo.distinguishedName == value && ouinfo.EndPointSystemId == endpointSystemId);
            }
        }
        public static IList<ADSOuInfo> GetDeletedByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = DiscoveryProcess.GetById(endPointSystemId);

                IList<ADSOuInfo> listDeletedOus = new List<ADSOuInfo>();

                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessId(objDiscoveryProcess.Id);
                    if (process != null)
                    {
                        listDeletedOus = ORMapList(new RepositoryBase<EntityModel.ADSOuInfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId && grp.LastModifiedDate < process.ProcessStartAt).ToList());
                    }
                }

                return listDeletedOus;

            }
        }
        public static void SaveOUs(IList<ADSOuInfo> list, string currentUser, long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                IList<ADSOuInfo> listExistingADous = new List<ADSOuInfo>();
                ADSOuInfo adexistingOus = null;
                listExistingADous = ADSOuInfo.GetByEndPointSystemId(endpointId);
                foreach (ADSOuInfo ou in list)
                {
                    adexistingOus = listExistingADous.Where(t => t.distinguishedName == ou.distinguishedName).FirstOrDefault();
                    if (adexistingOus == null)
                    {
                        ou.Save(dataAccessTransaction);
                    }
                    else
                    {
                        ou.Id = adexistingOus.Id;
                        ou.description = adexistingOus.description;
                        ou.CreatedDate = adexistingOus.CreatedDate;
                        ou.CreatedBy = adexistingOus.CreatedBy;
                        ou.Save(dataAccessTransaction);
                    }

                }
                dataAccessTransaction.Commit();
            }
        }
        public static void AddADSOuInfoBulk(IList<ADSOuInfo> listOus)
        {
            ADSUserInfo.DeleteAll<ADSOuInfo>(listOus.First().EndPointSystemId);
            SQLBulkUploader<ADSOuInfo> sql = null;
            string tableName = "DI_ADS_OuInfo";
            sql = new SQLBulkUploader<ADSOuInfo>(tableName);
            sql.Flush(listOus);
            sql.Close();
            DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added {0} AD OU objects", listOus.Count.ToString()));
        }
    }
}
