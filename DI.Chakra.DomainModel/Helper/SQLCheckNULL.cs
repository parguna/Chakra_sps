using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SQLCheckNULL
    {

        public static object CheckAgainstNull(object value)
        {
            if (value != null)
                return value;
            else
                return DBNull.Value;
        }

        public static object CheckStringAgainstNull(string value)
        {
            if (value != null)
                return value;
            else
                return string.Empty;
        }
    }
}
