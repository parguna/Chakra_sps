

using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{

    [RoutePrefix("NRulesetForApplication")]
    public class NRulesetForApplicationController : BaseController<DomainModel.NRulesetForApplication, EntityModel.NRulesetForApplication, Dto.NRulesetForApplication>
    {
        [HttpGet]
        [Route("GetByRuleId")]
        public List<NRulesetForApplication> GetByRuleId(long ruleId)
        {
            return (DomainModel.NRulesetForApplication.GetByRuleId(ruleId));
        }

        [HttpPost]
        [Route("SaveApplicationDefault")]
        public void SaveApplicationDefault(long ruleId,List<Dto.NRulesetForApplication> appDefaultList)
        {
            DomainModel.NRulesetForApplication.SaveApplicationDefault(ruleId,DtoUnMapList(appDefaultList), User.Identity.Name);
        }
      
       
    }
}
