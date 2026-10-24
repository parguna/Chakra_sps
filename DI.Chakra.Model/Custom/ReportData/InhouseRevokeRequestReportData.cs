using System;

namespace DI.Chakra.Model
{
    public class InhouseRevokeRequestReportData
    {
        public string AttestationName { get; set; }
        public string Endpoint { get; set; }
        public string Comments { get; set; }
        public string RevokedBy { get; set; }
        public Nullable<DateTime> RevokedOn { get; set; }
        public string HostName { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public long ReviewEndpointId { get; set; }
    }
}
