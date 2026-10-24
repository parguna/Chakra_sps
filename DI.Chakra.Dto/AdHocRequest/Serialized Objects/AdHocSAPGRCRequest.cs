using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AdHocSAPGRCRequest : IEntitlementDetail
    {
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public AdHocSAPSharedAccount AdHocSAPGRCSharedAccount { get; set; }
        public ISharedAccountDetail SharedAccountDetail { get { return AdHocSAPGRCSharedAccount; } }
    }
}
