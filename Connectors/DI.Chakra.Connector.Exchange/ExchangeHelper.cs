using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using ActiveDs;
using System.Collections;
using System.Management.Automation;

namespace DI.Chakra.Connector
{
    public class ExHelper
    {
       
        /// <summary>
        /// This will retreive the Specified Property Value from the Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directory Object to retrieve from</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public static string GetProperty(PSObject pso, string sPropertyName)
        {
            if (pso.Members.Contains(pso.Members[sPropertyName]))
            {

                return pso.Members[sPropertyName].Value.ToString();

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

        public static string GetProperty_GUID(PSObject pso, string sPropertyName)
        {
            byte[] info = GetProperty_Byte(pso, sPropertyName);

            if (null == info)
                return string.Empty;

            return new Guid(info).ToString();
        }

        public static byte[] GetProperty_Byte(PSObject pso, string sPropertyName)
        {
            if (pso.Members.Contains(pso.Members[sPropertyName]))
            {
                return (byte[])pso.Properties[sPropertyName].Value;
            }
            else
            {
                return null;
            }
        }
   
    }
}
