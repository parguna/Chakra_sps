using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;


namespace DI.Chakra.Model
{
    [Table("DI_M_Config_General_TrustedSource")]
    public class ConfigGeneralTrustedSource : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string AttributeName { get; set; }
        public EndpointType TrustedSource { get; set; }
        public string TrustedSourceAttribute { get; set; }
        public bool SelfUpdate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
