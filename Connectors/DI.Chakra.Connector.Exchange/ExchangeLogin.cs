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
