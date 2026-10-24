using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("DefaultRoleQuicklink")]
    public class DefaultRoleQuicklinkController : BaseController<DomainModel.DefaultRoleQuickLink, EntityModel.DefaultRoleQuickLink, Dto.DefaultRoleQuickLink>
    {
        [HttpGet]
        [Route("DoesExists")]
        public  bool DoesExists(long roleId, long quickLinkId)
        {
            return DomainModel.DefaultRoleQuickLink.DoesExists(roleId, quickLinkId);
        }


        [HttpGet]
        [Route("DoesExistsByPosition")]
        public  bool DoesExistsByPosition(long dbPosition, long roleID)
        {
            return DomainModel.DefaultRoleQuickLink.DoesExistsByPosition(dbPosition, roleID);
        }

        [HttpGet]
        [Route("GetQLCatagoriesByRoleID")]
        public  IList<string> GetQLCatagoriesByRoleID(long roleId)
        {
            return DomainModel.DefaultRoleQuickLink.GetQLCatagoriesByRoleID(roleId);

        }

    }
}
