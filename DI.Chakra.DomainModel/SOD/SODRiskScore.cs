using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;

namespace DI.Chakra.DomainModel
{
    public class SODRiskScore : DomainModelBase<SODRiskScore, EntityModel.SODRiskScore>
    {
        public int RiskScore { get; set; }

        public string RiskScoreDescription { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static bool DoesExists(long riskScore)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODRiskScore>(dataAccessTransaction).Any(riskscr => riskscr.RiskScore == riskScore);
            }
        }
       
        public static Dictionary<long, string> GetBySearchString(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RiskScoreRepository(dataAccessTransaction).GetBySearchString(CheckNull.AsteriskToString(searchString));
            }
        }
    }
}
