using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Permission")]
    public class PermissionController : BaseController<DomainModel.Permission, EntityModel.Permission, Dto.Permission>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string name)
        {
            return DomainModel.Permission.DoesExists(name);
        }

        [HttpGet]
        [Route("GetByPageId")]
        public List<Permission> GetByPageId(long pageId)
        {
            return DtoMapList(DomainModel.Permission.GetBypageId(pageId));
        }
    }
}
