using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using System.DirectoryServices;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;

namespace DI.Chakra.Connector.ActiveDirectory
{
    public class ADPasswordHelper
    {
        enum ADUserControl : int
        {
            SCRIPT = 0x0001,
            ACCOUNTDISABLE = 0x0002,
            HOMEDIR_REQUIRED = 0x0008,
            LOCKOUT = 0x0010,
            PASSWD_NOTREQD = 0x0020,
            PASSWD_CANT_CHANGE = 0x0040,
            ENCRYPTED_TEXT_PWD_ALLOWED = 0x0080,
            TEMP_DUPLICATE_ACCOUNT = 0x0100,
            NORMAL_ACCOUNT = 0x0200,
            INTERDOMAIN_TRUST_ACCOUNT = 0x0800,
            WORKSTATION_TRUST_ACCOUNT = 0x1000,
            SERVER_TRUST_ACCOUNT = 0x2000,
            DONT_EXPIRE_PASSWORD = 0x10000,
            MNS_LOGON_ACCOUNT = 0x20000,
            SMARTCARD_REQUIRED = 0x40000,
            TRUSTED_FOR_DELEGATION = 0x80000,
            NOT_DELEGATED = 0x100000,
            USE_DES_KEY_ONLY = 0x200000,
            DONT_REQ_PREAUTH = 0x400000,
            PASSWORD_EXPIRED = 0x800000,
            TRUSTED_TO_AUTH_FOR_DELEGATION = 0x1000000
        }


        private EndpointConfiguration m_adsConfig = null;


        public ADPasswordHelper(EndpointConfiguration adsConfig)
        {
            m_adsConfig = adsConfig;
            m_adsConfig.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.EndpointConfigurationKey.KeyName.ToString() == EndpointKey.Password.ToString())
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
        }

        private string Configuration(EndpointKey Key)
        {
            var config = m_adsConfig.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName.ToString() == Key.ToString());
            return config != null ? config.Value : "";
        }

        private DirectoryEntry getRootDirectory()
        {
            DirectoryEntry root = new DirectoryEntry();
            List<string> lstDomain = null;
            StringBuilder bldr = new StringBuilder();
            int index = 0;
            string domainString = string.Empty;


            if (Configuration(EndpointKey.DomainName) != string.Empty)
            {
                lstDomain = Configuration(EndpointKey.DomainName).Split(new char[] { '.' }).ToList();
            }



            foreach (string str in lstDomain)
            {

                bldr.AppendFormat("DC={0},", str);

                index = index + 1;

                if (lstDomain.Count == index)
                    domainString = bldr.ToString().Remove(bldr.ToString().Length - 1);
            }
            ADHelper.SetDirectoryPath(root, domainString, this.m_adsConfig);

            return root;
        }
        private DirectorySearcher getRootSearcher()
        {
            DirectoryEntry root = getRootDirectory();
            string domainString = string.Empty;
            List<string> lstDomain = null;
            StringBuilder bldr = new StringBuilder();
            int index = 0;

            if (Configuration(EndpointKey.DomainName) != string.Empty)
            {
                lstDomain = Configuration(EndpointKey.DomainName).Split(new char[] { '.' }).ToList();
            }

            foreach (string str in lstDomain)
            {
                bldr.AppendFormat("DC={0},", str);

                index = index + 1;

                if (lstDomain.Count == index)
                    domainString = bldr.ToString().Remove(bldr.ToString().Length - 1);
            }

            ADHelper.SetDirectoryPath(root, domainString, this.m_adsConfig);
            root.Username = Configuration(EndpointKey.UserName);
            root.Password = Configuration(EndpointKey.Password);
            //root.AuthenticationType = AuthenticationTypes.Secure | AuthenticationTypes.ServerBind;

            DirectorySearcher rootSearch = new DirectorySearcher(root);

            return rootSearch;
        }
        private bool CanTryLogin(string accountName, out string message)
        {
            message = string.Empty;
            try
            {
                DirectorySearcher dirSearch = getRootSearcher();

                dirSearch.Filter = "sAMAccountName=" + accountName.ToSecureLDAPFilter();


                System.DirectoryServices.SearchResult pADResult = dirSearch.FindOne();


                if (pADResult != null)
                {
                    DirectoryEntry objLoginEntry = (pADResult != null) ? pADResult.GetDirectoryEntry() : null;
                    if (objLoginEntry != null)
                    {

                        if (ADAccount.IsAccountDisabled(objLoginEntry))
                        {
                            message = "Account is disabled";
                            return false;
                        }
                        else if (ADAccount.IsAccountLocked(objLoginEntry))
                        {
                            message = "Account is locked";
                            return false;
                        }
                        else
                            return true;

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }
        public LoginResult Login(string userName, string password)
        {
            LoginResult result = new LoginResult();

            string message = string.Empty;
            try
            {
                bool canLogin = CanTryLogin(userName, out message);

                if (canLogin == false)
                {
                    result.Success = false;
                    result.Message = message;

                    return result;
                }


                bool success = Authenticate(userName, password);

                if (success)
                {
                    result.Success = true;
                    result.Message = "Success";
                }
                else
                {
                    result.Success = false;
                    result.Message = "User Name or Password is wrong";
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
        private bool Authenticate(string userName, string password)
        {
            bool authentic = false;
            try
            {
                DirectoryEntry dirEntry = getRootDirectory();

                dirEntry.Username = userName;
                dirEntry.Password = password;



                object nativeObject = dirEntry.NativeObject;
                authentic = true;

            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {
                authentic = false;
            }
            catch (Exception sysEx)
            {
                authentic = false;
            }
            return authentic;
        }
        public bool UnLock(string accountName)
        {
            try
            {
                DirectorySearcher dirSearch = getRootSearcher();

                dirSearch.Filter = "sAMAccountName=" + accountName.ToSecureLDAPFilter();


                System.DirectoryServices.SearchResult pADResult = dirSearch.FindOne();


                if (pADResult == null)
                    return false;

                DirectoryEntry objLoginEntry = (pADResult != null) ? pADResult.GetDirectoryEntry() : null;

                if (objLoginEntry == null)
                    return false;


                int val = (int)objLoginEntry.Properties["userAccountControl"].Value;

                objLoginEntry.Properties["userAccountControl"].Value = val & ~(int)ADUserControl.LOCKOUT;

                objLoginEntry.CommitChanges();
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Success, string.Format("Account Unlock for {0} is successful", accountName));
                return true;

            }
            catch (Exception ex)
            {
               DILog.ProvisioningLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }

        }
    }
}
