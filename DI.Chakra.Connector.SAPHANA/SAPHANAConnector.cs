using DI.Chakra.DomainModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using SAP.Connector;
using SAP.Middleware.Connector;
using SAPUserDetailsComponent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace DI.Chakra.Connector.SAPHANA
{
    public class SAPHANAConnector : ConnectorBase
    {
        SAPHANAUtil sapUtil = null;

        SAPHANAProcessorConfig processorConfig = null;
        EndpointConfiguration SAPConfiguration { get; set; }
        readonly string MasterConfigHelperServiceUser;
        //List<AccountReconcilation> accountReconcilations;
        readonly string _SAPentitlementDateStringFormat;

        #region Position Related

        SAPUserGrp.BAPIHR0283Table Employee_Archived_Objects = new SAPUserGrp.BAPIHR0283Table();
        SAPUserGrp.BAPIHR0009Table Employee_Bank_Details = new SAPUserGrp.BAPIHR0009Table();
        SAPUserGrp.BAPIHR0315Table Employee_Cats_Default_Values = new SAPUserGrp.BAPIHR0315Table();
        SAPUserGrp.BAPIHR0105Table Employee_Communications = new SAPUserGrp.BAPIHR0105Table();
        SAPUserGrp.BAPIHR0222Table Employee_Company_Cars = new SAPUserGrp.BAPIHR0222Table();
        SAPUserGrp.BAPIHR0001Table Employee_Org_Assignment = new SAPUserGrp.BAPIHR0001Table();
        SAPUserGrp.BAPIHR0003Table Employee_Payroll_Status = new SAPUserGrp.BAPIHR0003Table();
        SAPUserGrp.BAPIHR0002Table Employee_Personal_Data = new SAPUserGrp.BAPIHR0002Table();
        SAPUserGrp.BAPIHR0000Table Employee_Personnel_Action = new SAPUserGrp.BAPIHR0000Table();
        SAPUserGrp.BAPIHR0006Table Employee_Private_Address = new SAPUserGrp.BAPIHR0006Table();
        SAPUserGrp.BAPIHR0900Table Employee_Sales_Org_Assignment = new SAPUserGrp.BAPIHR0900Table();
        SAPUserGrp.BAPIHR0050Table Employee_Time_Recording_Info = new SAPUserGrp.BAPIHR0050Table();
        SAPUserGrp.BAPIHR0017Table Employee_Trip_Privileges = new SAPUserGrp.BAPIHR0017Table();
        SAPUserGrp.BAPIHRINFTable Hr_Infotype_Header = new SAPUserGrp.BAPIHRINFTable();
        SAPUserGrp.BAPIHROBJTable Hr_Object_Header = new SAPUserGrp.BAPIHROBJTable();
        SAPUserGrp.BAPIHR1000Table Pd_Object = new SAPUserGrp.BAPIHR1000Table();
        SAPUserGrp.BAPIHR1028Table Pd_Object_Address = new SAPUserGrp.BAPIHR1028Table();
        SAPUserGrp.BAPIHR1043Table Pd_Object_Appraisal_Sample = new SAPUserGrp.BAPIHR1043Table();
        SAPUserGrp.BAPIHR1044Table Pd_Object_Appraisal_Scale = new SAPUserGrp.BAPIHR1044Table();
        SAPUserGrp.BAPIHR1010Table Pd_Object_Authority_Resources = new SAPUserGrp.BAPIHR1010Table();
        SAPUserGrp.BAPIHR1023Table Pd_Object_B_Event_Availability = new SAPUserGrp.BAPIHR1023Table();
        SAPUserGrp.BAPIHR1041Table Pd_Object_B_Event_Blocks = new SAPUserGrp.BAPIHR1041Table();
        SAPUserGrp.BAPIHR1024Table Pd_Object_B_Event_Capacity = new SAPUserGrp.BAPIHR1024Table();
        SAPUserGrp.BAPIHR1036Table Pd_Object_B_Event_Costs = new SAPUserGrp.BAPIHR1036Table();
        SAPUserGrp.BAPIHR1060Table Pd_Object_B_Event_Demand = new SAPUserGrp.BAPIHR1060Table();
        SAPUserGrp.BAPIHR1026Table Pd_Object_B_Event_Info = new SAPUserGrp.BAPIHR1026Table();
        SAPUserGrp.BAPIHR1034Table Pd_Object_B_Event_Name_Format = new SAPUserGrp.BAPIHR1034Table();
        SAPUserGrp.BAPIHR1021Table Pd_Object_B_Event_Price = new SAPUserGrp.BAPIHR1021Table();
        SAPUserGrp.BAPIHR1030Table Pd_Object_B_Event_Procedure = new SAPUserGrp.BAPIHR1030Table();
        SAPUserGrp.BAPIHR1042Table Pd_Object_B_Event_Sched_Model = new SAPUserGrp.BAPIHR1042Table();
        SAPUserGrp.BAPIHR1035Table Pd_Object_B_Event_Schedule = new SAPUserGrp.BAPIHR1035Table();
        SAPUserGrp.BAPIHR1029Table Pd_Object_B_Event_Type = new SAPUserGrp.BAPIHR1029Table();
        SAPUserGrp.BAPIHR1062Table Pd_Object_B_Eventknowledgelink = new SAPUserGrp.BAPIHR1062Table();
        SAPUserGrp.BAPIHR1037Table Pd_Object_Cost_Alloc_Invoice = new SAPUserGrp.BAPIHR1037Table();
        SAPUserGrp.BAPIHR1018Table Pd_Object_Cost_Distribution = new SAPUserGrp.BAPIHR1018Table();
        SAPUserGrp.BAPIHR1015Table Pd_Object_Cost_Planning = new SAPUserGrp.BAPIHR1015Table();
        SAPUserGrp.BAPIHR1003Table Pd_Object_Department_Staff = new SAPUserGrp.BAPIHR1003Table();
        SAPUserGrp.BAPIHR1002Table Pd_Object_Description = new SAPUserGrp.BAPIHR1002Table();
        SAPUserGrp.BAPIHR1013Table Pd_Object_Empl_Group_Subgroup = new SAPUserGrp.BAPIHR1013Table();
        SAPUserGrp.BAPIHR1038Table Pd_Object_External_Key = new SAPUserGrp.BAPIHR1038Table();
        SAPUserGrp.BAPIHR1019Table Pd_Object_Head_Count_Budget = new SAPUserGrp.BAPIHR1019Table();
        SAPUserGrp.BAPIHR1009Table Pd_Object_Health_Examinations = new SAPUserGrp.BAPIHR1009Table();
        SAPUserGrp.BAPIHR1051Table Pd_Object_Job_Market_Info = new SAPUserGrp.BAPIHR1051Table();
        SAPUserGrp.BAPIHR1032Table Pd_Object_Mail_Address = new SAPUserGrp.BAPIHR1032Table();
        SAPUserGrp.BAPIHR1014Table Pd_Object_Obsolete = new SAPUserGrp.BAPIHR1014Table();
        SAPUserGrp.BAPIHR1040Table Pd_Object_Override_Reqmnt = new SAPUserGrp.BAPIHR1040Table();
        SAPUserGrp.BAPIHR1017Table Pd_Object_Pd_Profiles = new SAPUserGrp.BAPIHR1017Table();
        SAPUserGrp.BAPIHR1005Table Pd_Object_Planned_Compensation = new SAPUserGrp.BAPIHR1005Table();
        SAPUserGrp.BAPIHR1008Table Pd_Object_Posting_Specs = new SAPUserGrp.BAPIHR1008Table();
        SAPUserGrp.BAPIHR1047Table Pd_Object_Processing_Fm = new SAPUserGrp.BAPIHR1047Table();
        SAPUserGrp.BAPIHR1048Table Pd_Object_Proficiency_Desc = new SAPUserGrp.BAPIHR1048Table();
        SAPUserGrp.BAPIHR1001Table Pd_Object_Relationships = new SAPUserGrp.BAPIHR1001Table();
        SAPUserGrp.BAPIHR1006Table Pd_Object_Restrictions = new SAPUserGrp.BAPIHR1006Table();
        SAPUserGrp.BAPIHR1031Table Pd_Object_Room_Reserv_Info = new SAPUserGrp.BAPIHR1031Table();
        SAPUserGrp.BAPIHR1039Table Pd_Object_Shift_Group = new SAPUserGrp.BAPIHR1039Table();
        SAPUserGrp.BAPIHR1027Table Pd_Object_Site_Dependent_Info = new SAPUserGrp.BAPIHR1027Table();
        SAPUserGrp.BAPIHR1016Table Pd_Object_Standard_Profiles = new SAPUserGrp.BAPIHR1016Table();
        SAPUserGrp.BAPIHR1004Table Pd_Object_Task_Character = new SAPUserGrp.BAPIHR1004Table();
        SAPUserGrp.BAPIHR1007Table Pd_Object_Vacancy = new SAPUserGrp.BAPIHR1007Table();
        SAPUserGrp.BAPIHR1025Table Pd_Object_Validity = new SAPUserGrp.BAPIHR1025Table();
        SAPUserGrp.BAPIHR1045Table Pd_Object_Valuation = new SAPUserGrp.BAPIHR1045Table();
        SAPUserGrp.BAPIHR1050Table Pd_Object_Work_Evaluat_Result = new SAPUserGrp.BAPIHR1050Table();
        SAPUserGrp.BAPIHR1033Table Pd_Object_Work_Reqmnt_Scale = new SAPUserGrp.BAPIHR1033Table();
        SAPUserGrp.BAPIHR1011Table Pd_Object_Work_Schedule = new SAPUserGrp.BAPIHR1011Table();
        SAPUserGrp.BAPIHRAD48Table Pd_Rel_Application_For = new SAPUserGrp.BAPIHRAD48Table();
        SAPUserGrp.BAPIHRAD47Table Pd_Rel_Appraisal_Model = new SAPUserGrp.BAPIHRAD47Table();
        SAPUserGrp.BAPIHRAD50Table Pd_Rel_Appraises = new SAPUserGrp.BAPIHRAD50Table();
        SAPUserGrp.BAPIHRAD44Table Pd_Rel_Career_Develop_Action = new SAPUserGrp.BAPIHRAD44Table();
        SAPUserGrp.BAPIHRAD21Table Pd_Rel_Is_Equipped = new SAPUserGrp.BAPIHRAD21Table();
        SAPUserGrp.BAPIHRAD27Table Pd_Rel_Is_Prebooked_For = new SAPUserGrp.BAPIHRAD27Table();
        SAPUserGrp.BAPIHRAD63Table Pd_Rel_Job_Reqmnts = new SAPUserGrp.BAPIHRAD63Table();
        SAPUserGrp.BAPIHRADXNTable Pd_Rel_Message_Type = new SAPUserGrp.BAPIHRADXNTable();
        SAPUserGrp.BAPIHRAD31Table Pd_Rel_Qualifications_Reqmnts = new SAPUserGrp.BAPIHRAD31Table();
        SAPUserGrp.BAPIHRAD22Table Pd_Rel_Requires = new SAPUserGrp.BAPIHRAD22Table();
        SAPUserGrp.BAPIHRAD23Table Pd_Rel_Reserves = new SAPUserGrp.BAPIHRAD23Table();
        SAPUserGrp.BAPIHRAD51Table Pd_Rel_Session = new SAPUserGrp.BAPIHRAD51Table();
        SAPUserGrp.BAPIHRADD2Table Pd_Rel_Specified_Substitute = new SAPUserGrp.BAPIHRADD2Table();
        SAPUserGrp.BAPIHRAD53Table Pd_Rel_Still_Requires = new SAPUserGrp.BAPIHRAD53Table();
        SAPUserGrp.BAPIHRAD25Table Pd_Rel_Takes_Part_In = new SAPUserGrp.BAPIHRAD25Table();
        SAPUserGrp.BAPIRET2Table Return0 = new SAPUserGrp.BAPIRET2Table();

        List<ApplicationType> positionApplicationTypes = new List<ApplicationType> { ApplicationType.MSSL, ApplicationType.EBS };
        #endregion
        public bool EnableSSO { get; set; }
        public string SNCNamePrefix { get; set; }
        public string SNCNameSufix { get; set; }
        public string SNCParameterName { get; set; }
        public string GetSNCParamValue(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName))
            {
                return string.Empty;
            }
            else
            {
                return $"{SNCNamePrefix}{accountName}{SNCNameSufix}";
            }
        }
        public SAPHANAConnector(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {

            Endpoint = endpoint;
            CurrentRequest = currentRequest;
            SetDefaultValues();

            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            _SAPentitlementDateStringFormat = MasterConfigHelper.SAPentitlementDateStringFormat;

            if (string.IsNullOrWhiteSpace(_SAPentitlementDateStringFormat))
                _SAPentitlementDateStringFormat = "yyyy.MM.dd";

        }
        bool ExtendPositionAsWell()
        {
            return positionApplicationTypes.Contains(Endpoint.ApplicationType);
        }
        void SetDefaultValues()
        {
            SAPConfiguration = EndpointConfiguration.GetByEndpointSystemId(Endpoint.Id);
            WriteProvisioningLog(LogLevelType.DebugStep, $"Endpoint System in : SetDefaultValues{Endpoint.Id}");
            sapUtil = new SAPHANAUtil();
            processorConfig = new SAPHANAProcessorConfig();

            sapUtil.AppServer = Config(EndpointKey.HostName, SAPConfiguration);
            sapUtil.SystemNumber = Config(EndpointKey.SystemNumber, SAPConfiguration);
            sapUtil.UserId = Config(EndpointKey.UserName, SAPConfiguration);
            sapUtil.Password = RSAHelper.DecryptData(Config(EndpointKey.Password, SAPConfiguration));
            sapUtil.Client = Config(EndpointKey.ClientNumber, SAPConfiguration);
            sapUtil.DestinationName = Endpoint.DisplayName.Replace(" ", "");
            sapUtil.RowCount = 5000;
            sapUtil.IsPaginationEnabled = Convert.ToBoolean(Config(EndpointKey.IsPaginationEnabled, SAPConfiguration)); //Added
            string sncMode = Config(EndpointKey.SNCMode, SAPConfiguration);
            if (!string.IsNullOrWhiteSpace(sncMode) && Convert.ToBoolean(sncMode) == true)
            {
                sapUtil.SNCMode = Convert.ToBoolean(sncMode);
                sapUtil.SNCLib = Config(EndpointKey.SNCLib, SAPConfiguration);
                sapUtil.SNCQop = Config(EndpointKey.SNCQop, SAPConfiguration);
                sapUtil.SNCPartnerName = Config(EndpointKey.SNCPartnerName, SAPConfiguration);
            }

            processorConfig.EndpointId = Endpoint.Id;
            processorConfig.Process = Process;
            processorConfig.sapUtil = sapUtil;
            processorConfig.insertThreshold = 1000;

            if (!IsFirstTimeDiscovery)
            {
                processorConfig.IsDelta = true;
                processorConfig.DeltaDate = Process.LastRunOn.Value;
            }
            //SSO User Setting
            var strEnableSSO = Config(EndpointKey.EnableSSO, SAPConfiguration);
            var strSNCPrefix = Config(EndpointKey.SNCNamePrefix, SAPConfiguration);
            var strSNCSufix = Config(EndpointKey.SNCNameSuffix, SAPConfiguration);
            var strSNCParamName = Config(EndpointKey.SNCParameterName, SAPConfiguration);

            if (!string.IsNullOrWhiteSpace(strEnableSSO) && Convert.ToBoolean(strEnableSSO) == true)
            {
                this.EnableSSO = Convert.ToBoolean(strEnableSSO);
                if (!string.IsNullOrWhiteSpace(strSNCPrefix))
                {
                    this.SNCNamePrefix = strSNCPrefix;
                }
                if (!string.IsNullOrWhiteSpace(strSNCSufix))
                {
                    this.SNCNameSufix = strSNCSufix;
                }
                if (!string.IsNullOrWhiteSpace(strSNCParamName))
                {
                    this.SNCParameterName = strSNCParamName;
                }
            }
        }

        public override bool Connect()
        {
            bool isSuccess = false;
            try
            {
                WriteProvisioningLog(LogLevelType.Info, $"Trying to connect to SAPHANA Server, AppServer: {processorConfig.sapUtil.AppServer}, " +
                    $"SystemNumber:  {processorConfig.sapUtil.SystemNumber}, ClientNumber:  {processorConfig.sapUtil.Client}, UserId:  {processorConfig.sapUtil.UserId}");

                var rfcDestination = processorConfig.sapUtil.GetSAPDestination();
                WriteProvisioningLog(LogLevelType.DebugStep, $"Trying to connect to SAPHANA Server: RFCDestination, AppServer:{rfcDestination?.AppServerHost}, " +
                  $"SystemNumber: {rfcDestination?.SystemNumber}, ClientNumber: {rfcDestination?.Client}");

                isSuccess = processorConfig.sapUtil.Connect();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format("Could not connect to SAPHANA Server - {0}",
                                                    processorConfig.sapUtil.AppServer), ex);
                if (ex != null && ex.Message != null && ex.Message.Contains("Function module"))
                {
                    RetryConnection();
                }
            }
            return isSuccess;
        }

        bool RetryConnection()
        {
            var connected = false;
            var retryTimeMilliSeconds = MasterConfigHelper.RetryTimeMilliSeconds;
            var timesShouldRetry = MasterConfigHelper.HowManyTimesShouldRetry;
            if (timesShouldRetry > 1)
            {
                var count = 1;
                do
                {
                    LogDebugStep($"Trying attempt to SAPHANA Connect: {count}");
                    try
                    {
                        connected = processorConfig.sapUtil.Connect();
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Error, $"Retry Exception", ex);
                    }
                    count++;
                    if (!connected && count <= timesShouldRetry)
                        Thread.Sleep(retryTimeMilliSeconds);
                }
                while (!connected && count <= timesShouldRetry);
            }
            return connected;
        }

        public override void Disconnect()
        {
            WriteProvisioningLog(LogLevelType.DebugStep, $"Disconnect() , Endpoint: {Endpoint.Id}");
            try
            {
                this.Endpoint = null;
                this.CurrentRequest = null;
                sapUtil = null;
                processorConfig.sapUtil = null;
                processorConfig = null;
                SAPConfiguration = null;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, $"Inner Exception : {ex.StackTrace}", ex);
            }
        }

        public override void GetData()
        {
            try
            {
                long discoveryProcessId = base.DiscoveryProcess.Id;
                processorConfig.Process = Process;

                List<EndpointObjects> EndpointObjects = DiscoveryProcess.GetByDiscoveryProcessId(discoveryProcessId);
                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_User) != null;
                bool IsRoleSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Role) != null;
                bool IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Group) != null;
                bool IsProfileSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Profile) != null;
                bool IsAuthorizationSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Authorization) != null;
                bool IsTCodeSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_TCode) != null;
                bool IsParametersSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Parameter) != null;
                bool IsPositionSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Position) != null;
                bool IsSectionMappingSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_SectionMapping) != null;
                bool isCriticalTCodeReport = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_CriticalTCodeReport) != null;
                bool isConflictingRoleReport = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_ConflictingRoleReport) != null;

                List<int> lstEntitlementTypesToBeConsidered = new List<int>();
                
                if (IsUserSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Users Disocvery Started for {0} ", this.Endpoint.DisplayName));
                    //First discover all user related objects i.e. Group, Profile, Role
                    var profileobj = new SAPHANAProfile(MasterConfigHelperServiceUser);
                    profileobj.ProcessorConfig = processorConfig;
                    profileobj.StartProcess();

                    var groupobj = new SAPHANAGroup(MasterConfigHelperServiceUser);
                    groupobj.ProcessorConfig = processorConfig;
                    groupobj.StartProcess();

                    var roleobj = new SAPHANARole(MasterConfigHelperServiceUser);
                    roleobj.ProcessorConfig = processorConfig;
                    roleobj.ProcessRoles();



                    var obj = new SAPHANAUser(MasterConfigHelperServiceUser);
                    obj.ProcessorConfig = processorConfig;
                    obj.SSOEnabled = this.EnableSSO;
                    obj.SNCParamName = this.SNCParameterName;
                    if (this.EnableSSO && string.IsNullOrWhiteSpace(this.SNCParameterName))
                    {
                        WriteDiscoveryLog(LogLevelType.Info, $"SSO enabled for Endpoint {this.Endpoint.DisplayName} but SNCParameterName is empty");
                    }
                    obj.StartProcess();

                    AccountReconcilation.ProcessDeletedAccounts(Endpoint.Id, Process, MasterConfigHelperServiceUser);
                    var listUsers = SAPHANAUserinfo.GetDeletedByEndPointSystemId(Endpoint.Id, Process);
                    SAPHANAUserinfo.RemoveSAPUser(listUsers);

                    UpdateEntitlementRecon();

                    // delete entitlements that have not been updated
                    //DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, "ProcessDeletedEntitlements - Started");
                    //EntitlementReconcilation.ProcessDeletedEntitlements(processorConfig.EndpointId, processorConfig.Process, MasterConfigHelper.ServiceUser);
                    //DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, "ProcessDeletedEntitlements - Completed");

                    SAPHANAUserinfo.MapWithMasterUserForReconTable(Endpoint, SAPConfiguration.MapAll != null ? SAPConfiguration.MapAll : false, discoveryProcessId);
                    listUsers?.Clear();
                    listUsers = null;
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Users Discovery Ended for {0} ", this.Endpoint.DisplayName));
                }

                if (IsRoleSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Roles Discovery Started for {0} ", this.Endpoint.DisplayName));
                    var obj = new SAPHANARole(MasterConfigHelperServiceUser);
                    obj.ProcessorConfig = processorConfig;

                    obj.StartProcess(IsUserSelected);

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Roles Discovery Ended for {0} ", this.Endpoint.DisplayName));
                }
                if (IsGroupSelected && !IsUserSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Groups Discovery Started for {0} ", this.Endpoint.DisplayName));
                    var obj = new SAPHANAGroup(MasterConfigHelperServiceUser);
                    obj.ProcessorConfig = processorConfig;

                    obj.StartProcess();

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Groups Discovery Ended for {0} ", this.Endpoint.DisplayName));
                }
                if (IsProfileSelected && !IsUserSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Profiles Discovery Started for {0} ", this.Endpoint.DisplayName));
                    var obj = new SAPHANAProfile(MasterConfigHelperServiceUser);
                    obj.ProcessorConfig = processorConfig;

                    obj.StartProcess();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Profiles Discovery Ended for {0} ", this.Endpoint.DisplayName));
                }
                if (IsParametersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Parameters Discovery Started for {0} ", this.Endpoint.DisplayName));

                    var obj = new SAPHANAParemeter(MasterConfigHelperServiceUser);
                    obj.ProcessorConfig = processorConfig;

                    obj.StartProcess();

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Parameters Discovery Ended for {0} ", this.Endpoint.DisplayName));
                }
                if (IsTCodeSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("TCode Discovery Started for {0} ", this.Endpoint.DisplayName));

                    var obj = new SAPHANATCode(MasterConfigHelperServiceUser);
                    obj.ProcessorConfig = processorConfig;

                    obj.StartProcess();

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("TCode Discovery Ended for {0} ", this.Endpoint.DisplayName));
                }
                if (IsAuthorizationSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Authorization Discovery Started for {0} ", this.Endpoint.DisplayName));

                    var Fieldobj = new SAPHANAField(MasterConfigHelperServiceUser);
                    Fieldobj.ProcessorConfig = processorConfig;

                    Fieldobj.StartProcess();

                    var obj = new SAPHANAAuthObject(MasterConfigHelperServiceUser);
                    obj.ProcessorConfig = processorConfig;

                    obj.StartProcess();

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Authorization Discovery Ended for {0} ", this.Endpoint.DisplayName));
                }

                LogDebugStep($"IsPositionSelected:{IsPositionSelected}");
                if (IsPositionSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Position Discovery Started for {0} ", this.Endpoint.DisplayName));

                    var obj = new SAPHANAPosition(MasterConfigHelperServiceUser);
                    obj.ProcessorConfig = processorConfig;

                    obj.StartProcess();

                    UpdateEntitlementReconForPosition();

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Position Discovery Ended for {0} ", this.Endpoint.DisplayName));
                }
                if (IsSectionMappingSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Section Mapping Discovery Started for {0} ", this.Endpoint.DisplayName));

                    new SAPHANASection(processorConfig, _idmsLogger).StartProcess();

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Section Mapping Disocvery Ended for {0} ", this.Endpoint.DisplayName));
                }

                //Add entitlement types that have to be considered during deletion of entitlements
                if (IsUserSelected)
                    lstEntitlementTypesToBeConsidered.Add(Convert.ToInt32(EntitlementType.Role));
                if (IsPositionSelected)
                    lstEntitlementTypesToBeConsidered.Add(Convert.ToInt32(EntitlementType.Position));

                //
                if (this.Endpoint.ApplicationType == ApplicationType.EBS || this.Endpoint.ApplicationType == ApplicationType.MSSL)
                {
                    if (isCriticalTCodeReport)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, "CriticalTCodeReport - Started");
                        EntitlementReconcilation.GetActiveAccountsForSAPHANACriticalReport(this.Endpoint.Id);
                        WriteDiscoveryLog(LogLevelType.Info, "CriticalTCodeReport - Completed");
                    }
                    if (isConflictingRoleReport)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, "SODConflictingReport - Started");
                        EntitlementReconcilation.GetActiveAccountsForSAPHANASODConflictingReport(this.Endpoint.Id);
                        WriteDiscoveryLog(LogLevelType.Info, "SODConflictingReport - Completed");
                    }
                }

                if (lstEntitlementTypesToBeConsidered != null && lstEntitlementTypesToBeConsidered.Count > 0)
                {

                    LogDebugStep("EntitlementReconcilation.ProcessDeletedEntitlements - Started");
                    EntitlementReconcilation.ProcessDeletedEntitlements(Endpoint.Id, Process, MasterConfigHelperServiceUser, lstEntitlementTypesToBeConsidered);
                    WriteDiscoveryLog(LogLevelType.Info, "UpdateEntitlementReconForPosition - Ended");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// For testing not for production
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        public IList<SAPHANARoleToAuthMap> GetRoleToTCode(string rolename, bool showDeleted = false)
        {
            var obj = new SAPHANARole(MasterConfigHelperServiceUser);
            obj.ProcessorConfig = processorConfig;
            return obj.ProcessRoleToAuthMap(rolename);
        }
        public override bool DeactivatePassword(string userName)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            try
            {
                if (!IsUserExists(userName))
                {
                    WriteProvisioningLog(LogLevelType.Error, $"{userName} - account does not exists.");
                    return false;
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", userName);

                IRfcFunction fnUserChange = processorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                IRfcStructure structLogondata = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATA");
                IRfcStructure structLogondataX = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATAX");


                structLogondata.SetValue("Codvn", "X");
                structLogondataX.SetValue("Codvn", "X");
                structLogondata.SetValue("Codvc", "X");
                structLogondataX.SetValue("Codvc", "X");

                fnUserChange.SetValue("LOGONDATA", structLogondata);
                fnUserChange.SetValue("LOGONDATAX", structLogondataX);

                var result = processorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                bool isUserUpdated = false;
                foreach (DataRow row in result.Rows)
                {
                    if (row["Message"].ToString().ToUpper().Contains(string.Format("User {0} has changed", userName).ToUpper()))
                    {
                        isUserUpdated = true;
                        break;
                    }
                }
                if (isUserUpdated)
                {
                    return true;
                }
                else
                {
                    foreach (DataRow row in result.Rows)
                    {
                        WriteProvisioningLog(LogLevelType.Error, row["Message"].ToString(), null);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while deactivating the passsword for the account - {userName}.", ex);
                return false;
            }
        }

        public override bool ResetPassword(string UserName, string NewPassword, string OldPassword)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = IsUserExists(UserName);

                if (exists == false)
                {
                    return false;
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserName);

                IRfcFunction fnUserChange = processorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                IRfcStructure structPwd = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "PASSWORD");
                IRfcStructure structPwdX = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "PASSWORDX");

                structPwd.SetValue("Bapipwd", NewPassword);
                structPwdX.SetValue("Bapipwd", "X");

                fnUserChange.SetValue("PASSWORD", structPwd);
                fnUserChange.SetValue("PASSWORDX", structPwdX);

                var result = processorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                if (result.Rows.Count <= 0)
                    return false;


                if (result.Rows[0]["TYPE"].ToString() != "S")
                    return false;
                //unlocking account
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserName);

                IRfcFunction fnUserDel = processorConfig.sapUtil.getRfcFunction("BAPI_USER_UNLOCK", dicParams);

                result = processorConfig.sapUtil.ExecuteTable(fnUserDel, "RETURN");

                if (result.Rows.Count <= 0)
                    return false;


                if (result.Rows[0]["TYPE"].ToString() != "S")
                    return false;
            }


            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }


        private bool ChangePasswordForReactivation(string UserName, string Password, long DIUserId)
        {

            SAPUserDetailsComponent.BAPIRET2 Sapusrcomp = new SAPUserDetailsComponent.BAPIRET2();
            byte[] newByte = new byte[1];


            try
            {

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();
                    #region SAP BAPI CLASSES

                    BAPIADDR3 address = new BAPIADDR3();
                    BAPIUSCOMP companyName = new BAPIUSCOMP();
                    BAPIALIAS alias = new BAPIALIAS();
                    BAPIDEFAUL defaults = new BAPIDEFAUL();
                    BAPISLOCKD islocked = new BAPISLOCKD();
                    BAPILOGOND logonData = new BAPILOGOND();
                    BAPIPWD passworddata = new BAPIPWD();
                    BAPIMODDAT lastmofified = new BAPIMODDAT();
                    BAPIREFUS refuser = new BAPIREFUS();
                    BAPISNCU snc = new BAPISNCU();
                    BAPIUCLASS iuclass = new BAPIUCLASS();
                    BAPIAGRTable activeGroups = new BAPIAGRTable();
                    BAPICOMREMTable addcomrem = new BAPICOMREMTable();
                    BAPIADFAXTable addfax = new BAPIADFAXTable();
                    BAPIADPAGTable addpag = new BAPIADPAGTable();
                    BAPIADPRTTable addprt = new BAPIADPRTTable();
                    BAPIADRFCTable addrfc = new BAPIADRFCTable();
                    BAPIADRMLTable addrml = new BAPIADRMLTable();
                    BAPIADSMTPTable addsmtp = new BAPIADSMTPTable();
                    BAPIADSSFTable addssf = new BAPIADSSFTable();
                    BAPIADTELTable addtel = new BAPIADTELTable();
                    BAPIADTLXTable addtlx = new BAPIADTLXTable();
                    BAPIADTTXTable addttx = new BAPIADTTXTable();
                    BAPIADURITable adduri = new BAPIADURITable();
                    BAPIADX400Table addx400 = new BAPIADX400Table();
                    BAPIUSEXTIDHEADTable exithead = new BAPIUSEXTIDHEADTable();
                    BAPIUSEXTIDPARTTable extidpart = new BAPIUSEXTIDPARTTable();

                    BAPIGROUPSTable groups = new BAPIGROUPSTable();
                    BAPIPARAMTable parameter = new BAPIPARAMTable();
                    BAPIPARAM1Table parameter1 = new BAPIPARAM1Table();
                    BAPIRCVSYSTable systems = new BAPIRCVSYSTable();
                    BAPIPROFTable profiles = new BAPIPROFTable();
                    BAPIUCLASSSYSTable uclClassSys = new BAPIUCLASSSYSTable();
                    SAPUserDetailsComponent.BAPIRET2Table return0 = new SAPUserDetailsComponent.BAPIRET2Table();
                    BAPIUSSEXPTable sextable = new BAPIUSSEXPTable();
                    BAPIUSSRGETable gettable = new BAPIUSSRGETable();
                    BAPIUSNAMETable userNametable = new BAPIUSNAMETable();

                    AGR_TCODESTable TCodes = new AGR_TCODESTable();
                    AGR_SHIER_BORTable SHIERBOR = new AGR_SHIER_BORTable();
                    AGR_HIERTable HeirTable = new AGR_HIERTable();
                    AGR_BUFFITable BuffitTable = new AGR_BUFFITable();
                    AGR_HIERTTable HeirTTable = new AGR_HIERTTable();

                    BAPILOGONX BAPILOGONX = new BAPILOGONX();
                    BAPIPWDX BAPIPWDX = new BAPIPWDX();
                    BAPIADCOMX BAPIADCOMX = new BAPIADCOMX();
                    BAPIADDR3X BAPIADDR3X = new BAPIADDR3X();
                    BAPIALIASX BAPIALIASX = new BAPIALIASX();
                    BAPIFLAG BAPIFLAG = new BAPIFLAG();
                    BAPIUSCOMX BAPIUSCOMX = new BAPIUSCOMX();
                    BAPIDEFAX BAPIDEFAX = new BAPIDEFAX();
                    BAPIUSEXTIDX BAPIUSEXTIDX = new BAPIUSEXTIDX();
                    BAPIGROUPX BAPIGROUPX = new BAPIGROUPX();
                    BAPIPARAMX BAPIPARAMX = new BAPIPARAMX();
                    BAPIREFUSX BAPIREFUSX = new BAPIREFUSX();
                    BAPISNCUX BAPISNCUX = new BAPISNCUX();
                    BAPIUCLASSX BAPIUCLASSX = new BAPIUCLASSX();
                    BAPIUCLASSSYSTable BAPIUCLASSSYSTable = new BAPIUCLASSSYSTable();

                    SAPUserDetailsComponent.BAPIRET2 return2 = new SAPUserDetailsComponent.BAPIRET2();

                    #endregion
                    passworddata.Bapipwd = Password;
                    BAPIPWDX.Bapipwd = Password;


                    proxy.Bapi_User_Change(BAPIADCOMX, address, BAPIADDR3X, alias, BAPIALIASX, BAPIFLAG, companyName, BAPIUSCOMX, defaults, BAPIDEFAX, BAPIUSEXTIDX, BAPIGROUPX, logonData, BAPILOGONX, BAPIPARAMX, passworddata, BAPIPWDX, "", refuser, BAPIREFUSX, snc, BAPISNCUX, iuclass, BAPIUCLASSX, UserName, ref addcomrem,
                                      ref addfax, ref addpag, ref addprt, ref addrfc, ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx, ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter, ref parameter1, ref return0, ref BAPIUCLASSSYSTable);

                    string UserStatus = return0.ToADODataTable().Rows[0]["Message"].ToString().ToUpper();
                    string MessageType = return0.ToADODataTable().Rows[0]["Type"].ToString();
                    if (UserStatus.Contains(string.Format("User {0} has changed", UserName).ToUpper()))
                    {
                        SAPHANAUserinfo.UpdatePasswordresetcount(this.Endpoint.Id, UserName, "System");
                        return true;
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Exception, $" Failed in setting Password in ChangePasswordForReactivation for the user: {UserName}");
                        return false;
                    }
                }
            }

            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $" Exception in ChangePasswordForReactivation for the user: {UserName}");
                return false;
            }
        }

        /// <summary>
        /// Productive_PWD required additional authorization. Thats the reason we always reset with newPassword generated by system and then change the password which is entered by User.
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="NewPassword"></param>
        /// <param name="DIUserId"></param>
        /// <param name="isReactivation"></param>
        /// <returns></returns>
        public override bool ChangePassword(string UserName, string NewPassword, long DIUserId, bool isReactivation)
        {
            //if (!isReactivation) // Call reset password to force the user change the password on next login.
            //{
            //    return ResetPassword(UserName, NewPassword, string.Empty);
            //}

            IsResetPasswordSuccessfull = false;
            bool ispasswordChanged = false;
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            var tmpPwd = Constants.MSSL_EBS_DEFAULT_PASSWORD;
            bool isSuccess=false;
            try
            {
                tmpPwd = ConstructPassword(this.Endpoint.Id);
                while (tmpPwd == NewPassword)
                {
                    tmpPwd = ConstructPassword(this.Endpoint.Id);
                }
                _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword for User:{UserName} started.");

                var exists = IsUserExists(UserName);

                if (exists == false)
                {
                    UnLockChangePasswordAPIMessage = $"User: {UserName} does not exists.";
                    _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword IsUserExists {exists}.");
                    return false;
                }

                // reset password
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserName);

                IRfcFunction fnUserChange = processorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                IRfcStructure structPwd = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "PASSWORD");
                IRfcStructure structPwdX = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "PASSWORDX");

                structPwd.SetValue("Bapipwd", tmpPwd);
                structPwdX.SetValue("Bapipwd", "X");

                fnUserChange.SetValue("PASSWORD", structPwd);
                fnUserChange.SetValue("PASSWORDX", structPwdX);

                var result = processorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                if (result.Rows.Count <= 0)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword BAPI_USER_CHANGE return 0.");
                    return false;
                }
                
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow temp in result.Rows)
                    {
                        if (temp["TYPE"].ToString() == "S")
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }


                if (isSuccess==false)
                {
                    try
                    {
                        UnLockChangePasswordAPIMessage = result.Rows[0]["MESSAGE"]?.ToString();
                        _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword BAPI_USER_CHANGE return {result.Rows[0]["TYPE"]} MESSAGE:{result.Rows[0]["MESSAGE"]}.");
                    }
                    catch
                    {
                    }
                    return false;
                }
                IsResetPasswordSuccessfull = true;
                Thread.Sleep(5000);
                //unlocking account
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserName);

                IRfcFunction fnUserDel = processorConfig.sapUtil.getRfcFunction("BAPI_USER_UNLOCK", dicParams);

                result = processorConfig.sapUtil.ExecuteTable(fnUserDel, "RETURN");

                if (result.Rows.Count <= 0)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword BAPI_USER_UNLOCK return 0.");
                    return false;
                }
                
                if (result.Rows.Count > 0)
                {
                    foreach (DataRow temp in result.Rows)
                    {
                        if (temp["TYPE"].ToString() == "S")
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }

                if (isSuccess == false)
                {
                    try
                    {
                        UnLockChangePasswordAPIMessage = result.Rows[0]["MESSAGE"]?.ToString();
                        _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword BAPI_USER_UNLOCK return {result.Rows[0]["TYPE"]} MESSAGE:{result.Rows[0]["MESSAGE"]}.");
                    }
                    catch
                    {
                    }
                    return false;
                }
                Thread.Sleep(5000);
                // change password
                dicParams = new Dictionary<string, object>();
                dicParams.Add("bname", UserName);
                dicParams.Add("password", tmpPwd);
                dicParams.Add("new_password", NewPassword);

                IRfcFunction fnUserChangePwd = processorConfig.sapUtil.getRfcFunction("SUSR_USER_CHANGE_PASSWORD_RFC", dicParams);
                //Setting false because system already changed the user password. So no need to send temp password;
                ispasswordChanged = true;
                Thread.Sleep(5000);
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserName);

                IRfcFunction fnUserUnlock = processorConfig.sapUtil.getRfcFunction("BAPI_USER_UNLOCK", dicParams);

                result = processorConfig.sapUtil.ExecuteTable(fnUserUnlock, "RETURN");

                if (result.Rows.Count <= 0)
                {
                    UnLockChangePasswordAPIMessage = $"User: {UserName} SAPHANAUnlock account return no result. Please contact IAM Team.";
                    _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword BAPI_USER_UNLOCK return 0.");
                    return false;
                }

                if (result.Rows.Count > 0)
                {
                    foreach (DataRow temp in result.Rows)
                    {
                        if (temp["TYPE"].ToString() == "S")
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }

                if (isSuccess == false)
                {
                    try
                    {
                        UnLockChangePasswordAPIMessage = result.Rows[0]["MESSAGE"]?.ToString();
                        _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword BAPI_USER_UNLOCK return {result.Rows[0]["TYPE"]} MESSAGE:{result.Rows[0]["MESSAGE"]}.");
                    }
                    catch
                    {
                    }
                    return false;
                }

                
            }
            catch (SAP.Middleware.Connector.RfcAbapException rex)
            {
                string errMsg = string.Empty;

                if (rex.AbapMessageClass == "00" && rex.AbapMessageNumber == "193")
                {
                    UnLockChangePasswordAPIMessage = DI.Chakra.Infrastructure.Constants.SAPPasswordResetErrorMessage;
                    errMsg = "SAPABAPError-Choose a password that is different from the last 5 passwords";

                }
                else if (rex.AbapMessageClass == "00" && rex.AbapMessageNumber == "186")
                {

                    errMsg = "SAPABAPError-New password cannot begin with '?' or '!'";
                    UnLockChangePasswordAPIMessage = "New password cannot begin with '?' or '!'.";
                }
                else
                {
                    errMsg = String.Format("SAPABAPError-{0}. MessageClass: {1} MessageNumber: {2}", rex.Key, rex.AbapMessageClass, rex.AbapMessageNumber);
                    UnLockChangePasswordAPIMessage = errMsg;
                }
                if (IsThroughPasswordReset && ispasswordChanged == false)
                {
                    if (IsResetPasswordSuccessfull)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Exception, $"Sending temporary password to the user:{UserName}.", null);
                        AccountReconcilation.SendEmailForPasswordResetFailure(DIUserId, this.Endpoint.DisplayName, tmpPwd);
                    }
                }

                _idmsLogger.WriteLog(LogLevelType.Exception, errMsg, null);
                throw new Exception(errMsg, rex);
            }
            catch (Exception ex)
            {
                if (IsThroughPasswordReset && ispasswordChanged == false)
                {
                    if (IsResetPasswordSuccessfull)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Exception, $"Sending temporary password to the user:{UserName}.", null);
                        AccountReconcilation.SendEmailForPasswordResetFailure(DIUserId, this.Endpoint.DisplayName, tmpPwd);
                    }
                }
                _idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                throw ex;
            }
            _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword for User:{UserName} successfully completed.");
            return true;
        }

        public override bool UnLockAccount(IEndpointUserInfo _userInfo)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"UnlockAccount for User:{_userInfo.AccountName} started.");
                string accountName = _userInfo.AccountName;
                var exists = IsUserExists(accountName);

                if (exists == false)
                {
                    return false;
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", accountName);

                IRfcFunction fnUserDel = processorConfig.sapUtil.getRfcFunction("BAPI_USER_UNLOCK", dicParams);

                var result = processorConfig.sapUtil.ExecuteTable(fnUserDel, "RETURN");

                if (result.Rows.Count <= 0)
                    return false;

                if (result.Rows[0]["TYPE"].ToString() != "S")
                {
                    try
                    {
                        UnLockChangePasswordAPIMessage = result.Rows[0]["MESSAGE"]?.ToString();
                    }
                    catch (Exception)
                    {
                    }
                    return false;
                }
                base.UnLockAccount(_userInfo);
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Unlock Account failed for User:{_userInfo.AccountName}.Message: {ex.Message}", ex);
                throw ex;
            }
            _idmsLogger.WriteLog(LogLevelType.Info, $"UnlockAccount for User:{_userInfo.AccountName} successfully completed.");
            return true;
        }

        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {
                var _SAPUserInfo = (SAPHANAUserinfo)_userInfo;
                string UserLogonName = _SAPUserInfo.UserLogonName;

                if (IsUserExists(UserLogonName))
                {
                    WriteProvisioningLog(LogLevelType.Error, $"{UserLogonName} - Account already exists.");
                    return true;
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserLogonName);

                IRfcFunction fnCreate = processorConfig.sapUtil.getRfcFunction("BAPI_USER_CREATE", dicParams);
                IRfcStructure structPassword = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "PASSWORD");
                IRfcStructure structLogon = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "LOGONDATA");
                IRfcStructure structAddress = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "ADDRESS");
                IRfcStructure structDefaults = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "DEFAULTS");
                IRfcTable tblParameters = processorConfig.sapUtil.getRfcTable("BAPI_USER_CREATE", dicParams, "PARAMETER");
                IRfcStructure structLicense = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "UCLASS");
                structPassword.SetValue("BAPIPWD", _SAPUserInfo.Password);
                fnCreate.SetValue("PASSWORD", structPassword);

                if (!string.IsNullOrEmpty(_SAPUserInfo.Lastname))
                {
                    structAddress.SetValue("Lastname", _SAPUserInfo.Lastname);
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.Title))
                {
                    structAddress.SetValue("Title", _SAPUserInfo.Title);
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.Firstname))
                {
                    structAddress.SetValue("Firstname", _SAPUserInfo.Firstname);
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.Function0))
                {
                    structAddress.SetValue("Function", _SAPUserInfo.Function0);
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.Department))
                {
                    structAddress.SetValue("Department", _SAPUserInfo.Department);
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.E_Mail))
                {
                    structAddress.SetValue("E_Mail", _SAPUserInfo.E_Mail);
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.Tel1_Numbr))
                {
                    structAddress.SetValue("Tel1_Numbr", _SAPUserInfo.Tel1_Numbr);
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.Pers_No))
                {
                    structAddress.SetValue("Pers_No", _SAPUserInfo.Pers_No);
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.City))
                {
                    structAddress.SetValue("City", _SAPUserInfo.City);
                }

                if (!string.IsNullOrEmpty(fields["FormatCountry"].Value))
                {
                    structAddress.SetValue("Langu_P", "E");
                    structAddress.SetValue("Namcountry", fields["FormatCountry"].Value);
                }

                fnCreate.SetValue("ADDRESS", structAddress);

                //NOTE: Set DateFormat field to value 1 for DD.MM.YYYY
                //NOTE: Set TimeFormat field to value 0 for 12 Hour Format(Example: 12:05:10 PM)
                // NewUser_DecimalNotation to 'X' for 1,234,567.89

                if (!string.IsNullOrEmpty(_SAPUserInfo.CostCenter))
                {
                    structDefaults.SetValue("Kostl", _SAPUserInfo.CostCenter);
                }

                if (!string.IsNullOrEmpty(fields["DateFormat"].Value))
                {
                    structDefaults.SetValue("Datfm", fields["DateFormat"].Value);
                }
                if (!string.IsNullOrEmpty(fields["TimeFormat"].Value))
                {
                    structDefaults.SetValue("Timefm", fields["TimeFormat"].Value);
                }

                if (!string.IsNullOrEmpty(fields["NewUser_DecimalNotation"].Value))
                {
                    structDefaults.SetValue("DCPFM", fields["NewUser_DecimalNotation"].Value);
                }

                if (!string.IsNullOrEmpty(fields["OutputDevice"].Value))
                {
                    structDefaults.SetValue("Spld", fields["OutputDevice"].Value);
                }

                fnCreate.SetValue("DEFAULTS", structDefaults);

                if (!string.IsNullOrEmpty(fields["TimeZone"].Value))
                {
                    structLogon.SetValue("Tzone", fields["TimeZone"].Value);
                }

                DateTime validFrom = UtilityMethods.GetDateFromSAPDateString(_SAPUserInfo.UserValidFrom);
                structLogon.SetValue("Gltgv", validFrom != DateTime.MinValue ? validFrom : DateTime.Today);

                DateTime validTo = UtilityMethods.GetDateFromSAPDateString(_SAPUserInfo.UserValidTo);
                structLogon.SetValue("Gltgb", validTo != DateTime.MinValue ? validTo : DateTime.MaxValue);

                if (_SAPUserInfo.UserType == SAPUserType.Communication)
                {
                    structLogon.SetValue("Ustyp", "C");
                }
                else if (_SAPUserInfo.UserType == SAPUserType.Service)
                {
                    structLogon.SetValue("Ustyp", "S");
                }
                else if (_SAPUserInfo.UserType == SAPUserType.System)
                {
                    structLogon.SetValue("Ustyp", "B");
                }
                else if (_SAPUserInfo.UserType == SAPUserType.Reference)
                {
                    structLogon.SetValue("Ustyp", "L");
                }
                else
                {
                    structLogon.SetValue("Ustyp", "A");
                }

                if (_SAPUserInfo.Groups != null && _SAPUserInfo.Groups.Count != 0)
                    structLogon.SetValue("Class", _SAPUserInfo.Groups[0]);


                fnCreate.SetValue("LOGONDATA", structLogon);

                //parameters

                foreach (KeyValuePair<string, string> Param in _SAPUserInfo.Parameters)
                {
                    tblParameters.Append();
                    tblParameters.SetValue("Parid", Param.Key);
                    tblParameters.SetValue("Parva", Param.Value);
                }

                fnCreate.SetValue("PARAMETER", tblParameters);

                #region SSO Update 
                IRfcStructure structSNCFORSSO = null;
                if (this.EnableSSO && this.CurrentRequest != null && this.CurrentRequest.AccountType == AccountType.User)
                {
                    try
                    {
                        bool isAllParamExists = !string.IsNullOrWhiteSpace(SNCNamePrefix) && !string.IsNullOrWhiteSpace(SNCNameSufix) && !string.IsNullOrWhiteSpace(SNCParameterName);
                        if (isAllParamExists)
                        {
                            var sncValue = GetSNCParamValue(_SAPUserInfo.UserSamAccountName);
                            if (string.IsNullOrWhiteSpace(_SAPUserInfo.UserSamAccountName))
                            {
                                WriteProvisioningLog(LogLevelType.Info, $"Enable SSO. User ADID is empty for user:{UserLogonName}");
                            }
                            WriteProvisioningLog(LogLevelType.Warning, $"SNCNamePrefix: {SNCNamePrefix} User ADID: {_SAPUserInfo.UserSamAccountName} SNCNameSufix: {SNCNameSufix} SNCParameterName: {SNCParameterName} format SNC:{sncValue}.");
                            structSNCFORSSO = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "SNC");
                            structSNCFORSSO.SetValue(SNCParameterName, sncValue);
                            fnCreate.SetValue("SNC", structSNCFORSSO);
                        }
                        else
                        {
                            WriteProvisioningLog(LogLevelType.Warning, $"Unable to update the SNC for SSO. Endpoint configuration is missing. SNCNamePrefix: {SNCNamePrefix} SNCNameSufix: {SNCNameSufix} SNCParameterName: {SNCParameterName}.Please update EndpointConfiguration.");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteProvisioningLog(LogLevelType.Exception, $"Error while fetching BAPI_USER_CREATE SNC.Message:{ex.Message}", ex);
                    }
                }
                #endregion

                var createResult = processorConfig.sapUtil.ExecuteTable(fnCreate, "RETURN");

                if (createResult.Rows.Count <= 0)
                {
                    WriteProvisioningLog(LogLevelType.Error, "Function return null.");
                    return false;
                }

                bool success = false;
                string errorMessage = string.Empty;

                foreach (DataRow row in createResult.Rows)
                {
                    if (row["Message"] != null)
                    {
                        var msg = "User " + _SAPUserInfo.UserLogonName + " created";
                        WriteProvisioningLog(LogLevelType.DebugStep, string.Format("Received:{0}   Constructed:{1}", row["Message"].ToString(), msg));
                        if (row["Message"].ToString().Equals(msg, StringComparison.InvariantCultureIgnoreCase))
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"Account Creation - Status return from BAPI (Success) is : {row["Message"].ToString()}");
                            success = true;
                            break;
                        }
                    }
                }

                if (!success)
                {
                    WriteProvisioningLog(LogLevelType.Exception, $"Account Creation: Status return from BAPI (Not Success)");

                    foreach (DataRow row in createResult.Rows)
                    {
                        if (row["Message"] != null)
                        {
                            WriteProvisioningLog(LogLevelType.Error, "Return Msg ->" + row["Message"].ToString());
                        }
                    }
                    return false;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Info, $"{UserLogonName} - Account created successfully.");
                    AddProfile(fields, UserLogonName);
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, "Error while creating account.", ex);
                throw ex;
            }
            return true;
        }

        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            var _SAPUserInfo = (SAPHANAUserinfo)_userInfo;
            string UserLogonName = _SAPUserInfo.UserLogonName;

            try
            {
                if (!IsUserExists(UserLogonName))
                {
                    WriteProvisioningLog(LogLevelType.Error, $"{UserLogonName} - account does not exist.");
                    return false;
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserLogonName);

                IRfcFunction fnUserChange = processorConfig.sapUtil.getRfcFunction("ZCE_USER_CHANGE_ATTR", dicParams);
                IRfcStructure structAddress = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "ADDRESS");
                IRfcStructure structAddressX = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "ADDRESSX");
                IRfcStructure structDefaults = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "DEFAULTS");
                IRfcStructure structDefaultsX = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "DEFAULTSX");
                IRfcStructure structLogondata = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "LOGONDATA");
                IRfcStructure structLogondataX = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "LOGONDATAX");

                if (!string.IsNullOrEmpty(_SAPUserInfo.Lastname))
                {
                    structAddress.SetValue("Lastname", _SAPUserInfo.Lastname);
                    structAddressX.SetValue("Lastname", "X");
                }
                if (!string.IsNullOrEmpty(_SAPUserInfo.Firstname))
                {
                    structAddress.SetValue("Firstname", _SAPUserInfo.Firstname);
                    structAddressX.SetValue("Firstname", "X");
                }
                //3202 fixed
                //if (!string.IsNullOrEmpty(_SAPUserInfo.E_Mail))
                {
                    structAddress.SetValue("E_Mail", _SAPUserInfo.E_Mail);
                    structAddressX.SetValue("E_Mail", "X");
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.Tel1_Numbr))
                {
                    structAddress.SetValue("Tel1_Numbr", _SAPUserInfo.Tel1_Numbr);
                    structAddressX.SetValue("Tel1_Numbr", "X");
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.Function0))
                {
                    structAddress.SetValue("Function", _SAPUserInfo.Function0);
                    structAddressX.SetValue("Function", "X");
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.Department))
                {
                    structAddress.SetValue("Department", _SAPUserInfo.Department);
                    structAddressX.SetValue("Department", "X");
                }

                fnUserChange.SetValue("ADDRESS", structAddress);
                fnUserChange.SetValue("ADDRESSX", structAddressX);

                if (!string.IsNullOrEmpty(_SAPUserInfo.CostCenter))
                {
                    structDefaults.SetValue("Kostl", _SAPUserInfo.CostCenter);
                    structDefaultsX.SetValue("Kostl", "X");
                }

                fnUserChange.SetValue("DEFAULTS", structDefaults);
                fnUserChange.SetValue("DEFAULTSX", structDefaultsX);

                var logonInfo = GetLogonInfoFromSAP(_SAPUserInfo.UserLogonName);

                DateTime validTo = UtilityMethods.GetDateFromSAPDateString(_SAPUserInfo.UserValidTo);
                DateTime validFrom = UtilityMethods.GetDateFromSAPDateString(_SAPUserInfo.UserValidFrom);

                WriteProvisioningLog(LogLevelType.Info, $"validTo - {validTo},validFrom - {validFrom}");

                if (validFrom != DateTime.MinValue)
                {
                    structLogondata.SetValue("Gltgv", validFrom);
                    structLogondataX.SetValue("Gltgv", "X");
                }
                else
                {
                    if (logonInfo != null)
                    {
                        DateTime? tmpFrom = processorConfig.sapUtil.getDateTime(Convert.ToString(logonInfo.GetValue("Gltgv")));
                        if (tmpFrom.HasValue)
                            validFrom = tmpFrom.Value;

                        if (validFrom != DateTime.MinValue)
                        {
                            structLogondata.SetValue("Gltgv", validFrom);
                            structLogondataX.SetValue("Gltgv", "X");
                        }
                    }
                }

                if (validTo != DateTime.MinValue)
                {
                    if (validTo < validFrom)
                        validTo = validFrom.AddDays(1).AddSeconds(-1);

                    structLogondata.SetValue("Gltgb", validTo);
                    structLogondataX.SetValue("Gltgb", "X");
                }
                else
                {
                    if (logonInfo != null)
                    {
                        DateTime? tmpTo = processorConfig.sapUtil.getDateTime(Convert.ToString(logonInfo.GetValue("Gltgb")));
                        if (tmpTo.HasValue)
                            validTo = tmpTo.Value;

                        if (validTo != DateTime.MinValue)
                        {
                            structLogondata.SetValue("Gltgb", validTo);
                            structLogondataX.SetValue("Gltgb", "X");
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(_SAPUserInfo.SapUserType))
                {
                    structLogondata.SetValue("Ustyp", _SAPUserInfo.SapUserType);
                    structLogondataX.SetValue("Ustyp", "X");
                }
                if (_SAPUserInfo.Groups != null && _SAPUserInfo.Groups.Count != 0)
                {
                    structLogondata.SetValue("Class", _SAPUserInfo.Groups[0]);
                    structLogondataX.SetValue("Class", "X");
                }

                fnUserChange.SetValue("LOGONDATA", structLogondata);
                fnUserChange.SetValue("LOGONDATAX", structLogondataX);

                var result = processorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                bool isUserUpdated = false;
                foreach (DataRow row in result.Rows)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"{row["Message"]?.ToString()}", null);
                    if (row["Message"].ToString().ToUpper().Contains(string.Format("User {0} has changed", _SAPUserInfo.UserLogonName).ToUpper()) ||
                           row["Message"].ToString().ToUpper().Contains(string.Format("No changes were made to user {0}", _SAPUserInfo.UserLogonName).ToUpper()))
                    {
                        isUserUpdated = true;
                        break;
                    }
                }

                if (isUserUpdated)
                {
                    WriteProvisioningLog(LogLevelType.Info, $" Account - {UserLogonName} updated successfully.");
                    return true;
                }
                else
                {
                    foreach (DataRow row in result.Rows)
                    {
                        WriteProvisioningLog(LogLevelType.Error, row["Message"].ToString(), null);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while updating the account - {UserLogonName}.", ex);
                throw ex;
            }
        }

        public override bool LockAccount(IEndpointUserInfo _userInfo)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            SAPHANAUserinfo _SAPUserInfo = (SAPHANAUserinfo)_userInfo;
            string UserLogonName = _SAPUserInfo.UserLogonName;

            try
            {
                if (!IsUserExists(UserLogonName))
                {
                    WriteProvisioningLog(LogLevelType.Error, $"{UserLogonName} - account does not exists.");
                    return false;
                }

                WriteProvisioningLog(LogLevelType.Info, $"{UserLogonName} - Locking account Started.");

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserLogonName);

                IRfcFunction fnUserDel = processorConfig.sapUtil.getRfcFunction("BAPI_USER_LOCK", dicParams);

                var result = processorConfig.sapUtil.ExecuteTable(fnUserDel, "RETURN");

                if (result.Rows.Count <= 0)
                    return false;

                if (result.Rows[0]["TYPE"].ToString() != "S")
                    return false;

                base.LockAccount(_userInfo);
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while locking the account - {UserLogonName}.", ex);
                return false;
            }

            return true;
        }

        protected override bool LockAccountRecon(IEndpointUserInfo _userInfo)
        {
            try
            {
                var acctRecon = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id);
                if (null == acctRecon)
                    return false;
                else
                {
                    var sapuserInfo = (SAPHANAUserinfo)_userInfo;

                    //if (sapuserInfo != null)
                    acctRecon.ValidToDate = this.CurrentRequest.DeactivationDate.HasValue ? this.CurrentRequest.DeactivationDate.Value : DateTime.Now; // sapuserInfo.validToDate;
                    acctRecon.DeletedDate = DateTime.Now;
                    acctRecon.IsLocked = true;
                    acctRecon.IsDeleted = true;
                    acctRecon.AccessRequestId = this.CurrentRequest?.Id;
                    acctRecon.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while locking the account - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }

            return true;
        }


        protected override bool UnLockAccountRecon(IEndpointUserInfo _userInfo)
        {
            try
            {
                var acctRecon = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id);
                if (null == acctRecon)
                    return false;
                else
                {
                    var sapuserInfo = _userInfo as SAPHANAUserinfo;
                    if (this.CurrentRequest != null)
                    {
                        var user = DI.Chakra.DomainModel.User.GetById(this.CurrentRequest.UserId);
                        acctRecon.UserId = this.CurrentRequest.UserId;
                        if (sapuserInfo != null)
                            sapuserInfo.Function0 = user.EmployeeId;
                    }

                    if (sapuserInfo != null)
                    {
                        sapuserInfo.IsUserLocked = false;
                    }

                    acctRecon.IsDeleted = false;
                    acctRecon.IsLocked = false;
                    acctRecon.FullName = !string.IsNullOrWhiteSpace(_userInfo.DisplayName) ? _userInfo.DisplayName : _userInfo.AccountName;
                    if (CurrentRequest != null)
                    {
                        acctRecon.ValidFromDate = CurrentRequest.ActivationDate;
                        acctRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                        acctRecon.DeletedDate = null;
                    }
                    acctRecon.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while unlocking the account - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }

            return true;
        }

        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            SAPHANAUserinfo _SAPUserInfo = (SAPHANAUserinfo)_userInfo;
            string UserLogonName = _SAPUserInfo.UserLogonName;

            try
            {
                if (!IsUserExists(UserLogonName))
                {
                    WriteProvisioningLog(LogLevelType.Error, $"{UserLogonName} - account does not exists.");
                    return false;
                }

                WriteProvisioningLog(LogLevelType.Info, $"{UserLogonName} - Started updating user details.");
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserLogonName);

                IRfcFunction fnUserChange = processorConfig.sapUtil.getRfcFunction("ZCE_USER_CHANGE_ATTR", dicParams);
                IRfcStructure structAddress = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "ADDRESS");
                IRfcStructure structAddressX = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "ADDRESSX");
                IRfcStructure structDefaults = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "DEFAULTS");
                IRfcStructure structDefaultsX = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "DEFAULTSX");
                IRfcStructure structLogondata = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "LOGONDATA");
                IRfcStructure structLogondataX = processorConfig.sapUtil.getRfcStructure("ZCE_USER_CHANGE_ATTR", dicParams, "LOGONDATAX");


                if (!string.IsNullOrEmpty(_SAPUserInfo.Function0))
                {
                    structAddress.SetValue("Function", _SAPUserInfo.Function0);
                    structAddressX.SetValue("Function", "X");
                }

                if (!string.IsNullOrEmpty(_SAPUserInfo.Department))
                {
                    structAddress.SetValue("Department", _SAPUserInfo.Department);
                    structAddressX.SetValue("Department", "X");
                }

                fnUserChange.SetValue("ADDRESS", structAddress);
                fnUserChange.SetValue("ADDRESSX", structAddressX);

                if (!string.IsNullOrEmpty(_SAPUserInfo.CostCenter))
                {
                    structDefaults.SetValue("Kostl", _SAPUserInfo.CostCenter);
                    structDefaultsX.SetValue("Kostl", "X");
                }

                fnUserChange.SetValue("DEFAULTS", structDefaults);
                fnUserChange.SetValue("DEFAULTSX", structDefaultsX);

                var logonInfo = GetLogonInfoFromSAP(_SAPUserInfo.UserLogonName);

                DateTime validTo = UtilityMethods.GetDateFromSAPDateString(_SAPUserInfo.UserValidTo);
                DateTime validFrom = UtilityMethods.GetDateFromSAPDateString(_SAPUserInfo.UserValidFrom);

                WriteProvisioningLog(LogLevelType.Info, $"validTo - {validTo},validFrom - {validFrom}");

                if (validFrom != DateTime.MinValue)
                {
                    structLogondata.SetValue("Gltgv", validFrom);
                    structLogondataX.SetValue("Gltgv", "X");
                }
                else
                {
                    if (logonInfo != null)
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"validFrom-Gltgv - {processorConfig.sapUtil.getDateTime(Convert.ToString(logonInfo.GetValue("Gltgv")))}");

                        structLogondata.SetValue("Gltgv", processorConfig.sapUtil.getDateTime(Convert.ToString(logonInfo.GetValue("Gltgv"))));
                        structLogondataX.SetValue("Gltgv", "X");
                    }
                }

                if (validTo != DateTime.MinValue)
                {
                    structLogondata.SetValue("Gltgb", validTo);
                    structLogondataX.SetValue("Gltgb", "X");
                }
                else
                {
                    if (logonInfo != null)
                    {
                        WriteProvisioningLog(LogLevelType.Info, $"validTo-Gltgb - {processorConfig.sapUtil.getDateTime(Convert.ToString(logonInfo.GetValue("Gltgb")))}");
                        structLogondata.SetValue("Gltgb", processorConfig.sapUtil.getDateTime(Convert.ToString(logonInfo.GetValue("Gltgb"))));
                        structLogondataX.SetValue("Gltgb", "X");
                    }
                }

                if (_SAPUserInfo.Groups != null && _SAPUserInfo.Groups.Count != 0)
                {
                    structLogondata.SetValue("Class", _SAPUserInfo.Groups[0]);
                    structLogondataX.SetValue("Class", "X");
                }

                fnUserChange.SetValue("LOGONDATA", structLogondata);
                fnUserChange.SetValue("LOGONDATAX", structLogondataX);

                WriteProvisioningLog(LogLevelType.Debug, $"ExecuteTable Started");

                var result = processorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                WriteProvisioningLog(LogLevelType.Debug, $"ExecuteTable Completed");

                bool isUserUpdated = false;
                foreach (DataRow row in result.Rows)
                {
                    if (row["Message"].ToString().ToUpper().Contains(string.Format("User {0} has changed", _SAPUserInfo.UserLogonName).ToUpper()) ||
                           row["Message"].ToString().ToUpper().Contains(string.Format("No changes were made to user {0}", _SAPUserInfo.UserLogonName).ToUpper()))
                    {
                        isUserUpdated = true;
                        break;
                    }
                }

                if (isUserUpdated)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Account - {UserLogonName} updated successfully.");
                    return true;
                }
                else
                {
                    foreach (DataRow row in result.Rows)
                    {
                        WriteProvisioningLog(LogLevelType.Error, row["Message"].ToString(), null);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while updating the account - {UserLogonName}.", ex);
                throw ex;
            }
        }

        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            SAPHANAUserinfo _SAPUserInfo = (SAPHANAUserinfo)_userInfo;
            string UserLogonName = _SAPUserInfo.UserLogonName;

            try
            {
                if (!IsUserExists(UserLogonName))
                {
                    WriteProvisioningLog(LogLevelType.Error, $"{UserLogonName} - account does not exists.");
                    return false;
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserLogonName);

                IRfcFunction fnUserChange = processorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                IRfcStructure structLogondata = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATA");
                IRfcStructure structLogondataX = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATAX");

                var logonInfo = GetLogonInfoFromSAP(_SAPUserInfo.UserLogonName);

                DateTime validTo = UtilityMethods.GetDateFromSAPDateString(_SAPUserInfo.UserValidTo);
                DateTime validFrom = UtilityMethods.GetDateFromSAPDateString(_SAPUserInfo.UserValidFrom);

                if (validFrom != DateTime.MinValue)
                {
                    structLogondata.SetValue("Gltgv", validFrom);
                    structLogondataX.SetValue("Gltgv", "X");
                }
                else
                {
                    if (logonInfo != null)
                    {
                        structLogondata.SetValue("Gltgv", processorConfig.sapUtil.getDateTime(Convert.ToString(logonInfo.GetValue("Gltgv"))));
                        structLogondataX.SetValue("Gltgv", "X");
                    }
                }

                if (validTo != DateTime.MinValue)
                {
                    structLogondata.SetValue("Gltgb", validTo);
                    structLogondataX.SetValue("Gltgb", "X");
                }
                else
                {
                    if (logonInfo != null)
                    {
                        structLogondata.SetValue("Gltgb", processorConfig.sapUtil.getDateTime(Convert.ToString(logonInfo.GetValue("Gltgb"))));
                        structLogondataX.SetValue("Gltgb", "X");
                    }
                }

                if (_SAPUserInfo.Groups != null && _SAPUserInfo.Groups.Count != 0)
                {
                    structLogondata.SetValue("Class", _SAPUserInfo.Groups[0]);
                    structLogondataX.SetValue("Class", "X");
                }

                fnUserChange.SetValue("LOGONDATA", structLogondata);
                fnUserChange.SetValue("LOGONDATAX", structLogondataX);

                var result = processorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                bool isUserUpdated = false;
                foreach (DataRow row in result.Rows)
                {
                    if (row["Message"].ToString().ToUpper().Contains(string.Format("User {0} has changed", _SAPUserInfo.UserLogonName).ToUpper()) ||
                           row["Message"].ToString().ToUpper().Contains(string.Format("No changes were made to user {0}", _SAPUserInfo.UserLogonName).ToUpper()))
                    {
                        isUserUpdated = true;
                        break;
                    }
                }

                if (isUserUpdated)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Account - {UserLogonName} extended successfully.");
                    if (ExtendPositionAsWell())
                    {
                        LogDebugStep("ExtendPositionAsWell");
                        bool hasHRInfo = Endpoint.ApplicationType == ApplicationType.MSSL || Endpoint.ApplicationType == ApplicationType.EBS;
                        var positions = GetPositionsByUser(_SAPUserInfo.UserLogonName);
                        LogDebugStep($"Positions to extend: {string.Join(",", positions)}");
                        foreach (var position in positions)
                        {
                            LogDebugStep($"ExtendPositionValidity for {position} started.");
                            ExtendPositionValidity(_SAPUserInfo, position, hasHRInfo);
                            LogDebugStep($"ExtendPositionValidity for {position} completed.");
                        }
                    }
                    return true;
                }
                else
                {
                    foreach (DataRow row in result.Rows)
                    {
                        WriteProvisioningLog(LogLevelType.Error, row["Message"].ToString(), null);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while extending the account - {UserLogonName}.", ex);
                throw ex;
            }
        }

        public override bool DisableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }
        public override bool AddEntitlement(IEndpointUserInfo _userInfo, string Validfrom, string ValidTo, EntitlementType entitlementType)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            string RoleStatus = string.Empty;

            var _SAPUserInfo = (SAPHANAUserinfo)_userInfo;
            var existingRoles = new List<SAPHANAExistingRoleInfo>();
            DataTable Result = null;
            try
            {
                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    #region SAP BAPI CLASSES

                    BAPIADDR3 address = new BAPIADDR3();
                    BAPIUSCOMP companyName = new BAPIUSCOMP();
                    BAPIALIAS alias = new BAPIALIAS();
                    BAPIDEFAUL defaults = new BAPIDEFAUL();
                    BAPISLOCKD islocked = new BAPISLOCKD();
                    BAPILOGOND logonData = new BAPILOGOND();
                    BAPIPWD passworddata = new BAPIPWD();
                    BAPIMODDAT lastmofified = new BAPIMODDAT();
                    BAPIREFUS refuser = new BAPIREFUS();
                    BAPISNCU snc = new BAPISNCU();
                    BAPIUCLASS iuclass = new BAPIUCLASS();
                    BAPIAGRTable activeGroups = new BAPIAGRTable();
                    BAPICOMREMTable addcomrem = new BAPICOMREMTable();
                    BAPIADFAXTable addfax = new BAPIADFAXTable();
                    BAPIADPAGTable addpag = new BAPIADPAGTable();
                    BAPIADPRTTable addprt = new BAPIADPRTTable();
                    BAPIADRFCTable addrfc = new BAPIADRFCTable();
                    BAPIADRMLTable addrml = new BAPIADRMLTable();
                    BAPIADSMTPTable addsmtp = new BAPIADSMTPTable();
                    BAPIADSSFTable addssf = new BAPIADSSFTable();
                    BAPIADTELTable addtel = new BAPIADTELTable();
                    BAPIADTLXTable addtlx = new BAPIADTLXTable();
                    BAPIADTTXTable addttx = new BAPIADTTXTable();
                    BAPIADURITable adduri = new BAPIADURITable();
                    BAPIADX400Table addx400 = new BAPIADX400Table();
                    BAPIUSEXTIDHEADTable exithead = new BAPIUSEXTIDHEADTable();
                    BAPIUSEXTIDPARTTable extidpart = new BAPIUSEXTIDPARTTable();

                    BAPIGROUPSTable groups = new BAPIGROUPSTable();
                    BAPIPARAMTable parameter = new BAPIPARAMTable();
                    BAPIPARAM1Table parameter1 = new BAPIPARAM1Table();
                    BAPIRCVSYSTable systems = new BAPIRCVSYSTable();
                    BAPIPROFTable profiles = new BAPIPROFTable();
                    BAPIUCLASSSYSTable uclClassSys = new BAPIUCLASSSYSTable();
                    SAPUserDetailsComponent.BAPIRET2Table return0 = new SAPUserDetailsComponent.BAPIRET2Table();
                    BAPIUSSEXPTable sextable = new BAPIUSSEXPTable();
                    BAPIUSSRGETable gettable = new BAPIUSSRGETable();
                    BAPIUSNAMETable userNametable = new BAPIUSNAMETable();

                    AGR_TCODESTable TCodes = new AGR_TCODESTable();
                    AGR_SHIER_BORTable SHIERBOR = new AGR_SHIER_BORTable();
                    AGR_HIERTable HeirTable = new AGR_HIERTable();
                    AGR_BUFFITable BuffitTable = new AGR_BUFFITable();
                    AGR_HIERTTable HeirTTable = new AGR_HIERTTable();

                    BAPILOGONX BAPILOGONX = new BAPILOGONX();
                    BAPIPWDX BAPIPWDX = new BAPIPWDX();
                    BAPIADCOMX BAPIADCOMX = new BAPIADCOMX();
                    BAPIADDR3X BAPIADDR3X = new BAPIADDR3X();
                    BAPIALIASX BAPIALIASX = new BAPIALIASX();
                    BAPIFLAG BAPIFLAG = new BAPIFLAG();
                    BAPIUSCOMX BAPIUSCOMX = new BAPIUSCOMX();
                    BAPIDEFAX BAPIDEFAX = new BAPIDEFAX();
                    BAPIUSEXTIDX BAPIUSEXTIDX = new BAPIUSEXTIDX();
                    BAPIGROUPX BAPIGROUPX = new BAPIGROUPX();
                    BAPIPARAMX BAPIPARAMX = new BAPIPARAMX();
                    BAPIREFUSX BAPIREFUSX = new BAPIREFUSX();
                    BAPISNCUX BAPISNCUX = new BAPISNCUX();
                    BAPIUCLASSX BAPIUCLASSX = new BAPIUCLASSX();
                    BAPIUCLASSSYSTable BAPIUCLASSSYSTable = new BAPIUCLASSSYSTable();
                    BAPIDEFAX defaults1X = new BAPIDEFAX();

                    SAPUserDetailsComponent.BAPIRET2 return2 = new SAPUserDetailsComponent.BAPIRET2();

                    #endregion

                    LogDebugStep("Bapi_User_Existence_Check");
                    proxy.Bapi_User_Existence_Check(_SAPUserInfo.UserLogonName, out return2);
                    LogDebugStep("Bapi_User_Get_Detail");
                    proxy.Bapi_User_Get_Detail(_SAPUserInfo.UserLogonName, _SAPUserInfo.UserLogonName, out address, out alias, out companyName, out defaults, out islocked, out lastmofified, out logonData, out refuser, out snc, out iuclass,
                                                                     ref activeGroups, ref addcomrem, ref addfax, ref addpag, ref addprt, ref addrfc,
                                                           ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx,
                                                            ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter,
                                                            ref parameter1, ref profiles, ref return0, ref systems, ref uclClassSys);
                    UserExist = return2.Message.ToString();
                    LogDebugStep($"UserExist: {UserExist}");
                    if (!(UserExist.ToUpper().Contains("NOT EXIST")))
                    {
                        existingRoles = new List<SAPHANAExistingRoleInfo>();
                        BAPIAGR BAPIAGR = new BAPIAGR();

                        foreach (DataRow oldRole in activeGroups.ToADODataTable().Rows)
                        {
                            string roleName = oldRole["AGR_NAME"].ToString();
                            string validFrom = oldRole["From_Dat"].ToString();
                            string validTo = oldRole["To_Dat"].ToString();
                            string OrgFlag = oldRole["Org_Flag"].ToString();

                            if (!existingRoles.Any(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag,true)==0))
                            {
                                var existingRoleInfo = new SAPHANAExistingRoleInfo();
                                existingRoleInfo.Role_Code = oldRole["AGR_NAME"].ToString();
                                existingRoleInfo.ValidFrom = oldRole["From_Dat"].ToString();
                                existingRoleInfo.ValidTo = oldRole["To_Dat"].ToString();
                                existingRoleInfo.Org_Flag = oldRole["Org_Flag"].ToString().Trim();
                                existingRoles.Add(existingRoleInfo);
                            }
                            else
                            {

                                //if there any duplicate in SAP roles then should handle below code.

                                DateTime tmpStartDate1 = UtilityMethods.GetDateFromString(validFrom);
                                DateTime tmpEndDate1 = UtilityMethods.GetDateFromString(validTo);

                                DateTime tmpStartDate2 = UtilityMethods.GetDateFromString(existingRoles.FirstOrDefault(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0).ValidFrom);
                                DateTime tmpEndDate2 = UtilityMethods.GetDateFromString(existingRoles.FirstOrDefault(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0).ValidTo);

                                if (tmpStartDate1 < tmpStartDate2)
                                {
                                    existingRoles.FirstOrDefault(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0).ValidFrom = validFrom;
                                }

                                if (tmpEndDate1 > tmpEndDate2)
                                {
                                    existingRoles.FirstOrDefault(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0).ValidTo = validTo;
                                }

                                existingRoles.FirstOrDefault(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0).UsedInMultipleRoles = true;
                            }
                        }
                        LogDebugStep( $"Before Adding to existingRoles:{existingRoles.Count} and Roles: { string.Join(",", existingRoles.Select(i => string.Format("[Role:{0}- OrgFlag:{1}]", i.Role_Code, i.Org_Flag)))}");

                        List<string> rolesToAdd = _SAPUserInfo.Roles;
                        if (_SAPUserInfo.Roles != null && _SAPUserInfo.Roles.Count > 0)
                        {
                            LogDebugStep($"Roles to be updated: {string.Join(",", _SAPUserInfo.Roles)}");
                        }
                        foreach (string newRole in rolesToAdd)
                        {
                            var role = existingRoles.FirstOrDefault(item => item.Role_Code == newRole && string.IsNullOrEmpty(item.Org_Flag));
                            if (role != null)
                            {
                                if (role.ValidFrom != Validfrom.Replace(".", string.Empty) || role.ValidTo != ValidTo.Replace(".", string.Empty))
                                {
                                    //existingRoles.Add(new ExistingRoleInfo() { Role_Code = newRole, ValidFrom = Validfrom, ValidTo = ValidTo });
                                    if (this.CurrentRequest.AdHocAccessRequest != null && this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd)
                                    {
                                        bool updateValidTo = false;
                                        try
                                        {
                                            var toDate = role.ValidTo.Replace(".", string.Empty);
                                            var fromDate = role.ValidFrom.Replace(".", string.Empty);
                                            var newDate = ValidTo.Replace(".", string.Empty);
                                            if (!string.IsNullOrWhiteSpace(toDate) && !string.IsNullOrWhiteSpace(newDate) &&                    toDate.IsNumeric() && newDate.IsNumeric()&&
                                                !string.IsNullOrWhiteSpace(fromDate) && fromDate.IsNumeric())
                                            {
                                                if (Convert.ToInt64(newDate) >= Convert.ToInt64(fromDate))
                                                {
                                                    updateValidTo = true;
                                                }
                                            }
                                            else
                                            {
                                                updateValidTo = true;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            updateValidTo = true;
                                            LogDebugStep($"Setting ValidTo for existing role:{role.Role_Code} {role.ValidTo} error :{ex.Message.ToString()}");
                                        }
                                        if (updateValidTo)
                                        {
                                            role.ValidTo = ValidTo;
                                        }
                                    }
                                    else
                                    {
                                        role.ValidTo = ValidTo;
                                    }
                                    role.ValidFrom = Validfrom;
                                }
                            }
                            else
                            {
                                existingRoles.Add(new SAPHANAExistingRoleInfo() { Role_Code = newRole, ValidFrom = Validfrom, ValidTo = ValidTo });
                            }
                        }
                       
                        LogDebugStep($"After Adding to existingRoles:{existingRoles.Count} and Roles: {string.Join(",", existingRoles.Select(i => string.Format("[Role:{0}- OrgFlag:{1}]", i.Role_Code, i.Org_Flag)))}");

                        BAPIAGRTable Roles = new BAPIAGRTable();
                        foreach (var Role in existingRoles)
                        {
                            BAPIAGR = new BAPIAGR();
                            BAPIAGR.Agr_Name = Role.Role_Code;
                            BAPIAGR.From_Dat = Role.ValidFrom;
                            BAPIAGR.To_Dat = Role.ValidTo;
                            BAPIAGR.Org_Flag = Role.Org_Flag;
                            Roles.Add(BAPIAGR);
                        }

                        if (_SAPUserInfo.Groups != null && _SAPUserInfo.Groups.Count != 0)
                        {
                            LogDebugStep($"Groups to be updated: {string.Join(",", _SAPUserInfo.Groups)}");
                            foreach (string group in _SAPUserInfo.Groups)
                            {
                                BAPIGROUPS GroupName = new BAPIGROUPS();
                                GroupName.Usergroup = group;
                                groups.Add(GroupName);
                            }
                        }

                        proxy.Bapi_User_Change(BAPIADCOMX, address, BAPIADDR3X, alias, BAPIALIASX, BAPIFLAG, companyName, BAPIUSCOMX, defaults, defaults1X, BAPIUSEXTIDX, BAPIGROUPX, logonData, BAPILOGONX, BAPIPARAMX, passworddata, BAPIPWDX, "", refuser, BAPIREFUSX, snc, BAPISNCUX, iuclass, BAPIUCLASSX, _SAPUserInfo.UserLogonName, ref addcomrem,
                        ref addfax, ref addpag, ref addprt, ref addrfc, ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx, ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter, ref parameter1, ref return0, ref BAPIUCLASSSYSTable);
                        UserStatus = return0.ToADODataTable().Rows[0]["Message"].ToString();

                        if (!UserStatus.StartsWith("Maintenance of user"))
                        {
                            return0 = new SAPUserDetailsComponent.BAPIRET2Table();
                            proxy.Bapi_User_Actgroups_Assign(_SAPUserInfo.UserLogonName, ref Roles, ref return0);

                            Result = return0.ToADODataTable();

                            bool isUserUpdated = false;
                            foreach (DataRow row in Result.Rows)
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Info, $"Response Message from SAPHANA for Account Creation:{row["Message"]?.ToString()}");
                                if (row["Message"].ToString().ToUpper().Contains(string.Format("Role assignment to user {0} changed", _SAPUserInfo.UserLogonName).ToUpper()) ||
                                    row["Message"].ToString().ToUpper().Contains(string.Format("Role assignment to user {0} not executed completely", _SAPUserInfo.UserLogonName).ToUpper()) ||
                                    row["Message"].ToString().ToUpper().Contains(string.Format("No changes were made to user {0}", _SAPUserInfo.UserLogonName).ToUpper()))
                                {
                                    isUserUpdated = true;
                                    break;
                                }
                            }

                            if (isUserUpdated)
                            {
                                base.AddEntitlement(_SAPUserInfo, entitlementType);
                            }
                            else
                            {
                                List<string> errors = new List<string>();
                                foreach (DataRow row in Result.Rows)
                                {
                                    errors.Add(row["Message"].ToString());
                                }
                                this._idmsLogger.WriteLog(LogLevelType.Error, string.Join(",", errors));
                                return false;
                            }
                        }
                        else
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Error, UserStatus);
                            return false;
                        }
                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, UserExist);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                return false;
            }
            finally
            {
                Result = null;
                existingRoles?.Clear();
                existingRoles = null;
            }
        }

        DateTime GetValidTo(Nullable<long> userId)
        {
            if (CurrentRequest != null && CurrentRequest.DeactivationDate.HasValue)
            {
                return CurrentRequest.DeactivationDate.Value;
            }
            else
            {
                var validTo = DateTime.MaxValue;
                if (userId.HasValue && userId.Value > 0)
                {
                    var user = DomainModel.User.GetById(userId.Value);
                    if (user != null && user.TerminationDate.HasValue) validTo = user.TerminationDate.Value;
                }
                return validTo;
            }
        }

        protected override void AddEntitlementCommon(IEndpointUserInfo _userInfo, EntitlementType _entitlementType, AccountReconcilation acctRecon, string fieldName)
        {
            var entitlements = this.GetValueByFieldName(_userInfo, fieldName);

            var validTo = GetValidTo(acctRecon.UserId);

            if (entitlements != null)
            {
                foreach (string entitlement in entitlements)
                {
                    var entRecon = EntitlementReconcilation.GetByUser(_userInfo.Id, _userInfo.EndPointSystemId, entitlement.Trim(), _entitlementType);

                    if (null == entRecon)
                    {
                        CreateEntitlementReconcilationShellAndSave(acctRecon, _entitlementType, entitlement.Trim(), validTo);
                    }
                    else
                    {
                        entRecon.EndpointUserId = _userInfo.Id;
                        entRecon.ValidToDate = validTo;
                        entRecon.Entitlement = entitlement.Trim();
                        entRecon.IsDeleted = false;
                        entRecon.DeletedDate = null;
                        entRecon.Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
        }
        //public override bool AddEntitlement(IEndpointUserInfo _userInfo, string Validfrom, string ValidTo, EntitlementType entitlementType)
        //{
        //    LogDebugStep("AddEntitlement");
        //    Dictionary<string, object> dicParams = new Dictionary<string, object>();

        //    try
        //    {

        //        SAPUserinfo _SAPUserInfo = (SAPUserinfo)_userInfo;
        //        string UserLogonName = _SAPUserInfo.UserLogonName;

        //        LogDebugStep("UserLogonName:" + UserLogonName);
        //        if (!IsUserExists(UserLogonName))
        //        {
        //            WriteProvisioningLog(LogLevelType.Error, $"{UserLogonName} - account does not exists.");
        //            return false;
        //        }

        //        dicParams = new Dictionary<string, object>();
        //        dicParams.Add("USERNAME", UserLogonName);

        //        LogDebugStep("processorConfig.sapUtil");

        //        IRfcFunction fnUserDetail = processorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
        //        var tblRfcRoles = processorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
        //        //var tblRoles = processorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
        //        IRfcFunction fnRolesAssign = processorConfig.sapUtil.getRfcFunction("BAPI_USER_ACTGROUPS_ASSIGN", dicParams);

        //        LogDebugStep($"tblRfcRoles: {tblRfcRoles?.Count}");

        //        //processorConfig.sapUtil.DeleteAllRowsInTable(fnRolesAssign, tblRfcRoles, "ACTIVITYGROUPS");

        //        //List<ExistingRoleInfo> existingRoles = new List<ExistingRoleInfo>();

        //        LogDebugStep("foreach tblRoles.Rows");

        //        //foreach (DataRow oldRole in tblRoles.Rows)
        //        //{
        //        //    string roleName = oldRole["AGR_NAME"].ToString();
        //        //    string validFrom = oldRole["From_Dat"].ToString();
        //        //    string validTo = oldRole["To_Dat"].ToString();

        //        //    if (!existingRoles.Any(item => item.Role_Code == roleName))
        //        //    {
        //        //        ExistingRoleInfo existingRoleInfo = new ExistingRoleInfo();
        //        //        existingRoleInfo.Role_Code = oldRole["AGR_NAME"].ToString();
        //        //        existingRoleInfo.ValidFrom = oldRole["From_Dat"].ToString();
        //        //        existingRoleInfo.ValidTo = oldRole["To_Dat"].ToString();
        //        //        existingRoles.Add(existingRoleInfo);
        //        //    }
        //        //    else
        //        //    {
        //        //        DateTime tmpStartDate1 = UtilityMethods.GetDateFromString(validFrom);
        //        //        DateTime tmpEndDate1 = UtilityMethods.GetDateFromString(validTo);

        //        //        DateTime tmpStartDate2 = UtilityMethods.GetDateFromString(existingRoles.FirstOrDefault(item => item.Role_Code == roleName).ValidFrom);
        //        //        DateTime tmpEndDate2 = UtilityMethods.GetDateFromString(existingRoles.FirstOrDefault(item => item.Role_Code == roleName).ValidTo);

        //        //        if (tmpStartDate1 < tmpStartDate2)
        //        //        {
        //        //            existingRoles.FirstOrDefault(item => item.Role_Code == roleName).ValidFrom = validFrom;
        //        //        }

        //        //        if (tmpEndDate1 > tmpEndDate2)
        //        //        {
        //        //            existingRoles.FirstOrDefault(item => item.Role_Code == roleName).ValidTo = validTo;
        //        //        }

        //        //        existingRoles.FirstOrDefault(item => item.Role_Code == roleName).UsedInMultipleRoles = true;
        //        //    }
        //        //}


        //        List<string> rolesToAdd = _SAPUserInfo.Roles;
        //        LogDebugStep($"Roles to Add: {string.Join(",", rolesToAdd)}");

        //        //LogDebugStep($"_SAPUserInfo.Roles: {rolesToAdd?.Count}");
        //        //foreach (string newRole in rolesToAdd)
        //        //{
        //        //    var role = existingRoles.FirstOrDefault(item => item.Role_Code == newRole);

        //        //    if (role != null)
        //        //    {
        //        //        //LogDebugStep($"role == null: {role == null}, Validfrom: {Validfrom}, ValidTo: {ValidTo}, role.ValidFrom: {role.ValidFrom}, role.ValidTo: {role.ValidTo}");
        //        //        if (role.ValidFrom != Validfrom?.Replace(".", string.Empty) || role.ValidTo != ValidTo?.Replace(".", string.Empty))
        //        //        {
        //        //            existingRoles.Add(new ExistingRoleInfo() { Role_Code = newRole, ValidFrom = Validfrom, ValidTo = ValidTo });
        //        //            //LogDebugStep("existingRoles.Add");
        //        //        }
        //        //    }
        //        //    else
        //        //    {
        //        //        //LogDebugStep($"role == null: {role == null}, newRole:{newRole}");
        //        //        existingRoles.Add(new ExistingRoleInfo() { Role_Code = newRole, ValidFrom = Validfrom, ValidTo = ValidTo });
        //        //        //LogDebugStep("existingRoles.Add - else");
        //        //    }
        //        //}

        //        LogDebugStep($"tblRfcRoles - before adding: {tblRfcRoles.Count}");
        //        //tblRfcRoles.Clear();
        //        LogDebugStep($"Updating existing roles with new valid dates");
        //        foreach (var roleRFC in tblRfcRoles)
        //        {
        //            if (rolesToAdd.Contains(Convert.ToString(roleRFC.GetValue("Agr_Name"))))
        //            {
        //                roleRFC.SetValue("From_Dat", UtilityMethods.GetDateFromSAPDateString(Validfrom));
        //                roleRFC.SetValue("To_Dat", UtilityMethods.GetDateFromSAPDateString(ValidTo));
        //            }
        //        }

        //        LogDebugStep($"Adding new roles");
        //        foreach (var Role in rolesToAdd)
        //        {
        //            if (tblRfcRoles.Any(p => p.GetString("Agr_Name") == Role))
        //                continue;

        //            tblRfcRoles.SetValue("From_Dat", UtilityMethods.GetDateFromSAPDateString(Validfrom));
        //            tblRfcRoles.SetValue("To_Dat", UtilityMethods.GetDateFromSAPDateString(ValidTo));
        //            tblRfcRoles.SetValue("Agr_Name", Role);
        //            tblRfcRoles.Append();
        //        }

        //        //LogDebugStep($"Final Roles to be added: {tblRfcRoles.Select(p => p.GetString("Agr_Name"))}");
        //        LogDebugStep($"Final Roles to be added: {tblRfcRoles.Count}");

        //        fnRolesAssign.SetValue("ACTIVITYGROUPS", tblRfcRoles);
        //        var result = processorConfig.sapUtil.ExecuteTable(fnRolesAssign, "RETURN");

        //        LogDebugStep("isUserUpdated");

        //        bool isUserUpdated = false;
        //        foreach (DataRow row in result.Rows)
        //        {
        //            LogDebugStep(row["Message"].ToString().ToUpper());
        //            if (row["Message"].ToString().ToUpper().Contains(string.Format("Role assignment to user {0} changed", _SAPUserInfo.UserLogonName).ToUpper()) ||
        //                row["Message"].ToString().ToUpper().Contains(string.Format("Role assignment to user {0} not executed completely", _SAPUserInfo.UserLogonName).ToUpper()) ||
        //                row["Message"].ToString().ToUpper().Contains(string.Format("No changes were made to user {0}", _SAPUserInfo.UserLogonName).ToUpper()))
        //            {
        //                isUserUpdated = true;
        //                break;
        //            }
        //        }

        //        if (isUserUpdated)
        //        {
        //            LogDebugStep("base.AddEntitlement");
        //            base.AddEntitlement(_SAPUserInfo, entitlementType);
        //        }
        //        else
        //        {
        //            LogDebugStep("result.Rows");
        //            foreach (DataRow row in result.Rows)
        //            {
        //                WriteProvisioningLog(LogLevelType.Error, row["Message"].ToString());
        //            }
        //            return false;
        //        }
        //        LogDebugStep("AddEntitlement - completed");
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteProvisioningLog(LogLevelType.Exception, ex.Message, ex);
        //        throw ex;
        //    }

        //    return true;
        //}
        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            string UserExist = string.Empty;
            string UserStatus = string.Empty;
            List<SAPHANAExistingRoleInfo> lstOldRoles = null;
            List<string> defaultRoles = null;
            List<SAPHANAExistingRoleInfo> lstssDelete = null;
            List<string> lstDelRoles = null;
            DataTable Result = null;
            try
            {
                if (CurrentRequest != null && CurrentRequest.BulkRequestId.HasValue && CurrentRequest.BulkRequestId.Value > 0)
                {
                    UpdateValidRoles(_userInfo, _entitlementType);
                }

                var _SAPUserInfo = (SAPHANAUserinfo)_userInfo;

                if (_entitlementType == EntitlementType.Role && _SAPUserInfo.Roles.Count == 0)
                    return true;

                if (_entitlementType == EntitlementType.Group && _SAPUserInfo.Groups.Count == 0)
                    return true;

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    #region SAP BAPI CLASSES

                    BAPIADDR3 address = new BAPIADDR3();
                    BAPIUSCOMP companyName = new BAPIUSCOMP();
                    BAPIALIAS alias = new BAPIALIAS();
                    BAPIDEFAUL defaults = new BAPIDEFAUL();
                    BAPISLOCKD islocked = new BAPISLOCKD();
                    BAPILOGOND logonData = new BAPILOGOND();
                    BAPIPWD passworddata = new BAPIPWD();
                    BAPIMODDAT lastmofified = new BAPIMODDAT();
                    BAPIREFUS refuser = new BAPIREFUS();
                    BAPISNCU snc = new BAPISNCU();
                    BAPIUCLASS iuclass = new BAPIUCLASS();
                    BAPIAGRTable activeGroups = new BAPIAGRTable();
                    BAPICOMREMTable addcomrem = new BAPICOMREMTable();
                    BAPIADFAXTable addfax = new BAPIADFAXTable();
                    BAPIADPAGTable addpag = new BAPIADPAGTable();
                    BAPIADPRTTable addprt = new BAPIADPRTTable();
                    BAPIADRFCTable addrfc = new BAPIADRFCTable();
                    BAPIADRMLTable addrml = new BAPIADRMLTable();
                    BAPIADSMTPTable addsmtp = new BAPIADSMTPTable();
                    BAPIADSSFTable addssf = new BAPIADSSFTable();
                    BAPIADTELTable addtel = new BAPIADTELTable();
                    BAPIADTLXTable addtlx = new BAPIADTLXTable();
                    BAPIADTTXTable addttx = new BAPIADTTXTable();
                    BAPIADURITable adduri = new BAPIADURITable();
                    BAPIADX400Table addx400 = new BAPIADX400Table();
                    BAPIUSEXTIDHEADTable exithead = new BAPIUSEXTIDHEADTable();
                    BAPIUSEXTIDPARTTable extidpart = new BAPIUSEXTIDPARTTable();

                    BAPIGROUPSTable groups = new BAPIGROUPSTable();
                    BAPIPARAMTable parameter = new BAPIPARAMTable();
                    BAPIPARAM1Table parameter1 = new BAPIPARAM1Table();
                    BAPIRCVSYSTable systems = new BAPIRCVSYSTable();
                    BAPIPROFTable profiles = new BAPIPROFTable();
                    BAPIUCLASSSYSTable uclClassSys = new BAPIUCLASSSYSTable();
                    SAPUserDetailsComponent.BAPIRET2Table return0 = new SAPUserDetailsComponent.BAPIRET2Table();
                    BAPIUSSEXPTable sextable = new BAPIUSSEXPTable();
                    BAPIUSSRGETable gettable = new BAPIUSSRGETable();
                    BAPIUSNAMETable userNametable = new BAPIUSNAMETable();

                    AGR_TCODESTable TCodes = new AGR_TCODESTable();
                    AGR_SHIER_BORTable SHIERBOR = new AGR_SHIER_BORTable();
                    AGR_HIERTable HeirTable = new AGR_HIERTable();
                    AGR_BUFFITable BuffitTable = new AGR_BUFFITable();
                    AGR_HIERTTable HeirTTable = new AGR_HIERTTable();

                    BAPILOGONX BAPILOGONX = new BAPILOGONX();
                    BAPIPWDX BAPIPWDX = new BAPIPWDX();
                    BAPIADCOMX BAPIADCOMX = new BAPIADCOMX();
                    BAPIADDR3X BAPIADDR3X = new BAPIADDR3X();
                    BAPIALIASX BAPIALIASX = new BAPIALIASX();
                    BAPIFLAG BAPIFLAG = new BAPIFLAG();
                    BAPIUSCOMX BAPIUSCOMX = new BAPIUSCOMX();
                    BAPIDEFAX BAPIDEFAX = new BAPIDEFAX();
                    BAPIUSEXTIDX BAPIUSEXTIDX = new BAPIUSEXTIDX();
                    BAPIGROUPX BAPIGROUPX = new BAPIGROUPX();
                    BAPIPARAMX BAPIPARAMX = new BAPIPARAMX();
                    BAPIREFUSX BAPIREFUSX = new BAPIREFUSX();
                    BAPISNCUX BAPISNCUX = new BAPISNCUX();
                    BAPIUCLASSX BAPIUCLASSX = new BAPIUCLASSX();
                    BAPIUCLASSSYSTable BAPIUCLASSSYSTable = new BAPIUCLASSSYSTable();

                    SAPUserDetailsComponent.BAPIRET2 return2 = new SAPUserDetailsComponent.BAPIRET2();

                    #endregion
                    LogDebugStep("Bapi_User_Existence_Check");
                    proxy.Bapi_User_Existence_Check(_SAPUserInfo.UserLogonName, out return2);
                    LogDebugStep("Bapi_User_Get_Detail");
                    proxy.Bapi_User_Get_Detail(_SAPUserInfo.UserLogonName, _SAPUserInfo.UserLogonName, out address, out alias, out companyName, out defaults, out islocked, out lastmofified, out logonData, out refuser, out snc, out iuclass,
                                                                      ref activeGroups, ref addcomrem, ref addfax, ref addpag, ref addprt, ref addrfc,
                                                            ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx,
                                                             ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter,
                                                             ref parameter1, ref profiles, ref return0, ref systems, ref uclClassSys);
                    UserExist = return2.Message.ToString();
                    LogDebugStep($"UserExist: {UserExist}");
                    BAPIAGR BAPIAGR = new BAPIAGR();
                    lstOldRoles = new List<SAPHANAExistingRoleInfo>();

                    //retain default roles added thru create prov form
                    defaultRoles = this.GetDefaultRolesByEndpoint();

                    foreach (DataRow oldRole in activeGroups.ToADODataTable().Rows)
                    {
                        string roleName = oldRole["AGR_NAME"].ToString();
                        string validFrom = oldRole["From_Dat"].ToString();
                        string validTo = oldRole["To_Dat"].ToString();
                        string OrgFlag = oldRole["Org_Flag"].ToString();

                        if (!lstOldRoles.Any(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0))
                        {
                            var existingRoleInfo = new SAPHANAExistingRoleInfo();
                            existingRoleInfo.Role_Code = oldRole["AGR_NAME"].ToString();
                            existingRoleInfo.ValidFrom = oldRole["From_Dat"].ToString();
                            existingRoleInfo.ValidTo = oldRole["To_Dat"].ToString();
                            existingRoleInfo.Org_Flag = oldRole["Org_Flag"].ToString().Trim();
                            lstOldRoles.Add(existingRoleInfo);
                        }
                        else
                        {
                            DateTime tmpStartDate1 = UtilityMethods.GetDateFromString(validFrom);
                            DateTime tmpEndDate1 = UtilityMethods.GetDateFromString(validTo);

                            DateTime tmpStartDate2 = UtilityMethods.GetDateFromString(lstOldRoles.FirstOrDefault(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0).ValidFrom);
                            DateTime tmpEndDate2 = UtilityMethods.GetDateFromString(lstOldRoles.FirstOrDefault(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0).ValidTo);

                            if (tmpStartDate1 < tmpStartDate2)
                            {
                                lstOldRoles.FirstOrDefault(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0).ValidFrom = validFrom;
                            }

                            if (tmpEndDate1 > tmpEndDate2)
                            {
                                lstOldRoles.FirstOrDefault(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0).ValidTo = validTo;
                            }

                            lstOldRoles.FirstOrDefault(item => item.Role_Code == roleName && string.Compare(item.Org_Flag, OrgFlag, true) == 0).UsedInMultipleRoles = true;
                        }
                    }

                    LogDebugStep($"Before removing to lstOldRoles:{lstOldRoles.Count} and Roles: {string.Join(",", lstOldRoles.Select(i => string.Format("[Role:{0}- OrgFlag:{1}]", i.Role_Code, i.Org_Flag)))}");

                    lstDelRoles = _SAPUserInfo.Roles;
                    foreach (string newRole in lstDelRoles)
                    {
                        // Remove only direct roles (Composite & Indirect roles should not be removed)
                        var lsts = from lst in lstOldRoles
                                   where lst.Role_Code.ToUpper() == newRole.ToUpper() &&
                                         // !lst.UsedInMultipleRoles && 
                                         (string.IsNullOrWhiteSpace(lst.Org_Flag) || (lst.Org_Flag != "C" && lst.Org_Flag != "X"))
                                   select lst;
                        lstssDelete = lsts.ToList<SAPHANAExistingRoleInfo>();


                        foreach (var info in lstssDelete)
                        {
                            if (!defaultRoles.Contains(info.Role_Code))
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"{info.Role_Code} - Technical Role Removed");
                                lstOldRoles.Remove(info);
                            }
                        }
                        //lstOldRoles.Remove((OldRoleInfo)(.ToString()));
                    }

                    LogDebugStep($"after removing to lstOldRoles:{lstOldRoles.Count} and Roles: {string.Join(",", lstOldRoles.Select(i => string.Format("[Role:{0}- OrgFlag:{1}]", i.Role_Code, i.Org_Flag)))}");


                    BAPIAGRTable newRoles = new BAPIAGRTable();
                    foreach (var Role in lstOldRoles)
                    {
                        BAPIAGR = new BAPIAGR();
                        BAPIAGR.Agr_Name = Role.Role_Code;
                        BAPIAGR.From_Dat = Role.ValidFrom;
                        BAPIAGR.To_Dat = Role.ValidTo;
                        BAPIAGR.Org_Flag = Role.Org_Flag;
                        newRoles.Add(BAPIAGR);
                    }

                    LogDebugStep($"Roles to be updated: {newRoles.Count}");
                    if (!UserExist.ToUpper().Contains("NOT EXIST"))
                    {
                        proxy.Bapi_User_Actgroups_Assign(_SAPUserInfo.UserLogonName, ref newRoles, ref return0);

                        Result = return0.ToADODataTable();

                        bool isUserUpdated = false;
                        foreach (DataRow row in Result.Rows)
                        {
                            this._idmsLogger.WriteLog(LogLevelType.Info, $"Response Message from SAPHANA for Account Creation:{row["Message"]?.ToString()}");
                            if (row["Message"].ToString().ToUpper().Contains(string.Format("Role assignment to user {0} changed", _SAPUserInfo.UserLogonName).ToUpper()) ||
                                row["Message"].ToString().ToUpper().Contains(string.Format("Role assignment to user {0} not executed completely", _SAPUserInfo.UserLogonName).ToUpper()) ||
                                row["Message"].ToString().ToUpper().Contains(string.Format("No changes were made to user {0}", _SAPUserInfo.UserLogonName).ToUpper()))
                            {
                                isUserUpdated = true;
                                break;
                            }
                        }

                        if (isUserUpdated)
                        {
                            #region Remove UserGroup

                            BAPIGROUPSTable groupNew = groups;
                            //Assigning groups
                            if (_SAPUserInfo.Groups != null && _SAPUserInfo.Groups.Count > 0)
                            {
                                BAPIGROUPSTable tmpGroup = new BAPIGROUPSTable();
                                for (int cnt = 0; cnt < groupNew.Count; cnt++)
                                {
                                    if (_SAPUserInfo.Groups.Contains(groupNew[cnt].Usergroup))
                                    {
                                        tmpGroup.Add(groupNew[cnt]);
                                    }
                                }

                                for (int cnt = 0; cnt < tmpGroup.Count; cnt++)
                                {
                                    groupNew.Remove(tmpGroup[cnt]);
                                }

                                BAPIGROUPX.Usergroup = "X";

                                proxy.Bapi_User_Change(BAPIADCOMX, address, BAPIADDR3X, alias, BAPIALIASX, BAPIFLAG, companyName, BAPIUSCOMX, defaults, null, BAPIUSEXTIDX, BAPIGROUPX, logonData, BAPILOGONX, BAPIPARAMX, passworddata, BAPIPWDX, "", refuser, BAPIREFUSX, snc, BAPISNCUX, iuclass, BAPIUCLASSX, _SAPUserInfo.UserLogonName, ref addcomrem,
                                ref addfax, ref addpag, ref addprt, ref addrfc, ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx, ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter, ref parameter1, ref return0, ref BAPIUCLASSSYSTable);
                            }

                            #endregion

                            base.RemoveEntitlement(_userInfo, _entitlementType);

                            return true;
                        }
                        else
                        {
                            if (_SAPUserInfo.Roles.Count == 0)
                            {
                                return true;
                            }

                            List<string> errors = new List<string>();
                            foreach (DataRow row in Result.Rows)
                            {
                                errors.Add(row["Message"].ToString());
                            }
                            this._idmsLogger.WriteLog(LogLevelType.Error, string.Join(",", errors));
                            return false;
                        }
                    }
                    else
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, UserExist);
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                return false;
            }
            finally
            {
                Result = null;

                defaultRoles?.Clear();
                defaultRoles = null;

                lstssDelete?.Clear();
                lstssDelete = null;

                lstOldRoles?.Clear();
                lstOldRoles = null;
            }
        }

        private List<string> GetDefaultRolesByEndpoint()
        {
            var nprovisioningField = NProvisioningField.GetByFieldNameAndAccountType(Endpoint.Id, "Roles", CurrentRequest.AccountType);
            return string.IsNullOrWhiteSpace(nprovisioningField.Value) ? new List<string>() : nprovisioningField.Value.Split(',').ToList();
        }

        
        public override bool RemoveAllEntitlements(IEndpointUserInfo _userInfo)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {


                var _SAPUserInfo = (SAPHANAUserinfo)_userInfo;
                string UserLogonName = _SAPUserInfo.UserLogonName;

                WriteProvisioningLog(LogLevelType.DebugStep, $"{UserLogonName} - RemoveAllEntitlements Started");

                if (!IsUserExists(UserLogonName))
                {
                    WriteProvisioningLog(LogLevelType.Error, $"{UserLogonName} - account does not exists.");
                    return false;
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserLogonName);

                IRfcFunction fnUserDetail = processorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
                var tblRoles = processorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                IRfcFunction fnRolesAssign = processorConfig.sapUtil.getRfcFunction("BAPI_USER_ACTGROUPS_ASSIGN", dicParams);

                //clear all roles
                tblRoles.Clear();

                fnRolesAssign.SetValue("ACTIVITYGROUPS", tblRoles);
                var result = processorConfig.sapUtil.ExecuteTable(fnRolesAssign, "RETURN");

                bool isUserUpdated = false;
                foreach (DataRow row in result.Rows)
                {
                    if (row["Message"].ToString().ToUpper().Contains(string.Format("Role assignment to user {0} changed", _SAPUserInfo.UserLogonName).ToUpper()) ||
                        row["Message"].ToString().ToUpper().Contains(string.Format("Role assignment to user {0} not executed completely", _SAPUserInfo.UserLogonName).ToUpper()) ||
                        row["Message"].ToString().ToUpper().Contains(string.Format("No changes were made to user {0}", _SAPUserInfo.UserLogonName).ToUpper()))
                    {
                        isUserUpdated = true;
                        break;
                    }
                }

                if (isUserUpdated)
                {
                    base.RemoveAllEntitlements(_SAPUserInfo);
                }
                else
                {
                    foreach (DataRow row in result.Rows)
                    {
                        WriteProvisioningLog(LogLevelType.Error, row["Message"].ToString());
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, ex.Message, ex);
                throw ex;
            }

            return true;
        }
        public override bool AssignPosition(IEndpointUserInfo _userInfo, string positionCode, string positionText, DateTime? startDate, DateTime? endDate, bool hasHRInfo)
        {
            LogDebugStep($"AssignPosition, positionCode:{positionCode}, startDate: {startDate}, endDate: {endDate}, hasHRInfo: {hasHRInfo}");
            bool isPositionAssigned;

            var retryTimeMilliSeconds = MasterConfigHelper.RetryTimeMilliSeconds;
            var timesShouldRetry = MasterConfigHelper.HowManyTimesShouldRetry;
            var count = 1;
            do
            {
                LogDebugStep($"Trying attempt to AssignPosition: {count}");
                //SET True because all we are using ISP. Hence ISP the flag must be true
                isPositionAssigned = AssignHROrCRMPosition(_userInfo, positionCode, startDate, endDate, true);
                count++;

                if (!isPositionAssigned && timesShouldRetry > 1)
                    Thread.Sleep(retryTimeMilliSeconds);
            }
            while (!isPositionAssigned && count <= timesShouldRetry);

            if (isPositionAssigned)
            {
                base.AssignSAPHANAPosition(_userInfo, positionCode, positionText, startDate, endDate, hasHRInfo);
            }

            return isPositionAssigned;
        }

        public override bool RemovePosition(IEndpointUserInfo _userInfo, string positionCode, DateTime? startDate, DateTime? endDate, bool hasHRInfo)
        {
            if (!IsPositionExist(_userInfo.Id, positionCode, Endpoint.Id, _userInfo.AccountName) && CurrentRequest != null && CurrentRequest.BulkRequestId.HasValue && CurrentRequest.BulkRequestId.Value > 0)
            {
                return false;
            }

            bool isPositionRemoved;

            var retryTimeMilliSeconds = MasterConfigHelper.RetryTimeMilliSeconds;
            var timesShouldRetry = MasterConfigHelper.HowManyTimesShouldRetry;
            var count = 1;
            do
            {
                LogDebugStep($"Trying attempt to RemovePosition: {count}");
                isPositionRemoved = RemoveHROrCRMPosition(_userInfo, positionCode, startDate, hasHRInfo);
                count++;

                if (!isPositionRemoved && timesShouldRetry > 1)
                    Thread.Sleep(retryTimeMilliSeconds);
            }
            while (!isPositionRemoved && count <= timesShouldRetry);

            if (isPositionRemoved)
            {
                base.RemoveSAPHANAPosition(_userInfo, positionCode, startDate, endDate, hasHRInfo);
            }

            return isPositionRemoved;
        }

        public override bool CreateSAPPhone(IEndpointUserInfo _userInfo, string taskId)
        {
            var userInfo = (SAPHANAUserinfo)_userInfo;

            if (IsSAPPhoneTaskIdExists(taskId))
            {
                return CreateSAPPhoneTask(taskId, userInfo.UserLogonName);
            }
            return false;
        }

        public override bool AssignBusinessPartner(IEndpointUserInfo _userInfo)
        {
            var userInfo = (SAPHANAUserinfo)_userInfo;

            try
            {
                string bpNumber = CreateBP(userInfo);
                if (!string.IsNullOrEmpty(bpNumber))
                {
                    AssignRole(bpNumber);
                    CreateCentralPerson(userInfo.UserLogonName, bpNumber);
                    return true;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Error, string.Format("Business partner is returned as empty for the user {0}", userInfo.UserLogonName), null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Error, string.Format("Error occured while assigning business partner for the user {0}", userInfo.UserLogonName), ex);
                return false;
            }
        }

        bool IsSAPPhoneTaskIdExists(string taskId)
        {
            try
            {
                bool isPagingEnabled = Convert.ToBoolean(Config(EndpointKey.IsPaginationEnabled, SAPConfiguration));
                if (string.IsNullOrWhiteSpace(taskId))
                    return false;

                string actualTaskId = taskId.Replace("TS", string.Empty);

                DataTable m_dtHRS1201 = null;
                string filter = "PHONE_ABLE <> '' AND OTYPE = 'TS' AND OBJID = '" + actualTaskId + "'";

                m_dtHRS1201 = processorConfig.sapUtil.ReadRFCTblAsFunction("HRS1201", null, SAPHANAConnectorFields.SAP_PHONE_FIELDS, filter, isPagingEnabled);

                if (m_dtHRS1201.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.Warning, string.Format("Task Id - {0} is not available in the system", taskId), null);
                    return false;
                }

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, string.Format("Error occured while checking the existence of task - {0}", taskId), ex);
                return false;
            }
        }

        bool CreateSAPPhoneTask(string taskId, string userId)
        {
            try
            {

                using (SAPUserGrp.SAPNEWPRO proxy = new SAPUserGrp.SAPNEWPRO())
                {
                    proxy.Connection = GetSAPConnection();
                    proxy.Connection.Open();

                    SAPUserGrp.SPH_TELUSR User_Data = new SAPUserGrp.SPH_TELUSR();
                    SAPUserGrp.ZSPH_USERATIC_TABTable Atic_Tab = new SAPUserGrp.ZSPH_USERATIC_TABTable();

                    User_Data.Teluser = userId;
                    User_Data.Calltask = "X";
                    User_Data.Starttask = taskId;
                    User_Data.Expholdtim = "00:00:00";
                    User_Data.Mailexpire = "00";

                    proxy.Zcrm_Tel_User_Data_Write(User_Data, ref Atic_Tab);

                    proxy.CommitWork();
                    proxy.Connection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while assigning creating SAPHANAphone for the user {0}", userId), ex);
                return false;
            }
        }

        string CreateBP(SAPHANAUserinfo userInfo)
        {
            try
            {
                using (SAPUserGrp.SAPNEWPRO proxy = new SAPUserGrp.SAPNEWPRO())
                {
                    proxy.Connection = GetSAPConnection();
                    proxy.Connection.Open();
                    this._idmsLogger.WriteLog(LogLevelType.Info, $"CreateBP(Bapi_Bupa_Create_From_Data) started");
                    string Accept_Error = string.Empty;
                    string Duplicate_Message_Type = string.Empty;
                    string Businesspartner = string.Empty;
                    string Businesspartnerextern = string.Empty;

                    SAPUserGrp.BAPIBUS1006_ADDRESS Addressdata = new SAPUserGrp.BAPIBUS1006_ADDRESS();
                    SAPUserGrp.BAPIBUS1006_CENTRAL_GROUP Centraldatagroup = new SAPUserGrp.BAPIBUS1006_CENTRAL_GROUP(); ;
                    SAPUserGrp.BAPIBUS1006_CENTRAL_ORGAN Centraldataorganization = new SAPUserGrp.BAPIBUS1006_CENTRAL_ORGAN();
                    SAPUserGrp.BAPIBUS1006_ADDRESS_DUPLICATESTable Addressduplicates = new SAPUserGrp.BAPIBUS1006_ADDRESS_DUPLICATESTable();
                    SAPUserGrp.BAPIAD_REMTable Addressnotes = new SAPUserGrp.BAPIAD_REMTable();
                    SAPUserGrp.BAPICOMREMTable Communicationnotes = new SAPUserGrp.BAPICOMREMTable();
                    SAPUserGrp.BAPICOMREMTable Communicationnotesnonaddress = new SAPUserGrp.BAPICOMREMTable();
                    SAPUserGrp.BAPIADUSETable Communicationusage = new SAPUserGrp.BAPIADUSETable();
                    SAPUserGrp.BAPIADUSETable Communicationusagenonaddress = new SAPUserGrp.BAPIADUSETable();
                    SAPUserGrp.BAPIADSMTPTable E_Maildata = new SAPUserGrp.BAPIADSMTPTable();
                    SAPUserGrp.BAPIADSMTPTable E_Maildatanonaddress = new SAPUserGrp.BAPIADSMTPTable();
                    SAPUserGrp.BAPIADFAXTable Faxdata = new SAPUserGrp.BAPIADFAXTable();
                    SAPUserGrp.BAPIADFAXTable Faxdatanonaddress = new SAPUserGrp.BAPIADFAXTable();
                    SAPUserGrp.BAPIADPAGTable Pagaddressdata = new SAPUserGrp.BAPIADPAGTable();
                    SAPUserGrp.BAPIADPAGTable Pagaddressdatanonaddress = new SAPUserGrp.BAPIADPAGTable();
                    SAPUserGrp.BAPIADPRTTable Prtaddressdata = new SAPUserGrp.BAPIADPRTTable();
                    SAPUserGrp.BAPIADPRTTable Prtaddressdatanonaddress = new SAPUserGrp.BAPIADPRTTable();
                    SAPUserGrp.BAPIADRFCTable Rfcaddressdata = new SAPUserGrp.BAPIADRFCTable();
                    SAPUserGrp.BAPIADRFCTable Rfcaddressdatanonaddress = new SAPUserGrp.BAPIADRFCTable();
                    SAPUserGrp.BAPIADRMLTable Rmladdressdata = new SAPUserGrp.BAPIADRMLTable();
                    SAPUserGrp.BAPIADRMLTable Rmladdressdatanonaddress = new SAPUserGrp.BAPIADRMLTable();
                    SAPUserGrp.BAPIADSSFTable Ssfaddressdata = new SAPUserGrp.BAPIADSSFTable();
                    SAPUserGrp.BAPIADSSFTable Ssfaddressdatanonaddress = new SAPUserGrp.BAPIADSSFTable();
                    SAPUserGrp.BAPIADTELTable Telefondata = new SAPUserGrp.BAPIADTELTable();
                    SAPUserGrp.BAPIADTELTable Telefondatanonaddress = new SAPUserGrp.BAPIADTELTable();
                    SAPUserGrp.BAPIADTTXTable Teletexdata = new SAPUserGrp.BAPIADTTXTable();
                    SAPUserGrp.BAPIADTTXTable Teletexdatanonaddress = new SAPUserGrp.BAPIADTTXTable();
                    SAPUserGrp.BAPIADTLXTable Telexdata = new SAPUserGrp.BAPIADTLXTable();
                    SAPUserGrp.BAPIADTLXTable Telexdatanonaddress = new SAPUserGrp.BAPIADTLXTable();
                    SAPUserGrp.BAPIADURITable Uriaddressdata = new SAPUserGrp.BAPIADURITable();
                    SAPUserGrp.BAPIADURITable Uriaddressdatanonaddress = new SAPUserGrp.BAPIADURITable();
                    SAPUserGrp.BAPIADX400Table X400addressdata = new SAPUserGrp.BAPIADX400Table();
                    SAPUserGrp.BAPIADX400Table X400addressdatanonaddress = new SAPUserGrp.BAPIADX400Table(); ;

                    string Partnercategory = "1";
                    string Partnergroup = "ZEMP";

                    SAPUserGrp.BAPIBUS1006_CENTRAL Centraldata = new SAPUserGrp.BAPIBUS1006_CENTRAL();
                    Centraldata.Searchterm1 = userInfo.Function0;

                    SAPUserGrp.BAPIBUS1006_CENTRAL_PERSON Centraldataperson = new SAPUserGrp.BAPIBUS1006_CENTRAL_PERSON();
                    Centraldataperson.Lastname = userInfo.Lastname;

                    SAPUserGrp.BAPIRET2Table Return0 = new SAPUserGrp.BAPIRET2Table();

                    proxy.Bapi_Bupa_Create_From_Data(Accept_Error, Addressdata, Businesspartnerextern, Centraldata,
                        Centraldatagroup, Centraldataorganization, Centraldataperson, Duplicate_Message_Type, Partnercategory,
                         Partnergroup, out Businesspartner, ref Addressduplicates, ref Addressnotes, ref Communicationnotes,
                         ref Communicationnotesnonaddress, ref Communicationusage, ref Communicationusagenonaddress,
                         ref E_Maildata, ref E_Maildatanonaddress, ref Faxdata, ref Faxdatanonaddress,
                         ref Pagaddressdata, ref Pagaddressdatanonaddress, ref Prtaddressdata,
                         ref Prtaddressdatanonaddress, ref Return0, ref Rfcaddressdata, ref Rfcaddressdatanonaddress,
                         ref Rmladdressdata, ref Rmladdressdatanonaddress, ref Ssfaddressdata, ref Ssfaddressdatanonaddress,
                         ref Telefondata, ref Telefondatanonaddress, ref Teletexdata, ref Teletexdatanonaddress, ref Telexdata,
                         ref Telexdatanonaddress, ref Uriaddressdata, ref Uriaddressdatanonaddress, ref X400addressdata,
                         ref X400addressdatanonaddress);

                    proxy.CommitWork();
                    proxy.Connection.Close();
                    System.Threading.Thread.Sleep(10000);
                    this._idmsLogger.WriteLog(LogLevelType.Info, $"CreateBP(Bapi_Bupa_Create_From_Data) completed.Businesspartner:{Businesspartner}");
                    return Businesspartner;

                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, $"CreateBP(Bapi_Bupa_Create_From_Data) failed.{ex}", ex);
                throw ex;
            }
        }

        void AssignRole(string bpNumber)
        {
            try
            {
                using (SAPUserGrp.SAPNEWPRO proxy = new SAPUserGrp.SAPNEWPRO())
                {
                    proxy.Connection = GetSAPConnection();
                    proxy.Connection.Open();
                    this._idmsLogger.WriteLog(LogLevelType.Info, $"AssignRole(Bapi_Bupa_Role_Add) started");
                    SAPUserGrp.BAPIRET2Table Return0 = new SAPUserGrp.BAPIRET2Table();

                    proxy.Bapi_Bupa_Role_Add(bpNumber, "BUP003", string.Empty, ref Return0);

                    proxy.CommitWork();
                    proxy.Connection.Close();
                    System.Threading.Thread.Sleep(10000);
                    this._idmsLogger.WriteLog(LogLevelType.Info, $"AssignRole(Bapi_Bupa_Role_Add) completed");
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, $"AssignRole(Bapi_Bupa_Role_Add - BUP003) failed.{ex}", ex);
                throw ex;
            }
        }

        void CreateCentralPerson(string userId, string bpNumber)
        {
            try
            {
                using (SAPUserGrp.SAPNEWPRO proxy = new SAPUserGrp.SAPNEWPRO())
                {
                    this._idmsLogger.WriteLog(LogLevelType.Info, $"CreateCentralPerson(Zcrm_Bupa_Createcentralperson):bpNumber{bpNumber} - userId:{userId} started");
                    proxy.Connection = GetSAPConnection();
                    proxy.Connection.Open();

                    int retryCount = 0;

                    while (retryCount < 5)
                    {
                        string Iv_Asynchron = string.Empty;
                        string Iv_Name = string.Empty;
                        string Iv_No_Commit = string.Empty;
                        string Ev_Person_Id = string.Empty;
                        int Sysubrc = 0;

                        string Iv_Businesspartner = bpNumber;
                        string Iv_User_Id = userId;
                        byte[] Iv_Bu_Partner_Guid = Guid.Empty.ToByteArray();
                        string Iv_Employee_Id = "00000000";

                        proxy.Zcrm_Bupa_Createcentralperson(Iv_Asynchron, Iv_Bu_Partner_Guid, Iv_Businesspartner, Iv_Employee_Id, Iv_Name, Iv_No_Commit, Iv_User_Id, out Ev_Person_Id, out Sysubrc);

                        if (Sysubrc == 0)
                        {
                            proxy.CommitWork();
                            this._idmsLogger.WriteLog(LogLevelType.Info, $"CreateCentralPerson(Zcrm_Bupa_Createcentralperson) completed");
                            return;
                        }
                        else
                        {
                            if (retryCount == 4)
                            {
                                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Central person creation for the user - {0} is failed.", userId));
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(2000);
                            }
                        }
                        retryCount++;
                    }
                    proxy.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, $"CreateCentralPerson(Zcrm_Bupa_Createcentralperson) failed.{ex}", ex);
                throw ex;
            }
        }

        List<string> GetPositionsByUser(string userLogonName)
        {
            var lstPostions = new List<string>();
            try
            {
                bool isPagingEnabled = Convert.ToBoolean(Config(EndpointKey.IsPaginationEnabled, SAPConfiguration));
                string filter = $"SOBID EQ '{userLogonName}' AND OTYPE EQ 'S' AND SCLAS EQ 'US'";
                var dtResult = processorConfig.sapUtil.ReadRFCTblAsFunction("HRP1001", null, null, filter, isPagingEnabled);

                string positionCode = string.Empty;
                foreach (DataRow row in dtResult.Rows)
                {
                    positionCode = row["OBJID"].ToString().Trim();
                    if (!lstPostions.Contains(positionCode))
                        lstPostions.Add(positionCode);
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }

            return lstPostions;
        }

        bool ExtendPositionValidity(IEndpointUserInfo _userInfo, string positionCode, bool hasHRInfo)
        {
            LogDebugStep("AssignHRPosition");
            bool bReturn = false;

            var userInfo = (SAPHANAUserinfo)_userInfo;
            LogDebugStep($"userInfo is null: {userInfo == null} ");
            var acctRecon = AccountReconcilation.GetByUser(Endpoint.Id, userInfo.Id, false);
            if (acctRecon == null || !acctRecon.UserId.HasValue)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, $"No account found for the user: {userInfo.UserLogonName}");
                return false;
            }

            var user = DomainModel.User.GetById(acctRecon.UserId.Value);

            if (user == null || !user.TerminationDate.HasValue)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, $"No termination date available for the user: {userInfo.UserLogonName}");
                return false;
            }
            try
            {

                if (userInfo.IsUserLocked)
                {
                    LogDebugStep("userInfo.IsUserLocked");
                    if (CurrentRequest.AdHocAccessRequest != null && (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", userInfo.AccountName));
                        return false;
                    }
                }

                SAPUserGrp.BAPIHRINF hrInfo = new SAPUserGrp.BAPIHRINF();
                SAPUserGrp.BAPIHROBJ hrObj = new SAPUserGrp.BAPIHROBJ();
                List<SAPUserGrp.BAPIHR1001> hr1001s = new List<SAPUserGrp.BAPIHR1001>();
                SAPUserGrp.BAPIHR1001 hr1001 = null;

                LogDebugStep("SAPUserGrp.BAPIHR1001");

                hrObj.Plan_Ver = "01";
                hrObj.Obj_Type = "S";
                hrObj.Obj_Id = positionCode;
                hrObj.Proof = "X";
                hrObj.Operator = "U";

                hrInfo.Plan_Ver = "01";
                hrInfo.Obj_Type = "S";
                hrInfo.Obj_Id = positionCode;
                hrInfo.Infotype = "1001";
                hrInfo.Subtype = "A008";
                hrInfo.From_Date = "19000101";
                hrInfo.To_Date = "99991231";

                var posUsers = GetExistingUsersByPosition(positionCode);
                LogDebugStep("posUsers");
                var client = Config(EndpointKey.ClientNumber, SAPConfiguration);
                var username = Config(EndpointKey.UserName, SAPConfiguration);
                LogDebugStep($"client:{client}, username: {username}");
                foreach (SAPHANAExistingPositionUserInfo posUser in posUsers)
                {
                    if (posUser.SUBTY == "A008")
                    {
                        hr1001 = new SAPUserGrp.BAPIHR1001();

                        hr1001.Client = client;
                        hr1001.Plan_Ver = "01";
                        hr1001.Obj_Type = "S";
                        hr1001.Obj_Id = positionCode;
                        hr1001.Infotype = "1001";
                        hr1001.Subtype = "A008";
                        hr1001.Pl_Status = "1";
                        hr1001.Priority = "";
                        //LogDebugStep($"Inside foreach valid from:{posUser.ValidFrom}, valid to: {posUser.ValidTo}");
                        hr1001.From_Date = posUser.ValidFrom;
                        if (posUser.UserName.Equals(userInfo.UserLogonName, StringComparison.InvariantCultureIgnoreCase) && posUser.SUBTY == "A008")
                            hr1001.To_Date = user.TerminationDate.Value.ToString("yyyyMMdd");
                        else
                            hr1001.To_Date = posUser.ValidTo;
                        hr1001.Var_Field = "US" + posUser.UserName;
                        hr1001.It_Rec_No = "000";
                        hr1001.Changed_By = username;
                        hr1001.Reason = "";
                        hr1001.Hist_Flag = "";
                        hr1001.It_Txt_Mod = "00000000";
                        hr1001.Rel_Obj_Ty = "US";
                        hr1001.Rel_Obj_Id = posUser.UserName;
                        hr1001.Percentage = 100;

                        hr1001s.Add(hr1001);
                    }
                }

                Hr_Infotype_Header.Add(hrInfo);
                Hr_Object_Header.Add(hrObj);

                LogDebugStep("before foreach hr1001s");

                foreach (SAPUserGrp.BAPIHR1001 item in hr1001s)
                {
                    Pd_Object_Relationships.Add(item);
                }

                using (SAPUserGrp.SAPNEWPRO proxy = new SAPUserGrp.SAPNEWPRO())
                {
                    LogDebugStep("SAPUserGrp.SAPNEWPRO proxy");
                    proxy.Connection = GetSAPConnection();
                    proxy.Connection.Open();

                    LogDebugStep($"Connection Opened, hasHRInfo: {hasHRInfo}");

                    if (hasHRInfo)
                    {
                        proxy.Bapi_Hrmaster_Save_Repl_Mult(ref Employee_Archived_Objects, ref Employee_Bank_Details,
                                                        ref Employee_Cats_Default_Values, ref Employee_Communications, ref Employee_Company_Cars,
                                                        ref Employee_Org_Assignment, ref Employee_Payroll_Status,
                                                        ref Employee_Personal_Data, ref Employee_Personnel_Action, ref Employee_Private_Address, ref Employee_Sales_Org_Assignment, ref Employee_Time_Recording_Info,
                                                        ref Employee_Trip_Privileges, ref Hr_Infotype_Header, ref Hr_Object_Header,
                                                        ref Pd_Object, ref Pd_Object_Address, ref Pd_Object_Appraisal_Sample, ref Pd_Object_Appraisal_Scale, ref Pd_Object_Authority_Resources,
                                                        ref Pd_Object_B_Event_Availability, ref Pd_Object_B_Event_Blocks, ref Pd_Object_B_Event_Capacity,
                                                        ref Pd_Object_B_Event_Costs, ref Pd_Object_B_Event_Demand, ref Pd_Object_B_Event_Info, ref Pd_Object_B_Event_Name_Format,
                                                        ref Pd_Object_B_Event_Price, ref Pd_Object_B_Event_Procedure, ref Pd_Object_B_Event_Sched_Model,
                                                        ref Pd_Object_B_Event_Schedule, ref Pd_Object_B_Event_Type, ref Pd_Object_B_Eventknowledgelink, ref Pd_Object_Cost_Alloc_Invoice,
                                                        ref Pd_Object_Cost_Distribution, ref Pd_Object_Cost_Planning, ref Pd_Object_Department_Staff, ref Pd_Object_Description,
                                                        ref Pd_Object_Empl_Group_Subgroup, ref Pd_Object_External_Key, ref Pd_Object_Head_Count_Budget, ref Pd_Object_Health_Examinations,
                                                        ref Pd_Object_Job_Market_Info, ref Pd_Object_Mail_Address, ref Pd_Object_Obsolete, ref Pd_Object_Override_Reqmnt,
                                                        ref Pd_Object_Pd_Profiles, ref Pd_Object_Planned_Compensation, ref Pd_Object_Posting_Specs, ref Pd_Object_Processing_Fm,
                                                        ref Pd_Object_Proficiency_Desc, ref Pd_Object_Relationships, ref Pd_Object_Restrictions, ref Pd_Object_Room_Reserv_Info, ref Pd_Object_Shift_Group,
                                                        ref Pd_Object_Site_Dependent_Info, ref Pd_Object_Standard_Profiles, ref Pd_Object_Task_Character, ref Pd_Object_Vacancy,
                                                        ref Pd_Object_Validity, ref Pd_Object_Valuation, ref Pd_Object_Work_Evaluat_Result, ref Pd_Object_Work_Reqmnt_Scale, ref Pd_Object_Work_Schedule,
                                                        ref Pd_Rel_Application_For, ref Pd_Rel_Appraisal_Model, ref Pd_Rel_Appraises, ref Pd_Rel_Career_Develop_Action,
                                                        ref Pd_Rel_Is_Equipped, ref Pd_Rel_Is_Prebooked_For, ref Pd_Rel_Job_Reqmnts, ref Pd_Rel_Message_Type, ref Pd_Rel_Qualifications_Reqmnts,
                                                        ref Pd_Rel_Requires, ref Pd_Rel_Reserves, ref Pd_Rel_Session, ref Pd_Rel_Specified_Substitute,
                                                        ref Pd_Rel_Still_Requires, ref Pd_Rel_Takes_Part_In, ref Return0);
                    }
                    else
                    {
                        proxy.Bapi_Orgmaster_Save_Repl_Mult(ref Hr_Infotype_Header, ref Hr_Object_Header, ref Pd_Object, ref Pd_Object_Address,
                             ref Pd_Object_Description, ref Pd_Object_Mail_Address, ref Pd_Object_Pd_Profiles, ref Pd_Object_Relationships,
                             ref Pd_Object_Standard_Profiles, ref Pd_Rel_Message_Type, ref Pd_Rel_Specified_Substitute, ref Return0);
                    }

                    LogDebugStep("proxy.Bapi_Hrmaster_Save_Repl_Mult completed");
                    DataTable Result = Return0.ToADODataTable();
                    LogDebugStep($"Return0.ToADODataTable : {Result == null}");
                    if (Result != null)
                        LogDebugStep($"Return0.ToADODataTable : {Result.Rows.Count}");

                    proxy.Connection.Close();

                    if (Result.Rows.Count == 0)
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("Position {0} Assgined successfully to user {1}", positionCode, userInfo.UserLogonName));
                        ExtendPositionValidity(_userInfo, positionCode, user.TerminationDate.Value);
                        bReturn = true;
                    }
                    else
                    {
                        GetAndSaveSAPMessage(Result);
                    }
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while assigning position {0} to user {1}", positionCode, userInfo.UserLogonName), ex);
                bReturn = false;
            }

            return bReturn;
        }

        bool AssignHROrCRMPosition(IEndpointUserInfo _userInfo, string positionCode, DateTime? startDate, DateTime? endDate, bool hasHRInfo)
        {
            LogDebugStep("AssignHRPosition");
            bool bReturn = false;

            var userInfo = (SAPHANAUserinfo)_userInfo;
            LogDebugStep($"userInfo is null: {userInfo == null} ");
            SAPUserGrp.BAPIHRINF hrInfo = null;
            SAPUserGrp.BAPIHROBJ hrObj = null;
            List<SAPUserGrp.BAPIHR1001> hr1001s = null;
            SAPUserGrp.BAPIHR1001 hr1001 = null;

            try
            {

                if (userInfo.IsUserLocked)
                {
                    LogDebugStep("userInfo.IsUserLocked");
                    if (CurrentRequest.AdHocAccessRequest != null && (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", userInfo.AccountName));
                        return false;
                    }
                }

                hrInfo = new SAPUserGrp.BAPIHRINF();
                hrObj = new SAPUserGrp.BAPIHROBJ();
                hr1001s = new List<SAPUserGrp.BAPIHR1001>();


                LogDebugStep("SAPUserGrp.BAPIHR1001");

                hrObj.Plan_Ver = "01";
                hrObj.Obj_Type = "S";
                hrObj.Obj_Id = positionCode;
                hrObj.Proof = "X";
                hrObj.Operator = "U";

                hrInfo.Plan_Ver = "01";
                hrInfo.Obj_Type = "S";
                hrInfo.Obj_Id = positionCode;
                hrInfo.Infotype = "1001";
                hrInfo.Subtype = "A008";
                hrInfo.From_Date = "19000101";
                hrInfo.To_Date = "99991231";

                var posUsers = GetExistingUsersByPosition(positionCode);
                LogDebugStep("posUsers");
                var client = Config(EndpointKey.ClientNumber, SAPConfiguration);
                var username = Config(EndpointKey.UserName, SAPConfiguration);
                LogDebugStep($"client:{client}, username: {username}");
                foreach (SAPHANAExistingPositionUserInfo posUser in posUsers)
                {
                    if (posUser.SUBTY == "A008")
                    {
                        hr1001 = new SAPUserGrp.BAPIHR1001();

                        hr1001.Client = client;
                        hr1001.Plan_Ver = "01";
                        hr1001.Obj_Type = "S";
                        hr1001.Obj_Id = positionCode;
                        hr1001.Infotype = "1001";
                        hr1001.Subtype = "A008";
                        hr1001.Pl_Status = "1";
                        hr1001.Priority = "";
                        //LogDebugStep($"Inside foreach valid from:{posUser.ValidFrom}, valid to: {posUser.ValidTo}");
                        hr1001.From_Date = posUser.ValidFrom;
                        hr1001.To_Date = posUser.ValidTo;
                        hr1001.Var_Field = "US" + posUser.UserName;
                        hr1001.It_Rec_No = "000";
                        hr1001.Changed_By = username;
                        hr1001.Reason = "";
                        hr1001.Hist_Flag = "";
                        hr1001.It_Txt_Mod = "00000000";
                        hr1001.Rel_Obj_Ty = "US";
                        hr1001.Rel_Obj_Id = posUser.UserName;
                        hr1001.Percentage = 100;

                        hr1001s.Add(hr1001);
                    }

                    
                }

                hr1001 = new SAPUserGrp.BAPIHR1001();

                hr1001.Client = client;
                hr1001.Plan_Ver = "01";
                hr1001.Obj_Type = "S";
                hr1001.Obj_Id = positionCode;
                hr1001.Infotype = "1001";
                hr1001.Subtype = "A008";
                hr1001.Pl_Status = "1";
                hr1001.Priority = "";
                //LogDebugStep($"Outside foreach - 1 valid from:{startDate}, valid to: {endDate}");
                hr1001.From_Date = startDate.HasValue ? startDate.Value.ToString("yyyyMMdd") : DateTime.Now.ToString("yyyyMMdd");
                hr1001.To_Date = endDate.HasValue ? endDate.Value.ToString("yyyyMMdd") : "99991231";
                LogDebugStep($"Outside foreach valid from:{hr1001.From_Date}, valid to: {hr1001.To_Date}");

                hr1001.Var_Field = "US" + userInfo.UserLogonName;
                hr1001.It_Rec_No = "000";
                hr1001.Changed_By = username;
                hr1001.Reason = "";
                hr1001.Hist_Flag = "";
                hr1001.It_Txt_Mod = "00000000";
                hr1001.Rel_Obj_Ty = "US";
                hr1001.Rel_Obj_Id = userInfo.UserLogonName;
                hr1001.Percentage = 100;

                hr1001s.Add(hr1001);

                Hr_Infotype_Header.Add(hrInfo);
                Hr_Object_Header.Add(hrObj);

                LogDebugStep("before foreach hr1001s");

                foreach (SAPUserGrp.BAPIHR1001 item in hr1001s)
                {
                    Pd_Object_Relationships.Add(item);
                }

                using (SAPUserGrp.SAPNEWPRO proxy = new SAPUserGrp.SAPNEWPRO())
                {
                    LogDebugStep("SAPUserGrp.SAPNEWPRO proxy");
                    proxy.Connection = GetSAPConnection();
                    proxy.Connection.Open();

                    LogDebugStep($"Connection Opened, hasHRInfo: {hasHRInfo}");

                    if (hasHRInfo)
                    {
                        proxy.Bapi_Hrmaster_Save_Repl_Mult(ref Employee_Archived_Objects, ref Employee_Bank_Details,
                                                        ref Employee_Cats_Default_Values, ref Employee_Communications, ref Employee_Company_Cars,
                                                        ref Employee_Org_Assignment, ref Employee_Payroll_Status,
                                                        ref Employee_Personal_Data, ref Employee_Personnel_Action, ref Employee_Private_Address, ref Employee_Sales_Org_Assignment, ref Employee_Time_Recording_Info,
                                                        ref Employee_Trip_Privileges, ref Hr_Infotype_Header, ref Hr_Object_Header,
                                                        ref Pd_Object, ref Pd_Object_Address, ref Pd_Object_Appraisal_Sample, ref Pd_Object_Appraisal_Scale, ref Pd_Object_Authority_Resources,
                                                        ref Pd_Object_B_Event_Availability, ref Pd_Object_B_Event_Blocks, ref Pd_Object_B_Event_Capacity,
                                                        ref Pd_Object_B_Event_Costs, ref Pd_Object_B_Event_Demand, ref Pd_Object_B_Event_Info, ref Pd_Object_B_Event_Name_Format,
                                                        ref Pd_Object_B_Event_Price, ref Pd_Object_B_Event_Procedure, ref Pd_Object_B_Event_Sched_Model,
                                                        ref Pd_Object_B_Event_Schedule, ref Pd_Object_B_Event_Type, ref Pd_Object_B_Eventknowledgelink, ref Pd_Object_Cost_Alloc_Invoice,
                                                        ref Pd_Object_Cost_Distribution, ref Pd_Object_Cost_Planning, ref Pd_Object_Department_Staff, ref Pd_Object_Description,
                                                        ref Pd_Object_Empl_Group_Subgroup, ref Pd_Object_External_Key, ref Pd_Object_Head_Count_Budget, ref Pd_Object_Health_Examinations,
                                                        ref Pd_Object_Job_Market_Info, ref Pd_Object_Mail_Address, ref Pd_Object_Obsolete, ref Pd_Object_Override_Reqmnt,
                                                        ref Pd_Object_Pd_Profiles, ref Pd_Object_Planned_Compensation, ref Pd_Object_Posting_Specs, ref Pd_Object_Processing_Fm,
                                                        ref Pd_Object_Proficiency_Desc, ref Pd_Object_Relationships, ref Pd_Object_Restrictions, ref Pd_Object_Room_Reserv_Info, ref Pd_Object_Shift_Group,
                                                        ref Pd_Object_Site_Dependent_Info, ref Pd_Object_Standard_Profiles, ref Pd_Object_Task_Character, ref Pd_Object_Vacancy,
                                                        ref Pd_Object_Validity, ref Pd_Object_Valuation, ref Pd_Object_Work_Evaluat_Result, ref Pd_Object_Work_Reqmnt_Scale, ref Pd_Object_Work_Schedule,
                                                        ref Pd_Rel_Application_For, ref Pd_Rel_Appraisal_Model, ref Pd_Rel_Appraises, ref Pd_Rel_Career_Develop_Action,
                                                        ref Pd_Rel_Is_Equipped, ref Pd_Rel_Is_Prebooked_For, ref Pd_Rel_Job_Reqmnts, ref Pd_Rel_Message_Type, ref Pd_Rel_Qualifications_Reqmnts,
                                                        ref Pd_Rel_Requires, ref Pd_Rel_Reserves, ref Pd_Rel_Session, ref Pd_Rel_Specified_Substitute,
                                                        ref Pd_Rel_Still_Requires, ref Pd_Rel_Takes_Part_In, ref Return0);
                    }
                    else
                    {
                        proxy.Bapi_Orgmaster_Save_Repl_Mult(ref Hr_Infotype_Header, ref Hr_Object_Header, ref Pd_Object, ref Pd_Object_Address,
                             ref Pd_Object_Description, ref Pd_Object_Mail_Address, ref Pd_Object_Pd_Profiles, ref Pd_Object_Relationships,
                             ref Pd_Object_Standard_Profiles, ref Pd_Rel_Message_Type, ref Pd_Rel_Specified_Substitute, ref Return0);
                    }

                    LogDebugStep("proxy.Bapi_Hrmaster_Save_Repl_Mult completed");
                    DataTable Result = Return0.ToADODataTable();
                    LogDebugStep($"Return0.ToADODataTable : {Result == null}");
                    if (Result != null)
                        LogDebugStep($"Return0.ToADODataTable : {Result.Rows.Count}");

                    proxy.Connection.Close();

                    if (Result.Rows.Count == 0)
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("Position {0} Assgined successfully to user {1}", positionCode, userInfo.UserLogonName));
                        bReturn = true;
                    }
                    else
                    {
                        GetAndSaveSAPMessage(Result);
                    }
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while assigning position {0} to user {1}", positionCode, userInfo.UserLogonName), ex);
                bReturn = false;
            }
            finally
            {
                hrInfo = null;
                hrObj = null;
                hr1001s?.Clear();
                hr1001s = null;
                hr1001 = null;
            }
            return bReturn;
        }

        void GetAndSaveSAPMessage(DataTable Result)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Result.Rows.Count; i++)
                {
                    sb.Append($"Row:{i}, message: {Convert.ToString(Result.Rows[i]["Message"])}");
                }
                this._idmsLogger.WriteLog(LogLevelType.Error, sb.ToString());
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
        }

        private SAPConnection GetSAPConnection()
        {
            Destination destination = new Destination();
            var host = Config(EndpointKey.HostName, SAPConfiguration);
            var client = Config(EndpointKey.ClientNumber, SAPConfiguration);
            var pwdC = RSAHelper.DecryptData(Config(EndpointKey.Password, SAPConfiguration));
            var systemNumber = Config(EndpointKey.SystemNumber, SAPConfiguration);
            var username = Config(EndpointKey.UserName, SAPConfiguration);
            var connectionString = $"EndpointSystemId: {Endpoint.Id}, EndpointConfiguration: {SAPConfiguration.EndpointSystemId} ASHOST={host} SYSNR={systemNumber} CLIENT={client} USER={username} PASSWD={pwdC}";
            LogDebugStep($"SAPHANA connection string:{connectionString}");
            destination.AppServerHost = host; //Configuration.IPAddress;10.168.15.24";//
            destination.Client = Convert.ToInt16(client); //Configuration.IntClientNum; 300;//
            destination.Password = pwdC; //Configuration.Password;"orange123"; //
            destination.SystemNumber = Convert.ToInt16(systemNumber); //Configuration.SystemNumber;00;//
            destination.Username = username; //Configuration.UserName;sisuis";//"
            //destination.ConnectionString = connectionString;

            string sncMode = Config(EndpointKey.SNCMode, SAPConfiguration);
            if (!string.IsNullOrWhiteSpace(sncMode) && Convert.ToBoolean(sncMode) == true)
            {
                destination.SNCMode = Convert.ToBoolean(sncMode);
                destination.SNCLib = Config(EndpointKey.SNCLib, SAPConfiguration);
                destination.SNCQop = Convert.ToInt16(Config(EndpointKey.SNCQop, SAPConfiguration));
                destination.SNCPartnerName = Config(EndpointKey.SNCPartnerName, SAPConfiguration);
            }

            SAPConnection connection = new SAPConnection(destination);
            return connection;
        }

        //bool AssignCRMPosition(IEndpointUserInfo _userInfo, string positionCode, DateTime? startDate, DateTime? endDate)
        //{
        //    bool bReturn = false;

        //    SAPUserinfo userInfo = (SAPUserinfo)_userInfo;

        //    try
        //    {

        //        if (userInfo.IsUserLocked)
        //        {
        //            if (CurrentRequest.AdHocAccessRequest != null && (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
        //            {
        //                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", userInfo.AccountName));
        //                return false;
        //            }
        //        }

        //        SAPUserGrp.BAPIHRINF hrInfo = new SAPUserGrp.BAPIHRINF();
        //        SAPUserGrp.BAPIHROBJ hrObj = new SAPUserGrp.BAPIHROBJ();
        //        List<SAPUserGrp.BAPIHR1001> hr1001s = new List<SAPUserGrp.BAPIHR1001>();
        //        SAPUserGrp.BAPIHR1001 hr1001 = null;

        //        hrObj.Plan_Ver = "01";
        //        hrObj.Obj_Type = "S";
        //        hrObj.Obj_Id = positionCode;
        //        hrObj.Proof = "X";
        //        hrObj.Operator = "U";

        //        hrInfo.Plan_Ver = "01";
        //        hrInfo.Obj_Type = "S";
        //        hrInfo.Obj_Id = positionCode;
        //        hrInfo.Infotype = "1001";
        //        hrInfo.Subtype = "A008";
        //        hrInfo.From_Date = "19000101";
        //        hrInfo.To_Date = "99991231";

        //        var posUsers = GetExistingUsersByPosition(positionCode);
        //        var client = Config(EndpointKey.ClientNumber, SAPConfiguration);
        //        var username = Config(EndpointKey.UserName, SAPConfiguration);
        //        foreach (ExistingPositionUserInfo posUser in posUsers)
        //        {
        //            hr1001 = new SAPUserGrp.BAPIHR1001();

        //            hr1001.Client = client;
        //            hr1001.Plan_Ver = "01";
        //            hr1001.Obj_Type = "S";
        //            hr1001.Obj_Id = positionCode;
        //            hr1001.Infotype = "1001";
        //            hr1001.Subtype = "A008";
        //            hr1001.Pl_Status = "1";
        //            hr1001.Priority = "";
        //            hr1001.From_Date = posUser.ValidFrom;
        //            hr1001.To_Date = posUser.ValidTo;
        //            hr1001.Var_Field = "US" + posUser.UserName;
        //            hr1001.It_Rec_No = "000";
        //            hr1001.Changed_By = username;
        //            hr1001.Reason = "";
        //            hr1001.Hist_Flag = "";
        //            hr1001.It_Txt_Mod = "00000000";
        //            hr1001.Rel_Obj_Ty = "US";
        //            hr1001.Rel_Obj_Id = posUser.UserName;
        //            hr1001.Percentage = 100;

        //            hr1001s.Add(hr1001);
        //        }

        //        hr1001 = new SAPUserGrp.BAPIHR1001();

        //        hr1001.Client = client;
        //        hr1001.Plan_Ver = "01";
        //        hr1001.Obj_Type = "S";
        //        hr1001.Obj_Id = positionCode;
        //        hr1001.Infotype = "1001";
        //        hr1001.Subtype = "A008";
        //        hr1001.Pl_Status = "1";
        //        hr1001.Priority = "";
        //        hr1001.From_Date = startDate.HasValue ? startDate.Value.ToString("yyyyMMdd") : DateTime.Now.ToString("yyyyMMdd");
        //        hr1001.To_Date = endDate.HasValue ? endDate.Value.ToString("yyyyMMdd") : "99991231";
        //        hr1001.Var_Field = "US" + userInfo.UserLogonName;
        //        hr1001.It_Rec_No = "000";
        //        hr1001.Changed_By = username;
        //        hr1001.Reason = "";
        //        hr1001.Hist_Flag = "";
        //        hr1001.It_Txt_Mod = "00000000";
        //        hr1001.Rel_Obj_Ty = "US";
        //        hr1001.Rel_Obj_Id = userInfo.UserLogonName;
        //        hr1001.Percentage = 100;

        //        hr1001s.Add(hr1001);

        //        Hr_Infotype_Header.Add(hrInfo);
        //        Hr_Object_Header.Add(hrObj);

        //        foreach (SAPUserGrp.BAPIHR1001 item in hr1001s)
        //        {
        //            Pd_Object_Relationships.Add(item);
        //        }

        //        using (SAPUserGrp.SAPNEWPRO proxy = new SAPUserGrp.SAPNEWPRO())
        //        {
        //            proxy.Connection = GetSAPConnection();
        //            proxy.Connection.Open();

        //            proxy.Bapi_Orgmaster_Save_Repl_Mult(ref Hr_Infotype_Header, ref Hr_Object_Header, ref Pd_Object, ref Pd_Object_Address,
        //                      ref Pd_Object_Description, ref Pd_Object_Mail_Address, ref Pd_Object_Pd_Profiles, ref Pd_Object_Relationships,
        //                      ref Pd_Object_Standard_Profiles, ref Pd_Rel_Message_Type, ref Pd_Rel_Specified_Substitute, ref Return0);

        //            DataTable Result = Return0.ToADODataTable();
        //            proxy.Connection.Close();
        //            if (Result.Rows.Count == 0)
        //            {
        //                this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("Position {0} Assgined successfully to user {1}", positionCode, userInfo.UserLogonName));
        //                bReturn = true;
        //            }
        //            else
        //            {
        //                GetAndSaveSAPMessage(Result);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while assigning position {0} to user {1}", positionCode, userInfo.UserLogonName), ex);
        //        bReturn = false;
        //    }

        //    return bReturn;
        //}

        bool RemoveHROrCRMPosition(IEndpointUserInfo _userInfo, string positionCode, DateTime? endDate, bool hasHRInfo)
        {
            bool bReturn = false;

            SAPHANAUserinfo userInfo = (SAPHANAUserinfo)_userInfo;

            try
            {

                if (userInfo.IsUserLocked)
                {
                    if (CurrentRequest.AdHocAccessRequest != null && (CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", userInfo.AccountName));
                        return false;
                    }
                }

                SAPUserGrp.BAPIHRINF hrInfo = new SAPUserGrp.BAPIHRINF();
                SAPUserGrp.BAPIHROBJ hrObj = new SAPUserGrp.BAPIHROBJ();
                List<SAPUserGrp.BAPIHR1001> hr1001s = new List<SAPUserGrp.BAPIHR1001>();
                SAPUserGrp.BAPIHR1001 hr1001 = null;

                hrObj.Plan_Ver = "01";
                hrObj.Obj_Type = "S";
                hrObj.Obj_Id = positionCode;
                hrObj.Proof = "X";
                hrObj.Operator = "U";

                hrInfo.Plan_Ver = "01";
                hrInfo.Obj_Type = "S";
                hrInfo.Obj_Id = positionCode;
                hrInfo.Infotype = "1001";
                hrInfo.Subtype = "A008";
                hrInfo.From_Date = "19000101";
                hrInfo.To_Date = "99991231";

                var posUsers = GetExistingUsersByPosition(positionCode);
                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"positionCode:{positionCode}, posUsers count: {posUsers.Count}");

                var client = Config(EndpointKey.ClientNumber, SAPConfiguration);
                var username = Config(EndpointKey.UserName, SAPConfiguration);
                foreach (SAPHANAExistingPositionUserInfo posUser in posUsers)
                {

                    if (posUser.SUBTY == "A008")
                    {
                        hr1001 = new SAPUserGrp.BAPIHR1001();

                        hr1001.Client = client;
                        hr1001.Plan_Ver = "01";
                        hr1001.Obj_Type = "S";
                        hr1001.Obj_Id = positionCode;
                        hr1001.Infotype = "1001";
                        hr1001.Subtype = "A008";
                        hr1001.Pl_Status = "1";
                        hr1001.Priority = "";
                        hr1001.From_Date = posUser.ValidFrom;
                        hr1001.To_Date = posUser.ValidTo;

                        this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"userInfo.UserLogonName:{userInfo.UserLogonName}, posUser.UserName: {posUser.UserName}");
                        if (userInfo.UserLogonName.Equals(posUser.UserName, StringComparison.CurrentCultureIgnoreCase) && posUser.SUBTY == "A008")
                        {
                            DateTime tmpEndDate = UtilityMethods.GetDateFromString(posUser.ValidTo);
                            this._idmsLogger.WriteLog(LogLevelType.Info, $"tmpEndDate:{tmpEndDate}, endDate.Value: {endDate.Value}");
                            if (tmpEndDate >= endDate.Value)
                            {
                                hr1001.To_Date = endDate.Value.ToString("yyyyMMdd");
                                this._idmsLogger.WriteLog(LogLevelType.DebugStep, $"hr1001.To_Date: {hr1001.To_Date}");
                            }
                        }

                        hr1001.Var_Field = "US" + posUser.UserName;
                        hr1001.It_Rec_No = "000";
                        hr1001.Changed_By = username;
                        hr1001.Reason = "";
                        hr1001.Hist_Flag = "";
                        hr1001.It_Txt_Mod = "00000000";
                        hr1001.Rel_Obj_Ty = "US";
                        hr1001.Rel_Obj_Id = posUser.UserName;
                        hr1001.Percentage = 100;

                        hr1001s.Add(hr1001);
                    }
                }

                Hr_Infotype_Header.Add(hrInfo);
                Hr_Object_Header.Add(hrObj);

                foreach (SAPUserGrp.BAPIHR1001 item in hr1001s)
                {
                    Pd_Object_Relationships.Add(item);
                }

                using (SAPUserGrp.SAPNEWPRO proxy = new SAPUserGrp.SAPNEWPRO())
                {
                    proxy.Connection = GetSAPConnection();
                    proxy.Connection.Open();

                    if (hasHRInfo)
                    {
                        proxy.Bapi_Hrmaster_Save_Repl_Mult(ref Employee_Archived_Objects, ref Employee_Bank_Details,
                                                        ref Employee_Cats_Default_Values, ref Employee_Communications, ref Employee_Company_Cars,
                                                        ref Employee_Org_Assignment, ref Employee_Payroll_Status,
                                                        ref Employee_Personal_Data, ref Employee_Personnel_Action, ref Employee_Private_Address, ref Employee_Sales_Org_Assignment, ref Employee_Time_Recording_Info,
                                                        ref Employee_Trip_Privileges, ref Hr_Infotype_Header, ref Hr_Object_Header,
                                                        ref Pd_Object, ref Pd_Object_Address, ref Pd_Object_Appraisal_Sample, ref Pd_Object_Appraisal_Scale, ref Pd_Object_Authority_Resources,
                                                        ref Pd_Object_B_Event_Availability, ref Pd_Object_B_Event_Blocks, ref Pd_Object_B_Event_Capacity,
                                                        ref Pd_Object_B_Event_Costs, ref Pd_Object_B_Event_Demand, ref Pd_Object_B_Event_Info, ref Pd_Object_B_Event_Name_Format,
                                                        ref Pd_Object_B_Event_Price, ref Pd_Object_B_Event_Procedure, ref Pd_Object_B_Event_Sched_Model,
                                                        ref Pd_Object_B_Event_Schedule, ref Pd_Object_B_Event_Type, ref Pd_Object_B_Eventknowledgelink, ref Pd_Object_Cost_Alloc_Invoice,
                                                        ref Pd_Object_Cost_Distribution, ref Pd_Object_Cost_Planning, ref Pd_Object_Department_Staff, ref Pd_Object_Description,
                                                        ref Pd_Object_Empl_Group_Subgroup, ref Pd_Object_External_Key, ref Pd_Object_Head_Count_Budget, ref Pd_Object_Health_Examinations,
                                                        ref Pd_Object_Job_Market_Info, ref Pd_Object_Mail_Address, ref Pd_Object_Obsolete, ref Pd_Object_Override_Reqmnt,
                                                        ref Pd_Object_Pd_Profiles, ref Pd_Object_Planned_Compensation, ref Pd_Object_Posting_Specs, ref Pd_Object_Processing_Fm,
                                                        ref Pd_Object_Proficiency_Desc, ref Pd_Object_Relationships, ref Pd_Object_Restrictions, ref Pd_Object_Room_Reserv_Info, ref Pd_Object_Shift_Group,
                                                        ref Pd_Object_Site_Dependent_Info, ref Pd_Object_Standard_Profiles, ref Pd_Object_Task_Character, ref Pd_Object_Vacancy,
                                                        ref Pd_Object_Validity, ref Pd_Object_Valuation, ref Pd_Object_Work_Evaluat_Result, ref Pd_Object_Work_Reqmnt_Scale, ref Pd_Object_Work_Schedule,
                                                        ref Pd_Rel_Application_For, ref Pd_Rel_Appraisal_Model, ref Pd_Rel_Appraises, ref Pd_Rel_Career_Develop_Action,
                                                        ref Pd_Rel_Is_Equipped, ref Pd_Rel_Is_Prebooked_For, ref Pd_Rel_Job_Reqmnts, ref Pd_Rel_Message_Type, ref Pd_Rel_Qualifications_Reqmnts,
                                                        ref Pd_Rel_Requires, ref Pd_Rel_Reserves, ref Pd_Rel_Session, ref Pd_Rel_Specified_Substitute,
                                                        ref Pd_Rel_Still_Requires, ref Pd_Rel_Takes_Part_In, ref Return0);
                    }
                    else
                    {
                        proxy.Bapi_Orgmaster_Save_Repl_Mult(ref Hr_Infotype_Header, ref Hr_Object_Header, ref Pd_Object, ref Pd_Object_Address,
                             ref Pd_Object_Description, ref Pd_Object_Mail_Address, ref Pd_Object_Pd_Profiles, ref Pd_Object_Relationships,
                             ref Pd_Object_Standard_Profiles, ref Pd_Rel_Message_Type, ref Pd_Rel_Specified_Substitute, ref Return0);
                    }

                    DataTable Result = Return0.ToADODataTable();
                    proxy.Connection.Close();
                    if (Result.Rows.Count == 0)
                    {
                        this._idmsLogger.WriteLog(LogLevelType.Info, string.Format("Position {0} Removed successfully to user {1}", positionCode, userInfo.UserLogonName));
                        bReturn = true;
                    }
                    else
                    {
                        GetAndSaveSAPMessage(Result);
                    }
                }
            }
            catch (Exception ex)
            {
                this._idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while Removing position {0} to user {1}", positionCode, userInfo.UserLogonName), ex);
                bReturn = false;
            }

            return bReturn;
        }


        List<SAPHANAExistingPositionUserInfo> GetExistingUsersByPosition(string positionCode)
        {
            var posUsers = new List<SAPHANAExistingPositionUserInfo>();
            var posUser = new SAPHANAExistingPositionUserInfo();
            try
            {
                bool isPagingEnabled = Convert.ToBoolean(Config(EndpointKey.IsPaginationEnabled, SAPConfiguration));
                DataTable m_dtHRP1001 = new DataTable();
                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                FLD_SOBID.Fieldname = "SOBID";

                RFC_DB_FLD FLD_BEGDA = new RFC_DB_FLD();
                FLD_BEGDA.Fieldname = "BEGDA";

                RFC_DB_FLD FLD_ENDDA = new RFC_DB_FLD();
                FLD_ENDDA.Fieldname = "ENDDA";

                RFC_DB_FLD FLD_SUBTY = new RFC_DB_FLD();
                FLD_ENDDA.Fieldname = "SUBTY";

                dtFlds.Add(FLD_SOBID);
                dtFlds.Add(FLD_BEGDA);
                dtFlds.Add(FLD_ENDDA);
                dtFlds.Add(FLD_SUBTY);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "RSIGN = 'B' AND RELAT = '007' AND OTYPE = 'S' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter.Text = "SCLAS = 'US' AND OTYPE = 'S' AND OBJID = '" + positionCode + "' ";

                //filter.Text = "SCLAS = 'US' AND OTYPE = 'S' AND OBJID = '" + positionCode + "'  AND SUBTY='A008'";

                dtopts.Add(filter);
                List<string> fields = new List<string>() { "SOBID" , "BEGDA", "ENDDA", "SUBTY" };
                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    //proxy.Connection = GetSAPConnection();
                    //proxy.Connection.Open();
                    m_dtHRP1001 = processorConfig.sapUtil.ReadRFCTblAsFunction("HRP1001", "|", fields, filter.Text, isPagingEnabled);
                    //proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //DataTable dtTemp = dt.ToADODataTable();
                    //DataTable dtFields = dtFlds.ToADODataTable();
                    //m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);
                    //proxy.Connection.Close();
                }

                LogDebugStep($"m_dtHRP1001.Rows.Count: {m_dtHRP1001.Rows.Count}");
                foreach (DataRow row in m_dtHRP1001.Rows)
                {
                    posUser = new SAPHANAExistingPositionUserInfo()
                    {
                        ValidFrom = row["BEGDA"].ToString().Trim(),
                        ValidTo = row["ENDDA"].ToString().Trim(),
                        UserName = row["SOBID"].ToString().Trim(),
                        SUBTY = row["SUBTY"].ToString().Trim(),
                    };

                    LogDebugStep($"UserName: {posUser.UserName}, ValidFrom: {posUser.ValidFrom}, ValidTo: {posUser.ValidTo}");
                    var posValidTo = sapUtil.getDateTimeWithoutHyphen(posUser.ValidTo);
                    if (!posValidTo.HasValue || posValidTo.Value >= DateTime.Today)
                        posUsers.Add(posUser);
                }
                LogDebugStep($"posUsers.Count: {posUsers.Count}");
            }
            catch
            {
                throw;
            }
            return posUsers;
        }
        public override bool IsUserLocked(string userName)
        {
            bool isLocked = false;
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            try
            {
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", userName);

                WriteProvisioningLog(LogLevelType.DebugStep, $"IsUserLocked - {userName}");
                IRfcFunction fnUserExists = processorConfig.sapUtil.getRfcFunction("BAPI_USER_EXISTENCE_CHECK", dicParams);

                var rfcDestination = processorConfig.sapUtil.GetSAPDestination();
                WriteProvisioningLog(LogLevelType.DebugStep, $"Trying to connect to SAPHANA Server: RFCDestination, AppServer:{rfcDestination?.AppServerHost}, " +
                  $"SystemNumber: {rfcDestination?.SystemNumber}, ClientNumber: {rfcDestination?.Client}");

                var result = processorConfig.sapUtil.ExecuteStructure(fnUserExists, "RETURN");

                WriteProvisioningLog(LogLevelType.DebugStep, $"IsUserLocked result: {result.GetValue("MESSAGE").ToString()}");
                if (result.GetValue("MESSAGE").ToString().ToUpper().Contains("NOT EXIST"))
                {
                    WriteProvisioningLog(LogLevelType.DebugStep, $"IsUserLocked - {userName} NOT EXISTS");
                    return false;
                }
                else
                {
                    WriteProvisioningLog(LogLevelType.DebugStep, $"IsUserLocked - {userName} EXIST");
                    IRfcStructure structIsLocked = processorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "ISLOCKED");
                    isLocked = ((structIsLocked.GetValue("Glob_Lock") != null && structIsLocked.GetValue("Glob_Lock").ToString().ToUpper() == "L") || (structIsLocked.GetValue("Local_Lock") != null && structIsLocked.GetValue("Local_Lock").ToString().ToUpper() == "L"));
                    WriteProvisioningLog(LogLevelType.DebugStep, $"IsUserLocked - {userName} EXISTS and Locked:{isLocked}");
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, $"IsUserLocked - {userName}. Error:{ex.Message}", ex);
                isLocked = false;
            }
            return isLocked;
        }

        public override bool IsUserExists(string userName)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {
                //if (ReConnect())
                //{
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", userName);

                WriteProvisioningLog(LogLevelType.DebugStep, $"IsUserExists - {userName}");
                IRfcFunction fnUserExists = processorConfig.sapUtil.getRfcFunction("BAPI_USER_EXISTENCE_CHECK", dicParams);

                var rfcDestination = processorConfig.sapUtil.GetSAPDestination();
                WriteProvisioningLog(LogLevelType.DebugStep, $"Trying to connect to SAPHANA Server: RFCDestination, AppServer:{rfcDestination?.AppServerHost}, " +
                  $"SystemNumber: {rfcDestination?.SystemNumber}, ClientNumber: {rfcDestination?.Client}");

                var result = processorConfig.sapUtil.ExecuteStructure(fnUserExists, "RETURN");

                WriteProvisioningLog(LogLevelType.DebugStep, $"IsUserExists result: {result.GetValue("MESSAGE").ToString()}");
                if (result.GetValue("MESSAGE").ToString().ToUpper().Contains("NOT EXIST"))
                    return false;
                //}
                //else
                //{
                //    WriteProvisioningLog(LogLevelType.Warning, $"ReConnection failed for system:{Endpoint.DisplayName}");
                //    return false;
                //}
                //IRfcStructure structAddress = processorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "ADDRESS");
                //var pernr = structAddress.GetValue("Pers_No").ToString().Trim();
                //WriteProvisioningLog(LogLevelType.DebugStep, $"IsUserExists - Pernr : {pernr}");
                //if (string.IsNullOrWhiteSpace(pernr))
                //    return false;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, "Error while checking account name existence.", ex);
                throw ex;
            }

            return true;
        }

        bool AddProfile(ProvisioningParamCollection fields, string accountName)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {
                var exists = IsUserExists(accountName);

                if (exists == false)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"{accountName} - account does not exists.");
                    return false;
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", accountName);

                IRfcFunction fnUserDetail = processorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
                var tblRfcProfiles = processorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "PROFILES");
                IRfcFunction fnAssignProfile = processorConfig.sapUtil.getRfcFunction("BAPI_USER_PROFILES_ASSIGN", dicParams);

                var sapProfiles = fields["Profiles"].Value;
                List<string> lstProfiles = string.IsNullOrEmpty(sapProfiles) ? new List<string>() : sapProfiles.Split(',').ToList();

                foreach (string profile in lstProfiles)
                {
                    tblRfcProfiles.Append();
                    tblRfcProfiles.SetValue("Bapiprof", profile);
                }
                fnAssignProfile.SetValue("PROFILES", tblRfcProfiles);

                var result = processorConfig.sapUtil.ExecuteTable(fnAssignProfile, "RETURN");

                if (result.Rows.Count <= 0)
                {
                    WriteProvisioningLog(LogLevelType.Error, "Function return null.");
                    return false;
                }
                if (result.Rows[0]["TYPE"].ToString() != "S")
                {
                    WriteProvisioningLog(LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Warning, $"AddProfile {accountName} profile unable to create.", ex);
                return false;
            }

            WriteProvisioningLog(LogLevelType.Info, $"Profiles added to the account {accountName} successfully.");
            return true;
        }

        public void UpdateEntitlementRecon()
        {

            var sapusers = DI.Chakra.DomainModel.Presentation.PresentationSAPHANAUserDetails.GetAllUserByEndpointSystemId(processorConfig.EndpointId);
            var endpointSystemId = processorConfig.EndpointId;
            AccountReconcilation acctRecon = null;
            EntitlementReconcilation entRecon = null;
            var accountReconcilations = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id);
            List<PresentationSAPHANAUsersToRolesReport> lstSAPRoleInfos = null;
            try
            {

                foreach (var sapUserInfo in sapusers)
                {
                    lstSAPRoleInfos = SAPHANARoleInfo.GetUserRoles(sapUserInfo.Id, endpointSystemId);
                    if (lstSAPRoleInfos.Count == 0)
                    {
                        acctRecon = accountReconcilations.FirstOrDefault(p => p.EndpointUserId == sapUserInfo.Id && p.EndpointSystemId == Endpoint.Id);
                        if (acctRecon != null)
                        {
                            var entRecons = EntitlementReconcilation.GetByEndpointUserId(Endpoint.Id,
                                acctRecon.Id, EntitlementType.Role);

                            foreach (var ent in entRecons)
                            {
                                if (!ent.IsDeleted)
                                {
                                    ent.IsDeleted = true;
                                    ent.ExceptionType = ExceptionTypes.Removed;
                                    ent.DeletedDate = DateTime.Now;
                                    ent.Save(MasterConfigHelperServiceUser);
                                }
                            }
                        }
                        continue;
                    }
                    foreach (PresentationSAPHANAUsersToRolesReport entitlement in lstSAPRoleInfos)
                    {
                        //acctRecon = AccountReconcilation.GetByUser(sapUserInfo.EndPointSystemId, sapUserInfo.Id);
                        acctRecon = accountReconcilations.FirstOrDefault(p => p.EndpointUserId == sapUserInfo.Id && p.EndpointSystemId == Endpoint.Id);
                        if (acctRecon != null)
                        {
                            entRecon = EntitlementReconcilation.GetByUser(sapUserInfo.Id, sapUserInfo.EndpointSystemId, entitlement.RoleCode.Trim(), EntitlementType.Role);

                            if (entRecon == null)
                            {
                                entRecon = CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Role);
                                entRecon.Entitlement = entitlement.RoleCode.Trim();
                                entRecon.EntitlementId = entitlement.RoleId;
                                entRecon.ExceptionType = ExceptionTypes.Added;
                            }
                            else
                            {
                                if (entRecon.IsDeleted)
                                {
                                    entRecon.IsDeleted = false;
                                    entRecon.DeletedDate = null;
                                    entRecon.LastAttestedOn = null;
                                    entRecon.ExceptionType = ExceptionTypes.Added;
                                }
                            }
                            try
                            {
                                entRecon.ValidFromDate = string.IsNullOrWhiteSpace(entitlement.Role_FromDate) ? DateTime.Now : DateTime.ParseExact(entitlement.Role_FromDate, _SAPentitlementDateStringFormat, System.Globalization.CultureInfo.InvariantCulture);

                                entRecon.ValidToDate = string.IsNullOrWhiteSpace(entitlement.Role_ToDate) ? GetValidTo(acctRecon.UserId) : DateTime.ParseExact(entitlement.Role_ToDate, _SAPentitlementDateStringFormat, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (Exception e)
                            {
                                WriteDiscoveryLog(LogLevelType.Exception, $"Error while updating Entitlement reconciliation Validto & from for the account :{acctRecon.AccountName}.", e);
                            }

                            entRecon.AccountName = acctRecon.AccountName;
                            entRecon.UserId = acctRecon.UserId;
                            entRecon.EntitlementDescription = entitlement.RoleDescription;
                            entRecon.Save(MasterConfigHelperServiceUser);
                        }
                    }
                }

                //EntitlementReconcilation.ProcessDeletedEntitlements(Endpoint.Id, Process, MasterConfigHelperServiceUser);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation.", ex);
            }
            finally
            {
                sapusers?.Clear();
                sapusers = null;

                lstSAPRoleInfos?.Clear();
                lstSAPRoleInfos = null;

                accountReconcilations?.Clear();
                accountReconcilations = null;
            }

        }

        string GetPositionDescription(string positionKey, Dictionary<string, string> matrices)
        {
            if (matrices == null || matrices.Count == 0)
                return string.Empty;

            return matrices.ContainsKey(positionKey) ? matrices[positionKey] : string.Empty;
        }

        void UpdateEntitlementReconForPosition()
        {
            Dictionary<string, string> matrices = new Dictionary<string, string>();
            var accountReconcilations = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id);
            List<DI.Chakra.DomainModel.Presentation.PresentationSAPHANAUserDetails> sapusers = new List<DI.Chakra.DomainModel.Presentation.PresentationSAPHANAUserDetails>();
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "UpdateEntitlementReconForPosition - Started");
                sapusers = DI.Chakra.DomainModel.Presentation.PresentationSAPHANAUserDetails.GetAllUserByEndpointSystemId(Endpoint.Id);
                AccountReconcilation acctRecon = null;
                EntitlementReconcilation entRecon = null;

                var lstSAPHRP1001 = SAPHANAHRP1001.GetByEndpointSystemId(Endpoint.Id);
                if (lstSAPHRP1001 == null && lstSAPHRP1001.Count == 0)
                {
                    LogDebugStep("SAPHANAHRP1001.GetByEndpointSystemId - no data found.");
                    return;
                }

                LogDebugStep($"lstSAPHRP1001 - {lstSAPHRP1001.Count}.");
                IEnumerable<SAPHANAHRP1001> tmpSAPHRP1001;

                #region to get position description
                var tmpMatrices = SAPHANAXLPositionsMatrix.GetAll().Where(i => i.EndpointSystemId == this.Endpoint.Id).ToList();
                foreach (var matrix in tmpMatrices)
                {
                    if (!string.IsNullOrWhiteSpace(matrix.Position))
                    {
                        if (!matrices.ContainsKey(matrix.Position))
                            matrices.Add(matrix.Position, matrix.PositionText);
                    }
                    else
                    {
                        WriteDiscoveryLog(LogLevelType.Info, $"EBS CRM Matrix BusinessRole:{matrix.BusinessRole} Position_CRP empty");
                    }

                }
                #endregion

                foreach (var sapUserInfo in sapusers)
                {
                    tmpSAPHRP1001 = lstSAPHRP1001.Where(p => p.RelatedObjectCode.Equals(sapUserInfo.UserLogonName, StringComparison.InvariantCultureIgnoreCase));
                    if (tmpSAPHRP1001 != null)
                    {
                        acctRecon = accountReconcilations.FirstOrDefault(p => p.AccountName.Equals(sapUserInfo.UserLogonName, StringComparison.InvariantCultureIgnoreCase) && p.EndpointSystemId == Endpoint.Id);
                        if (acctRecon != null)
                        {
                            foreach (SAPHANAHRP1001 entitlement in tmpSAPHRP1001)
                            {
                                LogDebugStep($"Account Recon: {acctRecon == null}, for endpointId: {Endpoint.Id}, userId: {sapUserInfo.Id}");

                                entRecon = EntitlementReconcilation.GetByUser(sapUserInfo.Id, sapUserInfo.EndpointSystemId, entitlement.ActualObjectCode, EntitlementType.Position);

                                if (entRecon == null)
                                {
                                    entRecon = CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Position);
                                    entRecon.Entitlement = entitlement.ActualObjectCode;
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.ExceptionType = ExceptionTypes.Added;
                                }
                                else
                                {
                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    if (entRecon.EntitlementId == 0)
                                    {
                                        entRecon.EntitlementId = entitlement.Id;
                                    }
                                }

                                entRecon.EntitlementDescription = GetPositionDescription(entitlement.ActualObjectCode, matrices);
                                entRecon.AccountName = acctRecon.AccountName;
                                entRecon.UserId = acctRecon.UserId;

                                entRecon.Save(MasterConfigHelperServiceUser);
                            }
                        }
                        else
                        {
                            WriteDiscoveryLog(LogLevelType.Info, $"UpdateEntitlementReconForPosition  - Could not find Account in AccountRecon :{sapUserInfo.UserLogonName} && SAPUserInfo:{sapUserInfo.Id}");
                        }
                    }
                    else
                    {
                        WriteDiscoveryLog(LogLevelType.Info, $"UpdateEntitlementReconForPosition  - Could not find RelatedObjectCode in SAPHRP1001 :{sapUserInfo.UserLogonName} && SAPUserInfo:{sapUserInfo.Id}");
                    }
                }

                //LogDebugStep("EntitlementReconcilation.ProcessDeletedEntitlements - Started");
                //EntitlementReconcilation.ProcessDeletedEntitlements(Endpoint.Id, Process, MasterConfigHelperServiceUser);
                //WriteDiscoveryLog(LogLevelType.Info, "UpdateEntitlementReconForPosition - Ended");
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation.", ex);
            }
            finally
            {
                matrices?.Clear();
                matrices = null;

                accountReconcilations?.Clear();
                accountReconcilations = null;

                sapusers?.Clear();
                sapusers = null;
            }

        }
        string Config(EndpointKey Key, EndpointConfiguration endpointConfig)
        {
            var config = endpointConfig.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        IRfcStructure GetLogonInfoFromSAP(string userName)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            try
            {
                if (!IsUserExists(userName))
                {
                    WriteProvisioningLog(LogLevelType.Error, $"{userName} - account does not exists.");
                    return null;
                }

                WriteProvisioningLog(LogLevelType.Error, $"{userName} - Started GetLogonInfoFromSAP");
                // FIND USER TYPE
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", userName);

                // get logon data to get user Type
                IRfcStructure structLogonData = processorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "LOGONDATA");

                return structLogonData;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while fetching logondata for the account - {userName}.", ex);
                return null;
            }
        }

        public override void TestSAPConnection()
        {
            try
            {
                using (SAPUserGrp.SAPNEWPRO proxy = new SAPUserGrp.SAPNEWPRO())
                {
                    proxy.Connection = GetSAPConnection();
                    proxy.Connection.Open();
                }
            }
            catch (Exception ex)
            {
                LogDebugStep(ex.StackTrace);
            }
        }

        public override dynamic GetProcessorConfig()
        {
            return processorConfig;
        }

        public override bool UpdateSFBTelephoneNumbers(IEndpointUserInfo _userInfo)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            SAPHANAUserinfo _SAPUserInfo = (SAPHANAUserinfo)_userInfo;
            string UserLogonName = _SAPUserInfo.UserLogonName;

            try
            {
                if (!IsUserExists(UserLogonName))
                {
                    WriteProvisioningLog(LogLevelType.Error, $"{UserLogonName} - account does not exists.");
                    return false;
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserLogonName);

                IRfcFunction fnUserChange = processorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                IRfcStructure structAddress = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "ADDRESS");
                IRfcStructure structAddressX = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "ADDRESSX");
                //IRfcStructure structDefaults = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "DEFAULTS");
                //IRfcStructure structDefaultsX = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "DEFAULTSX");
                //IRfcStructure structLogondata = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATA");
                //IRfcStructure structLogondataX = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATAX");

                if (!string.IsNullOrEmpty(_SAPUserInfo.Tel1_Numbr))
                {
                    structAddress.SetValue("Tel1_Numbr", _SAPUserInfo.Tel1_Numbr);
                    structAddressX.SetValue("Tel1_Numbr", "X");
                }

                fnUserChange.SetValue("ADDRESS", structAddress);
                fnUserChange.SetValue("ADDRESSX", structAddressX);

                var result = processorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                bool isUserUpdated = false;
                foreach (DataRow row in result.Rows)
                {
                    if (row["Message"].ToString().ToUpper().Contains(string.Format("User {0} has changed", _SAPUserInfo.UserLogonName).ToUpper()) ||
                           row["Message"].ToString().ToUpper().Contains(string.Format("No changes were made to user {0}", _SAPUserInfo.UserLogonName).ToUpper()))
                    {
                        isUserUpdated = true;
                        break;
                    }
                }

                if (isUserUpdated)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"SFB telephone {_SAPUserInfo.Tel1_Numbr} for user - {UserLogonName} updated successfully.");
                    return true;
                }
                else
                {
                    foreach (DataRow row in result.Rows)
                    {
                        WriteProvisioningLog(LogLevelType.Error, row["Message"].ToString(), null);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"Error while updating the account - {UserLogonName}.", ex);
                throw ex;
            }
        }

        #region For Tool
        public bool CheckPostionExistingByUser(string positionCode, string username)
        {
            try
            {
                WriteProvisioningLog(LogLevelType.DebugStep, string.Format("Checking postition {0} existance for user {1}", positionCode, username));
                DataTable m_dtHRP1001 = new DataTable();
                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                FLD_SOBID.Fieldname = "SOBID";

                RFC_DB_FLD FLD_BEGDA = new RFC_DB_FLD();
                FLD_BEGDA.Fieldname = "BEGDA";

                RFC_DB_FLD FLD_ENDDA = new RFC_DB_FLD();
                FLD_ENDDA.Fieldname = "ENDDA";

                dtFlds.Add(FLD_SOBID);
                dtFlds.Add(FLD_BEGDA);
                dtFlds.Add(FLD_ENDDA);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "RSIGN = 'B' AND RELAT = '007' AND OTYPE = 'S' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                if (string.IsNullOrWhiteSpace(username))
                    filter.Text = "SCLAS = 'US' AND OTYPE = 'S' AND OBJID = '" + positionCode + "' ";
                else
                    filter.Text = "SCLAS = 'US' AND OTYPE = 'S' AND OBJID = '" + positionCode + "' AND SOBID = '" + username + "'";

                WriteProvisioningLog(LogLevelType.DebugStep, $"Filter added {filter.Text}");
                dtopts.Add(filter);
                WriteProvisioningLog(LogLevelType.DebugStep, "Step 1");
                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    WriteProvisioningLog(LogLevelType.DebugStep, "Step 2");
                    proxy.Connection = this.GetSAPConnection();
                    WriteProvisioningLog(LogLevelType.DebugStep, "Step 3");
                    proxy.Connection.Open();
                    WriteProvisioningLog(LogLevelType.DebugStep, "Step 4");
                    proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    WriteProvisioningLog(LogLevelType.DebugStep, "Step 5");
                    DataTable dtTemp = dt.ToADODataTable();
                    WriteProvisioningLog(LogLevelType.DebugStep, "Step 6");
                    DataTable dtFields = dtFlds.ToADODataTable();
                    WriteProvisioningLog(LogLevelType.DebugStep, "Step 7");
                    m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);
                    WriteProvisioningLog(LogLevelType.DebugStep, "Step 8");
                    proxy.Connection.Close();
                }

                bool exists = false;
                foreach (DataRow row in m_dtHRP1001.Rows)
                {
                    WriteProvisioningLog(LogLevelType.DebugStep, $"Username: {row["SOBID"].ToString().Trim()}");
                    if (row["SOBID"].ToString().Trim() == username)
                    {
                        exists = true;
                        break;
                    }
                }

                WriteProvisioningLog(LogLevelType.DebugStep, "Step 9");
                if (!exists)
                {
                    WriteProvisioningLog(LogLevelType.DebugStep, string.Format("Postition {0} not available at system for the user {1}", positionCode, username));
                }

                return exists;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.DebugStep, string.Format("Error in checking Postition {0} existance for the user {1}", positionCode, username));
                return true;
            }
        }
        #endregion
        public override bool IsAdminLock(string userLogonName)
        {
            bool isLocked = false;
            try
            {
                UnLockChangePasswordAPIMessage = string.Empty;
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", userLogonName);

                IRfcStructure structIsLocked = processorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "ISLOCKED");
                isLocked = (structIsLocked.GetValue("Glob_Lock").ToString().ToUpper() == "L" || structIsLocked.GetValue("Local_Lock").ToString().ToUpper() == "L");
                if (isLocked)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"Account: {userLogonName} is locked by Administrator. Please contact IAM Team.");
                    UnLockChangePasswordAPIMessage = $"Account: {userLogonName} is locked by Administrator. Please contact IAM Team.";
                }
            }
            catch (Exception ex)
            {
                UnLockChangePasswordAPIMessage = $"IsAdminLock check failed: {ex.Message}";
                WriteProvisioningLog(LogLevelType.Exception, $"Error while checking IsAdminLock for Account - {userLogonName}.", ex);
            }
            return isLocked;
        }
        /// <summary>
        /// Used for SAPHANASSO implementation for user account
        /// </summary>
        /// <param name="UserLogonName">SAP AccountName</param>
        /// <param name="samAccountName">User Primary AD samaccount account(Singaporpower.Local AD</param>
        /// <param name="accountType">Its for User Account only</param>
        /// <returns></returns>
        public override bool UpdateSSOAttributes(IEndpointUserInfo userInfo, string samAccountName, AccountType accountType)
        {
            bool isUserUpdated = false;
            string UserLogonName = string.Empty;
            try
            {
                SAPHANAUserinfo _SAPUserInfo = userInfo as SAPHANAUserinfo;
                UserLogonName = _SAPUserInfo.UserLogonName;
                if (this.EnableSSO && accountType == AccountType.User)
                {
                    if (!string.IsNullOrWhiteSpace(this.SNCNameSufix) && !string.IsNullOrWhiteSpace(this.SNCNamePrefix) && !string.IsNullOrWhiteSpace(this.SNCParameterName))
                    {
                        if (!IsUserExists(UserLogonName))
                        {
                            WriteProvisioningLog(LogLevelType.Error, $"{UserLogonName} - account does not exist.");
                            return false;
                        }

                        var dicParams = new Dictionary<string, object>();
                        dicParams.Add("USERNAME", UserLogonName);

                        IRfcFunction fnUserChange = processorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                        IRfcStructure structSNC = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "SNC");
                        IRfcStructure structSNCX = processorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "SNCX");

                        WriteProvisioningLog(LogLevelType.Info, $"samAccountName: {samAccountName} for user:{UserLogonName}");

                        var sncValue = GetSNCParamValue(samAccountName);
                        WriteProvisioningLog(LogLevelType.Info, $"SNCValue to update:{sncValue}");
                        structSNC.SetValue(this.SNCParameterName, sncValue);
                        structSNCX.SetValue(this.SNCParameterName, "X");
                        fnUserChange.SetValue("SNC", structSNC);
                        fnUserChange.SetValue("SNCX", structSNCX);
                        var result = processorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                        foreach (DataRow row in result.Rows)
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"{row["Message"]?.ToString()}", null);
                            if (row["Message"].ToString().ToUpper().Contains(string.Format("User {0} has changed", UserLogonName).ToUpper()) ||
                                   row["Message"].ToString().ToUpper().Contains(string.Format("No changes were made to user {0}", UserLogonName).ToUpper()))
                            {
                                isUserUpdated = true;
                                break;
                            }
                        }

                        if (isUserUpdated)
                        {
                            WriteProvisioningLog(LogLevelType.Info, $"UpdateSSOAttributes - SNC Parameter updated successfully for Account - {UserLogonName}.");
                            isUserUpdated = true;
                            _SAPUserInfo.SAPUserSNC = sncValue;
                        }
                        else
                        {
                            foreach (DataRow row in result.Rows)
                            {
                                WriteProvisioningLog(LogLevelType.Error, row["Message"].ToString(), null);
                            }
                            isUserUpdated = false;
                        }
                    }
                    else
                    {
                        WriteProvisioningLog(LogLevelType.Warning, $"Cannot update SNC for SSO. Missing SNC information. SNCNamePrefix:{SNCNamePrefix} SNCNameSufix:{SNCNameSufix} SNCParameterName:{SNCParameterName}");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, $"UpdateSSOAttributes: Error while updating SSO SNCParameter for Account - {UserLogonName}.", ex);
            }
            return isUserUpdated;
        }

       
    }

}
