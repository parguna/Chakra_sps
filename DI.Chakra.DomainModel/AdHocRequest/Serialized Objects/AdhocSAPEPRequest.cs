using System;

namespace DI.Chakra.DomainModel
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
                if (!string.IsNullOrEmpty(this.Groups))
                    entitlement = $"Groups: {Groups} ";
                if (!string.IsNullOrEmpty(this.Groups))
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
