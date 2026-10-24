using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Reflection;
using DI.Chakra.Infrastructure;
using System.Configuration;
using System.Text;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Connector
{
    public class ADLogin
    {

        DirectoryEntry oDE = null;
        DirectoryEntry oDEC = null;
        DirectorySearcher oDS = null;
        SearchResultCollection oResults = null;
        DataSet oDs = null;
        DataSet oDsUser = null;
        DataTable oTb = null;
        DataRow oRwUser = null;
        DataRow oRwResult = null;
        DataRow oNewCustomersRow = null;

        #region Private Variables

        private string sADPath = string.Empty;
        private string sADPathPrefix = "";
        private string sADUser = "";
        private string sADPassword = "";
        private string sADServer = "";
        private string sCharactersToTrim = "";
        private bool bAuthenticated = false;

        #endregion


        public void Login(string sUserName, string sPassword)
        {
            //Check if the Logon exists Based on the Username and Password
            if (IsUserValid(sUserName, sPassword))
            {
                oDE = GetUser(sUserName);
                if (oDE != null)
                {
                    //Check the Account Status
                    int iUserAccountControl = Convert.ToInt32(oDE.Properties["userAccountControl"][0]);
                    oDE.Close();

                    if (!IsAccountActive(iUserAccountControl))
                    {
                        bAuthenticated = false;
                    }
                    else
                    {
                        bAuthenticated = true;
                    }
                }
                else
                {
                    bAuthenticated = false;
                }
            }
            else
            {

                bAuthenticated = false;
            }
        }

        public bool IsUserValid(string sUserName, string sPassword)
        {
            try
            {
                oDE = GetUser(sUserName, sPassword);
                oDE.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DirectoryEntry GetUser(string sUserName)
        {
            //Create an Instance of the DirectoryEntry
            oDE = GetDirectoryObject();

            //Create Instance fo the Direcory Searcher
            oDS = new DirectorySearcher();

            oDS.SearchRoot = oDE;
            //Set the Search Filter
            oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
            oDS.SearchScope = SearchScope.Subtree;
            oDS.PageSize = 10000;

            //Find the First Instance
            SearchResult oResults = oDS.FindOne();

            //If found then Return Directory Object, otherwise return Null
            if (oResults != null)
            {
                oDE = new DirectoryEntry(oResults.Path, sADUser, sADPassword, AuthenticationTypes.Secure);
                return oDE;
            }
            else
            {
                return null;
            }
        }

        public bool IsAccountLocked(int iUserAccountControl)
        {
            int iUserAccountControl_Locked = Convert.ToInt32(ADAccountOptions.UF_ACCOUNT_LOCKOUT);
            int iFlagExists = iUserAccountControl & iUserAccountControl_Locked;

            //If a Match is Found, then the Disabled Flag Exists within the Control Flags
            if (iFlagExists > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsAccountActive(int iUserAccountControl)
        {
            int iUserAccountControl_Disabled = Convert.ToInt32(ADAccountOptions.UF_ACCOUNTDISABLE);
            int iFlagExists = iUserAccountControl & iUserAccountControl_Disabled;

            //If a Match is Found, then the Disabled Flag Exists within the Control Flags
            if (iFlagExists > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public DirectoryEntry GetUser(string sUserName, string sPassword)
        {
            //Create an Instance of the DirectoryEntry
            oDE = GetDirectoryObject(sUserName, sPassword);

            //Create Instance fo the Direcory Searcher
            oDS = new DirectorySearcher();
            oDS.SearchRoot = oDE;

            //Set the Search Filter
            oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
            oDS.SearchScope = SearchScope.Subtree;
            oDS.PageSize = 10000;

            //Find the First Instance
            SearchResult oResults = oDS.FindOne();

            //If a Match is Found, Return Directory Object, Otherwise return Null
            if (oResults != null)
            {
                oDE = new DirectoryEntry(oResults.Path, sADUser, sADPassword, AuthenticationTypes.Secure);
                return oDE;
            }
            else
            {
                return null;
            }

        }

        private DirectoryEntry GetDirectoryObject()
        {
            oDE = new DirectoryEntry(sADPath, sADUser, sADPassword, AuthenticationTypes.Secure);
            return oDE;
        }

        private DirectoryEntry GetDirectoryObject(string sUserName, string sPassword)
        {
            oDE = new DirectoryEntry(sADPath, sUserName, sPassword, AuthenticationTypes.Secure);
            return oDE;
        }

        public bool AuthenticateUser(ADSUserInfo _adUser, string password)
        {
            EndPoint ep = EndPoint.GetByIdIncludingEndPointSystemType(_adUser.EndPointSystemId);
            var adConfiguration = EndpointConfiguration.GetByEndpointSystemId(ep.Id);
            var ipadress = adConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName.ToString() == EndpointKey.IPAddress.ToString());
            var domainname = adConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName.ToString() == EndpointKey.DomainName.ToString());
            return AuthenticateUser1(_adUser.sAMAccountName, password, ipadress.Value, domainname.Value);
        }

        public bool AuthenticateUser1(string aStrUserName, string aStrPassword, string aLDAPServerName, string domainName)
        {
            bAuthenticated = false;

            IList<string> lstDomain = new List<string>();

            try
            {
                oDE = new DirectoryEntry();
                if (!string.IsNullOrEmpty(domainName))
                {
                    lstDomain = domainName.Split(new char[] { '.' }).ToList();
                }

                StringBuilder bldr = new StringBuilder();
                int index = 0;
                string domainString = string.Empty;

                foreach (string str in lstDomain)
                {

                    bldr.AppendFormat("DC={0},", str);

                    index = index + 1;

                    if (lstDomain.Count == index)
                        domainString = bldr.ToString().Remove(bldr.ToString().Length - 1);
                }

                sADPath = string.Format("LDAP://{0}/{1}", aLDAPServerName, domainString);

                sADUser = aStrUserName;
                sADPassword = aStrPassword;

                sADServer = aLDAPServerName;
                Login(sADUser, sADPassword);

                return bAuthenticated;

            }
            catch (Exception ex)
            {
                //'Log error 
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Error, string.Format("Authenticated failed for user {0}", aStrUserName), ex);
                return false;
            }
        }

    }
}