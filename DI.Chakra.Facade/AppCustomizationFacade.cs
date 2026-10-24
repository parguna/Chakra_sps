using DI.Chakra.DataAccess.Implementation;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Facade
{
    public class AppCustomizationFacade
    {
        AppCustomizationDA custDA = new AppCustomizationDA();

        #region "AdminModeItem"

        //public IList<AdminModeItem> GetAllAdminModeItems()
        //{
        //    return custDA.GetAllAdminModeItems();
        //}

        //public AdminModeItem GetItemById(int intAdminModeItemID)
        //{
        //    return custDA.GetItemById(intAdminModeItemID);
        //}

        //public bool SaveItems(AdminModeItem itemInfo)
        //{
        //    return custDA.SaveItems(itemInfo);
        //}

        //public bool DeleteItemById(int intItemID)
        //{
        //    return custDA.DeleteItemById(intItemID);
        //}

        //public bool IsItemExists(string strItemName)
        //{
        //    return custDA.IsItemExists(strItemName);
        //}

        #endregion

        #region "QuickLink"

        #region "DefaultRoleQuickLink"

        public IList<DefaultRoleQuickLink> GetAllDefaultRoleQLs()
        {
            return custDA.GetAllDefaultRoleQLs();
        }

        public DefaultRoleQuickLink GetDefaultRoleQLById(int intDRqlID)
        {
            return custDA.GetDefaultRoleQLById(intDRqlID);
        }

        public bool SaveItems(DefaultRoleQuickLink qlInfo)
        {
            return custDA.SaveItems(qlInfo);
        }

        public bool DeleteDefaultRoleQLById(int intDRqlID)
        {
            return custDA.DeleteDefaultRoleQLById(intDRqlID);
        }

        public bool IsQLExistsForRole(long intRoleID, int intQLId)
        {
            return custDA.IsQLExistsForRole(intRoleID, intQLId);
        }

        public IList<Role> GetAllRoles()
        {
            return custDA.GetAllRoles();
        }
        #endregion

        #region "QuickLink"

        public IList<QuickLink> GetAllQuickLinks()
        {
            return custDA.GetAllQuickLinks();
        }

        public QuickLink GetQLById(int intQLID)
        {
            return custDA.GetQLById(intQLID);
        }

        public bool SaveItems(QuickLink qlInfo)
        {
            return custDA.SaveItems(qlInfo);
        }

        public bool DeleteQLById(int intQLID)
        {
            return custDA.DeleteQLById(intQLID);
        }

        public bool IsQLExists(string QLName)
        {
            return custDA.IsQLExists(QLName);
        }

        public bool IsQLPositionExists(int dbPosition, int roleID)
        {
            return custDA.IsQLPositionExists(dbPosition, roleID);
        }

        public IList<string> GetAllQLCatagories()
        {
            return custDA.GetAllQLCatagories();
        }

        public IList<string> GetQLCatagoriesByRoleID(int roleid)
        {
            return custDA.GetQLCatagoriesByRoleID(roleid);
        }

        public IList<QuickLink> GetAllQLNamesByCatagory(string strCategoryName)
        {
            return custDA.GetAllQLNamesByCatagory(strCategoryName);
        }

        #endregion

        #region "QL Access Rights"

        public IList<QuickLinkAccessRights> GetAllQLAccessRights()
        {
            return custDA.GetAllQLAccessRights();
        }

        public QuickLinkAccessRights GetQLRightsById(int intqlARID)
        {
            return custDA.GetQLRightsById(intqlARID);
        }

        public bool IsQLExistsForAccessRights(long intRoleID, int intQLId)
        {
            return custDA.IsQLExistsForAccessRights(intRoleID, intQLId);
        }

        public bool SaveQLAccessRights(QuickLinkAccessRights itemInfo)
        {
            return custDA.SaveQLAccessRights(itemInfo);
        }

        public bool DeleteQLAccessRightsById(int intqlARID)
        {
            return custDA.DeleteQLAccessRightsById(intqlARID);
        }

        #endregion

        #endregion

        #region "DashBoard"

        #region "DefaultRoleDashBoard"

        public IList<DefaultRoleDashboardPreference> GetAllDefaultRoleDBs()
        {
            return custDA.GetAllDefaultRoleDBs();
        }

        public DefaultRoleDashboardPreference GetDefaultRoleDBById(int intDRdbID)
        {
            return custDA.GetDefaultRoleDBById(intDRdbID);
        }

        public bool SaveDefaultRoleDB(DefaultRoleDashboardPreference dbInfo)
        {
            return custDA.SaveDefaultRoleDB(dbInfo);
        }

        public bool DeleteDefaultRoleDBById(int intDRdbID)
        {
            return custDA.DeleteDefaultRoleDBById(intDRdbID);
        }

        public bool IsDBExistsForRole(long intRoleID, int intDBId)
        {
            return custDA.IsDBExistsForRole(intRoleID, intDBId);
        }

        #endregion

        #region "DashBoard"

        public IList<Dashboard> GetAllDashBoards()
        {
            return custDA.GetAllDashBoards();
        }

        public Dashboard GetDBById(int intDBID)
        {
            return custDA.GetDBById(intDBID);
        }

        public bool SaveDashBoard(Dashboard dbInfo)
        {
            return custDA.SaveDashBoard(dbInfo);
        }

        public bool DeleteDBById(int intDBID)
        {
            return custDA.DeleteDBById(intDBID);
        }

        public bool IsDBExists(long intDBID)
        {
            return custDA.IsDBExists(intDBID);
        }

        public bool IsDBPositionExists(int dbPosition, int roleID)
        {
            return custDA.IsDBPositionExists(dbPosition, roleID);
        }

        public IList<string> GetAllDBCatagories()
        {
            return custDA.GetAllDBCatagories();
        }

        public IList<string> GetDBCatagoriesByRoleID(int roleid)
        {
            return custDA.GetDBCatagoriesByRoleID(roleid);
        }

        public IList<Dashboard> GetAllDBNamesByCatagory(string strCategoryName)
        {
            return custDA.GetAllDBNamesByCatagory(strCategoryName);
        }

        #endregion

        #region "DashBoard AccessRights"

        public IList<DashboardAccessRights> GetAllDBAccessRights()
        {
            return custDA.GetAllDBAccessRights();
        }

        public DashboardAccessRights GetDBRightsById(int intdbARID)
        {
            return custDA.GetDBRightsById(intdbARID);
        }

        public bool IsDBExistsForAccessRights(long intRoleID, int intdbId)
        {
            return custDA.IsDBExistsForAccessRights(intRoleID, intdbId);
        }

        public bool SaveDBAccessRights(DashboardAccessRights itemInfo)
        {
            return custDA.SaveDBAccessRights(itemInfo);
        }

        public bool DeleteDBAccessRightsById(int intdbARID)
        {
            return custDA.DeleteDBAccessRightsById(intdbARID);
        }

        #endregion

        #endregion

    }
}
