using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model.SAP;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    [Serializable]
    public class EndpointInformation : IAggregateRoot
    {
        public long Endpointsystemid { get; set; }
        public string EndpointsystemType { get; set; }
        public int EndpointSystemTypeId { get; set; }
        public string Endpointsystemdescription { get; set; }
        public string TenantId { get; set; }
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string DomainName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
        public string Status { get; set; }
      

    }
}


