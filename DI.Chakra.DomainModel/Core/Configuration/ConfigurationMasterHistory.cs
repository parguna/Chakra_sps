using System;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class ConfigurationMasterHistory : DomainModelBase<ConfigurationMasterHistory, EntityModel.ConfigurationMasterHistory>
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
