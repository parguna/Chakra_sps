using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Serializable]
    [Table("DI_M_PIMConfigurationMaster")]
    public class ConfigurationMasterPIM : EntityBase, ICreatable, IUpdatable
    {
        public string ConfigKey { get; set; }

        public string ConfigValue { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public bool IsUiDisplay { get; set; }

    }
}
