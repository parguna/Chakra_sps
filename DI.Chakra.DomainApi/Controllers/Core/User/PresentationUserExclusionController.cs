using AutoMapper;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationUserExclusion")]
    public class PresentationUserExclusionController : ApiController
    {
        [HttpGet]
        [Route("GetAllUserExclusion")]
        public List<DI.Chakra.Dto.Presentation.PresentationUserExclusion> GetAllUserExclusion()
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserExclusion.GetAllUserExclusion());
        }

        public DI.Chakra.Dto.Presentation.PresentationUserExclusion DtoMap(DomainModel.Presentation.PresentationUserExclusion tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationUserExclusion, DI.Chakra.Dto.Presentation.PresentationUserExclusion>(tModel);
        }

        public List<DI.Chakra.Dto.Presentation.PresentationUserExclusion> DtoMapList(IList<DomainModel.Presentation.PresentationUserExclusion> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}