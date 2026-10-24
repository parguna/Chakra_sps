using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum AdHocActionType : byte
    {
        [DisplayAs("None")]
        [Description("None")]
        None = 0,
        [DisplayAs("New Account")]
        [Description("New Account")]
        NewAccount = 1,
        [DisplayAs("Amendment-Add")]
        [Description("Amendment Add")]
        AmendmentAdd = 2,
        [DisplayAs("Amendment-Remove")]
        [Description("Amendment Remove")]
        AmendmentRemove = 3,
        [DisplayAs("Delete Account")]
        [Description("Delete Account")]
        DeleteAccount = 4,
        [DisplayAs("Renewal Access")]
        [Description("Renewal Access")]
        RenewalAccess = 5,
        [DisplayAs("Standing Order")]
        [Description("Standing Order")]
        StandingOrder = 6,
        [DisplayAs("Extended Request")]
        [Description("Extended Request")]
        ExtendedRequest = 7,
        
    }
}
