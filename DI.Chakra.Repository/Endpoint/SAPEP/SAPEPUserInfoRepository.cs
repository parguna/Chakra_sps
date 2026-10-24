using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Repository
{
    public class SAPEPUserInfoRepository : RepositoryBase<SAPEPUserInfo>
    {
        public SAPEPUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        //public dynamic GetAllSAPEPUser(long endPtId)
        //{
        //    return (from usr in CurrentContext.User
        //            join endUser in CurrentContext.SAPEPUserInfos on usr.Id equals endUser.DIUserId
        //            where endUser.EndPointSystemId == endPtId && endUser.DIUserId != null && endUser.DIUserId != -1
        //            select new
        //            {
        //                UserId = usr.Id,
        //                usr.FirstName,
        //                usr.LastName,
        //                UserLogOnName = endUser.UserName,
        //                usr.EmailId,
        //                usr.DisplayName
        //            }).ToList();

        //}

        public IList<PresentationSAPEPUserToGroups> GetSAPEPUserToGroupsReportData(long EndpointSystemId)
        {
            return (from sapepUser in CurrentContext.SAPEPUserInfos
                    from sapepGrp in sapepUser.SAPEPGroups
                    where (!string.IsNullOrEmpty(sapepGrp.GroupName) && sapepUser.EndPointSystemId == EndpointSystemId)
                    select new PresentationSAPEPUserToGroups
                    {
                        UserId = sapepUser.Id,
                        UserName = sapepUser.UserName,
                        DisplayName = sapepUser.DisplayName,
                        InternetAddress = sapepUser.Email,
                        Department = sapepUser.Department,
                        GroupId = sapepGrp.Id,
                        GroupName = sapepGrp.GroupName,
                        GroupDescription = sapepGrp.GroupDescription
                    }
                   ).ToList();
        }

        public IList<PresentationSAPEPUserToRoles> GetSAPEPUserToRolesReportData(long EndpointSystemId)
        {
            return (from sapepUser in CurrentContext.SAPEPUserInfos
                    from sapepRle in sapepUser.SAPEPRoles
                    where (!string.IsNullOrEmpty(sapepRle.RoleName) && sapepUser.EndPointSystemId == EndpointSystemId)
                    select new PresentationSAPEPUserToRoles
                    {
                        UserId = sapepUser.Id,
                        UserName = sapepUser.UserName,
                        DisplayName = sapepUser.DisplayName,
                        InternetAddress = sapepUser.Email,
                        Department = sapepUser.Department,

                        RoleId = sapepRle.Id,
                        RoleName = sapepRle.RoleName,
                        RoleDescription = sapepRle.RoleDescription
                    }
                         ).ToList();
        }
        public IList<PresentationSAPEPUserInfo> GetAllSAPEPUsersReportData(long EndpointSystemId)
        {
            return (from users in CurrentContext.SAPEPUserInfos
                    where users.EndPointSystemId == EndpointSystemId
                    select new PresentationSAPEPUserInfo
                    {
                        UserId = users.Id,
                        UserName = users.UserName,
                        FirstName = users.FirstName,
                        LastName = users.LastName,
                        Salutation = users.Salutation,
                        DisplayName = users.DisplayName,
                        StartDate = users.StartDate,
                        Title = users.Title,
                        Company = users.Company,
                        EndDate = users.EndDate,
                        Email = users.Email,
                        Department = users.Department,
                        DataSource = users.UniqueId.Contains("USER.R3_DATASOURCE") ? "ABAP" : "UME Database",
                    }).ToList();
        }

        public void SaveUserRolesMap(Dictionary<long, string> dicUsers, Dictionary<long, string> dicGroups, Dictionary<string, System.Collections.ArrayList> dicGroupUsers, long endpointSystemid)
        {
            IList<SAPEPUserGroups> listUG = new List<SAPEPUserGroups>();

            SAPEPUserGroups userGroup = null;
            SAPEPRoleInfo sapepRoleInfo = null;

           
            foreach (var user in dicGroupUsers)
            {
                string UserName = user.ToString();

                foreach (var group in user.Value)
                {
                    userGroup = new SAPEPUserGroups();
                    string GroupName = group.ToString();
                    sapepRoleInfo = CurrentContext.SAPEPRoleInfos.Where(ep => ep.EndPointSystemId == endpointSystemid).FirstOrDefault(G => G.UniqueRoleName == GroupName);
                    if (sapepRoleInfo != null)
                    {
                        userGroup.SAPEPGroupInfo_Id = sapepRoleInfo.Id;
                        userGroup.SAPEPUserInfo_Id = Convert.ToInt32(user.Key);
                        userGroup.EndpointSystemId = (int)endpointSystemid;
                        userGroup.Version = Convert.ToInt16(VersionNo.Available);
                        SAPEPUserGroups existingUserGroup = listUG.FirstOrDefault(usrgrp => usrgrp.SAPEPGroupInfo_Id == userGroup.SAPEPGroupInfo_Id
                                                        && usrgrp.SAPEPUserInfo_Id == userGroup.SAPEPUserInfo_Id);

                        if (existingUserGroup == null)
                        {
                            InsertSAPEPUserRoleMapping(userGroup.EndpointSystemId, userGroup.SAPEPUserInfo_Id, userGroup.SAPEPGroupInfo_Id);
                            listUG.Add(userGroup);
                        }
                    }
                }
            }
        }

        public void SaveUserGroupMap(Dictionary<long, string> dicUsers, Dictionary<long, string> dicGroups, Dictionary<string, System.Collections.ArrayList> dicGroupUsers, long endpointSystemid)
        {
            IList<SAPEPUserGroups> listUG = new List<SAPEPUserGroups>();

            SAPEPUserGroups userGroup = null;
            SAPEPGroupInfo sapepGroup = null;

            foreach (var user in dicGroupUsers)
            {
                string UserName = user.ToString();

                foreach (var group in user.Value)
                {
                    userGroup = new SAPEPUserGroups();
                    string GroupName = group.ToString();
                    sapepGroup = CurrentContext.SAPEPGroupInfos.Where(ep => ep.EndPointSystemId == endpointSystemid).FirstOrDefault(G => G.GroupName == GroupName);
                    if (sapepGroup != null)
                    {
                        userGroup.SAPEPGroupInfo_Id = sapepGroup.Id;
                        userGroup.SAPEPUserInfo_Id = Convert.ToInt32(user.Key);
                        userGroup.EndpointSystemId = (int)endpointSystemid;
                        userGroup.Version = Convert.ToInt16(VersionNo.Available);
                        SAPEPUserGroups existingUserGroup = listUG.FirstOrDefault(usrgrp => usrgrp.SAPEPGroupInfo_Id == userGroup.SAPEPGroupInfo_Id
                                                        && usrgrp.SAPEPUserInfo_Id == userGroup.SAPEPUserInfo_Id);

                        if (existingUserGroup == null)
                        {
                            InsertSAPEPUserGroupMapping(userGroup.EndpointSystemId, userGroup.SAPEPUserInfo_Id, userGroup.SAPEPGroupInfo_Id);
                            listUG.Add(userGroup);
                        }
                    }
                }
            }
        }

        public List<SAPEPUserInfo> GetActiveAccountByDIUserId(long diUserId)
        {
            return (from sapepUser in CurrentContext.SAPEPUserInfos
                    from sapepgrp in sapepUser.SAPEPRoles
                    join accountRecon in CurrentContext.AccountReconcilations on sapepUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId && accountRecon.IsLocked == false && sapepUser.EndPointSystemId == accountRecon.EndpointSystemId
                    select sapepUser).ToList();
        }

        public List<SAPEPUserInfo> GetByDIUserId(long diUserId)
        {
            return (from sapepUser in CurrentContext.SAPEPUserInfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on sapepUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId && accountRecon.IsLocked == false
                    select sapepUser).OrderBy(sapepUsrInfo => sapepUsrInfo.CreatedDate).ToList();
        }

        public SAPEPUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {

            return (from sapUser in CurrentContext.SAPEPUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == AccountType.User && !accountRecon.IsLocked && !accountRecon.IsDeleted
                    select sapUser).FirstOrDefault();

        }

        public List<SAPEPUserInfo> GetPrimaryAccountsByDIUserId(long dIUserId, long endPointSystemId)
        {

            return (from sapUser in CurrentContext.SAPEPUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == AccountType.User && !accountRecon.IsLocked
                    select sapUser).ToList();

        }

        public List<SAPEPUserInfo> GetActiveAccounts(long dIUserId, long endPointSystemId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.SAPEPUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == accountType && (!accountRecon.IsLocked)
                    select sapUser).ToList();
        }

        public SAPEPUserInfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.SAPEPUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId &&
                    accountRecon.AccountType == accountType && (accountRecon.IsLocked || accountRecon.IsDeleted) &&
                    accountRecon.AccountName.Equals(accountName, StringComparison.CurrentCultureIgnoreCase)
                    select sapUser).FirstOrDefault();
        }


        public void DeleteSAPEPUserGroupMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPEPUserInfoSAPEPGroupInfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }
        public void DeleteSAPEPUserGroupMap(long endpointSystemId, long userId, long groupId)
        {
            string query = string.Format("Delete from SAPEPUserInfoSAPEPGroupInfo where EndpointSystemId={0} and SAPEPUserInfo_Id={1} and SAPEPGroupInfo_Id={2}", endpointSystemId, userId, groupId);
            ExecuteQuery(query);
        }

        public void DeleteSAPEPUserRoleMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPEPRoleInfoSAPEPUserInfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }
        public void DeleteSAPEPUserRoleMap(long endpointSystemId, long userId, long roleId)
        {
            string query = string.Format("Delete from SAPEPRoleInfoSAPEPUserInfo where EndpointSystemId={0} and SAPEPUserInfo_Id={1} and SAPEPRoleInfo_Id={2}", endpointSystemId, userId, roleId);
            ExecuteQuery(query);
        }

        public void InsertSAPEPUserRoleMapping(long endPointSystemId, long userId, long roleId)
        {
            string cmd = string.Format(@"IF NOT EXISTS (SELECT 1 FROM [dbo].[SAPEPRoleInfoSAPEPUserInfo] WHERE [SAPEPUserInfo_Id]={0} AND [SAPEPRoleInfo_Id]={1} AND [EndpointSystemId]={2}) BEGIN
                                         INSERT INTO [dbo].[SAPEPRoleInfoSAPEPUserInfo] ([SAPEPUserInfo_Id],[SAPEPRoleInfo_Id],[EndpointSystemId]) VALUES({0},{1},{2})  END", userId, roleId, endPointSystemId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void InsertSAPEPUserGroupMapping(long endPointSystemId, long userId, long groupId)
        {
            string cmd = string.Format(@"IF NOT EXISTS (SELECT 1 FROM [dbo].[SAPEPUserInfoSAPEPGroupInfo] WHERE [SAPEPUserInfo_Id]={0} AND [SAPEPGroupInfo_Id]={1} AND [EndpointSystemId]={2}) BEGIN
                                         INSERT INTO [dbo].[SAPEPUserInfoSAPEPGroupInfo] ([SAPEPUserInfo_Id],[SAPEPGroupInfo_Id],[EndpointSystemId]) VALUES({0},{1},{2}) END", userId, groupId, endPointSystemId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteSAPEPUserGroupMapByGroupId(long endpointSystemId, long groupId)
        {
            string query = string.Format("Delete from SAPEPUserInfoSAPEPGroupInfo where EndpointSystemId={0} and SAPEPGroupInfo_Id={1}", endpointSystemId, groupId);
            ExecuteQuery(query);
        }

        public void DeleteSAPEPUserRoleMapByRoleId(long endpointSystemId, long roleId)
        {
            string query = string.Format("Delete from SAPEPRoleInfoSAPEPUserInfo where EndpointSystemId={0} and SAPEPRoleInfo_Id ={1}", endpointSystemId, roleId);
            ExecuteQuery(query);
        }
    }
}
