using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserPreference")]
    public class UserPreferenceController : BaseController<DomainModel.UserPreference, EntityModel.UserPreference, Dto.UserPreference>
    {
        [HttpGet]
        [Route("GetDefaultMenus")]
        public IList<UserPreference> GetDefaultMenus(long roleId)
        {
           return DtoMapList(DomainModel.UserPreference.GetDefaultMenus(roleId));
        }


        [HttpGet]
        [Route("GetUserPreferenceMenus")]
        public IList<UserPreference> GetUserPreferenceMenus(long userId)
        {
            return DtoMapList(DomainModel.UserPreference.GetUserPreferenceMenus(userId));
        }
    }
}
