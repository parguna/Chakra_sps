using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;

namespace DI.Chakra.Repository
{
    public class NCostCenterRepository : RepositoryBase<NCostCenter>
    {
        public NCostCenterRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetNCostCenters(string searchString)
        {
            return CurrentContext.NCostCenters.Where(item => (item.Description.Contains(searchString)|| item.Code.Contains(searchString)))
                .Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Description })
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }
    }
}
