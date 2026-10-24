using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.EntityModel.Presentation
{
    public  class PresentationAccessRequest
    {
        public long Id { get; set; }
        public long? AdHocAccessRequestId { get; set; }
        public long? TriggerId { get; set; }
        public string DisplayName { get; set; }
        public string ADID { get; set; }
        public string RequestSource { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }
        public string EndpointsystemName { get; set; }
        public DateTime? ExecutionTime { get; set; }

        public DateTime? RequestDate { get; set; }
        public string EndpointName { get; set; }

        public long AccessRequestID { get; set; }
        public string Name { get; set; }
        public string RequestType { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public DI.Chakra.Infrastructure.Enums.AccessRequestStatus RequestStatus { get; set; }

        public string Status { get; set; }
        public string Comments { get; set; }

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

        public bool IsAdhocRequest { get; set; }
        public bool IsBulkRequest { get; set; }
        public bool IsUndoEventRequest { get; set; }
        public string Entitlement { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public  List<BulkRequestDetail> BulkRequestDetails { get; set; }
    }
}
