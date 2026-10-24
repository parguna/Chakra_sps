using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SystemSetupReports
    {
        public static IList<object> GetComplianceReportResult(string ReportType)
        {
            IList<object> resultList = new List<object>();
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                switch (ReportType)
                {
                    case "Functions":
                        IList<object> funreport = new SODFunctionRepository(tranaction).GetReport().ToList<object>();
                        resultList = funreport;
                        break;
                    case "RiskCode":
                        IList<object> riskcodereport = new RiskCodeRepository(tranaction).GetReportforRiskCode().ToList<object>();
                        resultList = riskcodereport;
                        break;
                    case "PolicyExecution":
                        IList<object> executionreport = new SODPolicyExceutionDefinitionRepository(tranaction).GetReportforPolicyExecution().ToList<object>();
                        resultList = executionreport;
                        break;
                    case "Conflicts":
                        IList<object> conflictreport = new ConflictRepository(tranaction).GetReportforConflict().ToList<object>();
                        resultList = conflictreport;
                        break;
                    case "Exceptions":
                        IList<object> exceptionreport = new SODExceptionRepository(tranaction).GetReportforException().ToList<object>();
                        resultList = exceptionreport;
                        break;
                    case "Mitigations":
                        IList<object> mitigationreport = new SODMitigationRepository(tranaction).GetReportforMitigation().ToList<object>();
                        resultList = mitigationreport;
                        break;
                    case "Policy":
                        IList<object> compliancepolicyreport = new CompliancePolicyRepository(tranaction).GetReportforCompliancePolicy().ToList<object>();
                        resultList = compliancepolicyreport;
                        break;
                }
            }
            return resultList;
        }
    }
}
