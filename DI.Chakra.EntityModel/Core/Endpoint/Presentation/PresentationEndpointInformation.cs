using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace DI.Chakra.EntityModel.Presentation
{
    [Serializable]
    public class PresentationEndpointInformation 
    {
        public long Endpointsystemid { get; set; }
        public string EndpointsystemType { get; set; }
        public int EndpointSystemTypeId { get; set; }
        public string Endpointsystemdescription { get; set; }
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string DomainName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
        public string Status { get; set; }
        
    }
}


