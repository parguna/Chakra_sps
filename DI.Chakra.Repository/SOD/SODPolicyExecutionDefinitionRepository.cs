using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SODPolicyExceutionDefinitionRepository : RepositoryBase<SODPolicyExecutionDefinition>
    {
        public SODPolicyExceutionDefinitionRepository (DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return (from policydef in CurrentContext.SODPolicyExecutionDefinition
                    join policy in CurrentContext.CompliancePolicy 
                    on policydef.CompliancePolicyId equals policy.Id
                    where policy.CompliancePolicyCode.Contains(searchString)
                    select new { Key = policydef.Id, Value = policy.CompliancePolicyCode + "_" + policydef.UniqueRunCode })
                    .Distinct().OrderBy(s=>s.Value)
                         .ToDictionary(c => c.Key, c => c.Value);
        }


        public List<object> GetReportforPolicyExecution()
        {
            return (from policy in CurrentContext.SODPolicyExecutionDefinition.Include("CompliancePolicy")
                    select new
                    {
                        PolicyCode = policy.CompliancePolicy.CompliancePolicyCode,
                        PolicyDescription = policy.CompliancePolicy.CompliancePolicyDescription,
                        UniqueRunCode = policy.UniqueRunCode,
                        OutputType = policy.OutputType,
                        RunAtObjectLevel = policy.RunAtObjectLevel.Value
                    }).ToList<object>();
        }
        public override long Insert(SODPolicyExecutionDefinition policyDefinition)
        {
            if (policyDefinition.Exceptions.Count > 0)
            {
                var selectedExceptions = policyDefinition.Exceptions.ToList();
                policyDefinition.Exceptions.Clear();
                foreach (var sodException in selectedExceptions)
                {
                    SODException exception = this.CurrentContext.SODException.Find(sodException.Id);
                    if (exception != null)
                        policyDefinition.Exceptions.Add(exception);
                }
            }
            return base.Insert(policyDefinition);

        }

        public override void Update(SODPolicyExecutionDefinition policyDefinition)
        {
            SODPolicyExecutionDefinition dbPolicyDefinition = this.GetById(policyDefinition.Id, r => r.Exceptions,r => r.EndpointSelection);
            // Delete Existing SODExceptionItems from db.
            dbPolicyDefinition.EndpointSelection.ToList<SODEndPointPolicySelection>().ForEach(t => this.CurrentContext.SODEndPointPolicySelection.Remove(t));
            // Add newly selected SODExceptionItems to db
            policyDefinition.EndpointSelection.ToList<SODEndPointPolicySelection>().ForEach(t => this.CurrentContext.SODEndPointPolicySelection.Add(t));

            dbPolicyDefinition.Exceptions.Clear();
            foreach (var sodException in policyDefinition.Exceptions)
            {
                SODException exception = this.CurrentContext.SODException.Find(sodException.Id);
                if (exception != null)
                    dbPolicyDefinition.Exceptions.Add(exception);
            }

            base.Update(policyDefinition);
        }
    }
}
