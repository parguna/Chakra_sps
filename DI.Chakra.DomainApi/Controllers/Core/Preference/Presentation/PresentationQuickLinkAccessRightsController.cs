using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationQuickLinkAccessRights")]
    public class PresentationQuickLinkAccessRightsController : BaseController<DomainModel.Presentation.PresentationQuickLinkAccessRights, EntityModel.Presentation.PresentationQuickLinkAccessRights, PresentationQuickLinkAccessRights>
    {
        [HttpGet]
        [Route("GetQuickLinkAccessRights")]
        public IList<PresentationQuickLinkAccessRights> GetQuickLinkAccessRights()
        {
            return DtoMapList(DomainModel.Presentation.PresentationQuickLinkAccessRights.GetQuickLinkAccessRights());
        }
    }
}