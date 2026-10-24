using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;

namespace DI.Chakra.Repository
{
    public class DesignationRepository : RepositoryBase<NDesignation>
    {
        public DesignationRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetDesignations(string searchString)
        {
            return CurrentContext.NDesignations.Where(item => item.Description.Contains(searchString))
                .Select(x => new { Key = x.Id, Value = x.Code + " - " + x.Description }).Take(100)
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }
    }
}
