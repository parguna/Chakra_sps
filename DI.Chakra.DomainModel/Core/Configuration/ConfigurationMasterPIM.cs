using System;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class ConfigurationMasterPIM : DomainModelBase<ConfigurationMasterPIM, EntityModel.ConfigurationMasterPIM>, IConfigurationMaster
    {
        public string ConfigKey { get; set; }

        public string ConfigValue { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public bool IsUiDisplay { get; set; }

        public static ConfigurationMasterPIM GetByConfigKey(string configKey)  
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ConfigurationMasterPIM>(dataAccessTransaction)
                       .Get(tm => tm.ConfigKey.ToUpper() == configKey.ToUpper()).FirstOrDefault());
            }
        }

        public static string GetConfigKey(string configKey)  
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return (new RepositoryBase<EntityModel.ConfigurationMasterPIM>(dataAccessTransaction)
                       .Get(uconfmaster => uconfmaster.ConfigKey == configKey).Select(uconfmaster => uconfmaster.ConfigValue).FirstOrDefault());
            }
        }

        public static bool DoesConfigKeyAlreadyExists(string configKey)  
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.ConfigurationMasterPIM>(dataAccessTransaction)
                      .Any(uconfmaster => uconfmaster.ConfigKey == configKey);
            }
        }

        public static List<ConfigurationMasterPIM> GetAllConfigurationWithUIDisplay()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ConfigurationMasterPIM>(dataAccessTransaction)
                      .Get(uconfmaster => uconfmaster.IsUiDisplay == true).ToList());
            }
        }
    }
}
