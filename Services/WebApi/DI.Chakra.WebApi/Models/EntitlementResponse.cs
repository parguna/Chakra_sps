using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChakraService.Models
{
    public class EntitlementResponse
    {
        public string EndpointSystem { get; set; }
        public long EndpointUserID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}