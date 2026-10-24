using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserExclusion")]
    public class UserExclusionController : BaseController<DomainModel.UserExclusion, EntityModel.UserExclusion, Dto.UserExclusion>
    {
        [HttpPost]
        [Route("SaveUserList")]
        [AllowAnonymous]
        public void SaveUserList(List<UserExclusion> UserExtList)
        {
            DomainModel.UserExclusion.SaveUserList(DtoUnMapList(UserExtList), User.Identity.Name);
        }

        [HttpGet]
        [Route("GetAllUserExclusionByUserID")]
        [AllowAnonymous]
        public List<UserExclusion> GetAllUserExclusionByUserID(long userID)
        {
           return DtoMapList(DomainModel.UserExclusion.GetAllUserExclusionByUserId(userID));
        }
    }
}
