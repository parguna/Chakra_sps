using System;
using System.ComponentModel;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class ConfigurationMasterDGM : DtoBase, IConfigurationMaster
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
