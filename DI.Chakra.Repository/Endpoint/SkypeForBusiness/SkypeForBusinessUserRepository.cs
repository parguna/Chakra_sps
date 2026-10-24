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
    public class SkypeForBusinessUserRepository : RepositoryBase<SkypeForBusinessUserInfo>
    {
        public SkypeForBusinessUserRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        /// <summary>
        /// Used to delete from ResponseGroup and AccountReconcilation
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="endpointsystemid"></param>
        public void DeleteGroupUser(long userId, long endpointsystemid)
        {
            string cmd = $@"DELETE FROM [SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] WHERE EndpointSystemId = {endpointsystemid} AND  SkypeForBusinessUserInfo_Id = {userId};
                            Update DI_M_AccountReconcilation SET IsDeleted=1,DeletedDate=GETDATE(),IsLocked=1 WHERE  EndpointSystemId = {endpointsystemid} AND EndpointUserId={userId};   
                            " ;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        
        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId)
        {
            return CurrentContext.SkypeForBusinessUserInfo.Where(ep => ep.EndPointSystemId == endPointSystemId).ToDictionary(item => item.Id, item => item.SamAccountName);
        }
        public List<int?> GetAssignedNumbers(long endPointSystemId)
        {
            return CurrentContext.SkypeForBusinessUserInfo.Where(ep => ep.EndPointSystemId == endPointSystemId && ep.LineURI != null && ep.LineURIExtension != null && ep.UserType != EntitlementType.DCC ).Select(i=>i.LineURIExtension).ToList();
        }
        public Dictionary<long, string> GetUserSipAddressByEndPointSystemId(long endPointSystemId)
        {
            return CurrentContext.SkypeForBusinessUserInfo.Where(ep => ep.EndPointSystemId == endPointSystemId).ToDictionary(item => item.Id, item => item.UserPrincipalName);
        }
        public void InsertOrUpdateSkypeForBusinessUserGroup(long endPointSystemId, long userId, long groupId, int ismasked)
        {
            string cmd = string.Format(@"IF NOT EXISTS(SELECT  1 from [dbo].[SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] WHERE EndpointSystemId ={2} AND [SkypeForBusinessResponseGroupInfo_Id]={1} AND [SkypeForBusinessUserInfo_Id]={0}) BEGIN INSERT INTO [dbo].[SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] ([SkypeForBusinessUserInfo_Id],[SkypeForBusinessResponseGroupInfo_Id],[EndpointSystemId],[CreatedDate],IsMasked) VALUES({0}, {1},{2},GETDATE(),{3}) 
                END ELSE BEGIN   UPDATE  [dbo].[SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] SET LastModifiedDate=GETDATE(),ismasked={3} WHERE   EndpointSystemId ={2} AND [SkypeForBusinessResponseGroupInfo_Id]={1} AND [SkypeForBusinessUserInfo_Id]={0}    END", userId, groupId, endPointSystemId, ismasked);
            //From Discovery
            if (ismasked == -1)
            {
                cmd = string.Format(@"IF NOT EXISTS(SELECT  1 from [dbo].[SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] WHERE EndpointSystemId ={2} AND [SkypeForBusinessResponseGroupInfo_Id]={1} AND [SkypeForBusinessUserInfo_Id]={0}) BEGIN INSERT INTO [dbo].[SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] ([SkypeForBusinessUserInfo_Id],[SkypeForBusinessResponseGroupInfo_Id],[EndpointSystemId],[CreatedDate]) VALUES({0}, {1},{2},GETDATE()) 
                END ELSE BEGIN   UPDATE  [dbo].[SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] SET LastModifiedDate=GETDATE() WHERE   EndpointSystemId ={2} AND [SkypeForBusinessResponseGroupInfo_Id]={1} AND [SkypeForBusinessUserInfo_Id]={0}    END", userId, groupId, endPointSystemId, ismasked);
            }
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public SkypeForBusinessUserInfo GetPrimaryByDIUserId(long dIUserId, long EndpointSystemId)
        {

            return (from sfbUser in CurrentContext.SkypeForBusinessUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sfbUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == EndpointSystemId
                 && accountRecon.AccountType == AccountType.User && !accountRecon.IsLocked && sfbUser.IsValid
                    select sfbUser).FirstOrDefault();
        }
        public SkypeForBusinessUserInfo GetByUserIdIncludeGroups(long userId, long endPointId)
        {
            return (from user in CurrentContext.SkypeForBusinessUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on user.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointUserId == userId && accountRecon.EndpointSystemId == endPointId
                    select user).FirstOrDefault();
        }
        public void DeleteGroupByUserIdGroupId(long userId,long groupId, long endpointsystemid)
        {
            string cmd = $"DELETE FROM [SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] WHERE EndpointSystemId = {endpointsystemid} AND  SkypeForBusinessUserInfo_Id = {userId} AND [SkypeForBusinessResponseGroupInfo_Id]={groupId}";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public void DeleteAllGroupByUserId(long userId, long endpointsystemid)
        {
            string cmd = $"DELETE FROM [SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] WHERE EndpointSystemId = {endpointsystemid} AND  SkypeForBusinessUserInfo_Id = {userId} ";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public IList<SkypeForBusinessUserInfo> GetBySIPAddress(List<string> sipAddresses)
        {
            return (
                from users in CurrentContext.SkypeForBusinessUserInfo.Include("SkypeForBusinessResponseGroup")
                where sipAddresses.Contains(users.SipAddress)
                select users
                ).ToList();
        }

        public void UpdateUserResponseGroup(long endpointsystemId, long userId, long groupId, bool isMasked)
        {
            var masked = isMasked ? 1 : 0;
            string cmd = $"UPDATE [SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] Set IsMasked = {masked},[LastModifiedDate]=GETDATE() WHERE EndpointSystemId = {endpointsystemId} AND  SkypeForBusinessUserInfo_Id = {userId} AND [SkypeForBusinessResponseGroupInfo_Id]={groupId}";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public bool IsDCCExists(long endpointId, long number,AdHocActionType actionType, long accessRequestId)
        {
            var userExtension = CurrentContext.SkypeForBusinessUserInfo.Where(i => i.EndPointSystemId == endpointId && i.LineURIExtension !=null && i.IsValid && i.UserType == EntitlementType.DCC).Select(i=>i.LineURIExtension.Value).ToList();
            var exisitingRequest = $@"
                                    SELECT CAST(Extension as int) FROM (
                                    SELECT 
                                    xmlNode.dataObject.value('EntitlementType[1]','VARCHAR(max)') AS EntitlementType,
                                    xmlNode.dataObject.value('Extension[1]','VARCHAR(max)') AS Extension
                                    FROM 
                                    (   SELECT AD.Id,Cast(ISNULL(EntitlementDetail,'') as xml)as XMLData FROM
                                        DI_M_AdHocAccessRequest AD inner join DI_M_AccessRequest A on A.AdHocAccessRequestId = Ad.Id
							            WHERE  (ISNULL(EntitlementDetail,''))<>'' AND AD.EndpointSystemId={endpointId} AND  A.EndpointSystemId={endpointId} AND AD.AdhocActionType={((int)actionType)} AND A.ProgressStatus NOT IN(2,5,8) AND A.Id <>{accessRequestId}
                                    )AS AD CROSS APPLY XMLData.nodes('/AdhocSkypeForBusinessRequest') xmlNode(dataObject)
                                    ) AS A WHERE EntitlementType='DCC'  AND Extension IS NOT NULL  AND ISNUMERIC(Extension)=1";
            var exisitingNumber= CurrentContext.Database.SqlQuery<int>(exisitingRequest).ToList();
            userExtension.AddRange(exisitingNumber);
            return userExtension.Exists(i => i == number);
        }

        public List<PresentationSkypeUserInfo> GetValidActiveUser(long endpointId)
        {
            return (from sfbUsr in CurrentContext.SkypeForBusinessUserInfo.Where(i => i.IsValid && string.IsNullOrEmpty(i.LineURI) == false)
                        join account in CurrentContext.AccountReconcilations.Where(i => i.AccountType == AccountType.User && i.EndpointSystemId == endpointId && i.IsDeleted == false && i.IsLocked == false) on sfbUsr.Id equals account.EndpointUserId
                        join usr in CurrentContext.User.Where(i => i.IsActive) on account.UserId equals usr.Id
                        where  account.EndpointSystemId == endpointId && sfbUsr.EndPointSystemId == endpointId && account.UserId != null && sfbUsr.LineURI != null && sfbUsr.LineURI.Length > 6
                    select new PresentationSkypeUserInfo
                        {
                            LineURI = sfbUsr.LineURI,
                            AccountName = sfbUsr.SamAccountName,
                            Id = sfbUsr.Id,
                            UserId = usr.Id,
                            UserTelephoneNumber = usr.TelephoneNumber,
                            EmployeeType = usr.EmployeeType
                        }).ToList();
        }

        public SkypeForBusinessUserInfo GetPrimaryByDIUserIdForUpdateTelephoneNumber(long dIUserId, long EndpointSystemId)
        {

            return (from sfbUser in CurrentContext.SkypeForBusinessUserInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sfbUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == EndpointSystemId
                 && accountRecon.AccountType == AccountType.User && !accountRecon.IsLocked && sfbUser.LineURI != null
                    select sfbUser).FirstOrDefault();
        }
    }
}
