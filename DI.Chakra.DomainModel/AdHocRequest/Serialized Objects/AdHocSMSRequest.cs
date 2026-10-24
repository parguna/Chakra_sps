using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class AdHocSMSRequest : IEntitlementDetail
    {
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public List<AdhocSmsGridInfo> GridInfo { get; set; }
        [Obsolete]
        public ISharedAccountDetail SharedAccountDetail { get; }
    }
    [Serializable]
    public class AdhocSmsGridInfo
    {
        public string TechnicalRole { get; set; }
        public string Description { get; set; }
        public string Building { set; get; }
        public string Section { get; set; }
    }

}
