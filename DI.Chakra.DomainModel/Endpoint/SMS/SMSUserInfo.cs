using DI.Chakra.Infrastructure;
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
    public class SMSUserInfo : DomainModelBase<SMSUserInfo, EntityModel.SMSUserInfo>, IEndpointUserInfo, IEndpointDomainModel
    {
        public SMSUserInfo()
        {
            this.SMSAccessLevels = new List<SMSAccessLevel>();
            this.SMSBadgeInfos = new List<SMSBadgeInfo>();
            this.AccessLevels = new List<string>();
        }
        public long EndPointSystemId { get; set; }

        #region SMS Data
        public int UserId { get; set; }
        //first name
        public string FIRSTNAME { get; set; }
        //last name
        public string LASTNAME { get; set; }
        //known as
        public string MIDNAME { get; set; }
        //PERNR
        public string SSNO { get; set; }

        //department/branch
        public string Addr1 { get; set; }
        //section
        public string City { get; set; }
        //nric
        public string Zip { get; set; }
        //AD ID
        public string OPHONE { get; set; }
        //email id
        public string EMAIL { get; set; }

        //lenel company id
        public int DEPT { get; set; }

        //lenel company id
        public string Extension { get; set; }
        #endregion

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public List<SMSAccessLevel> SMSAccessLevels { get; set; }
        public List<SMSBadgeInfo> SMSBadgeInfos { get; set; } = new List<SMSBadgeInfo>();
        public EndPoint EndPoint { get; set; }

        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (SMSAccessLevels != null)
                {
                    foreach (var item in SMSAccessLevels)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }
        public string AccountName
        {
            get
            {
                return this.SSNO;
            }
        }
        public string AccountPassword
        {
            get { return this.Password; }
        }
        public string DisplayName
        {
            get
            {
                return MIDNAME;
            } set
            {
                MIDNAME = value;
            }
        }
            
        public string NewPernr;
        public string Password;
        public string UserValidFrom;
        public string UserValidTo;
        public List<string> AccessLevels;
        IEndpointUserInfo IEndpointDomainModel.GetEndpointUserInfo(long EndpointUserId)
        {
            return GetById(EndpointUserId);
        }
        public static List<SMSUserInfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SMSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static SMSUserInfo GetByIdIncludeAccesslevels(long Id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SMSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.Id == Id, grp => grp.SMSAccessLevels).FirstOrDefault());
            }
        }
        public static void SaveUsers(IList<SMSUserInfo> list, long EndpointSystemId, string currentUser)
        {
            SMSUserInfo existingUser = null;
            AccountReconcilation acctRecon = null;
            IList<SMSUserInfo> listExistingusers = new List<SMSUserInfo>();

            CleanUpInactiveDuplicatedAccount(EndpointSystemId); 
            try
            {
                List<AccountReconcilation> exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(EndpointSystemId).ToList();

                listExistingusers = SMSUserInfo.GetAllByEndpointSystemId(EndpointSystemId);
                if (listExistingusers == null)
                    listExistingusers = new List<SMSUserInfo>();

                var endpoint = EndPoint.GetById(EndpointSystemId);
                var endpointSystemTypeId = endpoint.EndpointSystemTypeId;
                var currentDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                bool isActiveAccount = false;
                foreach (SMSUserInfo usr in list)
                {
                    try
                    {
                        isActiveAccount = false;
                        existingUser = listExistingusers.Where(t => t.UserId == usr.UserId).FirstOrDefault();

                        if (existingUser == null)
                        {
                            usr.Save("System");
                            listExistingusers.Add(usr);
                        }
                        else
                        {
                            usr.Id = existingUser.Id;
                            usr.LastModifiedBy = "System";
                            usr.LastModifiedDate = DateTime.Now;
                            usr.Save("System");
                        }
                        if (!string.IsNullOrWhiteSpace(usr.AccountName))
                        {

                            acctRecon = exsitngAccountrecon.Where(a => a.EndpointSystemId == usr.EndPointSystemId && a.AccountName.Equals(usr.AccountName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                            bool isExist = false;
                            if (acctRecon == null)
                            {
                                acctRecon = new AccountReconcilation();
                                AccountReconcilation.UpdateException(acctRecon, true);
                                acctRecon.AccountType = AccountType.None;
                                acctRecon.FullName = !string.IsNullOrWhiteSpace(usr.DisplayName) ? usr.DisplayName : usr.AccountName;
                            }
                            else
                            {
                                isExist = true;
                                if (acctRecon.IsDeleted || acctRecon.IsLocked)
                                {
                                    AccountReconcilation.UpdateException(acctRecon, true);
                                }
                            }
                            var smsBadges = SMSBadgeInfo.GetByBadgesByUserId(usr.UserId);
                            foreach (var item in smsBadges)
                            {
                                if (!string.IsNullOrWhiteSpace(item.DEACTIVATE) && item.DEACTIVATE.Length > 8)
                                {
                                    var date = Convert.ToInt32(item.DEACTIVATE.GetSubString(8));
                                    if (date > currentDate && item.STATUS == 1)
                                    {
                                        isActiveAccount = true;
                                        break;
                                    }
                                }
                            }
                            if (acctRecon.IsDeleted && isActiveAccount)
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been Added without any request made in the system");

                            }
                            if (acctRecon.IsDeleted == false && isActiveAccount == false)
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been removed without any request made in the system");
                            }
                            acctRecon.IsDeleted = !isActiveAccount;
                            acctRecon.IsLocked = !isActiveAccount;
                            //DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, $"SMS-AccountName:{usr.AccountName} IsLocked:{acctRecon.IsLocked} IsDeleted:{acctRecon.IsDeleted}");
                            acctRecon.FullName = !string.IsNullOrWhiteSpace(usr.MIDNAME) ? usr.DisplayName : usr.AccountName;
                            acctRecon.AccountName = usr.AccountName;
                            acctRecon.LastModifiedDate = DateTime.Now;
                            acctRecon.EndpointSystemId = usr.EndPointSystemId;
                            acctRecon.EndpointUserId = usr.Id;
                            acctRecon.Save("System");

                            if (!isExist)
                            {
                                exsitngAccountrecon.Add(acctRecon);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while Lenel SaveUsers.", ex);
                    }
                }

            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while Lenel SaveUsers.", ex);
            }
            
        }
        private static void CleanUpInactiveDuplicatedAccount(long EndpointSystemId)
        {
                List<AccountReconcilation> activeAccounts = AccountReconcilation.GetAllByEndpointId(EndpointSystemId).ToList();
                if (activeAccounts.Count > 0)
                {
                    List<AccountReconcilation> inactiveAccountsToBeDeleted = activeAccounts
                    .Where(x => x.IsDeleted && x.UserId.HasValue && x.EndpointUserId!=0 && activeAccounts.Any(y => !y.IsDeleted && y.EndpointSystemId == x.EndpointSystemId && y.UserId == x.UserId && y.EndpointUserId == x.EndpointUserId)).ToList();

                    if (inactiveAccountsToBeDeleted.Count > 0)
                    {
                        foreach (AccountReconcilation acc in inactiveAccountsToBeDeleted)
                        {
                             DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("Preparing to delete the duplicated inactive account(UserId:{0},EndPointUserId:{1})", acc.UserId,acc.EndpointUserId));
                                List<EntitlementReconcilation> entitlements= EntitlementReconcilation.GetAllByAccountReconcilationId(acc.Id);
                                foreach (EntitlementReconcilation entitlement in entitlements)
                                {
                                    try
                                    {
                                        entitlement.Delete();
                                    }
                                    catch(Exception e)
                                    {
                                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, string.Format("Error while deleting entitlement reconcilation Id:{0}",entitlement.Id),e);
                                    }
                                }
                                try
                                {
                                    acc.Delete();
                                }
                                catch (Exception e)
                                {
                                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, string.Format("Error while deleting account reconcilation Id:{0}", acc.Id), e);
                                }
                        
                        }
                    }
                }
        }
         
public static IList<SMSUserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<SMSUserInfo> listDeletedUsers = new List<SMSUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.SMSUserInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }
        public static void RemoveSMSUser(List<SMSUserInfo> list)
        {
            foreach (SMSUserInfo user in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new SMSUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM SMSUserInfoSMSAccessLevel WHERE SMSUserInfo_Id=" + user.Id));
                    dataAccessTransaction.Commit();
                }
                user.Delete();
            }
        }

        //public static void RemoveInActiveAccount(List<AccountReconcilation> list)
        //{
        //    foreach (AccountReconcilation user in list)
        //    {
        //        using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //        {
        //            AccountReconcilationRepository accountRepoistory = new AccountReconcilationRepository(dataAccessTransaction);
        //            accountRepoistory.ExecuteQuery(string.Format("Delete From DI_M_ Where "));

        //            //AccountReconcilationRepository repository=new Accoun  new AccountReconcilationRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM SMSUserInfoSMSAccessLevel WHERE SMSUserInfo_Id=" + user.Id));
        //            dataAccessTransaction.Commit();

        //        }
             
        //    }
        //}

        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SMSUserInfoRepository(dataAccessTransaction).GetByEndPointSystemId(endPointSystemId);
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
                bldr.AppendFormat(string.Format("(DI_SMS_UserInfo.{0}=DI_M_User.{1}  and DI_SMS_UserInfo.{2} != '' and DI_SMS_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
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
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_SMS_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_SMS_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_SMS_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString(), considerInActiveAccountForRecon);
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
                bldR.Append("Update DI_M_EntitlementReconcilation set DI_M_EntitlementReconcilation.UserId = DI_M_User.Id FROM DI_SMS_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_EntitlementReconcilation on DI_M_EntitlementReconcilation.EndpointUserId = DI_SMS_UserInfo.Id");
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.Id = DI_M_EntitlementReconcilation.AccountReconcilationId");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_SMS_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND DI_M_User.IsActive = 1  AND DI_M_User.Id <> 1 AND DI_M_EntitlementReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString());
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
        public static void SaveUserAccessLevels(IList<SMSUserAccessLevel> list, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SMSUserInfoRepository(dataAccessTransaction).DeleteUserAccessDataByEndpoint(endpointSystemId);
                dataAccessTransaction.Commit();
            }

            foreach (var userAccess in list)
            {
                InsertUserLevel(userAccess.SMSUserInfo_Id, userAccess.SMSAccessLevel_Id, endpointSystemId);
            }

        }
        public static SMSUserInfo GetLockedSMSUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new SMSUserInfoRepository(dataAccessTransaction).GetLockedSMSUserByDIUserId(DIUserId, endpointSystemId));
            }
        }

        public static SMSUserInfo GetUnLockedSMSUserByDIUserId(long DIUserId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new SMSUserInfoRepository(dataAccessTransaction).GetUnLockedSMSUserByDIUserId(DIUserId, endpointSystemId));
            }
        }
        public static void InsertUserLevel(long userId, long levelId, long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new SMSUserInfoRepository(dataAccessTransaction).InsertSMSUserLevelMapping(endPointSystemId, userId, levelId);
                dataAccessTransaction.Commit();
            }
        }
        public static void DeleteUserLevel(long userId, long levelId, long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new SMSUserInfoRepository(dataAccessTransaction).DeleteSMSUserLevelMapping(endPointSystemId, userId, levelId);
                dataAccessTransaction.Commit();
            }
        }
        public static SMSUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SMSUserInfoRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }

        public static Dictionary<string, string> GetExistingRolesByEndpointUserId(long endpointuserid, long endpointsystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                SMSUserInfoRepository repo = new SMSUserInfoRepository(transaction);
                var usr = repo.GetByEndpointUserId(endpointuserid, endpointsystemId);
                var defaultAccessLevels = SMSXLProvisioningMatrix.GetAll().Select(i => i.DefaultGroups).ToList();

                var tempDic = new Dictionary<string, string>();
                if (usr != null)
                {
                    foreach (var item in usr.SMSAccessLevels)
                    {
                        bool isexists = false;
                        foreach (var level in defaultAccessLevels)
                        {
                            if (!string.IsNullOrWhiteSpace(level))
                            {
                                var templevel = level.Split(',').ToList();
                                if (templevel.Contains(item.Name))
                                {
                                    isexists = true;
                                }
                            }
                        }

                        if (!isexists && !tempDic.ContainsKey(item.Name))
                            tempDic.Add(item.Name, item.Name);
                    }

                    return tempDic;
                }
                else
                {
                    return new Dictionary<string, string>();
                }
            }
        }
        public static Dictionary<string, string> GetExistingTechRoles(long userid, long endpointuserid, long endpointsystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                var user = User.GetById(userid);
                SMSUserInfoRepository repo = new SMSUserInfoRepository(transaction);
                var usr = repo.GetByEndpointUserId(endpointuserid, endpointsystemId);
                var defaultAccessLevels = SMSXLProvisioningMatrix.GetAll();

                var tempDic = new Dictionary<string, string>();
                if (usr != null)
                {
                    foreach (var item in usr.SMSAccessLevels)
                    {
                        bool isexists = false;
                        foreach (var level in defaultAccessLevels)
                        {
                            if (!string.IsNullOrWhiteSpace(level.DefaultGroups))
                            {
                                var templevel = level.DefaultGroups.Split(',').ToList();
                                //Permanent Employee
                                if (!string.IsNullOrWhiteSpace(user.PostalCode))
                                {
                                    if (!string.IsNullOrWhiteSpace(level.PostalCode) &&
                                        level.PostalCode.Equals(user.PostalCode, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (templevel.Contains(item.Name, StringComparer.InvariantCultureIgnoreCase))
                                        {
                                            isexists = true;
                                        }
                                    }
                                }
                            }
                        }
                        if (!isexists && !tempDic.ContainsKey(item.Name))
                            tempDic.Add(item.Name, item.Name);
                    }
                    return tempDic;
                }
                else
                {
                    return new Dictionary<string, string>();
                }
            }
        }
        public static SMSUserInfo GetSMSUserByADID(string adID)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SMSUserInfo>(transaction)
                      .Get(item => item.OPHONE.Equals(adID, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());
            }

        }
        public static List<SMSUserInfo> GetSMSUserInfoForUpdateMobileNumber(long endpointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SMSUserInfoRepository(dataAccessTransaction).GetSMSUserInfoForUpdateMobileNumber(endpointId));
            }
        }
    }
}
