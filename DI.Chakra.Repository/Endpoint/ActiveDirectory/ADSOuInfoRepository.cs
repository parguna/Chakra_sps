using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class ADSOuInfoRepository : RepositoryBase<ADSOuInfo>
    {
        public ADSOuInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBySearchString(string searchString, long endpointsystemid) 
        {
            
            return CurrentContext.ADSOuInfos.Where(item => item.distinguishedName.Contains(searchString) && 
                                                    item.EndPointSystemId == endpointsystemid)
                                                    .Select(x => new { Key = x.Id, Value = x.distinguishedName })
                                                    .Distinct().ToDictionary(o => o.Key, o => o.Value);
        }
    }
}
