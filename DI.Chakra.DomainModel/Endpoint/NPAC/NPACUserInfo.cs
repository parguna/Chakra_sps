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
    public class NPACUserInfo : DomainModelBase<NPACUserInfo, EntityModel.NPACUserInfo>, IEndpointUserInfo, IEndpointDomainModel
    {
        public NPACUserInfo()
        {
            this.NPACLevelInfos = new List<NPACLevelInfo>();
        }
        public long EndPointSystemId { get; set; }
        public string Emp_Id { get; set; }
        public string UserIdentity { get; set; }
        public string UserName { get; set; }
        public string Passwds { get; set; }
        public string Designation { get; set; }
        public string Phone { get; set; }
        public string Pager { get; set; }
        public string Fax { get; set; }
        public string Groups { get; set; }
        public Nullable<int> Levels { get; set; }
        public string Section { get; set; }
        public string Grade { get; set; }
        public string Zone { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? Invalid_Date { get; set; }
        public string Encrypt_Pwd { get; set; }
        public DateTime? Valid_Date { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public IList<NPACLevelInfo> NPACLevelInfos { get; set; }
        public string UserADID { get; set; }
        public string EmailId { get; set; }
        public string CompanyName { get; set; }
        public string SectionName { get; set; }


        public string AccountName
        {
            get
            {
                return this.UserIdentity;
            }
        }

        public string AccountPassword
        {
            get { return this.Encrypt_Pwd; }
        }

        public string DisplayName
        {
            get
            {
                return this.UserName;
            }
        }

        public List<string> Roles;
        public string UserValidFrom;
        public string UserValidTo;
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (NPACLevelInfos != null)
                {
                    foreach (var item in NPACLevelInfos)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }

        public IEndpointUserInfo GetEndpointUserInfo(long EndpointUserId)
        {
            return GetById(EndpointUserId);
        }

        public static List<NPACUserInfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.NPACUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static void SaveUsers(IList<NPACUserInfo> list, long EndpointSystemId, string currentUser)
        {
            NPACUserInfo existingUser = null;
            AccountReconcilation acctRecon = null;
            IList<NPACUserInfo> listExistingusers = new List<NPACUserInfo>();

            try
            {
                if (list != null && list.Count() > 0)
                {
                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                    {
                        new NPACUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM NPACUserInfoNPACLevelInfo WHERE EndpointSystemId={0}", EndpointSystemId));
                        dataAccessTransaction.Commit();
                    }
                }

                List<AccountReconcilation> exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(EndpointSystemId).ToList();
                if (exsitngAccountrecon == null)
                {
                    exsitngAccountrecon = new List<AccountReconcilation>();
                }
                listExistingusers = NPACUserInfo.GetAllByEndpointSystemId(EndpointSystemId);
                List<NPACLevelInfo> npacLevels = NPACLevelInfo.GetAllByEndpointSystemId(EndpointSystemId);
                var endpoint = EndPoint.GetById(EndpointSystemId);
                var endpointSystemTypeId = endpoint.EndpointSystemTypeId;
                bool isActive = true;
                foreach (NPACUserInfo usr in list)
                {
                    isActive = true;
                    try
                    {
                        existingUser = listExistingusers.Where(t =>
                        t.UserName?.Trim().ToLower() == usr.UserName?.Trim().ToLower() && t.UserIdentity?.Trim().ToLower() == usr.UserIdentity?.Trim().ToLower()
                        && t.EndPointSystemId == EndpointSystemId).FirstOrDefault();

                        if (existingUser == null)
                        {
                            usr.Save(currentUser);
                            listExistingusers.Add(usr);
                        }
                        else
                        {
                            AccountReconcilation.SaveAttributeException(existingUser, usr, (long)endpointSystemTypeId, null, usr.AccountName, currentUser);
                            usr.Id = existingUser.Id;
                            usr.LastModifiedBy = "System";
                            usr.LastModifiedDate = DateTime.Now;
                            usr.Save(currentUser);
                        }

                        if (usr.Levels != null)
                        {
                            var npacLevel = npacLevels.Where(n => n.Level == usr.Levels).FirstOrDefault();
                            if (npacLevel == null)
                            {
                                npacLevel = new NPACLevelInfo();
                                npacLevel.Level = usr.Levels;
                                npacLevel.Description = string.Format("Level {0}", usr.Levels);
                                npacLevel.EndPointSystemId = usr.EndPointSystemId;
                                npacLevel.CreatedBy = "System";
                                npacLevel.CreatedDate = DateTime.Now;
                                npacLevel.Save("System");
                                npacLevels.Add(npacLevel);
                            }
                            else
                            {
                                npacLevel.LastModifiedBy = "System";
                                npacLevel.LastModifiedDate = DateTime.Now;
                                npacLevel.Save("System");
                            }

                            NPACUserInfo.InsertUserLevel(usr.Id, npacLevel.Id, EndpointSystemId);
                        }
                        if(usr.Levels == null && string.IsNullOrWhiteSpace(usr.Section))
                        {
                            isActive = false;
                           
                        }
                        try
                        {
                            acctRecon = exsitngAccountrecon.Where(a => a.EndpointSystemId == EndpointSystemId && a.AccountName.Equals(usr.AccountName,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                            bool isExist = false;
                            if (acctRecon == null)
                            {
                                acctRecon = new AccountReconcilation();
                                AccountReconcilation.UpdateException(acctRecon, true);
                                acctRecon.AccountType = AccountType.None;
                                acctRecon.EndpointSystemId = EndpointSystemId;
                                acctRecon.CreatedDate = DateTime.Now;
                                acctRecon.CreatedBy = currentUser;
                                
                                if (isActive == false)
                                {
                                    AccountReconcilation.UpdateException(acctRecon, false);
                                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been removed without any request made in the system");
                                }
                            }
                            else
                            {
                                if (isActive == false)
                                {
                                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.UserName}(DisplayName{usr?.DisplayName}) has been REMOVED without any request made in the system");
                                    AccountReconcilation.UpdateException(acctRecon, false);
                                }
                                isExist = true;
                                if (acctRecon.IsDeleted && isActive)
                                {
                                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.UserName}(DisplayName{usr?.DisplayName}) has been Added without any request made in the system");
                                    AccountReconcilation.UpdateException(acctRecon, true);
                                }
                            }
                            acctRecon.EndpointUserId = usr.Id;
                            //Commented the below as per Jacqueline Request 29 Sep 2021
                            //acctRecon.ValidToDate = usr.Invalid_Date;
                            acctRecon.ValidToDate = null;
                            acctRecon.FullName = usr.DisplayName;
                            acctRecon.AccountName = usr.AccountName;
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
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while NPAC SaveUsers Account Recon.", ex);
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while NPAC SaveUsers.", ex);
                        throw ex;
                    }
                }

            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while NPAC SaveUsers.", ex);
                throw ex;
            }
        }

        public static IList<NPACUserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<NPACUserInfo> listDeletedUsers = new List<NPACUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.NPACUserInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }

        public static void RemoveNPACUser(List<NPACUserInfo> list)
        {
            foreach (NPACUserInfo user in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new NPACUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM GEMSUserInfoGEMSRoleInfo WHERE GEMSUserInfo_Id=" + user.Id + " and EndpointSystemId = " + user.EndPointSystemId + ""));
                    dataAccessTransaction.Commit();
                }
                user.Delete();
            }
        }

        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new NPACUserInfoRepository(dataAccessTransaction).GetByEndPointSystemId(endPointSystemId);
            }
        }

        public static void InsertUserLevel(long userId, long levelId, long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new NPACUserInfoRepository(dataAccessTransaction).InsertNPACUserLevelMapping(endPointSystemId, userId, levelId);
                dataAccessTransaction.Commit();
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
                bldr.AppendFormat(string.Format("(DI_NPAC_UserInfo.{0}=DI_M_User.{1}  and DI_NPAC_UserInfo.{2} != '' and DI_NPAC_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
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
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_NPAC_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_NPAC_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_NPAC_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_User.Id <> 1 AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString(),considerInActiveAccountForRecon);
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
                bldR.Append("Update DI_M_EntitlementReconcilation set DI_M_EntitlementReconcilation.UserId = DI_M_User.Id FROM DI_NPAC_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_EntitlementReconcilation on DI_M_EntitlementReconcilation.EndpointUserId = DI_NPAC_UserInfo.Id");
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.Id = DI_M_EntitlementReconcilation.AccountReconcilationId");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_NPAC_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND DI_M_EntitlementReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString());
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

        public static NPACUserInfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new NPACUserInfoRepository(transaction).GetLockedAccountByName(accountName, diuserId, endpointSystemId, accountType));
            }
        }

        public static List<NPACUserInfo> GetActiveAccounts(long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new NPACUserInfoRepository(transaction).GetActiveAccounts(diuserId, endpointSystemId, accountType));
            }
        }

        public static bool InsertNPACUserLevel(NPACUserInfo npacUser)
        {
            var userWithExistingLevels = NPACUserInfo.GetByUserIdentityIncludeLevels(npacUser.UserIdentity);

            foreach (string level in npacUser.Roles)
            {
                NPACLevelInfo npaLevel = NPACLevelInfo.GetByLevel(Convert.ToInt32(level));
                if (npaLevel != null && !userWithExistingLevels.NPACLevelInfos.Any(l => l.Id == npaLevel.Id))
                {
                    NPACUserInfo.InsertUserLevel(npacUser.Id, npaLevel.Id, npacUser.EndPointSystemId);
                }
            }

            return true;
        }
        public static bool DeleteNPACUserLEVELMap(NPACUserInfo userinfo, string npaclevels)
        {

            List<string> levels = npaclevels.Split(',').ToList();

            foreach (var level in levels)
            {
                NPACLevelInfo levelinfo = NPACLevelInfo.GetByLevel(Convert.ToInt32(level));
                if (levelinfo != null)
                {
                    using (var transaction = new DataAccessTransaction())
                    {
                        new NPACUserInfoRepository(transaction).DeleteRole(userinfo.Id, levelinfo.Id, userinfo.EndPointSystemId);
                        transaction.Commit();
                    }
                }
            }

            return true;
        }

        public static NPACUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                var userinfo = ORMap(new NPACUserInfoRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
                if (userinfo != null && !string.IsNullOrWhiteSpace(userinfo.UserIdentity))
                {
                    var tempUser = GetByUserIdentityIncludeLevels(userinfo.UserIdentity);
                    if(tempUser != null)
                        userinfo.Levels = tempUser.NPACLevelInfos.Count() > 0 ? tempUser.NPACLevelInfos[0].Level : null;
                }
                return userinfo;
            }
        }
        public static NPACUserInfo GetLockedPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new NPACUserInfoRepository(transaction).GetLockedPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }

        public static NPACUserInfo GetByUserIdentity(string userIdentity)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.NPACUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.UserIdentity == userIdentity).FirstOrDefault());
            }
        }

        public static NPACUserInfo GetByUserIdentityIncludeLevels(string userIdentity)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.NPACUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.UserIdentity == userIdentity, roles => roles.NPACLevelInfos).FirstOrDefault());
            }
        }

        public static NPACUserInfo GetByIdIncludeLevels(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.NPACUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.Id == id, roles => roles.NPACLevelInfos).FirstOrDefault());
            }
        }
        public static bool DisableNPACUser(NPACUserInfo npacUser)
        {
            NPACUserInfo userwithLevel = NPACUserInfo.GetByUserIdentityIncludeLevels(npacUser.UserIdentity);
            if (userwithLevel != null)
            {
                foreach (var level in userwithLevel.NPACLevelInfos)
                {
                    using (var dataAccessTransaction = new DataAccessTransaction())
                    {
                        new NPACUserInfoRepository(dataAccessTransaction).DeleteRole(userwithLevel.Id, level.Id, userwithLevel.Id);
                        dataAccessTransaction.Commit();
                    }
                }
                userwithLevel.Section = null;
                userwithLevel.Invalid_Date = null;
                userwithLevel.Save("System");
            }

            return true;
        }

        public static NPACUserInfo GetByUserIdentityAndEndpointId(string userIdentity, long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.NPACUserInfo>(dataAccessTransaction)
                      .Get(npusr => npusr.UserIdentity == userIdentity && npusr.EndPointSystemId == endpointId).FirstOrDefault());
            }
        }

        public static NPACUserInfo GetByUserByEndpointSystemAndId(long endpointId, long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.NPACUserInfo>(dataAccessTransaction)
                 .Get(grp => grp.Id == userId && grp.EndPointSystemId == endpointId).FirstOrDefault());
            }
        }

        public static NPACUserInfo GetNPACUserForAttestation(long endpointSystemId, long accountReconId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new NPACUserInfoRepository(dataAccessTransaction).GetNPACUserForAttestation(endpointSystemId, accountReconId));
            }
        }
    }
}
