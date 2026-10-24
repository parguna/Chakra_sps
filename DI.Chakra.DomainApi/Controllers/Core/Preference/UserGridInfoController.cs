using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserGridInfo")]
    public class UserGridInfoController : BaseController<DomainModel.UserGridInfo, EntityModel.UserGridInfo, Dto.UserGridInfo>
    {
        [HttpGet]
        [Route("GetByUserId")]
        public Dto.UserGridInfo GetByUserId(long userId, string gridId, string pagePath)
        {
            return DtoMap(DomainModel.UserGridInfo.GetByUserId(userId, gridId, pagePath));
        }

    }
}