
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using DI.Chakra.Logger;
using System.Collections;
using DI.Chakra.Connector;
using System.Data.SqlClient;
using System.Security;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Threading;
using System.Management.Automation.Remoting;

namespace DI.Chakra.Connector
{
    public class Office365Connector : ConnectorBase
    {
        readonly IdmsLogger _idmsLogger;
        #region Constants and Variable
        private const string MSG_Connected = "{0} Host {1} Connected";
        private const string MSG_ConnectionFailed = "{0} Host {1} Connection Failed";
        private const string PROCESSNAME = "DiscoveryProcess";
        //private readonly string o365Uri = "https://ps.outlook.com/PowerShell-LiveID?PSVersion=2.0";
        private const string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";
        readonly string MasterConfigHelperServiceUser;
        //private string cmdImportScriptV1 = @"$MFAExchangeModule = ((Get-ChildItem -Path $($env:LOCALAPPDATA+'\Apps\2.0\') -Filter CreateExoPSSession.ps1 -Recurse ).FullName | Select-Object -Last 1) "+Environment.NewLine+" . '$MFAExchangeModule'";
        #endregion

        #region private variables
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicGroups = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicGroupUsers = new Dictionary<string, ArrayList>();
        private string IPAddress = string.Empty;
        bool IsUserSelected = false;
        bool IsGroupSelected = false;
        bool IsPoliciesSelected = false;
        bool IsLicensesSelected = false;
        bool IsRecipientPermissionsSelected = false;
        bool IsMailBoxPermissionsSelected = false;
        bool IsMailBoxStatisticsSelected = false;
        private string[] psModules = new string[] { };
        private string psAzureADModules = string.Empty;
        #endregion

        #region Constructors
        public Office365Connector(DI.Chakra.DomainModel.EndPoint _endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            //o365Uri = MasterConfigHelper.O365ConnectionUri;
            this.Endpoint = _endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            this.IPAddress = Convert.ToString(Configuration(EndpointKey.IPAddress));
            _idmsLogger = logger;
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
            psModules = MasterConfigHelper.Office365PSModule;
            psAzureADModules = MasterConfigHelper.Office365PSAzureADModule;

        }
        #endregion

        #region Discovery
        public override bool Connect()
        {
            bool isConnected = false;
            try
            {

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Trying to Connect Connect to Office365 {0}", this.IPAddress));

                PSCredential credentials = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(iss))
                {
                    psRunSpace.Open();
                    using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                    {
                        try
                        {
                            this.WriteDiscoveryLog(LogLevelType.Info, $"Username:{this.Configuration(EndpointKey.UserName)}", null);
                            powershell.Runspace = psRunSpace;

                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credentials);
                            //connect.Parameters.Add("CommandName", new[] { "Get-DistributionGroup"});
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();

                            var err = powershell.Streams.Error.ReadAll();
                            if (err.Count() > 0)
                            {
                                string errMsg = string.Empty;
                                foreach (var msg in err)
                                {
                                    errMsg += msg;
                                }
                                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while connecting to Connect-ExchangeOnline." + errMsg, null);
                                return isConnected;
                            }
                            powershell.Commands.Clear();
                            this.WriteDiscoveryLog(LogLevelType.Info, "Connect-ExchangeOnline successful");
                            connect = new Command("Connect-MsolService");
                            connect.Parameters.Add("Credential", credentials);
                            powershell.Commands.AddCommand(connect);
                            results = powershell.Invoke();

                            //err = powershell.Streams.Error.ReadAll();
                            //if (err.Count() > 0)
                            //{
                            //    string errMsg = string.Empty;
                            //    foreach (var msg in err)
                            //    {
                            //        errMsg += msg;
                            //    }
                            //    this.WriteDiscoveryLog(LogLevelType.Exception, "Error while connecting to Connect-MsolService." + errMsg, null);
                            //    return isConnected;
                            //}
                            //powershell.Commands.Clear();
                            //this.WriteDiscoveryLog(LogLevelType.Info, "Connect-MsolService successful");
                            isConnected = true;
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("Office365 {0} Connected Successfully", this.IPAddress));
                        }
                        catch (Exception ex)
                        {
                            this.WriteDiscoveryLog(LogLevelType.Exception, "Failed to Connect to Office365", ex);
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            psRunSpace.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isConnected = false;
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Failed to Connect to Office365 {0}", this.IPAddress), ex);
            }
            return isConnected;
        }
        public override void Disconnect()
        {
            try
            {
                dicUsers?.Clear();
                dicUsers = null;

                dicGroups?.Clear();
                dicGroups = null;

                dicGroupUsers?.Clear();
                dicGroupUsers = null;
            }
            catch
            {
            }
        }
        public override void GetData()
        {
            long DiscoveryProcessId = base.DiscoveryProcess.Id;

            List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

            IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Office365_User) != null;
            IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Office365_Group) != null;
            IsPoliciesSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Office365_Policies) != null;
            IsLicensesSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Office365_Licenses) != null;
            IsRecipientPermissionsSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Office365_RecipientPermissions) != null;
            IsMailBoxPermissionsSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Office365_MailBoxPermissions) != null;
            IsMailBoxStatisticsSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Office365_MailBoxStatistics) != null;

            try
            {

                if (IsLicensesSelected)
                {
                    this.DiscoverLicenses();
                    Thread.Sleep(5000);
                }
                if (IsPoliciesSelected)
                {
                    this.DiscoverRolePolicies();
                    Thread.Sleep(5000);
                    this.DiscoverRetentionPolicies();
                    Thread.Sleep(5000);
                    this.DiscoverSharingPolicies();
                    Thread.Sleep(5000);
                }

                if (IsUserSelected || IsGroupSelected)
                {
                    DiscoverUsers();
                    Thread.Sleep(5000);
                    DiscoverGroups();
                    SaveUserGroups();
                }

                this.UpdateEntitlementRecon();
                WriteDiscoveryLog(LogLevelType.DebugStep, "Discovery MapWithMasterUserForReconTable started");
                Office365UserInfo.MapWithMasterUserForReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
                WriteDiscoveryLog(LogLevelType.DebugStep, "Discovery MapWithMasterUserForEntitlementReconTable started");
                Office365UserInfo.MapWithMasterUserForEntitlementReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Discovery failed for Office 365 system", ex);
                throw;
            }
            finally
            {
                dicUsers?.Clear();
                dicUsers = null;

                dicGroups?.Clear();
                dicGroups = null;

                dicGroupUsers?.Clear();
                dicGroupUsers = null;

                GC.Collect();
            }
        }
        private void DiscoverGroups()
        {
            IList<Office365GroupInfo> listOffice365Groups = new List<Office365GroupInfo>();
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Started discovering groups.", DiscoveryObjectType.Group);

                this.GetOffice365Groups();
                listOffice365Groups = Office365GroupInfo.GetDeletedGroups(this.Endpoint.Id, this.Process);
                Office365GroupInfo.RemoveOffice365Groups(listOffice365Groups.ToList(), this.Endpoint.Id);
                dicGroups = Office365GroupInfo.GetAllByEndPointSystemId(this.Endpoint.Id);
                WriteDiscoveryLog(LogLevelType.Info, "Completed discovering groups.", DiscoveryObjectType.Group);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                listOffice365Groups?.Clear();
                listOffice365Groups = null;
            }
        }

        private void DiscoverUsers()
        {
            IList<Office365UserInfo> userlist = null;
            List<AccountReconcilation> exsitngAccountrecon = null;
            List<Office365UserInfo> allExistingUsers = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Started discovering users.", DiscoveryObjectType.User);
                allExistingUsers = Office365UserInfo.GetAllByEndpointId(this.Endpoint.Id);
                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id).ToList();

                userlist = this.GetUsers(exsitngAccountrecon, allExistingUsers);
                Office365UserInfo.SaveUsers(userlist, this.Endpoint.Id, MasterConfigHelperServiceUser, exsitngAccountrecon, allExistingUsers);

                var listDeletedAccounts = AccountReconcilation.GetProcessDeletedAccounts(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
                foreach (var accountReconcilation in listDeletedAccounts)
                {
                    if (accountReconcilation != null && (!accountReconcilation.IsDeleted || !accountReconcilation.IsLocked))
                    {
                        EntitlementReconcilation.DeleteEntitlementsByAcountReconId(accountReconcilation.Id, this.Endpoint.Id, MasterConfigHelperServiceUser);
                        this.WriteDiscoveryLog(LogLevelType.Warning, string.Format("Reconciliation Exception: User {0} has been REMOVED without any request made in the system", accountReconcilation.FullName), DiscoveryObjectType.User);
                        accountReconcilation.IsDeleted = true;
                        accountReconcilation.IsLocked = true;
                        accountReconcilation.LastAttestedOn = null;
                        accountReconcilation.DeletedDate = DateTime.Now;
                        accountReconcilation.ExceptionType = ExceptionTypes.Removed;
                        accountReconcilation.ExceptionOccuredDate = DateTime.Now;
                        accountReconcilation.Save(MasterConfigHelperServiceUser);
                    }
                }

                userlist = Office365UserInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);

                Office365UserInfo.RemoveOffice365User(userlist.ToList());
                dicUsers = Office365UserInfo.GetAllByEndPointSystemId(this.Endpoint.Id);
                WriteDiscoveryLog(LogLevelType.Info, "Completed discovering users.", DiscoveryObjectType.User);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery Office 365 Users.", ex);
                throw ex;
            }
            finally
            {
                userlist?.Clear();
                userlist = null;

                exsitngAccountrecon?.Clear();
                exsitngAccountrecon = null;

                allExistingUsers?.Clear();
                allExistingUsers = null;
            }
        }

        private void SaveUserGroups()
        {
            Office365UserInfo.SaveUserGroupMap(dicUsers, dicGroups, dicGroupUsers, this.Endpoint.Id);
        }
        public void DiscoverLicenses()
        {
            IList<Office365License> licenselist = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Started Discovering o365 Licenses.", DiscoveryObjectType.License);

                licenselist = this.GetLicenses();
                Office365License.SaveLicenses(licenselist, this.Endpoint.Id, MasterConfigHelperServiceUser);


                WriteDiscoveryLog(LogLevelType.Info, "Completed discovering licenses.", DiscoveryObjectType.License);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                licenselist?.Clear();
                licenselist = null;
            }
        }
        private void DiscoverRolePolicies()
        {
            IList<Office365RoleAssignmentPolicy> RolePolicieslist = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Started discovering role policies.", DiscoveryObjectType.Role);
                RolePolicieslist = this.GetRolePolicy();
                Office365RoleAssignmentPolicy.SaveRolePolicy(RolePolicieslist, this.Endpoint.Id, MasterConfigHelperServiceUser);
                WriteDiscoveryLog(LogLevelType.Info, "Completed discovering role policies.", DiscoveryObjectType.Role);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                RolePolicieslist?.Clear();
                RolePolicieslist = null;
            }
        }
        private void DiscoverRetentionPolicies()
        {
            IList<Office365RetentionPolicy> RetentionPolicieslist = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Started discovering retention policies.", DiscoveryObjectType.RetentionPolicy);
                RetentionPolicieslist = this.GetRetentionPolicy();
                Office365RetentionPolicy.SaveRetentionPolicy(RetentionPolicieslist, this.Endpoint.Id, MasterConfigHelperServiceUser);

                WriteDiscoveryLog(LogLevelType.Info, "Completed discovering retention policies.", DiscoveryObjectType.RetentionPolicy);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                RetentionPolicieslist?.Clear();
                RetentionPolicieslist = null;
            }
        }
        private void DiscoverSharingPolicies()
        {
            IList<Office365SharingPolicy> RetentionPolicieslist = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Started discovering sharing policies.", DiscoveryObjectType.SharingPolicy);
                RetentionPolicieslist = this.GetSharingPolicy();
                Office365SharingPolicy.SaveSharingPolicy(RetentionPolicieslist, this.Endpoint.Id, MasterConfigHelperServiceUser);
                WriteDiscoveryLog(LogLevelType.Info, "Completed discovering sharing policies.", DiscoveryObjectType.SharingPolicy);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                RetentionPolicieslist?.Clear();
                RetentionPolicieslist = null;
            }
        }
        private void WaitForSeconds(int waitingTime = 5000)
        {
            Thread.Sleep(waitingTime);
        }
        public void GetOffice365Groups()
        {
            IList<Office365GroupInfo> list = new List<Office365GroupInfo>();
            try
            {
                Collection<PSObject> resultgroups = new Collection<PSObject>();
                GroupInfoOrchestrator orchestrator = new GroupInfoOrchestrator();
                Office365GroupInfo o365GrpInfo = null;
                PSCredential credential = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {

                        try
                        {
                            powershell.Runspace = runspace;
                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Get-DistributionGroup", "Get-DistributionGroupMember" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command Getgroups = new Command("Get-DistributionGroup");
                            powershell.Commands.AddCommand(Getgroups);
                            resultgroups = powershell.Invoke();
                            powershell.Commands.Clear();

                            foreach (PSObject psObject in resultgroups)
                            {

                                o365GrpInfo = new Office365GroupInfo();
                                o365GrpInfo = orchestrator.PopulateGroup(psObject, this.Endpoint.Id);
                                if (null != o365GrpInfo)
                                    list.Add(o365GrpInfo);

                                Command getDistGroup = new Command("Get-DistributionGroupMember");
                                getDistGroup.Parameters.Add("identity", o365GrpInfo.sAMAccountName);
                                getDistGroup.Parameters.Add("ResultSize", "unlimited");
                                var pipeline2 = runspace.CreatePipeline();
                                pipeline2.Commands.Add(getDistGroup);
                                ArrayList mem = new ArrayList();

                                ICollection<PSObject> results2 = pipeline2.Invoke();
                                pipeline2.Commands.Clear();

                                foreach (PSObject ps1 in results2)
                                {
                                    var member = ps1.Members["DistinguishedName"].Value?.ToString()?.Trim();
                                    if (!string.IsNullOrWhiteSpace(member))
                                        mem.Add(member);
                                }

                                dicGroupUsers.Add(o365GrpInfo.distinguishedName, mem);
                            }
                            WriteDiscoveryLog(LogLevelType.Info, $"Populated {list.Count.ToString()} group(s) objects InMemory. UserGroup Count:{dicGroupUsers.Count}", DiscoveryObjectType.Group);

                            if (list.Count > 0)
                            {
                                Office365GroupInfo.SaveGroups(list, this.Endpoint.Id, MasterConfigHelperServiceUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            resultgroups?.Clear();
                            resultgroups = null;
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Error While getting Office 365 Groups", DiscoveryObjectType.Group, ex);
                throw ex;
            }
            finally
            {
                list?.Clear();
                list = null;
            }
        }
        public IList<Office365UserInfo> GetUsers(List<AccountReconcilation> exsitngAccountrecon, List<Office365UserInfo> allExistingUsers)
        {
            Collection<PSObject> resultmsolusers = new Collection<PSObject>();
            Collection<PSObject> mailboxFullDetails = new Collection<PSObject>();
            Collection<PSObject> resultusers = new Collection<PSObject>();

            IList<Office365UserInfo> list = new List<Office365UserInfo>();
            try
            {

                UserInfoOrchestrator orchestrator = new UserInfoOrchestrator();
                PSCredential credential = GetPSCredential();
                Office365UserInfo o365userinfo = new Office365UserInfo();
                PSObject mailboxdetails = null;
                Office365UserServicePlan userServicePlan = new Office365UserServicePlan();
                List<ServicePlanDetail> listPlanDetails = new List<ServicePlanDetail>();


                resultmsolusers = this.GetAllMsolUsers();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Get-User", "Get-EXOMailbox" });
                            powershell.Commands.AddCommand(connect);
                            var moduelImport = powershell.Invoke();
                            if (!PowerShellErrorCheck(powershell, moduelImport))
                            {

                            }
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();
                            Command GetUsers = new Command("Get-User");
                            GetUsers.Parameters.Add("ResultSize", "unlimited");
                            powershell.Commands.AddCommand(GetUsers);
                            resultusers = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command GetmailboxFullDetails = new Command("Get-EXOMailbox");
                            GetmailboxFullDetails.Parameters.Add("ResultSize", "unlimited");
                            powershell.Commands.AddCommand(GetmailboxFullDetails);
                            mailboxFullDetails = powershell.Invoke();
                            powershell.Commands.Clear();
                            foreach (PSObject psObject in resultusers)
                            {

                                powershell.Commands.Clear();
                                o365userinfo = new Office365UserInfo();
                                mailboxdetails = null;

                                o365userinfo = orchestrator.PopulateUser(psObject, this.Endpoint.Id);

                                var msolUser = resultmsolusers.FirstOrDefault(s => s.Members["UserPrincipalName"].Value.ToString() == o365userinfo.UserPrincipalName);
                                var mailboxdet = mailboxFullDetails.Where(s => s.Members["UserPrincipalName"].Value.ToString() == o365userinfo.UserPrincipalName);

                                if (msolUser != null)
                                {
                                    userServicePlan = new Office365UserServicePlan();
                                    listPlanDetails = new List<ServicePlanDetail>();

                                    var boProp = msolUser.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "BaseObject");
                                    if (boProp != null)
                                    {
                                        var bo = boProp.GetValue(msolUser);

                                        if (bo != null)
                                        {
                                            var usgLocProp = bo.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "UsageLocation");
                                            var valStsProp = bo.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ValidationStatus");
                                            var licRecNeedProp = bo.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "LicenseReconciliationNeeded");

                                            o365userinfo.UsageLocation = usgLocProp != null ? (string)usgLocProp.GetValue(bo) : string.Empty;
                                            o365userinfo.ValidationStatus = valStsProp != null ? valStsProp.GetValue(bo).ToString() : string.Empty;

                                            bool flag;
                                            string LicenseReconciliationNeeded = licRecNeedProp != null ? licRecNeedProp.GetValue(bo).ToString() : string.Empty;
                                            Boolean.TryParse(LicenseReconciliationNeeded, out flag);
                                            o365userinfo.LicenseReconciliationNeeded = flag;

                                            var licensesProp = bo.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "Licenses");
                                            IList licenses = null;

                                            if (licensesProp != null)
                                            {
                                                licenses = (IList)licensesProp.GetValue(bo);
                                            }

                                            if (licenses != null && licenses.Count > 0)
                                            {
                                                foreach (object sp in licenses)
                                                {
                                                    string accountSkuId = string.Empty;

                                                    var accSkuProp = sp.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "AccountSkuId");
                                                    var svcStatusProp = sp.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServiceStatus");
                                                    accountSkuId = accSkuProp != null ? (string)accSkuProp.GetValue(sp) : string.Empty;
                                                    var svcStatuses = svcStatusProp != null ? (IList)svcStatusProp.GetValue(sp) : null;

                                                    if (svcStatuses != null)
                                                    {
                                                        foreach (object svcStatus in svcStatuses)
                                                        {
                                                            ServicePlanDetail svcPlan = new ServicePlanDetail();
                                                            svcPlan.AccountSkuId = accountSkuId;

                                                            var property = svcStatus.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServicePlan");
                                                            var prop3 = svcStatus.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ProvisioningStatus");

                                                            svcPlan.ProvisioningStatus = prop3 != null ? prop3.GetValue(svcStatus).ToString() : string.Empty;

                                                            if (property != null)
                                                            {
                                                                var servicePlanObj = property.GetValue(svcStatus);

                                                                var prop1 = servicePlanObj.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServiceName");
                                                                var prop2 = servicePlanObj.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServiceType");

                                                                svcPlan.ServiceName = prop1 != null ? (string)prop1.GetValue(servicePlanObj) : string.Empty;
                                                                svcPlan.ServiceType = prop2 != null ? (string)prop2.GetValue(servicePlanObj) : string.Empty;

                                                                listPlanDetails.Add(svcPlan);
                                                            }
                                                        }
                                                    }
                                                }

                                                userServicePlan.PlanDetails = listPlanDetails;
                                                o365userinfo.ServicePlanInfo = UtilityMethods.Serialize<Office365UserServicePlan>(userServicePlan);
                                            }
                                        }
                                    }
                                }

                                if (IsRecipientPermissionsSelected)
                                {
                                    Collection<PSObject> sendAsDetail = null;

                                    try
                                    {

                                        Command sendAs = new Command("Get-RecipientPermission");
                                        sendAs.Parameters.Add("Identity", o365userinfo.UserPrincipalName);
                                        powershell.Commands.AddCommand(sendAs);
                                        sendAsDetail = powershell.Invoke();
                                        powershell.Commands.Clear();
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteDiscoveryLog(LogLevelType.Exception, string.Format("Get-RecipientPermission for the user - {0}", o365userinfo.SamAccountName), DiscoveryObjectType.User, ex);
                                    }

                                    if (sendAsDetail != null && sendAsDetail.Count() > 0)
                                    {
                                        if (sendAsDetail.Count() > 0)
                                        {
                                            foreach (var item in sendAsDetail)
                                            {
                                                if (item.Members["Trustee"].Value != null)
                                                {
                                                    string Trustee = item.Members["Trustee"].Value.ToString();
                                                    if (o365userinfo.SendAs != null && o365userinfo.SendAs != string.Empty)
                                                    {
                                                        o365userinfo.SendAs = o365userinfo.SendAs + "," + Trustee;
                                                    }
                                                    else
                                                        o365userinfo.SendAs = Trustee;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (mailboxdet != null && mailboxdet.Count() > 0)
                                {
                                    if (IsMailBoxPermissionsSelected)
                                    {
                                        Collection<PSObject> accessDetails = null;
                                        try
                                        {
                                            powershell.Commands.Clear();
                                            Command mbPermission = new Command("Get-MailboxPermission");
                                            mbPermission.Parameters.Add("Identity", o365userinfo.UserPrincipalName);
                                            powershell.Commands.AddCommand(mbPermission);
                                            accessDetails = powershell.Invoke();
                                            powershell.Commands.Clear();
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Get-MailboxPermission for the user - {0}", o365userinfo.SamAccountName), DiscoveryObjectType.User, ex);
                                        }

                                        if (accessDetails != null && accessDetails.Count() > 0)
                                        {
                                            foreach (PSObject aobj in accessDetails)
                                            {
                                                if ((bool)aobj.Members["IsInherited"].Value != true && aobj.Members["User"].Value.ToString() != "NT AUTHORITY\\SELF")
                                                {

                                                    string AccessDet = aobj.Members["User"].Value.ToString();
                                                    if (o365userinfo.FullAccess != null && o365userinfo.FullAccess != string.Empty)
                                                    {
                                                        if (!(o365userinfo.FullAccess).Contains(AccessDet))
                                                        {
                                                            o365userinfo.FullAccess = o365userinfo.FullAccess + "," + AccessDet;
                                                        }
                                                    }
                                                    else
                                                        o365userinfo.FullAccess = AccessDet;
                                                }
                                            }
                                        }
                                    }

                                    if (IsMailBoxStatisticsSelected)
                                    {
                                        Collection<PSObject> usermailboxdetail = null;
                                        try
                                        {
                                            powershell.Commands.Clear();
                                            Command Stat = new Command("Get-MailboxStatistics");
                                            Stat.Parameters.Add("Identity", o365userinfo.UserPrincipalName);
                                            powershell.Commands.AddCommand(Stat);
                                            usermailboxdetail = powershell.Invoke();
                                            powershell.Commands.Clear();
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Get-RecipientPermission for the user - {0}", o365userinfo.SamAccountName), DiscoveryObjectType.User, ex);
                                        }

                                        if (usermailboxdetail != null && usermailboxdetail.Count() > 0)
                                        {
                                            PSObject ps = usermailboxdetail.FirstOrDefault();

                                            if (ps.Members["LastLogonTime"].Value != null)
                                            {
                                                DateTime st = (DateTime)ps.Members["LastLogonTime"].Value;
                                                o365userinfo.LastLogon = st;
                                            }
                                            if (ps.Members["TotalItemSize"].Value != null)
                                            {
                                                string size = ps.Members["TotalItemSize"].Value.ToString();
                                                o365userinfo.MailboxUsage = size;
                                            }
                                        }
                                    }

                                    mailboxdetails = mailboxdet.FirstOrDefault();
                                    if (mailboxdetails.Members["Alias"].Value != null)
                                    {
                                        string Alias = mailboxdetails.Members["Alias"].Value.ToString();
                                        o365userinfo.Alias = Alias;
                                    }
                                    if (mailboxdetails.Members["RetentionPolicy"] != null && mailboxdetails.Members["RetentionPolicy"].Value != null)
                                    {
                                        string RetentionPolicy = mailboxdetails.Members["RetentionPolicy"].Value.ToString();
                                        o365userinfo.RetentionPolicy = RetentionPolicy;
                                    }
                                    if (mailboxdetails.Members["LitigationHoldEnabled"] != null && mailboxdetails.Members["LitigationHoldEnabled"].Value != null)
                                    {
                                        bool LitigationHoldEnabled = (bool)mailboxdetails.Members["LitigationHoldEnabled"].Value;
                                        o365userinfo.LitigationHoldEnabled = LitigationHoldEnabled;
                                    }
                                    if (mailboxdetails.Members["RetentionHoldEnabled"] != null && mailboxdetails.Members["RetentionHoldEnabled"].Value != null)
                                    {
                                        bool RetentionHoldEnabled = (bool)mailboxdetails.Members["RetentionHoldEnabled"].Value;
                                        o365userinfo.RetentionHoldEnabled = RetentionHoldEnabled;
                                    }
                                    if (mailboxdetails.Members["ForwardingSmtpAddress"] != null && mailboxdetails.Members["ForwardingSmtpAddress"].Value != null)
                                    {
                                        string ForwardingSmtpAddress = mailboxdetails.Members["ForwardingSmtpAddress"].Value.ToString();
                                        o365userinfo.ForwardingSmtpAddress = ForwardingSmtpAddress;
                                    }
                                    if (mailboxdetails.Members["LitigationHoldDuration"] != null && mailboxdetails.Members["LitigationHoldDuration"].Value != null)
                                    {
                                        string LitigationHoldDuration = mailboxdetails.Members["LitigationHoldDuration"].Value.ToString();
                                        o365userinfo.LitigationHoldDuration = LitigationHoldDuration;
                                    }
                                    if (mailboxdetails.Members["IsMailboxEnabled"] != null && mailboxdetails.Members["IsMailboxEnabled"].Value != null)
                                    {
                                        bool IsMailboxEnabled = (bool)mailboxdetails.Members["IsMailboxEnabled"].Value;
                                        o365userinfo.IsMailboxEnabled = IsMailboxEnabled;
                                    }
                                    if (mailboxdetails.Members["Languages"] != null && mailboxdetails.Members["Languages"].Value != null)
                                    {
                                        string Languages = mailboxdetails.Members["Languages"].Value.ToString();
                                        o365userinfo.Languages = Languages;
                                    }
                                    if (mailboxdetails.Members["IsShared"] != null && mailboxdetails.Members["IsShared"].Value != null)
                                    {
                                        bool IsShared = (bool)mailboxdetails.Members["IsShared"].Value;
                                        o365userinfo.IsShared = IsShared;
                                    }
                                    if (mailboxdetails.Members["RoleAssignmentPolicy"] != null && mailboxdetails.Members["RoleAssignmentPolicy"].Value != null)
                                    {
                                        string RoleAssignmentPolicy = mailboxdetails.Members["RoleAssignmentPolicy"].Value.ToString();
                                        o365userinfo.RoleAssignmentPolicy = RoleAssignmentPolicy;
                                    }
                                    if (mailboxdetails.Members["SharingPolicy"] != null && mailboxdetails.Members["SharingPolicy"].Value != null)
                                    {
                                        string SharingPolicy = mailboxdetails.Members["SharingPolicy"].Value.ToString();
                                        o365userinfo.SharingPolicy = SharingPolicy;
                                    }
                                    if (mailboxdetails.Members["ArchiveName"] != null && mailboxdetails.Members["ArchiveName"].Value != null)
                                    {
                                        string ArchiveName = mailboxdetails.Members["ArchiveName"].Value.ToString();
                                        o365userinfo.ArchiveName = ArchiveName;
                                    }
                                    if (mailboxdetails.Members["ArchiveStatus"] != null && mailboxdetails.Members["ArchiveStatus"].Value != null)
                                    {
                                        string ArchiveStatus = mailboxdetails.Members["ArchiveStatus"].Value.ToString();
                                        o365userinfo.ArchiveStatus = ArchiveStatus;
                                    }
                                    if (mailboxdetails.Members["RecipientType"] != null && mailboxdetails.Members["RecipientType"].Value != null)
                                    {
                                        string RecipientType = mailboxdetails.Members["RecipientType"].Value.ToString();
                                        o365userinfo.RecipientType = RecipientType;
                                    }
                                    if (mailboxdetails.Members["grantsendonbehalfto"] != null && mailboxdetails.Members["grantsendonbehalfto"].Value != null)
                                    {
                                        string SendonBehalf = mailboxdetails.Members["grantsendonbehalfto"].Value.ToString();
                                        o365userinfo.SendonBehalf = SendonBehalf;
                                    }
                                }

                                if (null != o365userinfo)
                                    list.Add(o365userinfo);

                                if (list.Count() % 100 == 0)
                                {
                                    //WriteDiscoveryLog(LogLevelType.DebugStep, string.Format("Saving Users Started {0}", list.Count()), DiscoveryObjectType.User);

                                    Office365UserInfo.SaveUsers(list, this.Endpoint.Id, "System", exsitngAccountrecon, allExistingUsers);
                                    list.Clear();

                                    //WriteDiscoveryLog(LogLevelType.DebugStep, string.Format("Saving Users Ended"), DiscoveryObjectType.User);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                if (resultusers != null)
                    WriteDiscoveryLog(LogLevelType.Info, $"Total Users Discovered: {resultusers.Count()}", DiscoveryObjectType.User);

                return list.ToList();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Error While getting Users", DiscoveryObjectType.User, ex);
                throw ex;
            }
            finally
            {
                resultmsolusers?.Clear();
                resultmsolusers = null;

                mailboxFullDetails?.Clear();
                mailboxFullDetails = null;

                resultusers?.Clear();
                resultusers = null;
            }
        }
        //private void DisconnectExchange()
        //{
        //    try
        //    {
        //        InitialSessionState iss = InitialSessionState.CreateDefault();
        //        iss.ImportPSModule(new[] { "ExchangeOnlineManagement", "MSOnline" });
        //        using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
        //        {
        //            runspace.Open();
        //            using (PowerShell powershell = PowerShell.Create())
        //            {
        //                powershell.Runspace = runspace;

        //                Command Stat = new Command("Disconnect-ExchangeOnline");
        //                Stat.Parameters.Add("Confirm", false);
        //                Stat.Parameters.Add("InformationAction", "Ignore");
        //                Stat.Parameters.Add("ErrorAction", "SilentlyContinue");
        //                powershell.Commands.AddCommand(Stat);
        //                powershell.Invoke();
        //            }
        //            runspace.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        public IList<Office365License> GetLicenses()
        {
            Collection<PSObject> licenses = new Collection<PSObject>();

            try
            {
                IList<Office365License> listLicenses = new List<Office365License>();
                string userName = Configuration(EndpointKey.UserName);
                string password = Configuration(EndpointKey.Password);
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            Command connect = new Command("Connect-MsolService");
                            connect.Parameters.Add("Credential", GetPSCredential());
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            var err = powershell.Streams.Error.ReadAll();
                            if (PowerShellErrorCheck(powershell, results))
                            {
                                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while connecting to msonline.", DiscoveryObjectType.License);
                            }
                            powershell.Commands.Clear();

                            Command GetLicenses = new Command("Get-MsolAccountSku");
                            powershell.Commands.AddCommand(GetLicenses);
                            licenses = powershell.Invoke();

                            var err1 = powershell.Streams.Error.ReadAll();
                            if (err1.Count() > 0)
                            {
                                string errMsg = string.Empty;
                                foreach (var msg in err1)
                                {
                                    errMsg += msg;
                                }
                                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while connecting to msonline." + errMsg, DiscoveryObjectType.License);
                            }

                            WriteDiscoveryLog(LogLevelType.Info, string.Format("Discovered {0} licenses.", licenses.Count()), DiscoveryObjectType.License);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }

                Office365License o365License = null;
                foreach (PSObject item in licenses)
                {
                    o365License = new Office365License();

                    o365License.AccountSkuId = item.Members[Office365Properties.AccountSkuId].Value.ToString();
                    o365License.ActiveUnits = Convert.ToInt32(item.Members[Office365Properties.ActiveUnits].Value);
                    o365License.WarningUnits = Convert.ToInt32(item.Members[Office365Properties.WarningUnits].Value);
                    o365License.ConsumedUnits = Convert.ToInt32(item.Members[Office365Properties.ConsumedUnits].Value);
                    o365License.EndPointSystemId = this.Endpoint.Id;

                    List<Office365ServicePlan> listServicePlans = new List<Office365ServicePlan>();
                    var servicePlans = (IList)item.Members["ServiceStatus"].Value;

                    foreach (object sp in servicePlans)
                    {
                        Office365ServicePlan svcPlan = new Office365ServicePlan();

                        var property = sp.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServicePlan");

                        if (property != null)
                        {
                            svcPlan.AccountSkuId = o365License.AccountSkuId;
                            var servicePlanObj = property.GetValue(sp);
                            var prop1 = servicePlanObj.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServiceName");
                            var prop2 = servicePlanObj.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServiceType");
                            svcPlan.ServiceName = prop1 != null ? (string)prop1.GetValue(servicePlanObj) : string.Empty;
                            svcPlan.ServiceType = prop2 != null ? (string)prop2.GetValue(servicePlanObj) : string.Empty;
                            svcPlan.EndPointSystemId = this.Endpoint.Id;
                            listServicePlans.Add(svcPlan);
                        }
                    }

                    o365License.ServicePlans = listServicePlans;
                    listLicenses.Add(o365License);
                }

                return listLicenses;

            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Error While getting Licenses", ex);
                throw ex;
            }
            finally
            {
                licenses?.Clear();
            }
        }
        public IList<Office365RoleAssignmentPolicy> GetRolePolicy()
        {
            Collection<PSObject> resultPolicy = new Collection<PSObject>();
            try
            {
                PSCredential credentials = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credentials);
                            //connect.Parameters.Add("CommandName", new[] { "Get-RoleAssignmentPolicy"});
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();

                            powershell.Commands.Clear();
                            Command GetPolicy = new Command("Get-RoleAssignmentPolicy");
                            powershell.Commands.AddCommand(GetPolicy);
                            resultPolicy = powershell.Invoke();

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }

                IList<Office365RoleAssignmentPolicy> listpolicies = new List<Office365RoleAssignmentPolicy>();
                foreach (PSObject item in resultPolicy)
                {
                    Office365RoleAssignmentPolicy o365RolePolicy = new Office365RoleAssignmentPolicy();

                    o365RolePolicy.Name = item.Members[Office365Properties.Name]?.Value?.ToString() ?? string.Empty;
                    o365RolePolicy.PolicyId = item.Members[Office365Properties.Id]?.Value?.ToString() ?? string.Empty;
                    o365RolePolicy.DistinguishedName = item.Members[Office365Properties.DistinguishedName]?.Value?.ToString() ?? string.Empty;
                    o365RolePolicy.IsDefault = (bool)(item.Members[Office365Properties.IsDefault]?.Value ?? null);
                    o365RolePolicy.Description = item.Members[Office365Properties.Description]?.Value?.ToString() ?? string.Empty;
                    o365RolePolicy.Identity = item.Members[Office365Properties.Identity]?.Value?.ToString() ?? string.Empty;
                    o365RolePolicy.WhenCreated = (DateTime)item.Members[Office365Properties.WhenCreated].Value;
                    o365RolePolicy.IsValid = (bool)(item.Members[Office365Properties.IsValid]?.Value ?? null);
                    o365RolePolicy.EndPointSystemId = this.Endpoint.Id;
                    o365RolePolicy.CreatedDate = DateTime.Now;
                    o365RolePolicy.LastModifiedBy = PROCESSNAME;
                    o365RolePolicy.LastModifiedDate = DateTime.Now;

                    listpolicies.Add(o365RolePolicy);
                }
                return listpolicies;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception Occured While Office365/GetRolePolicy method", DiscoveryObjectType.Role, ex);
                throw ex;
            }
            finally
            {
                resultPolicy?.Clear();
                resultPolicy = null;
            }
        }
        public IList<Office365RetentionPolicy> GetRetentionPolicy()
        {
            IList<Office365RetentionPolicy> listpolicies = new List<Office365RetentionPolicy>();
            Collection<PSObject> resultPolicy = new Collection<PSObject>();
            try
            {
                PSCredential credential = GetPSCredential();

                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Get-RetentionPolicy" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();

                            powershell.Commands.Clear();
                            Command GetPolicy = new Command("Get-RetentionPolicy");
                            powershell.Commands.AddCommand(GetPolicy);
                            resultPolicy = powershell.Invoke();

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                Office365RetentionPolicy o365RolePolicy = null;
                foreach (PSObject item in resultPolicy)
                {
                    o365RolePolicy = new Office365RetentionPolicy();

                    o365RolePolicy.Name = item.Members[Office365Properties.Name].Value.ToString();
                    o365RolePolicy.PolicyId = item.Members[Office365Properties.Id].Value.ToString();
                    o365RolePolicy.DistinguishedName = item.Members[Office365Properties.DistinguishedName].Value.ToString();
                    o365RolePolicy.IsDefault = (bool)(item.Members[Office365Properties.IsDefault].Value);
                    o365RolePolicy.Identity = item.Members[Office365Properties.Identity].Value.ToString();
                    o365RolePolicy.WhenCreated = (DateTime)item.Members[Office365Properties.WhenCreated].Value;
                    o365RolePolicy.IsValid = (bool)(item.Members[Office365Properties.IsValid].Value);
                    o365RolePolicy.EndPointSystemId = this.Endpoint.Id;
                    o365RolePolicy.CreatedDate = DateTime.Now;
                    o365RolePolicy.LastModifiedBy = PROCESSNAME;
                    o365RolePolicy.LastModifiedDate = DateTime.Now;

                    listpolicies.Add(o365RolePolicy);
                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Total Retention Policies Discovered -{0}", listpolicies.Count()), DiscoveryObjectType.RetentionPolicy);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception Occured While Office365/GetRetentionPolicy method", DiscoveryObjectType.RetentionPolicy, ex);
                throw ex;
            }
            finally
            {
                listpolicies?.Clear();
            }
            return listpolicies;
        }
        public IList<Office365SharingPolicy> GetSharingPolicy()
        {
            IList<Office365SharingPolicy> listpolicies = new List<Office365SharingPolicy>();
            Collection<PSObject> resultPolicy = new Collection<PSObject>();

            try
            {

                PSCredential credential = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            powershell.Commands.Clear();
                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Get-SharingPolicy" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command GetPolicy = new Command("Get-SharingPolicy");
                            powershell.Commands.AddCommand(GetPolicy);
                            resultPolicy = powershell.Invoke();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                        Office365SharingPolicy o365RolePolicy = null;
                        foreach (PSObject item in resultPolicy)
                        {
                            o365RolePolicy = new Office365SharingPolicy();

                            o365RolePolicy.Name = item.Members[Office365Properties.Name].Value.ToString();
                            o365RolePolicy.PolicyId = item.Members[Office365Properties.Id].Value.ToString();
                            o365RolePolicy.DistinguishedName = item.Members[Office365Properties.DistinguishedName].Value.ToString();
                            o365RolePolicy.IsEnabled = (bool)(item.Members[Office365Properties.Enabled].Value);
                            o365RolePolicy.IsDefault = (bool)(item.Members[Office365Properties.Default].Value);
                            o365RolePolicy.Identity = item.Members[Office365Properties.Identity].Value.ToString();
                            o365RolePolicy.WhenCreated = (DateTime)item.Members[Office365Properties.WhenCreated].Value;
                            o365RolePolicy.IsValid = (bool)(item.Members[Office365Properties.IsValid].Value);
                            o365RolePolicy.EndPointSystemId = this.Endpoint.Id;
                            o365RolePolicy.CreatedDate = DateTime.Now;
                            o365RolePolicy.LastModifiedBy = PROCESSNAME;
                            o365RolePolicy.LastModifiedDate = DateTime.Now;

                            //enabled

                            listpolicies.Add(o365RolePolicy);
                        }
                    }
                }

                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Total Share Policies Discovered -{0}", listpolicies.Count()));
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Exception Occured While Office365/GetSharingPolicy method", ex);
                throw ex;
            }
            finally
            {
                resultPolicy?.Clear();
                resultPolicy = null;
            }
            return listpolicies;
        }
        public bool IsMailBoxExists(string upn)
        {
            try
            {

                PSCredential credential = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                bool isExists = false;
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {

                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;

                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Get-EXOMailbox" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command CheckMailBox = new Command("Get-EXOMailbox");
                            CheckMailBox.Parameters.Add("Identity", upn);
                            powershell.Commands.AddCommand(CheckMailBox);
                            IList<PSObject> listresult = powershell.Invoke();
                            powershell.Commands.Clear();

                            if (listresult.Count > 0)
                            {
                                isExists = true;
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                    return isExists;
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while finding mailbox {upn}. Error{ex.ToString()}", ex);
                throw ex;
            }
        }
        private PSCredential GetPSCredential()
        {
            string userName = this.Configuration(EndpointKey.UserName);
            string password = this.Configuration(EndpointKey.Password);
            System.Security.SecureString secureString = new System.Security.SecureString();
            string myPassword = password;
            foreach (char c in myPassword)
                secureString.AppendChar(c);
            return new PSCredential(userName, secureString);
        }
        public Collection<PSObject> GetAllMsolUsers()
        {
            Collection<PSObject> msolUsersList = new Collection<PSObject>();
            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, "GetAllMsolUsers Started");
                var credential = GetPSCredential();

                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;

                            powershell.Commands.Clear();
                            var connect = new Command("Connect-MsolService");
                            connect.Parameters.Add("Credential", credential);
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command CheckUserExists = new Command("Get-MsolUser");
                            CheckUserExists.Parameters.Add("All");
                            powershell.Commands.AddCommand(CheckUserExists);
                            msolUsersList = powershell.Invoke();

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }

                _idmsLogger.WriteLog(LogLevelType.Info, "GetAllMsolUsers Ended");
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while getting all msol users. Error:{ex.ToString()}", ex);
                return null;
            }
            return msolUsersList;
        }

        private Collection<PSObject> GetAllMsolLicences()
        {
            try
            {
                string userName = this.Configuration(EndpointKey.UserName);
                string password = this.Configuration(EndpointKey.Password);

                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);

                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                    {
                        powershell.Runspace = runspace;
                        Command connect = new Command("Connect-MsolService");
                        System.Security.SecureString secureString = new System.Security.SecureString();
                        string myPassword = password;
                        foreach (char c in myPassword)
                            secureString.AppendChar(c);

                        connect.Parameters.Add("Credential", new PSCredential(userName, secureString));
                        powershell.Commands.AddCommand(connect);
                        var results = powershell.Invoke();
                        powershell.Commands.Clear();

                        Command CheckUserExists = new Command("Get-MsolUser");
                        CheckUserExists.Parameters.Add("All");
                        powershell.Commands.AddCommand(CheckUserExists);
                        return powershell.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while getting all msol users. Error:{ex.ToString()}", ex);
                return null;
            }
        }
        private Collection<PSObject> GetMsolUserByUPN(string upn)
        {
            try
            {
                var creadential = GetPSCredential();

                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);

                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(iss))
                {
                    psRunSpace.Open();
                    using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                    {
                        powershell.Runspace = psRunSpace;
                        Command connect = new Command("Connect-MsolService");
                        connect.Parameters.Add("Credential", creadential);
                        powershell.Commands.AddCommand(connect);
                        var results = powershell.Invoke();
                        powershell.Commands.Clear();

                        Command CheckUserExists = new Command("Get-MsolUser");
                        CheckUserExists.Parameters.Add("UserPrincipalName", upn);
                        powershell.Commands.AddCommand(CheckUserExists);
                        var result = powershell.Invoke();
                        powershell.Commands.Clear();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while getting all msol user {0}. Error:{1}", upn, ex.ToString()), ex);
                return null;
            }
        }
        private void UpdateEntitlementRecon()
        {
            List<AccountReconcilation> exsitngAccountrecon = null;
            List<EntitlementReconcilation> existingEntilements = null;
            try
            {
                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id).ToList();
                existingEntilements = EntitlementReconcilation.GetAllByEndpointSystemId(this.Endpoint.Id);
                AccountReconcilation acctRecon = null;
                EntitlementReconcilation entRecon = null;
                // List<Office365UserInfo> listnpacUserInfo = Office365UserInfo.GetAllByEndpointId(this.Endpoint.Id);

                Office365UserInfo o365UserInfo = null;
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    o365UserInfo = null;
                    try
                    {
                        o365UserInfo = Office365UserInfo.GetByIdIncludeGroups(item.Key);

                        if (o365UserInfo != null && o365UserInfo.O365Groups != null)
                        {
                            foreach (Office365GroupInfo entitlement in o365UserInfo.O365Groups)
                            {
                                acctRecon = exsitngAccountrecon.Where(a => a.EndpointUserId == o365UserInfo.Id).FirstOrDefault();
                                if (acctRecon == null) continue;

                                entRecon = existingEntilements.Where(e => e.EndpointUserId == o365UserInfo.Id &&
                                                                            e.Entitlement == entitlement.name.Trim() &&
                                                                            e.EntitlementType == (int)EntitlementType.Group).FirstOrDefault();

                                if (entRecon == null)
                                {
                                    entRecon = new EntitlementReconcilation();
                                    var entitlements = (o365UserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Group);
                                    entRecon.Entitlement = entitlement.name.Trim();
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = entitlement.description;
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                    entRecon.CreatedDate = DateTime.Now;
                                    entRecon.CreatedBy = "System";
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                    existingEntilements.Add(entRecon);
                                }
                                else
                                {
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.Entitlement = entitlement.name;
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = entitlement.description;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while updating Entitlement reconciliation. O365 User -{0}", o365UserInfo.SamAccountName), ex);
                    }
                }

                try
                {
                    EntitlementReconcilation.ProcessDeletedEntitlements(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
                }
                catch (Exception ex)
                {
                    this.WriteDiscoveryLog(LogLevelType.Exception, "Error while ProcessDelete Entitlements Recon.", ex);
                }
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Process Entitlements Recon.", ex);
            }
            finally
            {
                exsitngAccountrecon?.Clear();
                exsitngAccountrecon = null;
                existingEntilements?.Clear();
                existingEntilements = null;
            }


        }


        #endregion

        #region Provisioning
        public override bool ChangePassword(string userName, string newPassword, long DIUserId, bool isReactivation)
        {
            return true;
        }
        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            try
            {
                Office365UserInfo userInfo = (Office365UserInfo)_userInfo;

                PSCredential credential = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "New-Mailbox", "Set-Mailbox", "Get-EXOMailbox","Set-User" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Creating shared email box {0}.", userInfo.WindowsEmailAddress), null);
                            Command cmd = new Command("New-Mailbox");
                            cmd.Parameters.Add("Name", userInfo.DisplayName);
                            cmd.Parameters.Add("Alias", userInfo.Alias);
                            cmd.Parameters.Add("PrimarySmtpAddress", userInfo.WindowsEmailAddress);
                            if (userInfo.IsShared)
                            {
                                cmd.Parameters.Add("Shared");
                            }
                            else
                            {
                                System.Security.SecureString securePwd = new System.Security.SecureString();
                                foreach (char c in _userInfo.AccountPassword)
                                {
                                    securePwd.AppendChar(c);
                                }

                                cmd.Parameters.Add("Password", securePwd);
                                cmd.Parameters.Add("MicrosoftOnlineServicesID", userInfo.UserPrincipalName);
                            }

                            powershell.Commands.AddCommand(cmd);
                            results = powershell.Invoke();
                            if (PowerShellErrorCheck(powershell, results))
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, "Error occured while creating New-Mailbox", null);
                                return false;
                            }
                            powershell.Commands.Clear();
                            results.Clear();

                            Thread.Sleep(MasterConfigHelper.TimeDelayForDownloadO365User);

                            _idmsLogger.WriteLog(LogLevelType.Info, $"Check Mailbox created or not");
                            Command CheckUserExists = new Command("Get-EXOMailbox");
                            CheckUserExists.Parameters.Add("Identity", userInfo.Alias);
                            powershell.Commands.AddCommand(CheckUserExists);
                            results = powershell.Invoke();
                            if (PowerShellErrorCheck(powershell, results))
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, "Error occured while creating New-Mailbox", null);
                                return false;
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Mail Box:{userInfo.DisplayName} created successfully.");
                            }

                            powershell.Commands.Clear();
                            results.Clear();

                            try
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Set-Mailbox RecipientLimits.", userInfo.DisplayName), null);
                                Command setMB = new Command("Set-Mailbox");
                                setMB.Parameters.Add("identity", userInfo.DisplayName);
                                setMB.Parameters.Add("RecipientLimits", MasterConfigHelper.Office365RecipientLimits);
                                powershell.Commands.AddCommand(setMB);
                                powershell.Invoke();
                                powershell.Commands.Clear();
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Set-Mailbox RecipientLimits completed.", userInfo.DisplayName), null);

                                if (userInfo.IsShared)
                                {
                                    User manger = User.GetById(this.CurrentRequest.UserId);

                                    if (manger != null)
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Debug, $"Started updating manager {manger.UserName} in shared emailbox {_userInfo.AccountName}.");

                                        Command addManager = new Command("Set-User");
                                        addManager.Parameters.Add("Identity", userInfo.WindowsEmailAddress);
                                        addManager.Parameters.Add("Manager", manger.UserName);
                                        powershell.Commands.AddCommand(addManager);
                                        powershell.Invoke();
                                        powershell.Commands.Clear();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while updating manager in shared emailbox {0}. Error:{1}", _userInfo.AccountName, ex.ToString()), ex);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while creating shared emailbox {0}. Error:{1}", _userInfo.AccountName, ex.ToString()), ex);
                return false;
            }
        }
        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool LockAccount(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public override bool UnLockAccount(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public override void ForceSyncOffice365()
        {
            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Started Force Sync Active Directory to {0}", "Office 365"), null);
                string userName = this.Configuration(EndpointKey.UserName);
                string password = this.Configuration(EndpointKey.Password);
                string HostName = this.Configuration(EndpointKey.DirectorySyncServerName);

                string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";

                SecureString ssRunasPassword = new SecureString();
                foreach (char x in password)
                    ssRunasPassword.AppendChar(x);
                PSCredential credentials = new PSCredential(userName, ssRunasPassword);
                WSManConnectionInfo connInfo = new WSManConnectionInfo(false, HostName, 5985, "/wsman", shellUri, credentials);

                connInfo.AuthenticationMechanism = AuthenticationMechanism.Default;

                var runspace = RunspaceFactory.CreateRunspace(connInfo);

                runspace.Open();
                var command = new Command("Out-String");
                string accessScriptPath = "import-module dirsync -verbose; Start-OnlineCoexistenceSync";
                var pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(accessScriptPath);
                pipeline.Commands.Add(command);
                var results = pipeline.Invoke();
                pipeline.Commands.Clear();

                if (results.Count > 0)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Force AdhocSync Initiated."), null);
                }
                runspace.Close();
                runspace.Dispose();
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Could not connect to Sync Server.", ex);
            }
        }
        public override bool DisableAccount(IEndpointUserInfo _userInfo)
        {
            return false;
        }
        public override bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteAccount(IEndpointUserInfo _userInfo)
        {
            Office365UserInfo office365User = null;
            try
            {
                office365User = (Office365UserInfo)_userInfo;
                PSCredential credential = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            var upn = office365User.UserPrincipalName;
                            powershell.Runspace = runspace;
                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Remove-Mailbox", "Get-EXOMailbox" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();


                            Command CheckUserExists = new Command("Get-EXOMailbox");
                            CheckUserExists.Parameters.Add("Identity", upn);
                            powershell.Commands.AddCommand(CheckUserExists);

                            results = powershell.Invoke();
                            powershell.Commands.Clear();
                            if (results != null && results.Count > 0)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("deleting the mailbox {0}", upn), null);
                                Command cmd = new Command("Remove-Mailbox");
                                cmd.Parameters.Add("Identity", upn);
                                cmd.Parameters.Add("Confirm", false);
                                powershell.Commands.AddCommand(cmd);
                                results = powershell.Invoke();
                                powershell.Commands.Clear();
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Mailbox {0} not found in O365.", upn), null);
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }

                base.DeleteAccount(_userInfo);
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while deleting the mailbox {0}. Error:{1}", office365User?.UserPrincipalName, ex.ToString()), ex);
                return false;
            }
        }

        private void DisconnectExchangeOnline(PowerShell powerShell, bool isVersionOne = false)
        {
            try
            {
                powerShell.Commands.Clear();
                Command Stat = null;
                if (isVersionOne == false)
                {
                    Stat = new Command("Disconnect-ExchangeOnline");
                    Stat.Parameters.Add("Confirm", false);
                    Stat.Parameters.Add("InformationAction", "Ignore");
                    Stat.Parameters.Add("ErrorAction", "SilentlyContinue");
                    powerShell.Commands.AddCommand(Stat);
                    powerShell.Invoke();
                }
                else
                {
                    powerShell.Commands.AddScript("Get-PSSession | Remove-PSSession");
                    powerShell.Invoke();
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Error while DisconnectExchangeOnline {ex.ToString()}.", ex);
            }
        }
        public override bool AssignMembers(string emailAddress, Dictionary<string, string> members)
        {
            try
            {
                PSCredential credential = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;

                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Add-MailboxPermission", "Get-EXOMailbox","Add-RecipientPermission" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command CheckUserExists = new Command("Get-EXOMailbox");
                            CheckUserExists.Parameters.Add("Identity", emailAddress);
                            powershell.Commands.AddCommand(CheckUserExists);
                            results = powershell.Invoke();
                            powershell.Commands.Clear();
                            if (results != null && results.Count > 0)
                            {
                                foreach (KeyValuePair<string, string> upn in members)
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Assigning member {0} to mailbox {1}.", upn.Key, emailAddress), null);

                                    Command cmd = new Command("Add-MailboxPermission");
                                    cmd.Parameters.Add("Identity", emailAddress);
                                    cmd.Parameters.Add("User", upn.Key);
                                    cmd.Parameters.Add("AccessRights", "FullAccess");
                                    cmd.Parameters.Add("InheritanceType", "All");
                                    //cmd.Parameters.Add("AutoMapping", 1);

                                    powershell.Commands.AddCommand(cmd);
                                    results = powershell.Invoke();
                                    powershell.Commands.Clear();
                                    results.Clear();

                                    cmd = new Command("Add-RecipientPermission");
                                    cmd.Parameters.Add("Identity", emailAddress);
                                    cmd.Parameters.Add("Trustee", upn.Key);
                                    cmd.Parameters.Add("AccessRights", "SendAs");
                                    cmd.Parameters.Add("Confirm", false);

                                    powershell.Commands.AddCommand(cmd);
                                    results = powershell.Invoke();
                                    powershell.Commands.Clear();
                                    results.Clear();
                                }
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Shared mailbox {0} not found in O365.", emailAddress), null);
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while assigning members to shared mailbox {0}.", emailAddress), ex);
                return false;
            }
        }
        public override bool RemoveMembers(string emailAddress, Dictionary<string, string> members)
        {
            try
            {
                PSCredential credential = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Remove-MailboxPermission", "Get-EXOMailbox", "Remove-RecipientPermission" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command CheckUserExists = new Command("Get-EXOMailbox");
                            CheckUserExists.Parameters.Add("Identity", emailAddress);
                            powershell.Commands.AddCommand(CheckUserExists);
                            results = powershell.Invoke();
                            powershell.Commands.Clear();
                            if (results != null && results.Count > 0)
                            {
                                foreach (KeyValuePair<string, string> upn in members)
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Removing member {0} to mailbox {1}.", upn.Key, emailAddress), null);

                                    Command cmd = new Command("Remove-MailboxPermission");
                                    cmd.Parameters.Add("Identity", emailAddress);
                                    cmd.Parameters.Add("User", upn.Key);
                                    cmd.Parameters.Add("AccessRights", "FullAccess");
                                    cmd.Parameters.Add("Confirm", false);

                                    powershell.Commands.AddCommand(cmd);
                                    results = powershell.Invoke();
                                    powershell.Commands.Clear();
                                    results.Clear();

                                    cmd = new Command("Remove-RecipientPermission");
                                    cmd.Parameters.Add("Identity", emailAddress);
                                    cmd.Parameters.Add("Trustee", upn.Key);
                                    cmd.Parameters.Add("AccessRights", "SendAs");
                                    cmd.Parameters.Add("Confirm", false);

                                    powershell.Commands.AddCommand(cmd);
                                    results = powershell.Invoke();
                                    powershell.Commands.Clear();
                                    results.Clear();
                                }
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Shared mailbox {0} not found in O365.", emailAddress), null);
                                return false;
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while removing members to shared mailbox {0}.", emailAddress), ex);
                return false;
            }
        }
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            try
            {
                Office365UserInfo userInfo = (Office365UserInfo)_userInfo;
                var credential = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {

                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;

                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Add-DistributionGroupMember"});
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Add Groups email box {0}.", userInfo.WindowsEmailAddress), null);
                            //Add Users to Groups

                            if (userInfo.Groups != null)
                            {
                                foreach (var grpName in userInfo.Groups)
                                {
                                    powershell.Commands.Clear();
                                    _idmsLogger.WriteLog(LogLevelType.Info, $"Adding group {grpName}");

                                    Command AddGroups = new Command("Add-DistributionGroupMember");
                                    AddGroups.Parameters.Add("Identity", grpName);
                                    AddGroups.Parameters.Add("Member", _userInfo.AccountName);
                                    AddGroups.Parameters.Add("BypassSecurityGroupManagerCheck");
                                    AddGroups.Parameters.Add("Confirm", false);
                                    powershell.Commands.AddCommand(AddGroups);
                                    powershell.Invoke();
                                    powershell.Commands.Clear();
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                base.AddEntitlement(_userInfo, _entitlementType);
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while Adding Groups to emailbox {0}.", _userInfo.AccountName), ex);
                return false;
            }
        }
        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            try
            {
                Office365UserInfo userInfo = (Office365UserInfo)_userInfo;
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {

                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;

                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", GetPSCredential());
                            //connect.Parameters.Add("CommandName", new[] { "Remove-DistributionGroupMember" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Removing Groups email box {0}.", userInfo.WindowsEmailAddress), null);

                            //Add Users to Groups

                            if (userInfo.Groups != null)
                            {
                                foreach (var grpName in userInfo.Groups)
                                {
                                    powershell.Commands.Clear();
                                    _idmsLogger.WriteLog(LogLevelType.Info, $"Removing group {grpName}");
                                    Command AddGroups = new Command("Remove-DistributionGroupMember");
                                    AddGroups.Parameters.Add("Identity", grpName);
                                    AddGroups.Parameters.Add("Member", _userInfo.AccountName);
                                    AddGroups.Parameters.Add("BypassSecurityGroupManagerCheck");
                                    AddGroups.Parameters.Add("Confirm", false);
                                    powershell.Commands.AddCommand(AddGroups);
                                    powershell.Invoke();
                                    powershell.Commands.Clear();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                base.RemoveEntitlement(userInfo, _entitlementType);
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while Removing Groups to emailbox {0}. Error:{1}", _userInfo.AccountName, ex.ToString()), ex);
                return false;
            }
        }
        public override bool RemoveAllEntitlements(IEndpointUserInfo _userInfo)
        {
            return false;
        }
        public override bool UpdateMailBoxSettings(string userPrincipalName, ProvisioningParamCollection fields)
        {
            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateMailBoxSettings for -{userPrincipalName}", null);

                int retryCount = 0;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

                //bool mailBoxExists = this.IsMailBoxExists(userPrincipalName);

                //if (!mailBoxExists)
                //{
                //    //wait for the mailbox to create.
                //    int minuteMultiplier = 60000;
                //    int waitTime = MasterConfigHelper.O365MailBoxWaitTime * minuteMultiplier;
                //    Thread.Sleep(waitTime);

                //    while (retryCount < 2)
                //    {
                //        mailBoxExists = this.IsMailBoxExists(userPrincipalName);

                //        if (mailBoxExists)
                //        {
                //            break;
                //        }
                //        else
                //        {
                //            Thread.Sleep(waitTime);
                //            retryCount = retryCount + 1;
                //        }
                //    }
                //}

                //var mailBoxExists = this.IsMailBoxExists(userPrincipalName);

                //if (!mailBoxExists)
                //{
                //    _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User {0} mail box not found.", userPrincipalName), null);
                //    return false;
                //}

                _idmsLogger.WriteLog(LogLevelType.Info, $"Started RunSpace UpdateMailBoxSettings for -{userPrincipalName}", null);

                PSCredential credential = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            //var cmd1 = new PSCommand();
                            //cmd1.AddCommand("Connect-MsolService");
                            //cmd1.AddParameter("Credential", credential);
                            //powershell.Commands = cmd1;

                            //var results = powershell.Invoke();
                            //if (PowerShellErrorCheck(powershell, results))
                            //{
                            //    _idmsLogger.WriteLog(LogLevelType.Info, $"Failed to connect to Connect-MsolService", null);
                            //}
                            //else
                            //{
                            //    _idmsLogger.WriteLog(LogLevelType.Info, $"Connect-MsolService successful", null);
                            //}
                            //powershell.Commands.Clear();

                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", GetPSCredential());
                            //connect.Parameters.Add("CommandName", new[] { "Set-CASMailbox","Enable-Mailbox","Set-Mailbox", "Add-DistributionGroupMember" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command ImapEnabled = new Command("Set-CASMailbox");
                            ImapEnabled.Parameters.Add("identity", userPrincipalName);
                            ImapEnabled.Parameters.Add("ImapEnabled", (fields["DisableIMAP"].Value == "1" ? true : false));
                            ImapEnabled.Parameters.Add("PopEnabled", fields["DisablePOP3"].Value == "1" ? true : false);
                            powershell.Commands.AddCommand(ImapEnabled);
                            powershell.Invoke();

                            var err = powershell.Streams.Error.ReadAll();
                            if (err.Count() > 0)
                            {
                                foreach (var msg in err)
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Error, "Error Occured IN IMAP.", msg.Exception);
                                }
                                return false;
                            }

                            powershell.Commands.Clear();

                            Command Archive = new Command("Enable-Mailbox");
                            Archive.Parameters.Add("identity", userPrincipalName);
                            Archive.Parameters.Add("Archive", fields["EnableArchiving"].Value == "1" ? true : false);
                            powershell.Commands.AddCommand(Archive);
                            powershell.Invoke();
                            powershell.Commands.Clear();

                            Command setMB = new Command("Set-Mailbox");
                            setMB.Parameters.Add("identity", userPrincipalName);
                            setMB.Parameters.Add("RecipientLimits", MasterConfigHelper.Office365RecipientLimits);
                            setMB.Parameters.Add("AuditEnabled", fields["EnableMailBoxAuditing"].Value == "1" ? true : false);
                            setMB.Parameters.Add("LitigationHoldEnabled", fields["EnableLitigationHold"].Value == "1" ? true : false);
                            setMB.Parameters.Add("LitigationHoldDuration", fields["LitigationDuration"].Value.ToString());
                            setMB.Parameters.Add("RetainDeletedItemsFor", fields["RetainDeletedItemsForDays"].Value);
                            setMB.Parameters.Add("RoleAssignmentPolicy", fields["RoleAssignmentPolicy"].Value);
                            setMB.Parameters.Add("RetentionPolicy", fields["RetentionPolicy"].Value);
                            setMB.Parameters.Add("SingleItemRecoveryEnabled", true);
                            powershell.Commands.AddCommand(setMB);
                            powershell.Invoke();
                            powershell.Commands.Clear();

                            //Add Users to Groups
                            List<string> defaultO365Grps = !string.IsNullOrEmpty(fields["Groups"].Value) ? fields["Groups"].Value.Split(',').ToList() : new List<string>();
                            foreach (string grpName in defaultO365Grps)
                            {
                                if (!string.IsNullOrEmpty(grpName))
                                {
                                    Command AddGroups = new Command("Add-DistributionGroupMember");
                                    AddGroups.Parameters.Add("Identity", grpName);
                                    AddGroups.Parameters.Add("Member", userPrincipalName);
                                    AddGroups.Parameters.Add("BypassSecurityGroupManagerCheck");
                                    AddGroups.Parameters.Add("Confirm", false);
                                    powershell.Commands.AddCommand(AddGroups);
                                    powershell.Invoke();
                                    powershell.Commands.Clear();
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while updating mailbox settings for user {0}. Error:{1}", userPrincipalName, ex.ToString()), ex);
                return false;
            }

        }
        public override bool DownloadUser(string userPrincipalName)
        {
            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"O365 DownloadUser {userPrincipalName}");

                IList<Office365UserInfo> list = new List<Office365UserInfo>();
                UserInfoOrchestrator orchestrator = new UserInfoOrchestrator();

                PSCredential credential = GetPSCredential();
                Collection<PSObject> resultmsolusers = new Collection<PSObject>();
                Collection<PSObject> resultusers = new Collection<PSObject>();
                Collection<PSObject> mailboxFullDetails = new Collection<PSObject>();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;

                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Get-User", "Get-EXOMailbox", "Get-RecipientPermission", "Get-MailboxPermission","Get-EXOMailboxStatistics" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            connect = new Command("Connect-MsolService");
                            connect.Parameters.Add("Credential", credential);
                            powershell.Commands.AddCommand(connect);
                            results = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command CheckUserExists = new Command("Get-MsolUser");
                            CheckUserExists.Parameters.Add("UserPrincipalName", userPrincipalName);
                            powershell.Commands.AddCommand(CheckUserExists);
                            resultmsolusers = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command GetUsers = new Command("Get-User");
                            GetUsers.Parameters.Add("Identity", userPrincipalName);
                            powershell.Commands.AddCommand(GetUsers);
                            resultusers = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command GetmailboxFullDetails = new Command("Get-EXOMailbox");
                            GetmailboxFullDetails.Parameters.Add("Identity", userPrincipalName);
                            powershell.Commands.AddCommand(GetmailboxFullDetails);
                            mailboxFullDetails = powershell.Invoke();
                            powershell.Commands.Clear();

                            foreach (PSObject psObject in resultusers)
                            {
                                Office365UserInfo o365userinfo = new Office365UserInfo();
                                PSObject mailboxdetails = null;

                                o365userinfo = orchestrator.PopulateUser(psObject, this.Endpoint.Id);

                                var msolUser = resultmsolusers.FirstOrDefault(s => s.Members["UserPrincipalName"].Value.ToString().ToUpper() == o365userinfo.UserPrincipalName.ToUpper());
                                var mailboxdet = mailboxFullDetails.Where(s => s.Members["UserPrincipalName"].Value.ToString() == o365userinfo.UserPrincipalName);

                                if (msolUser != null)
                                {
                                    Office365UserServicePlan userServicePlan = new Office365UserServicePlan();
                                    List<ServicePlanDetail> listPlanDetails = new List<ServicePlanDetail>();

                                    var boProp = msolUser.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "BaseObject");
                                    if (boProp != null)
                                    {
                                        var bo = boProp.GetValue(msolUser);

                                        if (bo != null)
                                        {
                                            var usgLocProp = bo.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "UsageLocation");
                                            var valStsProp = bo.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ValidationStatus");
                                            var licRecNeedProp = bo.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "LicenseReconciliationNeeded");

                                            o365userinfo.UsageLocation = usgLocProp != null ? (string)usgLocProp.GetValue(bo) : string.Empty;
                                            o365userinfo.ValidationStatus = valStsProp != null ? valStsProp.GetValue(bo).ToString() : string.Empty;

                                            bool flag;
                                            string LicenseReconciliationNeeded = licRecNeedProp != null ? licRecNeedProp.GetValue(bo).ToString() : string.Empty;
                                            Boolean.TryParse(LicenseReconciliationNeeded, out flag);
                                            o365userinfo.LicenseReconciliationNeeded = flag;

                                            var licensesProp = bo.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "Licenses");
                                            IList licenses = null;

                                            if (licensesProp != null)
                                            {
                                                licenses = (IList)licensesProp.GetValue(bo);
                                            }

                                            if (licenses != null && licenses.Count > 0)
                                            {
                                                foreach (object sp in licenses)
                                                {
                                                    string accountSkuId = string.Empty;

                                                    var accSkuProp = sp.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "AccountSkuId");
                                                    var svcStatusProp = sp.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServiceStatus");
                                                    accountSkuId = accSkuProp != null ? (string)accSkuProp.GetValue(sp) : string.Empty;
                                                    var svcStatuses = svcStatusProp != null ? (IList)svcStatusProp.GetValue(sp) : null;

                                                    if (svcStatuses != null)
                                                    {
                                                        foreach (object svcStatus in svcStatuses)
                                                        {
                                                            ServicePlanDetail svcPlan = new ServicePlanDetail();
                                                            svcPlan.AccountSkuId = accountSkuId;
                                                            var property = svcStatus.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServicePlan");
                                                            var prop3 = svcStatus.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ProvisioningStatus");
                                                            svcPlan.ProvisioningStatus = prop3 != null ? prop3.GetValue(svcStatus).ToString() : string.Empty;

                                                            if (property != null)
                                                            {
                                                                var servicePlanObj = property.GetValue(svcStatus);

                                                                var prop1 = servicePlanObj.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServiceName");
                                                                var prop2 = servicePlanObj.GetType().GetProperties().FirstOrDefault(prop => prop.Name == "ServiceType");
                                                                svcPlan.ServiceName = prop1 != null ? (string)prop1.GetValue(servicePlanObj) : string.Empty;
                                                                svcPlan.ServiceType = prop2 != null ? (string)prop2.GetValue(servicePlanObj) : string.Empty;

                                                                listPlanDetails.Add(svcPlan);
                                                            }
                                                        }
                                                    }
                                                }

                                                userServicePlan.PlanDetails = listPlanDetails;
                                                o365userinfo.ServicePlanInfo = UtilityMethods.Serialize<Office365UserServicePlan>(userServicePlan);
                                            }
                                        }
                                    }
                                }

                                Collection<PSObject> sendAsDetail = null;

                                try
                                {
                                    powershell.Commands.Clear();
                                    Command sendAs = new Command("Get-RecipientPermission");
                                    sendAs.Parameters.Add("Identity", o365userinfo.UserPrincipalName);
                                    powershell.Commands.AddCommand(sendAs);
                                    sendAsDetail = powershell.Invoke();
                                    powershell.Commands.Clear();
                                }
                                catch
                                {
                                }

                                if (sendAsDetail != null && sendAsDetail.Count() > 0)
                                {
                                    if (sendAsDetail.Count() > 0)
                                    {
                                        foreach (var item in sendAsDetail)
                                        {
                                            if (item.Members["Trustee"] != null && item.Members["Trustee"].Value != null)
                                            {
                                                string Trustee = item.Members["Trustee"].Value.ToString();
                                                if (o365userinfo.SendAs != null && o365userinfo.SendAs != string.Empty)
                                                {
                                                    o365userinfo.SendAs = o365userinfo.SendAs + "," + Trustee;
                                                }
                                                else
                                                    o365userinfo.SendAs = Trustee;
                                            }
                                        }
                                    }
                                }

                                if (mailboxdet != null && mailboxdet.Count() > 0)
                                {
                                    Collection<PSObject> accessDetails = null;

                                    try
                                    {
                                        powershell.Commands.Clear();
                                        Command mbPermission = new Command("Get-MailboxPermission");
                                        mbPermission.Parameters.Add("Identity", o365userinfo.UserPrincipalName);
                                        powershell.Commands.AddCommand(mbPermission);
                                        accessDetails = powershell.Invoke();
                                        powershell.Commands.Clear();
                                    }
                                    catch { }

                                    if (accessDetails != null && accessDetails.Count() > 0)
                                    {
                                        foreach (PSObject aobj in accessDetails)
                                        {
                                            if (aobj.Members["IsInherited"] != null && aobj.Members["User"] != null && (bool)aobj.Members["IsInherited"].Value != true && aobj.Members["User"].Value.ToString() != "NT AUTHORITY\\SELF")
                                            {

                                                string AccessDet = aobj.Members["User"].Value.ToString();
                                                if (o365userinfo.FullAccess != null && o365userinfo.FullAccess != string.Empty)
                                                {
                                                    if (!(o365userinfo.FullAccess).Contains(AccessDet))
                                                    {
                                                        o365userinfo.FullAccess = o365userinfo.FullAccess + "," + AccessDet;
                                                    }
                                                }
                                                else
                                                    o365userinfo.FullAccess = AccessDet;
                                            }
                                        }
                                    }

                                    Collection<PSObject> usermailboxdetail = null;
                                    try
                                    {
                                        powershell.Commands.Clear();
                                        Command Stat = new Command("Get-EXOMailboxStatistics");
                                        Stat.Parameters.Add("Identity", o365userinfo.UserPrincipalName);
                                        powershell.Commands.AddCommand(Stat);
                                        usermailboxdetail = powershell.Invoke();
                                        powershell.Commands.Clear();
                                    }
                                    catch { }

                                    if (usermailboxdetail != null && usermailboxdetail.Count() > 0)
                                    {
                                        PSObject ps = usermailboxdetail.FirstOrDefault();

                                        if (ps.Members["LastLogonTime"] != null && ps.Members["LastLogonTime"].Value != null)
                                        {
                                            DateTime st = (DateTime)ps.Members["LastLogonTime"].Value;
                                            o365userinfo.LastLogon = st;
                                        }
                                        if (ps.Members["TotalItemSize"] != null && ps.Members["TotalItemSize"].Value != null)
                                        {
                                            string size = ps.Members["TotalItemSize"].Value.ToString();
                                            o365userinfo.MailboxUsage = size;
                                        }
                                    }

                                    mailboxdetails = mailboxdet.FirstOrDefault();
                                    if (mailboxdetails.Members["Alias"] != null && mailboxdetails.Members["Alias"].Value != null)
                                    {
                                        string Alias = mailboxdetails.Members["Alias"].Value.ToString();
                                        o365userinfo.Alias = Alias;
                                    }
                                    if (mailboxdetails.Members["RetentionPolicy"] != null && mailboxdetails.Members["RetentionPolicy"].Value != null)
                                    {
                                        string RetentionPolicy = mailboxdetails.Members["RetentionPolicy"].Value.ToString();
                                        o365userinfo.RetentionPolicy = RetentionPolicy;
                                    }
                                    if (mailboxdetails.Members["LitigationHoldEnabled"] != null && mailboxdetails.Members["LitigationHoldEnabled"].Value != null)
                                    {
                                        bool LitigationHoldEnabled = (bool)mailboxdetails.Members["LitigationHoldEnabled"].Value;
                                        o365userinfo.LitigationHoldEnabled = LitigationHoldEnabled;
                                    }
                                    if (mailboxdetails.Members["RetentionHoldEnabled"] != null && mailboxdetails.Members["RetentionHoldEnabled"].Value != null)
                                    {
                                        bool RetentionHoldEnabled = (bool)mailboxdetails.Members["RetentionHoldEnabled"].Value;
                                        o365userinfo.RetentionHoldEnabled = RetentionHoldEnabled;
                                    }
                                    if (mailboxdetails.Members["ForwardingSmtpAddress"] != null && mailboxdetails.Members["ForwardingSmtpAddress"].Value != null)
                                    {
                                        string ForwardingSmtpAddress = mailboxdetails.Members["ForwardingSmtpAddress"].Value.ToString();
                                        o365userinfo.ForwardingSmtpAddress = ForwardingSmtpAddress;
                                    }
                                    if (mailboxdetails.Members["LitigationHoldDuration"] != null && mailboxdetails.Members["LitigationHoldDuration"].Value != null)
                                    {
                                        string LitigationHoldDuration = mailboxdetails.Members["LitigationHoldDuration"].Value.ToString();
                                        o365userinfo.LitigationHoldDuration = LitigationHoldDuration;
                                    }
                                    if (mailboxdetails.Members["IsMailboxEnabled"] != null && mailboxdetails.Members["IsMailboxEnabled"].Value != null)
                                    {
                                        bool IsMailboxEnabled = (bool)mailboxdetails.Members["IsMailboxEnabled"].Value;
                                        o365userinfo.IsMailboxEnabled = IsMailboxEnabled;
                                    }
                                    if (mailboxdetails.Members["Languages"] != null && mailboxdetails.Members["Languages"].Value != null)
                                    {
                                        string Languages = mailboxdetails.Members["Languages"].Value.ToString();
                                        o365userinfo.Languages = Languages;
                                    }
                                    if (mailboxdetails.Members["IsShared"] != null && mailboxdetails.Members["IsShared"].Value != null)
                                    {
                                        bool IsShared = (bool)mailboxdetails.Members["IsShared"].Value;
                                        o365userinfo.IsShared = IsShared;
                                    }
                                    if (mailboxdetails.Members["RoleAssignmentPolicy"] != null && mailboxdetails.Members["RoleAssignmentPolicy"].Value != null)
                                    {
                                        string RoleAssignmentPolicy = mailboxdetails.Members["RoleAssignmentPolicy"].Value.ToString();
                                        o365userinfo.RoleAssignmentPolicy = RoleAssignmentPolicy;
                                    }
                                    if (mailboxdetails.Members["SharingPolicy"] != null && mailboxdetails.Members["SharingPolicy"].Value != null)
                                    {
                                        string SharingPolicy = mailboxdetails.Members["SharingPolicy"].Value.ToString();
                                        o365userinfo.SharingPolicy = SharingPolicy;
                                    }
                                    if (mailboxdetails.Members["ArchiveName"] != null && mailboxdetails.Members["ArchiveName"].Value != null)
                                    {
                                        string ArchiveName = mailboxdetails.Members["ArchiveName"].Value.ToString();
                                        o365userinfo.ArchiveName = ArchiveName;
                                    }
                                    if (mailboxdetails.Members["ArchiveStatus"] != null && mailboxdetails.Members["ArchiveStatus"].Value != null)
                                    {
                                        string ArchiveStatus = mailboxdetails.Members["ArchiveStatus"].Value.ToString();
                                        o365userinfo.ArchiveStatus = ArchiveStatus;
                                    }
                                    if (mailboxdetails.Members["RecipientType"] != null && mailboxdetails.Members["RecipientType"].Value != null)
                                    {
                                        string RecipientType = mailboxdetails.Members["RecipientType"].Value.ToString();
                                        o365userinfo.RecipientType = RecipientType;
                                    }
                                    if (mailboxdetails.Members["grantsendonbehalfto"] != null && mailboxdetails.Members["grantsendonbehalfto"].Value != null)
                                    {
                                        string SendonBehalf = mailboxdetails.Members["grantsendonbehalfto"].Value.ToString();
                                        o365userinfo.SendonBehalf = SendonBehalf;
                                    }
                                }

                                if (null != o365userinfo)
                                    list.Add(o365userinfo);
                            }

                        }
                        catch (Exception ex)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Exception, $"DownloadUser failed:{ex.ToString()}", ex);
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }

                //_idmsLogger.WriteLog(LogLevelType.Info, "Save Users Started");
                var allExistingUsers = Office365UserInfo.GetAllByEndpointId(this.Endpoint.Id);
                var exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id).ToList();

                Office365UserInfo.SaveUsers(list, this.Endpoint.Id, MasterConfigHelperServiceUser, exsitngAccountrecon, allExistingUsers);

                Office365UserInfo.MapWithMasterUserForReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, 0);
                try
                {
                    // _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateOffBoardingByEndpointId started");
                    AccountReconcilation.UpdateOffBoardingByEndpointId(this.Endpoint.Id);
                    // _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateOffBoardingByEndpointId Completed");
                }
                catch (Exception ex)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception, $"UpdateOffBoardingByEndpointId exception", ex);
                }
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while downloading user {0}. Error {1}", userPrincipalName, ex.ToString()), ex);
                return false;
            }
        }
        public override bool AssignLicense(string userPrincipalName, Dictionary<string, string> licenses)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

                var psModuleList = new List<string>();
                foreach (var item in this.psModules)
                {
                    psModuleList.Add(item);
                }
                if (!string.IsNullOrWhiteSpace(this.psAzureADModules))
                {
                    psModuleList.Add(this.psAzureADModules);
                }

                PSCredential credential = GetPSCredential();
                Collection<System.Management.Automation.PSObject> results = null;
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;

                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            powershell.Commands.AddCommand(connect);
                            results = powershell.Invoke();

                            if (PowerShellErrorCheck(powershell, results))
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Failed to connect Connect-ExchangeOnline", null);
                                return false;
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Connect-ExchangeOnline successful", null);
                            }
                            powershell.Commands.Clear();
                            var cmd1 = new PSCommand();
                            cmd1.AddCommand("Connect-MsolService");
                            cmd1.AddParameter("Credential", credential);
                            powershell.Commands = cmd1;
                            results = powershell.Invoke();
                            powershell.Commands.Clear();
                            _idmsLogger.WriteLog(LogLevelType.Info, "Connect-MsolService Connected", null);

                            powershell.Runspace = runspace;
                            var cmd = $"Get-MsolUser -UserPrincipalName \"{userPrincipalName}\"";
                            powershell.Commands.AddScript(cmd);
                            results = powershell.Invoke();

                            if (PowerShellErrorCheck(powershell, results))
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, $"Failed to Get-MsolUser {userPrincipalName}", null);
                                return false;
                            }
                            powershell.Commands.Clear();

                            if (results != null && results.Count > 0)
                            {
                                powershell.Runspace = runspace;
                                cmd = $"Set-MSOLUser -UserPrincipalName {userPrincipalName} -UsageLocation {"SG"}";
                                powershell.Commands.AddScript(cmd);
                                results = powershell.Invoke();
                                if (PowerShellErrorCheck(powershell, results))
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Error, $"Failed to set UsageLocation: {cmd}", null);
                                    return false;
                                }

                                powershell.Commands.Clear();

                                results.Clear();

                                var exisitingLicense = new List<string>();
                                try
                                {
                                    //Below code is to verify that whether the license is already assigned to the user or not
                                    powershell.Runspace = runspace;
                                    cmd = $"(Get-MsolUser -UserPrincipalName {userPrincipalName}).licenses.AccountSkuId";
                                    powershell.Commands.AddScript(cmd);
                                    results = powershell.Invoke();
                                    if (PowerShellErrorCheck(powershell, results))
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Error, $"Failed to check : {cmd}", null);
                                    }

                                    if (results != null)
                                    {
                                        foreach (var item in results)
                                        {
                                            if (item == null) continue;
                                            exisitingLicense.Add(item.ToString().Trim());
                                            _idmsLogger.WriteLog(LogLevelType.Info, $"Get existing User Licenses: {item}");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Error, $"Get User Licenses: {cmd} failed.Error:{ex.ToString()}", ex);
                                }
                                powershell.Commands.Clear();
                                results.Clear();
                                powershell.Commands.Clear();
                                var connectAzureAD = new PSCommand();
                                connectAzureAD.AddCommand("Connect-AzureAD");
                                connectAzureAD.AddParameter("Credential", credential);
                                powershell.Commands = connectAzureAD;
                                results = powershell.Invoke();
                                powershell.Commands.Clear();
                                _idmsLogger.WriteLog(LogLevelType.Info, "Connect-AzureAD Connected", null);

                                if (PowerShellErrorCheck(powershell, results))
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Error, $"{cmd}. Connect-AzureAD failed", null);
                                    return false;
                                }
                                powershell?.Streams?.Error?.Clear();
                                if (licenses.Count > 0)
                                {
                                    var licenseList = string.Join(",", licenses.Select(i => $"'{i.Key}'"));
                                    string scriptAzureLicenseAssignment = @"
                                        $user = Get-AzureADUser -ObjectId '@@@@USERPRINCIPALNAME@@@@'
                                        $licenseSKUs = @(@@@@LICENSES@@@@)
                                        foreach ($skuId in $licenseSKUs) {
                                            $license = Get-AzureADSubscribedSku | Where-Object { $_.SkuId -eq $skuId }
                                            $isAssigned = $user.AssignedLicenses | Where-Object { $_.SkuId -eq $skuId }
                                            if ($isAssigned) {
                                                Write-Host ""License with SKU $skuId is already assigned.""
                                            } elseif ($license.AvailableUnits -gt 0) {
                                                Add-AzureADUserLicense -ObjectId $user.ObjectId -AssignedLicenses $license
                                                Write-Host ""Assigned license with SKU: $skuId""
                                            } else {
                                                Write-Host ""License with SKU $skuId is not available.""
                                            }
                                        }
                                    ";

                                    scriptAzureLicenseAssignment = scriptAzureLicenseAssignment.Replace("@@@@USERPRINCIPALNAME@@@@", userPrincipalName).Replace("@@@@LICENSES@@@@", licenseList);

                                    _idmsLogger.WriteLog(LogLevelType.Info, $"Assign License Script:  {scriptAzureLicenseAssignment}");


                                    powershell.Commands.AddScript(scriptAzureLicenseAssignment);
                                    results = powershell.Invoke();
                                    try
                                    {
                                        foreach (PSObject outputItem in results)
                                        {
                                            if (outputItem != null)
                                            {
                                                _idmsLogger.WriteLog(LogLevelType.Info, $"Response from PS: {outputItem?.BaseObject?.ToString()}");
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    if (PowerShellErrorCheck(powershell, results))
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Error, $"{cmd}. License already assigned/no License available", null);
                                        return false;
                                    }
                                }
                                else
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Error, $"No License available", null);
                                }
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User {0} not found in O365.", userPrincipalName), null);
                                return false;
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while assigning License to user {0}. Error:{1}", userPrincipalName, ex.ToString()), ex);
                return false;
            }
        }
        public override bool RemoveLicense(string userPrincipalName, Dictionary<string, string> licenses, List<string> licensesToRemove)
        {
            try
            {
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                var credentails = GetPSCredential();
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            Command connect = new Command("Connect-MsolService");
                            connect.Parameters.Add("Credential", credentails);
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();

                            PowerShellErrorCheck(powershell, results);
                            powershell.Commands.Clear();

                            connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credentails);
                            powershell.Commands.AddCommand(connect);
                            results = powershell.Invoke();

                            if (PowerShellErrorCheck(powershell, results))
                            {
                                return false;
                            }

                            powershell.Commands.Clear();
                            Command CheckUserExists = new Command("Get-MsolUser");
                            CheckUserExists.Parameters.Add("UserPrincipalName", userPrincipalName);
                            powershell.Commands.AddCommand(CheckUserExists);
                            results = powershell.Invoke();
                            powershell.Commands.Clear();
                            if (results != null && results.Count > 0)
                            {
                                foreach (KeyValuePair<string, string> license in licenses)
                                {
                                    if (licensesToRemove.Contains(license.Key))
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Removing {0} License to user {1}", license.Key, userPrincipalName), null);
                                        Command licence = new Command("Set-MsolUserLicense");
                                        licence.Parameters.Add("UserPrincipalName", userPrincipalName);
                                        licence.Parameters.Add("RemoveLicenses", license.Key);
                                        powershell.Commands.AddCommand(licence);
                                        results = powershell.Invoke();
                                        powershell.Commands.Clear();
                                        results.Clear();
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(license.Value))
                                        {
                                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Removing subscriptions {0} to License {1} to user {2}", license.Value, license.Key, userPrincipalName), null);
                                            Command lic1 = new Command("Out-String");
                                            string accessScriptPath = string.Format("$options = New-MsolLicenseOptions -AccountSkuId {0} -DisabledPlans {1} \n Set-MsolUserLicense -UserPrincipalName {2} -LicenseOptions $options", license.Key, license.Value, userPrincipalName);
                                            powershell.Commands.AddScript(accessScriptPath);
                                            powershell.Commands.AddCommand(lic1);
                                            results = powershell.Invoke();
                                            powershell.Commands.Clear();
                                            results.Clear();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("User {0} not found in O365.", userPrincipalName), null);
                                return false;
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while removing License to user {0}. Error{1}", userPrincipalName, ex.ToString()), ex);
                return false;
            }
        }
        public override bool ConvertSharedToRegularMailBox(string emailAddress)
        {
            try
            {

                string userName = this.Configuration(EndpointKey.UserName);
                string password = this.Configuration(EndpointKey.Password);

                System.Security.SecureString securePassword = new System.Security.SecureString();
                foreach (char c in password)
                {
                    securePassword.AppendChar(c);
                }

                PSCredential credential = new PSCredential(userName, securePassword);
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;

                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Get-EXOMailbox", "Set-Mailbox" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command CheckUserExists = new Command("Get-EXOMailbox");
                            CheckUserExists.Parameters.Add("Identity", emailAddress);
                            powershell.Commands.AddCommand(CheckUserExists);
                            results = powershell.Invoke();
                            powershell.Commands.Clear();
                            if (results != null && results.Count > 0)
                            {
                                var pwd = password.Reverse();
                                System.Security.SecureString securePwd = new System.Security.SecureString();
                                foreach (char c in pwd)
                                {
                                    securePwd.AppendChar(c);
                                }

                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Converting shared to regular mailbox {0}", emailAddress), null);

                                Command cmd = new Command("Set-Mailbox");
                                cmd.Parameters.Add("Identity", emailAddress);
                                cmd.Parameters.Add("RecipientLimits", MasterConfigHelper.Office365RecipientLimits);
                                cmd.Parameters.Add("Type", "Regular");
                                cmd.Parameters.Add("Password", securePwd);
                                cmd.Parameters.Add("ResetPasswordOnNextLogon", 0);
                                cmd.Parameters.Add("Confirm", false);

                                powershell.Commands.AddCommand(cmd);
                                results = powershell.Invoke();
                                powershell.Commands.Clear();
                                results.Clear();

                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Mailbox {0} not found in O365.", emailAddress), null);
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while converting shared to regular mailbox {0}. Error:{1}", emailAddress, ex.ToString()), ex);
                return false;
            }
        }
        public override bool ConvertRegularToSharedMailBox(string emailAddress)
        {
            try
            {
                PSCredential credential = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;

                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            //connect.Parameters.Add("CommandName", new[] { "Get-EXOMailbox", "Set-Mailbox" });
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();
                            powershell.Commands.Clear();

                            Command CheckUserExists = new Command("Get-EXOMailbox");
                            CheckUserExists.Parameters.Add("Identity", emailAddress);
                            powershell.Commands.AddCommand(CheckUserExists);
                            results = powershell.Invoke();
                            powershell.Commands.Clear();
                            if (results != null && results.Count > 0)
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Converting regular to shared mailbox {0}", emailAddress), null);

                                Command cmd = new Command("Set-Mailbox");
                                cmd.Parameters.Add("Identity", emailAddress);
                                cmd.Parameters.Add("RecipientLimits", MasterConfigHelper.Office365RecipientLimits);
                                cmd.Parameters.Add("Type", "Shared");
                                cmd.Parameters.Add("Confirm", false);

                                powershell.Commands.AddCommand(cmd);
                                results = powershell.Invoke();
                                powershell.Commands.Clear();
                                results.Clear();
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Mailbox {0} not found in O365.", emailAddress), null);
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while converting regular to shared mailbox {0}. Error{1}", emailAddress, ex.ToString()), ex);
                return false;
            }
        }
        #endregion

        private bool PowerShellErrorCheck(PowerShell powershell, Collection<PSObject> results)
        {
            bool isError = false;
            if (powershell.Streams != null && powershell.Streams.Error != null && powershell.Streams.Error.Count > 0)
            {
                isError = true;
                var strError = string.Empty;
                foreach (ErrorRecord er in powershell.Streams.Error)
                {
                    strError += "Exception:" + er.Exception?.ToString();
                    strError += "InnerException:" + er.Exception?.InnerException?.ToString();
                }
                _idmsLogger.WriteLog(LogLevelType.Error, $"PowerShellErrorCheck: Powershell Invoke Error: {strError}");
            }
            return isError;
        }
        public override bool CheckOnlineAccountExists(string accountName)
        {
            var isExists = false;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };


                PSCredential credential = GetPSCredential();
                Collection<System.Management.Automation.PSObject> results = null;
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(this.psModules);
                using (Runspace runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(iss))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", credential);
                            powershell.Commands.AddCommand(connect);
                            results = powershell.Invoke();

                            if (PowerShellErrorCheck(powershell, results))
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Failed to connect Connect-ExchangeOnline", null);
                                return false;
                            }
                            else
                            {
                                isExists = true;
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Connect-ExchangeOnline successful", null);
                            }
                            powershell.Commands.Clear();

                            //var cmd1 = new PSCommand();
                            //cmd1.AddCommand("Connect-MsolService");
                            //cmd1.AddParameter("Credential", credential);
                            //powershell.Commands = cmd1;
                            //results = powershell.Invoke();
                            //powershell.Commands.Clear();
                            //_idmsLogger.WriteLog(LogLevelType.Info, "Connect-MsolService Connected", null);

                            //powershell.Runspace = runspace;
                            //var cmd = $"Get-MsolUser -UserPrincipalName \"{accountName}\" -ErrorAction SilentlyContinue";
                            //powershell.Commands.AddScript(cmd);
                            //results = powershell.Invoke();

                            //if (!PowerShellErrorCheck(powershell, results))
                            //{
                            //    if (results != null && results.Count > 0)
                            //    {
                            //        _idmsLogger.WriteLog(LogLevelType.Error, $"User {accountName} found", null);
                            //        isExists = true;
                            //    }
                            //    else
                            //    {
                            //        _idmsLogger.WriteLog(LogLevelType.Error, $"User {accountName} waiting for Sync", null);
                            //    }
                            //}
                           // powershell.Commands.Clear();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            DisconnectExchangeOnline(powershell);
                            runspace.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while assigning License to user {0}. Error:{1}", accountName, ex.ToString()), ex);
                isExists = false;
            }
            return isExists;
        }

        public override bool MailBoxExists(string accountName)
        {
            return IsMailBoxExists(accountName);
        }
    }

}
