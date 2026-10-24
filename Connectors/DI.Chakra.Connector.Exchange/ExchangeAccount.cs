using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
namespace DI.Chakra.Connector
{
    public class ExAccount
    {
        public static bool IsAccountDisabled(DirectoryEntry oDE)
        {
            try
            {
                return Convert.ToBoolean(oDE.InvokeGet("AccountDisabled"));
            }
            catch (Exception)
            {

                return false;
            }
           
        }
        public static bool IsAccountLocked(DirectoryEntry oDE)
        {
            try
            {
                return Convert.ToBoolean(oDE.InvokeGet("IsAccountLocked").ToString());
            }
            catch (Exception)
            {

                return false;
            }
            
        }
        public static bool IsPasswordRequired(DirectoryEntry oDE)
        {
            try
            {
                return Convert.ToBoolean(oDE.InvokeGet("PasswordRequired").ToString());
            }
            catch (Exception)
            {

                return false;
            }

        }

        public static DateTime? PasswordExpirationDate(DirectoryEntry oDE)
        {
            try
            {
                return Convert.ToDateTime(oDE.InvokeGet("PasswordExpirationDate"));
            }
            catch (Exception)
            {

                return null;
            }

        }

    }
}
