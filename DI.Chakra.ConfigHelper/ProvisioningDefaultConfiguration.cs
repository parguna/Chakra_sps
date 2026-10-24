using DeepIdentity.Infrastructure;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
//using DeepIdentity.Logger;
using DeepIdentity.Infrastructure.Enums;

namespace DeepIdentity.ConfigHelper
{
    public class ProvisioningDefaultConfiguration
    {
        public bool isConfigurationDataOK;

        #region IM General
        public int DeleteUserAfterNoOfDays { get; set; }
        public bool EnableEmailInProvisioning { get; set; }
        public bool UpdateEndpointFromTrustedSourceChanges { get; set; }
        public string PostAutoProvisioningRoleName { get; set; }
        public bool SAPTrustedSourceThreadSleepEnable { get; set; }
        public bool EnableEmailService { get; set; }
        #endregion

        #region Emails
        public string Email_HR { get; set; }
        public string Email_HelpDesk { get; set; }
        public string Email_ADAdmin { get; set; }
        public string Email_SAPAdmin { get; set; }
        public string Email_SAPEPAdmin { get; set; }
        public string Email_LotusNotesAdmin { get; set; }
        public string Email_NiSoftAdmin { get; set; }
        #endregion

        #region Access Request
        public int AutomaticClosureTimeOfChangeRequestInDays { get; set; }
        #endregion

        public ProvisioningDefaultConfiguration()
        {
            try
            {
                string path = string.Empty;
                XPathDocument configXMLDoc = null;
                try
                {
                    path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\IMConfiguration.xml";
                    configXMLDoc = new XPathDocument(path);
                }
                catch (System.IO.FileNotFoundException)
                {
                    path = ConfigurationManager.AppSettings["IMConfigurationFilePath"];
                    configXMLDoc = new XPathDocument(path);
                }

                XPathNavigator configXmlNav = configXMLDoc.CreateNavigator();
                XmlNamespaceManager ns = new XmlNamespaceManager(configXmlNav.NameTable);
                String strExpression = "//Configuration[@ActivityName='Provisioning']/Item ";
                XPathNodeIterator NodeIter = configXmlNav.Select(strExpression);

                string tmpkey;
                string tmpvalue;
                string tmptype;
                Type type;
                int tmpPropValue_Int;
                bool tmpPropValue_Bool;
                float tmpPropValue_Float;
                double tmpPropValue_Double;

                while (NodeIter.MoveNext())
                {
                    tmpkey = NodeIter.Current.GetAttribute("Key", ns.DefaultNamespace);
                    tmpvalue = NodeIter.Current.GetAttribute("Value", ns.DefaultNamespace);
                    tmptype = NodeIter.Current.GetAttribute("type", ns.DefaultNamespace);
                    type = typeof(ProvisioningDefaultConfiguration);

                    foreach (PropertyInfo pInfo in type.GetProperties())
                    {
                        if (pInfo.Name == tmpkey)
                        {
                            switch (tmptype)
                            {
                                case "int":
                                    int.TryParse(tmpvalue, out tmpPropValue_Int);
                                    pInfo.SetValue(this, tmpPropValue_Int, null);
                                    break;
                                case "bool":
                                    bool.TryParse(tmpvalue, out tmpPropValue_Bool);
                                    pInfo.SetValue(this, tmpPropValue_Bool, null);
                                    break;
                                case "float":
                                    float.TryParse(tmpvalue, out tmpPropValue_Float);
                                    pInfo.SetValue(this, tmpPropValue_Float, null);
                                    break;
                                case "double":
                                    double.TryParse(tmpvalue, out tmpPropValue_Double);
                                    pInfo.SetValue(this, tmpPropValue_Double, null);
                                    break;
                                case "string":
                                default:
                                    pInfo.SetValue(this, tmpvalue, null);
                                    break;
                            }
                            break;
                        }
                    }
                }
                isConfigurationDataOK = true;
            }
            catch (Exception ex)
            {
                isConfigurationDataOK = false;
                //DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Configuration Constant not Initialized", ex, ProvisioningEventType.Others, "Provisioning Engine");
              
            }
        }
    }
}
