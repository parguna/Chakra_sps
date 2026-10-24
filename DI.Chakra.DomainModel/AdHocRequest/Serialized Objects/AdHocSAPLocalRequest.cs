using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class AdHocSAPLocalRequest : IEntitlementDetail
    {
        public string AccountName { get; set; }
        public long EndpointUserId { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public string RequesterComment { get; set; }

        //only for create
        public string DisplayAccountName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string OU { get; set; }

        public ISharedAccountDetail SharedAccountDetail
        {
            get
            {
                return new AdHocADSharedAccount() { AccountName = this.AccountName, Description = this.Description, DisplayName = this.DisplayName, OU = this.OU };
            }
        }
    }

}
