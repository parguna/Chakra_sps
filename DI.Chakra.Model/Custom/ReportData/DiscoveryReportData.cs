using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class DiscoveryReportData
    {
        public long discoveryId { get; set; }
        public string endPointSystemType { get; set; }
        public string endPointSystemName { get; set; }
        public string endPointHostName { get; set; }
        public string endPointDomainName { get; set; }
        public string tenantId { get; set; }
        public string endPointIPAddress { get; set; }
        public string discoveryStatus { get; set; }
        public DateTime? discoverStartAt { get; set; }
        public DateTime? discoverEndAt { get; set; }
    }
}
