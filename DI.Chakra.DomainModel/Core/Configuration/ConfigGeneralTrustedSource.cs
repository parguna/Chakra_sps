using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ConfigGeneralTrustedSource : DomainModelBase<ConfigGeneralTrustedSource, EntityModel.ConfigGeneralTrustedSource>
    {
        public string AttributeName { get; set; }

        public EndpointType TrustedSource { get; set; }

        public string TrustedSourceAttribute { get; set; }

        public bool SelfUpdate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public static void AddConfigTrustedSource(List<ConfigGeneralTrustedSource> configGeneralTrustedSource,string currentUser)
        {
            DeleteConfigTrustedSource();
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                foreach (ConfigGeneralTrustedSource item in configGeneralTrustedSource)
                {
                    item.Save(dataAccessTransaction);
                }
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteConfigTrustedSource()
        {
            List<ConfigGeneralTrustedSource> listTrustedSrcs = ConfigGeneralTrustedSource.GetAll();
            if (listTrustedSrcs != null)
            {
                ConfigGeneralTrustedSource.DeleteList(listTrustedSrcs);
            }
        }

        public override long Save(string currentUser)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(currentUser))
            {
                this.CreatedDate = DateTime.Now;
                this.LastModifiedDate = DateTime.Now;
                base.Save(tranaction);
                tranaction.Commit();
            }
            return 0;
        }

        public static bool IsSelfUpdated(string attributeName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.ConfigGeneralTrustedSource>(dataAccessTransaction)
                      .Count(configTrust => configTrust.AttributeName == attributeName && configTrust.SelfUpdate) > 0 ? true : false;
            }
        }

        public static bool DoesAlreadyExists(string attributename, EndpointType endpointType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.ConfigGeneralTrustedSource>(dataAccessTransaction).Any(trustsource => trustsource.TrustedSource == (EndpointType)endpointType && trustsource.AttributeName == attributename);

            }

        }
    }
}

