using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AdHocGEMSRequest : IEntitlementDetail 
    {
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }        
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public AdHocGEMSSharedAccount AdHocGEMSSharedAccount { get; set; }
        public ISharedAccountDetail SharedAccountDetail
        {
            get
            {
                return AdHocGEMSSharedAccount;
            }
        }
    }

}

