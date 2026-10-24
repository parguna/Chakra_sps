using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum AttestationDefaultReviewStatus : byte
    {

        [Description("None")]
        None = 0,

        [Description("Retain")]
        Retain = 1,

        [Description("Revoke")]
        Revoke = 2,
        
    }
}
