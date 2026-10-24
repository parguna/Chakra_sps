using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SODFunctionItemRepository : RepositoryBase<SODFunctionItem>
    {
        public SODFunctionItemRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBySearchString(string inputParams, string searchString)
        {
            return (from funitm in CurrentContext.SODFuntionItem
                    join func in CurrentContext.SODFunctions  on funitm.SodFunctionId equals func.Id
                    where func.SodFunctionCode.Equals(inputParams) && funitm.SodRoleId.Contains(searchString)
                    select new { funitems = funitm.Id, Value =funitm.SodRoleId+"-"+funitm.SodRoleDesc })
                       .ToDictionary(c => c.funitems, c => c.Value);
        }
    }
}

