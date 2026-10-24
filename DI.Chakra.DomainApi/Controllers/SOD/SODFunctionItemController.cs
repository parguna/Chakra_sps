using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODFunctionItem")]
    public class SODFunctionItemController : BaseController<DomainModel.SODFunctionItem, EntityModel.SODFunctionItem, Dto.SODFunctionItem>
    {
        [HttpGet]
        [Route("GetBySearchString")]
        public Dictionary<long, string> GetByUrl(string inputParams, string searchString)
        {
            return DomainModel.SODFunctionItem.GetBySearchString(inputParams,searchString);
        }

        [HttpGet]
        [Route("GetByIdWithNavigation")]
        public SODFunctionItem GetByIdWithNavigation(long id)
        {
            return DtoMap(DomainModel.SODFunctionItem.GetByIdWithNavigation(id));
        }
    }
}
