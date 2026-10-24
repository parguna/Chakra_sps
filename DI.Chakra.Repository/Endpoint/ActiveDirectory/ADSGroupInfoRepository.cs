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
    public class ADSGroupInfoRepository : RepositoryBase<ADSGroupInfo>
    {
        public ADSGroupInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetByEndpointId(long endpointId)
        {
            return (from grp in CurrentContext.ADSGroupInfos.Where(ep => ep.EndPointSystemId == endpointId)
                    select new { grp.Id, grp.objectSid })
                         .ToDictionary(c => c.Id, c => c.objectSid);
        }

        public Dictionary<long, string> GetAllByName(string searchString)
        {
            return (from grp in CurrentContext.ADSGroupInfos.Where(ep => ep.name.Contains(searchString))
                    select new { grp.Id, grp.name }).Take(100).Distinct().OrderBy(s => s.name)
                        .ToDictionary(c => c.Id, c => c.name);
        }

        public Dictionary<long, string> GetByEndpointIdAndName(string searchString, long endpointId,long accessRequestId=0)
        {
            var query = from grp in CurrentContext.ADSGroupInfos.Where(ep => ep.EndPointSystemId == endpointId)
                        select grp;
           
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(i => i.name.Contains(searchString));
            }
            return query.Select(grp => new { grp.Id, grp.name }).ToDictionary(c => c.Id, c => c.name);
        }

        public List<PresentationAccessRequestItem> GetAccessRequestItem(string searchString)
        {
            return (from groups in CurrentContext.ADSGroupInfos
                    where groups.sAMAccountName.Contains(searchString) && groups.CanRequest == true 
                    orderby groups.name
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EntitlementType = Infrastructure.Enums.EntitlementType.Group,
                        EndpointType = EndpointType.ActiveDirectory,
                        KeyId = groups.Id,
                        EndpointSystemId = groups.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                        ItemName = groups.sAMAccountName,
                        ItemDescription = groups.description,
                        ItemDetail = groups.UserDefinedDesc
                    }).ToList();
        }

        public List<PresentationAccessRequestItem> GetADAccessRequestItemIncludingentitlrment(string searchString, long? DIUserId)
        {

            return (from groups in CurrentContext.ADSGroupInfos
                    where groups.sAMAccountName.Contains(searchString)
                    orderby groups.name
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EndpointType = EndpointType.ActiveDirectory,
                        KeyId = groups.Id,
                        EndpointSystemId = groups.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                        ItemName = groups.sAMAccountName,
                        ItemDescription = groups.description
                    }).ToList();

        }
        public List<PresentationAccessRequestItem> GetAccessRequestItemForRemoveRequestSpecailACR(string SearchKey, long? DIUserId)
        {
            return (from groups in CurrentContext.ADSGroupInfos
                    where groups.sAMAccountName.Contains(SearchKey)
                    orderby groups.name
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EndpointType = EndpointType.ActiveDirectory,
                        KeyId = groups.Id,
                        EndpointSystemId = groups.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.ActiveDirectory,
                        ItemName = groups.sAMAccountName,
                        ItemDescription = groups.description
                    }).ToList();
        }
        public List<string> GetExistingGroupByEndpointUserId(long endpointId, List<long> endpointUserIds, AdHocActionType type=AdHocActionType.AmendmentAdd)
        {
            if(type == AdHocActionType.AmendmentAdd)
            {
                return (from accountRecon in CurrentContext.AccountReconcilations
                        join entitlement in CurrentContext.EntitlementReconcilations on accountRecon.Id equals entitlement.AccountReconcilationId
                        where accountRecon.EndpointSystemId == endpointId && entitlement.EndpointSystemId == endpointId && endpointUserIds.Contains(accountRecon.EndpointUserId)
                        && accountRecon.IsLocked == false && accountRecon.IsDeleted == false
                        && entitlement.IsDeleted == false && entitlement.EntitlementType == (int)EntitlementType.Group
                        select entitlement.Entitlement).ToList();
            }
            else
            {
                
                var result = (from accountRecon in CurrentContext.AccountReconcilations
                        join entitlement in CurrentContext.EntitlementReconcilations on accountRecon.Id equals entitlement.AccountReconcilationId
                        where accountRecon.EndpointSystemId == endpointId && entitlement.EndpointSystemId == endpointId && endpointUserIds.Contains(accountRecon.EndpointUserId)
                        && accountRecon.IsLocked == false && accountRecon.IsDeleted == false
                        && entitlement.IsDeleted == false && entitlement.EntitlementType == (int)EntitlementType.Group
                        select entitlement.Entitlement);
                if (endpointUserIds.Count > 1)
                    return result.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                else
                    return result.ToList();
            }
        }
    }
}
