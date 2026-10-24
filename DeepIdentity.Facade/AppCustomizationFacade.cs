using DeepIdentity.DataAccess.Implementation;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.Facade
{
    public class AppCustomizationFacade
    {
        AppCustomizationDA custDA = new AppCustomizationDA();

        #region "AdminModeItem"

        public IList<AdminModeItem> GetAllAdminModeItems()
        {
            return custDA.GetAllAdminModeItems();
        }

        public AdminModeItem GetItemById(int intAdminModeItemID)
        {
            return custDA.GetItemById(intAdminModeItemID);
        }

        public bool SaveItems(AdminModeItem itemInfo)
        {
            return custDA.SaveItems(itemInfo);
        }

        public bool DeleteItemById(int intItemID)
        {
            return custDA.DeleteItemById(intItemID);
        }

        public bool IsItemExists(string strItemName)
        {
            return custDA.IsItemExists(strItemName);
        }

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

        public bool IsQLExistsForRole(int intRoleID,string strCategory, string strName, out int intQLId)
        {
            return custDA.IsQLExistsForRole(intRoleID,strCategory,strName,out intQLId);
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

        public bool IsQLExists(int intQLID)
        {
            return custDA.IsQLExists(intQLID);
        }

        public IList<string> GetAllQLCatagories()
        {
            return custDA.GetAllQLCatagories();
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

        public bool IsQLExistsForAccessRights(int intRoleID, string strCategory, string strName, out int intQLId)
        {
            return custDA.IsQLExistsForAccessRights(intRoleID, strCategory, strName, out intQLId);
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
        #endregion



        
    }
}
