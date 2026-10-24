using System.Configuration;

namespace DI.Chakra.Ruleset.Helper
{
    /// <summary>
    /// This class is a helper class to get the application's default configuration. 
    /// </summary>
    public class ConfigHelper
    {
        public static string RuleSetStoreConnectionString
        {
            get
            {
                string connectionType = ConfigurationManager.AppSettings["Connection"];
                string connectionString = string.Empty;
                ConnectionStringSettingsCollection connectionStringSettingsCollection = ConfigurationManager.ConnectionStrings;
                foreach (ConnectionStringSettings connectionStringSettings in connectionStringSettingsCollection)
                {
                    if (string.CompareOrdinal(connectionStringSettings.Name, connectionType) == 0)
                        connectionString = connectionStringSettings.ConnectionString;
                }

                return connectionString;
            }
        }

    }
}
