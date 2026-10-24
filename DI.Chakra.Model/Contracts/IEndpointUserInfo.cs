using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;

namespace DI.Chakra.Model
{
    public interface IEndpointUserInfo
    {
        long Id { get; set; }
        long? DIUserId { get; set; }
        long EndPointSystemId { get; set; }
        int Version { get; set; }
        bool IsLocked { get; set; }
        LockScenario LockScenario { get; set; }
        AccountType AccountType { get; set; }
        string DisplayName { get; }
        string AccountName { get; }
        string AccountPassword { get; }

        List<IEndpointEntitlementInfo> Entitlements { get; }
    }
}
