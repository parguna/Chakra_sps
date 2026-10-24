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
    [RoutePrefix("PresentationSAPRoleToUsers")]

    public class PresentationSAPRoleToUsersReportController : ApiController
    {
        public PresentationSAPRoleToUsers DtoMap(DomainModel.Presentation.PresentationSAPRoleToUsers tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPRoleToUsers, PresentationSAPRoleToUsers>(tModel);
        }

        public  List<PresentationSAPRoleToUsers> DtoMapList(IList<DomainModel.Presentation.PresentationSAPRoleToUsers> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPRoleToUsersReportData")]
        public IList<PresentationSAPRoleToUsers> GetSAPRoleToUsersReportData(long EndpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPRoleToUsers.GetSAPRoleToUsersReportData(EndpointSystemId));
        }
    }
}
