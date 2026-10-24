using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("RoleTypes")]
    public class RoleTypesController : BaseController<DomainModel.RoleTypes, EntityModel.RoleTypes, RoleTypes>
    {
        [HttpGet]
        [Route("GetByIsActive")]
        public  List<RoleTypes> GetByIsActive()
        {
            return DtoMapList(DomainModel.RoleTypes.GetByIsActive());
        }
    }
}
