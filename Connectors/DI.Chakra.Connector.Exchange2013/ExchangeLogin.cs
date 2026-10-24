using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Reflection;
//using DI.Chakra.Facade;
using DI.Chakra.Infrastructure;
//using DeepIdentity.Support;
using System.Configuration;
using DI.Chakra.Logger;

namespace DI.Chakra.Connector
{
    public class ExLogin
    {
        //#region Validate Methods

        ///// <summary>
        ///// This Method will verify if the User Account Exists
        ///// By Matching both the Username and Password as well as
        ///// checking if the Account is Active.
        ///// </summary>
        ///// <param name="sUserName">Username to Validate</param>
        ///// <param name="sPassword">Password of the Username to Validate</param>
        ///// <returns></returns>
        //public LoginResult Login(string sUserName, string sPassword)
        //{
        //    //Check if the Logon exists Based on the Username and Password
        //    if (IsUserValid(sUserName, sPassword))
        //    {
        //        oDE = GetUser(sUserName);
        //        if (oDE != null)
        //        {
        //            //Check the Account Status
        //            int iUserAccountControl = Convert.ToInt32(oDE.Properties["userAccountControl"][0]);
        //            oDE.Close();

        //            //If the Disabled Item does not Exist then the Account is Active
        //            if (!IsAccountActive(iUserAccountControl))
        //            {
        //                return LoginResult.LOGIN_USER_ACCOUNT_INACTIVE;
        //            }
        //            else
        //            {
        //                return LoginResult.LOGIN_OK;
        //            }

        //        }
        //        else
        //        {
        //            return LoginResult.LOGIN_USER_DOESNT_EXIST;
        //        }
        //    }
        //    else
        //    {
        //        return LoginResult.LOGIN_USER_DOESNT_EXIST;
        //    }
        //}

        ///// <summary>
        ///// This will perfrom a logical operation on the iUserAccountControl values
        ///// to see if the user Account is Enabled or Disabled.
        ///// The Flag for Determining if the Account is active is a Bitwise value (Decimal = 2)
        ///// </summary>
        ///// <param name="iUserAccountControl"></param>
        ///// <returns></returns>
        //public bool IsAccountActive(int iUserAccountControl)
        //{
        //    int iUserAccountControl_Disabled = Convert.ToInt32(ExAccountOptions.UF_ACCOUNTDISABLE);
        //    int iFlagExists = iUserAccountControl & iUserAccountControl_Disabled;

        //    //If a Match is Found, then the Disabled Flag Exists within the Control Flags
        //    if (iFlagExists > 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        ///// <summary>
        ///// This will perfrom a logical operation on the sUserName values
        ///// to see if the user Account is Enabled or Disabled.  
        ///// The Flag for Determining if the Account is active is a Bitwise value (Decimal = 2)
        ///// </summary>
        ///// <param name="sUserName">Username to Validate</param>
        ///// <returns></returns>
        //public bool IsAccountActive(string sUserName)
        //{
        //    oDE = GetUser(sUserName);
        //    if (oDE != null)
        //    {

        //        //to check of the Disabled option exists.
        //        int iUserAccountControl = Convert.ToInt32(oDE.Properties["userAccountControl"][0]);
        //        oDE.Close();

        //        //Check if the Disabled Item does not Exist then the Account is Active
        //        if (!IsAccountActive(iUserAccountControl))
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// This Method will Attempt to log in a User Based on the Username and Password
        ///// to Ensure that they have been set up within the Active Directory.  
        ///// This is the basic UserName and Password check.
        ///// </summary>
        ///// <param name="sUserName">Username to Validate</param>
        ///// <param name="sPassword">Password of the Username to Validate</param>
        ///// <returns></returns>
        //public bool IsUserValid(string sUserName, string sPassword)
        //{
        //    try
        //    {
        //        oDE = GetUser(sUserName, sPassword);
        //        oDE.Close();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //#endregion

        public bool AuthenticateUser(string aStrUserName, string aStrPassword, string aLDAPServerName, string aLDAPString)
        {

            //LDAP Related objects 
            System.DirectoryServices.DirectoryEntry pExEntry = default(System.DirectoryServices.DirectoryEntry);
            System.DirectoryServices.DirectorySearcher pExSearcher = default(System.DirectoryServices.DirectorySearcher);
            System.DirectoryServices.SearchResult pExResult = default(System.DirectoryServices.SearchResult);

            string pLDAPPath = null;           

            bool pFunctionResult = false;            

            try
            {
                
                pLDAPPath = "LDAP://" + aLDAPServerName + "/" + aLDAPString;


                //'Verify if the user is a valid domain user 
                pExEntry = new System.DirectoryServices.DirectoryEntry(pLDAPPath, aStrUserName, aStrPassword);
                pExSearcher = new System.DirectoryServices.DirectorySearcher(pExEntry);

                //'Setting the search parameters for the Active directory search 
                pExSearcher.Filter = "sAMAccountName=" + aStrUserName;
                pExSearcher.PropertiesToLoad.Add("cn");
                pExSearcher.PropertiesToLoad.Add("givenName");
                pExSearcher.PropertiesToLoad.Add("SN");


                //'This statement will throw an error if the Username or Password are incorrect 
                pExResult = pExSearcher.FindOne();
                pFunctionResult = (pExResult != null);

                return pFunctionResult;

            }
            catch (Exception ex)
            {

                //'Log error 
                //DILog.General.WriteLog(LogType.ERROR, ex.ToString());
                pFunctionResult = false;

            }


            return pFunctionResult;
        }

    }
}
