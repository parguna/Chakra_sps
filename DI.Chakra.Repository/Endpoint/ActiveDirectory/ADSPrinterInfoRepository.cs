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
    public class ADSPrinterInfoRepository : RepositoryBase<ADSPrinterInfo>
    {
        public ADSPrinterInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationADSPrinterInfo> GetAll()
        {
            return (from objs in CurrentContext.ADSPrinterInfos
                    join eps in CurrentContext.EndPoint
                    on objs.EndPointSystemId equals eps.Id
                    select new PresentationADSPrinterInfo
                    {
                        Id = objs.Id,
                        DisplayName = eps.DisplayName,
                        name = objs.name,
                        whenCreated = objs.whenCreated.Value
                    }).ToList();
        }

    }
}
