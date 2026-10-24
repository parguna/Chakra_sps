using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AttestationEndpoint")]
    public class AttestationEndpointController : BaseController<DomainModel.AttestationEndpoint, EntityModel.AttestationEndpoint, Dto.AttestationEndpoint>
    {
        [HttpGet]
        [Route("GetAttestationEndpoints")]
        public List<string> GetAttestationEndpoints()
        {
            return DomainModel.AttestationEndpoint.GetAttestationEndpoints();
        }

        [HttpGet]
        [Route("GetAttestationEndpointsForActiveCampaign")]
        public List<string> GetAttestationEndpointsForActiveCampaign()
        {
            return DomainModel.AttestationEndpoint.GetAttestationEndpointsForActiveCampaign();
        }

        [HttpGet]
        [Route("GetAttestationByEndpointName")]
        public List<string> GetAttestationByEndpointName(string endPointTypeName)
        {
            return DomainModel.AttestationEndpoint.GetAttestationByEndpointName(endPointTypeName);
        }
    }
}
