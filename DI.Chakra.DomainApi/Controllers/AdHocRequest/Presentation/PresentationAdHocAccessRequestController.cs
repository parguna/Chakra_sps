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
    [RoutePrefix("PresentationAdHocAccessRequest")]
    public class PresentationAdHocAccessRequestController : AdHocAccessRequestController
    {
        [HttpGet]
        [Route("GetRequestsByActionUserId")]
        public List<PresentationAdHocAccessRequest> GetRequestsByActionUserId(long actionUserId) 
        {
            return DtoMapList(DomainModel.Presentation.PresentationAdHocAccessRequest.GetRequestsByActionUserId(actionUserId));
        }

        [HttpGet]
        [Route("GetFailedRequestDetails")]
        public List<PresentationAdHocAccessRequest> GetFailedRequestDetails()
        {
            return DtoMapList(DomainModel.Presentation.PresentationAdHocAccessRequest.GetFailedRequestDetails());
        }

        public  PresentationAdHocAccessRequest DtoMap(DomainModel.Presentation.PresentationAdHocAccessRequest tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationAdHocAccessRequest, PresentationAdHocAccessRequest>(tModel);
        }

        public  List<PresentationAdHocAccessRequest> DtoMapList(IList<DomainModel.Presentation.PresentationAdHocAccessRequest> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
