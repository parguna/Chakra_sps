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
    public class SAPHANAUserinfo : DomainModelBase<SAPHANAUserinfo, EntityModel.SAPHANAUserinfo>, IEndpointUserInfo, IEndpointDomainModel
    {

        public SAPHANAUserinfo()
        {
            this.SAPGroupInfos = new List<SAPHANAGroupInfo>();
            this.SAPProfileInfos = new List<SAPHANAProfileInfo>();
            this.SAPParameterInfo = new List<SAPHANAParameterInfo>();
            this.SAPRoleInfo = new List<SAPHANARoleInfo>();
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

        public List<SAPHANAGroupInfo> SAPGroupInfos { get; set; }

        public List<SAPHANAProfileInfo> SAPProfileInfos { get; set; }

        public List<SAPHANAParameterInfo> SAPParameterInfo { get; set; }

        public List<SAPHANARoleInfo> SAPRoleInfo { get; set; }

        public List<SAPHANAPosition> SAPPositions { get; set; }

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
        public static List<SAPHANAUserinfo> GetByEndPointSystemId(long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(transaction)
                       .Get(adsInfo => adsInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static SAPHANAUserinfo GetUserAccountByEndPointSystemId(long id, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAUserinfo>(transaction)
                       .Get(adsInfo => adsInfo.Id == id && adsInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static SAPHANAUserinfo GetByUserLogonName(string userLogonName, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAUserinfo>(transaction)
                       .Get(sap => sap.UserLogonName == userLogonName && sap.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static SAPHANAUserinfo GetActiveByUserLogonName(string userLogonName, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAUserinfo>(transaction)
                       .Get(sap => sap.UserLogonName.ToLower() == userLogonName.ToLower() && sap.EndPointSystemId == endPointSystemId && !sap.IsUserLocked).FirstOrDefault());
            }
        }

        public static List<SAPHANAUserinfo> GetByUsersLogonNames(List<string> lstLogonNames, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(transaction)
                       .Get(sap => lstLogonNames.Any(s => s.Equals(sap.UserLogonName, StringComparison.OrdinalIgnoreCase)) && sap.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<SAPHANAUserinfo> GetAllSAPUsers(long endPointSystemId, string userLogonName)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(transaction)
                       .Get(sap => sap.UserLogonName.Contains(userLogonName) && sap.EndPointSystemId == endPointSystemId).ToList());
            }
        }


        public static void UpdateSAPRoleInfoSAPUserinfoByEndPointSystemId(long endPointSystemId, long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPHANAGroupInfoRepository(dataAccessTransaction).UpdateSAPRoleInfoSAPUserinfoByEndPointSystemId(endPointSystemId, id);
                dataAccessTransaction.Commit();
            }
        }

        

        public static SAPHANAUserinfo GetSAPUserByUserId(long userId, string AccountUserName)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    var list = new RepositoryBase<EntityModel.SAPHANAUserinfo>(dataAccessTransaction).Get(item => item.Id == userId, true);
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
                SAPHANAUserinfo currentSAPUser = SAPHANAUserinfo.GetByIdIncludeSAPRoleInfo(epUserId);

                if (currentSAPUser == null)
                    return false;

                foreach (var item in entitlements.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var role = currentSAPUser.SAPRoleInfo.Find(g => g.ROLE_CODE.ToUpper() == item.ToUpper());

                        if (role == null)
                        {
                            var newRoleToAdd = DomainModel.SAPHANARoleInfo.GetByRoleCode(item, currentSAPUser.EndPointSystemId);
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
                    SAPHANAUserinfo currentSAPUser = SAPHANAUserinfo.GetByIdIncludeSAPRoleInfo(epUserId);

                    if (currentSAPUser == null)
                        return false;

                    foreach (var item in entitlements.Split(','))
                    {
                        var role = currentSAPUser.SAPRoleInfo.Find(g => g.ROLE_CODE.ToUpper() == item.ToUpper());

                        if (null != role)
                        {
                            var newRoleToRemove = DomainModel.SAPHANARoleInfo.GetByRoleCode(item, currentSAPUser.EndPointSystemId);
                            if (newRoleToRemove != null)
                                new SAPHANAUserInfoRepository(transaction).DeleteSAPUserInfoSAPRoleInfo(currentSAPUser.Id, newRoleToRemove.Id);
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
                    SAPHANAUserinfo currentSAPUser = SAPHANAUserinfo.GetByIdIncludeSAPRoleInfo(epUserId);

                    if (currentSAPUser == null)
                        return false;

                    if (currentSAPUser.SAPRoleInfo == null)
                        return true;

                    foreach (var item in currentSAPUser.SAPRoleInfo)
                    {
                        var newRoleToRemove = DomainModel.SAPHANARoleInfo.GetByRoleCode(item.ROLE_CODE, currentSAPUser.EndPointSystemId);
                        if (newRoleToRemove != null)
                            new SAPHANAUserInfoRepository(transaction).DeleteSAPUserInfoSAPRoleInfo(currentSAPUser.Id, newRoleToRemove.Id);
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

        public static Dictionary<long, string> GetPositionTextByUser(long endpointUserId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPHANAUserPositionInfoRepository(dataAccessTransaction).GetPositionTextByUser(endpointUserId, endpointSystemId);
            }
        }
        public static List<string> GetPositionTextByUser(string logonName, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPHANAUserPositionInfoRepository(dataAccessTransaction).GetPositionsByUser(logonName, endpointSystemId);
            }
        }


        public static Dictionary<string, string> GetActivePrimarySAPUserByDIUserId(long dIUserId, ApplicationType applicationType, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPHANAUserInfoRepository(dataAccessTransaction).GetActivePrimarySAPUserByDIUserId(dIUserId, applicationType, accountType);
            }
        }
        
        public static List<string> GetSAPSODConflicts(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new SAPHANAUserInfoRepository(dataAccessTransaction).GetSAPSODConflicts(dIUserId, requestId, isBulkRequest, applicationType);
            }
        }
        public static List<PresentationConflictRemarks> GetSAPSODConflictsShowingrid(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            List<PresentationConflictRemarks> fResults = new List<PresentationConflictRemarks>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var results = new SAPHANAUserInfoRepository(dataAccessTransaction).GetSAPSODConflictsShowingrid(dIUserId, requestId, isBulkRequest, applicationType);
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

        public static bool AddSAPUserIntoRoles(SAPHANAUserinfo sapUsr)
        {
            bool isDataSaved = false;
            if (!(sapUsr != null && sapUsr.Roles != null))
                return isDataSaved;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPHANAUserinfo newSapUser = SAPHANAUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id);
                if (newSapUser != null)
                {
                    foreach (string strRole in sapUsr.Roles)
                    {
                        newSapUser = SAPHANAUserinfo.GetByIdIncludeSAPRoleInfo(sapUsr.Id);

                        SAPHANARoleInfo roleinfo = DomainModel.SAPHANARoleInfo.GetByRoleCode(strRole, sapUsr.EndPointSystemId);
                        SAPHANARoleInfo existingrole = null;

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
                    new SAPHANAUserInfoRepository(dataAccessTransaction).InsertSAPUserRoleMapping(endPointSystemId, userId, roleId, fromDate, toDate);
                    dataAccessTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, "Error while InsertSAPHANAUserRoleMapping", ex);
            }
        }

        public static List<SAPHANAUserinfo> GetActiveAccounts(long usrId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPHANAUserInfoRepository(transaction).GetActiveAccounts(usrId));
            }
        }

        public static List<SAPHANAUserinfo> GetByDIUserID(long diUserId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPHANAUserInfoRepository(transaction).GetByDIUserID(diUserId));
            }
        }

        public static List<SAPHANAUserinfo> GetByDIUserID(long diUserId, AccountType acccType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPHANAUserInfoRepository(transaction).GetByDIUserID(diUserId, acccType));
            }
        }
        public static SAPHANAUserinfo GetByIdIncludeSAPRoleInfo(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAUserinfo>(tranaction)
                      .Get(sapUser => sapUser.Id == id, sapUser => sapUser.SAPRoleInfo).FirstOrDefault());
            }
        }

        public static SAPHANAUserinfo GetByIdIncludeSAPProfileInfo(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAUserinfo>(tranaction)
                      .Get(sapUser => sapUser.Id == id, sapUser => sapUser.SAPProfileInfos).FirstOrDefault());
            }
        }

        public static SAPHANAUserinfo GetByIdIncludeSAPGroupInfo(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAUserinfo>(tranaction)
                      .Get(sapUser => sapUser.Id == id, sapUser => sapUser.SAPGroupInfo).FirstOrDefault());
            }
        }

        public static SAPHANAUserinfo GetByDIUserIdIncludeSAPRoleInfo(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPHANAUserInfoRepository(tranaction).GetByDIUserIdIncludeSAPRoleInfo(id));
            }
        }

        public static SAPHANAUserinfo GetByIdWithNavigation(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAUserinfo>(tranaction).
                    GetById(id, sapUser => sapUser.SAPRoleInfo, sapUser => sapUser.SAPProfileInfos,
                    sapUser => sapUser.SAPGroupInfo, sapUser => sapUser.EndPointSystem,
                    sapUser => sapUser.EndPointSystem.EndPointSystemType));
            }
        }

        

        public static List<SAPHANAUserinfo> GetByDIUserIdIncludeEndPointSystem(long DIUserId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(tranaction)
                       .Get(sapUser => sapUser.Id == DIUserId, sapUser => sapUser.EndPointSystem, sapUser => sapUser.EndPointSystem.EndPointSystemType).ToList());
            }
        }

        public static List<SAPHANAUserinfo> GetByDIUserId(long DIUserId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPHANAUserInfoRepository(dataAccessTransaction).GetByDIUserId(DIUserId, endpointSystemId));
            }
        }

        

        public static SAPHANAUserinfo GetByIdIncludeAllType(long Id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAUserinfo>(tranaction)
                       .GetById(Id, sapUser => sapUser.SAPRoleInfo, sapUser => sapUser.SAPGroupInfo, sapUser => sapUser.SAPParameterInfo));
            }
        }

        public static List<SAPHANAUserinfo> GetByEndpointSystemIdIncludeRoles(long endpointSystemId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(tranaction)
                       .Get(sapUser => sapUser.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static List<SAPHANAUserinfo> GetDeletedByEndPointSystemId(long endPointSystemId, Process discoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<SAPHANAUserinfo> listDeletedUsers = new List<SAPHANAUserinfo>();
                if (discoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(dataAccessTransaction)
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
                SAPHANAUserinfo sapUser = SAPHANAUserinfo.GetById(id);
                if (sapUser != null)
                {
                    ds = new SAPHANAUserInfoRepository(dataAccessTransaction).GetAccessByUserId(id);
                }
            }
            return ds;
        }

        public static DataSet GetObjectsByUserId(long id)
        {
            DataSet ds = null;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                SAPHANAUserinfo sapUser = SAPHANAUserinfo.GetById(id);

                if (sapUser != null)
                {
                    ds = new SAPHANAUserInfoRepository(dataAccessTransaction).GetObjectsByUserId(id);
                }
            }
            return ds;
        }

        

        public static SAPHANAUserinfo GetInfoByName(string userLogOnName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAUserinfo>(transaction).Get(adsInfo => adsInfo.UserLogonName == userLogOnName).FirstOrDefault());
            }
        }

        public static IList<SAPHANAUserinfo> GetAllUsersInATCode(string TCode, long endpointId)
        {
            List<SAPHANAUserinfo> usrList = new List<SAPHANAUserinfo>();
            List<SapHANAUserProfile> usrProfileList = new List<SapHANAUserProfile>();
            List<string> AuthorizationCodes = SAPAuthorizationToObject.GetByFromValue(TCode, endpointId);
            if (AuthorizationCodes != null)
            {
                foreach (string AuthCode in AuthorizationCodes)
                {
                    IList<SAPHANAAuthorizationProfile> AuthProfileLst = SAPHANAAuthorizationProfile.GetByAuthcode(AuthCode);

                    foreach (var AuthProfile in AuthProfileLst)
                    {
                        IList<SapHANAUserProfile> SapUsrProfile = SapHANAUserProfile.GetByProfileCode(AuthProfile.ProfileCode);
                        usrProfileList = usrProfileList.Union(SapUsrProfile).ToList();
                    }
                }
            }

            foreach (var usrinfo in usrProfileList)
            {
                SAPHANAUserinfo user = SAPHANAUserinfo.GetInfoByName(usrinfo.UserName);
                if (user != null)
                {
                    usrList.Add(user);
                }
            }

            return usrList;
        }

        public static IList<SAPHANAUserinfo> GetAllUsersInObject(string TCode, string ObjectCode, string FieldCode, string Value)
        {
            var authobject = SAPHANAAuthorizationToObject.GetByObjectCode(ObjectCode, FieldCode, Value);

            var AuthCompositeProfile = SAPHANAAuthorizationProfile.GetByAuthcode(authobject.AuthorizationCode).FirstOrDefault();

            var compositeprofile = SAPHANACompositeProfileInfo.GetByProfileCode(AuthCompositeProfile.ProfileCode);

            var profile = SAPHANAProfileInfo.GetByProfileCode(compositeprofile.PROFILE_CODE);

            return profile.SAPUserinfos;
        }

        public static List<SAPHANAUserinfo> GetByEndPointSystemIdIsLockedIncludingEndpointSystem(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPHANAUserInfoRepository(transaction).GetByEndPointSystemIdIsLockedIncludingEndpointSystem(endpointSystemId));
            }
        }

        public static List<long> GetByUserLogonNames(string userLogonName, long endpointsystemid)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPHANAUserinfo>(transaction).Get(ui => ui.UserLogonName == userLogonName && ui.EndPointSystemId == endpointsystemid).Select(ui => ui.Id).ToList();
            }
        }

        public static bool DoesExists(string userName, long endpointsystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPHANAUserinfo>(dataAccessTransaction).Any(item => item.UserLogonName.Equals(userName, StringComparison.CurrentCultureIgnoreCase) && item.EndPointSystemId == endpointsystemId);
            }
        }

        public static IList<SAPHANAUserinfo> GetAllSAPUserInfo(long EndpointSystemId, string FilterText)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var SapUsers = SAPHANAUserinfo.GetByEndPointSystemIdIsLockedIncludingEndpointSystem(EndpointSystemId);

                if (FilterText == "*")
                {
                    return SapUsers.ToList();
                }
                return SapUsers.Where(sapUser => sapUser.UserLogonName.Contains(FilterText)).ToList();
            }
        }

        public static List<SAPHANAUserinfo> GetSAPUserByEndpointSystemID(long endpointsystemId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(tranaction)
                       .Get(sapUser => sapUser.EndPointSystemId == endpointsystemId).ToList());
            }
        }

        public static List<SAPHANAUserinfo> GetByEndpointIdAndUsersWithNoRoles(long endpointsystemId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(tranaction)
                       .Get(sapUser => sapUser.EndPointSystemId == endpointsystemId
                            && sapUser.SAPRoleInfo.Count == 0
                            , item => item.EndPointSystem, item => item.EndPointSystem.EndPointSystemType).ToList());
            }
        }

        public static List<SAPHANAUserinfo> GetByEndpointIdAndDormantDays(long endpointsystemId, int dormantDays)
        {
            DateTime dormantDate = DateTime.Today.Subtract(new TimeSpan(dormantDays, 0, 0, 0));

            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(tranaction)
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

        public static List<SAPHANAUserinfo> GetByEndpointIdIncludeAccountExpiryDays(long endPointSysemId, int accountExpiryDays)
        {
            DateTime accountExpiryDate = DateTime.Today.Add(new TimeSpan(accountExpiryDays, 0, 0, 0));

            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(transaction)
                    .Get(user => user.EndPointSystemId == endPointSysemId
                    && user.IsUserLocked == false
                    && user.validToDate.HasValue == true
                    && DateTime.Compare(user.validToDate.Value, DateTime.Today) > 0
                    && DateTime.Compare(user.validToDate.Value, accountExpiryDate) < 0
                    , user => user.EndPointSystem, user => user.EndPointSystem.EndPointSystemType).ToList());

            }

        }

        public static List<SAPHANAUserinfo> GetByEndpointIdIncludeIsUserLocked(long endPointSysemId)
        {

            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAUserinfo>(transaction)
                    .Get(user => user.EndPointSystemId == endPointSysemId
                    && user.IsUserLocked == true
                    && user.validToDate.HasValue == true
                    && DateTime.Compare(user.validToDate.Value, DateTime.Today) > 0
                    , user => user.EndPointSystem, user => user.EndPointSystem.EndPointSystemType).ToList());

            }

        }

        public static void SaveUsers(IList<SAPHANAUserinfo> list, long EndpointSystemId, string currentUser)
        {
            IList<SAPHANAUserinfo> listExistingUsers = new List<SAPHANAUserinfo>();
            Dictionary<string, SAPHANAUserinfo> dictExistingUsers = new Dictionary<string, DomainModel.SAPHANAUserinfo>();
            SAPHANAUserinfo sapexistingUser = null;

            listExistingUsers = SAPHANAUserinfo.GetSAPUserByEndpointSystemID(EndpointSystemId);

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
            foreach (var user in list)
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
                new SAPHANAUserInfoRepository(dataAccessTransaction).DeleteSAPUserGroupMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPUserProfileMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPHANAUserInfoRepository(dataAccessTransaction).DeleteSAPUserProfileMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPUserRoleMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPHANAUserInfoRepository(dataAccessTransaction).DeleteSAPUserRoleMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPUserGroupMap(long endpointSystemId, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPHANAUserInfoRepository(dataAccessTransaction).DeleteSAPUserGroupMap(endpointSystemId, userId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPUserProfileMap(long endpointSystemId, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPHANAUserInfoRepository(dataAccessTransaction).DeleteSAPUserProfileMap(endpointSystemId, userId);
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSAPUserRoleMap(long endpointSystemId, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPHANAUserInfoRepository(dataAccessTransaction).DeleteSAPUserRoleMap(endpointSystemId, userId);
                dataAccessTransaction.Commit();
            }
        }

        #region EF Conjunction Table Classes
        public class SAPHANAUserGroup : IAggregateRoot
        {
            public long SAPHANAGroupInfo_Id { get; set; }
            public long SAPHANAUserinfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPHANAUserProfile : IAggregateRoot
        {
            public long SAPHANAUserinfo_Id { get; set; }
            public long SAPHANAProfileInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPHANAUserParameter : IAggregateRoot
        {
            public int SAPHANAUserInfo_Id { get; set; }
            public long SAPHANAParameterInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPHANACompositeChildRoleMap : IAggregateRoot
        {
            public long SAPHANASingleRoleInfo_Id { get; set; }
            public long SAPHANACompositeRoleInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        public class SAPHANAUserRole : IAggregateRoot
        {
            public long SAPHANARoleInfo_Id { get; set; }
            public long SAPHANAUserinfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
            public bool? IsDirectlyAttached { get; set; }
            public string Role_FromDate { get; set; }
            public string Role_ToDate { get; set; }
        }
        public class SAPHANASingleRoleTcodes : IAggregateRoot
        {
            public long SAPHANATCodeInfo_Id { get; set; }
            public long SAPHANASingleRoleInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }
        #endregion

        public static void AddSingleRoleTCodes(DataTable dtAlltcode, long endpointSystemId, IList<string> deltaRoles, bool isFirstTimeDiscovery)
        {
            IList<SAPHANASingleRoleTcodes> listSingleRoleTcodes = new List<SAPHANASingleRoleTcodes>();
            try
            {
                SAPHANATCodeInfo objSAPTCodeInfo = null;
                SAPHANASingleRoleInfo objSingleRoleInfo = null;
                SAPHANASingleRoleTcodes objSapSingleRoleTcodeMap = null;
                var lstTCodes = SAPHANATCodeInfo.GetByEndpointId(endpointSystemId);
                var lstSapRoles = SAPHANASingleRoleInfo.GetByEndPointSystemId(endpointSystemId).ToList();
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
                        objSAPTCodeInfo = new SAPHANATCodeInfo();
                        objSAPTCodeInfo = lstTCodes.FirstOrDefault(tcode => tcode.TCODE_Code == TCode && tcode.EndPointSystemId == endpointSystemId);

                        objSingleRoleInfo = new SAPHANASingleRoleInfo();
                        objSingleRoleInfo = lstSapRoles.FirstOrDefault(role => role.SingleRole_Code == RoleName && role.EndPointSystemId == endpointSystemId);


                        if (objSingleRoleInfo != null && objSAPTCodeInfo != null)
                        {
                            SAPHANASingleRoleTcodes objSingleRoleTCode = listSingleRoleTcodes.FirstOrDefault
                                            (
                                                roletcode => roletcode.SAPHANASingleRoleInfo_Id == objSingleRoleInfo.Id
                                                && roletcode.SAPHANATCodeInfo_Id == objSAPTCodeInfo.Id
                                                && roletcode.EndpointSystemId == endpointSystemId
                                            );
                            if (objSingleRoleTCode == null)
                            {
                                objSapSingleRoleTcodeMap = new SAPHANASingleRoleTcodes();
                                objSapSingleRoleTcodeMap.SAPHANATCodeInfo_Id = objSAPTCodeInfo.Id;
                                objSapSingleRoleTcodeMap.SAPHANASingleRoleInfo_Id = objSingleRoleInfo.Id;
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

        private static void SaveSAPSingleRoleTcodeMap(IList<SAPHANASingleRoleTcodes> listSAPSingleRoleTcodes, long endpointSystemId, bool clearAllFirst)
        {
            if (clearAllFirst)
            {
                SAPHANAUserinfo.DeleteSAPTCodeSingleRoleMap(endpointSystemId);
            }
            else
            {
                var UniqueSingleRoles =
                                from roletcode in listSAPSingleRoleTcodes
                                join roleinfo in SAPHANASingleRoleInfo.GetAll()
                                on roletcode.SAPHANASingleRoleInfo_Id equals roleinfo.Id
                                select roleinfo;
                UniqueSingleRoles = UniqueSingleRoles.Distinct().ToList();
                UniqueSingleRoles.ToList().ForEach(l => SAPHANAUserinfo.DeleteSAPTCodeSingleRoleMap(endpointSystemId, l.Id));
            }

            SQLBulkUploader<SAPHANASingleRoleTcodes> sqlSAPSingleRoleTcodeMap = null;
            try
            {
                string tableName = "SAPHANATCodeInfoSAPHANASingleRoleInfo";
                sqlSAPSingleRoleTcodeMap = new SQLBulkUploader<SAPHANASingleRoleTcodes>(tableName);
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
                new SAPHANAUserInfoRepository(dataAccessTransaction).DeleteSAPTCodeSingleRoleMap(endpointSystemId, singleRoleId);
                dataAccessTransaction.Commit();
            }
        }

        private static void DeleteSAPTCodeSingleRoleMap(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SAPHANAUserInfoRepository(dataAccessTransaction).DeleteSAPTCodeSingleRoleMap(endpointSystemId);
                dataAccessTransaction.Commit();
            }
        }

        public static void SaveSAPUserGroupMap(IList<SAPHANAUserGroup> listUserGroup, long endpointSystemId)
        {
            SQLBulkUploader<SAPHANAUserGroup> sqlSAPUserGroupMap = null;
            try
            {
                string tableName = "SAPHANAUserinfoSAPHANAGroupInfo";

                sqlSAPUserGroupMap = new SQLBulkUploader<SAPHANAUserGroup>(tableName);

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

        public static void SaveSAPUserProfileMap(IList<SAPHANAUserProfile> listUserProfile, long endpointSystemId)
        {
            SQLBulkUploader<SAPHANAUserProfile> sqlSAPUserProfileMap = null;
            try
            {
                string tableName = "SAPHANAProfileInfoSAPHANAUserinfo";

                sqlSAPUserProfileMap = new SQLBulkUploader<SAPHANAUserProfile>(tableName);

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

        public static void SaveSAPUserRoleMap(IList<SAPHANAUserRole> listUserRole, long endpointSystemId)
        {
            if (listUserRole != null && listUserRole.Count > 0)
            {
                SQLBulkUploader<SAPHANAUserRole> sqlSAPUserRoleMap = null;
                try
                {
                    listUserRole = listUserRole.Distinct().ToList();
                    string tableName = "SAPHANARoleInfoSAPHANAUserinfo";

                    sqlSAPUserRoleMap = new SQLBulkUploader<SAPHANAUserRole>(tableName);

                    sqlSAPUserRoleMap.Flush(listUserRole);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlSAPUserRoleMap?.Close();
                }
            }
        }

        public static void SaveCompositeChildRoleMap(IList<SAPHANACompositeChildRoleMap> listCompositeChildRoleMap, long endpointSystemId)
        {

            SAPHANAUserinfo.DeleteSAPCompositeRoleSingleRoleMap(endpointSystemId);
            SQLBulkUploader<SAPHANACompositeChildRoleMap> sqlSAPCompositeChildRoleMap = null;
            try
            {
                string tableName = "SAPHANASingleRoleInfoSAPHANACompositeRoleInfo";
                sqlSAPCompositeChildRoleMap = new SQLBulkUploader<SAPHANACompositeChildRoleMap>(tableName);
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
                new SAPHANAUserInfoRepository(dataAccessTransaction).DeleteSAPCompositeRoleSingleRoleMap(endpointSystemId);
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
            IList<AttributeMappingColumns> listAttributes = AttributeMappingColumns.GetByEndpointSystemId(systemId);
            foreach (AttributeMappingColumns attDto in listAttributes)
            {
                bldr.AppendFormat(string.Format("(DI_SAPHANA_UserInfo.{0}=DI_M_User.{1} and DI_SAPHANA_UserInfo.{2} != '' and DI_SAPHANA_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null)  {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
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
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_SAPHANA_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_SAPHANA_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_SAPHANA_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2 AND DI_M_AccountReconcilation.EndpointSystemId = {0}", endPoint.Id.ToString(), considerInActiveAccountForRecon);
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new SAPHANAUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
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

        public static void RemoveSAPUser(List<SAPHANAUserinfo> listUsers)
        {
            foreach (var user in listUsers)
            {
                SAPHANAUserinfo.DeleteSAPUserGroupMap(user.EndPointSystemId, user.Id);
                SAPHANAUserinfo.DeleteSAPUserRoleMap(user.EndPointSystemId, user.Id);
                SAPHANAUserinfo.DeleteSAPUserProfileMap(user.EndPointSystemId, user.Id);
                user.Delete();
            }
        }

        public static void DeleteSAPUserRoleMap(SAPHANAUserinfo sapUser)
        {
            try
            {
                foreach (string role in sapUser.Roles)
                {
                    using (var transaction = new DataAccessTransaction())
                    {
                        long roleid = DomainModel.SAPHANARoleInfo.GetByRoleCode(role).Id;
                        new SAPHANAUserInfoRepository(transaction).DeleteSAPUserInfoSAPRoleInfo(sapUser.Id, roleid);
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

        public static List<SAPHANAUserinfo> GetByRoleCode(string roleCode, long endPointSystemId)
        {
            var roleInfo = DomainModel.SAPHANARoleInfo.GetSAPUserInfoByRoleCodeIncludingSAPUserInfo(roleCode, endPointSystemId);
            return (roleInfo == null || roleInfo.SAPUserinfo == null) ? new List<SAPHANAUserinfo>() : roleInfo.SAPUserinfo;
        }

        public static IList<SAPHANAUserinfo> GetByProfileCodeWithNavigation(string profileCode, long endPointSystemId)
        {
            var profileInfo = SAPHANAProfileInfo.GetByProfileCodeWithNavigation(profileCode, endPointSystemId);
            return (profileInfo == null || profileInfo.SAPUserinfos == null) ? new List<SAPHANAUserinfo>() : profileInfo.SAPUserinfos;
        }

        public static IList<SAPHANAUserinfo> GetSAPUserInfoByDIUserId(long DIUserId, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPHANAUserInfoRepository(dataAccessTransaction).GetSAPUserInfoByDIUserId(DIUserId, accountType));
            }
        }

        public static SAPHANAUserinfo GetByAccountType(long diUserId, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPHANAUserInfoRepository(dataAccessTransaction).GetByAccountType(diUserId, accountType));
            }
        }
        public static void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            var sapUserInfo = SAPHANAUserinfo.GetByUserLogonName(userName, endpointId);
            if (sapUserInfo != null)
            {
                if (sapUserInfo.PwdResetCnt > 0)
                    sapUserInfo.PwdResetCnt++;
                else
                    sapUserInfo.PwdResetCnt = 1;
                sapUserInfo.Save(currentUser);
            }
        }

        public static SAPHANAUserinfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPHANAUserInfoRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }

        public static SAPHANAUserinfo GetUserAccountByUser(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPHANAUserInfoRepository(transaction).GetUserAccountByUser(dIUserId, endPointSystemId));
            }
        }
        public static SAPHANAUserinfo GetPositionPrefixMatchedUser(long dIUserId, long endPointSystemId, string positionPrefix)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPHANAUserInfoRepository(transaction).GetPositionPrefixMatchedUser(dIUserId, endPointSystemId, positionPrefix));
            }
        }
        public static List<SAPHANAUserinfo> GetActiveAccounts(long dIUserId, long endPointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPHANAUserInfoRepository(transaction).GetActiveAccounts(dIUserId, endPointSystemId, accountType));
            }
        }

        public static SAPHANAUserinfo GetLockedAccount(long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPHANAUserInfoRepository(transaction).GetLockedAccount(diuserId, endpointSystemId, accountType));
            }
        }

        public static SAPHANAUserinfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new SAPHANAUserInfoRepository(transaction).GetLockedAccountByName(accountName, diuserId, endpointSystemId, accountType));
            }
        }

        
        /// <summary>
        /// Only for SRM
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="endpointSystemId"></param>
        /// <returns></returns>
        public static SAPHANAUserinfo GetByIsActive(string accountName, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAUserinfo>(dataAccessTransaction)
                       .Get(sapUser => sapUser.UserLogonName == accountName && sapUser.EndPointSystemId == endpointSystemId && sapUser.IsActive).FirstOrDefault());
            }
        }

        
    }
}
