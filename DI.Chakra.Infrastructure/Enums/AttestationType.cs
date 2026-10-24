using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum AttestationType
    {
        //TODO: need to be removed, when we fix nullable enum
        [Description("User")]
        User = 0,
    }
}
