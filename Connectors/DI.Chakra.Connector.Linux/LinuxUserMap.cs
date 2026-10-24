using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
//using DI.Chakra.Facade;
using DI.Chakra.Infrastructure;
//using DI.Chakra.Infrastructure.;
using DI.Chakra.DomainModel;

using System.Text;


namespace DI.Chakra.Connector
{
    public class LinuxUserMap
    {
     

        public bool MapAll { get; set; }

      

        private Object GetPropValue(String name, Object obj)
        {

            Type type = obj.GetType();

            PropertyInfo info = type.GetProperty(name);

            if (info == null)
                return null;


            if (info.ToString().Equals(string.Empty))
                return null;

            obj = info.GetValue(obj, null);

            if (info.PropertyType == typeof(DateTime))
            {
                if (obj == null || obj.ToString() == string.Empty)
                    return DBNull.Value;
            }

            return obj;
        }


       
    }
}
