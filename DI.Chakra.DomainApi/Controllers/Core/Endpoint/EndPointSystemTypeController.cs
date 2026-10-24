using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EndPointSystemType")]
    public class EndPointSystemTypeController : BaseController<DomainModel.EndPointSystemType, EntityModel.EndPointSystemType,Dto.EndPointSystemType>
    {
        [HttpGet]
        [Route("GetByIdIncludingEndpoints")]
        public Dto.EndPointSystemType GetByIdIncludingEndpoints(long id)
        {
            return DtoMap(DomainModel.EndPointSystemType.GetByIdIncludingEndpoints(id));
        }

        [HttpGet]
        [Route("GetAllforAttestation")]
        public List<Dto.EndPointSystemType> GetAllforAttestation()
        {
            return DtoMapList(DomainModel.EndPointSystemType.GetAllforAttestation());
        }

        [HttpGet]
        [Route("GetByEndpointSystemTypeName")]
        public Dto.EndPointSystemType GetByEndpointSystemTypeName(EndpointType endpointSystemTypeName)
        {
            return DtoMap(DomainModel.EndPointSystemType.GetByEndpointSystemTypeName(endpointSystemTypeName));
        }

        [HttpGet]
        [Route("GetEndPointTypeName")]
        public EndpointType GetEndPointTypeName(long id)
        {
            return DomainModel.EndPointSystemType.GetEndPointTypeName(id);
        }

        [HttpGet]
        [Route("GetByName")]
        public Dto.EndPointSystemType GetByName(EndpointType name)
        {
            return DtoMap(DomainModel.EndPointSystemType.GetByEndpointSystemTypeName(name));
        }

        [HttpGet]
        [Route("GetByIsActiveForProvisioning")]
        public List<Dto.EndPointSystemType> GetByIsActiveForProvisioning()
        {
            return DtoMapList(DomainModel.EndPointSystemType.GetByIsActiveForProvisioning());
        }

        [HttpGet]
        [Route("GetEndpointSystemType")]
        public Dictionary<long,string> GetEndpointSystemType()
        {
            return DomainModel.EndPointSystemType.GetEndpointSystemType();
        }
    }
}
