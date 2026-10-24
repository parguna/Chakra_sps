using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AccessRights")]
    public class AccessRightsController : BaseController<DomainModel.AccessRights, EntityModel.AccessRights, Dto.AccessRights>
    {
        [HttpPost]
        [Route("CloneRole")]
        public long CloneRole([FromBody]Role role, [FromUri]bool cloneMenu, [FromUri]bool cloneQL, [FromUri]bool cloneDB, [FromUri]long cloneRoleID)
        {
            return DomainModel.AccessRights.CloneRole(AutoMapperHelper.DtoUnMap<Dto.Role, DomainModel.Role> (role),cloneMenu,cloneQL,cloneDB,cloneRoleID,User.Identity.Name);
        }

        [HttpGet]
        [Route("GetAccessRightsByRoleId")]
        public List<AccessRights> GetAccessRightsByRoleId(long roleID)
        {
            return DtoMapList(DomainModel.AccessRights.GetAccessRightsByRoleId(roleID));
        }

        [HttpPost]
        [Route("DeleteAccessRights")]

        public bool DeleteAccessRights([FromBody]AccessRights accessRight)
        {
            return DomainModel.AccessRights.DeleteAccessRights(AutoMapperHelper.DtoUnMap<Dto.AccessRights,DomainModel.AccessRights>(accessRight));
        }

        [HttpGet]
        [Route("GetByPagePermission")]
        public List<AccessRights> GetByPagePermission(long pagePermissionId)
        {
            return DtoMapList(DomainModel.AccessRights.GetByPagePermission(pagePermissionId));
        }
    }
}
