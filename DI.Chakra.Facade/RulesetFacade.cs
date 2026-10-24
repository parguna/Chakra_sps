using DI.Chakra.DataAccess.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Model;
namespace DI.Chakra.Facade
{
   public class RulesetFacade
    {
       
       RulesetDA rulesetDA = new RulesetDA();

       public List<Ruleset> GetRulesetlist()
       {
           try
           {
               return rulesetDA.GetRulesetlist();
           }
           catch
           {
               throw;
           }
       }
       public Ruleset GetRulesetbyId(int id)
        {
            try
            {
                return rulesetDA.GetRulesetbyId(id);
            }
            catch
            {
                throw;
            }
        }
       
    }
}
