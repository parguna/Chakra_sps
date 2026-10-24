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
    public class Office365UserInfo : DomainModelBase<Office365UserInfo, EntityModel.Office365UserInfo>, IEndpointUserInfo, IEndpointDomainModel
    {

        public Office365UserInfo()
        {
            this.O365Groups = new List<Office365GroupInfo>();
        }

        public long EndPointSystemId { get; set; }
        public string City { get; set; }
        public string Company { get; set; }
        public string ConsumerNetID { get; set; }
        public string CountryOrRegion { get; set; }
        public string Department { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public string ExchangeVersion { get; set; }
        public string Fax { get; set; }
        public string FirstName { get; set; }
        public string HomePhone { get; set; }
        public string Identity { get; set; }
        public bool? IsLinked { get; set; }
        public bool? IsSecurityPrincipal { get; set; }
        public bool? IsValid { get; set; }
        public string LastName { get; set; }
        public string LinkedMasterAccount { get; set; }
        public string Manager { get; set; }
        public string MicrosoftOnlineServicesID { get; set; }
        public string MobilePhone { get; set; }
        public string Name { get; set; }
        public string NetID { get; set; }
        public string Office { get; set; }
        public string OrganizationalUnit { get; set; }
        public string OrganizationId { get; set; }
        public string Phone { get; set; }
        public string SamAccountName { get; set; }
        public string Sid { get; set; }
        public string SimpleDisplayName { get; set; }
        public string SKUAssigned { get; set; }
        public string Title { get; set; }
        public string UserAccountControl { get; set; }
        public string UserPrincipalName { get; set; }
        public DateTime? WhenCreated { get; set; }
        public string WindowsEmailAddress { get; set; }
        public string WindowsLiveID { get; set; }
        public int PwdResetCnt { get; set; }
        public string mail { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? IACMLastPasswordChange { get; set; }
        public string MailboxUsage { get; set; }
        public DateTime? LastLogon { get; set; }
        public string ValidationStatus { get; set; }
        public bool? LicenseReconciliationNeeded { get; set; }
        public string UsageLocation { get; set; }
        public string Alias { get; set; }
        public string RetentionPolicy { get; set; }
        public bool? LitigationHoldEnabled { get; set; }
        public bool? RetentionHoldEnabled { get; set; }
        public string ForwardingSmtpAddress { get; set; }
        public string LitigationHoldDuration { get; set; }
        public bool IsMailboxEnabled { get; set; }
        public string Languages { get; set; }
        public string RoleAssignmentPolicy { get; set; }
        public string SharingPolicy { get; set; }
        public string ArchiveName { get; set; }
        public string ArchiveStatus { get; set; }
        public string RecipientType { get; set; }
        public string SendAs { get; set; }
        public string SendonBehalf { get; set; }
        public string FullAccess { get; set; }
        public string AccountSkuId { get; set; }
        public string ServicePlanInfo { get; set; }
        public bool IsSkypeOnPrem { get; set; }
        public bool IsShared { get; set; }

        public string NewUsrPassword;

        public List<string> Groups;
        public EndPoint EndPointSystem { get; set; }
        public List<Office365GroupInfo> O365Groups { get; set; }

        public string AccountName
        {
            get
            {
                return this.WindowsEmailAddress;
            }
        }

        public string AccountPassword
        {
            get
            {
                return NewUsrPassword;
            }
        }

        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();
                if (O365Groups != null)
                {
                    foreach (var item in O365Groups)
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

        public Office365UserServicePlan ServicePlanObject
        {
            get
            {
                if (this.ServicePlanInfo != null && this.ServicePlanInfo != string.Empty)
                {
                    return UtilityMethods.Deserialize<Office365UserServicePlan>(this.ServicePlanInfo);
                }
                else
                {
                    return null;
                }
            }
        }

        public static bool DoesExists(string emailId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.Office365UserInfo>(dataAccessTransaction).Any(item => item.WindowsEmailAddress.Equals(emailId, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public static Office365UserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new O365UserInfoRepository(transaction).GetPrimaryByDIUserId(dIUserId, endPointSystemId));
            }
        }

        public static List<Office365UserInfo> GetAllByEndpointId(long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.Office365UserInfo>(dataAccessTransaction).Get(item => item.EndPointSystemId == endpointId).ToList());
            }
        }

        public static void SaveUsers(IList<Office365UserInfo> list, long endpointId, string currentUser, List<AccountReconcilation> exsitngAccountrecon, List<Office365UserInfo> allExistingUsers)
        {
            try
            {               

                if (allExistingUsers == null) allExistingUsers = new List<Office365UserInfo>();
                if (exsitngAccountrecon == null) exsitngAccountrecon = new List<AccountReconcilation>();

                AccountReconcilation acctRecon = null;
                Office365UserInfo o365existingUser = null;
                foreach (Office365UserInfo usr in list)
                {
                    try
                    {
                        o365existingUser = allExistingUsers.Where(item => item.UserPrincipalName.Equals(usr.UserPrincipalName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        if (o365existingUser == null)
                        {
                            usr.EndPointSystemId = endpointId;
                            usr.CreatedBy = "System";
                            usr.CreatedDate = DateTime.Now;
                            usr.Save("System");

                            allExistingUsers.Add(usr);
                        }
                        else
                        {
                            usr.Id = o365existingUser.Id;
                            usr.EndPointSystemId = endpointId;
                            usr.CreatedDate = o365existingUser.CreatedDate;
                            usr.CreatedBy = o365existingUser.CreatedBy;
                            usr.LastModifiedDate = DateTime.Now;
                            usr.LastModifiedBy = "System";
                            usr.Save("System");
                        }

                        acctRecon = exsitngAccountrecon.Where(a => a.EndpointSystemId == usr.EndPointSystemId && a.AccountName != null && a.AccountName.Equals(usr.WindowsEmailAddress,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                        bool isExist = false;
                        if (acctRecon == null)
                        {
                            acctRecon = new AccountReconcilation();
                            AccountReconcilation.UpdateException(acctRecon, true);
                            acctRecon.AccountType = AccountType.None;
                            acctRecon.EndpointSystemId = usr.EndPointSystemId;
                            acctRecon.AccountCreatedDate = usr.CreatedDate;
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
                        acctRecon.LastLogonDate = (usr.LastLogon == null || usr.LastLogon == DateTime.MinValue ? null : usr.LastLogon);
                        acctRecon.FullName = usr.DisplayName;
                        acctRecon.AccountName = usr.WindowsEmailAddress;
                        acctRecon.LastModifiedDate = DateTime.Now;
                        acctRecon.Save("System");

                        if (!isExist)
                        {
                            exsitngAccountrecon.Add(acctRecon);
                        }

                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Error while Office 365 SaveUsers AccountName:{usr?.AccountName}.", ex);
                    }
                }
            }
            catch(OutOfMemoryException ex)
            {
                throw new Exception("OutOfMemoryException on SaveUsers ", ex);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Error while Office 365 SaveUsers.", ex);
            }
        }

        public static IList<Office365UserInfo> GetDeletedUsers(long EndpointId, Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<Office365UserInfo> listDeletedUsers = new List<Office365UserInfo>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.Office365UserInfo>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId &&
                           (objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        )).ToList());
                }
                return listDeletedUsers;
            }
        }

        public static void RemoveOffice365User(List<Office365UserInfo> list)
        {
            foreach (Office365UserInfo user in list)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new O365UserInfoRepository(dataAccessTransaction).DeleteUserGroup(user.Id, user.EndPointSystemId);
                    dataAccessTransaction.Commit();
                }
                user.Delete();
            }
        }

        public static Dictionary<long, string> GetAllByEndPointSystemId(long endPointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new O365UserInfoRepository(dataAccessTransaction).GetByEndpointSystemId(endPointSystemId);
            }
        }

        public static void SaveUserGroupMap(Dictionary<long, string> dicUsers, Dictionary<long, string> dicGroups, Dictionary<string, ArrayList> dicGroupUsers, long endpointSystemid)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new O365UserInfoRepository(dataAccessTransaction).ExecuteQuery(string.Format("DELETE FROM Office365UserInfoOffice365GroupInfo WHERE EndpointSystemId = " + endpointSystemid + ""));
                dataAccessTransaction.Commit();
            }

            IList<Office365UserGroup> listUG = new List<Office365UserGroup>();

            Office365UserGroup userGroup = null;

            foreach (var group in dicGroupUsers)
            {
                try
                {
                    var groupId = dicGroups.FirstOrDefault(x => x.Value.Trim().Equals(group.Key.Trim(), StringComparison.InvariantCultureIgnoreCase)).Key;
                    if (groupId == 0) continue;
                    foreach (var user in group.Value)
                    {
                        try
                        {
                            var userId = dicUsers.FirstOrDefault(x => x.Value.ToString().Trim().Equals(user.ToString().Trim(), StringComparison.InvariantCultureIgnoreCase)).Key;
                            if (userId == 0) continue;

                            userGroup = new Office365UserGroup();
                            userGroup.Office365GroupInfo_GroupId = groupId;
                            userGroup.Office365UserInfo_UserId = userId;
                            userGroup.EndpointSystemId = endpointSystemid;
                            listUG.Add(userGroup);
                        }
                        catch (Exception ex)
                        {
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Save UserGroup{group.Key} user:{user} failed",ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Save UserGroup{group.Key} failed", ex);
                }
            }

            listUG = listUG.Distinct().ToList();
            foreach (var item in listUG)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new O365UserInfoRepository(dataAccessTransaction).InsertOffice365UserGroupMapping(endpointSystemid, item.Office365UserInfo_UserId, item.Office365GroupInfo_GroupId);
                    dataAccessTransaction.Commit();
                }
            }

        }
        public static void DeleteOffic365UserGroupMapping(long endpointSystemid, long userId, List<string> groups)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if(groups != null)
                {
                    foreach (var item in groups)
                    {
                        var group = new O365GroupInfoRepository(dataAccessTransaction).GetByEndPointSystemIdAndName(endpointSystemid, item);
                        if (group != null)
                        {
                            new O365UserInfoRepository(dataAccessTransaction).DeleteGroup(userId, group.Id, endpointSystemid);
                        }
                    }
                    dataAccessTransaction.Commit();
                }
            }
        }
        public static void InsertOffic365UserGroupMapping(long endpointSystemid,long userId,List<string> groups)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (groups != null)
                {
                    foreach (var item in groups)
                    {
                        var group = new O365GroupInfoRepository(dataAccessTransaction).GetByEndPointSystemIdAndName(endpointSystemid, item);
                        if (group != null)
                        {
                            new O365UserInfoRepository(dataAccessTransaction).InsertOffice365UserGroupMapping(endpointSystemid, userId, group.Id);
                        }
                    }
                    dataAccessTransaction.Commit();
                }
                
            }
        }
        public static Office365UserInfo GetByIdIncludeGroups(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Office365UserInfo>(dataAccessTransaction)
                      .Get(users => users.Id == id, users => users.O365Groups).FirstOrDefault());
            }
        }

        public static Office365UserInfo GetByUserPrincipleName(string userPrinciple)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Office365UserInfo>(dataAccessTransaction)
                      .Get(users => users.UserPrincipalName == userPrinciple).FirstOrDefault());
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
                bldr.AppendFormat(string.Format("(DI_Office365_UserInfo.{0}=DI_M_User.{1}  and DI_Office365_UserInfo.{2} != '' and DI_Office365_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null) {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
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
                bldR.Append("Update DI_M_AccountReconcilation set DI_M_AccountReconcilation.UserId = DI_M_User.Id, DI_M_AccountReconcilation.MappingType=1, DI_M_AccountReconcilation.AccountType=1 FROM DI_Office365_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.EndpointUserId = DI_Office365_UserInfo.Id");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_Office365_UserInfo.EndpointSystemId = {0} AND DI_M_AccountReconcilation.EndpointSystemId = {0} AND {1}  AND DI_M_User.Id <> 1 AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString(),considerInActiveAccountForRecon);
                ExecuteQueryWithCommandTimeout(Convert.ToString(bldR), 0);
            }
            catch (DbException ex)
            {
                if (ex.ErrorCode == -2146232060)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Error converting data type nvarchar to bigint. Invalid attribute mapping.This exception occurs when invalid attribute mapping is given while creating the endpoint.discoveryProcessId:{discoveryProcessId}",ex);
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
                bldR.Append("Update DI_M_EntitlementReconcilation set DI_M_EntitlementReconcilation.UserId = DI_M_User.Id FROM DI_Office365_UserInfo JOIN DI_M_User on");
                bldR.Append(" ");
                bldR.Append(strJoin);
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_EntitlementReconcilation on DI_M_EntitlementReconcilation.EndpointUserId = DI_Office365_UserInfo.Id");
                bldR.Append(" ");
                bldR.Append("JOIN DI_M_AccountReconcilation on DI_M_AccountReconcilation.Id = DI_M_EntitlementReconcilation.AccountReconcilationId");
                bldR.Append(" ");
                bldR.AppendFormat("WHERE DI_Office365_UserInfo.EndpointSystemId = {0} AND DI_M_User.Id <> 1 AND DI_M_EntitlementReconcilation.EndpointSystemId = {0} AND ISNULL(DI_M_AccountReconcilation.MappingType,'') <> 2", endPoint.Id.ToString());
                ExecuteQueryWithCommandTimeout(Convert.ToString(bldR), 0);
            }
            catch (DbException ex)
            {
                if (ex.ErrorCode == -2146232060)
                {
                    DILog.DiscoveryLogger.WriteLog(discoveryProcessId, LogLevelType.Exception, string.Format("Error converting data type nvarchar to bigint. Invalid attribute mapping.This exception occurs when invalid attribute mapping is given while creating the endpoint."),ex?.InnerException);
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

        public static Office365UserInfo GetO365UserByUserId(long userId, AccountType accountType, long endpointSytemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new O365UserInfoRepository(dataAccessTransaction).GetO365UserByUserId(userId, accountType, endpointSytemId));
            }
        }

        public static List<Office365LicenseConfig> GetAdhocOffice365ServicePlans(string license, List<ServicePlanDetail> existingServiceplanDetails, AdHocActionType adhocActiontype)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var existingservicePlans = new List<string>();
                if (existingServiceplanDetails != null)
                {
                    existingservicePlans = existingServiceplanDetails.Where(item => item.AccountSkuId == license).Select(item => item.ServiceName).ToList();
                }

                var repo = new RepositoryBase<EntityModel.Office365LicenseConfig>(dataAccessTransaction);
                if (adhocActiontype == AdHocActionType.AmendmentAdd)
                {
                    var result = repo.Get(item => !existingservicePlans.Contains(item.ServicePlan) && item.AccountSkuId == license).Select(item => Mapper.Map<DomainModel.Office365LicenseConfig>(item)).ToList();
                    return result;
                }
                else if (adhocActiontype == AdHocActionType.AmendmentRemove)
                {
                    var result = repo.Get(item => existingservicePlans.Contains(item.ServicePlan) && item.AccountSkuId == license).Select(item => Mapper.Map<DomainModel.Office365LicenseConfig>(item)).ToList();
                    return result;
                }
                else if (adhocActiontype == AdHocActionType.NewAccount)
                {
                    var result = repo.Get(item => item.AccountSkuId == license).Select(item => Mapper.Map<DomainModel.Office365LicenseConfig>(item)).ToList();
                    return result;
                }
                return null;

            }
        }

        public static void O365LastPwdChange(string lastpwdChnageDateTime)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                new O365UserInfoRepository(dataAccessTransaction).O365LastPwdChangeDateTime(lastpwdChnageDateTime);
                dataAccessTransaction.Commit();
            }
        }

        public static List<Office365UserInfo> GetUsersForLicenseAssignment(long endpointId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new O365UserInfoRepository(dataAccessTransaction)
                      .GetAllActiveAccounts(endpointId));
            }

        }
    }
}
