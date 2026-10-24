using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NRulesetType")]
    public class NRulesetTypeController : BaseController<DomainModel.NRulesetType, EntityModel.NRulesetType, Dto.NRulesetType>
    {
        [HttpGet]
        [Route("GetNRulesetTypes")]
        public  Dictionary<long, string> GetNRulesetTypes(string searchString)
        {
            return DomainModel.NRulesetType.GetAllRulesetTypes().Where(item => item.RulesetType.Contains(searchString))
                .Select(x => new { Key = x.Id, Value = x.RulesetType }).Take(100)
                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        }
    }
}
