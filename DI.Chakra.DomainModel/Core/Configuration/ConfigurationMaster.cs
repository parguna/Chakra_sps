using System;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public class ConfigurationMaster : DomainModelBase<ConfigurationMaster, EntityModel.ConfigurationMaster>, IConfigurationMaster
    {
        public string ConfigKey { get; set; }

        public string ConfigValue { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public bool IsUiDisplay { get; set; }

        public static IConfigurationMaster GetByConfigKey(string configKey, Product product)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                switch (product)
                {
                    case Product.IACM:
                        IConfigurationMaster v = ORMap(new RepositoryBase<EntityModel.ConfigurationMaster>(dataAccessTransaction)
                       .Get(tm => tm.ConfigKey.ToUpper() == configKey.ToUpper()).FirstOrDefault());
                        return v;
                    case Product.IM:
                        return ConfigurationMasterIM.GetByConfigKey(configKey);
                    case Product.PWDManagement:
                        return ConfigurationMasterPasswordManagement.GetByConfigKey(configKey);
                    case Product.Compliance:
                        return ConfigurationMasterCompliance.GetByConfigKey(configKey);
                    case Product.DGM:
                        return ConfigurationMasterDGM.GetByConfigKey(configKey);
                    default:
                        return ORMap(new RepositoryBase<EntityModel.ConfigurationMaster>(dataAccessTransaction)
                       .Get(tm => tm.ConfigKey.ToUpper() == configKey.ToUpper()).FirstOrDefault());
                }
            }
        }

        public static string GetConfigKey(string configKey, Product product)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                switch (product)
                {
                    case Product.IACM:
                        return (new RepositoryBase<EntityModel.ConfigurationMaster>(dataAccessTransaction)
                        .Get(uconfmaster => uconfmaster.ConfigKey == configKey).Select(uconfmaster => uconfmaster.ConfigValue).FirstOrDefault());
                    case Product.IM:
                        return ConfigurationMasterIM.GetByConfigKey(configKey).ConfigValue;
                    case Product.PWDManagement:
                        return ConfigurationMasterPasswordManagement.GetByConfigKey(configKey).ConfigValue;
                    case Product.Compliance:
                        return ConfigurationMasterCompliance.GetByConfigKey(configKey).ConfigValue;
                    case Product.DGM:
                        return ConfigurationMasterDGM.GetByConfigKey(configKey).ConfigValue;
                    default:
                        return (new RepositoryBase<EntityModel.ConfigurationMaster>(dataAccessTransaction)
                       .Get(uconfmaster => uconfmaster.ConfigKey == configKey).Select(uconfmaster => uconfmaster.ConfigValue).FirstOrDefault());
                }
            }
        }

        public static bool DoesConfigKeyAlreadyExists(string configKey)  
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.ConfigurationMaster>(dataAccessTransaction)
                      .Any(uconfmaster => uconfmaster.ConfigKey == configKey);
            }
        }

        public static bool DoesConfigKeyAlreadyExists(string configKey, Product product)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                switch (product)
                {
                    case Product.IACM:
                        return DoesConfigKeyAlreadyExists(configKey);
                    case Product.IM:
                        return ConfigurationMasterIM.DoesConfigKeyAlreadyExists(configKey);
                    case Product.PWDManagement:
                        return ConfigurationMasterPasswordManagement.DoesConfigKeyAlreadyExists(configKey);
                    case Product.Compliance:
                        return ConfigurationMasterCompliance.DoesConfigKeyAlreadyExists(configKey);
                    case Product.PIM:
                        return ConfigurationMasterPIM.DoesConfigKeyAlreadyExists(configKey);
                    case Product.DGM:
                        return ConfigurationMasterDGM.DoesConfigKeyAlreadyExists(configKey);
                    default:
                        return DoesConfigKeyAlreadyExists(configKey);
                }
            }
        }

        public static List<ConfigurationMaster> GetAllConfigurationWithUIDisplay()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ConfigurationMaster>(dataAccessTransaction)
                      .Get(uconfmaster => uconfmaster.IsUiDisplay == true).ToList());
            }
        }

        public static List<IConfigurationMaster> GetConfigurationWithUIDisplayByProduct(Product product)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                switch (product)
                {
                    case Product.IACM:
                        return GetAllConfigurationWithUIDisplay().Cast<IConfigurationMaster>().ToList();
                    case Product.IM:
                        return ConfigurationMasterIM.GetAllConfigurationWithUIDisplay().Cast<IConfigurationMaster>().ToList();
                    case Product.PWDManagement:
                        return ConfigurationMasterPasswordManagement.GetAllConfigurationWithUIDisplay().Cast<IConfigurationMaster>().ToList();
                    case Product.Compliance:
                        return ConfigurationMasterCompliance.GetAllConfigurationWithUIDisplay().Cast<IConfigurationMaster>().ToList();
                    case Product.PIM:
                        return ConfigurationMasterPIM.GetAllConfigurationWithUIDisplay().Cast<IConfigurationMaster>().ToList();
                    case Product.DGM:
                        return ConfigurationMasterDGM.GetAllConfigurationWithUIDisplay().Cast<IConfigurationMaster>().ToList();
                    default:
                        return GetAllConfigurationWithUIDisplay().Cast<IConfigurationMaster>().ToList();
                }
            }
        }

        public static IConfigurationMaster GetByProduct(long id, Product product)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                switch (product)
                {
                    case Product.IACM:
                        return (IConfigurationMaster)GetById(id);
                    case Product.IM:
                        return (IConfigurationMaster)ConfigurationMasterIM.GetById(id);
                    case Product.PWDManagement:
                        return (IConfigurationMaster)ConfigurationMasterPasswordManagement.GetById(id);
                    case Product.Compliance:
                        return (IConfigurationMaster)ConfigurationMasterCompliance.GetById(id);
                    case Product.PIM:
                        return (IConfigurationMaster)ConfigurationMasterPIM.GetById(id);
                    case Product.DGM:
                        return (IConfigurationMaster)ConfigurationMasterDGM.GetById(id);
                    default:
                        return (IConfigurationMaster)GetById(id);
                }
            }
        }

        public static void DeleteConfiguration(long id, Product product)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var configuration = GetByProduct(id, product);
                switch (product)
                {
                    case Product.IACM:
                        ((ConfigurationMaster)configuration).Delete(); break;
                    case Product.IM:
                        ((ConfigurationMasterIM)configuration).Delete(); break;
                    case Product.PWDManagement:
                        ((ConfigurationMasterPasswordManagement)configuration).Delete(); break;
                    case Product.Compliance:
                        ((ConfigurationMasterCompliance)configuration).Delete(); break;
                    case Product.PIM:
                        ((ConfigurationMasterPIM)configuration).Delete(); break;
                    case Product.DGM:
                        ((ConfigurationMasterDGM)configuration).Delete(); break;
                    default:
                        ((ConfigurationMaster)configuration).Delete(); break;
                }
                dataAccessTransaction.Commit();
            }
        }

        public static void SaveConfiguration(IConfigurationMaster configuration, Product product,string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                switch (product)
                {
                    case Product.IACM:
                        ((ConfigurationMaster)configuration).Save(dataAccessTransaction); break;
                    case Product.IM:
                        ((ConfigurationMasterIM)configuration).Save(dataAccessTransaction); break;
                    case Product.PWDManagement:
                        ((ConfigurationMasterPasswordManagement)configuration).Save(dataAccessTransaction); break;
                    case Product.Compliance:
                        ((ConfigurationMasterCompliance)configuration).Save(dataAccessTransaction); break;
                    case Product.PIM:
                        ((ConfigurationMasterPIM)configuration).Save(dataAccessTransaction); break;
                    case Product.DGM:
                        ((ConfigurationMasterDGM)configuration).Save(dataAccessTransaction); break;
                    default:
                        ((ConfigurationMaster)configuration).Save(dataAccessTransaction); break;
                }
                dataAccessTransaction.Commit();
            }
        }
    }
}
