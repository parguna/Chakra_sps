using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using DI.Chakra.Connector.Common.Enum;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Connector
{
    public class ExchangeConnector : ConnectorBase
    {
        #region private fields
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicGroups = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicGroupUsers = new Dictionary<string, ArrayList>();
        private DirectoryEntry m_root = null;
        private bool IsRunSpaceOpened = false;
        private int ResultSize = 20000;
        private const int ThreadSleeperTime = 10000;
        Runspace dummyRunspace = null;
        private EndpointConfiguration m_Config = null;
        private bool Isoffice365 = true;

        #endregion
        enum ExUserControl : int
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

        private bool IsConnected = false;
        public enum objectClass
        {
            user, group
        }
        public ExchangeConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            this.EndPoint = endpoint;
            this.CurrentRequest = currentRequest;
            var exConfig = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            exConfig.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString())
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.ExchangeConfiguration = exConfig;

            if (ConfigurationMaster.GetByConfigKey("ExChangeResultSize", Product.IACM) != null)
            {
                this.ResultSize = Convert.ToInt32(ConfigurationMaster.GetByConfigKey("ExChangeResultSize", Product.IACM).ConfigValue);
                this.ResultSize = this.ResultSize == 0 ? 20000 : this.ResultSize;
            }
        }

        public EndpointConfiguration ExchangeConfiguration
        {
            get
            {
                return m_Config;
            }

            set
            {
                this.m_Config = value;
            }

        }

        public string DomainController
        {
            get
            {
                var domainControl = ConfigurationMaster.GetByConfigKey("DOMAINCONTROLSERVER", Product.IM);
                return domainControl != null && !string.IsNullOrEmpty(domainControl.ConfigValue) ? domainControl.ConfigValue.Trim() : "dvappdom6v.rws-dev.rwsentosa.com";
            }
            set { }
        }

        public string DefaultDomain
        {
            get
            {
                var defaultDoamin = ConfigurationMaster.GetByConfigKey("DefaultDomain", Product.IACM);
                return defaultDoamin != null && !string.IsNullOrEmpty(defaultDoamin.ConfigValue) ? defaultDoamin.ConfigValue.Trim() : "rws";
            }
            set { }
        }

        public bool IsCrossDomain
        {
            get
            {
                var domainControl = ConfigurationMaster.GetByConfigKey("ISCROSSDOMAIN", Product.IM);
                return domainControl != null && !string.IsNullOrEmpty(domainControl.ConfigValue) && domainControl.ConfigValue == "1" ? true : false;
            }
            set
            {

            }
        }


        private string Configuration(EndpointKey Key)
        {
            var config = ExchangeConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }


        public override bool Connect()
        {
            //bool bReturn = false;
            //Isoffice365 = false;
            //if (!Isoffice365)
            //{
            //    bReturn = ConnectToExchangeServer();
            //}
            //else
            //{
            //    bReturn = ConnectToOffice365();
            //}

            return true;
        }
        private Object GetPropValue(String name, Object obj)
        {

            Type type = obj.GetType();

            PropertyInfo info = type.GetProperty(name);

            if (info == null)
                return null;


            if (info.ToString().Equals(string.Empty))
                return null;

            obj = info.GetValue(obj, null);

            if (info.PropertyType == typeof(DateTime))
            {
                if (obj == null || obj.ToString() == string.Empty)
                    return DBNull.Value;
            }

            return obj;
        }


        private void DiscoverUsers()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Users- {0}", Configuration(EndpointKey.IPAddress)));
            this.GetUsers();
            IList<ExUserInfo> listExUsers = new List<ExUserInfo>();
            listExUsers = ExUserInfo.GetDeletedUsers(this.EndPoint.Id, this.Process);
            foreach (ExUserInfo user in listExUsers)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Deleted User- {0}", user.sAMAccountName));
            }

            ExUserInfo.RemoveExUser(listExUsers.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Users- {0}", Configuration(EndpointKey.IPAddress)));
        }

        private void DiscoverGroups()
        {
            this.GetGroups();
            IList<ExGroupInfo> listExGroups = new List<ExGroupInfo>();
            listExGroups = ExGroupInfo.GetDeletedExGroups(this.EndPoint.Id, this.Process);
            ExGroupInfo.RemoveExGroup(listExGroups.ToList());

        }

        public override void GetData()
        {
            #region  Test Code
            ExUserInfo exUserValue = new ExUserInfo();

            #endregion
            long DiscoveryProcessId = base.DiscoveryProcess.Id;

            List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

            bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_EX_User) != null;
            bool IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_EX_Group) != null;

            WriteDiscoveryLog(LogLevelType.Info, string.Format("GetData Started for Exchange - {0}", Configuration(EndpointKey.IPAddress)));
            try
            {
                if (IsUserSelected)
                {
                    DiscoverUsers();
                    DiscoverUserMailBoxQuotaInfo();
                }
                dicUsers = ExUserInfo.getUserIds();

                if (IsGroupSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Groups- {0}", Configuration(EndpointKey.IPAddress)));
                    DiscoverGroups();
                    dicGroups = ExGroupInfo.GetEXGroupInfo();
                    SaveUserGroups();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Groups- {0}", Configuration(EndpointKey.IPAddress)));
                }

                if (IsUserSelected || IsGroupSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Exchange Account recon started- {0}", Configuration(EndpointKey.IPAddress)));
                    ExChangeAccountReconcilation();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Exchange Account recon ended- {0}", Configuration(EndpointKey.IPAddress)));

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Exchange Entitlement recon started- {0}", Configuration(EndpointKey.IPAddress)));
                    this.UpdateEntitlementRecon();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Exchange Entitlement recon ended- {0}", Configuration(EndpointKey.IPAddress)));

                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Post discovery mapping started - {0}", Configuration(EndpointKey.IPAddress)));
                ExUserInfo.MapWithMasterUserForReconTable(EndPoint, ExchangeConfiguration.MapAll != null ? ExchangeConfiguration.MapAll : false);
                ExUserInfo.MapWithMasterUserForEntitlementReconTable(EndPoint, ExchangeConfiguration.MapAll != null ? ExchangeConfiguration.MapAll : false, DiscoveryProcessId);

                WriteDiscoveryLog(LogLevelType.Info, string.Format("updating email account into user table- {0}", Configuration(EndpointKey.IPAddress)));
                ExUserInfo.PopulateMasterUserFromEx(EndPoint);

                WriteDiscoveryLog(LogLevelType.Info, string.Format("GetData End for Exchange - {0}", Configuration(EndpointKey.IPAddress)));

            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error in Exchange GetData()-{0}.", Configuration(EndpointKey.IPAddress)), ex);
                throw ex;
            }

        }


        #region AccountReconcilation
        private void ExChangeAccountReconcilation()
        {
            try
            {
                AccountReconcilation.ProcessDeletedAccounts(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
                List<ExUserInfo> userList = ExUserInfo.GetDeletedUsers(this.EndPoint.Id, this.Process).ToList();
                if (userList != null && userList.Count > 0)
                {
                    ExUserInfo.RemoveExUser(userList.ToList());
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception at Exchange Account Recon", Configuration(EndpointKey.IPAddress)), ex, true);

            }
        }
        #endregion

        private void UpdateEntitlementRecon()
        {
            try
            {
                // List<ExUserInfo> allexistingUsers = ExUserInfo.GetByEndPointSystemIdIncludeGrps(this.EndPoint.Id).ToList();
                List<AccountReconcilation> exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(this.EndPoint.Id).ToList();
                List<EntitlementReconcilation> existingEntilements = EntitlementReconcilation.GetAllByEndpointSystemId(this.EndPoint.Id);
                AccountReconcilation acctRecon = null;
                EntitlementReconcilation entRecon = null;
                dicUsers = ExUserInfo.getUserIds();
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    //  var exUserInfo = allexistingUsers.Where(a => a.Id == item.Key).FirstOrDefault();

                    var exUserInfo = ExUserInfo.GetByEndPointSystemIdByuserId(item.Key);
                    if (exUserInfo != null)
                    {
                        acctRecon = exsitngAccountrecon.Where(a => a.EndpointUserId == exUserInfo.Id).FirstOrDefault();
                        if (acctRecon == null) continue;

                        if (exUserInfo.EXGroups != null)
                        {
                            foreach (ExGroupInfo entitlement in exUserInfo.EXGroups)
                            {
                                if (!string.IsNullOrEmpty(entitlement.name))
                                {
                                    entRecon = existingEntilements.Where(e => e.EndpointUserId == exUserInfo.Id &&
                                                                             e.Entitlement == Convert.ToString(entitlement.name).Trim() &&
                                                                             e.EntitlementType == (int)EntitlementType.Group).FirstOrDefault();


                                    if (entRecon != null)
                                    {
                                        if (entRecon.IsDeleted)
                                        {
                                            EntitlementReconcilation.UpdateException(entRecon, true);
                                        }
                                        entRecon.IsDeleted = false;
                                        entRecon.Save(MasterConfigHelper.ServiceUser);
                                    }
                                    else
                                    {
                                        var entitlements = (exUserInfo.GroupEntitlements.Select(x => Convert.ToString(x.EntitlementName)));
                                        //removed the SOD check
                                        entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Group, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                        entRecon.Entitlement = !string.IsNullOrEmpty(entitlement.name) ? Convert.ToString(entitlement.name).Trim() : string.Empty;
                                        entRecon.EntitlementId = entitlement.Id;
                                        entRecon.EndpointSystemId = this.EndPoint.Id;
                                        entRecon.EntitlementType = (int)EntitlementType.Group;
                                        entRecon.EntitlementDescription = entitlement.description;
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                        entRecon.Save(MasterConfigHelper.ServiceUser);

                                        existingEntilements.Add(entRecon);
                                    }
                                }
                            }
                        }

                        if (exUserInfo.EXMailQuota != null)
                        {
                            foreach (ExMailBoxQuotaInfo entitlement in exUserInfo.EXMailQuota)
                            {
                                if (!string.IsNullOrEmpty(entitlement.Name))
                                {
                                    entRecon = existingEntilements.Where(e => e.EndpointUserId == exUserInfo.Id &&
                                                                         e.Entitlement == entitlement.Name.Trim() &&
                                                                         e.EntitlementType == (int)EntitlementType.MailQuota).FirstOrDefault();


                                    if (entRecon != null)
                                    {
                                        if (entRecon.IsDeleted)
                                        {
                                            EntitlementReconcilation.UpdateException(entRecon, true);
                                        }
                                        entRecon.LastModifiedDate = DateTime.Now;
                                        entRecon.LastModifiedBy = "System";
                                        entRecon.Entitlement = entitlement.Name.Trim();
                                        entRecon.EntitlementId = entitlement.Id;
                                        entRecon.EntitlementDescription = entitlement.Description;
                                        entRecon.EntitlementType = (int)EntitlementType.MailQuota;
                                        entRecon.EndpointSystemId = this.EndPoint.Id;
                                        /*entRecon.IsDeleted = false*/;
                                        entRecon.Save(MasterConfigHelper.ServiceUser);
                                    }
                                    else
                                    {
                                        var entitlements = (exUserInfo.MailQuotaEntitlements.Select(x => Convert.ToString(x.EntitlementName)));
                                        //removed the SOD check
                                        entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Group, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                        entRecon.Entitlement = entitlement.Name.Trim();
                                        entRecon.EntitlementId = entitlement.Id;
                                        entRecon.EntitlementDescription = entitlement.Description;
                                        entRecon.EntitlementType = (int)EntitlementType.MailQuota;
                                        entRecon.EndpointSystemId = this.EndPoint.Id;
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                        entRecon.Save(MasterConfigHelper.ServiceUser);

                                        existingEntilements.Add(entRecon);
                                    }
                                }
                            }
                        }
                    }
                }

                EntitlementReconcilation.ProcessDeletedEntitlements(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation.", ex);
            }
        }


        private void GetGroups()
        {
            try
            {
                IList<ExGroupInfo> list = new List<ExGroupInfo>();

                ExGroupInfo excInfo = null;
                GroupInfoOrchestrator orchestrator = new GroupInfoOrchestrator();
                orchestrator.EndpointSystemId = this.EndPoint.Id;

                var results = geGroupsByRunspace();

                foreach (PSObject ps in results)
                {
                    excInfo = new ExGroupInfo();
                    excInfo = orchestrator.PopulateGroup(ps);
                    if (null != excInfo)
                        list.Add(excInfo);
                }

                if (list.Count > 0)
                {
                    ExGroupInfo.SaveGroups(list, MasterConfigHelper.ServiceUser);
                }

                using (Runspace rsp4 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(20)))
                {
                    rsp4.StateChanged += rsp_StateChanged;
                    rsp4.AvailabilityChanged += rsp_AvailabilityChanged;
                    rsp4.Open();

                    try
                    {
                        while (!IsRunSpaceOpened)
                        {
                            System.Threading.Thread.Sleep(10000);
                        }

                        foreach (ExGroupInfo ps in list)
                        {
                            var name = Convert.ToString(ps.name);
                            try
                            {
                                if (!string.IsNullOrEmpty(name))
                                {
                                    using (PowerShell grpMm = PowerShell.Create())
                                    {
                                        grpMm.InvocationStateChanged += powershell_InvocationStateChanged;
                                        grpMm.Runspace = rsp4;
                                        grpMm.AddCommand("Get-DistributionGroupMember");
                                        grpMm.AddParameter("identity", name);

                                        var tempresult = grpMm.Invoke();

                                        ArrayList mem = new ArrayList();
                                        foreach (PSObject ps1 in tempresult)
                                        {
                                            mem.Add(ps1.Members["DistinguishedName"].Value.ToString());
                                        }
                                        dicGroupUsers.Add(ps.distinguishedName, mem);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void GetUsers()
        {

            IList<ExUserInfo> list = new List<ExUserInfo>();
            try
            {

                ExUserInfo excUserInfo = null;
                UserInfoOrchestrator orchestrator = new UserInfoOrchestrator();
                List<AccountReconcilation> exsitingAcctRecon = AccountReconcilation.GetAllByEndpointId(this.EndPoint.Id).ToList();
                List<ExUserInfo> listExistingExusers = ExUserInfo.GetAllByEndpointId(this.EndPoint.Id).ToList();
                orchestrator.EndpointSystemId = this.EndPoint.Id;

                Dictionary<string, string> dc = new Dictionary<string, string>();
                List<string> lst = new List<string>();
                int counter = 1;

                var results = getUsersByRunspace();

                this.WriteDiscoveryLog(LogLevelType.Info, string.Format("Total usersdiscovered- {0}", results != null ? results.Count() : 0));

                foreach (PSObject ps in results)
                {
                    excUserInfo = orchestrator.PopulateExchangeUser(ps);

                    if (null != excUserInfo)
                    {
                        list.Add(excUserInfo);

                        //this.WriteDiscoveryLog(LogLevelType.Info, string.Format("SAMAccountName - {0}", excUserInfo.sAMAccountName));
                    }

                    //if (list.Count() % 10 == 0)
                    //{
                    //    if (counter == 1)
                    //    {
                    //        this.WriteDiscoveryLog(LogLevelType.Info, string.Format("save users started Batch - {0}", counter));
                    //    }
                    //    ExUserInfo.SaveUsers(list, EndPoint.Id, MasterConfigHelper.ServiceUser, exsitingAcctRecon, listExistingExusers);
                    //    counter++;
                    //    list.Clear();
                    //}

                }
                if (list.Count() > 0)
                {
                    ExUserInfo.SaveUsers(list, EndPoint.Id, MasterConfigHelper.ServiceUser, exsitingAcctRecon, listExistingExusers);
                }
                //if (counter > 1)
                //    this.WriteDiscoveryLog(LogLevelType.Info, string.Format("save users completed Batch - {0}", counter));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Disconnect()
        {
            //   throw new NotImplementedException();
        }

        #region SAVE USER GROUPS
        private void SaveUserGroups()
        {
            ExUserInfo.SaveUserGroupMap(dicUsers, dicGroups, dicGroupUsers, this.EndPoint.Id);
        }
        #endregion

        #region Connector Helper Methods

        private WSManConnectionInfo CreateConnectionForRunspace(int timeout)
        {
            WSManConnectionInfo connInfo = null;
            string hostName = string.Empty;

            try
            {

                string[] hostNames = Convert.ToString(Configuration(EndpointKey.HostName)).Split(';');

                string userName = Configuration(EndpointKey.UserName);
                string password = Configuration(EndpointKey.Password);
                string ipaddress = Configuration(EndpointKey.IPAddress);

                SecureString ssRunasPassword = new SecureString();
                foreach (char x in password)
                    ssRunasPassword.AppendChar(x);
                PSCredential credentials = new PSCredential(userName, ssRunasPassword);

                foreach (string thostName in hostNames)
                {
                    hostName = thostName;
                    string url = string.Format("http://{0}{1}", hostName, "/PowerShell");

                    connInfo = new WSManConnectionInfo(new Uri(url), "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credentials);
                    connInfo.AuthenticationMechanism = AuthenticationMechanism.Kerberos;
                    connInfo.OpenTimeout = timeout * 60 * 1000;
                    connInfo.OperationTimeout = timeout * 60 * 1000;
                    connInfo.IdleTimeout = timeout * 60 * 1000;
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Trying to connect exchange box {0}", hostName));
                    if (TestConnection(connInfo))
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Exchange box connected succesfully {0}", hostName));
                        return connInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, string.Format(" Failed to Create Connection ", hostName));
                WriteDiscoveryLog(LogLevelType.Info, string.Format(ex.Message));
            }

            return connInfo;
        }

        private bool TestConnection(WSManConnectionInfo connection)
        {
            using (Runspace rsp1 = RunspaceFactory.CreateRunspace((connection)))
            {

                try
                {
                    rsp1.StateChanged += rsp_StateChanged;
                    rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                    rsp1.Open();

                    while (!IsRunSpaceOpened)
                    {
                        System.Threading.Thread.Sleep(10000);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private bool ConnectToOffice365()
        {
            bool isConnected = false;
            try
            {
                this.ExchangeConfiguration = EndpointConfiguration.GetByEndpointSystemId(EndPoint.Id);

                string userName = Configuration(EndpointKey.UserName);
                string password = Configuration(EndpointKey.Password);
                string ipaddress = Configuration(EndpointKey.IPAddress);
                string url = string.Format("http://{0}/{1}", ipaddress, "/PowerShell");

                SecureString ssRunasPassword = new SecureString();
                foreach (char x in password)
                    ssRunasPassword.AppendChar(x);
                PSCredential credentials = new PSCredential(userName, ssRunasPassword);

                WSManConnectionInfo connInfo = new WSManConnectionInfo(new Uri("https://ps.outlook.com/PowerShell-LiveID?PSVersion=2.0"), "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credentials);

                connInfo.MaximumConnectionRedirectionCount = 5;
                connInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;
                connInfo.SkipCACheck = true;
                connInfo.SkipCNCheck = true;
                connInfo.SkipRevocationCheck = true;

                dummyRunspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(connInfo);
                dummyRunspace.Open();
                isConnected = true;
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("Office365 {0} Connected", Configuration(EndpointKey.IPAddress)));

            }
            catch (Exception ex)
            {

                isConnected = false;
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format(" Failed to Connect to Office365 ", Configuration(EndpointKey.IPAddress)));
                WriteDiscoveryLog(LogLevelType.Info, string.Format(ex.Message));
            }

            return isConnected;
        }

        #endregion

        #region Office 365

        //private bool CreateOffice365MailBox(IEndpointUserInfo _userInfo, IProvisioningForm _formInfo)
        //{
        //    bool isMailboxCreated = false;
        //    ExUserInfo _exUserInfo = (ExUserInfo)_userInfo;

        //    //For testing
        //    _exUserInfo = new ExUserInfo();
        //    _exUserInfo.FirstName = "Test";
        //    _exUserInfo.LastName = "User29";
        //    _exUserInfo.name = "Tuser29";
        //    ExchangeConfiguration.ConfigurationDetails.ForEach(lq =>
        //    {
        //        if (lq.KeyId == Convert.ToInt64(EndpointKey.DomainName))
        //        {
        //            lq.Value = "deepidentity.onmicrosoft.com";
        //        }
        //    });
        //    _exUserInfo.NewUsrPassword = "@bcd1234";


        //    try
        //    {

        //        if (this.Connect())
        //        {
        //            var domainName = Configuration(EndpointKey.DomainName);
        //            var password = _exUserInfo.NewUsrPassword;
        //            var ssPassword = new SecureString();
        //            foreach (char c in password)
        //                ssPassword.AppendChar(c);

        //            var command = new Command("New-Mailbox");
        //            command.Parameters.Add("Name", _exUserInfo.FirstName + " " + _exUserInfo.LastName);
        //            command.Parameters.Add("Alias", _exUserInfo.name);
        //            command.Parameters.Add("MicrosoftOnlineServicesID", _exUserInfo.name + "@" + domainName);
        //            command.Parameters.Add("FirstName", _exUserInfo.FirstName);
        //            command.Parameters.Add("LastName", _exUserInfo.LastName);
        //            command.Parameters.Add("Password", ssPassword);
        //            command.Parameters.Add("ResetPasswordOnNextLogon", false);
        //            command.Parameters.Add("mail", _exUserInfo.mail);



        //            var pipeline = runspace.CreatePipeline();
        //            pipeline.Commands.Add(command);

        //            var results = pipeline.Invoke();
        //            if (results.Count > 0)
        //                isMailboxCreated = true;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        isMailboxCreated = false;

        //    }

        //    return isMailboxCreated;
        //}

        private void DiscoverUserMailBoxQuotaInfo()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering UserMailBoxQuotaInfo"));
            this.GetUserMailBoxQuotaInfo();
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering UserMailBoxQuotaInfo"));
        }

        private void GetUserMailBoxQuotaInfo()
        {

            try
            {
                IList<ExUserMailBoxQuotaInfo> list = new List<ExUserMailBoxQuotaInfo>();
                ExUserMailBoxQuotaInfo excInfo = null;
                UserMailBoxQuotaInfoOrchastrator orchestrator = new UserMailBoxQuotaInfoOrchastrator();
                orchestrator.EndpointSystemId = this.EndPoint.Id;

                var results = getUserMailQuotaByRunspace();

                List<string> lst = new List<string>();
                foreach (PSObject ps in results)
                {
                    excInfo = orchestrator.UserMailBoxQuotaInfo(ps);
                    if (null != excInfo)
                        list.Add(excInfo);
                }
                if (list.Count > 0)
                {
                    ExUserMailBoxQuotaInfo.SaveUserMailBoxQuotaInfo(list, MasterConfigHelper.ServiceUser, this.EndPoint.Id);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool DeleteOffice365MailBox(IEndpointUserInfo _userInfo)
        {
            bool isRemoved = false;
            ExUserInfo _Userinfo = (ExUserInfo)_userInfo;
            try
            {

                if (this.Connect())
                {
                    var command = new Command("Remove-Mailbox");
                    command.Parameters.Add("Identity", _Userinfo.mail);
                    command.Parameters.Add("Confirm", false);

                    var pipeline = dummyRunspace.CreatePipeline();
                    pipeline.Commands.Add(command);

                    var results = pipeline.Invoke();
                    if (results.Count > 0)
                    {
                        isRemoved = true;

                    }
                    else
                        isRemoved = false;


                }
            }
            catch (Exception ex)
            {
                isRemoved = false;

            }
            return isRemoved;
        }

        private bool Changeoffice365MailPassword(string UserName, string Password, int DIUserId)
        {
            bool isChanged = false;
            try
            {
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(new string[] { "MSOnline" });

                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    var command = new Command("Set-MsolUserPassword");
                    command.Parameters.Add("UserPrincipalName", "Tuser29@deepidentity.onmicrosoft.com");
                    var password = "pass@1234";
                    var ssPassword = new SecureString();
                    foreach (char c in password)
                        ssPassword.AppendChar(c);
                    command.Parameters.Add("NewPassword", ssPassword);
                    command.Parameters.Add("ForceChangePassword", false);
                    runspace.Open();
                    var pipeline = runspace.CreatePipeline();
                    pipeline.Commands.Add(command);
                    var results = pipeline.Invoke();
                    if (results.Count > 0)
                        Console.WriteLine("SUCCESS");
                    else
                        Console.WriteLine("FAIL");
                }
            }
            catch (Exception)
            {

                isChanged = false;
            }
            return isChanged;
        }

        #endregion

        public bool SetTheAutoReplyMessage(string EmailAddress, string AutoReplyMessageInternal, string AutoReplyMessageExternal)
        {
            try
            {
                Connect();
                var command = new Command("Set-MailboxAutoReplyConfiguration");
                command.Parameters.Add("Identity", EmailAddress);
                command.Parameters.Add("AutoReplyState", "Enabled");

                command.Parameters.Add("InternalMessage", AutoReplyMessageInternal);
                command.Parameters.Add("ExternalMessage", AutoReplyMessageExternal);
                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool DisableAutoReplyMessage(string EmailAddress)
        {
            try
            {
                Connect();
                var command = new Command("Set-MailboxAutoReplyConfiguration");
                command.Parameters.Add("Identity", EmailAddress);
                command.Parameters.Add("AutoReplyState", "Disabled");


                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool SetTheMailForwardingAddress(string EmailAddress, string MailForwardingAddress)
        {
            try
            {
                Connect();
                var command = new Command("Set-Mailbox");
                command.Parameters.Add("Identity", EmailAddress);
                command.Parameters.Add("ForwardingAddress", MailForwardingAddress);
                command.Parameters.Add("DeliverToMailboxAndForward", true);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool DisableMailForwardingAddress(string EmailAddress)
        {
            try
            {
                Connect();
                var command = new Command("Set-Mailbox");
                command.Parameters.Add("Identity", EmailAddress);
                command.Parameters.Add("ForwardingAddress", null);
                command.Parameters.Add("DeliverToMailboxAndForward", false);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool AddEmailAddressToMailbox(string UPN, string SAMAccountName, string primaryEmailId, string emailAddress)
        {
            try
            {
                RemoveEmailAddressFromMailBox(UPN, SAMAccountName + "@smrtlan.com.sg");
                Connect();
                var command = new Command("set-mailbox");
                command.Parameters.Add("Identity", UPN);
                List<string> emailIds = emailAddress.Split(',').ToList();

                ArrayList emailAddresses = new ArrayList();
                emailAddresses.Add("SMTP:" + primaryEmailId.ToLower());
                foreach (string email in emailIds)
                    emailAddresses.Add("smtp:" + String.Format("{0}@{1}", SAMAccountName.Trim().ToLower(), email.Trim().ToLower()));

                command.Parameters.Add("EmailAddresses", emailAddresses);


                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);
                var results = pipeline.Invoke();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool RemoveEmailAddressFromMailBox(string UPN, string emailAddress)
        {
            try
            {
                Connect();
                var command = new Command("set-mailbox");
                command.Parameters.Add("Identity", UPN);
                ArrayList emailAddresses = new ArrayList();
                var addresses = new Hashtable();
                addresses.Add("remove", emailAddress);
                emailAddresses.Add(emailAddress);
                command.Parameters.Add("EmailAddresses", addresses);


                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SetMailboxDatabase(string UPN, string Database)
        {
            try
            {
                Connect();
                var command = new Command("New-MoveRequest");
                command.Parameters.Add("Identity", UPN);
                command.Parameters.Add("TargetDatabase", Database);



                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool MailEnableUser(string UserName, string EMail)
        {
            try
            {
                Connect();
                var command = new Command("Enable-MailUser");
                command.Parameters.Add("Identity", UserName);
                command.Parameters.Add("ExternalEmailAddress", EMail);


                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool EnableMailBox(string ObjectName, string EMail)
        {
            try
            {
                Connect();
                var command = new Command("Enable-Mailbox");
                command.Parameters.Add("Identity", ObjectName);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool DisableMailBox(string ObjectName)
        {
            try
            {
                Connect();
                var command = new Command("Disable-Mailbox");
                command.Parameters.Add("Identity", ObjectName);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool MailEnableSecurityGroup(string GroupName)
        {
            try
            {
                Connect();

                var command1 = new Command("Set-Group");
                command1.Parameters.Add("Identity", GroupName);
                command1.Parameters.Add("-Universal ", true);
                var pipeline1 = dummyRunspace.CreatePipeline();
                pipeline1.Commands.Add(command1);
                var results1 = pipeline1.Invoke();

                var command = new Command("Enable-DistributionGroup");
                command.Parameters.Add("Identity", GroupName);
                command.Parameters.Add("Alias", GroupName);
                command.Parameters.Add("PrimarySMTPAddress", GroupName);
                command.Parameters.Add("DisplayName", GroupName);
                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool SetPrimarySmtpAddress(string UPN, string EmailAddress)
        {
            try
            {
                Connect();
                var command = new Command("Set-Mailbox");
                command.Parameters.Add("Identity", UPN);
                command.Parameters.Add("EmailAddressPolicyEnabled", false);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                pipeline.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EnableEmailAddressPolicy(string UPN)
        {
            try
            {
                Connect();
                var command = new Command("Set-Mailbox");
                command.Parameters.Add("Identity", UPN);
                command.Parameters.Add("EmailAddressPolicyEnabled", true);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DisableEmailAddressPolicy(string UPN)
        {
            try
            {
                Connect();
                var command = new Command("Set-Mailbox");
                command.Parameters.Add("Identity", UPN);
                command.Parameters.Add("EmailAddressPolicyEnabled", false);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HideFromAddressBook(string UPN)
        {
            try
            {
                Connect();
                var command = new Command("set-mailbox");
                command.Parameters.Add("Identity", UPN);
                command.Parameters.Add("HiddenFromAddressListsEnabled", true);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HideGroupFromAddressBook(string UPN)
        {
            try
            {
                Connect();
                var command = new Command("Set-DistributionGroup");
                command.Parameters.Add("Identity", UPN);
                command.Parameters.Add("HiddenFromAddressListsEnabled", true);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ShowInAddressBook(string UPN)
        {
            try
            {
                Connect();
                var command = new Command("set-mailbox");
                command.Parameters.Add("Identity", UPN);
                command.Parameters.Add("HiddenFromAddressListsEnabled", false);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public bool IsSMTPAlreadyExisting(string EmailId)
        {
            try
            {
                Connect();
                var command = new Command("Get-Recipient");
                command.Parameters.Add("Identity", EmailId);

                var pipeline = dummyRunspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                if (results.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            return true;
        }
        private DirectoryEntry GetDirectoryEntry(objectClass objectCls, string objectName, string LdapDomain)
        {
            DirectorySearcher mySearcher = new DirectorySearcher(m_root);

            switch (objectCls)
            {
                case objectClass.user:
                    mySearcher.Filter = "(&(objectClass=user)(|(cn=" + objectName + ")(sAMAccountName=" + objectName + ")))";
                    break;
                case objectClass.group:
                    mySearcher.Filter = "(&(objectClass=group)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                    break;

            }
            SearchResult result = mySearcher.FindOne();

            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                objectName + " in the " + LdapDomain + " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();

            return directoryObject;
        }

        public override IMProcessor IMProcessor
        {
            get
            {
                Exchange.IMProcessor processor = new Exchange.IMProcessor();

                processor.EndPoint = this.EndPoint;
                processor.ExchangeConfiguration = this.ExchangeConfiguration;


                return processor;
            }
        }

        private ICollection<PSObject> getUsersByRunspace()
        {
            ICollection<PSObject> gpcOutput = null;

            using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(10)))
            {
                try
                {
                    rsp1.StateChanged += rsp_StateChanged;
                    rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                    rsp1.Open();

                    while (!IsRunSpaceOpened)
                    {
                        System.Threading.Thread.Sleep(ThreadSleeperTime);
                    }

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("users runspace connected and open {0}", rsp1.InstanceId));

                    using (PowerShell gpc = PowerShell.Create())
                    {
                        gpc.InvocationStateChanged += powershell_InvocationStateChanged;

                        gpc.Runspace = rsp1;
                        gpc.AddCommand("Get-User");
                        gpc.AddParameter("ResultSize", ResultSize);
                        gpc.AddParameter("RecipientTypeDetails", "Usermailbox");
                        if (IsCrossDomain)
                        {
                            gpc.AddParameter("DomainController", DomainController);
                        }

                        gpcOutput = gpc.Invoke();
                    }
                    if (gpcOutput != null)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("users fetched from runspace {0}", gpcOutput.Count()));
                    }
                    else
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("users fetched from runspace {0}", 0));
                    }


                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Exception, ex.Message, ex);
                }
            }

            return gpcOutput;
        }
        private ICollection<PSObject> getUserMailQuotaByRunspace()
        {
            ICollection<PSObject> gpcOutput = null;

            using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(10)))
            {
                try
                {
                    rsp1.StateChanged += rsp_StateChanged;
                    rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                    rsp1.Open();

                    while (!IsRunSpaceOpened)
                    {
                        System.Threading.Thread.Sleep(ThreadSleeperTime);
                    }
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("mail quota runspace connected and open {0}", rsp1.InstanceId));

                    using (PowerShell gpc = PowerShell.Create())
                    {
                        gpc.InvocationStateChanged += powershell_InvocationStateChanged;

                        gpc.Runspace = rsp1;

                        gpc.AddCommand("GET-MAILBOX");
                        gpc.AddParameter("ResultSize", ResultSize);
                        if (IsCrossDomain)
                        {
                            gpc.AddParameter("DomainController", DomainController);
                        }

                        gpcOutput = gpc.Invoke();
                    }

                    if (gpcOutput != null)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("userMailQuota fetched from runspace {0}", gpcOutput.Count()));
                    }
                    else
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("userMailQuota fetched from runspace {0}", 0));
                    }

                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Exception, ex.Message, ex);
                }
            }

            return gpcOutput;
        }
        private ICollection<PSObject> geGroupsByRunspace()
        {
            ICollection<PSObject> gpcOutput = null;

            using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(10)))
            {
                try
                {
                    rsp1.StateChanged += rsp_StateChanged;
                    rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                    rsp1.Open();

                    while (!IsRunSpaceOpened)
                    {
                        System.Threading.Thread.Sleep(ThreadSleeperTime);
                    }
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("group runspace connected and open {0}", rsp1.InstanceId));

                    using (PowerShell gpc = PowerShell.Create())
                    {
                        gpc.InvocationStateChanged += powershell_InvocationStateChanged;

                        gpc.Runspace = rsp1;
                        gpc.AddCommand("Get-DistributionGroup");
                        gpc.AddParameter("ResultSize", ResultSize);
                        if (IsCrossDomain)
                        {
                            gpc.AddParameter("DomainController", DomainController);
                        }

                        gpcOutput = gpc.Invoke();
                    }

                    if (gpcOutput != null)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("exmailGroups fetched from runspace {0}", gpcOutput.Count()));
                    }
                    else
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("exmailGroups fetched from runspace {0}", 0));
                    }

                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Exception, ex.Message, ex);
                }
            }

            return gpcOutput;
        }
        private void rsp_AvailabilityChanged(object sender, RunspaceAvailabilityEventArgs e)
        {
            //WriteDiscoveryLog(LogLevelType.Info, string.Format("run space availability {0}", e.RunspaceAvailability));
            if (e.RunspaceAvailability == RunspaceAvailability.None)
            {
                System.Threading.Thread.Sleep(ThreadSleeperTime);
                IsRunSpaceOpened = false;
            }
        }
        private void powershell_InvocationStateChanged(object sender, PSInvocationStateChangedEventArgs e)
        {
            //  WriteDiscoveryLog(LogLevelType.Info, string.Format("Powershel state change {0}", e.InvocationStateInfo.State));

        }
        private void rsp_StateChanged(object sender, RunspaceStateEventArgs e)
        {
            //  WriteDiscoveryLog(LogLevelType.Info, string.Format("run space statechanged {0}", e.RunspaceStateInfo.State + " - " + e.RunspaceStateInfo.Reason));

            if (e.RunspaceStateInfo.State == RunspaceState.Broken || e.RunspaceStateInfo.State == RunspaceState.BeforeOpen
                || e.RunspaceStateInfo.State == RunspaceState.Opening)
            {
                IsRunSpaceOpened = false;
                System.Threading.Thread.Sleep(ThreadSleeperTime);
            }

            if (e.RunspaceStateInfo.State == RunspaceState.Opened)
            {
                IsRunSpaceOpened = true;
            }
        }
    }
}
