using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    public class MyRequestsGridData
    {
        public long AdHocAccessRequestId { get; set; }
        public long AccessRequestID { get; set; }
        public string Name { get; set; }
        public string RequestType { get; set; }
        public System.DateTime RequestedDate { get; set; }
        public System.DateTime? ExecutionDate { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public string RequesterName { get; set; }
        public string RequesteeName { get; set; }
        public AdHocActionType AdhocActionType { get; set; }
        public long? EndpointSystemId { get; set; }
        public EndPoint EndPoint { get; set; }
        public AdHocAccessRequest AdHocAccessRequests { get; set; }
        public AccountType AccountType { get; set; }
        public System.DateTime? ValidFrom { get; set; }
        public System.DateTime? ValidTo { get; set; }
        public long UserID { get; set; }

        public string Entitlement
        {
            get
            {
                string Entitle = DeserializeEntitlement(this.AdHocAccessRequestId);
                if (!string.IsNullOrEmpty(Entitle))
                    return Entitle.Replace(",",", ").Replace("  ", " ").Trim();
                else
                    return "";
            }
        }

        public string ExecuteDateString()
        {
            return (this.ExecutionDate.HasValue)?ExecutionDate.Value.ToString("dd-MMM-yyyy"):"";        
        }

        public string DeserializeEntitlement(long? AdhocRequestId)
        {
            try
            {
                if (AdhocRequestId.HasValue)
                {
                    AdHocAccessRequest adhoxReq = new IACMDBContext().AdHocAccessRequests.Find(AdhocRequestId);

                    switch (adhoxReq.EndpointSystemType)
                    {
                        case EndpointType.ActiveDirectory :
                            return UtilityMethods.Deserialize<AdHocADRequest>(adhoxReq.EntitlementDetail).Entitlement;

                        case EndpointType.LotusNotes:
                            return UtilityMethods.Deserialize<AdHocLNSRequest>(adhoxReq.EntitlementDetail).Entitlement;

                        case EndpointType.SAP:
                            return UtilityMethods.Deserialize<AdHocSAPRequest>(adhoxReq.EntitlementDetail).Entitlement;

                        case EndpointType.VPN:
                            var adhocReqVPN = UtilityMethods.Deserialize<AdHocVPNRequest>(adhoxReq.EntitlementDetail);
                            return JoinedEntitlementAdHocVPNRequestDetail(adhocReqVPN.AdHocVPNRequestDetails);

                        case EndpointType.InhouseSystems:
                            return UtilityMethods.Deserialize<AdHocInhouseSystemRequest>(adhoxReq.EntitlementDetail).Entitlement;
                        
                        default:
                            return string.Empty;
                    }

                }
                return "";

            }
            catch { return ""; }

        }

        private string JoinedEntitlementAdHocVPNRequestDetail(List<AdHocVPNRequestDetail> AdhocReqDetails)
        {
            string strItems = "";
            foreach (var items in AdhocReqDetails)
            {
                strItems += (strItems == "") ? items.ApplicationName : "," + items.ApplicationName;
            }
            return strItems;
        }


        public DateTime? EndDate { get; set; }

        public DateTime? StartDate { get; set; }
    }
}
