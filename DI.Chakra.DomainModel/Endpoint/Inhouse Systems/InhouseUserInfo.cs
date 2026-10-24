using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.CustomObjects;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DI.Chakra.DomainModel
{
    public class InhouseUserInfo : DomainModelBase<InhouseUserInfo, EntityModel.InhouseUserInfo>, IEndpointUserInfo, IXLData
    {
        public long UserId { get; set; }

        public long EndPointSystemId { get; set; }

        public string InhouseSystemCode { get; set; }

        public string NRIC { get; set; }
        public string IdentificationType { get; set; }
        public string IdentificationCountry { get; set; }
        public AccountType AccountType { get; set; }
        public string IsMapped { get; set; }
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? AccountCreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LockedDate { get; set; }
        public string LockScenario { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDisabled { get; set; }

        public DateTime? DisabledDate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string Remarks1 { get; set; }

        public string Remarks2 { get; set; }

        public string Remarks3 { get; set; }

        public string Remarks4 { get; set; }

        public string Remarks5 { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<bool> AddedByDiscovery { get; set; }
        public string AccountName
        {
            get { return UserName; }
        }

        public List<InhousePermission> Permissions { get; set; }
        public string Permission { get; set; }
        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }

        public EndPoint EndPoint { get; set; }

        public string EmployeeId { get; set; }


        // public Nullable<long> WogUserId { get; set; }
        public string AccountPassword
        {
            get
            {
                return RSAHelper.DecryptData(Password);
            }
        }

        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (Permissions != null)
                {
                    foreach (var item in Permissions)
                        li.Add(item);
                }
                return li;
            }
        }

        public static string CheckUserNames(string userNames, long endPointSystemId)
        {
            DataSet ds = null;
            string response = string.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();

            string query =
                    @"SELECT Item Result FROM (
                    SELECT A.Item, CASE WHEN B.Id IS NOT NULL THEN 'X' ELSE '' END Message FROM dbo.Split(@usernames,',') A LEFT JOIN DI_INH_UserInfo B ON A.Item = B.UserName WHERE B.EndPointSystemId=@endpointId AND B.IsDeleted=0 AND B.IsLocked=0) Res
                    WHERE Res.Message = 'X'";

            SqlParameter sqlParamUserNames = new SqlParameter();
            sqlParamUserNames.ParameterName = "@usernames";
            sqlParamUserNames.SqlDbType = SqlDbType.VarChar;
            sqlParamUserNames.Value = userNames;

            SqlParameter sqlParamEndpointId = new SqlParameter();
            sqlParamEndpointId.ParameterName = "@endpointId";
            sqlParamEndpointId.SqlDbType = SqlDbType.BigInt;
            sqlParamEndpointId.Value = endPointSystemId;

            parameters.Add(sqlParamUserNames);
            parameters.Add(sqlParamEndpointId);

            IACMDbOperator m_iacmOperator = new IACMDbOperator(query, CommandType.Text, parameters);
            QueryExecutor m_executor = new QueryExecutor(m_iacmOperator);
            ds = m_executor.ExecuteQueryDS();

            if (ds != null & ds.Tables[0] != null)
            {
                response = string.Join(",", ds.Tables[0].AsEnumerable().Select(s => s.Field<string>("Result")).ToArray());
            }

            return response;
        }

        public static bool PopulateUserInfos(List<InhouseUserCustomInfo> inhCustomUserInfos)
        {
            bool response = false;

            try
            {
                foreach (var userInfo in inhCustomUserInfos)
                {
                    CreateINHUser(userInfo);
                }
                response = true;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, "Exception occured while populating inhouse user infos", ex);
            }

            return response;
        }

        private static long CreateINHUser(InhouseUserCustomInfo inhCustomUserInfo)
        {
            var inhUser = GetByUserName(inhCustomUserInfo.EndpointSystemId, inhCustomUserInfo.Username);
            var globalUser = User.GetById(inhCustomUserInfo.UserId);
            var endpoint = EndPoint.GetById(inhCustomUserInfo.EndpointSystemId);

            inhUser = GetByEmployeeId(inhCustomUserInfo.EndpointSystemId, globalUser.EmployeeId);

            if (inhUser == null)
                inhUser = new InhouseUserInfo();

            inhUser.EndPointSystemId = inhCustomUserInfo.EndpointSystemId;
            inhUser.InhouseSystemCode = endpoint.ApplicationCode;
            inhUser.EmployeeId = globalUser.EmployeeId;
            inhUser.UserName = inhCustomUserInfo.Username;
            inhUser.DisplayName = globalUser.DisplayName;
            inhUser.FirstName = globalUser.FirstName;
            inhUser.LastName = globalUser.LastName;
            inhUser.AccountType = AccountType.User;
            inhUser.CreatedBy = inhCustomUserInfo.CreatedBy;
            inhUser.CreatedDate = DateTime.Now;
            inhUser.IsLocked = false;
            inhUser.Password = RSAHelper.EncryptData(inhCustomUserInfo.Password);
            inhUser.AccountCreatedDate = DateTime.Now;
            inhUser.IsDisabled = false;
            inhUser.IsDeleted = false;
            inhUser.DeletedDate = null;
            inhUser.DisabledDate = null;
            inhUser.Email = globalUser.EmailId;
            return inhUser.Save("System");
        }

        public string FullName
        {
            get { return DisplayName; }
            // set { DisplayName = value; }
        }
        public static List<InhouseUserInfo> GetByDIUserId(long diUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetByDIUserId(diUserId));
            }
        }

        public static List<InhouseUserInfo> GetByUserName(string userName, string inhousesystemcode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
                    Get(ihUserinfo =>
                        ihUserinfo.UserName == userName &&
                        ihUserinfo.InhouseSystemCode == inhousesystemcode,
                        ihUserinfo => ihUserinfo.Permissions
                        ).
                        ToList());
            }
        }

        public static InhouseUserInfo GetByIdWithNavigation(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).GetById(id, inhUserInfo => inhUserInfo.EndPoint, inhUserInfo => inhUserInfo.EndPoint.EndPointSystemType, inhUserInfo => inhUserInfo.Permissions));
            }
        }

        public static List<InhouseUserInfo> GetByDIUserIdIncludePermissions(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetByDIUserIdIncludePermissions(id));
            }
        }

        public static List<InhouseUserInfo> GetUnlockedAccountsByDIUserId(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetUnlockedAccountsByDIUserId(userId));
            }
        }

        public static List<InhouseUserInfo> GetActiveAccountsByDIUserId(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetActiveAccountsByDIUserId(userId));
            }
        }

        public static List<InhouseUserInfo> GetByDIUserId(long userId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetByDIUserId(userId, endpointSystemId));
            }
        }

        public static List<InhouseUserInfo> GetByUserIdAndAccountType(long userId, long endpointSystemId, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetByUserIdAndAccountType(userId, endpointSystemId, accountType));
            }
        }

        public static InhouseUserInfo GetByDIUserIdAndEndPointSystemId(long diUserId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new InhouseUserInfoRepository(dataAccessTransaction).GetByDIUserIdAndEndPointSystemId(diUserId, endpointSystemId));
            }
        }

        public static List<InhouseUserInfo> GetByEndpointSystemId(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
                                    Get(ihUserinfo => ihUserinfo.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static void DeleteByUserid(long userId, string currentUser)
        {
                var userInfo = GetById(userId);
                if (userInfo != null)
                {
                    string cmd = string.Format("DELETE FROM [DI_INH_UserPermissionFunction] WHERE EndpointUserId = {0} ", userInfo.Id);
                    ExecuteQuery(cmd);

                    Delete(userInfo.Id);
                    AccountReconcilation recon = AccountReconcilation.GetByUser(userInfo.EndPointSystemId, userId, false);
                    if (recon != null)
                        AccountReconcilation.DeleteAccount(recon.EndpointSystemId, recon.EndpointUserId, currentUser, null);
                }
        }

        public static IList<InhouseUserInfo> GetDeletedByEndPointSystemId(long EndpointId, DateTime runDatetime, string currentUser)
        {
                List<InhouseUserInfo> listDeletedUserInfo = new List<InhouseUserInfo>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                listDeletedUserInfo = ORMapList(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId && (objUser.LastModifiedDate < runDatetime
                       || (objUser.CreatedDate < runDatetime && objUser.LastModifiedDate == null))).ToList());

                return listDeletedUserInfo;
            }
                
        }

        public static InhouseUserInfo GetByNRIC(string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
                                   Get(ihUserinfo => ihUserinfo.NRIC == value).FirstOrDefault());
            }
        }

        public static InhouseUserInfo GetByNRIC(long endpointSystemId, string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
                    Get(ihUserinfo => ihUserinfo.NRIC == value && ihUserinfo.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
        }

        public static InhouseUserInfo GetByUserNameAndEndPointSystemId(string userName, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
                    Get(ihUserinfo => ihUserinfo.UserName == userName && ihUserinfo.EndPointSystemId == endPointSystemId,
                    ihUserinfo => ihUserinfo.Permissions).FirstOrDefault());
            }
                
        }

        public static InhouseUserInfo GetBySimilarObject(InhouseUserInfo obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                EntityModel.InhouseUserInfo inhUserInfo=null;
                if (!string.IsNullOrEmpty(obj.UserName))
                {
                    inhUserInfo = new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).Get(ihUserinfo => ihUserinfo.InhouseSystemCode == obj.InhouseSystemCode && ihUserinfo.UserName == obj.UserName)?.FirstOrDefault();
                }
                else if (!string.IsNullOrEmpty(obj.Email))
                {
                    inhUserInfo = new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
                    Get(ihUserinfo => ihUserinfo.InhouseSystemCode == obj.InhouseSystemCode && ihUserinfo.Email == obj.Email)?.FirstOrDefault();
                }
                else if (!string.IsNullOrEmpty(obj.EmployeeId))
                {
                    inhUserInfo = new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
                    Get(ihUserinfo => ihUserinfo.InhouseSystemCode == obj.InhouseSystemCode && ihUserinfo.EmployeeId == obj.EmployeeId)?.FirstOrDefault();
                }
                else if (!string.IsNullOrEmpty(obj.NRIC) && !string.IsNullOrEmpty(obj.IdentificationType) && !string.IsNullOrEmpty(obj.IdentificationCountry))
                {
                    inhUserInfo = new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
                    Get(ihUserinfo => ihUserinfo.InhouseSystemCode == obj.InhouseSystemCode && ihUserinfo.NRIC == obj.NRIC && ihUserinfo.IdentificationType == obj.IdentificationType && ihUserinfo.IdentificationCountry == obj.IdentificationCountry)?.FirstOrDefault();
                }
                else
                {
                    inhUserInfo = null;
                }
                return ORMap(inhUserInfo);
            }
        }

        // Remove during end to end implementation if not required
        public static InhouseUserInfo GetByEndPointSystemId(long id, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseUserInfo>(transaction)
                    .Get(adsInfo => adsInfo.Id == id && adsInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        //public static List<InhouseUserInfo> GetActiveAccountsIncludingEndpoint(long endPointsystemid)
        //{
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return ORMapList(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
        //            Get(ihUserinfo => ihUserinfo.DIUserId == endPointsystemid &&
        //                ihUserinfo.IsLocked == false &&
        //                ihUserinfo.IsDeleted == false,
        //                ihuserinfo => ihuserinfo.EndPoint,
        //                ihuserinfo => ihuserinfo.EndPoint.EndPointSystemType).
        //                ToList());
        //    }
        //}

        public static List<InhouseUserInfo> GetByIdIncludeEndPoint(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction)
                                         .Get(inh => inh.Id == userId, inh => inh.EndPoint, inh => inh.Permissions).ToList());
            }
        }

        public static bool EndpointHasAnyRoleAndAssignedToUser(long endpointId)
        {
                var isPermissionAvailable = InhousePermission.GetByEndPointSystemId(endpointId);
                if (isPermissionAvailable == null || isPermissionAvailable.Count == 0) return false;
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    var userinfo = ORMapList(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction)
                                             .Get(inh => inh.EndPointSystemId == endpointId, inh => inh.Permissions).ToList());

                    if (userinfo == null || userinfo.Count == 0) return false;

                    return (userinfo.Any(a => a.Permissions.Count > 0));
                }
                
        }

        public static InhouseUserInfo GetByIdEndPointIncludingOwner(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
                       Get(inh => inh.Id == id, inh => inh.EndPoint, inh => inh.Permissions).FirstOrDefault());
            }
                
        }

        public static List<InhouseUserInfo> GetByIdIncludeEndPointWithOwner(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction)
                                         .Get(inh => inh.Id == userId, inh => inh.EndPoint, inh => inh.EndPoint.Owner, inh => inh.Permissions).ToList());
            }

        }

        public static void MapUser(EndPoint endPoint)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                MapWithMasterUser(endPoint, dataAccessTransaction);
                //MapWithMasterUserForReconTable(endPoint.Id, dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }
        public static void MapWithMasterUser(EndPoint endPoint, DataAccessTransaction dataAccessTransaction)
        {

            //StringBuilder bldR = new StringBuilder();
            //string strJoin = GetJoinQry(endPoint.Id);
            //if (strJoin == string.Empty)
            //    return;
            //bldR.Append("Update DI_INH_UserInfo set DI_INH_UserInfo.DIUserId = DI_M_User.Id FROM DI_INH_UserInfo JOIN DI_M_User on");
            //bldR.Append(" ");
            //bldR.Append(strJoin);
            //bldR.Append(" ");
            //bldR.AppendFormat("WHERE DI_INH_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 ", endPoint.Id.ToString());
            //new InhouseSystemsRepository(dataAccessTransaction).ExecuteQry(bldR.ToString());                       
        }

        public static void MapWithMasterUser(long endPointSystemID)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                //string strQry = string.Format("Update DI_INH_UserInfo Set DIUserId=NULL where EndpointSystemId={0} ", endPointSystemID);
                //new InhouseSystemsRepository(dataAccessTransaction).ExecuteQry(strQry);
                StringBuilder bldR = new StringBuilder();
                //string strJoin = GetJoinQry(endPointSystemID);
                //if (strJoin == string.Empty)
                //    return;

                //bldR.Append("Update DI_INH_UserInfo set DI_INH_UserInfo.DIUserId = DI_M_User.Id FROM DI_INH_UserInfo JOIN DI_M_User on");
                //bldR.Append(" ");
                ////bldR.Append(strJoin);
                //bldR.Append(" ");
                //bldR.AppendFormat("WHERE DI_INH_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1", endPointSystemID);
                //new InhouseSystemsRepository(dataAccessTransaction).ExecuteQry(bldR.ToString());
                //dataAccessTransaction.Commit();
                return;
            }
        }

        public static void MapWithMasterUserForReconTable(long endPointSystemID, bool mapall, bool canConsiderForInactiveAccount, DataAccessTransaction dataAccessTransaction=null)
        {
            try
            {
                var considerInActiveAccountForRecon = (canConsiderForInactiveAccount == false ? " DI_M_User.IsActive  = 1 " : " 1 = 1 ");
                StringBuilder bldR = new StringBuilder();
                string strJoin = GetJoinQry(endPointSystemID, mapall);
                if (strJoin == string.Empty)
                    return;

                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1 FROM DI_INH_UserInfo JOIN DI_M_User on ");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_INH_UserInfo.Id  ");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_INH_UserInfo.EndPointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_User.Id <> 1 AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2 AND DI_M_AccountReconcilation.EndpointSystemId = {0}", endPointSystemID.ToString(), considerInActiveAccountForRecon);
                using (dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).ExecuteQuery(bldR.ToString());
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(0, LogLevelType.Exception, ex.Message);
            }
        }

        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    // string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
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

        public static void MapWithMasterUserForEntitlementReconTable(long endPointSystemID, bool mapall, DataAccessTransaction dataAccessTransaction)
        {
            try
            {
                StringBuilder bldR = new StringBuilder();
                string strJoin = GetJoinQry(endPointSystemID, mapall);
                if (strJoin == string.Empty)
                    return;
                bldR.Append("Update DI_M_EntitlementReconcilation set DI_M_EntitlementReconcilation.UserId = DI_M_User.Id FROM DI_INH_UserInfo JOIN DI_M_User on ");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_EntitlementReconcilation on DI_M_EntitlementReconcilation.EndpointUserId = DI_INH_UserInfo.Id  ");
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.Id = DI_M_EntitlementReconcilation.AccountReconcilationId  ");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_INH_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND DI_M_EntitlementReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPointSystemID.ToString());
                using (dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).ExecuteQuery(bldR.ToString());
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(0, LogLevelType.Exception, ex.Message);
            }
        }

        public static string GetJoinQry(long endPointId, bool mapall)
        {
            string clauseType = "AND";
            if (!mapall)
            {
                clauseType = "OR";
            }

            StringBuilder bldr = new StringBuilder();
            //TO DO: Get Attribute Mapping Columns from DI_M_AttributeMapping

            IList<AttributeMappingColumns> listAttributes = AttributeMappingColumns.GetByEndPoindId(endPointId);
            foreach (AttributeMappingColumns attDto in listAttributes)
            {
                bldr.AppendFormat(string.Format(
                "(DI_INH_UserInfo.{0}=DI_M_User.{1}  and DI_INH_UserInfo.{2} != ''and DI_INH_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ",
                attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));

            return string.Empty;
        }

        public static void TruncateDI_INH_UserInfo()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                InhouseUserInfo.DeleteAll();
            }
        }

        public static InhouseUserInfo GetByEndpoint(InhouseUserInfo obj)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new InhouseUserInfoRepository(dataAccessTransaction).GetByEndpoint(obj.ORUnMap()));
            }
        }

        public static bool DoesExists(long endPointSystemId, string userName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).Any(inhUser => inhUser.UserName == userName && inhUser.EndPointSystemId == endPointSystemId);
            }
        }
        public static InhouseUserInfo GetByUserName(long endPointSystemId, string userName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).Get(inhUser => inhUser.UserName == userName && inhUser.EndPointSystemId == endPointSystemId, inhUser => inhUser.EndPoint, inhUser => inhUser.Permissions).FirstOrDefault());
            }
        }

        public static InhouseUserInfo GetByEmployeeId(long endPointSystemId, string employeeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).Get(inhUser => inhUser.EmployeeId == employeeId && inhUser.EndPointSystemId == endPointSystemId, inhUser => inhUser.EndPoint, inhUser => inhUser.Permissions).FirstOrDefault());
            }
        }

        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new InhouseUserInfoRepository(transaction).Insert(ORUnMap());
                }
                else
                {
                    new InhouseUserInfoRepository(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return this.Id;
        }

        public static InhouseUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new InhouseUserInfoRepository(dataAccessTransaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }

        public static InhouseUserInfo GetByUsernameOrEmployeeIdOrNRIC(string username, string employeeId, string applicationCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).
                   Get(ihUserinfo =>
                       (ihUserinfo.UserName == username || ihUserinfo.EmployeeId == employeeId) &&
                       ihUserinfo.InhouseSystemCode == applicationCode
                       ).FirstOrDefault());
            }
        }

        public static bool DeleteByAccReconId(long accReconId, string currentUser)
        {
            try
            {
                var accountRecon = AccountReconcilation.GetById(accReconId);
                if (accountRecon == null)
                    return false;

                // currentUser is attacker-influenced, so it is bound as a real SqlParameter
                // ("{0}" is an EF token, not a string.Format/interpolation substitution) instead
                // of being concatenated into the SQL text. The numeric ids interpolated below
                // come from the accountRecon entity loaded above (long-typed columns), not from
                // raw request text, so they carry no injection risk.
                StringBuilder query = new StringBuilder();
                query.AppendLine($"DELETE FROM DI_INH_UserPermissionFunction WHERE EndpointUserId={accountRecon.EndpointUserId} AND EndPointSystemId={accountRecon.EndpointSystemId};");
                query.AppendLine($"UPDATE DI_M_EntitlementReconcilation SET IsDeleted=1,DeletedDate=GETDATE(), LastModifiedBy={{0}} , LastModifiedDate=GETDATE() WHERE AccountReconcilationId={accountRecon.Id}");
                query.AppendLine($"UPDATE DI_INH_UserInfo SET IsLocked=1,IsDeleted=1,IsDisabled =1, DeletedDate=GETDATE(),LastModifiedBy={{0}}, LastModifiedDate=GETDATE() WHERE ID ={accountRecon.EndpointUserId} AND EndPointSystemId={accountRecon.EndpointSystemId}");
                query.AppendLine($"UPDATE DI_M_AccountReconcilation SET EndpointUserId=0,IsDeleted=1,IsLocked=1,DeletedDate=GETDATE(), LastModifiedDate=GETDATE(), LastModifiedBy={{0}} where id ={accountRecon.Id}");

                ExecuteQuery(query.ToString(), currentUser);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<InhouseUserInfo> GetAllAccountsByEmployeeId(long endPointSystemId, string employeeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).Get(inhUser => inhUser.EmployeeId == employeeId && inhUser.EndPointSystemId == endPointSystemId, inhUser => inhUser.EndPoint, inhUser => inhUser.Permissions).ToList());
            }
        }
        public static InhouseUserInfo GetAccountsByEmployeeIdAccountType(long endPointSystemId, string employeeId, AccountType accountType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhouseUserInfo>(dataAccessTransaction).Get(inhUser => inhUser.AccountType == accountType &&  inhUser.EmployeeId == employeeId && inhUser.EndPointSystemId == endPointSystemId, inhUser => inhUser.EndPoint, inhUser => inhUser.Permissions).FirstOrDefault());
            }
        }
        public static List<PresentationOtherSystemUser> GetOtherSystemDataNoADGroups(string accountName, long endPointSystemId, long adEndPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var pList = new List<PresentationOtherSystemUser>();
                var data = new InhouseUserInfoRepository(dataAccessTransaction).GetOtherSystemDataNoADGroups(accountName, endPointSystemId, adEndPointSystemId);
                foreach (var item in data)
                {
                    pList.Add(new PresentationOtherSystemUser(item.AccountReconcilationId, item.UserName, item.Permission, item.EndpointUserId, item.DisplayName, item.AccountType));
                }
                return pList;
            }
        }
        /// <summary>
        /// Used to get the user not found in Other system and present in AD and Group match with Other system permission
        /// </summary>
        /// <param name="endPointSystemId"></param>
        /// <param name="employeeId"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public static List<PresentationOtherSystemData> GetPresentationOtherSystemDatas(long endPointSystemId, long adEndPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var pList = new List<PresentationOtherSystemData>();
                var data = new InhouseUserInfoRepository(dataAccessTransaction).GetPresentationOtherSystemDatas(endPointSystemId, adEndPointSystemId);
                foreach (var item in data)
                {
                    pList.Add(new PresentationOtherSystemData(item.PermissionId, item.Permission, item.FullName, item.AccountType, item.MappingType, item.Entitlement, item.EmployeeId, item.UserId, item.AccountName, item.FirstName, item.LastName, item.Description));
                }
                return pList;
            }
        }
        public static List<PresentationOtherSystemUser> GetUserWithNoPermission(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var pList = new List<PresentationOtherSystemUser>();
                var data = new InhouseUserInfoRepository(dataAccessTransaction).GetOtherSystemDataNoADGroups(endPointSystemId);
                foreach (var item in data)
                {
                    pList.Add(new PresentationOtherSystemUser(item.AccountReconcilationId, item.UserName, item.Permission, item.EndpointUserId, item.DisplayName, item.AccountType));
                }
                return pList;
            }
        }
        public static List<PresentationOtherSystemUser> GetInhouseUsersWithoutADAccount(long endPointSystemId, long adEndPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var pList = new List<PresentationOtherSystemUser>();

                var data = new InhouseUserInfoRepository(dataAccessTransaction).GetInhouseUsersWithoutADAccount(endPointSystemId, adEndPointSystemId).ToList();
                foreach (var item in data)
                {
                    pList.Add(new PresentationOtherSystemUser(item.AccountReconcilationId, item.UserName, item.Permission, item.EndpointUserId, item.DisplayName, item.AccountType));
                }
                return pList;
            }
        }
        public static List<PresentationOtherSystemData> GetOtherSystemDataGetMissingPermission(string accountName, long endPointSystemId, long adEndPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var pList = new List<PresentationOtherSystemData>();
                var data = new InhouseUserInfoRepository(dataAccessTransaction).GetOtherSystemDataGetMissingPermission(accountName, endPointSystemId, adEndPointSystemId);
                foreach (var item in data)
                {
                    pList.Add(new PresentationOtherSystemData(item.PermissionId, item.Permission, item.FullName, item.AccountType, item.MappingType, item.Entitlement, item.EmployeeId, item.UserId, item.AccountName, item.FirstName, item.LastName, item.Description));
                }
                return pList;
            }
        }
        /// <summary>
        /// Used to update the accounttype,mapping type and offboarding action type
        /// </summary>
        /// <param name="endPointSystemId"></param>
        /// <param name="adEndPointSystemId"></param>
        public static void UpdateADMappingAccountType(long endPointSystemId, long adEndPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                new InhouseUserInfoRepository(dataAccessTransaction).UpdateADMappingAccountType(endPointSystemId, adEndPointSystemId);
            }
        }
        public static void UpdateINHReconEntitlementDescription(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                new InhouseUserInfoRepository(dataAccessTransaction).UpdateINHReconEntitlementDescription(endPointSystemId);
            }
        }
        public static void UpdateAllINHReconEntitlementDescription()
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                new InhouseUserInfoRepository(dataAccessTransaction).UpdateAllINHReconEntitlementDescription();
            }
        }
    }
}
