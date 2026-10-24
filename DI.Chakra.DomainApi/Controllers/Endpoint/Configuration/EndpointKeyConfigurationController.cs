using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EndpointKeyConfiguration")]
    public class EndpointKeyConfigurationController : BaseController<DomainModel.EndpointKeyConfiguration, EntityModel.EndpointKeyConfiguration, Dto.EndpointKeyConfiguration>
    {
        [HttpGet]
        [Route("GetByEndpointSystemTypeId")]
        public List<EndpointKeyConfiguration> GetByEndpointSystemTypeId(long endpointSystemTypeId)
        {
            return DtoMapList(DomainModel.EndpointKeyConfiguration.GetByEndpointSystemTypeId(endpointSystemTypeId));
        }
    }
}