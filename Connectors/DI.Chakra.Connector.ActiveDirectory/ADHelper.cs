using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using ActiveDs;
using System.Collections;
using System.Runtime.InteropServices;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DomainModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace DI.Chakra.Connector
{
    public class ADHelper
    {

        /// <summary>
        /// This will retreive the Specified Property Value from the Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directory Object to retrieve from</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public static string GetProperty(DirectoryEntry oDE, string sPropertyName)
        {
            if (oDE.Properties.Contains(sPropertyName))
            {
                return oDE.Properties[sPropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// This will retreive the Specified Property Value if its an Array Type from the Directory Entry object
        /// </summary>
        /// <param name="oDE">The Directory Object to retrieve from</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public static ArrayList GetProperty_Array(DirectoryEntry oDE, string sPropertyName)
        {
            ArrayList myItems = new ArrayList();
            if (oDE.Properties.Contains(sPropertyName))
            {
                for (int i = 0; i < oDE.Properties[sPropertyName].Count; i++)
                {
                    myItems.Add(oDE.Properties[sPropertyName][i].ToString());
                }
                return myItems;
            }
            else
            {
                return myItems;
            }
        }

        /// <summary>
        /// This will retreive the Specified Property Value if its a Byte Type from the Directory Entry object
        /// </summary>
        /// <param name="oDE">The Directory Object to retrieve from</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public static byte[] GetProperty_Byte(DirectoryEntry oDE, string sPropertyName)
        {
            if (oDE.Properties.Contains(sPropertyName))
            {
                return (byte[])oDE.Properties[sPropertyName].Value;
            }
            else
            {
                return null;
            }
        }

        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public static string GetProperty_SID(DirectoryEntry oDE, string sPropertyName)
        {
            byte[] info = GetProperty_Byte(oDE, sPropertyName);

            if (info == null)
                return string.Empty;


            System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(info, 0);
            return sid.ToString();
        }
        public static string GetProperty_GUID(DirectoryEntry oDE, string sPropertyName)
        {
            byte[] info = GetProperty_Byte(oDE, sPropertyName);

            if (null == info)
                return string.Empty;

            return new Guid(info).ToString();
        }
        public static DateTime? GetProperty_DateTime(DirectoryEntry oDE, string sPropertyName)
        {
            string str = GetProperty(oDE, sPropertyName);

            if (string.Empty == str || "0" == str)
                return null;
            else
                return Convert.ToDateTime(str);

        }
        //TODO: Immediate,Wasim Write Nullable properties
        public static int GetProperty_Int(DirectoryEntry oDE, string sPropertyName)
        {
            string str = GetProperty(oDE, sPropertyName);

            if (string.Empty == str)
                return -1;
            else
                return Convert.ToInt32(str);

        }

        public static DateTime? GetCOMProperty_DateTime(DirectoryEntry oDE, string sPropertyName)
        {
            if (oDE.Properties.Contains(sPropertyName))
            {
                IADsLargeInteger li_ad = (IADsLargeInteger)oDE.Properties[sPropertyName].Value;
                long dtLng = GetLongFromLargeInteger(li_ad);

                if (dtLng == 0)
                    return null;

                //Can't change AccountExpires value(for eg. Administrator)
                if (long.MaxValue == dtLng)
                    return null;

                return DateTime.FromFileTime(dtLng);
            }
            else
            {
                return null;
            }
        }
        public static Int64 GetCOMProperty_Int64(DirectoryEntry oDE, string sPropertyName)
        {
            if (oDE.Properties.Contains(sPropertyName))
            {
                IADsLargeInteger li_ad = (IADsLargeInteger)oDE.Properties[sPropertyName].Value;
                return GetLongFromLargeInteger(li_ad);
            }
            return -1;
        }
        private static long GetLongFromLargeInteger(IADsLargeInteger Li)
        {
            long retval = Li.HighPart;
            retval <<= 32;
            retval |= (uint)Li.LowPart;
            return retval;
        }
        public static Int64 GetInt64FromLargeInteger(object largeInteger)
        {
            //Declarations
            Int32 low;
            Int32 high;
            byte[] valBytes = new byte[70];

            try
            {
                ActiveDs.IADsLargeInteger longInt = (ActiveDs.IADsLargeInteger)largeInteger;
                low = longInt.LowPart;
                high = longInt.HighPart;


                BitConverter.GetBytes(low).CopyTo(valBytes, 0);
                BitConverter.GetBytes(high).CopyTo(valBytes, 4);
            }
            catch (COMException ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.Source);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.Source);
                return 0;
            }
            finally
            {

            }

            return BitConverter.ToInt64(valBytes, 0);

        }

        /// <summary>
        /// This is an Override that will Allow a Property to be Extracted Directly
        /// from a Search Result Object
        /// </summary>
        /// <param name="oSearchResult">The Search Result</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public static string GetProperty(SearchResult oSearchResult, string sPropertyName)
        {
            if (oSearchResult.Properties.Contains(sPropertyName))
            {
                return oSearchResult.Properties[sPropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        public static string Configuration(EndpointKey Key, EndpointConfiguration endpointConfig)
        {
            var config = endpointConfig.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }
        public static void SetDirectoryPath(DirectoryEntry rootEntry, string domainString, EndpointConfiguration endpointConfig)
        {
            string getSSL = Configuration(EndpointKey.SSL, endpointConfig);
            string portno = Configuration(EndpointKey.PortNo, endpointConfig);
            string hostName = Configuration(EndpointKey.IPAddress, endpointConfig);
            bool isSSLEnabled = false;

            

            if (!string.IsNullOrEmpty(getSSL))
                isSSLEnabled = Convert.ToBoolean(getSSL);

            if (isSSLEnabled)
                rootEntry.AuthenticationType = AuthenticationTypes.SecureSocketsLayer;
            else
                rootEntry.AuthenticationType = AuthenticationTypes.Secure | AuthenticationTypes.ServerBind;

            if (string.IsNullOrEmpty(portno))
                rootEntry.Path = string.Format("LDAP://{0}/{1}", hostName, domainString);
            else
                rootEntry.Path = string.Format("LDAP://{0}:{1}/{2}", hostName, portno, domainString);
        }
        public static DirectoryEntry GetDirectoryEntry(objectClass objectCls, string objectName, string LdapDomain, DirectoryEntry rootEntry)
        {
            DirectorySearcher mySearcher = new DirectorySearcher(rootEntry);

            switch (objectCls)
            {
                case objectClass.user:
                    mySearcher.Filter = "(&(objectClass=user)(|(cn=" + objectName + ")(sAMAccountName=" + objectName + ")))";
                    break;
                case objectClass.group:
                    mySearcher.Filter = "(&(objectClass=group)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                    break;
                case objectClass.computer:
                    mySearcher.Filter = "(&(objectClass=computer)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                    break;
            }
            SearchResult result = mySearcher.FindOne();

            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                objectName + " in the " + LdapDomain + " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();

            return directoryObject;
        }
        public static DirectoryEntry GetDirectoryEntries(string objectName, DirectoryEntry rootEntry)
        {
            DirectoryEntry directoryObject = null;
            DirectorySearcher mySearcher = new DirectorySearcher(rootEntry);
            mySearcher.Filter = "(&(objectClass=user)(|(cn=" + objectName + ")(sAMAccountName=" + objectName + ")))";
            SearchResult result = mySearcher.FindOne();
            if (result == null)
            {
                mySearcher.Filter = "(&(objectClass=group)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                result = mySearcher.FindOne();
            }
            if (result != null)
                directoryObject = result.GetDirectoryEntry();

            return directoryObject;
        }
        public static string GetObjectDistinguishedName(objectClass objectCls, returnType returnValue, string objectName, string LdapDomain, DirectoryEntry root)
        {
            string distinguishedName = string.Empty;


            DirectorySearcher mySearcher = new DirectorySearcher(root);

            switch (objectCls)
            {
                case objectClass.user:
                    mySearcher.Filter = "(&(objectClass=user)(|(cn=" + objectName + ")(sAMAccountName=" + objectName + ")))";
                    break;
                case objectClass.group:
                    mySearcher.Filter = "(&(objectClass=group)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                    break;
                case objectClass.computer:
                    mySearcher.Filter = "(&(objectClass=computer)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                    break;
            }
            SearchResult result = mySearcher.FindOne();

            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                objectName + " in the " + LdapDomain + " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();
            if (returnValue.Equals(returnType.distinguishedName))
            {
                distinguishedName = directoryObject.Properties["distinguishedName"].Value.ToString();
            }
            if (returnValue.Equals(returnType.ObjectGUID))
            {
                distinguishedName = directoryObject.Guid.ToString();
            }
            mySearcher.Dispose();
            return distinguishedName;
        }
    }
}
