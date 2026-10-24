using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class AdHocINHRequest : IEntitlementDetail 
    {
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public string ApplicationCode { get; set; }
        public string Entitlement { get; set; } = string.Empty;
        public EntitlementType EntitlementType { get; set; }
        public AdHocINHSharedAccount AdHocINHSharedAccount { get; set; }
        public bool IsOffBoardRequest { get; set; }
        public ISharedAccountDetail SharedAccountDetail
        {
            get
            {
                return AdHocINHSharedAccount;
            }
        }
    }

}
