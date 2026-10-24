using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Dto
{
    public class EntitlementWrapper
    {
        public InhouseUserInfo InhouseUser { get; set; }

        public EntitlementType Entitlement { get; set; }
    }
}