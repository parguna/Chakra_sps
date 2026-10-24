using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DI.Chakra.DomainModel
{
    public class VPNUserInfo : DomainModelBase<VPNUserInfo, EntityModel.VPNUserInfo>, IEndpointUserInfo, IEndpointDomainModel
    {
        public VPNUserInfo()
        {

        }
        public string OneKeyTokenId { get; set; }
        public string UserName { get; set; }
        public long UserId { get; set; }
        public string DisplayName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public EndPoint EndPointSystem { get; set; }
        public string AccountName
        {
            get
            {
                return this.UserName;
            }
        }
        public List<IEndpointEntitlementInfo> Entitlements { get; set; }
        public long EndPointSystemId { get; set; }

        public string AccountPassword { get; set; }

        public IEndpointUserInfo GetEndpointUserInfo(long EndpointUserId)
        {
            return GetById(EndpointUserId);
        }

        public static void AddUpdateAccountRecon(IList<VPNUserInfo> userlist, long EndpointSystemId, string currentUser)
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Add/Update AccountReconcilation started");
            VPNUserInfo existingUser = null;
            AccountReconcilation acctRecon = null;
            var listExistingusers = new List<VPNUserInfo>();

            try
            {
                List<AccountReconcilation> exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(EndpointSystemId).ToList();
                if (exsitngAccountrecon == null)
                {
                    exsitngAccountrecon = new List<AccountReconcilation>();
                }
                var endpoint = EndPoint.GetById(EndpointSystemId);
                var endpointSystemTypeId = endpoint.EndpointSystemTypeId;

                foreach (var usr in userlist)
                {
                    try
                    {
                        acctRecon = exsitngAccountrecon.Where(a => a.EndpointSystemId == EndpointSystemId && !string.IsNullOrWhiteSpace(a.AccountName) && a.AccountName.Equals(usr.OneKeyTokenId,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                        bool isExist = false;
                        if (acctRecon == null)
                        {
                            acctRecon = new AccountReconcilation();
                            AccountReconcilation.UpdateException(acctRecon, true);
                            acctRecon.AccountType = AccountType.User;
                            acctRecon.EndpointSystemId = EndpointSystemId;
                            acctRecon.CreatedDate = DateTime.Now;
                            acctRecon.CreatedBy = currentUser;
                        }
                        else
                        {

                            isExist = true;
                            if (acctRecon.IsDeleted)
                            {
                                AccountReconcilation.UpdateException(acctRecon, true);
                            }
                        }
                        acctRecon.EndpointUserId = usr.Id;
                        acctRecon.UserId = usr.UserId;
                        acctRecon.FullName = usr.DisplayName;
                        acctRecon.AccountName = usr.OneKeyTokenId;
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
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while Saving AccountReconcilation.", ex);
                        throw;
                    }
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Add/Update AccountReconcilation completed");
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while saving AccountReconcilation.", ex);
                throw;
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
                bldr.AppendFormat(string.Format("(DI_VPN_UserInfo.{0}=DI_M_User.{1}  and DI_VPN_UserInfo.{2} != '' and DI_VPN_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));
            return string.Empty;
        }
        public static void MapWithMasterUserForReconTable(EndPoint endPoint, bool mapall, long discoveryProcessId)
        {
            try
            {
                var considerInActiveAccountForRecon = AccountReconcilation.GetCanConsiderInActiveFilterCondition();
                StringBuilder bldR = new StringBuilder();
                string strJoin = GetJoinQry(endPoint.Id, mapall);
                if (strJoin == string.Empty)
                    return;
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_VPN_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_VPN_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_VPN_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_User.Id <> 1 AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString(), considerInActiveAccountForRecon);
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
                bldR.Append("Update DI_M_EntitlementReconcilation set DI_M_EntitlementReconcilation.UserId = DI_M_User.Id FROM DI_VPN_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_EntitlementReconcilation on DI_M_EntitlementReconcilation.EndpointUserId = DI_VPN_UserInfo.Id");
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.Id = DI_M_EntitlementReconcilation.AccountReconcilationId");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_VPN_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND DI_M_EntitlementReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString());
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
        public static List<EntityModel.PresentationVPNUserInfo> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new VPNUserInfoRepository(dataAccessTransaction).GetByEndPointSystemId(endPointSystemId);
            }
        }



        public static List<VPNUserInfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.VPNUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }
        public static VPNUserInfo GetByOnekeyTokenId(string oneKeyTokenId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.VPNUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.OneKeyTokenId.ToString() == oneKeyTokenId).FirstOrDefault());
            }
        }

        public static IList<VPNUserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<VPNUserInfo> listDeletedUsers = new List<VPNUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.VPNUserInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }
        public static void RemoveUser(List<VPNUserInfo> list)
        {
            foreach (var user in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.VPNUserInfo>(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM DI_VPN_UserInfo WHERE Id={0} and EndpointSystemId = {1}", user.Id, user.EndPointSystemId));
                    dataAccessTransaction.Commit();
                }
            }
        }
        public static void DeleteByEndpointUserId(long endpointSystemId, long endpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new RepositoryBase<EntityModel.VPNUserInfo>(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM DI_VPN_UserInfo WHERE Id={0} and EndpointSystemId = {1}", endpointUserId, endpointSystemId));
                dataAccessTransaction.Commit();
            }

        }

        public static VPNUserInfo GetByEndpointUserId(long endpointUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.VPNUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.Id == endpointUserId, ar => ar.EndPointSystem, ar1 => ar1.EndPointSystem.EndPointSystemType).FirstOrDefault());
            }
        }
        public static VPNUserInfo GetByUserId(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.VPNUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.UserId == userId).FirstOrDefault());
            }
        }
        public static void InsertUserOnKeyToken(long endPointId, long userId, long oneKeyTokenId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new VPNUserInfoRepository(dataAccessTransaction).InsertVPNOneKeyTokenMapping(endPointId, userId, oneKeyTokenId);
                dataAccessTransaction.Commit();
            }
        }
        public static void TruncateUserOneyToken()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new VPNUserInfoRepository(dataAccessTransaction).ReomveAllUseroneyKeyToken();
                dataAccessTransaction.Commit();
            }
        }
    }
    public class SnapshotUserDetails<T>
    {
        public T UserInfo { get; set; }
        public List<SnapshotEntitlement> Entitlements { get; set; } = new List<SnapshotEntitlement>();
    }
    public class SnapshotEntitlement
    {
        public string Entitlement { get; set; }
        public string Function { get; set; }
        public EntitlementType EntitlementType { get; set; }
    }
    public class SnapshotUser
    {
        public string UserName { get; set; }
        public string Details { get; set; }
        public string UserType { get; set; }
        public bool HasIDD { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

    }
    public class SRMSnapShot
    {
        public List<SRMUserRoleMappingData> SRMUserRoles { get; set; } = new List<SRMUserRoleMappingData>();

    }
}
