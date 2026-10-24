using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("QuickLinkAccessRights")]
    public class QuickLinkAccessRightsController : BaseController<DomainModel.QuickLinkAccessRights, EntityModel.QuickLinkAccessRights, Dto.QuickLinkAccessRights>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(long roleId, int quickLinkId)
        {
            return DomainModel.QuickLinkAccessRights.DoesExists(roleId, quickLinkId);
        }
     

       
    }
}


