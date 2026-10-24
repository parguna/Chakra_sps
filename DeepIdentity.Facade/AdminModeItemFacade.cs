using DeepIdentity.DataAccess;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.Facade
{
    public class AdminModeItemFacade
    {
        private AdminModeItemDA preferenceitemDA = null;

        public AdminModeItemFacade()
        {
            preferenceitemDA = new AdminModeItemDA();
        }

        public IList<AdminModeItem> GetAllAdminModeItems()
        {
            try
            {
                return preferenceitemDA.GetAllAdminModeItems();
            }
            catch
            {
                throw;
            }
        }
        public IList<UserPreference> GetDefaultMenus(int RoleId)
        {
            try
            {

                return preferenceitemDA.GetDefaultMenus(RoleId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<UserPreference> GetUserPreferenceMenus(int UserId)
        {
            try
            {

                return preferenceitemDA.GetUserPreferenceMenus(UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeleteUserLinks(int quickLinkId, int UserId)
        {
            try
            {
                preferenceitemDA.DeleteUserLinks(quickLinkId, UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<UserQuickLink> GetUserQuicklinks(int UserId)
        {
            try
            {
                return preferenceitemDA.GetUserQuicklinks(UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SaveUserLinks(UserQuickLink link)
        {
            try
            {
                preferenceitemDA.SaveUserLinks(link);
            }
            catch (Exception)
            {

                throw;
            }
        }
       
       
    }
}
