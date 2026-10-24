using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Repository;
using DI.Chakra.Repository.Core.ACL;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class PagePermission : DomainModelBase<PagePermission, EntityModel.PagePermission>
    {
        public long PageId { get; set; }

        public long PermissionId { get; set; }

        public long? UrlId { get; set; }

        public WebPage WebPage { get; set; }

        public Permission Permission { get; set; }

        public PageUrl PageUrl { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string Url
        {
            get
            {
                if (PageUrl != null)
                    return PageUrl.Url;
                return string.Empty;
            }
        }

        public class PermissionComparer : IEqualityComparer<PagePermission>
        {
            public bool Equals(PagePermission x, PagePermission y)
            {
                return x.PermissionId.Equals(y.PermissionId) && x.PageId.Equals(y.PageId) && x.UrlId.Equals(y.UrlId);
            }

            public int GetHashCode(PagePermission obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        public static List<PagePermission> GetByPageId(long pageId, bool eagerLoadAllProperties)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.PagePermission>(dataAccessTransaction).Get(pagePermission => pagePermission.PageId == pageId, eagerLoadAllProperties).ToList());
            }
        }

        public bool UpdatePermissionsForPage(PresentationPagePermission lstPermission, long pageId,string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                PagePermission pagePermission = new PagePermission();
                pagePermission.PageId = pageId;
                pagePermission.PermissionId = lstPermission.PermissionId;
                pagePermission.UrlId = lstPermission.UrlId;
                pagePermission.CreatedDate = DateTime.Now;
                pagePermission.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();

                return true;
            }
        }

        public static PagePermission GetByAbsolutePath(string absolutePath)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.PagePermission>(dataAccessTransaction).Get(t => t.PageUrl.Url.ToUpper() == absolutePath.ToUpper(), t => t.PageUrl, t => t.Permission).FirstOrDefault());
            }
        }

        public static bool HasAccess(long pageId, long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                User user = User.GetByIdIncludeRoles(userId);
                List<PagePermission> permissions = new List<PagePermission>();

                if (user.Roles != null && user.Roles.Count > 0)
                {
                    foreach (Role role in user.Roles)
                    {
                        var per = ORMapList(new PagePermissionRepository(dataAccessTransaction).HasAccess(pageId, role.Id)).ToList();
                        permissions.AddRange(per);
                    }
                }

                if (permissions.Count > 0)
                    return true;
                return false;
            }
        }

        public static IList<PagePermission> GetPagePermissions(long pageId, long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                User user = User.GetByIdIncludeRoles(userId);
                List<PagePermission> permissions = new List<PagePermission>();

                if (user.Roles != null && user.Roles.Count > 0)
                {
                    foreach (Role role in user.Roles)
                    {
                        permissions = permissions.Union(ORMapList(new PagePermissionRepository(dataAccessTransaction).GetPagePermissions(pageId, role.Id)), new IdBaseEqualityComparer<PagePermission>()).ToList();
                    }
                }

                return permissions;
            }

        }

        public static IList<PagePermission> GetPagePermissionsByUser(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PagePermissionRepository(dataAccessTransaction).GetPagePermissions(userId)).ToList();
            }
        }
        public static bool DoesExistsByPermissionId(int permissionId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.PagePermission>(dataAccessTransaction).Any(u => u.PermissionId == permissionId);
            }
        }

        public static IList<PagePermission> GetPagePermissions(long pageId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.PagePermission>(dataAccessTransaction).Get(page => page.PageId == pageId, page => page.Permission).ToList());
            }
        }
        public static List<PagePermission> GetByUrlId(long urlId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                WebPage page = WebPage.GetByUrlId(urlId);
                if(page != null)
                    return ORMapList(new RepositoryBase<EntityModel.PagePermission>(dataAccessTransaction).Get(PP=>PP.PageId == page.Id && PP.UrlId == null).ToList());
                return null;
            }
        }

        public static bool  UpdateUrlPermission(bool isAddPermission, long urlId, long roleID)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                WebPage page = WebPage.GetByUrlId(urlId);
                if (page != null)
                {
                    var permissions = ORMapList(new RepositoryBase<EntityModel.PagePermission>(dataAccessTransaction).Get(PP => PP.PageId == page.Id && PP.UrlId == null).ToList());
                    var accessRightList = AccessRights.GetAccessRightsByRoleId(roleID);
                    foreach (var item in permissions)
                    {
                        if (isAddPermission)
                        {
                            
                            var accessRights = accessRightList.Where(a => a.PagePermissionId == item.Id).FirstOrDefault();
                            if (accessRights == null)
                            {
                                AccessRights addAccessRight = new AccessRights();
                                addAccessRight.PagePermissionId = item.Id;
                                addAccessRight.RoleId = roleID;
                                addAccessRight.Save(dataAccessTransaction);
                            }
                        }
                        else
                        {
                            var accessRights = accessRightList.Where(a => a.PagePermissionId == item.Id).FirstOrDefault();
                            if (accessRights != null)
                                AccessRights.DeleteAccessRights(accessRights);
                        }
                    }
                    
                }
                dataAccessTransaction.Commit();
            }
            return true;
        }
    }
}
