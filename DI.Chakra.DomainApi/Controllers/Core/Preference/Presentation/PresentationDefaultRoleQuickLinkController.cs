using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationDefaultRoleQuickLink")]
    public class PresentationDefaultRoleQuickLinkController : BaseController<DomainModel.Presentation.PresentationDefaultRoleQuickLink, EntityModel.Presentation.PresentationDefaultRoleQuickLink, PresentationDefaultRoleQuickLink>
    {

        [HttpGet]
        [Route("GetDefaultRoleQLRights")]
        public IList<PresentationDefaultRoleQuickLink> GetDefaultRoleQLRights()
        {
            return DtoMapList(DomainModel.Presentation.PresentationDefaultRoleQuickLink.GetDefaultRoleQLRights());
        }
    }
}