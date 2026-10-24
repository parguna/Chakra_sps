using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_EndpointConfiguration")]
    public class EndpointConfiguration : EntityBase
    {
        public Nullable<long> EndpointSystemId { get; set; }

        public bool MapAll { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndpointSystemId")]
        public EndPoint EndpointSystem { get; set; }

        public List<EndpointConfigurationDetail> ConfigurationDetails { get; set; } 
    }
}
