using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.DomainModel
{    
    [Serializable]
    public class AdHocSSIDSRequest : IEntitlementDetail 
    {
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        [Obsolete]
        public ISharedAccountDetail SharedAccountDetail { get; }

    }
}
