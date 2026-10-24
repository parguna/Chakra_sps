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
     

        //public static int GetProperty_Int(DirectoryEntry oDE, string sPropertyName)
        //{
        //    string str = GetProperty(oDE, sPropertyName);

        //    if (string.Empty == str)
        //        return -1;
        //    else
        //        return Convert.ToInt32(str);

        //}

        ///// <summary>
        ///// This will retreive the Specified Property Value if its an Array Type from the Directory Entry object
        ///// </summary>
        ///// <param name="oDE">The Directory Object to retrieve from</param>
        ///// <param name="sPropertyName">The Property to retrieve</param>
        ///// <returns></returns>
        //public static  ArrayList GetProperty_Array(DirectoryEntry oDE, string sPropertyName)
        //{
        //    ArrayList myItems = new ArrayList();
        //    if (oDE.Properties.Contains(sPropertyName))
        //    {
        //        for (int i = 0; i < oDE.Properties[sPropertyName].Count; i++)
        //        {
        //            myItems.Add(oDE.Properties[sPropertyName][i].ToString());
        //        }
        //        return myItems;
        //    }
        //    else
        //    {
        //        return myItems;
        //    }
        //}

        ///// <summary>
        ///// This will retreive the Specified Property Value if its a Byte Type from the Directory Entry object
        ///// </summary>
        ///// <param name="oDE">The Directory Object to retrieve from</param>
        ///// <param name="sPropertyName">The Property to retrieve</param>
        ///// <returns></returns>
     
        //public static string GetProperty_SID(DirectoryEntry oDE, string sPropertyName)
        //{
        //    byte[] info = GetProperty_Byte(oDE, sPropertyName);

        //    if (info == null)
        //        return string.Empty;


        //    System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(info, 0);
        //    return sid.ToString();
        //}
       
        //public static DateTime? GetCOMProperty_DateTime(DirectoryEntry oDE, string sPropertyName)
        //{
        //    if (oDE.Properties.Contains(sPropertyName))
        //    {
        //        IADsLargeInteger li_ad = (IADsLargeInteger)oDE.Properties[sPropertyName].Value;
        //        long dtLng = GetLongFromLargeInteger(li_ad);

        //        if (dtLng == 0)
        //            return null;

        //        //Can't change AccountExpires value(for eg. Administrator)
        //        if (long.MaxValue == dtLng)
        //            return null;
                
        //        return DateTime.FromFileTime(dtLng);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //public static Int64 GetCOMProperty_Int64(DirectoryEntry oDE, string sPropertyName)
        //{
        //    if (oDE.Properties.Contains(sPropertyName))
        //    {
        //        IADsLargeInteger li_ad = (IADsLargeInteger)oDE.Properties[sPropertyName].Value;
        //        return GetLongFromLargeInteger(li_ad);
        //    }
        //    return -1;
        //}
        //private static long GetLongFromLargeInteger(IADsLargeInteger Li)
        //{
        //    long retval = Li.HighPart;
        //    retval <<= 32;
        //    retval |= (uint)Li.LowPart;
        //    return retval;
        //}


        ///// <summary>
        ///// This is an Override that will Allow a Property to be Extracted Directly
        ///// from a Search Result Object
        ///// </summary>
        ///// <param name="oSearchResult">The Search Result</param>
        ///// <param name="sPropertyName">The Property to retrieve</param>
        ///// <returns></returns>
        //public static string GetProperty(SearchResult oSearchResult, string sPropertyName)
        //{
        //    if (oSearchResult.Properties.Contains(sPropertyName))
        //    {
        //        return oSearchResult.Properties[sPropertyName][0].ToString();
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}


    }
}
