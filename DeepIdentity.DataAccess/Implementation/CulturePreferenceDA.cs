using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace DeepIdentity.DataAccess.Implementation
{
    public class CulturePreferenceDA
    {

        public List<UserCulturePreference> GetUserPreference(int UserId)
        {    
            try
            {
                List<UserCulturePreference> preferencelist = new List<UserCulturePreference>();
                if (UserId != -1)
                {
                    using (IACMDBContext oIACMDBContext = new IACMDBContext())
                    {
                        preferencelist = oIACMDBContext.UserCulturePreferences.Where(lq => lq.UserId == UserId).ToList();
                    }
                }
                return preferencelist;
            }
                
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Geting GetUserCulturePreference");
                throw;          
            }
           
        }


        
        public  void SaveUserCulturepreference(UserCulturePreference Culturepreference)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    //bool isUserExists = oIACMDBContext.UserCulturePreferences.Any(u => u.UserId == Culturepreference.UserId);
                    var culturepreferenceID = oIACMDBContext.UserCulturePreferences.Where(ucp => ucp.Id == Culturepreference.Id).Select(ucp => ucp.Id).FirstOrDefault();

                    if (Culturepreference.Id == null || Culturepreference.Id  == 0)
                    {
                        oIACMDBContext.UserCulturePreferences.Add(Culturepreference);
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        Culturepreference.Id = culturepreferenceID;
                        UserCulturePreference editPref = oIACMDBContext.UserCulturePreferences.Where(ucp => ucp.Id == Culturepreference.Id).First();
                        if (editPref == null)
                            return;
                        oIACMDBContext.Entry(editPref).CurrentValues.SetValues(Culturepreference);
                        oIACMDBContext.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
        }

      }
}

