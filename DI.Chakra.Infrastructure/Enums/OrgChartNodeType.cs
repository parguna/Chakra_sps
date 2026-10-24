using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum OrgChartNodeType :byte
    {
        [Description("OrgHeadNode")]
        OrgHeadNode = 1,

        [Description("NormalUserNode")]
        NormalUserNode,
    }
}
