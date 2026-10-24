using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepIdentity.Model
{
    public class SAPRevokeRequestReportData
    {
        public string AttestationName { get; set; }

        public string AccountName { get; set; }

        public string DisplayName { get; set; }

        public string Endpoint { get; set; }

        public string HostName { get; set; }

        public string RevokedBy { get; set; }

        public long ReviewEndpointId { get; set; }

        public DateTime? RevokedOn { get; set; }

        public string Comments { get; set; }
    }
}
