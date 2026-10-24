using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public class ExpandoHelper
    {
        public static dynamic AddProperty(ExpandoObject obj, string propName, object propValue)
        {
            var expandoDict = obj as IDictionary<string, object>;

            if (expandoDict.ContainsKey(propName))
                expandoDict[propName] = propValue;

            else
                expandoDict.Add(propName, propValue);

            return obj;
        }
    }
}
