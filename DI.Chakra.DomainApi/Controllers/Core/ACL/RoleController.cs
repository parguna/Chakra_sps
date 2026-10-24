using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Role")]
    public class RoleController : BaseController<DomainModel.Role, EntityModel.Role, Dto.Role>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string roleName,long roleId)
        {
            return DomainModel.Role.DoesExists(roleName,roleId);
        }

        [Route("GetAllRolesByEndpointID")]
        [HttpGet]
        public IList<Dto.Role> GetAllRolesByEndpointID(long endpointSystemId, long EndPointID)
        {
            return DtoMapList(DomainModel.Role.GetAllRolesByEndpointID(endpointSystemId, EndPointID));
        }

        [HttpGet]
        [Route("GetByIdIncludingUser")]
        public Dto.Role GetByIdIncludingUser(long id)
        {
            return DtoMap(DomainModel.Role.GetByIdIncludingUser(id));
        }

        [HttpGet]
        [Route("DeleteRole")]
        public bool DeleteRole(long roleId, string currentUser)
        {
            return DomainModel.Role.DeleteRole(roleId, currentUser);
        }
    }
}
