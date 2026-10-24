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
    public class SSIDSUserInfo : DomainModelBase<SSIDSUserInfo, EntityModel.SSIDSUserInfo>, IEndpointUserInfo, IEndpointDomainModel
    {
        public SSIDSUserInfo()
        {
            this.SSIDSRolelnfos = new List<SSIDSRolelnfo>();
        }
        public long EndPointSystemId { get; set; }
        public string Emp_Id { get; set; }
        public string User_Id { get; set; }
        public string UserName { get; set; }
        public string Section { get; set; }
        public string Designation { get; set; }
        public string Company { get; set; }
        public string Branch { get; set; }
        public string OrgUnit { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string OfficeTelePhone { get; set; }
        public string HandPhone { get; set; }
        public string Fax { get; set; }
        public string ImageLink { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LastLogon { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Encrypt_Pwd { get; set; }
        public DateTime? ValidDate { get; set; }
        public DateTime? InValidDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public IList<SSIDSRolelnfo> SSIDSRolelnfos { get; set; }
        public bool AccountLocked { get; set; }
        public string Email_Id { get; set; }
        public string AccountName
        {
            get
            {
                return this.User_Id;
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
                if (SSIDSRolelnfos != null)
                {
                    foreach (var item in SSIDSRolelnfos)
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

        public static List<SSIDSUserInfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SSIDSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static void SaveUsers(IList<SSIDSUserInfo> list, long EndpointSystemId, string currentUser, ApplicationType appType, long endpointType)
        {
            SSIDSUserInfo existingUser = null;
            AccountReconcilation acctRecon = null;
            IList<SSIDSUserInfo> listExistingusers = new List<SSIDSUserInfo>();

            try
            {
                if (list != null && list.Count() > 0)
                {
                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                    {
                        new SSIDSUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM SSIDSUserInfoSSIDSRolelnfo WHERE EndpointSystemId={0}", EndpointSystemId));
                        dataAccessTransaction.Commit();
                    }
                }

                List<AccountReconcilation> exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(EndpointSystemId).ToList();
                if (exsitngAccountrecon == null)
                {
                    exsitngAccountrecon = new List<AccountReconcilation>();
                }
                listExistingusers = SSIDSUserInfo.GetAllByEndpointSystemId(EndpointSystemId);
                List<SSIDSRolelnfo> ssidsRoleinfos = SSIDSRolelnfo.GetAllByEndpointSystemId(EndpointSystemId);
                var endpoint = EndPoint.GetById(EndpointSystemId);
                var endpointSystemTypeId = endpoint.EndpointSystemTypeId;
                var entitlementType = (int)EntitlementType.HandPhone;

                foreach (SSIDSUserInfo usr in list)
                {
                    try
                    {

                        if (appType == ApplicationType.SSIDS_SMS)
                        {
                            var uName = User.GetByEmployeeId(usr.Emp_Id.Trim())?.UserName;
                            if (!string.IsNullOrWhiteSpace(uName))
                                usr.User_Id = uName;

                            existingUser = listExistingusers.Where(t =>
                                                                  t.Emp_Id.Trim() == usr.Emp_Id.Trim()
                                                                  && t.EndPointSystemId == EndpointSystemId).FirstOrDefault();
                        }
                        else
                        {
                            existingUser = listExistingusers.Where(t =>
                                                                  t.User_Id.Trim() == usr.User_Id.Trim()
                                                                  && t.EndPointSystemId == EndpointSystemId).FirstOrDefault();
                        }

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

                        if (usr.Roles != null && usr.Roles.Count() > 0 && usr.IsActive.HasValue && usr.IsActive.Value)
                        {
                            foreach (var role in usr.Roles)
                            {
                                var userRole = ssidsRoleinfos.Where(n => n.RoleName == role).FirstOrDefault();
                                if (userRole == null)
                                {
                                    userRole = new SSIDSRolelnfo();
                                    userRole.RoleName = role;
                                    userRole.Description = role;
                                    userRole.EndPointSystemId = usr.EndPointSystemId;
                                    userRole.CreatedBy = "System";
                                    userRole.CreatedDate = DateTime.Now;
                                    userRole.Save("System");
                                    ssidsRoleinfos.Add(userRole);
                                }
                                else
                                {
                                    userRole.LastModifiedBy = "System";
                                    userRole.LastModifiedDate = DateTime.Now;
                                    userRole.Save("System");
                                }

                                SSIDSUserInfo.InsertUserRole(usr.Id, userRole.Id, EndpointSystemId);
                            }
                        }

                        try
                        {
                            acctRecon = exsitngAccountrecon.Where(a => a.EndpointSystemId == EndpointSystemId && !string.IsNullOrWhiteSpace(a.AccountName) && a.AccountName.Equals(usr.AccountName,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                            bool isExist = false;
                            if (acctRecon == null)
                            {
                                acctRecon = new AccountReconcilation();
                                AccountReconcilation.UpdateException(acctRecon, true);
                                acctRecon.AccountType = AccountType.None;
                                acctRecon.EndpointSystemId = EndpointSystemId;
                                acctRecon.CreatedDate = DateTime.Now;
                                acctRecon.CreatedBy = currentUser;
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been added without any request made in the system");
                            }
                            else
                            {
                                isExist = true;
                                
                                if (acctRecon.IsDeleted == false && usr.IsActive.HasValue && usr.IsActive.Value == false)
                                {
                                    AccountReconcilation.UpdateException(acctRecon, false);
                                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been removed without any request made in the system");
                                }
                                if (acctRecon.IsDeleted == true && usr.IsActive.HasValue && usr.IsActive.Value)
                                {
                                    AccountReconcilation.UpdateException(acctRecon, true);
                                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been added without any request made in the system");
                                }
                            }
                            acctRecon.EndpointUserId = usr.Id;
                            acctRecon.LastLogonDate = usr.LastLogon;
                            acctRecon.FullName = usr.DisplayName;
                            acctRecon.AccountName = usr.AccountName;
                            acctRecon.LastModifiedDate = DateTime.Now;
                            acctRecon.LastModifiedBy = currentUser;
                            acctRecon.IsDeleted = usr.IsActive.HasValue ? !usr.IsActive.Value : false;
                            acctRecon.Save(currentUser);

                            if (!isExist)
                            {
                                exsitngAccountrecon.Add(acctRecon);
                            }

                            try
                            {
                                if (appType == ApplicationType.SSIDS_SMS && usr.IsActive.HasValue && usr.IsActive.Value)
                                {
                                    var entRecon = EntitlementReconcilation.GetEndpointUserObjectByParameter(EndpointSystemId,usr.Id,Convert.ToString(usr.HandPhone).Trim(),
                                                                             entitlementType);

                                    if (entRecon == null)
                                    {
                                        entRecon = new EntitlementReconcilation();
                                        var entitlements = (usr.HandPhone.ToString());
                                        entRecon = CreateEntitlementReconcilationShell(acctRecon, EntitlementType.HandPhone, (long)EndpointType.SSIDS, entitlements);
                                        entRecon.Entitlement = Convert.ToString(usr.HandPhone);
                                        entRecon.EntitlementId = usr.Id;
                                        entRecon.EntitlementDescription = usr.HandPhone;
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                        entRecon.CreatedDate = DateTime.Now;
                                        entRecon.CreatedBy = "System";
                                        entRecon.EndpointSystemId = EndpointSystemId;
                                        entRecon.Save(currentUser);
                                    }
                                    else
                                    {
                                        if(entRecon.Entitlement != Convert.ToString(usr.HandPhone))
                                        {
                                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName} Existing HandPhone:{entRecon.Entitlement} New: {usr.HandPhone}");
                                        }
                                        entRecon.LastModifiedDate = DateTime.Now;
                                        entRecon.LastModifiedBy = "System";
                                        entRecon.Entitlement = Convert.ToString(usr.HandPhone);
                                        entRecon.EntitlementId = usr.Id;
                                        entRecon.EntitlementDescription = usr.HandPhone;
                                        entRecon.Save(currentUser);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, string.Format("Error while SSIDS-SMS SaveUsers Entitlement Recon for the User {0}", usr.User_Id), ex);
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while SSIDS SaveUsers Account Recon.", ex);
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while SSIDS SaveUsers.", ex);
                        throw ex;
                    }
                }

            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Error while SSIDS SaveUsers.{ex}", ex);
                throw ex;
            }
        }

        public static IList<SSIDSUserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<SSIDSUserInfo> listDeletedUsers = new List<SSIDSUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.SSIDSUserInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }

        public static void RemoveSSIDSUser(List<SSIDSUserInfo> list)
        {
            foreach (SSIDSUserInfo user in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new SSIDSUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM SSIDSUserInfoSSIDSRolelnfo WHERE SSIDSUserInfo_Id=" + user.Id + " and EndpointSystemId = " + user.EndPointSystemId + ""));
                    dataAccessTransaction.Commit();
                }
                user.Delete();
            }
        }

        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SSIDSUserInfoRepository(dataAccessTransaction).GetByEndPointSystemId(endPointSystemId);
            }
        }

        public static void InsertUserRole(long userId, long rolelId, long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new SSIDSUserInfoRepository(dataAccessTransaction).InsertUserRoleMapping(endPointSystemId, userId, rolelId);
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
                bldr.AppendFormat(string.Format("(DI_SSIDS_UserInfo.{0}=DI_M_User.{1}  and DI_SSIDS_UserInfo.{2} != '' and DI_SSIDS_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
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
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_SSIDS_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_SSIDS_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_SSIDS_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_User.Id <> 1 AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString(), considerInActiveAccountForRecon);
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
                bldR.Append("Update DI_M_EntitlementReconcilation set DI_M_EntitlementReconcilation.UserId = DI_M_User.Id FROM DI_SSIDS_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_EntitlementReconcilation on DI_M_EntitlementReconcilation.EndpointUserId = DI_SSIDS_UserInfo.Id");
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.Id = DI_M_EntitlementReconcilation.AccountReconcilationId");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_SSIDS_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND DI_M_User.IsActive = 1  AND DI_M_User.Id <> 1 AND DI_M_EntitlementReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString());
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

        public static SSIDSUserInfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new SSIDSUserInfoRepository(transaction).GetLockedAccountByName(accountName, diuserId, endpointSystemId, accountType));
            }
        }

        public static List<SSIDSUserInfo> GetActiveAccounts(long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SSIDSUserInfoRepository(transaction).GetActiveAccounts(diuserId, endpointSystemId, accountType));
            }
        }

        public static bool InsertUserRole(SSIDSUserInfo user)
        {
            var userWithExistingroles = SSIDSUserInfo.GetByUserIdentityIncludeLevels(user.User_Id);

            foreach (string role in user.Roles)
            {
                SSIDSRolelnfo roleinfo = SSIDSRolelnfo.GetByRoleName(role);
                if (roleinfo != null && !userWithExistingroles.SSIDSRolelnfos.Any(l => l.Id == roleinfo.Id))
                {
                    SSIDSUserInfo.InsertUserRole(user.Id, roleinfo.Id, user.EndPointSystemId);
                }
            }

            return true;
        }
        public static bool DeleteUserRoleMap(SSIDSUserInfo userinfo, string roles)
        {

            List<string> tempRoles = roles.Split(',').ToList();

            foreach (var role in tempRoles)
            {
                SSIDSRolelnfo roleinfo = SSIDSRolelnfo.GetByRoleName(role);
                if (roleinfo != null)
                {
                    using (var transaction = new DataAccessTransaction())
                    {
                        new SSIDSUserInfoRepository(transaction).DeleteRole(userinfo.Id, roleinfo.Id, userinfo.EndPointSystemId);
                        transaction.Commit();
                    }
                }
            }

            return true;
        }

        public static SSIDSUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SSIDSUserInfoRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }
        public static SSIDSUserInfo GetLockedPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SSIDSUserInfoRepository(transaction).GetLockedPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }

        public static SSIDSUserInfo GetByUserIdentity(string userIdentity)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SSIDSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.User_Id == userIdentity).FirstOrDefault());
            }
        }

        public static SSIDSUserInfo GetByUserIdentityIncludeLevels(string userIdentity)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SSIDSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.User_Id == userIdentity, roles => roles.SSIDSRolelnfos).FirstOrDefault());
            }
        }

        public static SSIDSUserInfo GetByIdIncludeRoles(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SSIDSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.Id == id, roles => roles.SSIDSRolelnfos).FirstOrDefault());
            }
        }

        public static SSIDSUserInfo GetByUserIdentityAndEndpointId(string userIdentity, long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SSIDSUserInfo>(dataAccessTransaction)
                      .Get(npusr => npusr.User_Id == userIdentity && npusr.EndPointSystemId == endpointId).FirstOrDefault());
            }
        }

        protected static EntitlementReconcilation CreateEntitlementReconcilationShell(AccountReconcilation acctRecon, EntitlementType _entitlementType, long endpointType, string Entitlements = "")
        {
            return new EntitlementReconcilation()
            {
                IsSODConflict = false,
                IsSensitiveAccess = false,
                AccountReconcilationId = acctRecon.Id,
                EndpointSystemId = acctRecon.EndpointSystemId,
                EndpointUserId = acctRecon.EndpointUserId,
                AccountName = acctRecon.AccountName,
                UserId = acctRecon.UserId,
                EnteredDate = DateTime.Now,
                IsDeleted = false,
                EntitlementType = (int)_entitlementType,
                EndpointType = (EndpointType)endpointType,
            };
        }

        public static void DeleteAllUserRoleMaps(long epUserId)
        {
            string query = string.Format("DELETE FROM SSIDSUserInfoSSIDSRolelnfo WHERE SSIDSUserInfo_Id={0}", epUserId);

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new GEMSUserInfoRepository(dataAccessTransaction).ExecuteQuery(query);
                dataAccessTransaction.Commit();
            }
        }
        public static SSIDSUserInfo GetByIdAndEndpointId(long id,long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SSIDSUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.Id == id && grp.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
        }
    }
}
