using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SODRiskLevel : DomainModelBase<SODRiskLevel, EntityModel.SODRiskLevel>
    {
        public string RiskLevelDescription { get; set; }

        public string RiskLevelCode { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static bool DoesExists(string riskLevelCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODRiskLevel>(dataAccessTransaction)
                    .Any(risk => risk.RiskLevelCode == riskLevelCode);
            }
        }

        public static SODRiskLevel GetByRiskLevelCode(string riskLevelCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODRiskLevel>(dataAccessTransaction)
                    .Get(riskl => riskl.RiskLevelCode == riskLevelCode).FirstOrDefault());
            }
        }

        public static Dictionary<long, string> GetBySearchString(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RiskLevelRepository(dataAccessTransaction).GetBySearchString(CheckNull.AsteriskToString(searchString));
            }
        }
    }
}
