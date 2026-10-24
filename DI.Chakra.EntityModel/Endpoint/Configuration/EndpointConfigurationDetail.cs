using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_EndpointConfigurationDetail")]
    public class EndpointConfigurationDetail : EntityBase
    {
        public Nullable<long> EndpointConfigurationId { get; set; }

        public Nullable<long> KeyId { get; set; }

        public string Value { get; set; }
        
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndpointConfigurationId")]
        public EndpointConfiguration EndpointConfiguration { get; set; }

        [ForeignKey("KeyId")]
        public EndpointConfigurationKey EndpointConfigurationKey { get; set; }

    }

}
