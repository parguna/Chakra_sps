using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum AttestationCampaignStatus : byte
    {
        
        [Description("WorkInProgress")]
        WorkInProgress = 0,

        [Description("Remediation")]
        Remediation = 1,

        [Description("Completed")]
        Completed = 2,

        [Description("IgnoreNoUserFound")]
        IgnoreNoUserFound = 3,

        [Description("Initiated")]
        Initiated = 4

    }
}
