using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserImage")]
    public class UserImageController : BaseController<DomainModel.UserImage, EntityModel.UserImage, Dto.UserImage>
    {
        [HttpGet]
        [Route("GetImageByUserId")]
        public UserImage GetImageByUserId(long userId) 
        {
            return DtoMap(DomainModel.UserImage.GetImageByUserId(userId));
        }
    }
}
