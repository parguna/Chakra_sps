using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AdHocADRequest : IEntitlementDetail 
    {       
     
        public long EndpointUserId { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public string RequesterComment { get; set; }
        public string DisplayAccountName { get; set; }
        public AdHocADSharedAccount AdHocADSharedAccount { get; set; }

        public ISharedAccountDetail SharedAccountDetail
        {
            get
            {
                return AdHocADSharedAccount;
            }
        }
    }

}
