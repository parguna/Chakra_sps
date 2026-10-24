using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class UserPreferenceRepository : RepositoryBase<UserPreference>
    {
        public UserPreferenceRepository(DataAccessTransaction transaction)
            : base(transaction)
        {
        }

        UserPreference GetDefaultMenu()
        {
            return new UserPreference { Id = 0, MenuName = "New Request", TabMenuId = 0, Position = 0, TargetUrl = "/NACR/CreateCart.aspx" };
        }

        public IList<UserPreference> GetDefaultMenus(long roleId)
        {
            var menus = (from drq in CurrentContext.DefaultRoleQuickLink
                         join ql in CurrentContext.QuickLinks on drq.QuickLinkId equals ql.Id
                         join tm in CurrentContext.TabMenu.Include("Page").Include("PageUrl") on ql.QuickLinkId equals tm.Id
                         join r in CurrentContext.Roles on drq.RoleId equals r.Id
                         where drq.RoleId == roleId
                         orderby drq.Position
                         select new UserPreference
                         {
                             Id = drq.QuickLinkId,
                             MenuName = tm.MenuName,
                             TabMenuId = tm.Id,
                             TargetUrl = tm.WebPage.PageUrl.Url,
                             Position = drq.Position
                         }).ToList();

            if (menus == null)
                menus = new List<UserPreference>();

            var def = GetDefaultMenu();
            if (def != null && !menus.Any(p => p.TabMenuId == def.TabMenuId))
                menus.Add(def);

            return menus;
        }

        public IList<UserPreference> GetUserPreferenceMenus(long userId)
        {
            var menus = (from uq in CurrentContext.UserQuickLinks
                         join ql in CurrentContext.QuickLinks on uq.QuickLinkId equals ql.Id
                         join tm in CurrentContext.TabMenu.Include("Page").Include("PageUrl") on ql.QuickLinkId equals tm.Id
                         where uq.UserId == userId
                         orderby uq.Position
                         select new UserPreference
                         {
                             Id = uq.QuickLinkId,
                             MenuName = tm.MenuName,
                             TabMenuId = tm.Id,
                             TargetUrl = tm.WebPage.PageUrl.Url,
                             Position = uq.Position
                         }).ToList();

            if (menus == null)
                menus = new List<UserPreference>();

            var def = GetDefaultMenu();
            if (def != null && !menus.Any(p => p.TabMenuId == def.TabMenuId))
                menus.Add(def);

            return menus;
        }
    }
}

