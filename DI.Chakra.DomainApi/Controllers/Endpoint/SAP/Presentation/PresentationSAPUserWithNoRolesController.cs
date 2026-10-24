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
    [RoutePrefix("PresentationSAPUserWithNoRoles")]

    public class PresentationSAPUserWithNoRolesController : ApiController
    {
        public PresentationSAPUserWithNoRoles DtoMap(DomainModel.Presentation.PresentationSAPUserWithNoRoles tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPUserWithNoRoles, PresentationSAPUserWithNoRoles>(tModel);
        }

        public  List<PresentationSAPUserWithNoRoles> DtoMapList(IList<DomainModel.Presentation.PresentationSAPUserWithNoRoles> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPUsersWithNoRolesReportData")]
        public IList<PresentationSAPUserWithNoRoles> GetSAPUsersWithNoRolesReportData(long EndpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPUserWithNoRoles.GetSAPUserWithNoGroupsReportData(EndpointSystemId));
        }
    }
}
