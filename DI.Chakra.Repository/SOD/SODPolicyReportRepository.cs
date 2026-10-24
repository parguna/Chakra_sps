using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SODPolicyReportRepository : RepositoryBase<SODPolicyReport>
    {
        public SODPolicyReportRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<string> GetUniqueRunCodes()
        {
            var distinctRunCodes = CurrentContext.SODPolicyReport.Select(m => m.UniqueRunCode).Distinct().ToList();
            return distinctRunCodes;
        }

    }
}
