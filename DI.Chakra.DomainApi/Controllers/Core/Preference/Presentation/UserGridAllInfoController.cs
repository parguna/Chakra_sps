using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserGridAllInfo")]
    public class UserGridAllInfoController : BaseController<DomainModel.Presentation.UserGridAllInfo, EntityModel.Presentation.UserGridAllInfo, UserGridAllInfo>
    {
        [HttpGet]
        [Route("GetByUserId")]
        public Dto.Presentation.UserGridAllInfo GetByUserId(long userId, string gridId, string pagePath)
        {
            return DtoMap(DomainModel.Presentation.UserGridAllInfo.GetByUserId(userId, gridId, pagePath));
        }
    }
}