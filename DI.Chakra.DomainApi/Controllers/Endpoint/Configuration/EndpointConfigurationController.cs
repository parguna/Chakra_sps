using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EndpointConfiguration")]
    public class EndpointConfigurationController : BaseController<DomainModel.EndpointConfiguration, EntityModel.EndpointConfiguration, Dto.EndpointConfiguration>
    {
        [HttpGet]
        [Route("GetByEndpointSystemId")]
        public EndpointConfiguration GetByEndpointSystemId(long endpointSystemId)
        {
            return DtoMap(DomainModel.EndpointConfiguration.GetByEndpointSystemId(endpointSystemId));
        }

        [HttpGet]
        [Route("GetByEndpointSystemTypeId")]
        public EndpointConfiguration GetByEndpointSystemTypeId(long endpointSystemTypeId)
        {
            return DtoMap(DomainModel.EndpointConfiguration.GetByEndpointSystemTypeId(endpointSystemTypeId));
        }
    }
}