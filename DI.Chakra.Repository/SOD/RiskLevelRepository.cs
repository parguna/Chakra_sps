using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class RiskLevelRepository : RepositoryBase<SODRiskLevel>
    {
        public RiskLevelRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return (from risklevel in CurrentContext.SODRiskLevels
                    where risklevel.RiskLevelCode.Contains(searchString)
                    select new { risklevels = risklevel.Id, Value = risklevel.RiskLevelCode + " - " + risklevel.RiskLevelDescription })
                         .ToDictionary(c => c.risklevels, c => c.Value);
        }
    }
}
