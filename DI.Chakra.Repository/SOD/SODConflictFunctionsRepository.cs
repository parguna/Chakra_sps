using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SODConflictFunctionsRepository : RepositoryBase<SODConflictFunctions>
    {
        public SODConflictFunctionsRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return (from riskcd in CurrentContext.SODConflictFunctions
                    where riskcd.SodConflictCode.Contains(searchString)
                    select new { Key = riskcd.Id, Value = riskcd.SodConflictCode + " - " + riskcd.SodConflictDescription })
                         .ToDictionary(c => c.Key, c => c.Value);
        }

        public Dictionary<long, string> GetBySodConflictCode(string searchString, string filterString)
        {
            if (String.IsNullOrEmpty(filterString))
            {
                return (from conFun in CurrentContext.SODConflictFunctions
                        where conFun.SodConflictCode.Contains(searchString)
                        select new { Key = conFun.Id, Value = conFun.SodConflictCode + " - " + conFun.SodConflictDescription })
                        .Distinct().OrderBy(s => s.Value)
                 .ToDictionary(c => c.Key, c => c.Value);
            }
            else
            {
                return (from conFun in CurrentContext.SODConflictFunctions
                        where conFun.SodConflictCode.Contains(searchString) && conFun.CompliancePolicyType == filterString
                        select new { Key = conFun.Id, Value = conFun.SodConflictCode + " - " + conFun.SodConflictDescription })
               .Distinct().OrderBy(s => s.Value)
                 .ToDictionary(c => c.Key, c => c.Value);
            }
        }

        public override long Insert(SODConflictFunctions conflictFunction)
        {
            if (conflictFunction.Functions.Count > 0)
            {
                var selectedFunctions = conflictFunction.Functions.ToList();
                conflictFunction.Functions.Clear();
                foreach (var sodfunction in selectedFunctions)
                {
                    SODFunction function = this.CurrentContext.SODFunctions.Find(sodfunction.Id);
                    if (function != null)
                        conflictFunction.Functions.Add(function);
                }
            }
            return base.Insert(conflictFunction);

        }

        public override void Update(SODConflictFunctions conflictFunction)
        {
            SODConflictFunctions dbConflictFunction = this.GetById(conflictFunction.Id, r => r.Functions);
            dbConflictFunction.Functions.Clear();
            foreach (var sodFunction in conflictFunction.Functions)
            {
                SODFunction function = this.CurrentContext.SODFunctions.Find(sodFunction.Id);
                if (function != null)
                    dbConflictFunction.Functions.Add(function);
            }

            base.Update(conflictFunction);
        }
    }
}

