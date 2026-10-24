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
    [RoutePrefix("PresentationSAPLockedUser")]

    public class PresentationSAPLockedUserController : ApiController
    {
        public  PresentationSAPLockedUser DtoMap(DomainModel.Presentation.PresentationSAPLockedUser tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPLockedUser, PresentationSAPLockedUser>(tModel);
        }

        public  List<PresentationSAPLockedUser> DtoMapList(IList<DomainModel.Presentation.PresentationSAPLockedUser> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPLockedUserReportData")]
        public IList<PresentationSAPLockedUser> GetSAPLockedUserReportData(long EndpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPLockedUser.GetSAPLockedUserReportData(EndpointSystemId));
        }
    }
}
