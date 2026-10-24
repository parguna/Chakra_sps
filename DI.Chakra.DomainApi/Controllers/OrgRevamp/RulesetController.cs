using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NRuleset")]

    public class NRulesetController : BaseController<DomainModel.NRuleset, EntityModel.NRuleset, Dto.NRuleset>
    {
       
        [HttpGet]
        [Route("GetAllRulesetSearchBystring")]
        public static Dictionary<long, string> GetAllRulesetSearchBystring(string searchString)
        {
            return DomainModel.NRuleset.GetAllRulesetstring(searchString);
        }


        [HttpGet]
        [Route("GetUniqueRulesets")]
        public List<Dto.NRuleset> GetUniqueRulesets()
        {
            return DtoMapList(DomainModel.NRuleset.GetUniqueRulesets());
        }

        [HttpGet]
        [Route("GetUserUniqueRulesets")]
        public List<Dto.NRuleset> GetUserUniqueRulesets(long userId)
        {
            return DtoMapList(DomainModel.NRuleset.GetUserUniqueRulesets(userId));
        }

        [HttpGet]
        [Route("DeleteRuleSetsByRuleId")]
        public int DeleteRuleSetsByRuleId(long ruleId)
        {
            return DomainModel.NRuleset.DeleteRuleSetsByRuleId(ruleId);
        }
        [HttpGet]
        [Route("DoesCodeExists")]
        public bool DoesCodeExists(string Code)
        {
            return DomainModel.NRuleset.DoesCodeExists(Code);
        }
        [HttpGet]
        [Route("GetAllRuleset")]
        public Dictionary<long, string> GetAllRulesetstring(string searchString)
        {
            return DomainModel.NRuleset.GetAllRulesetstring(searchString);
        }

        [HttpGet]
        [Route("GetSingleRulesetByRuleId")]
        public Dto.NRuleset GetSingleRulesetByRuleId(long ruleId)
        {
            return DtoMap(DomainModel.NRuleset.GetSingleRulesetByRuleId(ruleId));
        }


        [HttpGet]
        [Route("GetRuleId")]
        public List<Dto.NRuleset> GetRuleId(long RuleId)
        {
            return DtoMapList(DomainModel.NRuleset.GetRulesetByRuleId(RuleId));
        }

        [HttpGet]
        [Route("GetRuleSetByCodeAndRuleId")]
        public Dto.NRuleset GetRuleSetByCodeAndRuleId(long RuleId, string rulesetCode)
        {
            return DtoMap(DomainModel.NRuleset.GetRuleSetByCodeAndRuleId(RuleId, rulesetCode));
           
        }
        

        [HttpGet]
        [Route("DeleteRuleSets")]
        public int DeleteRuleSets(long ruleId)
        {
            DomainModel.NRuleset.DeleteRuleSets(ruleId);
            return 1;
        }

        [HttpGet]
        [Route("GetRulesByRuleID")]
        public Dictionary<string, string> GetRulesByRuleID(long ruleId)
        {
              return DomainModel.NRuleset.GetRulesByRuleID(ruleId);
            
        }

        [HttpGet]
        [Route("GetRuleset")]
        public Dto.NRuleset GetRuleset(long RuleId, long TypeId, string Code)
        {
            return DtoMap(DomainModel.NRuleset.GetRuleset(Code, RuleId, TypeId));
        }


        [HttpGet]
        [Route("GetMaxRuleId")]
        public long GetMaxRuleId()
        {
            return DomainModel.NRuleset.GetMaxRuleId();
        }


    }
}
