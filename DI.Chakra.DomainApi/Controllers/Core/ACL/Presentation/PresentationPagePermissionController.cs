using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;
using AutoMapper;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationPagePermission")]
    public class PresentationPagePermissionController : PagePermissionController
    {
        public PresentationPagePermission DtoMap(DomainModel.Presentation.PresentationPagePermission tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationPagePermission, PresentationPagePermission>(tModel);
        }

        public List<PresentationPagePermission> DtoMapList(IList<DomainModel.Presentation.PresentationPagePermission> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetByPageId")]
        public List<PresentationPagePermission> GetByPageId(long pageId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationPagePermission.GetByPageId(pageId));
        }
    }
}