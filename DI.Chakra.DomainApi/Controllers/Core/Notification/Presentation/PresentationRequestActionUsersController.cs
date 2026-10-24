using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationRequestActionUsers")]
    public class PresentationRequestActionUsersController : ApiController
    {
        [HttpGet]
        [Route("GetActionUsersByRequestId")]
        public List<PresentationRequestActionUsers> GetActionUsersByRequestId(long requestId, string source)
        {
            return DtoMapList(DomainModel.Presentation.PresentationRequestActionUsers.GetActionUsersByRequestId(requestId,source));
        }

        public PresentationRequestActionUsers DtoMap(DomainModel.Presentation.PresentationRequestActionUsers tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationRequestActionUsers, PresentationRequestActionUsers>(tModel);
        }

        public List<PresentationRequestActionUsers> DtoMapList(IList<DomainModel.Presentation.PresentationRequestActionUsers> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
