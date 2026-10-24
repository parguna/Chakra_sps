using DI.Chakra.DataAccess;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Facade
{
    public class AdminModeItemFacade
    {
        private AdminModeItemDA preferenceitemDA = null;

        public AdminModeItemFacade()
        {
            preferenceitemDA = new AdminModeItemDA();
        }

        //public IList<AdminModeItem> GetAllAdminModeItems()
        //{
        //    try
        //    {
        //        return preferenceitemDA.GetAllAdminModeItems();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        public IList<UserPreference> GetDefaultMenus(long RoleId)
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

        public IList<UserPreference> GetUserPreferenceMenus(long UserId)
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

        public void DeleteUserLinks(int quickLinkId, long UserId)
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
        public IList<UserQuickLink> GetUserQuicklinks(long UserId)
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
