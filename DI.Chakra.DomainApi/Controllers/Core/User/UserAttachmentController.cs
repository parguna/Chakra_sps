using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserAttachment")]
    public class UserAttachmentController : BaseController<DomainModel.UserAttachment, EntityModel.UserAttachment, Dto.UserAttachment>
    {
        [HttpGet]
        [Route("GetByUserId")]
        [AllowAnonymous]
        public List<UserAttachment> GetByUserId(long userId)
        {
            return DtoMapList(DomainModel.UserAttachment.GetByUserId(userId));
        }
    }
}
