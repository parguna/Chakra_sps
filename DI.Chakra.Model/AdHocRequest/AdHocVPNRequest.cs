using System;
using System.Collections.Generic;

namespace DI.Chakra.Model
{
    [Serializable]
    public class AdHocVPNRequest : IEntitlementDetail
    {
        public AdHocVPNRequest()
        {
            this.AdHocVPNRequestDetails = new List<AdHocVPNRequestDetail>();
        }

        public int EndpointUserId { get; set; }
        public string OneKey { get; set; }
        public string Entitlement { get; set; }
        public string SharedAccountName
        {
            get
            {
                return "";
            }
        }
        public List<AdHocVPNRequestDetail> AdHocVPNRequestDetails { get; set; }
    }

    [Serializable]
    public class AdHocVPNRequestDetail
    {
        public int? identity { get; set; }
        public string ApplicationName { get; set; }
        public string Remarks { get; set; }
        public bool Production { get; set; }
        public bool QA { get; set; }
        public bool Dev { get; set; }
        public bool isOthers { get; set; }
    }
}
