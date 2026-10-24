using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{

    public class AdhocSingleEntitlementRequest
    {
        public long EndpointUserId { get; set; }
        public AccessItemType AccessItemType { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementDetail { get; set; }
        public string EntitlementDescription { get; set; }
        public string RequesterComment { get; set; }
    }

    public class AdHocAccountInfo
    {
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }

}
