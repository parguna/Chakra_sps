using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("RequestTypes")]
    public class RequestTypesController  : BaseController<DomainModel.RequestTypes, EntityModel.RequestTypes, RequestTypes>
    {
        [HttpGet]
        [Route("GetByIsActive")]
        public  List<RequestTypes> GetByIsActive()
        {
            return DtoMapList(DomainModel.RequestTypes.GetByIsActive());
        }
    }
}
