using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EndpointObjects")]
    public class EndpointObjectsController : BaseController<DomainModel.EndpointObjects, EntityModel.EndpointObjects, Dto.EndpointObjects>
    {
        [HttpGet]
        [Route("GetByEndpointSystemTypeId")]
        public IList<Dto.EndpointObjects> GetByEndpointSystemTypeId(long? endPointId,long endPointSystemTypeId)
        {
            return DtoMapList(DomainModel.EndpointObjects.GetByEndpointSystemTypeId(endPointId,endPointSystemTypeId));
        }
        [HttpGet]
        [Route("GetTrustedSourceObjectsByEndpointSystemType")]
        public IList<Dto.EndpointObjects> GetTrustedSourceObjectsByEndpointSystemType(long endPointSystemTypeId)
        {
            return DtoMapList(DomainModel.EndpointObjects.GetTrustedSourceObjectsByEndpointSystemType(endPointSystemTypeId));
        }
    }
}
