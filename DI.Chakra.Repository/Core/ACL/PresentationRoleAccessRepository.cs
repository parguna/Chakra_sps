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
    public class PresentationRoleAccessRepository : RepositoryBase<PresentationRoleAccess>
    {
        public PresentationRoleAccessRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IList<PresentationRoleAccess> GetAccessToRoles()
        {
            return (from mRole in CurrentContext.Roles
                    join ar in CurrentContext.AccessRights on mRole.Id equals ar.RoleId
                    join u in CurrentContext.PagePermissions on ar.PagePermissionId equals u.Id into pgPermissions
                    from o1 in pgPermissions.DefaultIfEmpty()
                    join p in CurrentContext.Permissions on o1.PermissionId equals p.Id into permissions
                    from o2 in permissions.DefaultIfEmpty()
                    join mPage in CurrentContext.Pages on o1.PageId equals mPage.Id into pages
                    from o3 in pages.DefaultIfEmpty()
                    where o3.Name != null
                    select new PresentationRoleAccess
                    {
                        RoleName = mRole.RoleName,
                        PageName =o3.Name,
                        AccessRights = o2.Name
                    }).Distinct().OrderBy(role=> role.RoleName).ThenBy(page=> page.PageName).ThenBy(ar=>ar.AccessRights).ToList();
        }

        public IList<PresentationRoleAccess> GetAccessToRoles(string roleCode)
        {
            return (from mRole in CurrentContext.Roles
                    join ar in CurrentContext.AccessRights on mRole.Id equals ar.RoleId
                    join u in CurrentContext.PagePermissions on ar.PagePermissionId equals u.Id into pgPermissions
                    from o1 in pgPermissions.DefaultIfEmpty()
                    join p in CurrentContext.Permissions on o1.PermissionId equals p.Id into permissions
                    from o2 in permissions.DefaultIfEmpty()
                    join mPage in CurrentContext.Pages on o1.PageId equals mPage.Id into pages
                    from o3 in pages.DefaultIfEmpty()
                    where o3.Name != null && mRole.RoleName == roleCode
                    select new PresentationRoleAccess
                    {
                        AccessId = ar.Id,
                        RoleName = mRole.RoleName,
                        PageName = o3.Name,
                        AccessRights = o2.Name
                    }).Distinct().OrderBy(role => role.RoleName).ThenBy(page => page.PageName).ThenBy(ar => ar.AccessRights).ToList();
        }
    }
}
