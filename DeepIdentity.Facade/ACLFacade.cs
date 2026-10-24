using DeepIdentity.DataAccess.Implementation;
using DeepIdentity.Infrastructure;
using DeepIdentity.Model;
using DeepIdentity.Model.Custom.GridData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.Facade
{
    public class ACLFacade
    {

        #region private Members
        private ACLDA aclDA = null;
        #endregion


        public ACLFacade()
        {
            aclDA = new ACLDA();
        }

        #region "URL"

        public IList<PageUrl> GetAllPageURLs()
        {
            try
            {
                return aclDA.GetAllPageURLs();
            }
            catch
            {
                throw;
            }
        }

        public PageUrl GetURLById(int inturlID)
        {
            try
            {
                return aclDA.GetURLById(inturlID);
            }
            catch
            {
                throw;
            }
        }

        public void SaveURL(PageUrl urlInfo)
        {
            try
            {
                aclDA.SaveURL(urlInfo);
            }
            catch
            {
                throw;
            }
        }

        public bool DeleteURLById(int intURLID)
        {
            try
            {
                return aclDA.DeleteURLById(intURLID);
            }
            catch
            {
                throw;
            }
        }

        public bool IsURLExists(string strUrl)
        {
            try
            {
                return aclDA.IsURLExists(strUrl);
            }
            catch
            {
                throw;
            }
        }

        public bool IsRelationExistsForURL(int intURLID)
        {
            try
            {
                return aclDA.IsRelationExistsForURL(intURLID);
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "Permission"

        public IList<Permission> GetAllPermissions()
        {
            try
            {
                return aclDA.GetAllPermissions();
            }
            catch
            {
                throw;
            }
        }

        public Permission GetPermissionById(int intPermissionID)
        {
            try
            {
                return aclDA.GetPermissionById(intPermissionID);
            }
            catch
            {
                throw;
            }
        }

        public void SavePermissions(Permission permissionInfo)
        {
            try
            {
                aclDA.SavePermissions(permissionInfo);
            }
            catch
            {
                throw;
            }
        }

        public bool DeletePermissionById(int intPermissionID)
        {
            try
            {
                return aclDA.DeletePermissionById(intPermissionID);
            }
            catch
            {
                throw;
            }
        }

        public bool IsPermissionExists(string strPermissionName)
        {
            try
            {
                return aclDA.IsPermissionExists(strPermissionName);
            }
            catch
            {
                throw;
            }
        }

        public bool IsRelationExistsForPermission(int intPerID)
        {
            try
            {
                return aclDA.IsRelationExistsForPermission(intPerID);
            }
            catch
            {
                throw;
            }

        }

        #endregion

        #region "Pages"

        public IList<WebPage> GetAllPages()
        {
            try
            {
                return aclDA.GetAllPages();
            }
            catch
            {
                throw;
            }
        }

        public WebPage GetPageById(int intPageID)
        {
            try
            {
                return aclDA.GetPageById(intPageID);
            }
            catch
            {
                throw;
            }
        }

        public void SavePage(WebPage pageInfo)
        {
            try
            {
                aclDA.SavePage(pageInfo);
            }
            catch
            {
                throw;
            }
        }

        public bool DeletePageById(int intPageID)
        {
            try
            {
                return aclDA.DeletePageById(intPageID);
            }
            catch
            {
                throw;
            }
        }

        public bool IsPageNameExists(WebPage pageinfo)   
        {
            try
            {
                return aclDA.IsPageNameExists(pageinfo);
            }
            catch
            {
                throw;
            }
        }

        public bool IsRelationExistsForPages(int intPageID)
        {
            try
            {
                return aclDA.IsRelationExistsForPages(intPageID);
            }
            catch
            {
                throw;
            }

        }

        #endregion

        #region "Menu"

        public IList<MenuGridData> GetAllMenus()
        {
            try
            {
                return aclDA.GetAllMenus();
            }
            catch
            {
                throw;
            }
        }

        public TabMenu GetMenuById(int intMenuID)
        {
            try
            {
                return aclDA.GetMenuById(intMenuID);
            }
            catch
            {
                throw;
            }
        }

        public void SaveMenu(TabMenu menuInfo)
        {
            try
            {
                aclDA.SaveMenu(menuInfo);
            }
            catch
            {
                throw;
            }
        }

        public bool DeleteMenuById(int intMenuID)
        {
            try
            {
                return aclDA.DeleteMenuById(intMenuID);
            }
            catch
            {
                throw;
            }
        }

        public bool IsMenuNameExists(TabMenu menuInfo)
        {
            try
            {
                return aclDA.IsMenuNameExists(menuInfo);
            }
            catch
            {
                throw;
            }
        }
        public TabMenu GetParentTabMenu(int ParentTabMenuId)
        {
            try
            {
                return aclDA.GetParentTabMenu(ParentTabMenuId);
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }
        public TabMenu GetPageNameById(int PageId)
        {
            try
            {
                return aclDA.GetPageNameById(PageId);
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }
        public PageUrl GetPageUrlById(int PageUrlId)
        {
            try
            {
                return aclDA.GetPageUrlById(PageUrlId);
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }
        
        public bool IsRelationExistsForMenus(int intMenuID)
        {
            try
            {
                return aclDA.IsRelationExistsForMenus(intMenuID);
            }
            catch
            {
                throw;
            }

        }

        #endregion

        #region "PagePermissions"

        public List<ACLPagePermissionGridData> GetPermissionByPageId(int intPageID)
        {
            try
            {
                return aclDA.GetPermissionByPageId(intPageID);
            }
            catch
            {
                throw;
            }
        }

        public ACLPagePermissionGridData GetPagePermissionById(int intPagePerID)
        {
            try
            {
                return aclDA.GetPagePermissionById(intPagePerID);
            }
            catch
            {
                throw;
            }
        }

        public bool UpdatePermissionsForPage(ACLPagePermissionGridData lstPermission, int intPageId)
        {
            return aclDA.UpdatePermissionsForPage(lstPermission, intPageId);
        }

        public bool DeletePagePermissionByID(int intPagePermissionID)
        {
            return aclDA.DeletePagePermissionByID(intPagePermissionID);
        }
        
        #endregion

    }
}
