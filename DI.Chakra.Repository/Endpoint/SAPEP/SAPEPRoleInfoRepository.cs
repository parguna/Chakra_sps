using DI.Chakra.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Transaction;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Repository
{
    public class SAPEPRoleInfoRepository : RepositoryBase<SAPEPRoleInfo>
    {

        public Dictionary<string, string> GetRolesById(long Id, AdHocActionType actionType, string searchString, long endPointSystemId)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            if (Id > 0)
            {
                if (actionType == AdHocActionType.AmendmentAdd)
                {
                    return this.CurrentContext.SAPEPRoleInfos
                    .Where(role => role.EndPointSystemId == endPointSystemId && role.CanRequest.HasValue && role.CanRequest.Value && !this.CurrentContext.EntitlementReconcilations
                                                            .Where(ent => ent.EndpointUserId == Id && ent.EntitlementType == (int)EntitlementType.Role
                                                                           && !ent.IsDeleted
                                                                           && string.IsNullOrEmpty(searchString) ? true : ent.Entitlement.Contains(searchString))
                                                            .Select(ent => ent.Entitlement)
                                        .Contains(role.RoleName))
                    .Select(role => new { Id = role.Id, Name = role.RoleName }).Distinct().ToDictionary(key => key.Id.ToString(), value => value.Name);

                }
                else if (actionType == AdHocActionType.AmendmentRemove)
                {
                    return this.CurrentContext.SAPEPRoleInfos
                   .Where(role => role.EndPointSystemId == endPointSystemId && role.CanRequest.HasValue && role.CanRequest.Value && this.CurrentContext.EntitlementReconcilations
                                                            .Where(ent => ent.EndpointUserId == Id && ent.EntitlementType == (int)EntitlementType.Role
                                                                            && !ent.IsDeleted
                                                                            && string.IsNullOrEmpty(searchString) ? true : ent.Entitlement.Contains(searchString))
                                                            .Select(ent => ent.Entitlement)
                                        .Contains(role.RoleName))
                   .Select(role => new { Id = role.Id, Name = role.RoleName }).Distinct().ToDictionary(key => key.Id.ToString(), value => value.Name);
                }
            }
            else
            {
                return this.CurrentContext.SAPEPRoleInfos.Where(role => role.EndPointSystemId == endPointSystemId && role.CanRequest.HasValue && role.CanRequest.Value
                                        && string.IsNullOrEmpty(searchString) ? true : role.RoleName.Contains(searchString)).Select(role => new { Id = role.Id, Name = role.RoleName }).Distinct().ToDictionary(key => key.Id.ToString(), value => value.Name);
            }
            return keyValuePairs;
        }
        public Dictionary<string, string> GetRoleSAPEP(int? EndpointSystemId)
        {

            var result = (from source in CurrentContext.SAPEPRoleInfos
                          where source.EndPointSystemId == EndpointSystemId &&
                                source.IsXLUploaded == true
                          select source);

            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var items in result)
            {
                if (!data.ContainsValue(items.RoleName))
                {
                    data.Add(items.UniqueRoleName, items.RoleName);
                }
            }
            return data;

        }
        public SAPEPRoleInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationSAPEPRoleToUsersReport> GetByEndpointSystemId(long endpointSystemId)
        {
            return (from sapepRle in CurrentContext.SAPEPRoleInfos
                    from sapepUser in sapepRle.SAPEPUserInfos
                    where (!string.IsNullOrEmpty(sapepUser.UserName) && sapepRle.EndPointSystemId == endpointSystemId)
                    select new PresentationSAPEPRoleToUsersReport
                    {
                        RoleId = sapepRle.Id,
                        RoleName = sapepRle.RoleName,
                        RoleDescription = sapepRle.RoleDescription,

                        UserId = sapepUser.Id,
                        UserName = sapepUser.UserName,
                        DisplayName = sapepUser.DisplayName,
                        InternetAddress = sapepUser.Email,
                        Department = sapepUser.Department,
                        IACMPasswordChangeDate = sapepUser.IACMLastPasswordChange
                        //IsCompanyUser = sapepUser.IsCompanyUser ? "YES" : "NO",
                    }
                           ).ToList();
        }

        public Dictionary<long, string> GetAllNames()
        {
            return CurrentContext.SAPEPRoleInfos.ToDictionary(item => (long)item.Id, item => item.RoleName);
        }



        public Dictionary<string, string> GetRolesToRemove(List<string> sapRoles)
        {
            var result = (from businessRole in CurrentContext.SAPEPRoleInfos
                          join tRoles in sapRoles on businessRole.RoleName equals tRoles
                          select businessRole)
                          .Select(role => new { key = role.UniqueRoleName, value = role.RoleName }).OrderBy(item => item.value)
                          .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);


            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var items in result)
            {
                if (!data.ContainsValue(items.Value))
                {
                    data.Add(items.Key, items.Value);
                }
            }

            return data;
        }

        public Dictionary<string, string> GetPortalRoles(long? endpointSystemId, string searchString)
        {
            var result = (from source in CurrentContext.SAPEPRoleInfos where !string.IsNullOrEmpty(source.UniqueRoleName) && source.RoleName.Contains(searchString) && source.EndPointSystemId == endpointSystemId && source.IsXLUploaded select source);

            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var items in result)
            {
                if (!data.ContainsValue(items.RoleName))
                {
                    data.Add(items.UniqueRoleName, items.RoleName);
                }
            }
            return data;
        }
        public Dictionary<string, string> GetPortalRolesByActionType(long? endpointSystemId, string searchString, List<long> endpointUserId, AdHocActionType actionType)
        {
            
            Dictionary<string, string> data = new Dictionary<string, string>();
            
            
            if(actionType == AdHocActionType.AmendmentAdd || actionType == AdHocActionType.AmendmentRemove)
            {

                var entitlements = (from accountRecon in CurrentContext.AccountReconcilations
                                join entitlement in CurrentContext.EntitlementReconcilations on accountRecon.Id equals entitlement.AccountReconcilationId
                                where accountRecon.EndpointSystemId == endpointSystemId && entitlement.EndpointSystemId == endpointSystemId && endpointUserId.Contains(accountRecon.EndpointUserId)
                                && accountRecon.IsLocked == false && accountRecon.IsDeleted == false
                                && entitlement.IsDeleted == false && entitlement.EntitlementType == (int)EntitlementType.Role
                                select entitlement.Entitlement);
                if(actionType == AdHocActionType.AmendmentRemove)
                {
                    var entitlementList = new List<string>();
                    if (endpointUserId.Count > 1)
                        entitlementList = entitlements.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                    else
                        entitlementList = entitlements.Distinct().ToList();

                    if (entitlementList.Count == 0)
                    {
                        return data;
                    }
                    else
                    {
                        return this.CurrentContext.SAPEPRoleInfos
                       .Where(role => role.EndPointSystemId == endpointSystemId && !string.IsNullOrEmpty(role.UniqueRoleName) && role.IsXLUploaded
                       && (string.IsNullOrEmpty(searchString) ? true : role.RoleName.Contains(searchString))
                              && entitlementList.Contains(role.UniqueRoleName))
                       .Select(role => new { role.UniqueRoleName, Name = role.RoleName }).Distinct().ToDictionary(key => key.UniqueRoleName.ToString(), value => value.Name);
                    }
                }
                else
                {
                    return this.CurrentContext.SAPEPRoleInfos
                      .Where(role => role.EndPointSystemId == endpointSystemId && !string.IsNullOrEmpty(role.UniqueRoleName) && role.IsXLUploaded
                      && (string.IsNullOrEmpty(searchString) ? true : role.RoleName.Contains(searchString))
                             && !entitlements.Contains(role.UniqueRoleName))
                      .Select(role => new { role.UniqueRoleName, Name = role.RoleName }).Distinct().ToDictionary(key => key.UniqueRoleName.ToString(), value => value.Name);
                }
            }
            var result = (from source in CurrentContext.SAPEPRoleInfos where !string.IsNullOrEmpty(source.UniqueRoleName)
                          && (string.IsNullOrEmpty(searchString) ? true : source.RoleName.Contains(searchString))
                          && source.EndPointSystemId == endpointSystemId && source.IsXLUploaded select source);

            foreach (var items in result)
            {
                if (!data.ContainsValue(items.RoleName))
                {
                    data.Add(items.UniqueRoleName, items.RoleName);
                }
            }
            return data;
        }
    }
}
