using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_SAPEP_Configuration")]
    public partial class SAPEPConfiguration : IEndpointConfiguration
    {
        public long EndPointSystemId { get; set; }
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string DomainName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string WSDLUrl { get; set; }
        public Nullable<bool> MapAll { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        [Key]
        public long Id { get; set; }
        
        [ForeignKey("EndPointSystemId")]
        public virtual EndPoint EndpointSystem { get; set; }

    }
}
