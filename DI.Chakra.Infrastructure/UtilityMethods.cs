using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace DI.Chakra.Infrastructure
{
    [Serializable]
    public class PagedResults<T>
    {
        /// <summary>
        /// The page number this page represents.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The size of this page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of pages available.
        /// </summary>
        public int TotalNumberOfPages { get; set; }

        /// <summary>
        /// The total number of records available.
        /// </summary>
        public int TotalNumberOfRecords { get; set; }

        /// <summary>
        /// The URL to the next page - if null, there are no more pages.
        /// </summary>

        public List<T> Results { get; set; }

        public IEnumerable<T> QueryResults { get; set; }
    }
    public sealed class UtilityMethods
    {
        private UtilityMethods() { }
        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                if (EmailIsValid(email))
                {
                    var emailChecked = new System.Net.Mail.MailAddress(email);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool EmailIsValid(string email)
        {
            if (string.IsNullOrEmpty(email) || email.Length > 320) // RFC 5321 max address length
                return false;

            Regex reg = new Regex(@"^[a-zA-Z0-9.!#$%&amp;'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$", RegexOptions.None, TimeSpan.FromSeconds(2));
            try
            {
                return reg.IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static string StripLogForgingCharacters(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Replace("\r", "").Replace("\n", "");
        }

        /// <summary>
        /// Reduces a request URL (e.g. Request.Url.PathAndQuery) to a same-app relative path,
        /// so it cannot be used to redirect off-site (e.g. via a Host header or a leading "//").
        /// </summary>
        public static string GetSafeLocalRedirectUrl(string pathAndQuery, string fallback = "~/Default.aspx")
        {
            if (string.IsNullOrWhiteSpace(pathAndQuery))
                return fallback;

            string s = pathAndQuery.Replace('\\', '/');
            while (s.StartsWith("//"))
                s = s.Substring(1);

            if (!s.StartsWith("/"))
                return fallback;

            return s;
        }

        /// <summary>
        /// True only for a same-app relative URL (e.g. Request.Url.PathAndQuery), so callers can
        /// guard a redirect and refuse anything that could send the browser off-site (a leading
        /// "//" or "/\" is treated as scheme-relative, not local).
        /// </summary>
        public static bool IsLocalUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            string s = url.Replace('\\', '/');

            if (!s.StartsWith("/"))
                return false;

            if (s.StartsWith("//"))
                return false;

            return true;
        }
        public static bool IsValidMobileNumber(string phoneNumber)
        {

            Regex rx = new Regex(@"^[89]\d{7}$");
            return !string.IsNullOrWhiteSpace(phoneNumber) && rx.IsMatch(phoneNumber) && phoneNumber.Length == 8;
        }
        public static bool IsValidTelephoneNumber(string phoneNumber)
        {
            Regex rx = new Regex(@"^\d+$");
            return !string.IsNullOrWhiteSpace(phoneNumber) && rx.IsMatch(phoneNumber);
        }
        public static void DisableControls(System.Web.UI.Control control)
        {
            foreach (System.Web.UI.Control c in control.Controls)
            {
                // Get the Enabled property by reflection.
                Type type = c.GetType();
                PropertyInfo prop = type.GetProperty("Enabled");

                // Set it to False to disable the control.
                if (prop != null)
                {
                    prop.SetValue(c, false, null);
                }
                else
                {
                    prop = type.GetProperty("Disabled");
                    if (prop != null)
                    {
                        prop.SetValue(c, true, null);
                    }
                }
                // Recurse into child controls.
                if (c.Controls.Count > 0)
                {
                    DisableControls(c);
                }
            }
        }
        /// <summary>
        /// This method returns the xml representation of a given instance.
        /// </summary>
        /// <typeparam name="T">Represents the type of the instance that should be serialized</typeparam>
        /// <param name="instance">Object that should be serialised</param>
        /// <returns>Xml/String</returns>
        /// <remarks>The type should be serializable</remarks>
        public static string Serialize<T>(T instance, bool isAnything = false)
        {
            StringBuilder sb = new StringBuilder();
            if (isAnything)
            {
                return SerializeAnything(instance).ToString();
            }
            else
            {
                dynamic sretub = new StringBuilder();
                try
                {
                    using (TextWriter writer = new StringWriter(sb))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        serializer.Serialize(writer, instance);
                        return sb.ToString();
                    }
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }


        /// <summary>
        /// This method returns the xml representation of a given instance.
        /// </summary>
        /// <typeparam name="T">Represents the type of the instance that should be serialized</typeparam>
        /// <param name="instance">Object that should be serialised</param>
        /// <returns>Xml/String</returns>
        /// <remarks>The type should be serializable</remarks>
        public static string SerializeAsAttributes<T>(T instance)
        {
            dynamic sb = new StringBuilder();
            try
            {
                using (TextWriter writer = new StringWriter(sb))
                {
                    XmlAttributeOverrides overrides = GenerateXmlAttributeOverrides(typeof(T));
                    XmlSerializer serializer = new XmlSerializer(typeof(T), overrides);
                    serializer.Serialize(writer, instance);
                    return sb.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string DataSerialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public static object DataDeserialize(string xml, Type toType)
        {
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(memoryStream, Encoding.UTF8, new XmlDictionaryReaderQuotas(), null);
                DataContractSerializer serializer = new DataContractSerializer(toType);
                return serializer.ReadObject(reader);
            }
        }

        static XmlAttributeOverrides GenerateXmlAttributeOverrides(Type type)
        {
            XmlAttributeOverrides xmlAttributeOverrides = new XmlAttributeOverrides();
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if ((propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType == typeof(string)) &&
                    !propertyInfo.GetCustomAttributes().Any(a => a.GetType() == typeof(XmlElementAttribute) ||
                                                                 a.GetType() == typeof(XmlAttributeAttribute)))
                    xmlAttributeOverrides.Add(type, propertyInfo.Name, new XmlAttributes { XmlAttribute = new XmlAttributeAttribute() });
            }
            return xmlAttributeOverrides;
        }

        /// <summary>
        /// This method returns the xml representation of a given instance.
        /// </summary>
        /// <typeparam name="T">Represents the type of the instance that should be serialized</typeparam>
        /// <param name="instance">Object that should be serialised</param>
        /// <returns>Xml/String</returns>
        /// <remarks>The type should be serializable</remarks>
        public static string SerializeCollection<T>(List<T> instance)
        {
            dynamic sb = new StringBuilder();
            try
            {
                using (TextWriter writer = new StringWriter(sb))
                {
                    XmlSerializer x = new XmlSerializer(typeof(List<T>));
                    x.Serialize(writer, instance);
                    return sb.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// This method creates an instance of the given type based on the xml representation
        /// </summary>
        /// <typeparam name="T">Represents the type of the instance that should be created</typeparam>
        /// <param name="xml">Xml representation of the instance</param>
        /// <returns>Instance of the given type</returns>
        /// <remarks>The type should be serializable</remarks>
        public static T Deserialize<T>(string xml, bool isAnything = false)
        {
            if (isAnything)
            {
                return DeSerializeAnything<T>(xml);
            }
            else
            {
                if (xml != null && !string.IsNullOrWhiteSpace(xml))
                {
                    using (TextReader tReader = new StringReader(xml))
                    {
                        using (XmlReader xReader = new XmlTextReader(tReader))
                        {
                            XmlSerializer x = new XmlSerializer(typeof(T));

                            if ((x.CanDeserialize(xReader)))
                            {
                                return (T)x.Deserialize(xReader);
                            }
                        }
                    }
                }
                return default(T);
            }
        }



        /// <summary>
        /// This method creates an instance of the given type based on the xml representation
        /// </summary>
        /// <typeparam name="T">Represents the type of the instance that should be created</typeparam>
        /// <param name="xml">Xml representation of the instance</param>
        /// <returns>Instance of the given type</returns>
        /// <remarks>The type should be serializable</remarks>
        public static List<T> DeserializeCollection<T>(string xml)
        {
            using (TextReader tReader = new StringReader(xml))
            {
                using (XmlReader xReader = new XmlTextReader(tReader))
                {
                    XmlSerializer x = new XmlSerializer(typeof(List<T>));
                    if ((x.CanDeserialize(xReader)))
                    {
                        return (List<T>)x.Deserialize(xReader);
                    }
                }
            }
            return default(List<T>);
        }

        public static DateTime? GetDateFromString(string dateString, string formatString)
        {
            if (string.IsNullOrWhiteSpace(dateString)) return null;
            return DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm tt", null);
        }

        public static string GetDateFromString(DateTime? datetime, string formatString)
        {
            if (!datetime.HasValue) return null;
            return datetime.Value.ToString(formatString);
        }

        public static DateTime GetDateFromString(string dateString)
        {

            if (string.IsNullOrEmpty(dateString))
                return DateTime.MinValue;

            int tmp_Year;
            int tmp_Month;
            int tmp_Day;

            int.TryParse(dateString.Substring(0, 4), out tmp_Year);
            int.TryParse(dateString.Substring(4, 2), out tmp_Month);
            int.TryParse(dateString.Substring(6, 2), out tmp_Day);

            return new DateTime(tmp_Year, tmp_Month, tmp_Day);
        }

        public static DateTime GetDateFromSAPDateString(string dateString)
        {
            return dateString != null ? GetDateFromString(dateString.Replace(".", "")) : DateTime.MinValue;
        }

        public static string GetSAPDateString(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        public static string GetEnumAttribute<T>(Enum e)
        {
            try
            {
                Type t = e.GetType();
                MemberInfo[] memInfo = t.GetMember(e.ToString());

                if ((memInfo != null) && memInfo.Length > 0)
                {
                    object[] attribs = memInfo[0].GetCustomAttributes(typeof(T), false);
                    if ((attribs != null) && attribs.Length > 0)
                    {
                        return ((IAttribute)attribs[0]).Value;
                    }
                    else
                    {
                        return e.ToString();
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetEnumAttribute<T>(object e)
        {
            try
            {
                Type t = e.GetType();
                MemberInfo[] memInfo = t.GetMember(e.ToString());

                if ((memInfo != null) && memInfo.Length > 0)
                {
                    object[] attribs = memInfo[0].GetCustomAttributes(typeof(T), false);
                    if ((attribs != null) && attribs.Length > 0)
                    {
                        return ((IAttribute)attribs[0]).Value;
                    }
                    else
                    {
                        return e.ToString();
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetCustomEnumAttributeValueIfExists<T>(Enum e)
        {

            Type t = e.GetType();
            MemberInfo[] memInfo = t.GetMember(e.ToString());

            if ((memInfo != null) && memInfo.Length > 0)
            {
                object[] attribs = memInfo[0].GetCustomAttributes(typeof(T), false);
                if ((attribs != null) && attribs.Length > 0)
                {
                    return ((IAttribute)attribs[0]).Value;
                }
                else
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        public static string GetStringFromByteArray(byte[] byteArray)
        {
            return Encoding.UTF8.GetString(byteArray);
        }

        public static byte[] GetByteArrayFromString(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public static XElement SerializeAnything(object instance)
        {
            Type type = instance.GetType();

            Type[] extraTypes = type.GetProperties()
                .Where(leProperties => leProperties.PropertyType.IsInterface)
                .Select(leValue => leValue.GetValue(instance, null).GetType())
                .ToArray();

            DataContractSerializer serializer = new DataContractSerializer(type, extraTypes);
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            serializer.WriteObject(xw, instance);
            return XElement.Parse(sw.ToString());

        }

        public static T DeSerializeAnything<T>(string xml)
        {

            using (TextReader tReader = new StringReader(xml))
            {
                using (XmlReader xReader = new XmlTextReader(tReader))
                {
                    DataContractSerializer x = new DataContractSerializer(typeof(T));

                    return (T)x.ReadObject(xReader);

                }
            }

        }
        //public static Dictionary<long, string> GetAll<TEnum>() where TEnum : struct
        //{
        //    var enumerationType = typeof(TEnum);

        //    if (!enumerationType.IsEnum)
        //        throw new ArgumentException("Enumeration type is expected.");

        //    var dictionary = new Dictionary<long, string>();

        //    foreach (int value in Enum.GetValues(enumerationType))
        //    {
        //        var name = Enum.GetName(enumerationType, value);
        //        dictionary.Add(Convert.ToInt64(value), name);
        //    }

        //    return dictionary;
        //}

        public static T GetEnumValue<T>(int intValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }
            T val = ((T[])Enum.GetValues(typeof(T)))[0];

            foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
            {
                if (Convert.ToInt32(enumValue).Equals(intValue))
                {
                    val = enumValue;
                    break;
                }
            }
            return val;
        }

        public static T GetEnumValue<T>(string str) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }
            T val = ((T[])Enum.GetValues(typeof(T)))[0];
            if (!string.IsNullOrEmpty(str))
            {
                foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
                {
                    if (enumValue.ToString().ToUpper().Equals(str.ToUpper()))
                    {
                        val = enumValue;
                        break;
                    }
                }
            }

            return val;
        }


        public static Dictionary<long, string> GetAll<TEnum>() where TEnum : struct
        {
            var enumerationType = typeof(TEnum);

            if (!enumerationType.IsEnum)
                throw new ArgumentException("Enumeration type is expected.");

            var dictionary = new Dictionary<long, string>();

            foreach (var value in Enum.GetValues(enumerationType))
            {
                var name = Enum.GetName(enumerationType, value);
                dictionary.Add(Convert.ToInt64(value), name);
            }

            return dictionary;
        }

        public static bool CheckDate(string date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> GetListFromString(string data)
        {
            List<string> grpList = new List<string>();
            if (!string.IsNullOrWhiteSpace(data))
            {
                var groups = data.Split(',');

                foreach (string item in groups)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        grpList.Add(item);
                    }
                }
            }

            return grpList.Distinct().ToList();
        }

        public static string GetStringFromList(IList<string> grpList)
        {
            return string.Join(",", grpList);
        }
        public static int GetMeExtension(List<int> exclude, int start, int end)
        {
            var range = Enumerable.Range(start, end).Where(i => !exclude.Contains(i));
            var rand = new System.Random();
            int index = rand.Next(start, end - exclude.Count);
            return range.ElementAt(index);
        }

        public static string PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            List<Tuple<string, string, string>> modified = new List<Tuple<string, string, string>>();
            if (self != null && to != null)
            {
                Type type = typeof(T);
                List<string> ignoreList = new List<string>(ignore);
                foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                        object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                        if (toValue !=null && selfValue !=null && selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                        {
                            modified.Add(new Tuple<string, string, string>(pi.Name, selfValue.ToString(), toValue.ToString()));
                        }
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in modified)
            {
                sb.Append($"{item.Item1}: [Old Value: {item.Item2} - New Value: {item.Item3}],");
            }
            return sb.ToString().TrimEnd(',');
        }
        public static string PublicInstancePropertiesEqualWithAdditionalValue<T>(T self, T to, string oldUserName, string newUserName, params string[] ignore) where T : class
        {
            List<Tuple<string, string, string>> modified = new List<Tuple<string, string, string>>();
            if (self != null && to != null)
            {
                Type type = typeof(T);
                List<string> ignoreList = new List<string>(ignore);
                foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                        object toValue = type.GetProperty(pi.Name).GetValue(to, null);
                        
                        if (toValue != null && selfValue != null && selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                        {
                            if (pi.Name == "OwnerUserId")
                            {
                                modified.Add(new Tuple<string, string, string>(pi.Name, oldUserName, newUserName));
                            }
                            else
                            {
                                modified.Add(new Tuple<string, string, string>(pi.Name, selfValue.ToString(), toValue.ToString()));
                            }
                        }
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in modified)
            {
                sb.Append($"{item.Item1}: [Old Value: {item.Item2} - New Value: {item.Item3}],");
            }
            return sb.ToString().TrimEnd(',');
        }
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static String ObjectToXMLGeneric<T>(T obj)
        {

            string xml = null;
            using (StringWriter sw = new StringWriter())
            {

                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(sw, obj);
                try
                {
                    xml = sw.ToString();

                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return xml;
        }
    }
}
