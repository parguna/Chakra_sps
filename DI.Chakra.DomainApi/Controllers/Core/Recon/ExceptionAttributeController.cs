using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ExceptionAttribute")]
    public class ExceptionAttributeController : BaseController<DomainModel.ExceptionAttribute, EntityModel.ExceptionAttribute, ExceptionAttribute>
    {
        [HttpGet]
        [Route("GetByEndpointTypeId")]
        public List<ExceptionAttribute> GetByEndpointTypeId(long endpointTypeId)
        {
            return DtoMapList(DomainModel.ExceptionAttribute.GetByEndpointTypeId(endpointTypeId));
        }

        [HttpPost]
        [Route("SaveExceptionAttribute")]
        public void SaveExceptionAttribute(List<ExceptionAttribute> attributeList)
        {
            DomainModel.ExceptionAttribute.SaveExceptionAttribute(DtoUnMapList(attributeList), User.Identity.Name);
        }
    }
}
