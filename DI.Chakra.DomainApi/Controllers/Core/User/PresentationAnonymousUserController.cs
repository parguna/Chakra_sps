using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Infrastructure.Enums;
using AutoMapper;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationAnonymousUser")]
    public class PresentationAnonymousUserController : ApiController
    {
        [HttpGet]
        [Route("GetUserByUserName")]
        [AllowAnonymous]
        public PresentationAnonymousUser GetUserByUserName(string name)
        {
            return DtoMap(DomainModel.Presentation.PresentationAnonymousUser.GetUserByUserName(name));
        }

        public DI.Chakra.Dto.Presentation.PresentationAnonymousUser DtoMap(DomainModel.Presentation.PresentationAnonymousUser tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationAnonymousUser, DI.Chakra.Dto.Presentation.PresentationAnonymousUser>(tModel);
        }

        public List<DI.Chakra.Dto.Presentation.PresentationAnonymousUser> DtoMapList(IList<DomainModel.Presentation.PresentationAnonymousUser> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
