using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class AdHocVPNRequest : IEntitlementDetail
    {
        public AdHocVPNRequest()
        {
            this.AdHocVPNRequestDetails = new List<AdHocVPNRequestDetail>();
        }
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public bool UpdateOneKeyToken { get; set; }

        public string OneKey { get; set; }
        public bool IsOffboardOrTransferRequest { get; set; }
        public List<AdHocVPNRequestDetail> AdHocVPNRequestDetails { get; set; } = new List<AdHocVPNRequestDetail>();
        [Obsolete]
        public ISharedAccountDetail SharedAccountDetail
        {
            get;
        }
        public string Entitlement
        {
            get
            {
                string strItems = "";
                if (AdHocVPNRequestDetails != null)
                {
                    foreach (var items in AdHocVPNRequestDetails)
                    {
                        strItems += (strItems == "") ? items.ApplicationName : "," + items.ApplicationName;
                    }
                }
                return strItems;
            }
        }

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
        public long VPNMatrixId { get; set; }
        public string MatrixType { get; set; }
        public string ProjectADGroups { get; set; }
        public string ProductionADGroups { get; set; }
        public string QAADgroups { get; set; }
        public string DevelopmentADGroups { get; set; }
        public VPNType VPNType { get; set; }

    }
}
