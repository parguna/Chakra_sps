using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationAdHocAccessRequest : AdHocAccessRequest
    {
        public long AdHocAccessRequestId { get; set; }
        public long AccessRequestID { get; set; }
        public long? BulkRequestId { get; set; }
        public string Name { get; set; }
        public string RequestSource { get; set; }
        public string RequestType { get; set; }
        public System.DateTime RequestedDate { get; set; }
        public System.DateTime? ExecutionDate { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress RequestProgress { get; set; }
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

        public string ExecuteDateString()
        {
            return (this.ExecutionDate.HasValue) ? ExecutionDate.Value.ToString("dd-MM-yyyy") : "";
        }
        public DateTime? EndDate { get; set; }

        public DateTime? StartDate { get; set; }
    }
}
