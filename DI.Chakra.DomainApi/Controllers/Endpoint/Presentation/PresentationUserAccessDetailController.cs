using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationUserAccessDetail")]
    public class PresentationUserAccessDetailController : ApiController
    {
        [HttpGet]
        [Route("GetUserAccessDetail")]
        public IList<Dto.Presentation.PresentationUserAccessDetail> GetUserAccessDetail(long userId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserAccessDetail.GetUserAccessDetail(userId));
        }
        public  List<Dto.Presentation.PresentationUserAccessDetail> DtoMapList(IList<DomainModel.Presentation.PresentationUserAccessDetail> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        public  Dto.Presentation.PresentationUserAccessDetail DtoMap(DomainModel.Presentation.PresentationUserAccessDetail tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationUserAccessDetail, Dto.Presentation.PresentationUserAccessDetail>(tModel);
        }
    }
}
