using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationApprovers")]
    public class PresentationApproverController : AccessRequestController
    {
        [HttpGet]
        [Route("GetByAccessRequestId")]

        public List<Dto.Presentation.PresentationApprovers> GetByAccessRequestId(long accessRequestId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationApprovers.GetByAccessRequestId(accessRequestId));

        }

        public  List<Dto.Presentation.PresentationApprovers> DtoMapList(IList<DomainModel.Presentation.PresentationApprovers> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
        public  Dto.Presentation.PresentationApprovers DtoMap(DomainModel.Presentation.PresentationApprovers tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationApprovers, Dto.Presentation.PresentationApprovers>(tModel);
        }

    }
  
}
