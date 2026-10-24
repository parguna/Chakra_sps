using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DI.Chakra.DomainModel.SAPHANAUserinfo;

namespace DI.Chakra.DomainModel
{
    public class SAPHANARoleToAuthMap : DomainModelBase<SAPHANARoleToAuthMap, EntityModel.SAPHANARoleToAuthMap>
    {
        public string RoleName { get; set; }

        public string ObjectCode { get; set; }

        public string AuthCode { get; set; }

        public string Variant { get; set; }

        public string BiwMenuCounter { get; set; }

        public string FieldCode { get; set; }

        public string ProfileGeneratorVariant { get; set; }

        public string FromValue { get; set; }

        public string ToValue { get; set; }

        

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public static List<SAPHANARoleToAuthMap> GetByCodes(string roleCode, string objectCode, string fieldCode)
        {
            using(DataAccessTransaction dataAccessTransaction =new DataAccessTransaction ())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.RoleName == roleCode && roleInfo.ObjectCode == objectCode && roleInfo.FieldCode == fieldCode).ToList());
            }
        }

        public static SAPHANARoleToAuthMap GetByAuthCode(string authCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.AuthCode == authCode).FirstOrDefault());
            }
        }

        public static SAPHANARoleToAuthMap GetByAuthCode(string authCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.AuthCode == authCode && roleInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static List<SAPHANARoleToAuthMap> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using(DataAccessTransaction dataAccessTransaction =new DataAccessTransaction ())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId && roleInfo.LastModifiedDate < modifiedDate).ToList());
            }
        }
        public static List<SAPHANARoleToAuthMap> GetByCodes(string objectCode, long endPointSystemId)
        {
            using(DataAccessTransaction dataAccessTransaction =new DataAccessTransaction ())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.ObjectCode == objectCode && roleInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }
       
        public static List<SAPHANARoleToAuthMap> GetRolesByTCode(string tCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.ObjectCode == "S_TCODE" && roleInfo.EndPointSystemId == endPointSystemId && roleInfo.FieldCode == "TCD" && roleInfo.FromValue == tCode).ToList());
            }
        }

        public static List<SAPHANARoleToAuthMap> GetRolesByRoleName(string roleName, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.ObjectCode == "S_TCODE" && roleInfo.EndPointSystemId == endPointSystemId && roleInfo.FieldCode == "TCD" && roleInfo.RoleName == roleName).ToList());
            }
        }

        public static List<SAPHANARoleToAuthMap> GetByCodes(string roleCode, string objectCode)
        {
            using(DataAccessTransaction dataAccessTransaction =new DataAccessTransaction ())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.RoleName == roleCode && roleInfo.ObjectCode == objectCode).ToList());
            }
        }

         public static SAPHANARoleToAuthMap GetByAuthCode(string authCode, string objectCode, string fieldCode)
         {
             using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 return ORMap(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.AuthCode == authCode && roleInfo.ObjectCode == objectCode && roleInfo.FieldCode == fieldCode).FirstOrDefault());
             }
         }

        


         public static List<SAPHANARoleToAuthMap> GetByDistinctRoles(List<string> distinctRoles, long endPointSystemId)
         {
             List<SAPHANARoleToAuthMap> SapRoleToAuthorizations = new List<SAPHANARoleToAuthMap>();
               using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                 {
                     return SapRoleToAuthorizations.ToList();
                 }
            
         }

         public IList<SAPHANARoleToAuthMap> GetDeletedSAPRoleToAuthMap(long endPointSystemId)
         {
             using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                 IList<SAPHANARoleToAuthMap> listDeletedSapRoleToAuthMap = new List<SAPHANARoleToAuthMap>();
                 objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                 if (objDiscoveryProcess != null)
                 {
                     Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                     
                     if (process != null)
                     {
                         listDeletedSapRoleToAuthMap = GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
                      //       SAPHANARoleToAuthMap.GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
                     }
                 }

                 return listDeletedSapRoleToAuthMap;

             }
         }

     
         public static List<string> GetByRoleCode(string roleCode, long endPointSystemId)
         {
             using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                List<string> tcodes = new List<string>();
                var roleTCodes = ORMapList(new SAPHANARoleToAuthMapRepository(dataAccessTransaction).GetTCodesBySAPRole(roleCode, endPointSystemId));

                foreach (SAPHANARoleToAuthMap item in roleTCodes)
                {
                    if (string.IsNullOrEmpty(item.ToValue))
                    {
                        if (item.FromValue.Contains("*"))
                        {
                            var tcodesFromRange = new SAPHANARoleToAuthMapRepository(dataAccessTransaction).GetTCodesByPattern(item.FromValue, endPointSystemId);
                            tcodes.AddRange(tcodesFromRange);
                        }
                        else
                        {
                            tcodes.Add(item.FromValue);
                        }
                    }
                    else
                    {
                        var tcodesFromRange = new SAPHANARoleToAuthMapRepository(dataAccessTransaction).GetTCodesByRange(item.FromValue, item.ToValue, endPointSystemId);
                        tcodes.AddRange(tcodesFromRange);
                    }
                }
                return tcodes.Distinct().ToList();
             }
         }

         public Dictionary<long, string> GetAssignedTCodes(long endPointSystemId)
         {
             using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 return new SAPHANARoleToAuthMapRepository(dataAccessTransaction).GetAllAssignedTCodes(endPointSystemId);
             }
         }



         public static void DeleteByEndpointSystemId(long endPointSystemId)
         {
             using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId).ToList());
                 DeleteList(ItemstobeDeleted);
             }
         }

        /// <summary>
        /// Using querey to delete.
        /// </summary>
        /// <param name="endpointSystemId"></param>
        public static void DeleteSAPRoleToAuthMapForEndpoint(long endpointSystemId)
        {
            string query = "DELETE [DI_SAPHANA_RoleToAuthMap] WHERE EndPointSystemId = {0}";
            ExecuteQueryWithCommandTimeout(String.Format(query, endpointSystemId),300);
        }

        public static void SaveSapRoleAuthMap(IList<SAPHANARoleToAuthMap> list, long endpointSystemId, string currentUser)
         {
             using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 IList<SAPHANARoleToAuthMap> listExistingSAPRoleToAuthMaps = new List<SAPHANARoleToAuthMap>();
                 SAPHANARoleToAuthMap sapexistingSAPRoleToAuthMap = null;
                 listExistingSAPRoleToAuthMaps = SAPHANARoleToAuthMap.GetByEndpointId(endpointSystemId);
                foreach (SAPHANARoleToAuthMap saproleauthmap in list)
                {
                    sapexistingSAPRoleToAuthMap = listExistingSAPRoleToAuthMaps.Where(t => t.RoleName == saproleauthmap.RoleName
                    && t.AuthCode == saproleauthmap.AuthCode && t.BiwMenuCounter == saproleauthmap.BiwMenuCounter && t.ObjectCode == saproleauthmap.ObjectCode && t.EndPointSystemId == endpointSystemId).FirstOrDefault();
                    if (sapexistingSAPRoleToAuthMap == null)
                    {
                        saproleauthmap.Save(currentUser);
                    }
                    else
                    {
                        saproleauthmap.Id = sapexistingSAPRoleToAuthMap.Id;
                        saproleauthmap.Save(currentUser);
                    }
                }

                dataAccessTransaction.Commit();
             }
         }

        public static void SaveSapRoleAuthMap(IList<SAPHANARoleToAuthMap> list, string currentUser)
        {
            if (list != null && list.Count > 0)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    //// saving list assuming all existing data has been bulk deleted already
                    //// so no existing item check
                    SQLBulkUploader<SAPHANARoleToAuthMap> sqlSapRoleAuthMap = null;
                    try
                    {

                        list = list.Distinct().ToList();
                        string tableName = "DI_SAPHANA_RoleToAuthMap";

                        sqlSapRoleAuthMap = new SQLBulkUploader<SAPHANARoleToAuthMap>(tableName);

                        sqlSapRoleAuthMap.Flush(list);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlSapRoleAuthMap?.Close();
                    }
                    //// saving list assuming all existing data has been bulk deleted already
                    //// so no existing item check
                    //foreach (SAPHANARoleToAuthMap saproleauthmap in list)
                    //{
                    //    saproleauthmap.Save(currentUser);
                    //}
                }
            }
        }

        public static List<SAPHANARoleToAuthMap> GetByEndpointId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleToAuthMap>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }
    }
}
