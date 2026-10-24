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
    [RoutePrefix("PresentationSAPRoleReport")]

    public class PresentationSAPRoleReportController : ApiController
    {
        public  PresentationSAPRoleReport DtoMap(DomainModel.Presentation.PresentationSAPRoleReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPRoleReport, PresentationSAPRoleReport>(tModel);
        }

        public  List<PresentationSAPRoleReport> DtoMapList(IList<DomainModel.Presentation.PresentationSAPRoleReport> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetAllSAPRolesReportData")]
        public IList<PresentationSAPRoleReport> GetAllSAPRolesReportData(long endpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPRoleReport.GetAllSAPRolesReportData(endpointSystemId));
        }
    }
}
