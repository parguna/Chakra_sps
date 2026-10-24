using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public class ServiceParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }


        public override string ToString()
        {
            if (Name != null && !string.IsNullOrEmpty(Value))
                return string.Format("{0}-{1}", Name, Value);

            return Value;
        }
    }
}
