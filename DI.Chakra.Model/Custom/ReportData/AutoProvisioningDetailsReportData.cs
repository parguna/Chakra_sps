using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model.ReportData
{
    public class AutoProvisioningDetailsReportData
    {
        public long accessRequestId { get; set; }
        public long symentecRequestId { get; set; }
        public string userName { get; set; }
        public string isRemoved { get; set; }
        public string requestStatus { get; set; }
        public string progressStatus { get; set; }
        public DateTime? executionTime { get; set; }
        public DateTime? activationDate { get; set; }
        public string remarks { get; set; }
        public string description { get; set; }
        public AccessRequestProgress progressStatusENUM { get; set; }
        public AccessRequestStatus requestStatusENUM { get; set; }
    }
}
