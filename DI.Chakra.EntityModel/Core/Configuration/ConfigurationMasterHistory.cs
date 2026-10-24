using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_ConfigurationMasterHistory")]
    public class ConfigurationMasterHistory : EntityBase, ICreatable, IUpdatable
    {
        public short ConfigId { get; set; }

        public string ConfigKey { get; set; }

        public string ConfigValue { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
