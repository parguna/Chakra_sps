using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Model
{
    [Serializable]
    public class AdHocSAPRequest : AdhocSingleEntitlementRequest, IEntitlementDetail
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

        public AdHocSAPSharedAccount AdHocSAPSharedAccount { get; set; }

        public string SharedAccountName
        {
            get
            {
                return AdHocSAPSharedAccount.AccountName;
            }
        }
    }

    public class AdHocSAPSharedAccount : AdHocAccountInfo
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

        public SAPUserType SAPUserType { get; set; }
    }

}
