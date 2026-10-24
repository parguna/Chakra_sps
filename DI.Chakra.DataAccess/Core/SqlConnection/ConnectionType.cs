using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.DataAccess
{
    public enum ConnectionType
    {
        Local =0,

        DIServer,

        Development,        

        Integration,

        UAT,         

        Production

    }
}
