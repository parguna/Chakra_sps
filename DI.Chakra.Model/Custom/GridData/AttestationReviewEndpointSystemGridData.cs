
using System;
namespace DI.Chakra.Model
{
    public class AttestationReviewEndpointSystemGridData
    {
        public long ReviewEndpointId { get; set; }
        public long EndpointUserId { get; set; }
        public string EndpointUserName { get; set; }
        public long EndpointSystemId { get; set; }
        public string HostName { get; set; }
        public long EndpointSystemTypeId { get; set; }
        public string EndpointSystemTypeName { get; set; }
        public Nullable<bool> Approve { get; set; }
        public string Comments { get; set; }
        public string AccountType { get; set; }
        public string IsActive { get; set; }
    }
}
