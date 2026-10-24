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
    public class SMSUserInfoRepository : RepositoryBase<SMSUserInfo>
    {
        public SMSUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public void InsertSMSUserLevelMapping(long endPointSystemId, long userId, long levelId)
        {
            string cmd = string.Format("IF NOT EXISTS(SELECT 1 FROM [dbo].[SMSUserInfoSMSAccessLevel] WHERE SMSUserInfo_Id={0} AND [SMSAccessLevel_Id]={1} AND [EndpointSystemId]={2}) BEGIN INSERT INTO [dbo].[SMSUserInfoSMSAccessLevel] ([SMSUserInfo_Id],[SMSAccessLevel_Id],[EndpointSystemId]) VALUES({0}, {1},{2}) END", userId, levelId, endPointSystemId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteSMSUserLevelMapping(long endPointSystemId, long userId, long levelId)
        {
            string cmd = string.Format("DELETE FROM [dbo].[SMSUserInfoSMSAccessLevel] WHERE SMSUserInfo_Id ={0} AND SMSAccessLevel_Id = {1}", userId, levelId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteUserAccessDataByEndpoint(long endpointsystemid)
        {
            string cmd = string.Format("DELETE FROM SMSUserInfoSMSAccessLevel WHERE EndpointSystemId = {0}", endpointsystemid);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteUserAccessDataByAccessLevelId(long endpointsystemid, long accessLevelId)
        {
            string cmd = string.Format("DELETE FROM SMSUserInfoSMSAccessLevel WHERE EndpointSystemId = {0} AND SMSAccessLevel_Id = {1}", endpointsystemid, accessLevelId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId)
        {
            return CurrentContext.SMSUserInfo.Where(ep => ep.EndPointSystemId == endPointSystemId).ToDictionary(item => item.Id, item => item.MIDNAME);
        }

        public SMSUserInfo GetLockedSMSUserByDIUserId(long userId, long endpointSystemId)
        {

            return (from smsUser in CurrentContext.SMSUserInfo.Include("SMSBadgeInfos")
                    join accountRecon in CurrentContext.AccountReconcilations
                    on smsUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == userId && accountRecon.AccountType == AccountType.User
                    && (accountRecon.IsLocked == true || accountRecon.IsDeleted == true) && accountRecon.EndpointSystemId == endpointSystemId
                    select smsUser).FirstOrDefault();
        }
        public SMSUserInfo GetUnLockedSMSUserByDIUserId(long userId, long endpointSystemId)
        {

            return (from smsUser in CurrentContext.SMSUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations
                    on smsUser.Id equals accountRecon.EndpointUserId
                    where (accountRecon.UserId == userId && accountRecon.AccountType == AccountType.User
                    && (accountRecon.IsLocked == false || accountRecon.IsDeleted == false) && accountRecon.EndpointSystemId == endpointSystemId)
                    select smsUser).FirstOrDefault();
        }

        public void ExecuteQuery(string query)
        {
            CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public SMSUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            return (from smsUser in CurrentContext.SMSUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on smsUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == AccountType.User && !accountRecon.IsLocked && !accountRecon.IsDeleted
                    select smsUser).FirstOrDefault();

        }

        public SMSUserInfo GetByEndpointUserId(long EndpointUserId, long endpointSystemId)
        {

            var id =  (from smsUser in CurrentContext.SMSUserInfo 
                    join accountRecon in CurrentContext.AccountReconcilations
                    on smsUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId== endpointSystemId && 
                    accountRecon.EndpointUserId == EndpointUserId && 
                    accountRecon.AccountType == AccountType.User  &&
                    accountRecon.IsLocked == false
                    select smsUser.Id).FirstOrDefault();

            return this.GetById(id, true);
        }

        public IList<SMSUserInfo> GetSMSUserInfoForUpdateMobileNumber(long endpointId)
        {
            return (from usr in CurrentContext.SMSUserInfo
                    from emp in CurrentContext.User.Where(i=> i.IsActive)
                    where usr.EndPointSystemId == endpointId && usr.SSNO == emp.EmployeeId && (!(usr.SSNO == null || usr.SSNO.Trim() == string.Empty) && (usr.Extension == null || usr.Extension.Trim() == string.Empty))
                          select usr).ToList();
        }
    }
}
