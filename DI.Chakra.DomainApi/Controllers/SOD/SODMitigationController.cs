using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODMitigation")]
    public class SODMitigationController : BaseController<DomainModel.SODMitigation, EntityModel.SODMitigation, Dto.SODMitigation>
    {
        [HttpGet]
        [Route("GetByIdIncludeSodRiskCode")]
        public SODMitigation GetByIdIncludeSodRiskCode(long id)
        {
            return DtoMap(DomainModel.SODMitigation.GetByIdIncludeSodRiskCode(id));
        }

        [HttpGet]
        [Route("HasRiskCode")]
        public bool HasRiskCode(long riskCodeId)
        {
            return DomainModel.SODMitigation.HasRiskCode(riskCodeId);
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string mitiCode)
        {
            return DomainModel.SODMitigation.DoesExists(mitiCode);
        }
    }
}
