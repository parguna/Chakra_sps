using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel 
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
                return string.Join(",", AdHocIACMRequestDetails.Select(d=>d.Entitlement).ToList() );
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
        public EntitlementType EntitlementType { get; set; }
    }
}
