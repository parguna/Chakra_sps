using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public interface IEndpointEntitlementInfo
    {

        string EntitlementName { get; }
        string EntitlementDesc { get; }
        EntitlementType EntitlementType { get; }
    }
}
