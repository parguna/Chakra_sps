using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum OrgScopeLevel : byte
    {
        [Description("None")]
        None = 0,

        //[Description("Group")]
        //Group = 1,

        [Description("Company")]
        Company = 1,

        [Description("Department")]
        Department = 2,

        [Description("Division")]
        Division = 3,

        [Description("Branch")]
        Branch = 4,

        [Description("Section")]
        Section = 5,

        [Description("Unit")]
        Unit = 6,
    }
}
