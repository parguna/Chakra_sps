using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class UserEntitlement
    {
        public long EndpointUserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; } //Role , Group, Profile
        public string Remarks1 { get; set; }
        public EndpointType EndPointSystemType { get; set; }
        public string EndpointSystemName { get; set; }
        public long EndpointSystemId { get; set; }
    }
}
