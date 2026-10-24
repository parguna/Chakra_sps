using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace DeepIdentity.Facade
{
    public class UserQuickLinkFacade
    {
       UserQuickLinkDA QuickLnkDA = new UserQuickLinkDA();

         public IList<QuickLink> GetCategoryAndName(int RoleId)
       {
           return QuickLnkDA.GetCategoryAndName(RoleId);
       }

    }
}
