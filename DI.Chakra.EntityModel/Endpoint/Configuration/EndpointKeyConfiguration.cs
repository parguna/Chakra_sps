using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_EndpointKeyConfiguration")]
    public class EndpointKeyConfiguration : EntityBase
    {
        public long EndpointSystemTypeId { get; set; }

        public Nullable<long> KeyId { get; set; }

        public int Order { get; set; }

        public string DataType { get; set; }

        public string RegExValidationPattern { get; set; }

        public string ClientScript { get; set; }

        public bool IsRequired { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("KeyId")]
        public EndpointConfigurationKey EndpointConfigurationKey { get; set; }

    }

}
