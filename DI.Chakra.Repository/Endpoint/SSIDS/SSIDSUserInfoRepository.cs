using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class SSIDSUserInfoRepository : RepositoryBase<SSIDSUserInfo>
    {
        public SSIDSUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public void InsertUserRoleMapping(long endPointSystemId, long userId, long roleId)
        {
            string cmd = string.Format("DELETE FROM [dbo].[SSIDSUserInfoSSIDSRolelnfo] WHERE SSIDSUserInfo_Id = {0} AND EndpointSystemId = {2}; INSERT INTO [dbo].[SSIDSUserInfoSSIDSRolelnfo] ([SSIDSUserInfo_Id],[SSIDSRolelnfo_Id],[EndpointSystemId]) VALUES({0}, {1},{2})", userId, roleId, endPointSystemId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteRole(long userid, long roleId, long endpointsystemid)
        {
            string cmd = "DELETE FROM [SSIDSUserInfoSSIDSRolelnfo] WHERE EndpointSystemId = " + endpointsystemid + " AND  SSIDSUserInfo_Id = " + userid + " AND  SSIDSRolelnfo_Id = " + roleId;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId)
        {
            return CurrentContext.SSIDSUserInfo.Where(ep => ep.EndPointSystemId == endPointSystemId).ToDictionary(item => item.Id, item => item.UserName);
        }
        public void ExecuteQuery(string query)
        {
            CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public List<SSIDSUserInfo> GetActiveAccounts(long diuserId, long endpointSystemId, AccountType accountType)
        {

            return (from npacUser in CurrentContext.SSIDSUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on npacUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId &&
                    accountRecon.AccountType == accountType && !accountRecon.IsLocked
                    select npacUser).ToList();

        }

        public SSIDSUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            return (from npacUser in CurrentContext.SSIDSUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on npacUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == AccountType.User && (!accountRecon.IsLocked && !accountRecon.IsDeleted)
                    select npacUser).FirstOrDefault();

        }

        public SSIDSUserInfo GetLockedPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            return (from npacUser in CurrentContext.SSIDSUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on npacUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == AccountType.User && (accountRecon.IsLocked || accountRecon.IsDeleted)
                    select npacUser).FirstOrDefault();

        }

        public SSIDSUserInfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {

            return (from npacUser in CurrentContext.SSIDSUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on npacUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId &&
                    accountRecon.AccountType == accountType && accountRecon.IsLocked &&
                    accountRecon.AccountName.Equals(accountName, StringComparison.CurrentCultureIgnoreCase)
                    select npacUser).FirstOrDefault();
        }

    }
}
