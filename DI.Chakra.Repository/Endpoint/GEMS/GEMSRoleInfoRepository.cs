using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.Logger;

namespace DI.Chakra.Repository
{
    public class GEMSRoleInfoRepository : RepositoryBase<GEMSRoleInfo>
    {
        public GEMSRoleInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetByEndpointId(long endpointId)
        {
            return (from grp in CurrentContext.GEMSRoleInfo.Where(ep => ep.EndPointSystemId == endpointId)
                    select new { grp.Id, grp.Name })
                         .ToDictionary(c => c.Id, c => c.Name);
        }

        public Dictionary<long, string> GetAllByName(string searchString)
        {
            return (from grp in CurrentContext.GEMSRoleInfo.Where(ep => ep.Name.Contains(searchString))
                    select new { grp.Id, grp.Name }).Take(100).Distinct().OrderBy(s => s.Name)
                        .ToDictionary(c => c.Id, c => c.Name);
        }

        public Dictionary<long, string> GetByEndpointIdAndName(string searchString, long endpointId)
        {
            return (from grp in CurrentContext.GEMSRoleInfo.Where(ep => ep.EndPointSystemId == endpointId
                    && string.IsNullOrEmpty(searchString) ? 1 == 1 : ep.Name.Contains(searchString))
                    select new { grp.Id, grp.Name })
                         .ToDictionary(c => c.Id, c => c.Name);
        }

        public List<PresentationAccessRequestItem> GetAccessRequestItem(string searchString)
        {
            return (from groups in CurrentContext.GEMSRoleInfo
                    where groups.Name.Contains(searchString)
                    orderby groups.Name
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EntitlementType = Infrastructure.Enums.EntitlementType.Group,
                        EndpointType = EndpointType.GEMS,
                        KeyId = groups.Id,
                        EndpointSystemId = groups.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.GEMS,
                        ItemName = groups.Name,
                        ItemDescription = groups.Description,
                        ItemDetail = groups.Display_Name
                    }).ToList();
        }

        public List<PresentationAccessRequestItem> GetGEMSAccessRequestItemIncludingentitlrment(string searchString, long? DIUserId)
        {

            return (from groups in CurrentContext.GEMSRoleInfo
                    where groups.Name.Contains(searchString)
                    orderby groups.Name
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EndpointType = EndpointType.GEMS,
                        KeyId = groups.Id,
                        EndpointSystemId = groups.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.GEMS,
                        ItemName = groups.Name,
                        ItemDescription = groups.Description
                    }).ToList();

        }
        public List<PresentationAccessRequestItem> GetAccessRequestItemForRemoveRequestSpecailACR(string SearchKey, long? DIUserId)
        {
            return (from groups in CurrentContext.GEMSRoleInfo
                    where groups.Name.Contains(SearchKey)
                    orderby groups.Name
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EndpointType = EndpointType.ActiveDirectory,
                        KeyId = groups.Id,
                        EndpointSystemId = groups.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.GEMS,
                        ItemName = groups.Name,
                        ItemDescription = groups.Description
                    }).ToList();
        }

        public Dictionary<long, string> GetBusinessRoles(long EndpointUserId, long endpointSystemId, AdHocActionType actionType, string searchstring)
        {
            long key = 0;
            searchstring = searchstring ?? string.Empty;

            List<string> businessRoles = this.CurrentContext.UserBusinessRoleMapping.Where(b => b.EndpointUserId == EndpointUserId && b.EndpointSystemId == endpointSystemId)
                                                                                    .Select(b => b.BusinessRole).ToList();

            if (actionType == AdHocActionType.AmendmentRemove) return businessRoles.Distinct().ToDictionary(Key => key++, Value => Value);

            var businessRolesQuery = CurrentContext.GEMSRoleMatrix.Select(b => b).AsQueryable();

            if (actionType == AdHocActionType.AmendmentRemove)
            {
                businessRolesQuery = businessRolesQuery.Where(businessRole => businessRoles.Contains(businessRole.BusinessRoleName)).Select(businessRole => businessRole);
            }
            else if (actionType == AdHocActionType.AmendmentAdd)
            {
                businessRolesQuery = businessRolesQuery.Where(businessRole => !businessRoles.Contains(businessRole.BusinessRoleName)).Select(businessRole => businessRole);
            }

            var bs = businessRolesQuery.ToList();

            return bs.Where(b => b.BusinessRoleName.StartsWith(searchstring, System.StringComparison.OrdinalIgnoreCase) || b.BusinessRoleName.Contains(searchstring, System.StringComparison.OrdinalIgnoreCase)).ToDictionary(item => item.Id, item => item.BusinessRoleName);
        }
    }
}
