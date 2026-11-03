using System;
using System.Configuration;

namespace DI.Chakra.Infrastructure
{
    public class AppSettingsHelper
    {

        public static string DatabaseConnectionStringName
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["Connection"]);
            }
        }

        /// <summary>
        /// Command timeout (seconds) for Entity Framework queries. Read from the optional
        /// "DbCommandTimeout" appSetting; defaults to 300 when missing or invalid.
        /// EF's built-in default is 30s, which long upload/download queries exceed.
        /// </summary>
        public static int DbCommandTimeout
        {
            get
            {
                int timeout;
                return int.TryParse(ConfigurationManager.AppSettings["DbCommandTimeout"], out timeout) && timeout >= 0
                    ? timeout
                    : 300;
            }
        }
        public static string DatabaseConnectionString
        {
            get
            {
                string connectionName = DatabaseConnectionStringName;
                return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            }
        }
        public static string DomainApiBaseUrl
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["DomainApiBaseUrl"]);
            }
        }
        public static string EnableBundling
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["EnableBundling"]??"");
            }
        }
        public static string ReportApiBaseUrl
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ReportApiBaseUrl"]);
            }
        }
        public static string DomainServiceBaseUrl
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["DomainServiceBaseUrl"]);
            }
        }

        public static double ProvisioningTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["ProvisioningTimerIntervalInMinutes"]);
            }
        }
        public static int ComplianceTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ComplianceTimerIntervalInMinutes"]);
            }
        }
        public static int MiscProcessTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["MiscProcessTimerIntervalInMinutes"]);
            }
        }

        public static int ExpiredRequestEmailNotificationDays
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ExpiredRequestEmailNotificationDays"]); 
            }
        }

        public static int TelephoneDirectorySyncTimeInHourOfDay
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["TelephoneDirectorySyncTimeInHourOfDay"]);
            }
        }

        public static int TrustedSourceChangePropagationTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["TrustedSourceChangePropagationTimerIntervalInMinutes"]);
            }
        }

        public static int AttestationTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["AttestationTimerIntervalInMinutes"]);
            }
        }

       /* public static int HousekeepingTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["HousekeepingTimerIntervalInMinutes"]);
            }
        }*/
        public static int MiscellaneousTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["MiscellaneousTimerIntervalInMinutes"]);
            }
        }
        public static int WakeupWorkflowTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["WakeupWorkflowTimerIntervalInMinutes"]);
            }
        }
        public static int WakeupServiceTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["WakeupServiceTimerIntervalInMinutes"]);
            }
        }

        public static int HealthCheckServiceTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["HealthCheckServiceTimerIntervalInMinutes"]);
            }
        }


        public static int EmailingTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["EmailingTimerIntervalInMinutes"]);
            }
        }

        public static int DiscoveryTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["DiscoveryTimerIntervalInMinutes"]);
            }
        }

        public static int DashBoardDataInDays
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["DashBoardDataInDays"]);
            }
        }

        public static string AutoDeprovisionExpireAccessRequest
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["AutoDeprovisionExpireAccessRequest"]); 
            }
        }

        public static int ReportingTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ReportingServicesTimerIntervalInMinutes"]);
            }
        }

        public static string WFService
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["WFService"]);
            }
        }

        public static int AdHocJobTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["AdHocJobTimerIntervalInMinutes"]);
            }
        }
        public static int DiscoveryInstance
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["DiscoveryInstance"]);
            }
        }
        private static bool _IsFirstTimeDBExecute = false; 
        public static bool IsFirstTimeDBExecute 
        {
            get
            {
                return _IsFirstTimeDBExecute;
            }
            set
            {
                _IsFirstTimeDBExecute = value;
            }
        }
        public static int MinuteMultiplier
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["MinuteMultiplier"]);
            }
        }

        public static int TCodeTimerIntervalInMinutes { get; set; }
    }
}
