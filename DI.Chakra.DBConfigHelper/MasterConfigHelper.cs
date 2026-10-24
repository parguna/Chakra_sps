
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DI.Chakra.DBConfigHelper
{
    public class EventTypeReason
    {
        public string EventType { get; set; }
        public string EventReason { get; set; }
    }
    public class MasterConfigHelper
    {
        static List<EventTypeReason> getEventTypeReason(string str)
        {
            var list = new List<EventTypeReason>();
            if (string.IsNullOrWhiteSpace(str))
                return list;

            var colonstrings = str.Split(',');
            foreach (var s in colonstrings)
            {
                string[] eventsWithReason = s.Split(':');
                if (!(eventsWithReason == null || eventsWithReason.Count() < 2))
                    list.Add(new EventTypeReason() { EventType = eventsWithReason[0], EventReason = eventsWithReason[1] });
            }
            return list;
        }
        //public static bool CanReadNRIC
        //{
        //    get
        //    {
        //        return Convert.ToBoolean(GetConfigValue(ConfigConstants.CanReadNRIC, Product.IM)?.ConfigValue);
        //    }
        //}

        public static List<EventTypeReason> OnBoardTriggers
        {
            get
            {
                return getEventTypeReason(GetConfigValue(ConfigConstants.ON_BOARD_TRIGGERS, Product.IM).ConfigValue);
            }
        }

        public static List<EventTypeReason> OffBoardTriggers
        {
            get
            {
                return getEventTypeReason(GetConfigValue(ConfigConstants.OFF_BOARD_TRIGGERS, Product.IM).ConfigValue);
            }
        }

        public static List<EventTypeReason> OnBoardTriggersNotToBeConsideredForJoinDate
        {
            get
            {
                return getEventTypeReason(GetConfigValue(ConfigConstants.OnBoardTriggersNotToBeConsideredForJoinDate, Product.IM)?.ConfigValue);
            }
        }


        public static List<EventTypeReason> TranferTriggers
        {
            get
            {
                return getEventTypeReason(GetConfigValue(ConfigConstants.TRANSFER_TRIGGERS, Product.IM).ConfigValue);
            }
        }
        public static List<EventTypeReason> ReOrgTriggers
        {
            get
            {
                return getEventTypeReason(GetConfigValue(ConfigConstants.RE_ORG_TRIGGERS, Product.IM).ConfigValue);
            }
        }

        public static string[] ExecutiveEmployeeFlags
        {
            get
            {
                return GetConfigValue(ConfigConstants.EXEC_EMP_FLAGS, Product.IACM).ConfigValue.Split(',');
            }
        }

        //public static string[] AlertNotificationReceivers
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.ALERT_NOTIFICAION_RECEIVER, Product.IACM).ConfigValue.Split(',');
        //    }
        //}

        //public static string[] FailureNotificationReceivers
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.FAILURE_NOTIFICAION_RECEIVER, Product.IACM).ConfigValue.Split(',');
        //    }
        //}

        public static string[] ActiveEmployeeFlags
        {
            get
            {
                return GetConfigValue(ConfigConstants.ACTIVE_EMP_FLAGS, Product.IACM).ConfigValue.Split(',');
            }
        }

        public static string DefaultPassword
        {
            get
            {
                return GetConfigValue(ConfigConstants.DEFAULT_PASSWORD, Product.PWDManagement).ConfigValue;
            }
        }

        public static int RoleUser
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.USER_ROLE, Product.IACM).ConfigValue);
            }
        }

        public static string AccountCreatedNotificationTO
        {
            get
            {
                return GetConfigValue(ConfigConstants.ACCOUNT_CREATED_NOTIFICATION_TO, Product.IACM).ConfigValue;
            }
        }

        public static string AdhocLicenseMsg
        {
            get
            {
                return GetConfigValue(ConfigConstants.AdhocLicenseMsg, Product.IACM).ConfigValue;
            }
        }
        //public static string AccountCreatedNotificationCC
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.ACCOUNT_CREATED_NOTIFICATION_CC, Product.IACM).ConfigValue;
        //    }
        //}

        public static string EmployeeDefaultKey
        {
            get
            {
                return GetConfigValue(ConfigConstants.EmployeeDefaultKey, Product.IACM).ConfigValue;
            }
        }

        public static int WintelGroupId
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.WINTEL_GROUP_ID, Product.IACM).ConfigValue);
            }
        }

        public static int WBSAdmin
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.WBS_GROUP_ID, Product.IACM).ConfigValue);
            }
        }

        public static int O365Admin
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.O365Admin, Product.IACM).ConfigValue);
            }
        }
        public static string ProcurementTeam
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.O365ProcurementTeam, Product.IACM)?.ConfigValue);
            }
        }
        public static string ADDiscoveryJobForWintelTeam
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_DISCOVERY_JOB_WINTELTEAM, Product.IACM).ConfigValue;

            }
        }

        public static string SAPLOCALDiscoveryJobForWintelTeam
        {
            get
            {
                return GetConfigValue(ConfigConstants.SAPLOCAL_DISCOVERY_JOB_WINTELTEAM, Product.IACM).ConfigValue;

            }
        }

        public static int ITSecurityGroupId
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.IT_SECURITY_GROUP_ID, Product.IACM).ConfigValue);
            }
        }

        public static int NetworkManagement
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.NetworkManagement, Product.IACM).ConfigValue);
            }
        }

        public static List<string> PropagationFields
        {
            get
            {
                return GetConfigValue(ConfigConstants.PROPAGATION_FIELDS, Product.IACM).ConfigValue.Split(',').ToList();
            }
        }

        public static double OnBoardDaysBeforeJoining
        {
            get
            {
                return Convert.ToDouble(GetConfigValue(ConfigConstants.ON_BOARD_DAYS_BEFORE_JOINING, Product.IACM).ConfigValue);
            }
        }


        public static string IACMLoginUrl
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.IACMLoginURL, Product.IACM).ConfigValue);
            }
        }

        public static bool CanRunOnBoardTrigger
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_ON_BOARD_TRIGGER, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        public static long AttestationDefaultReviewStatus
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.ATTESTATION_DEFAULT_REVIEWSTATUS, Product.IACM).ConfigValue;
                return Convert.ToInt64(value);
            }
        }

        public static bool CanRunRehireTrigger
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_REHIRE_TRIGGER, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }


        public static bool CanRunOffBoardTrigger
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_OFF_BOARD_TRIGGER, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        public static bool CanRunTransferTrigger
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_TRANSFER_TRIGGER, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        public static bool CanRunReOrgTrigger
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_REORG_TRIGGER, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        public static bool CanRunPropagationTrigger
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_PROPAGATION_TRIGGER, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        //public static bool CanConsiderDefaultMatrix
        //{
        //    get
        //    {
        //        var value = GetConfigValue(ConfigConstants.CAN_CONSIDER_DEFAULT_MATRIX, Product.IACM).ConfigValue;
        //        return value == "1" ? true : false;
        //    }
        //}

        //public static bool CanConsiderBestMatrixChooser
        //{
        //    get
        //    {
        //        string valueConfigured = "";
        //        var value = GetConfigValue(ConfigConstants.CAN_CONSIDER_BEST_MATRIX_CHOOSER, Product.IACM);
        //        if (value != null) valueConfigured = value.ConfigValue;

        //        if (string.IsNullOrEmpty(valueConfigured)) return false;

        //        return valueConfigured == "1" ? true : false;
        //    }
        //}

        public static bool RunAttestationRemediation
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.RUN_ATTESTATION_REMEDIATION, Product.IACM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        public static int DateTimePickerInterval
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.DATETIMEPICKERINTERVAL, Product.IACM).ConfigValue);
            }
        }

        public static string AuthenticationMode
        {
            get
            {
                return GetConfigValue(ConfigConstants.AUTHENTICATIONMODE, Product.IACM).ConfigValue;
            }
        }

        public static long AuthenticationDomain
        {
            get
            {
                return Convert.ToInt64(GetConfigValue(ConfigConstants.AUTHENTICATIONDOMAIN, Product.IACM).ConfigValue);
            }
        }

        //public static string DefaultDomain
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.DEFAULTDOMAIN, Product.IACM).ConfigValue;
        //    }
        //}
        public static string SFBWarningMessageForDCCIfResponseGroupExists
        {
            get
            {
                var msg = GetConfigValue(ConfigConstants.SFBWarningMessageForDCCIfResponseGroupExists, Product.IACM)?.ConfigValue;
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = "Warning: User has ResponseGroup. Please review the user access before process the request";
                }
                return msg;
            }
        }
        public static string PasswordResetOption
        {
            get
            {
                return GetConfigValue(ConfigConstants.PASSWORDRESETOPTION, Product.PWDManagement).ConfigValue;
            }
        }

        public static string ApplicationCulture
        {
            get
            {
                return GetConfigValue(ConfigConstants.APPLICATIONCULTURE, Product.IACM).ConfigValue;
            }
        }

        public static string OTPOption
        {
            get
            {
                return GetConfigValue(ConfigConstants.OTPOPTION, Product.PWDManagement).ConfigValue;
            }
        }

        public static string PurgeTables
        {
            get
            {
                return GetConfigValue(ConfigConstants.PURGETABLES, Product.IACM).ConfigValue;
            }
        }
        public static int RiskThresholdValue
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.RISKTHRESHOLDVALUE, Product.IACM).ConfigValue);
            }
        }

        public static List<string> ITDDKey
        {
            get
            {
                var keys = GetConfigValue(ConfigConstants.IT_DepartmentCode, Product.IACM).ConfigValue;
                if (string.IsNullOrWhiteSpace(keys)) return new List<string>();
                return keys.Split(new char[] { ',' }).ToList();
            }
        }

        public static List<string> SecondaryAccountEnabledDepartments
        {
            get
            {
                var keys = GetConfigValue(ConfigConstants.SecondaryAccountEnabledDepartments, Product.IACM).ConfigValue;
                if (string.IsNullOrWhiteSpace(keys)) return new List<string>();
                return keys.Split(new char[] { ',' }).Select(p => p.Trim()).ToList();
            }
        }

        public static List<long> SuccessFactorEndpointSystemIDs
        {
            get
            {
                long val;
                List<long> endpoints = new List<long>();
                string config = GetConfigValue(ConfigConstants.SuccessFactorEndpointSystemIDs, Product.IM)?.ConfigValue;
                if (string.IsNullOrWhiteSpace(config)) return endpoints;
                config.Split(',').ToList().ForEach(e =>
                {
                    if (long.TryParse(e, out val))
                    {
                        endpoints.Add(Convert.ToInt64(e));
                    }
                });
                return endpoints;
            }
        }


        public static string ServiceUser
        {
            get
            {
                return GetConfigValue(ConfigConstants.SERVICEUSER, Product.IACM).ConfigValue;
            }
        }

        public static string AttestationSnapshotFilePath
        {
            get
            {
                return GetConfigValue(ConfigConstants.ATTESTATION_SNAPSHOT_FILEPATH, Product.IACM).ConfigValue;
            }
        }

        public static string DiscoveryServiceController
        {
            get
            {
                return GetConfigValue(ConfigConstants.DISCOVERY_SVCCONTROLLER, Product.IACM).ConfigValue;
            }
        }

        public static string AttestationServiceController
        {
            get
            {
                return GetConfigValue(ConfigConstants.ATTESTATION_SVCCONTROLLER, Product.IACM).ConfigValue;
            }
        }
        public static string EmailServiceController
        {
            get
            {
                return GetConfigValue(ConfigConstants.EMAIL_SVCCONTROLLER, Product.IACM).ConfigValue;
            }
        }
        public static string MiscServiceController
        {
            get
            {
                return GetConfigValue(ConfigConstants.MISC_SVCCONTROLLER, Product.IACM).ConfigValue;
            }
        }
        public static string ProvisioningServiceController
        {
            get
            {
                return GetConfigValue(ConfigConstants.PROVISIONING_SVCCONTROLLER, Product.IM).ConfigValue;
            }
        }

        public static string ADPostOfficeBoxValue
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_POST_OFFICE_BOX_VALUE, Product.IM).ConfigValue;
            }
        }

        public static string ADUPNDomainName
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_UPN_DOMAIN_NAME, Product.IM).ConfigValue;

            }
        }

        public static string[] ExecutiveEmployeeADGroups
        {
            get
            {
                return GetConfigValue(ConfigConstants.EXEC_EMP_AD_GROUPS, Product.IM).ConfigValue.Split(',');
            }
        }

        //public static string O365AllowExternalEmail
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.O365_ALLOW_EXTERNAL_EMAIL, Product.IM).ConfigValue;
        //    }
        //}

        //public static string O365DenyExternalEmail
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.O365_DENY_EXTERNAL_EMAIL, Product.IM).ConfigValue;
        //    }
        //}

        public static string O365EmailDomainName
        {
            get
            {
                return GetConfigValue(ConfigConstants.O365_EMAIL_DOMAIN_NAME, Product.IM).ConfigValue;
            }
        }

        public static int O365SvcAccountPwdChangeDuration
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.O365_SVCACCOUNT_PWD_CHANGE_DURATION, Product.IM).ConfigValue);
            }
        }

        public static DateTime LastPwdChangeTime
        {
            get
            {
                return Convert.ToDateTime(GetConfigValue(ConfigConstants.LastPwdChangeTime, Product.IM).ConfigValue);
            }
        }

        public static int O365ADSyncWaitTime
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.O365_AD_SYNC_WAIT_TIME, Product.IM).ConfigValue);
            }
        }

        public static int O365MailBoxWaitTime
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.O365_MAILBOX_WAIT_TIME, Product.IM).ConfigValue);
            }
        }

        public static string ADSharedAccountOU
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_SHARED_ACCOUNT_OU, Product.IM).ConfigValue;
            }
        }

        public static string ADServiceAccountOU
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_SERVICE_ACCOUNT_OU, Product.IM).ConfigValue;
            }
        }

        public static string ADSecondaryAccountOU
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_SECONDARY_ACCOUNT_OU, Product.IM).ConfigValue;
            }
        }

        public static string ADDisabledAccountsOU
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_DISABLED_OU, Product.IM).ConfigValue;
            }
        }

        public static string ADNonEmployeeAccountOU
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_NON_EMPLOYEE_ACCOUNT_OU, Product.IM).ConfigValue;
            }
        }

        public static string ADSAPLocalDisabledAccountsOU
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_SAPLOCAL_DISABLEDACCOUNT_OU, Product.IM).ConfigValue;
            }
        }
        public static string ADSAPLocalUserAccountsOU
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_SAP_LOCAL_USER_ACCOUNT_OU, Product.IM).ConfigValue;
            }
        }

        public static string ADSAPLocalServiceAccountsOU
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_SAP_LOCAL_SERVICE_ACCOUNT_OU, Product.IM).ConfigValue;
            }
        }

        public static string ADSAPLocalSharedAccountsOU
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_SAP_LOCAL_SHARED_ACCOUNT_OU, Product.IM).ConfigValue;
            }
        }

        public static string ADSAPLocalSecondaryAccountsOU
        {
            get
            {
                return GetConfigValue(ConfigConstants.AD_SAP_LOCAL_SECONDARY_ACCOUNT_OU, Product.IM).ConfigValue;
            }
        }

        public static string SharedMailboxText
        {
            get
            {
                return GetConfigValue(ConfigConstants.SharedMailboxText, Product.IACM).ConfigValue;
            }
        }

        public static string RegularMailboxText
        {
            get
            {
                return GetConfigValue(ConfigConstants.RegularMailboxText, Product.IACM).ConfigValue;
            }
        }
        public static IConfigurationMaster GetConfigValue(string key, Product product)
        {
            string baseUrl = AppSettingsHelper.DomainApiBaseUrl;
            ////if "WebApiBaseUrl" returns null, then we assume that we need to make direct domain model call.
            //if (baseUrl == null)
            //{
            //    return ConfigurationMaster.GetByConfigKey(key, product);
            //}
            //else
            //{
            //TODO - Move DomainApiClient in Web project to seperate Library and replace DomainApiClient for below code.
            string routeUrl = "ConfigurationMaster/GetByConfigKey?configKey=" + System.Uri.EscapeDataString(key) + "&product=" + product.ToString();

            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = httpClient.GetAsync(routeUrl).Result;

            response.EnsureSuccessStatusCode();
            string json = response.Content.ReadAsStringAsync().Result;
            if (json == null)
                return null;

            switch (product)
            {
                case Product.IACM:
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationMaster>(json);
                case Product.IM:
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationMasterIM>(json);
                case Product.PWDManagement:
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationMasterPasswordManagement>(json);
                case Product.Compliance:
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationMasterCompliance>(json);
                case Product.DGM:
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationMasterDGM>(json);
                default:
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationMaster>(json);
            }

            // }
        }

        public static string NPACNonEmployeeDesignation
        {
            get
            {
                return GetConfigValue(ConfigConstants.NPACNonEmployeeDesignation, Product.IM).ConfigValue
                    ;
            }
        }
        public static string NPACNonEmployeeSection
        {
            get
            {
                return GetConfigValue(ConfigConstants.NPACNonEmployeeSection, Product.IM)?.ConfigValue;
            }
        }

        public static string NPACUnits
        {
            get
            {
                return GetConfigValue(ConfigConstants.NPACUnits, Product.IM)?.ConfigValue;
            }
        }
        public static string NPACLevel99
        {
            get
            {
                return GetConfigValue(ConfigConstants.NPACLevel99, Product.IM)?.ConfigValue;
            }
        }
        public static List<string> NPACSections
        {
            get
            {
                var str = GetConfigValue(ConfigConstants.NPACSections, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(str))
                {
                    return str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
        }
        //SMS

        public static long SMSDummyBadgeStartRange
        {
            get
            {
                return Convert.ToInt64(GetConfigValue(ConfigConstants.SMS_DUMMY_BADGE_START_RANGE, Product.IM).ConfigValue);

            }
        }

        public static string[] SMSPassOfficeEmail
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.SMS_PASS_OFFICE_EMAIL, Product.IM).ConfigValue).Split(',');
            }

        }

        public static string[] MSPDefaultProfile
        {
            get
            {
                var val = GetConfigValue(ConfigConstants.MSP_DEFAULT_PROFILE, Product.IM);
                if (val == null || val.ConfigValue == null)
                    return null;

                return Convert.ToString(val.ConfigValue).Split(',');
            }
        }

        public static string[] MCPDefaultProfile
        {
            get
            {
                var val = GetConfigValue(ConfigConstants.MCP_DEFAULT_PROFILE, Product.IM);
                if (val == null || val.ConfigValue == null)
                    return null;

                return Convert.ToString(GetConfigValue(ConfigConstants.MCP_DEFAULT_PROFILE, Product.IM).ConfigValue).Split(',');
            }
        }
        public static string EBSSAPPhoneTaskId
        {
            get
            {
                var val = GetConfigValue(ConfigConstants.EBS_SAP_PHONE_TASKID, Product.IM);
                if (val == null || val.ConfigValue == null)
                    return null;

                return Convert.ToString(GetConfigValue(ConfigConstants.EBS_SAP_PHONE_TASKID, Product.IM).ConfigValue);
            }
        }

        public static string MSSLSAPPhoneTaskId
        {
            get
            {
                var val = GetConfigValue(ConfigConstants.MSSL_SAP_PHONE_TASKID, Product.IM);
                if (val == null || val.ConfigValue == null)
                    return null;

                return Convert.ToString(GetConfigValue(ConfigConstants.MSSL_SAP_PHONE_TASKID, Product.IM).ConfigValue);
            }
        }

        public static string MSSLEBSROWorkflowStateId
        {
            get
            {
                var val = GetConfigValue(ConfigConstants.MSSLEBSROWorkflowStateId, Product.IM);
                if (val == null || val.ConfigValue == null)
                    return "2";

                return Convert.ToString(GetConfigValue(ConfigConstants.MSSLEBSROWorkflowStateId, Product.IM).ConfigValue);
            }
        }

        public static KeyValuePair<long, string> LAST_ASSIGNED_USERNAME_SEQUENCE
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.LAST_ASSIGNED_USERNAME_SEQUENCE, Product.IACM);
                if (config == null)
                    return new KeyValuePair<long, string>(0, string.Empty);
                return new KeyValuePair<long, string>(config.Id, config.ConfigValue);
            }
        }

        public static List<int> NonEmployeeNoticePeriod
        {
            get
            {
                List<int> alertDays = new List<int>();
                var config = GetConfigValue(ConfigConstants.NON_EMPLOYEE_NOTICE_PERIOD, Product.IM);

                string alertDaysString = config != null ? config.ConfigValue : "";
                var list = UtilityMethods.GetListFromString(alertDaysString);

                foreach (string item in list)
                {
                    int day = Convert.ToInt32(item);

                    if (!alertDays.Contains(day))
                    {
                        alertDays.Add(day);
                    }
                }

                return alertDays;
            }
        }

        public static bool CanSyncSmartDirectory
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_SYNC_SMART_DIRECTORY, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        public static bool CanRessetO365Pwd
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RESET_O365_PWD, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }
        public static int BackDaysAllowedForHRScan
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.BACK_DAYS_ALLOWED_FOR_HR_SCAN, Product.IM);
                if (config == null || config.ConfigValue == null)
                    return 1;
                return Convert.ToInt32(config.ConfigValue);
            }
        }

        public static int UndoOffBoardTransfer
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.UndoOffBoardTransfer, Product.IM);
                if (config == null || config.ConfigValue == null)
                    return 30;
                return Convert.ToInt32(config.ConfigValue);
            }
        }

        public static List<string> ExcludedUsers
        {
            get
            {
                var excludedUserString = GetConfigValue(ConfigConstants.EXCLUDED_USERS, Product.IM);

                if (excludedUserString != null && !string.IsNullOrEmpty(excludedUserString.ConfigValue))
                {
                    return excludedUserString.ConfigValue.Split(',').ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
        }
        public static string InhouseSystemsToShowInTaskDetailForAD
        {
            get
            {
                return GetConfigValue(ConfigConstants.InhouseSystemsToShowInTaskDetailForAD, Product.IACM).ConfigValue;
            }
        }
        public static string SFBNewAccountInformationMessage
        {
            get
            {
                return GetConfigValue(ConfigConstants.SFBNewAccountInformationMessage, Product.IACM)?.ConfigValue;
            }
        }
        public static string InvalidApprovalTypeMatrixMessage
        {
            get
            {
                return GetConfigValue(ConfigConstants.InvalidApprovalTypeMatrixMessage, Product.IACM)?.ConfigValue;
            }
        }
        public static double DateFilter
        {
            get
            {
                var data= GetConfigValue(ConfigConstants.DateFilterRange, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(data)) return Convert.ToInt64(data);
                else return 180;
            }
        }
        public static string VPNDefaultApplicationText
        {
            get
            {
                return GetConfigValue(ConfigConstants.VPNDefaultApplicationText, Product.IM)?.ConfigValue;
            }
        }

        public static string PDPAConsentStatement
        {
            get
            {
                return GetConfigValue(ConfigConstants.PDPAConsentStatement, Product.IM)?.ConfigValue;
            }
        }
        public static string EBS_MSSL_WarningMessage
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.EBSMSSLWarningMessage, Product.IM)?.ConfigValue;
                if (config != null && !string.IsNullOrWhiteSpace(config)) return config;
                return "Please remind your staff to perform GRC simulation run for the ID/Roles applied, ensuring role conflicts, if any, are mitigated or remediated.";
            }
        }
        public static int AccountNameMinimumLength
        {
            get
            {
                var cfg = GetConfigValue(ConfigConstants.AccountNameMinimumLength, Product.IM)?.ConfigValue;
                if(cfg != null && !string.IsNullOrWhiteSpace(cfg))
                {
                    int minimumLgth = 0;
                    if(Int32.TryParse(cfg,out minimumLgth))
                    {
                        return minimumLgth;
                    }
                    else
                    {
                        return 7;
                    }
                }
                else
                {
                    return 7;
                }
            }
        }
        public static int Office365AccountNameMinimumLength
        {
            get
            {
                var cfg = GetConfigValue(ConfigConstants.Office365AccountNameMinimumLength, Product.IM)?.ConfigValue;
                if (cfg != null && !string.IsNullOrWhiteSpace(cfg))
                {
                    int minimumLgth = 0;
                    if (Int32.TryParse(cfg, out minimumLgth))
                    {
                        return minimumLgth;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    return 3;
                }
            }
        }
        public static bool ValidateSAPSpecialAccountName
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.ValidateSAPSpecialAccountName, Product.IM)?.ConfigValue;
                if(!string.IsNullOrWhiteSpace(value)) return value == "1" ? true : false;
                return false;
            }
        }
        public static string SAPSpecialAccountNameWarningMessage
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.SAPSpecialAccountNameWarningMessage, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(value)) return value;
                return string.Empty;
            }
        }
        public static string AttestationServiceAccountWarningMessage
        {
            get
            {
                return GetConfigValue(ConfigConstants.AttestationServiceAccountWarningMessage, Product.IM)?.ConfigValue;
            }
        }
        public static string AttestationFlinsWarningMessage
        {
            get
            {
                return GetConfigValue(ConfigConstants.AttestationFlinsWarningMessage, Product.IM)?.ConfigValue;
            }
        }
        public static List<string> SelfServiceAccountType
        {
            get
            {
                var selfserviceaccounttype = GetConfigValue(ConfigConstants.SELFSERVICEACCOUNTTYPE, Product.IM);

                if (selfserviceaccounttype != null && !string.IsNullOrEmpty(selfserviceaccounttype.ConfigValue))
                {
                    return selfserviceaccounttype.ConfigValue.Split(',').ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
        }
        public static string AmendmentRemoveDCCMessage
        {
            get
            {
                var cfg = GetConfigValue(ConfigConstants.AmendmentRemoveDCCMessage, Product.IM)?.ConfigValue;
                if (string.IsNullOrWhiteSpace(cfg))
                {
                    cfg = "New DID number will be assigned upon removal of DCC";
                }
                return cfg;
            }
        }
        public static string AmendmentAddDCCMessage
        {
            get
            {
                var cfg = GetConfigValue(ConfigConstants.AmendmentAddDCCMessage, Product.IM)?.ConfigValue;
                if (string.IsNullOrWhiteSpace(cfg))
                {
                    cfg = "Current DID number will be re-assigned to other users upon request for DCC";
                }
                return cfg;
            }
        }
        public static bool ExcludeMSTeamsDCCDCCSPolicy
        {
            get
            {
                var value =  GetConfigValue(ConfigConstants.ExcludeMSTeamsDCCDCCSPolicy, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(value)) return value == "1" ? true : false;
                return false;
            }
        }

        public static string LoginHeadingNotes
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.LoginHeadingNotes, Product.IM)?.ConfigValue;
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "Application access request for PRODUCTION environment only";
                }
                return Convert.ToString(value);
            }
        }

        public static string LoginSubHeadingNotes
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.LoginSubHeadingNotes, Product.IM)?.ConfigValue;
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "For QA/DEV access request, please raise through ServiceNow";
                }
                return Convert.ToString(value);
            }
        }
        public static string ADOtherSystemGroupValidationMessage
        {
            get
            {
                var msg = "The selected Group cannot be added.";
                var data = GetConfigValue(ConfigConstants.ADOtherSystemGroupValidationMessage, Product.IM)?.ConfigValue;
                if(data != null && !string.IsNullOrWhiteSpace(data))
                {
                    msg = data;
                }
                return msg;
            }
        }
        public static bool CanRunOtherSystemReverseEngineeringService
        {
            get
            {
                var data = GetConfigValue(ConfigConstants.CanRunOtherSystemReverseEngineeringService, Product.IM);
                if (data != null && !string.IsNullOrWhiteSpace(data.ConfigValue) && (data.ConfigValue == "1" || data.ConfigValue.ToUpper() == "True".ToUpper()))
                {
                    return true;
                }
                return false;
            }
        }

        public static string UserAttestationComparisionNotes
        {
            get
            {
                return GetConfigValue(ConfigConstants.UserAttestationComparisionNotes, Product.IM)?.ConfigValue;
            }
        }
        public static string ReviewAttestationNotes
        {
            get
            {
                return GetConfigValue(ConfigConstants.ReviewAttestationNotes, Product.IM)?.ConfigValue;
            }
        }

        public static bool UseOldReviewPage
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.UseOldReviewPage, Product.IM)?.ConfigValue;
                return (value == "1" || value == "true" || value == "True") ? true : false;
            }
        }
        public static bool ShowReviewAttestationNotes
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.ShowReviewAttestationNotes, Product.IM)?.ConfigValue;
                return (value == "1" || value == "true" || value == "True") ? true : false;
            }
        }
        public static bool ShowUserComparisionLinkForOnlyOtherSystem
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.ShowUserComparisionLinkForOnlyOtherSystem, Product.IM)?.ConfigValue;
                return (value == "1" || value == "true" || value == "True") ? true : false;
            }
        }
    }
}
