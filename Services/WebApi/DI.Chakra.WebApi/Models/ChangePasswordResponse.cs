using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChakraService.Models
{
    public class ChangePasswordResponse
    {
        public int EndpointId { get; set; }
        public ProcessResponse ProcessResponse { get; set; }
    }
}