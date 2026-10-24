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
    [RoutePrefix("PresentationSAPUsersToRolesReport")]

    public class PresentationSAPUsersToRolesReportController : ApiController
    {
        public  PresentationSAPUsersToRolesReport DtoMap(DomainModel.Presentation.PresentationSAPUsersToRolesReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPUsersToRolesReport, PresentationSAPUsersToRolesReport>(tModel);
        }

        public  List<PresentationSAPUsersToRolesReport> DtoMapList(IList<DomainModel.Presentation.PresentationSAPUsersToRolesReport> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPUsersToRolesReportData")]
        public IList<PresentationSAPUsersToRolesReport> GetSAPUsersToRolesReportData(long EndpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPUsersToRolesReport.GetSAPUserToRolesReportDataIncludeUserId(EndpointSystemId));
        }
    }
}
