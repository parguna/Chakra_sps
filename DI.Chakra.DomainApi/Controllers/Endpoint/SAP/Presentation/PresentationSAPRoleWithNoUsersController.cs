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
    [RoutePrefix("PresentationSAPRoleWithNoUsers")]

    public class PresentationSAPRoleWithNoUsersController : ApiController
    {
        public PresentationSAPRoleWithNoUsers DtoMap(DomainModel.Presentation.PresentationSAPRoleWithNoUsers tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPRoleWithNoUsers, PresentationSAPRoleWithNoUsers>(tModel);
        }

        public  List<PresentationSAPRoleWithNoUsers> DtoMapList(IList<DomainModel.Presentation.PresentationSAPRoleWithNoUsers> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPRolesWithNoUsersReportData")]
        public IList<PresentationSAPRoleWithNoUsers> GetSAPRoleWithNoUsersReportData(long endpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPRoleWithNoUsers.GetSAPRolesWithNoUsers(endpointSystemId));
        }
    }
}
