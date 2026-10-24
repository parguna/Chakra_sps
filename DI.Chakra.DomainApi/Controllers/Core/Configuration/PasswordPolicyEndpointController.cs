using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PasswordPolicyEndpoint")]
    public class PasswordPolicyEndpointController : BaseController<DomainModel.PasswordPolicyEndpoint, EntityModel.PasswordPolicyEndpoint, PasswordPolicyEndpoint>
    {
        [HttpGet]
        [Route("GetByPolicyId")]
        public List<PasswordPolicyEndpoint> GetByPolicyId(long policyId)
        {
            return DtoMapList(DomainModel.PasswordPolicyEndpoint.GetByPolicyId(policyId));
        }


        [HttpGet]
        [Route("GetEndpointPasswordPolicy")]
        public PasswordPolicyEndpoint GetEndpointPasswordPolicy(long endpointSystemId)
        {
            return DtoMap(DomainModel.PasswordPolicyEndpoint.GetEndpointPasswordPolicy(endpointSystemId));
        }
    }
}
