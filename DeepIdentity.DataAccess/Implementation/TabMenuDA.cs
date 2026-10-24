using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Model;
using System.Data;
using System.Data.SqlClient;
using DeepIdentity.Infrastructure;
using System.Data.Entity;

namespace DeepIdentity.DataAccess
{
    public class TabMenuDA
    {

        public IList<TabMenu> GetAllTabMenu()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.TabMenu.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public TabMenu GetTabMenu(string absolutePath)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                            return (from TM in _context.TabMenu
                                           where TM.Page.PageUrl.Url.ToUpper() == absolutePath.ToUpper()
                                           select TM).FirstOrDefault();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public TabMenuPermission GetTabMenuPermission(string absolutePath)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            return _context.TabMenuPermissions.Include("TabMenu").Include("Permission").Where(t => t.ActionURL.ToUpper() == absolutePath.ToUpper()).FirstOrDefault();
        //            return (from TM in _context.TabMenuPermissions
        //                    where TM.ActionURL.ToUpper() == absolutePath.ToUpper()
        //                    select TM).FirstOrDefault();

        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public WebPage GetPage(string absolutePath)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.Pages.Include("PageUrl").Where(t => t.PageUrl.Url.ToUpper() == absolutePath.ToUpper()).FirstOrDefault();


                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PagePermission GetPagePermission(string absolutePath)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.PagePermissions.Include("Url").Include("Permission").Where(t => t.Url.Url.ToUpper() == absolutePath.ToUpper()).FirstOrDefault();
                    

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<TabMenu> GetAllTabMenu(User userInfo)
        {
            List<TabMenu> menus = new List<TabMenu>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (userInfo.Roles != null && userInfo.Roles.Count > 0)
                    {
                        foreach (Role role in userInfo.Roles)
                        {


                            var tabQuery = (from TM in _context.TabMenu.Include("Page").Include("Page.PageUrl")
                                            join AR in _context.AccessRights.Include("PagePermission").Include("PagePermission.Page")
                                            on TM.Page.Id equals AR.PagePermission.Page.Id
                                            join R in _context.Roles
                                            on AR.RoleId equals R.RoleId
                                            where R.RoleId == role.RoleId || TM.Page == null
                                            select TM).Include("Page").Include("Page.PageUrl").Distinct();
                            List<TabMenu> menuList = tabQuery.OrderBy(t=>t.Id).ToList();
                            if (menuList.Count > 0)
                                menus = menus.Union(menuList).ToList();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return menus;
        }
    }
       
       
}
