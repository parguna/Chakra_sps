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

namespace DI.Chakra.Connector
{
    public class Exchange2013Connector : ConnectorBase
    {

        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();

        private Dictionary<long, string> dicGroups = new Dictionary<long, string>();

        private Dictionary<string, ArrayList> dicGroupUsers = new Dictionary<string, ArrayList>();

        private DirectoryEntry m_root = null;

        private Runspace runspace = null;


        private ExConfiguration m_exConfig = null;

      
        private bool Isoffice365 = true;

        private bool IsConnected = false;

        public Exchange2013Connector(EndPoint endpoint, AccessRequest currentRequest)
        {
            this.EndPoint = endpoint;
            this.CurrentRequest = currentRequest;
            var exConfig = (ExConfiguration)EndPoint.GetConfiguration(endpoint);
            exConfig.Password = RSAHelper.DecryptData(exConfig.Password);
            this.ExchangeConfiguration = exConfig;
        }

        public ExConfiguration ExchangeConfiguration
        {
            get
            {
                return m_exConfig;
            }

            set
            {
                this.m_exConfig = value;
            }

        }

        
        #region IConnector Members


        public override bool Connect()
        {
            bool bReturn = false;
            Isoffice365 = false;
            if (!Isoffice365)
            {
                bReturn = ConnectToExchangeServer();
            }
            else
            {
                bReturn = ConnectToOffice365();
            }

            return bReturn;
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
            IList<ExUserInfo> userlist = null;
            ExUserInfo.SaveUsers(userlist,EndPoint.Id);
            userlist = null;
            userlist = ExUserInfo.GetDeletedUsers(this.EndPoint.Id, this.Process);
            ExUserInfo.RemoveExUser(userlist.ToList());
        }

        private void DiscoverGroups()
        {
            //runspace.Open();
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Groups"));
            this.GetGroups();
            IList<ExGroupInfo> listExGroups = new List<ExGroupInfo>();
            listExGroups = ExGroupInfo.GetDeletedExGroups(this.EndPoint.Id);
            ExGroupInfo.RemoveExGroup(listExGroups.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Groups"));
        }

        public override void GetData()
        {
            #region  Test Code
            ProvisioningForm form;
            ExUserInfo exUserValue = new ExUserInfo();

           // ProvisioningFormFacade FormFacade = new ProvisioningFormFacade();

            //  ProvisioningFormEX exForm = form.FormDetails as ProvisioningFormEX;
            //if (exForm == null)
            //    exForm = FormFacade.GetProvisioningFormAD(form.ProvisioningFormId);


            // bool created = CreateUser(null, null);
            //  bool updated = UpdateUser(null);
            //  bool deleted = DeleteUser(null);
            //  bool adgrps = AddEntitlementsToUser(null, EntitlementType.Group);
            //   bool remgrps = RemoveEntitlementsFromUser(null, EntitlementType.Group);
            // bool chngd = this.Changeoffice365MailPassword(null, null, 1);


            #endregion
            long DiscoveryProcessId = base.DiscoveryProcess.Id;

            List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

            bool IsUserSelected = true;// EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_EX_User) != null;
            bool IsGroupSelected = true;// EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_EX_Group) != null;

          
            IList<ExUserInfo> listExUsers = null;
            IList<User> listUser = null;

            try
            {
                if (IsUserSelected)
                {
                    DiscoverUsers();
                }
                dicUsers = ExUserInfo.getUserIds();


                if (IsGroupSelected)
                {
                    DiscoverGroups();
                    dicGroups = ExGroupInfo.GetEXGroupInfo();
                    SaveUserGroups();
                }


                ExUserInfo.MapWithMasterUser(EndPoint, ExchangeConfiguration.MapAll != null ? ExchangeConfiguration.MapAll.Value : false);
                ExUserInfo.MapWithMasterUserForReconTable(EndPoint, ExchangeConfiguration.MapAll != null ? ExchangeConfiguration.MapAll.Value : false);
                ExUserInfo.PopulateMasterUserFromEx(EndPoint);


                //EndpointUserMap userMap = new EndpointUserMap("DI_Ex_UserInfo", "sAMAccountName");
                //userMap.MapAll = ExchangeConfiguration.MapAll.Value;
                //userMap.MapWithMasterUser(EndPoint);
                //userMap.MapWithMasterUserForReconTable(EndPoint);



            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        private void GetGroups()
        {
            IList<ExGroupInfo> list = new List<ExGroupInfo>();

            ExGroupInfo excInfo = null;
            GroupInfoOrchestrator orchestrator = new GroupInfoOrchestrator();

            // //DILogDiscovery.WriteLog(LogType.INFO, string.Format("Found {0} group(s) objects ", seCol.Count.ToString()));

            orchestrator.TenantId = this.EndPoint.TenantId;
            orchestrator.EndpointSystemId = this.EndPoint.Id;

            var command = new Command("Get-DistributionGroup");
            ScriptBlock filter3 = ScriptBlock.Create("100000");
            command.Parameters.Add("resultsize", filter3);
            var pipeline = runspace.CreatePipeline();
            pipeline.Commands.Add(command);

            var results = pipeline.Invoke();
            Dictionary<string, string> dc = new Dictionary<string, string>();
            List<string> lst = new List<string>();
            foreach (PSObject ps in results)
            {

                excInfo = new ExGroupInfo();
                excInfo = orchestrator.PopulateGroup(ps);
                if (null != excInfo)
                    list.Add(excInfo);

                Command getDistGroup = new Command("Get-DistributionGroupMember");
                getDistGroup.Parameters.Add("identity", excInfo.name);
                var pipeline2 = runspace.CreatePipeline();
                pipeline2.Commands.Add(getDistGroup);
                ArrayList mem = new ArrayList();

                ICollection<PSObject> results2 = pipeline2.Invoke();
                foreach (PSObject ps1 in results2)
                {
                    mem.Add(ps1.Members["DistinguishedName"].Value.ToString());

                }

                dicGroupUsers.Add(excInfo.distinguishedName, mem);

                //DILogDiscovery.WriteLog(LogType.INFO, string.Format("Populated {0} group(s) objects InMemory ", list.Count.ToString()));

            }
            if (list.Count > 0)
            {
                ExGroupInfo.SaveGroups(list);

                // dicGroups = ExchangeFacade.GetEXGroupInfo();
            }
        }


        private IList<ExUserInfo> GetUsers()
        {
            IList<ExUserInfo> list = new List<ExUserInfo>();
            ExUserInfo excUserInfo = null;



            UserInfoOrchestrator orchestrator = new UserInfoOrchestrator();
            orchestrator.TenantId = this.EndPoint.TenantId;
            orchestrator.EndpointSystemId = this.EndPoint.Id;
            //orchestrator.HistoryId = this.HistoryId;


            var command = new Command("Get-User");
            ScriptBlock filter3 = ScriptBlock.Create("100000");
            command.Parameters.Add("resultsize", filter3);

            var pipeline = runspace.CreatePipeline();
            pipeline.Commands.Add(command);

            var results = pipeline.Invoke();
            Dictionary<string, string> dc = new Dictionary<string, string>();
            List<string> lst = new List<string>();
            foreach (PSObject ps in results)
            {

                excUserInfo = orchestrator.PopulateExchangeUser(ps);

                if (null != excUserInfo)
                    list.Add(excUserInfo);


                if (list.Count() % 10 == 0)
                {
                    ExUserInfo.SaveUsers(list, EndPoint.Id);
                    list.Clear();
                }


            }


            //DILogDiscovery.WriteLog(LogType.INFO, string.Format("Populated {0} Exchange User(s) objects InMemory ", list.Count.ToString()));

            return list;

        }

        //public override void AddUnmappedUsers(int _processId)
        //{
        //    try
        //    {
        //        if (!EndPoint.MapIacm.Value)
        //            return;
        //        ExchangeFacade.AddAllUnmapperUsers(EndPoint.EndpointSystemId, _processId);
        //        //ActiveDirectoryFacade.AddAllUnmapperUsers(EndPoint.Endpointsystemid, _processId);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        #region SAVE USER GROUPS
        private void SaveUserGroups()
        {

            ExUserInfo.SaveUserGroupMap(dicUsers, dicGroups, dicGroupUsers, this.EndPoint.Id);
        }
        #endregion

        public override bool CreateUser(IEndpointUserInfo _userInfo, IProvisioningForm _formInfo)
        {
            ExUserInfo exUser = (ExUserInfo)_userInfo;
            try
            {
                bool isMailboxCreated = CreateExchangemailBox(_userInfo, _formInfo);
                if (isMailboxCreated)
                {
                    System.Threading.Thread.Sleep(10000);
                    DisableEmailAddressPolicy(exUser.UserPrincipalName);
                    //SetPrimarySmtpAddress(exUser.UserPrincipalName, exUser.mail);
                    System.Threading.Thread.Sleep(5000);



                }



                return isMailboxCreated;
            }
            catch (Exception ex)
            {
                //DILogProvisioningLogger.WriteLog(null, LogLevelType.Error, string.Format("Error while creating mailbox for {0}", exUser.displayName), ex, ProvisioningEventType.OnBoarding, "Provisioning Engine");
            }
            
            return false;
        }

        public void FreeResources()
        {
            runspace.Close();
            System.Threading.Thread.Sleep(500);
            runspace.Dispose();
            //GC.Collect();
            System.Threading.Thread.Sleep(500);
  
        }



        public override bool UpdateUser(IEndpointUserInfo _userInfo)
        {
            ExUserInfo EXUserInfo = (ExUserInfo)_userInfo;
            bool isUpdated = false;


            ////For Test
            //EXUserInfo = new ExUserInfo();
            //EXUserInfo.FirstName = "Kishore";
            //EXUserInfo.LastName = "Ganti";
            //EXUserInfo.displayName = "Kishore";
            //EXUserInfo.department = "DIDev";
            //EXUserInfo.title = "DIDev";
            //EXUserInfo.company = "DeepIdentity";
            //EXUserInfo.mobile = "98214366";
            //EXUserInfo.CountryOrRegion = "Singapore";
            //EXUserInfo.UserPrincipalName = "Tuser28@deepidentity.onmicrosoft.com";


            try
            {
                if (this.Connect())
                {
                    var command = new Command("Set-User");
                    command.Parameters.Add("DisplayName", EXUserInfo.displayName);
                    command.Parameters.Add("department", EXUserInfo.department);
                    command.Parameters.Add("Title", EXUserInfo.title);
                    command.Parameters.Add("Company", EXUserInfo.company);
                    command.Parameters.Add("Mobile", EXUserInfo.mobile);
                    command.Parameters.Add("Firstname", EXUserInfo.FirstName);
                    command.Parameters.Add("Lastname", EXUserInfo.LastName);
                    command.Parameters.Add("CountryOrRegion", EXUserInfo.CountryOrRegion);
                    command.Parameters.Add("Identity", EXUserInfo.UserPrincipalName);

                    var pipeline = runspace.CreatePipeline();
                    pipeline.Commands.Add(command);
                    var results = pipeline.Invoke();
                    if (results.Count > 0)
                        isUpdated = true;

                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
                //DILogProvisioning.WriteLog(LogType.INFO, string.Format("Failed to Update  for user {0} ", EXUserInfo.name), ex);
            }

            return isUpdated;
        }

        public override bool LockUser(IEndpointUserInfo _userInfo)
        {
            //NOTE: Disable user Mailbox
            ExUserInfo _Userinfo = (ExUserInfo)_userInfo;
            bool isLocked = false;
            try
            {
                if (this.Connect())
                {

                    //var command = new Command("Disable-Mailbox");
                    //command.Parameters.Add("Identity", _Userinfo.UserPrincipalName);
                    //command.Parameters.Add("Confirm", false);
                    //var pipeline = runspace.CreatePipeline();
                    //pipeline.Commands.Add(command);

                    //var results = pipeline.Invoke();
                    return true;

                    

                }
            }
            catch (Exception ex)
            {

                isLocked = false;
                //DILogProvisioning.WriteLog(LogType.INFO, string.Format("Failed to Disable  user {0} ", _Userinfo.name), ex);
            }
            return isLocked;
        }

        public override bool UnLockUser(IEndpointUserInfo _userInfo)
        {

            ExUserInfo _Userinfo = (ExUserInfo)_userInfo;
            bool isUnLocked = false;
            try
            {
                if (this.Connect())
                {
                    var command = new Command("Enable-Mailbox");
                    command.Parameters.Add("Identity", _Userinfo.mail);
                    runspace.Open();
                    var pipeline = runspace.CreatePipeline();
                    pipeline.Commands.Add(command);
                    var results = pipeline.Invoke();
                    if (results.Count > 0)
                        isUnLocked = true;
                    else
                        isUnLocked = false;
                }
            }
            catch (Exception ex)
            {

                isUnLocked = false;
                //DILogProvisioning.WriteLog(LogType.INFO, string.Format("Failed to enable  user {0} ", _Userinfo.name), ex);
            }
            return isUnLocked;
        }

        public override bool EnableUser(IEndpointUserInfo _userInfo)
        {
            ExUserInfo exUser = (ExUserInfo)_userInfo;
            try
            {
                EnableMailBox(exUser.UserPrincipalName, exUser.mail);
                //DILogProvisioningLogger.WriteLog(this.CurrentRequest, EndPoint, LogLevelType.Info, _userInfo.AccountName + " is Enabled in Microsoft Exchange", null, ProvisioningEventType.Others, "Provisioning Engine");
                return true;
            }
            catch (Exception ex)
            {
                //DILogProvisioningLogger.WriteLog(this.CurrentRequest, EndPoint, LogLevelType.Error, ex.Message, ex, ProvisioningEventType.Others, "Provisioning Engine");
                return false;
            }
        }

        public override bool DisableUser(IEndpointUserInfo _userInfo)
        {
            ExUserInfo exUser = (ExUserInfo)_userInfo;
            try
            {
                //LockUser(_userInfo);
                HideFromAddressBook(exUser.UserPrincipalName);
                return true;
            }
            catch (Exception ex)
            {
                //DILogProvisioning.WriteLog(LogType.INFO, string.Format("Failed to disable  user {0} ",exUser.name), ex);
                return false;
            }
        }

        public override bool DeleteUser(IEndpointUserInfo _userInfo)
        {
            bool bReturn = false;
            if (!Isoffice365)
            {
                bReturn = DeleteExchangeMailBox(_userInfo);
            }
            else
                bReturn = DeleteOffice365MailBox(_userInfo);

            return bReturn;
        }

        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            bool isAdded = false;
            try
            {
                ExUserInfo EXUserInfo = (ExUserInfo)_userInfo;


                //For testing..
                EXUserInfo = new ExUserInfo();
                EXUserInfo.UserPrincipalName = "Tuser28@deepidentity.onmicrosoft.com";
                IList<string> grp = new List<string>();
                grp.Add("DIDevelopers");
                grp.Add("DISales");
                grp.Add("DIHumanResources");
                EXUserInfo.Groups = grp;


                this.Connect();

               // EndPointFacade EndPointFacade = new EndPointFacade();
               // EndPoint EndPoint = EndPointFacade.GetEndPointSystemByKey(EXUserInfo.EndPointSystemId);


                //var command = new Command("Add-DistributionGroupMember");
                //command.Parameters.Add("Identity", EXUserInfo.Groups);
                //command.Parameters.Add("Member", EXUserInfo.mail);

                foreach (var grpName in EXUserInfo.Groups)
                {
                    var command = new Command("Add-DistributionGroupMember");
                    command.Parameters.Add("Identity", grpName);
                    command.Parameters.Add("Member", EXUserInfo.UserPrincipalName);

                    var pipeline = runspace.CreatePipeline();
                    pipeline.Commands.Add(command);

                    var results = pipeline.Invoke();

                    isAdded = true;

                }

            }
            catch (Exception ex)
            {

                //DILogProvisioning.WriteLog(LogType.ERROR, ex.Message);
                isAdded = false;

            }
         

            return isAdded;
        }

        public override bool RemoveAllEntitlements(IEndpointUserInfo _userInfo)
        {
            return base.RemoveAllEntitlements(_userInfo);
        }

        public bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            bool isRemoved = false;
            try
            {

                ExUserInfo EXUserInfo = (ExUserInfo)_userInfo;

                

                //EndPointFacade EndPointFacade = new EndPointFacade();
               // EndPoint EndPoint = EndPointFacade.GetEndPointSystemByKey(EXUserInfo.EndPointSystemId);


             
                foreach (var grpName in EXUserInfo.Groups)
                {
                    var command = new Command("Remove-DistributionGroupMember");
                    command.Parameters.Add("Identity", grpName);
                    command.Parameters.Add("Member", EXUserInfo.UserPrincipalName);
                    command.Parameters.Add("Confirm", false);

                    var pipeline = runspace.CreatePipeline();
                    pipeline.Commands.Add(command);

                    var results = pipeline.Invoke();

                    isRemoved = true;
                }


            }
            catch (Exception ex)
            {
                //DILogProvisioning.WriteLog(LogType.ERROR, ex.Message);
                m_root.Close();
                isRemoved = false;
            }

          
            return isRemoved;
        }



        #region Connector Helper Methods

        private bool ConnectToExchangeServer()
        {
            bool _returnValue = false;
            try
            {
                if (!IsConnected)
                {
                    this.ExchangeConfiguration = (ExConfiguration)EndPoint.GetConfiguration(EndPoint);
                    string userName = ExchangeConfiguration.UserName;
                    ExchangeConfiguration.Password = RSAHelper.DecryptData(ExchangeConfiguration.Password);
                    string password = ExchangeConfiguration.Password;
                    string ipaddress = ExchangeConfiguration.IPAddress;
                    string url = string.Format("http://{0}{1}", ExchangeConfiguration.DomainName, "/PowerShell");

                    SecureString ssRunasPassword = new SecureString();
                    foreach (char x in password)
                        ssRunasPassword.AppendChar(x);
                    PSCredential credentials = new PSCredential(userName, ssRunasPassword);

                    var connInfo = new WSManConnectionInfo(new Uri(url), "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credentials);

                    connInfo.AuthenticationMechanism = AuthenticationMechanism.Kerberos;

                    runspace = RunspaceFactory.CreateRunspace(connInfo);

                    runspace.Open();
                    _returnValue = true;
                    IsConnected = true;
                    //DILogDiscovery.WriteLog(LogType.INFO, string.Format("Exchange Server {0} Connected",
                                                                // EndPoint.IpAddress));
                }
                else
                {
                    _returnValue = true;
                }
            }
            catch (Exception)
            {
                _returnValue = false;
                //DILogDiscovery.WriteLog(LogType.INFO, string.Format(" Failed to Connect to Exchange Server ",
                                                             //EndPoint.IpAddress));
            }

            return _returnValue;
        }

        private bool ConnectToOffice365()
        {
            bool isConnected = false;
            try
            {
                this.ExchangeConfiguration = (ExConfiguration)EndPoint.GetConfiguration(EndPoint);

                string userName = ExchangeConfiguration.UserName;
                string password = ExchangeConfiguration.Password;
                string ipaddress = ExchangeConfiguration.IPAddress;
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

                runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(connInfo);
                runspace.Open();
                isConnected = true;
                //DILogDiscovery.WriteLog(LogType.INFO, string.Format("Office365 {0} Connected",
                                                          //EndPoint.IpAddress));

            }
            catch (Exception)
            {

                isConnected = false;
                //DILogDiscovery.WriteLog(LogType.INFO, string.Format(" Failed to Connect to Office365 ",
                                                      //EndPoint.IpAddress));
            }

            return isConnected;
        }

        #endregion


        #region  Exchange Server

        public bool CreateExchangemailBox(IEndpointUserInfo _userInfo, IProvisioningForm _formInfo)
        {
            bool isMailboxCreated = false;
            ExUserInfo _exUserInfo = (ExUserInfo)_userInfo;
            try
            {

                if (this.Connect())
                {
                    var domainName = ExchangeConfiguration.DomainName; ;
                    var password = _exUserInfo.NewUsrPassword; ;
                    var ssPassword = new SecureString();
                    foreach (char c in password)
                        ssPassword.AppendChar(c);

                    var command = new Command("New-Mailbox");
                    if (_exUserInfo.displayName.Trim() == string.Empty)
                        _exUserInfo.displayName = _exUserInfo.LastName.Trim();

                    command.Parameters.Add("Name", _exUserInfo.LastName.Trim());
                    command.Parameters.Add("Alias", _exUserInfo.sAMAccountName);
                    command.Parameters.Add("UserPrincipalName", _exUserInfo.sAMAccountName + "@" + domainName);
                    command.Parameters.Add("FirstName", _exUserInfo.givenName);
                    command.Parameters.Add("LastName", _exUserInfo.LastName);
                    command.Parameters.Add("DisplayName", _exUserInfo.displayName.Trim());
                    command.Parameters.Add("Password", ssPassword);
                    command.Parameters.Add("ResetPasswordOnNextLogon", false);
                    command.Parameters.Add("Database", _exUserInfo.Database);
                    command.Parameters.Add("OrganizationalUnit", _exUserInfo.OrganizationalUnit);


                    var pipeline = runspace.CreatePipeline();
                    pipeline.Commands.Add(command);

                    var results = pipeline.Invoke();
                    if (results.Count > 0)
                    {
                        isMailboxCreated = true;
                    }
                    else
                    {
                        command = new Command("New-Mailbox");
                        command.Parameters.Add("Name", _exUserInfo.sAMAccountName);
                        command.Parameters.Add("Alias", _exUserInfo.sAMAccountName);
                        command.Parameters.Add("UserPrincipalName", _exUserInfo.sAMAccountName + "@" + domainName);
                        command.Parameters.Add("FirstName", _exUserInfo.givenName);
                        command.Parameters.Add("LastName", _exUserInfo.sAMAccountName);
                        command.Parameters.Add("DisplayName", _exUserInfo.sAMAccountName);
                        command.Parameters.Add("Password", ssPassword);
                        command.Parameters.Add("ResetPasswordOnNextLogon", false);
                        command.Parameters.Add("Database", _exUserInfo.Database);
                        command.Parameters.Add("OrganizationalUnit", _exUserInfo.OrganizationalUnit);


                        pipeline = runspace.CreatePipeline();
                        pipeline.Commands.Add(command);
                        results = pipeline.Invoke();
                        if (results.Count > 0)
                        {
                            isMailboxCreated = true;
                        }

                    }

                    pipeline.Dispose();
                }

            }
            catch (Exception ex)
            {
                isMailboxCreated = false;
                //DILogProvisioningLogger.WriteLog(CurrentRequest, LogLevelType.Error, string.Format("Failed to Create Mailbox for user {0} ", _exUserInfo.name), ex, ProvisioningEventType.OnBoarding, "Provisioning Engine");
            }
            

            return isMailboxCreated;
        }

        private bool DeleteExchangeMailBox(IEndpointUserInfo _userInfo)
        {
            bool isRemoved = false;
            ExUserInfo _Userinfo = (ExUserInfo)_userInfo;
            try
            {

                if (this.Connect())
                {
                    var command = new Command("Remove-Mailbox");
                    command.Parameters.Add("Identity", _Userinfo.mail);
                    command.Parameters.Add("Permanent", true);
                    command.Parameters.Add("Confirm", false);

                    var pipeline = runspace.CreatePipeline();
                    pipeline.Commands.Add(command);

                    var results = pipeline.Invoke();
                    if (results.Count > 0)
                    {
                        isRemoved = true;
                        //DILogProvisioning.WriteLog(LogType.INFO, string.Format("Mailbox removed Successfully  for user  {0} ", _Userinfo.name));
                    }
                    else
                        isRemoved = false;


                }
            }
            catch (Exception ex)
            {
                isRemoved = false;
                //DILogProvisioning.WriteLog(LogType.INFO, string.Format("Failed to remove Mailbox    for user  {0} ", _Userinfo.name));
            }
            return isRemoved;
        }

        public bool CreateSecurityGroup(string groupName, string groupDesc, string OU, string EmailAddress, ADSUserInfo GroupOwner)
        {

            try
            {
                Connect();
                var command = new Command("New-DistributionGroup");
                command.Parameters.Add("Name", groupName);
                command.Parameters.Add("SamAccountName", groupName);
                command.Parameters.Add("PrimarySmtpAddress", EmailAddress);
                command.Parameters.Add("Type", "Security");
                if (GroupOwner != null)
                    command.Parameters.Add("ManagedBy", GroupOwner.mail);
                command.Parameters.Add("OrganizationalUnit", OU);

                var pipeline = runspace.CreatePipeline();
                pipeline.Commands.Add(command);

                var results = pipeline.Invoke();
                return true;
            }
            catch (Exception Ex)
            {
                //DILogProvisioningLogger.WriteLog(this.CurrentRequest, EndPoint, LogLevelType.Error, Ex.Message, Ex, ProvisioningEventType.Others, "Provisioning Engine");
                return false;
            }
        }

      


        #endregion


        #region Office 365

        private bool CreateOffice365MailBox(IEndpointUserInfo _userInfo, IProvisioningForm _formInfo)
        {
            bool isMailboxCreated = false;
            ExUserInfo _exUserInfo = (ExUserInfo)_userInfo;

            //For testing
            _exUserInfo = new ExUserInfo();
            _exUserInfo.FirstName = "Test";
            _exUserInfo.LastName = "User29";
            _exUserInfo.name = "Tuser29";
            ExchangeConfiguration.DomainName = "deepidentity.onmicrosoft.com";
            _exUserInfo.NewUsrPassword = "@bcd1234";


            try
            {

                if (this.Connect())
                {
                    var domainName = ExchangeConfiguration.DomainName;
                    var password = _exUserInfo.NewUsrPassword;
                    var ssPassword = new SecureString();
                    foreach (char c in password)
                        ssPassword.AppendChar(c);

                    var command = new Command("New-Mailbox");
                    command.Parameters.Add("Name", _exUserInfo.FirstName + " " + _exUserInfo.LastName);
                    command.Parameters.Add("Alias", _exUserInfo.name);
                    command.Parameters.Add("MicrosoftOnlineServicesID", _exUserInfo.name + "@" + domainName);
                    command.Parameters.Add("FirstName", _exUserInfo.FirstName);
                    command.Parameters.Add("LastName", _exUserInfo.LastName);
                    command.Parameters.Add("Password", ssPassword);
                    command.Parameters.Add("ResetPasswordOnNextLogon", false);
                    command.Parameters.Add("mail", _exUserInfo.mail);



                    var pipeline = runspace.CreatePipeline();
                    pipeline.Commands.Add(command);

                    var results = pipeline.Invoke();
                    if (results.Count > 0)
                        isMailboxCreated = true;
                    //DILogProvisioning.WriteLog(LogType.INFO, string.Format(" Created Mailbox Successfully in office 365 for user {0}.", _exUserInfo.name));
                }
            }
            catch (Exception ex)
            {
                isMailboxCreated = false;
                //DILogProvisioning.WriteLog(LogType.INFO, string.Format("Failed to Create Mailbox for user {0} ", _exUserInfo.name), ex);
            }

            return isMailboxCreated;
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

                    var pipeline = runspace.CreatePipeline();
                    pipeline.Commands.Add(command);

                    var results = pipeline.Invoke();
                    if (results.Count > 0)
                    {
                        isRemoved = true;
                        //DILogProvisioning.WriteLog(LogType.INFO, string.Format("Mailbox removed Successfully in Office 365 for user  {0} ", _Userinfo.name));
                    }
                    else
                        isRemoved = false;


                }
            }
            catch (Exception ex)
            {
                isRemoved = false;
                //DILogProvisioning.WriteLog(LogType.INFO, string.Format("Failed to remove Mailbox   for user  {0}  in Office 365", _Userinfo.name));
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
                    //  var runspace = RunspaceFactory.CreateRunspace(connInfo);
                    var command = new Command("Set-MsolUserPassword");
                    //Set-MsolUserPassword
                    //Set-MsolUser
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

                isChanged = false; ;
            }
            return isChanged;
        }

        #endregion



        #endregion

        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        //public override bool ChangePassword(string UserName, string Password, int DIUserId, bool isReactivation)
        //{
        //    throw new NotImplementedException();
        //}


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
                var pipeline = runspace.CreatePipeline();
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

              
                var pipeline = runspace.CreatePipeline();
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

                var pipeline = runspace.CreatePipeline();
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

                var pipeline = runspace.CreatePipeline();
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


                var pipeline = runspace.CreatePipeline();
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


                var pipeline = runspace.CreatePipeline();
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


                var pipeline = runspace.CreatePipeline();
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
              

                var pipeline = runspace.CreatePipeline();
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

                var pipeline = runspace.CreatePipeline();
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
                var pipeline1 = runspace.CreatePipeline();
                pipeline1.Commands.Add(command1);
                var results1 = pipeline1.Invoke();

                var command = new Command("Enable-DistributionGroup");
                command.Parameters.Add("Identity", GroupName);
                command.Parameters.Add("Alias", GroupName);
                command.Parameters.Add("PrimarySMTPAddress", GroupName);
                command.Parameters.Add("DisplayName", GroupName);
                var pipeline = runspace.CreatePipeline();
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

                var pipeline = runspace.CreatePipeline();
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

                var pipeline = runspace.CreatePipeline();
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

                var pipeline = runspace.CreatePipeline();
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

                var pipeline = runspace.CreatePipeline();
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

                var pipeline = runspace.CreatePipeline();
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

                var pipeline = runspace.CreatePipeline();
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

                var pipeline = runspace.CreatePipeline();
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

        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }

    }
}
