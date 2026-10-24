using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class AttestationActionReportData
    {
        public long reviewId { get; set; }
        public long endPointId { get; set; }
        public string endPointSystemName { get; set; }
        public string endPointHostName { get; set; }
        public string useName { get; set; }
        public string displayName { get; set; }
        public string actionId { get; set; }
        public string actionTakenBy { get; set; }
        public string actionComments { get; set; }
        public string attestationActionUser { get; set; }
    }
}
