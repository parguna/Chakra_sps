using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum CustomReportParameterTypes : int
    {

        [Description("varchar")]
        varchar = 1,

        [Description("DateTime")]
        DateTime = 2,

        [Description("Long")]
        Long = 3,

        [Description("Bit")]
        Bit = 4,

        [Description("User")]
        UserName = 5,

        [Description("EmployeeId")]
        EmployeeId = 6,

        [Description("EndpointSystem")]
        EndpointSystem = 7 
    }

    public enum CustomReportParameterVariant : int
    {

        [Description("Column")]
        Column = 0,

        [Description("Value")]
        Value = 1,
    }
}
