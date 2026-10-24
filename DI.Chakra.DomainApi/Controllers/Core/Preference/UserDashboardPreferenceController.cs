using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserDashboardPreference")]
    public class UserDashboardPreferenceController : BaseController<DomainModel.UserDashboardPreference, EntityModel.UserDashboardPreference, Dto.UserDashboardPreference>
    {
        [HttpGet]
        [Route("Delete")]
        public void Delete(long dashBoardId, long userId)
        {
            DomainModel.UserDashboardPreference.Delete(dashBoardId, userId);
        }
    }
}
