using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Model
{
    [Serializable]
    public class AdHocOracleEBSRequest : AdhocSingleEntitlementRequest, IEntitlementDetail
    {
        public AdhocSingleEntitlementRequest SingleEntitlementRequestObject
        {
            set
            {
                this.EndpointUserId = value.EndpointUserId;
                this.Entitlement = value.Entitlement;
                this.EntitlementType = value.EntitlementType;
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
