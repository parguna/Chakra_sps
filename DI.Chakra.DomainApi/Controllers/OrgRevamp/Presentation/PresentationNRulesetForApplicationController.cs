using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationNRulesetForApplication")]
    public class PresentationNRulesetForApplicationController : ApiController
    {
        [HttpGet]
        [Route("GetByRuleset")]
        public List<PresentationNRulesetForApplication> GetByRuleEndpointandRuleset(long rulesetId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationNRulesetForApplication.GetByRuleset(rulesetId));
        }

        public PresentationNRulesetForApplication DtoMap(DomainModel.Presentation.PresentationNRulesetForApplication tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationNRulesetForApplication, PresentationNRulesetForApplication>(tModel);
        }

        public List<PresentationNRulesetForApplication> DtoMapList(IList<DomainModel.Presentation.PresentationNRulesetForApplication> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
