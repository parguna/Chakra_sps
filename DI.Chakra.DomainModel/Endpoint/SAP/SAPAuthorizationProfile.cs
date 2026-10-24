using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPAuthorizationProfile : DomainModelBase<SAPAuthorizationProfile, EntityModel.SAPAuthorizationProfile>
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

                       
        public static IList<SAPAuthorizationProfile> GetByAuthcode(string authorizationCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPAuthorizationProfile>(dataAccessTransaction).Get(authProfile => authProfile.AuthorizationCode == authorizationCode).ToList());      
            }
        }


        public static SAPAuthorizationProfile GetByAuthCode(string authorizationCode,string objectCode,string profileCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPAuthorizationProfile>(dataAccessTransaction).Get(authProfile => authProfile.AuthorizationCode == authorizationCode && authProfile.ObjectCode ==
                    objectCode && authProfile.ProfileCode == profileCode, true).FirstOrDefault());
            }
        }

        public static List<SAPAuthorizationProfile> GetByModifiedDate(long endPointSystemId, DateTime? modifiedEndDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPAuthorizationProfile>(dataAccessTransaction).Get(authProfile => authProfile.EndPointSystemId == endPointSystemId && authProfile.LastModifiedDate < modifiedEndDate).ToList());

            }
           
        }

        public static IList<SAPAuthorizationProfile> GetDeletedProfile(long endpointsystemid)
        {
            DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
            List<SAPAuthorizationProfile> listDeletedAuthorizations = new List<SAPAuthorizationProfile>();
            objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endpointsystemid).FirstOrDefault();
               
            if (objDiscoveryProcess != null)
            {
                Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                if (process != null)
                {
                    listDeletedAuthorizations = SAPAuthorizationProfile.GetByModifiedDate(endpointsystemid, process.ProcessStartAt);
                }
            }

            return listDeletedAuthorizations;
        }

        public static void SaveAuthorizations(IList<SAPAuthorizationProfile> list, long EndpointSystemId, string currentUser, IList<SAPAuthorizationProfile> existingObjects = null)
        {
            try
            {
                //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
                {
                    IList<SAPAuthorizationProfile> listExistingObjects = new List<SAPAuthorizationProfile>();
                    SAPAuthorizationProfile sapexistingobj = null;
                    if (existingObjects == null)
                    {
                        listExistingObjects = SAPAuthorizationProfile.GetByEndpointId(EndpointSystemId);
                    }
                    else
                    {
                        listExistingObjects = existingObjects;
                    }
                    foreach (SAPAuthorizationProfile obj in list)
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

        public static void SaveAuthorizations(IList<SAPAuthorizationProfile> list, string currentUser)
        {
            try
            {
                foreach (SAPAuthorizationProfile obj in list)
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
                var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPAuthorizationProfile>(dataAccessTransaction).Get(azprofile => azprofile.EndPointSystemId == endPointSystemId).ToList());
                DeleteList(ItemstobeDeleted);
            }
        }

        public static IList<SAPAuthorizationProfile> GetByEndpointId(long endPointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPAuthorizationProfile>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointId).ToList());
            }
        }

        public static void DeleteSAPAuthorizationProfileByEndpoint(long endpointSystemId, string currentUser)
        {
            string query = "DELETE [DI_SAP_AuthorizationProfile] where [EndPointSystemId] = {0}";
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQuery(String.Format(query, endpointSystemId));
            }
        }
    }
}
