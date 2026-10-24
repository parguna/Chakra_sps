using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using DI.Chakra.Infrastructure;
using DI.Chakra.DataAccess;

namespace DI.Chakra.Facade
{
    public class AccessRightsFacade
    {

        AccessRightsDA accessRightsDA = null;

        public AccessRightsFacade()
        {
            accessRightsDA = new AccessRightsDA();
        }

        #region AccessRightsFacade Members

        public int Add(AccessRights _accessRights)
        {
            try
            {
                return accessRightsDA.Add(_accessRights);
            }
            catch
            {

                throw;
            }
        }

        #endregion


        
        public IList<AccessRights> GetAccessRightsByRoleId(int roleId)
        {
            return accessRightsDA.GetAccessRightsByRoleId(roleId);
        }

        public bool DeleteAccessRights(AccessRights AccessRight)
        {
            return accessRightsDA.DeleteAccessRights(AccessRight);
        }


      

        public void Update(AccessRights _aggregateRoot)
        {
            throw new NotImplementedException();
        }
        public AccessRights GetAccessRights(User userInfo, string absolutePath)
        {
            return accessRightsDA.GetAccessRights(userInfo, absolutePath);
        }
        public IList<AccessRights> GetAccessRights(int TabMenuId)
        {
            return accessRightsDA.GetAccessRights(TabMenuId);
        }
        //public IList<TabMenuPermission> GetTabMenuPermissions(int TabMenuId,User _user)
        //{
        //    return accessRightsDA.GetTabMenuPermissions(TabMenuId,_user);
        //}
        public IList<PagePermission> GetPagePermissions(int pageId, User _user)
        {
            return accessRightsDA.GetPagePermissions(pageId, _user);
        }
        public bool HasAccess(int pageId, User _user)
        {
            return accessRightsDA.HasAccess(pageId, _user);
        }
        public AccessRights GetAccessRights(int RoleId, int TabMenuId)
        {
            return accessRightsDA.GetAccessRight(RoleId, TabMenuId);
        }

        public List<PagePermission> GetPagePermission(int PageId)
        {
            return accessRightsDA.GetPagePermission(PageId);
        }

        public void SaveAccessRights(AccessRights AccessRightDetails)
        {
            accessRightsDA.SaveAccessRights(AccessRightDetails);
        }

        public PagePermission GetPermission(int PageId, int PermissionID)
        {
            return accessRightsDA.GetPermission(PageId, PermissionID);
        }

        public IList<Permission> GetActions(int PageId)
        {
            var List = accessRightsDA.GetPagePermission(PageId);
            IList<Permission> returnList = new List<Permission>();
            List.ForEach(lq =>
                {
                    returnList.Add(lq.Permission);
                });
            return returnList;
        }
    }
}
