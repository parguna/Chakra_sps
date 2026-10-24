using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DI.Chakra.DomainModel
{
    public class SAPEPUserInfo : DomainModelBase<SAPEPUserInfo, EntityModel.SAPEPUserInfo>, IEndpointUserInfo
    {

        public SAPEPUserInfo()
        {
            this.SAPEPGroups = new List<SAPEPGroupInfo>();
            this.SAPEPRoles = new List<SAPEPRoleInfo>();
            Roles = new List<string>();
            Groups = new List<string>();
        }

        // public Nullable<long> WogUserId { get; set; }
        public long EndPointSystemId { get; set; }

        public string UserName { get; set; }

        public string UniqueName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Salutation { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Department { get; set; }

        public string JobTitle { get; set; }

        public string Title { get; set; }

        public string Company { get; set; }

        public string Cellphone { get; set; }

        public string Telephone { get; set; }

        public string Fax { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Zip { get; set; }

        public string Currency { get; set; }

        public string Language { get; set; }

        public string Password { get; set; }

        public Nullable<System.DateTime> UserLastLogOnTime { get; set; }

        public Nullable<System.DateTime> StartDate { get; set; }

        public Nullable<System.DateTime> EndDate { get; set; }

        public Nullable<bool> IsCompanyUser { get; set; }

        public string UniqueId { get; set; }

        public int PwdResetCnt { get; set; }

        public string Extra1 { get; set; }

        public string Extra2 { get; set; }

        public string Extra3 { get; set; }

        public DateTime? IACMLastPasswordChange { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public List<string> Groups;

        public List<string> Roles;

        public EndPoint EndPointSystem { get; set; }

        public IList<SAPEPRoleInfo> SAPEPRoles { get; set; }

        public List<SAPEPGroupInfo> SAPEPGroups { get; set; }

        public override string ToString()
        {
            return this.UserName;
        }
        public static SAPEPUserInfo CreateSAPEPUserInfo()
        {
            SAPEPUserInfo sapEPUserInfo = new SAPEPUserInfo();

            return sapEPUserInfo;
        }

        public override bool Equals(object obj)
        {
            SAPEPUserInfo other = obj as SAPEPUserInfo;
            if (obj != null)
            {
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.UserName.GetHashCode();
        }


        public string AccountName
        {
            get { return this.UserName; }
        }

        public string AccountPassword
        {
            get
            {
                return this.Password;
            }
        }
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (SAPEPRoles != null)
                {
                    foreach (var item in SAPEPRoles)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }

        public static List<SAPEPUserInfo> GetByEndPointSystemId(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction).
                    Get(c => c.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static List<SAPEPUserInfo> GetActiveAccountByDIUserId(long diUserId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPEPUserInfoRepository(transaction).GetActiveAccountByDIUserId(diUserId));
            }
        }

        public static List<SAPEPUserInfo> GetActiveAccountByDIUserIdByEndpointId(long diUserId, long endpointId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPEPUserInfoRepository(transaction).GetPrimaryAccountsByDIUserId(diUserId, endpointId));
            }
        }

        public static List<SAPEPUserInfo> GetActiveAccounts(long diUserId, long endpointId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPEPUserInfoRepository(transaction).GetActiveAccounts(diUserId, endpointId, accountType));
            }
        }

        // Remove during end to end implementation if not required
        public static SAPEPUserInfo GetBysAMAccountNameEndPointSystemId(string userName, long endPointId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction).
                    Get(sapepUsrInfo =>
                        sapepUsrInfo.UserName == userName &&
                        sapepUsrInfo.EndPointSystemId == endPointId).
                        FirstOrDefault());
            }
        }

        public static Dictionary<string, string> GetGroupSAPEPForRemove(int EndpointUserID)
        {
            var finalResult = new Dictionary<string, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var result = new RepositoryBase<EntityModel.SAPEPUserInfo>(dataAccessTransaction).
                     Get(sapepUsrInfo =>
                         sapepUsrInfo.Id == EndpointUserID,
                         sapepUsrInfo => sapepUsrInfo.SAPEPGroups
                         ).FirstOrDefault();
                if (result != null)
                {
                    finalResult = result.SAPEPGroups.Where(grp => grp.IsXLUploaded && !string.IsNullOrWhiteSpace(grp.GroupUniqueName)).Select(grp => new { grp.GroupUniqueName }).Distinct()
                          .Select(grp => new { Key = grp.GroupUniqueName, Value = grp.GroupUniqueName })
                          .OrderBy(item => item.Value)
                          .Distinct()
                          .ToDictionary(c => c.Key, c => c.Value);
                }
                return finalResult;
            }
        }

        public static Dictionary<string, string> GetRolesSAPEPForRemove(long EndpointUserID)
        {
            var finalResult = new Dictionary<string, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var result = new RepositoryBase<EntityModel.SAPEPUserInfo>(dataAccessTransaction).
                    Get(sapepUsrInfo =>
                         sapepUsrInfo.Id == EndpointUserID,
                         sapepUsrInfo => sapepUsrInfo.SAPEPRoles).FirstOrDefault();

                if (result != null)
                {
                    finalResult = result.SAPEPRoles.Where(grp => grp.IsXLUploaded)
                          .Select(grp => new { Key = grp.UniqueRoleName, Value = grp.RoleName })
                          .OrderBy(item => item.Value)
                          .Distinct()
                          .ToDictionary(c => c.Key, c => c.Value);
                }
                return finalResult;
            }
        }
        public static SAPEPUserInfo GetByIdEndPointSystemId(long id, long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction).
                    Get(sapepUsrInfo =>
                        sapepUsrInfo.Id == id &&
                        sapepUsrInfo.EndPointSystemId == endpointSystemId).
                        FirstOrDefault());
            }
        }

        // Remove during end to end implementation if not required
        public static SAPEPUserInfo GetByIdWithNavigation(long id)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction).
                    Get(sapepUsrInfo =>
                        sapepUsrInfo.Id == id,
                        sapepUsrInfo => sapepUsrInfo.EndPointSystem,
                        sapepUsrInfo => sapepUsrInfo.EndPointSystem.EndPointSystemType,
                        sapepUsrInfo => sapepUsrInfo.SAPEPGroups,
                        sapepUsrInfo => sapepUsrInfo.SAPEPRoles).
                        FirstOrDefault());
            }
        }

        public static List<SAPEPUserInfo> GetByEndPointSystemIdWithEndPointSystem(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction).
                    Get(sapepUsrInfo =>
                        sapepUsrInfo.EndPointSystemId == endpointSystemId,
                        sapepUsrInfo => sapepUsrInfo.EndPointSystem).
                        ToList());
            }
        }

        public static SAPEPUserInfo GetByUserName(string userName, long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction).
                    Get(sapepUsrInfo =>
                        sapepUsrInfo.UserName == userName &&
                        sapepUsrInfo.EndPointSystemId == endpointSystemId).
                        FirstOrDefault());
            }
        }

        public static List<SAPEPUserInfo> GetActiveByUserName(string userName, List<long> endpointSystemIds)
        {
            using (var transaction = new DataAccessTransaction(false))
            {

                return ORMapList(
                    new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction)
                     .Get(sapepUsrInfo => sapepUsrInfo.UserName == userName &&
                       endpointSystemIds.Contains(sapepUsrInfo.EndPointSystemId)).ToList()
                       );
            }
        }

        public static List<SAPEPUserInfo> GetDeletedUsers(long endpointSystemId, Process discoveryProcess)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                List<SAPEPUserInfo> listDeletedUsers = new List<SAPEPUserInfo>();
                if (discoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction)
                       .Get(objUser => objUser.EndPointSystemId == endpointSystemId &&
                                       (objUser.LastModifiedDate < discoveryProcess.ProcessStartAt ||
                                           (objUser.CreatedDate < discoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                                       )).ToList());
                }
                return listDeletedUsers;
            }
        }

        //public static SAPEPUserInfo GetByDIUserId(long diUserId, long endpointSystemId)
        //{
        //    using (var transaction = new DataAccessTransaction())
        //    {
        //        return ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction).
        //            Get(sapepUsrInfo =>
        //                sapepUsrInfo.DIUserId == diUserId &&
        //                sapepUsrInfo.EndPointSystemId == endpointSystemId &&
        //                sapepUsrInfo.AccountType == AccountType.User &&
        //                sapepUsrInfo.IsLocked).
        //                FirstOrDefault());
        //    }
        //}

        public static List<SAPEPUserInfo> GetByDIUserId(long diUserId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPEPUserInfoRepository(transaction).GetByDIUserId(diUserId));
            }
        }

        //public static List<SAPEPUserInfo> GetUserAccountsByEndPointSystemId(long diUserId, long endpointSystemId)
        //{
        //    using (var transaction = new DataAccessTransaction())
        //    {
        //        return ORMapList(new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction).
        //            Get(sapepUsrInfo =>
        //                sapepUsrInfo.DIUserId == diUserId &&
        //                (sapepUsrInfo.AccountType == AccountType.User ||
        //                sapepUsrInfo.AccountType == AccountType.Secondary) &&
        //                sapepUsrInfo.EndPointSystemId == endpointSystemId &&
        //                !sapepUsrInfo.IsLocked).
        //                ToList());
        //    }
        //}

        // Returns dynamic type
        //public static dynamic GetAllSAPEPUser(long endPtId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return new SAPEPUserInfoRepository(dataAccessTransaction).GetAllSAPEPUser(endPtId);
        //    }
        //}

        //public static List<SAPEPUserInfo> GetUnlockedSAPEPUsersByEndPointSystemId(long endpointSystemId)
        //{
        //    using (var transaction = new DataAccessTransaction())
        //    {
        //        return ORMapList(new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction)
        //               .Get(sapepUsrInfo =>
        //                    sapepUsrInfo.EndPointSystemId == endpointSystemId &&
        //                    !sapepUsrInfo.IsLocked,
        //                    item => item.EndPointSystem,
        //                    item => item.EndPointSystem.EndPointSystemType).
        //                    ToList());
        //    }
        //}
        public static List<long> GetIdByUserName(string userName, long endpointsystemid)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.SAPEPUserInfo>(transaction).
                    Get(ui =>
                        ui.UserName == userName &&
                        ui.EndPointSystemId == endpointsystemid).
                        Select(ui => ui.Id).ToList();
            }
        }
        public static bool DoesExists(string userName, long endpointsystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.SAPEPUserInfo>(dataAccessTransaction).Any(item => item.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase) && item.EndPointSystemId == endpointsystemId);
            }
        }

        public static SAPEPUserInfo GetByIdIncludeAllType(long Id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(tranaction)
                       .GetById(Id, sapUser => sapUser.SAPEPGroups, sapUser => sapUser.SAPEPRoles));
            }
        }

        public static SAPEPUserInfo GetByIdIncludeGroup(long Id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(tranaction)
                       .GetById(Id, sapUser => sapUser.SAPEPGroups));
            }
        }

        public static SAPEPUserInfo GetByIdIncludeRole(long Id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(tranaction)
                       .GetById(Id, sapUser => sapUser.SAPEPRoles));
            }
        }

        public static Dictionary<long, List<string>> GetUserGroupsByEndpointSystemId(long endpointId)
        {
            Dictionary<long, List<string>> result = new Dictionary<long, List<string>>();
            DataTable dataTable;
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                string query = $@"SELECT      SU.Id, SG.GroupName
                                FROM        DI_SAPEP_UserInfo AS SU INNER JOIN
                                            SAPEPUserInfoSAPEPGroupInfo AS UG ON SU.Id = UG.SAPEPUserInfo_Id INNER JOIN
                                            DI_SAPEP_GroupInfo AS SG ON SG.Id = UG.SAPEPGroupInfo_Id
                                WHERE        (SU.EndPointSystemId = {endpointId})";

                dataTable = new RepositoryBase<EntityModel.SAPEPUserInfo>(tranaction).GetDataTableByQuery(query);
            }

            if (dataTable == null || dataTable.Rows.Count == 0)
                return result;

            long id;
            string group;
            foreach (DataRow datarow in dataTable.Rows)
            {
                id = Convert.ToInt64(datarow["Id"]);
                group = Convert.ToString(datarow["GroupName"]).Trim();

                if (!result.ContainsKey(id))
                    result.Add(id, new List<string>());

                if (!string.IsNullOrWhiteSpace(group))
                    result[id].Add(group);
            }
            return result;
        }
        public static Dictionary<long, List<string>> GetUserRolesByEndpointSystemId(long endpointId)
        {
            Dictionary<long, List<string>> result = new Dictionary<long, List<string>>();
            DataTable dataTable;
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                string query = $@"SELECT      SU.Id, SR.RoleName
                                    FROM        DI_SAPEP_UserInfo AS SU INNER JOIN
                                                SAPEPRoleInfoSAPEPUserInfo AS UR ON SU.Id = UR.SAPEPUserInfo_Id JOIN
                                                DI_SAPEP_RoleInfo AS SR ON SR.Id = UR.SAPEPRoleInfo_Id
                                WHERE        (SU.EndPointSystemId = {endpointId})";

                dataTable = new RepositoryBase<EntityModel.SAPEPUserInfo>(tranaction).GetDataTableByQuery(query);
            }

            if (dataTable == null || dataTable.Rows.Count == 0)
                return result;

            long id;
            string role;
            foreach (DataRow datarow in dataTable.Rows)
            {
                id = Convert.ToInt64(datarow["Id"]);
                role = Convert.ToString(datarow["RoleName"]).Trim();

                if (!result.ContainsKey(id))
                    result.Add(id, new List<string>());

                if (!string.IsNullOrWhiteSpace(role))
                    result[id].Add(role);
            }
            return result;
        }

        public static void SaveUsers(List<SAPEPUserInfo> list, long EndpointSystemId, string currentUser, long endpointTypeId)
        {

            SAPEPUserInfo sapepexistingUser = null;

            foreach (SAPEPUserInfo usr in list)
            {
                try
                {
                    bool isActive = true;

                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
                    {
                        sapepexistingUser = ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(dataAccessTransaction).Get(c => c.UserName == usr.UserName && c.EndPointSystemId == EndpointSystemId).FirstOrDefault());
                    }

                    if (sapepexistingUser == null)
                    {
                        usr.Save("System");
                    }
                    else
                    {

                        usr.Id = sapepexistingUser.Id;
                        usr.CreatedDate = sapepexistingUser.CreatedDate;
                        usr.CreatedBy = sapepexistingUser.CreatedBy;
                        usr.LastModifiedDate = DateTime.Now;
                        usr.LastModifiedBy = "System";
                        usr.Save("System");

                    }
                    //Fixed Duplicate record creation
                    AccountReconcilation acctRecon = AccountReconcilation.GetByAccountName(usr.UserName, EndpointSystemId);
                    if (acctRecon == null)
                    {
                        acctRecon = new AccountReconcilation();
                        acctRecon.ExceptionType = ExceptionTypes.Added;
                        acctRecon.AccountType = AccountType.None;
                        acctRecon.EndpointSystemId = EndpointSystemId;
                        acctRecon.AccountCreatedDate = usr.StartDate;
                    }

                    if (usr.EndDate.HasValue)
                        isActive = usr.EndDate > DateTime.Now;

                    if (acctRecon.IsDeleted && isActive)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been added without any request made in the system");
                        // if deleted account and again added at endpoint
                        acctRecon.ExceptionType = ExceptionTypes.Added;
                        acctRecon.IsDeleted = false;
                        acctRecon.IsLocked = false;
                        acctRecon.DeletedDate = null;
                        acctRecon.LastAttestedOn = null;
                    }
                    else if (!acctRecon.IsDeleted && !isActive)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been removed without any request made in the system");
                        acctRecon.IsDeleted = true;
                        acctRecon.IsLocked = true;
                        acctRecon.ExceptionType = ExceptionTypes.Removed;
                        acctRecon.DeletedDate = DateTime.Now;
                    }

                    acctRecon.ValidToDate = usr.EndDate.HasValue ? usr.EndDate : acctRecon.ValidToDate;
                    acctRecon.FullName = usr.DisplayName;
                    acctRecon.AccountName = usr.UserName;
                    acctRecon.LastModifiedDate = DateTime.Now;
                    acctRecon.EndpointUserId = usr.Id;
                    acctRecon.Save("System");

                }
                catch (Exception ex)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, string.Format("Exception Occured at SaveUsers {0}", usr.UserName), ex);
                }
            }
        }

        public static void SaveUsers(List<SAPEPUserInfo> list, long EndpointSystemId, string currentUser, long endpointTypeId, long DIUserId, AccountType acctType)
        {

            SAPEPUserInfo sapepexistingUser = null;

            foreach (SAPEPUserInfo usr in list)
            {
                try
                {
                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
                    {
                        sapepexistingUser = ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(dataAccessTransaction).Get(c => c.UserName == usr.UserName && c.EndPointSystemId == EndpointSystemId).FirstOrDefault());
                    }

                    if (sapepexistingUser == null)
                    {
                        usr.Save("System");
                    }
                    else
                    {
                        AccountReconcilation.SaveAttributeException(sapepexistingUser, usr, endpointTypeId, null, usr.AccountName, currentUser);
                        usr.Id = sapepexistingUser.Id;
                        usr.CreatedDate = sapepexistingUser.CreatedDate;
                        usr.CreatedBy = sapepexistingUser.CreatedBy;
                        usr.Save("System");

                    }

                    AccountReconcilation acctRecon = AccountReconcilation.GetByUser(EndpointSystemId, usr.Id);
                    if (acctRecon == null)
                    {
                        acctRecon = new AccountReconcilation();
                        acctRecon.ExceptionType = ExceptionTypes.Added;
                        acctRecon.AccountType = acctType;
                        acctRecon.EndpointSystemId = EndpointSystemId;
                        acctRecon.EndpointUserId = usr.Id;
                        acctRecon.AccountCreatedDate = usr.StartDate;
                        acctRecon.ValidToDate = usr.EndDate;
                    }
                    acctRecon.ValidFromDate = usr.StartDate;
                    acctRecon.FullName = usr.DisplayName;
                    acctRecon.AccountName = usr.UserName;
                    acctRecon.LastModifiedDate = DateTime.Now;
                    acctRecon.LastModifiedBy = "System";
                    acctRecon.UserId = DIUserId;
                    acctRecon.OffboardingAccountAction = (acctRecon.AccountType == AccountType.User || acctRecon.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;
                    acctRecon.Save("System");
                }
                catch (Exception ex)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, string.Format("Exception Occured at SaveUsers {0}", usr.UserName), ex);
                }
            }
        }


        //public static void UpdatePasswordresetcount(long EndpointId, string userName, string currentUser)
        //{
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
        //    {
        //        SAPEPUserInfo userInfo = ORMap(new RepositoryBase<EntityModel.SAPEPUserInfo>(dataAccessTransaction).Get(usr => usr.UserName == userName && usr.EndPointSystemId == EndpointId).FirstOrDefault());
        //        if (userInfo != null)
        //        {
        //            int _pwdResetCnt = userInfo.PwdResetCnt;
        //            if (_pwdResetCnt == null)
        //                userInfo.PwdResetCnt = 1;
        //            else
        //                userInfo.PwdResetCnt++;
        //            userInfo.Save(dataAccessTransaction);
        //        }
        //    }
        //}

        public static void SaveUserRolesMap(Dictionary<long, string> dicUsers, Dictionary<long, string> dicRoles, Dictionary<string, System.Collections.ArrayList> dicroleUsers, long EndpointsystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (dicroleUsers.Count() > 0)
                {
                    string query = string.Format("Delete from SAPEPRoleInfoSAPEPUserInfo where EndpointSystemId={0}", EndpointsystemId);
                    new SAPEPUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(query));
                    dataAccessTransaction.Commit();
                }
            }

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPEPUserInfoRepository(dataAccessTransaction).SaveUserRolesMap(dicUsers, dicRoles, dicroleUsers, EndpointsystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void SaveUserGroupMap(Dictionary<long, string> dicUsers, Dictionary<long, string> dicGroups, Dictionary<string, System.Collections.ArrayList> dicGroupUsers, long EndpointsystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (dicGroupUsers.Count > 0)
                {
                    string query = string.Format("Delete from SAPEPUserInfoSAPEPGroupInfo where EndpointSystemId={0}", EndpointsystemId);
                    new SAPEPUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(query));
                    dataAccessTransaction.Commit();
                }
            }
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPEPUserInfoRepository(dataAccessTransaction).SaveUserGroupMap(dicUsers, dicGroups, dicGroupUsers, EndpointsystemId);
                dataAccessTransaction.Commit();
            }
        }




        //public static void MapWithMasterUser(EndPoint endPoint, bool mapall)
        //{
        //    StringBuilder bldR = new StringBuilder();
        //    string strJoin = GetJoinQry(endPoint.Id, mapall);
        //    if (strJoin == string.Empty)
        //        return;

        //    bldR.AppendFormat("Update A set A.DIUserId = DI_M_User.Id, A.MappingType='System', A.AccountType=1 FROM DI_SAPEP_UserInfo as A JOIN DI_M_User on");
        //    bldR.Append(" ");
        //    bldR.Append(strJoin);
        //    bldR.Append(" ");
        //    bldR.AppendFormat("WHERE A.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND ISNULL(MappingType,'') <> 'Manual'", endPoint.Id.ToString());
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        new PeopleSoftUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
        //        dataAccessTransaction.Commit();
        //    }
        //}

        public static void MapWithMasterUserForReconTable(EndPoint endPoint, bool mapall)
        {
            var considerInActiveAccountForRecon = AccountReconcilation.GetCanConsiderInActiveFilterCondition();
            StringBuilder bldR = new StringBuilder();
            string strJoin = GetJoinQry(endPoint.Id, mapall);
            if (strJoin == string.Empty)
                return;
            bldR.AppendFormat("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_SAPEP_UserInfo as A JOIN DI_M_User on");
            bldR.Append(" ");
            bldR.Append(strJoin);
            bldR.Append(" ");
            bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = A.Id");
            bldR.Append(" ");
            bldR.AppendFormat("WHERE A.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_User.Id <> 1 AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString(), considerInActiveAccountForRecon);
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPEPUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
                dataAccessTransaction.Commit();
            }
        }

        private static string GetJoinQry(long systemId, bool mapall)
        {
            string clauseType = "AND";
            if (!mapall)
            {
                clauseType = "OR";
            }
            StringBuilder bldr = new StringBuilder();
            //TO DO: Get Attribute Mapping Columns from DI_M_AttributeMapping

            IList<AttributeMappingColumns> listAttributes = AttributeMappingColumns.GetByEndpointSystemId(systemId);
            foreach (AttributeMappingColumns attDto in listAttributes)
            {
                bldr.AppendFormat(string.Format("(A.{0}=DI_M_User.{1}  and A.{2} != '' and A.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));
            return string.Empty;
        }
        public static void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            SAPEPUserInfo sapEPUserInfo = SAPEPUserInfo.GetBysAMAccountNameEndPointSystemId(userName, endpointId);
            if (sapEPUserInfo != null)
            {
                if (sapEPUserInfo.PwdResetCnt > 0)
                    sapEPUserInfo.PwdResetCnt++;
                else
                    sapEPUserInfo.PwdResetCnt = 1;
                sapEPUserInfo.Save(currentUser);
            }
        }

        public static SAPEPUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPEPUserInfoRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }

        public static SAPEPUserInfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPEPUserInfoRepository(transaction).GetLockedAccountByName(accountName, diuserId, endpointSystemId, accountType));
            }
        }

        public static void DeleteSAPEPUserGroupMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPEPUserInfoRepository(dataAccessTransaction).DeleteSAPEPUserGroupMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPEPUserRoleMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPEPUserInfoRepository(dataAccessTransaction).DeleteSAPEPUserRoleMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPEPUserRoleMapByRoleId(long endpointSystemId, long roleId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPEPUserInfoRepository(dataAccessTransaction).DeleteSAPEPUserRoleMapByRoleId(endpointSystemId, roleId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPEPUserGroupMapByGroupId(long endpointSystemId, long groupId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPEPUserInfoRepository(dataAccessTransaction).DeleteSAPEPUserGroupMapByGroupId(endpointSystemId, groupId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPEPUserGroupMap(SAPEPUserInfo userInfo)
        {
            if (userInfo != null && userInfo.Groups.Count() > 0)
            {
                foreach (var grp in userInfo.Groups)
                {
                    var sapepgrp = SAPEPGroupInfo.GetByName(userInfo.EndPointSystemId, grp);
                    if (sapepgrp != null)
                    {
                        using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                        {
                            new SAPEPUserInfoRepository(dataAccessTransaction).DeleteSAPEPUserGroupMap((long)userInfo.EndPointSystemId, userInfo.Id, sapepgrp.Id);
                            dataAccessTransaction.Commit();
                        }
                    }
                }
            }
        }

        public static void DeleteSAPEPUserRoleMap(SAPEPUserInfo userInfo)
        {
            if (userInfo != null && userInfo.Roles.Count() > 0)
            {
                foreach (var role in userInfo.Roles)
                {
                    var sapeprole = SAPEPRoleInfo.GetByUniqueName(userInfo.EndPointSystemId, role);
                    if (sapeprole != null)
                    {
                        using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                        {
                            new SAPEPUserInfoRepository(dataAccessTransaction).DeleteSAPEPUserRoleMap((long)userInfo.EndPointSystemId, userInfo.Id, sapeprole.Id);
                            dataAccessTransaction.Commit();
                        }
                    }
                }
            }
        }

        public static void InsertSAPUserRoleMapping(long endPointSystemId, long userId, long roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPEPUserInfoRepository(dataAccessTransaction).InsertSAPEPUserRoleMapping(endPointSystemId, userId, roleId);
                dataAccessTransaction.Commit();
            }
        }

        public static void InsertSAPUserGroupMapping(long endPointSystemId, long userId, long groupId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPEPUserInfoRepository(dataAccessTransaction).InsertSAPEPUserGroupMapping(endPointSystemId, userId, groupId);
                dataAccessTransaction.Commit();
            }
        }

        public static bool AddEntitlement(long epUserId, EntitlementType entitlementType, string entitlements)
        {
            try
            {
                if (entitlementType == EntitlementType.Role)
                {
                    SAPEPUserInfo currentSAPUser = SAPEPUserInfo.GetByIdIncludeRole(epUserId);

                    if (currentSAPUser == null)
                        return false;

                    foreach (var item in entitlements.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            var role = currentSAPUser.SAPEPRoles.FirstOrDefault(g => g.UniqueRoleName.ToUpper() == item.ToUpper());

                            if (role == null)
                            {
                                var newRoleToAdd = DomainModel.SAPEPRoleInfo.GetByUniqueName(currentSAPUser.EndPointSystemId, item);
                                if (newRoleToAdd != null)
                                    InsertSAPUserRoleMapping(currentSAPUser.EndPointSystemId, currentSAPUser.Id, newRoleToAdd.Id);
                            }
                        }
                    }
                }
                else if (entitlementType == EntitlementType.Group)
                {
                    SAPEPUserInfo currentSAPUser = SAPEPUserInfo.GetByIdIncludeGroup(epUserId);

                    if (currentSAPUser == null)
                        return false;

                    foreach (var item in entitlements.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            var role = currentSAPUser.SAPEPGroups.FirstOrDefault(g => g.GroupName.ToUpper() == item.ToUpper());

                            if (role == null)
                            {
                                var newGrpToAdd = DomainModel.SAPEPGroupInfo.GetByName(currentSAPUser.EndPointSystemId, item);
                                if (newGrpToAdd != null)
                                    InsertSAPUserGroupMapping(currentSAPUser.EndPointSystemId, currentSAPUser.Id, newGrpToAdd.Id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }
    }
}
