using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class AdhocExchnageRequest : AdhocSingleEntitlementRequest, IEntitlementDetail
    {
        public AdhocSingleEntitlementRequest SingleEntitlementRequestObject
        {
            set
            {
                this.EndpointUserId = value.EndpointUserId;
                this.Entitlement = value.Entitlement;
                this.EntitlementType = Infrastructure.Enums.EntitlementType.Group;
            }
        }

        public bool ExternalEmailRequest { get; set; }

        public AdHocExchangeSharedAccount AdHocLNSSharedAccount { get; set; }

        public string SharedAccountName
        {
            get
            {
                return AdHocLNSSharedAccount != null ? AdHocLNSSharedAccount.AccountName : "";
            }
        }
    }

    public class AdHocExchangeSharedAccount : AdHocAccountInfo
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
