using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository.Core.ACL
{
    public class PagePermissionRepository : RepositoryBase<PagePermission>
    {
        public PagePermissionRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationPagePermission> GetByPageId(long pageId)
        {
            return (from item in CurrentContext.PagePermissions
                    join per in CurrentContext.Permissions
                    on item.PermissionId equals per.Id
                    join url in CurrentContext.PageUrls
                    on item.UrlId equals url.Id into lq
                    from urlLeft in lq.DefaultIfEmpty()
                    where item.PageId == pageId
                    select new PresentationPagePermission
                    {
                        Id = item.Id,
                        PermissionId = item.PermissionId,
                        PermissionName = per.Name,
                        UrlId = item.UrlId,
                        PageUrlName = urlLeft.Url
                    }).ToList();
        }

        public PresentationPagePermission GetById(long id)
        {
            return (from item in CurrentContext.PagePermissions
                    join per in CurrentContext.Permissions
                    on item.PermissionId equals per.Id
                    join url in CurrentContext.PageUrls
                    on item.UrlId equals (long)url.Id
                    where item.Id == id
                    select new PresentationPagePermission
                    {
                        Id = item.Id,
                        PermissionId = item.PermissionId,
                        PermissionName = per.Name,
                        UrlId = item.UrlId,
                        PageUrlName = url.Url
                    }).FirstOrDefault();
        }

        public List<PagePermission> HasAccess(long pageId, long roleId)
        {
            return (from perms in this.CurrentContext.PagePermissions
                    join AR in this.CurrentContext.AccessRights
                    on (long)perms.Id equals AR.PagePermissionId
                    join R in this.CurrentContext.Roles
                    on AR.RoleId equals R.Id
                    where perms.PageId == pageId && R.Id == roleId
                    select perms).Distinct().ToList();

        }

        public List<PagePermission> GetPagePermissions(long pageId, long roleId)
        {
            return ((from perms in this.CurrentContext.PagePermissions
                     join AR in this.CurrentContext.AccessRights
                     on (long)perms.Id equals AR.PagePermissionId
                     join R in this.CurrentContext.Roles
                     on AR.RoleId equals R.Id
                     where perms.PageId == pageId && perms.Permission.Name != "View" && R.Id == roleId
                     select perms).Include("PageUrl").Include("Permission")).Distinct().ToList();
        }

        public List<PagePermission> GetPagePermissions(long userId)
        {
            return ((from perms in this.CurrentContext.PagePermissions
                     join AR in this.CurrentContext.AccessRights
                     on (long)perms.Id equals AR.PagePermissionId
                     join R in this.CurrentContext.Roles
                     on AR.RoleId equals R.Id
                     from roleUser in R.Users
                     //join u in CurrentContext.User

                     where roleUser.Id == userId
                     select perms).Include("PageUrl").Include("Permission")).Distinct().ToList();
        }
    }
}



