using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DI.Chakra.DataAccess
{
    public class AdminModeItemDA
    {
        public IACMDbOperator m_dbOperator = null;
        private IACMDBContext oIACMDBContext = new IACMDBContext();

        //public IList<AdminModeItem> GetAllAdminModeItems()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            var Items = _context.AdminModeItems.Include("TabMenu.Page.PageUrl").ToList();
        //            foreach (var item in Items)
        //            {
        //                try
        //                {
        //                     var results = _context.Set(item.TableName)
        //                                        .AsQueryable().ToListAsync();
        //                     item.Number = results.Result.Count();
        //                   // item.Number = _context.Database.SqlQuery<int>("select count(1) from " + item.TableName).First();
        //                }
        //                catch
        //                {
        //                    item.Number = 0;
        //                }
        //            }
        //            return Items;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public IList<UserPreference> GetDefaultMenus(long RoleId)
        {
            IList<UserPreference> menuList = new List<UserPreference>();
            try
            {
                if (RoleId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        var query = from drp in oIACMDBContext.DefaultRoleQuickLink
                                    join r in oIACMDBContext.Roles
                                on drp.RoleId equals r.Id
                                    join TM in oIACMDBContext.TabMenu.Include("Page").Include("PageUrl")
                                on drp.QuickLinkId equals TM.Id
                                    where drp.RoleId == RoleId
                                    orderby drp.Position
                                    select new UserPreference
                                    {
                                        MenuName = TM.MenuName,
                                        TabMenuId = TM.Id,
                                        TargetUrl = TM.Page.PageUrl.Url,
                                        Position = drp.Position
                                    };

                        menuList = query.ToList();
                    }
                }
                return menuList;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Geting GetUserDefaultPreferenceMenus");
                throw;
            }
        }

        public IList<UserPreference> GetUserPreferenceMenus(long UserId)
        {

            IList<UserPreference> menuList = new List<UserPreference>();
            try
            {
                if (UserId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        var query = from UQ in oIACMDBContext.UserQuickLinks
                                    join TM in oIACMDBContext.TabMenu.Include("Page").Include("PageUrl")
                                on UQ.QuickLinkId equals TM.Id
                                    where UQ.UserId == UserId
                                    orderby UQ.Position
                                    select new UserPreference
                                    {
                                        MenuName = TM.MenuName,
                                        TabMenuId = TM.Id,
                                        TargetUrl = TM.Page.PageUrl.Url,
                                        Position = UQ.Position
                                    };

                        menuList = query.ToList();
                    }
                }
                return menuList;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Geting GetUserPreferenceMenus");
                throw;
            }
        }

        public void DeleteUserLinks(int quickLinkId, long UserId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (UserId != 0)
                    {
                        UserQuickLink link = oIACMDBContext.UserQuickLinks.Where(UQ => UQ.QuickLinkId == quickLinkId && UQ.UserId == UserId).FirstOrDefault();
                        oIACMDBContext.UserQuickLinks.Remove(link);
                        oIACMDBContext.SaveChanges();
                    }

                }

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, ex.Message);
            }
        }
        public IList<UserQuickLink> GetUserQuicklinks(long UserId)
        {
            IList<UserQuickLink> menuList = new List<UserQuickLink>();
            try
            {
                if (UserId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        menuList = oIACMDBContext.UserQuickLinks.Where(UQ => UQ.UserId == UserId).OrderBy(UQ => UQ.Position).ToList();
                    }
                }
                return menuList;
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Geting GetUserQuicklinks");
                throw;
            }
        }

        public void SaveUserLinks(UserQuickLink link)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (link.Id == 0)
                    {
                        oIACMDBContext.UserQuickLinks.Add(link);
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        UserQuickLink objLinks = new UserQuickLink();
                        objLinks = oIACMDBContext.UserQuickLinks.Find(link.Id);
                        oIACMDBContext.Entry(objLinks).CurrentValues.SetValues(link);
                        oIACMDBContext.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, ex.Message);
            }
        }


    }
}
