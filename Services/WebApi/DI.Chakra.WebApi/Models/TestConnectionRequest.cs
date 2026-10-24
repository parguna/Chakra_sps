using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChakraService.Models
{
    public class TestConnectionRequest
    {
        public string IMEI { get; set; }
        public string MobileNumber { get; set; }
        public string SerialNumber { get; set; }
    }
}