using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANAAuthorizationToObject : DomainModelBase<SAPHANAAuthorizationToObject, EntityModel.SAPHANAAuthorizationToObject>
    {
        public string AuthorizationCode { get; set; }

        public string ObjectCode { get; set; }

        public string FieldCode { get; set; }

        public string FromValue { get; set; }

        public string ToValue { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        

        public int? Version { get; set; }



        public static SAPHANAAuthorizationToObject GetByObjectCode(string objectCode, string fieldCode, string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.ObjectCode == objectCode && authObject.FieldCode ==
                   fieldCode && authObject.FromValue == value, true).FirstOrDefault());
              
            }
        }

        public static SAPHANAAuthorizationToObject GetByObjectCode(string objectCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.ObjectCode == objectCode).FirstOrDefault());
            }
        }

        public static SAPHANAAuthorizationToObject GetByObjectCode(string objectCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction=new DataAccessTransaction ())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.ObjectCode == objectCode && authObject.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }

        }

        public static IList<SAPHANAAuthorizationToObject> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.EndPointSystemId == endPointSystemId && authObject.LastModifiedDate < modifiedDate).ToList());
            }
        }

        public static IList<SAPHANAAuthorizationToObject> GetByFieldCode(long endPointSystemId, string fieldCode, string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.EndPointSystemId == endPointSystemId && authObject.FieldCode == fieldCode && authObject.FromValue == value).ToList());
            }
        }


        public static IList<SAPHANAAuthorizationToObject> GetByFieldCode(string fieldCode, string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.FieldCode == fieldCode && authObject.FromValue == value).ToList());
            }
        }

        public static IList<SAPHANAAuthorizationToObject> GetByFieldCode(long endPointSystemId, string authCode, string objectCode, string fieldCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.FieldCode == fieldCode && authObject.AuthorizationCode == authCode && authObject.EndPointSystemId == endPointSystemId && authObject.ObjectCode == objectCode).ToList());
            }
        }

        public static IList<SAPHANAAuthorizationToObject> GetByObjectAuthCode(long endPointSystemId, string authCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.EndPointSystemId == endPointSystemId && authObject.AuthorizationCode == authCode).ToList());
            }
        }

            public static IList<SAPHANAAuthorizationToObject> GetByObjectAuthCode(string objectCode,string authCode)
            {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.ObjectCode == objectCode && authObject.AuthorizationCode == authCode).ToList());
            }
        }

        public static List<string> GetByFromValue(string FromValue, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<string> list = null;

                if (endPointSystemId != -1)
                {
                    list = new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.EndPointSystemId == endPointSystemId && authObject.FieldCode == "TCD" && authObject.FromValue == FromValue).Distinct().Select(authObject => authObject.AuthorizationCode).ToList();
                }
                else
                {
                    list = new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.FieldCode == "TCD" && authObject.FromValue == FromValue).Distinct().Select(authObject => authObject.AuthorizationCode).ToList();
                }
                return list;
            }
        }

        public static SAPHANAAuthorizationToObject GetByObjectCode(string objectCode, string fieldCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(authObject => authObject.ObjectCode == objectCode && authObject.FieldCode == fieldCode).FirstOrDefault());
            }
        }



        public static IList<SAPHANAAuthorizationToObject> GetDeletedSapObjectsAuthMap(long endpointId)
        {

            DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
            IList<SAPHANAAuthorizationToObject> listDeletedSapObjects = new List<SAPHANAAuthorizationToObject>();
            objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endpointId).FirstOrDefault();
            if (objDiscoveryProcess != null)
            {
                Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);

                if (process != null)
                {
                    listDeletedSapObjects = SAPHANAAuthorizationToObject.GetByModifiedDate(endpointId, process.ProcessStartAt);

                }
            }
            return listDeletedSapObjects;
        }

        public static void DeleteSAPObjectsAuthMap(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(sapazObject => sapazObject.EndPointSystemId == endPointSystemId).ToList());
                DeleteList(ItemstobeDeleted);
            }
        }

        public static void SaveSAPObjectsAuthMap(IList<SAPHANAAuthorizationToObject> list, long endPointSystemId, string currentUser, IList<SAPHANAAuthorizationToObject> existingSAPObjectsAuthMaps = null)
        {
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                IList<SAPHANAAuthorizationToObject> listExistingSAPObjectsAuthMaps = new List<SAPHANAAuthorizationToObject>();
                SAPHANAAuthorizationToObject sapexistingSAPObjectsAuthMap = null;
                if (existingSAPObjectsAuthMaps == null)
                {
                    listExistingSAPObjectsAuthMaps = SAPHANAAuthorizationToObject.GetByEndpointId(endPointSystemId);
                }
                else
                {
                    listExistingSAPObjectsAuthMaps = existingSAPObjectsAuthMaps;
                }
                foreach (SAPHANAAuthorizationToObject sapobjectauthmap in list)
                {
                    sapexistingSAPObjectsAuthMap = listExistingSAPObjectsAuthMaps.Where(t => t.AuthorizationCode == sapobjectauthmap.AuthorizationCode 
                    && t.FieldCode == sapobjectauthmap.FieldCode && t.ObjectCode == sapobjectauthmap.ObjectCode
                    && t.EndPointSystemId == endPointSystemId).FirstOrDefault();
                    if (sapexistingSAPObjectsAuthMap == null)
                    {
                        sapobjectauthmap.Save(currentUser);
                        listExistingSAPObjectsAuthMaps.Add(sapobjectauthmap);
                    }
                    else
                    {
                        sapobjectauthmap.Id = sapexistingSAPObjectsAuthMap.Id;
                        sapobjectauthmap.Save(currentUser);
                    }
                }

                //dataAccessTransaction.Commit();
            }
        }

        public static void SaveSAPObjectsAuthMap(IList<SAPHANAAuthorizationToObject> list, string currentUser)
        {
            foreach (SAPHANAAuthorizationToObject sapobjectauthmap in list)
            {
                sapobjectauthmap.Save(currentUser);
            }
        }

        public static IList<SAPHANAAuthorizationToObject> GetByEndpointId(long endPointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAAuthorizationToObject>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointId).ToList());
            }
        }

        public static void DeleteSAPHANAAuthorizationToObjectByEndpoint(long endpointSystemId, string currentUser)
        {
            string query = "DELETE [DI_SAPHANA_AuthObjectMap] where [EndPointSystemId] = {0}";
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQuery(String.Format(query, endpointSystemId));
            }
        }
    }
}
