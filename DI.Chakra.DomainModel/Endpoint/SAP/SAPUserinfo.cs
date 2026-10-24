using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using DI.Chakra.DomainModel.Presentation;

namespace DI.Chakra.DomainModel
{
    public class SAPUserinfo : DomainModelBase<SAPUserinfo, EntityModel.SAPUserinfo>, IEndpointUserInfo, IEndpointDomainModel
    {

        public SAPUserinfo()
        {
            this.SAPGroupInfos = new List<SAPGroupInfo>();
            this.SAPProfileInfos = new List<SAPProfileInfo>();
            this.SAPParameterInfo = new List<SAPParameterInfo>();
            this.SAPRoleInfo = new List<SAPRoleInfo>();
            Roles = new List<string>();
            Profiles = new List<string>();
            Positions = new List<string>();
            Groups = new List<string>();
        }


        // public Nullable<long> WogUserId { get; set; }
        public long EndPointSystemId { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Initials { get; set; }

        public string Department { get; set; }

        public string E_Mail { get; set; }

        public string Pers_No { get; set; }

        public string Tel1_Numbr { get; set; }

        public string Function0 { get; set; }

        public string Title { get; set; }

        public string City { get; set; }

        public string Namcountry { get; set; }

        public string Pboxcit_No { get; set; }

        public string Postl_Cod1 { get; set; }

        public string Street { get; set; }

        public string Start_Menu { get; set; }

        public string ActivityGroup { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public string UserLogonName { get; set; }

        public string AccntLockNoPwd { get; set; }

        public string AccntLocDurPwd { get; set; }

        public string LOCKEDBYINTRUDER { get; set; }

        public string EMPLOYEEID { get; set; }

        public string EMPLOYEETYPE { get; set; }

        public DateTime? PASSWORDEXPTIME { get; set; }

        public string LOGINDISABLED { get; set; }

        public bool IsActive { get; set; }

        public string DomainName { get; set; }

        public int TargetSysId { get; set; }

        public int PwdResetCnt { get; set; }

        public DateTime? IACMLastPasswordChange { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        public string Position { get; set; }

        public string CostCenter { get; set; }

        public DateTime? validFromDate { get; set; }

        public DateTime? validToDate { get; set; }

        public string SapUserType { get; set; }

        public bool IsUserLocked { get; set; }

        public bool NeedEPGroupAssignment { get; set; }

        public Dictionary<string, string> Parameters;

        public SAPUserType UserType;

        public string Password;

        public string OuputDevice;

        public string UserValidFrom;

        public string UserValidTo;

        public List<string> Roles;

        public List<string> Profiles;

        public List<string> Positions;

        public List<string> Groups;

        public DateTime StartDate;

        public DateTime EndDate;

        public EndPoint EndPointSystem { get; set; }

        public List<SAPGroupInfo> SAPGroupInfos { get; set; }

        public List<SAPProfileInfo> SAPProfileInfos { get; set; }

        public List<SAPParameterInfo> SAPParameterInfo { get; set; }

        public List<SAPRoleInfo> SAPRoleInfo { get; set; }

        public List<SAPPosition> SAPPositions { get; set; }

        public string AccountName
        {
            get
            {
                return this.UserLogonName;
            }
        }

        public string DisplayName
        {
            get
            {
                return String.Format("{0} {1}", this.Firstname, this.Lastname).Trim();
            }
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
                if (SAPRoleInfo != null)
                {
                    foreach (var item in SAPRoleInfo)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }

        /// <summary>
        /// Gets whether the user is a Dialogue user
        /// i.e. SAP UserType is "A"
        /// </summary>
        public bool IsDialogUser
        {
            get
            {
                return String.Compare(this.SapUserType, "A", true) == 0;
            }
        }
        public string SAPUserSNC { get; set; }
        public string UserSamAccountName { get; set; }
        public static List<SAPUserinfo> GetByEndPointSystemId(long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(transaction)
                       .Get(adsInfo => adsInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static SAPUserinfo GetUserAccountByEndPointSystemId(long id, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(transaction)
                       .Get(adsInfo => adsInfo.Id == id && adsInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static SAPUserinfo GetByUserLogonName(string userLogonName, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(transaction)
                       .Get(sap => sap.UserLogonName == userLogonName && sap.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static SAPUserinfo GetActiveByUserLogonName(string userLogonName, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(transaction)
                       .Get(sap => sap.UserLogonName.ToLower() == userLogonName.ToLower() && sap.EndPointSystemId == endPointSystemId && !sap.IsUserLocked).FirstOrDefault());
            }
        }

        public static List<SAPUserinfo> GetByUsersLogonNames(List<string> lstLogonNames, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(transaction)
                       .Get(sap => lstLogonNames.Any(s => s.Equals(sap.UserLogonName, StringComparison.OrdinalIgnoreCase)) && sap.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<SAPUserinfo> GetAllSAPUsers(long endPointSystemId, string userLogonName)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(transaction)
                       .Get(sap => sap.UserLogonName.Contains(userLogonName) && sap.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        //public static long AddSAPUserInfo(SAPUserinfo sapUsr)
        //{
        //    long userID = -1;
        //    if (sapUsr == null)
        //        return userID;

        //    using (var transaction = new DataAccessTransaction())
        //    {
        //        sapUsr.PASSWORDEXPTIME = sapUsr.PASSWORDEXPTIME == DateTime.MinValue ? null : sapUsr.PASSWORDEXPTIME;
        //        sapUsr.Save();
        //        userID = sapUsr.Id;
        //        transaction.Commit();
        //    }
        //    return userID;
        //}

        public static void UpdateSAPRoleInfoSAPUserinfoByEndPointSystemId(long endPointSystemId, long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPGroupInfoRepository(dataAccessTransaction).UpdateSAPRoleInfoSAPUserinfoByEndPointSystemId(endPointSystemId, id);
                dataAccessTransaction.Commit();
            }
        }

        //public static void UpdatePasswordresetcount(long endPointSystemId, string userLogonName)
        //{
        //    int recordsAffected = 0, _pwdResetCnt = 0;

        //    SAPUserinfo userInfo = null;
        //    userInfo = SAPUserinfo.GetByUserLogonName(userLogonName, endPointSystemId);
        //    if (userInfo != null)
        //    {
        //        _pwdResetCnt = userInfo.PwdResetCnt;
        //        if (_pwdResetCnt > 0)
        //            userInfo.PwdResetCnt = 1;
        //        else
        //            userInfo.PwdResetCnt++;
        //        userInfo.Save();
        //        recordsAffected = 1;
        //    }
        //}

        //public static bool AddSAPUserIntoGroups(SAPUserinfo sapUsr)
        //{
        //    bool isDataSaved = false;
        //    if (!(sapUsr != null && sapUsr.Groups != null))
        //        return isDataSaved;

        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        SAPUserinfo newSapUser = SAPUserinfo.GetById(sapUsr.Id);
        //        if (newSapUser != null)
        //        {
        //            foreach (string strGroup in sapUsr.Groups)
        //            {
        //                SAPGroupInfo groupInfo = SAPGroupInfo.GetByEndPointSystemId(sapUsr.EndPointSystemId, strGroup);

        //                if (groupInfo != null)
        //                {
        //                    SAPGroupInfo existingGroup = newSapUser.SAPGroupInfos.FirstOrDefault(group => group.GROUP_CODE == groupInfo.GROUP_CODE);
        //                    if (existingGroup == null)
        //                    {
        //                        newSapUser.SAPGroupInfos.Add(groupInfo);
        //                        newSapUser.Save();
        //                    }
        //                }
        //            }
        //        }
        //        SAPGroupInfo.UpdateSAPUserinfoSAPGroupInfoByEndPointSystemId(sapUsr.EndPointSystemId, sapUsr.Id);

        //        dataAccessTransaction.Commit();
        //    }
        //    return isDataSaved;
        //}

        public static SAPUserinfo GetSAPUserByUserId(long userId, string AccountUserName)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    var list = new RepositoryBase<EntityModel.SAPUserinfo>(dataAccessTransaction).Get(item => item.Id == userId, true);
                    if (!string.IsNullOrEmpty(AccountUserName))
                    {
                        return ORMap(list.FirstOrDefault(item => item.UserLogonName == AccountUserName));
                    }
                    return ORMap(list.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool AddEntitlement(long epUserId, EntitlementType _entitlementType, string entitlements)
        {
            try
            {
                SAPUserinfo currentSAPUser = SAPUserinfo.GetByIdIncludeSAPRoleInfo(epUserId);

                if (currentSAPUser == null)
                    return false;

                foreach (var item in entitlements.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var role = currentSAPUser.SAPRoleInfo.Find(g => g.ROLE_CODE.ToUpper() == item.ToUpper());

                        if (role == null)
                        {
                            var newRoleToAdd = DomainModel.SAPRoleInfo.GetByRoleCode(item, currentSAPUser.EndPointSystemId);
                            if (newRoleToAdd != null)
                                InsertSAPUserRoleMapping(currentSAPUser.EndPointSystemId, currentSAPUser.Id, newRoleToAdd.Id, currentSAPUser.UserValidFrom, currentSAPUser.UserValidTo);
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
        public static bool RemoveEntitlement(long epUserId, EntitlementType _entitlementType, string entitlements)
        {
            try
            {
                using (var transaction = new DataAccessTransaction())
                {
                    SAPUserinfo currentSAPUser = SAPUserinfo.GetByIdIncludeSAPRoleInfo(epUserId);

                    if (currentSAPUser == null)
                        return false;

                    foreach (var item in entitlements.Split(','))
                    {
                        var role = currentSAPUser.SAPRoleInfo.Find(g => g.ROLE_CODE.ToUpper() == item.ToUpper());

                        if (null != role)
                        {
                            var newRoleToRemove = DomainModel.SAPRoleInfo.GetByRoleCode(item, currentSAPUser.EndPointSystemId);
                            if (newRoleToRemove != null)
                                new SAPUserInfoRepository(transaction).DeleteSAPUserInfoSAPRoleInfo(currentSAPUser.Id, newRoleToRemove.Id);
                        }
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }

        public static bool RemoveAllEntitlements(long epUserId)
        {
            try
            {
                using (var transaction = new DataAccessTransaction())
                {
                    SAPUserinfo currentSAPUser = SAPUserinfo.GetByIdIncludeSAPRoleInfo(epUserId);

                    if (currentSAPUser == null)
                        return false;

                    if (currentSAPUser.SAPRoleInfo == null)
                        return true;

                    foreach (var item in currentSAPUser.SAPRoleInfo)
                    {
                        var newRoleToRemove = DomainModel.SAPRoleInfo.GetByRoleCode(item.ROLE_CODE, currentSAPUser.EndPointSystemId);
                        if (newRoleToRemove != null)
                            new SAPUserInfoRepository(transaction).DeleteSAPUserInfoSAPRoleInfo(currentSAPUser.Id, newRoleToRemove.Id);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }

        public static Dictionary<long, string> GetPositionTextByUser(long endpointUserId, long endpointSystemId, int applicationType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPUserPositionInfoRepository(dataAccessTransaction).GetPositionTextByUser(endpointUserId, endpointSystemId, applicationType);
            }
        }
        public static List<string> GetPositionTextByUser(string logonName, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPUserPositionInfoRepository(dataAccessTransaction).GetPositionsByUser(logonName, endpointSystemId);
            }
        }


        public static Dictionary<string, string> GetActivePrimarySAPUserByDIUserId(long dIUserId, ApplicationType applicationType, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPUserInfoRepository(dataAccessTransaction).GetActivePrimarySAPUserByDIUserId(dIUserId, applicationType, accountType);
            }
        }


        public static List<string> GetConflictRemarks(long dIUserId, long requestId, bool isBulkRequest)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPUserInfoRepository(dataAccessTransaction).GetConflictRemarks(dIUserId, requestId, isBulkRequest);
            }
        }

        public static List<string> GetSAPSODConflicts(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPUserInfoRepository(dataAccessTransaction).GetSAPSODConflicts(dIUserId, requestId, isBulkRequest, applicationType);
            }
        }
        public static List<PresentationConflictRemarks> GetSAPSODConflictsShowingrid(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            List<PresentationConflictRemarks> fResults = new List<PresentationConflictRemarks>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var results = new SAPUserInfoRepository(dataAccessTransaction).GetSAPSODConflictsShowingrid(dIUserId, requestId, isBulkRequest, applicationType);
                if(results != null && results.Count > 0)
                {
                    foreach (var item in results)
                    {
                        fResults.Add(new PresentationConflictRemarks() {
                            EndpointName = item.EndpointName,
                            ConflictedRole = item.ConflictedRole,
                            ConflictRemarks = item.ConflictRemarks,
                            RequestedRole = item.RequestedRole,
                            Source = item.Source
                        });
                    }
                }

            }
            return fResults;
        }

        public static bool AddSAPUserIntoRoles(SAPUserinfo sapUsr)
        {
            bool isDataSaved = false;
            if (!(sapUsr != null && sapUsr.Roles != null))
                return isDataSaved;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPUserinfo newSapUser = SAPUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id);
                if (newSapUser != null)
                {
                    foreach (string strRole in sapUsr.Roles)
                    {
                        newSapUser = SAPUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id);

                        SAPRoleInfo roleinfo = DomainModel.SAPRoleInfo.GetByRoleCode(strRole, sapUsr.EndPointSystemId);
                        SAPRoleInfo existingrole = null;

                        if (roleinfo != null && newSapUser.SAPRoleInfo != null)
                        {
                            existingrole = newSapUser.SAPRoleInfo.FirstOrDefault(role => role.ROLE_CODE == roleinfo.ROLE_CODE);
                        }

                        if (roleinfo != null && existingrole == null)
                        {
                            //DateTime fromDate = UtilityMethods.GetDateFromString(sapUsr.UserValidFrom.Replace(".", string.Empty));
                            //DateTime toDate = UtilityMethods.GetDateFromString(sapUsr.UserValidTo.Replace(".", string.Empty));
                            InsertSAPUserRoleMapping(newSapUser.EndPointSystemId, newSapUser.Id, roleinfo.Id, sapUsr.UserValidFrom, sapUsr.UserValidTo);
                        }
                    }
                }
                dataAccessTransaction.Commit();
            }
            isDataSaved = true;
            return isDataSaved;
        }
        public static void InsertSAPUserRoleMapping(long endPointSystemId, long userId, long roleId, string fromDate, string toDate)
        {
            try
            {
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    new SAPUserInfoRepository(dataAccessTransaction).InsertSAPUserRoleMapping(endPointSystemId, userId, roleId, fromDate, toDate);
                    dataAccessTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, "Error while InsertSAPUserRoleMapping", ex);
            }
        }

        public static List<SAPUserinfo> GetActiveAccounts(long usrId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPUserInfoRepository(transaction).GetActiveAccounts(usrId));
            }
        }

        public static List<SAPUserinfo> GetByDIUserID(long diUserId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPUserInfoRepository(transaction).GetByDIUserID(diUserId));
            }
        }

        public static List<SAPUserinfo> GetByDIUserID(long diUserId, AccountType acccType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPUserInfoRepository(transaction).GetByDIUserID(diUserId, acccType));
            }
        }
        public static SAPUserinfo GetByIdIncludeSAPRoleInfo(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction)
                      .Get(sapUser => sapUser.Id == id, sapUser => sapUser.SAPRoleInfo).FirstOrDefault());
            }
        }

        public static SAPUserinfo GetByIdIncludeSAPProfileInfo(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction)
                      .Get(sapUser => sapUser.Id == id, sapUser => sapUser.SAPProfileInfos).FirstOrDefault());
            }
        }

        public static SAPUserinfo GetByIdIncludeSAPGroupInfo(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction)
                      .Get(sapUser => sapUser.Id == id, sapUser => sapUser.SAPGroupInfo).FirstOrDefault());
            }
        }

        public static SAPUserinfo GetByDIUserIdIncludeSAPRoleInfo(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPUserInfoRepository(tranaction).GetByDIUserIdIncludeSAPRoleInfo(id));
            }
        }

        public static SAPUserinfo GetByIdWithNavigation(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction).
                    GetById(id, sapUser => sapUser.SAPRoleInfo, sapUser => sapUser.SAPProfileInfos,
                    sapUser => sapUser.SAPGroupInfo, sapUser => sapUser.EndPointSystem,
                    sapUser => sapUser.EndPointSystem.EndPointSystemType));
            }
        }

        //public bool DeleteByRoleInfo(SAPUserinfo sapUser)
        //{
        //    bool isDataSaved = false;

        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        SAPUserinfo currentSapUser = GetByIdIncludeSAPRoleInfo(sapUser.Id);
        //        foreach (string SapRole in sapUser.Roles)
        //        {
        //            SAPRoleInfo saproleinfo = DomainModel.SAPRoleInfo.GetByRoleCode(SapRole);
        //            if (saproleinfo != null)
        //            {
        //                currentSapUser.SAPRoleInfo.Remove(saproleinfo);
        //                currentSapUser.Save();
        //            }
        //        }
        //        dataAccessTransaction.Commit();
        //    }
        //    return isDataSaved;
        //}

        //public static void UpdateDiUserIdWithMappingInfo(int UserId, int DiUserId, string MapTo, int EndPointSystemId, EndpointType EndPointSystemType, string UserName, AccountType accountType)
        //{
        //    SAPUserinfo SAPUInfo = SAPUserinfo.GetByEndPointSystemId(UserId, EndPointSystemId);
        //    if (SAPUInfo != null)
        //    {
        //        SAPUInfo.DIUserId = DiUserId;
        //        SAPUInfo.LastModifiedDate = DateTime.Today;
        //        SAPUInfo.AccountType = accountType;
        //        SAPUInfo.IfUnmappedMapTo = MapTo;
        //        SAPUInfo.LastModifiedBy = UserName;
        //        SAPUInfo.Save();
        //    }
        //}

        public static List<SAPUserinfo> GetByDIUserIdIncludeEndPointSystem(long DIUserId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction)
                       .Get(sapUser => sapUser.Id == DIUserId, sapUser => sapUser.EndPointSystem, sapUser => sapUser.EndPointSystem.EndPointSystemType).ToList());
            }
        }

        public static List<SAPUserinfo> GetByDIUserId(long DIUserId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPUserInfoRepository(dataAccessTransaction).GetByDIUserId(DIUserId, endpointSystemId));
            }
        }

        //public static List<SAPUserinfo> GetByIsActive(int dIUserId, long endpointSystemId)
        //{
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(dataAccessTransaction)
        //               .Get(sapUser => sapUser.DIUserId == dIUserId && sapUser.AccountType == AccountType.User
        //                        && sapUser.EndPointSystemId == endpointSystemId && sapUser.IsActive && !sapUser.IsLocked).ToList());
        //    }
        //}

        //public static SAPUserinfo GetByAccountType(long dIUserId, long endpointSystemId)
        //{
        //    using (DataAccessTransaction tranaction = new DataAccessTransaction())
        //    {
        //        return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction)
        //              .Get(sapUser => sapUser.DIUserId == dIUserId && sapUser.EndPointSystemId == endpointSystemId
        //                    && sapUser.AccountType == AccountType.User && sapUser.IsLocked).FirstOrDefault());
        //    }
        //}

        public static SAPUserinfo GetByIdIncludeAllType(long Id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction)
                       .GetById(Id, sapUser => sapUser.SAPRoleInfo, sapUser => sapUser.SAPGroupInfo, sapUser => sapUser.SAPParameterInfo));
            }
        }

        public static List<SAPUserinfo> GetByEndpointSystemIdIncludeRoles(long endpointSystemId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction)
                       .Get(sapUser => sapUser.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static List<SAPUserinfo> GetDeletedByEndPointSystemId(long endPointSystemId, Process discoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<SAPUserinfo> listDeletedUsers = new List<SAPUserinfo>();
                if (discoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(dataAccessTransaction)
                       .Get(sapUser => sapUser.EndPointSystemId == endPointSystemId && ((sapUser.LastModifiedDate.HasValue == false && sapUser.CreatedDate < discoveryProcess.ProcessStartAt) ||
                       sapUser.LastModifiedDate < discoveryProcess.ProcessStartAt)).ToList());
                }
                return listDeletedUsers;
            }
        }

        public static DataSet GetByDIUserId(long id)
        {
            DataSet ds = null;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                SAPUserinfo sapUser = SAPUserinfo.GetById(id);
                if (sapUser != null)
                {
                    ds = new SAPUserInfoRepository(dataAccessTransaction).GetAccessByUserId(id);
                }
            }
            return ds;
        }

        public static DataSet GetObjectsByUserId(long id)
        {
            DataSet ds = null;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                SAPUserinfo sapUser = SAPUserinfo.GetById(id);

                if (sapUser != null)
                {
                    ds = new SAPUserInfoRepository(dataAccessTransaction).GetObjectsByUserId(id);
                }
            }
            return ds;
        }

        //public static bool UpdateSAPUserInfoFromProvisioning(SAPUserinfo sapUserInfo)
        //{
        //    bool isDataSaved = false;
        //    if (sapUserInfo == null)
        //        return isDataSaved;

        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        SAPUserinfo sapUser = SAPUserinfo.GetById(sapUserInfo.Id);
        //        sapUser.LastModifiedBy = string.IsNullOrEmpty(sapUserInfo.LastModifiedBy) ? "System" : sapUserInfo.LastModifiedBy;
        //        sapUser.Save();
        //        dataAccessTransaction.Commit();
        //    }

        //    isDataSaved = true;
        //    return isDataSaved;
        //}

        //public bool HasExistSecondayAccount(long diIUserId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return new RepositoryBase<EntityModel.SAPUserinfo>(dataAccessTransaction)
        //             .Get(item => item.DIUserId == diIUserId && item.AccountType == AccountType.User).Count() > 0 ? true : false;
        //    }
        //}

        //public static IList<SAPUserinfo> GetLoackedUserByEndPointSystemId(long diIUserId, long endPointSystemID, AccountType adhocAccountType, EndpointType endpointSystemType)
        //{
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        IList<SAPUserinfo> sapUserList = new List<SAPUserinfo>();
        //        if (diIUserId > 0)
        //        {
        //            sapUserList = ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(dataAccessTransaction)
        //               .Get(item =>
        //                        adhocAccountType == AccountType.User ? (item.EndPointSystemId == endPointSystemID && (item.AccountType == AccountType.User || item.AccountType == AccountType.Shared) && item.DIUserId == diIUserId) :
        //                        adhocAccountType == AccountType.Shared ? (item.AccountType == AccountType.Shared && item.EndPointSystemId == endPointSystemID) :
        //                        (item.EndPointSystemId == endPointSystemID && item.AccountType == AccountType.Service)).ToList());
        //        }
        //        return sapUserList;
        //    }
        //}

        //public bool GetLockedUserByDIUserId(long diIUserId, long endPointSystemID, AccountType adhocAccountType, EndpointType endpointSystemType)
        //{
        //    var result = false;
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {

        //        IList<SAPUserinfo> sapUserList = new List<SAPUserinfo>();
        //        sapUserList = GetLoackedUserByEndPointSystemId(diIUserId, endPointSystemID, adhocAccountType, endpointSystemType);

        //        if (sapUserList.Count > 0)
        //        {
        //            var userAccountIsAvaliable = sapUserList.Where(lq => lq.IsLocked == false).Select(lq => lq);
        //            result = userAccountIsAvaliable.Count() > 0 ? false : true;
        //        }
        //        else
        //        {
        //            result = false;
        //        }
        //    }
        //    return result;
        //}

        //public bool HasSecondayAccountExist(long diUserId, int endPointSystemID, EndpointType endpointSystemType)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return new SAPUserInfoRepository(dataAccessTransaction).HashUserAccountExist(diUserId, endPointSystemID, endpointSystemType);
        //    }
        //}

        public static SAPUserinfo GetInfoByName(string userLogOnName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(transaction).Get(adsInfo => adsInfo.UserLogonName == userLogOnName).FirstOrDefault());
            }
        }

        public static IList<SAPUserinfo> GetAllUsersInATCode(string TCode, long endpointId)
        {
            List<SAPUserinfo> usrList = new List<SAPUserinfo>();
            List<SapUserProfile> usrProfileList = new List<SapUserProfile>();
            List<string> AuthorizationCodes = SAPAuthorizationToObject.GetByFromValue(TCode, endpointId);
            if (AuthorizationCodes != null)
            {
                foreach (string AuthCode in AuthorizationCodes)
                {
                    IList<SAPAuthorizationProfile> AuthProfileLst = SAPAuthorizationProfile.GetByAuthcode(AuthCode);

                    foreach (SAPAuthorizationProfile AuthProfile in AuthProfileLst)
                    {
                        IList<SapUserProfile> SapUsrProfile = SapUserProfile.GetByProfileCode(AuthProfile.ProfileCode);
                        usrProfileList = usrProfileList.Union(SapUsrProfile).ToList();
                    }
                }
            }

            foreach (SapUserProfile usrinfo in usrProfileList)
            {
                SAPUserinfo user = SAPUserinfo.GetInfoByName(usrinfo.UserName);
                if (user != null)
                {
                    usrList.Add(user);
                }
            }

            return usrList;
        }

        public static IList<SAPUserinfo> GetAllUsersInObject(string TCode, string ObjectCode, string FieldCode, string Value)
        {
            SAPAuthorizationToObject authobject = SAPAuthorizationToObject.GetByObjectCode(ObjectCode, FieldCode, Value);

            SAPAuthorizationProfile AuthCompositeProfile = SAPAuthorizationProfile.GetByAuthcode(authobject.AuthorizationCode).FirstOrDefault();

            SAPCompositeProfileInfo compositeprofile = SAPCompositeProfileInfo.GetByProfileCode(AuthCompositeProfile.ProfileCode);

            SAPProfileInfo profile = SAPProfileInfo.GetByProfileCode(compositeprofile.PROFILE_CODE);

            return profile.SAPUserinfos;
        }


        //public static dynamic GetAllSAPUser(long endPtId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return new SAPUserInfoRepository(dataAccessTransaction).GetAllSAPUser(endPtId);
        //    }
        //}

        //public static bool IsSecondaryAccount(long endpointUserID)
        //{
        //    using (var transaction = new DataAccessTransaction())
        //    {
        //        return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(transaction).Get(usr => usr.DIUserId == endpointUserID && usr.AccountType == AccountType.User).ToList()).Count() > 0 ? true : false;
        //    }
        //}


        public static List<SAPUserinfo> GetByEndPointSystemIdIsLockedIncludingEndpointSystem(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPUserInfoRepository(transaction).GetByEndPointSystemIdIsLockedIncludingEndpointSystem(endpointSystemId));
            }
        }

        public static List<long> GetByUserLogonNames(string userLogonName, long endpointsystemid)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPUserinfo>(transaction).Get(ui => ui.UserLogonName == userLogonName && ui.EndPointSystemId == endpointsystemid).Select(ui => ui.Id).ToList();
            }
        }

        public static bool DoesExists(string userName, long endpointsystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPUserinfo>(dataAccessTransaction).Any(item => item.UserLogonName.Equals(userName, StringComparison.CurrentCultureIgnoreCase) && item.EndPointSystemId == endpointsystemId);
            }
        }

        public static IList<SAPUserinfo> GetAllSAPUserInfo(long EndpointSystemId, string FilterText)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var SapUsers = SAPUserinfo.GetByEndPointSystemIdIsLockedIncludingEndpointSystem(EndpointSystemId);

                if (FilterText == "*")
                {
                    return SapUsers.ToList();
                }
                return SapUsers.Where(sapUser => sapUser.UserLogonName.Contains(FilterText)).ToList();
            }
        }

        public static List<SAPUserinfo> GetSAPUserByEndpointSystemID(long endpointsystemId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction)
                       .Get(sapUser => sapUser.EndPointSystemId == endpointsystemId).ToList());
            }
        }

        public static List<SAPUserinfo> GetByEndpointIdAndUsersWithNoRoles(long endpointsystemId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction)
                       .Get(sapUser => sapUser.EndPointSystemId == endpointsystemId
                            && sapUser.SAPRoleInfo.Count == 0
                            , item => item.EndPointSystem, item => item.EndPointSystem.EndPointSystemType).ToList());
            }
        }

        public static List<SAPUserinfo> GetByEndpointIdAndDormantDays(long endpointsystemId, int dormantDays)
        {
            DateTime dormantDate = DateTime.Today.Subtract(new TimeSpan(dormantDays, 0, 0, 0));

            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(tranaction)
                       .Get(sapUser => sapUser.EndPointSystemId == endpointsystemId
                                && (sapUser.LastLoginTime == null
                                    || (sapUser.LastLoginTime.HasValue == true
                                        && DateTime.Compare(sapUser.LastLoginTime.Value, dormantDate) < 0
                                        )
                                    )
                                , sapUser => sapUser.EndPointSystem, sapUser => sapUser.EndPointSystem.EndPointSystemType
                            ).ToList());
            }
        }

        public static List<SAPUserinfo> GetByEndpointIdIncludeAccountExpiryDays(long endPointSysemId, int accountExpiryDays)
        {
            DateTime accountExpiryDate = DateTime.Today.Add(new TimeSpan(accountExpiryDays, 0, 0, 0));

            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(transaction)
                    .Get(user => user.EndPointSystemId == endPointSysemId
                    && user.IsUserLocked == false
                    && user.validToDate.HasValue == true
                    && DateTime.Compare(user.validToDate.Value, DateTime.Today) > 0
                    && DateTime.Compare(user.validToDate.Value, accountExpiryDate) < 0
                    , user => user.EndPointSystem, user => user.EndPointSystem.EndPointSystemType).ToList());

            }

        }

        public static List<SAPUserinfo> GetByEndpointIdIncludeIsUserLocked(long endPointSysemId)
        {

            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserinfo>(transaction)
                    .Get(user => user.EndPointSystemId == endPointSysemId
                    && user.IsUserLocked == true
                    && user.validToDate.HasValue == true
                    && DateTime.Compare(user.validToDate.Value, DateTime.Today) > 0
                    , user => user.EndPointSystem, user => user.EndPointSystem.EndPointSystemType).ToList());

            }

        }

        public static void SaveUsers(IList<SAPUserinfo> list, long EndpointSystemId, string currentUser)
        {
            IList<SAPUserinfo> listExistingUsers = new List<SAPUserinfo>();
            Dictionary<string, SAPUserinfo> dictExistingUsers = new Dictionary<string, DomainModel.SAPUserinfo>();
            SAPUserinfo sapexistingUser = null;

            listExistingUsers = SAPUserinfo.GetSAPUserByEndpointSystemID(EndpointSystemId);

            foreach (var usr in listExistingUsers)
            {
                if (!dictExistingUsers.ContainsKey(usr.UserLogonName.ToLower()))
                {
                    dictExistingUsers.Add(usr.UserLogonName.ToLower(), usr);
                }
            }
            listExistingUsers.Clear();
            listExistingUsers = null;

            var endpoint = EndPoint.GetById(EndpointSystemId);
            int cnt = 0;
            foreach (SAPUserinfo user in list)
            {
                cnt++;
                try
                {
                    if (!dictExistingUsers.ContainsKey(user.UserLogonName.ToLower()))
                    {
                        user.Save(currentUser);
                    }
                    else
                    {
                        sapexistingUser = dictExistingUsers[user.UserLogonName.ToLower()];
                        AccountReconcilation.SaveAttributeException(sapexistingUser, user, endpoint.EndpointSystemTypeId, null, user.AccountName, currentUser);
                        user.Id = sapexistingUser.Id;
                        user.Save(currentUser);
                    }
                    //Fixed Duplicate record creation
                    AccountReconcilation acctRecon = AccountReconcilation.GetByAccountName(user.AccountName, user.EndPointSystemId);
                    
                    if (acctRecon == null)
                    {
                        acctRecon = new AccountReconcilation();
                        acctRecon.ExceptionType = ExceptionTypes.Added;
                        acctRecon.AccountType = AccountType.None;
                        acctRecon.EndpointSystemId = user.EndPointSystemId;
                    }


                    //acctRecon.CanSelfService = user.IsDialogUser; // if sap user type is "A"
                    // Set last Logon date only if Dialog User
                    if (user.IsDialogUser)
                    {
                        acctRecon.LastLogonDate = user.LastLoginTime;
                    }
                    else
                    {
                        acctRecon.LastLogonDate = null;
                    }

                    acctRecon.EndpointUserId = user.Id;
                    acctRecon.FullName = user.DisplayName;
                    acctRecon.AccountName = user.UserLogonName;
                    acctRecon.LastModifiedDate = DateTime.Now;
                    acctRecon.ValidFromDate = user.validFromDate;
                    acctRecon.ValidToDate = user.validToDate;
                    if (acctRecon.IsDeleted && !user.IsUserLocked)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {user?.AccountName}(DisplayName{user?.DisplayName}) has been added without any request made in the system");
                        // if deleted account and again added at endpoint
                        acctRecon.ExceptionType = ExceptionTypes.Added;
                        acctRecon.IsDeleted = false;
                        acctRecon.IsLocked = false;
                        acctRecon.LastAttestedOn = null;
                        acctRecon.DeletedDate = null;
                        acctRecon.LastAttestedOn = null;
                    }
                    else if (!acctRecon.IsDeleted && user.IsUserLocked)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {user?.AccountName}(DisplayName{user?.DisplayName}) has been removed without any request made in the system");
                        acctRecon.IsDeleted = true;
                        acctRecon.IsLocked = true;
                        acctRecon.ExceptionType = ExceptionTypes.Removed;
                        acctRecon.DeletedDate = DateTime.Now;
                    }
                    acctRecon.Save(currentUser);
                }
                catch (Exception ex)
                {
                    DI.Chakra.Logger.DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Error while saving SAP user {user.UserLogonName}: InnerException: {ex.GetCompleteException()}", ex);
                }
            }

            DI.Chakra.Logger.DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Total saved SAP users {cnt}");
        }


        public static void DeleteSAPUserGroupMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPUserInfoRepository(dataAccessTransaction).DeleteSAPUserGroupMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPUserProfileMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPUserInfoRepository(dataAccessTransaction).DeleteSAPUserProfileMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPUserRoleMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPUserInfoRepository(dataAccessTransaction).DeleteSAPUserRoleMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPUserGroupMap(long endpointSystemId, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPUserInfoRepository(dataAccessTransaction).DeleteSAPUserGroupMap(endpointSystemId, userId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPUserProfileMap(long endpointSystemId, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPUserInfoRepository(dataAccessTransaction).DeleteSAPUserProfileMap(endpointSystemId, userId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPUserRoleMap(long endpointSystemId, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPUserInfoRepository(dataAccessTransaction).DeleteSAPUserRoleMap(endpointSystemId, userId);
                dataAccessTransaction.Commit();
            }
        }

        #region EF Conjunction Table Classes
        public class SAPUserGroup : IAggregateRoot
        {
            public long SAPGroupInfo_Id { get; set; }
            public long SAPUserinfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPUserProfile : IAggregateRoot
        {
            public long SAPUserinfo_Id { get; set; }
            public long SAPProfileInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPUserParameter : IAggregateRoot
        {
            public int SAPUserInfo_Id { get; set; }
            public long SAPParameterInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPCompositeChildRoleMap : IAggregateRoot
        {
            public long SAPSingleRoleInfo_Id { get; set; }
            public long SAPCompositeRoleInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPUserRole : IAggregateRoot
        {
            public long SAPRoleInfo_Id { get; set; }
            public long SAPUserinfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
            public bool? IsDirectlyAttached { get; set; }
            public string Role_FromDate { get; set; }
            public string Role_ToDate { get; set; }
        }
        public class SapSingleRoleTcodes : IAggregateRoot
        {
            public long SAPTCodeInfo_Id { get; set; }
            public long SAPSingleRoleInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        #endregion

        public static void AddSingleRoleTCodes(DataTable dtAlltcode, long endpointSystemId, IList<string> deltaRoles, bool isFirstTimeDiscovery)
        {
            IList<SapSingleRoleTcodes> listSingleRoleTcodes = new List<SapSingleRoleTcodes>();
            try
            {
                SAPTCodeInfo objSAPTCodeInfo = null;
                SAPSingleRoleInfo objSingleRoleInfo = null;
                SapSingleRoleTcodes objSapSingleRoleTcodeMap = null;
                List<SAPTCodeInfo> lstTCodes = SAPTCodeInfo.GetByEndpointId(endpointSystemId);
                List<SAPSingleRoleInfo> lstSapRoles = SAPSingleRoleInfo.GetByEndPointSystemId(endpointSystemId).ToList();
                for (int RowNo = 0; RowNo < dtAlltcode.Rows.Count; RowNo++)
                {
                    string TCode = dtAlltcode.Rows[RowNo]["LOW"].ToString().Trim();
                    string RoleName = dtAlltcode.Rows[RowNo]["AGR_Name"].ToString().Trim();

                    bool DiscoverRoleTCode = true;
                    if (!isFirstTimeDiscovery)
                    {
                        DiscoverRoleTCode = deltaRoles.FirstOrDefault(deltarole => deltarole.Trim() == RoleName.Trim()) != null;
                    }
                    if (DiscoverRoleTCode)
                    {
                        objSAPTCodeInfo = new SAPTCodeInfo();
                        objSAPTCodeInfo = lstTCodes.FirstOrDefault(tcode => tcode.TCODE_Code == TCode && tcode.EndPointSystemId == endpointSystemId);

                        objSingleRoleInfo = new SAPSingleRoleInfo();
                        objSingleRoleInfo = lstSapRoles.FirstOrDefault(role => role.SingleRole_Code == RoleName && role.EndPointSystemId == endpointSystemId);


                        if (objSingleRoleInfo != null && objSAPTCodeInfo != null)
                        {
                            SapSingleRoleTcodes objSingleRoleTCode = listSingleRoleTcodes.FirstOrDefault
                                            (
                                                roletcode => roletcode.SAPSingleRoleInfo_Id == objSingleRoleInfo.Id
                                                && roletcode.SAPTCodeInfo_Id == objSAPTCodeInfo.Id
                                                && roletcode.EndpointSystemId == endpointSystemId
                                            );
                            if (objSingleRoleTCode == null)
                            {
                                objSapSingleRoleTcodeMap = new SapSingleRoleTcodes();
                                objSapSingleRoleTcodeMap.SAPTCodeInfo_Id = objSAPTCodeInfo.Id;
                                objSapSingleRoleTcodeMap.SAPSingleRoleInfo_Id = objSingleRoleInfo.Id;
                                objSapSingleRoleTcodeMap.EndpointSystemId = endpointSystemId;
                                objSapSingleRoleTcodeMap.Version = Convert.ToInt16(VersionNo.Available);
                                listSingleRoleTcodes.Add(objSapSingleRoleTcodeMap);
                            }
                        }
                    }
                }
                SaveSAPSingleRoleTcodeMap(listSingleRoleTcodes, endpointSystemId, isFirstTimeDiscovery);

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void SaveSAPSingleRoleTcodeMap(IList<SapSingleRoleTcodes> listSAPSingleRoleTcodes, long endpointSystemId, bool clearAllFirst)
        {
            if (clearAllFirst)
            {
                SAPUserinfo.DeleteSAPTCodeSingleRoleMap(endpointSystemId);
            }
            else
            {
                var UniqueSingleRoles =
                                from roletcode in listSAPSingleRoleTcodes
                                join roleinfo in SAPSingleRoleInfo.GetAll()
                                on roletcode.SAPSingleRoleInfo_Id equals roleinfo.Id
                                select roleinfo;
                UniqueSingleRoles = UniqueSingleRoles.Distinct().ToList();
                UniqueSingleRoles.ToList().ForEach(l => SAPUserinfo.DeleteSAPTCodeSingleRoleMap(endpointSystemId, l.Id));
            }

            SQLBulkUploader<SapSingleRoleTcodes> sqlSAPSingleRoleTcodeMap = null;
            try
            {
                string tableName = "SAPTCodeInfoSAPSingleRoleInfo";
                sqlSAPSingleRoleTcodeMap = new SQLBulkUploader<SapSingleRoleTcodes>(tableName);
                sqlSAPSingleRoleTcodeMap.Flush(listSAPSingleRoleTcodes);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlSAPSingleRoleTcodeMap.Close();
            }
        }

        private static void DeleteSAPTCodeSingleRoleMap(long endpointSystemId, long singleRoleId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPUserInfoRepository(dataAccessTransaction).DeleteSAPTCodeSingleRoleMap(endpointSystemId, singleRoleId);
                dataAccessTransaction.Commit();
            }
        }

        private static void DeleteSAPTCodeSingleRoleMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPUserInfoRepository(dataAccessTransaction).DeleteSAPTCodeSingleRoleMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void SaveSAPUserGroupMap(IList<SAPUserGroup> listUserGroup, long endpointSystemId)
        {
            SQLBulkUploader<SAPUserGroup> sqlSAPUserGroupMap = null;
            try
            {
                string tableName = "SAPUserinfoSAPGroupInfo";

                sqlSAPUserGroupMap = new SQLBulkUploader<SAPUserGroup>(tableName);

                sqlSAPUserGroupMap.Flush(listUserGroup);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlSAPUserGroupMap.Close();
            }
        }

        public static void SaveSAPUserProfileMap(IList<SAPUserProfile> listUserProfile, long endpointSystemId)
        {
            SQLBulkUploader<SAPUserProfile> sqlSAPUserProfileMap = null;
            try
            {
                string tableName = "SAPProfileInfoSAPUserinfo";

                sqlSAPUserProfileMap = new SQLBulkUploader<SAPUserProfile>(tableName);

                sqlSAPUserProfileMap.Flush(listUserProfile);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlSAPUserProfileMap.Close();
            }
        }

        public static void SaveSAPUserRoleMap(IList<SAPUserRole> listUserRole, long endpointSystemId)
        {
            SQLBulkUploader<SAPUserRole> sqlSAPUserRoleMap = null;
            try
            {
                string tableName = "SAPRoleInfoSAPUserinfo";

                sqlSAPUserRoleMap = new SQLBulkUploader<SAPUserRole>(tableName);

                sqlSAPUserRoleMap.Flush(listUserRole);

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlSAPUserRoleMap.Close();
            }
        }

        public static void SaveCompositeChildRoleMap(IList<SAPCompositeChildRoleMap> listCompositeChildRoleMap, long endpointSystemId)
        {

            SAPUserinfo.DeleteSAPCompositeRoleSingleRoleMap(endpointSystemId);
            SQLBulkUploader<SAPCompositeChildRoleMap> sqlSAPCompositeChildRoleMap = null;
            try
            {
                string tableName = "SAPSingleRoleInfoSAPCompositeRoleInfo";
                sqlSAPCompositeChildRoleMap = new SQLBulkUploader<SAPCompositeChildRoleMap>(tableName);
                sqlSAPCompositeChildRoleMap.Flush(listCompositeChildRoleMap);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlSAPCompositeChildRoleMap.Close();
            }
        }

        private static void DeleteSAPCompositeRoleSingleRoleMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPUserInfoRepository(dataAccessTransaction).DeleteSAPCompositeRoleSingleRoleMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        //public static void MapWithMasterUser(EndPoint endPoint, bool mapall)
        //{
        //    StringBuilder bldR = new StringBuilder();
        //    string strJoin = GetJoinQry(endPoint.Id, mapall);
        //    if (strJoin == string.Empty)
        //        return;
        //    bldR.Append("Update DI_SAP_UserInfo set DI_SAP_UserInfo.DIUserId = DI_M_User.Id,MappingType='System',DI_SAP_UserInfo.AccountType=1  FROM DI_SAP_UserInfo JOIN DI_M_User on");
        //    bldR.Append(" ");
        //    bldR.Append(strJoin);
        //    bldR.Append(" ");
        //    bldR.AppendFormat("WHERE DI_SAP_UserInfo.EndpointSystemId = {0}  AND DI_M_User.Id <> 1 AND ISNULL(MappingType,'') <> 'Manual'", endPoint.Id.ToString());
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        new SAPUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
        //        dataAccessTransaction.Commit();
        //    }
        //}

        private static string GetJoinQry(long systemId, bool mapall)
        {
            string clauseType = "AND";

            if (!mapall)
            {
                clauseType = "OR";
            }

            StringBuilder bldr = new StringBuilder();
            IList<AttributeMappingColumns> listAttributes = AttributeMappingColumns.GetByEndpointSystemId(systemId);
            foreach (AttributeMappingColumns attDto in listAttributes)
            {
                bldr.AppendFormat(string.Format("(DI_SAP_UserInfo.{0}=DI_M_User.{1} and DI_SAP_UserInfo.{2} != '' and DI_SAP_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null)  {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
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
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_SAP_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_SAP_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_SAP_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2 AND DI_M_AccountReconcilation.EndpointSystemId = {0}", endPoint.Id.ToString(), considerInActiveAccountForRecon);
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new SAPUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
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
        IEndpointUserInfo IEndpointDomainModel.GetEndpointUserInfo(long EndpointUserId)
        {
            return GetById(EndpointUserId);
        }

        public static void RemoveSAPUser(List<SAPUserinfo> listUsers)
        {
            foreach (SAPUserinfo user in listUsers)
            {
                SAPUserinfo.DeleteSAPUserGroupMap(user.EndPointSystemId, user.Id);
                SAPUserinfo.DeleteSAPUserRoleMap(user.EndPointSystemId, user.Id);
                SAPUserinfo.DeleteSAPUserProfileMap(user.EndPointSystemId, user.Id);
                user.Delete();
            }
        }

        public static void DeleteSAPUserRoleMap(SAPUserinfo sapUser)
        {
            try
            {
                foreach (string role in sapUser.Roles)
                {
                    using (var transaction = new DataAccessTransaction())
                    {
                        long roleid = DomainModel.SAPRoleInfo.GetByRoleCode(role).Id;
                        new SAPUserInfoRepository(transaction).DeleteSAPUserInfoSAPRoleInfo(sapUser.Id, roleid);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, "Error occur while deleting SAP User Role Map.", ex);
                throw ex;
            }
        }

        public static List<SAPUserinfo> GetByRoleCode(string roleCode, long endPointSystemId)
        {
            var roleInfo = DomainModel.SAPRoleInfo.GetSAPUserInfoByRoleCodeIncludingSAPUserInfo(roleCode, endPointSystemId);
            return (roleInfo == null || roleInfo.SAPUserinfo == null) ? new List<SAPUserinfo>() : roleInfo.SAPUserinfo;
        }

        public static IList<SAPUserinfo> GetByProfileCodeWithNavigation(string profileCode, long endPointSystemId)
        {
            var profileInfo = SAPProfileInfo.GetByProfileCodeWithNavigation(profileCode, endPointSystemId);
            return (profileInfo == null || profileInfo.SAPUserinfos == null) ? new List<SAPUserinfo>() : profileInfo.SAPUserinfos;
        }

        public static IList<SAPUserinfo> GetSAPUserInfoByDIUserId(long DIUserId, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPUserInfoRepository(dataAccessTransaction).GetSAPUserInfoByDIUserId(DIUserId, accountType));
            }
        }

        public static SAPUserinfo GetByAccountType(long diUserId, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPUserInfoRepository(dataAccessTransaction).GetByAccountType(diUserId, accountType));
            }
        }
        public static void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            SAPUserinfo sapUserInfo = SAPUserinfo.GetByUserLogonName(userName, endpointId);
            if (sapUserInfo != null)
            {
                if (sapUserInfo.PwdResetCnt > 0)
                    sapUserInfo.PwdResetCnt++;
                else
                    sapUserInfo.PwdResetCnt = 1;
                sapUserInfo.Save(currentUser);
            }
        }

        public static SAPUserinfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPUserInfoRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }

        public static SAPUserinfo GetUserAccountByUser(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPUserInfoRepository(transaction).GetUserAccountByUser(dIUserId, endPointSystemId));
            }
        }
        public static SAPUserinfo GetPositionPrefixMatchedUser(long dIUserId, long endPointSystemId, string positionPrefix)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPUserInfoRepository(transaction).GetPositionPrefixMatchedUser(dIUserId, endPointSystemId, positionPrefix));
            }
        }
        public static List<SAPUserinfo> GetActiveAccounts(long dIUserId, long endPointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPUserInfoRepository(transaction).GetActiveAccounts(dIUserId, endPointSystemId, accountType));
            }
        }

        public static SAPUserinfo GetLockedAccount(long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPUserInfoRepository(transaction).GetLockedAccount(diuserId, endpointSystemId, accountType));
            }
        }

        public static SAPUserinfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPUserInfoRepository(transaction).GetLockedAccountByName(accountName, diuserId, endpointSystemId, accountType));
            }
        }

        
        /// <summary>
        /// Only for SRM
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="endpointSystemId"></param>
        /// <returns></returns>
        public static SAPUserinfo GetByIsActive(string accountName, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPUserinfo>(dataAccessTransaction)
                       .Get(sapUser => sapUser.UserLogonName == accountName && sapUser.EndPointSystemId == endpointSystemId && sapUser.IsActive).FirstOrDefault());
            }
        }

        
    }
}
