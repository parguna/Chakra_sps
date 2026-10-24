using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SAPEPGroupInfoRepository : RepositoryBase<SAPEPGroupInfo>
    {
        public SAPEPGroupInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<string, string> GetPortalGroups(long? endpointSystemId, string searchString)
        {
            var result = (from source in CurrentContext.SAPEPGroupInfos where !string.IsNullOrEmpty(source.GroupUniqueName) && source.GroupName.Contains(searchString) && source.EndPointSystemId == endpointSystemId && source.IsXLUploaded select source).ToList();

            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var items in result)
            {
                if (!data.ContainsValue(items.GroupName))
                {
                    data.Add(items.GroupUniqueName, items.GroupName);
                }
            }
            return data;
        }
        public Dictionary<string, string> GetPortalGroupsByActionType(long endpointSystemId, string searchString,List<long> endpointUserId, AdHocActionType actionType)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (actionType == AdHocActionType.AmendmentAdd || actionType == AdHocActionType.AmendmentRemove)
            {
               
                var entitlements = (from accountRecon in CurrentContext.AccountReconcilations
                                    join entitlement in CurrentContext.EntitlementReconcilations on accountRecon.Id equals entitlement.AccountReconcilationId
                                    where accountRecon.EndpointSystemId == endpointSystemId && entitlement.EndpointSystemId == endpointSystemId && endpointUserId.Contains(accountRecon.EndpointUserId)
                                    && accountRecon.IsLocked == false && accountRecon.IsDeleted == false
                                    && entitlement.IsDeleted == false && entitlement.EntitlementType == (int)EntitlementType.Group
                                    select entitlement.Entitlement);
               
                if (actionType == AdHocActionType.AmendmentRemove)
                {
                    var entitlementList = new List<string>();
                    if (endpointUserId.Count > 1)
                        entitlementList = entitlements.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                    else
                        entitlementList = entitlements.Distinct().ToList();
                    if(entitlementList.Count == 0)
                    {
                        return data;
                    }
                    else
                    {
                        return this.CurrentContext.SAPEPGroupInfos
                           .Where(group => group.EndPointSystemId == endpointSystemId && group.IsXLUploaded && !string.IsNullOrEmpty(group.GroupUniqueName)
                           && (string.IsNullOrEmpty(searchString) ? true : group.GroupName.Contains(searchString))
                           && entitlementList.Contains(group.GroupUniqueName))
                           .Select(group => new { group.GroupUniqueName, Name = group.GroupName }).Distinct().ToDictionary(key => key.GroupUniqueName, value => value.Name);
                    }
                }
                else
                {
                    return this.CurrentContext.SAPEPGroupInfos
                   .Where(group => group.EndPointSystemId == endpointSystemId && group.IsXLUploaded && !string.IsNullOrEmpty(group.GroupUniqueName)
                            && (string.IsNullOrEmpty(searchString) ? true : group.GroupName.Contains(searchString))
                            && !entitlements.Contains(group.GroupName))
                   .Select(group => new { group.GroupUniqueName, Name = group.GroupName }).Distinct().ToDictionary(key => key.GroupUniqueName, value => value.Name);
                }
            }
               
            var result = (from source in CurrentContext.SAPEPGroupInfos
                          where !string.IsNullOrEmpty(source.GroupUniqueName) && (string.IsNullOrEmpty(searchString) ? true : source.GroupName.Contains(searchString)) 
                          && source.EndPointSystemId == endpointSystemId && source.IsXLUploaded
                          select source);

            foreach (var items in result)
            {
                if (!data.ContainsValue(items.GroupName))
                {
                    data.Add(items.GroupUniqueName, items.GroupName);
                }
            }
            return data;
        }

        public List<PresentationSAPEPGroupToUsersReport> GetByEndpointSystemId(long endpointSystemId)
        {
            return (from sapepGrp in CurrentContext.SAPEPGroupInfos
                    from sapepUser in sapepGrp.SAPEPUserInfos
                    where (!string.IsNullOrEmpty(sapepUser.UserName) && sapepGrp.EndPointSystemId == endpointSystemId)
                    select new PresentationSAPEPGroupToUsersReport
                    {
                        GroupId = sapepGrp.Id,
                        GroupName = sapepGrp.GroupName,
                        GroupDescription = sapepGrp.GroupDescription,

                        UserId = sapepUser.Id,
                        UserName = sapepUser.UserName,
                        DisplayName = sapepUser.DisplayName,
                        InternetAddress = sapepUser.Email,
                        Department = sapepUser.Department,
                        IACMPasswordChangeDate = sapepUser.IACMLastPasswordChange
                    }
                            ).ToList();
        }

        public Dictionary<long, string> GetAllNamesByEndpoint(long endpointID)
        {
            return CurrentContext.SAPEPGroupInfos.Where(e => e.EndPointSystemId == endpointID).ToDictionary(item => (long)item.Id, item => item.GroupName);
        }

        public Dictionary<string, string> GetGroupSAPEP(long EndpointSystemId)
        {
            return CurrentContext.SAPEPGroupInfos.Where(source => source.EndPointSystemId == EndpointSystemId &&
                                                                       source.IsXLUploaded == true)
                                                                       .Select(source => new { Key = source.GroupUniqueName, Value = source.GroupName })
                                                                         .OrderBy(source => source.Value)
                                                                         .Distinct()
                                                                         .ToDictionary(c => c.Key, c => c.Value);


        }



        public Dictionary<string, string> GetGroupsById(long Id, AdHocActionType actionType, string searchString, long endPointSystemId)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            if (Id > 0)
            {
                if (actionType == AdHocActionType.AmendmentAdd)
                {
                    return this.CurrentContext.SAPEPGroupInfos
                    .Where(group => group.EndPointSystemId == endPointSystemId && group.IsValid && !this.CurrentContext.EntitlementReconcilations
                                                           .Where(ent => ent.EndpointUserId == Id
                                                                           && ent.EntitlementType == (int)EntitlementType.Group
                                                                           && !ent.IsDeleted
                                                                           && string.IsNullOrEmpty(searchString) ? true : ent.Entitlement.Contains(searchString))
                                                           .Select(ent => ent.Entitlement)
                                       .Contains(group.GroupName))
                    .Select(group => new { Id = group.Id, Name = group.GroupName }).Distinct().ToDictionary(key => key.Id.ToString(), value => value.Name);

                }
                else if (actionType == AdHocActionType.AmendmentRemove)
                {
                    return this.CurrentContext.SAPEPGroupInfos
                   .Where(group => group.EndPointSystemId == endPointSystemId && group.IsValid && this.CurrentContext.EntitlementReconcilations
                                                            .Where(ent => ent.EndpointUserId == Id
                                                                            && ent.EntitlementType == (int)EntitlementType.Group
                                                                            && !ent.IsDeleted
                                                                            && string.IsNullOrEmpty(searchString) ? true : ent.Entitlement.Contains(searchString))
                                                            .Select(ent => ent.Entitlement)
                                        .Contains(group.GroupName))
                   .Select(group => new { Id = group.Id, Name = group.GroupName }).Distinct().ToDictionary(key => key.Id.ToString(), value => value.Name);
                }
            }
            else
            {
                return this.CurrentContext.SAPEPGroupInfos.Where(group => group.IsValid && string.IsNullOrEmpty(searchString) ? true : group.GroupName.Contains(searchString)).Select(group => new { Id = group.Id, Name = group.GroupName }).Distinct().ToDictionary(key => key.Id.ToString(), value => value.Name);
            }
            return keyValuePairs;

        }

     



    }
}
