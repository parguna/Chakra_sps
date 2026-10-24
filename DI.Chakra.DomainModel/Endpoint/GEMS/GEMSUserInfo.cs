using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class GEMSUserInfo : DomainModelBase<GEMSUserInfo, EntityModel.GEMSUserInfo>, IEndpointUserInfo, IEndpointDomainModel
    {
        public GEMSUserInfo()
        {
            this.GEMSRoles = new List<GEMSRoleInfo>();
        }
        public long UserId { get; set; }
        public long EndPointSystemId { get; set; }
        public string USER_NAME { get; set; }
        public string DISPLAY_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string EMAIL { get; set; }
        public Nullable<DateTime> DATEMODIFIED { get; set; }
        public string LASTUSER { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public bool IsDummyEntry { get; set; }
        public List<GEMSRoleInfo> GEMSRoles { get; set; }
        public EndPoint EndPoint { get; set; }

        public string Password;
        public string UserValidFrom;
        public string UserValidTo;
        public string AccountName
        {
            get
            {
                return this.USER_NAME;
            }
        }

        public string AccountPassword
        {
            get { return ""; }
        }

        public string DisplayName
        {
            get
            {
                return this.DISPLAY_NAME;
            }
        }

        public List<string> Roles;
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (GEMSRoles != null)
                {
                    foreach (var item in GEMSRoles)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }
        IEndpointUserInfo IEndpointDomainModel.GetEndpointUserInfo(long EndpointUserId)
        {
            return GetById(EndpointUserId);
        }

        public static List<GEMSUserInfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.GEMSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static GEMSUserInfo GetByUserName(string userName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.GEMSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.USER_NAME == userName).FirstOrDefault());
            }
        }

        public static GEMSUserInfo GetByUserId(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.GEMSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.UserId == userId).FirstOrDefault());
            }
        }


        public static GEMSUserInfo GetByUserIdIncludeRoles(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.GEMSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.Id == userId, roles => roles.GEMSRoles).FirstOrDefault());
            }
        }

        public static void SaveUsers(IList<GEMSUserInfo> list, long EndpointSystemId, string currentUser)
        {
            GEMSUserInfo existingUser = null;
            AccountReconcilation acctRecon = null;
            IList<GEMSUserInfo> listExistingusers = new List<GEMSUserInfo>();

            try
            {
                List<AccountReconcilation> exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(EndpointSystemId).ToList();
                if (exsitngAccountrecon == null)
                {
                    exsitngAccountrecon = new List<AccountReconcilation>();
                }
                listExistingusers = GEMSUserInfo.GetAllByEndpointSystemId(EndpointSystemId);
                var endpoint = EndPoint.GetById(EndpointSystemId);
                var endpointSystemTypeId = endpoint.EndpointSystemTypeId;

                foreach (GEMSUserInfo usr in list)
                {
                    try
                    {
                        existingUser = listExistingusers.Where(t =>
                        t.USER_NAME == usr.USER_NAME && t.UserId == usr.UserId
                        && t.EndPointSystemId == EndpointSystemId).FirstOrDefault();

                        if (existingUser == null)
                        {
                            usr.CreatedBy = currentUser;
                            usr.CreatedDate = DateTime.Now;
                            usr.Save(currentUser);

                            listExistingusers.Add(usr);
                        }
                        else
                        {
                            AccountReconcilation.SaveAttributeException(existingUser, usr, (long)endpointSystemTypeId, null, usr.AccountName, currentUser);
                            usr.Id = existingUser.Id;
                            usr.LastModifiedBy = currentUser;
                            usr.LastModifiedDate = DateTime.Now;
                            usr.Save(currentUser);
                        }

                        try
                        {
                            acctRecon = exsitngAccountrecon.Where(a => a.EndpointSystemId == EndpointSystemId && a.EndpointUserId == usr.Id).FirstOrDefault();

                            bool isExist = false;
                            if (acctRecon == null)
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been added without any request made in the system");
                                acctRecon = new AccountReconcilation();
                                AccountReconcilation.UpdateException(acctRecon, true);
                                acctRecon.AccountType = AccountType.None;
                                acctRecon.EndpointSystemId = EndpointSystemId;
                                acctRecon.CreatedDate = DateTime.Now;
                                acctRecon.CreatedBy = currentUser;
                            }
                            else
                            {
                                isExist = true;
                                if (acctRecon.IsDeleted)
                                {
                                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been added without any request made in the system");
                                    AccountReconcilation.UpdateException(acctRecon, true);
                                }
                            }
                            acctRecon.EndpointUserId = usr.Id;
                            acctRecon.FullName = usr.DisplayName;
                            acctRecon.AccountName = usr.USER_NAME;
                            acctRecon.LastModifiedDate = DateTime.Now;
                            acctRecon.LastModifiedBy = currentUser;
                            acctRecon.Save(currentUser);


                            if (!isExist)
                            {
                                exsitngAccountrecon.Add(acctRecon);
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while GEMS SaveUsers AccountRecon.", ex);
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while GEMS SaveUsers.", ex);
                        throw;
                    }
                }

            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while GEMS SaveUsers.", ex);
                throw;
            }
        }

        public static IList<GEMSUserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<GEMSUserInfo> listDeletedUsers = new List<GEMSUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.GEMSUserInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }

        public static void RemoveGEMSUser(List<GEMSUserInfo> list)
        {
            foreach (GEMSUserInfo user in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new GEMSUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM GEMSUserInfoGEMSRoleInfo WHERE GEMSUserInfo_Id=" + user.Id + " and EndpointSystemId = " + user.EndPointSystemId + ""));
                    dataAccessTransaction.Commit();
                }
                user.Delete();
            }
        }

        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new GEMSUserInfoRepository(dataAccessTransaction).GetByEndPointSystemId(endPointSystemId);
            }
        }

        public static void SaveUserRolesMap(List<GEMSUserRole> userRoles, long endpointSystemid)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new GEMSUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM GEMSUserInfoGEMSRoleInfo WHERE EndpointSystemId={0}", endpointSystemid));
                dataAccessTransaction.Commit();
            }

            List<GEMSUserRole> listUR = new List<GEMSUserRole>();
            GEMSUserInfo userinfo = null;
            GEMSRoleInfo roleinfo = null;
            GEMSUserRole gemusersRole = null;

            foreach (var usrrole in userRoles)
            {
                userinfo = GEMSUserInfo.GetByUserId(usrrole.GEMSUserInfo_Id);
                roleinfo = GEMSRoleInfo.GetByRoleId(usrrole.GEMSRoleInfo_Id);

                if (userinfo != null && roleinfo != null)
                {
                    gemusersRole = new GEMSUserRole();
                    gemusersRole.GEMSRoleInfo_Id = roleinfo.Id;
                    gemusersRole.GEMSUserInfo_Id = userinfo.Id;
                    gemusersRole.EndpointSystemId = endpointSystemid;

                    listUR.Add(gemusersRole);
                }
            }

            SQLBulkUploader<GEMSUserRole> sqlUserGroupMap = null;
            try
            {
                string tableName = "GEMSUserInfoGEMSRoleInfo";
                sqlUserGroupMap = new SQLBulkUploader<GEMSUserRole>(tableName);
                sqlUserGroupMap.Flush(listUR);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "", ex);
                throw ex;
            }
            finally
            {
                sqlUserGroupMap.Close();
            }
        }

        public static void SaveUserRoles(GEMSUserInfo userInfo)
        {
            var userwithRoles = GEMSUserInfo.GetByIdIncludeRoles(userInfo.Id);
            List<GEMSUserRole> userRole = new List<GEMSUserRole>();

            if (userwithRoles != null)
            {
                GEMSUserRole tempUsrRole = new GEMSUserRole();
                foreach (var role in userInfo.Roles)
                {
                    tempUsrRole = new GEMSUserRole();
                    GEMSRoleInfo roleinfo = GEMSRoleInfo.GetByRoleName(role);
                    if (!userwithRoles.GEMSRoles.Any(g => g.Id == roleinfo.Id))
                    {
                        tempUsrRole.GEMSUserInfo_Id = userInfo.Id;
                        tempUsrRole.GEMSRoleInfo_Id = roleinfo.Id;
                        tempUsrRole.EndpointSystemId = userInfo.EndPointSystemId;
                        userRole.Add(tempUsrRole);
                    }
                }
                userRole = userRole.Distinct().ToList();
                if (userRole.Count() > 0)
                {
                    SQLBulkUploader<GEMSUserRole> sqlUserGroupMap = null;
                    try
                    {
                        string tableName = "GEMSUserInfoGEMSRoleInfo";
                        sqlUserGroupMap = new SQLBulkUploader<GEMSUserRole>(tableName);
                        sqlUserGroupMap.Flush(userRole);
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "", ex);
                        throw ex;
                    }
                    finally
                    {
                        sqlUserGroupMap.Close();
                    }
                }
            }
        }

        public static GEMSUserInfo GetByIdIncludeRoles(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.GEMSUserInfo>(tranaction)
                      .Get(gemsUser => gemsUser.Id == id, gemsUser => gemsUser.GEMSRoles).FirstOrDefault());
            }
        }

        public static List<GEMSUserInfo> GetAllEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.GEMSUserInfo>(dataAccessTransaction)
                       .Get(gemsUser => gemsUser.EndPointSystemId == endPointSystemId, gemsUser => gemsUser.GEMSRoles).ToList());
            }
        }

        private static string GetJoinQry(long systemId, bool mapall)
        {
            string clauseType = "OR";
            if (mapall)
            {
                clauseType = "AND";
            }
            StringBuilder bldr = new StringBuilder();
            //TO DO: Get Attribute Mapping Columns from DI_M_AttributeMapping

            IList<AttributeMappingColumns> listAttributes = AttributeMappingColumns.GetByEndpointSystemId(systemId);
            foreach (AttributeMappingColumns attDto in listAttributes)
            {
                bldr.AppendFormat(string.Format("(DI_GEMS_UserInfo.{0}=DI_M_User.{1}  and DI_GEMS_UserInfo.{2} != '' and DI_GEMS_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));
            return string.Empty;
        }

        public static void MapWithMasterUserForReconTable(EndPoint endPoint, bool mapall, long discoveryProcessId)
        {
            try
            {
                var considerInActiveAccountForRecon = AccountReconcilation.GetCanConsiderInActiveFilterCondition();;
                StringBuilder bldR = new StringBuilder();
                string strJoin = GetJoinQry(endPoint.Id, mapall);
                if (strJoin == string.Empty)
                    return;
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_GEMS_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_GEMS_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_GEMS_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_User.Id <> 1 AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString(), considerInActiveAccountForRecon);
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new GEMSUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
                    dataAccessTransaction.Commit();
                }
            }
            catch (DbException ex)
            {
                if (ex.ErrorCode == -2146232060)
                {
                    DILog.DiscoveryLogger.WriteLog(discoveryProcessId, LogLevelType.Exception, string.Format("Error converting data type nvarchar to bigint. Invalid attribute mapping.This exception occurs when invalid attribute mapping is given while creating the endpoint."));
                    return;
                }
                else
                {
                    DILog.DiscoveryLogger.WriteLog(discoveryProcessId, LogLevelType.Exception, ex.Message);
                    throw;
                }
            }
            catch (Exception e)
            {
                DILog.DiscoveryLogger.WriteLog(discoveryProcessId, LogLevelType.Exception, e.Message);
                throw;
            }
        }
        public static void MapWithMasterUserForEntitlementReconTable(EndPoint endPoint, bool mapall, long discoveryProcessId)
        {
            try
            {
                StringBuilder bldR = new StringBuilder();
                string strJoin = GetJoinQry(endPoint.Id, mapall);
                if (strJoin == string.Empty)
                    return;
                bldR.Append("Update DI_M_EntitlementReconcilation set DI_M_EntitlementReconcilation.UserId = DI_M_User.Id FROM DI_GEMS_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_EntitlementReconcilation on DI_M_EntitlementReconcilation.EndpointUserId = DI_GEMS_UserInfo.Id");
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.Id = DI_M_EntitlementReconcilation.AccountReconcilationId");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_GEMS_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND DI_M_EntitlementReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString());
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new GEMSUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
                    dataAccessTransaction.Commit();
                }
            }
            catch (DbException ex)
            {
                if (ex.ErrorCode == -2146232060)
                {
                    DILog.DiscoveryLogger.WriteLog(discoveryProcessId, LogLevelType.Exception, string.Format("Error converting data type nvarchar to bigint. Invalid attribute mapping.This exception occurs when invalid attribute mapping is given while creating the endpoint."));
                    return;
                }
                else
                {
                    DILog.DiscoveryLogger.WriteLog(discoveryProcessId, LogLevelType.Exception, ex.Message);
                    throw;
                }
            }
            catch (Exception e)
            {
                DILog.DiscoveryLogger.WriteLog(discoveryProcessId, LogLevelType.Exception, e.Message);
                throw;
            }
        }

        public static GEMSUserInfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new GEMSUserInfoRepository(transaction).GetLockedAccountByName(accountName, diuserId, endpointSystemId, accountType));
            }
        }

        public static long GetMaxUserId()
        {
            using (var transaction = new DataAccessTransaction())
            {
                return (new GEMSUserInfoRepository(transaction).GetMaxUserId());
            }
        }

        public static List<GEMSUserInfo> GetActiveAccounts(long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new GEMSUserInfoRepository(transaction).GetActiveAccounts(diuserId, endpointSystemId, accountType));
            }
        }

        public static bool DeleteGEMSUserRoleMap(GEMSUserInfo userinfo, string gemsRoles)
        {

            foreach (var role in userinfo.Roles)
            {
                using (var transaction = new DataAccessTransaction())
                {
                    GEMSRoleInfo roleinfo = GEMSRoleInfo.GetByRoleName(role);
                    if (roleinfo != null)
                    {
                        new GEMSUserInfoRepository(transaction).DeleteRole(userinfo.Id, roleinfo.Id, userinfo.EndPointSystemId);
                    }
                    transaction.Commit();
                }               
            }

            return true;
        }

        public static GEMSUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new GEMSUserInfoRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }

        public static Dictionary<long, string> GetGEMSUsersByDIUserId(long userid, int endpointsystemid, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return new GEMSUserInfoRepository(transaction).GetGEMSUsersByDIUserId(userid, endpointsystemid, accountType);
            }
        }
        public static GEMSUserInfo GetByUserByEndpointSystemAndId(long userId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.GEMSUserInfo>(transaction)
              .Get(grp => grp.Id == userId && grp.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
                
        }
        
    }
}
