using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum EmailActivityType
    {
        [Description("None")]
        None,
        [Description("Discovery")]
        Discovery,
        [Description("Attestation")]
        Attestation,
        [Description("Provisioning")]
        Provisioning,
        [Description("Workflow")]
        Workflow,
        [Description("ForgotPasswordOTP")]
        OTP,
        [Description("InhouseAccountCreated")]
        Inhouse,
    }
}
