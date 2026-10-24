using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DataAccess.Implementation
{
    public class AppCustomizationDA
    {

        #region "AdminModeItem"

        //public IList<AdminModeItem> GetAllAdminModeItems()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.AdminModeItems.Include("TabMenu").ToList();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public AdminModeItem GetItemById(int intAdminModeItemID)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return (from item in _context.AdminModeItems
        //                    where item.Id == intAdminModeItemID
        //                    select item).FirstOrDefault();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public bool SaveItems(AdminModeItem itemInfo)
        //{
        //    try
        //    {
        //        int recordsAffected = 0;
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            var perid = oIACMDBContext.AdminModeItems.Where(a => a.Id == itemInfo.Id).Select(a => a.Id).FirstOrDefault();

        //            if (itemInfo.Id == null || itemInfo.Id == 0)
        //            {
        //                oIACMDBContext.AdminModeItems.Add(itemInfo);
        //                recordsAffected = oIACMDBContext.SaveChanges();

        //            }
        //            else
        //            {
        //                itemInfo.Id = perid;
        //                AdminModeItem editper = oIACMDBContext.AdminModeItems.Where(p => p.Id == itemInfo.Id).First();
        //                if (editper == null)
        //                    return false;
        //                oIACMDBContext.Entry(editper).CurrentValues.SetValues(itemInfo);
        //                recordsAffected = oIACMDBContext.SaveChanges();

        //            }

        //        }
        //        if (recordsAffected == 1)
        //            return true;

        //        return false;

        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, ex.Message);
        //        throw ex;
        //    }
        //}

        //public bool DeleteItemById(int intItemID)
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            int recordsAffected = 0;
        //            AdminModeItem delItem = oIACMDBContext.AdminModeItems.FirstOrDefault(Per => Per.Id == intItemID);
        //            if (delItem != null)
        //            {
        //                oIACMDBContext.AdminModeItems.Remove(delItem);
        //                recordsAffected = oIACMDBContext.SaveChanges();
        //            }

        //            if (recordsAffected == 1)
        //                return true;

        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Deleting the Permissions", ex);
        //        throw ex;
        //    }
        //}

        //public bool IsItemExists(string strItemName)
        //{
        //    using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //    {
        //        var dbItem = oIACMDBContext.AdminModeItems.Where(c => c.Name == strItemName).FirstOrDefault();
        //        return dbItem != null ? true : false;
        //    }
        //}

        #endregion

        #region "QuickLink"

        #region "DefaultRoleQuickLink"

        public IList<DefaultRoleQuickLink> GetAllDefaultRoleQLs()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from db in _context.DefaultRoleQuickLink
                                .Include("Role")
                                .Include("QuickLink")
                            orderby db.RoleId, db.Position
                            select db).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DefaultRoleQuickLink GetDefaultRoleQLById(int intDRqlID)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return (from item in _context.DefaultRoleQuickLink.Include("QuickLink").Include("Role")
                        where item.Id == intDRqlID
                        select item).FirstOrDefault();

            }
        }

        public bool SaveItems(DefaultRoleQuickLink qlInfo)
        {
            try
            {
                int recordsAffected = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (qlInfo.Id == 0)
                    {
                        qlInfo.CreatedDate = DateTime.Now;
                        oIACMDBContext.DefaultRoleQuickLink.Add(qlInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        var perid = oIACMDBContext.DefaultRoleQuickLink.Where(a => a.Id == qlInfo.Id).Select(a => a.Id).FirstOrDefault();
                        qlInfo.Id = perid;

                        DefaultRoleQuickLink editper = oIACMDBContext.DefaultRoleQuickLink.Where(p => p.Id == qlInfo.Id).FirstOrDefault();
                        if (editper == null)
                            return false;
                        qlInfo.LastModifiedDate = DateTime.Now;
                        qlInfo.CreatedDate = DateTime.Now;
                        oIACMDBContext.Entry(editper).CurrentValues.SetValues(qlInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }

                }
                if (recordsAffected == 1)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, ex.Message);
                throw ex;
            }
        }

        public bool DeleteDefaultRoleQLById(int intDRqlID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    DefaultRoleQuickLink delItem = oIACMDBContext.DefaultRoleQuickLink.FirstOrDefault(Per => Per.Id == intDRqlID);
                    if (delItem != null)
                    {
                        oIACMDBContext.DefaultRoleQuickLink.Remove(delItem);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Deleting the Derault Role QL", ex);
                throw ex;
            }
        }

        public bool IsQLExistsForRole(long intRoleID, int intQLId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    DefaultRoleQuickLink dbItem = oIACMDBContext.DefaultRoleQuickLink.Where(q => (q.QuickLinkId == intQLId) && (q.RoleId == intRoleID)).FirstOrDefault();
                    return dbItem != null ? true : false;
                }

            }
            catch
            {
                throw;
            }

        }

        public IList<Role> GetAllRoles()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Roles.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region "QuickLink"

        public IList<QuickLink> GetAllQuickLinks()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.QuickLinks.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public QuickLink GetQLById(int intQLID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.QuickLinks
                            where item.Id == intQLID
                            select item).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool SaveItems(QuickLink qlInfo)
        {
            try
            {
                int recordsAffected = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var perid = oIACMDBContext.QuickLinks.Where(a => a.Id == qlInfo.Id).Select(a => a.Id).FirstOrDefault();

                    if (qlInfo.Id == null || qlInfo.Id == 0)
                    {
                        oIACMDBContext.QuickLinks.Add(qlInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        qlInfo.Id = perid;
                        QuickLink editper = oIACMDBContext.QuickLinks.Where(p => p.Id == qlInfo.Id).First();
                        if (editper == null)
                            return false;
                        oIACMDBContext.Entry(editper).CurrentValues.SetValues(qlInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }

                }
                if (recordsAffected == 1)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, ex.Message);
                throw ex;
            }
        }

        public bool DeleteQLById(int intQLID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    QuickLink delItem = oIACMDBContext.QuickLinks.FirstOrDefault(Per => Per.Id == intQLID);
                    if (delItem != null)
                    {
                        oIACMDBContext.QuickLinks.Remove(delItem);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Deleting the Quick Link", ex);
                throw ex;
            }
        }

        public bool IsQLExists(string QLName)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                var dbItem = oIACMDBContext.QuickLinks.Where(c => c.Name == QLName).FirstOrDefault();
                return dbItem != null ? true : false;
            }
        }

        public bool IsQLPositionExists(int dbPosition, int roleID)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                var dbItem = oIACMDBContext.DefaultRoleQuickLink.Where(c => (c.Position == dbPosition) && (c.RoleId == roleID)).FirstOrDefault();
                return dbItem != null ? true : false;
            }
        }

        public List<string> GetAllQLCatagories()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.QuickLinks.Where(m=>m.Category!=null).Select(m => m.Category).Distinct().ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<string> GetQLCatagoriesByRoleID(int intRoleID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var db = from item in _context.QuickLinks
                             join ar in _context.QuickLinkAccessRights on item.Id equals ar.QuickLinkId
                             where ar.RoleId == intRoleID
                             select item.Id;

                    return (from d in _context.QuickLinks where db.Contains(d.Id) select d.Category).Distinct().ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<QuickLink> GetAllQLNamesByCatagory(string strCategoryName)
        {

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.QuickLinks.Where(m => m.Category == strCategoryName).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region "QL Access Rights"

        public IList<QuickLinkAccessRights> GetAllQLAccessRights()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from db in _context.QuickLinkAccessRights
                            .Include("Role")
                            .Include("QuickLink")
                            orderby db.RoleId
                            select db).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public QuickLinkAccessRights GetQLRightsById(int intqlARID)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return (from item in _context.QuickLinkAccessRights.Include("QuickLink").Include("Role")
                        where item.Id == intqlARID
                        select item).FirstOrDefault();
            }
        }

        public bool IsQLExistsForAccessRights(long intRoleID, int intQLId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    QuickLinkAccessRights dbItem = oIACMDBContext.QuickLinkAccessRights.Where(d => (d.QuickLinkId == intQLId) && (d.RoleId == intRoleID)).FirstOrDefault();
                    return dbItem != null ? true : false;
                }
            }
            catch (Exception ex)
            { throw ex; }

        }

        public bool SaveQLAccessRights(QuickLinkAccessRights qlInfo)
        {
            try
            {
                int recordsAffected = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var perid = oIACMDBContext.QuickLinkAccessRights.Where(a => a.Id == qlInfo.Id).Select(a => a.Id).FirstOrDefault();

                    if (qlInfo.Id == 0)
                    {
                        qlInfo.CreatedDate = DateTime.Now;
                        oIACMDBContext.QuickLinkAccessRights.Add(qlInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        qlInfo.Id = perid;
                        QuickLinkAccessRights editper = oIACMDBContext.QuickLinkAccessRights.Where(p => p.Id == qlInfo.Id).First();
                        if (editper == null)
                            return false;
                        qlInfo.LastModifiedDate = DateTime.Now;
                        qlInfo.CreatedDate = editper.CreatedDate;
                        oIACMDBContext.Entry(editper).CurrentValues.SetValues(qlInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }

                }
                if (recordsAffected == 1)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, ex.Message);
                throw ex;
            }
        }

        public bool DeleteQLAccessRightsById(int intqlARID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;

                    QuickLinkAccessRights delItem = oIACMDBContext.QuickLinkAccessRights.FirstOrDefault(Per => Per.Id == intqlARID);

                    DefaultRoleQuickLink drql = oIACMDBContext.DefaultRoleQuickLink.FirstOrDefault(Per => (Per.RoleId == delItem.RoleId) && (Per.QuickLinkId == delItem.QuickLinkId));
                    if (drql != null)
                    {
                        oIACMDBContext.DefaultRoleQuickLink.Remove(drql);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (delItem != null)
                    {
                        oIACMDBContext.QuickLinkAccessRights.Remove(delItem);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Deleting the Quick Link  Access Rights", ex);
                throw ex;
            }

        }

        #endregion

        #endregion

        #region "DashBoard"

        #region "Default Role DashBoard"

        public IList<DefaultRoleDashboardPreference> GetAllDefaultRoleDBs()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return (from db in _context.DefaultRoleDashboardPreferences
                                .Include("Role")
                                .Include("Dashboard")
                            orderby db.RoleId, db.Position
                            select db).ToList();
                }


            }
            catch (Exception)
            {
                throw;
            }
        }

        public DefaultRoleDashboardPreference GetDefaultRoleDBById(int intDRdbID)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return (from item in _context.DefaultRoleDashboardPreferences.Include("Dashboard").Include("Role")
                        where item.Id == intDRdbID
                        select item).FirstOrDefault();

            }
        }

        public bool SaveDefaultRoleDB(DefaultRoleDashboardPreference dbInfo)
        {
            try
            {
                int recordsAffected = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (dbInfo.Id == 0)
                    {
                        dbInfo.CreatedDate = DateTime.Now;
                        oIACMDBContext.DefaultRoleDashboardPreferences.Add(dbInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        var perid = oIACMDBContext.DefaultRoleDashboardPreferences.Where(a => a.Id == dbInfo.Id).Select(a => a.Id).FirstOrDefault();
                        dbInfo.Id = perid;

                        DefaultRoleDashboardPreference editper = oIACMDBContext.DefaultRoleDashboardPreferences.Where(p => p.Id == dbInfo.Id).First();
                        if (editper == null)
                            return false;
                        dbInfo.LastModifiedDate = DateTime.Now;
                        dbInfo.CreatedDate = editper.CreatedDate;
                        oIACMDBContext.Entry(editper).CurrentValues.SetValues(dbInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }

                }
                if (recordsAffected == 1)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, ex.Message);
                throw ex;
            }
        }

        public bool DeleteDefaultRoleDBById(int intDRdbID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    DefaultRoleDashboardPreference delItem = oIACMDBContext.DefaultRoleDashboardPreferences.FirstOrDefault(Per => Per.Id == intDRdbID);
                    if (delItem != null)
                    {
                        oIACMDBContext.DefaultRoleDashboardPreferences.Remove(delItem);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Deleting the Derault Role Dashboard", ex);
                throw ex;
            }
        }

        public bool IsDBExistsForRole(long intRoleID, int intDBId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    DefaultRoleDashboardPreference dbItem = oIACMDBContext.DefaultRoleDashboardPreferences.Where(d => (d.DashBoardId == intDBId) && (d.RoleId == intRoleID)).FirstOrDefault();
                    return dbItem != null ? true : false;
                }

            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region "DashBoard"

        public IList<Dashboard> GetAllDashBoards()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Dashboards.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dashboard GetDBById(int intDBID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.Dashboards
                            where item.Id == intDBID
                            select item).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool SaveDashBoard(Dashboard dbInfo)
        {
            try
            {
                int recordsAffected = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var perid = oIACMDBContext.Dashboards.Where(a => a.Id == dbInfo.Id).Select(a => a.Id).FirstOrDefault();

                    if (dbInfo.Id == null || dbInfo.Id == 0)
                    {
                        oIACMDBContext.Dashboards.Add(dbInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        dbInfo.Id = perid;
                        Dashboard editper = oIACMDBContext.Dashboards.Where(p => p.Id == dbInfo.Id).First();
                        if (editper == null)
                            return false;
                        oIACMDBContext.Entry(editper).CurrentValues.SetValues(dbInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }

                }
                if (recordsAffected == 1)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, ex.Message);
                throw ex;
            }
        }

        public bool DeleteDBById(int intDBID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    Dashboard delItem = oIACMDBContext.Dashboards.FirstOrDefault(Per => Per.Id == intDBID);
                    if (delItem != null)
                    {
                        oIACMDBContext.Dashboards.Remove(delItem);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Deleting the Dashboard", ex);
                throw ex;
            }
        }

        public bool IsDBExists(long intDBID)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                var dbItem = oIACMDBContext.Dashboards.Where(c => c.Id == intDBID).FirstOrDefault();
                return dbItem != null ? true : false;
            }
        }

        public bool IsDBPositionExists(int dbPosition, int roleID)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                var dbItem = oIACMDBContext.DefaultRoleDashboardPreferences.Where(c => (c.Position == dbPosition) && (c.RoleId == roleID)).FirstOrDefault();
                return dbItem != null ? true : false;
            }
        }

        public IList<string> GetAllDBCatagories()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Dashboards.Select(m => m.DashboardCategory).Distinct().ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<string> GetDBCatagoriesByRoleID(int intRoleID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var db = from item in _context.Dashboards
                             join ar in _context.DashboardAccessRights on item.Id equals ar.DashboardId
                             where ar.RoleId == intRoleID
                             select item.Id;

                    return (from d in _context.Dashboards where db.Contains(d.Id) select d.DashboardCategory).Distinct().ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<Dashboard> GetAllDBNamesByCatagory(string strCategoryName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from db in _context.Dashboards
                            where db.DashboardCategory == strCategoryName
                            select db).ToList();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region "DashBoard AccessRights"

        public IList<DashboardAccessRights> GetAllDBAccessRights()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from db in _context.DashboardAccessRights
                              .Include("Role")
                              .Include("Dashboard")
                            orderby db.RoleId
                            select db).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DashboardAccessRights GetDBRightsById(int intdbARID)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return (from item in _context.DashboardAccessRights.Include("Dashboard").Include("Role")
                        where item.Id == intdbARID
                        select item).FirstOrDefault();
            }
        }

        public bool IsDBExistsForAccessRights(long intRoleID, int intdbId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    DashboardAccessRights dbItem = oIACMDBContext.DashboardAccessRights.Where(d => (d.DashboardId == intdbId) && (d.RoleId == intRoleID)).FirstOrDefault();
                    return dbItem != null ? true : false;
                }
            }
            catch { throw; }
        }

        public bool SaveDBAccessRights(DashboardAccessRights itemInfo)
        {
            try
            {
                int recordsAffected = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var perid = oIACMDBContext.DashboardAccessRights.Where(a => a.Id == itemInfo.Id).Select(a => a.Id).FirstOrDefault();

                    if (itemInfo.Id == 0)
                    {
                        itemInfo.CreatedDate = DateTime.Now;
                        oIACMDBContext.DashboardAccessRights.Add(itemInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        itemInfo.Id = perid;
                        DashboardAccessRights editper = oIACMDBContext.DashboardAccessRights.Where(p => p.Id == itemInfo.Id).First();
                        if (editper == null)
                            return false;
                        itemInfo.LastModifiedDate = DateTime.Now;
                        itemInfo.CreatedDate = editper.CreatedDate;
                        oIACMDBContext.Entry(editper).CurrentValues.SetValues(itemInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }

                }
                if (recordsAffected == 1)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, ex.Message);
                throw ex;
            }
        }

        public bool DeleteDBAccessRightsById(int intdbARID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;

                    DashboardAccessRights delItem = oIACMDBContext.DashboardAccessRights.FirstOrDefault(Per => Per.Id == intdbARID);

                    DefaultRoleDashboardPreference drdb = oIACMDBContext.DefaultRoleDashboardPreferences.FirstOrDefault(Per => (Per.RoleId == delItem.RoleId) && (Per.DashBoardId == delItem.DashboardId));
                    if (drdb != null)
                    {
                        oIACMDBContext.DefaultRoleDashboardPreferences.Remove(drdb);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (delItem != null)
                    {
                        oIACMDBContext.DashboardAccessRights.Remove(delItem);
                        recordsAffected = oIACMDBContext.SaveChanges();
                    }

                    if (recordsAffected == 1)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Deleting the Dashboard Access Rights", ex);
                throw ex;
            }
        }

        #endregion

        #endregion

    }
}
