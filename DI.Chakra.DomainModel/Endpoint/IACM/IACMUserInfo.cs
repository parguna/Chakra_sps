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
    public class IACMUserInfo : DomainModelBase<IACMUserInfo, EntityModel.IACMUserInfo>, IEndpointUserInfo
    {
        public long EndPointSystemId { get; set; }

        public string NRIC { get; set; }

        public string EmployeeId { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ADID { get; set; }

        public DateTime? AccountCreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LockedDate { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime? DisabledDate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public virtual List<IACMRoleInfo> Roles { get; set; }

        public virtual List<IACMGroupInfo> Groups { get; set; }

        public EndPoint EndPoint { get; set; }

        public string AccountName { get { return this.UserName; } set { } }

        public string AccountPassword { get; set; }

        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (Roles != null)
                {
                    foreach (var item in Roles)
                    {
                        li.Add(item);
                    }
                }

                if (Groups != null)
                {
                    foreach (var item in Groups)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }

        public static IACMUserInfo GetByIdIncludeRoles(long id)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.IACMUserInfo>(transaction).GetById(id, user => user.Roles, user => user.Groups));
            }
        }

        public static IList<IACMUserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<IACMUserInfo> listDeletedUsers = new List<IACMUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.IACMUserInfo>(dataAccessTransaction)
                       .Get(
                        objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }

        public static void RemoveUser(List<IACMUserInfo> list)
        {
            foreach (IACMUserInfo user in list)
            {
                string executeQuery = string.Format("DELETE FROM IACMRoleInfoIACMUserInfo WHERE IACMUserInfo_Id = {0}", user.Id);
                IACMRoleInfo.ExecuteQuery(executeQuery);
                executeQuery = string.Format("DELETE FROM IACMUserInfoIACMGroupInfo WHERE IACMUserInfo_Id = {0}", user.Id);
                IACMRoleInfo.ExecuteQuery(executeQuery);
                user.Delete();
            }
        }

        public static void SaveUsers(IList<IACMUserInfo> list, long EndpointSystemId, string currentUser)
        {
            //var ep = EndPoint.GetById(EndpointSystemId);
            AccountReconcilation acctRecon = null;
            EntitlementReconcilation entRecon = null;
            List<User> users = new List<User>();
            try
            {
                users = User.GetAllActiveUser();
                foreach (IACMUserInfo usr in list)
                {
                    try
                    {
                        var userValidTo = GetUserValidTo(users.FirstOrDefault(i=> i.EmployeeId == usr.EmployeeId));
                        acctRecon = AccountReconcilation.GetByUser(usr.EndPointSystemId, usr.Id, false);
                        if (acctRecon == null)
                        {
                            acctRecon = new AccountReconcilation();
                            AccountReconcilation.UpdateException(acctRecon, true);
                            acctRecon.EndpointUserId = usr.Id;
                            acctRecon.AccountCreatedDate = usr.CreatedDate;
                        }

                        acctRecon.EndpointSystemId = usr.EndPointSystemId;
                        acctRecon.FullName = usr.DisplayName;
                        acctRecon.AccountName = usr.AccountName;
                        var lastlogon = usr.LastLoginDate;
                        if (lastlogon != null && lastlogon != DateTime.MinValue && lastlogon != DateTime.MaxValue)
                        {
                            acctRecon.LastLogonDate = lastlogon;
                        }

                        acctRecon.LastModifiedDate = DateTime.Now;
                        acctRecon.IsLocked = usr.IsDisabled;
                        acctRecon.IsDeleted = usr.IsDeleted;
                        if((usr.AccountName.ToUpper() != "IACMADMIN")|| (usr.AccountName.ToUpper() == "IACMADMIN" && acctRecon.Id > 0 && acctRecon.MappingType != MappingType.Manual))
                            acctRecon.AccountType = AccountType.User;
                   
                        acctRecon.ValidToDate = userValidTo;
                        acctRecon.Save(currentUser);

                        if (usr.Roles != null && usr.Roles.Count > 0)
                        {
                            foreach (IACMRoleInfo role in usr.Roles)
                            {
                                try
                                {
                                    if (acctRecon == null) continue;
                                    entRecon = EntitlementReconcilation.GetByUser(usr.Id, usr.EndPointSystemId, role.Name.Trim(), EntitlementType.Role);

                                    if (entRecon == null)
                                    {
                                        entRecon = new EntitlementReconcilation();
                                        //entRecon.Id = 0;
                                        entRecon.EnteredDate = DateTime.Now;
                                        entRecon.CreatedDate = DateTime.Now;
                                        entRecon.CreatedBy = currentUser;
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }

                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    
                                    entRecon.EntitlementDescription = role.Description;
                                    entRecon.AccountReconcilationId = acctRecon.Id;
                                    entRecon.EndpointType = EndpointType.IDMS;
                                    entRecon.EndpointSystemId = acctRecon.EndpointSystemId;
                                    entRecon.EndpointUserId = acctRecon.EndpointUserId;
                                    entRecon.UserId = acctRecon.UserId;
                                    entRecon.EntitlementType = (int)EntitlementType.Role;
                                    entRecon.AccountName = acctRecon.AccountName;
                                    entRecon.Entitlement = role.Name;
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.ValidToDate = userValidTo;
                                    entRecon.Save(currentUser);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        //else
                        //{
                        //    if (acctRecon != null)
                        //    {
                        //        var entRecons = EntitlementReconcilation.GetByAccountReconcilationId(acctRecon.Id, acctRecon.EndpointSystemId).Where(a => a.EntitlementType == (int)EntitlementType.Role);
                        //        foreach (var entRecon in entRecons)
                        //        {
                        //            EntitlementReconcilation.UpdateException(entRecon, false);
                        //            entRecon.LastModifiedBy = currentUser;
                        //            entRecon.LastModifiedDate = DateTime.Now;
                        //            entRecon.Save(currentUser);
                        //        }
                        //    }
                        //}

                        if (usr.Groups != null && usr.Groups.Count > 0)
                        {
                            foreach (IACMGroupInfo grp in usr.Groups)
                            {
                                try
                                {
                                    if (acctRecon == null) continue;
                                    entRecon = EntitlementReconcilation.GetByUser(usr.Id, usr.EndPointSystemId, grp.Name.Trim(), EntitlementType.Group);

                                    if (entRecon == null)
                                    {
                                        entRecon = new EntitlementReconcilation();
                                        //entRecon.Id = 0;
                                        entRecon.EnteredDate = DateTime.Now;
                                        entRecon.CreatedDate = DateTime.Now;
                                        entRecon.CreatedBy = currentUser;
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }

                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    entRecon.EntitlementDescription = grp.Description;
                                    entRecon.AccountReconcilationId = acctRecon.Id;
                                    entRecon.EndpointType = EndpointType.IDMS;
                                    entRecon.EndpointSystemId = acctRecon.EndpointSystemId;
                                    entRecon.EndpointUserId = acctRecon.EndpointUserId;
                                    entRecon.UserId = acctRecon.UserId;
                                    entRecon.EntitlementType = (int)EntitlementType.Group;
                                    entRecon.AccountName = acctRecon.AccountName;
                                    entRecon.Entitlement = grp.Name;
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = currentUser;
                                    entRecon.ValidToDate = userValidTo;
                                    entRecon.Save(currentUser);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        //else
                        //{
                        //    if (acctRecon != null)
                        //    {
                        //        var entRecons = EntitlementReconcilation.GetByAccountReconcilationId(acctRecon.Id, acctRecon.EndpointSystemId).Where(a => a.EntitlementType == (int)EntitlementType.Group);
                        //        foreach (var entRecon in entRecons)
                        //        {
                        //            EntitlementReconcilation.UpdateException(entRecon, false);
                        //            entRecon.LastModifiedBy = "System";
                        //            entRecon.LastModifiedDate = DateTime.Now;
                        //            entRecon.Save(currentUser);
                        //        }
                        //    }
                        //}
                }
                catch (Exception ex)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Error Occured@SaveUser. User Id - {usr.Id}, User Display Name - {usr.DisplayName}", ex);
                }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                users.Clear();
                users = null;
                entRecon = null;
                acctRecon = null;
            }
        }
        public static DateTime? GetUserValidTo(User usr)
        {
            DateTime? date = null;
            if(usr != null)
            {
                if (usr.FullTimeEmployee)
                {
                    date = usr.TerminationDate.HasValue ? usr.TerminationDate : DateTime.MaxValue;
                }
                else
                {
                    date = usr.TerminationDate;
                }
            }
            return date;
        }
        public static void SaveUsers(IList<IACMUserInfo> list, long EndpointSystemId, string currentUser, User user)
        {
            //var ep = EndPoint.GetById(EndpointSystemId);
            AccountReconcilation acctRecon = null;
            foreach (IACMUserInfo usr in list)
            {
                try
                {
                    acctRecon = AccountReconcilation.GetByUser(EndpointSystemId, usr.Id, false);
                    if (acctRecon == null)
                    {
                        acctRecon = new AccountReconcilation();
                        AccountReconcilation.UpdateException(acctRecon, true);
                        acctRecon.EndpointUserId = usr.Id;
                        acctRecon.AccountCreatedDate = DateTime.Now;
                    }

                    acctRecon.EndpointSystemId = usr.EndPointSystemId;
                    acctRecon.FullName = usr.DisplayName;
                    acctRecon.AccountName = usr.AccountName;
                    acctRecon.LastModifiedDate = DateTime.Now;
                    acctRecon.UserId = user.Id;
                    acctRecon.IsLocked = usr.IsDisabled;
                    acctRecon.ValidToDate = !user.FullTimeEmployee && user.TerminationDate.HasValue ? user.TerminationDate.Value : DateTime.MaxValue;
                    acctRecon.Save(currentUser);
                }
                catch (Exception ex)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Error Occured@SaveUser. User Id - {usr.Id}, User Display Name - {usr.DisplayName}", ex);
                }
            }
        }

        public static IACMUserInfo GetByEmployeeIdAndUserName(string employeeId, string userName)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.IACMUserInfo>(tranaction).Get(usr => usr.UserName == userName && usr.EmployeeId == employeeId,
                    usr => usr.Roles, usr => usr.Groups).FirstOrDefault());
            }
        }

        public static IACMUserInfo GetByIdentificationNo(string identificationNo)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.IACMUserInfo>(tranaction).Get(usr => usr.NRIC == identificationNo).FirstOrDefault());
            }
        }

        public static IACMUserInfo GetByEmployeeId(string employeeId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.IACMUserInfo>(tranaction).Get(usr => usr.EmployeeId == employeeId).FirstOrDefault());
            }
        }

        public static IACMUserInfo GetByUserName(string userName, bool eagerLoad = false)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.IACMUserInfo>(tranaction).Get(usr => usr.UserName == userName, eagerLoad).FirstOrDefault());
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
                bldr.AppendFormat(string.Format("(DI_IACM_UserInfo.{0}=DI_M_User.{1}  and DI_IACM_UserInfo.{2} != '' and DI_IACM_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));
            return string.Empty;
        }

        public static void MapWithMasterUserForReconTable(EndPoint endPoint, bool mapall, long discoveryProcessId, string commaSeparatedUserIds = null)
        {
            try
            {
                var considerInActiveAccountForRecon = AccountReconcilation.GetCanConsiderInActiveFilterCondition();;
                StringBuilder bldR = new StringBuilder();
                string strJoin = GetJoinQry(endPoint.Id, mapall);
                if (strJoin == string.Empty)
                    return;
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_IACM_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_IACM_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_IACM_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_User.Id <> 1 AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString(), considerInActiveAccountForRecon);

                if (!string.IsNullOrEmpty(commaSeparatedUserIds))
                {
                    bldR.Append($" and DI_M_User.Id in ( { commaSeparatedUserIds } ) ");
                }

                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new IACMUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
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
        public static void MapWithMasterUserForEntitlementReconTable(EndPoint endPoint, bool mapall, long discoveryProcessId, string commaSeparatedUserIds = null)
        {
            try
            {
                StringBuilder bldR = new StringBuilder();
                string strJoin = GetJoinQry(endPoint.Id, mapall);
                if (strJoin == string.Empty)
                    return;
                bldR.Append("Update DI_M_EntitlementReconcilation set DI_M_EntitlementReconcilation.UserId = DI_M_User.Id FROM DI_IACM_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_EntitlementReconcilation on DI_M_EntitlementReconcilation.EndpointUserId = DI_IACM_UserInfo.Id");
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.Id = DI_M_EntitlementReconcilation.AccountReconcilationId");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_IACM_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND DI_M_EntitlementReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString());

                if (!string.IsNullOrEmpty(commaSeparatedUserIds))
                {
                    bldR.Append($" and DI_M_User.Id in ( { commaSeparatedUserIds } ) ");
                }

                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new ADSUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
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

        public static IACMUserInfo GetByIdIncludeRolesGroups(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.IACMUserInfo>(tranaction)
                      .Get(adsUser => adsUser.Id == id, adsUser => adsUser.Roles, adsUser => adsUser.Groups).FirstOrDefault());
            }
        }

        public static bool AddEntitlement(long epUserId, EntitlementType _entitlementType, string entitlements)
        {
            try
            {
                var systemUser = ConfigurationMaster.GetByConfigKey("ServiceUser", Product.IACM).ConfigValue;
                using (DataAccessTransaction tranaction = new DataAccessTransaction(string.IsNullOrEmpty(systemUser) ? "System" : systemUser))
                {
                    IACMUserInfo currentADUser = IACMUserInfo.GetByIdIncludeRolesGroups(epUserId);

                    if (currentADUser == null)
                        return false;
                    if (entitlements != null)
                    {
                        if (_entitlementType == EntitlementType.Role)
                        {
                            foreach (var item in entitlements.Split(','))
                            {
                                var grp = currentADUser.Roles.Find(g => g.Name.ToUpper() == item.ToUpper());

                                if (grp == null)
                                {
                                    var newGrpToAdd = IACMRoleInfo.GetByRoleName(item);
                                    if (newGrpToAdd != null)
                                    {
                                        new IACMUserInfoRepository(tranaction).InsertIACMUserRoleMapping(currentADUser.Id, newGrpToAdd.Id);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in entitlements.Split(','))
                            {
                                var grp = currentADUser.Groups.Find(g => g.Name.ToUpper() == item.ToUpper());

                                if (grp == null)
                                {
                                    var newGrpToAdd = IACMGroupInfo.GetByGroupName(item);
                                    if (newGrpToAdd != null)
                                    {
                                        new IACMUserInfoRepository(tranaction).InsertIACMUserGroupMapping(currentADUser.Id, newGrpToAdd.Id);
                                    }
                                }
                            }
                        }
                    }

                    tranaction.Commit();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
        public static bool RemoveEntitlement(long epUserId, EntitlementType _entitlementType, string entitlements)
        {
            try
            {
                var systemUser = ConfigurationMaster.GetByConfigKey("ServiceUser", Product.IACM).ConfigValue;
                using (DataAccessTransaction tranaction = new DataAccessTransaction(string.IsNullOrEmpty(systemUser) ? "System" : systemUser))
                {
                    IACMUserInfo currentADUser = IACMUserInfo.GetByIdIncludeRolesGroups(epUserId);

                    if (currentADUser == null)
                        return false;
                    if (entitlements != null)
                    {
                        if (_entitlementType == EntitlementType.Role)
                        {
                            foreach (var item in entitlements.Split(','))
                            {
                                var grp = currentADUser.Roles.Find(g => g.Name.ToUpper() == item.ToUpper());

                                if (grp != null)
                                {
                                    var newGrpToAdd = IACMRoleInfo.GetByRoleName(item);
                                    if (newGrpToAdd != null)
                                    {
                                        new IACMUserInfoRepository(tranaction).DeleteIACMUserRoleMapping(currentADUser.Id, newGrpToAdd.Id);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in entitlements.Split(','))
                            {
                                var grp = currentADUser.Groups.Find(g => g.Name.ToUpper() == item.ToUpper());

                                if (grp != null)
                                {
                                    var newGrpToAdd = IACMGroupInfo.GetByGroupName(item);
                                    if (newGrpToAdd != null)
                                    {
                                        new IACMUserInfoRepository(tranaction).DeleteIACMUserGroupMapping(currentADUser.Id, newGrpToAdd.Id);
                                    }
                                }
                            }
                        }
                    }

                    tranaction.Commit();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
        public static List<IACMUserInfo> GetActiveAccounts(long dIUserId, long endPointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new IACMUserInfoRepository(transaction).GetActiveAccounts(dIUserId, endPointSystemId, accountType));
            }
        }

        public static IACMUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new IACMUserInfoRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }
    }
}
