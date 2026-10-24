using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public class EntitlementmanagementNavigatorHelper
    {
        public EntitlementmanagementNavigatorHelper()
        {
            FilterFieldAndValue = new Dictionary<string, string>();
        }
        public long EndpointsystemId { get; set; }
        public EndpointType ApplicationType { get; set; }
        public long ApplicationTypeId { get; set; }
        public Dictionary<string, string> FilterFieldAndValue { get; set; }
    }
}
