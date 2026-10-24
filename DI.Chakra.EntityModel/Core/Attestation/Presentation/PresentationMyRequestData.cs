using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationMyRequestData
    {
        public long AdHocAccessRequestId { get; set; }
        public long AccessRequestID { get; set; }
        public string Name { get; set; }
        public string RequestSource { get; set; }
        public string RequestType { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public DI.Chakra.Infrastructure.Enums.AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public string RequesterName { get; set; }
        public string RequesteeName { get; set; }
        public AdHocActionType AdhocActionType { get; set; }
        public long? EndpointSystemId { get; set; }
        public EndPoint EndPoint { get; set; }
        public AdHocAccessRequest AdHocAccessRequests { get; set; }
        public AccountType AccountType { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public long UserID { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string ErrorMessage { get; set; }
        public long? ParentrequestId { get; set; }
        public string RequesteeUserName { get; set; }
        public string RequesterUserName { get; set; }
        public string RequesteeEmployeeId { get; set; }
        public string RequesteeDepartment { get; set; }
        public bool IsAdhocRequest { get; set; }
        public bool IsBulkRequest { get; set; }
        public bool IsUndoEventRequest { get; set; }
        public string Entitlement { get; set; }       
        public ApplicationType ApplicationType {get;set;} 
        public string ServiceAccountComplianceString { get; set; }
        public long? BulkRequestId { get; set; }
        public string Remarks { get; set; }
        public string AccessRequestComments { get; set; }
        public string ConflictRemarks { get; set; }
        public List<BulkRequestDetail> BulkReqDetails { get; set; }

    }
}
