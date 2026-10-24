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
using System.Text;
using DI.Chakra.Services.EmailingProcessor;

namespace DI.Chakra.Connector
{
    public class SkypeForBusinessConnector : ConnectorBase
    {
        readonly IdmsLogger _idmsLogger;
        #region Constants and Variable
        private const string userStatusOnline = "OnLine";
        private const string userStatusOnPrem = "On-Premises";
        private const string PROCESSNAME = "DiscoveryProcess";

        private string connectionUri = string.Empty;
        private const string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";
        private string userName;
        private string password;
        private SecureString securePassword;
        private PSCredential credential = null;

        private string onLineUserName;
        private string onLinePassword;
        private SecureString onLineSecurePassword;
        private PSCredential onLineCredential = null;

        private string currentUser = string.Empty;
        private string hostingproviderfqdn = string.Empty;
        string userSIPAddress = string.Empty;

        private string registrarPool1 = string.Empty;
        private string registrarPool2 = string.Empty;
        private string LineUriPrefix = string.Empty;
        private string onLineToOnPremTarget = string.Empty;
        private string onPremToOnLineTarget = string.Empty;

        private string conferencingpolicy = string.Empty;
        private string externalAccessPolicy = string.Empty;

        private string HostedMigrationOverrideUrl = string.Empty;
        private string domainController = string.Empty;
        private const string userObject = "GetCSUser";
        private const string unAssignedNumber = "GetUnAssignedNumber";
        private const string responseGroup = "GetResponseGroup";
        private const string voicePolicy = "GetVoicePolicy";
        private const string dialPlan = "GetDialPlan";
        private readonly int PSVersion;
        private readonly int MinorPSVersion;

        private const string getUnAssignedNumberScript = "Get-CsUnassignedNumber";


        private int PowershellScriptExecutionDelay = 0;
        private SkypeForBusinessProperties properties = null;
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private List<SkypeForBusinessResponseGroupInfo> Groups = new List<SkypeForBusinessResponseGroupInfo>();
        private Dictionary<string, ArrayList> dicGroupUsers = new Dictionary<string, ArrayList>();
        private bool isDiscoveryError = false;
        private string defaulthostProviderString = "SRV:";
        private string[] psModules = new string[] { };
        private string[] psMSTeamO365Modules = new string[] { };

        #endregion
        #region public property
        public string VoicePolicyName { get; set; } = string.Empty;
        public string VoiceRoutingPolicyName { get; set; } = string.Empty;
        public string DialPlanName { get; set; } = string.Empty;
        public string IDDVoicePolicyName { get; set; } = string.Empty;
        public string IDDVoiceRoutingPolicyName { get; set; } = string.Empty;
        public string IDDDialPlanName { get; set; } = string.Empty;
        public string DCCVoicePolicyName { get; set; } = string.Empty;
        public string DCCVoiceRoutingPolicyName { get; set; } = string.Empty;
        public string DCCDialPlanName { get; set; } = string.Empty;
        public string DCCSVoicePolicyName { get; set; } = string.Empty;
        public string DCCSVoiceRoutingPolicyName { get; set; } = string.Empty;
        public string DCCSDialPlanName { get; set; } = string.Empty;
        //For Default Policy
        public string DefaultTeamDialPlanPolicy { get; set; } = string.Empty;
        public string DefaultOnlineVoiceRoutingPolicy { get; set; } = string.Empty;
        //For IDD without Response Group
        public string IDDTenantDialPlanPolicy { get; set; } = string.Empty;
        public string IDDOnlineVoiceRoutingPolicy { get; set; } = string.Empty;
        private string TeamsUpgradePolicy = string.Empty;
        
        private string DefaultDCCLineUriPrefix{ get; set; }// ="tel:+6569167777;ext=";
        public string SFBFrontEndServer { get; set; } = string.Empty;
        private string clearADAttribute = "Set-ADUser -Identity {0} -Server {1} -Clear msRTCSIP-DeploymentLocator";
        Collection<PSObject> Onlineusers = new Collection<PSObject>();
        List<SkypeForBusinessUserInfo> sfbUsers = new List<SkypeForBusinessUserInfo>();
        

        public string LineUri
        {
            get;
            set;
        }
        private string SystemName
        {
            get
            {
                if(this.Endpoint != null)
                {
                    return this.Endpoint.DisplayName;
                }
                return "Microsoft Teams";
            }
        }
        
        //public List<string> IDDTenantDialPlanPolicies { get; set; } = new List<string>();
        //public List<string> TeamsIDDOnlineVoiceRoutingPolicies { get; set; } = new List<string>();

        public string ON_SetCsUser { get; private set; }
        public string ON_SetCsUserEnterpriseVoiceEnabled { get; private set; }
        public string ON_GRANT_CS_TDP { get; private set; }
        public string ON_GRANT_CS_OVRP { get; private set; }
        public string ON_GRANT_CS_TUP { get; private set; }
        public string ON_GRANT_CS_EAP { get; private set; }
        public string ON_GRANT_CS_CONFPLY { get; private set; }
        public string ON_GET_CSUSER { get; private set; }
        public string ON_DGMemberCheck { get; private set; }
        private string ON_CsCallQueueScript = "Get-CsCallQueue | Select Identity,Name,DistributionLists";
        
        public string ON_UserHasResponseGroup { get; private set; }
        public string ON_GETCS_SPECIFIC_USER { get; set; }
        //public string OP_GETCS_SPECIFIC_USER { get; set; }
        public string ON_FindUserLineURI { get; private set; }
        
        public string ON_Remove_MSTeamLineURI { get; private set; }
        /// <summary>
        /// Must be used once Response group were added
        /// </summary>
        public string ON_AddResponseGroup_Delegate { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string ON_RemoveDelegate_AfterResponseGroup { get; private set; }
        public string IDDDialPlanWithResponseGroupPolicy { get; set; } = string.Empty;
        public string IDDOnlineVoiceRoutingWithResponseGroupPolicy { get; set; } = string.Empty;

        #endregion

        public SkypeForBusinessConnector(DI.Chakra.DomainModel.EndPoint _endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            psModules = MasterConfigHelper.MicrosoftTeamsPSModule;
            this.psMSTeamO365Modules = MasterConfigHelper.Office365PSModuleForMSTeam;
            isDiscoveryError = false;
            this.Endpoint = _endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            _idmsLogger = logger;
            this.userName = Convert.ToString(Configuration(EndpointKey.UserName));
            this.password = Convert.ToString(Configuration(EndpointKey.Password));
            securePassword = this.password.SecureStringPassword();
            this.credential = new PSCredential(userName, securePassword);

            this.onLineUserName = Convert.ToString(Configuration(EndpointKey.AzueADUserName));
            this.onLinePassword = Convert.ToString(Configuration(EndpointKey.AzureADUserPassword));
            this.onLineSecurePassword = this.onLinePassword.SecureStringPassword();
            this.onLineCredential = new PSCredential(onLineUserName, onLineSecurePassword);

            this.connectionUri = Convert.ToString(Configuration(EndpointKey.ConnectionUri));
            currentUser = MasterConfigHelper.ServiceUser;
            var provisioningFields = NProvisioningField.GetByEndpointSystemId(this.Endpoint.Id);
            SetProvisionongFields();
            SFBFrontEndServer = MasterConfigHelper.SkypeForBusinessFrontEndServer;
            this.PSVersion = MasterConfigHelper.PSVersion;
            MinorPSVersion = MasterConfigHelper.MinorPSVersion;
        }
        #region Connect
        public override bool Connect()
        {
            return this.ConnectOnLine();
        }
        /// <summary>
        /// no longer required
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public bool ConnectOnPremise()
        {
            bool isConnected = false;
            try
            {
                using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
                {
                    using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                    {
                        runspace.Open();
                        using (PowerShell powershell = PowerShell.Create())
                        {
                            PSCommand command = new PSCommand();
                            command.AddCommand("New-PSSession");
                            command.AddParameter("ConnectionUri", new Uri(connectionUri));
                            command.AddParameter("Credential", credential);
                            command.AddParameter("Authentication", "Default");
                            powershell.Commands = command;
                            powershell.Runspace = runspace;
                            Collection<System.Management.Automation.PSObject> result = powershell.Invoke();
                            if (!PowerShellErrorCheck(powershell, result))
                            {
                                isConnected = true;
                            }
                            else
                            {
                                WriteDiscoveryLog(LogLevelType.Exception, "Failed to Connect to OnPremise");
                                isConnected = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isConnected = false;
                WriteDiscoveryLog(LogLevelType.Exception, "Failed to Connect to OnPremise", ex);
            }
            return isConnected;
        }
        /// <summary>
        /// Try to connect to online
        /// </summary>
        /// <returns></returns>
        public override bool ConnectOnLine()
        {
            bool isConnected = false;
            try
            {
                using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
                {
                    using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                    {
                        runspace.Open();
                        using (PowerShell powershell = PowerShell.Create())
                        {
                            powershell.Runspace = runspace;
                            try
                            {
                                foreach (var module in psModules)
                                {
                                    powershell.AddScript($"Import-Module \"{module}\"");
                                }
                                var impModule = powershell.Invoke();
                                if (!PowerShellErrorCheck(powershell, impModule))
                                {
                                    powershell.Commands.Clear();
                                    var connect = new Command("Connect-MicrosoftTeams");
                                    connect.Parameters.Add("Credential", onLineCredential);
                                    powershell.Commands.AddCommand(connect);
                                    var mdlResult = powershell.Invoke();
                                    if (!PowerShellErrorCheck(powershell, mdlResult))
                                    {
                                        isConnected = true;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteProvisioningLog(LogLevelType.Info, $"Connect to OnLine failed", ex);
                            }
                            finally
                            {
                                try
                                {
                                    powershell.Commands.Clear();
                                    var disConnect = new Command("Disconnect-MicrosoftTeams");
                                    powershell.Commands.AddCommand(disConnect);
                                    var moduelImport = powershell.Invoke();
                                }
                                catch (Exception disConnectEx)
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"Disconnect-MicrosoftTeams failed", disConnectEx);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isConnected = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, "Failed to Connect to OnLine", ex);
            }
            return isConnected;
        }
        #endregion
        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            return true;
        }
        
        
        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            bool isAccountCreated = false;
            var userInfo = _userInfo as SkypeForBusinessUserInfo;
            var onLineExecutionscripts = new List<SkypeForBusinessScriptBlock>();

            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, "Create Account started");
                var userIdentity = userInfo.UserPrincipalName;

                if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TDP))
                {
                    var tenantDialPlan = ON_GRANT_CS_TDP.Replace("@@#UPN#@@", userIdentity).Replace("@@#TENANTDIALPLAN#@@", userInfo.TenantDialPlan);
                    onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = tenantDialPlan });
                }
                if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_OVRP))
                {
                    var onlineVRP = ON_GRANT_CS_OVRP.Replace("@@#UPN#@@", userIdentity).Replace("@@#ONLINEVOICEROUTINGPOLICY#@@", userInfo.OnlineVoiceRoutingPolicy);
                    onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = onlineVRP });
                }

                if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_EAP))
                {
                    var GrantCsExternalAccessPolicy = ON_GRANT_CS_EAP.Replace("@@#UPN#@@", userIdentity);
                    onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = GrantCsExternalAccessPolicy });
                }
                //Moved this line before set phone number assignment
                if (!string.IsNullOrWhiteSpace(ON_SetCsUserEnterpriseVoiceEnabled))
                {
                    var setOnlineEnterPriseVoiceEnabled = ON_SetCsUserEnterpriseVoiceEnabled.Replace("@@#UPN#@@", userIdentity);
                    onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = setOnlineEnterPriseVoiceEnabled });
                }

                if (!string.IsNullOrWhiteSpace(ON_SetCsUser))
                {
                    var setOnlineEnterPriseVoice = ON_SetCsUser.Replace("@@#UPN#@@", userIdentity).Replace("@@#LINEURI#@@", userInfo.LineURI);
                    onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = setOnlineEnterPriseVoice });
                }
               
                if (!string.IsNullOrWhiteSpace(ON_GETCS_SPECIFIC_USER))
                {
                    onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = ON_GETCS_SPECIFIC_USER.Replace("@@#UPN#@@", userIdentity), IncludeObject = true });
                }
                isAccountCreated = ExecuteOnLineLineScript(onLineExecutionscripts);
            }
            catch (Exception ex)
            {
                isAccountCreated = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, "Create Account Enable Command is failed ", ex);
            }
            finally
            {
                onLineExecutionscripts?.Clear();
                onLineExecutionscripts = null;
            }    
            return isAccountCreated;
        }
        public override bool ExecuteNewAccountPolicy(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields, AdHocActionType actionType)
        {
            bool isAccountCreated = false;
            var userInfo = _userInfo as SkypeForBusinessUserInfo;
            var onLineExecutionscripts = new List<SkypeForBusinessScriptBlock>();
            try
            {
                if (actionType == AdHocActionType.NewAccount)
                {
                    #region NewAccount
                    //if (Connect() && ConnectOnLine())
                    {
                        var userIdentity = userInfo.UserPrincipalName;
                        userSIPAddress = userInfo.SipAddress;
                        this.LineUri = userInfo.LineURI;
                        
                        if (CheckOnlineAccountExists(userIdentity))
                        {
                            
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                isAccountCreated = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, "PostAccountCreation Failed", ex);
            }
            finally
            {
                onLineExecutionscripts?.Clear();
                onLineExecutionscripts = null;
            }     
            return isAccountCreated;
        }
        public override void Disconnect()
        {

        }
        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            return true;
        }
        public override void GetData()
        {
            try
            {
                long DiscoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

                var IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SFB_User) != null;
                var IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SFB_ResponseGroup) != null;
                var IsUnAssignedNumberSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SFB_UnAssignedNumberRange) != null;
                if (IsUnAssignedNumberSelected && !IsUserSelected && !IsGroupSelected)
                {
                    GenerateNumberMatrix();
                }
                else
                {
                    properties = new SkypeForBusinessProperties();
                    if (IsUserSelected || IsGroupSelected)
                    {
                        DiscoverUser();
                        DiscoverResponseGroup();
                        DiscoverResponseGroupMember();
                        
                        //DiscoverMaskingDetails();
                    }
                    UpdateEntitlementRecon();

                    SkypeForBusinessUserInfo.MapWithMasterUserForReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
                    SkypeForBusinessUserInfo.MapWithMasterUserForEntitlementReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
                    GenerateNumberMatrix();
                    //DiscoverResponseGroupDelegateInfo();
                }
                //Dont update
                //UpdateHRMSandUserProfileForTelephoneNumber();


            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Discovery failed", ex);
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// Used to update the telephonenumber
        /// </summary>
        public void UpdateHRMSandUserProfileForTelephoneNumber()
        {
            try
            {
                var usrs = SkypeForBusinessUserInfo.GetValidActiveUser(this.Endpoint.Id);
                var updatingFields = "";
                foreach (var usr in usrs)
                {
                    updatingFields = "";
                    if (usr.EmployeeType == EmployeeType.Permanent)
                    {
                        updatingFields = ",NeedTelephoneUpdateToHR = 1 ";
                    }
                    // {0} is EF's own positional-parameter token (bound as a real SqlParameter
                    // by Database.ExecuteSqlCommand), not a string.Format substitution -
                    // usr.UserTelephoneNumber comes from the HR/telephony feed and must never be
                    // concatenated into the SQL text directly.
                    var query = $"UPDATE DI_M_User SET TelephoneNumber={{0}} {updatingFields} WHERE ID = {usr.UserId}";
                    SkypeForBusinessUserInfo.ExecuteQuery(query, usr.UserTelephoneNumber);

                    WriteDiscoveryLog(LogLevelType.Info, $"UserName: {usr.AccountName} Old TelephoneNumber:{usr.UserOldTelephoneNumber} MSTeam Number:{usr.UserTelephoneNumber}");
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"UpdateHRMSandUserProfileForTelephoneNumber failed:{ex.Message}", ex);
            }
        }

        private void DiscoverResponseGroupMember()
        {
            List<string> queryList = new List<string>();
            try
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"Discover ResponseGroup Member started",DiscoveryObjectType.ResponseGroup);
                var spDomain = MasterConfigHelper.ADUPNDomainName;
                using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
                {
                    using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                    {
                        runspace.Open();
                        using (PowerShell powershell = PowerShell.Create())
                        {
                            powershell.Runspace = runspace;
                            foreach (var module in psMSTeamO365Modules)
                            {
                                powershell.AddScript($"Import-Module \"{module}\"");
                            }
                            var impModule = powershell.Invoke();
                            if (!PowerShellErrorCheck(powershell, impModule))
                            {
                                try
                                {
                                    powershell.Commands.Clear();
                                    powershell.Runspace = runspace;
                                    var connect = new Command("Connect-ExchangeOnline");
                                    connect.Parameters.Add("Credential", credential);
                                    //connect.Parameters.Add("CommandName", new[] { "Get-DistributionGroup", "Get-DistributionGroupMember" });
                                    powershell.Commands.AddCommand(connect);
                                    var results = powershell.Invoke();
                                    powershell.Commands.Clear();
                                    List<string> mem = new List<string>();
                                    foreach (var respGrp in Groups)
                                    {
                                        powershell.Commands.Clear();
                                        if (!string.IsNullOrWhiteSpace(respGrp.DistributionGroupAddress))
                                        {
                                            Command getDistGroup = new Command("Get-DistributionGroup");
                                            getDistGroup.Parameters.Add("identity", respGrp.DistributionGroupAddress);
                                            getDistGroup.Parameters.Add("ResultSize", "unlimited");
                                            var pipeline2 = runspace.CreatePipeline();
                                            pipeline2.Commands.Add(getDistGroup);
                                            var resultgroups = pipeline2.Invoke();
                                            foreach (PSObject psObject in resultgroups)
                                            {
                                                mem.Clear();
                                                if (psObject != null)
                                                {
                                                    powershell.Commands.Clear();
                                                    var dgName = SkypeForBusinessHelper.GetProperty(psObject, "DisplayName");
                                                    var Identity = SkypeForBusinessHelper.GetProperty(psObject, "Identity");

                                                    if (!string.IsNullOrWhiteSpace(Identity))
                                                    {
                                                        powershell.Commands.Clear();
                                                        respGrp.DistributionGroupName = dgName;
                                                        Command getgroupMember = new Command("Get-DistributionGroupMember");
                                                        getgroupMember.Parameters.Add("identity", Identity);
                                                        getgroupMember.Parameters.Add("ResultSize", "unlimited");
                                                        var resultgroupMembersPipeline = runspace.CreatePipeline();
                                                        resultgroupMembersPipeline.Commands.Add(getgroupMember);
                                                        var resultgroupMembers = resultgroupMembersPipeline.Invoke();
                                                        if (resultgroupMembers != null)
                                                        {
                                                            foreach (PSObject ps1 in resultgroupMembers)
                                                            {
                                                                if (ps1 != null)
                                                                {
                                                                    var member = ps1.Members["Name"]?.Value?.ToString()?.Trim();
                                                                    if (!string.IsNullOrWhiteSpace(member))
                                                                    {
                                                                        Guid userIdentity;
                                                                        if (Guid.TryParse(member, out userIdentity))
                                                                        {
                                                                            var userInfo = SkypeForBusinessUserInfo.GetByIdentity(userIdentity.ToString());
                                                                            if(userInfo != null)
                                                                            {
                                                                                mem.Add(userInfo.UserPrincipalName);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            mem.Add($"{member}@{spDomain}");
                                                                        }
                                                                    }
                                                                        
                                                                }
                                                            }
                                                        }

                                                        resultgroupMembers = null;
                                                        respGrp.AgentsByUri = string.Join($",", mem);
                                                        // {0}/{1} are EF's own positional-parameter tokens (bound as real SqlParameters),
                                                        // not string interpolation - dgName/AgentsByUri come from Exchange Online
                                                        // (distribution group display name / member list) and must never be
                                                        // concatenated into the SQL text directly.
                                                        if (mem.Count > 0)
                                                        {
                                                            SkypeForBusinessUserInfo.ExecuteQuery($"Update DI_SFB_ResponseGroupInfo Set DistributionGroupName={{0}}, AgentsByUri={{1}} where Id = {respGrp.Id}", dgName, respGrp.AgentsByUri);
                                                        }
                                                        else
                                                        {
                                                            SkypeForBusinessUserInfo.ExecuteQuery($"Update DI_SFB_ResponseGroupInfo Set DistributionGroupName={{0}} where Id = {respGrp.Id}", dgName);
                                                        }
                                                    }
                                                }
                                                Thread.Sleep(10000);
                                            }
                                            resultgroups = null;
                                            Thread.Sleep(10000);
                                        }
                                    }
                                    results = null;
                                }
                                catch (Exception ex)
                                {
                                    WriteProvisioningLog(LogLevelType.Exception, $"ExecuteOnLineLineScript Execution failed", ex);
                                    throw ex;
                                }
                                finally
                                {
                                    DisconnectExchangeOnline(powershell);
                                }
                            }
                            
                        }
                    }
                }
                
                SaveResponseGroupMember();
                WriteDiscoveryLog(LogLevelType.Exception, $"Discover ResponseGroup Member completed", DiscoveryObjectType.ResponseGroup);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "DiscoverResponseGroupMember failed", DiscoveryObjectType.ResponseGroup, ex);
                throw ex;
            }
        }
        
        public void GenerateNumberMatrix()
        {
            _idmsLogger.WriteLog(LogLevelType.Info, "Generate UnAssigned Number Matrix started");
            var numbers = new List<SkypeForBusinessUnAssignedNumberMatrix>();
            var numberRanges = new List<SkypeForBusinessUnAssignedNumber>();
            var users = new List<int?>();
            try
            {
                //Exclude the DCC number, because this number will not be present in Matrix
                users = SkypeForBusinessUserInfo.GetUserExtensionByExcludeDCC(this.Endpoint.Id);
                
                numberRanges = SkypeForBusinessUnAssignedNumber.GetAllByEndpointId(this.Endpoint.Id, currentUser);
                var exisitingMatrixData = SkypeForBusinessUnAssignedNumberMatrix.GetAllByEndpointId(this.Endpoint.Id, currentUser);
                foreach (var numberRange in numberRanges)
                {
                    for (int newnum = numberRange.StartRange; newnum <= numberRange.EndRange; newnum++)
                    {
                        var data = exisitingMatrixData.FirstOrDefault(i => i.Number == newnum);
                        var assigned = users.Exists(i => i == newnum);
                        if (data == null)
                        {
                            var number = new SkypeForBusinessUnAssignedNumberMatrix();
                            number.Priority = numberRange.Priority;
                            number.Number = newnum;
                            number.CreatedBy = currentUser;
                            number.CreatedDate = DateTime.Now;
                            number.Assigned = assigned;
                            number.EndPointSystemId = this.Endpoint.Id;
                            number.Save(currentUser);
                            exisitingMatrixData.Add(number);
                        }
                        else
                        {
                            data.LastModifiedBy = currentUser;
                            data.LastModifiedDate = DateTime.Now;
                            data.Assigned = assigned;
                            //if (assigned && data.Reserved)
                            //{
                            //    data.Reserved = false;
                            //    _idmsLogger.WriteLog(LogLevelType.Info, $"Extension Number: {data.Number} is assigned to the User.Hence Number Reserved status updated to false");
                            //}
                            data.Save(currentUser);
                        }
                    }
                }
                SkypeForBusinessUnAssignedNumberMatrix.DeleteNumbers(this.Endpoint.Id, this.Process);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "GenerateNumberMatrix failed", ex);
            }
            finally
            {
                numbers?.Clear();
                numbers = null;

                numberRanges?.Clear();
                numberRanges = null;

                users?.Clear();
                users = null;
            }
            _idmsLogger.WriteLog(LogLevelType.Info, "Generate UnAssigned Number Matrix completed");
        }

        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteAccount(IEndpointUserInfo _userInfo)
        {
            return true;
        }

        private bool DisabledSFBUser(IEndpointUserInfo _userInfo)
        {
            bool isSuccess = true;
            var userInfo = _userInfo as SkypeForBusinessUserInfo;
            var onlineExecutionscripts = new List<SkypeForBusinessScriptBlock>();
            try
            {
                var userWithGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(userInfo.Id, this.Endpoint.Id);
                if (CheckOnlineAccountExists(userInfo.AccountName))
                {
                    var userIdentity = userWithGroups.UserPrincipalName;
                    this.userSIPAddress = userWithGroups.SipAddress;
                    var removeDelegationScript = string.Empty;
                    var script = new List<SkypeForBusinessScriptBlock>();
                    if (userWithGroups.SkypeForBusinessResponseGroup.Count > 0)
                    {
                        foreach (var grp in userWithGroups.SkypeForBusinessResponseGroup)
                        {
                            if(RemoveResponseGroupMember(grp.Name, grp.DistributionGroupAddress, userWithGroups.AccountName, userWithGroups.Identity, userWithGroups.AccountNameFromUPN))
                            {
                                try
                                {
                                    var matrixADID = SkypeForBusinessProvisioningMatrix.GetByName(grp.Name, EntitlementType.ResponseGroup);
                                    if (matrixADID != null && !string.IsNullOrWhiteSpace(matrixADID.ADID))
                                    {
                                        removeDelegationScript = ON_RemoveDelegate_AfterResponseGroup.Replace("@@#GROUP_UPNADID#@@", matrixADID.ADID).Replace("@@#UPN#@@", userInfo.AccountName);
                                        try
                                        {
                                            script = new List<SkypeForBusinessScriptBlock>();
                                            script.Add(new SkypeForBusinessScriptBlock() { Script = removeDelegationScript });
                                            ExecuteOnLineLineScript(script);
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteProvisioningLog(LogLevelType.Info, $"Remove Delegation failed {ex.ToString()}. Script:{removeDelegationScript}", ex);
                                        }
                                    }
                                    else
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"Unable to Add Delegation becaue ADID for Group:{grp} not exists in matrix failed for ResponseGroup:{grp}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"Unable to Add Delegation becaue ADID for Group:{grp} not exists in matrix failed for ResponseGroup failed.{ex.ToString()}");
                                }
                            }
                        }
                    }
                    
                    if (!string.IsNullOrWhiteSpace(ON_Remove_MSTeamLineURI))
                    {
                        var disableScript = ON_Remove_MSTeamLineURI.Replace("@@#UPN#@@", userIdentity);
                        onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = disableScript });
                    }
                    if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TDP))
                    {
                        var tenantDialPlan = ON_GRANT_CS_TDP.Replace("@@#UPN#@@", userIdentity).Replace("\"@@#TENANTDIALPLAN#@@\"", "$null");
                        onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = tenantDialPlan });
                    }
                    if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_OVRP))
                    {
                        var onlineVRP = ON_GRANT_CS_OVRP.Replace("@@#UPN#@@", userIdentity).Replace("\"@@#ONLINEVOICEROUTINGPOLICY#@@\"", "$null");
                        onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = onlineVRP });
                    }
                    //if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TUP))
                    //{
                    //    var teamUpgradePolicy = ON_GRANT_CS_TUP.Replace("@@#UPN#@@", userIdentity).Replace("\"UpgradeToTeams\"", "$null");
                    //    onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = teamUpgradePolicy });
                    //}
                    isSuccess = this.ExecuteOnLineLineScript(onlineExecutionscripts);
                }
                else
                {
                    isSuccess = true;
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Account({userWithGroups.AccountName}) does not exists");
                }
                _idmsLogger.WriteLog(LogLevelType.Info, $"Disable Account({userWithGroups.AccountName}) is completed?:{isSuccess}");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                WriteProvisioningLog(LogLevelType.Exception, $"DeleteAccount failed for the User:{userInfo.AccountName}", ex);
            }
            finally
            {
                onlineExecutionscripts?.Clear();
                onlineExecutionscripts = null;
            }
            return isSuccess;
        }

        public override bool DisableAccount(IEndpointUserInfo _userInfo)
        {

            return DisabledSFBUser(_userInfo);
        }

        private bool PowerShellErrorCheck(PowerShell powershell, Collection<PSObject> results, bool throwException = true)
        {
            bool isError = false;
            if (powershell.Streams.Error.Count > 0)
            {
                isError = true;
                var strError = string.Empty;
                foreach (ErrorRecord er in powershell.Streams.Error)
                {
                    strError += "Exception:" + er.Exception?.ToString();
                    strError += "InnerException:" + er.Exception?.InnerException?.ToString();
                }
                _idmsLogger.WriteLog(LogLevelType.Error, $"PowerShellErrorCheck: Powershell Invoke Error: {strError}");
                if (throwException) throw new Exception(strError);
            }
            return isError;
        }
        public override bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            //TODO:implementation pending
            return true;
        }
        #region private method
        private void UpdateEntitlementRecon()
        {
            WriteDiscoveryLog(LogLevelType.Info, "UpdateEntitlement User Profile started");
            List<AccountReconcilation> exsitngAccountrecon = null;
            List<EntitlementReconcilation> existingEntilements = null;
            AccountReconcilation acctRecon = null;
            EntitlementReconcilation entRecon = null;
            SkypeForBusinessUserInfo userInfo = null;
            try
            {
                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id).ToList();
                existingEntilements = EntitlementReconcilation.GetAllByEndpointSystemId(this.Endpoint.Id);
                var userStatus = userStatusOnline;
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    userInfo = null;
                    try
                    {
                        userInfo = SkypeForBusinessUserInfo.GetByIdIncludeGroups(item.Key);
                        if (userInfo != null)
                        {
                            acctRecon = exsitngAccountrecon.Where(a => a.EndpointUserId == userInfo.Id).FirstOrDefault();
                            if (acctRecon == null) continue;
                            var isMarkForDeletion = !(userInfo.IsValid);
                            #region responseGroup

                            foreach (var entitlement in userInfo.SkypeForBusinessResponseGroup)
                            {
                                entRecon = existingEntilements.Where(e => e.EndpointUserId == userInfo.Id &&
                                                                            e.Entitlement == Convert.ToString(entitlement.Name).Trim() &&
                                                                            e.EntitlementType == (int)EntitlementType.ResponseGroup).FirstOrDefault();
                                if (entRecon == null)
                                {
                                    entRecon = new EntitlementReconcilation();
                                    var entitlements = (userInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.ResponseGroup, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                    entRecon.Entitlement = Convert.ToString(entitlement.Name);
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = entitlement.Description;
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                    entRecon.CreatedDate = DateTime.Now;
                                    entRecon.CreatedBy = "System";
                                    entRecon.IsDeleted = isMarkForDeletion;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.Save(currentUser);
                                    existingEntilements.Add(entRecon);
                                }
                                else
                                {
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.AccountName = acctRecon.AccountName;
                                    entRecon.AccountReconcilationId = acctRecon.Id;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.Entitlement = Convert.ToString(entitlement.Name);
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = entitlement.Description;
                                    entRecon.IsDeleted = isMarkForDeletion;
                                    entRecon.UserId = acctRecon.UserId;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    entRecon.Save(currentUser);
                                }
                            }
                            #endregion
                            #region IDD,DCC,TelephoneNumber
                            var entitlementType = userInfo.UserType;
                            if (!string.IsNullOrWhiteSpace(userInfo.LineURI) || entitlementType == EntitlementType.DCC)
                            {
                                entRecon = existingEntilements.Where(e => e.EndpointUserId == userInfo.Id 
                                                                    &&
                                                                    e.EntitlementType == (int)entitlementType).FirstOrDefault();
                                if (entRecon == null)
                                {
                                    //var entitlements = (userInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, entitlementType, string.Empty);
                                    entRecon.Entitlement = userInfo.AnyOneLineURI;
                                    entRecon.EntitlementId = userInfo.Id;
                                    entRecon.EntitlementDescription = userStatus;
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                    entRecon.IsDeleted = isMarkForDeletion;
                                    entRecon.CreatedDate = DateTime.Now;
                                    entRecon.CreatedBy = "System";
                                    entRecon.UserId = acctRecon.UserId;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.Save(currentUser);
                                    existingEntilements.Add(entRecon);
                                }
                                else
                                {
                                    entRecon.EntitlementDescription = userStatus;
                                    entRecon.IsDeleted = false;
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.Entitlement = userInfo.AnyOneLineURI;
                                    entRecon.EntitlementId = userInfo.Id;
                                    entRecon.IsDeleted = isMarkForDeletion;
                                    entRecon.UserId = acctRecon.UserId;
                                    entRecon.AccountName = acctRecon.AccountName;
                                    entRecon.AccountReconcilationId = acctRecon.Id;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    entRecon.Save(currentUser);
                                }
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while updating Entitlement reconciliation. User -{0}", userInfo.Name), ex);
                    }
                }

                try
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, "Delete Entitlement started");
                    EntitlementReconcilation.ProcessDeletedEntitlements(this.Endpoint.Id, this.Process, currentUser);
                    _idmsLogger.WriteLog(LogLevelType.Info, "Delete Entitlement completed");
                }
                catch (Exception ex)
                {
                    this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Deleting EntitlementReconcilation.", ex);
                }
                _idmsLogger.WriteLog(LogLevelType.Info, "UpdateEntitlementRecon completed");
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Process Entitlements Recon.", ex);
            }
            finally
            {
                acctRecon = null;
                entRecon = null;
                userInfo = null;
                dicUsers?.Clear();
                dicUsers = null;
                exsitngAccountrecon?.Clear();
                exsitngAccountrecon = null;

                existingEntilements?.Clear();
                existingEntilements = null;
            }
        }

        private void SaveResponseGroupMember()
        {
            WriteDiscoveryLog(LogLevelType.Info, "Save CallQueue Member(ResponseGroupMember) started",DiscoveryObjectType.ResponseGroup);
            //SkypeForBusinessUserInfo.DeleteAllUserGroups(this.Endpoint.Id);
            if (Groups.Count > 0)
            {
                foreach (var Group in Groups)
                {
                    try
                    {
                        if (Group.MemberList.Count > 0)
                        {
                            foreach (var member in Group.MemberList)
                            {
                                var user = dicUsers.FirstOrDefault(i => i.Value.Equals($"{member.Trim()}", StringComparison.InvariantCultureIgnoreCase));
                                if (user.Key > 0 && Group != null && Group.Id > 0)
                                {
                                    SkypeForBusinessUserInfo.InsertUserGroup(this.Endpoint.Id, user.Key, Group.Id,-1);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, $"CallQueue Member(ResponseGroupMember) failed for Group:{Group.Name}",DiscoveryObjectType.ResponseGroup, ex);
                        throw ex;
                    }
                }
            }
            SkypeForBusinessUserInfo.DeleteAllUserGroups(this.Endpoint.Id, this.Process);
           WriteDiscoveryLog(LogLevelType.Info, "Save CallQueue Member(ResponseGroupMember) completed", DiscoveryObjectType.ResponseGroup);
        }
       
        private bool ExecuteOnPremScript(List<SkypeForBusinessScriptBlock> executionSteps, bool checkUserExists = false, bool retrieveUserObject = false, bool throwException = true)
        {
            if (executionSteps.Count == 0)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"ExecuteOnPremScript: No Command found for execution");
                return true;
            }

            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"OnPremScript: {string.Join("<br/>", executionSteps.Select(i => i.Script))}");
            
            var isSuccess = false;
            using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
            {
                using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        powershell.Runspace = runspace;
                        PSCommand command = new PSCommand();
                        powershell.Commands.AddCommand("Import-Module").AddArgument("SkypeOnlineConnector");
                        powershell.Commands.AddCommand("Import-Module").AddArgument("LyncOnlineConnector");
                        var moduelImport = powershell.Invoke();
                        if (!PowerShellErrorCheck(powershell, moduelImport))
                        {
                            powershell.Commands.Clear();
                            command.AddCommand("New-PSSession");
                            command.AddParameter("ConnectionUri", new Uri(connectionUri));
                            command.AddParameter("Credential", this.credential);
                            command.AddParameter("Authentication", "Default");
                            powershell.Commands = command;
                            powershell.Runspace = runspace;
                            Collection<System.Management.Automation.PSObject> result = powershell.Invoke();
                            if (!PowerShellErrorCheck(powershell, result))
                            {
                                powershell.Commands.Clear();
                                Command setVar = new Command("Set-Variable");
                                setVar.Parameters.Add("Name", "SFBSession");
                                setVar.Parameters.Add("Value", result[0]);

                                powershell.Commands.AddCommand(setVar);
                                powershell.Runspace = runspace;
                                powershell.Invoke();
                                powershell.Commands.Clear();

                                PSCommand importSession = new PSCommand();
                                importSession.AddScript("Import-PSSession $(Get-PSSession)[0] -allowclobber");
                                powershell.Commands = importSession;
                                powershell.Runspace = runspace;
                                powershell.Invoke();

                                foreach (var executionStep in executionSteps)
                                {
                                    powershell.Commands.Clear();
                                    var script = executionStep.Script;
                                    if (executionStep.IncludeCredential)
                                    {
                                        Command setCredVar = new Command("Set-Variable");
                                        setCredVar.Parameters.Add("Name", "Cred");
                                        setCredVar.Parameters.Add("Value", credential);

                                        powershell.Commands.AddCommand(setCredVar);
                                        powershell.Runspace = runspace;
                                        powershell.Invoke();
                                        powershell.Commands.Clear();
                                        script += "  -Credential (Get-Variable -Name Cred).Value";
                                    }
                                    WriteProvisioningLog(LogLevelType.Info, $"OnPremScript Execution:{script}");
                                    powershell.Runspace = runspace;
                                    powershell.Commands.AddScript(script);
                                    var executionResult = powershell.Invoke();
                                    if (!PowerShellErrorCheck(powershell, executionResult, throwException))
                                    {
                                        if (checkUserExists)
                                        {
                                            isSuccess = executionResult.Count == 1;
                                            WriteProvisioningLog(LogLevelType.Info, $"User Exists:{isSuccess}");
                                        }
                                        isSuccess = true;
                                        if ((retrieveUserObject || executionStep.IncludeObject) && executionResult.Count > 0)
                                        {
                                            WriteProvisioningLog(LogLevelType.Info, $"checkUserExists && Get UserAccount");
                                            properties = new SkypeForBusinessProperties();
                                            this.EndpointUserAccount = SkypeForBusinessOrchestrator.PopulateOnPremUser(executionResult[0], this.Endpoint.Id, properties);
                                        }
                                        WriteProvisioningLog(LogLevelType.Info, $"OnPremScript Execution({script}) success");
                                    }
                                    else
                                    {
                                        isSuccess = false;
                                        WriteProvisioningLog(LogLevelType.Info, $"OnPremScript Execution({script}) failed");
                                    }
                                    Thread.Sleep(PowershellScriptExecutionDelay);
                                }
                            }
                        }
                    }
                }
            }
            return isSuccess;
        }
        public bool ExecuteOnLineLineScript(List<SkypeForBusinessScriptBlock> executionSteps, bool requireAccountObject = false)
        {
            if (executionSteps.Count == 0)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"ExecuteOnLineLineScript: No Command found for execution");
                return true;
            }
            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"ExecuteOnLineLineScript: {string.Join("<br/>", executionSteps.Select(i => i.Script))}");

            bool isSuccess = false;
            using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
            {
                using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            foreach (var module in psModules)
                            {
                                powershell.AddScript($"Import-Module \"{module}\"");
                            }
                            var moduelImport = powershell.Invoke();
                            if (!PowerShellErrorCheck(powershell, moduelImport))
                            {
                                powershell.Commands.Clear();
                                var connect = new Command("Connect-MicrosoftTeams");
                                connect.Parameters.Add("Credential", onLineCredential);
                                powershell.Commands.AddCommand(connect);
                                moduelImport = powershell.Invoke();
                                if (!PowerShellErrorCheck(powershell, moduelImport))
                                {
                                    powershell.Commands.Clear();
                                    WriteProvisioningLog(LogLevelType.Info, $"Connect-MicrosoftTeams successful");
                                    foreach (var executionStep in executionSteps)
                                    {
                                        powershell.Commands.Clear();
                                        var script = executionStep.Script;
                                        WriteProvisioningLog(LogLevelType.Info, $"OnLine Execution Script:{script}");
                                        powershell.Runspace = runspace;
                                        powershell.Commands.AddScript(script);
                                        var executionResult = powershell.Invoke();
                                        if (!PowerShellErrorCheck(powershell, executionResult))
                                        {
                                            isSuccess = true;
                                            if ((requireAccountObject || executionStep.IncludeObject) && executionResult.Count > 0)
                                            {
                                                WriteProvisioningLog(LogLevelType.Info, $"Populate Online User details started");
                                                properties = new SkypeForBusinessProperties();
                                                try
                                                {
                                                    this.EndpointOnlineUserAccount = SkypeForBusinessOrchestrator.PopulateUser(executionResult[0], this.Endpoint.Id, properties, this.DCCVoiceRoutingPolicyName);
                                                    var usrTemp = this.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                                                    WriteProvisioningLog(LogLevelType.Warning, $"Account Online Status After Account Creation: UserPrincipalName - {usrTemp?.AccountName} LineURI - {usrTemp?.LineURI} EnterpriseVoiceEnabled - {usrTemp?.EnterpriseVoiceEnabled} TenantDialPlan - {usrTemp?.TenantDialPlan} OnlineVoiceRoutingPolicy - {usrTemp?.OnlineVoiceRoutingPolicy} ExternalAccessPolicy - {usrTemp?.ExternalAccessPolicy}");
                                                }
                                                catch (Exception ex)
                                                {
                                                    WriteProvisioningLog(LogLevelType.Warning, $"Populate Online User:({ex})");
                                                }
                                            }
                                            WriteProvisioningLog(LogLevelType.Info, $"OnLine Script Execution({script}) success");
                                        }
                                        else
                                        {
                                            isSuccess = false;
                                            WriteProvisioningLog(LogLevelType.Info, $"OnLine Script Execution({script}) failed");
                                        }
                                        Thread.Sleep(PowershellScriptExecutionDelay);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, $"ExecuteOnLineLineScript Execution failed", ex);
                            throw ex;
                        }
                        finally
                        {
                            try
                            {
                                powershell.Commands.Clear();
                                var disConnect = new Command("Disconnect-MicrosoftTeams");
                                powershell.Commands.AddCommand(disConnect);
                                var moduelImport = powershell.Invoke();
                            }
                            catch (Exception disConnectEx)
                            {
                                WriteProvisioningLog(LogLevelType.Info, $"Disconnect-MicrosoftTeams failed", disConnectEx);
                            }
                        }
                    }
                }
            }
            return isSuccess;
        }
        public override bool IsUserPartOfAnyGroup(IEndpointUserInfo userInfo)
        {
            var grps = SkypeForBusinessResponseGroupInfo.GetDistributionGroup(this.Endpoint.Id);
            bool isSuccess = false;
            SkypeForBusinessUserInfo sfbUserInfo = (SkypeForBusinessUserInfo)userInfo;
           
            using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
            {
                using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                {
                    runspace.Open();
                    using (PowerShell powershell = PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = runspace;
                            foreach (var module in psMSTeamO365Modules)
                            {
                                powershell.AddScript($"Import-Module \"{module}\"");
                            }
                            var moduelImport = powershell.Invoke();
                            if (!PowerShellErrorCheck(powershell, moduelImport))
                            {
                                powershell.Commands.Clear();
                                var connect = new Command("Connect-ExchangeOnline");
                                connect.Parameters.Add("Credential", credential);
                                //connect.Parameters.Add("CommandName", new[] { "Get-DistributionGroup", "Get-DistributionGroupMember" });
                                powershell.Commands.AddCommand(connect);
                                moduelImport = powershell.Invoke();
                                if (!PowerShellErrorCheck(powershell, moduelImport))
                                {
                                    powershell.Commands.Clear();
                                    foreach (var distributionGroupAddress in grps)
                                    {
                                        if (!string.IsNullOrWhiteSpace(distributionGroupAddress))
                                        {
                                            Command getDistGroup = new Command("Get-DistributionGroup");
                                            getDistGroup.Parameters.Add("identity", distributionGroupAddress);
                                            getDistGroup.Parameters.Add("ResultSize", "unlimited");
                                            var pipeline2 = runspace.CreatePipeline();
                                            pipeline2.Commands.Add(getDistGroup);
                                            var resultgroups = pipeline2.Invoke();

                                            foreach (PSObject psObject in resultgroups)
                                            {
                                                if (psObject != null)
                                                {
                                                    powershell.Commands.Clear();
                                                    var Identity = SkypeForBusinessHelper.GetProperty(psObject, "Identity");
                                                    var DisplayName = SkypeForBusinessHelper.GetProperty(psObject, "DisplayName");
                                                    if (!string.IsNullOrWhiteSpace(Identity))
                                                    {
                                                        powershell.Commands.Clear();
                                                        Command getgroupMember = new Command("Get-DistributionGroupMember");
                                                        getgroupMember.Parameters.Add("identity", Identity);
                                                        getgroupMember.Parameters.Add("ResultSize", "unlimited");
                                                        var resultgroupMembersPipeline = runspace.CreatePipeline();
                                                        resultgroupMembersPipeline.Commands.Add(getgroupMember);
                                                        var resultgroupMembers = resultgroupMembersPipeline.Invoke();
                                                        if (resultgroupMembers != null)
                                                        {
                                                            foreach (PSObject ps1 in resultgroupMembers)
                                                            {
                                                                if (ps1 != null)
                                                                {
                                                                    var member = ps1.Members["Name"]?.Value?.ToString()?.Trim();
                                                                    if (!string.IsNullOrWhiteSpace(member) && sfbUserInfo.AccountNameFromUPN.Equals(member, StringComparison.InvariantCultureIgnoreCase)
                                                                        ||
                                                                        !string.IsNullOrWhiteSpace(member) && sfbUserInfo.Identity.Equals(member, StringComparison.InvariantCultureIgnoreCase)
                                                                        ||
                                                                        !string.IsNullOrWhiteSpace(member) && sfbUserInfo.AccountName.Equals(member, StringComparison.InvariantCultureIgnoreCase)
                                                                        )
                                                                    {
                                                                        _idmsLogger.WriteLog(LogLevelType.Info, $"User:{sfbUserInfo.UserPrincipalName} has ResponseGroup(DistributionGroup):{DisplayName}");
                                                                        return true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteProvisioningLog(LogLevelType.Exception, $"ExecuteOnLineLineScript Execution failed", ex);
                            throw ex;
                        }
                        finally
                        {
                            grps = null;
                            try
                            {
                                DisconnectExchangeOnline(powershell);
                            }
                            catch (Exception disConnectEx)
                            {
                                WriteProvisioningLog(LogLevelType.Info, $"Disconnect-Exchange failed", disConnectEx);
                            }
                        }
                    }
                }
            }
            
            return isSuccess;
        }
        private Collection<PSObject> GetCallQueue()
        {
            Collection<PSObject> psObjects = new Collection<PSObject>();
            try
            {
                using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
                {
                    using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                    {
                        runspace.Open();
                        using (PowerShell powershell = PowerShell.Create())
                        {
                            powershell.Runspace = runspace;
                            foreach (var module in psModules)
                            {
                                powershell.AddScript($"Import-Module \"{module}\"");
                            }
                            var impModule = powershell.Invoke();
                            if (!PowerShellErrorCheck(powershell, impModule))
                            {
                                try
                                {
                                    powershell.Commands.Clear();
                                    var connect = new Command("Connect-MicrosoftTeams");
                                    connect.Parameters.Add("Credential", onLineCredential);
                                    powershell.Commands.AddCommand(connect);
                                    var moduelImport = powershell.Invoke();
                                    if (!PowerShellErrorCheck(powershell, moduelImport))
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"Connect-MicrosoftTeams successful");
                                        powershell.Commands.Clear();
                                        var script = ON_CsCallQueueScript;
                                        WriteDiscoveryLog(LogLevelType.Info, $"Execution Script:{script}");
                                        powershell.Commands.Clear();
                                        powershell.Runspace = runspace;
                                        powershell.Commands.AddScript(script);
                                        var executionResult = powershell.Invoke();
                                        if (!PowerShellErrorCheck(powershell, executionResult))
                                        {
                                            psObjects = executionResult;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteProvisioningLog(LogLevelType.Exception, $"ExecuteOnLineLineScript Execution failed", ex);
                                    throw ex;
                                }
                                finally
                                {
                                    try
                                    {
                                        powershell.Commands.Clear();
                                        var disConnect = new Command("Disconnect-MicrosoftTeams");
                                        powershell.Commands.AddCommand(disConnect);
                                        var moduelImport = powershell.Invoke();
                                    }
                                    catch (Exception disConnectEx)
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"Disconnect-MicrosoftTeams failed", disConnectEx);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Info, $"GetCallQueue failed,{ex.Message}", ex);
                throw ex;
            }
            return psObjects;
        }
        public Collection<PSObject> GetSkypeForBusinessOnlineUser()
        {
            string scriptBlock = ON_GET_CSUSER;
            Collection<PSObject> psObjects = new Collection<PSObject>();
            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"GetSkypeForBusinessObjectByType:Started discovering Online User started");
                using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
                {
                    using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                    {
                        runspace.Open();
                        using (PowerShell powershell = PowerShell.Create())
                        {
                            powershell.Runspace = runspace;
                            foreach (var module in psModules)
                            {
                                powershell.AddScript($"Import-Module \"{module}\"");
                            }
                            var impModule = powershell.Invoke();
                            if (!PowerShellErrorCheck(powershell, impModule))
                            {
                                powershell.Commands.Clear();
                                try
                                {
                                    var connect = new Command("Connect-MicrosoftTeams");
                                    connect.Parameters.Add("Credential", onLineCredential);
                                    powershell.Commands.AddCommand(connect);
                                    var mdlResult = powershell.Invoke();
                                    if (!PowerShellErrorCheck(powershell, mdlResult))
                                    {
                                        _idmsLogger.WriteLog(LogLevelType.Info, $"Execution Script:{scriptBlock}");
                                        powershell.Commands.Clear();
                                        powershell.Runspace = runspace;
                                        powershell.Commands.AddScript(scriptBlock);
                                        var executionResult = powershell.Invoke();
                                        if (!PowerShellErrorCheck(powershell, mdlResult))
                                        {
                                            psObjects = executionResult;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"Connect to OnLine failed", ex);
                                    throw ex;
                                }
                                finally
                                {
                                    try
                                    {
                                        powershell.Commands.Clear();
                                        var disConnect = new Command("Disconnect-MicrosoftTeams");
                                        powershell.Commands.AddCommand(disConnect);
                                        var moduelImport = powershell.Invoke();
                                    }
                                    catch (Exception disConnectEx)
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"Disconnect-MicrosoftTeams failed", disConnectEx);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Failed to Connect to OnLine", ex);
                throw ex;
            }
            return psObjects;
        }
        private void DiscoverResponseGroup()
        {
            Collection<PSObject> csCallQueue = new Collection<PSObject>();
            IList<SkypeForBusinessResponseGroupInfo> groups = new List<SkypeForBusinessResponseGroupInfo>();
            try
            {
                this.WriteDiscoveryLog(LogLevelType.Info, "Discovering CallQueue(ResponseGroup) started", DiscoveryObjectType.ResponseGroup);
                //resGroup = GetSkypeForBusinessObjectByType(responseGroup, getResponseGroupScript);
                //Get Call Queue
                int retry = 1;
                while (retry <= 2 && csCallQueue != null && csCallQueue.Count == 0)
                {
                    csCallQueue = GetCallQueue();
                    Thread.Sleep(60000);
                    retry++;
                    if (retry > 2 && csCallQueue.Count == 0)
                    {
                        throw new Exception("Unbale to retrieve CallQueue(ResponseGroup)");
                    }
                    WriteDiscoveryLog(LogLevelType.Info, $"Discovering CallQueue(ResponseGroup) retry:{retry}.", DiscoveryObjectType.ResponseGroup);
                }
                if (csCallQueue.Count > 0)
                {
                    this.WriteDiscoveryLog(LogLevelType.Info, "Save CallQueue(ResponseGroup) Started", DiscoveryObjectType.ResponseGroup);
                    SaveResponseGroup(csCallQueue);
                    this.WriteDiscoveryLog(LogLevelType.Info, "Save CallQueue(ResponseGroup) completed", DiscoveryObjectType.ResponseGroup);

                    this.WriteDiscoveryLog(LogLevelType.Info, "Delete  CallQueue(ResponseGroup) started", DiscoveryObjectType.ResponseGroup);

                    groups = SkypeForBusinessResponseGroupInfo.GetDeletedGroups(this.Endpoint.Id, this.Process);
                    SkypeForBusinessResponseGroupInfo.RemoveSkypeForBusinessResponseGroups(groups.ToList(), this.Endpoint.Id);
                    this.WriteDiscoveryLog(LogLevelType.Info, "Discovering CallQueue(ResponseGroup) completed", DiscoveryObjectType.ResponseGroup);
                }
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery CallQueue(ResponseGroup).", DiscoveryObjectType.ResponseGroup, ex);
                throw ex;
            }
            finally
            {
                csCallQueue?.Clear();
                csCallQueue = null;

                groups?.Clear();
                groups = null;
            }
        }

        private void SaveResponseGroup(Collection<PSObject> resGroup)
        {
            if (resGroup != null && resGroup.Count > 0)
            {
                Groups.Clear();
                this._idmsLogger.WriteLog(LogLevelType.Info, "Construct ResponseGroup started");
                var newGroups = new List<SkypeForBusinessResponseGroupInfo>();
                var exisitingGroups = SkypeForBusinessResponseGroupInfo.GetAllByEndpointId(this.Endpoint.Id);
                foreach (PSObject grpObject in resGroup)
                {
                    SkypeForBusinessResponseGroupInfo sfbGroup = null;
                    try
                    {
                        if (grpObject != null)
                        {
                            sfbGroup = SkypeForBusinessOrchestrator.PopulateResponseGroup(grpObject, this.Endpoint.Id, properties);
                            if (sfbGroup != null)
                            {
                                var exisitingRecord = exisitingGroups.Where(i=> !string.IsNullOrWhiteSpace(i.Identity) && i.Identity.Equals(sfbGroup.Identity,StringComparison.InvariantCultureIgnoreCase));
                                if (exisitingRecord == null)
                                {
                                    newGroups.Add(sfbGroup);
                                }
                                var group = SkypeForBusinessResponseGroupInfo.SaveGroup(sfbGroup, this.Endpoint.Id, currentUser);
                                Groups.Add(group);
                            }
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Info, "CallQueue(ResponseGroup) null");
                        }
                    }
                    catch (Exception ex)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Exception while constructing/Saving CallQueue(ResponseGroup).{0} ", sfbGroup != null ? sfbGroup.Identity : "Group not found"), ex);
                    }
                }
                if (newGroups.Count > 0)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, "New CallQueue(ResponseGroup) notification started");
                    foreach (var newGroup in newGroups)
                    {
                        var matrix = SkypeForBusinessProvisioningMatrix.GetByName(newGroup.Name, EntitlementType.ResponseGroup);
                        if (matrix == null)
                        {
                            matrix = new SkypeForBusinessProvisioningMatrix();
                            matrix.Name = newGroup.Name;
                            matrix.Description = newGroup.Description;
                            matrix.EndPointSystemId = this.Endpoint.Id;
                            matrix.RequestType = EntitlementType.ResponseGroup;
                            matrix.CreatedBy = currentUser;
                            matrix.CreatedDate = DateTime.Now;
                            matrix.IsActive = true;
                            matrix.ValidFrom = DateTime.Now;
                            matrix.ValidTo = DateTime.MaxValue;
                            matrix.IsValid = true;
                            matrix.Save(currentUser);
                        }
                    }
                    SendEmail(string.Join(",", newGroups.Select(i => i.Name)));
                    _idmsLogger.WriteLog(LogLevelType.Info, "New CallQueue(ResponseGroup) notification completed");
                }
                _idmsLogger.WriteLog(LogLevelType.Info, "Construct CallQueue(ResponseGroup) completed");
                exisitingGroups = null;
                exisitingGroups = null;
            }
        }

        private void DiscoverUser()
        {
            var sfbUsers = new List<SkypeForBusinessUserInfo>();
            
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, $"Started discovering users.", DiscoveryObjectType.User);
                //Fetch Online User 
                int retry = 1;
                while (retry <= 2 && Onlineusers != null && Onlineusers.Count == 0)
                {
                    Onlineusers = GetSkypeForBusinessOnlineUser();
                    Thread.Sleep(60000);
                    retry++;
                    if (retry > 2 && Onlineusers.Count == 0)
                    {
                        throw new Exception("Unbale to retrieve Online User account.");
                    }
                    WriteDiscoveryLog(LogLevelType.Info, $"Discovering users retry:{retry}.", DiscoveryObjectType.User);
                }
                WriteDiscoveryLog(LogLevelType.Info, $"Total No.Of Onlineusers users:{Onlineusers?.Count}", DiscoveryObjectType.User);
                //Fetch OnPremise User
                //OnPremiseusers = GetSkypeForBusinessObjectByType();
                //WriteDiscoveryLog(LogLevelType.Info, $"Total No.Of OnPremise users:{OnPremiseusers?.Count}", DiscoveryObjectType.User);
                //Process User Data
                WriteDiscoveryLog(LogLevelType.Info, $"Processing Online User Data started", DiscoveryObjectType.User);
                SaveSkypeUser();
                WriteDiscoveryLog(LogLevelType.Info, "User Discovery Completed.", DiscoveryObjectType.User);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discover Users.", DiscoveryObjectType.User, ex);
                throw ex;
            }
            finally
            {
                Onlineusers?.Clear();
                Onlineusers = null;

                //OnPremiseusers?.Clear();
                //OnPremiseusers = null;

                sfbUsers?.Clear();
                sfbUsers = null;
            }
        }

        private void SaveSkypeUser()
        {
            IList<SkypeForBusinessUserInfo> userlist = null;
            try
            {
                if (sfbUsers == null) return;

                _idmsLogger.WriteLog(LogLevelType.Info, $"Construct Online User");
                if (Onlineusers != null && Onlineusers.Count > 0)
                {

                    foreach (PSObject usrObject in Onlineusers)
                    {
                        if (usrObject != null)
                        {
                            var sfbUser = SkypeForBusinessOrchestrator.PopulateUser(usrObject, this.Endpoint.Id, properties, this.DCCVoiceRoutingPolicyName);
                            if(sfbUser != null)
                            {
                                sfbUsers.Add(sfbUser);
                            }
                        }
                    }
                }

                //WriteDiscoveryLog(LogLevelType.Info, $"Update Online User Details with OnPrem Details for DCC/DCCS user", DiscoveryObjectType.User, null);
                //foreach (var onPremUser in onPremUsers)
                //{
                //    var exisitingUsr = sfbUsers.FirstOrDefault(i =>
                //    (   !string.IsNullOrWhiteSpace(i.UserPrincipalName) && !string.IsNullOrWhiteSpace(onPremUser.UserPrincipalName) && i.UserPrincipalName.Equals(onPremUser.UserPrincipalName, StringComparison.InvariantCultureIgnoreCase))
                //    || (!string.IsNullOrWhiteSpace(i.SipAddress) && !string.IsNullOrWhiteSpace(onPremUser.SipAddress) && i.SipAddress.Equals(onPremUser.SipAddress, StringComparison.InvariantCultureIgnoreCase))
                //    );
                //    if (exisitingUsr != null)
                //    {
                //        exisitingUsr.IsOnPremAccountExists = true;
                //        //Check for DCC, DCCS
                //        if (onPremUser != null && !string.IsNullOrWhiteSpace(onPremUser.HostingProvider) && onPremUser.HostingProvider.Equals(defaulthostProviderString, StringComparison.InvariantCultureIgnoreCase))
                //        {
                //            exisitingUsr.VoicePolicy = onPremUser.VoicePolicy;
                //            exisitingUsr.VoiceRoutingPolicy = onPremUser.VoiceRoutingPolicy;
                //            exisitingUsr.DialPlan = onPremUser.DialPlan;
                //            exisitingUsr.RegistrarPool = onPremUser.RegistrarPool;
                //            exisitingUsr.HostingProvider = onPremUser.HostingProvider;
                //        }
                //    }
                //}

                //WriteDiscoveryLog(LogLevelType.Info, $"Total No.of OnPrem User Count:{onPremUsers.Count()}", DiscoveryObjectType.User, null);
                WriteDiscoveryLog(LogLevelType.Info, $"Total No.of Online User Count:{sfbUsers.Count()}", DiscoveryObjectType.User, null);
                //sfbUsers = sfbUsers.Where(i => i.IsOnPremAccountExists).ToList();
                //WriteDiscoveryLog(LogLevelType.Info, $"Matching With OnPrem User Count:{sfbUsers.Count()}", DiscoveryObjectType.User, null);

                if (sfbUsers.Count > 0)
                {
                    SaveUser();

                    userlist = SkypeForBusinessUserInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);
                    if (userlist != null && userlist.Count > 0)
                    {
                        userlist = userlist.Where(i => i.WaitingAccountSync == null).ToList();
                        SkypeForBusinessUserInfo.RemoveSkypeForBusinessUser(userlist.ToList());
                    }

                    AccountReconcilation.ProcessDeletedAccounts(this.Endpoint.Id, this.Process, currentUser);
                    dicUsers = SkypeForBusinessUserInfo.GetUserSipAddressByEndPointSystemId(this.Endpoint.Id);
                }
                else
                {
                    throw new Exception("No online account found");
                }
                _idmsLogger.WriteLog(LogLevelType.Info, $"Process Online and OnPremise User Data update completed");
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception,$"{ex.Message}",ex);
                throw ex;
            }
            finally
            {
                userlist?.Clear();
                userlist = null;

                sfbUsers?.Clear();
                sfbUsers = null;

                //onPremUsers?.Clear();
                //onPremUsers = null;
            }
            
        }

        private void SaveUser()
        {
            List<AccountReconcilation> exsitngAccountrecon = null;
            List<SkypeForBusinessUserInfo> allExistingUsers = null;
            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"Total Number of User Discovered : {sfbUsers?.Count}");
                var endpointId = this.Endpoint.Id;
                allExistingUsers = SkypeForBusinessUserInfo.GetAllByEndpointId(endpointId);
                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(endpointId).ToList();

                AccountReconcilation acctRecon = null;
                SkypeForBusinessUserInfo sfbExistingUser = null;

                //_idmsLogger.WriteLog(LogLevelType.Info, $"Total Number of Valid User(With LineUrI) : {sfbUsers.Where(i => !string.IsNullOrWhiteSpace(i.LineURI) || !string.IsNullOrWhiteSpace(i.OnPremLineURI)).Count()}");
                
                //As per requirement, if user dont have lineuri then consider as inactive account
                foreach (SkypeForBusinessUserInfo userInfo in sfbUsers)
                {
                    var entitlementType = EntitlementType.None;
                    try
                    {
                        if (!string.IsNullOrEmpty(userInfo.OnlineVoiceRoutingPolicy) && userInfo.OnlineVoiceRoutingPolicy.Trim().ToUpper() == DCCVoiceRoutingPolicyName.Trim().ToUpper())
                        {
                            entitlementType = EntitlementType.DCC;
                        }
                        else if (string.IsNullOrWhiteSpace(userInfo.AnyOneLineURI))
                        {
                            if (!string.IsNullOrWhiteSpace(userInfo.OnlineVoiceRoutingPolicy) && userInfo.OnlineVoiceRoutingPolicy.ToUpper() == DCCVoiceRoutingPolicyName.ToUpper())
                            {
                                entitlementType = EntitlementType.DCC;
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"No LineURI for user:{userInfo.UserPrincipalName} OnlineVoiceRoutingPolicy:{userInfo.OnlineVoiceRoutingPolicy} DCCVoiceRoutingPolicyName:{DCCVoiceRoutingPolicyName} ");
                                //var sfb = allExistingUsers.FirstOrDefault(i => i.UserPrincipalName.Equals(userInfo.UserPrincipalName, StringComparison.InvariantCultureIgnoreCase));
                                //if (sfb != null && sfb.AccessRequestId.HasValue == false)
                                //{
                                //    continue;
                                //}
                            }
                        }

                        
                        else if (!string.IsNullOrEmpty(userInfo.OnlineVoiceRoutingPolicy) && (userInfo.OnlineVoiceRoutingPolicy.ToUpper() == IDDOnlineVoiceRoutingWithResponseGroupPolicy.ToUpper()
                            || userInfo.OnlineVoiceRoutingPolicy.ToUpper() == IDDOnlineVoiceRoutingPolicy.ToUpper()
                            ))
                        {
                            entitlementType = EntitlementType.IDD;
                        }
                        else if (!string.IsNullOrEmpty(userInfo.OnlineVoiceRoutingPolicy) && userInfo.OnlineVoiceRoutingPolicy.ToUpper() == DefaultOnlineVoiceRoutingPolicy.ToUpper())
                        {
                            entitlementType = EntitlementType.Default;
                        }
                            
                        if (userInfo.IsValid || entitlementType == EntitlementType.DCC)
                        {
                            sfbExistingUser = allExistingUsers.Where(item => item.UserPrincipalName.Equals(userInfo.UserPrincipalName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                            if (sfbExistingUser == null)
                            {
                                userInfo.UserType = entitlementType;
                                userInfo.EndPointSystemId = endpointId;
                                userInfo.CreatedBy = currentUser;
                                userInfo.CreatedDate = DateTime.Now;
                                userInfo.Save(currentUser);
                                allExistingUsers.Add(userInfo);
                                this._idmsLogger.WriteLog(LogLevelType.Info, $"New User :{userInfo.UserPrincipalName} LineUri:{userInfo.LineURI}");
                            }
                            else
                            {
                                userInfo.UserType = entitlementType;
                                userInfo.Id = sfbExistingUser.Id;
                                userInfo.EndPointSystemId = endpointId;
                                userInfo.CreatedDate = sfbExistingUser.CreatedDate;
                                userInfo.CreatedBy = sfbExistingUser.CreatedBy;
                                userInfo.LastModifiedDate = DateTime.Now;
                                userInfo.LastModifiedBy = currentUser;
                                userInfo.WaitingAccountSync = sfbExistingUser.WaitingAccountSync;
                                userInfo.AccessRequestId = sfbExistingUser.AccessRequestId;
                                userInfo.Save(currentUser);
                                if(
                                    (!string.IsNullOrWhiteSpace(sfbExistingUser.LineURI) && !string.IsNullOrWhiteSpace(userInfo.LineURI) &&
                                    sfbExistingUser.LineURI.ToUpper() != userInfo.LineURI.ToUpper())
                                )
                                {
                                    this._idmsLogger.WriteLog(LogLevelType.Info, $"User:{userInfo.DisplayName} UserPrincipalName:{userInfo.UserPrincipalName} Old LineUri:{sfbExistingUser.LineURI} New:{userInfo.LineURI}");
                                }
                            }

                            acctRecon = exsitngAccountrecon.Where(a => a.EndpointSystemId == userInfo.EndPointSystemId && 
                            (!string.IsNullOrWhiteSpace(a.AccountName) && a.AccountName.Equals(userInfo.SamAccountName,StringComparison.InvariantCultureIgnoreCase))
                            ).FirstOrDefault();

                            bool isExist = false;
                            if (acctRecon == null)
                            {
                                acctRecon = new AccountReconcilation();
                                AccountReconcilation.UpdateException(acctRecon, true);
                                acctRecon.AccountType = AccountType.User;
                                acctRecon.EndpointSystemId = userInfo.EndPointSystemId;
                                acctRecon.EndpointUserId = userInfo.Id;
                                acctRecon.IsDeleted = false;
                                acctRecon.AccountCreatedDate = userInfo.CreatedDate;
                                acctRecon.CreatedDate = DateTime.Now;
                                acctRecon.CreatedBy = currentUser;
                            }
                            else
                            {
                                acctRecon.EndpointUserId = userInfo.Id;
                                acctRecon.AccountType = AccountType.User;
                                acctRecon.IsDeleted = false;
                                acctRecon.DeletedDate = null;
                                acctRecon.IsLocked = false;
                                acctRecon.LastModifiedDate = DateTime.Now;
                                isExist = true;
                            }
                            acctRecon.FullName = userInfo.DisplayName;
                            acctRecon.AccountName = userInfo.SamAccountName;
                            acctRecon.Save(currentUser);

                            if (!isExist)
                            {
                                exsitngAccountrecon.Add(acctRecon);
                            }
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, $" No LineURIExtension for user {userInfo.UserPrincipalName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.isDiscoveryError = true;
                        this._idmsLogger.WriteLog(LogLevelType.Exception, "Error while SaveUsers.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, "Error while  SaveUsers.", ex);
                throw new Exception("SaveUser failed.", ex);
            }
            finally
            {
                exsitngAccountrecon?.Clear();
                exsitngAccountrecon = null;

                allExistingUsers?.Clear();
                allExistingUsers = null;

                sfbUsers?.Clear();
                sfbUsers = null;
            }
        }

        #endregion
        public override bool ExecuteTransferAction(IEndpointUserInfo _userInfo)
        {
            bool isSuccess = true;
            var onLineExecutionscripts = new List<SkypeForBusinessScriptBlock>();

            try
            {
                var userInfo = (SkypeForBusinessUserInfo)_userInfo;
                var userWithGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(userInfo.Id, this.Endpoint.Id);
                SetProvisionongFields();
                if (userWithGroups != null)
                {
                    var userIdentity = userWithGroups.UserPrincipalName;
                    this.userSIPAddress = userWithGroups.SipAddress;

                    _idmsLogger.WriteLog(LogLevelType.Info, $"Execute Transfer Action started");

                    if (userWithGroups.SkypeForBusinessResponseGroup.Count > 0)
                    {
                        var removeDelegationScript = string.Empty;
                        var script = new List<SkypeForBusinessScriptBlock>();
                        foreach (var grp in userWithGroups.SkypeForBusinessResponseGroup)
                        {
                            if(RemoveResponseGroupMember(grp.Name, grp.DistributionGroupAddress, userInfo.AccountName, userWithGroups.Identity, userInfo.AccountNameFromUPN))
                            {
                                try
                                {
                                    var matrixADID = SkypeForBusinessProvisioningMatrix.GetByName(grp.Name, EntitlementType.ResponseGroup);
                                    if (matrixADID != null && !string.IsNullOrWhiteSpace(matrixADID.ADID))
                                    {
                                        removeDelegationScript = ON_RemoveDelegate_AfterResponseGroup.Replace("@@#GROUP_UPNADID#@@", matrixADID.ADID).Replace("@@#UPN#@@", userInfo.AccountName);
                                        try
                                        {
                                            script = new List<SkypeForBusinessScriptBlock>();
                                            script.Add(new SkypeForBusinessScriptBlock() { Script = removeDelegationScript });
                                            ExecuteOnLineLineScript(script);
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteProvisioningLog(LogLevelType.Info, $"Remove Delegation failed {ex.ToString()}. Script:{removeDelegationScript}", ex);
                                        }
                                    }
                                    else
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"Unable to Add Delegation becaue ADID for Group:{grp} not exists in matrix failed for ResponseGroup:{grp}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"Unable to Add Delegation becaue ADID for Group:{grp} not exists in matrix failed for ResponseGroup failed.{ex.ToString()}");
                                }
                            }
                        }
                    }

                    userInfo.TenantDialPlan = DefaultTeamDialPlanPolicy;
                    userInfo.OnlineVoiceRoutingPolicy = DefaultOnlineVoiceRoutingPolicy;
                    //userInfo.TeamsUpgradePolicy = TeamsUpgradePolicy;

                    userIdentity = userInfo.UserPrincipalName;

                    if (userInfo.IsRequireLineUriAssignment && !string.IsNullOrWhiteSpace(ON_SetCsUser) && !string.IsNullOrWhiteSpace(userInfo.AnyOneLineURI))
                    {
                        var setCsUsercmd = ON_SetCsUser.Replace("@@#UPN#@@", userIdentity).Replace("@@#LINEURI#@@", userInfo.LineURI);
                        onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = setCsUsercmd });
                        if (!string.IsNullOrWhiteSpace(ON_SetCsUserEnterpriseVoiceEnabled))
                        {
                            var setOnlineEnterPriseVoiceEnabled = ON_SetCsUserEnterpriseVoiceEnabled.Replace("@@#UPN#@@", userIdentity);
                            onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = setOnlineEnterPriseVoiceEnabled });
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TDP))
                    {
                        var tenantDialPlan = ON_GRANT_CS_TDP.Replace("@@#UPN#@@", userIdentity).Replace("@@#TENANTDIALPLAN#@@", userInfo.TenantDialPlan);
                        onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = tenantDialPlan });
                    }
                    if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_OVRP))
                    {
                        var orp = ON_GRANT_CS_OVRP.Replace("@@#UPN#@@", userIdentity).Replace("@@#ONLINEVOICEROUTINGPOLICY#@@", userInfo.OnlineVoiceRoutingPolicy);
                        onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = orp });
                    }
                    if (!string.IsNullOrWhiteSpace(ON_GETCS_SPECIFIC_USER))
                    {
                        onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = ON_GETCS_SPECIFIC_USER.Replace("@@#UPN#@@", userIdentity), IncludeObject = true });
                    }
                    if (onLineExecutionscripts.Count > 0)
                    {
                        //Online
                        isSuccess = ExecuteOnLineLineScript(onLineExecutionscripts);
                    }
                   
                    userInfo.UserType = EntitlementType.Default;
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Execute Transfer Action Completed");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                this._idmsLogger.WriteLog(LogLevelType.Exception, "Error while Discover UnassignedNumber.", ex);
            }
            finally
            {
                onLineExecutionscripts?.Clear();
                onLineExecutionscripts = null;
            }
            return isSuccess;
        }
        private void SetProvisionongFields()
        {
            var provisioningFields = NProvisioningField.GetByEndpointSystemId(this.Endpoint.Id);
            if (provisioningFields != null && provisioningFields.Count > 0)
            {
                VoicePolicyName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "VOICEPOLICY".ToUpper())?.Value;
                VoiceRoutingPolicyName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "VOICEROUTINGPOLICY".ToUpper())?.Value;
                DialPlanName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DIALPLANPOLICY".ToUpper())?.Value;

                DCCVoicePolicyName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DCC_VOICEPOLICY".ToUpper())?.Value;
                DCCVoiceRoutingPolicyName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DCC_VOICEROUTINGPOLICY".ToUpper())?.Value;
                DCCDialPlanName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DCC_DIALPLAN".ToUpper())?.Value;
                DefaultDCCLineUriPrefix = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DefaultDCC".ToUpper())?.Value;

                DCCSVoicePolicyName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DCCS_VOICEPOLICY".ToUpper())?.Value;
                DCCSVoiceRoutingPolicyName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DCCS_VOICEROUTINGPOLICY".ToUpper())?.Value;
                DCCSDialPlanName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DCCS_DIALPLAN".ToUpper())?.Value;

                IDDVoicePolicyName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "IDD_VOICEPOLICY".ToUpper())?.Value;
                IDDVoiceRoutingPolicyName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "IDD_VOICEROUTINGPOLICY".ToUpper())?.Value;
                IDDDialPlanName = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "IDD_DIALPLAN".ToUpper())?.Value;
                
                hostingproviderfqdn = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "Hostingproviderproxyfqdn".ToUpper())?.Value;
                registrarPool1 = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "Registrarpool".ToUpper())?.Value;
                registrarPool2 = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "RegistrarPool2".ToUpper())?.Value;
                HostedMigrationOverrideUrl = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "HostedMigrationOverrideUrl".ToUpper())?.Value;
                domainController = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DomainController".ToUpper())?.Value;

                LineUriPrefix = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "LineUriPrefix".ToUpper())?.Value;
                onLineToOnPremTarget = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "OnLineToOnPremTarget".ToUpper())?.Value;
                onPremToOnLineTarget = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "OnPremToOnLineTarget".ToUpper())?.Value;
                conferencingpolicy = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "Conferencingpolicy".ToUpper())?.Value;
                externalAccessPolicy = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ExternalAccessPolicy".ToUpper())?.Value;
                IDDOnlineVoiceRoutingWithResponseGroupPolicy = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "IDDOnlineVoiceRoutingWithResponseGroupPolicy".ToUpper())?.Value;
                //For Provisioning Default Policy
                DefaultTeamDialPlanPolicy = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DefaultTeamDialPlanPolicy".ToUpper())?.Value;
                DefaultOnlineVoiceRoutingPolicy = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "DefaultOnlineVoiceRoutingPolicy".ToUpper())?.Value;
                //IDD Policy for Response Group
                IDDDialPlanWithResponseGroupPolicy = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "IDDDialPlanWithResponseGroupPolicy".ToUpper())?.Value;
                IDDOnlineVoiceRoutingWithResponseGroupPolicy = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "IDDOnlineVoiceRoutingWithResponseGroupPolicy".ToUpper())?.Value;
                //IDD Policy without response group
                IDDTenantDialPlanPolicy = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "IDDTenantDialPlanPolicy".ToUpper())?.Value;
                IDDOnlineVoiceRoutingPolicy = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "IDDOnlineVoiceRoutingPolicy".ToUpper())?.Value;

                TeamsUpgradePolicy = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "TeamsUpgradePolicy".ToUpper())?.Value;

                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "PowershellScriptExecutionDelay".ToUpper()) != null)
                {
                    var str = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "PowershellScriptExecutionDelay".ToUpper()).Value;
                    Int32 delay;
                    if (!string.IsNullOrWhiteSpace(str) && Int32.TryParse(str, out delay))
                    {
                        PowershellScriptExecutionDelay = Convert.ToInt32(delay);
                    }
                    else
                    {
                        PowershellScriptExecutionDelay = 10000;
                    }
                }

                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_SetCsUser".ToUpper()) != null)
                {
                    ON_SetCsUser = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_SetCsUser".ToUpper()).Value;
                }
                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_SetCsUserEnterpriseVoiceEnabled".ToUpper()) != null)
                {
                    ON_SetCsUserEnterpriseVoiceEnabled = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_SetCsUserEnterpriseVoiceEnabled".ToUpper()).Value;
                }

                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GRANT_CS_TDP".ToUpper()) != null)
                {
                    ON_GRANT_CS_TDP = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GRANT_CS_TDP".ToUpper()).Value;
                }

                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GRANT_CS_OVRP".ToUpper()) != null)
                {
                    ON_GRANT_CS_OVRP = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GRANT_CS_OVRP".ToUpper()).Value;
                }

                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GRANT_CS_TUP".ToUpper()) != null)
                {
                    ON_GRANT_CS_TUP = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GRANT_CS_TUP".ToUpper()).Value;
                }

                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GRANT_CS_EAP".ToUpper()) != null)
                {
                    ON_GRANT_CS_EAP = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GRANT_CS_EAP".ToUpper()).Value;
                }

                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GRANT_CS_CONFPLY".ToUpper()) != null)
                {
                    ON_GRANT_CS_CONFPLY = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GRANT_CS_CONFPLY".ToUpper()).Value;
                }

                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GET_CSUSER".ToUpper()) != null)
                {
                    ON_GET_CSUSER = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GET_CSUSER".ToUpper()).Value;
                }

               
                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_FindUserLineURI".ToUpper()) != null)
                {
                    ON_FindUserLineURI = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_FindUserLineURI".ToUpper()).Value;
                }
               
                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_UserHasResponseGroup".ToUpper()) != null)
                {
                    ON_UserHasResponseGroup = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_UserHasResponseGroup".ToUpper()).Value;
                }
                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_DGMemberCheck".ToUpper()) != null)
                {
                    ON_DGMemberCheck = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_DGMemberCheck".ToUpper()).Value;
                }
                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GETCS_SPECIFIC_USER".ToUpper()) != null)
                {
                    ON_GETCS_SPECIFIC_USER = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_GETCS_SPECIFIC_USER".ToUpper()).Value;
                }
                
                if(provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_CsCallQueueScript".ToUpper()) != null)
                {
                    ON_CsCallQueueScript = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_CsCallQueueScript".ToUpper()).Value;
                }
                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_Remove_MSTeamLineURI".ToUpper()) != null)
                {
                    ON_Remove_MSTeamLineURI = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_Remove_MSTeamLineURI".ToUpper()).Value;
                }
                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_AddResponseGroup_Delegate".ToUpper()) != null)
                {
                    ON_AddResponseGroup_Delegate = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_AddResponseGroup_Delegate".ToUpper()).Value;
                }
                if (provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_RemoveDelegate_AfterResponseGroup".ToUpper()) != null)
                {
                    ON_RemoveDelegate_AfterResponseGroup = provisioningFields.FirstOrDefault(i => i.FieldName.ToUpper() == "ON_RemoveDelegate_AfterResponseGroup".ToUpper()).Value;
                }
            }
            else
            {
                throw new Exception("MicrosoftTeams provisioningFields are missing");
            }
        }

        private List<SkypeForBusinessResponseGroupInfo> GetAllResponseGroup()
        {
            var responseGroups = new List<SkypeForBusinessResponseGroupInfo>();
            responseGroups = SkypeForBusinessResponseGroupInfo.GetAllByEndpointId(this.Endpoint.Id);
            return responseGroups;
        }
        public override bool ApplyNewPolicy(IEndpointUserInfo _userInfo, EntitlementType requestedType, bool moveActionRequired = false)
        {
            bool isPolicyApplied = false;
            var onLineExecutionscripts = new List<SkypeForBusinessScriptBlock>();
            try
            {
                var userInfo = _userInfo as SkypeForBusinessUserInfo;
                var userIdentity = userInfo.UserPrincipalName;
                {
                    if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TDP))
                    {
                        onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = ON_GRANT_CS_TDP.Replace("@@#UPN#@@", userIdentity).Replace("@@#TENANTDIALPLAN#@@", userInfo.TenantDialPlan) });
                    }
                    if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_OVRP))
                    {
                        onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = ON_GRANT_CS_OVRP.Replace("@@#UPN#@@", userIdentity).Replace("@@#ONLINEVOICEROUTINGPOLICY#@@", userInfo.OnlineVoiceRoutingPolicy) });
                    }
                    if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TUP))
                    {
                        onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = ON_GRANT_CS_TUP.Replace("@@#UPN#@@", userIdentity)});
                    }
                    if(requestedType == EntitlementType.DCC && !string.IsNullOrWhiteSpace(ON_Remove_MSTeamLineURI))
                    {
                        var removeLineUri = ON_Remove_MSTeamLineURI.Replace("@@#UPN#@@", userIdentity);
                        onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = removeLineUri });
                    }
                    //Online
                    if (userInfo.IsRequireLineUriAssignment && !string.IsNullOrWhiteSpace(ON_SetCsUser))
                    {
                        var onlineSeCsUser = ON_SetCsUser.Replace("@@#UPN#@@", userIdentity).Replace("@@#LINEURI#@@", userInfo.LineURI);
                        onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = onlineSeCsUser });

                        if (!string.IsNullOrWhiteSpace(ON_SetCsUserEnterpriseVoiceEnabled))
                        {
                            var setOnlineEnterPriseVoiceEnabled = ON_SetCsUserEnterpriseVoiceEnabled.Replace("@@#UPN#@@", userIdentity);
                            onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = setOnlineEnterPriseVoiceEnabled });
                        }
                    }
                }

                if(!string.IsNullOrWhiteSpace(ON_GETCS_SPECIFIC_USER) && (requestedType == EntitlementType.IDD))
                {
                    var onlineScript = ON_GETCS_SPECIFIC_USER.Replace("@@#UPN#@@", userIdentity);
                    onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = onlineScript, IncludeObject = true });
                }

                isPolicyApplied = ExecuteOnLineLineScript(onLineExecutionscripts);
                _idmsLogger.WriteLog(LogLevelType.Info, $"AddEntitlement({requestedType.ToString()}) completed.");
            }
            catch (Exception ex)
            {
                isPolicyApplied = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"ApplyNewPolicy failed", ex);
            }
            finally
            {
                onLineExecutionscripts.Clear();
                onLineExecutionscripts = null;
            }
            return isPolicyApplied;
        }
        
        public override bool UpdateEndpointUserDetails(IEndpointUserInfo _userInfo)
        {
            bool isUpdate = false;
            try
            {
                var sfbAccount = _userInfo as SkypeForBusinessUserInfo;
                var onlineExecutionscripts = new List<SkypeForBusinessScriptBlock>();

                if (!string.IsNullOrWhiteSpace(ON_SetCsUser))
                {
                    var SetOnPremEnterPriseVoice = ON_SetCsUser.Replace("@@#UPN#@@", sfbAccount.AccountName).Replace("@@#LINEURI#@@", sfbAccount.LineURI);
                    onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = SetOnPremEnterPriseVoice, IncludeCredential = true });

                    if (!string.IsNullOrWhiteSpace(ON_SetCsUserEnterpriseVoiceEnabled))
                    {
                        var setOnlineEnterPriseVoiceEnabled = ON_SetCsUserEnterpriseVoiceEnabled.Replace("@@#UPN#@@", sfbAccount.AccountName);
                        onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = setOnlineEnterPriseVoiceEnabled });
                    }
                }
                isUpdate = ExecuteOnLineLineScript(onlineExecutionscripts);
                _idmsLogger.WriteLog(LogLevelType.Info, $"AddEntitlement({sfbAccount.UserType.ToString()}) LineUri Updated successfully for User:{sfbAccount.AccountName}.");
            }
            catch (Exception ex)
            {
                isUpdate = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"AddEntitlement  UpdateEndpointUserDetails failed for user:{_userInfo.AccountName}", ex);
            }
            return isUpdate;
        }
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<string> entitlement)
        {
            bool isSuccess = false;
            var onlineExecutionscripts = new List<SkypeForBusinessScriptBlock>();
            try
            {
                var userInfo = SkypeForBusinessUserInfo.GetByIdIncludeGroups(_userInfo.Id, this.Endpoint.Id);
                var userIdentity = userInfo.UserPrincipalName;
                this.userSIPAddress = userInfo.SipAddress;
                if (CheckOnlineAccountExists(userIdentity))
                {
                    var sfbAccount = this.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                    EntitlementType usrType = EntitlementType.None;
                    //SetProvisionongFields();
                    
                    if (_entitlementType == EntitlementType.ResponseGroup)
                    {
                        
                        if (sfbAccount == null) throw new Exception($"User:{userIdentity} not found in Endpoint");
                        else
                        {
                            if (string.IsNullOrWhiteSpace(userInfo.Identity) && !string.IsNullOrWhiteSpace(sfbAccount.Identity))
                            {
                                userInfo.Identity = sfbAccount.Identity;
                            }
                            usrType = GetUserType(sfbAccount);
                            if (usrType == EntitlementType.DCC || usrType == EntitlementType.DCCS)
                            {
                                throw new Exception($"User is holding :{usrType}. Cannot assign Response Group");
                            }
                        }

                        #region group
                        //resGroups = GetAllResponseGroup();
                        entitlement = entitlement.Where(i => !userInfo.SkypeForBusinessResponseGroup.Exists(j => j.Name.Equals(i, StringComparison.InvariantCultureIgnoreCase))).ToList();
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Responsegroup: {string.Join(",", entitlement)}");
                        
                        if (userInfo != null && entitlement.Count > 0)
                        {
                            #region Group
                            
                            var adddelegationScript = string.Empty;
                            var script = new List<SkypeForBusinessScriptBlock>();
                            foreach (var grpEntitlement in entitlement)
                            {
                                var grp = SkypeForBusinessResponseGroupInfo.GetGroupByName(grpEntitlement);
                                if (grp != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(grp.DistributionGroupAddress))
                                    {
                                        if (AddResponseGroupMember(grp.Name, grp.DistributionGroupAddress, userInfo.AccountName, userInfo.Identity, userInfo.AccountNameFromUPN))
                                        {
                                            _idmsLogger.WriteLog(LogLevelType.Info, $"Add Entitlement:ResponseGroup:{grp.Name} for user :{userInfo.AccountName}  completed");
                                            var matrixADID = SkypeForBusinessProvisioningMatrix.GetByName(grp.Name, EntitlementType.ResponseGroup);
                                            if(matrixADID != null && !string.IsNullOrWhiteSpace(matrixADID.ADID))
                                            {
                                                adddelegationScript = ON_AddResponseGroup_Delegate.Replace("@@#GROUP_UPNADID#@@", matrixADID.ADID).Replace("@@#UPN#@@", userInfo.AccountName);
                                                try
                                                {
                                                    script = new List<SkypeForBusinessScriptBlock>();
                                                    script.Add(new SkypeForBusinessScriptBlock() { Script = adddelegationScript });
                                                    ExecuteOnLineLineScript(script);
                                                }
                                                catch (Exception ex)
                                                {
                                                    WriteProvisioningLog(LogLevelType.Info, $"Add Delegation failed {ex.ToString()}. Script:{adddelegationScript}", ex);
                                                }
                                            }
                                            else
                                            {
                                                WriteProvisioningLog(LogLevelType.Info, $"Unable to Add Delegation becaue ADID for Group:{grp.Name} not exists in matrix failed for ResponseGroup:{grpEntitlement}");
                                            }
                                        }
                                        else
                                        {
                                            WriteProvisioningLog(LogLevelType.Info, $"DistributionGroup AddEntitlement :{grp.Name} & UPN:{userInfo.AccountName} failed for ResponseGroup:{grpEntitlement}");
                                        }
                                    }
                                    else
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"DistributionGroup identity does not exists for ResponseGroup:{grpEntitlement}");
                                    }
                                }
                                else
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"ResponseGroup:{grpEntitlement} does not exists");
                                }
                            }

                            bool executeScriptRequired = (usrType != EntitlementType.DCC);

                            if (usrType == EntitlementType.IDD)
                            {
                                WriteProvisioningLog(LogLevelType.Info, $"IDMS usertype:{userInfo.UserType} Current MSTeams UserType:{usrType}");
                                userInfo.TenantDialPlan = IDDTenantDialPlanPolicy;
                                userInfo.DialPlan = IDDDialPlanName;
                                userInfo.OnlineVoiceRoutingPolicy = IDDOnlineVoiceRoutingWithResponseGroupPolicy;
                                userInfo.TeamsUpgradePolicy = TeamsUpgradePolicy;
                                executeScriptRequired = true;
                            }
                            else if (usrType != EntitlementType.DCC || usrType != EntitlementType.DCCS)
                            {
                                userInfo.TenantDialPlan = DefaultTeamDialPlanPolicy;
                                userInfo.DialPlan = DialPlanName;
                                userInfo.OnlineVoiceRoutingPolicy = DefaultOnlineVoiceRoutingPolicy;
                                userInfo.TeamsUpgradePolicy = TeamsUpgradePolicy;
                                executeScriptRequired = true;
                            }

                            if (executeScriptRequired)
                            {
                                if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TDP))
                                {
                                    var tdp = ON_GRANT_CS_TDP.Replace("@@#UPN#@@", userInfo.UserPrincipalName).Replace("@@#TENANTDIALPLAN#@@", userInfo.TenantDialPlan);
                                    onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = tdp });
                                }

                                if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_OVRP))
                                {
                                    var tdp = ON_GRANT_CS_OVRP.Replace("@@#UPN#@@", userInfo.UserPrincipalName).Replace("@@#ONLINEVOICEROUTINGPOLICY#@@", userInfo.OnlineVoiceRoutingPolicy);
                                    onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = tdp });
                                }
                                if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TUP))
                                {
                                    var tup = ON_GRANT_CS_TUP.Replace("@@#UPN#@@", userInfo.UserPrincipalName);
                                    onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = tup });
                                }
                            }
                            isSuccess = ExecuteOnLineLineScript(onlineExecutionscripts);
                            #endregion
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Entitlement count:{entitlement.Count}.");
                            isSuccess = true;
                        }
                        #endregion
                    }
                }
                else
                {
                    this._idmsLogger.WriteLog(LogLevelType.Warning,$"CheckOnlineAccountExists failed");
                   isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, $"AddEntitlement failed for the User{_userInfo.AccountName}.", ex);
                isSuccess = false;
            }
            finally
            {
                onlineExecutionscripts?.Clear();
                onlineExecutionscripts = null;
            }
            return isSuccess;
        }
        public override bool RemoveResponseGroupMembers(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<string> entitlement)
        {
            bool isSuccess = true;
            var onlineExecutionscripts = new List<SkypeForBusinessScriptBlock>();
            List<SkypeForBusinessResponseGroupInfo> resGroups = null;
            try
            {
                EntitlementType userType = EntitlementType.None;
                if (CheckOnlineAccountExists(_userInfo.AccountName))
                {
                    var currentSFBUser = this.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                    if (currentSFBUser == null)
                    {
                        throw new Exception($"User:{_userInfo.AccountName} not found.");
                    }

                    var userInfo = (SkypeForBusinessUserInfo)_userInfo;
                    var userIdentity = userInfo.AccountName;
                    this.userSIPAddress = userInfo.SipAddress;
                    var userWithGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(userInfo.Id, this.Endpoint.Id);
                    #region removing response group
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Remove Entitlement:ResponseGroup started");
                    string adddelegationScript = string.Empty;
                    List<SkypeForBusinessScriptBlock> script = new List<SkypeForBusinessScriptBlock>();
                    foreach (var grpEntitlement in userWithGroups.SkypeForBusinessResponseGroup)
                    {
                        if (grpEntitlement != null && entitlement.Exists(i=> i.Equals(grpEntitlement.Name,StringComparison.InvariantCultureIgnoreCase)))
                        {
                            if(RemoveResponseGroupMember(grpEntitlement.Name, grpEntitlement.DistributionGroupName, userInfo.AccountName, userInfo.Identity, userInfo.AccountNameFromUPN))
                            {
                                try
                                {
                                    var matrixADID = SkypeForBusinessProvisioningMatrix.GetByName(grpEntitlement.Name, EntitlementType.ResponseGroup);
                                    if (matrixADID != null && !string.IsNullOrWhiteSpace(matrixADID.ADID))
                                    {
                                        adddelegationScript = ON_RemoveDelegate_AfterResponseGroup.Replace("@@#GROUP_UPNADID#@@", matrixADID.ADID).Replace("@@#UPN#@@", userInfo.AccountName);
                                        try
                                        {
                                            script = new List<SkypeForBusinessScriptBlock>();
                                            script.Add(new SkypeForBusinessScriptBlock() { Script = adddelegationScript });
                                            ExecuteOnLineLineScript(script);
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteProvisioningLog(LogLevelType.Info, $"Remove Delegation failed {ex.ToString()}. Script:{adddelegationScript}", ex);
                                        }
                                    }
                                    else
                                    {
                                        WriteProvisioningLog(LogLevelType.Info, $"Unable to Remove Delegation becaue ADID for Group:{grpEntitlement.Name} not exists in matrix failed for ResponseGroup:{grpEntitlement}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"Unable to Remove Delegation becaue ADID for Group:{grpEntitlement.Name} not exists in matrix failed for ResponseGroup failed.{ex.ToString()}");
                                }
                            }
                        }
                    }
                    bool isLastGroup = false;
                    if (!string.IsNullOrWhiteSpace(ON_UserHasResponseGroup))
                    {
                        isLastGroup = !IsUserPartOfAnyGroup(_userInfo);
                    }
                    onlineExecutionscripts.Clear();
                    userType = GetUserType(currentSFBUser);

                    if (userType == EntitlementType.Default || userType == EntitlementType.IDD)
                    {
                        if (userType == EntitlementType.IDD)
                        {
                            userInfo.TenantDialPlan = IDDTenantDialPlanPolicy;
                            userInfo.OnlineVoiceRoutingPolicy = IDDOnlineVoiceRoutingPolicy;
                            userInfo.TeamsUpgradePolicy = TeamsUpgradePolicy;
                            if (isLastGroup)
                            {
                                userInfo.TenantDialPlan = IDDTenantDialPlanPolicy;
                                userInfo.OnlineVoiceRoutingPolicy = IDDOnlineVoiceRoutingPolicy;
                                userInfo.TeamsUpgradePolicy = TeamsUpgradePolicy;
                            }
                            else
                            {
                                userInfo.TenantDialPlan = IDDDialPlanWithResponseGroupPolicy;
                                userInfo.OnlineVoiceRoutingPolicy = IDDOnlineVoiceRoutingWithResponseGroupPolicy;
                                userInfo.TeamsUpgradePolicy = TeamsUpgradePolicy;
                            }
                            if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TDP))
                            {
                                var tenantDialPlan = ON_GRANT_CS_TDP.Replace("@@#UPN#@@", userIdentity).Replace("@@#TENANTDIALPLAN#@@", userInfo.TenantDialPlan);
                                onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = tenantDialPlan });
                            }
                            if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_OVRP))
                            {
                                var onlineVRP = ON_GRANT_CS_OVRP.Replace("@@#UPN#@@", userIdentity).Replace("@@#ONLINEVOICEROUTINGPOLICY#@@", userInfo.OnlineVoiceRoutingPolicy);
                                onlineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = onlineVRP });
                            }
                        }
                    }

                    isSuccess = ExecuteOnLineLineScript(onlineExecutionscripts);
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Remove Entitlement(ResponseGroup)  completed");
                    #endregion
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, $"Remove Entitlement: Failed to remove User({_userInfo.AccountName}) from ResponseGroup.", ex);
                isSuccess = false;
            }
            finally
            {
                resGroups?.Clear();
                resGroups = null;
            }
            return isSuccess;
        }
        /// <summary>
        /// This method can be used for Both Removal of DCC and DCCS
        /// </summary>
        /// <param name="_userInfo"></param>
        /// <param name="_entitlementType"></param>
        /// <returns></returns>
        public override bool RemoveDCCPolicy(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            var onLineExecutionscripts = new List<SkypeForBusinessScriptBlock>();
            bool isSuccess = false;
            try
            {
                //SetProvisionongFields();
                var userInfo = (SkypeForBusinessUserInfo)_userInfo;
                var userWithGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(userInfo.Id, this.Endpoint.Id);
                var userIdentity = userWithGroups.AccountName;
                this.userSIPAddress = userWithGroups.SipAddress;
                if (CheckOnlineAccountExists(userIdentity))
                {
                    var temp = this.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                    if (temp != null)
                    {
                        userInfo.HostingProvider = this.onPremToOnLineTarget;

                        _idmsLogger.WriteLog(LogLevelType.Info, $"RemoveEntitlement({_entitlementType.ToString()}) started.");
                        if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TDP))
                        {
                            var tenantDialPlan = ON_GRANT_CS_TDP.Replace("@@#UPN#@@", userIdentity).Replace("@@#TENANTDIALPLAN#@@", userInfo.TenantDialPlan);
                            onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = tenantDialPlan });
                        }
                        if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_OVRP))
                        {
                            var onlineVRP = ON_GRANT_CS_OVRP.Replace("@@#UPN#@@", userIdentity).Replace("@@#ONLINEVOICEROUTINGPOLICY#@@", userInfo.OnlineVoiceRoutingPolicy);
                            onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = onlineVRP });
                        }
                        if (!string.IsNullOrWhiteSpace(ON_SetCsUser) && userInfo.IsRequireLineUriAssignment)
                        {
                            var assignLineUri = ON_SetCsUser.Replace("@@#UPN#@@", userIdentity).Replace("@@#LINEURI#@@", userInfo.LineURI);
                            onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = assignLineUri });

                            if (!string.IsNullOrWhiteSpace(ON_SetCsUserEnterpriseVoiceEnabled))
                            {
                                var setOnlineEnterPriseVoiceEnabled = ON_SetCsUserEnterpriseVoiceEnabled.Replace("@@#UPN#@@", userIdentity);
                                onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = setOnlineEnterPriseVoiceEnabled });
                            }
                        }
                        isSuccess = ExecuteOnLineLineScript(onLineExecutionscripts);
                        Thread.Sleep(30000);
                    }
                    else
                    {
                        isSuccess = false;
                        _idmsLogger.WriteLog(LogLevelType.Info, $"RemoveEntitlement({_entitlementType.ToString()}) EndpointUserAccount empty.");
                    }
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, $"Removing DCC Policy: Failed for the User:{_userInfo.AccountName}.", ex);
                isSuccess = false;
            }
            finally
            {
                onLineExecutionscripts?.Clear();
                onLineExecutionscripts = null;
            }
            return isSuccess;
        }
        [Obsolete]
        public override bool RemoveDCCSPolicy(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            return true;
        }
        public override bool RemoveIDDPolicy(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            bool isSuccess = false;
            List<SkypeForBusinessScriptBlock> onLineExecutionscripts = new List<SkypeForBusinessScriptBlock>();

            try
            {
                //SetProvisionongFields();
                var userInfo = (SkypeForBusinessUserInfo)_userInfo;
                var userWithGroups = SkypeForBusinessUserInfo.GetByIdIncludeGroups(userInfo.Id, this.Endpoint.Id);
                var userIdentity = userWithGroups.UserPrincipalName;
                this.userSIPAddress = userWithGroups.SipAddress;
                List<SkypeForBusinessScriptBlock> checkUserInGroup = new List<SkypeForBusinessScriptBlock>();
                #region remove IDD
                if (CheckOnlineAccountExists(userIdentity))
                {

                    var temp = this.EndpointOnlineUserAccount as SkypeForBusinessUserInfo;
                    if (temp != null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"RemoveEntitlement(IDD) started.");
                        userInfo.HostingProvider = this.onPremToOnLineTarget;

                        if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TDP))
                        {
                            var tenantDialPlan = ON_GRANT_CS_TDP.Replace("@@#UPN#@@", userIdentity).Replace("@@#TENANTDIALPLAN#@@", userInfo.TenantDialPlan);
                            onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = tenantDialPlan });
                        }
                        
                        if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_OVRP))
                        {
                            var onlineVRP = ON_GRANT_CS_OVRP.Replace("@@#UPN#@@", userIdentity).Replace("@@#ONLINEVOICEROUTINGPOLICY#@@", userInfo.OnlineVoiceRoutingPolicy);
                            onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = onlineVRP });
                        }
                        //if (!string.IsNullOrWhiteSpace(ON_GRANT_CS_TUP))
                        //{
                        //    var SetOnPremEnterPriseVoice = ON_GRANT_CS_TUP.Replace("@@#UPN#@@", userIdentity);
                        //    onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = SetOnPremEnterPriseVoice });
                        //}
                        //if (!string.IsNullOrWhiteSpace(ON_SetCsUser))
                        //{
                        //    var setOnlineEnterPriseVoice = ON_SetCsUser.Replace("@@#UPN#@@", userIdentity).Replace("@@#LINEURI#@@", userInfo.LineURI);
                        //    onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = setOnlineEnterPriseVoice });
                        //}
                        
                        isSuccess = ExecuteOnLineLineScript(onLineExecutionscripts);
                    }
                    else
                    {
                        isSuccess = false;
                        _idmsLogger.WriteLog(LogLevelType.Info, $"RemoveEntitlement({_entitlementType.ToString()}) EndpointUserAccount empty.");
                    }
                }
                else
                {
                    isSuccess = false;
                    _idmsLogger.WriteLog(LogLevelType.Info, $"RemoveEntitlement({_entitlementType.ToString()}) CheckEndPointAccountExists failed.");
                }
                #endregion
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, $"Skype for business:Removing IDD Policy: Failed for the User:{_userInfo.AccountName}.", ex);
                isSuccess = false;
            }
            finally
            {
                onLineExecutionscripts?.Clear();
                onLineExecutionscripts = null;
            }
            return isSuccess;
        }
        /// <summary>
        /// Not required
        /// </summary>
        /// <param name="_userInfo"></param>
        /// <param name="_entitlementType"></param>
        /// <param name="entitlement"></param>
        /// <returns></returns>
        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, List<string> entitlement)
        {
            throw new Exception("Method not implemented in IDMS");
        }

        public void SendEmail(string responseGroupNames)
        {
            string strMessage = string.Empty;
            StringBuilder strBldr = new StringBuilder();
            DomainModel.MailTemplate template = null;
            StringBuilder subject = null;
            try
            {
                EmailInfo emailInfo = new EmailInfo();
                template = MailTemplate.GetByTemplateName(Constants.NotifySkypeForBusinessNewResponseGroup);
                subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@ENDPOINTNAME@@", this.Endpoint.DisplayName);
                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@ENDPOINTTYPE@@", Endpoint.EndPointSystemType.DisplayName);
                body.Replace("@@ENDPOINTNAME@@", Endpoint.DisplayName);
                body.Replace("@@RESPONSEGROUPNAMES@@", responseGroupNames);
                body.Replace("@@IACMURL@@", DI.Chakra.DomainModel.MasterConfigHelper.IACMLoginUrl);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                var emailTeam = DI.Chakra.DomainModel.MasterConfigHelper.EmailTeam;
                var group = Group.GetByGroupName(emailTeam);
                emailInfo.To = string.Format("{0}",string.Join(",", group.Users.Where(i=>!string.IsNullOrEmpty(i.EmailId)).Select(i=>i.EmailId)));
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Discovery.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Failed in composing mail from template", ex);
            }
        }

        public override bool CheckEndPointAccountExists(string accountName)
        {
            _idmsLogger.WriteLog(LogLevelType.Info, $"Check Account Exists");
            bool isExists = false;
            var onLineExecutionscripts = new List<SkypeForBusinessScriptBlock>();

            try
            {
                if (!string.IsNullOrWhiteSpace(ON_GETCS_SPECIFIC_USER))
                {
                    onLineExecutionscripts.Add(new SkypeForBusinessScriptBlock() { Script = ON_GETCS_SPECIFIC_USER.Replace("@@#UPN#@@", accountName) });
                }
                isExists = ExecuteOnLineLineScript(onLineExecutionscripts, true);
            }
            catch (Exception ex)
            {
                isExists = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, $"CheckEndPointAccountExists failed {ex.ToString()}", ex);
            }
            finally
            {
                onLineExecutionscripts?.Clear();
                onLineExecutionscripts = null;
            }
            return isExists;
        }
        public override bool CheckOnlineAccountExists(string accountName)
        {
            _idmsLogger.WriteLog(LogLevelType.Info, $"Check Online Account Exists");
            bool isExists = false;
            var onLineScripts = new List<SkypeForBusinessScriptBlock>();

            try
            {
                if (string.IsNullOrWhiteSpace(accountName)) throw new Exception("AccountName is empty");

                if (!string.IsNullOrWhiteSpace(ON_GETCS_SPECIFIC_USER))
                {
                    onLineScripts.Add(new SkypeForBusinessScriptBlock() { Script = ON_GETCS_SPECIFIC_USER.Replace("@@#UPN#@@", accountName) });
                }
                isExists = ExecuteOnLineLineScript(onLineScripts, true);
                if (isExists)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Online Account Exists.");
                }
            }
            catch (Exception ex)
            {
                isExists = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, "CheckOnlineAccountExists failed", ex);
            }
            finally
            {
                onLineScripts?.Clear();
                onLineScripts = null;
            }
            return isExists;
        }
        
        public override bool UndoOffBoardEvent(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            var isAccountCreated = true;
            var onLineExecutionscripts = new List<SkypeForBusinessScriptBlock>();
            try
            {
                var userInfo = _userInfo as SkypeForBusinessUserInfo;
                var userIdentity = userInfo.UserPrincipalName;
                
                
            }
            catch (Exception ex)
            {
                isAccountCreated = false;
                _idmsLogger.WriteLog(LogLevelType.Exception, "UndoOffBoardEvent failed", ex);
            }
            finally
            {
                onLineExecutionscripts?.Clear();
                onLineExecutionscripts = null;
            }
            return isAccountCreated;
        }
        public override bool IsUserPropertyExists(string propertyName, string propertyValue, string accountName)
        {
            var isSuccess = false;
            try
            {
                //if its true then get next number.
                isSuccess = IsUserPropertyExistsInOnLine(propertyName, propertyValue, accountName);
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "IsUserPropertyExistsInOnLine", ex);
                isSuccess = false;
            }
            return isSuccess;
        }
        public bool IsUserPropertyExistsInOnLine(string propertyName, string propertyValue, string accountName)
        {
            var isSuccess = false;
            try
            {
                
                using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
                {
                    using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                    {
                        runspace.Open();
                        using (PowerShell powershell = PowerShell.Create())
                        {
                            try
                            {
                                powershell.Runspace = runspace;
                                foreach (var module in psModules)
                                {
                                    powershell.AddScript($"Import-Module \"{module}\"");
                                }
                                var impModule = powershell.Invoke();
                                if (!PowerShellErrorCheck(powershell, impModule))
                                {
                                    powershell.Commands.Clear();
                                    var connect = new Command("Connect-MicrosoftTeams");
                                    connect.Parameters.Add("Credential", onLineCredential);
                                    powershell.Commands.AddCommand(connect);
                                    var moduelImport = powershell.Invoke();
                                    if (!PowerShellErrorCheck(powershell, moduelImport))
                                    {
                                        powershell.Commands.Clear();
                                        WriteProvisioningLog(LogLevelType.Info, $"Connect-MicrosoftTeams successful");
                                        var script = ON_FindUserLineURI.Replace("@@#LINEURI#@@", propertyValue);
                                        WriteProvisioningLog(LogLevelType.Info, $"Execution Script:{script}");
                                        powershell.Commands.AddScript(script);
                                        var executionResult = powershell.Invoke();
                                        if (!PowerShellErrorCheck(powershell, executionResult))
                                        {
                                            powershell.Commands.Clear();
                                            if (executionResult != null && executionResult.Count > 0)
                                            {
                                                var UserPrincipalName = SkypeForBusinessHelper.GetProperty(executionResult.FirstOrDefault(), new SkypeForBusinessProperties().UserPrincipalName);
                                                WriteProvisioningLog(LogLevelType.Info, $"Input:AccountName:{accountName} - output:UserPrincipalName:{UserPrincipalName} LineUri:{propertyValue}");
                                                if(!string.IsNullOrWhiteSpace(UserPrincipalName) && !string.IsNullOrWhiteSpace(accountName) && accountName.ToUpper() != UserPrincipalName.ToUpper())
                                                {
                                                    isSuccess = true;
                                                }
                                                else
                                                {
                                                    WriteProvisioningLog(LogLevelType.Info, $"Input:AccountName:{accountName} - output: UserPrincipalName:{UserPrincipalName} LineUri:{propertyValue}");
                                                }
                                            }
                                            else
                                            {
                                                WriteProvisioningLog(LogLevelType.Info, $"IsUserPropertyExists accountName:{accountName} LineUri:{propertyValue} not exists - no result");
                                            }
                                        }
                                        else
                                        {
                                            isSuccess = false;
                                            WriteProvisioningLog(LogLevelType.Info, $"IsUserPropertyExists script ({script}) failed");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteProvisioningLog(LogLevelType.Exception, $"IsUserPropertyExists script failed", ex);
                                throw ex;
                            }
                            finally
                            {
                                try
                                {
                                    powershell.Commands.Clear();
                                    var disConnect = new Command("Disconnect-MicrosoftTeams");
                                    powershell.Commands.AddCommand(disConnect);
                                    var moduelImport = powershell.Invoke();
                                }
                                catch (Exception disConnectEx)
                                {
                                    WriteProvisioningLog(LogLevelType.Info, $"Disconnect-MicrosoftTeams failed", disConnectEx);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Check IsUserPropertyExistsInOnLine", ex);
                isSuccess = false;
            }
            return isSuccess;
        }
        

        private bool AddResponseGroupMember(string responseGroupName,string distributionGroupIdentity, string userPrincipalName, string userIdentity, string samAccountName)
        {
            bool IsAdded = false;
            try
            {
                if (string.IsNullOrWhiteSpace(ON_DGMemberCheck)) throw new Exception("Command not found");

                var script = ON_DGMemberCheck.Replace("@@#GROUPIDENTITY#@@", distributionGroupIdentity).Replace("@@#UPN#@@", samAccountName).Replace("@@#USER_IDENTITY#@@", userIdentity ?? userPrincipalName);

                using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
                {
                    using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                    {
                        runspace.Open();
                        using (PowerShell powershell = PowerShell.Create())
                        {
                            powershell.Runspace = runspace;
                            try
                            {
                                foreach (var module in psMSTeamO365Modules)
                                {
                                    powershell.Commands.AddCommand("Import-Module").AddArgument(module);
                                }
                                var moduelImport = powershell.Invoke();
                                if (!PowerShellErrorCheck(powershell, moduelImport))
                                {
                                    powershell.Commands.Clear();
                                    var connect = new Command("Connect-ExchangeOnline");
                                    connect.Parameters.Add("Credential", credential);
                                    //connect.Parameters.Add("CommandName", new[] { "Get-DistributionGroup", "Get-DistributionGroupMember", "Add-DistributionGroupMember" });
                                    powershell.Commands.AddCommand(connect);
                                    var results = powershell.Invoke();
                                    powershell.Commands.Clear();
                                    if (!string.IsNullOrWhiteSpace(distributionGroupIdentity))
                                    {
                                        Command getDistGroup = new Command("Get-DistributionGroup");
                                        getDistGroup.Parameters.Add("identity", distributionGroupIdentity);
                                        var pipeline2 = runspace.CreatePipeline();
                                        pipeline2.Commands.Add(getDistGroup);
                                        var resultgroups = pipeline2.Invoke();
                                        if (resultgroups != null && resultgroups.Count > 0)
                                        {
                                            WriteProvisioningLog(LogLevelType.Info, script);

                                            powershell.Commands.Clear();
                                            bool isNotExists = false;
                                            try
                                            {
                                                WriteProvisioningLog(LogLevelType.Info, $"Check member:{userPrincipalName} exists in the group.");

                                                powershell.Runspace = runspace;
                                                powershell.Commands.AddScript(script);
                                                var executionResult = powershell.Invoke();
                                                if(executionResult != null && executionResult.Count > 0)
                                                {
                                                    WriteProvisioningLog(LogLevelType.Info, $"Member:{userPrincipalName} already exists in the group.");
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                WriteProvisioningLog(LogLevelType.Exception, $"{ex.ToString()}");
                                                isNotExists = true;
                                            }
                                            
                                            if (isNotExists == false)
                                            {
                                                powershell.Commands.Clear();

                                                Command AddGroups = new Command("Add-DistributionGroupMember");
                                                AddGroups.Parameters.Add("Identity", distributionGroupIdentity);
                                                AddGroups.Parameters.Add("Member", userPrincipalName);
                                                AddGroups.Parameters.Add("BypassSecurityGroupManagerCheck");
                                                AddGroups.Parameters.Add("Confirm", false);
                                                powershell.Commands.AddCommand(AddGroups);
                                                var exeResult = powershell.Invoke();
                                                if (!PowerShellErrorCheck(powershell, exeResult,false))
                                                {
                                                    IsAdded = true;
                                                    WriteProvisioningLog(LogLevelType.Exception, $"Response Group:{responseGroupName} Member:{userPrincipalName} added successfully");
                                                }
                                                powershell.Commands.Clear();
                                            }
                                            else
                                            {
                                                WriteProvisioningLog(LogLevelType.Exception, $"Member:{userPrincipalName} already exists in the Response Group:{responseGroupName}");
                                                IsAdded = true;
                                            }
                                        }
                                        else
                                        {
                                            WriteProvisioningLog(LogLevelType.Exception, $"Response Group:{responseGroupName} does not exists");
                                            throw new Exception("Response Group:{responseGroupName} does not exists");
                                        }
                                        resultgroups = null;
                                    }
                                    results = null;
                                }
                                moduelImport = null;
                            }
                            catch (Exception ex)
                            {
                                WriteProvisioningLog(LogLevelType.Exception, $"AddResponseGroupMember Execution failed", ex);
                                throw ex;
                            }
                            finally
                            {
                                DisconnectExchangeOnline(powershell);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "AddResponseGroupMember failed", ex);
                throw ex;
            }
            return IsAdded;
        }
        
        private bool RemoveResponseGroupMember(string responseGroupName, string distributionGroupIdentity, string userPrincipalName, string userIdentity, string samAccountName)
        {
            bool IsRemoved = false;
            try
            {

                if (string.IsNullOrWhiteSpace(ON_DGMemberCheck)) throw new Exception("Command not found");

                var script = ON_DGMemberCheck.Replace("@@#GROUPIDENTITY#@@", distributionGroupIdentity).Replace("@@#UPN#@@", samAccountName).Replace("@@#USER_IDENTITY#@@", userIdentity ?? userPrincipalName);
                using (PowerShellProcessInstance instance = new PowerShellProcessInstance(new Version(this.PSVersion, this.MinorPSVersion), null, null, false))
                {
                    using (var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(new string[0]), instance))
                    {
                        runspace.Open();
                        using (PowerShell powershell = PowerShell.Create())
                        {
                            powershell.Runspace = runspace;
                            try
                            {
                                foreach (var module in psMSTeamO365Modules)
                                {
                                    powershell.Commands.AddCommand("Import-Module").AddArgument(module);
                                }
                                var moduelImport = powershell.Invoke();
                                if (!PowerShellErrorCheck(powershell, moduelImport))
                                {
                                    powershell.Commands.Clear();
                                    powershell.Runspace = runspace;
                                    var connect = new Command("Connect-ExchangeOnline");
                                    connect.Parameters.Add("Credential", credential);
                                    //connect.Parameters.Add("CommandName", new[] { "Get-DistributionGroup", "Get-DistributionGroupMember", "Remove-DistributionGroupMember" });
                                    powershell.Commands.AddCommand(connect);
                                    var results = powershell.Invoke();
                                    powershell.Commands.Clear();
                                    List<string> mem = new List<string>();
                                    if (!string.IsNullOrWhiteSpace(distributionGroupIdentity))
                                    {
                                        Command getDistGroup = new Command("Get-DistributionGroup");
                                        getDistGroup.Parameters.Add("identity", distributionGroupIdentity);
                                        var pipeline2 = runspace.CreatePipeline();
                                        pipeline2.Commands.Add(getDistGroup);
                                        var resultgroups = pipeline2.Invoke();
                                        if (resultgroups != null && resultgroups.Count > 0)
                                        {
                                            Collection<PSObject> executionResult = null;
                                            WriteProvisioningLog(LogLevelType.Info, script);
                                            try
                                            {
                                                powershell.Commands.Clear();
                                                powershell.Runspace = runspace;
                                                powershell.Commands.AddScript(script);
                                                executionResult = powershell.Invoke();
                                                if (executionResult != null && executionResult.Count > 0)
                                                {
                                                    WriteProvisioningLog(LogLevelType.Info, $"Member exists and continue to remove from group:{responseGroupName}");
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                WriteProvisioningLog(LogLevelType.Exception, $"Script:{script} failed - {ex.ToString()}");
                                            }
                                            
                                            if (executionResult != null && executionResult.Count > 0)
                                            {

                                                powershell.Commands.Clear();
                                                Command rmvGroups = new Command("Remove-DistributionGroupMember");
                                                rmvGroups.Parameters.Add("Identity", distributionGroupIdentity);
                                                rmvGroups.Parameters.Add("Member", userPrincipalName);
                                                rmvGroups.Parameters.Add("BypassSecurityGroupManagerCheck");
                                                rmvGroups.Parameters.Add("Confirm", false);
                                                powershell.Commands.AddCommand(rmvGroups);
                                                var exeResult = powershell.Invoke();
                                                if (!PowerShellErrorCheck(powershell, exeResult))
                                                {
                                                    IsRemoved = true;
                                                    WriteProvisioningLog(LogLevelType.Info, $"Response Group:{responseGroupName} Member:{userPrincipalName} removed successfully");
                                                }
                                                powershell.Commands.Clear();
                                            }
                                            else
                                            {
                                                WriteProvisioningLog(LogLevelType.Exception, $" Member:{userPrincipalName} not found in Response Group:{responseGroupName}");
                                                IsRemoved = true;
                                            }
                                        }
                                        else
                                        {
                                            WriteProvisioningLog(LogLevelType.Exception, $"Response Group:{responseGroupName} does not exists");
                                            throw new Exception($"Response Group:{responseGroupName} does not exists");
                                        }
                                        resultgroups = null;
                                    }
                                    results = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteProvisioningLog(LogLevelType.Exception, $"RemoveResponseGroupMember Execution failed", ex);
                                throw ex;
                            }
                            finally
                            {
                                DisconnectExchangeOnline(powershell);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"AddResponseGroupMember failed.{ex.ToString()}", ex);
                throw ex;
            }
            return IsRemoved;
        }
        private void DisconnectExchangeOnline(PowerShell powerShell)
        {
            try
            {
                powerShell.Commands.Clear();
                Command Stat = null;
                Stat = new Command("Disconnect-ExchangeOnline");
                Stat.Parameters.Add("Confirm", false);
                Stat.Parameters.Add("InformationAction", "Ignore");
                Stat.Parameters.Add("ErrorAction", "SilentlyContinue");
                powerShell.Commands.AddCommand(Stat);
                powerShell.Invoke();
            }
            catch (Exception ex)
            {
            }
        }

        public override EntitlementType GetUserType(IEndpointUserInfo usrInfo)
        {
            if (usrInfo == null) throw new Exception($"GetUserType - User not found");

            EntitlementType entitlementType = EntitlementType.None;
            try
            {
                var userInfo = (SkypeForBusinessUserInfo)usrInfo;
                //if (CheckOnlineAccountExists(userInfo.AccountName))
                {
                    if (userInfo.OnlineVoiceRoutingPolicy.ToUpper() == DCCVoiceRoutingPolicyName.ToUpper())
                    {
                        entitlementType = EntitlementType.DCC;
                    }
                    else if (!string.IsNullOrEmpty(userInfo.OnlineVoiceRoutingPolicy) && userInfo.OnlineVoiceRoutingPolicy.ToUpper() == DefaultOnlineVoiceRoutingPolicy.ToUpper()) 
                    {
                        entitlementType = EntitlementType.Default;
                    }
                    else if (!string.IsNullOrWhiteSpace(userInfo.OnlineVoiceRoutingPolicy) && (
                        userInfo.OnlineVoiceRoutingPolicy.ToUpper() == IDDOnlineVoiceRoutingPolicy.ToUpper()
                        ||
                        userInfo.OnlineVoiceRoutingPolicy.ToUpper() == IDDOnlineVoiceRoutingWithResponseGroupPolicy.ToUpper()
                        )
                    )
                    {
                        entitlementType = EntitlementType.IDD;
                    }
                    else if(!string.IsNullOrEmpty(userInfo.OnlineVoiceRoutingPolicy) && userInfo.OnlineVoiceRoutingPolicy.ToUpper() == DefaultOnlineVoiceRoutingPolicy.ToUpper())
                    {
                        entitlementType = EntitlementType.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entitlementType;
        }
    }

}