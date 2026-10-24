using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPFieldValueMapping : DomainModelBase<SAPFieldValueMapping, EntityModel.SAPFieldValueMapping>
    {
        public string RoleCode { get; set; }

        public string FieldCode { get; set; }

        public string FromValue { get; set; }

        public string ToValue { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        

        public int? Version { get; set; }

        public static void SaveSAPFieldValueMap(IList<SAPFieldValueMapping> list, long endpointSystemId, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                IList<SAPFieldValueMapping> listExistingObjects = new List<SAPFieldValueMapping>();
                SAPFieldValueMapping sapExistingObj = null;
                listExistingObjects = SAPFieldValueMapping.GetAll();
                foreach (SAPFieldValueMapping obj in list)
                {
                    sapExistingObj = listExistingObjects.Where(t => t.RoleCode == obj.RoleCode 
                    && t.FieldCode == obj.FieldCode
                    && t.FromValue == obj.FromValue
                    && t.ToValue == obj.ToValue
                    && t.EndPointSystemId == endpointSystemId).FirstOrDefault();
                    if (sapExistingObj == null)
                    {
                        obj.Save(dataAccessTransaction);
                    }
                    else
                    {
                        obj.Id = sapExistingObj.Id;
                        obj.Save(dataAccessTransaction);
                    }
                }

                dataAccessTransaction.Commit();
            }
        }

        public static void SaveSAPFieldValueMap(IList<SAPFieldValueMapping> list, string currentUser)
        {
            foreach (SAPFieldValueMapping obj in list)
            {
                obj.Save(currentUser);
            }
        }

        public static SAPFieldValueMapping GetByFieldCode(string roleCode, string fieldCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPFieldValueMapping>(dataAccessTransaction).
                    Get(sapFVMap => 
                        sapFVMap.FieldCode == fieldCode && 
                        sapFVMap.RoleCode == roleCode).
                        FirstOrDefault());
            }
        }

        public static List<SAPFieldValueMapping> GetByEndpointSystemId(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPFieldValueMapping>(dataAccessTransaction).
                    Get(sapFVMap => 
                        sapFVMap.EndPointSystemId == endpointSystemId).ToList());
            }
        }


        public static List<SAPFieldValueMapping> GetDeletedSAPFieldValueMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                List<SAPFieldValueMapping> listDeletedSapObjects = new List<SAPFieldValueMapping>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endpointSystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id,Infrastructure.Enums.ProcessType.Discovery);
                    if (process != null)
                    {
                        listDeletedSapObjects = ORMapList(new RepositoryBase<EntityModel.SAPFieldValueMapping>(dataAccessTransaction).
                            Get(sapFVMap => 
                                sapFVMap.EndPointSystemId == endpointSystemId && 
                                sapFVMap.LastModifiedDate < process.ProcessStartAt).
                                ToList());
                    }
                }
                return listDeletedSapObjects;
            }
        }

        public static void Remove(SAPFieldValueMapping sapFVMapObj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPFieldValueMapping sapObject = ORMap(new RepositoryBase<EntityModel.SAPFieldValueMapping>(dataAccessTransaction).
                    Get(sapFVMap =>
                        sapFVMap.RoleCode == sapFVMapObj.RoleCode &&
                        sapFVMap.FieldCode == sapFVMapObj.FieldCode &&
                        sapFVMap.EndPointSystemId == sapFVMapObj.EndPointSystemId).
                        FirstOrDefault());
                Delete(sapObject.Id);
            }
        }

        public static string GetFieldValue(string value, string authCode, string objectCode, string fieldCode)
        {
            string matchString = string.Empty;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPRoleToAuthMap roleToAuthMap = SAPRoleToAuthMap.GetByAuthCode(authCode, objectCode, fieldCode);
                {
                    if (roleToAuthMap != null)
                    {
                        SAPFieldValueMapping fieldvalue = GetByFieldCode(fieldCode, roleToAuthMap.RoleName);
                        matchString = fieldvalue.FromValue;
                    }
                    return matchString;
                }
            }
        }

        public static SAPFieldValueMapping GetByFieldCode(string authCode, string objectCode, string fieldCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPFieldValueMapping fieldvalue = null;

                SAPRoleToAuthMap roletoauthmap = SAPRoleToAuthMap.GetByAuthCode(authCode, objectCode, fieldCode);

                if (roletoauthmap != null)
                {
                    fieldvalue = GetByFieldCode(fieldCode, roletoauthmap.RoleName);
                }
                return fieldvalue;
            }
        }
        public static void DeleteSAPFieldValueMap(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPFieldValueMapping>(dataAccessTransaction).Get(sapazObject => sapazObject.EndPointSystemId == endPointSystemId).ToList());
                DeleteList(ItemstobeDeleted);
            }
        }

        public static void DeleteSAPFieldValueMapByEndpoint(long endpointSystemId, string currentUser)
        {
            string query = "DELETE [DI_SAP_TCodeObject] where [EndPointSystemId] = {0}";
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQuery(String.Format(query, endpointSystemId));
            }
        }

    }
}
