using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class SAPHANAObjectInfoRepository : RepositoryBase<SAPHANAObjectInfo>
    {
        public SAPHANAObjectInfoRepository(DataAccessTransaction transaction) : base(transaction) { }


        public Dictionary<long, string> GetByEndpointObjCode(long endPointId, string objCode)
        {
            return (from objInfo in CurrentContext.SAPHANAObjectInfo
                    where objInfo.EndPointSystemId == endPointId && objInfo.SapObjectCode.Contains(objCode)
                    select new { objInfo.Id, objInfo.SapObjectCode }).Take(100)
                    .Distinct().OrderBy(s => s.SapObjectCode)
                    .ToDictionary(c => c.Id, c => c.SapObjectCode);
                      
        }

        
    }
}
