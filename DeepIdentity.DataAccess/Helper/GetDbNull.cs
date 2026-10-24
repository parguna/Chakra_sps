using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepIdentity.DataAccess
{
    public class GetDbNull
    {
        public static object IfObjectIsNull(object obj)
        {
            if (null == obj)
                return DBNull.Value;

            if (obj.GetType().Name == "DateTime")
            {
                if (DateTime.MinValue == ((DateTime)obj))
                    return DBNull.Value;
            }

            if (obj.ToString() == string.Empty)
                return DBNull.Value;


            return obj;
        }



        
    }
}
