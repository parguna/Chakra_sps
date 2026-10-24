using DI.Chakra.Dto;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.DomainModel
{
    public class ADSUserInfo : DomainModelBase<ADSUserInfo, EntityModel.ADSUserInfo>, IEndpointUserInfo, IEndpointDomainModel
    {

        public ADSUserInfo()
        {
            this.ADSGroups = new List<ADSGroupInfo>();

            this.Groups = new List<string>();
        }

        public long EndPointSystemId { get; set; }

        public string cn { get; set; }

        public string sn { get; set; }

        public string ou { get; set; }

        public int PwdResetCnt { get; set; }

        public string st { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string postalAddress { get; set; }

        public string postalCode { get; set; }

        public string postOfficeBox { get; set; }

        public string physicalDeliveryOfficeName { get; set; }

        public string telephoneNumber { get; set; }

        public string facsimileTelephoneNumber { get; set; }

        public string givenName { get; set; }

        public string initials { get; set; }

        public string distinguishedName { get; set; }

        public string instanceType { get; set; }

        public DateTime? whenCreated { get; set; }

        public DateTime? whenChanged { get; set; }

        public string displayName { get; set; }

        public Int64 uSNCreated { get; set; }

        public string info { get; set; }

        public Int64 uSNChanged { get; set; }

        public string co { get; set; }

        public string department { get; set; }

        public string company { get; set; }

        public string manager { get; set; }

        public string streetAddress { get; set; }

        public string wWWHomePage { get; set; }

        public string name { get; set; }

        public string objectGUID { get; set; }

        public string userAccountControl { get; set; }

        public bool IsAccountDisabled { get; set; }

        public bool IsAccountLocked { get; set; }

        public bool IsPasswordRequired { get; set; }

        public DateTime? PasswordExpirationDate { get; set; }

        public string badPwdCount { get; set; }

        public string codePage { get; set; }

        public string countryCode { get; set; }

        public DateTime? badPasswordTime { get; set; }

        public DateTime? lastLogoff { get; set; }

        public DateTime? lastLogon { get; set; }

        public DateTime? pwdLastSet { get; set; }

        public string primaryGroupID { get; set; }

        public string objectSid { get; set; }

        public DateTime? accountExpires { get; set; }

        public string logonCount { get; set; }

        public string sAMAccountName { get; set; }

        public string sAMAccountType { get; set; }

        public string managedObjects { get; set; }

        public string ipPhone { get; set; }

        public string objectCategory { get; set; }

        public string mail { get; set; }

        public string homePhone { get; set; }

        public string mobile { get; set; }

        public string pager { get; set; }

        public string nTSecurityDescriptor { get; set; }

        public Nullable<int> ExUserId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public DateTime? IACMLastPasswordChange { get; set; }


        public string NewUsrPassword;


        public string profilePath;

        public string businessCategory { get; set; }

        public string division { get; set; }

        public string homeDrive { get; set; }

        public string homeDirectory { get; set; }

        public bool IsHomeFolderPermissionSetFailed { get; set; }

        public string otherPager { get; set; }
        public string otherFascimileTelephoneNumber { get; set; }
        public string extensionAttribute1 { get; set; }
        public string extensionAttribute2 { get; set; }
        public string extensionAttribute3 { get; set; }
        public string extensionAttribute4 { get; set; }
        public string extensionAttribute5 { get; set; }
        public bool NeedO365License { get; set; }

        public string extensionAttribute6 { get; set; }
        public string extensionAttribute7 { get; set; }
        public string extensionAttribute8 { get; set; }
        public string extensionAttribute9 { get; set; }
        public string extensionAttribute10 { get; set; }
        public string extensionAttribute11 { get; set; }
        public string extensionAttribute12 { get; set; }
        public string extensionAttribute13 { get; set; }

        public List<string> Groups;

        public Nullable<long> WogUserId { get; set; }

        public EndPoint EndPointSystem { get; set; }

        public List<ADSGroupInfo> ADSGroups { get; set; }

        //private static IList<ADUserGroup> diUserGroups = new List<ADUserGroup>();

        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (ADSGroups != null)
                {
                    foreach (var item in ADSGroups)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }

        public override string ToString()
        {
            return this.name;
        }
        public static ADSUserInfo CreateADSUserInfo()
        {
            ADSUserInfo adsUserInfo = new ADSUserInfo();

            return adsUserInfo;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is ADSUserInfo)
            {
                ADSUserInfo other = (ADSUserInfo)obj;
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.cn.GetHashCode();
        }

        public string AccountName
        {
            get
            {
                return this.sAMAccountName;
            }
        }

        public string AccountPassword
        {
            get { return this.NewUsrPassword; }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
        }

        public static ADSUserInfo GetByEndPointSystemId(long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .Get(adsInfo => adsInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static List<ADSUserInfo> GetByIsHomeFolderPermissionSetFailed()
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                       .Get(adsUsr => adsUsr.IsHomeFolderPermissionSetFailed).ToList());
            }
        }

        public static ADSUserInfo GetByEndPointSystemId(long id, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .GetById(id, adsInfo => adsInfo.EndPointSystemId == endPointSystemId));
            }
        }

        public static ADSUserInfo GetUserAccountByEndPointSystemId(long id, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .Get(adsInfo => adsInfo.EndPointSystemId == endPointSystemId && adsInfo.Id == id, adsInfo => adsInfo.ADSGroups).FirstOrDefault());
            }
        }
        [Obsolete]
        public static ADSUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }
        public static ADSUserInfo GetActivePrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetActivePrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }
        public static ADSUserInfo GetPrimaryByUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetPrimaryByUserId(dIUserId, endPointSystemId));
            }
        }
        public static ADSUserInfo GetPrimaryByDIUserIdAccountType(long dIUserId, string accountName, long endPointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetPrimaryByDIUserIdAccountType(dIUserId, accountName, endPointSystemId, accountType));
            }
        }
        //public static IList<ADSUserInfo> GetPrimaryUsersByDIUserId(long dIUserId, long endPointSystemId)
        //{
        //    using (var transaction = new DataAccessTransaction())
        //    {
        //        return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(ui => ui.DIUserId == dIUserId && ui.EndPointSystemId == endPointSystemId
        //            && (ui.AccountType == AccountType.User || ui.AccountType == AccountType.Secondary) && !ui.IsAccountDisabled && !ui.IsLocked
        //            ).ToList());
        //    }
        //}

        //public static void UpdateDiUserId(int UserId, int DiUserId, int endPointSystemId, EndpointType EndPointSystemType)
        //{
        //    ADSUserInfo ADSUInfo = GetByEndPointSystemId(UserId, endPointSystemId);
        //    if (ADSUInfo != null)
        //    {
        //        ADSUInfo.DIUserId = DiUserId;
        //        ADSUInfo.LastModifiedDate = DateTime.Today;
        //        ADSUInfo.Save();
        //    }
        //}

        //public static void UpdateByDiUserIdWithMappingInfo(int userId, int diUserId, string mapTo, int endPointSystemId, EndpointType endPointSystemType, string userName)
        //{
        //    User DIUser = User.GetById(diUserId);

        //    ADSUserInfo ADSUInfo = GetByEndPointSystemId(userId, endPointSystemId);
        //    if (ADSUInfo != null)
        //    {
        //        ADSUInfo.DIUserId = diUserId;
        //        ADSUInfo.IfUnmappedMapTo = mapTo;
        //        ADSUInfo.LastModifiedDate = DateTime.Today;
        //        ADSUInfo.LastModifiedBy = userName;
        //        ADSUInfo.Save();
        //    }
        //}

        public static List<ADSUserInfo> GetUnlockedAccountDetails(long usrId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                //return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                //      .Get(adsUsrInfo => adsUsrInfo.DIUserId == usrId && adsUsrInfo.IsLocked == false, ad => ad.ADSGroups).ToList());

                return ORMapList(new ADSUserInfoRepository(transaction).GetUnlockedAccountDetails(usrId));
            }
        }

        public static List<ADSUserInfo> GetByIsAccountDisabled(long dIUserId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                //return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(item => item.DIUserId == dIUserId
                //        && (item.AccountType == AccountType) && !item.IsAccountDisabled && !item.IsLocked).ToList());

                return ORMapList(new ADSUserInfoRepository(transaction).GetByIsAccountDisabled(dIUserId, accountType));
            }
        }

        //public static ADSUserInfo GetByIsLocked(int DIUserId, int endpointSystemId, bool primaryAccount)
        //{
        //    using (var transaction = new DataAccessTransaction())
        //    {
        //        if (primaryAccount)
        //            return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
        //                   && item.AccountType == AccountType.User && (item.IsAccountDisabled || item.IsAccountLocked)).FirstOrDefault());
        //        else
        //            return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(item => item.DIUserId == DIUserId && item.EndPointSystemId == endpointSystemId
        //                          && item.AccountType == AccountType.Secondary && (item.IsAccountDisabled || item.IsAccountLocked)).FirstOrDefault());

        //    }
        //}

        public static void UpdateADSUserInfoADSGroupInfoByEndPointSystemId(long endPointSystemId, long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new ADSUserInfoRepository(dataAccessTransaction).UpdateADSUserInfoADSGroupInfoByEndPointSystemId(endPointSystemId, id);
                dataAccessTransaction.Commit();
            }
        }

        public static void InsertADUserGroupMapping(long endPointSystemId, long userId, long groupId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new ADSUserInfoRepository(dataAccessTransaction).InsertADUserGroupMapping(endPointSystemId, userId, groupId);
                dataAccessTransaction.Commit();
            }
        }

        public static ADSUserInfo GetByIdIncludeADSGroups(long id)
        {
                using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(tranaction)
                      .Get(adsUser => adsUser.Id == id, adsUser => adsUser.ADSGroups).FirstOrDefault());
            }
        }

        public static List<ADSUserInfo> GetAllPrimaryActiveAccounts(long endpointSystemId, AccountType accountType)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new ADSUserInfoRepository(tranaction).GetAllPrimaryActiveAccounts(endpointSystemId, accountType).ToList());
            }
        }

        public static ADSUserInfo GetByDIUserIdIncludeADSGroups(long userId, AccountType accounttype)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                //return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(tranaction)
                // .Get(adsUser => (adsUser.DIUserId == userId && adsUser.AccountType == accounttype), adsUser => adsUser.ADSGroups).FirstOrDefault());

                return ORMap(new ADSUserInfoRepository(tranaction).GetByDIUserIdIncludeADSGroups(userId, accounttype));
            }
        }



        public static bool DoesExistActiveADSUserInfoByAccountType(long userId, AccountType accounttype)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                return new ADSUserInfoRepository(tranaction).DoesExistActiveADSUserInfoByAccountType(userId, accounttype);
            }
        }



        public static ADSUserInfo GetByDIUserIdIncludeADSGroups(long diUserId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                //return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(tranaction)
                //     .Get(adsUser => adsUser.DIUserId == diUserId, adsUser => adsUser.ADSGroups).FirstOrDefault());

                return ORMap(new ADSUserInfoRepository(tranaction).GetByDIUserIdIncludeADSGroups(diUserId));
            }
        }

        public static List<ADSUserInfo> GetByIdIncludeEndPointSystemType(long dIUserId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                //if (dIUserId > 0)
                //    return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(tranaction)
                //         .Get(adsUser => adsUser.DIUserId == dIUserId && adsUser.IsLocked == false, adsUser => adsUser.EndPointSystem, adsUser => adsUser.EndPointSystem.EndPointSystemType).ToList());
                //else
                //    return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(tranaction)
                //      .Get(adsUser => adsUser.IsLocked == false, adsUser => adsUser.EndPointSystem, adsUser => adsUser.EndPointSystem.EndPointSystemType).ToList());

                if (dIUserId > 0)
                    return ORMapList(new ADSUserInfoRepository(tranaction).GetByIdIncludeEndPointSystemType(dIUserId));
                else
                    return ORMapList(new ADSUserInfoRepository(tranaction).GetByIdIncludeEndPointSystem());
            }
        }



        public static bool DeleteADUserGroupMap(ADSUserInfo adUser)
        {
            bool isDataSaved = false;
            try
            {

                //ADSUserInfo currentADUser = GetByIdIncludeADSGroups(adUser.Id);
                foreach (string group in adUser.Groups)
                {
                    using (var dataAccessTransaction = new DataAccessTransaction())
                    {
                        var GroupOfAd = ADSGroupInfo.GetByCnAndEndpointId(group, adUser.EndPointSystemId);
                        new ADSUserInfoRepository(dataAccessTransaction).DeleteGroup(adUser.Id, GroupOfAd.Id, adUser.EndPointSystemId);

                        dataAccessTransaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                throw ex;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public static void AddUserToGroup(List<User> users, long groupId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                User userToAdd;
                if (users != null && users.Count > 0 && groupId > -1)
                {
                    Group targetGroup = Group.GetById(groupId);
                    if (targetGroup != null)
                    {
                        foreach (User usr in users)
                        {
                            userToAdd = User.GetById(usr.Id);
                            if (userToAdd != null)
                                targetGroup.Users.Add(userToAdd);
                        }
                    }
                    targetGroup.Save(tranaction);
                    tranaction.Commit();
                }
            }
        }

        public static bool AddEntitlement(long epUserId, EntitlementType _entitlementType, string entitlements)
        {
            try
            {
                ADSUserInfo currentADUser = ADSUserInfo.GetByIdIncludeADSGroups(epUserId);

                if (currentADUser == null)
                    return false;
                if (entitlements != null)
                {
                    var _entitlements = entitlements.Split(',').Distinct();
                    foreach (var item in _entitlements)
                    {
                        var grp = currentADUser.ADSGroups.Find(g => g.cn.ToUpper() == item.ToUpper());

                        if (grp == null)
                        {
                            var newGrpToAdd = ADSGroupInfo.GetByCnAndEndpointId(item, currentADUser.EndPointSystemId);
                            if (newGrpToAdd != null)
                                InsertADUserGroupMapping(currentADUser.EndPointSystemId, epUserId, newGrpToAdd.Id);
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

                ADSUserInfo currentADUser = ADSUserInfo.GetByIdIncludeADSGroups(epUserId);

                if (currentADUser == null)
                    return false;

                foreach (var item in entitlements.Split(','))
                {
                    var grp = currentADUser.ADSGroups.Find(g => g.cn.ToUpper() == item.ToUpper());

                    if (null != grp)
                    {
                        using (var transaction = new DataAccessTransaction())
                        {
                            new ADSUserInfoRepository(transaction).DeleteGroup(currentADUser.Id, grp.Id, currentADUser.EndPointSystemId);
                            transaction.Commit();
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


        public static bool AddADSUserIntoGroup(ADSUserInfo adUsr)
        {
            bool isDataSaved = false;
            if (!(adUsr != null && adUsr.Groups != null))
                return isDataSaved;

            try
            {
                using (var transaction = new DataAccessTransaction())
                {
                    ADSUserInfo currentADUser = ADSUserInfo.GetByIdIncludeADSGroups(adUsr.Id);

                    if (currentADUser != null)
                    {
                        foreach (string grpStr in adUsr.Groups)
                        {
                            currentADUser = ADSUserInfo.GetByIdIncludeADSGroups(adUsr.Id);

                            ADSGroupInfo grp = ADSGroupInfo.GetByCnAndEndpointId(grpStr, currentADUser.EndPointSystemId);
                            ADSGroupInfo tmpgrpItem = null;

                            if (grp != null && currentADUser.ADSGroups != null)
                            {
                                tmpgrpItem = currentADUser.ADSGroups.Where(g => g.cn == grp.cn).FirstOrDefault();
                            }

                            if (grp != null && tmpgrpItem == null)
                            {
                                InsertADUserGroupMapping(adUsr.EndPointSystemId, currentADUser.Id, grp.Id);
                            }
                        }
                    }
                }
                isDataSaved = true;
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                throw;
            }
            return isDataSaved;
        }

        public static bool DeleteADUserGroupMapAll(ADSUserInfo adUser)
        {
            bool isDataSaved = false;
            try
            {
                using (var transaction = new DataAccessTransaction())
                {
                    ADSUserInfo currentADUser = ADSUserInfo.GetByIdIncludeADSGroups(adUser.Id);
                    foreach (ADSGroupInfo group in adUser.ADSGroups)
                    {
                        currentADUser.ADSGroups.Remove(group);
                        currentADUser.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                throw ex;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        //public static long AddADSUserInfo(ADSUserInfo adUsr)
        //{
        //    long userID = -1;
        //    if (adUsr == null)
        //        return userID;

        //    try
        //    {
        //        using (var transaction = new DataAccessTransaction())
        //        {
        //            adUsr.whenCreated = adUsr.whenCreated == DateTime.MinValue ? null : adUsr.whenCreated;
        //            adUsr.whenChanged = adUsr.whenChanged == DateTime.MinValue ? null : adUsr.whenChanged;
        //            adUsr.PasswordExpirationDate = adUsr.PasswordExpirationDate == DateTime.MinValue ? null : adUsr.PasswordExpirationDate;
        //            adUsr.badPasswordTime = adUsr.badPasswordTime == DateTime.MinValue ? null : adUsr.badPasswordTime;
        //            adUsr.lastLogoff = adUsr.lastLogoff == DateTime.MinValue ? null : adUsr.lastLogoff;
        //            adUsr.lastLogon = adUsr.lastLogon == DateTime.MinValue ? null : adUsr.lastLogon;
        //            adUsr.pwdLastSet = adUsr.pwdLastSet == DateTime.MinValue ? null : adUsr.pwdLastSet;

        //            adUsr.Save();
        //            transaction.Commit();
        //            userID = adUsr.Id;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        userID = -1;
        //        throw;
        //    }
        //    return userID;
        //}

        public static List<ADSUserInfo> GetByEndpointSystemIdIncludeEndPointSystem(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(dataAccessTransaction)
                       .Get(adsUser => adsUser.EndPointSystemId == endpointSystemId).ToList());
            }
        }

        public static void SaveUsers(IList<ADSUserInfo> list, long EndpointSystemId, string currentUser)
        {
            ADSUserInfo adexistingUser = null;
            AccountReconcilation acctRecon = null;
            List<AccountReconcilation> exsitngAccountrecon = new List<AccountReconcilation>();
            IList <ADSUserInfo> listExistingADusers = new List<ADSUserInfo>();
            List<string> DenyOUs = MasterConfigHelper.ADDeletedUsersOU; // new List<string> { MasterConfigHelper.ADDisabledAccountsOU, MasterConfigHelper.ADSAPLocalDisabledAccountsOU };
            try
            {
                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(EndpointSystemId).ToList();
                listExistingADusers = ADSUserInfo.GetAllEndPointSystemId(EndpointSystemId).ToList();
                var endpoint = EndPoint.GetById(EndpointSystemId);
                var endpointSystemTypeId = endpoint.EndpointSystemTypeId;

                foreach (ADSUserInfo usr in list)
                {
                    try
                    {
                        adexistingUser = listExistingADusers.Where(t =>
                        t.sAMAccountName == usr.sAMAccountName
                        && t.EndPointSystemId == EndpointSystemId).FirstOrDefault();

                        if (adexistingUser == null)
                        {
                            usr.Save(currentUser);
                            listExistingADusers.Add(usr);
                        }
                        else
                        {
                            if(string.IsNullOrWhiteSpace(adexistingUser.mail) && !string.IsNullOrWhiteSpace(usr.mail))
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"MailId:{usr.mail} Updated from back End for user {usr.sAMAccountName}");
                            }
                            if (!string.IsNullOrWhiteSpace(adexistingUser.mail) && string.IsNullOrWhiteSpace(usr.mail))
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"User:{usr.sAMAccountName} MailId:{adexistingUser.mail} Updated to Empty through backend");
                            }
                            if(!string.IsNullOrWhiteSpace(adexistingUser.mail) && !string.IsNullOrWhiteSpace(usr.mail) && usr.mail.ToUpper() != adexistingUser.mail.ToUpper())
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"User:{usr.sAMAccountName} Existing MailId:{adexistingUser.mail} New mailId:{usr.mail} from backend");
                            }
                            AccountReconcilation.SaveAttributeException(adexistingUser, usr, (long)endpointSystemTypeId, usr.whenCreated, usr.sAMAccountName, currentUser);
                            usr.Id = adexistingUser.Id;
                            usr.Save(currentUser);
                        }
                        // acctRecon = AccountReconcilation.GetByUser(usr.EndPointSystemId, usr.Id);
                        acctRecon = exsitngAccountrecon.Where(a => a.EndpointSystemId == usr.EndPointSystemId && a.AccountName.Equals(usr.sAMAccountName,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                        bool isExist = false;
                        if (acctRecon == null)
                        {
                            acctRecon = new AccountReconcilation();
                            AccountReconcilation.UpdateException(acctRecon, true);
                            acctRecon.AccountType = AccountType.None;
                            acctRecon.EndpointSystemId = usr.EndPointSystemId;
                            acctRecon.AccountCreatedDate = usr.whenCreated;
                            if (!string.IsNullOrWhiteSpace(usr.ou) && DenyOUs.Any(u => usr.ou.ToLower().Contains(u.ToLower())))
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"sAMAccountName:{usr.sAMAccountName} Marking as InActive received from backend", DiscoveryObjectType.User);
                                acctRecon.IsDeleted = true;
                                acctRecon.IsLocked = true;
                                acctRecon.ExceptionType = ExceptionTypes.Removed;
                                acctRecon.ExceptionOccuredDate = DateTime.Now;
                            }
                        }
                        else
                        {
                            isExist = true;
                            if (acctRecon.IsDeleted && !DenyOUs.Any(u => !string.IsNullOrWhiteSpace(usr.ou) && usr.ou.ToLower().Contains(u.ToLower())))
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been added without any request made in the system");
                                AccountReconcilation.UpdateException(acctRecon, true);
                            }

                            if (!acctRecon.IsDeleted && DenyOUs.Any(u => !string.IsNullOrWhiteSpace(usr.ou) && usr.ou.ToLower().Contains(u.ToLower())))
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Reconciliation Exception: User {usr?.AccountName}(DisplayName{usr?.DisplayName}) has been removed without any request made in the system");
                                AccountReconcilation.UpdateException(acctRecon, false);
                            }
                            
                        }
                        if ((acctRecon.IsDeleted && acctRecon.IsLocked == false) || (acctRecon.IsDeleted == false && acctRecon.IsLocked == true))
                        {
                            acctRecon.IsLocked = acctRecon.IsDeleted;
                        }
                        acctRecon.ValidToDate = usr.accountExpires.HasValue ? usr.accountExpires : DateTime.MaxValue;
                        acctRecon.FullName = usr.DisplayName;
                        acctRecon.AccountName = usr.sAMAccountName;
                        acctRecon.EndpointUserId = usr.Id;
                        var lastlogon = usr.lastLogon;
                        if (usr.pwdLastSet.HasValue)
                            lastlogon = (lastlogon.HasValue ? lastlogon : DateTime.MinValue) > usr.pwdLastSet ? lastlogon : usr.pwdLastSet;

                        if (lastlogon.HasValue)
                            acctRecon.LastLogonDate = lastlogon.Value;

                        acctRecon.LastModifiedDate = DateTime.Now;

                        acctRecon.Save(currentUser);

                        if (!isExist)
                        {
                            exsitngAccountrecon.Add(acctRecon);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while SaveUsers.", ex);
                    }
                }
                //acctRecon.Save(currentUser);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while SaveUsers.", ex);
            }
            finally
            {
                exsitngAccountrecon?.Clear();
                exsitngAccountrecon = null;

                listExistingADusers?.Clear();
                listExistingADusers = null;
            }
        }

        public static void SaveAccountReconciliation(ADSUserInfo usr, DataAccessTransaction dataAccessTransaction)
        {
            AccountReconcilation acctRecon = AccountReconcilation.GetByUser(usr.EndPointSystemId, usr.Id);
            if (acctRecon == null)
            {
                acctRecon = new AccountReconcilation();
                AccountReconcilation.UpdateException(acctRecon, true);
                acctRecon.AccountType = AccountType.None;
                acctRecon.EndpointSystemId = usr.EndPointSystemId;
                acctRecon.EndpointUserId = usr.Id;
                acctRecon.AccountCreatedDate = usr.whenCreated;
            }
            acctRecon.FullName = usr.DisplayName;
            acctRecon.AccountName = usr.sAMAccountName;
            acctRecon.LastModifiedDate = DateTime.Now;
            acctRecon.Save(dataAccessTransaction);
        }

        public static ADSUserInfo GetByobjectSid(string objectSid)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .Get(usr => usr.objectSid == objectSid).FirstOrDefault());
            }
        }

        public static ADSUserInfo GetBysAMAccountName(string sAMAccountName)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .Get(adsUser => adsUser.sAMAccountName == sAMAccountName).FirstOrDefault());
            }
        }

        public static ADSUserInfo GetByEndPointSystemId(string sAMAccountName, long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                     .Get(adsUser => adsUser.sAMAccountName == sAMAccountName && adsUser.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
        }

        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new ADSUserInfoRepository(dataAccessTransaction).GetByEndPointSystemId(endPointSystemId);
            }
        }

        public static List<ADSUserInfo> GetAllEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(dataAccessTransaction)
                       .Get(adsUser => adsUser.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<ADSUserInfo> GetAllEndPointSystemIdWithoutEagerLoad(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(dataAccessTransaction)
                       .Get(adsUser => adsUser.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<ADSUserInfo> GetByEndPointSystemIdProcess(long endpointSystemId, Process objDiscoveryProcess)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(dataAccessTransaction)
                       .Get(adsUser => adsUser.EndPointSystemId == endpointSystemId && adsUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt).ToList());
            }
        }

        public static IList<ADSUserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<ADSUserInfo> listDeletedUsers = new List<ADSUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }

        public static List<ADSUserInfo> GetByEndPointSystemId(long endpointSystemId, Process objDiscoveryProcess)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(dataAccessTransaction)
                       .Get(adsUser => adsUser.EndPointSystemId == endpointSystemId && adsUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt).ToList());
            }
        }

        public static bool DoesExists(string sAMAccountName, long endpointsystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (endpointsystemId != 0)
                    return new RepositoryBase<EntityModel.ADSUserInfo>(dataAccessTransaction).Any(item => item.IsAccountDisabled == false && item.sAMAccountName.Equals(sAMAccountName, StringComparison.CurrentCultureIgnoreCase) && item.EndPointSystemId == endpointsystemId);
                else
                    return new RepositoryBase<EntityModel.ADSUserInfo>(dataAccessTransaction).Any(item => item.sAMAccountName.Equals(sAMAccountName, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public static List<ADSUserInfo> GetByDIUserId(long diuserId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                //return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                //    .Get(item => item.DIUserId == diuserId && item.AccountType == AccountType.User || item.AccountType == AccountType.Secondary && !item.IsAccountDisabled && !item.IsLocked).ToList());

                return ORMapList(new ADSUserInfoRepository(transaction).GetByDIUserId(diuserId));
            }
        }

        public static ADSUserInfo GetByDIUserIdbyEndpointId(long diuserId, long endpointId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetByDIUserId(diuserId).Where(a => a.EndPointSystemId == endpointId).FirstOrDefault());
            }
        }

        public static ADSUserInfo GetByDIUserIdByUserAccount(long diuserId, long endpointId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetByDIUserIdByUserAccount(diuserId, endpointId).FirstOrDefault());
            }
        }
        public static ADSUserInfo GetByDIUserIdByEndPointId(long diuserId, long endpointId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetByDIUserIdByEndPointId(diuserId, endpointId));
            }
        }

        public static ADSUserInfo GetBysAMAccountNameEndPointSystemId(string sAMAccountName, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(usr => usr.sAMAccountName == sAMAccountName && usr.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        //public static bool IsSecondaryAccount(long endpointUserID)
        //{
        //    using (var transaction = new DataAccessTransaction(false))
        //    {
        //        return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(usr => usr.DIUserId == endpointUserID && usr.AccountType == AccountType.User).ToList()).Count() > 0 ? true : false;
        //    }
        //}

        public static List<ADSUserInfo> GetByPasswordExpirationDate()
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(item => item.PasswordExpirationDate <= DateTime.Now).ToList());
            }
        }

        //public static List<ADSUserInfo> GetUnlockedAccountsIncludingEndpointSystemType(long endpointsystemid)
        //{
        //    using (var transaction = new DataAccessTransaction(false))
        //    {
        //        return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(ept => ept.EndPointSystemId == endpointsystemid && ept.IsLocked == false, ept => ept.EndPointSystem, ept => ept.EndPointSystem.EndPointSystemType).ToList());
        //    }
        //}

        public static ADSUserInfo GetByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                //return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(ui => ui.DIUserId == dIUserId && ui.EndPointSystemId == endPointSystemId).FirstOrDefault());

                return ORMap(new ADSUserInfoRepository(transaction).GetByDIUserId(dIUserId, endPointSystemId));
            }
        }

        public static ADSUserInfo GetByIdWithNavigation(long id)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).GetById(id, adUserInfo => adUserInfo.ADSGroups, adUserInfo => adUserInfo.EndPointSystem, adUserInfo => adUserInfo.EndPointSystem.EndPointSystemType));
            }
        }

        public static List<long> GetBysAMAccountName(string sAMAccountName, long endpointsystemid)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(ui => ui.sAMAccountName == sAMAccountName && ui.EndPointSystemId == endpointsystemid).Select(ui => ui.Id).ToList();
            }
        }

        public static bool HasUserExistByAccountName(int[] endPointSystemId, string usrLogInName, int DIUserId, string ADUserLogOnName)
        {
            bool isIdExist = false;
            ADUserLogOnName = string.Empty;
            if (!(endPointSystemId != null && endPointSystemId.Length == 4))
                return isIdExist;

            using (var transaction = new DataAccessTransaction())
            {
                ADSUserInfo tmpADSUserInfo = GetByDIUserId(DIUserId, endPointSystemId[0]);
                if (tmpADSUserInfo != null)
                    ADUserLogOnName = tmpADSUserInfo.sAMAccountName;

                List<long> adsUser = GetBysAMAccountName(usrLogInName, endPointSystemId[0]);
                List<long> sapUser = SAPUserinfo.GetByUserLogonNames(usrLogInName, endPointSystemId[1]);
                var sapepUser = SAPEPUserInfo.GetIdByUserName(usrLogInName, endPointSystemId[2]);

                var listTotal1 = adsUser.Union(sapUser);
                var listTotal = listTotal1.Union(sapepUser);

                if (listTotal.Count() > 0)
                    isIdExist = true;
                else
                    isIdExist = false;
            }

            return isIdExist;
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
                bldr.AppendFormat(string.Format("(DI_ADS_UserInfo.{0}=DI_M_User.{1}  and DI_ADS_UserInfo.{2} != '' and DI_ADS_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));
            return string.Empty;
        }

        //public static void MapWithMasterUser(EndPoint endPoint, bool mapall)
        //{
        //    StringBuilder bldR = new StringBuilder();
        //    string strJoin = GetJoinQry(endPoint.Id, mapall);
        //    if (strJoin == string.Empty)
        //        return;

        //    bldR.Append("Update DI_ADS_UserInfo set DI_ADS_UserInfo.DIUserId = DI_M_User.Id, DI_ADS_UserInfo.MappingType='System', DI_ADS_UserInfo.AccountType=1 FROM DI_ADS_UserInfo JOIN DI_M_User on");
        //    bldR.Append(" ");
        //    bldR.Append(strJoin);
        //    bldR.Append(" ");
        //    bldR.AppendFormat("WHERE DI_ADS_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND ISNULL(MappingType,'') <> 'Manual'", endPoint.Id.ToString());
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        new ADSUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
        //        dataAccessTransaction.Commit();
        //    }
        //}

        public static void MapWithMasterUserForReconTable(EndPoint endPoint, bool mapall, long discoveryProcessId)
        {
            try
            {
                var considerInActiveAccountForRecon = AccountReconcilation.GetCanConsiderInActiveFilterCondition(); ;
                StringBuilder bldR = new StringBuilder();
                string strJoin = GetJoinQry(endPoint.Id, mapall);
                if (strJoin == string.Empty)
                    return;
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_ADS_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_ADS_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_ADS_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_User.Id <> 1 AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString(), considerInActiveAccountForRecon);
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
                    DILog.DiscoveryLogger.WriteLog(discoveryProcessId, LogLevelType.Exception, string.Format("Error converting data type nvarchar to bigint. Invalid attribute mapping.This exception occurs when invalid attribute mapping is given while creating the endpoint."), ex);
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
                bldR.Append("Update DI_M_EntitlementReconcilation set DI_M_EntitlementReconcilation.UserId = DI_M_User.Id FROM DI_ADS_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_EntitlementReconcilation on DI_M_EntitlementReconcilation.EndpointUserId = DI_ADS_UserInfo.Id");
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.Id = DI_M_EntitlementReconcilation.AccountReconcilationId");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_ADS_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND DI_M_EntitlementReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString());
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

        public static void DeleteAll<T>(long? endpointSystemId)
        {
            string strTableName = string.Empty;
            try
            {
                Type type = typeof(T);

                switch (type.Name)
                {
                    case "ADSUserInfo":
                        strTableName = "DI_ADS_UserInfo";
                        break;
                    case "ADSGroupInfo":
                        strTableName = "DI_ADS_GroupInfo";
                        break;
                    case "ADSOuInfo":
                        strTableName = "DI_ADS_OuInfo";
                        break;
                    case "ADSComputerInfo":
                        strTableName = "DI_ADS_ComputerInfo";
                        break;
                    case "ADSPrinterInfo":
                        strTableName = "DI_ADS_PrinterInfo";
                        break;
                    case "ADSSharedFolderInfo":
                        strTableName = "DI_ADS_SharedFolderInfo";
                        break;
                    case "User":
                        strTableName = "DI_M_User";
                        break;
                    case "ADSUserGroup":
                        strTableName = "DI_ADS_UserGroup";
                        break;
                    case "ADSComputerGroup":
                        strTableName = "DI_ADS_ComputerGroup";
                        break;
                    case "LinuxUserGroup":
                        strTableName = "DI_Linux_UserGroupMap";
                        break;
                    case "LinuxGroupInfo":
                        strTableName = "DI_Linux_GroupInfo";
                        break;
                    case "LinuxResourceInfo":
                        strTableName = "DI_Linux_ResourceInfo";
                        break;
                    case "":
                    default:
                        break;
                }

                string strQuery = string.Format("DELETE {0}", strTableName);

                if (endpointSystemId != null)
                {
                    strQuery = string.Format("{0} WHERE EndpointSystemId={1}", strQuery, endpointSystemId);
                }

                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Query {0}", strQuery));

                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new ADSUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(strQuery));
                    dataAccessTransaction.Commit();
                }
                DILog.GeneralLogger.WriteLog(LogLevelType.Debug, string.Format("Deleted records from table {0} ", strTableName));

            }
            catch (Exception)
            {
                throw;
            }


        }
        IEndpointUserInfo IEndpointDomainModel.GetEndpointUserInfo(long EndpointUserId)
        {
            return GetById(EndpointUserId);
        }
        public static void PopulateMasterUserFromAD(EndPoint endPoint)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new ADSUserInfoRepository(dataAccessTransaction).PopulateMasterUserFromAD(endPoint.Id, MasterConfigHelper.AuthenticationMode);
                dataAccessTransaction.Commit();
            }
        }


        public static void SaveUserGroupMap(Dictionary<long, string> dicUsers, Dictionary<long, string> dicGroups, Dictionary<string, ArrayList> dicGroupUsers, long endpointSystemid)
        {
            SQLBulkUploader<ADUserGroup> sqlUserGroupMap = null;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new ADSUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM ADSUserInfoADSGroupInfo WHERE EndpointSystemId = {0}", endpointSystemid));
                    dataAccessTransaction.Commit();
                }
                IList<ADUserGroup> listUG = new List<ADUserGroup>();
                ADUserGroup userGroup = null;
                foreach (var group in dicGroupUsers)
                {
                    if (group.Value != null)
                    {
                        foreach (var user in group.Value)
                        {
                            userGroup = new ADUserGroup();

                            userGroup.ADSGroupInfo_Id = (int)dicGroups.FirstOrDefault(x => x.Value.ToString().ToUpper() == group.Key.ToString().ToUpper()).Key;
                            userGroup.ADSUserInfo_Id = (int)dicUsers.FirstOrDefault(x => x.Value.ToString().ToUpper() == user.ToString().ToUpper()).Key;
                            userGroup.EndpointSystemId = (int)endpointSystemid;
                            userGroup.Version = Convert.ToInt16(VersionNo.Available);

                            if (userGroup.ADSUserInfo_Id != 0)
                            {
                                var usrGrp = listUG.FirstOrDefault(item => item.ADSGroupInfo_Id == userGroup.ADSGroupInfo_Id && item.ADSUserInfo_Id == userGroup.ADSUserInfo_Id);
                                if (usrGrp == null)
                                {
                                    listUG.Add(userGroup);
                                }
                            }
                            //diUserGroups.Add(userGroup);
                        }
                    }
                }

                string tableName = "ADSUserInfoADSGroupInfo";
                sqlUserGroupMap = new SQLBulkUploader<ADUserGroup>(tableName);
                sqlUserGroupMap.Flush(listUG);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"SaveUserGroupMap failed.StackTrace {ex.StackTrace} Message:{ex.Message}", ex);
                throw ex;
            }
            finally
            {
                sqlUserGroupMap.Close();
            }
        }

        public class ADUserGroup : IAggregateRoot
        {
            public long ADSGroupInfo_Id { get; set; }
            public long ADSUserInfo_Id { get; set; }
            public long EndpointSystemId { get; set; }
            public int? Version { get; set; }
        }

        public static void RemoveADsUser(List<ADSUserInfo> list)
        {
            foreach (ADSUserInfo user in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new ADSUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM ADSUserInfoADSGroupInfo WHERE ADSUserInfo_Id=" + user.Id + " and EndpointSystemId = " + user.EndPointSystemId + ""));
                    dataAccessTransaction.Commit();
                }
                user.Delete();
            }
        }
        //public static void ProcessDeletedEntitlements(string currentUser)
        //{
        //    try
        //    {
        //        using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
        //        {
        //            if (diUserGroups != null && diUserGroups.Count > 0)
        //            {
        //                List<EntitlementReconcilation> getLst = new List<EntitlementReconcilation>();
        //                List<long> groupIds = diUserGroups.Select(x => x.ADSGroupInfo_Id).Distinct().ToList();
        //                foreach (long groupId in groupIds)
        //                {
        //                    List<long> userIds = diUserGroups.Where(x => x.ADSGroupInfo_Id == groupId).Select(x => x.ADSUserInfo_Id).ToList();
        //                    List<EntitlementReconcilation> getEntitleDetail = EntitlementReconcilation.GetAll().Where(x => x.EntitlementId == groupId).ToList();
        //                    if (getEntitleDetail != null)
        //                        getLst = getEntitleDetail.Where(x => !userIds.Contains(x.EndpointUserId)).ToList();
        //                    foreach (EntitlementReconcilation Econ in getLst)
        //                    {
        //                        if (Econ != null)
        //                        {
        //                            Econ.IsDeleted = true;
        //                            Econ.DeletedDate = DateTime.Now;
        //                            Econ.ExceptionType = ExceptionTypes.Removed;
        //                            Econ.IsResolved = false;
        //                            Econ.ResolvedBy = null;
        //                            Econ.Save(dataAccessTransaction);
        //                        }
        //                    }
        //                }
        //                dataAccessTransaction.Commit();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public static List<ADSUserInfo> GetByCnAndEndpointId(string cn, long endPointSystemId)
        {
            var item = ADSGroupInfo.GetByCnAndEndpointIdForCompilance(cn, endPointSystemId);
            return (item == null | item.ADSUsers == null ? new List<ADSUserInfo>() : item.ADSUsers);
        }

        public static List<ADSUserInfo> GetByEndpointIdAndDormantDays(long endPointSysemId, int dormantDays)
        {
            DateTime dormantDate = DateTime.Today.Subtract(new TimeSpan(dormantDays, 0, 0, 0));

            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .Get(user => user.EndPointSystemId == endPointSysemId
                    && user.IsAccountLocked == false
                    && user.IsAccountDisabled == false
                    && (user.lastLogon == null
                            || (user.lastLogon.HasValue == true
                                    && DateTime.Compare(user.lastLogon.Value, dormantDate) < 0))).ToList());
            }

        }


        public static List<ADSUserInfo> GetByEndpointIdAndLockedDisabledAccounts(long endPointSysemId)
        {

            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .Get(user => user.EndPointSystemId == endPointSysemId
                    && (user.IsAccountDisabled == true
                    || user.IsAccountLocked == true
                    || (user.PasswordExpirationDate.HasValue == true && DateTime.Compare(user.PasswordExpirationDate.Value, DateTime.Today) < 0))).ToList());
            }

        }


        public static List<ADSUserInfo> GetByEndpointIdAndAccountWithNoGroups(long endPointSysemId)
        {

            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .Get(user => user.EndPointSystemId == endPointSysemId
                    && user.IsAccountDisabled == false
                    && user.IsAccountLocked == false
                    && user.ADSGroups.Count == 0).ToList());
            }

        }


        public static List<ADSUserInfo> GetByEndpointIdAndPasswordExpiryDays(long endPointSysemId, int passwordExpiryDays)
        {
            DateTime passwordExpiryDate = DateTime.Today.Add(new TimeSpan(passwordExpiryDays, 0, 0, 0));

            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .Get(user => user.EndPointSystemId == endPointSysemId
                    && user.IsAccountLocked == false
                    && user.IsAccountDisabled == false
                    && user.PasswordExpirationDate.HasValue == true
                    && DateTime.Compare(user.PasswordExpirationDate.Value, DateTime.Today) > 0
                    && DateTime.Compare(user.PasswordExpirationDate.Value, passwordExpiryDate) < 0).ToList());

            }

        }

        public static List<ADSUserInfo> GetByEndpointIdIncludeAccountExpiryDays(long endPointSysemId, int accountExpiryDays)
        {
            DateTime accountExpiryDate = DateTime.Today.Add(new TimeSpan(accountExpiryDays, 0, 0, 0));

            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .Get(user => user.EndPointSystemId == endPointSysemId
                    && user.IsAccountLocked == false
                    && user.IsAccountDisabled == false
                    && user.accountExpires.HasValue == true
                    && DateTime.Compare(user.accountExpires.Value, DateTime.Today) > 0
                    && DateTime.Compare(user.accountExpires.Value, accountExpiryDate) < 0).ToList());

            }

        }
        public static void UpdatePasswordresetcount(long endpointId, string userName, string currentUser)
        {
            ADSUserInfo adsUserInfo = ADSUserInfo.GetBysAMAccountNameEndPointSystemId(userName, endpointId);
            if (adsUserInfo != null)
            {
                if (adsUserInfo.PwdResetCnt > 0)
                    adsUserInfo.PwdResetCnt++;
                else
                    adsUserInfo.PwdResetCnt = 1;
                adsUserInfo.Save(currentUser);
            }
        }
        public static Dictionary<long, string> GetFLINSByDIUserId(long userid, long endpointsystemid, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return new ADSUserInfoRepository(transaction).GetFLINSByDIUserId(userid, endpointsystemid, accountType);
            }
        }

        public static Dictionary<long, string> GetADUsersByDIUserId(long userid, long endpointsystemid, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return new ADSUserInfoRepository(transaction).GetADUsersByDIUserId(userid, endpointsystemid, accountType);
            }
        }

        public static Dictionary<long, string> GetADUsersByEndpointUserId(long userid, long endpointsystemid, AccountType accountType, long endpointUserID)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return new ADSUserInfoRepository(transaction).GetADUsersByDIUserId(userid, endpointsystemid, accountType, endpointUserID);
            }
        }

        public static AccountStatus IsAccountLockedByEndPointId(long userID, List<long> endPointSystemIDs, int adhocAccountType, int endpointSystemType)
        {
            var accountstatus = new AccountStatus() { IsLocked = false, Message = "" };
            using (var transaction = new DataAccessTransaction(false))
            {
                string outmsg = string.Empty;
                accountstatus.IsLocked = new ADSUserInfoRepository(transaction).IsAccountLocked(userID, endPointSystemIDs, (AccountType)adhocAccountType, (EndpointType)endpointSystemType, out outmsg);
                accountstatus.Message = outmsg;
            }
            return accountstatus;
        }

        public static List<ADSUserInfo> GetActiveAccounts(long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new ADSUserInfoRepository(transaction).GetActiveAccounts(diuserId, endpointSystemId, accountType));
            }
        }

        public static ADSUserInfo GetLockedAccount(long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetLockedAccount(diuserId, endpointSystemId, accountType));
            }
        }

        public static ADSUserInfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetLockedAccountByName(accountName, diuserId, endpointSystemId, accountType));
            }
        }

        public static void SaveUserGroup(ADSUserInfo adsUserinfo)
        {

            foreach (var group in adsUserinfo.Groups)
            {
                using (var transaction = new DataAccessTransaction())
                {
                    var adsGroup = ADSGroupInfo.GetByName(group);
                    if (adsGroup != null)
                    {
                        new ADSUserInfoRepository(transaction).InsertADUserGroupMapping(adsUserinfo.EndPointSystemId, adsUserinfo.Id, adsGroup.Id);
                    }
                    transaction.Commit();
                }
            }
        }

        public static bool CheckMailExists(string mailId)
        {

            using (var transaction = new DataAccessTransaction(false))
            {
                return (new RepositoryBase<EntityModel.ADSUserInfo>(transaction)
                    .Any(item => item.mail.Equals(mailId, StringComparison.CurrentCultureIgnoreCase)));
            }
        }

        public static List<ADSUserInfo> GetADUsersForLicenseAssignment()
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.ADSUserInfo>(transaction).Get(usr => usr.NeedO365License).ToList());
            }

        }

        public static ADSUserInfo GetSharedADSUserInfoByName(string userName, AccountType accountType)
        {
            var adEP = DomainModel.EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetSharedADSUserInfoByName(adEP.Id, userName, accountType));
            }
        }
        public static void RemoveADGroupFromUserGroup(List<long> adGroups)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new ADSUserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM ADSUserInfoADSGroupInfo WHERE ADSGroupInfo_Id IN({0})", string.Join(",", adGroups)));
                    dataAccessTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RemoveADGroupFromUserGroup failed", ex);
            }
        }

        public static List<ADSUserInfo> GetHomeFolderAccessAssignmentFailedUsers()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new ADSUserInfoRepository(dataAccessTransaction).Get(usr => usr.IsHomeFolderPermissionSetFailed == true).ToList());
            }
        }

        public static bool IsAllowedExternalEmailAccess(long userId, string mail)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new ADSUserInfoRepository(dataAccessTransaction).IsAllowedExternalEmailAccess(userId, mail);
            }
        }

        public static ADSUserInfo GetAccountDisabledByEndpoint(long diuserId, long endpointId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new ADSUserInfoRepository(transaction).GetAccountDisabledByEndpoint(diuserId, endpointId));
            }
        }
        /// <summary>
        /// This is used to get the EEGroup as per User(FullTimeEmployee). IDMS will update the EEGroup from Successfactor.
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetExtensionAttribute13(DomainModel.User user)
        {
            var eeGroupsValues = MasterConfigHelper.SFEEGroupValueAndName;
            if (user != null && user.FullTimeEmployee && !string.IsNullOrWhiteSpace(user.EEGroup) && user.EEGroup != "0")
            {
                var defaultValue = default(KeyValuePair<int, string>);
                var matched = eeGroupsValues.FirstOrDefault(i => i.Key.ToUpper() == user.EEGroup.Trim().ToUpper());
                if (!defaultValue.Equals(matched))
                {
                    return matched.Value;
                }
                return string.Empty;
            }
            else
            {
                return string.Empty;
            }

        }
    }
}
