using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EndpointConfigurationKey")]
    public class EndpointConfigurationKeyController : BaseController<DomainModel.EndpointConfigurationKey, EntityModel.EndpointConfigurationKey, Dto.EndpointConfigurationKey>
    {
        [HttpGet]
        [Route("GetByKeyName")]
        public EndpointConfigurationKey GetByKeyName(string keyName)
        {
            return DtoMap(DomainModel.EndpointConfigurationKey.GetByKeyName(keyName));
        }
    }
}