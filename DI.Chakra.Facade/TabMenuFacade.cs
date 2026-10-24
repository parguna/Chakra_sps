using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using DI.Chakra.Infrastructure;
using System.Xml;
using System.Xml.XPath;
using DI.Chakra.DataAccess;

namespace DI.Chakra.Facade
{
    public class TabMenuFacade
    {
        #region private Members
        private TabMenuDA tabMenuDA = null;
        #endregion


        public TabMenuFacade()
        {
            tabMenuDA = new TabMenuDA();
        }


        public IList<TabMenu> GetAllTabMenu()
        {
            try
            {
               return tabMenuDA.GetAllTabMenu();
            }
            catch  
            {
                throw;
            }
        }
        
        public TabMenu GetTabMenu(string absolutePath)
        {
            try
            {
                return tabMenuDA.GetTabMenu(absolutePath);
            }
            catch
            {
                throw;
            }
        }
        //public TabMenuPermission GetTabMenuPermission(string absolutePath)
        //{
        //    try
        //    {
        //        return tabMenuDA.GetTabMenuPermission(absolutePath);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        public WebPage GetPage(string absolutePath)
        {
            try
            {
                return tabMenuDA.GetPage(absolutePath);
            }
            catch
            {
                throw;
            }
        }
        public PagePermission GetPagePermission(string absolutePath)
        {
            try
            {
                return tabMenuDA.GetPagePermission(absolutePath);
            }
            catch
            {
                throw;
            }
        }
        public IList<TabMenu> GetAllTabMenu(User userInfo)
        {
            try
            {
                return tabMenuDA.GetAllTabMenu(userInfo);
            }
            catch
            {
                throw;
            }
        }
        public IList<TabMenu> GetMenu(User userInfo,string menuUniqueName)
        {
            try
            {
                return tabMenuDA.GetMenu(userInfo, menuUniqueName);
            }
            catch
            {
                throw;
            }
        }
        
    }
}
