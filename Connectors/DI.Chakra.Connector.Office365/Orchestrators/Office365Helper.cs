using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector
{
    public class Office365Helper
    {

        /// <summary>
        /// This will retreive the Specified Property Value from the Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directory Object to retrieve from</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public static string GetProperty(PSObject pso, string sPropertyName)
        {

            if (pso.Members.Any(i=> !string.IsNullOrWhiteSpace(i.Name) && i.Name.ToLower() == sPropertyName.ToLower()))
            {
                if (pso.Members[sPropertyName] != null && pso.Members[sPropertyName].Value != null)
                    return pso.Members[sPropertyName].Value.ToString();
                else
                    return null;
            }
            else
            {
                return string.Empty;
            }
        }

        public static DateTime? GetProperty_DateTime(PSObject pso, string sPropertyName)
        {
            string str = GetProperty(pso, sPropertyName);

            if (string.Empty == str || "0" == str)
                return null;
            else
                return Convert.ToDateTime(str);

        }
        public static bool? GetProperty_Boolean(PSObject pso, string sPropertyName)
        {
            string str = GetProperty(pso, sPropertyName);

            if (string.IsNullOrWhiteSpace(str) || "0" == str)
                return null;
            else
            {
                bool value;
                if (bool.TryParse(str, out value))
                {
                    return value;
                }
                return null;
            }
            
        }
    }
}
