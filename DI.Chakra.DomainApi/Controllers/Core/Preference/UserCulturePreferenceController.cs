using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserCulturePreference")]
    public class UserCulturePreferenceController : BaseController<DomainModel.UserCulturePreference, EntityModel.UserCulturePreference, Dto.UserCulturePreference>
    {
        [HttpGet]
        [Route("GetByUserId")]
        public  List<UserCulturePreference> GetByUserId(long userId)
        {
            return DtoMapList(DomainModel.UserCulturePreference.GetByUserId(userId));
        }
    }
}
