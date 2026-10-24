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
    [RoutePrefix("PresentationSAPEPRoleToUsersReport")]

    public class PresentationSAPEPRoleToUsersReportController
    {
        public  PresentationSAPEPRoleToUsersReport DtoMap(DomainModel.Presentation.PresentationSAPEPRoleToUsersReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPEPRoleToUsersReport, PresentationSAPEPRoleToUsersReport>(tModel);
        }

        public  List<PresentationSAPEPRoleToUsersReport> DtoMapList(IList<DomainModel.Presentation.PresentationSAPEPRoleToUsersReport> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetByEndpointSystemId")]
        public IList<PresentationSAPEPRoleToUsersReport> GetByEndpointSystemId(long endpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPEPRoleToUsersReport.GetByEndpointSystemId(endpointSystemId));
        }
    }
}
