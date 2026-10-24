using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public interface IEndpointUserInfo
    {
        long Id { get; set; }

        long EndPointSystemId { get; set; }

        string DisplayName { get; }

        string AccountName { get; }

        string AccountPassword { get; }

        //Nullable<long> WogUserId { get; }

        List<IEndpointEntitlementInfo> Entitlements { get; }
    }
}
