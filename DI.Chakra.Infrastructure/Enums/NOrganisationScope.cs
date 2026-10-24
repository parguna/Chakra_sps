using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum NOrganisationScopeValue : byte
    {
        [Description("None")]
        None = 0,

        [Description("Company")]
        Company = 1,

        [Description("BusinessUnit")]
        BusinessUnit = 2,

        [Description("Department")]
        Department = 3,

        [Description("Location")]
        Location = 4,
    }
}
