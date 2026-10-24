using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODExecutionConfig")]
    public class SODExecutionConfigController : BaseController<DomainModel.SODExecutionConfig, EntityModel.SODExecutionConfig, Dto.SODExecutionConfig>
    {
        [HttpGet]
        [Route("GetByEndpointSystenTypeId")]
        public SODExecutionConfig GetByEndpointSystenTypeId(long endpointSystemTypeId)
        {
            return DtoMap(DomainModel.SODExecutionConfig.GetByEndpointSystenTypeId(endpointSystemTypeId));
        }

        [HttpGet]
        [Route("GetAllWithEndPointSystemType")]
        public List<SODExecutionConfig> GetAllWithEndPointSystemType()
        {
            return DtoMapList(DomainModel.SODExecutionConfig.GetAllWithEndPointSystemType());
        }
    }
}
