using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPRoleToAuthMap : DomainModelBase<SAPRoleToAuthMap, EntityModel.SAPRoleToAuthMap>
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

        public static List<SAPRoleToAuthMap> GetByCodes(string roleCode, string objectCode, string fieldCode)
        {
            using(DataAccessTransaction dataAccessTransaction =new DataAccessTransaction ())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.RoleName == roleCode && roleInfo.ObjectCode == objectCode && roleInfo.FieldCode == fieldCode).ToList());
            }
        }

        public static SAPRoleToAuthMap GetByAuthCode(string authCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.AuthCode == authCode).FirstOrDefault());
            }
        }

        public static SAPRoleToAuthMap GetByAuthCode(string authCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.AuthCode == authCode && roleInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static List<SAPRoleToAuthMap> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using(DataAccessTransaction dataAccessTransaction =new DataAccessTransaction ())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId && roleInfo.LastModifiedDate < modifiedDate).ToList());
            }
        }
        public static List<SAPRoleToAuthMap> GetByCodes(string objectCode, long endPointSystemId)
        {
            using(DataAccessTransaction dataAccessTransaction =new DataAccessTransaction ())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.ObjectCode == objectCode && roleInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }
       
        public static List<SAPRoleToAuthMap> GetRolesByTCode(string tCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.ObjectCode == "S_TCODE" && roleInfo.EndPointSystemId == endPointSystemId && roleInfo.FieldCode == "TCD" && roleInfo.FromValue == tCode).ToList());
            }
        }

        public static List<SAPRoleToAuthMap> GetRolesByRoleName(string roleName, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.ObjectCode == "S_TCODE" && roleInfo.EndPointSystemId == endPointSystemId && roleInfo.FieldCode == "TCD" && roleInfo.RoleName == roleName).ToList());
            }
        }

        public static List<SAPRoleToAuthMap> GetByCodes(string roleCode, string objectCode)
        {
            using(DataAccessTransaction dataAccessTransaction =new DataAccessTransaction ())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.RoleName == roleCode && roleInfo.ObjectCode == objectCode).ToList());
            }
        }

         public static SAPRoleToAuthMap GetByAuthCode(string authCode, string objectCode, string fieldCode)
         {
             using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 return ORMap(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.AuthCode == authCode && roleInfo.ObjectCode == objectCode && roleInfo.FieldCode == fieldCode).FirstOrDefault());
             }
         }

        


         public static List<SAPRoleToAuthMap> GetByDistinctRoles(List<string> distinctRoles, long endPointSystemId)
         {
             List<SAPRoleToAuthMap> SapRoleToAuthorizations = new List<SAPRoleToAuthMap>();
               using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                 {
                   
                     //var SapRoleToAuthorizations = from Roles in DistinctRoles 
                     //                              join RoleToAuth in _context.SapRoleToAuthMap on Roles equals RoleToAuth.RoleName
                     //                              where RoleToAuth.EndPointSystemId == EndpointSystemId
                     //                              select RoleToAuth;

                     //var SapRoleToAuthorizations = from RoleToAuth in SAPRoleToAuthMap.GetAll()
                     //                              where distinctRoles.Contains(RoleToAuth.RoleName)
                     //                              select RoleToAuth;

                     return SapRoleToAuthorizations.ToList();

                 }
            
         }

         public IList<SAPRoleToAuthMap> GetDeletedSAPRoleToAuthMap(long endPointSystemId)
         {
             using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                 IList<SAPRoleToAuthMap> listDeletedSapRoleToAuthMap = new List<SAPRoleToAuthMap>();
                 objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                 if (objDiscoveryProcess != null)
                 {
                     Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                     
                     if (process != null)
                     {
                         listDeletedSapRoleToAuthMap = GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
                      //       SAPRoleToAuthMap.GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
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
                var roleTCodes = ORMapList(new SAPRoleToAuthMapRepository(dataAccessTransaction).GetTCodesBySAPRole(roleCode, endPointSystemId));

                foreach (SAPRoleToAuthMap item in roleTCodes)
                {
                    if (string.IsNullOrEmpty(item.ToValue))
                    {
                        if (item.FromValue.Contains("*"))
                        {
                            var tcodesFromRange = new SAPRoleToAuthMapRepository(dataAccessTransaction).GetTCodesByPattern(item.FromValue, endPointSystemId);
                            tcodes.AddRange(tcodesFromRange);
                        }
                        else
                        {
                            tcodes.Add(item.FromValue);
                        }
                    }
                    else
                    {
                        var tcodesFromRange = new SAPRoleToAuthMapRepository(dataAccessTransaction).GetTCodesByRange(item.FromValue, item.ToValue, endPointSystemId);
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
                 return new SAPRoleToAuthMapRepository(dataAccessTransaction).GetAllAssignedTCodes(endPointSystemId);
             }
         }



         public static void DeleteByEndpointSystemId(long endPointSystemId)
         {
             using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId).ToList());
                 DeleteList(ItemstobeDeleted);
             }
         }

        /// <summary>
        /// Using querey to delete.
        /// </summary>
        /// <param name="endpointSystemId"></param>
        public static void DeleteSAPRoleToAuthMapForEndpoint(long endpointSystemId)
        {
            string query = "DELETE [DI_SAP_RoleToAuthMap] WHERE EndPointSystemId = {0}";
            ExecuteQueryWithCommandTimeout(String.Format(query, endpointSystemId),300);
        }

        public static void SaveSapRoleAuthMap(IList<SAPRoleToAuthMap> list, long endpointSystemId, string currentUser)
         {
             using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
             {
                 IList<SAPRoleToAuthMap> listExistingSAPRoleToAuthMaps = new List<SAPRoleToAuthMap>();
                 SAPRoleToAuthMap sapexistingSAPRoleToAuthMap = null;
                 listExistingSAPRoleToAuthMaps = SAPRoleToAuthMap.GetByEndpointId(endpointSystemId);
                foreach (SAPRoleToAuthMap saproleauthmap in list)
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

        public static void SaveSapRoleAuthMap(IList<SAPRoleToAuthMap> list, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                // saving list assuming all existing data has been bulk deleted already
                // so no existing item check
                foreach (SAPRoleToAuthMap saproleauthmap in list)
                {
                    saproleauthmap.Save(currentUser);
                }
            }
        }

        public static List<SAPRoleToAuthMap> GetByEndpointId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPRoleToAuthMap>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }
    }
}
