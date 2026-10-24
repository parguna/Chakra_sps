using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Dto 
{
    [Serializable]
    public class AdHocIACMRequest : IEntitlementDetail
    {
        public long EndpointUserId { get; set; }
        public string DisplayAccountName { get; set; }
        public string Entitlement
        {
            get
            {
                string strItems = "";
                if (AdHocIACMRequestDetails != null)
                {
                    foreach (var detail in AdHocIACMRequestDetails)
                    {
                        var entitlement = $"EntitlementType : { UtilityMethods.GetEnumAttribute<Description>(detail.EntitlementType)}; Entitlement : {detail.Entitlement}";
                        strItems = strItems == "" ? entitlement + " ," : entitlement;
                    }
                }
                return strItems;
            }
        }

        public List<AdHocIACMRequestDetail> AdHocIACMRequestDetails { get; set; }

        [Obsolete]
        public ISharedAccountDetail SharedAccountDetail
        {
            get;
        }
    }

    [Serializable]
    public class AdHocIACMRequestDetail
    {
        public string Entitlement { get; set; }
        public EntitlementType EntitlementType { get ;set;}
    }
}
