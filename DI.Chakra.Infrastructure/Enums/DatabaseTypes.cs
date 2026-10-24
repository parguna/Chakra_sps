using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum DatabaseTypes: int
    {
        [Description("Microsoft SQL Server")]
        MSSql= 1,

        [Description("My SQL")]
        MySql = 2,

        [Description("Oracle")]
        Oracle = 3

    }
}
