using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserQuickLink")]
    public class UserQuickLinkController : BaseController<DomainModel.UserQuickLink, EntityModel.UserQuickLink, Dto.UserQuickLink>
    {
        [HttpGet]
        [Route("Delete")]
        public void Delete(long quickLinkId, long userId)
        {
            DomainModel.UserQuickLink.Delete(quickLinkId, userId);
        }

        [HttpGet]
        [Route("GetByUserId")]
        public List<UserQuickLink> GetByUserId(long userId)
        {
            return DtoMapList(DomainModel.UserQuickLink.GetByUserId(userId));
        }
    }
}
