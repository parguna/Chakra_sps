using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class CheckNull
    {
        public static int AgainstInt(object obj)
        {

            if (obj == DBNull.Value)
                return -1;

            if (null == obj)
                return -1;

            if (string.Empty == obj.ToString())
                return -1;

            return Convert.ToInt32(obj);
        }

        public static long AgainstLong(object obj)
        {

            if (obj == DBNull.Value)
                return -1;

            if (null == obj)
                return -1;

            if (string.Empty == obj.ToString())
                return -1;

            return Convert.ToInt64(obj);
        }

        public static short AgainstShort(object obj)
        {

            if (obj == DBNull.Value)
                return -1;

            if (null == obj)
                return -1;

            if (string.Empty == obj.ToString())
                return -1;

            return Convert.ToInt16(obj);
        }

        public static int? AgainstNullableInt(object obj)
        {

            if (obj == DBNull.Value)
                return null;

            if (null == obj)
                return null;

            if (string.Empty == obj.ToString())
                return null;


            return Convert.ToInt32(obj);
        }
        public static double AgainstDouble(object obj)
        {

            if (obj == DBNull.Value)
                return -1;

            if (null == obj)
                return -1;

            if (string.Empty == obj.ToString())
                return -1;

            return Convert.ToDouble(obj);
        }
        public static double? AgainstNullableDouble(object obj)
        {

            if (obj == DBNull.Value)
                return null;

            if (null == obj)
                return null;

            if (string.Empty == obj.ToString())
                return null;

            return Convert.ToDouble(obj);
        }
        public static string AgainstString(object obj)
        {
            if (obj == DBNull.Value)
                return string.Empty;
            if (null == obj)
                return string.Empty;

            return Convert.ToString(obj);
        }
        public static bool AgainstBit(object obj)
        {
            if (obj == DBNull.Value)
                return false;
            if (null == obj)
                return false;

            if (string.Empty == obj.ToString())
                return false;

            return Convert.ToBoolean(obj);
        }
        public static byte[] AgainstByte(object obj)
        {
            byte[] retval = null;
            if (obj == DBNull.Value)
                return retval;
            if (null == obj)
                return retval;

            if (string.Empty == obj.ToString())
                return retval;

            byte[] binaryString = (byte[])obj;

            return binaryString;
        }
        public static bool? AgainstNullableBit(object obj)
        {
            if (obj == DBNull.Value)
                return null;
            if (null == obj)
                return null;

            if (string.Empty == obj.ToString())
                return null;

            return Convert.ToBoolean(obj);
        }
        public static DateTime? AgainstNullableDatetime(object obj)
        {

            if (obj == DBNull.Value)
                return null;

            if (null == obj)
                return null;

            if (string.Empty == obj.ToString())
                return null;

            return Convert.ToDateTime(obj);
        }
        public static DateTime AgainstDatetime(object obj)
        {

            if (obj == DBNull.Value)
                return DateTime.MinValue;

            if (null == obj)
                return DateTime.MinValue;

            if (string.Empty == obj.ToString())
                return DateTime.MinValue;

            return Convert.ToDateTime(obj);
        }
        public static TimeSpan? AgainstNullableTimeSpan(object obj)
        {

            if (obj == DBNull.Value)
                return null;

            if (null == obj)
                return null;

            if (string.Empty == obj.ToString())
                return null;

            return TimeSpan.Parse(obj.ToString());
        }
        public static TimeSpan AgainstTimeSpan(object obj)
        {

            if (obj == DBNull.Value)
                return new TimeSpan();

            if (null == obj)
                return new TimeSpan();

            if (string.Empty == obj.ToString())
                return new TimeSpan();

            return TimeSpan.Parse(obj.ToString());
        }

    }
}
