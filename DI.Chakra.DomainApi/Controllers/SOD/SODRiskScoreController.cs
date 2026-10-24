using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODRiskScore")]
    public class SODRiskScoreController : BaseController<DomainModel.SODRiskScore, EntityModel.SODRiskScore, Dto.SODRiskScore>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(int riskScore)
        {
            return DomainModel.SODRiskScore.DoesExists(riskScore);
        }

        [HttpGet]
        [Route("GetBySearchString")]
        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return DomainModel.SODRiskScore.GetBySearchString(searchString);
        }
    }
}
