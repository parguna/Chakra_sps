using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers.Core.ACL.Presentation
{
    [RoutePrefix("PresentationTabMenu")]
    public class PresentationTabMenuController : TabMenuController
    {
        [HttpGet]
        [Route("GetAll")]
        public List<PresentationTabMenu> GetAll()
        {
            return DtoMapList(DomainModel.Presentation.PresentationTabMenu.GetAll());
        }

        public  PresentationTabMenu DtoMap(DomainModel.Presentation.PresentationTabMenu tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationTabMenu, PresentationTabMenu>(tModel);
        }

        public  List<PresentationTabMenu> DtoMapList(IList<DomainModel.Presentation.PresentationTabMenu> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
