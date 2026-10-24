using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class CompliancePolicyRepository : RepositoryBase<CompliancePolicy>
    {
        public CompliancePolicyRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return (from comp in CurrentContext.CompliancePolicy
                    where comp.CompliancePolicyCode.Contains(searchString)
                    select new { compls = comp.Id, Value = comp.CompliancePolicyCode + " - " + comp.CompliancePolicyDescription })
                    .Distinct().OrderBy(s => s.Value)
                         .ToDictionary(c => c.compls, c => c.Value);
        }

        public List<object> GetReportforCompliancePolicy()
        {
            return (from policy in CurrentContext.CompliancePolicy
                    join user in CurrentContext.User
                    on (long)policy.OwnerId equals user.Id
                    select new
                    {
                        PolicyName = policy.CompliancePolicyCode,
                        PolicyCode = policy.CompliancePolicyCode,
                        PolicyDescription = policy.CompliancePolicyDescription,
                        PolicyOwner = user.UserName
                    }).ToList<object>();
        }
        public override long Insert(CompliancePolicy policy)
        {
            if (policy.Conflicts.Count > 0)
            {
                var selectedConflicts = policy.Conflicts.ToList();
                policy.Conflicts.Clear();
                foreach (var sodconflict in selectedConflicts)
                {
                    SODConflictFunctions function = this.CurrentContext.SODConflictFunctions.Find(sodconflict.Id);
                    if (function != null)
                        policy.Conflicts.Add(function);
                }
            }
            return base.Insert(policy);

        }

        public override void Update(CompliancePolicy policy)
        {
            CompliancePolicy dbConflictPolicy = this.GetById(policy.Id, r => r.Conflicts);
            dbConflictPolicy.Conflicts.Clear();
            foreach (var sodConflict in policy.Conflicts)
            {
                SODConflictFunctions conflict = this.CurrentContext.SODConflictFunctions.Find(sodConflict.Id);
                if (conflict != null)
                    dbConflictPolicy.Conflicts.Add(conflict);
            }

            base.Update(policy);
        }
    }
}
