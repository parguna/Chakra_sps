using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public static class EnumExt
    {
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);           
            if (!type.IsEnum) return default(T);
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(Description)) as Description;
                if (attribute != null)
                {
                    if (attribute.Value == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            return default(T);
        }

        public static IDictionary<int, string> GetAll<TEnum>() where TEnum : struct
        {
            var enumerationType = typeof(TEnum);

            if (!enumerationType.IsEnum)
                throw new ArgumentException("Enumeration type is expected.");

            var dictionary = new Dictionary<int, string>();

            foreach (byte value in Enum.GetValues(enumerationType))
            {
                var name = Enum.GetName(enumerationType, value);
                dictionary.Add((int)value, name);
            }

            return dictionary;
        }

        public static string GetDescription(this Enum e)
        {
            try
            {
                Type t = e.GetType();
                MemberInfo[] memInfo = t.GetMember(e.ToString());

                if ((memInfo != null) && memInfo.Length > 0)
                {
                    object[] attribs = memInfo[0].GetCustomAttributes(typeof(Description), false);
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
        public static string GetDisplayAs(this Enum e)
        {
            try
            {
                Type t = e.GetType();
                MemberInfo[] memInfo = t.GetMember(e.ToString());

                if ((memInfo != null) && memInfo.Length > 0)
                {
                    object[] attribs = memInfo[0].GetCustomAttributes(typeof(DisplayAs), false);
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
    }
}
