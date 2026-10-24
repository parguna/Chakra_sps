using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EndpointUserViewConfiguration")]
    public class EndpointUserViewConfigurationController : BaseController<DomainModel.EndpointUserViewConfiguration, EntityModel.EndpointUserViewConfiguration, Dto.EndpointUserViewConfiguration>
    {
        [HttpGet]
        [Route("GetByEndpointSystemId")]
        public List<EndpointUserViewConfiguration> GetByEndpointSystemId(long endpointSystemId)
        {
            return DtoMapList(DomainModel.EndpointUserViewConfiguration.GetByEndpointSystemId(endpointSystemId));
        }
    }
}