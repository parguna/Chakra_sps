using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{
    public interface IEndpointEntitlementInfo
    {
        long EndPointSystemId { get; set; }
        string EntitlementName { get; }
        string EntitlementDesc { get; }
        EntitlementType EntitlementType { get; }
    }
}
