using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.Infrastructure
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                         System.AttributeTargets.Struct)
  ]
    public class TableInfoAttribute : System.Attribute
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }

        public TableInfoAttribute(string _name,string _friendlyName)
        {
            this.Name = _name;
            this.FriendlyName = _friendlyName;
          
        }
    }


}
