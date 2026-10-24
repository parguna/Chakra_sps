using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
 public   class LNSGroupRequestReportData
    {
        public string Group { get; set; }
        public string AttestationName { get; set; }
        public string Endpoint { get; set; }
        public string Comments { get; set; }
        public string ReviewedBy { get; set; }
        public Nullable<DateTime> ReviewedOn { get; set; }
        public string HostName { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public long L2DetailId { get; set; }
        public long ReviewEndpointId { get; set; }

        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public bool IsChanged { get; set; }


        public string Status
        {
            get
            {
                if (this.Approve)
                {
                    return "Approved";
                }
                else
                {
                    return "Revoked";
                }
            }
        }

    }
}
