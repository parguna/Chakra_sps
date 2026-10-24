using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class ConflictRepository : RepositoryBase<SODConflict>
    {
        public ConflictRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<object> GetReportforConflict()
        {
            return (from conflict in CurrentContext.SODConflicts
                    join function1 in CurrentContext.SODFunctions
                    on conflict.function1.Id equals function1.Id
                    join function2 in CurrentContext.SODFunctions
                    on conflict.function2.Id equals function2.Id
                    join riskCode in CurrentContext.SODRiskCodes
                    on conflict.RiskCodeId equals riskCode.Id
                    select new
                    {
                        SODConflictCode = conflict.SODConflictCode,
                        SODConflictDescription = conflict.SODConflictDescription,
                        SodConflictType = conflict.SodConflictType,
                        Function1 = function1.SodFunctionCode,
                        Function2 = function2.SodFunctionCode,
                        RiskCode = riskCode.RiskCode
                    }).ToList<object>();
          }

        public List<SODConflict> GetConflictPairByFunction(long functionId)
        {
            return (from conflict in CurrentContext.SODConflicts where  conflict.FunctionId1==functionId || conflict.FunctionId2==functionId select conflict).ToList();
        }
    }
}
