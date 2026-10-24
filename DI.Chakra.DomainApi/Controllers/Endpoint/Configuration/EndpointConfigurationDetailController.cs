using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EndpointConfigurationDetail")]
    public class EndpointConfigurationDetailController : BaseController<DomainModel.EndpointConfigurationDetail, EntityModel.EndpointConfigurationDetail, Dto.EndpointConfigurationDetail>
    {
        [HttpGet]
        [Route("GetKeyValue")]
        public string GetKeyValue(long endpointId,long keyId)
        {
            return DomainModel.EndpointConfigurationDetail.GetKeyValue(endpointId, keyId);
        }
    }
}