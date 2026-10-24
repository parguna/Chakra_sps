using DI.Chakra.DomainApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi 
{
    [RoutePrefix("SSIDSRolelnfo")]
    public class SSIDSRolelnfoController : BaseController<DomainModel.SSIDSRolelnfo, EntityModel.SSIDSRolelnfo, Dto.SSIDSRolelnfo>
    {
        [HttpGet]
        [Route("GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            return DomainModel.SSIDSRolelnfo.GetAllRoles();
        }
    }
}