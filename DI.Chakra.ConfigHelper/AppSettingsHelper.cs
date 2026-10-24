using System;
using System.Configuration;

namespace DeepIdentity.ConfigHelper
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

        public static int ProvisioningTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["ProvisioningTimerIntervalInMinutes"]);
            }
        }

        public static int MiscProcessTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["MiscProcessTimerIntervalInMinutes"]);
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

        public static int HousekeepingTimerIntervalInMinutes
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["HousekeepingTimerIntervalInMinutes"]);
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

    }
}
