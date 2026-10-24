using DeepIdentity.DataAccess;
using DeepIdentity.DataAccess.Implementation;
using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.Core;
using DeepIdentity.Model.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace DeepIdentity.Facade
{
  public  class CulturePreferenceFacade
    {

      CulturePreferenceDA culturepreferenceDA = new CulturePreferenceDA();


      public List<UserCulturePreference> GetUserPreference(int UserId)
      {
          try
          {
              return culturepreferenceDA.GetUserPreference(UserId);
          }
          catch (Exception)
          {
            throw;
          }
      }

      public void SaveUserCulturepreference(UserCulturePreference Culturepreference)
      {
          try
          {
              culturepreferenceDA.SaveUserCulturepreference(Culturepreference);
          }
          catch (Exception)
          {

              throw;
          }
      }



    }
}
