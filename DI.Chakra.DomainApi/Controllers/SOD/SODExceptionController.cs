using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODException")]
    public class SODExceptionController : BaseController<DomainModel.SODException, EntityModel.SODException, Dto.SODException>
    {
        [HttpGet]
        [Route("GetByIdIncludeRiskOwner")]
        public SODException GetByIdIncludeRiskOwner(long id)
        {
            return DtoMap(DomainModel.SODException.GetByIdIncludeRiskOwner(id));
        }

        [HttpGet]
        [Route("GetByExceptionCode")]
        public SODException GetByExceptionCode(string exceptionCode)
        {
            return DtoMap(DomainModel.SODException.GetByExceptionCode(exceptionCode));
        }

        [HttpGet]
        [Route("HasFunction")]
        public bool HasFunction(long functionId)
        {
            return DomainModel.SODException.HasFunction(functionId);
        }

        [HttpGet]
        [Route("GetBySearchString")]
        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return DomainModel.SODException.GetBySearchString(searchString);
        }

        [HttpGet]
        [Route("GetAllIncludingRiskOwner")]
        public  List<SODException> GetAllIncludingRiskOwner()
        {
            return DtoMapList(DomainModel.SODException.GetAllIncludingRiskOwner());
        }
    }
}
