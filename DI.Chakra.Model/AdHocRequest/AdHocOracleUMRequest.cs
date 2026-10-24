using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Model
{
    [Serializable]
    public class AdHocOracleUMRequest : AdhocSingleEntitlementRequest, IEntitlementDetail
    {
        public AdhocSingleEntitlementRequest SingleEntitlementRequestObject
        {
            set
            {
                this.EndpointUserId = value.EndpointUserId;
                this.Entitlement = value.Entitlement;
                this.EntitlementType = Infrastructure.Enums.EntitlementType.Role;
            }
        }
        public string SharedAccountName
        {
            get
            {
                return "";
            }
        }
    }
}
