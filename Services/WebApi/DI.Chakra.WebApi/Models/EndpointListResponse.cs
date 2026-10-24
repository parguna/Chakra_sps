using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChakraService.Models
{
    public class EndpointListResponse
    {
        public ProcessResponse ProcessResponse { get; set; }
        public List<EndpointDTO> EndpointList { get; set; }
    }
}