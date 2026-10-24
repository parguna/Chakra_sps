using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DI.Chakra.Model
{
    [TableInfo("DI_ADS_Configuration", "DI_ADS_Configuration")]
    [Table("DI_ADS_Configuration")]
    public class ADSConfiguration : IAggregateRoot, IEndpointConfiguration
    {
        [Key]
        public long Id { get; set; }
        public Nullable<long> EndpointSystemId { get; set; }
        public string IPAddress { get; set; }
        public string DomainName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Ou { get; set; }
        public Nullable<bool> MapAll { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndpointSystemId")]
        public virtual EndPoint EndpointSystem { get; set; }

    }

}
