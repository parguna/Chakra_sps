
using AutoMapper;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class EventTypeReason
    {
        public string EventType { get; set; }
        public string EventReason { get; set; }
        public string RetainAccessForTransfer { get; set; }
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
        public static List<EventTypeReason> RejoinTriggers
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.REJOIN_TRIGGERS, Product.IM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigKey)) return new List<EventTypeReason>();
                return getEventTypeReason(config.ConfigValue);
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
        public static TimeSpan EventProcessTime
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.EVENT_PROCESS_TIME, Product.IM);
                string rawValue = config?.ConfigValue;
                TimeSpan parsed;
                if (!string.IsNullOrWhiteSpace(rawValue) && TimeSpan.TryParse(rawValue, out parsed) && parsed >= TimeSpan.Zero && parsed < TimeSpan.FromDays(1))
                {
                    return parsed;
                }
                return TimeSpan.Zero;
            }
        }

        public static List<EventTypeReason> OnBoardTriggers
        {
            get
            {
                return getEventTypeReason(GetConfigValue(ConfigConstants.ON_BOARD_TRIGGERS, Product.IM).ConfigValue);
            }
        }

        public static List<EventTypeReason> PromotionTriggers
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PROMOTION_TRIGGERS, Product.IM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue)) return new List<EventTypeReason>();
                {
                    return getEventTypeReason(config.ConfigValue);
                }
            }
        }

        public static List<EventTypeReason> PropagationTriggers
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PROPAGATION_TRIGGERS, Product.IM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue)) return new List<EventTypeReason>();
                {
                    return getEventTypeReason(config.ConfigValue);
                }
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

        //public static string[] OnBoardTriggers
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.ON_BOARD_TRIGGERS, Product.IM).ConfigValue.Split(',');
        //    }
        //}

        //public static string[] OffBoardTriggers
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.OFF_BOARD_TRIGGERS, Product.IM).ConfigValue.Split(',');
        //    }
        //}

        //public static string[] HRMSTranferTriggers
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.HRMS_TRANSFER_TRIGGERS, Product.IM).ConfigValue.Split(',');
        //    }
        //}
        //public static string[] HRMSReOrgTriggers
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.HRMS_RE_ORG_TRIGGERS, Product.IM).ConfigValue.Split(',');
        //    }
        //}

        //public static string ReOrgTriggers
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.RE_ORG_TRIGGERS, Product.IM).ConfigValue;
        //    }
        //}

        //public static string TranferTriggers
        //{
        //    get
        //    {
        //        return GetConfigValue(ConfigConstants.TRANSFER_TRIGGERS, Product.IM).ConfigValue;
        //    }
        //}

        public static string[] ExecutiveEmployeeFlags
        {
            get
            {
                return GetConfigValue(ConfigConstants.EXEC_EMP_FLAGS, Product.IACM).ConfigValue.Split(',');
            }
        }

        public static string[] SF_ExecutiveEmployeeFlags
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.SF_EXEC_EMP_FLAGS, Product.IACM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue)) return new string[0];

                return config.ConfigValue.Split(',');
            }
        }

        public static string[] AlertNotificationReceivers
        {
            get
            {
                return User.GetEmailIdsByGroupId(ITSecurityGroupId);
                //return GetConfigValue(ConfigConstants.ALERT_NOTIFICAION_RECEIVER, Product.IACM).ConfigValue.Split(',');
            }
        }

        public static string[] FailureNotificationReceivers
        {
            get
            {
                return User.GetEmailIdsByGroupId(ITSecurityGroupId);
                //return GetConfigValue(ConfigConstants.FAILURE_NOTIFICAION_RECEIVER, Product.IACM).ConfigValue.Split(',');
            }
        }

        public static string[] ActiveEmployeeFlags
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.ACTIVE_EMP_FLAGS, Product.IACM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue)) return new string[0];

                return config.ConfigValue.Split(',');
            }
        }

        public static string[] SF_ActiveEmployeeFlags
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.SF_ACTIVE_EMP_FLAGS, Product.IACM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue)) return new string[0];

                return config.ConfigValue.Split(',');
            }
        }

        public static string DefaultPassword
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.DEFAULT_PASSWORD, Product.PWDManagement);
                if (config == null) { return string.Empty; }
                else
                {
                    return config.ConfigValue;
                }
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

        public static string SAPentitlementDateStringFormat
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.SAPentitlementDateStringFormat, Product.IM);
                return value != null && !string.IsNullOrWhiteSpace(value.ConfigValue) ? value.ConfigValue : null;
            }
        }

        public static string AdhocLicenseMsg
        {
            get
            {
                return GetConfigValue(ConfigConstants.AdhocLicenseMsg, Product.IACM).ConfigValue;
            }
        }
        public static string AccountCreatedNotificationCC
        {
            get
            {
                return GetConfigValue(ConfigConstants.ACCOUNT_CREATED_NOTIFICATION_CC, Product.IACM).ConfigValue;
            }
        }

        public static int O365Admin
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.O365Admin, Product.IACM).ConfigValue);
            }
        }
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

        public static bool CanRunReOrgTrigger
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_REORG_TRIGGER, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        public static bool CanRunPromotionTrigger
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_PROMOTION_TRIGGER, Product.IM);
                return  (value!=null && value.ConfigValue == "1") ? true : false;
            }
        }

        public static bool CanRunPropagationTrigger
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_PROPAGATION_TRIGGER, Product.IM);
                return (value != null && value.ConfigValue == "1") ? true : false;
            }
        }

        public static bool CanRunPropagationTriggerNonEmployee
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_PROPAGATION_TRIGGER_NON_EMPLOYEE, Product.IM);
                return (value != null && value.ConfigValue == "1") ? true : false;
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

        public static bool CanCheckUserActiveForProvisioning
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.CanCheckUserActiveForProvisioning, Product.IM);
                return (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue) && config.ConfigValue == "1") ? true : false;

            }
        }




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
                var keys =  GetConfigValue(ConfigConstants.IT_DepartmentCode, Product.IACM).ConfigValue;
                if (string.IsNullOrWhiteSpace(keys)) return new List<string>();
                return keys.Split(new char[] { ',' }).ToList();
            }
        }

        static string _serviceUser;
        public static string ServiceUser
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_serviceUser))
                    _serviceUser = GetConfigValue(ConfigConstants.SERVICEUSER, Product.IACM).ConfigValue;
                return _serviceUser;
            }
        }

        public static List<string> TypesConsideredFromDateParamter
        {
            get
            {
                var keys = GetConfigValue(ConfigConstants.TypesConsideredFromDateParamter, Product.IM);
                if (keys ==null || string.IsNullOrWhiteSpace(keys.ConfigValue)) return new List<string>();
                return keys.ConfigValue.Split(new char[] { ',' }).ToList();
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
        public static string DiscoveryServiceController2
        {
            get
            {
                return GetConfigValue(ConfigConstants.DISCOVERY_SVCCONTROLLER2, Product.IACM).ConfigValue;
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

        public static string O365AllowExternalEmail
        {
            get
            {
                return GetConfigValue(ConfigConstants.O365_ALLOW_EXTERNAL_EMAIL, Product.IM).ConfigValue;
            }
        }

        public static string O365DenyExternalEmail
        {
            get
            {
                return GetConfigValue(ConfigConstants.O365_DENY_EXTERNAL_EMAIL, Product.IM).ConfigValue;
            }
        }

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
                var cfValue = GetConfigValue(ConfigConstants.O365_AD_SYNC_WAIT_TIME, Product.IM).ConfigValue;
                return Convert.ToInt32(!string.IsNullOrWhiteSpace(cfValue) ? cfValue : "1");
            }
        }

        public static int O365MailBoxWaitTime
        {
            get
            {
                var cfValue = GetConfigValue(ConfigConstants.O365_MAILBOX_WAIT_TIME, Product.IM)?.ConfigValue;
                return Convert.ToInt32(!string.IsNullOrWhiteSpace(cfValue) ? cfValue:"1");
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

        public static List<string> ADDeletedUsersOU
        {
            get
            {
                var list = new List<string>();
                var config = GetConfigValue(ConfigConstants.ADDeletedUsers, Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue) )
                {
                    list.AddRange( config.ConfigValue.Split('|').ToList());
                }
                return list;
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
        public static string EmailRegards
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.EmailRegards, Product.IM);
                return config == null ? "IAM Administrator" : config.ConfigValue;
            }
        }
        static IConfigurationMaster GetConfigValue(string configKey, Product product)
        {
            IConfigurationMaster configMaster;
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                switch (product)
                {
                    case Product.IACM:
                        configMaster = Mapper.Map<DomainModel.ConfigurationMaster>(new RepositoryBase<EntityModel.ConfigurationMaster>(dataAccessTransaction)
                           .Get(tm => tm.ConfigKey.ToUpper() == configKey.ToUpper()).FirstOrDefault());
                        break;
                    case Product.IM:
                        configMaster = ConfigurationMasterIM.GetByConfigKey(configKey);
                        break;
                    case Product.PWDManagement:
                        configMaster = ConfigurationMasterPasswordManagement.GetByConfigKey(configKey);
                        break;
                    case Product.Compliance:
                        configMaster = ConfigurationMasterCompliance.GetByConfigKey(configKey);
                        break;
                    case Product.DGM:
                        configMaster = ConfigurationMasterDGM.GetByConfigKey(configKey);
                        break;
                    default:
                        configMaster = Mapper.Map<DomainModel.ConfigurationMaster>(new RepositoryBase<EntityModel.ConfigurationMaster>(dataAccessTransaction)
                           .Get(tm => tm.ConfigKey.ToUpper() == configKey.ToUpper()).FirstOrDefault());
                        break;
                }
                if (configMaster == null)
                {
                    switch (product)
                    {
                        case Product.IACM:
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Warning, $"ConfigKey not found - {configKey}");
                            break;
                        case Product.IM:
                            DILog.ProvisioningLogger.WriteLog(LogLevelType.Warning, $"ConfigKey not found - {configKey}");
                            break;

                        default:
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Warning, $"ConfigKey not found - {configKey}");
                            break;

                    }

                }
            }
            return configMaster;
        }

        public static string NPACNonEmployeeDesignation
        {
            get
            {
                return GetConfigValue(ConfigConstants.NPACNonEmployeeDesignation, Product.IM).ConfigValue;
            }
        }
        public static string NPACNonEmployeeSection
        {
            get
            {
                return GetConfigValue(ConfigConstants.NPACNonEmployeeSection, Product.IM).ConfigValue
                    ;
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

        public static string[] HelpDeskEmail
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.HELP_DESK_EMAIL, Product.IM)?.ConfigValue)?.Split(',');
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

        public static int GroupUser
        {
            get
            {
                // 0 is never a real Group.Id (identity PK starting at 1), so a missing/invalid
                // config means "exclude no group" rather than throwing — the one call site
                // (ProvisioningEngine.cs OffBoarding approver-email step) is already wrapped in
                // its own try/catch, but this avoids relying on that as the only safety net.
                var config = GetConfigValue(ConfigConstants.USER_GROUP, Product.IM);
                int groupId;
                if (config != null && int.TryParse(config.ConfigValue, out groupId))
                {
                    return groupId;
                }
                return 0;
            }
        }


        public static string CIOFlag
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.CIO_FLAG, Product.IM).ConfigValue);
            }
        }


        //public static int DefaultMailQuota
        //{
        //    get
        //    {
        //        return Convert.ToInt32(GetConfigValue(ConfigConstants.DEFAULT_MAIL_QUOTA, Product.IM).ConfigValue);
        //    }
        //}

        //public static int SharedAccountMailQuota
        //{
        //    get
        //    {
        //        return Convert.ToInt32(GetConfigValue(ConfigConstants.SHARED_ACCT_MAIL_QUOTA, Product.IM).ConfigValue);
        //    }
        //}

        //public static int MailQuotaThreshold
        //{
        //    get
        //    {
        //        return Convert.ToInt32(GetConfigValue(ConfigConstants.MAIL_QUOTA_THRESHOLD, Product.IM).ConfigValue);
        //    }
        //}

        public static int DeleteUserAfterNoOfDays
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.DELETE_USER_AFTER, Product.IM).ConfigValue);
            }
        }



        public static string LNSServiceAccountCertifier
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.LNS_SERVICE_ACCOUNT_CERTIFIER, Product.IM).ConfigValue);
            }
        }



        public static string NonStaffGroup
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.NON_STAFF_GROUP, Product.IM).ConfigValue);
            }
        }

        public static string LNSAdminUserIdPath
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.LNS_ADMIN_USERID_PATH, Product.IM).ConfigValue);
            }
        }

        public static string Coordinator
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.Coordinator, Product.IM).ConfigValue);
            }
        }

        public static string BulkAssignTaskRoleToExclude
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.BulkAssignTask_RoleToExclude, Product.IM)?.ConfigValue);
            }
        }



        public static string ADSPDisabledAccountsOU
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.AD_SP_DISABLED_OU, Product.IM).ConfigValue);
            }
        }

        public static string ADSDCDisabledAccountsOU
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.AD_SDC_DISABLED_OU, Product.IM).ConfigValue);
            }
        }

        public static bool SkipWeekendsOnRunEveryDaySchedule
        {
            get
            {
                return Convert.ToBoolean(GetConfigValue(ConfigConstants.SKIP_WEEKENDS_ON_RUN_EVERY_DAY_SCHEDULE, Product.IM).ConfigValue);
            }
        }

        public static string EmergencyRequestUserName
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.EMERGENCY_USERNAME, Product.IM).ConfigValue);
            }
        }

        //public static string SMSCode
        //{
        //    get
        //    {
        //        return Convert.ToString(GetConfigValue(ConfigConstants.SMSCode, Product.IM).ConfigValue);
        //    }
        //}


        public static string GTSSReportEmail
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.GTSS_REPORT_EMAIL, Product.IM).ConfigValue);

            }
        }

        public static string HRMSHostName
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.HRMSHostName, Product.IM).ConfigValue);

            }
        }

        public static int GTSSDefaultCompanyId
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.GTSS_DEFAULT_COMPANY_ID, Product.IM).ConfigValue);

            }
        }



        public static int BackDaysAllowedForHRScan
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.BACK_DAYS_ALLOWED_FOR_HR_SCAN, Product.IM).ConfigValue);
            }
        }

        public static int RetryTimeMilliSeconds
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.RetryTimeSeconds, Product.IM);
                //Default to 10 seconds;
                return config == null ? 10000 : Convert.ToInt32(config.ConfigValue) * 1000;
            }
        }

        public static int HowManyTimesShouldRetry
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.HowManyTimesShouldRetry, Product.IM);
                //Default to 1 time;
                return config == null ? 1 : Convert.ToInt32(config.ConfigValue);
            }
        }
        public static int SFBRetryTimeMilliSeconds
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.SFBRetryTimeSeconds, Product.IM);
                //Default to 10 seconds;
                return config == null ? 10000 : Convert.ToInt32(config.ConfigValue) * 1000;
            }
        }

        public static int SFBHowManyTimesShouldRetry
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.SFBHowManyTimesShouldRetry, Product.IM);
                //Default to 1 time;
                return config == null ? 2 : Convert.ToInt32(config.ConfigValue);
            }
        }

        public static int UndoOffBoardTransfer
        {
            get
            {
                return Convert.ToInt32(GetConfigValue(ConfigConstants.UndoOffBoardTransfer, Product.IM).ConfigValue);
            }
        }

        public static List<string> ExcludedUsers
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.EXCLUDED_USERS, Product.IM);
                string excludedUserString = Convert.ToString(config?.ConfigValue);

                if (!string.IsNullOrWhiteSpace(excludedUserString))
                {
                    return excludedUserString.Split(',').ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
        }
        public static string SkypeForBusinessFrontEndServer
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.SkypeForBusinessFrontEndServer, Product.IM).ConfigValue);
            }
        }
        public static bool UpdateTelehoneNumberInHRMS
        {
            get
            {
                var temp = GetConfigValue(ConfigConstants.UpdateTelehoneNumberInHRMS, Product.IM)?.ConfigValue;
                if(temp != null)
                {
                    return temp == "1" ? true : false;
                }
                return false;
            }
        }

        public static bool CanRunUpdateADManagerJob
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_UPDATE_AD_MANGER_JOB, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        public static bool CanRunUpdateOneCardUsers
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_UPDATE_ONE_CARD_USERS_JOB, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        public static string[] RejoinAfterRetirementTriggers
        {
            get
            {
                return GetConfigValue(ConfigConstants.REJOIN_AFTER_RETIREMENT_TRIGGERS, Product.IM).ConfigValue.Split(',');
            }
        }
        public static string O365SharedMailNoInternetGroup
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.O365_SHARED_MAIL_NO_INTERNET_GROUP, Product.IM).ConfigValue);
            }
        }

        public static long VPNADAccountCreationState
        {
            get
            {
                return Convert.ToByte(GetConfigValue(ConfigConstants.VPNADAccountCreationState, Product.IM).ConfigValue);
            }
        }
        public static long O365ADAccountCreationState
        {
            get
            {
                return Convert.ToByte(GetConfigValue(ConfigConstants.O365ADAccountCreationState, Product.IM).ConfigValue);
            }
        }

        //public static byte GEMSADAccountCreationState
        //{
        //    get
        //    {
        //        return Convert.ToByte(GetConfigValue(ConfigConstants.GEMSADAccountCreationState, Product.IM).ConfigValue);
        //    }
        //}
        public static string UserGroup
        {
            get
            {
                return Convert.ToString(GetConfigValue(ConfigConstants.UserGroup, Product.IM).ConfigValue);
            }
        }
        public static int DefaultManagerRole
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.DefaultManagerRole, Product.IM);
                return config == null ? 9 : Convert.ToInt32(config.ConfigValue);
            }
        }
        public static string EmailTeam
        {
            get
            {
                return GetConfigValue(ConfigConstants.EmailTeam, Product.IACM).ConfigValue;
            }
        }
        public static string NPACLevel99
        {
            get
            {
                return GetConfigValue(ConfigConstants.NPACLevel99, Product.IM)?.ConfigValue;
            }
        }
        public static string NPACApprovalGroup
        {
            get
            {
                return GetConfigValue(ConfigConstants.NPACApprovalGroup, Product.IM)?.ConfigValue;
            }
        }
        public static string NPACAdhocActionTypeApprovals
        {
            get
            {
                return GetConfigValue(ConfigConstants.NPACAdhocActionTypeApprovals, Product.IM)?.ConfigValue;
            }
        }
        public static int NPACApprovalSkipLevel
        {
            get
            {
                return Convert.ToByte(GetConfigValue(ConfigConstants.NPACApprovalSkipLevel, Product.IM).ConfigValue);
            }
        }
        public static bool CanIncludeOneCardMatrix
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CanIncludeOneCardMatrix, Product.IM)?.ConfigValue;
                if(!string.IsNullOrWhiteSpace(value) && value == "1")
                {
                    return true;
                }
                return false;
            }
        }
        public static bool SkipConnectorCall
        {
            get
            {
                var configuration = GetConfigValue(ConfigConstants.SKIP_CONNECTOR_CALL, Product.IACM);
                return configuration == null || configuration.ConfigValue == "0" ? false : true;
            }
        }
        public static bool MockSkipApproval
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.AutoApproveWorkflow, Product.IM)?.ConfigValue;
                if(!string.IsNullOrWhiteSpace(value)) return value == "1" ? true : false;
                return false;
            }
        }

        public static string IACMCoordinatorRoles
        {
            get
            {
                return GetConfigValue(ConfigConstants.IACMCoordinatorRoles, Product.IACM).ConfigValue;
            }
        }
        public static string O365ConnectionUri
        {
            get
            {
                var connectionUri= GetConfigValue(ConfigConstants.O365ConnectionUri, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(connectionUri))
                {
                    return connectionUri;
                }
                else
                {
                    return "https://ps.outlook.com/powershell";
                }

            }
        }


        public static bool RunRolesToTCodeGeneration
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.RunRolesToTCodeGeneration, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(value)) return value == "1" ? true : false;
                return false;
            }
        }

        public static int Office365RecipientLimits {
            get
            {
                var emailLimit = GetConfigValue(ConfigConstants.Office365RecipientLimits, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(emailLimit))
                {
                    int limit;
                    if (Int32.TryParse(emailLimit, out limit))
                    {
                        return limit;
                    }
                }
                return 50;
            }
        }
        public static int TimeDelayForDownloadO365User
        {
            get
            {
                var emailLimit = GetConfigValue(ConfigConstants.TimeDelayForDownloadO365User, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(emailLimit))
                {
                    int limit;
                    if (Int32.TryParse(emailLimit, out limit))
                    {
                        return limit;
                    }
                }
                return 10000;

            }
        }
        public static int TCodeGenerationTimer
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.TCodeGenerationTimer, Product.IM);
                //Default to 1 time;
                return config == null ? 0 : Convert.ToInt32(config.ConfigValue);
            }
        }
        public static bool TriggerHRMSUserDataUpdate
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.TriggerHRMSUserDataUpdate, Product.IM);
                //Default to 1 time;
                return (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue) && config.ConfigValue == "1") ? true : false;
            }
        }
        public static bool ConsiderInActiveAccountForReconcilation
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.ConsiderInActiveAccountForReconcilation, Product.IM);
                return (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue) && config.ConfigValue == "1") ? true : false;
            }
        }

        public static string IDMS_TO_PUB_FILEPATH
        {
            get
            {
                return GetConfigValue(ConfigConstants.IDMS_TO_PUB_FILEPATH, Product.IACM).ConfigValue;
            }
        }

        public static string PUB_TO_IDMS_FILEPATH
        {
            get
            {
                return GetConfigValue(ConfigConstants.PUB_TO_IDMS_FILEPATH, Product.IACM).ConfigValue;
            }
        }

        public static string PUB_TO_IDMS_BACKUP_FILEPATH
        {
            get
            {
                return GetConfigValue(ConfigConstants.PUB_TO_IDMS_BACKUP_FILEPATH, Product.IACM).ConfigValue;
            }
        }

        public static string IDMS_TO_PUB_PROCESSED_FILEPATH
        {
            get
            {
                return GetConfigValue(ConfigConstants.IDMS_TO_PUB_PROCESSED_FILEPATH, Product.IACM).ConfigValue;
            }
        }


        public static string CRON_SCHEDULE_EXPRESSION
        {
            get
            {
                return GetConfigValue(ConfigConstants.CRON_SCHEDULE_EXPRESSION, Product.IACM).ConfigValue;
            }
        }

        public static string NOFILE_RECEIVED_CRON_SCHEDULE_EXPRESSION
        {
            get
            {
                return GetConfigValue(ConfigConstants.NOFILE_RECEIVED_CRON_SCHEDULE_EXPRESSION, Product.IACM).ConfigValue;
            }
        }
        public static long PUBFileBackupDeleteStrategy_Process1
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBFileBackupDeleteStrategy_Process1, Product.IACM) ;
                return config == null ? 0 : Convert.ToInt32(config.ConfigValue);
            }
        }

        public static long PUBFileBackupDeleteStrategy_Process2
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBFileBackupDeleteStrategy_Process2, Product.IACM);
                return config == null ? 0 : Convert.ToInt32(config.ConfigValue);
            }
        }

        public static long PUBFileBackupDeleteStrategy_Process3
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBFileBackupDeleteStrategy_Process3, Product.IACM);
                return config == null ? 0 : Convert.ToInt32(config.ConfigValue);
            }
        }

        public static long PUBFile_Process3_Timer
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBFile_Process3_Timer, Product.IACM);
                return config == null ? 1 : Convert.ToInt32(config.ConfigValue);
            }
        }

        public static long PUBFile_Process2_Timer
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBFile_Process2_Timer, Product.IACM);
                return config == null ? 1 : Convert.ToInt32(config.ConfigValue);
            }
        }

        public static long PUBFile_Process1_Timer
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBFile_Process1_Timer, Product.IACM);
                return config == null ? 1 : Convert.ToInt32(config.ConfigValue);
            }
        }

        public static string PUBProcess1NotificationTo
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBProcess1NotificationTo, Product.IACM);
                return config == null ? string.Empty : config.ConfigValue;
            }
        }

        public static string PUBProcess2NotificationTo
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBProcess2NotificationTo, Product.IACM);
                return config == null ? string.Empty : config.ConfigValue;
            }
        }

        public static string PUBProcess3NotificationTo
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBProcess3NotificationTo, Product.IACM);
                return config == null ? string.Empty : config.ConfigValue;
            }
        }

        public static string PUBProcess4NotificationTo
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBProcess4NotificationTo, Product.IACM);
                return config == null ? string.Empty : config.ConfigValue;
            }
        }

        public static string PUBProcess1NotificationCC
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBProcess1NotificationCC, Product.IACM);
                return config == null ? string.Empty : config.ConfigValue;
            }
        }

        public static string PUBProcess2NotificationCC
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBProcess2NotificationCC, Product.IACM);
                return config == null ? string.Empty : config.ConfigValue;
            }
        }

        public static string PUBProcess3NotificationCC
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBProcess3NotificationCC, Product.IACM);
                return config == null ? string.Empty : config.ConfigValue;
            }
        }

        public static string PUBProcess4NotificationCC
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUBProcess4NotificationCC, Product.IACM);
                return config == null ? string.Empty : config.ConfigValue;
            }
        }

        public static string PUB_CostCenter
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUB_CostCenter, Product.IACM);
                if (config == null) return string.Empty;

                var list = config.ConfigValue.Split(',').ToList() ;

                return string.Join(",", list.Select(s => string.Format("'{0}'", s.Trim() )));
            }
        }

        public static string PUB_Company
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.PUB_Company, Product.IACM);
                if (config == null) return string.Empty;

                var list = config.ConfigValue.Split(',').ToList();

                return string.Join(",", list.Select(s => string.Format("'{0}'", s.Trim())));
            }
        }
        public static double HRMSDataUpdateTimer
        {
            get
            {
                var config = GetConfigValue("HRMSDataUpdateTimer", Product.IM);
                return config != null && !string.IsNullOrWhiteSpace(config.ConfigValue) ? Convert.ToDouble(config.ConfigValue) : (10*60*1000);
            }
        }
        public static string InhouseAllowSamePermissionForAmendmentAdd
        {
            get
            {
                return GetConfigValue(ConfigConstants.InhouseAllowSamePermissionForAmendmentAdd, Product.IM)?.ConfigValue;
            }
        }
        public static long GetDailyHRMSProcessId
        {
            get
            {
                var cfgValue = GetConfigValue(ConfigConstants.DailyHRMSProcessId, Product.IM);
                if(cfgValue != null && !string.IsNullOrWhiteSpace(cfgValue.ConfigValue))
                {
                    return Convert.ToInt64(cfgValue.ConfigValue);
                }
                return 4;

            }
        }
        public static string PhoneTypeForMobile
        {
            get
            {
                return GetConfigValue(ConfigConstants.PhoneTypeForMobile, Product.IM)?.ConfigValue;
            }
        }

        public static string PhoneTypeForTelephone
        {
            get
            {
                return GetConfigValue(ConfigConstants.PhoneTypeForTelephone, Product.IM)?.ConfigValue;
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
                    if (long.TryParse(e,out val))
                    {
                        endpoints.Add(Convert.ToInt64(e));
                    }
                });
                return endpoints;
            }
        }
        public static string AttestationFlinsWarningMessage
        {
            get
            {
                return GetConfigValue(ConfigConstants.AttestationFlinsWarningMessage, Product.IM)?.ConfigValue;
            }
        }
        public static string CRON_Expression_WorkflowReminderTimer
        {
            get
            {
                string defaultValue = "0 */2 * ? * *";
                var config = GetConfigValue("WorkflowReminderTimer", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }

        public static bool AllowAttestationTaskReminder
        {
            get
            {
                bool defaultValue = true;
                var config = GetConfigValue("AllowAttestationTaskReminder", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    bool isAllowed;
                    if (bool.TryParse(config.ConfigValue, out isAllowed))
                    {
                        defaultValue = Convert.ToBoolean(isAllowed);
                    }
                }
                return defaultValue;
            }
        }
        public static bool AllowProvisioningTaskReminder
        {
            get
            {
                bool defaultValue = false;
                var config = GetConfigValue("AllowProvisioningTaskReminder", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue) && (config.ConfigValue == "1" || config.ConfigValue.ToUpper() == "TRUE"))
                {
                    defaultValue = true;
                }
                return defaultValue;
            }
        }
        public static string CRON_Expression_AttestationReportTimer
        {
            get
            {
                string defaultValue = "0 0 */3 ? * *";//Default every 3 hours
                var config = GetConfigValue("AttestationGenerateReportTimer", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }


        public static string CRON_Expression_PurgeConfigTimer
        {
            get
            {
                string defaultValue = "0 0 12 1 1 ? *";//Default every year 

                // string defaultValue = "* * * * *"; //Default every minute 
                var config = GetConfigValue("PurgeConfigurationTimer", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }

        public static bool CanGenerateAttestationReport
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.CanGenerateAttestationReport, Product.IM);
                return (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue) && config.ConfigValue == "1") ? true : false;
            }
        }

        public static string SelfServiceAccountType
        {
            get
            {
                var selfserviceaccounttype = GetConfigValue(ConfigConstants.SELFSERVICEACCOUNTTYPE, Product.IM);
                return GetConfigValue(ConfigConstants.SELFSERVICEACCOUNTTYPE, Product.IM)?.ConfigValue;

            }
        }

        /// <summary>
        /// Used to differentiate the UAT and Production. Default it should take Production. 
        /// Order of Endpoint System is differ from Production and UAT. Default Yes.
        /// </summary>
        public static bool EnableOnBoardingRequestSequenceInProdution
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.EnableOnBoardingRequestSequence, Product.IM);
                if(config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    return config.ConfigValue == "1" ? true : false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static string TCodeReportFilePath
        {
            get
            {
                string defaultValue = "C:\\temp\\";
                var config = GetConfigValue("AttestationTCodeReportFilePath", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }
        public static List<string> IDMSAttestationRoleToExclude
        {
            get
            {
                var list = new List<string>();
                var config = GetConfigValue(ConfigConstants.IDMSAttestationRoleToExclude, Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    var roles = config.ConfigValue.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var role in roles)
                    {
                        list.Add(role.Trim());
                    }
                }
                else
                {
                    list.Add("User");
                    list.Add("Manager");
                }
                return list;
            }
        }
        public static string PasswordResetSAPErrorMessageDescription
        {
            get
            {
                var msg = string.Empty;
                var obj = GetConfigValue(ConfigConstants.SAPPasswordErrorDescription, Product.IM);
                if(obj != null)
                {
                    msg = obj.ConfigValue;
                }
                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = "IDMS sent you a temporary password. Please check your mail and login with temporary password";
                }
                return msg;
            }
        }
        public static List<KeyValuePair<string, string>> SFEEGroupValueAndName
        {
            get
            {
                var keyValue = new List<KeyValuePair<string, string>>();
                var config = GetConfigValue(ConfigConstants.SF_EEGroupValueAndName, Product.IM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    keyValue.Add(new KeyValuePair<string, string>("1", "Exec:VPs & Above"));
                    keyValue.Add(new KeyValuePair<string, string>("2", "Exec: Below VP"));
                    keyValue.Add(new KeyValuePair<string, string>("3", "Non Executive"));
                    keyValue.Add(new KeyValuePair<string, string>("I", "Intern"));
                }
                else
                {
                    var values = config.ConfigValue.SplitBySeparator("|", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var value in values)
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            var data = value.SplitBySeparator("~", StringSplitOptions.RemoveEmptyEntries);
                            if (data != null && data.Length == 2 && !string.IsNullOrWhiteSpace(data[0]) && !string.IsNullOrWhiteSpace(data[1]))
                            {
                                if(!keyValue.Exists(i=> i.Key == data[0]))
                                    keyValue.Add(new KeyValuePair<string, string>(data[0].Trim(), data[1].Trim()));
                            }
                        }
                    }
                }
                return keyValue;
            }
        }
        /// <summary>
        /// Used to Create AD Propagation
        /// </summary>
        public static bool CreateADPropagation
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.CreateADPropagation, Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    return config.ConfigValue == "1" ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        public static double ADPropagationTimer
        {
            get
            {
                var config = GetConfigValue("ADPropagationTimer", Product.IM);
                return config != null && !string.IsNullOrWhiteSpace(config.ConfigValue) ? Convert.ToDouble(config.ConfigValue) : (5 * 60 * 1000);
            }
        }
        public static string O365OnlineDomainName
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.O365OnlineDomainName, Product.IM);
                if(config == null || string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    return "sppower.onmicrosoft.com";
                }
                return config.ConfigValue;
            }
        }
        public static string[] Office365PSModule
        {
            get
            {
                var mPath = @"C:\Program Files\WindowsPowerShell\Modules\ExchangeOnlineManagementV1\CreateExoPSSession.ps1";
                var modules = new List<string>();
                var config = GetConfigValue("Office365PSModule", Product.IM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    modules.Add(mPath);
                }
                else
                {
                    var data = config.ConfigValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if(data == null || data.Length == 0)
                    {
                        modules.Add(mPath);
                    }
                    else
                    {
                        foreach (var item in data)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                modules.Add(item.Trim());
                            }
                        }
                    }
                }
                return modules.ToArray();
            }
        }
        public static bool IsDeploymentInProgress
        {
            get
            {
                var config = GetConfigValue(ConfigConstants.IsDeploymentInProgress, Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    return config.ConfigValue == "1" ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        public static string[] MicrosoftTeamsPSModule
        {
            get
            {
                var mPath = @"C:\Program Files\WindowsPowerShell\Modules\MicrosoftTeams\2.3.1\MicrosoftTeams.psd1";
                var modules = new List<string>();
                var config = GetConfigValue("MicrosoftTeamsPSModule", Product.IM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    modules.Add(mPath);
                }
                else
                {
                    var data = config.ConfigValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (data == null || data.Length == 0)
                    {
                        modules.Add(mPath);
                    }
                    else
                    {
                        foreach (var item in data)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                modules.Add(item.Trim());
                            }
                        }
                    }
                }
                return modules.ToArray();
            }
        }
        public static string[] Office365PSModuleForMSTeam
        {
            get
            {
                var mPath = @"C:\Program Files\WindowsPowerShell\Modules\ExchangeOnlineManagement\2.0.5\ExchangeOnlineManagement.psd1";
                var modules = new List<string>();
                var config = GetConfigValue("Office365PSModuleForMSTeam", Product.IM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    modules.Add(mPath);
                }
                else
                {
                    var data = config.ConfigValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (data == null || data.Length == 0)
                    {
                        modules.Add(mPath);
                    }
                    else
                    {
                        foreach (var item in data)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                modules.Add(item.Trim());
                            }
                        }
                    }
                }
                return modules.ToArray();
            }
        }
        public static bool EnableSFFailedNotification
        {
            get
            {
                var config = GetConfigValue("EnableSFFailedNotification", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    return config.ConfigValue == "1" ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        public static int PSVersion
        {
            get
            {
                var emailLimit = GetConfigValue(ConfigConstants.PSVersion, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(emailLimit))
                {
                    int limit;
                    if (Int32.TryParse(emailLimit, out limit))
                    {
                        return limit;
                    }
                }
                return 5;
            }
        }
        public static int MinorPSVersion
        {
            get
            {
                var emailLimit = GetConfigValue(ConfigConstants.PowershellMinorVersion, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(emailLimit))
                {
                    int limit;
                    if (Int32.TryParse(emailLimit, out limit))
                    {
                        return limit;
                    }
                }
                return 0;
            }
        }
        public static int SFAPITimeoutDuration
        {
            get
            {
                var emailLimit = GetConfigValue(ConfigConstants.SuccessfactorAPITimeout, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(emailLimit))
                {
                    int limit;
                    if (Int32.TryParse(emailLimit, out limit))
                    {
                        return limit;
                    }
                }
                return 180;
            }
        }

        public static bool CanRunUpdateOneCardBadgeInADAttribute
        {
            get
            {
                var value = GetConfigValue(ConfigConstants.CAN_RUN_UPDATE_AD_ONECARD_BADGE_JOB, Product.IM).ConfigValue;
                return value == "1" ? true : false;
            }
        }

        public static string CRON_Expression_CustomReportTimer
        {
            get
            {
                string defaultValue = "0 */2 * ? * *";
                var config = GetConfigValue("AccessRequestReportTimer", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }

        public static string AccessRequestReportFilePath
        {
            get
            {
                string defaultValue = "C:\\temp\\";
                var config = GetConfigValue("AccessRequestReportFilePath", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }

        public static string CRON_Expression_UpdateADManagerTimer
        {
            get
            {
                string defaultValue = "0 30 12 * * ?";
                var config = GetConfigValue("UpdateADManagerTimer", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }
        public static string PasswordMessage
        {
            get
            {
                var pwdMsg = GetConfigValue(ConfigConstants.PasswordMessage, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(pwdMsg))
                {
                    return pwdMsg;
                }
                else
                {
                    return "Please use your {0} password to login and ignore the change password instructions below.";
                }

            }
        }

        public static string CRON_Expression_DecommissionSystemProcess
        {
            get
            {
                string defaultValue = "0 30 12 * * ?";
                var config = GetConfigValue("DecommissionSystemTimer", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }
        /// <summary>
        /// Used to deactivate the expired Advanced delegation and default value is Every day at 1am. Admin can change this setting in IMConfiguration.
        /// </summary>
        public static string CRON_Expression_DeactivateExpiredAdvancedDelegation
        {
            get
            {
                string defaultValue = "0 0 1 * * ?";
                var config = GetConfigValue("DeactivateExpiredDelegationTimer", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }
        public static string CRON_Expression_DiscoveryRestart
        {
            get
            {
                string defaultValue = "0 0 18 * * ?";
                var config = GetConfigValue("RestartDiscoveryTimer", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }
        /// <summary>
        /// Delay the campaign generation to avoid deadlock; Delay in millisecond default 8000(8 Seconds)
        /// </summary>
        public static int DelayAttestationCampaignCreation
        {
            get
            {
                var delay = GetConfigValue(ConfigConstants.DelayAttestationCampaignCreation, Product.IM)?.ConfigValue;
                if (!string.IsNullOrWhiteSpace(delay))
                {
                    int limit;
                    if (Int32.TryParse(delay, out limit))
                    {
                        return limit;
                    }
                }
                return 8000;
            }
        }
        public static string TranferTriggersNotes
        {
            get
            {
                string defaultValue = "Yes";//As per Success factor the value is Yes
                var config = GetConfigValue(ConfigConstants.TranferTriggersNotes, Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue.Trim();
                }
                return defaultValue;
            }
        }

        public static string CRON_Expression_OtherSystemDiscoveryTimer
        {
            get
            {
                string defaultValue = "0 */2 * ? * *";
                var config = GetConfigValue("OtherSystemDiscoveryTimer", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
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
        public static string Office365PSAzureADModule
        {
            get
            {
                var mPath = @"";
                var modules = string.Empty;
                var config = GetConfigValue("Office365PSAzureADModule", Product.IM);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    return modules;
                }
                else
                {
                    modules = config.ConfigValue;
                }
                return modules;
            }
        }

        public static string UserAttestationComparisionNotes
        {
            get
            {
                string defaultValue = "Below the entitlement to click on Tick to revoke the access";
                var config = GetConfigValue("UserAttestationComparisionNotes", Product.IM);
                if (config != null && !string.IsNullOrWhiteSpace(config.ConfigValue))
                {
                    defaultValue = config.ConfigValue;
                }
                return defaultValue;
            }
        }
        /*SAP HANA*/
        public static string[] SAPHANAMSPDefaultProfile
        {
            get
            {
                var val = GetConfigValue(ConfigConstants.SAPHANAMSP_DEFAULT_PROFILE, Product.IM);
                if (val == null || val.ConfigValue == null)
                    return null;

                return Convert.ToString(val.ConfigValue).Split(',');
            }
        }

        public static string EBSSAPHANAPhoneTaskId
        {
            get
            {
                var val = GetConfigValue(ConfigConstants.EBS_SAPHANA_PHONE_TASKID, Product.IM);
                if (val == null || val.ConfigValue == null)
                    return null;

                return Convert.ToString(GetConfigValue(ConfigConstants.EBS_SAPHANA_PHONE_TASKID, Product.IM).ConfigValue);
            }
        }

        public static string MSSLSAPHANAPhoneTaskId
        {
            get
            {
                var val = GetConfigValue(ConfigConstants.MSSL_SAPHANA_PHONE_TASKID, Product.IM);
                if (val == null || val.ConfigValue == null)
                    return null;

                return Convert.ToString(GetConfigValue(ConfigConstants.MSSL_SAPHANA_PHONE_TASKID, Product.IM).ConfigValue);
            }
        }

        public static bool ExcludeHiringNotCompleted
        {
            get
            {
                var val = GetConfigValue(ConfigConstants.EXCLUDE_HIRING_NOT_COMPLETED, Product.IACM);
                if (val == null || val.ConfigValue == null)
                    return false;

                return Convert.ToBoolean(val.ConfigValue);
            }
        }

    }
}
