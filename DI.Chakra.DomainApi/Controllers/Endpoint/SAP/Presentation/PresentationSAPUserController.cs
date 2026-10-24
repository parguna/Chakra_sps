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
    [RoutePrefix("PresentationSAPUser")]

    public class PresentationSAPUserController : ApiController
    {
        public  PresentationSAPUser DtoMap(DomainModel.Presentation.PresentationSAPUser tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPUser, PresentationSAPUser>(tModel);
        }

        public  List<PresentationSAPUser> DtoMapList(IList<DomainModel.Presentation.PresentationSAPUser> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetAllSAPUsersReportData")]
        public IList<PresentationSAPUser> GetAllSAPUsersReportData(long EndpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPUser.GetAllSAPUsersReportData(EndpointSystemId));
        }
    }
}
