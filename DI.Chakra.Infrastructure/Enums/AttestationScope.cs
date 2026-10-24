using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.Infrastructure
{
    public enum AttestationScopeEnum 
    {
        None = 0,
        [VariantType("NoNeededForSplit")]
        User,
        [VariantType("User")]
        RO,
        [VariantType("NoNeededForSplit")]
        IACMGroup,
        [VariantType("User")]
        Company,
        [VariantType("User")]
        Department,
        [VariantType("User")]
        Branch,
        [VariantType("User")]
        Section,
        [VariantType("User")]
        Unit
    }
}
