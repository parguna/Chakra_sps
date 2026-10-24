using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class AdHocSAPSRMRequest : IEntitlementDetail
    {
        public AdHocSAPSRMRequest()
        {
            this.AdHocSAPSRMRequestDetails = new List<AdHocSAPSRMRequestDetail>();
        }
        public string DisplayAccountName { get; set; }
        public string AccountName { get; set; }

        public List<AdHocSAPSRMRequestDetail> AdHocSAPSRMRequestDetails { get; set; }
        public AdHocSAPSharedAccount AdHocSAPSRMSharedAccount { get; set; }

        public long EndpointUserId { get; set; }
        public string Entitlement
        {
            get
            {
                string strItems = "";
                if (AdHocSAPSRMRequestDetails != null)
                {
                    strItems = string.Join("|", AdHocSAPSRMRequestDetails.Select(i => $"{i.Entitlement} : {i.PurchaseOrg} - {i.PurchaseGroup}"));
                }
                return strItems;
            }
        }
        public ISharedAccountDetail SharedAccountDetail { get { return AdHocSAPSRMSharedAccount; } }
    }


    [Serializable]
    public class AdHocSAPSRMRequestDetail
    {
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }      
        public string PurchaseOrg { get; set; }
        public string PurchaseGroup { get; set; }
        public bool RetainMainRole { get; set; }
    }    

}
