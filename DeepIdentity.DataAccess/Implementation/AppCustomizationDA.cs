using DeepIdentity.Infrastructure;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.DataAccess.Implementation
{
    public class AppCustomizationDA
    {

        #region "AdminModeItem"

        public IList<AdminModeItem> GetAllAdminModeItems()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AdminModeItems.Include("TabMenu").ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AdminModeItem GetItemById(int intAdminModeItemID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.AdminModeItems
                            where item.Id == intAdminModeItemID
                            select item).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool SaveItems(AdminModeItem itemInfo)
        {
            try
            {
                int recordsAffected = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var perid = oIACMDBContext.AdminModeItems.Where(a => a.Id == itemInfo.Id).Select(a => a.Id).FirstOrDefault();

                    if (itemInfo.Id == null || itemInfo.Id == 0)
                    {
                        oIACMDBContext.AdminModeItems.Add(itemInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        itemInfo.Id = perid;
                        AdminModeItem editper = oIACMDBContext.AdminModeItems.Where(p => p.Id == itemInfo.Id).First();
                        if (editper == null)
                            return false;
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
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
                throw ex;
            }
        }

        public bool DeleteItemById(int intItemID)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    int recordsAffected = 0;
                    AdminModeItem delItem = oIACMDBContext.AdminModeItems.FirstOrDefault(Per => Per.Id == intItemID);
                    if (delItem != null)
                    {
                        oIACMDBContext.AdminModeItems.Remove(delItem);
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

        public bool IsItemExists(string strItemName)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                var dbItem = oIACMDBContext.AdminModeItems.Where(c => c.Name == strItemName).FirstOrDefault();
                return dbItem != null ? true : false;
            }
        }

        #endregion

        #region "QuickLink"

        #region "DefaultRoleQuickLink"

        public IList<DefaultRoleQuickLink> GetAllDefaultRoleQLs()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.DefaultRoleQuickLink.Include("Role").Include("QuickLink").ToList();
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
                    var perid = oIACMDBContext.DefaultRoleQuickLink.Where(a => a.Id == qlInfo.Id).Select(a => a.Id).FirstOrDefault();

                    if (qlInfo.Id == 0)
                    {
                        oIACMDBContext.DefaultRoleQuickLink.Add(qlInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        qlInfo.Id = perid;
                        DefaultRoleQuickLink editper = oIACMDBContext.DefaultRoleQuickLink.Where(p => p.Id == qlInfo.Id).First();
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
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
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
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured While Deleting the Derault Role QL", ex);
                throw ex;
            }
        }

        public bool IsQLExistsForRole(int intRoleID, string strCategory,string strName,out int intQLId)
        {
            try
            {
                intQLId = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    QuickLink qlItem = oIACMDBContext.QuickLinks.Where(c => (c.Name == strName) && c.Category == strCategory).FirstOrDefault();

                    DefaultRoleQuickLink dbItem = oIACMDBContext.DefaultRoleQuickLink.Where(d => (d.Id == qlItem.Id) && (d.RoleId == intRoleID)).FirstOrDefault();

                    if (dbItem != null)
                        intQLId = dbItem.Id;

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
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
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
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured While Deleting the Quick Link", ex);
                throw ex;
            }
        }

        public bool IsQLExists(int intQLID)
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                var dbItem = oIACMDBContext.QuickLinks.Where(c => c.QuickLinkId == intQLID).FirstOrDefault();
                return dbItem != null ? true : false;
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
                    return _context.QuickLinkAccessRights.Include("Role").Include("QuickLink").ToList();
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

        public bool IsQLExistsForAccessRights(int intRoleID, string strCategory, string strName, out int intQLId)
        {
            try {
                intQLId = 0;
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    QuickLink qlItem = oIACMDBContext.QuickLinks.Where(c => (c.Name == strName) && c.Category == strCategory).FirstOrDefault();

                    QuickLinkAccessRights dbItem = oIACMDBContext.QuickLinkAccessRights.Where(d => (d.QuickLinkId == qlItem.Id) && (d.RoleId == intRoleID)).FirstOrDefault();

                    if (dbItem != null)
                        intQLId = dbItem.Id;

                    return dbItem != null ? true : false;
                }
            }
            catch { throw; }
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
                        oIACMDBContext.QuickLinkAccessRights.Add(qlInfo);
                        recordsAffected = oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        qlInfo.Id = perid;
                        QuickLinkAccessRights editper = oIACMDBContext.QuickLinkAccessRights.Where(p => p.Id == qlInfo.Id).First();
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
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
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
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured While Deleting the Quick Link Access Rights", ex);
                throw ex;
            }
        }

        #endregion

        #endregion

        #region "DashBoard"
        #endregion

        public List<string> GetAllQLCatagories()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.QuickLinks.Select(m => m.Category).Distinct().ToList();
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

    }
}
