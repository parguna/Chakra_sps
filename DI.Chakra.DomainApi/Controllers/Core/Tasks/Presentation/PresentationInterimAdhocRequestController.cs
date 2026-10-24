using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DomainModel.Presentation;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationInterimAdhocRequest")]
    public class PresentationInterimAdhocRequestController : InterimStoreController
    {
        [HttpGet]
        [Route("GetByUser")]
        public IList<Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest> GetByUser(long userId, bool isSaved, InterimStoreItemType itemType)
        {
            return DtoMapList(DomainModel.Presentation.PresentationInterimAdhocRequest.GetByUser(userId, isSaved, itemType));
        }

        public Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest DtoMap(DomainModel.Presentation.PresentationInterimAdhocRequest tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationInterimAdhocRequest, Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest>(tModel);
        }
        public List<Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest> DtoMapList(IList<DomainModel.Presentation.PresentationInterimAdhocRequest> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

    }
}