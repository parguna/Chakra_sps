using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public enum ConnectionType
    {
        Local = 0,

        DIServer,

        Development,

        Integration,

        UAT,

        Production,

        QA,

        QA_India

    }
}
