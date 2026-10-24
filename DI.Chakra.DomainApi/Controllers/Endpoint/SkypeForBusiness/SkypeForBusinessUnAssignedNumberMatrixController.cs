using AutoMapper;
using DI.Chakra.DomainApi.Controllers;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SkypeForBusinessUnAssignedNumberMatrix")]
    public class SkypeForBusinessUnAssignedNumberMatrixController : BaseController<DomainModel.SkypeForBusinessUnAssignedNumberMatrix, EntityModel.SkypeForBusinessUnAssignedNumberMatrix, Dto.SkypeForBusinessUnAssignedNumberMatrix>
    {
        [HttpGet]
        [Route("GetAllByEndpointId")]
        public List<Dto.SkypeForBusinessUnAssignedNumberMatrix> GetAllByEndpointId(long endpointId, string currentUser)
        {
            var result = DomainModel.SkypeForBusinessUnAssignedNumberMatrix.GetAllByEndpointId(endpointId, currentUser).ToList();
            return DtoMapList(result);
        }
        [HttpGet]
        [Route("GetSFBExtensionMatrixData")]
        public List<Dto.SkypeForBusinessUnAssignedNumberMatrix> GetSFBExtensionMatrixData(long endpointId, string currentUser)
        {
            var result = DomainModel.SkypeForBusinessUnAssignedNumberMatrix.GetSFBExtensionMatrixData(endpointId, currentUser).ToList();
            return DtoMapList(result);
        }
        [HttpGet]
        [Route("GetAllReservedNumbers")]
        public Dictionary<long, string> GetAllReservedNumbers(long endpointId, string search)
        {
            return DomainModel.SkypeForBusinessUnAssignedNumberMatrix.GetAllReservedNumbers(endpointId, search);
        }
    }
}