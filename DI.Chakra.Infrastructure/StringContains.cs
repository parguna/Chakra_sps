using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public static class StringExtension
    {
        public static string ToSecureString(this string param)
        {
            /**** commented out
            //convert to secure string
            SecureString _output = new SecureString();
            param.ToCharArray().ToList().ForEach((q) => _output.AppendChar(q));

            string str = SecureStringToString(_output);
            //change to tuple
            Tuple<string> _cliente = Tuple.Create(str);
            return _cliente.Item1;
            **/

            Tuple<string> _cliente = Tuple.Create(param);
            return _cliente.Item1;
        }
        public static string ToSecureLDAPFilter(this string param)
        {
            return EscapeForSearchFilter(param);
        }
        public static string EscapeForSearchFilter(string filter)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < filter.Length; i++)
            {
                char curChar = filter[i];
                switch (curChar)
                {
                    case '\\':
                        sb.Append(@"\5c");
                        break;
                    case '*':
                        sb.Append(@"\2a");
                        break;
                    case '(':
                        sb.Append(@"\28");
                        break;
                    case ')':
                        sb.Append(@"\29");
                        break;
                    case '\u0000':
                        sb.Append(@"\00");
                        break;
                    default:
                        sb.Append(curChar);
                        break;
                }
            }
            return sb.ToString();
        }
    }
    public static class StringContains
    {
        public static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> enumerable,
                                                         int chunkSize)
        {
            int itemsReturned = 0;
            var list = enumerable.ToList(); // Prevent multiple execution of IEnumerable.
            int count = list.Count;
            while (itemsReturned < count)
            {
                int currentChunkSize = Math.Min(chunkSize, count - itemsReturned);
                yield return list.GetRange(itemsReturned, currentChunkSize);
                itemsReturned += currentChunkSize;
            }
        }
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
        public static SecureString SecureStringPassword(this string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return null;
            var securePassword = new SecureString();
            foreach (char c in source)
            {
                securePassword.AppendChar(c);
            }
            return securePassword;
        }
        public static string GetLastDigit(this string source, int last)
        {
            if (!string.IsNullOrWhiteSpace(source) && source.Length > 0 && source.Length >= last)
            {
                return last >= source.Length ? source : source.Substring(source.Length - last);
            }
            return "0";
        }
        public static string GetValidFileName(this string name)
        {
            Regex iligalFileName = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))), RegexOptions.Compiled);
            return iligalFileName.Replace(name ?? "", "");
        }
        public static string GetSubString(this string source, int size)
        {
            if (!string.IsNullOrWhiteSpace(source) && source.Length > 0 && source.Length >= size)
            {
                return source.Substring(0, size);
            }
            return "0";
        }
        public static bool IsNumeric(this string theValue)
        {
            long retNum;
            if (string.IsNullOrWhiteSpace(theValue)) return false;
            return long.TryParse(theValue, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }
        public static string[] SplitBySeparator(this string @this, string separator, StringSplitOptions option = StringSplitOptions.None)
        {
            return @this.Split(new[] { separator }, option);
        }
    }

    public static class BooleanToString
    {
        public static string GetBoolToString(this bool boolValue)
        {
            return boolValue ? "Yes" : "No";
        }
        public static string GetNullableBoolToString(this Nullable<bool> boolValue)
        {
            if (boolValue.HasValue)
                return boolValue.Value ? "Yes" : "No";
            return "";
        }
    }

    public static class DateTimeExtension
    {
        public static string MailDateFormat(this DateTime value)
        {
            return value.ToString("dd/MM/yyyy");
        }

        public static string MailDateTimeFormat(this DateTime value)
        {
            return value.ToString("dd/MM/yyyy hh:mm tt");
        }

        
    }
    
}
