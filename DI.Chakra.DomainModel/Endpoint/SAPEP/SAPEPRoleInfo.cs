using DI.Chakra.Infrastructure;
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
    public class SAPEPRoleInfo : DomainModelBase<SAPEPRoleInfo, EntityModel.SAPEPRoleInfo>, IComparer<SAPEPRoleInfo>, IXLData, IEndpointEntitlementInfo
    {

        public long EndPointSystemId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string UniqueRoleName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }
        public bool IsXLUploaded { get; set; }

        public IList<SAPEPUserInfo> SAPEPUserInfos { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public bool? CanRequest { get; set; }
        public string UserDefinedDesc { get; set; }
        public override string ToString()
        {
            return this.RoleId.ToString();
        }
        public string EntitlementName
        {
            get { return UniqueRoleName; }
        }

        public string EntitlementDesc
        {
            get { return RoleName; }
        }

        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Role; }
        }



        public static SAPEPRoleInfo CreateSAPEPRoleInfo()
        {
            SAPEPRoleInfo sapEPRoleInfo = new SAPEPRoleInfo();
            return sapEPRoleInfo;
        }

        public override bool Equals(object obj)
        {
            SAPEPRoleInfo other = obj as SAPEPRoleInfo;
            if (obj != null)
            {
                return this.RoleId.Equals(other.RoleId);
            }
            else
                return false;
        }

        public static Dictionary<string, string> GetPortalRolesByActionType(long endpointSystemId, string searchString, List<long> endpointUserId, AdHocActionType actionType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEPRoleInfoRepository(dataAccessTransaction).GetPortalRolesByActionType(endpointSystemId, CheckNull.AsteriskToString(searchString), endpointUserId, actionType);

            }
        }

        public override int GetHashCode()
        {
            return this.RoleId.GetHashCode() ^ this.RoleName.GetHashCode();
        }

        public int Compare(SAPEPRoleInfo x, SAPEPRoleInfo y)
        {
            int cmprValue = -1;
            if (x != null && y != null)
            {

                cmprValue = string.Compare(y.RoleName, x.RoleName);
            }

            return cmprValue;
        }

        public static List<SAPEPRoleInfo> GetByEndPointId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static SAPEPRoleInfo GetByName(long endpointSystemId, string roleName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.RoleName == roleName).FirstOrDefault());
            }
        }
        public static List<SAPEPRoleInfo> GetByRoleName(long endpointSystemId, string roleName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.RoleName == roleName).ToList());
            }
        }
        public static SAPEPRoleInfo GetByUniqueName(long endpointSystemId, string uniqueroleName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.UniqueRoleName == uniqueroleName).FirstOrDefault());
            }
        }

        public static List<SAPEPRoleInfo> GetAll(long endpointSystemId, bool isXLUploaded)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.IsXLUploaded == isXLUploaded).ToList());
            }
        }

        public static List<SAPEPRoleInfo> GetDeletedRoles(long endpointId, Process process)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (process != null)
                    return ORMapList(new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction)
                        .Get(sap => sap.EndPointSystemId == endpointId && sap.IsXLUploaded == false && ((sap.LastModifiedDate == null && sap.CreatedDate < process.ProcessStartAt) || sap.LastModifiedDate < process.ProcessStartAt)).ToList());
                else
                    return null;
            }
        }

        //private static Process GetProcess(long endpointId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        DiscoveryProcess objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemIdDefault(endpointId);

        //        if (objDiscoveryProcess != null)
        //        {
        //            Process process = Process.GetByProcessId(objDiscoveryProcess.Id);
        //            return process;
        //        }
        //        else return null;
        //    }
        //}

        public static Dictionary<long, string> GetAllNames()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEPRoleInfoRepository(dataAccessTransaction).GetAllNames();
            }
        }

        public static Dictionary<string, string> GetRolesToRemove(long endpointUserID)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<string> saproles = SAPEPUserInfo.GetById(endpointUserID).SAPEPRoles.ToList().Select(r => r.RoleName).ToList();

                return new SAPEPRoleInfoRepository(dataAccessTransaction).GetRolesToRemove(saproles);
            }
        }

        public static Dictionary<string, string> GetRoleSAPEP(long EndpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEPRoleInfoRepository(dataAccessTransaction).Get(role => role.EndPointSystemId == EndpointSystemId && role.IsXLUploaded)
                    .Select(role => new { Key = role.UniqueRoleName, Value = role.RoleName }).OrderBy(item => item.Value).Distinct()
                    .ToDictionary(c => c.Key, c => c.Value);
            }
        }
        public static Dictionary<string, string> GetPortalRoles(long? endpointSystemId, string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEPRoleInfoRepository(dataAccessTransaction).GetPortalRoles(endpointSystemId, CheckNull.AsteriskToString(searchString));
            }
        }

        public static List<SAPEPRoleInfo> GetByEndpointSystem(long endPointSysemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPRoleInfo>(transaction)
                    .Get(entitlement => entitlement.EndPointSystemId == endPointSysemId).ToList());
            }
        }
        public static List<SAPEPRoleInfo> GetAllSAPEPRoles(long endpointSystemId, bool isXLuploaded)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPRoleInfo>(transaction)
                    .Get(role => role.EndPointSystemId == endpointSystemId && role.IsXLUploaded == isXLuploaded).ToList());
            }
        }
        public static bool TruncateTable(long? endpointSystemId)
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPRoleMatrix>(dataAccessTransaction).ExecuteQuery("update DI_SAPEP_RoleInfo set IsXLUploaded=0 where  IsXLUploaded=1 and EndPointSystemId=" + endpointSystemId);
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPEPRoleInfo GetSAPEPRoleObject(long? endpointSystemId, string uniqueRoleName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.UniqueRoleName == uniqueRoleName).FirstOrDefault());
            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                    {
                        //string userDisplayName = User.GetByUserName(lastModifiedDate.CreatedBy)?.DisplayName;
                        //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastCreatedDate.CreatedDate);
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }
        public static bool DoesExist(string entitlement)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction).Any(grp => grp.UniqueRoleName == entitlement);
            }
        }

        public static Dictionary<string, string> GetRolesById(long Id, AdHocActionType actionType, string searchString, long endpointsystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPEPRoleInfoRepository(dataAccessTransaction).GetRolesById(Id, actionType, searchString, endpointsystemId);
            }
        }
        public static SAPEPRoleInfo GetRoleByRoleId(long endpointSystemId, string roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPRoleInfo>(dataAccessTransaction).Get(sap => sap.EndPointSystemId == endpointSystemId && sap.RoleId == roleId).FirstOrDefault());
            }
        }
    }
}

