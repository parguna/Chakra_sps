using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class RiskCodeRepository : RepositoryBase<SODRiskCode>
    {
        public RiskCodeRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return (from riskcd in CurrentContext.SODRiskCodes
                    where riskcd.Status != 0 && riskcd.RiskCode.Contains(searchString)
                    select new { Key = riskcd.Id, Value = riskcd.RiskCode + " - " + riskcd.RiskDescription })
                         .ToDictionary(c => c.Key, c => c.Value);
        }

        

        public IList<object> GetReportforRiskCode()
        {
            return (from riskCode in CurrentContext.SODRiskCodes
                    join riskLevel in CurrentContext.SODRiskLevels
                    on riskCode.RiskLevelId equals riskLevel.Id
                    join riskScore in CurrentContext.SODRiskScores
                    on riskCode.RiskScoreId equals riskScore.Id
                    join user in CurrentContext.User
                    on riskCode.UserId equals user.Id
                    select new
                    {
                        RiskCode = riskCode.RiskCode,
                        RiskDescription = riskCode.RiskDescription,
                        RiskLevel = riskLevel.RiskLevelCode,
                        RiskScore = riskScore.RiskScore,
                        RiskCodeOwner = user.UserName
                    }).ToList<object>();
        }
    }
}
