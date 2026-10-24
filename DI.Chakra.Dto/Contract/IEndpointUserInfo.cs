using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public interface IEndpointUserInfo
    {
        long Id { get; set; }

        long EndPointSystemId { get; set; }

        string DisplayName { get; }

        string AccountName { get; }

        string AccountPassword { get; }

        List<IEndpointEntitlementInfo> Entitlements { get; }
    }
}
