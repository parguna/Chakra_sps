
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
    public class O365UserInfoRepository : RepositoryBase<Office365UserInfo>
    {

        public O365UserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public Office365UserInfo GetPrimaryByDIUserId(long dIUserId, long EndpointSystemId)
        {

            return (from o365User in CurrentContext.Office365UserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on o365User.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == EndpointSystemId
                 && accountRecon.AccountType == AccountType.User && accountRecon.IsDeleted == false
                    select o365User).FirstOrDefault();

        }

        public Dictionary<string, string> GetO365Users(string searchString, long EndpointSystemId)
        {

            var collection = (from o365User in CurrentContext.Office365UserInfo
                              join accountRecon in CurrentContext.AccountReconcilations on o365User.Id equals accountRecon.EndpointUserId
                              where accountRecon.EndpointSystemId == EndpointSystemId && accountRecon.AccountType == AccountType.User
                              && !accountRecon.IsLocked && o365User.IsValid.HasValue && o365User.IsValid.Value && accountRecon.IsDeleted==false
                              select o365User);

            if (!string.IsNullOrEmpty(searchString))
            {
                collection = collection.Where(item => item.DisplayName.Contains(searchString));
            }
            var results = collection.ToList().Where(i => string.IsNullOrWhiteSpace(i.UserPrincipalName) ? false : true);
            var data = new Dictionary<string, string>();
            foreach (var item in results)
            {
                if(!data.Any(i=> i.Key == item.UserPrincipalName))
                        data.Add(item.UserPrincipalName, item.DisplayName + " [" + item.UserPrincipalName + "]");
            }
            return data;
        }

        public Office365UserInfo GetO365UserByUserId(long userId, AccountType accountType, long endpointSytemId)
        {
            return (from o365User in CurrentContext.Office365UserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on o365User.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointUserId == userId && accountRecon.EndpointSystemId == endpointSytemId
                 && accountRecon.AccountType == accountType && !accountRecon.IsLocked
                    select o365User).FirstOrDefault();

        }

        public Dictionary<long, string> GetByEndpointSystemId(long EndpointSystemId)
        {
            return CurrentContext.Office365UserInfo.Where(ep => ep.EndPointSystemId == EndpointSystemId).ToDictionary(item => item.Id, item => item.DistinguishedName);

        }

        public void InsertOffice365UserGroupMapping(long EndpointSystemId, long userId, long groupId)
        {
            string cmd = string.Format(@"IF NOT EXISTS(SELECT 1 FROM [DBO].[Office365UserInfoOffice365GroupInfo] WHERE [Office365UserInfo_Id]= {0} AND [Office365GroupInfo_Id]= {1} AND [EndpointSystemId]= {2})
                BEGIN
                    INSERT INTO [dbo].[Office365UserInfoOffice365GroupInfo] ([Office365UserInfo_Id],[Office365GroupInfo_Id],[EndpointSystemId]) VALUES({0}, {1},{2})
                END", userId, groupId, EndpointSystemId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteGroup(long userid, long groupid, long EndpointSystemId)
        {
            string cmd = "DELETE FROM [Office365UserInfoOffice365GroupInfo] WHERE EndpointSystemId = " + EndpointSystemId + " AND  Office365UserInfo_Id = " + userid + " AND  Office365GroupInfo_Id = " + groupid;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteUserGroup(long userid, long EndpointSystemId)
        {
            string cmd = "DELETE FROM [Office365UserInfoOffice365GroupInfo] WHERE EndpointSystemId = " + EndpointSystemId + " AND  Office365UserInfo_Id = " + userid;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteGroupUser(long groupId, long EndpointSystemId)
        {
            string cmd = "DELETE FROM [Office365UserInfoOffice365GroupInfo] WHERE EndpointSystemId = " + EndpointSystemId + " AND  Office365GroupInfo_Id = " + groupId;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void O365LastPwdChangeDateTime(string datetime)
        {

            string cmd = string.Format("UPDATE [dbo].[DI_M_IMConfigurationMaster] SET ConfigValue = '{0}' WHERE [ConfigKey] = 'LastPwdChangeTime'", datetime);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public List<Office365UserInfo> GetAllActiveAccounts(long endpointId)
        {
            return (from o365User in CurrentContext.Office365UserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on o365User.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointId &&
                  accountRecon.AccountType == AccountType.User && !accountRecon.IsLocked && !accountRecon.IsDeleted
                    select o365User).ToList();
        }
    }
}
