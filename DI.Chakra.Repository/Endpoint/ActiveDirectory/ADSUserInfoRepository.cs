using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class ADSUserInfoRepository : RepositoryBase<ADSUserInfo>
    {
        public ADSUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public void InsertADUserGroupMapping(long endPointSystemId, long userId, long groupId)
        {
            string cmd = string.Format("INSERT INTO [dbo].[ADSUserInfoADSGroupInfo] ([ADSUserInfo_Id],[ADSGroupInfo_Id],[EndpointSystemId]) VALUES({0}, {1},{2})", userId, groupId, endPointSystemId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void UpdateADSUserInfoADSGroupInfoByEndPointSystemId(long endPointSystemId, long id)
        {
            string cmd = "UPDATE [ADSUserInfoADSGroupInfo] SET EndpointSystemId = " + endPointSystemId + " WHERE ADSUserInfo_Id = " + id;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteGroup(long userid, long groupid, long endpointsystemid)
        {
            string cmd = "DELETE FROM [ADSUserInfoADSGroupInfo] WHERE EndpointSystemId = " + endpointsystemid + " AND  ADSUserInfo_Id = " + userid + " AND  ADSGroupInfo_Id = " + groupid;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void PopulateMasterUserFromAD(long endpointsystemid, string authenticationMode)
        {
            StringBuilder bldR = new StringBuilder();
            bldR.AppendFormat($@"Update DI_M_User set
                            DI_M_User.UserName = DI_ADS_UserInfo.sAMAccountName, DI_M_User.AuthenticationMode = '{authenticationMode}'
                            from DI_M_User
                            join DI_M_AccountReconcilation on DI_M_User.Id = DI_M_AccountReconcilation.UserId
                            join DI_ADS_UserInfo on DI_ADS_UserInfo.EndPointSystemId = DI_M_AccountReconcilation.EndpointSystemId and 
                            DI_ADS_UserInfo.Id = DI_M_AccountReconcilation.EndpointUserId
                            where 
                             DI_M_User.Id <> 1 AND DI_M_AccountReconcilation.AccountType = 1
                             AND DI_ADS_UserInfo.EndpointSystemId = {0}", endpointsystemid.ToString());

            this.CurrentContext.Database.ExecuteSqlCommand(bldR.ToString());
        }

        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId)
        {
            return CurrentContext.ADSUserInfos.Where(ep => ep.EndPointSystemId == endPointSystemId).ToDictionary(item => item.Id, item => item.objectSid);
        }

        public IList<PresentationADSUserToGroups> GetADSUserToGroupsReportData(long endPointSystemId)
        {
            return (from adsUser in CurrentContext.ADSUserInfos
                    from adsGrp in adsUser.ADSGroups
                    where adsGrp.groupTypeDesc == Constants.SECURITY_GROUP && adsGrp.EndPointSystemId == endPointSystemId
                    select new PresentationADSUserToGroups
                    {
                        UserName = adsUser.sAMAccountName,
                        UserOU = adsUser.ou,
                        DisplayName = adsUser.displayName,
                        AccountCreatedDate = adsUser.whenCreated.Value,
                        PasswordExpirationDate = adsUser.PasswordExpirationDate.Value,
                        LastLoginDate = adsUser.lastLogon.Value,
                        Disabled = adsUser.IsAccountDisabled ? "YES" : "NO",
                        Email = adsUser.mail,
                        Department = adsUser.department,
                        GroupName = adsGrp.sAMAccountName,
                        GroupDescription = adsGrp.description,
                        GroupOU = adsGrp.distinguishedName
                    }
                    ).ToList();
        }

        public ADSUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && (accountRecon.AccountType == AccountType.User || accountRecon.AccountType == AccountType.Secondary) && accountRecon.IsDeleted == false
                    select adUser).FirstOrDefault();

        }
        public ADSUserInfo GetActivePrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && (accountRecon.AccountType == AccountType.User) && accountRecon.IsDeleted == false
                    select adUser).FirstOrDefault();

        }
        public ADSUserInfo GetPrimaryByUserId(long dIUserId, long endPointSystemId)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && (accountRecon.AccountType == AccountType.User) && accountRecon.IsDeleted == false
                    select adUser).FirstOrDefault();

        }
        public ADSUserInfo GetPrimaryByDIUserIdAccountType(long dIUserId,string accountName, long endPointSystemId, AccountType accountType)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId && adUser.sAMAccountName == accountName
                 && (accountRecon.AccountType == accountType) && !adUser.IsAccountDisabled && (!accountRecon.IsLocked && accountRecon.IsDeleted == false)
                    select adUser).FirstOrDefault();

        }

        public ADSUserInfo GetSharedADSUserInfoByName(long endpointId, string userName, AccountType accountType)
        {
            
            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where adUser.sAMAccountName  == userName && accountRecon.AccountName == userName && adUser.EndPointSystemId == endpointId && accountRecon.EndpointSystemId == endpointId
                 && (accountRecon.AccountType == accountType) && !adUser.IsAccountDisabled && !accountRecon.IsLocked
                    select adUser).FirstOrDefault(); 
        }

        public ADSUserInfo GetByDIUserIdIncludeADSGroups(long userId, AccountType accounttype)
        {

            return (from adUser in CurrentContext.ADSUserInfos.Include("ADSGroups")
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == userId && accountRecon.AccountType == accounttype
                    select adUser).FirstOrDefault();
        }

        public bool  DoesExistActiveADSUserInfoByAccountType(long userId, AccountType accounttype)
        { 
            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == userId && accountRecon.AccountType == accounttype && !accountRecon.IsLocked && !accountRecon.IsDeleted
                    select adUser).Any();
        }

        public ADSUserInfo GetByDIUserIdIncludeADSGroups(long diUserId)
        {
            return (from adUser in CurrentContext.ADSUserInfos.Include("ADSGroups")
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId
                    select adUser).FirstOrDefault();
        }

        public List<ADSUserInfo> GetByIdIncludeEndPointSystemType(long dIUserId)
        {

            return (from adUser in CurrentContext.ADSUserInfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.IsLocked == false
                    select adUser).ToList();
        }

        public List<ADSUserInfo> GetByIdIncludeEndPointSystem()
        {

            return (from adUser in CurrentContext.ADSUserInfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.IsLocked == false
                    select adUser).ToList();
        }

        public List<ADSUserInfo> GetByDIUserId(long diuserId)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diuserId && (accountRecon.AccountType == AccountType.User || accountRecon.AccountType == AccountType.Secondary) 
                   // && !adUser.IsAccountDisabled 
                    && !accountRecon.IsLocked
                    select adUser).ToList();

        }

        public List<ADSUserInfo> GetByDIUserIdByUserAccount(long diuserId, long endPointId)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diuserId && accountRecon.EndpointSystemId == endPointId  
                    && (accountRecon.AccountType == AccountType.User ) 
                   // && !adUser.IsAccountDisabled 
                    && !accountRecon.IsLocked
                    select adUser).ToList();

        }
        public ADSUserInfo GetByDIUserIdByEndPointId(long diuserId, long endPointId)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diuserId && accountRecon.EndpointSystemId == endPointId
                    && (accountRecon.AccountType == AccountType.User) && accountRecon.IsDeleted==false
                   // && !adUser.IsAccountDisabled 
                    && !accountRecon.IsLocked
                    select adUser).FirstOrDefault();

        }
        public List<ADSUserInfo> GetActiveAccounts(long diuserId, long endpointSystemId, AccountType accountType)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId && accountRecon.IsDeleted==false && 
                    accountRecon.AccountType == accountType   && !accountRecon.IsLocked
                    select adUser).ToList();

        }

        public List<ADSUserInfo> GetAllPrimaryActiveAccounts(long endpointSystemId, AccountType accountType)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId &&
                    accountRecon.AccountType == accountType  && !accountRecon.IsLocked && !accountRecon.IsDeleted
                    select adUser).ToList();

        }

        public ADSUserInfo GetLockedAccount(long diuserId, long endpointSystemId, AccountType accountType)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId &&
                    accountRecon.AccountType == accountType // && adUser.IsAccountDisabled 
                    && accountRecon.IsLocked
                    select adUser).FirstOrDefault();
        }

        public ADSUserInfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {
            var accountReconQuery = CurrentContext.AccountReconcilations.Where(accountRecon =>
                        accountRecon.EndpointSystemId == endpointSystemId && accountRecon.AccountType == accountType && (accountRecon.IsLocked || accountRecon.IsDeleted) &&
                       accountRecon.AccountName.Equals(accountName, StringComparison.CurrentCultureIgnoreCase)).AsQueryable();

            if (accountType == AccountType.Service || accountType == AccountType.Shared)
            {
                var UserIds = CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                          .Where(bothuser => bothuser.currentUser.Id == diuserId).Select(bothuser => bothuser.UserList.Id).ToList();

                accountReconQuery = accountReconQuery.Where(a => UserIds.Contains(a.UserId.Value)).Select(a => a);
            }
            else
            {
                accountReconQuery = accountReconQuery.Where(a => a.UserId.Value == diuserId).Select(a => a);
            }

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in accountReconQuery on adUser.Id equals accountRecon.EndpointUserId
                    where adUser.IsAccountDisabled 
                    select adUser).FirstOrDefault();
        }

        public ADSUserInfo GetByDIUserId(long dIUserId, long endPointSystemId)
        {
            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                    select adUser).FirstOrDefault();
        }

        public List<ADSUserInfo> GetUnlockedAccountDetails(long usrId)
        {

            var GetUnlockedAccountDetail = (from adUser in CurrentContext.ADSUserInfos.Include("ADSGroups") select adUser).ToList();

            return (from adUser in CurrentContext.ADSUserInfos.Include("ADSGroups")
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == usrId && accountRecon.IsLocked == false && adUser.EndPointSystemId == accountRecon.EndpointSystemId
                    select adUser).ToList();
        }

        public List<ADSUserInfo> GetByIsAccountDisabled(long dIUserId, AccountType accountType)
        {
            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId
                    && (accountRecon.AccountType == accountType)  // && !adUser.IsAccountDisabled
                    && !accountRecon.IsLocked
                    select adUser).ToList();

        }

        public IList<PresentationADSUserToGroups> GetADSUserToGroupsReportDataByDepartmentAndGroup(long endPointSystemId, long orgUnitId, string GroupName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                List<string> DepartmentUsers = new List<string>();

                string departmentName = "";
                var department = CurrentContext.NOrganisations.Where(a => a.Id == orgUnitId).FirstOrDefault();
                if (department != null)
                    departmentName = department.Name;

                if (orgUnitId != 0)
                {

                    DepartmentUsers = (from user in CurrentContext.User
                                       where user.NOrganisationId == orgUnitId && user.UserName != ""
                                       select user.UserName).ToList();
                }
                else
                {
                    DepartmentUsers = (from user in CurrentContext.User
                                       where user.UserName != ""
                                       select user.UserName).ToList();
                }

                List<PresentationADSUserToGroups> TotalAdsUserGroups = (from adsUser in CurrentContext.ADSUserInfos
                                                                        from adsGrp in adsUser.ADSGroups
                                                                        where adsGrp.cn == GroupName && adsUser.EndPointSystemId == endPointSystemId
                                                                        select new PresentationADSUserToGroups
                                                                        {
                                                                            UserName = adsUser.sAMAccountName,
                                                                            UserOU = adsUser.ou,
                                                                            DisplayName = adsUser.displayName,
                                                                            AccountCreatedDate = adsUser.whenCreated.Value,
                                                                            PasswordExpirationDate = adsUser.PasswordExpirationDate.Value,
                                                                            LastLoginDate = adsUser.lastLogon.Value,
                                                                            Disabled = adsUser.IsAccountDisabled ? "YES" : "NO",
                                                                            Locked = adsUser.IsAccountLocked ? "YES" : "NO",
                                                                            Email = adsUser.mail,
                                                                            Department = string.IsNullOrEmpty(departmentName) ? adsUser.department : departmentName,
                                                                            GroupName = adsGrp.cn,
                                                                            GroupDescription = adsGrp.description,
                                                                            GroupOU = adsGrp.distinguishedName
                                                                        }
                        ).ToList();



                return (from usergrp in TotalAdsUserGroups
                        where DepartmentUsers.FirstOrDefault(user => user == usergrp.UserName) != null
                        select usergrp).ToList();




            }
        }

        public IList<PresentationADSUserToGroups> GetADSUserToGroupsReportData(long endPointSystemId, long orgUnitId, long CutOff)
        {
            using (var transaction = new DataAccessTransaction())
            {
                List<string> DepartmentUsers = new List<string>();
                string departmentName = "";
                var department = CurrentContext.NOrganisations.Where(a => a.Id == orgUnitId).FirstOrDefault();
                if (department != null)
                    departmentName = department.Name;
                if (orgUnitId != 0)
                {

                    DepartmentUsers = (from user in CurrentContext.User
                                       where user.NOrganisationId == orgUnitId && user.UserName != ""
                                       select user.UserName).ToList();
                }
                else
                {
                    DepartmentUsers = (from user in CurrentContext.User
                                       where user.UserName != ""
                                       select user.UserName).ToList();
                }

                List<PresentationADSUserToGroups> TotalAdsUserGroups = (from adsUser in CurrentContext.ADSUserInfos
                                                                        from adsGrp in adsUser.ADSGroups
                                                                        where adsGrp.EndPointSystemId == endPointSystemId
                                                                        select new PresentationADSUserToGroups
                                                                        {
                                                                            UserName = adsUser.sAMAccountName,
                                                                            UserOU = adsUser.ou,
                                                                            DisplayName = adsUser.displayName,
                                                                            AccountCreatedDate = adsUser.whenCreated.Value,
                                                                            PasswordExpirationDate = adsUser.PasswordExpirationDate.Value,
                                                                            LastLoginDate = adsUser.lastLogon.Value,
                                                                            Disabled = adsUser.IsAccountDisabled ? "YES" : "NO",
                                                                            Locked = adsUser.IsAccountLocked ? "YES" : "NO",
                                                                            Email = adsUser.mail,
                                                                            Department = string.IsNullOrEmpty(departmentName) ? adsUser.department : departmentName,
                                                                            GroupName = adsGrp.cn,
                                                                            GroupDescription = adsGrp.description,
                                                                            GroupOU = adsGrp.distinguishedName
                                                                        }
                        ).ToList();



                List<PresentationADSUserToGroups> AdsUserGroups = (from usergrp in TotalAdsUserGroups
                                                                   where DepartmentUsers.FirstOrDefault(user => user == usergrp.UserName) != null
                                                                   select usergrp).ToList();





                long TotalDepartmentUsers = DepartmentUsers.Count;

                List<PresentationADSUserToGroups> list = new List<PresentationADSUserToGroups>();

                foreach (PresentationADSUserToGroups adsUserGrp in AdsUserGroups)
                {


                    if (list.FirstOrDefault(item => item.GroupName == adsUserGrp.GroupName) == null)
                    {
                        //if (AdsUserGroups.Where(grp => grp.GroupName == adsUserGrp.GroupName).ToList().Count > CutOff)
                        //{
                        List<PresentationADSUserToGroups> DepartmentUsersUsingTheGroup = AdsUserGroups.Where(grp => grp.GroupName == adsUserGrp.GroupName).ToList();

                        long TotalUsage = DepartmentUsersUsingTheGroup.Count;
                        double TotalUsageInPercentage = TotalDepartmentUsers > 0 ? 100 * TotalUsage / TotalDepartmentUsers : 0;
                        //TotalUsageInPercentage = TotalUsageInPercentage * 100;

                        PresentationADSUserToGroups userToGrp = new PresentationADSUserToGroups();
                        userToGrp.GroupName = adsUserGrp.GroupName;
                        userToGrp.GroupDescription = adsUserGrp.GroupDescription;
                        userToGrp.TotalUsage = TotalUsage.ToString();
                        userToGrp.UsageInPercentage = TotalUsageInPercentage.ToString();

                        list.Add(userToGrp);
                        //}
                    }

                }

                return list.Distinct().Where(a => Convert.ToInt64(a.UsageInPercentage) >= CutOff).ToList();
            }
        }

        public List<ADSUserInfo> GetByInternetUsers()
        {
            return (from adsUser in CurrentContext.ADSUserInfos
                    from adsGroup in adsUser.ADSGroups
                    where adsGroup.name == "InternetUsers"
                    select adsUser).ToList();

        }

        public void ExecuteQuery(string query)
        {
            CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public List<PresentationAccessRequestItem> GetADAccessRequestItemForDIUserId(string searchString, long? DIUserId)
        {
            return (from apps in CurrentContext.ADSUserInfos
                    where apps.sAMAccountName.Contains(searchString)
                    orderby apps.sAMAccountName
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Application,
                        EndpointType = EndpointType.ActiveDirectory,
                        KeyId = apps.EndPointSystemId,
                        EndpointSystemId = apps.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                        ItemName = apps.sAMAccountName,
                        ItemDescription = apps.displayName
                    }).ToList();
        }

        public List<PresentationAccessRequestItem> GetAccessRequestItemForRemoveRequestSpecailACR(string SearchKey, long DIUserId)
        {

            return (from apps in CurrentContext.ADSUserInfos
                    where apps.sAMAccountName.Contains(SearchKey)
                    orderby apps.sAMAccountName
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Application,
                        EndpointType = EndpointType.ActiveDirectory,
                        KeyId = apps.EndPointSystemId,
                        EndpointSystemId = apps.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                        ItemName = apps.sAMAccountName,
                        ItemDescription = apps.displayName
                    }).ToList();
        }

        public List<PresentationADSDormantAccount> GetByEndpointIdAndDormantDays(long endPointSystemId, int dormantDays)
        {
            DateTime dormantDate = DateTime.Today.Subtract(new TimeSpan(dormantDays, 0, 0, 0));

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    join endpointSystem in CurrentContext.EndPoint on accountRecon.EndpointSystemId equals endpointSystem.Id
                    join diUsers in CurrentContext.User on accountRecon.UserId equals diUsers.Id into owners
                    from o in owners.DefaultIfEmpty()
                    join nOrg in CurrentContext.NOrganisations on o.NOrganisationId equals nOrg.Id into org
                    from ownerOrg in org.DefaultIfEmpty()
                    join nDes in CurrentContext.NDesignations on o.NBusinessRoleId equals nDes.Id into designations
                    from designation in designations.DefaultIfEmpty()
                    where adUser.EndPointSystemId == endPointSystemId && accountRecon.EndpointSystemId == endPointSystemId &&
                    accountRecon.EndpointUserId == adUser.Id && accountRecon.IsLocked == false && accountRecon.LastLogonDate < dormantDate
                    select new PresentationADSDormantAccount
                    {
                        UserId = adUser.Id,
                        sAMAccountName = adUser.sAMAccountName,
                        displayName = adUser.displayName,
                        physicalDeliveryOfficeName = adUser.physicalDeliveryOfficeName,
                        company = adUser.company,
                        pager = adUser.pager,
                        businessCategory = ownerOrg.Name,
                        division = adUser.division,
                        mail = o.EmailId,
                        title = designation.Description,
                        //whenCreated = adUser.whenCreated.HasValue ? adUser.whenCreated.Value.ToString() : string.Empty,
                        //PasswordExpirationDate = adUser.PasswordExpirationDate.HasValue ? adUser.PasswordExpirationDate.Value.ToString() : string.Empty,
                        //lastLogon = adUser.lastLogon.HasValue ? adUser.lastLogon.Value.ToString() : string.Empty,
                        //pwdLastSet = adUser.pwdLastSet.HasValue ? adUser.pwdLastSet.Value.ToString() : string.Empty,
                        //accountExpires = adUser.accountExpires.HasValue ? adUser.accountExpires.Value.ToString() : string.Empty,
                        whenCreated = adUser.whenCreated,
                        PasswordExpirationDate = adUser.PasswordExpirationDate,
                        lastLogon = adUser.lastLogon,
                        pwdLastSet = adUser.pwdLastSet,
                        accountExpires = adUser.accountExpires,
                        logonCount = adUser.logonCount,
                        IsAccountDisabled = adUser.IsAccountDisabled.ToString(),
                        cn = adUser.cn,
                        OU = adUser.ou,
                        Owner = o.DisplayName,
                        IsMapped = (accountRecon.MappingType != MappingType.None).ToString(),
                        HostName = endpointSystem.DisplayName,
                    }).ToList();

        }
        public List<PresentationADSFutureAccountExpiryAccount> GetByEndpointSystemIdAndAccountExpiryDays(long endPointSystemId, int accountExpiryDays)
        {
            DateTime accountExpiryDate = DateTime.Today.Add(new TimeSpan(accountExpiryDays, 0, 0, 0));

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    join endpointSystem in CurrentContext.EndPoint on accountRecon.EndpointSystemId equals endpointSystem.Id
                    join diUsers in CurrentContext.User on accountRecon.UserId equals diUsers.Id into owners
                    from o in owners.DefaultIfEmpty()
                    join nOrg in CurrentContext.NOrganisations on o.NOrganisationId equals nOrg.Id into org
                    from ownerOrg in org.DefaultIfEmpty()
                    join nDes in CurrentContext.NDesignations on o.NBusinessRoleId equals nDes.Id into designations
                    from designation in designations.DefaultIfEmpty()
                    where adUser.EndPointSystemId == endPointSystemId && accountRecon.EndpointSystemId == endPointSystemId
                    && adUser.IsAccountLocked == false
                    && adUser.accountExpires.HasValue == true
                    && DateTime.Compare(adUser.accountExpires.Value, DateTime.Today) > 0
                    && DateTime.Compare(adUser.accountExpires.Value, accountExpiryDate) < 0
                    select new PresentationADSFutureAccountExpiryAccount
                    {
                        UserId = adUser.Id,
                        sAMAccountName = adUser.sAMAccountName,
                        displayName = adUser.displayName,
                        physicalDeliveryOfficeName = adUser.physicalDeliveryOfficeName,
                        company = adUser.company,
                        pager = adUser.pager,
                        businessCategory = ownerOrg.Name,
                        division = adUser.division,
                        mail = o.EmailId,
                        title = designation.Description,
                        whenCreated = adUser.whenCreated,
                        PasswordExpirationDate = adUser.PasswordExpirationDate,
                        lastLogon = adUser.lastLogon,
                        pwdLastSet = adUser.pwdLastSet,
                        accountExpires = adUser.accountExpires,
                        logonCount = adUser.logonCount,
                        IsAccountDisabled = adUser.IsAccountDisabled.ToString(),
                        cn = adUser.cn,
                        OU = adUser.ou,
                        Owner = o.DisplayName,
                        IsMapped = (accountRecon.MappingType != MappingType.None).ToString(),
                        HostName = endpointSystem.DisplayName,
                    }).ToList();

        }
        public List<PresentationADSFuturePasswordExpiryAccount> GetByEndpointIdAndPasswordExpiryDays(long endPointSystemId, int passwordExpiryDays)
        {
            DateTime passwordExpiryDate = DateTime.Today.Add(new TimeSpan(passwordExpiryDays, 0, 0, 0));

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    join endpointSystem in CurrentContext.EndPoint on accountRecon.EndpointSystemId equals endpointSystem.Id
                    join diUsers in CurrentContext.User on accountRecon.UserId equals diUsers.Id into owners
                    from o in owners.DefaultIfEmpty()
                    join nOrg in CurrentContext.NOrganisations on o.NOrganisationId equals nOrg.Id into org
                    from ownerOrg in org.DefaultIfEmpty()
                    join nDes in CurrentContext.NDesignations on o.NBusinessRoleId equals nDes.Id into designations
                    from designation in designations.DefaultIfEmpty()
                    where adUser.EndPointSystemId == endPointSystemId && accountRecon.EndpointSystemId == endPointSystemId
                    && adUser.IsAccountLocked == false
                    && adUser.IsAccountDisabled == false
                    && adUser.PasswordExpirationDate.HasValue == true
                    && DateTime.Compare(adUser.PasswordExpirationDate.Value, DateTime.Today) > 0
                    && DateTime.Compare(adUser.PasswordExpirationDate.Value, passwordExpiryDate) < 0
                    select new PresentationADSFuturePasswordExpiryAccount
                    {
                        UserId = adUser.Id,
                        sAMAccountName = adUser.sAMAccountName,
                        displayName = adUser.displayName,
                        physicalDeliveryOfficeName = adUser.physicalDeliveryOfficeName,
                        company = adUser.company,
                        pager = adUser.pager,
                        businessCategory = ownerOrg.Name,
                        division = adUser.division,
                        mail = o.EmailId,
                        title = designation.Description,
                        whenCreated = adUser.whenCreated,
                        PasswordExpirationDate = adUser.PasswordExpirationDate,
                        lastLogon = adUser.lastLogon,
                        pwdLastSet = adUser.pwdLastSet,
                        accountExpires = adUser.accountExpires,
                        logonCount = adUser.logonCount,
                        IsAccountDisabled = adUser.IsAccountDisabled.ToString(),
                        cn = adUser.cn,
                        OU = adUser.ou,
                        Owner = o.DisplayName,
                        IsMapped = (accountRecon.MappingType != MappingType.None).ToString(),
                        HostName = endpointSystem.DisplayName,
                    }).ToList();

        }
        public List<PresentationADSAccountWithNoGroups> GetByEndpointIdIncludeAccountWithoutGroups(long endPointSystemId)
        {
            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    join endpointSystem in CurrentContext.EndPoint on accountRecon.EndpointSystemId equals endpointSystem.Id
                    join diUsers in CurrentContext.User on accountRecon.UserId equals diUsers.Id into owners
                    from o in owners.DefaultIfEmpty()
                    join nOrg in CurrentContext.NOrganisations on o.NOrganisationId equals nOrg.Id into org
                    from ownerOrg in org.DefaultIfEmpty()
                    where adUser.EndPointSystemId == endPointSystemId && accountRecon.EndpointSystemId == endPointSystemId
                    && adUser.IsAccountLocked == false
                    && adUser.IsAccountDisabled == false
                    && adUser.ADSGroups.Count == 0
                    && (adUser.sAMAccountName != null && adUser.sAMAccountName.Trim() != "")
                    select new PresentationADSAccountWithNoGroups
                    {
                        UserId = adUser.Id,
                        sAMAccountName = adUser.sAMAccountName,
                        displayName = adUser.displayName,
                        physicalDeliveryOfficeName = adUser.physicalDeliveryOfficeName,
                        company = adUser.company,
                        pager = adUser.pager,
                        businessCategory = ownerOrg.Name,
                        division = adUser.division,
                        mail = o.EmailId,
                        title = adUser.title,
                        whenCreated = adUser.whenCreated.HasValue ? adUser.whenCreated.Value.ToString() : string.Empty,
                        PasswordExpirationDate = adUser.PasswordExpirationDate.HasValue ? adUser.PasswordExpirationDate.Value.ToString() : string.Empty,
                        lastLogon = adUser.lastLogon.HasValue ? adUser.lastLogon.Value.ToString() : string.Empty,
                        pwdLastSet = adUser.pwdLastSet.HasValue ? adUser.pwdLastSet.Value.ToString() : string.Empty,
                        accountExpires = adUser.accountExpires.HasValue ? adUser.accountExpires.Value.ToString() : string.Empty,
                        logonCount = adUser.logonCount,
                        cn = adUser.cn,
                        OU = adUser.ou,
                        Owner = o.DisplayName,
                        IsMapped = (accountRecon.MappingType != MappingType.None).ToString(),
                        HostName = endpointSystem.DisplayName,
                    }).Distinct().ToList();

        }
        public List<PresentationADSUsers> GetByEndpointSystemIdIncludeEndPointSystem(long endPointSystemId)
        {
            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    join endpointSystem in CurrentContext.EndPoint on accountRecon.EndpointSystemId equals endpointSystem.Id
                    join diUsers in CurrentContext.User on accountRecon.UserId equals diUsers.Id into owners
                    from o in owners.DefaultIfEmpty()
                    join nOrg in CurrentContext.NOrganisations on o.NOrganisationId equals nOrg.Id into org
                    from ownerOrg in org.DefaultIfEmpty()
                    where adUser.EndPointSystemId == endPointSystemId && accountRecon.EndpointSystemId == endPointSystemId
                    && adUser.IsAccountLocked == false
                    select new PresentationADSUsers
                    {
                        UserId = adUser.Id,
                        sAMAccountName = adUser.sAMAccountName,
                        displayName = adUser.displayName,
                        department = adUser.department,
                        physicalDeliveryOfficeName = adUser.physicalDeliveryOfficeName,
                        company = adUser.company,
                        pager = adUser.pager,
                        businessCategory = adUser.businessCategory,
                        OjectSID = adUser.objectSid,
                        division = adUser.division,
                        mail = o.EmailId,
                        title = adUser.title,
                        whenCreated = adUser.whenCreated,
                        PasswordExpirationDate = adUser.PasswordExpirationDate,
                        lastLogon = adUser.lastLogon,
                        pwdLastSet = adUser.pwdLastSet,
                        accountExpires = adUser.accountExpires,
                        logonCount = adUser.logonCount,
                        cn = adUser.cn,
                        OU = adUser.ou,
                        Owner = o.DisplayName,
                        IsMapped = (accountRecon.MappingType != MappingType.None).ToString(),
                        HostName = endpointSystem.DisplayName,
                        description = adUser.description,
                    }).ToList();

        }

        public Dictionary<long, string> GetADUsersByDIUserId(long userid, long endpointsystemid, AccountType accountType, long endpointUserId)
        {
            bool considerEndpointSystemId = endpointsystemid == 0 ? true : false;

            var temp = (from accountRecon in CurrentContext.AccountReconcilations
                        where accountRecon.UserId != null && !accountRecon.IsLocked && !accountRecon.IsDeleted && accountRecon.AccountType == accountType &&
                          (considerEndpointSystemId || accountRecon.EndpointSystemId == endpointsystemid) && accountRecon.EndpointUserId == endpointUserId
                        select accountRecon);

            if (accountType == AccountType.User || accountType == AccountType.Secondary)
            {
                return temp.Where(accountRecon => accountRecon.UserId.Value == userid).Select(accountRecon => new { Key = accountRecon.EndpointUserId, Value = accountRecon.AccountName }).Distinct().ToDictionary(o => o.Key, o => o.Value);
            }

            var listofUser = CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                        .Where(bothuser => bothuser.currentUser.Id == userid).Select(bothuser => bothuser.UserList.Id).ToList();

            //string ouOfRequestee = CurrentContext.User.Where(lq => lq.Id == userid).Select(lq => lq.SectionCode).FirstOrDefault();
            //var listofUser = CurrentContext.User.Where(lq => lq.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
            return temp.Where(item => item.UserId != null && listofUser.Contains(item.UserId.Value)).Distinct().ToDictionary(o => o.EndpointUserId, o => o.AccountName);

        }

        public Dictionary<long, string> GetFLINSByDIUserId(long userid, long endpointsystemid, AccountType accountType)
        {
            try
            {
                var srmEndpoint = CurrentContext.EndPoint.Where(i => i.ApplicationType == ApplicationType.SRM).FirstOrDefault();

                var srmAccounts = (from accountRecon in CurrentContext.AccountReconcilations
                                  where accountRecon.UserId != null && !accountRecon.IsLocked && !accountRecon.IsDeleted && accountRecon.AccountType == accountType &&
                                    (accountRecon.EndpointSystemId == srmEndpoint.Id)
                                  select accountRecon.AccountName);

                var flinsQuery = (from accountRecon in CurrentContext.AccountReconcilations
                            where accountRecon.UserId != null && !accountRecon.IsLocked && !accountRecon.IsDeleted && accountRecon.AccountType == accountType &&
                              (accountRecon.EndpointSystemId == endpointsystemid)
                            select accountRecon);

                var adEndpoint = CurrentContext.EndPoint.Where(i => i.ApplicationType == ApplicationType.ADSingaporePowerLocal).FirstOrDefault();
                var adQuery = (from accountRecon in CurrentContext.AccountReconcilations
                               where accountRecon.UserId != null && !accountRecon.IsLocked && !accountRecon.IsDeleted && accountRecon.AccountType == accountType &&
                                 (accountRecon.EndpointSystemId == adEndpoint.Id)
                               select accountRecon);
                if (accountType == AccountType.User || accountType == AccountType.Secondary)
                {
                    var adAccounts = adQuery.Where(i=> i.UserId == userid).Select(i=>i.AccountName).Distinct();
                    var flinsUserAccounts = flinsQuery.Where(accountRecon => accountRecon.UserId.Value == userid).Select(accountRecon => new { accountRecon.EndpointUserId, accountRecon.AccountName });

                    flinsUserAccounts = flinsUserAccounts.Where(i=> adAccounts.Contains(i.AccountName));

                    if (srmAccounts.Count() > 0)
                        flinsUserAccounts = flinsUserAccounts.Where(i=> !srmAccounts.Contains(i.AccountName));

                    return flinsUserAccounts.Select(accountRecon => new { Key = accountRecon.EndpointUserId, Value = accountRecon.AccountName }).Distinct().ToDictionary(o => o.Key, o => o.Value);
                }

                var listofUser = CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                            .Where(bothuser => bothuser.currentUser.Id == userid).Select(bothuser => bothuser.UserList.Id).ToList();

                var flinsSpecialAccounts = flinsQuery.Where(item => item.UserId != null && listofUser.Contains(item.UserId.Value)).Distinct();

                var adAccount = adQuery.Where(item => item.UserId != null && listofUser.Contains(item.UserId.Value)).Distinct().Select(i => i.AccountName);
                
                flinsSpecialAccounts = flinsSpecialAccounts.Where(i => adAccount.Contains(i.AccountName));

                if (srmAccounts.Count() > 0)
                {
                    flinsSpecialAccounts = flinsSpecialAccounts.Where(i => !srmAccounts.Contains(i.AccountName));
                }
                return flinsSpecialAccounts.Select(accountRecon => new { Key = accountRecon.EndpointUserId, Value = accountRecon.AccountName }).Distinct().ToDictionary(o => o.Key, o => o.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, string> GetADUsersByDIUserId(long userid, long endpointsystemid, AccountType accountType)
        {
            try
            {
                bool considerEndpointSystemId = endpointsystemid == 0 ? true : false;

                var temp = (from accountRecon in CurrentContext.AccountReconcilations
                            where accountRecon.UserId != null && !accountRecon.IsLocked && !accountRecon.IsDeleted && accountRecon.AccountType == accountType &&
                              (considerEndpointSystemId || accountRecon.EndpointSystemId == endpointsystemid)
                            select accountRecon);

                if (accountType == AccountType.User || accountType == AccountType.Secondary)
                {
                    return temp.Where(accountRecon => accountRecon.UserId.Value == userid).Select(accountRecon=> new { Key = accountRecon.EndpointUserId , Value = accountRecon.AccountName }).Distinct().ToDictionary(o => o.Key, o => o.Value);
                }

                var listofUser = CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                            .Where(bothuser => bothuser.currentUser.Id == userid).Select(bothuser => bothuser.UserList.Id).ToList();

                //string ouOfRequestee = CurrentContext.User.Where(lq => lq.Id == userid).Select(lq => lq.SectionCode).FirstOrDefault();
                //var listofUser = CurrentContext.User.Where(lq => lq.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList();
                return temp.Where(item => item.UserId!=null && listofUser.Contains(item.UserId.Value)).Distinct().ToDictionary(o => o.EndpointUserId, o => o.AccountName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsAccountLocked(long userID, List<long> endPointSystemIDs, AccountType adhocAccountType, EndpointType endpointSystemType, out string outmsg)
        {
            outmsg = "";

            try
            {
                
                var acclistQuery = CurrentContext.AccountReconcilations
                                 .Where(accountRecon => accountRecon.UserId  != null && !accountRecon.IsDeleted
                                         && accountRecon.AccountType == adhocAccountType
                                         && endPointSystemIDs.Contains(accountRecon.EndpointSystemId))
                                 .Select(acc => acc);

                if (adhocAccountType == AccountType.Service || adhocAccountType == AccountType.Shared)
                {
                    var listofUser = CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                           .Where(bothuser => bothuser.currentUser.Id == userID).Select(bothuser => bothuser.UserList.Id).ToList();

                    acclistQuery = acclistQuery.Where(a => a.UserId != null && listofUser.Contains(a.UserId.Value)).Select(a => a);

                }
                else
                {
                    acclistQuery = acclistQuery.Where(a => a.UserId != null && a.UserId.Value == userID).Select(a => a);
                }

                var acclist = acclistQuery.ToList();

                if (acclist.Count == 0)
                {
                    outmsg = "Cannot raise an Amendment";
                    return false;
                }

                if (acclist.Count > 0)
                {
                    var isdeleted = acclist.All(lq => lq.IsDeleted == true);
                    if (isdeleted)
                    {
                        outmsg = "Cannot raise an Amendment for deleted Account";
                        return false;
                    }
                }

                var userAccountIsAvaliable = acclist.Where(lq => lq.IsLocked == false).Select(lq => lq);
                return userAccountIsAvaliable.Count() > 0 ? false : true;

            }
            catch
            {
                throw;
            }

        }
        public PagedResults<PresentationADSDormantAccount> GetByEndpointSystemIdAndDormantDaysPaging(long endPointSystemId, int dormantDays, int? pageSize, int? pageNumber)
        {
            IList<PresentationADSDormantAccount> list = GetByEndpointIdAndDormantDays(endPointSystemId, dormantDays);

            var aa = list.Skip(pageNumber.Value * pageSize.Value).Take(pageSize.Value).ToList();

            PagedResults<PresentationADSDormantAccount> paged = new PagedResults<PresentationADSDormantAccount>()
            {
                Results = aa,
                TotalNumberOfRecords = list.Count,
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value
            };

            return paged;

        }
        public bool IsAllowedExternalEmailAccess(long userId, string mail)
        {
            var context = CurrentContext.ConfigurationMastersIM.FirstOrDefault(i => i.ConfigKey == ConfigConstants.O365_ALLOW_EXTERNAL_EMAIL);
            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    join enp in CurrentContext.EndPoint on accountRecon.EndpointSystemId equals enp.Id
                    where enp.ApplicationType == ApplicationType.ADSingaporePowerLocal && accountRecon.UserId == userId && adUser.mail == mail
                    && accountRecon.AccountType == AccountType.User
                    && adUser.extensionAttribute4 != context.ConfigValue
                    && accountRecon.IsDeleted == false
                    select adUser.extensionAttribute4).Any();
        }

        public ADSUserInfo GetAccountDisabledByEndpoint(long diuserId, long endPointId)
        {

            return (from adUser in CurrentContext.ADSUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diuserId && accountRecon.EndpointSystemId == endPointId
                    && (accountRecon.AccountType == AccountType.User) && accountRecon.IsDeleted == false
                    select adUser).FirstOrDefault();

        }

        public List<PresentationADUserDetails> GetADUsersForLicenseAssignment(long endpointSystemId)
        {
            return (from adsUsr in CurrentContext.ADSUserInfos
                    where adsUsr.NeedO365License && adsUsr.EndPointSystemId == endpointSystemId
                    select new PresentationADUserDetails
                    {
                        Id = adsUsr.Id,
                        EndpointSystemId = adsUsr.EndPointSystemId,
                        AccountName = adsUsr.sAMAccountName,
                        WogUserId = adsUsr.WogUserId,
                        NeedO365License = adsUsr.NeedO365License,
                        physicalDeliveryOfficeName = adsUsr.physicalDeliveryOfficeName
                    }
                    ).ToList();
        }
    }
}
