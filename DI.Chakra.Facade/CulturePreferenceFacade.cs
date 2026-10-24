using DI.Chakra.DataAccess;
using DI.Chakra.DataAccess.Implementation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using DI.Chakra.Model.Core;
using DI.Chakra.Model.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace DI.Chakra.Facade
{
    public class CulturePreferenceFacade
    {

        CulturePreferenceDA culturepreferenceDA = new CulturePreferenceDA();


        //public List<UserCulturePreference> GetUserPreference(int UserId)
        //{
        //    try
        //    {
        //        return culturepreferenceDA.GetUserPreference(UserId);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public bool SaveUserCulturepreference(UserCulturePreference Culturepreference)
        //{
        //    try
        //    {
        //        return culturepreferenceDA.SaveUserCulturepreference(Culturepreference);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}



    }
}
