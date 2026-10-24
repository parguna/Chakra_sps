using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODRiskCode")]
    public class SODRiskCodeController : BaseController<DomainModel.SODRiskCode, EntityModel.SODRiskCode, Dto.SODRiskCode>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string riskCode)
        {
            return DomainModel.SODRiskCode.DoesExists(riskCode);
        }

        [HttpGet]
        [Route("GetBySearchString")]
        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return DomainModel.SODRiskCode.GetBySearchString(searchString);
        }

        [HttpGet]
        [Route("GetAllByIncludeUser")]
        public List<SODRiskCode> GetAllByIncludeUser()
        {
            return DtoMapList(DomainModel.SODRiskCode.GetAllByIncludeUser());
        }

        [HttpGet]
        [Route("HasRiskLevel")]
        public bool HasRiskLevel(long riskLevelId)
        {
            return DomainModel.SODRiskCode.HasRiskLevel(riskLevelId);
        }

        [HttpGet]
        [Route("HasRiskScore")]
        public bool HasRiskScore(long riskScoreId)
        {
            return DomainModel.SODRiskCode.HasRiskScore(riskScoreId);
        }

        [HttpPost]
        [Route("IsExists")]
        public bool IsExists([FromBody]SODRiskCode risk)
        {
            return DomainModel.SODRiskCode.IsExists(AutoMapperHelper.DtoUnMap<Dto.SODRiskCode, DomainModel.SODRiskCode>(risk));
        }
    }
}
