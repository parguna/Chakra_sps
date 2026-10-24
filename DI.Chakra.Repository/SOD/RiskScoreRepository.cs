using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class RiskScoreRepository : RepositoryBase<SODRiskScore>
    {
        public RiskScoreRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return (from riskscr in CurrentContext.SODRiskScores
                    where riskscr.RiskScore.ToString().Contains(searchString)
                    select new { riskscrs= riskscr.Id, Value = riskscr.RiskScore + " - " + riskscr.RiskScoreDescription })
                         .ToDictionary(c => c.riskscrs, c => c.Value);
        }
    }
}