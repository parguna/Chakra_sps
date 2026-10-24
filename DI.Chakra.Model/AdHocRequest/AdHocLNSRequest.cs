
namespace DI.Chakra.Model
{

    public class AdHocLNSRequest : AdhocSingleEntitlementRequest, IEntitlementDetail
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

        public bool ExternalEmailRequest { get; set; }

        public AdHocLNSSharedAccount AdHocLNSSharedAccount { get; set; }

        public string SharedAccountName
        {
            get
            {
                return AdHocLNSSharedAccount.AccountName;
            }
        }
    }

    public class AdHocLNSSharedAccount : AdHocAccountInfo
    {
        public AdHocAccountInfo AdHocAccountInfoObject
        {
            set
            {
                this.AccountName = value.AccountName;
                this.Description = value.Description;
                this.DisplayName = value.DisplayName;
            }
        }

        public string EmailAddress { get; set; }
        public string Certifier { get; set; }
    }

}
