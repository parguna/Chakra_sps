using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ConfigGeneralTrustedSource")]
    public class ConfigGeneralTrustedSourceController : BaseController<DomainModel.ConfigGeneralTrustedSource, EntityModel.ConfigGeneralTrustedSource, Dto.ConfigGeneralTrustedSource>
    {
        [HttpGet]
        [Route("IsSelfUpdated")]
        public bool IsSelfUpdated(string attributeName)
        {
            return DomainModel.ConfigGeneralTrustedSource.IsSelfUpdated(attributeName);
        }

        [HttpPost]
        [Route("AddConfigTrustedSource")]
        public void AddConfigTrustedSource(List<DomainModel.ConfigGeneralTrustedSource> configGeneralTrustedSource) 
        {
            DomainModel.ConfigGeneralTrustedSource.AddConfigTrustedSource(configGeneralTrustedSource,User.Identity.Name);
        }

        [HttpDelete]
        [Route("DeleteConfigTrustedSource")]
        public void DeleteConfigTrustedSource()
        {
            DomainModel.ConfigGeneralTrustedSource.DeleteConfigTrustedSource();
        }

        [HttpGet]
        [Route("DoesAlreadyExists")]
        public  bool DoesAlreadyExists(string attributename, EndpointType endpointType)
        {
            return DomainModel.ConfigGeneralTrustedSource.DoesAlreadyExists(attributename, endpointType);
        
        }

        
    }
}
