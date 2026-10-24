using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PagePermission")]
    public class PagePermissionController : BaseController<DomainModel.PagePermission, EntityModel.PagePermission, Dto.PagePermission>
    {
        [HttpGet]
        [Route("DoesExistsByPermissionId")]
        public bool DoesExistsByPermissionId(int permissionId)
        {
            return DomainModel.PagePermission.DoesExistsByPermissionId(permissionId);
        }
        [HttpGet]
        [Route("HasAccess")]
        public bool HasAccess(long pageId, long userId)
        {
            return DomainModel.PagePermission.HasAccess(pageId, userId);
        }


        [HttpGet]
        [Route("GetPagePermissions")]

        public List<PagePermission> GetPagePermissions(long pageId, long userId)
        {
            
            return DtoMapList(DomainModel.PagePermission.GetPagePermissions(pageId, userId));
        }

        [HttpGet]
        [Route("GetPagePermissionsByUser")]
        public List<PagePermission> GetPagePermissionsByUser(long userId)
        {
            return DtoMapList(DomainModel.PagePermission.GetPagePermissionsByUser(userId));
        }

        [HttpGet]
        [Route("GetPagePermissions")]

        public List<PagePermission> GetPagePermissions(long pageId)
        {

            return DtoMapList(DomainModel.PagePermission.GetPagePermissions(pageId));
        }
        [HttpGet]
        [Route("GetByUrlId")]
        public List<PagePermission> GetByUrlId(long urlId)
        {
            var result = DomainModel.PagePermission.GetByUrlId(urlId);
            if (result != null)
                return DtoMapList(result);
            else
                return null;
        }

        [HttpGet]
        [Route("UpdateUrlPermission")]
        public bool UpdateUrlPermission(bool isAddPermission, long urlId, long roleID)
        {
            return DomainModel.PagePermission.UpdateUrlPermission(isAddPermission, urlId, roleID);
        }
    }
}
