using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Repository
{
    public class EndpointConfigurationRepository : RepositoryBase<EndpointConfiguration>
    {
        public EndpointConfigurationRepository(DataAccessTransaction transaction)
             : base(transaction)
        {

        }

        public EndpointConfiguration GetBySystemCode(string SystemCode)
        {

            EndpointConfiguration obj = (from inhouseSystems in this.CurrentContext.EndpointConfigurations
                                              join epSystems in this.CurrentContext.EndPoint
                                              on inhouseSystems.EndpointSystemId equals epSystems.Id
                                         where epSystems.ApplicationCode == SystemCode && epSystems.EndpointSystemTypeId == Convert.ToInt64(EndpointType.InhouseSystems)
                                              select inhouseSystems).FirstOrDefault();

            return obj;

        }
    }
}
