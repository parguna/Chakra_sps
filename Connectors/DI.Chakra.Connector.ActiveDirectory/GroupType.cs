using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.Connector
{
    public enum GroupType : uint
    {
        UniversalGroup = 0x08,
        DomainLocalGroup = 0x04,
        GlobalGroup = 0x02,
        SecurityGroup = 0x80000000
    }


}
