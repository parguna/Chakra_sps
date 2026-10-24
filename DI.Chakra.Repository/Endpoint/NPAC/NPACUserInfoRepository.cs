using DI.Chakra.DBClient;
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
    public class NPACUserInfoRepository : RepositoryBase<NPACUserInfo>
    {
        public NPACUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public void InsertNPACUserLevelMapping(long endPointSystemId, long userId, long levelId)
        {
            string cmd = string.Format("DELETE FROM [NPACUserInfoNPACLevelInfo] WHERE  NPACUserInfo_Id = " + userId + ";INSERT INTO [dbo].[NPACUserInfoNPACLevelInfo] ([NPACUserInfo_Id],[NPACLevelInfo_Id],[EndpointSystemId]) VALUES({0}, {1},{2})", userId, levelId, endPointSystemId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteRole(long userid, long levelid, long endpointsystemid)
        {
            string cmd = "DELETE FROM [NPACUserInfoNPACLevelInfo] WHERE EndpointSystemId = " + endpointsystemid + " AND  NPACUserInfo_Id = " + userid + " AND  NPACLevelInfo_Id = " + levelid;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId)
        {
            return CurrentContext.NPACUserInfo.Where(ep => ep.EndPointSystemId == endPointSystemId).ToDictionary(item => item.Id, item => item.UserName);
        }
        public void ExecuteQuery(string query)
        {
            CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public List<NPACUserInfo> GetActiveAccounts(long diuserId, long endpointSystemId, AccountType accountType)
        {

            return (from npacUser in CurrentContext.NPACUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on npacUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId &&
                    accountRecon.AccountType == accountType && (!accountRecon.IsLocked && !accountRecon.IsDeleted)
                    select npacUser).ToList();

        }

        public NPACUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            return (from npacUser in CurrentContext.NPACUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on npacUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == AccountType.User && (!accountRecon.IsLocked && !accountRecon.IsDeleted)
                    select npacUser).FirstOrDefault();

        }

        public NPACUserInfo GetLockedPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            return (from npacUser in CurrentContext.NPACUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on npacUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == AccountType.User && (accountRecon.IsLocked || accountRecon.IsDeleted)
                    select npacUser).FirstOrDefault();

        }

        public NPACUserInfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
        {

            return (from npacUser in CurrentContext.NPACUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on npacUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId &&
                    accountRecon.AccountType == accountType && (accountRecon.IsLocked || accountRecon.IsDeleted) &&
                    accountRecon.AccountName.Equals(accountName, StringComparison.CurrentCultureIgnoreCase)
                    select npacUser).FirstOrDefault();
        }
        /// <summary>
        /// Id is used for AccountRecon Id.
        /// </summary>
        /// <param name="endpointSystemId"></param>
        /// <returns></returns>
        public NPACUserInfo GetNPACUserForAttestation(long endpointSystemId, long accountReconId)
        {
            return (from npacUser in CurrentContext.NPACUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on npacUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.Id == accountReconId && accountRecon.EndpointSystemId == endpointSystemId && accountRecon.IsDeleted == false 
                    select new { Id = accountRecon.Id, UserIdentity = accountRecon.AccountName, Section = npacUser.Section, SectionName = npacUser.SectionName, Zone= npacUser.Zone }).AsEnumerable().Select(i=> 
                        new NPACUserInfo() {
                            Id = i.Id,
                            UserIdentity = i.UserIdentity,
                            Section = i.Section,
                            SectionName = i.SectionName,
                            Zone = i.Zone
                        }
                    ).FirstOrDefault();
                    
        }
    }
}
