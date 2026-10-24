using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class IACMUserInfoRepository : RepositoryBase<IACMUserInfo>
    {
        public IACMUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public void InsertIACMUserGroupMapping(long userId, long groupId)
        {
            string cmd = string.Format("INSERT INTO [dbo].[IACMUserInfoIACMGroupInfo] ([IACMUserInfo_Id],[IACMGroupInfo_Id]) VALUES({0}, {1})", userId, groupId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }        

        public void DeleteIACMUserGroupMapping(long userid, long groupid)
        {
            string cmd = "DELETE FROM [IACMUserInfoIACMGroupInfo] WHERE IACMUserInfo_Id = " + userid + " AND  IACMGroupInfo_Id = " + groupid;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void InsertIACMUserRoleMapping(long userId, long roleId)
        {
            string cmd = string.Format("INSERT INTO [dbo].[IACMRoleInfoIACMUserInfo] ([IACMUserInfo_Id],[IACMRoleInfo_Id]) VALUES({0}, {1})", userId, roleId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteIACMUserRoleMapping(long userid, long roleId)
        {
            string cmd = "DELETE FROM [IACMRoleInfoIACMUserInfo] WHERE IACMUserInfo_Id = " + userid + " AND  IACMRoleInfo_Id = " + roleId;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void ExecuteQuery(string query)
        {
            CurrentContext.Database.ExecuteSqlCommand(query);
        }
        public List<IACMUserInfo> GetActiveAccounts(long dIUserId, long endPointSystemId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.IACMUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == accountType && (!accountRecon.IsLocked)
                    select sapUser).ToList();
        }
        public IACMUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {

            return (from IACMUser in CurrentContext.IACMUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on IACMUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && (accountRecon.AccountType == AccountType.User || accountRecon.AccountType == AccountType.Secondary) && accountRecon.IsDeleted == false
                    select IACMUser).FirstOrDefault();

        }


    }
}
