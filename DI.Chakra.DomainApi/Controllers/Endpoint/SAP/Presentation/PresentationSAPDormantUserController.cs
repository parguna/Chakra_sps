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
    [RoutePrefix("PresentationSAPDormantUser")]

    public class PresentationSAPDormantUserController : ApiController
    {
        public  PresentationSAPDormantUser DtoMap(DomainModel.Presentation.PresentationSAPDormantUser tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPDormantUser, PresentationSAPDormantUser>(tModel);
        }

        public  List<PresentationSAPDormantUser> DtoMapList(IList<DomainModel.Presentation.PresentationSAPDormantUser> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPDormantUsersReportData")]
        public IList<PresentationSAPDormantUser> GetSAPDormantUsersReportData(long EndpointSystemId, int dormantDays)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPDormantUser.GetSAPDormantUsersReportData(EndpointSystemId, dormantDays));
        }
    }
}
