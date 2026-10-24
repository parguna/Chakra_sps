using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SODPolicyConfigurationRepository : RepositoryBase<SODPolicyConfiguration>
    {
        public SODPolicyConfigurationRepository(DataAccessTransaction transaction) : base(transaction) { }
        public bool DoesExists(SODPolicyConfiguration policy)
        {
            bool isExists = false;
            List<SODPolicyConfiguration> policies = (from comp in CurrentContext.SODPolicyConfigurations
                                                     where comp.RiskLevelId.Equals(policy.RiskLevelId)
                                                     select comp).ToList();
            foreach(SODPolicyConfiguration pol in  policies)
            {
                if (policy.Operator == pol.Operator && policy.RiskScore == pol.RiskScore && policy.RiskScoreOperator == pol.RiskScoreOperator)
                {
                    isExists = true;
                    break;
                }
            }
            return isExists;
           
        }
    }
}
