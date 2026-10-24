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
    public class GEMSUserInfoRepository : RepositoryBase<GEMSUserInfo>
    {
        public GEMSUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public void InsertGEMSUserRoleMapping(long endPointSystemId, long userId, long roleId)
        {
            string cmd = string.Format("INSERT INTO [dbo].[GEMSUserInfoGEMSRoleInfo] ([GEMSUserInfo_Id],[GEMSRoleInfo_Id],[EndpointSystemId]) VALUES({0}, {1},{2})", userId, roleId, endPointSystemId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteRole(long userid, long groupid, long endpointsystemid)
        {
            string cmd = "DELETE FROM [GEMSUserInfoGEMSRoleInfo] WHERE EndpointSystemId = " + endpointsystemid + " AND  GEMSUserInfo_Id = " + userid + " AND  GEMSRoleInfo_Id = " + groupid;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId)
        {
            return CurrentContext.GEMSUserInfo.Where(ep => ep.EndPointSystemId == endPointSystemId).ToDictionary(item => item.Id, item => item.USER_NAME);
        }



        public GEMSUserInfo GetByDIUserIdIncludeGEMSRole(long userId, AccountType accounttype)
        {

            return (from gemsUser in CurrentContext.GEMSUserInfo
                    from gemsrole in gemsUser.GEMSRoles
                    join accountRecon in CurrentContext.AccountReconcilations on gemsUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == userId && accountRecon.AccountType == accounttype
                    select gemsUser).FirstOrDefault();
        }

        public void ExecuteQuery(string query)
        {
            CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public GEMSUserInfo GetLockedAccountByName(string accountName, long diuserId, long endpointSystemId, AccountType accountType)
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

            return (from adUser in CurrentContext.GEMSUserInfo
                    join accountRecon in accountReconQuery on adUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId  
                    select adUser).FirstOrDefault();
        }

        public int GetMaxUserId()
        {
            try
            {
                return CurrentContext.GEMSUserInfo.Max(c => c.UserId);
            }
            catch
            {
                throw;
            }
        }

        public List<GEMSUserInfo> GetActiveAccounts(long diuserId, long endpointSystemId, AccountType accountType)
        {

            return (from gemsUser in CurrentContext.GEMSUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on gemsUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId &&
                    accountRecon.AccountType == accountType && !accountRecon.IsLocked
                    select gemsUser).ToList();

        }

        public GEMSUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            return (from gemsUser in CurrentContext.GEMSUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on gemsUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == AccountType.User && !accountRecon.IsLocked && !accountRecon.IsDeleted
                    select gemsUser).FirstOrDefault();

        }

        public Dictionary<long, string> GetGEMSUsersByDIUserId(long userid, int endpointsystemid, AccountType accountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    
                    if (accountType == AccountType.User || accountType == AccountType.Secondary)
                    {  
                        return  (from gemsUser in CurrentContext.GEMSUserInfo
                         join accountRecon in CurrentContext.AccountReconcilations on gemsUser.Id equals accountRecon.EndpointUserId
                         where accountRecon.EndpointSystemId == endpointsystemid && accountRecon.UserId == userid &&
                         accountRecon.AccountType == accountType && !accountRecon.IsLocked
                         select new { Key = gemsUser.Id, Value = gemsUser.USER_NAME }).Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }
                    else if (accountType == AccountType.Service || accountType == AccountType.Shared)
                    {
                        string ouOfRequestee = CurrentContext.User.Where(lq => lq.Id == userid).Select(lq => lq.SectionCode).FirstOrDefault();
                        List<long> listofUser = CurrentContext.User.Where(lq => lq.SectionCode == ouOfRequestee).Select(lq => lq.Id).ToList(); 
                        return (from gemsUser in CurrentContext.GEMSUserInfo
                                join accountRecon in CurrentContext.AccountReconcilations on gemsUser.Id equals accountRecon.EndpointUserId
                                where accountRecon.EndpointSystemId == endpointsystemid && listofUser.Contains(accountRecon.UserId.Value) &&
                                accountRecon.AccountType == accountType && !accountRecon.IsLocked  
                                select new { Key = gemsUser.Id, Value = gemsUser.USER_NAME }).Distinct().ToDictionary(o => o.Key, o => o.Value);
                    }

                    return new Dictionary<long, string>();


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
