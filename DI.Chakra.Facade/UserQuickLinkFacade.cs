using DI.Chakra.DataAccess;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using DI.Chakra.Model.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace DI.Chakra.Facade
{
    public class UserQuickLinkFacade
    {
       UserQuickLinkDA QuickLnkDA = new UserQuickLinkDA();

         public IList<QuickLink> GetCategoryAndName(long RoleId)
       {
           return QuickLnkDA.GetCategoryAndName(RoleId);
       }


         public QuickLink GetUserLinks(int menuId)
         {
             return QuickLnkDA.GetUserLinks(menuId);
         }

         public QuickLink Getlinkid(string id)
         {
             return QuickLnkDA.Getlinkid(id);
         }

         public IList<UserQuickLink> SaveLinks(UserQuickLink selectedmenu)
         {
             try
             {
                 QuickLnkDA.SaveLinks(selectedmenu);
                 return null;
             }
             catch (Exception)
             {

                 throw;
             }
         }
    }
}
