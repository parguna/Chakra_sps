using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector
{
    public class SkypeForBusinessHelper
    {
        /// <summary>
        /// This will retreive the Specified Property Value from the Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directory Object to retrieve from</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public static string GetProperty(PSObject pso, string sPropertyName)
        {

            try
            {
                if (pso.Members.Contains(pso.Members[sPropertyName]))
                {
                    if (pso.Members[sPropertyName] != null && pso.Members[sPropertyName].Value != null)
                        return pso.Members[sPropertyName].Value.ToString();
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"{ex.Message}", ex);
                return null;
            }
        }
        public static bool GetBooleanProperty(PSObject pso, string sPropertyName)
        {
            try
            {
                if (pso.Members.Contains(pso.Members[sPropertyName]))
                {
                    if (pso.Members[sPropertyName] != null && pso.Members[sPropertyName].Value != null)
                        return pso.Members[sPropertyName].Value.ToString().Equals("True", StringComparison.InvariantCultureIgnoreCase) ? true : false;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }
        public static Guid? GetGuidProperty(PSObject pso, string sPropertyName)
        {
            try
            {
                if (pso.Members.Contains(pso.Members[sPropertyName]))
                {
                    if (pso.Members[sPropertyName] != null && pso.Members[sPropertyName].Value != null)
                        return Guid.Parse(pso.Members[sPropertyName].Value.ToString());
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            
        }
        public static int GetIntegerProperty(PSObject pso, string sPropertyName)
        {
            try
            {
                if (pso.Members.Contains(pso.Members[sPropertyName]))
                {
                    if (pso.Members[sPropertyName] != null && pso.Members[sPropertyName].Value != null && !string.IsNullOrWhiteSpace(pso.Members[sPropertyName].Value.ToString()))
                    {
                        int num;
                        if (int.TryParse(pso.Members[sPropertyName].Value.ToString(), out num))
                        {
                            return num;
                        }
                        return 0;

                    }
                    else
                        return 0;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
            
        }
        public static DateTime? GetProperty_DateTime(PSObject pso, string sPropertyName)
        {
            try
            {
                string str = GetProperty(pso, sPropertyName);

                if (string.IsNullOrWhiteSpace(str) || "0" == str)
                    return null;
                else
                    return Convert.ToDateTime(str);
            }
            catch
            {
                return null;
            }
            
        }
    }
}
