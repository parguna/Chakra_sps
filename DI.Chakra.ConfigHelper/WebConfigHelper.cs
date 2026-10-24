using DeepIdentity.Infrastructure;
using System;
using System.Configuration;
using System.Web.Configuration;

namespace DeepIdentity.ConfigHelper
{
    public class WebConfigHelper
    {
        public static void WebConfigChange(string key, string Value)
        {

            try
            {
                Configuration configuration;
                configuration = WebConfigurationManager.OpenWebConfiguration("~");
                string s = ConfigurationManager.AppSettings[key].ToString();
                if (!String.IsNullOrEmpty(s))
                {
                    // Key exists
                    configuration.AppSettings.Settings[key].Value = Value;
                    configuration.Save();

                    Log.UIAuditLog.WriteMessage(LogLevel.INFO, string.Format("Configuration Key {0} is Changed with value {1}", key, Value));
                }
                else
                {
                    // Key doesn't exist
                    throw new EntityNotFoundException("Key is not found for {0}", key);

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static void getWebConfigValue(string key, out string Value)
        {
            Value = string.Empty;
            try
            {
                if (key != string.Empty)
                {
                    Configuration configuration;
                    configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    string s = ConfigurationManager.AppSettings[key].ToString();
                    if (!String.IsNullOrEmpty(s))
                        Value = configuration.AppSettings.Settings[key].Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void WebConfigSessionModel(bool bAllowMultipleSession)
        {

            try
            {
                Configuration configuration;
                configuration = WebConfigurationManager.OpenWebConfiguration("~");

                if (bAllowMultipleSession)
                    configuration.AppSettings.Settings["AllowMultipleSession"].Value = "True";
                else
                    configuration.AppSettings.Settings["AllowMultipleSession"].Value = "False";

                configuration.Save();

                Log.UIAuditLog.WriteMessage(LogLevel.INFO, string.Format("Configuration Key {0} is Changed with value {1}", "AllowMultipleSession", bAllowMultipleSession.ToString()));

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }       
    }
}
