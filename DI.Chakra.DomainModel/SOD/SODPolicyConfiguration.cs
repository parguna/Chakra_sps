using System;
using System.Collections.Generic;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DI.Chakra.DomainModel
{
    public class SODPolicyConfiguration : DomainModelBase<SODPolicyConfiguration, EntityModel.SODPolicyConfiguration>
    {
        public long RiskLevelId { get; set; }

        public string Operator { get; set; }

        public long RiskScore { get; set; }

        public string RiskScoreOperator { get; set; }

    
        public SODRiskLevel SODRiskLevel { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }



        public static bool  DoesExists(long riskrevel)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODPolicyConfiguration>(dataAccessTransaction)
                    .Any(rl=> rl.RiskLevelId == riskrevel);
            }
        }


        public static bool isExists(SODPolicyConfiguration policy)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return (new SODPolicyConfigurationRepository(dataAccessTransaction).DoesExists(ORMap(policy)));
            }
        }

        public static EntityModel.SODPolicyConfiguration ORMap(DomainModel.SODPolicyConfiguration policy)
        {
            if (policy == null)
                return null;
            return Mapper.Map<DomainModel.SODPolicyConfiguration, EntityModel.SODPolicyConfiguration>(policy);
        }

    }
}
