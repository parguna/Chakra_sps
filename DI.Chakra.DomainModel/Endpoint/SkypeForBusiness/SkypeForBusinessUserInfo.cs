using AutoMapper;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Dto;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
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
    public class SkypeForBusinessUserInfo : DomainModelBase<SkypeForBusinessUserInfo, EntityModel.SkypeForBusinessUserInfo>, IEndpointUserInfo, IEndpointDomainModel
    {
        public SkypeForBusinessUserInfo()
        {
            this.SkypeForBusinessResponseGroup = new List<SkypeForBusinessResponseGroupInfo>();
        }
        public string SamAccountName { get; set; }
        public string UserPrincipalName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WindowsEmailAddress { get; set; }
        public string Sid { get; set; }
        public string LineServerURI { get; set; }
        public string OriginatorSid { get; set; }
        public string AudioVideoDisabled { get; set; }
        public string IPPBXSoftPhoneRoutingEnabled { get; set; }
        public string RemoteCallControlTelephonyEnabled { get; set; }
        public string PrivateLine { get; set; }
        public string AcpInfo { get; set; }
        public string HostedVoiceMail { get; set; }
        public string DisplayName { get; set; }
        public string ProxyAddresses { get; set; }
        public string HomeServer { get; set; }
        public string TargetServerIfMoving { get; set; }
        public string EnabledForFederation { get; set; }
        public string EnabledForInternetAccess { get; set; }
        public string PublicNetworkEnabled { get; set; }
        public string EnterpriseVoiceEnabled { get; set; }
        public string EnabledForRichPresence { get; set; }
        public string ExchangeArchivingPolicy { get; set; }
        public string LineURI { get; set; }
        public string SipAddress { get; set; }
        public bool Enabled { get; set; }
        public string TenantId { get; set; }
        public string UserRoutingGroupId { get; set; }
        public string TargetRegistrarPool { get; set; }
        public string VoicePolicy { get; set; }
        public string MobilityPolicy { get; set; }
        public string ConferencingPolicy { get; set; }
        public string PresencePolicy { get; set; }
        public string VoiceRoutingPolicy { get; set; }
        public string RegistrarPool { get; set; }
        public string DialPlan { get; set; }
        public string LocationPolicy { get; set; }
        public string ClientPolicy { get; set; }
        public string ClientVersionPolicy { get; set; }
        public string ArchivingPolicy { get; set; }
        public string LegalInterceptPolicy { get; set; }
        public string PinPolicy { get; set; }
        public string CallViaWorkPolicy { get; set; }
        public string GraphPolicy { get; set; }
        public string ExternalAccessPolicy { get; set; }
        public string HostedVoicemailPolicy { get; set; }
        public string PersistentChatPolicy { get; set; }
        public string UserServicesPolicy { get; set; }
        public string ExperiencePolicy { get; set; }
        public string AddressBookPolicy { get; set; }
        public string ThirdPartyVideoSystemPolicy { get; set; }
        public string HostingProvider { get; set; }
        public string ObjectId { get; set; }
        public string ExUmEnabled { get; set; }
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public string Identity { get; set; }
        public string Guid { get; set; }
        public string ObjectCategory { get; set; }
        public string ObjectClass { get; set; }
        public DateTime? WhenChanged { get; set; }
        public DateTime? WhenCreated { get; set; }
        public string OriginatingServer { get; set; }
        public string IsByPassValidation { get; set; }
        public bool IsValid { get; set; }
        public string ObjectState { get; set; }
        public int? LineURIExtension { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public long EndPointSystemId { get; set; }
        public EndPoint EndPointSystem { get; set; }
        public EntitlementType UserType { get; set; }
        public Nullable<bool> WaitingAccountSync { get; set; }
        public Nullable<long> AccessRequestId{ get; set;}
        public string TenantDialPlan { get; set; }
        public string OnlineVoiceRoutingPolicy { get; set; }
        public string TeamsUpgradePolicy { get; set; }
        public string OnPremLineURI { get; set; }
        public bool IsOnPremAccountExists { get; set; }
        public List<SkypeForBusinessResponseGroupInfo> SkypeForBusinessResponseGroup { get; set; }
        /// <summary>
        /// For temporary purpose we keep the flag true during transfer.
        /// </summary>
        public bool IsRequireLineUriAssignment { get; set; }
        public string AccountName
        {
            get
            {
                return this.UserPrincipalName;
            }
        }
        public string AccountNameFromUPN
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(this.UserPrincipalName))
                    return this.UserPrincipalName.Split(new[] {'@'})[0];

                return SamAccountName;
            }
        }
        public string AccountPassword
        {
            get
            {
                return null;
            }
        }
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (SkypeForBusinessResponseGroup != null)
                {
                    foreach (var item in SkypeForBusinessResponseGroup)
                    {
                        li.Add(item);
                    }
                }
                return li;
            }
        }
        public bool IsLineURIEmpty {
            get
            {
                return string.IsNullOrWhiteSpace(this.LineURI) || string.IsNullOrWhiteSpace(this.OnPremLineURI);
            }
        }
        public string AnyOneLineURI
        {
            get
            {
                string lineUri = string.Empty;
                if (!string.IsNullOrWhiteSpace(this.LineURI))
                    lineUri = this.LineURI;
                //if (!string.IsNullOrWhiteSpace(this.OnPremLineURI))
                //    lineUri = this.OnPremLineURI;
                return lineUri;
            }
        }
        public IEndpointUserInfo GetEndpointUserInfo(long EndpointUserId)
        {
            return GetById(EndpointUserId);
        }
        public static List<SkypeForBusinessUserInfo> GetAllByEndpointId(long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessUserInfo>(dataAccessTransaction).Get(item => item.EndPointSystemId == endpointId).ToList());
            }
        }
        public static List<int?> GetUserExtensionByExcludeDCC(long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SkypeForBusinessUserRepository(dataAccessTransaction).GetAssignedNumbers(endpointId);
            }
        }
        public static void SaveUsers(IList<SkypeForBusinessUserInfo> list, long endpointId, string currentUser)
        {
            try
            {
                var allExistingUsers = SkypeForBusinessUserInfo.GetAllByEndpointId(endpointId);
                List<AccountReconcilation> exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(endpointId).ToList();
                AccountReconcilation acctRecon = null;
                SkypeForBusinessUserInfo sfbExistingUser = null;
                foreach (SkypeForBusinessUserInfo usr in list)
                {
                    try
                    {

                        sfbExistingUser = allExistingUsers.Where(item => item.UserPrincipalName.Equals(usr.UserPrincipalName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        if (sfbExistingUser == null)
                        {
                            usr.EndPointSystemId = endpointId;
                            usr.CreatedBy = currentUser;
                            usr.CreatedDate = DateTime.Now;
                            usr.Save(currentUser);
                            allExistingUsers.Add(usr);
                        }
                        else
                        {
                            usr.Id = sfbExistingUser.Id;
                            usr.EndPointSystemId = endpointId;
                            usr.CreatedDate = sfbExistingUser.CreatedDate;
                            usr.CreatedBy = sfbExistingUser.CreatedBy;
                            usr.LastModifiedDate = DateTime.Now;
                            usr.LastModifiedBy = currentUser;
                            usr.Save(currentUser);
                        }

                        acctRecon = exsitngAccountrecon.Where(a => a.EndpointSystemId == usr.EndPointSystemId && a.EndpointUserId == usr.Id).FirstOrDefault();

                        bool isExist = false;
                        if (acctRecon == null)
                        {
                            acctRecon = new AccountReconcilation();
                            AccountReconcilation.UpdateException(acctRecon, true);
                            acctRecon.AccountType = AccountType.User;
                            acctRecon.EndpointSystemId = usr.EndPointSystemId;
                            acctRecon.EndpointUserId = usr.Id;
                            acctRecon.IsDeleted = !usr.IsValid;
                            acctRecon.AccountCreatedDate = usr.CreatedDate;
                        }
                        else
                        {
                            acctRecon.AccountType = AccountType.User;
                            acctRecon.IsDeleted = !usr.IsValid;
                            isExist = true;
                            if (acctRecon.IsDeleted)
                            {
                                AccountReconcilation.UpdateException(acctRecon, true);
                            }
                        }
                        acctRecon.FullName = usr.DisplayName;
                        acctRecon.AccountName = usr.SamAccountName;
                        acctRecon.LastModifiedDate = DateTime.Now;
                        acctRecon.Save(currentUser);

                        if (!isExist)
                        {
                            exsitngAccountrecon.Add(acctRecon);
                        }

                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while SkypeForBusiness SaveUsers.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while SkypeForBusiness SaveUsers.", ex);
            }
        }

        public static IList<SkypeForBusinessUserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<SkypeForBusinessUserInfo> listDeletedUsers = new List<SkypeForBusinessUserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessUserInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }

        public static void RemoveSkypeForBusinessUser(List<SkypeForBusinessUserInfo> list)
        {
            foreach (SkypeForBusinessUserInfo user in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new SkypeForBusinessUserRepository(dataAccessTransaction).DeleteGroupUser(user.Id, user.EndPointSystemId);
                    dataAccessTransaction.Commit();
                }
                user.Delete();
            }
            
        }
        public static void DeleteSkypeForBusinessUserFroups(long userId,long groupId,long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SkypeForBusinessUserRepository(dataAccessTransaction).DeleteGroupByUserIdGroupId(userId, groupId, endPointSystemId);
                dataAccessTransaction.Commit();
            }
        }
        public static void DeleteAllUserGroups(long endpointSystemid,Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var query = $"DELETE FROM SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo WHERE EndpointSystemId = { endpointSystemid } AND LastModifiedDate < '{objDiscoveryProcess.ProcessStartAt.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")}' OR (CreatedDate < '{objDiscoveryProcess.ProcessStartAt.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")}' AND LastModifiedDate=NULL)";
                new SkypeForBusinessUserRepository(dataAccessTransaction).ExecuteQuery(query);
                dataAccessTransaction.Commit();
            }
        }
        public static void InsertUserGroup(long endpointSystemid, long userId,long groupId, int ismasked)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SkypeForBusinessUserRepository(dataAccessTransaction).InsertOrUpdateSkypeForBusinessUserGroup(endpointSystemid, userId, groupId, ismasked);
                dataAccessTransaction.Commit();
            }
        }
        public static void DeleteUserGroup(long endpointSystemid, long userId, long groupId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SkypeForBusinessUserRepository(dataAccessTransaction).DeleteGroupByUserIdGroupId(userId, groupId,endpointSystemid);
                dataAccessTransaction.Commit();
            }
        }
        public static void DeleteAllGroupByUserId(long endpointSystemid, long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new SkypeForBusinessUserRepository(dataAccessTransaction).DeleteAllGroupByUserId(userId, endpointSystemid);
                dataAccessTransaction.Commit();
            }
        }
        
        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SkypeForBusinessUserRepository(dataAccessTransaction).GetByEndPointSystemId(endPointSystemId);
            }
        }
        public static Dictionary<long, string> GetUserSipAddressByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SkypeForBusinessUserRepository(dataAccessTransaction).GetUserSipAddressByEndPointSystemId(endPointSystemId);
            }
        }
        public static SkypeForBusinessUserInfo GetByIdIncludeGroups(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessUserInfo>(dataAccessTransaction)
                      .Get(users => users.Id == id, users => users.SkypeForBusinessResponseGroup).FirstOrDefault());
            }
        }
        public static SkypeForBusinessUserInfo GetByIdIncludeGroups(long userId, long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
               var data = ORMap(new RepositoryBase<EntityModel.SkypeForBusinessUserInfo>(dataAccessTransaction)
                      .Get(users =>users.EndPointSystemId == endpointId && users.Id == userId, users => users.SkypeForBusinessResponseGroup).FirstOrDefault());
                return data;
            }
        }
        public static SkypeForBusinessUserInfo GetByIdIncludeGroups(long id,EntitlementType entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessUserInfo>(dataAccessTransaction)
                      .Get(users => users.Id == id, users => users.SkypeForBusinessResponseGroup).FirstOrDefault());
            }
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
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_SFB_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_SFB_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_SFB_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_User.Id <> 1 AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString(), considerInActiveAccountForRecon);
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

        public static void UpdateUserResponseGroup(long endpointId, long userId, long groupId,bool isMasked)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new SkypeForBusinessUserRepository(dataAccessTransaction).UpdateUserResponseGroup(endpointId, userId, groupId, isMasked);
                dataAccessTransaction.Commit();
            }
        }

        public static List<SkypeForBusinessUserInfo> GetBySipAddress(List<string> results)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SkypeForBusinessUserRepository(dataAccessTransaction).GetBySIPAddress(results));
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
                bldr.AppendFormat(string.Format("(DI_SFB_UserInfo.{0}=DI_M_User.{1}  and DI_SFB_UserInfo.{2} != '' and DI_SFB_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));
            return string.Empty;
        }
        public static void MapWithMasterUserForEntitlementReconTable(EndPoint endPoint, bool mapall, long discoveryProcessId)
        {
            try
            {
                StringBuilder bldR = new StringBuilder();
                string strJoin = GetJoinQry(endPoint.Id, mapall);
                if (strJoin == string.Empty)
                    return;
                bldR.Append("Update DI_M_EntitlementReconcilation set DI_M_EntitlementReconcilation.UserId = DI_M_User.Id FROM DI_SFB_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_EntitlementReconcilation on DI_M_EntitlementReconcilation.EndpointUserId = DI_SFB_UserInfo.Id");
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.Id = DI_M_EntitlementReconcilation.AccountReconcilationId");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_SFB_UserInfo.EndpointSystemId = {0}  AND DI_M_User.Id <> 1 AND DI_M_EntitlementReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString());
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

        public static SkypeForBusinessEntitlementInfo GetByIdAndType(long id,EntitlementType type)
        {
            SkypeForBusinessEntitlementInfo entitlement =null;
            var userInfo = SkypeForBusinessUserInfo.GetById(id);
            if(userInfo != null)
            {
                entitlement = new SkypeForBusinessEntitlementInfo();
                entitlement.Id = userInfo.Id;
                entitlement.EntitlementName = userInfo.LineURI;
                entitlement.EntitlementDesc = userInfo.LineURI;
                entitlement.EntitlementType = userInfo.UserType;
                entitlement.EndPointSystemId = userInfo.Id;
            }
            return entitlement;


        }
        public static SkypeForBusinessUserInfo GetByEndpointUserId(long endpointUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessUserInfo>(dataAccessTransaction)
                      .Get(grp => grp.Id == endpointUserId, ar => ar.EndPointSystem, ar1 => ar1.EndPointSystem.EndPointSystemType).FirstOrDefault());
            }
        }
        public static SkypeForBusinessUserInfo GetByEndpointSystemId(long endpointSystemId, long endpointUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessUserInfo>(dataAccessTransaction)
                      .Get(i => i.Id == endpointUserId && i.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
        }
        public static SkypeForBusinessUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new SkypeForBusinessUserRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }
        public static SkypeForBusinessUserInfo GetByUserPrincipalName(string userPrincipalName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessUserInfo>(dataAccessTransaction)
                      .Get(users => users.UserPrincipalName == userPrincipalName).FirstOrDefault());
            }
        }
        public static bool IsDCCExists(long endpointId, long number, AdHocActionType actionType, long accessRequestId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new SkypeForBusinessUserRepository(dataAccessTransaction).IsDCCExists(endpointId, number, actionType, accessRequestId);
            }
        }
        public static List<SkypeForBusinessUserInfo> GetAccountWaitingForSync()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessUserInfo>(dataAccessTransaction)
                    .Get(users => users.WaitingAccountSync.HasValue && users.WaitingAccountSync.Value).ToList());
            }
        }
        public static List<PresentationSkypeUserInfo> GetValidActiveUser(long endpointId)
        {
            List<PresentationSkypeUserInfo> results = new List<PresentationSkypeUserInfo>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var delimiter = new string[] { ";ext=", ";EXT=" };
                var usrList =new SkypeForBusinessUserRepository(dataAccessTransaction).GetValidActiveUser(endpointId).ToList();
                foreach (var usr in usrList)
                {
                    if (!string.IsNullOrWhiteSpace(usr.LineURI))
                    {
                        var lineUri = (usr.LineURI.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)[0]).Replace("tel:", "").Trim();
                        if(lineUri.Length == 11 && ((string.IsNullOrWhiteSpace(usr.UserTelephoneNumber) && !string.IsNullOrWhiteSpace(lineUri)) ||
                            (!string.IsNullOrWhiteSpace(usr.UserTelephoneNumber) && !lineUri.Equals(usr.UserTelephoneNumber)
                            )))
                        {
                            results.Add(new PresentationSkypeUserInfo()
                            {
                                AccountName = usr.AccountName,
                                LineURI = usr.LineURI,
                                UserId = usr.UserId,
                                Id = usr.Id,
                                UserOldTelephoneNumber = usr.UserTelephoneNumber,
                                UserTelephoneNumber = lineUri,
                                EmployeeType = usr.EmployeeType
                            });
                        }
                    }
                }
                return results;
            }
        }
        public static SkypeForBusinessUserInfo GetPrimaryByDIUserIdForUpdateTelephoneNumber(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new SkypeForBusinessUserRepository(transaction).GetPrimaryByDIUserIdForUpdateTelephoneNumber(dIUserId, endPointSystemId));
            }
        }
        public static SkypeForBusinessUserInfo GetByIdentity(string userIdentity)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessUserInfo>(dataAccessTransaction)
                      .Get(usr => usr.Identity == userIdentity).FirstOrDefault());
            }
        }
    }
}
