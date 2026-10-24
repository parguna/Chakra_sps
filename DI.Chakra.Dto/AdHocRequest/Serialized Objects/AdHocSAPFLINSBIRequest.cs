using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AdHocSAPFLINSBIRequest : IEntitlementDetail 
    {
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public AdHocSAPSharedAccount AdHocFLINSBISharedAccount { get; set; } 
        public ISharedAccountDetail SharedAccountDetail { get { return AdHocFLINSBISharedAccount; } }
    }
}

