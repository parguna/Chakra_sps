using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AdhocSAPEPRequest : IEntitlementDetail
    {
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public string RequestDetail { get; set; }
        public string Groups { get; set; }
        public string Roles { get; set; }
        public AdHocSAPEPSharedAccount AdHocSAPEPSharedAccount { get; set; }
        public string Entitlement
        {
            get
            {
                string entitlement = string.Empty;
                if (!string.IsNullOrWhiteSpace(this.Groups))
                    entitlement = $"Groups: {Groups} ";
                if (!string.IsNullOrWhiteSpace(this.Roles))
                    entitlement = string.IsNullOrEmpty(entitlement) ? $"Roles: {Roles} " : entitlement+ $" / Roles: {Roles} ";
                return entitlement;
              }
        }
        public ISharedAccountDetail SharedAccountDetail
        {
            get
            {
                return AdHocSAPEPSharedAccount;
            }
        }
    }

}
