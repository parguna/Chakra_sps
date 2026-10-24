using DeepIdentity.Infrastructure;
using DeepIdentity.Model;
using DeepIdentity.Model.Custom.GridData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.DataAccess.Implementation
{
    public class ACLDA
    {

        #region "URL"

        public IList<PageUrl> GetAllPageURLs()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.PageUrls.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PageUrl GetURLById(int inturlID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.PageUrls
                            where item.Id == inturlID
                            select item).FirstOrDefault();
                }
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
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var urlid = oIACMDBContext.PageUrls.Where(u => u.Id == urlInfo.Id).Select(u => u.Id).FirstOrDefault();

                    if (urlInfo.Id == null || urlInfo.Id == 0)
                    {
                        oIACMDBContext.PageUrls.Add(urlInfo);
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        urlInfo.Id = urlid;
                        PageUrl editUrl = oIACMDBContext.PageUrls.Where(u => u.Id == urlInfo.Id).First();
                        if (editUrl == null)
                            return;
                        oIACMDBContext.Entry(editUrl).CurrentValues.SetValues(urlInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
        }

        public bool DeleteURLById(int intURLID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    PageUrl delurl = oIACMDBContext.PageUrls.FirstOrDefault(u => u.Id == intURLID);
                    if (delurl != null)
                    {
                        oIACMDBContext.PageUrls.Remove(delurl);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured While Deleting the URL", ex);
                throw ex;
            }
        }

        public bool IsURLExists(string strUrl)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                var dbPermission = oIACMDBContext.PageUrls.Where(c => c.Url == strUrl).FirstOrDefault();
                return dbPermission != null ? true : false;
            }
        }

        public bool IsRelationExistsForURL(int intURLID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    bool isRecordsExists = false;
                    if (oIACMDBContext.Pages.Any(u => u.PageUrlId == intURLID) || oIACMDBContext.PagePermissions.Any(u => u.UrlId == intURLID))
                        isRecordsExists = true;

                    return isRecordsExists;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region "Permissions"

        public IList<Permission> GetAllPermissions()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Permissions.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Permission GetPermissionById(int intPermissionID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.Permissions
                            where item.Id == intPermissionID
                            select item).FirstOrDefault();
                }
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
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var perid = oIACMDBContext.Permissions.Where(p => p.Id == permissionInfo.Id).Select(p => p.Id).FirstOrDefault();

                    if (permissionInfo.Id == null || permissionInfo.Id == 0)
                    {
                        oIACMDBContext.Permissions.Add(permissionInfo);
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        permissionInfo.Id = perid;
                        Permission editper = oIACMDBContext.Permissions.Where(p => p.Id == permissionInfo.Id).First();
                        if (editper == null)
                            return;
                        oIACMDBContext.Entry(editper).CurrentValues.SetValues(permissionInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
        }

        public bool DeletePermissionById(int intPermissionID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    Permission delPermission = oIACMDBContext.Permissions.FirstOrDefault(Per => Per.Id == intPermissionID);
                    if (delPermission != null)
                    {
                        oIACMDBContext.Permissions.Remove(delPermission);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured While Deleting the Permissions", ex);
                throw ex;
            }
        }

        public bool IsPermissionExists(string strPermissionName)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                var dbPermission = oIACMDBContext.Permissions.Where(c => c.Name == strPermissionName).FirstOrDefault();
                return dbPermission != null ? true : false;
            }
        }

        public bool IsRelationExistsForPermission(int intPerID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    bool isRecordsExists = false;
                    if (oIACMDBContext.PagePermissions.Any(u => u.PermissionId == intPerID))
                        isRecordsExists = true;

                    return isRecordsExists;
                }
            }
            catch (Exception ex)
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Pages.Include("PageUrl").ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public WebPage GetPageById(int intPageID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.Pages.Include("PageUrl")
                            where item.Id == intPageID
                            select item).FirstOrDefault();
                }
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
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var pageid = oIACMDBContext.Pages.Where(p => p.Id == pageInfo.Id).Select(p => p.Id).FirstOrDefault();

                    if (pageInfo.Id == null || pageInfo.Id == 0)
                    {
                        oIACMDBContext.Pages.Add(pageInfo);
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        pageInfo.Id = pageid;
                        WebPage editpage = oIACMDBContext.Pages.Where(p => p.Id == pageInfo.Id).First();
                        if (editpage == null)
                            return;
                        oIACMDBContext.Entry(editpage).CurrentValues.SetValues(pageInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }

        }

        public bool DeletePageById(int intPageID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    WebPage delPage = oIACMDBContext.Pages.FirstOrDefault(Per => Per.Id == intPageID);
                    if (delPage != null)
                    {
                        oIACMDBContext.Pages.Remove(delPage);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured While Deleting the Page", ex);
                throw ex;
            }
        }

        public bool IsPageNameExists(WebPage pageInfo)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                var dbGroup = oIACMDBContext.Pages.Where(c => (c.Name == pageInfo.Name) || (c.UniqueName == pageInfo.UniqueName)).FirstOrDefault();
                return dbGroup != null ? true : false;
            }
        }

        public bool IsRelationExistsForPages(int intPageID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    bool isRecordsExists = false;
                    if (oIACMDBContext.TabMenu.Any(u => u.PageId == intPageID))
                        isRecordsExists = true;

                    return isRecordsExists;
                }
            }
            catch (Exception ex)
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.TabMenu
                            join men in _context.TabMenu
                            on item.ParentTabMenuId equals men.Id
                            join page in _context.Pages
                            on item.PageId equals page.Id
                            select new MenuGridData
                            {
                                Id = item.Id,
                                UniqueName = item.UniqueName,
                                MenuName = item.MenuName,
                                ParentMenuName = men.MenuName,
                                PageName = page.Name
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public TabMenu GetMenuById(int intMenuID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.TabMenu.Include("Page")
                            where item.Id == intMenuID
                            select item).FirstOrDefault();
                }
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
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var menuid = oIACMDBContext.Pages.Where(m => m.Id == menuInfo.Id).Select(m => m.Id).FirstOrDefault();

                    if (menuInfo.Id == null || menuInfo.Id == 0)
                    {
                        oIACMDBContext.TabMenu.Add(menuInfo);
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        menuInfo.Id = menuid;
                        Permission editpage = oIACMDBContext.Permissions.Where(m => m.Id == menuInfo.Id).First();
                        if (editpage == null)
                            return;
                        oIACMDBContext.Entry(editpage).CurrentValues.SetValues(menuInfo);
                        oIACMDBContext.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
        }

        public bool DeleteMenuById(int intMenuID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    TabMenu menu = oIACMDBContext.TabMenu.FirstOrDefault(m => m.Id == intMenuID);
                    if (menu != null)
                    {
                        oIACMDBContext.TabMenu.Remove(menu);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured While Deleting the Tabmenu", ex);
                throw ex;
            }
        }

        public bool IsMenuNameExists(TabMenu menuInfo)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                var menu = oIACMDBContext.TabMenu.Where(c => (c.MenuName == menuInfo.MenuName) || c.UniqueName == menuInfo.UniqueName).FirstOrDefault();
                return menu != null ? true : false;
            }
        }

        public bool IsRelationExistsForMenus(int intMenuID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    bool isRecordsExists = false;
                    if (oIACMDBContext.TabMenu.Any(u => u.ParentTabMenuId == intMenuID))
                        isRecordsExists = true;

                    return isRecordsExists;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region "PagePermission"

        public List<ACLPagePermissionGridData> GetPermissionByPageId(int intPageID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.PagePermissions
                            join per in _context.Permissions
                            on item.PermissionId equals per.Id
                            join url in _context.PageUrls
                            on item.UrlId equals url.Id
                            where item.PageId == intPageID
                            select new ACLPagePermissionGridData
                            {
                                Id = item.Id,
                                PermissionId = item.PermissionId,
                                PermissionName = per.Name,
                                UrlId = item.UrlId,
                                PageUrl = url.Url
                            }).ToList();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.PagePermissions
                            join per in _context.Permissions
                            on item.PermissionId equals per.Id
                            join url in _context.PageUrls
                            on item.UrlId equals url.Id
                            where item.Id == intPagePerID
                            select new ACLPagePermissionGridData
                            {
                                Id = item.Id,
                                PermissionId = item.PermissionId,
                                PermissionName = per.Name,
                                UrlId = item.UrlId,
                                PageUrl = url.Url
                            }).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool UpdatePermissionsForPage(ACLPagePermissionGridData lstPermission, int intPageId)
        {
            try
            {
                int recordsAffected = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    PagePermission per = new PagePermission();

                    per.PageId = intPageId;
                    per.PermissionId = lstPermission.PermissionId;
                    per.UrlId = lstPermission.UrlId;
                    per.CreatedDate = DateTime.Now;

                    oIACMDBContext.PagePermissions.Add(per);
                    recordsAffected = oIACMDBContext.SaveChanges();

                }

                if (recordsAffected > 0)
                    return true;

                return false;

            }
            catch { throw; }

        }

        public bool DeletePagePermissionByID(int intPagePermissionID)
        {
            try
            {
                int recordsAffected = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    PagePermission per = oIACMDBContext.PagePermissions.FirstOrDefault(p => p.Id == intPagePermissionID);

                    if (per != null)
                    {
                        oIACMDBContext.PagePermissions.Remove(per);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }
                }

                if (recordsAffected > 0)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured While Deleting the PagePermission", ex);
                throw ex;
            }
        }

        #endregion

        #region "AutoCompleteMethods"

        public Dictionary<int, string> GetMenusBySearchString(string searchString)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var uList = (from menu in oIACMDBContext.TabMenu
                                 where menu.MenuName.Contains(searchString) && menu.Id != 0
                                 select new { menu.MenuName, menu.Id }).ToDictionary(c => c.Id, c => c.MenuName);
                    return uList;
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error occured while getting Menu Details.", ex);
                throw ex;
            }
        }
        public TabMenu GetParentTabMenu(int ParentTabMenuId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.TabMenu.FirstOrDefault(ptm => ptm.Id == ParentTabMenuId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public TabMenu GetPageNameById(int PageId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.TabMenu.FirstOrDefault(ptm => ptm.PageId == PageId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PageUrl GetPageUrlById(int PageUrlId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.PageUrls.FirstOrDefault(ptm => ptm.Id == PageUrlId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public Dictionary<int, string> GetPagesBySearchString(string searchString)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var uList = (from page in oIACMDBContext.Pages
                                 where page.Name.Contains(searchString) && page.Id != 0
                                 select new { page.Name, page.Id }).ToDictionary(c => c.Id, c => c.Name);
                    return uList;
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error occured while getting Page Details.", ex);
                throw ex;
            }
        }

        public Dictionary<int, string> GetUrlsBySearchString(string searchString)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var uList = (from url in oIACMDBContext.PageUrls
                                 where url.Url.Contains(searchString) && url.Id != 0
                                 select new { url.Url, url.Id }).ToDictionary(c => c.Id, c => c.Url);
                    return uList;
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error occured while getting Url Details.", ex);
                throw ex;
            }
        }

        #endregion

    }
}
