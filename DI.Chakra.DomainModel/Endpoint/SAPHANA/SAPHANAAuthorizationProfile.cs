using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANAAuthorizationProfile : DomainModelBase<SAPHANAAuthorizationProfile, EntityModel.SAPHANAAuthorizationProfile>
    {
        public string ProfileCode { get; set; }

        public string ObjectCode { get; set; }

        public string AuthorizationCode { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        

        public int? Version { get; set; }

                       
        public static IList<SAPHANAAuthorizationProfile> GetByAuthcode(string authorizationCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationProfile>(dataAccessTransaction).Get(authProfile => authProfile.AuthorizationCode == authorizationCode).ToList());      
            }
        }


        public static SAPHANAAuthorizationProfile GetByAuthCode(string authorizationCode,string objectCode,string profileCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAAuthorizationProfile>(dataAccessTransaction).Get(authProfile => authProfile.AuthorizationCode == authorizationCode && authProfile.ObjectCode ==
                    objectCode && authProfile.ProfileCode == profileCode, true).FirstOrDefault());
            }
        }

        public static List<SAPHANAAuthorizationProfile> GetByModifiedDate(long endPointSystemId, DateTime? modifiedEndDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationProfile>(dataAccessTransaction).Get(authProfile => authProfile.EndPointSystemId == endPointSystemId && authProfile.LastModifiedDate < modifiedEndDate).ToList());

            }
           
        }

        public static IList<SAPHANAAuthorizationProfile> GetDeletedProfile(long endpointsystemid)
        {
            DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
            List<SAPHANAAuthorizationProfile> listDeletedAuthorizations = new List<SAPHANAAuthorizationProfile>();
            objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endpointsystemid).FirstOrDefault();
               
            if (objDiscoveryProcess != null)
            {
                Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                if (process != null)
                {
                    listDeletedAuthorizations = SAPHANAAuthorizationProfile.GetByModifiedDate(endpointsystemid, process.ProcessStartAt);
                }
            }

            return listDeletedAuthorizations;
        }

        public static void SaveAuthorizations(IList<SAPHANAAuthorizationProfile> list, long EndpointSystemId, string currentUser, IList<SAPHANAAuthorizationProfile> existingObjects = null)
        {
            try
            {
                //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
                {
                    IList<SAPHANAAuthorizationProfile> listExistingObjects = new List<SAPHANAAuthorizationProfile>();
                    SAPHANAAuthorizationProfile sapexistingobj = null;
                    if (existingObjects == null)
                    {
                        listExistingObjects = SAPHANAAuthorizationProfile.GetByEndpointId(EndpointSystemId);
                    }
                    else
                    {
                        listExistingObjects = existingObjects;
                    }
                    foreach (SAPHANAAuthorizationProfile obj in list)
                    {
                        sapexistingobj = listExistingObjects.Where(t => t.ProfileCode == obj.ProfileCode && t.ObjectCode == obj.ObjectCode
                        && t.AuthorizationCode == obj.AuthorizationCode && t.EndPointSystemId == EndpointSystemId).FirstOrDefault();
                        if (sapexistingobj == null)
                        {
                            obj.Save(currentUser);
                            listExistingObjects.Add(obj);
                        }
                        else
                        {
                            obj.Id = sapexistingobj.Id;
                            obj.Save(currentUser);
                        }
                    }

                    //dataAccessTransaction.Commit();
                }

               
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void SaveAuthorizations(IList<SAPHANAAuthorizationProfile> list, string currentUser)
        {
            try
            {
                foreach (SAPHANAAuthorizationProfile obj in list)
                {
                    obj.Save(currentUser);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void DeleteAuthorizations(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationProfile>(dataAccessTransaction).Get(azprofile => azprofile.EndPointSystemId == endPointSystemId).ToList());
                DeleteList(ItemstobeDeleted);
            }
        }

        public static IList<SAPHANAAuthorizationProfile> GetByEndpointId(long endPointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationProfile>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointId).ToList());
            }
        }

        public static void DeleteSAPHANAAuthorizationProfileByEndpoint(long endpointSystemId, string currentUser)
        {
            string query = "DELETE [DI_SAPHANA_AuthorizationProfile] where [EndPointSystemId] = {0}";
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQuery(String.Format(query, endpointSystemId));
            }
        }
    }
}
