using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODRiskLevel")]
    public class SODRiskLevelController : BaseController<DomainModel.SODRiskLevel, EntityModel.SODRiskLevel, Dto.SODRiskLevel>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string riskLevelCode)
        {
            return DomainModel.SODRiskLevel.DoesExists(riskLevelCode);
        }

        [HttpGet]
        [Route("GetBySearchString")]
        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return DomainModel.SODRiskLevel.GetBySearchString(searchString);
        }
    }
}
