using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class ADSComputerInfoRepository : RepositoryBase<ADSComputerInfo>
    {
        public ADSComputerInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationADSComputerInfo> GetAll()
        {
            return (from objs in CurrentContext.ADSComputerInfo
                    join eps in CurrentContext.EndpointSystemTypes
                    on objs.EndPointSystemId equals eps.Id
                    select new PresentationADSComputerInfo
                    {
                        Id = objs.Id,
                        DisplayName = eps.DisplayName,
                        lastLogoff = objs.lastLogoff.Value,
                        lastLogon = objs.lastLogon.Value,
                        sAMAccountName = objs.sAMAccountName,
                        whenCreated = objs.whenCreated.Value
                    }).ToList();
        }
    }
}
