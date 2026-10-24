using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SkypeForBusinessResponseGroupRepository : RepositoryBase<SkypeForBusinessResponseGroupInfo>
    {
        public SkypeForBusinessResponseGroupRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public void DeleteGroupUser(long groupId, long endpointsystemid)
        {
            string cmd = "DELETE FROM [SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo] WHERE EndpointSystemId = " + endpointsystemid + " AND  SkypeForBusinessResponseGroupInfo_Id = " + groupId;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public void UpdateResponseGroupMasked(long endPointSystemId,string userSIPAddress, string responseGroupName)
        {
            string cmd = $@"UPDATE A 
                            SET A.IsMasked = 1,LastModifiedDate=GETDATE() FROM SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo A inner join DI_SFB_ResponseGroupInfo RG on A.SkypeForBusinessResponseGroupInfo_Id = RG.Id
                            INNER JOIN DI_SFB_UserInfo U on A.SkypeForBusinessUserInfo_Id = U.Id
                            WHERE U.SipAddress = '{userSIPAddress}' AND RG.Name ='{responseGroupName}' AND  A.EndpointSystemId={endPointSystemId} AND RG.EndPointSystemId={endPointSystemId} AND U.EndPointSystemId={endPointSystemId}";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public void UpdateResponseGroupMaskedById(long endPointSystemId, long userId, long groupId)
        {
            string cmd = $@"UPDATE SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo 
                            SET IsMasked = 1,LastModifiedDate=GETDATE() WHERE SkypeForBusinessUserInfo_Id = {userId} AND SkypeForBusinessResponseGroupInfo_Id ={groupId} AND  EndpointSystemId={endPointSystemId}";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId)
        {
            return CurrentContext.SkypeForBusinessResponseGroup.Where(ep => ep.EndPointSystemId == endPointSystemId).ToDictionary(item => item.Id, item => item.Name);

        }

        public Dictionary<long, string> GetResponseGroup(long endPointUserId, long endpointId, string searchString, AdHocActionType adHocActionType)
        {
            var returnValue = new Dictionary<long, string>();
            
            var provisioningMatrix = CurrentContext.SkypeForBusinessProvisioningMatrix.ToList().Where(i => !string.IsNullOrWhiteSpace(i.ADID)).ToList();
            provisioningMatrix = provisioningMatrix.Where(i => i.EndPointSystemId == endpointId && i.RequestType == EntitlementType.ResponseGroup).ToList();
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                provisioningMatrix = provisioningMatrix.Where(i => i.Name.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            if (provisioningMatrix == null || provisioningMatrix.Count() == 0)
            {
                return returnValue;
            }
            var responseGroups = (from rGroup in CurrentContext.SkypeForBusinessResponseGroup
                                where rGroup.EndPointSystemId == endpointId
                                  select rGroup).ToList();
            
            var userWithGroups = (from user in CurrentContext.SkypeForBusinessUserInfo.Include("SkypeForBusinessResponseGroup")
                                  where user.EndPointSystemId == endpointId && user.Id == endPointUserId
                                  select user).FirstOrDefault();
            responseGroups = responseGroups.Where(i => !string.IsNullOrWhiteSpace(i.DistributionGroupName) && provisioningMatrix.Exists(j => j.Name.Equals(i.Name, StringComparison.InvariantCultureIgnoreCase))).OrderBy(i=>i.Name).ToList();

            if (adHocActionType == AdHocActionType.AmendmentAdd || adHocActionType == AdHocActionType.NewAccount)
            {
                foreach (var responseGroup in responseGroups)
                {
                    if(userWithGroups != null && userWithGroups.SkypeForBusinessResponseGroup.Count > 0)
                    {
                        if (userWithGroups.SkypeForBusinessResponseGroup.FirstOrDefault(i => i.Name == responseGroup.Name) == null)
                        {
                            returnValue.Add(responseGroup.Id, responseGroup.Name);
                        }
                    }
                    else
                    {
                        returnValue.Add(responseGroup.Id, responseGroup.Name);
                    }
                }
            }
            else if(adHocActionType == AdHocActionType.AmendmentRemove)
            {
                foreach (var responseGroup in responseGroups)
                {
                    if (userWithGroups.SkypeForBusinessResponseGroup.Count > 0 && userWithGroups.SkypeForBusinessResponseGroup.Exists(i => i.Name == responseGroup.Name && !string.IsNullOrWhiteSpace(i.DistributionGroupName)))
                    {
                        returnValue.Add(responseGroup.Id, responseGroup.Name);
                    }
                }
            }
            return returnValue;
        }
        public Dictionary<long, string> GetResponseGroupForBulkRequest(long endpointId, string searchString, AdHocActionType adHocActionType)
        {
            var returnValue = new Dictionary<long, string>();

            var provisioningMatrix = CurrentContext.SkypeForBusinessProvisioningMatrix.ToList();
            provisioningMatrix = provisioningMatrix.Where(i => i.EndPointSystemId == endpointId && i.RequestType == EntitlementType.ResponseGroup).ToList();
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                provisioningMatrix = provisioningMatrix.Where(i => i.Name.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            if (provisioningMatrix == null || provisioningMatrix.Count() == 0)
            {
                return returnValue;
            }

            var responseGroups = (from rGroup in CurrentContext.SkypeForBusinessResponseGroup
                                  where rGroup.EndPointSystemId == endpointId
                                  select rGroup).ToList();

            responseGroups = responseGroups.Where(i => !string.IsNullOrWhiteSpace(i.DistributionGroupName) && provisioningMatrix.Exists(j => j.Name.Equals(i.Name, StringComparison.InvariantCultureIgnoreCase))).OrderBy(i => i.Name).ToList();

            foreach (var responseGroup in responseGroups)
            {
                returnValue.Add(responseGroup.Id, responseGroup.Name);
            }

            return returnValue;
        }
        public Dictionary<string, string> GetResponseGroupWithADID(long endpointsystemId)
        {
            var result = new Dictionary<string, string>();
            var query = from responseGroup in CurrentContext.SkypeForBusinessResponseGroup
                        join matrix in CurrentContext.SkypeForBusinessProvisioningMatrix on responseGroup.Name equals matrix.Name
                        where responseGroup.EndPointSystemId == endpointsystemId && matrix.EndPointSystemId == endpointsystemId && matrix.RequestType == EntitlementType.ResponseGroup
                                        && matrix.ADID != null
                        select new
                        {
                            Name = responseGroup != null ? responseGroup.Name : null,
                            ADID = matrix != null ? matrix.ADID : null
                        };
            foreach (var item in query)
            {
                if (!result.ContainsKey(item.Name))
                    result.Add(item.Name, item.ADID);
            }

            return result;
        }
        public List<PresentationResponseGroupMember> GetMaskedUser(long endpointsystemId,long userId, Nullable<bool> takeMaskedUser, string groupName = null)
        {
            var query = $@" SELECT 
                            G.Name as ResponseGroupName,U.UserPrincipalName as MemberPrincipalName,M.ADID,RG.IsMasked,G.EndPointSystemId,RG.SkypeForBusinessUserInfo_Id as UserId
                            FROM SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo RG INNER JOIN DI_SFB_ResponseGroupInfo G on RG.SkypeForBusinessResponseGroupInfo_Id = G.Id
                            INNER JOIN DI_SFB_UserInfo U on RG.SkypeForBusinessUserInfo_Id = U.Id
                            INNER JOIN DI_SFB_ProvisioningMatrix M ON G.Name = M.Name AND M.RequestType = 30
                            WHERE RG.EndpointSystemId={endpointsystemId} AND U.EndPointSystemId={endpointsystemId} AND G.EndPointSystemId={endpointsystemId} AND
                                 U.Id = {userId}";
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                query += $" AND G.Name='{groupName.Replace("'","''")}'";
            }
            if (takeMaskedUser.HasValue)
            {
                if (takeMaskedUser.Value)
                {
                    query += $" AND RG.IsMasked=1";
                }else
                {
                    query += $" AND (RG.IsMasked=NULL OR RG.IsMasked=0)";
                }
                
            }else
            {
                query += $" AND (RG.IsMasked=NULL OR RG.IsMasked=0)";
            }
            
            return CurrentContext.Database.SqlQuery<PresentationResponseGroupMember>(query).ToList();
        }
        public List<PresentationResponseGroupMember> GetResponseGroupWithMaskStatus(long endpointsystemId, long userId)
        {
            var query = $@" SELECT 
                            G.Name as ResponseGroupName,U.UserPrincipalName as MemberPrincipalName,M.ADID,RG.IsMasked,G.EndPointSystemId,RG.SkypeForBusinessUserInfo_Id as UserId
                            FROM SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo RG INNER JOIN DI_SFB_ResponseGroupInfo G on RG.SkypeForBusinessResponseGroupInfo_Id = G.Id
                            INNER JOIN DI_SFB_UserInfo U on RG.SkypeForBusinessUserInfo_Id = U.Id
                            INNER JOIN DI_SFB_ProvisioningMatrix M ON G.Name = M.Name AND M.RequestType = 30
                            WHERE RG.EndpointSystemId={endpointsystemId} AND U.EndPointSystemId={endpointsystemId} AND G.EndPointSystemId={endpointsystemId} AND
                                 U.Id = {userId}";
            return CurrentContext.Database.SqlQuery<PresentationResponseGroupMember>(query).ToList();
        }
        public List<PresentationResponseGroupUserMember> GetResponseGroupUser(string responsegroupname)
        {
            var result = new Dictionary<string, string>();
            var query = $@" SELECT G.id,
                            G.Name as ResponseGroupName,U.UserPrincipalName ,U.DisplayName as LastName,U.LineURI
                            FROM SkypeForBusinessUserInfoSkypeForBusinessResponseGroupInfo RG INNER JOIN DI_SFB_ResponseGroupInfo G on RG.SkypeForBusinessResponseGroupInfo_Id = G.Id
                            INNER JOIN DI_SFB_UserInfo U on RG.SkypeForBusinessUserInfo_Id = U.Id
                            INNER JOIN DI_SFB_ProvisioningMatrix M ON G.Name = M.Name
                            ";
            var parameters = new List<System.Data.SqlClient.SqlParameter>();
            if (!string.IsNullOrWhiteSpace(responsegroupname))
            {
                query += " AND G.Name=@ResponseGroupName";
                parameters.Add(new System.Data.SqlClient.SqlParameter("@ResponseGroupName", responsegroupname));
            }
            var items = this.CurrentContext.Database.SqlQuery<PresentationResponseGroupUserMember>(query, parameters.ToArray()).ToList();
            return items;
        }

        public List<string> GetDistributionGroup(long endpointId)
        {
            return CurrentContext.SkypeForBusinessResponseGroup.Where(i => i.EndPointSystemId == endpointId && i.DistributionGroupAddress != null).Select(i=> i.DistributionGroupAddress).ToList();
        }
    }
}
