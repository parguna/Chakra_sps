using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.Model
{
    public class SAPAccountRequestsReportData
    {
        public string AttestationName { get; set; }

        public string AccountName { get; set; }

        public string DisplayName { get; set; }

        public string Endpoint { get; set; }

        public string HostName { get; set; }

        public long ReviewEndpointId { get; set; }

        public string ReviewedBy { get; set; }

        public DateTime? ReviewedOn { get; set; }

        public string Comments { get; set; }

        public bool Approve { get; set; }

        public bool Revoke { get; set; }

        public bool IsChanged { get; set; }
    }
}
