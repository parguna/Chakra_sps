using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum LogReferenceType : int
    {
        Discovery = 1,
        Attestation,
        Email,
        Compliance,
        Provisioning,
        HouseKeeping,
        Scheduler,
        General,
        Workflow,
        Reporting,
        PasswordReset,
        PasswordChange,
        AccountUnlock,
        Authentication,
        PUBProcess
    }
}
