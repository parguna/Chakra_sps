using AutoMapper;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationAccessRequestItem")]
    public class PresentationAccessRequestItemController : ApiController
    {
        [HttpPost]
        [Route("GetByCriteria")]
        public List<Dto.Presentation.PresentationAccessRequestItem> GetByCriteria([FromBody]PresentationAccessRequestFilterCriteria criteria)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAccessRequestItem.GetByCriteria(AutoMapperHelper.DtoUnMap<Dto.Presentation.PresentationAccessRequestFilterCriteria, DomainModel.Presentation.PresentationAccessRequestFilterCriteria>(criteria)));
        }

        public  List<Dto.Presentation.PresentationAccessRequestItem> DtoMapList(List<DomainModel.Presentation.PresentationAccessRequestItem> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        public  Dto.Presentation.PresentationAccessRequestItem DtoMap(DomainModel.Presentation.PresentationAccessRequestItem tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationAccessRequestItem,Dto.Presentation.PresentationAccessRequestItem>(tModel);
        }

        [HttpGet]
        [Route("GetByAccountReconcilationId")]
        public List<Dto.Presentation.PresentationAccessRequestItem> GetByGetByAccountReconcilationId(long accountReconcilationId, long userId, long epId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationAccessRequestItem.GetAllEntitlementForRemoveByReconcilationId(accountReconcilationId, userId, epId));
        }
    }
}
