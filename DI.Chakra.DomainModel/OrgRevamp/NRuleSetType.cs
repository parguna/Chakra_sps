using DI.Chakra.Transaction;
using DI.Chakra.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public partial class NRulesetType : DomainModelBase<NRulesetType, EntityModel.NRulesetType>
    {
        public string RulesetType { get; set; }

        public string Description { get; set; }

        public long Score { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static List<NRulesetType> GetAllRulesetTypes()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var rulesetType = new RepositoryBase<EntityModel.NRulesetType>(dataAccessTransaction)
                       .GetAll().ToList();

                return ORMapList(rulesetType);
            }
        }
    }
}
