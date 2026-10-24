using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.DomainModel
{    
    [Serializable]
    public class AdHocNPACRequest : IEntitlementDetail 
    {
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public string Section { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Unit { get; set; }
        [Obsolete]
        public ISharedAccountDetail SharedAccountDetail { get; }

    }
}
