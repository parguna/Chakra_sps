using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationDashBoardPreference")]
    public class PresentationDashBoardPreferenceController : BaseController<DomainModel.Presentation.PresentationDashBoardPreference, EntityModel.Presentation.PresentationDashBoardPreference, PresentationDashBoardPreference>
    {
        [HttpGet]
        [Route("GetUserDashboards")]
        public IList<PresentationDashBoardPreference> GetUserDashboards(long userId, long roleId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationDashBoardPreference.GetUserDashboards(userId, roleId));
        }

        [HttpGet]
        [Route("GetDefaultDashboards")]
        public IList<PresentationDashBoardPreference> GetDefaultDashboards(long roleId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationDashBoardPreference.GetDefaultDashboards(roleId));
        }
    }
}
