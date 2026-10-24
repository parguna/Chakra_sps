using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Model;
using DI.Chakra.Model.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace DI.Chakra.DataAccess.Implementation
{
    public class CulturePreferenceDA
    {

        //public List<UserCulturePreference> GetUserPreference(int UserId)
        //{    
        //    try
        //    {
        //        List<UserCulturePreference> preferencelist = new List<UserCulturePreference>();
        //        if (UserId != -1)
        //        {
        //            using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //            {
        //                preferencelist = oIACMDBContext.UserCulturePreferences.Where(lq => lq.UserId == UserId).ToList();
        //            }
        //        }
        //        return preferencelist;
        //    }
                
        //    catch (Exception ex)
        //    {
        //       DILog.GeneralLogger.WriteLog(LogLevelType.Info,"Error Occured while Geting GetUserCulturePreference");
        //        throw;          
        //    }
           
        //}


        
        //public  bool SaveUserCulturepreference(UserCulturePreference Culturepreference)
        //{
        //    try
        //    {
        //        int recordsAffected = 0;
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            //bool isUserExists = oIACMDBContext.UserCulturePreferences.Any(u => u.UserId == Culturepreference.UserId);
        //            var culturepreferenceID = oIACMDBContext.UserCulturePreferences.Where(ucp => ucp.Id == Culturepreference.Id).Select(ucp => ucp.Id).FirstOrDefault();

        //            if (Culturepreference.Id == null || Culturepreference.Id  == 0)
        //            {
        //                oIACMDBContext.UserCulturePreferences.Add(Culturepreference);
        //                recordsAffected= oIACMDBContext.SaveChanges();
        //            }
        //            else
        //            {
        //                Culturepreference.Id = culturepreferenceID;
        //                UserCulturePreference editPref = oIACMDBContext.UserCulturePreferences.Where(ucp => ucp.Id == Culturepreference.Id).First();
        //                if (editPref == null)
        //                    return false;
        //                oIACMDBContext.Entry(editPref).CurrentValues.SetValues(Culturepreference);
        //                recordsAffected= oIACMDBContext.SaveChanges();
        //            }
        //        }

        //        if (recordsAffected == 1)
        //            return true;

        //        return false;

        //    }
        //    catch (Exception ex)
        //    {
        //       DILog.GeneralLogger.WriteLog(LogLevelType.Info,ex.Message);
        //        throw ex;
        //    }
        //}

      }
}

