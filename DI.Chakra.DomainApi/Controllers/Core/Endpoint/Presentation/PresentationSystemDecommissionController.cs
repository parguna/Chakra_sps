using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationSystemDecommission")]
    public class PresentationSystemDecommissionController : ApiController
    { 
        [HttpPost]
        [Route("InsertDecommission")]
        public string InsertDecommission(Dto.PresentationSystemDecommission Decommissionvalues)
        {
            return DomainModel.PresentationSystemDecommission.InsertDecommission(Mapper.Map<Dto.PresentationSystemDecommission, DomainModel.PresentationSystemDecommission>(Decommissionvalues));
        }

    }
}
