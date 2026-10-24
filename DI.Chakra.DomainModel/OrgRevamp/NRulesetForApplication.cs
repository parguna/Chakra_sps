using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public partial class NRulesetForApplication : DomainModelBase<NRulesetForApplication, EntityModel.NRulesetForApplication>
    {
        public long RuleId { get; set; }

        public long EndpointSystemId { get; set; }

        public string AccountType { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public NRuleset Ruleset { get; set; }

        public EndPoint EndPoint { get; set; }

        public static List<NRulesetForApplication> GetRulesetForApplication(long UserId)
        {
            var rulesetCode = NRuleset.GetMatchingRulesets(UserId).Distinct().ToList();
            var ruleId = rulesetCode.Select(a => a.RuleId);
            return NRulesetForApplication.GetAll().Where(a => ruleId.Contains(a.RuleId)).GroupBy(b => b.EndpointSystemId).Select(a => a.First()).ToList();
        }
        public static List<NRulesetForApplication> GetRulesetForApplication(long UserId,EmployeeEventTrigger trigger)
        {
            var rulesetCode = NRuleset.GetMatchingRulesets(UserId, trigger).Distinct().ToList();
            var ruleId = rulesetCode.Select(a => a.RuleId);
            return NRulesetForApplication.GetAll(true).Where(a => ruleId.Contains(a.RuleId)).GroupBy(b => b.EndpointSystemId).Select(a => a.First()).ToList();
        }

        public static List<NRulesetForApplication> GetRulesetForApplication(User user)
        {
            var rulesetCode = NRuleset.GetMatchingRulesets(user).Distinct().ToList();
            var ruleId = rulesetCode.Select(a => a.RuleId);
            return NRulesetForApplication.GetAll(true).Where(a => ruleId.Contains(a.RuleId)).GroupBy(b => b.EndpointSystemId).Select(a => a.First()).ToList();
        }
     
        public static List<NRulesetForApplication> GetByRuleId(long ruleId)
        {
            return NRulesetForApplication.GetAll(true).Where(a => a.RuleId == ruleId).ToList();
        }

        public static void SaveApplicationDefault(long ruleId,List<NRulesetForApplication> appDefaultList, string currentUser)
        {
            List<NRulesetForApplication> removedAppDefaultList = null;
            using (var transaction = new DataAccessTransaction(currentUser))
            {
                List<NRulesetForApplication> dbAppDefaultList = NRulesetForApplication.GetByRuleId(ruleId);

                if (dbAppDefaultList.Count > 0)
                {
                    removedAppDefaultList =
                            dbAppDefaultList.Except(appDefaultList, new IdBaseEqualityComparer<NRulesetForApplication>()).ToList();
                    NRulesetForApplication.DeleteList(transaction, removedAppDefaultList);
                } 
                if(appDefaultList.Count > 0)
                    NRulesetForApplication.SaveList(transaction, appDefaultList);
                transaction.Commit();
            }
        }

   }
}
