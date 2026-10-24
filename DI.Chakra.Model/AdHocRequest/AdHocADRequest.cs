
namespace DI.Chakra.Model
{


    public class AdHocADRequest : AdhocSingleEntitlementRequest, IEntitlementDetail
    {
        public AdhocSingleEntitlementRequest SingleEntitlementRequestObject
        {
            set
            {
                this.EndpointUserId = value.EndpointUserId;
                this.AccessItemType = value.AccessItemType;
                this.Entitlement = value.Entitlement;
                this.EntitlementDetail = value.EntitlementDetail;
                this.EntitlementDescription = value.EntitlementDescription;
                this.EntitlementType = value.EntitlementType;
            }
        }

        public AdHocADSharedAccount AdHocADSharedAccount { get; set; }
        public string SharedAccountName
        {
            get
            {
                return AdHocADSharedAccount.AccountName;
            }
        }
    }

    public class AdHocADSharedAccount : AdHocAccountInfo
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

        public string OU { get; set; }
    }


}
