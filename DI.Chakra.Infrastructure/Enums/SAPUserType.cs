using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum SAPUserType : byte
    {
        [Description("Dialog")]
        Dialog,

        [Description("System")]
        System,

        [Description("Communication")]
        Communication,

        [Description("Service")]
        Service,

        [Description("Reference")]
        Reference,
    }
}
