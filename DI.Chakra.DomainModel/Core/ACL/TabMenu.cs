using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class TabMenu : DomainModelBase<TabMenu, EntityModel.TabMenu>
    {
        public string UniqueName { get; set; }

        public string MenuName { get; set; }

        public long? ParentTabMenuId { get; set; }

        public long? PageId { get; set; }

        public WebPage WebPage { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string PageUrl
        {
            get
            {
                if (WebPage != null && WebPage.PageUrl != null)
                    return WebPage.PageUrl.Url;
                return string.Empty;
            }
        }

        public static TabMenu GetByPageId(int pageId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.TabMenu>(dataAccessTransaction).Get(tabmenu => tabmenu.PageId.HasValue && tabmenu.PageId.Value == pageId).FirstOrDefault());
            }
        }

        public static bool DoesChildExists(int id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.TabMenu>(dataAccessTransaction).Any(tabmenu => tabmenu.PageId.HasValue && tabmenu.ParentTabMenuId.Value == id);
            }
        }

        public static bool DoesAnyExists(string menuName, string uniqueName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.TabMenu>(dataAccessTransaction).Any(tm => tm.MenuName == menuName || tm.UniqueName == uniqueName);
            }
        }

        public static TabMenu GetByMenuname(string menuName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.TabMenu>(dataAccessTransaction).Get(tm => tm.MenuName == menuName).FirstOrDefault());
            }
        }

        public static Dictionary<long, string> GetAllReportMenu(string filter)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new TabMenuRepository(dataAccessTransaction).GetAllReportMenu(filter);
            }
        }

        public static List<TabMenu> GetByParentTabMenuId(long parentTabMenuId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.TabMenu>(dataAccessTransaction).Get(tm => tm.ParentTabMenuId == parentTabMenuId, tm => tm.WebPage.PageUrl).ToList());
            }
        }

        public static Dictionary<long, string> GetBySearchString(string menuName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new TabMenuRepository(dataAccessTransaction).GetBySearchString(CheckNull.AsteriskToString(menuName));
            }
        }

        public static TabMenu GetByAbsolutePath(string absolutePath)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new TabMenuRepository(dataAccessTransaction).GetTabMenu(absolutePath));
            }
        }

        public static List<TabMenu> GetAllByRoleInfo(Role roleInfo)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new TabMenuRepository(dataAccessTransaction).GetAllTabMenu(roleInfo.ORUnMap()));
            }
        }

        public static List<TabMenu> GetAllByMenuUniqueName(Role roleInfo, string menuUniqueName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new TabMenuRepository(dataAccessTransaction).GetMenu(roleInfo.ORUnMap(), menuUniqueName));
            }
        }

        public static List<TabMenu> GetAllTabMenu(User userInfo)
        {
            List<TabMenu> menus = new List<TabMenu>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (userInfo.Roles != null && userInfo.Roles.Count > 0)
                {
                    foreach (Role role in userInfo.Roles)
                    {
                        List<TabMenu> tabmenus = TabMenu.GetAllByRoleInfo(role);
                        List<TabMenu> menuList = tabmenus.OrderBy(t => t.Id).ToList();
                        if (menuList.Count > 0)
                            menus = menus.Union(menuList).ToList();
                    }
                }
            }
            return menus;
        }

        public static List<TabMenu> GetMenu(long userId, string menuUniqueName)
        {
            User userInfo = User.GetByIdIncludeRoles(userId);
            List<TabMenu> menus = new List<TabMenu>();
            List<TabMenu> ReportsMenu = new List<TabMenu>();
            List<TabMenu> tabmenus = new List<TabMenu>();
            List<TabMenu> menuList = new List<TabMenu>();

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (userInfo.Roles != null && userInfo.Roles.Count > 0)
                {
                    foreach (Role role in userInfo.Roles)
                    {
                        tabmenus = TabMenu.GetAllByMenuUniqueName(role, menuUniqueName);
                        menuList = tabmenus.OrderBy(t => t.Id).ToList();
                        if (menuList.Count > 0)
                            menus = menus.Union(menuList, new IdBaseEqualityComparer<TabMenu>()).ToList();
                    }
                }
                long reportID = (from TM in menus
                                 where TM.UniqueName == menuUniqueName
                                 select TM.Id).FirstOrDefault();

                ReportsMenu = TabMenu.GetByIdList(reportID);

                foreach (TabMenu tmenu in ReportsMenu)
                {
                    TabMenu.GetSubMenus(menus, tmenu.Id, ref ReportsMenu);

                }

                menus = ReportsMenu;
            }
            return menus;
        }

        public static List<TabMenu> GetByIdList(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.TabMenu>(dataAccessTransaction).Get(tm => tm.Id == id).ToList());
            }
        }

        private static void GetSubMenus(List<TabMenu> menus, long parentid, ref List<TabMenu> ReportsMenu)
        {


            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<TabMenu> submenus = new List<TabMenu>();

                foreach (var child in menus)
                {
                    if (child.ParentTabMenuId == parentid)
                    {
                        submenus.Add(child);
                    }
                }

                if (submenus != null && submenus.Count > 0)
                    ReportsMenu = ReportsMenu.Union(submenus).ToList();

                foreach (TabMenu submenu in submenus)
                {
                    GetSubMenus(menus, submenu.Id, ref ReportsMenu);
                }
            }
        }


        public static List<TabMenu> GetTabMenu(long userId)
        {
            User userInfo = User.GetByIdIncludeRoles(userId);
            List<TabMenu> menus = new List<TabMenu>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (userInfo.Roles != null && userInfo.Roles.Count > 0)
                {
                    foreach (Role role in userInfo.Roles)
                    {
                        menus = menus.Union(ORMapList(new TabMenuRepository(dataAccessTransaction).GetTabMenu(role.Id)), new IdBaseEqualityComparer<TabMenu>()).ToList();
                    }
                }
            }
            return menus;
        }
        public static TabMenu GetCategoryById(int menuId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                string parentTabMenuId = TabMenu.GetById(menuId).ParentTabMenuId.ToString();

                if (!string.IsNullOrEmpty(parentTabMenuId))
                    return TabMenu.GetById(Convert.ToInt64(parentTabMenuId));
                else
                    return null;

            }
        }
    }
}

