using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_Config_General_TrustedSource")]
    public class ConfigGeneralTrustedSource : EntityBase, ICreatable, IUpdatable
    {
        public string AttributeName { get; set; }

        public EndpointType TrustedSource { get; set; }

        public string TrustedSourceAttribute { get; set; }

        public bool SelfUpdate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}
