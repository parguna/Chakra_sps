using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class DiscoveryJobGridData
    {
        public Nullable<int> DiscoveryProcessId { get; set; }
        public int SchedulerID { get; set; }
        public long EndpointSystemId { get; set; }
        public string EndpointSystemName { get; set; }
        public Nullable<System.DateTime> RunDate { get; set; }
        public Nullable<System.DateTime> ProcessStartAt { get; set; }
        public Nullable<System.DateTime> ProcessEndAt { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public bool Default { get; set; }
        public string ProcessName { get; set; }
        public string Description { get; set; }
    }
}
