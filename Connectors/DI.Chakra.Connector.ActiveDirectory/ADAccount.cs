using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
//using ActiveDs;
namespace DI.Chakra.Connector
{
    public class ADAccount
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

        //public static DateTime LastLogon(DirectoryEntry oDE)
        //{
        //    Dictionary<string, Int64> lastLogons = new Dictionary<string, Int64>();
        //    Int64 lastLogonThisServer = new Int64();
        //    if (oDE.Properties["lastLogon"].Value != null)
        //    {
        //        IADsLargeInteger lgInt =
        //            (IADsLargeInteger)deUser.Properties["lastLogon"].Value;
        //        lastLogonThisServer = ((long)lgInt.HighPart << 32) + lgInt.LowPart;
        //    }

        //    // Save the most recent logon for each user in a Dictionary object
        //    if (lastLogons.ContainsKey(distinguishedName))
        //    {
        //        if (lastLogons[distinguishedName] < lastLogonThisServer)
        //        {
        //            lastLogons[distinguishedName] = lastLogonThisServer;
        //        }
        //    }
        //    else
        //    {
        //        lastLogons.Add(distinguishedName, lastLogonThisServer);
        //    }
        //}
    }
}
