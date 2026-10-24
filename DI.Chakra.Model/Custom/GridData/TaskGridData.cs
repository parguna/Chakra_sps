using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    [Serializable]
    public class TaskGridData: IAggregateRoot
    {
        public long TaskId { get; set; }
        public byte RequestTypeId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public Nullable<long> AttestationId { get; set; }
        public string AttestationName { get; set; }
        public string EventName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReponseTimeInDays { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string RequestedBy { get; set; }
        public Nullable<long> AdHocAccessRequestId { get; set; }
        public Nullable<long> AccessRequestID { get; set; } 
        public Nullable<long> AssignedTo { get; set; }
        public Nullable<long> AssignedBy { get; set; }
        public string RequestedFor { get; set; }
        public StatusType Action { get; set; }
        public AccountType AccountType { get; set; }
        public bool IsDelegated { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedByName { get; set; }
        public string DelegatedBy { get; set; }
        public string DelegatedComment { get; set; }
        public bool IsGroup { get; set; }
        public AccessRequestStatus TaskAccessRequest { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string Type { get; set; }
        public StatusType StatusType { get; set; }
        public string RequestReason { get; set; }
        public DateTime SubmitBefore
        {
            get
            {
                return this.CreatedDate.AddDays(this.ReponseTimeInDays);
            }
        }

        public string Status
        {
            get
            {

                if (this.IsActive && !this.IsCompleted)
                {
                    return "Pending";
                }
                else if (this.IsCompleted)
                {
                    return "Completed";
                }
                else
                {
                    return "Auto Completed";
                }
            }
        }

        public string AccRequestStatus
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<DisplayAs>(TaskAccessRequest);
            }
        }
        public string IsDelegate
        {
            get
            {
                if (IsDelegated)
                    return "Yes";
                else
                    return "No";
            }
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Entitlement
        {
            get
            {
                return DeserializeEntitlement(this.AdHocAccessRequestId);
            }
        }

        public int RequesterId { get; set; }

        public int RequesteeId { get; set; }

        public string RequesterName { get { return GetUserName(RequesterId); } }

        public string RequesteeName { get { return GetUserName(RequesteeId); } }

        private string GetUserName(int UserId)
        {
            if (UserId != 0)
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    try
                    {
                        int UserIdValue = Convert.ToInt32(UserId);
                        return _context.Users.Find(UserIdValue).DisplayName;

                    }
                    catch { return ""; }
                }
            }
            else
            {
                return "";
            }
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
                        case EndpointType.ActiveDirectory:
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


        public string RequestType
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>((RequestType)this.RequestTypeId);
            }
        }

        public string TaskName
        {
            get
            {
                if (this.AttestationId.HasValue)
                {
                    return this.AttestationName;
                }
                else if (this.AdHocAccessRequestId.HasValue)
                {
                    return this.EventName;
                }
                return string.Empty;
            }
        }

    }
}
