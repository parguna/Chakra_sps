using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class AccessRequestConfiguration : DomainModelBase<AccessRequestConfiguration, EntityModel.AccessRequestConfiguration>
    {
        public long EndpointSystemId { get; set; }
        public DI.Chakra.Infrastructure.Enums.AccountType AccountType { get; set; }
        public DI.Chakra.Infrastructure.Enums.AdHocActionType EnableActionType { get; set; }
        public EndPoint EndPointSystem { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }


        public static List<AccessRequestConfiguration> GetAccessRequestConfigurationByEndPointSystemId(long endpointSystemId, long AccoutTypeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var enumerablelist = new RepositoryBase<EntityModel.AccessRequestConfiguration>(dataAccessTransaction).Get(config => config.EndpointSystemId == endpointSystemId);

                if( Enum.IsDefined(typeof(Infrastructure.Enums.AccountType), (byte)AccoutTypeId) && (Infrastructure.Enums.AccountType)AccoutTypeId != Infrastructure.Enums.AccountType.None)
                    enumerablelist = enumerablelist.Where(config => config.AccountType == (Infrastructure.Enums.AccountType) AccoutTypeId).Select(config=>config);


                return ORMapList(enumerablelist.ToList());
            }
        }

          
    }
}
