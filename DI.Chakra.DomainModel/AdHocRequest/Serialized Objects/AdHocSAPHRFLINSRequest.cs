using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class AdHocSAPHRFLINSRequest : IEntitlementDetail
    {
        public AdHocSAPHRFLINSRequest()
        {
            this.AdHocSAPHRFLINSRequestDetails = new List<AdHocSAPHRFLINSRequestDetail>();
        }
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public List<AdHocSAPHRFLINSRequestDetail> AdHocSAPHRFLINSRequestDetails { get; set; }
        public AdHocSAPSharedAccount AdHocSAPHRFLINSSharedAccount { get; set; }

        public string Entitlement
        {

            get
            {
                string strItems = "";
                if (AdHocSAPHRFLINSRequestDetails != null)
                {
                    foreach (var item in AdHocSAPHRFLINSRequestDetails)
                    {
                        if (string.IsNullOrEmpty(item.Company))
                        {
                            strItems += (strItems == "") ? item.Entitlement : "," + item.Entitlement;
                        }
                        else
                        {
                            strItems += (strItems == "") ? string.Format("{0} ({1})", item.Entitlement, item.Company) : "," + string.Format("{0} ({1})", item.Entitlement, item.Company);
                        }
                    }
                }
                return strItems;
            }

        }
        public ISharedAccountDetail SharedAccountDetail
        {
            get
            {
                return AdHocSAPHRFLINSSharedAccount;
            }
        }


    }


    [Serializable]
    public class AdHocSAPHRFLINSRequestDetail
    {
        public string Module { get; set; }
        public string SubModule { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public string Company { get; set; }
        public bool RetainCompanyRoles { get; set; }
        public string CostCenter { get; set; }
        public string PurchaseOrg { get; set; }
        public string PurchaseGroup { get; set; }
        public string Plant { get; set; }
        public string StorageLocation { get; set; }
        public string MRPController { get; set; }
    }

   
}
