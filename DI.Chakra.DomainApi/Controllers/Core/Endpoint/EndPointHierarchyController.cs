using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EndpointHierarchy")]
    public class EndPointHierarchyController : BaseController<DomainModel.EndpointHierarchy, EntityModel.EndpointHierarchy, Dto.EndpointHierarchy>
    {
        [HttpGet]
        [Route("GetEndpointHierarchyByEndpoint")]
        public List<EndpointHierarchy> GetEndpointHierarchyByEndpoint(long EndpointSystemId, AttestationType attestationType)
        {
            return DtoMapList(DomainModel.EndpointHierarchy.GetEndpointHierarchy(EndpointSystemId, attestationType));
        }
    }
}